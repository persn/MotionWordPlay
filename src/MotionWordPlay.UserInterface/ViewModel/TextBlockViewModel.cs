namespace NTNU.MotionWordPlay.UserInterface.ViewModel
{
    using EmptyKeys.UserInterface;
    using EmptyKeys.UserInterface.Media;
    using EmptyKeys.UserInterface.Mvvm;

    public class TextBlockViewModel : BindableBase
    {
        private string _text;
        private Brush _background;
        private Brush _foreground;
        private Visibility _visibility;

        public TextBlockViewModel()
        {
            Background = new SolidColorBrush(ColorW.TransparentBlack);
            Foreground = new SolidColorBrush(ColorW.White);
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

        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                SetProperty(ref _visibility, value);
            }
        }
    }
}
