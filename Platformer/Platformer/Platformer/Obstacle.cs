using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;


namespace Platformer
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    abstract class Obstacle
    {
        private Level level;
        private Vector2 origin;
        private Vector2 basePosition;
        private Texture2D texture;
        private bool passable;

        public bool Passable
        {
            get { return passable; }
            set { passable = value; }
        }

        public Level Level
        {
            get { return level; }
        }

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        public Vector2 Position
        {
            get
            {
                return basePosition;
            }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        public Obstacle(Level level, Vector2 position)
        {
            this.level = level;
            this.basePosition = position;

            LoadContent();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.Red, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public void LoadContent()
        {
            Texture = Level.Content.Load<Texture2D>("Sprites/Gem");
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public abstract void Update(Player player, GameTime gameTime);
    }
}
