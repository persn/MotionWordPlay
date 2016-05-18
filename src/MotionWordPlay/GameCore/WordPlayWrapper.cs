namespace NTNU.MotionWordPlay.GameCore
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using WordPlay;

    public class WordPlayWrapper : IGameLoop
    {
        public event EventHandler<GameDataEventArgs> PreGame;
        public event EventHandler<GameDataEventArgs> GameUpdate;
        public event EventHandler<GameDataEventArgs> PostGame;
        public event EventHandler<GameDataEventArgs> NewGameLoaded;
        public event EventHandler<GameDataEventArgs> AnswersChangedPlaces;
        public event EventHandler<GameDataEventArgs> AnswersIncorrect;
        public event EventHandler<GameDataEventArgs> AnswersCorrect;

        private const double CooldownTime = 1000;

        private readonly DemoGame _demoGame;
        private bool _gameRunning;
        private double _timer;
        private int _elapsedTime;
        private bool _recentlyPerformedAction;
        private double _actionCooldownTimer;

        public WordPlayWrapper(int numPlayers)
        {
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
                InvokeAnswersIncorrect(result);

                return;
            }

            int scoreChange;
            bool gameOver = _demoGame.CorrectAnswerGiven(out scoreChange);

            if (gameOver)
            {
                _gameRunning = false;

                InvokePostGame();
            }
            else
            {
                InvokeAnswersCorrect(scoreChange);
            }
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

        private void InvokeAnswersIncorrect(bool[] result)
        {
            if (AnswersIncorrect != null)
            {
                AnswersIncorrect.Invoke(this, GenerateEventArgs(result: result));
            }
        }

        private void InvokeAnswersCorrect(int scoreIncrement)
        {
            if (AnswersCorrect != null)
            {
                AnswersCorrect.Invoke(this, GenerateEventArgs(scoreIncrement));
            }
        }

        private GameDataEventArgs GenerateEventArgs(int scoreIncrement = 0, bool[] result = null)
        {
            return new GameDataEventArgs(
                _elapsedTime,
                _demoGame.AnswerCounter,
                _demoGame.Score,
                scoreIncrement,
                _demoGame.Combo,
                _demoGame.CurrentTask != null,
                _demoGame.CurrentTask == null ? null : _demoGame.CurrentTask.Select(item => item.Item1).ToArray(),
                result);
        }
    }
}
