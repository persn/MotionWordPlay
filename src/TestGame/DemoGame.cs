namespace NTNU.TestGame
{
    using System;

    public class DemoGame
    {
        private const int ScoreIncrementAmount = 50;
        private const int ComboBonus = 25;

        public Tuple<string, int>[] CurrentTask { get; private set; } //Contains a tuple with <"word", correctIndex>
        public int Score { get; private set; }
        public int AnswerCounter { get; private set; }
        private readonly TaskLoader _taskLoader;
        private readonly int _numPlayers;
        private int _combo;

        public DemoGame(int numPlayers, int numAnswers)
        {
            _numPlayers = numPlayers;
            _taskLoader = new TaskLoader();
            Score = 0;
            _combo = 0;
            AnswerCounter = numAnswers;
        }

        public void CreateNewTask()
        {
            SplitSentence(_taskLoader.LoadTask(_numPlayers));
            ScrambleWordOrder();
        }

        private void SplitSentence(string input)
        {
            string[] segments = input.Split(' ');
            CurrentTask = new Tuple<string, int>[segments.Length];
            for (int i = 0; i < segments.Length; i++)
            {
                CurrentTask[i] = new Tuple<string, int>(segments[i], i);
            }
        }

        private void ScrambleWordOrder()
        {
            Random rng = new Random();
            int i = CurrentTask.Length;
            while (i > 1)
            {
                int j = rng.Next(i--);
                Tuple<string, int> temp = CurrentTask[i];
                CurrentTask[i] = CurrentTask[j];
                CurrentTask[j] = temp;
            }
        }

        public bool IsCorrect(out bool[] result)
        {
            bool returnValue = true;
            result = new bool[CurrentTask.Length];
            for (int i = 0; i < CurrentTask.Length; i++)
            {
                result[i] = CurrentTask[i].Item2 == i;
                if (!result[i])
                {
                    returnValue = false;
                    _combo = 0;
                }
            }
            return returnValue;
        }

        public void SwapObjects(int index1, int index2)
        {
            Tuple<string, int> temp = CurrentTask[index1];
            CurrentTask[index1] = CurrentTask[index2];
            CurrentTask[index2] = temp;
        }

        /// <summary>
        /// Updates score and checks if it should create new task or end the game. 
        /// </summary>
        /// <returns>True if game is over, false if there is more tasks left</returns>
        public bool CorrectAnswerGiven()
        {
            Score += ScoreIncrementAmount + ComboBonus * _combo;
            _combo++;
            if (AnswerCounter == 0)
            {
                return true;
            }
            AnswerCounter--;
            CreateNewTask();

            return false;
        }

    }
}
