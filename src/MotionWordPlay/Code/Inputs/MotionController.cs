namespace NTNU.MotionWordPlay.Inputs
{
    using System;
    using System.Drawing;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using MotionControlWrapper;

    public class MotionController : IGameLoop, IDisposable
    {
        private GraphicsDevice _graphicsDevice;

        private IMotionController _motionController;
        private Texture2D _currentColorFrame;
        private Texture2D _currentDepthFrame;
        private Texture2D _currentInfraredFrame;
        private Texture2D _currentSilhouetteFrame;

        public MotionController()
        {
            _motionController = MotionControllerFactory.CreateMotionController(
                MotionControllerAPI.Kinectv2);
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

            _motionController?.Dispose();
            _motionController = null;
        }

        public void Initialize()
        {
            _motionController.LoadGestures(@"Content\MotionGestures\GesturesDB.gbd");
        }

        public void Load(ContentManager contentManager)
        {
            _currentColorFrame = CreateTexture(_motionController.ColorFrameSize);
            _currentDepthFrame = CreateTexture(_motionController.DepthFrameSize);
            _currentInfraredFrame = CreateTexture(_motionController.InfraredFrameSize);
            _currentSilhouetteFrame = CreateTexture(_motionController.SilhouetteFrameSize);
        }

        public void Update(GameTime gameTime)
        {
            switch (CurrentFrameState)
            {
                case FrameState.Color:
                    UpdateFrame(
                        _currentColorFrame,
                        _motionController.MostRecentColorFrame,
                        () => _motionController.PollMostRecentColorFrame());
                    break;
                case FrameState.Depth:
                    UpdateFrame(
                        _currentDepthFrame,
                        _motionController.MostRecentDepthFrame,
                        () => _motionController.PollMostRecentDepthFrame());
                    break;
                case FrameState.Infrared:
                    UpdateFrame(
                        _currentInfraredFrame,
                        _motionController.MostRecentInfraredFrame,
                        () => _motionController.PollMostRecentInfraredFrame());
                    break;
                case FrameState.Silhouette:
                    UpdateFrame(
                        _currentSilhouetteFrame,
                        _motionController.MostRecentSilhouetteFrame,
                        () => _motionController.PollMostRecentSilhouetteFrame());
                    break;
                default:
                    throw new NotSupportedException("Switch case reached somewhere it shouldn't.");
            }

            _motionController.PollMostRecentBodyFrame();
            _motionController.PollMostRecentGestureFrame();

            #region Debug proof-of-concept for gestures retrieval from framework
            for (int i = 0; i < 6; i++)
            {
                IList<GestureResult> gestures = _motionController.MostRecentGestures.GetGestures(i);

                if (gestures.Count > 0)
                {//Breakpoint goes here
                    //System.Windows.Forms.MessageBox.Show("hOi! Welcome to da tem shop");
                }
            }
            #endregion
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
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

        public void GraphicsDeviceCreated(GraphicsDevice graphicsDevice, Vector2 nativeSize)
        {
            _graphicsDevice = graphicsDevice;
        }

        private Texture2D CreateTexture(Size size)
        {
            return new Texture2D(_graphicsDevice, size.Width, size.Height);
        }

        private static void UpdateFrame(Texture2D frame, byte[] data, Action pollNewFrame)
        {
            pollNewFrame();

            if (data != null)
            {
                frame.SetData(data);
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
