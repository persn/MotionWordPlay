namespace NTNU.MotionWordPlay.UserInterface.ViewModel
{
    using EmptyKeys.UserInterface.Mvvm;

    public class PuzzleFractionViewModel : BindableBase
    {
        private string _puzzleText;
        private int _top;
        private int _left;

        public string PuzzleText
        {
            get
            {
                return _puzzleText;
            }
            set
            {
                SetProperty(ref _puzzleText, value);
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
