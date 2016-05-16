namespace NTNU.MotionWordPlay.GameCore
{
    using System;

    public class GameDataEventArgs : EventArgs
    {
        public GameDataEventArgs(int elapsedTime, int answerCounter, int score, bool isGameLoaded)
        {
            ElapsedTime = elapsedTime;
            AnswerCounter = answerCounter;
            Score = score;
            IsGameLoaded = isGameLoaded;
        }

        public int ElapsedTime { get; private set; }

        public int AnswerCounter { get; private set; }

        public int Score { get; private set; }

        public bool IsGameLoaded { get; private set; }
    }
}
