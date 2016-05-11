namespace NTNU.MotionWordPlay.UserInterface
{
    using EmptyKeys.UserInterface;
    using EmptyKeys.UserInterface.Data;
    using EmptyKeys.UserInterface.Generated;
    using EmptyKeys.UserInterface.Media;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Controls;
    using ViewModel;
    using Color = System.Drawing.Color;

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

        public TextLine Status
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
            _rootViewModel.PuzzleFractions[index].Text = text;
        }

        public void UpdatePuzzleFraction(int index, Color background, Color foreground)
        {
            _rootViewModel.PuzzleFractions[index].Background = GetBrush(background);
            _rootViewModel.PuzzleFractions[index].Foreground = GetBrush(foreground);
        }

        public void UpdatePuzzleFraction(int index, bool isVisible)
        {
            _rootViewModel.PuzzleFractions[index].Visibility = GetVisibility(isVisible);
        }

        public void UpdatePuzzleFraction(int index, int x, int y)
        {
            _rootViewModel.PuzzleFractions[index].Left = x;
            _rootViewModel.PuzzleFractions[index].Top = y;
        }

        public void UpdatePuzzleFraction(int index, string text, int x, int y)
        {
            _rootViewModel.PuzzleFractions[index].Text = text;
            _rootViewModel.PuzzleFractions[index].Left = x;
            _rootViewModel.PuzzleFractions[index].Top = y;
        }

        public void ResetUI()
        {
            Time = new TextBlockWrapper(_rootViewModel.Time);
            Task = new TextBlockWrapper(_rootViewModel.Task);
            Score = new TextBlockWrapper(_rootViewModel.Score);
            Status = new TextBlockWrapper(_rootViewModel.Status);

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

            Time = new TextBlockWrapper(_rootViewModel.Time);
            Task = new TextBlockWrapper(_rootViewModel.Task);
            Score = new TextBlockWrapper(_rootViewModel.Score);
            Status = new TextBlockWrapper(_rootViewModel.Status);
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

        private static Color GetColor(Brush brush)
        {
            ColorW color = ((SolidColorBrush)brush).Color;
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private static Brush GetBrush(Color color)
        {
            return new SolidColorBrush(new ColorW(color.R, color.G, color.B, color.A));
        }

        private static bool GetBool(Visibility visibility)
        {
            BooleanToVisibilityConverter converter = new BooleanToVisibilityConverter();
            return (bool)converter.ConvertBack(visibility, null, null, null);
        }

        private static Visibility GetVisibility(bool boolean)
        {
            BooleanToVisibilityConverter converter = new BooleanToVisibilityConverter();
            return (Visibility)converter.Convert(boolean, null, null, null);
        }

        private class TextBlockWrapper : TextLine
        {
            private readonly TextBlockViewModel _textBlockViewModel;

            public TextBlockWrapper(TextBlockViewModel textBlockViewModel)
            {
                _textBlockViewModel = textBlockViewModel;
            }

            public override string Text
            {
                get
                {
                    return _textBlockViewModel.Text;
                }
                set
                {
                    _textBlockViewModel.Text = value;
                }
            }

            public override Color Background
            {
                get
                {
                    return GetColor(_textBlockViewModel.Background);
                }
                set
                {
                    _textBlockViewModel.Background = GetBrush(value);
                }
            }

            public override Color Foreground
            {
                get
                {
                    return GetColor(_textBlockViewModel.Foreground);
                }
                set
                {
                    _textBlockViewModel.Foreground = GetBrush(value);
                }
            }

            public override bool Visible
            {
                get
                {
                    return GetBool(_textBlockViewModel.Visibility);
                }
                set
                {
                    _textBlockViewModel.Visibility = GetVisibility(value);
                }
            }
        }
    }
}
