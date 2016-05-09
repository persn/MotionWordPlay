namespace NTNU.MotionWordPlay
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public interface IGameLoop
    {
        void Initialize();
        void Load(ContentManager contentManager);
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        void GraphicsDeviceCreated(GraphicsDevice graphicsDevice, Vector2 nativeSize);
    }
}
