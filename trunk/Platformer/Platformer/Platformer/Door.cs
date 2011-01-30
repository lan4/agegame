using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;


namespace Platformer
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class Door
    {
        private Level level;
        private Vector2 origin;
        private Vector2 basePosition;
        private Texture2D texture;
        private bool passable;
        //private Tile obTile;

        public bool Passable
        {
            get { return passable; }
            set { passable = value; }
        }

        //public Tile ObTile
        //{
        //    get { return obTile; }
        //    set { obTile = value; }
        //}

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

        public Door(Level level, Vector2 position)
        {
            this.level = level;
            this.basePosition = position;

            LoadContent();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }

        public void Update(Player player, GameTime gameTime)
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

        public void LoadContent()
        {
            Passable = false;
            Texture = Level.Content.Load<Texture2D>("Tiles/Tile_Door");
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
        }
    }
}
