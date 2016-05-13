namespace NTNU.MotionWordPlay
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using UserInterface;
    using WordPlay;
    using Color = System.Drawing.Color;

    class DemoGameLogic : IGameLoop
    {
        public int NumPlayers
        {
            get { return _demoGame.NumPlayers; }
        }

        private readonly IUserInterface _userInterface;
        private readonly DemoGame _demoGame;
        private bool _gameRunning;
        private double _timer;
        private int _elapsedTime;

        public DemoGameLogic(int numPlayers, IUserInterface userInterface)
        {
            _userInterface = userInterface;
            _demoGame = new DemoGame(numPlayers);
        }

        public void Initialize()
        {
            _timer = 1000;
            _elapsedTime = 0;
        }

        public void Load(ContentManager contentManager)
        {
            _gameRunning = false;
            _userInterface.Status.Text = "Do stuff to start game";
        }

        public void Update(GameTime gameTime)
        {
            if (!_gameRunning)
            {
                return;
            }
            _timer -= gameTime.ElapsedGameTime.Milliseconds;
            if (!(_timer < 0))
            {
                return;
            }
            _elapsedTime++;
            _userInterface.Time.Text = _elapsedTime.ToString();
            _timer = 1000;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            throw new System.NotImplementedException();
        }

        public void GraphicsDeviceCreated(GraphicsDevice graphicsDevice, Vector2 nativeSize)
        {
            throw new System.NotImplementedException();
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

        public void LoadTask()
        {
            _demoGame.CreateNewTask(true);
            _gameRunning = true;
            _elapsedTime = 0;
            _timer = 1000;
            RefreshText();
        }

        public void CheckAnswer()
        {
            if (_demoGame.CurrentTask == null || !_gameRunning)
            {
                return;
            }
            bool[] result;
            bool correct = _demoGame.IsCorrect(out result);
            if (!correct)
            {
                _userInterface.Status.Foreground = Color.Red;
                _userInterface.Status.Text = "Wrong! Try again";
                for (int i = 0; i < _demoGame.CurrentTask.Length; i++)
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

        private void EndGame()
        {
            _gameRunning = false;
            _userInterface.ResetUI();
            _userInterface.Score.Text = _demoGame.Score.ToString();
            _userInterface.Time.Text = _elapsedTime.ToString();
            _userInterface.Status.Text = "Game Over\nFinal Score: " + _demoGame.Score;
        }

        public void SwapObjects(int index1, int index2)
        {
            _demoGame.SwapObjects(index1, index2);
            RefreshText();
        }
    }
}
