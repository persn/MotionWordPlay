namespace NTNU.MotionWordPlay
{
    using System;
    using System.Drawing;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MotionControlWrapper;

    public class MotionController : IGameLoop, IDisposable
    {
        private readonly IMotionController _motionController;
        private Texture2D _currentColorFrame;
        private Texture2D _currentDepthFrame;
        private Texture2D _currentInfraredFrame;
        private Texture2D _currentSilhouetteFrame;

        public MotionController(GraphicsDevice graphicsDevice)
        {
            _motionController = MotionControllerFactory.CreateMotionController(
                MotionControllerAPI.Kinectv2);

            _currentColorFrame = LoadTexture(
                graphicsDevice,
                _motionController.ColorFrameSize);
            _currentDepthFrame = LoadTexture(
                graphicsDevice,
                _motionController.DepthFrameSize);
            _currentInfraredFrame = LoadTexture(
                graphicsDevice,
                _motionController.InfraredFrameSize);
            _currentSilhouetteFrame = LoadTexture(
                graphicsDevice,
                _motionController.SilhouetteFrameSize);
        }

        ~MotionController()
        {
            Dispose();
        }

        public FrameState CurrentFrameState { get; set; }
        public Size ColorFrameSize => _motionController.ColorFrameSize;
        public Size DepthFrameSize => _motionController.DepthFrameSize;
        public Size InfraredFrameSize => _motionController.InfraredFrameSize;
        public Size SilhouetteFrameSize => _motionController.SilhouetteFrameSize;

        public void Update(GameTime gameTime)
        {
            switch (CurrentFrameState)
            {
                case FrameState.Color:
                    UpdateFrame(
                        _currentColorFrame,
                        () => _motionController.AcquireLatestColorFrame());
                    break;
                case FrameState.Depth:
                    UpdateFrame(
                        _currentDepthFrame,
                        () => _motionController.AcquireLatestDepthFrame());
                    break;
                case FrameState.Infrared:
                    UpdateFrame(
                        _currentInfraredFrame,
                        () => _motionController.AcquireLatestInfraredFrame());
                    break;
                case FrameState.Silhouette:
                    UpdateFrame(_currentSilhouetteFrame,
                        () => _motionController.AcquireLatestSilhouetteFrame());
                    break;
                default:
                    throw new NotSupportedException("Switch case reached somewhere it shouldn't.");
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (CurrentFrameState)
            {
                case FrameState.Color:
                    DrawFrame(_currentColorFrame, spriteBatch);
                    break;
                case FrameState.Depth:
                    DrawFrame(_currentDepthFrame, spriteBatch);
                    break;
                case FrameState.Infrared:
                    DrawFrame(_currentInfraredFrame, spriteBatch);
                    break;
                case FrameState.Silhouette:
                    DrawFrame(_currentSilhouetteFrame, spriteBatch);
                    break;
                default:
                    throw new NotSupportedException("Switch case reached somewhere it shouldn't.");
            }
        }

        public void Dispose()
        {
            _currentColorFrame?.Dispose();
            _currentColorFrame = null;

            _currentDepthFrame?.Dispose();
            _currentDepthFrame = null;

            _currentInfraredFrame?.Dispose();
            _currentInfraredFrame = null;

            _currentSilhouetteFrame?.Dispose();
            _currentSilhouetteFrame = null;
        }

        private static Texture2D LoadTexture(GraphicsDevice graphicsDevice, Size size)
        {
            return new Texture2D(graphicsDevice, size.Width, size.Height);
        }

        private static void UpdateFrame(Texture2D frame, Func<byte[]> acquireNewFrame)
        {
            byte[] newFrame = acquireNewFrame();

            if (newFrame != null)
            {
                frame.SetData(newFrame);
            }
        }

        private static void DrawFrame(Texture2D frame, SpriteBatch spriteBatch)
        {
            if (frame != null)
            {
                spriteBatch.Draw(frame, Vector2.Zero);
            }
        }
    }
}
