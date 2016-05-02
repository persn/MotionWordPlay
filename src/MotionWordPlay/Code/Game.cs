namespace NTNU.MotionWordPlay
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using UserInterface;
    using Inputs;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private static readonly Vector2 BaseScreenSize = new Vector2(640, 360);
        private readonly GraphicsDeviceManager _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Matrix _globalTransformation;

        private KeyboardInput _keyboardInput;
        private MotionController _motionController;
        private IUserInterface _userInterface;

        public Game()
        {
            _graphicsDevice = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int)BaseScreenSize.X,
                PreferredBackBufferHeight = (int)BaseScreenSize.Y,
                GraphicsProfile = GraphicsProfile.Reach
            };
            _graphicsDevice.DeviceCreated += Graphics_DeviceCreated;

            Content.RootDirectory = "Content";

            _keyboardInput = new KeyboardInput();
            _keyboardInput.KeyPressed += KeyboardInputKeyPressed;
            _motionController = new MotionController();
            _userInterface = new EmptyKeysWrapper();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ChangeDrawScale(FrameState.Color);

            _keyboardInput.Initialize();
            _motionController.Initialize();
            _userInterface.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _keyboardInput?.Load(Content);
            _motionController?.Load(Content);
            _userInterface?.Load(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();

            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _keyboardInput?.Update(gameTime);
            _motionController?.Update(gameTime);
            _userInterface.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(SpriteSortMode.Deferred, transformMatrix: _globalTransformation);

            _keyboardInput?.Draw(gameTime, _spriteBatch);
            _motionController?.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            // Independent draw
            _userInterface.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            _motionController?.Dispose();
            _motionController = null;

            base.OnExiting(sender, args);
        }

        private void Graphics_DeviceCreated(object sender, EventArgs e)
        {
            _keyboardInput.GraphicsDeviceCreated(GraphicsDevice, BaseScreenSize);
            _motionController.GraphicsDeviceCreated(GraphicsDevice, BaseScreenSize);
            _userInterface.GraphicsDeviceCreated(GraphicsDevice, BaseScreenSize);
        }

        private void KeyboardInputKeyPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.PressedKey)
            {
                case Keys.D1:
                    _motionController.CurrentFrameState = FrameState.Color;
                    break;
                case Keys.D2:
                    _motionController.CurrentFrameState = FrameState.Depth;
                    break;
                case Keys.D3:
                    _motionController.CurrentFrameState = FrameState.Infrared;
                    break;
                case Keys.D4:
                    _motionController.CurrentFrameState = FrameState.Silhouette;
                    break;
                case Keys.F4:
                    _graphicsDevice.IsFullScreen = !_graphicsDevice.IsFullScreen;
                    _graphicsDevice.ApplyChanges();
                    break;
                default:
                    throw new NotSupportedException("Key is not supported");
            }
            ChangeDrawScale(_motionController.CurrentFrameState);
        }

        private void ChangeDrawScale(FrameState frameState)
        {
            float width;
            float height;

            switch (frameState)
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

            float horScaling = BaseScreenSize.X / width;
            float verScaling = BaseScreenSize.Y / height;

            if (horScaling < verScaling)
            {
                verScaling = horScaling;
            }
            else
            {
                horScaling = verScaling;
            }

            _globalTransformation = Matrix.CreateScale(new Vector3(horScaling, verScaling, 1));
        }
    }
}
