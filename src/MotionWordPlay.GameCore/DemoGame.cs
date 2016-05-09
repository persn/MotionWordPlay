namespace NTNU.MotionWordPlay.GameCore
{
    using System;

    public class DemoGame
    {
        private const int ScoreIncrementAmount = 50;
        private const int ComboBonus = 25;
        private const int AnswersToFinish = 5;

        public Tuple<string, int>[] CurrentTask { get; private set; } //Contains a tuple with <"word", correctIndex>
        public int Score { get; private set; }
        public int AnswerCounter { get; private set; }
        public int Combo { get; private set; }
        private readonly TaskLoader _taskLoader;
        private readonly int _numPlayers;

        public DemoGame(int numPlayers)
        {
            _numPlayers = numPlayers;
            _taskLoader = new TaskLoader();
            Score = 0;
            Combo = 0;
            AnswerCounter = AnswersToFinish;
        }

        public void CreateNewTask(bool newGame = false)
        {
            if (newGame)
            {
                Score = 0;
                AnswerCounter = AnswersToFinish;
                Combo = 0;
            }
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
                    Combo = 0;
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
        public bool CorrectAnswerGiven(out int scoreChange)
        {
            scoreChange = ScoreIncrementAmount + ComboBonus * Combo;
            Score += scoreChange;
            Combo++;
            if (AnswerCounter == 0)
            {
                CurrentTask = new Tuple<string, int>[0];
                return true;
            }
            AnswerCounter--;
            CreateNewTask();

            return false;
        }

    }
}
