namespace NTNU.MotionWordPlay.Inputs.Motion
{
    using System;
    using System.Drawing;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using MotionControlWrapper;
    using Path = System.IO.Path;

    public class MotionController : IGameLoop, IDisposable
    {
        public event EventHandler<GestureReceivedEventArgs> GesturesReceived;
        public event EventHandler<int[]> BodyFrameReceived;

        private GraphicsDevice _graphicsDevice;

        private IMotionController _motionController;
        private FrameState _currentFrameState;
        private Texture2D _currentColorFrame;
        private Texture2D _currentDepthFrame;
        private Texture2D _currentInfraredFrame;
        private Texture2D _currentSilhouetteFrame;

        public MotionController()
        {
            _motionController = MotionControllerFactory.CreateMotionController(
                MotionControllerAPI.Kinectv2);
            _currentFrameState = FrameState.Silhouette;
        }

        ~MotionController()
        {
            Dispose();
        }

        public FrameState CurrentFrameState
        {
            get
            {
                return _currentFrameState;
            }
            set
            {
                _currentFrameState = value;
            }
        }

        public void Dispose()
        {
            if (_currentColorFrame != null)
            {
                _currentColorFrame.Dispose();
                _currentColorFrame = null;
            }

            if (_currentDepthFrame != null)
            {
                _currentDepthFrame.Dispose();
                _currentDepthFrame = null;
            }

            if (_currentInfraredFrame != null)
            {
                _currentInfraredFrame.Dispose();
                _currentInfraredFrame = null;
            }

            if (_currentSilhouetteFrame != null)
            {
                _currentSilhouetteFrame.Dispose();
                _currentSilhouetteFrame = null;
            }

            if (_motionController != null)
            {
                _motionController.Dispose();
                _motionController = null;
            }
        }

        public void Initialize()
        {
            _motionController.LoadGestures(string.Format(@"Content{0}MotionGestures{0}GesturesDB.gbd", Path.DirectorySeparatorChar));
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

            bool gesturesDetected = false;
            int[] xCoordinates = new int[6];
            for (int i = 0; i < 6; i++)
            {
                IList<GestureResult> gestures = _motionController.MostRecentGestures.GetGestures(i);

                xCoordinates[i] = _motionController.MostRecentGestures.GetXPosition(i);

                if (gestures.Count > 0)
                {
                    gesturesDetected = true;
                }
            }
            if (gesturesDetected)
            {
                InvokeGesturesReceived(_motionController.MostRecentGestures);
            }
            InvokeBodyFrameReceived(xCoordinates);
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

        public Matrix CalculateDrawScale(Vector2 nativeScreenSize)
        {
            float width;
            float height;

            switch (_currentFrameState)
            {
                case FrameState.Color:
                    width = _motionController.ColorFrameSize.Width;
                    height = _motionController.ColorFrameSize.Height;
                    break;
                case FrameState.Depth:
                    width = _motionController.DepthFrameSize.Width;
                    height = _motionController.DepthFrameSize.Height;
                    break;
                case FrameState.Infrared:
                    width = _motionController.InfraredFrameSize.Width;
                    height = _motionController.InfraredFrameSize.Height;
                    break;
                case FrameState.Silhouette:
                    width = _motionController.SilhouetteFrameSize.Width;
                    height = _motionController.SilhouetteFrameSize.Height;
                    break;
                default:
                    throw new NotSupportedException("Switch case reached somewhere it shouldn't.");
            }

            float horScaling = nativeScreenSize.X / width;
            float verScaling = nativeScreenSize.Y / height;

            return Matrix.CreateScale(new Vector3(horScaling, verScaling, 1));
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

        private void InvokeGesturesReceived(GestureResults gestures)
        {
            if (GesturesReceived != null)
            {
                GesturesReceived.Invoke(this, new GestureReceivedEventArgs(gestures));
            }
        }

        private void InvokeBodyFrameReceived(int[] coordinates)
        {
            if (BodyFrameReceived != null)
            {
                BodyFrameReceived.Invoke(this, coordinates);
            }
        }
    }
}
