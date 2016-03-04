using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NTNU.MotionWordPlay
{
    public interface IGameLoop
    {
        void Load();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
