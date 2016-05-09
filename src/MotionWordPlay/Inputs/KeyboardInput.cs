namespace NTNU.MotionWordPlay.Inputs
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class KeyboardInput : IGameLoop
    {
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        private static readonly Keys[] ValidKeys = { Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.F4, Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.Q, Keys.W };

        private KeyboardState _previousState;
        private KeyboardState _currentState;

        public void Initialize()
        {
        }

        public void Load(ContentManager contentManager)
        {
        }

        public void Update(GameTime gameTime)
        {
            _previousState = _currentState;
            _currentState = Keyboard.GetState();

            foreach (Keys validKey in ValidKeys)
            {
                if (IsKeyPressed(validKey))
                {
                    KeyPressed?.Invoke(this, new KeyPressedEventArgs(validKey));
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        public void GraphicsDeviceCreated(GraphicsDevice graphicsDevice, Vector2 nativeSize)
        {
        }

        private bool IsKeyPressed(Keys key)
        {
            return _currentState.IsKeyDown(key) && !_previousState.IsKeyDown(key);
        }
    }
}
