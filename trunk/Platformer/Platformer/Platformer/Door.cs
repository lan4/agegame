using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;


namespace Platformer
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class Door : Obstacle
    {
        public Door(Level level, Vector2 position)
            : base(level, position)
        {
            Passable = false;
        }

        public override void Update(Player player, GameTime gameTime)
        {
            if (player.ageState >= 1)
            {
                Passable = true;
            }
            else
            {
                Passable = false;
            }
        }

    }
}
