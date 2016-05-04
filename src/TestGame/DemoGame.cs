namespace NTNU.TestGame
{
    using System;

    public class DemoGame
    {
        public Tuple<string, int>[] CurrentTask { get; private set; } //Contains a tuple with <"word", correctIndex>
        private TaskLoader _taskLoader;

        public DemoGame()
        {
            _taskLoader = new TaskLoader();
        }

        public void CreateNewTask(int numPlayers)
        {
            SplitSentence(_taskLoader.LoadTask(numPlayers));
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
