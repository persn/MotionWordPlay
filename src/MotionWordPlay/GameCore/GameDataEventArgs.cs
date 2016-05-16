namespace NTNU.MotionWordPlay.GameCore
{
    using System;

    public class GameDataEventArgs : EventArgs
    {
        public GameDataEventArgs(int elapsedTime, int score)
        {
            ElapsedTime = elapsedTime;
            Score = score;
        }

        public int ElapsedTime { get; private set; }

        public int Score { get; private set; }
    }
}
