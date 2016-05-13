namespace NTNU.MotionWordPlay.GameCore
{
    using System;

    public class GameLoopUpdateEventArgs : EventArgs
    {
        public GameLoopUpdateEventArgs(float elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }

        public float ElapsedTime
        {
            get; private set;
        }
    }
}
