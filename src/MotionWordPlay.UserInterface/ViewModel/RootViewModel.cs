namespace NTNU.MotionWordPlay.UserInterface.ViewModel
{
    using EmptyKeys.UserInterface.Mvvm;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class RootViewModel : ViewModelBase
    {
        private TextBlockViewModel _time;
        private TextBlockViewModel _task;
        private TextBlockViewModel _score;
        private TextBlockViewModel _status;
        private IList<PuzzleFractionViewModel> _puzzleFractions;

        public RootViewModel()
        {
            Time = new TextBlockViewModel();
            Task = new TextBlockViewModel();
            Score = new TextBlockViewModel();
            Status = new TextBlockViewModel();
            PuzzleFractions = new ObservableCollection<PuzzleFractionViewModel>();
        }

        public TextBlockViewModel Time
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

        public TextBlockViewModel Task
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

        public TextBlockViewModel Score
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

        public TextBlockViewModel Status
        {
            get
            {
                return _status;
            }
            set
            {
                SetProperty(ref _status, value);
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
