﻿namespace NTNU.MotionControlWrapper.Controllers.Kinect
{
    using System;
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
            _sensor?.Open();

            _reader = _sensor?.OpenMultiSourceFrameReader(
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

        public Size ColorFrameSize { get; }

        public Size DepthFrameSize { get; }

        public Size InfraredFrameSize { get; }

        public Size SilhouetteFrameSize { get; }

        public byte[] MostRecentColorFrame { get; }

        public byte[] MostRecentDepthFrame { get; }

        public byte[] MostRecentInfraredFrame { get; }

        public byte[] MostRecentSilhouetteFrame { get; }

        public GestureResults MostRecentGestures { get; }

        private MultiSourceFrame MultiFrame => _reader.AcquireLatestFrame();

        private FrameDescription ColorFrameDescription =>
            _sensor.ColorFrameSource.FrameDescription;

        private FrameDescription DepthFrameDescription =>
            _sensor.DepthFrameSource.FrameDescription;

        private FrameDescription InfraredFrameDescription =>
            _sensor.InfraredFrameSource.FrameDescription;

        private FrameDescription SilhouetteFrameDescription =>
            _sensor.BodyIndexFrameSource.FrameDescription;

        public void LoadGestures(string gesturesDB)
        {
            for (int i = 0; i < _sensor.BodyFrameSource.BodyCount; i++)
            {
                _gestureTrackers.Add(new GestureTracker(_sensor, gesturesDB));
            }
        }

        public void PollMostRecentColorFrame()
        {
            using (ColorFrame frame = MultiFrame?.ColorFrameReference.AcquireFrame())
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
            using (DepthFrame frame = MultiFrame?.DepthFrameReference.AcquireFrame())
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
            using (InfraredFrame frame = MultiFrame?.InfraredFrameReference.AcquireFrame())
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
            using (BodyIndexFrame frame = MultiFrame?.BodyIndexFrameReference.AcquireFrame())
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
            using (BodyFrame frame = MultiFrame?.BodyFrameReference.AcquireFrame())
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

            for (int i = 0; i < _sensor.BodyFrameSource.BodyCount; i++)
            {
                ulong trackingId = _bodies[i].TrackingId;

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
            }
        }

        public void Dispose()
        {
            _gestureTrackers?.Clear();
            _gestureTrackers = null;

            _reader?.Dispose();
            _reader = null;

            _sensor?.Close();
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