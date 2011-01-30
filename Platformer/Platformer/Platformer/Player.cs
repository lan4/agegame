#region File Description
//-----------------------------------------------------------------------------
// Player.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Platformer
{
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    class Player
    {
        // Animations
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation celebrateAnimation;
        private Animation dieAnimation;
        private Animation cryAnimation;
        private SpriteEffects flip = SpriteEffects.FlipHorizontally;
        private AnimationPlayer sprite;

        // Sounds
        private SoundEffect killedSound;
        private SoundEffect jumpSound;
        private SoundEffect fallSound;

        public int ageState = 0;
        private int cryUses = 1;
        private bool cryStatus = false;
        private TimeSpan cryTime;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public bool IsCrying
        {
            get { return cryStatus; }
            set { cryStatus = value; }
        }

        Vector2 velocity;

 
        // Constants for controling horizontal movement
        private float MoveAcceleration = 13000.0f;
        private float MaxMoveSpeed = 1000.0f;
        private float GroundDragFactor = 0.48f;
        private float AirDragFactor = 0.58f;
        private float MovementSpeed = 0.5f;

        // Constants for controlling vertical movement
       
        private float MaxJumpTime = 0.0f;
        private float JumpLaunchVelocity = -3500.0f;
        private float GravityAcceleration = 3400.0f;
        private float MaxFallSpeed = 550.0f;
        private float JumpControlPower = 0.0f;


        // Input configuration
        private const float MoveStickScale = 1.0f;
        private const float AccelerometerScale = 1.0f;
        private const Buttons JumpButton = Buttons.A;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
            set { isOnGround = value; }
        }
        bool isOnGround;

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private float movement;
        private float moveScalar = 1.0f;

        public float MoveScalar
        {
            get { return moveScalar; }
            set { moveScalar = value; }
        }

        public float Movement
        {
            get { return movement; }
            set { movement = value; }
        }

        public void CheckAge(TimeSpan gameTime)
        {
            TimeSpan timeToAge;
            switch (ageState)
            {
                case(0):
                    timeToAge = TimeSpan.FromSeconds(240);
                break;
                case(1):
                    timeToAge = TimeSpan.FromSeconds(120);
                break;
                case(2):
                    timeToAge = TimeSpan.FromSeconds(0);
                break;
                default:
                    timeToAge = TimeSpan.FromSeconds(0);
                break;
            }

            if (gameTime < timeToAge)
            {
                this.AdvanceAge();
            }
        }

        //Call whenever character reaches age of next transition to modify character physics
        public void AdvanceAge()
        {
            /*  Starting values
            MoveAcceleration = 13000.0f;
            MaxMoveSpeed = 10000.0f;
            GroundDragFactor = 0.48f;
            AirDragFactor = 0.58f;
                   
            MaxJumpTime = 0.35f;
            JumpLaunchVelocity = -3500.0f;
            GravityAcceleration = 3400.0f;
            MaxFallSpeed = 550.0f;
            JumpControlPower = 0.14f; 
            */

            ageState++;
            switch (ageState)
            {
                case(1)://Set movement constants for Adult
                    MediaPlayer.Volume = 0.0f;
                    MoveAcceleration = 13000.0f;
                    MaxMoveSpeed = 1000.0f;
                    GroundDragFactor = 0.48f;
                    AirDragFactor = 0.58f; 
                    MaxJumpTime = 0.35f;
                    JumpLaunchVelocity = -3500.0f;
                    GravityAcceleration = 3400.0f;
                    MaxFallSpeed = 550.0f;
                    JumpControlPower = 0.14f;
                    MovementSpeed = 1.0f;
                    break;
                case(2)://Set movement constants for old man
                    MediaPlayer.Volume = 0.0f;
                    MoveAcceleration = 13000.0f;
                    MaxMoveSpeed = 1000.0f;
                    GroundDragFactor = 0.48f;
                    AirDragFactor = 0.58f;    
                    MaxJumpTime = 0.2f;
                    JumpLaunchVelocity = -2000.0f;
                    GravityAcceleration = 3400.0f;
                    MaxFallSpeed = 550.0f;
                    JumpControlPower = 0.14f;
                    MovementSpeed = 0.5f;
                    break;

            }
        }

        public void Cry()
        {
            if (cryUses >= 1)
            {
                cryUses--;
                cryStatus = true;
                cryTime = new TimeSpan();
            }
        }

        public void stopPlayer()
        {
            position = new Vector2(position.X - 5.0f, position.Y);
        }
        // Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;
        
        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Player(Level level, Vector2 position)
        {
            this.level = level;

            LoadContent();

            Reset(position);
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            // Load animated textures.
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Player_Baby_Idle"), 0.1f, true);
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Player_Baby_Walk"), 0.1f, true);
            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Player_Baby_Idle"), 0.1f, false);
            celebrateAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Player_Baby_Idle"), 0.1f, false);
            dieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Player_Baby_Cry"), 0.2f, true);
            cryAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Player_Baby_Cry"), 0.2f, true);

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Load sounds.            
            killedSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            fallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame. We also pass the game's orientation because when using the accelerometer,
        /// we need to reverse our motion when the orientation is in the LandscapeRight orientation.
        /// </remarks>
        public void Update(
            GameTime gameTime, 
            KeyboardState keyboardState, 
            GamePadState gamePadState, 
            TouchCollection touchState, 
            AccelerometerState accelState,
            DisplayOrientation orientation)
        {
            GetInput(keyboardState, gamePadState, touchState, accelState, orientation);

            ApplyPhysics(gameTime);

            if (IsAlive && IsOnGround)
            {
                if (Math.Abs(Velocity.X) - 0.02f > 0)
                {
                    sprite.PlayAnimation(runAnimation);
                }
                else if (cryStatus)
                {
                    sprite.PlayAnimation(cryAnimation);
                }
                else
                {
                    sprite.PlayAnimation(idleAnimation);
                }
            }

            if (cryStatus == true)
            {
                cryTime += gameTime.ElapsedGameTime;
            }
            else
            {
                cryStatus = false;
            }

            if (cryStatus == true)
            {
                if (cryTime.Seconds >= 2)
                {
                    cryStatus = false;
                }
            }

            // Clear input.
            movement = 0.0f;
            isJumping = false;
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput(
            KeyboardState keyboardState, 
            GamePadState gamePadState, 
            TouchCollection touchState,
            AccelerometerState accelState, 
            DisplayOrientation orientation)
        {
            // Get analog horizontal movement.
            movement = gamePadState.ThumbSticks.Left.X * MoveStickScale;

            // Ignore small movements to prevent running in place.
            if (Math.Abs(movement) < 0.1f)
                movement = 0.0f;

            // Move the player with accelerometer
            if (Math.Abs(accelState.Acceleration.Y) > 0.10f)
            {
                // set our movement speed
                movement = MathHelper.Clamp(-accelState.Acceleration.Y * AccelerometerScale, -1f, 1f);

                // if we're in the LandscapeLeft orientation, we must reverse our movement
                if (orientation == DisplayOrientation.LandscapeRight)
                    movement = -movement;
            }

            // If any digital horizontal movement input is found, override the analog movement.
            if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.Left) ||
                keyboardState.IsKeyDown(Keys.A))
            {
                movement = -MovementSpeed * moveScalar;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(Keys.Right) ||
                     keyboardState.IsKeyDown(Keys.D))
            {
                movement = MovementSpeed * moveScalar;
            }
            else if ((gamePadState.IsButtonDown(Buttons.B) ||
                     keyboardState.IsKeyDown(Keys.F)) && !cryStatus)
            {
                Cry();
            }


            // Check if the player wants to jump.
            isJumping =
                gamePadState.IsButtonDown(JumpButton) ||
                keyboardState.IsKeyDown(Keys.Space) ||
                keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.W) ||
                touchState.AnyTouch();
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            velocity.Y = DoJump(velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping && (this.ageState > 0))
            {
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                        jumpSound.Play();

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.PlayAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        public void OnKilled(Enemy killedBy)
        {
            isAlive = false;

            if (killedBy != null)
                killedSound.Play();
            else
                fallSound.Play();

            sprite.PlayAnimation(dieAnimation);
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            sprite.PlayAnimation(celebrateAnimation);
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the sprite to face the way we are moving.
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;
           

            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
    }
}
