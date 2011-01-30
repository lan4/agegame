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
        private bool collected;
        private TimeSpan powerupTimer;

        public Bottle(Level level, Vector2 position)
            : base(level, position)
        {
            
        }

        public override void LoadContent()
        {
            Texture = Level.Content.Load<Texture2D>("Sprites/Powerup_Bottle");
            Origin = new Vector2(Texture.Width / 2.0f, Texture.Height / 2.0f);
            CollectedSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected");
        }

        public override void OnCollected(Player collectedBy)
        {
            collectedBy.CryCount += 1;
            if (collectedBy.collectedMilk == 0)
                collectedBy.collectedMilk = 1;
        }

        public override void PowerupTimer(Player collectedBy, GameTime gameTime)
        {
           

        }

        public override void Update(GameTime gameTime)
        {

        }

    }
}
