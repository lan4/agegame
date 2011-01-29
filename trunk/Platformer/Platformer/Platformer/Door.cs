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
                //Tile.Collision = TileCollision.Passable;
            }
            else
            {
                Passable = false;
                //Tile.Collision = TileCollision.Impassable;
            }
        }

        public override void LoadContent()
        {
            Texture = Level.Content.Load<Texture2D>("Tiles/Tile_Door");
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
        }

    }
}
