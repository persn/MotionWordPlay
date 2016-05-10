namespace NTNU.MotionWordPlay.Inputs
{
    using System.Collections.Generic;
    using NTNU.MotionControlWrapper;

    public class GestureReceivedEventArgs
    {
        public GestureReceivedEventArgs(GestureResults receivedGestures)
        {
            Gestures = receivedGestures;
        }

        public GestureResults Gestures { get; private set; }
    }
}
