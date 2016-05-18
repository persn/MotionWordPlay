namespace NTNU.MotionWordPlay.GameCore
{
    using System;

    public class GameDataEventArgs : EventArgs
    {
        public GameDataEventArgs(
            int elapsedTime,
            int answerCounter,
            int score,
            int scoreIncrement,
            int combo,
            bool isGameLoaded,
            string[] wordFractions,
            bool[] result)
        {
            ElapsedTime = elapsedTime;
            AnswerCounter = answerCounter;
            Score = score;
            ScoreIncrement = scoreIncrement;
            Combo = combo;
            IsGameLoaded = isGameLoaded;
            WordFractions = wordFractions;
            Result = result;
        }

        public int ElapsedTime { get; private set; }

        public int AnswerCounter { get; private set; }

        public int Score { get; private set; }

        public int ScoreIncrement { get; private set; }

        public int Combo { get; private set; }

        public bool IsGameLoaded { get; private set; }

        public string[] WordFractions { get; set; }

        public bool[] Result { get; private set; }
    }
}
