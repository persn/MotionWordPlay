namespace NTNU.MotionWordPlay
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using UserInterface;
    using Inputs;
    using TestGame;

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

        private DemoGame _demoGame;

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

            _keyboardInput.Load(Content);
            _motionController.Load(Content);
            _userInterface.Load(Content);
            _demoGame = new DemoGame(6, 1);
            _userInterface.AddNewPuzzleFractions(1);
            _userInterface.UpdatePuzzleFraction(0, "Do stuff to start game", 200, 150);
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
            _keyboardInput.Update(gameTime);
            _motionController.Update(gameTime);
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

            _keyboardInput.Draw(gameTime, _spriteBatch);
            _motionController.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            // Independent draw
            _userInterface.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (_motionController != null)
            {
                _motionController.Dispose();
                _motionController = null;
            }

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
                case Keys.A:
                    SwapObjects(0, 1);
                    break;
                case Keys.S:
                    SwapObjects(1, 2);
                    break;
                case Keys.D:
                    SwapObjects(2, 3);
                    break;
                case Keys.F:
                    SwapObjects(3, 4);
                    break;
                case Keys.G:
                    SwapObjects(4, 5);
                    break;
                case Keys.Q:
                    LoadTask(6);
                    break;
                case Keys.W:
                    CheckAnswer();
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

        #region Game Specific functions

        private void SwapObjects(int index1, int index2)
        {
            if (_demoGame.CurrentTask == null)
            {
                return;
            }
            _demoGame.SwapObjects(index1, index2);
            RefreshText();
        }

        private void RefreshText ()
        {
            if (_demoGame.CurrentTask == null)
            {
                return;
            }
            for (int i = 0; i < _demoGame.CurrentTask.Length; i++)
            {
                _userInterface.UpdatePuzzleFraction(i, _demoGame.CurrentTask[i].Item1);
            }
        }

        private void LoadTask(int numPlayers)
        {
            _demoGame.CreateNewTask();
            _userInterface.ResetUI();
            _userInterface.Task = _demoGame.AnswerCounter.ToString();
            _userInterface.AddNewPuzzleFractions(numPlayers);
            for (int i = 0; i < _demoGame.CurrentTask.Length; i++)
            {
                _userInterface.UpdatePuzzleFraction(i, _demoGame.CurrentTask[i].Item1, 50 + i * 100, 150);
            }
        }

        private void CheckAnswer()
        {
            if (_demoGame.CurrentTask == null)
            {
                return;
            }
            bool[] result;
            bool correct = _demoGame.IsCorrect(out result);
            for (int i = 0; i < _demoGame.CurrentTask.Length; i++)
            {
                _userInterface.UpdatePuzzleFraction(i, _demoGame.CurrentTask[i].Item1 + "\n" + result[i]);
            }
            if (!correct)
            {
                return;
            }
            bool gameOver = _demoGame.CorrectAnswerGiven();
            _userInterface.Score = _demoGame.Score.ToString();
            if (gameOver)
            {
                EndGame();
            }
            _userInterface.Task = _demoGame.AnswerCounter.ToString();
        }

        private void EndGame()
        {
            _userInterface.AddNewPuzzleFractions(1);
            _userInterface.UpdatePuzzleFraction(_demoGame.CurrentTask.Length, "Game Over\n Final Score: "+_demoGame.Score, 200, 50);
        }
        #endregion
    }
}
