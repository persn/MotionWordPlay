namespace NTNU.TestGame
{
    using System;
    using System.Collections.Generic;

    public class DemoGame
    {
        private string _dummyTestString = "Word0 word1 word2 word3 word4 word5."; //TODO: Delete
        public Tuple<string, int>[] CurrentTask { get; private set; } //Contains a tuple with <"word", correctIndex>

        public void testNewTask() //TODO: Delete
        {
            SplitSentence(_dummyTestString);
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

        public bool[] IsCorrect()
        {
            bool[] result = new bool[CurrentTask.Length];
            for (int i = 0; i < CurrentTask.Length; i++)
            {
                result[i] = CurrentTask[i].Item2 == i;
            }
            return result;
        }

        public void SwapObjects(int index1, int index2)
        {
            Tuple<string, int> temp = CurrentTask[index1];
            CurrentTask[index1] = CurrentTask[index2];
            CurrentTask[index2] = temp;
        }

    }
}
