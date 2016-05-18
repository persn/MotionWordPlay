namespace NTNU.MotionWordPlay.GameCore
{
    using System;
    using System.IO;
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

        private readonly WordPlayGame _wordPlayGame;
        private bool _isGameRunning;
        private double _timer;
        private int _elapsedTime;
        private bool _recentlyPerformedAction;
        private double _actionCooldownTimer;

        public WordPlayWrapper(int numPlayers)
        {
            _wordPlayGame = new WordPlayGame(
                numPlayers,
                string.Format(@"Content{0}WordPlays{0}{1}playertasks.txt", Path.DirectorySeparatorChar, numPlayers));
        }

        public WordPlayGame WordPlayGame
        {
            get
            {
                return _wordPlayGame;
            }
        }

        public void Initialize()
        {
            _timer = 1000;
            _elapsedTime = 0;
            _isGameRunning = false;
            _recentlyPerformedAction = false;
            _actionCooldownTimer = CooldownTime;
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
            _wordPlayGame.CreateNewTask(true);
            _isGameRunning = true;
            _elapsedTime = 0;
            _timer = 1000;

            InvokeNewGameLoaded();
        }

        public void CheckAnswer()
        {
            if (_wordPlayGame.CurrentTask == null || !_isGameRunning || _recentlyPerformedAction)
            {
                return;
            }

            _recentlyPerformedAction = true;
            bool[] result;
            bool correct = _wordPlayGame.IsCorrect(out result);

            if (!correct)
            {
                InvokeAnswersIncorrect(result);

                return;
            }

            int scoreChange;
            bool gameOver = _wordPlayGame.CorrectAnswerGiven(out scoreChange);

            if (gameOver)
            {
                _isGameRunning = false;

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
            _wordPlayGame.SwapObjects(index1, index2);

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
                _wordPlayGame.AnswerCounter,
                _wordPlayGame.Score,
                scoreIncrement,
                _wordPlayGame.Combo,
                _wordPlayGame.CurrentTask != null,
                _wordPlayGame.CurrentTask == null ? null : _wordPlayGame.CurrentTask.Select(item => item.Item1).ToArray(),
                result);
        }
    }
}
