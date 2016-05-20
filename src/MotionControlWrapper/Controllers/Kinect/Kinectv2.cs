namespace NTNU.MotionControlWrapper.Controllers.Kinect
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Drawing;
    using Microsoft.Kinect;

    public class Kinectv2 : IMotionController
    {
        private const int BytesPerPixelRGBA = 4;
        private const int MapDepthToByte = 8000 / 256;
        private const float InfraredSourceValueMaximum = ushort.MaxValue;
        private const float InfraredSourceScale = 0.75f;
        private const float InfraredOutputValueMinimum = 0.01f;
        private const float InfraredOutputValueMaximum = 1.0f;

        private static readonly uint[] BodyColor =
        {
            0x0000FF00,
            0x00FF0000,
            0xFFFF4000,
            0x40FFFF00,
            0xFF40FF00,
            0xFF808000,
        };

        private KinectSensor _sensor;
        private MultiSourceFrameReader _reader;
        private IList<GestureTracker> _gestureTrackers;
        private Body[] _bodies;

        public Kinectv2()
        {
            _sensor = KinectSensor.GetDefault();

            if (_sensor == null)
            {
                return;
            }

            _sensor.Open();

            _reader = _sensor.OpenMultiSourceFrameReader(
                FrameSourceTypes.Color |
                FrameSourceTypes.Depth |
                FrameSourceTypes.Infrared |
                FrameSourceTypes.Body |
                FrameSourceTypes.BodyIndex);

            if (_sensor == null)
            {
                return;
            }

            MostRecentColorFrame = new byte[CalculateImageByteCount(ColorFrameDescription)];
            MostRecentDepthFrame = new byte[CalculateImageByteCount(DepthFrameDescription)];
            MostRecentInfraredFrame = new byte[CalculateImageByteCount(InfraredFrameDescription)];
            MostRecentSilhouetteFrame = new byte[CalculateImageByteCount(SilhouetteFrameDescription)];
            MostRecentGestures = new GestureResults(_sensor.BodyFrameSource.BodyCount);

            ColorFrameSize = CalculateImageSize(ColorFrameDescription);
            DepthFrameSize = CalculateImageSize(DepthFrameDescription);
            InfraredFrameSize = CalculateImageSize(InfraredFrameDescription);
            SilhouetteFrameSize = CalculateImageSize(SilhouetteFrameDescription);

            _gestureTrackers = new List<GestureTracker>();
        }

        ~Kinectv2()
        {
            Dispose();
        }

        public Size ColorFrameSize { get; private set; }

        public Size DepthFrameSize { get; private set; }

        public Size InfraredFrameSize { get; private set; }

        public Size SilhouetteFrameSize { get; private set; }

        public byte[] MostRecentColorFrame { get; private set; }

        public byte[] MostRecentDepthFrame { get; private set; }

        public byte[] MostRecentInfraredFrame { get; private set; }

        public byte[] MostRecentSilhouetteFrame { get; private set; }

        public GestureResults MostRecentGestures { get; private set; }

        private FrameDescription ColorFrameDescription
        {
            get { return _sensor.ColorFrameSource.FrameDescription; }
        }

        private FrameDescription DepthFrameDescription
        {
            get { return _sensor.DepthFrameSource.FrameDescription; }
        }

        private FrameDescription InfraredFrameDescription
        {
            get { return _sensor.InfraredFrameSource.FrameDescription; }
        }

        private FrameDescription SilhouetteFrameDescription
        {
            get { return _sensor.BodyIndexFrameSource.FrameDescription; }
        }

        public void LoadGestures(string gesturesDB)
        {
            for (int i = 0; i < _sensor.BodyFrameSource.BodyCount; i++)
            {
                _gestureTrackers.Add(new GestureTracker(_sensor, gesturesDB));
            }
        }

        public void PollMostRecentColorFrame()
        {
            MultiSourceFrame multiFrame = _reader.AcquireLatestFrame();

            if (multiFrame == null)
            {
                return;
            }

            using (ColorFrame frame = multiFrame.ColorFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return; // Could not find multi-frame or color-frame
                }

                frame.CopyConvertedFrameDataToArray(MostRecentColorFrame, ColorImageFormat.Rgba);
            }
        }

        public void PollMostRecentDepthFrame()
        {
            MultiSourceFrame multiFrame = _reader.AcquireLatestFrame();

            if (multiFrame == null)
            {
                return;
            }

            using (DepthFrame frame = multiFrame.DepthFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return; // Could not find multi-frame or depth-frame
                }

                using (KinectBuffer buffer = frame.LockImageBuffer())
                {
                    if (DepthFrameDescription.Width * DepthFrameDescription.Height == buffer.Size / DepthFrameDescription.BytesPerPixel)
                    {
                        ProcessDepthFrameData(
                            buffer.UnderlyingBuffer,
                            buffer.Size,
                            frame.DepthMinReliableDistance,
                            ushort.MaxValue);
                    }
                }
            }
        }

        public void PollMostRecentInfraredFrame()
        {
            MultiSourceFrame multiFrame = _reader.AcquireLatestFrame();

            if (multiFrame == null)
            {
                return;
            }

            using (InfraredFrame frame = multiFrame.InfraredFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return; // Could not find multi-frame or infrared-frame
                }

                using (KinectBuffer buffer = frame.LockImageBuffer())
                {
                    if (InfraredFrameDescription.Width * InfraredFrameDescription.Height == buffer.Size / InfraredFrameDescription.BytesPerPixel)
                    {
                        ProcessInfraredFrameData(buffer.UnderlyingBuffer, buffer.Size);
                    }
                }
            }
        }

        public void PollMostRecentSilhouetteFrame()
        {
            MultiSourceFrame multiFrame = _reader.AcquireLatestFrame();

            if (multiFrame == null)
            {
                return;
            }

            using (BodyIndexFrame frame = multiFrame.BodyIndexFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return; // Could not find multi-frame or body index frame
                }

                using (KinectBuffer buffer = frame.LockImageBuffer())
                {
                    if (
                        SilhouetteFrameDescription.Width *
                        SilhouetteFrameDescription.Height == buffer.Size)
                    {
                        ProcessSilhouetteData(buffer.UnderlyingBuffer, buffer.Size);
                    }
                }
            }
        }

        public void PollMostRecentBodyFrame()
        {
            MultiSourceFrame multiFrame = _reader.AcquireLatestFrame();

            if (multiFrame == null)
            {
                return;
            }

            using (BodyFrame frame = multiFrame.BodyFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return;
                }

                if (_bodies == null)
                {
                    _bodies = new Body[frame.BodyCount];
                }

                frame.GetAndRefreshBodyData(_bodies);
            }

            if (_bodies == null)
            {
                return;
            }
            
            
            _bodies = _bodies.OrderBy(body => _sensor.CoordinateMapper.MapCameraPointToColorSpace(body.Joints[JointType.Head].Position).X).ToArray();
            for (int i = 0; i < _sensor.BodyFrameSource.BodyCount; i++)
            {
                ulong trackingId = _bodies[i].TrackingId;
                _gestureTrackers[i].XPosition = (int)_sensor.CoordinateMapper.MapCameraPointToColorSpace(_bodies[i].Joints[JointType.Head].Position).X;
                _gestureTrackers[i].YPosition = (int)_sensor.CoordinateMapper.MapCameraPointToColorSpace(_bodies[i].Joints[JointType.Head].Position).Y;
                _gestureTrackers[i].TrackingId = trackingId;
                _gestureTrackers[i].IsPaused = trackingId == 0;
            }
        }

        public void PollMostRecentGestureFrame()
        {
            for (int i = 0; i < _gestureTrackers.Count; i++)
            {
                IList<GestureResult> gestures = _gestureTrackers[i].PollMostRecentGestureFrame();

                MostRecentGestures.Clear(i);
                MostRecentGestures.AddGestures(i, gestures);
                MostRecentGestures.SetXPosition(i, _gestureTrackers[i].XPosition);
                MostRecentGestures.SetYPosition(i, _gestureTrackers[i].YPosition);
            }
        }

        public void Dispose()
        {
            if (_gestureTrackers != null)
            {
                _gestureTrackers.Clear();
            }
            _gestureTrackers = null;

            if (_reader != null)
            {
                _reader.Dispose();
            }
            _reader = null;

            if (_sensor != null)
            {
                _sensor.Close();
            }
            _sensor = null;
        }

        private unsafe void ProcessDepthFrameData(
            IntPtr data,
            uint size,
            ushort minDepth,
            ushort maxDepth)
        {
            ushort* frameData = (ushort*)data;

            int pixelIndex = 0;
            for (int i = 0; i < (int)size / DepthFrameDescription.BytesPerPixel; i++)
            {
                byte pixelIntensity = (byte)(frameData[i] >= minDepth && frameData[i] <= maxDepth ? (frameData[i] / MapDepthToByte) : 0);

                MostRecentDepthFrame[pixelIndex++] = pixelIntensity;
                MostRecentDepthFrame[pixelIndex++] = pixelIntensity;
                MostRecentDepthFrame[pixelIndex++] = pixelIntensity;
                MostRecentDepthFrame[pixelIndex++] = 0x00;
            }
        }

        private unsafe void ProcessInfraredFrameData(IntPtr data, uint size)
        {
            ushort* frameData = (ushort*)data;

            int pixelIndex = 0;
            for (int i = 0; i < size / InfraredFrameDescription.BytesPerPixel; i++)
            {
                float pixelIntensity = Math.Min(InfraredOutputValueMaximum, ((frameData[i] / InfraredSourceValueMaximum * InfraredSourceScale) * (1.0f - InfraredOutputValueMinimum)) + InfraredOutputValueMinimum);

                MostRecentInfraredFrame[pixelIndex++] = (byte)(pixelIntensity * 0xFF);
                MostRecentInfraredFrame[pixelIndex++] = (byte)(pixelIntensity * 0xFF);
                MostRecentInfraredFrame[pixelIndex++] = (byte)(pixelIntensity * 0xFF);
                MostRecentInfraredFrame[pixelIndex++] = 0x00;
            }
        }

        private unsafe void ProcessSilhouetteData(IntPtr data, uint size)
        {
            byte* frameData = (byte*)data;

            int pixelIndex = 0;
            for (int i = 0; i < (int)size; i++)
            {
                if (frameData[i] < BodyColor.Length)
                {
                    MostRecentSilhouetteFrame[pixelIndex++] = (byte)(BodyColor[frameData[i]] >> 24);
                    MostRecentSilhouetteFrame[pixelIndex++] = (byte)(BodyColor[frameData[i]] >> 16);
                    MostRecentSilhouetteFrame[pixelIndex++] = (byte)(BodyColor[frameData[i]] >> 8);
                    MostRecentSilhouetteFrame[pixelIndex++] = (byte)(BodyColor[frameData[i]] >> 0);
                }
                else
                {
                    MostRecentSilhouetteFrame[pixelIndex++] = 0x00;
                    MostRecentSilhouetteFrame[pixelIndex++] = 0x00;
                    MostRecentSilhouetteFrame[pixelIndex++] = 0x00;
                    MostRecentSilhouetteFrame[pixelIndex++] = 0xFF;
                }
            }
        }

        private static int CalculateImageByteCount(FrameDescription frameDescription)
        {
            return frameDescription.Width * frameDescription.Height * BytesPerPixelRGBA;
        }

        private static Size CalculateImageSize(FrameDescription frameDescription)
        {
            return new Size()
            {
                Width = frameDescription.Width,
                Height = frameDescription.Height
            };
        }
    }
}
