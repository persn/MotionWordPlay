namespace NTNU.MotionWordPlay.Inputs
{
    using Microsoft.Xna.Framework.Input;

    public class KeyPressedEventArgs
    {
        public KeyPressedEventArgs(Keys pressedKey)
        {
            PressedKey = pressedKey;
        }

        public Keys PressedKey { get; private set; }
    }
}
