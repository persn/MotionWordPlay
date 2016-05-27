using System.Collections.Generic;
using System.Linq;

namespace NTNU.WordPlay
{
    using System;
    using System.IO;

    public class WordPlayGame
    {
        private const int ScoreIncrementAmount = 50;
        private const int ComboBonus = 25;
        private const int AnswersToFinish = 5;
        
        private readonly Random _random;
        private readonly string _file;

        private List<string> _currentGame;

        public WordPlayGame(int playerCount, string file)
        {
            _random = new Random();

            PlayerCount = playerCount;
            Score = 0;
            Combo = 0;
            _file = file;
        }

        // Contains a tuple with <"word", correctIndex>
        public Tuple<string, int>[] CurrentTask
        {
            get; private set;
        }

        public int Score
        {
            get; private set;
        }

        public int AnswerCounter
        {
            get; private set;
        }

        public int Combo
        {
            get; private set;
        }

        public int PlayerCount
        {
            get; private set;
        }

        public void CreateNewTask(bool newGame = false)
        {
            if (newGame)
            {
                Score = 0;
                _currentGame = File.ReadAllLines(_file).ToList();
                if (AnswersToFinish <= _currentGame.Count)
                {
                    AnswerCounter = AnswersToFinish;
                }
                else
                {
                    AnswerCounter = _currentGame.Count-1;
                }
                Combo = 0;
            }

            int taskIndex = _random.Next(_currentGame.Count);
            SplitSentence(_currentGame[taskIndex]);
            _currentGame.RemoveAt(taskIndex);
            ScrambleWordOrder();
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
            if (CurrentTask == null || index1 > CurrentTask.Length - 1 || index2 > CurrentTask.Length - 1)
            {
                return;
            }

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
            int i = CurrentTask.Length;

            while (i > 1)
            {
                int j = _random.Next(i--);
                Tuple<string, int> temp = CurrentTask[i];
                CurrentTask[i] = CurrentTask[j];
                CurrentTask[j] = temp;
            }
        }
    }
}
