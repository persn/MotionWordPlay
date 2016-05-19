namespace NTNU.MotionWordPlay
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using GameCore;
    using UserInterface;
    using Inputs;
    using Inputs.Keyboard;
    using Inputs.Motion;
    using MotionControlWrapper;
    using Color = System.Drawing.Color;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private const string SwapObjectGestureName = "CrossedArms";
        private const string CheckAnswerGestureName = "RaisedHands";

        private static readonly Vector2 BaseScreenSize = new Vector2(640, 360);
        private readonly GraphicsDeviceManager _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Matrix _globalTransformation;

        private readonly InputHandler _inputHandler;
        private readonly IUserInterface _userInterface;
        private readonly WordPlayWrapper _wordPlayGame;

        public Game()
        {
            _graphicsDevice = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int)BaseScreenSize.X,
                PreferredBackBufferHeight = (int)BaseScreenSize.Y,
                GraphicsProfile = GraphicsProfile.Reach
            };
            _graphicsDevice.DeviceCreated += GraphicsDeviceCreated;

            Content.RootDirectory = "Content";

            _inputHandler = new InputHandler();
            _inputHandler.KeyboardInput.KeyPressed += KeyboardInputKeyPressed;
            _inputHandler.MotionController.GesturesReceived += MotionControllerGesturesReceived;

            _userInterface = new EmptyKeysWrapper();

            _wordPlayGame = new WordPlayWrapper(2);
            _wordPlayGame.PreGame += WordPlayPreGame;
            _wordPlayGame.GameUpdate += WordPlayGameUpdate;
            _wordPlayGame.PostGame += WordPlayPostGame;
            _wordPlayGame.NewGameLoaded += WordPlayNewGameLoaded;
            _wordPlayGame.AnswersChangedPlaces += WordPlayAnswersChangedPlaces;
            _wordPlayGame.AnswersIncorrect += WordPlayAnswersIncorrect;
            _wordPlayGame.AnswersCorrect += WordPlayAnswersCorrect;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _globalTransformation = _inputHandler.MotionController.CalculateDrawScale(BaseScreenSize);

            _inputHandler.Initialize();
            _userInterface.Initialize();
            _wordPlayGame.Initialize();

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

            _inputHandler.Load(Content);
            _userInterface.Load(Content);
            _wordPlayGame.Load(Content);
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
            _inputHandler.Update(gameTime);
            _wordPlayGame.Update(gameTime);
            _userInterface.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);

            _spriteBatch.Begin(SpriteSortMode.Deferred, transformMatrix: _globalTransformation);

            _inputHandler.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            // Independent draw
            _userInterface.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (_inputHandler != null)
            {
                _inputHandler.Dispose();
            }

            base.OnExiting(sender, args);
        }

        private void GraphicsDeviceCreated(object sender, EventArgs e)
        {
            _inputHandler.GraphicsDeviceCreated(GraphicsDevice, BaseScreenSize);
            _userInterface.GraphicsDeviceCreated(GraphicsDevice, BaseScreenSize);
        }

        private void KeyboardInputKeyPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.PressedKey)
            {
                case Keys.D1:
                    _inputHandler.MotionController.CurrentFrameState = FrameState.Color;
                    break;
                case Keys.D2:
                    _inputHandler.MotionController.CurrentFrameState = FrameState.Depth;
                    break;
                case Keys.D3:
                    _inputHandler.MotionController.CurrentFrameState = FrameState.Infrared;
                    break;
                case Keys.D4:
                    _inputHandler.MotionController.CurrentFrameState = FrameState.Silhouette;
                    break;
                case Keys.F4:
                    _graphicsDevice.IsFullScreen = !_graphicsDevice.IsFullScreen;
                    _graphicsDevice.ApplyChanges();
                    break;
                case Keys.A:
                    _wordPlayGame.SwapObjects(0, 1);
                    break;
                case Keys.S:
                    _wordPlayGame.SwapObjects(1, 2);
                    break;
                case Keys.D:
                    _wordPlayGame.SwapObjects(2, 3);
                    break;
                case Keys.F:
                    _wordPlayGame.SwapObjects(3, 4);
                    break;
                case Keys.G:
                    _wordPlayGame.SwapObjects(4, 5);
                    break;
                case Keys.Q:
                    _wordPlayGame.LoadTask();
                    break;
                case Keys.W:
                    _wordPlayGame.CheckAnswer();
                    break;
                default:
                    throw new NotSupportedException("Key is not supported");
            }

            _globalTransformation = _inputHandler.MotionController.CalculateDrawScale(BaseScreenSize);
        }

        private void MotionControllerGesturesReceived(object sender, GestureReceivedEventArgs e)
        {
            List<int> playersDoingSwapObjectGesture = new List<int>();
            List<int> playersDoingCheckAnswerGesture = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                IList<GestureResult> gestures = e.Gestures.GetGestures(i);

                if (gestures.Count > 0)
                {
                    foreach (GestureResult gestureResult in gestures)
                    {
                        if (gestureResult.Name.Equals(SwapObjectGestureName) && gestureResult.IsDetected)
                        {
                            playersDoingSwapObjectGesture.Add(i);
                        }
                        else if (gestureResult.Name.Equals(CheckAnswerGestureName) && gestureResult.Confidence > 0.7)
                        {
                            playersDoingCheckAnswerGesture.Add(i);
                        }
                    }
                }

            }
            if (playersDoingCheckAnswerGesture.Count == _wordPlayGame.WordPlayGame.PlayerCount)
            {
                _wordPlayGame.CheckAnswer();
            }
            if (playersDoingSwapObjectGesture.Count >= 2)
            {
                _wordPlayGame.SwapObjects(playersDoingSwapObjectGesture[0], playersDoingSwapObjectGesture[1]);
            }
        }

        private void WordPlayPreGame(object sender, GameDataEventArgs e)
        {
            _userInterface.Status.Text = "Do stuff to start game";
        }

        private void WordPlayGameUpdate(object sender, GameDataEventArgs e)
        {
            _userInterface.Time.Text = e.ElapsedTime.ToString();
        }

        private void WordPlayPostGame(object sender, GameDataEventArgs e)
        {
            _userInterface.ResetUI();

            _userInterface.Time.Text = e.ElapsedTime.ToString();
            _userInterface.Status.Text = "Game Over\nFinal Score: " + e.Score;
            _userInterface.Score.Text = e.Score.ToString();
        }

        private void WordPlayNewGameLoaded(object sender, GameDataEventArgs e)
        {
            _userInterface.ResetUI();
            _userInterface.AddNewPuzzleFractions(_wordPlayGame.WordPlayGame.CurrentTask.Length);

            ResetUIToDefaultValues(e);
        }

        private void WordPlayAnswersChangedPlaces(object sender, GameDataEventArgs e)
        {
            ResetUIToDefaultValues(e);
        }

        private void WordPlayAnswersIncorrect(object sender, GameDataEventArgs e)
        {
            ResetUIToDefaultValues(e);

            _userInterface.Status.Foreground = Color.Red;
            _userInterface.Status.Text = "Wrong! Try again";

            for (int i = 0; i < _userInterface.PuzzleFractions.Count; i++)
            {
                _userInterface.PuzzleFractions[i].Foreground = e.Result[i] ? Color.Green : Color.Red;
            }
        }

        private void WordPlayAnswersCorrect(object sender, GameDataEventArgs e)
        {
            ResetUIToDefaultValues(e);

            _userInterface.Status.Foreground = Color.Green;
            _userInterface.Status.Text = "Correct! + " + e.ScoreIncrement + " points";

            if (e.Combo > 1)
            {
                _userInterface.Status.Text += "Combo: " + e.Combo;
            }
        }

        private void ResetUIToDefaultValues(GameDataEventArgs e)
        {
            _userInterface.Time.Text = e.ElapsedTime.ToString();
            _userInterface.Task.Text = e.AnswerCounter.ToString();
            _userInterface.Score.Text = e.Score.ToString();
            _userInterface.Status.Text = string.Empty;
            _userInterface.Status.Foreground = Color.White;

            if (!e.IsGameLoaded)
            {
                return;
            }

            for (int i = 0; i < _userInterface.PuzzleFractions.Count; i++)
            {
                _userInterface.PuzzleFractions[i].Text = e.WordFractions[i];
                _userInterface.PuzzleFractions[i].Foreground = Color.White;
                _userInterface.PuzzleFractions[i].X = 50 + i * 100;
                _userInterface.PuzzleFractions[i].Y = 150;
            }
        }
    }
}
