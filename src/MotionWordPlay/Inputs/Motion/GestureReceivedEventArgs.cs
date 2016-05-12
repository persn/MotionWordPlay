namespace NTNU.MotionWordPlay.Inputs.Motion
{
    using MotionControlWrapper;

    public class GestureReceivedEventArgs
    {
        public GestureReceivedEventArgs(GestureResults receivedGestures)
        {
            Gestures = receivedGestures;
        }

        public GestureResults Gestures { get; private set; }
    }
}
