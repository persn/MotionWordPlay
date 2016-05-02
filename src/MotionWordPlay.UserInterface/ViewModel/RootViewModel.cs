namespace NTNU.MotionWordPlay.UserInterface.ViewModel
{
    using EmptyKeys.UserInterface.Mvvm;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class RootViewModel : ViewModelBase
    {
        private string _time;
        private string _task;
        private string _score;
        private IList<PuzzleFractionViewModel> _puzzleFractions;

        public RootViewModel()
        {
            _puzzleFractions = new ObservableCollection<PuzzleFractionViewModel>();
        }

        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                SetProperty(ref _time, value);
            }
        }

        public string Task
        {
            get
            {
                return _task;
            }
            set
            {
                SetProperty(ref _task, value);
            }
        }

        public string Score
        {
            get
            {
                return _score;
            }
            set
            {
                SetProperty(ref _score, value);
            }
        }

        public IList<PuzzleFractionViewModel> PuzzleFractions
        {
            get
            {
                return _puzzleFractions;
            }
            set
            {
                SetProperty(ref _puzzleFractions, value);
            }
        }
    }
}
