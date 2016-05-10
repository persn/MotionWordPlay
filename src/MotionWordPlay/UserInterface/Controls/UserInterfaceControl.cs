namespace NTNU.MotionWordPlay.UserInterface.Controls
{
    using System.Drawing;

    public abstract class UserInterfaceControl
    {
        public abstract string Text
        {
            get; set;
        }

        public abstract Color Background
        {
            get; set;
        }

        public abstract Color Foreground
        {
            get; set;
        }
    }
}
