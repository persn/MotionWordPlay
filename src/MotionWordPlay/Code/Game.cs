namespace NTNU.MotionWordPlay
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private static readonly Vector2 BaseScreenSize = new Vector2(640, 360);
        private readonly GraphicsDeviceManager _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Matrix _globalTransformation;

        private MotionController _motionController;

        public Game()
        {
            _graphicsDevice = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int)BaseScreenSize.X,
                PreferredBackBufferHeight = (int)BaseScreenSize.Y,
                GraphicsProfile = GraphicsProfile.Reach
            };

            Content.RootDirectory = "Content";
        }

        private KeyboardState KeyboardPreviousState { get; set; }
        private KeyboardState KeyboardCurrentState { get; set; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _motionController = new MotionController(_graphicsDevice.GraphicsDevice);

            ChangeDrawScale(FrameState.Color);

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

            _motionController?.Load();
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
            KeyboardPreviousState = KeyboardCurrentState;
            KeyboardCurrentState = Keyboard.GetState();

            if (KeyboardCurrentState.IsKeyDown(Keys.Escape) &&
                !KeyboardPreviousState.IsKeyDown(Keys.Escape))
            {
                ExitGame();
            }

            if (KeyboardCurrentState.IsKeyDown(Keys.D1) ||
                KeyboardCurrentState.IsKeyDown(Keys.D2) ||
                KeyboardCurrentState.IsKeyDown(Keys.D3) ||
                KeyboardCurrentState.IsKeyDown(Keys.D4))
            {
                if (KeyboardCurrentState.IsKeyDown(Keys.D1) &&
                !KeyboardPreviousState.IsKeyDown(Keys.D1))
                {
                    _motionController.CurrentFrameState = FrameState.Color;
                }

                if (KeyboardCurrentState.IsKeyDown(Keys.D2) &&
                    !KeyboardPreviousState.IsKeyDown(Keys.D2))
                {
                    _motionController.CurrentFrameState = FrameState.Depth;
                }

                if (KeyboardCurrentState.IsKeyDown(Keys.D3) &&
                    !KeyboardPreviousState.IsKeyDown(Keys.D3))
                {
                    _motionController.CurrentFrameState = FrameState.Infrared;
                }

                if (KeyboardCurrentState.IsKeyDown(Keys.D4) &&
                    !KeyboardPreviousState.IsKeyDown(Keys.D4))
                {
                    _motionController.CurrentFrameState = FrameState.Silhouette;
                }

                ChangeDrawScale(_motionController.CurrentFrameState);
            }
            else if (KeyboardCurrentState.IsKeyDown(Keys.F4) &&
                !KeyboardPreviousState.IsKeyDown(Keys.F4))
            {
                _graphicsDevice.IsFullScreen = !_graphicsDevice.IsFullScreen;
                _graphicsDevice.ApplyChanges();
            }

            _motionController?.Update(gameTime);

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

            _motionController?.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ExitGame()
        {
            _motionController?.Dispose();
            _motionController = null;

            //Exit();
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
