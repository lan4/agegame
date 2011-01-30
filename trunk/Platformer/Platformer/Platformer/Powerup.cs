#region File Description
//-----------------------------------------------------------------------------
// Powerup.cs
//
// An abstract class used for all of the powerups. Each powerup that
// uses the abstract class needs it's own OnCollected method, and should
// override the LoadContent method.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Platformer
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    abstract class Powerup
    {
        private Texture2D texture;
        private Vector2 origin;
        private SoundEffect collectedSound;

        public Color color = Color.Yellow;

        // The gem is animated from a base position along the Y axis.
        private Vector2 basePosition;
        private float bounce;

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this gem in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return basePosition + new Vector2(0.0f, bounce);
            }
        }

        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            set
            {
                texture = value;
            }
        }

        public Vector2 Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
            }
        }

        public SoundEffect CollectedSound
        {
            get
            {
                return collectedSound;
            }
            set
            {
                collectedSound = value;
            }
        }

        /// <summary>
        /// Gets a circle which bounds this powerup in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        /// <summary>
        /// Constructs a new Powerup.
        /// </summary>
        public Powerup(Level level, Vector2 position)
        {
            this.level = level;
            this.basePosition = position;

            LoadContent();
        }

        public Powerup()
        {
            // TODO: Complete member initialization
        }

        /// <summary>
        /// Loads the gem texture and collected sound. Override this.
        /// </summary>
        public abstract void LoadContent();

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public abstract void Update(GameTime gameTime);
        //{
        //    // Bounce control constants
        //    const float BounceHeight = 0.18f;
        //    const float BounceRate = 3.0f;
        //    const float BounceSync = -0.75f;

        //    // Bounce along a sine curve over time.
        //    // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
        //    double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
        //    bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        //}

        /// <summary>
        /// Draws a gem in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch,float percentage)
        {
            spriteBatch.Draw(texture, Position, null, new Color((1 - percentage) * 255, (1 - percentage) * 255, (1 - percentage) * 255, (1 - percentage)), 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// Called when this powerup has been collected by a player and removed from the level. It
        /// is abstract to define the various benefits that could result.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this powerup.
        /// </param>

        public abstract void PowerupTimer(Player collectedBy, GameTime gameTime);
        public abstract void OnCollected(Player collectedBy);

    }
}
