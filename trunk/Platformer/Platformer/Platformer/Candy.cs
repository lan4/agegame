using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Platformer
{

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class Candy : Powerup
    {

        private bool collected;
        private TimeSpan powerupTimer;

        public Candy(Level level, Vector2 position)
            : base(level, position)
        {
            this.collected = false;
            powerupTimer = new TimeSpan();
            Color = Color.Aquamarine;
        }

        public override void LoadContent()
        {
            Texture = Level.Content.Load<Texture2D>("Sprites/Powerup_Candy");
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
            CollectedSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected");
        }

        public override void OnCollected(Player collectedBy)
        {
            collected = true;
            if (collectedBy.ageState == 0)
            {
                collectedBy.MoveScalar = 3.0f;
            }
            else
            {
                collectedBy.MoveScalar = 0.5f;
            }
            if (collectedBy.collectedCandy == 0)
                collectedBy.collectedCandy = 1;
        }

        public override void PowerupTimer(Player collectedBy, GameTime gameTime)
        {
            //Times the candy movement speed boost.
            Update(gameTime);
            if (collected  == true)
            {
                if (powerupTimer.Seconds >= 5)
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
