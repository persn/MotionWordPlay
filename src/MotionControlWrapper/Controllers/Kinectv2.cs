namespace NTNU.MotionControlWrapper.Controllers
{
    using System;
    using System.Drawing;
    using System.Linq;
    using Microsoft.Kinect;

    public class Kinectv2 : IMotionController
    {
        private const int BytesPerPixelRGBA = 4;

        private static readonly uint[] BodyColor =
        {
            0x0000FF00,
            0x00FF0000,
            0xFFFF4000,
            0x40FFFF00,
            0xFF40FF00,
            0xFF808000,
        };

        private readonly KinectSensor _sensor;
        private readonly MultiSourceFrameReader _reader;

        public Kinectv2()
        {
            _sensor = KinectSensor.GetDefault();
            _sensor?.Open();

            _reader = _sensor?.OpenMultiSourceFrameReader(
                FrameSourceTypes.Color |
                FrameSourceTypes.Depth |
                FrameSourceTypes.Infrared |
                FrameSourceTypes.BodyIndex);

            if (_sensor == null)
            {
                return;
            }

            MostRecentColorFrame = new byte[CalculateImageByteCount(ColorFrameDescription)];
            MostRecentDepthFrame = new byte[CalculateImageByteCount(DepthFrameDescription)];
            MostRecentInfraredFrame = new byte[CalculateImageByteCount(InfraredFrameDescription)];
            MostRecentSilhouetteFrame = new byte[CalculateImageByteCount(SilhouetteFrameDescription)];

            ColorFrameSize = CalculateImageSize(ColorFrameDescription);
            DepthFrameSize = CalculateImageSize(DepthFrameDescription);
            InfraredFrameSize = CalculateImageSize(InfraredFrameDescription);
            SilhouetteFrameSize = CalculateImageSize(SilhouetteFrameDescription);
        }

        public Size ColorFrameSize { get; }

        public Size DepthFrameSize { get; }

        public Size InfraredFrameSize { get; }

        public Size SilhouetteFrameSize { get; }

        public byte[] MostRecentColorFrame { get; }

        public byte[] MostRecentDepthFrame { get; }

        public byte[] MostRecentInfraredFrame { get; }

        public byte[] MostRecentSilhouetteFrame { get; }

        private MultiSourceFrame MultiFrame => _reader.AcquireLatestFrame();

        private FrameDescription ColorFrameDescription =>
            _sensor.ColorFrameSource.FrameDescription;

        private FrameDescription DepthFrameDescription =>
            _sensor.DepthFrameSource.FrameDescription;

        private FrameDescription InfraredFrameDescription =>
            _sensor.InfraredFrameSource.FrameDescription;

        private FrameDescription SilhouetteFrameDescription =>
            _sensor.BodyIndexFrameSource.FrameDescription;

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
            ushort[] depthData = new ushort[DepthFrameDescription.Width * DepthFrameDescription.Height];

            using (DepthFrame frame = MultiFrame?.DepthFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return; // Could not find multi-frame or depth-frame
                }

                ushort minDepth = frame.DepthMinReliableDistance;
                ushort maxDepth = frame.DepthMaxReliableDistance;

                frame.CopyFrameDataToArray(depthData);

                int colorIndex = 0;
                foreach (byte intensity in depthData.Select(
                    depth => (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0)))
                {
                    MostRecentDepthFrame[colorIndex++] = intensity;
                    MostRecentDepthFrame[colorIndex++] = intensity;
                    MostRecentDepthFrame[colorIndex++] = intensity;
                    MostRecentDepthFrame[colorIndex++] = 0x00;
                }
            }
        }

        public void PollMostRecentInfraredFrame()
        {
            ushort[] infraredData = new ushort[InfraredFrameDescription.Width * InfraredFrameDescription.Height];

            using (InfraredFrame frame = MultiFrame?.InfraredFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return; // Could not find multi-frame or infrared-frame
                }

                frame.CopyFrameDataToArray(infraredData);

                int colorIndex = 0;
                foreach (byte intensity in infraredData.Select(ir => (byte)(ir >> 8)))
                {
                    MostRecentInfraredFrame[colorIndex++] = intensity;
                    MostRecentInfraredFrame[colorIndex++] = intensity;
                    MostRecentInfraredFrame[colorIndex++] = intensity;
                    MostRecentInfraredFrame[colorIndex++] = 0x00;
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
