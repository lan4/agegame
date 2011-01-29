using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Platformer
{
    class Layer
    {
        public Texture2D[] Textures { get; private set; }
        public float ScrollRate { get; private set; }
        
        //Desaturation Effect
        private Effect desaturationEffect;
        

        public Layer(ContentManager content, string basePath, float scrollRate)
        {
            // Assumes each layer only has 3 segments.
            Textures = new Texture2D[3];
            for (int i = 0; i < 3; ++i)
                Textures[i] = content.Load<Texture2D>(basePath + "_" + i);

            //Load Effect
            desaturationEffect = content.Load<Effect>("Effect/desaturate");
            //Texture2D

            

            ScrollRate = scrollRate;
        }

        public void Draw(SpriteBatch spriteBatch, float cameraPosition, float percentage)
        {
            // Assume each segment is the same width.
            int segmentWidth = Textures[0].Width;

            // Calculate which segments to draw and how much to offset them.
            float x = cameraPosition * ScrollRate;
            int leftSegment = (int)Math.Floor(x / segmentWidth);
            int rightSegment = leftSegment + 1;
            x = (x / segmentWidth - leftSegment) * -segmentWidth;

            spriteBatch.Draw(Textures[leftSegment % Textures.Length], new Vector2(x, 0.0f), new Color(255,255,255,percentage));
            spriteBatch.Draw(Textures[rightSegment % Textures.Length], new Vector2(x + segmentWidth, 0.0f), new Color(255,255,255,percentage));
            
        }

        /// <summary>
        /// Effect dynamically changes color saturation.
        /// </summary>
        void DrawDesaturate(GameTime gameTime)
        {
            
            // Draw four copies of the same sprite with different saturation levels.
            // The saturation amount is passed into the effect using the alpha of the
            // SpriteBatch.Draw color parameter. This isn't as flexible as using a
            // regular effect parameter, but makes it easy to draw many sprites with
            // a different saturation amount for each. If we had used an effect
            // parameter for this, we would have to end the sprite batch, then begin
            // a new one, each time we wanted to change the saturation setting.

            //byte pulsate = (byte)Pulsate(gameTime, 4, 0, 255);
        }


    }
}
