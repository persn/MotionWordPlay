namespace NTNU.MotionWordPlay.GameCore
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using WordPlay;

    public class WordPlayWrapper : IGameLoop
    {
        public event EventHandler<EventArgs> PreGame;
        public event EventHandler<GameLoopUpdateEventArgs> GameLoopUpdate;
        public event EventHandler<PostGameEventArgs> PostGame;

        private readonly WordPlayGame _wordPlayGame;
        private bool _isGameRunning;
        private double _timer;
        private int _elapsedTime;

        public WordPlayWrapper()
        {
            _wordPlayGame = new WordPlayGame(3);
            _isGameRunning = false;
            _timer = 1000;
            _elapsedTime = 0;
        }

        //public WordPlayGame WordPlayGame
        //{
        //    get
        //    {
        //        return _wordPlayGame;
        //    }
        //}

        public int PlayersCount
        {
            get
            {
                return _wordPlayGame.NumPlayers;
            }
        }

        public void Initialize()
        {
        }

        public void Load(ContentManager contentManager)
        {
            InvokePreGame();
        }

        public void Update(GameTime gameTime)
        {
            if (!_isGameRunning)
            {
                return;
            }

            _timer -= gameTime.ElapsedGameTime.Milliseconds;

            if (!(_timer < 0))
            {
                return;
            }

            _elapsedTime++;
            InvokeGameLoopUpdate();
            _timer = 1000;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public void GraphicsDeviceCreated(GraphicsDevice graphicsDevice, Vector2 nativeSize)
        {
        }

        public void LoadTask()
        {
            _wordPlayGame.CreateNewTask(true);
            _isGameRunning = true;
            _elapsedTime = 0;
            _timer = 1000;

            RefreshText();
        }

        public void SwapObjects(int index1, int index2)
        {
            _wordPlayGame.SwapObjects(index1, index2);

            RefreshText();
        }

        public void CheckAnswer()
        {
            if (_wordPlayGame.CurrentTask == null || !_isGameRunning)
            {
                return;
            }

            bool[] result;
            bool correct = WordPlayGame.IsCorrect(out result);

            if (!correct)
            {
                _userInterface.Status.Foreground = Color.Red;
                _userInterface.Status.Text = "Wrong! Try again";

                for (int i = 0; i < WordPlayGame.CurrentTask.Length; i++)
                {
                    _userInterface.PuzzleFractions[i].Foreground = result[i] ? Color.Green : Color.Red;
                }

                return;
            }

            int scoreChange;
            bool gameOver = _demoGame.CorrectAnswerGiven(out scoreChange);
            RefreshText();

            _userInterface.Status.Foreground = Color.Green;
            _userInterface.Status.Text = "Correct! + " + scoreChange + " points";

            if (_demoGame.Combo > 1)
            {
                _userInterface.Status.Text += " Combo: " + (_demoGame.Combo);
            }

            if (gameOver)
            {
                EndGame();
            }

            _userInterface.Task.Text = _demoGame.AnswerCounter.ToString();
        }

        private void RefreshText()
        {
            _userInterface.Score.Text = _demoGame.Score.ToString();
            _userInterface.Task.Text = _demoGame.AnswerCounter.ToString();
            _userInterface.Time.Text = _elapsedTime.ToString();
            _userInterface.Status.Text = string.Empty;
            _userInterface.Status.Foreground = Color.White;

            if (_demoGame.CurrentTask == null)
            {
                return;
            }

            _userInterface.AddNewPuzzleFractions(_demoGame.CurrentTask.Length);

            for (int i = 0; i < _demoGame.CurrentTask.Length; i++)
            {
                _userInterface.PuzzleFractions[i].Text = _demoGame.CurrentTask[i].Item1;
                _userInterface.PuzzleFractions[i].Foreground = Color.White;
                _userInterface.PuzzleFractions[i].X = 50 + i * 100;
                _userInterface.PuzzleFractions[i].Y = 150;
            }
        }

        private void EndGame()
        {
            _isGameRunning = false;

            InvokePostGame();
        }

        private void InvokePreGame()
        {
            if (PreGame != null)
            {
                PreGame.Invoke(this, EventArgs.Empty);
            }
        }

        private void InvokeGameLoopUpdate()
        {
            if (GameLoopUpdate != null)
            {
                GameLoopUpdate.Invoke(this, new GameLoopUpdateEventArgs(_elapsedTime));
            }
        }

        private void InvokePostGame()
        {
            if (PostGame != null)
            {
                PostGame.Invoke(this, new PostGameEventArgs(_wordPlayGame.Score, _elapsedTime));
            }
        }
    }
}
