namespace NTNU.MotionWordPlay.UserInterface.ViewModel
{
    using EmptyKeys.UserInterface.Mvvm;

    public class PuzzleFractionViewModel : BindableBase
    {
        private TextBlockViewModel _textBlock;
        private int _left;
        private int _top;

        public PuzzleFractionViewModel()
        {
            TextBlock = new TextBlockViewModel();
        }

        public TextBlockViewModel TextBlock
        {
            get
            {
                return _textBlock;
            }
            set
            {
                SetProperty(ref _textBlock, value);
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
    }
}
