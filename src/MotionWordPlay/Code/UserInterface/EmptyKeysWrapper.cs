namespace NTNU.MotionWordPlay.UserInterface
{
    using EmptyKeys.UserInterface;
    using EmptyKeys.UserInterface.Generated;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using ViewModel;

    public class EmptyKeysWrapper : IUserInterface
    {
        private RootView _rootView;
        private RootViewModel _rootViewModel;

        public string Time
        {
            get
            {
                return _rootViewModel.Time;
            }
            set
            {
                _rootViewModel.Time = value;
            }
        }

        public string Task
        {
            get
            {
                return _rootViewModel.Task;
            }
            set
            {
                _rootViewModel.Task = value;
            }
        }

        public string Score
        {
            get
            {
                return _rootViewModel.Score;
            }
            set
            {
                _rootViewModel.Score = value;
            }
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
            _rootViewModel.PuzzleFractions[index].Top = x;
            _rootViewModel.PuzzleFractions[index].Left = y;
        }

        public void UpdatePuzzleFraction(int index, string text, int x, int y)
        {
            _rootViewModel.PuzzleFractions[index].PuzzleText = text;
            _rootViewModel.PuzzleFractions[index].Top = x;
            _rootViewModel.PuzzleFractions[index].Left = y;
        }

        public void ResetUI()
        {
            Time = string.Empty;
            Task = string.Empty;
            Score = string.Empty;
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
    }
}
