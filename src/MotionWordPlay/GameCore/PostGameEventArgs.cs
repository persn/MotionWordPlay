namespace NTNU.MotionWordPlay.GameCore
{
    using System;

    public class PostGameEventArgs : EventArgs
    {
        public PostGameEventArgs(int elapsedTime, int score)
        {
            ElapsedTime = elapsedTime;
            Score = score;
        }

        public int ElapsedTime { get; private set; }

        public int Score { get; private set; }
    }
}
