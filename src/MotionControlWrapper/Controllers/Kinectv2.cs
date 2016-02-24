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

            ColorFrameSize = new Size()
            {
                Width = _sensor.ColorFrameSource.FrameDescription.Width,
                Height = _sensor.ColorFrameSource.FrameDescription.Height
            };

            DepthFrameSize = new Size()
            {
                Width = _sensor.DepthFrameSource.FrameDescription.Width,
                Height = _sensor.DepthFrameSource.FrameDescription.Height
            };

            InfraredFrameSize = new Size()
            {
                Width = _sensor.InfraredFrameSource.FrameDescription.Width,
                Height = _sensor.InfraredFrameSource.FrameDescription.Height
            };

            SilhouetteFrameSize = new Size()
            {
                Width = _sensor.BodyIndexFrameSource.FrameDescription.Width,
                Height = _sensor.BodyIndexFrameSource.FrameDescription.Height
            };
        }

        public Size ColorFrameSize { get; }
        public Size DepthFrameSize { get; }
        public Size InfraredFrameSize { get; }
        public Size SilhouetteFrameSize { get; }

        private MultiSourceFrame MultiFrame => _reader.AcquireLatestFrame();

        public byte[] AcquireLatestColorFrame()
        {
            FrameDescription frameDescription = _sensor.ColorFrameSource.CreateFrameDescription(
                ColorImageFormat.Rgba);

            byte[] pixels = new byte[
                frameDescription.Width *
                frameDescription.Height *
                frameDescription.BytesPerPixel];

            using (ColorFrame frame = MultiFrame?.ColorFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return null; // Could not find multi-frame or color-frame
                }

                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Rgba);
                return pixels;
            }
        }

        public byte[] AcquireLatestDepthFrame()
        {
            FrameDescription frameDescription = _sensor.DepthFrameSource.FrameDescription;

            byte[] pixels = new byte[
                frameDescription.Width *
                frameDescription.Height *
                BytesPerPixelRGBA];
            ushort[] depthData = new ushort[frameDescription.Width * frameDescription.Height];

            using (DepthFrame frame = MultiFrame?.DepthFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return null; // Could not find multi-frame or depth-frame
                }

                ushort minDepth = frame.DepthMinReliableDistance;
                ushort maxDepth = frame.DepthMaxReliableDistance;

                frame.CopyFrameDataToArray(depthData);

                int colorIndex = 0;
                foreach (byte intensity in depthData.Select(
                    depth => (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0)))
                {
                    pixels[colorIndex++] = intensity; // Blue
                    pixels[colorIndex++] = intensity; // Green
                    pixels[colorIndex++] = intensity; // Red

                    colorIndex++;
                }

                return pixels;
            }
        }

        public byte[] AcquireLatestInfraredFrame()
        {
            FrameDescription frameDescription = _sensor.InfraredFrameSource.FrameDescription;

            byte[] pixels = new byte[
                frameDescription.Width *
                frameDescription.Height *
                BytesPerPixelRGBA];
            ushort[] infraredData = new ushort[frameDescription.Width * frameDescription.Height];

            using (InfraredFrame frame = MultiFrame?.InfraredFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return null; // Could not find multi-frame or infrared-frame
                }

                frame.CopyFrameDataToArray(infraredData);

                int colorIndex = 0;
                foreach (byte intensity in infraredData.Select(ir => (byte)(ir >> 8)))
                {
                    pixels[colorIndex++] = intensity; // Blue
                    pixels[colorIndex++] = intensity; // Green
                    pixels[colorIndex++] = intensity; // Red

                    colorIndex++;
                }

                return pixels;
            }
        }

        public byte[] AcquireLatestSilhouetteFrame()
        {
            FrameDescription frameDescription = _sensor.BodyIndexFrameSource.FrameDescription;

            //byte[] pixels = new byte[
            //    frameDescription.Width *
            //    frameDescription.Height *
            //    BytesPerPixelRGBA];

            using (BodyIndexFrame frame = MultiFrame?.BodyIndexFrameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return null; // Could not find multi-frame or body index frame
                }

                using (KinectBuffer buffer = frame.LockImageBuffer())
                {
                    if (frameDescription.Width * frameDescription.Height == buffer.Size)
                    {
                        return ProcessSilhouetteData(buffer.UnderlyingBuffer, buffer.Size);
                    }
                }
            }

            return null;
        }

        private static unsafe byte[] ProcessSilhouetteData(IntPtr data, uint size)
        {
            byte[] pixels = new byte[size * BytesPerPixelRGBA];

            byte* frameData = (byte*)data;

            int pixelIndex = 0;
            for (int i = 0; i < (int)size; i++)
            {
                if (frameData[i] < BodyColor.Length)
                {
                    pixels[pixelIndex++] = (byte)(BodyColor[frameData[i]] >> 24); // Red
                    pixels[pixelIndex++] = (byte)(BodyColor[frameData[i]] >> 16); // Green
                    pixels[pixelIndex++] = (byte)(BodyColor[frameData[i]] >> 8);  // Blue
                    pixels[pixelIndex++] = (byte)(BodyColor[frameData[i]] >> 0);  // Alpha
                }
                else
                {
                    pixels[pixelIndex++] = 0x00; // Red
                    pixels[pixelIndex++] = 0x00; // Green
                    pixels[pixelIndex++] = 0x00; // Blue
                    pixels[pixelIndex++] = 0xFF; // Alpha
                }
            }

            return pixels;
        }
    }
}
