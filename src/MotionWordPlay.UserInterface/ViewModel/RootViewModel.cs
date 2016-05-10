namespace NTNU.MotionWordPlay.UserInterface.ViewModel
{
    using EmptyKeys.UserInterface.Media;
    using EmptyKeys.UserInterface.Mvvm;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class RootViewModel : ViewModelBase
    {
        private string _time;
        private Brush _timeBackground;
        private Brush _timeForeground;

        private string _task;
        private Brush _taskBackground;
        private Brush _taskForeground;

        private string _score;
        private Brush _scoreBackground;
        private Brush _scoreForeground;

        private IList<PuzzleFractionViewModel> _puzzleFractions;

        public RootViewModel()
        {
            Time = string.Empty;
            TimeBackground = new SolidColorBrush(ColorW.TransparentBlack);
            TimeForeground = new SolidColorBrush(ColorW.White);

            Task = string.Empty;
            TaskBackground = new SolidColorBrush(ColorW.TransparentBlack);
            TaskForeground = new SolidColorBrush(ColorW.White);

            Score = string.Empty;
            ScoreBackground = new SolidColorBrush(ColorW.TransparentBlack);
            ScoreForeground = new SolidColorBrush(ColorW.White);

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

        public Brush TimeBackground
        {
            get
            {
                return _timeBackground;
            }
            set
            {
                SetProperty(ref _timeBackground, value);
            }
        }

        public Brush TimeForeground
        {
            get
            {
                return _timeForeground;
            }
            set
            {
                SetProperty(ref _timeForeground, value);
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

        public Brush TaskBackground
        {
            get
            {
                return _taskBackground;
            }
            set
            {
                SetProperty(ref _taskBackground, value);
            }
        }

        public Brush TaskForeground
        {
            get
            {
                return _taskForeground;
            }
            set
            {
                SetProperty(ref _taskForeground, value);
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

        public Brush ScoreBackground
        {
            get
            {
                return _scoreBackground;
            }
            set
            {
                SetProperty(ref _scoreBackground, value);
            }
        }

        public Brush ScoreForeground
        {
            get
            {
                return _scoreForeground;
            }
            set
            {
                SetProperty(ref _scoreForeground, value);
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
