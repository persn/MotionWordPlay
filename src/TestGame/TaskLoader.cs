namespace NTNU.TestGame
{
    using System;

    public class TaskLoader
    {
        private string[] _3PlayerTasks;
        private string[] _4PlayerTasks;
        private string[] _5PlayerTasks;
        private string[] _6PlayerTasks;
        private readonly Random _random = new Random();

        public TaskLoader()
        {
            ReadTasksFromFiles();
        }

        private void ReadTasksFromFiles()
        {
            //TODO: Read from actual file(s)
            _3PlayerTasks = new[] {"Word0 word1 word2."};
            _4PlayerTasks = new[] {"Word0 word1 word2 word3."};
            _5PlayerTasks = new[] {"Word0 word1 word2 word3 word4."};
            _6PlayerTasks = new[] {"Word0 word1 word2 word3 word4 word5."};
        }

        public string LoadTask(int numPlayers)
        {
            switch (numPlayers)
            {
                case 3:
                    return _3PlayerTasks[_random.Next(_3PlayerTasks.Length)];
                case 4:
                    return _4PlayerTasks[_random.Next(_4PlayerTasks.Length)];
                case 5:
                    return _5PlayerTasks[_random.Next(_5PlayerTasks.Length)];
                case 6:
                    return _6PlayerTasks[_random.Next(_6PlayerTasks.Length)];
                default:
                    throw new NotSupportedException("Invalid number of players");
            }
            
        }
    }
}
