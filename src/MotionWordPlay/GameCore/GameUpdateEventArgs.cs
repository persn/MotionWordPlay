namespace NTNU.MotionWordPlay.GameCore
{
    using System;

    public class GameUpdateEventArgs : EventArgs
    {
        public GameUpdateEventArgs(int elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }

        public int ElapsedTime { get; private set; }
    }
}
