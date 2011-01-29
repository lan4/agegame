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
        private GameTime powerTime;
        private TimeSpan startTime;
        private int timeDifference;

        public Candy(Level level, Vector2 position)
            : base(level, position)
        {
            this.collected = false;
        }

        public override void LoadContent()
        {
            Texture = Level.Content.Load<Texture2D>("Sprites/Gem");
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
            CollectedSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected");
        }

        public override void OnCollected(Player collectedBy)
        {
            collectedBy.MoveScalar = 1.3f;
            collected = true;
            startTime = powerTime.TotalGameTime;
        }

        public override void PowerupTimer(Player collectedBy)
        {
            //Times the candy movement speed boost.
            if (collected)
            {
                TimeSpan endTime;
                endTime = powerTime.TotalGameTime;
                timeDifference = Math.Abs(endTime.Seconds - startTime.Seconds);
                if (timeDifference >= 5)
                {
                    collected = false;
                    collectedBy.MoveScalar = 1.0f;
                }
            }
        }

        //public void Update(GameTime gameTime)
        //{
        //    //// Bounce control constants
        //    //const float BounceHeight = 0.18f;
        //    //const float BounceRate = 3.0f;
        //    //const float BounceSync = -0.75f;

        //    //// Bounce along a sine curve over time.
        //    //// Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
        //    //double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
        //    //bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        //}

    }
}
