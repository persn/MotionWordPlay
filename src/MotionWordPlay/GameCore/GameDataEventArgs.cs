namespace NTNU.MotionWordPlay.GameCore
{
    using System;

    public class GameDataEventArgs : EventArgs
    {
        public GameDataEventArgs(int elapsedTime, int answerCounter, int score, bool isGameLoaded, string[] wordFractions)
        {
            ElapsedTime = elapsedTime;
            AnswerCounter = answerCounter;
            Score = score;
            IsGameLoaded = isGameLoaded;
            WordFractions = wordFractions;
        }

        public int ElapsedTime { get; private set; }

        public int AnswerCounter { get; private set; }

        public int Score { get; private set; }

        public bool IsGameLoaded { get; private set; }

        public string[] WordFractions { get; set; }
    }
}
