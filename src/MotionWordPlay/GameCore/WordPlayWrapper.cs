namespace NTNU.MotionWordPlay.GameCore
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using UserInterface;
    using WordPlay;
    using Color = System.Drawing.Color;

    public class WordPlayWrapper : IGameLoop
    {
        public event EventHandler<GameDataEventArgs> PreGame;
        public event EventHandler<GameDataEventArgs> GameUpdate;
        public event EventHandler<GameDataEventArgs> PostGame;
        public event EventHandler<GameDataEventArgs> NewGameLoaded;
        public event EventHandler<GameDataEventArgs> AnswersChangedPlaces;

        private const double CooldownTime = 1000;

        private readonly IUserInterface _userInterface;
        private readonly DemoGame _demoGame;
        private bool _gameRunning;
        private double _timer;
        private int _elapsedTime;
        private bool _recentlyPerformedAction;
        private double _actionCooldownTimer;

        public WordPlayWrapper(int numPlayers, IUserInterface userInterface)
        {
            _userInterface = userInterface;
            _demoGame = new DemoGame(numPlayers);
        }

        public DemoGame WordPlayGame
        {
            get
            {
                return _demoGame;
            }
        }

        public void Initialize()
        {
            _timer = 1000;
            _elapsedTime = 0;
            _gameRunning = false;
            _recentlyPerformedAction = false;
            _actionCooldownTimer = CooldownTime;
        }

        public void Load(ContentManager contentManager)
        {
            InvokePreGame();
        }

        public void Update(GameTime gameTime)
        {
            if (!_gameRunning)
            {
                return;
            }

            _timer -= gameTime.ElapsedGameTime.Milliseconds;

            if (_timer < 0)
            {
                _elapsedTime++;
                _timer = 1000;
            }

            if (_recentlyPerformedAction)
            {
                _actionCooldownTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (_actionCooldownTimer <= 0)
                {
                    _recentlyPerformedAction = false;
                    _actionCooldownTimer = CooldownTime;
                }
            }

            InvokeGameUpdate();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public void GraphicsDeviceCreated(GraphicsDevice graphicsDevice, Vector2 nativeSize)
        {
        }

        public void LoadTask()
        {
            _demoGame.CreateNewTask(true);
            _gameRunning = true;
            _elapsedTime = 0;
            _timer = 1000;

            InvokeNewGameLoaded();
        }

        public void CheckAnswer()
        {
            if (_demoGame.CurrentTask == null || !_gameRunning || _recentlyPerformedAction)
            {
                return;
            }

            _recentlyPerformedAction = true;
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

        public void SwapObjects(int index1, int index2)
        {
            if (_recentlyPerformedAction)
            {
                return;
            }

            _recentlyPerformedAction = true;
            _demoGame.SwapObjects(index1, index2);

            InvokeAnswersChangedPlaces();
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
            _gameRunning = false;

            InvokePostGame();
        }

        private void InvokePreGame()
        {
            if (PreGame != null)
            {
                PreGame.Invoke(this, GenerateEventArgs());
            }
        }

        private void InvokeGameUpdate()
        {
            if (GameUpdate != null)
            {
                GameUpdate.Invoke(this, GenerateEventArgs());
            }
        }

        private void InvokePostGame()
        {
            if (PostGame != null)
            {
                PostGame.Invoke(this, GenerateEventArgs());
            }
        }

        private void InvokeNewGameLoaded()
        {
            if (NewGameLoaded != null)
            {
                NewGameLoaded.Invoke(this, GenerateEventArgs());
            }
        }

        private void InvokeAnswersChangedPlaces()
        {
            if (AnswersChangedPlaces != null)
            {
                AnswersChangedPlaces.Invoke(this, GenerateEventArgs());
            }
        }

        private GameDataEventArgs GenerateEventArgs()
        {
            return new GameDataEventArgs(
                _elapsedTime,
                _demoGame.AnswerCounter,
                _demoGame.Score,
                _demoGame.CurrentTask != null,
                _demoGame.CurrentTask == null ? null : _demoGame.CurrentTask.Select(item => item.Item1).ToArray());
        }
    }
}
