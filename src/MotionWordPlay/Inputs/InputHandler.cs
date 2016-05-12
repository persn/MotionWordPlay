namespace NTNU.MotionWordPlay.Inputs
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Keyboard;
    using Motion;

    public class InputHandler : IDisposable, IGameLoop
    {
        public InputHandler()
        {
            KeyboardInput = new KeyboardInput();
            MotionController = new MotionController();
        }

        public KeyboardInput KeyboardInput
        {
            get; private set;
        }

        public MotionController MotionController
        {
            get; private set;
        }

        public void Dispose()
        {
            if (MotionController == null)
            {
                return;
            }

            MotionController.Dispose();
            MotionController = null;
        }

        public void Initialize()
        {
            KeyboardInput.Initialize();
            MotionController.Initialize();
        }

        public void Load(ContentManager contentManager)
        {
            KeyboardInput.Load(contentManager);
            MotionController.Load(contentManager);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardInput.Update(gameTime);
            MotionController.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            KeyboardInput.Draw(gameTime, spriteBatch);
            MotionController.Draw(gameTime, spriteBatch);
        }

        public void GraphicsDeviceCreated(GraphicsDevice graphicsDevice, Vector2 nativeSize)
        {
            KeyboardInput.GraphicsDeviceCreated(graphicsDevice, nativeSize);
            MotionController.GraphicsDeviceCreated(graphicsDevice, nativeSize);
        }
    }
}
