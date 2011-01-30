using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Platformer
{

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class Coffee : Powerup
    {

        private bool collected;
        private TimeSpan powerupTimer;

        public Coffee(Level level, Vector2 position)
            : base(level, position)
        {
            this.collected = false;
            powerupTimer = new TimeSpan();
            Color = Color.White;
        }

        public override void LoadContent()
        {
            Texture = Level.Content.Load<Texture2D>("Sprites/Powerup_Coffee");
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
            CollectedSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected");
        }

        public override void OnCollected(Player collectedBy)
        {
            collected = true;
            if (collectedBy.ageState == 0)
            {
                collectedBy.MoveScalar = 6.0f;
            }
            else if (collectedBy.ageState == 1 || collectedBy.ageState == 2)
            {
                collectedBy.MoveScalar = 2.0f;
            }
        }

        public override void PowerupTimer(Player collectedBy, GameTime gameTime)
        {
            //Times the candy movement speed boost.
            Update(gameTime);
            if (collected == true)
            {
                if (powerupTimer.Seconds >= 15)
                {
                    collected = false;
                    collectedBy.MoveScalar = 1.0f;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (collected == true)
            {
                powerupTimer += gameTime.ElapsedGameTime;
            }
            else
            {
                collected = false;
            }
        }

    }
}
