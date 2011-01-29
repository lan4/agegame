using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Platformer
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class Bottle : Powerup
    {
        public Bottle(Level level, Vector2 position)
            : base(level, position)
        {
            
        }

        public override void LoadContent()
        {
            Texture = Level.Content.Load<Texture2D>("Sprites/Gem");
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
            CollectedSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected");
        }

        public override void OnCollected(Player collectedBy)
        {

        }

        public override void PowerupTimer(Player collectedBy)
        {
            //Times the candy movement speed boost.
            //if (collected)
            //{
            //    TimeSpan endTime;
            //    endTime = powerTime.TotalGameTime;
            //    timeDifference = Math.Abs(endTime.Seconds - startTime.Seconds);
            //    if (timeDifference >= 5)
            //    {
            //        collected = false;
            //        collectedBy.MoveScalar = 1.0f;
            //    }
            //}
        }

    }
}
