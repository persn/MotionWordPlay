namespace NTNU.MotionWordPlay.UserInterface.ViewModel
{
    using EmptyKeys.UserInterface.Media;
    using EmptyKeys.UserInterface.Mvvm;

    public class PuzzleFractionViewModel : BindableBase
    {
        private string _text;
        private Brush _background;
        private Brush _foreground;
        private int _top;
        private int _left;

        public PuzzleFractionViewModel()
        {
            _text = string.Empty;
            _background = new SolidColorBrush(ColorW.TransparentBlack);
            _foreground = new SolidColorBrush(ColorW.White);
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetProperty(ref _text, value);
            }
        }

        public Brush Background
        {
            get
            {
                return _background;
            }
            set
            {
                SetProperty(ref _background, value);
            }
        }

        public Brush Foreground
        {
            get
            {
                return _foreground;
            }
            set
            {
                SetProperty(ref _foreground, value);
            }
        }

        public int Top
        {
            get
            {
                return _top;
            }
            set
            {
                SetProperty(ref _top, value);
            }
        }

        public int Left
        {
            get
            {
                return _left;
            }
            set
            {
                SetProperty(ref _left, value);
            }
        }
    }
}
