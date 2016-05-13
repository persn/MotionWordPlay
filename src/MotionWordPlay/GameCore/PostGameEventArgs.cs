namespace NTNU.MotionWordPlay.GameCore
{
    using System;

    public class PostGameEventArgs : EventArgs
    {
        public PostGameEventArgs(int score, float elapsedTime)
        {
            Score = score;
            ElapsedTime = elapsedTime;
        }

        public int Score
        {
            get; private set;
        }

        public float ElapsedTime
        {
            get; private set;
        }
    }
}
