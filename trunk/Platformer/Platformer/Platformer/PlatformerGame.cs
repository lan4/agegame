#region File Description
//-----------------------------------------------------------------------------
// PlatformerGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;


namespace Platformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformerGame : Microsoft.Xna.Framework.Game
    {
        // Resources for drawing.
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Global content.
        private SpriteFont hudFont;

        private Texture2D menuOverlay;
        private Texture2D pauseOverlay;
        private Texture2D milkOverlay;
        private Texture2D candyOverlay;
        private Texture2D coffeeOverlay;
        private Texture2D moneyOverlay;
        private Texture2D winOverlay;
        private Texture2D loseOverlay;
        private Texture2D diedOverlay;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;
        private bool wasContinuePressed;
        private bool gameStarted = false;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        // We store our input states so that we only poll once per frame, 
        // then we use the same input state wherever needed
        private GamePadState gamePadState;
        private KeyboardState keyboardState;
        private TouchCollection touchState;
        private AccelerometerState accelerometerState;
        
        // The number of levels in the Levels directory of our content. We assume that
        // levels in our content are 0-based and that all numbers under this constant
        // have a level file present. This allows us to not need to check for the file
        // or handle exceptions, both of which can add unnecessary time to level loading.
        private const int numberOfLevels = 1;

        private bool paused = false;
        private bool pauseKeyDown = false;
        

        public PlatformerGame()
        {   
            graphics = new GraphicsDeviceManager(this);
            // graphics.IsFullScreen = true;
            // graphics.PreferredBackBufferHeight = 340;
            // graphics.PreferredBackBufferWidth = 480;
            Content.RootDirectory = "Content";

#if WINDOWS_PHONE
            graphics.IsFullScreen = true;
            TargetElapsedTime = TimeSpan.FromTicks(333333);
#endif

            Accelerometer.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            hudFont = Content.Load<SpriteFont>("Fonts/Hud");

            // Load overlay textures
            menuOverlay = Content.Load<Texture2D>("Overlays/main_menu");
            pauseOverlay = Content.Load<Texture2D>("Overlays/pause_menu");
            milkOverlay = Content.Load<Texture2D>("Overlays/milk_menu");
            coffeeOverlay = Content.Load<Texture2D>("Overlays/coffee_menu");
            candyOverlay = Content.Load<Texture2D>("Overlays/candy_menu");
            moneyOverlay = Content.Load<Texture2D>("Overlays/money_menu");
            winOverlay = Content.Load<Texture2D>("Overlays/death");
            loseOverlay = Content.Load<Texture2D>("Overlays/death");
            diedOverlay = Content.Load<Texture2D>("Overlays/death");

            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
            //Which means its impossible to test this from VS.
            //So we have to catch the exception and throw it away
            try
            {
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Volume = 1.0f;
                // MediaPlayer.Play(Content.Load<Song>("Sounds/BackgroundMusic"));
            }
            catch { }

            LoadNextLevel();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Handle polling for our input and handling high-level input
            HandleInput();
            CheckPauseKey(keyboardState);
            if (level.Player.collectedMilk == 1)
            {
                BeginPause(true);
            }
            else if (level.Player.collectedCoffee == 1)
            {
                BeginPause(true);
            }
            else if (level.Player.collectedCandy == 1)
            {
                BeginPause(true);
            }
            else if (level.Player.collectedMoney == 1)
            {
                BeginPause(true);
            }

            if(!paused)
            {
                // update our level, passing down the GameTime along with all of our input states
                level.Update(gameStarted, gameTime, keyboardState, gamePadState, touchState, 
                         accelerometerState, Window.CurrentOrientation);
            }
            
            base.Update(gameTime);
        }

        private void HandleInput()
        {
            // get all of our input states
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
            touchState = TouchPanel.GetState();
            accelerometerState = Accelerometer.GetState();

            // Exit the game when back is pressed.
            if (gamePadState.Buttons.Back == ButtonState.Pressed)
                Exit();

            bool continuePressed =
                keyboardState.IsKeyDown(Keys.Enter) ||
                gamePadState.IsButtonDown(Buttons.A) ||
                touchState.AnyTouch();

            bool quitPressed =
                keyboardState.IsKeyDown(Keys.Q) ||
                gamePadState.IsButtonDown(Buttons.B);

            // Perform the appropriate action to start the game
            if (!gameStarted && continuePressed && !wasContinuePressed)
            {
                gameStarted = true;
                MediaPlayer.Play(Content.Load<Song>("Sounds/BackgroundMusic"));
            }
            if ((!gameStarted || paused) && quitPressed)
                Exit();

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (gameStarted && !wasContinuePressed && continuePressed)
            {
                if (level.Player.collectedMilk == 1)
                {
                    EndPause();
                    level.Player.collectedMilk = 2;
                }
                else if (level.Player.collectedCoffee == 1)
                {
                    EndPause();
                    level.Player.collectedCoffee = 2;
                }
                else if (level.Player.collectedCandy == 1)
                {
                    EndPause();
                    level.Player.collectedCandy = 2;
                }
                else if (level.Player.collectedMoney == 1)
                {
                    EndPause();
                    level.Player.collectedMoney = 2;
                }
                else if (!level.Player.IsAlive)
                {
                    level.StartNewLife();
                }
                else if (level.TimeRemaining == TimeSpan.Zero)
                {
                    if (level.ReachedExit)
                        LoadNextLevel();
                    else
                        MediaPlayer.Play(Content.Load<Song>("Sounds/BackgroundMusic"));
                    ReloadCurrentLevel();
                }
            }

            wasContinuePressed = continuePressed;
        }

        private void BeginPause(bool userInitiated)
        {
            paused = true;
            MediaPlayer.Pause();
            
        }

        private void EndPause()
        {
            paused = false;
            MediaPlayer.Resume();
        }

        private void CheckPauseKey(KeyboardState keyboardstate)
        {
            bool pauseKeyDownThisFrame = keyboardState.IsKeyDown(Keys.P);
            if (!pauseKeyDown && pauseKeyDownThisFrame)
            {
                if (!paused)
                    BeginPause(true);
                else
                {
                    EndPause();
                }
            }
            pauseKeyDown = pauseKeyDownThisFrame;
        }


        private void LoadNextLevel()
        {
            // move to the next level
            levelIndex = (levelIndex + 1) % numberOfLevels;

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Services, fileStream, levelIndex);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }

        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            level.Draw(gameTime, spriteBatch);

            DrawHud();

            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);

            //float percentage = (gameTime.TotalGameTime.Seconds /TimeSpan.FromMinutes(6.00).Seconds);
            spriteBatch.Begin();
            //DrawShadowedString(hudFont, percentage.ToString(), hudLocation+new Vector2(0,50), Color.Yellow);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawHud()
        {
            spriteBatch.Begin();

            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            string timeString = "TIME: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");

            int percentage = (int)level.TimeRemaining.TotalSeconds*80/360;

            Color timeColor;
            if (level.TimeRemaining > WarningTime ||
                level.ReachedExit ||
                (int)level.TimeRemaining.TotalSeconds % 2 == 0)
            {
                timeColor = Color.Yellow;
            }
            else
            {
                timeColor = Color.Red;
            }
            //DrawShadowedString(hudFont, timeString, hudLocation, timeColor);

            DrawShadowedString(hudFont, "Age: "+(80-percentage).ToString(), hudLocation, Color.Black);

            // Draw score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            DrawShadowedString(hudFont, "Money: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.Black);
           
            // Determine the status overlay message to show.
            Texture2D status = null;
            if (!gameStarted)
            {
                status = menuOverlay;
            }
            else if (level.Player.collectedMilk == 1)
            {
                status = milkOverlay;
            }
            else if (level.Player.collectedCoffee == 1)
            {
                status = coffeeOverlay;
            }
            else if (level.Player.collectedCandy == 1)
            {
                status = candyOverlay;
            }
            else if (level.Player.collectedMoney == 1)
            {
                status = moneyOverlay;
            }
            else if (paused)
            {
                status = pauseOverlay;
            }
            else if (level.TimeRemaining == TimeSpan.Zero)
            {
                if (level.ReachedExit)
                {
                    status = winOverlay;
                }
                else
                {
                    status = loseOverlay;
                }
            }
            else if (!level.Player.IsAlive)
            {
                status = diedOverlay;
            }

            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }

            spriteBatch.End();
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}
