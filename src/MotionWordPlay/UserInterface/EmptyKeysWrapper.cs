namespace NTNU.MotionWordPlay.UserInterface
{
    using EmptyKeys.UserInterface;
    using EmptyKeys.UserInterface.Generated;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Controls;
    using ViewModel;

    public class EmptyKeysWrapper : IUserInterface
    {
        private RootView _rootView;
        private RootViewModel _rootViewModel;

        public TextLine Time
        {
            get; set;
        }

        public TextLine Task
        {
            get; set;
        }

        public TextLine Score
        {
            get; set;
        }

        public void AddNewPuzzleFractions(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                _rootViewModel.PuzzleFractions.Add(new PuzzleFractionViewModel());
            }
        }

        public void UpdatePuzzleFraction(int index, string text)
        {
            _rootViewModel.PuzzleFractions[index].PuzzleText = text;
        }

        public void UpdatePuzzleFraction(int index, int x, int y)
        {
            _rootViewModel.PuzzleFractions[index].Left = x;
            _rootViewModel.PuzzleFractions[index].Top = y;
        }

        public void UpdatePuzzleFraction(int index, string text, int x, int y)
        {
            _rootViewModel.PuzzleFractions[index].PuzzleText = text;
            _rootViewModel.PuzzleFractions[index].Left = x;
            _rootViewModel.PuzzleFractions[index].Top = y;
        }

        public void ResetUI()
        {
            Time.Text = string.Empty;
            Task.Text = string.Empty;
            Score.Text = string.Empty;
            _rootViewModel.PuzzleFractions.Clear();
        }

        public void Initialize()
        {
        }

        public void Load(ContentManager contentManager)
        {
            SpriteFont font = contentManager.Load<SpriteFont>("Segoe_UI_15_Regular");
            FontManager.DefaultFont = Engine.Instance.Renderer.CreateFont(font);

            _rootView = new RootView();
            _rootViewModel = new RootViewModel();

            _rootView.DataContext = _rootViewModel;

            FontManager.Instance.LoadFonts(contentManager);

            Time = new TimeTextLine(this);
            Task = new TaskTextLine(this);
            Score = new ScoreTextLine(this);
        }

        public void Update(GameTime gameTime)
        {
            _rootView.UpdateInput(gameTime.TotalGameTime.Milliseconds);
            _rootView.UpdateLayout(gameTime.TotalGameTime.Milliseconds);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _rootView.Draw(gameTime.TotalGameTime.Milliseconds);
        }

        public void GraphicsDeviceCreated(GraphicsDevice graphicsDevice, Vector2 nativeSize)
        {
            // Unused variable must exist because of magic code happening in "new MonoGameEngine"
            Engine engine = new MonoGameEngine(
                graphicsDevice,
                (int)nativeSize.X,
                (int)nativeSize.Y);
        }

        private class TimeTextLine : TextLine
        {
            private readonly EmptyKeysWrapper _emptyKeysWrapper;

            public TimeTextLine(EmptyKeysWrapper emptyKeysWrapper)
            {
                _emptyKeysWrapper = emptyKeysWrapper;
            }

            public override string Text
            {
                get
                {
                    return _emptyKeysWrapper._rootViewModel.Time;
                }
                set
                {
                    _emptyKeysWrapper._rootViewModel.Time = value;
                }
            }
        }

        private class TaskTextLine : TextLine
        {
            private readonly EmptyKeysWrapper _emptyKeysWrapper;

            public TaskTextLine(EmptyKeysWrapper emptyKeysWrapper)
            {
                _emptyKeysWrapper = emptyKeysWrapper;
            }

            public override string Text
            {
                get
                {
                    return _emptyKeysWrapper._rootViewModel.Task;
                }
                set
                {
                    _emptyKeysWrapper._rootViewModel.Task = value;
                }
            }
        }

        private class ScoreTextLine : TextLine
        {
            private readonly EmptyKeysWrapper _emptyKeysWrapper;

            public ScoreTextLine(EmptyKeysWrapper emptyKeysWrapper)
            {
                _emptyKeysWrapper = emptyKeysWrapper;
            }

            public override string Text
            {
                get
                {
                    return _emptyKeysWrapper._rootViewModel.Score;
                }
                set
                {
                    _emptyKeysWrapper._rootViewModel.Score = value;
                }
            }
        }
    }
}
