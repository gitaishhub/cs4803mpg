#region File Description
//-----------------------------------------------------------------------------
// SpacewarGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace Spacewar {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    partial class SpacewarGame : Microsoft.Xna.Framework.Game {
        // these are the size of the offscreen drawing surface
        // in general, no one wants to change these as there
        // are all kinds of UI calculations and positions based
        // on these dimensions.
        const int FixedDrawingWidth = 1280;
        const int FixedDrawingHeight = 720;

        #region Private Variables
        // these are the size of the output window, ignored
        // on Xbox 360
        private int preferredWindowWidth = 1280;
        private int preferredWindowHeight = 720;

        private static ContentManager contentManager;

        /// <summary>
        /// The game settings from settings.xml
        /// </summary>
        private static Settings settings = new Settings();

        private static Camera camera;

        /// <summary>
        /// Information about the players such as score, health etc
        /// </summary>
        private static Player[] players;

        /// <summary>
        /// The current game state
        /// </summary>
        private static GameState gameState = GameState.Started;

        /// <summary>
        /// Which game board are we playing on
        /// </summary>
        private static int gameLevel;

        /// <summary>
        /// Stores game paused state
        /// </summary>
        private bool paused;

        private GraphicsDeviceManager graphics;

        private bool enableDrawScaling;
        private RenderTarget2D drawBuffer;
        private DepthStencilBuffer drawDepthBuffer;
        private SpriteBatch spriteBatch;

        private static PlatformID currentPlatform;

        private static KeyboardState keyState;
        //        private bool justWentFullScreen;
        #endregion

        /// ///////////////////////////////////////////////
        /// Added for multi-threading
        private Thread soundThread;
        private Thread updateThread;
        // use volatile to prevent caching
        private volatile GameTime timeStamp;
        private volatile bool update;
        private volatile bool play;

        //Thank god this code is reasonably structured.
        private Screen currentScreen;
        //private Screen drawingScreen;

        public SpriteBatch SpriteBatch { get { return this.spriteBatch; } }

        #region Properties


        public static GameState GameState {
            get {
                return gameState;
            }
        }

        public static int GameLevel {
            get {
                return gameLevel;
            }
            set {
                gameLevel = value;
            }
        }

        public static Camera Camera {
            get {
                return camera;
            }
        }

        public static Settings Settings {
            get {
                return settings;
            }
        }

        public static Player[] Players {
            get {
                return players;
            }
        }

        public static ContentManager ContentManager {
            get {
                return contentManager;
            }
        }

        public static PlatformID CurrentPlatform {
            get {
                return currentPlatform;
            }
        }

        public static KeyboardState KeyState {
            get {
                return keyState;
            }
        }
        #endregion

        public SpacewarGame() {
#if XBOX360
            // we might as well use the xbox in all its glory
            preferredWindowWidth = FixedDrawingWidth;
            preferredWindowHeight = FixedDrawingHeight;
            enableDrawScaling = false;
#else
            enableDrawScaling = true;
#endif

            this.graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = preferredWindowWidth;
            this.graphics.PreferredBackBufferHeight = preferredWindowHeight;
            this.graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(PreparingDeviceSettings);

            // Game should run as fast as possible.
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 60f);
        }

        void PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e) {
            // We turn off auto depth buffer creation when scaling output.
            //
            e.GraphicsDeviceInformation.PresentationParameters.EnableAutoDepthStencil = !enableDrawScaling;
        }

        protected override void Initialize() {
            // game initialization code here

            //Uncomment this line to force a save of the default settings file. Useful when you had added things to settings.cs
            //NOTE in VS this will go in DEBUG or RELEASE - need to copy up to main project
            //Settings.Save("settings.xml");

            settings = Settings.Load("settings.xml");

            currentPlatform = System.Environment.OSVersion.Platform;

            //Initialise the sound
            Sound.Initialize();

            Window.Title = Settings.WindowTitle;

            base.Initialize();
        }

        protected override void BeginRun() {
            Sound.PlayCue(Sounds.TitleMusic);

            //Kick off the game by loading the logo splash screen
            ChangeState(GameState.LogoSplash);
            
            //Make a reference copy so that drawingScreen doesn't start out as null.
            //this.drawingScreen = this.currentScreen;

            float fieldOfView = (float)Math.PI / 4;
            float aspectRatio = (float)FixedDrawingWidth / (float)FixedDrawingHeight;
            float nearPlane = 10f;
            float farPlane = 700f;

            camera = new Camera(fieldOfView, aspectRatio, nearPlane, farPlane);
            camera.ViewPosition = new Vector3(0, 0, 500);

            /////////////////////////////
            // start threads for Update and Draw
            soundThread = new Thread(new ThreadStart(startSoundMT));
            updateThread = new Thread(new ThreadStart(startUpdateMT));

            soundThread.Start();
            updateThread.Start();

            base.BeginRun();
        }

        protected override void EndRun() {
            // //////////////////////////////////
            // kill threads *stabby, stabby*
            soundThread.Abort();
            updateThread.Abort();
        }

        private void startSoundMT() {
#if XBOX360
            // set thread affinity
            Thread.CurrentThread.SetProcessorAffinity(3);
#endif
            while (true) {
                if (this.play) {
                    this.play = false;
                    soundMT();
                }
            }
        }
        private void soundMT() {
            // Update the AudioEngine - MUST call this every frame!!
            Sound.Update();
        }



        private void startUpdateMT() {
#if XBOX360
            // set thread affinity
            Thread.CurrentThread.SetProcessorAffinity(5);
#endif
            while (true) {
                if (update) {
                    update = false;
                    updateMT(timeStamp);
                }
            }
        }

        private void updateMT(GameTime gameTime) {

            // everything to update goes in here
            TimeSpan elapsedTime = gameTime.ElapsedGameTime;
            TimeSpan time = gameTime.TotalGameTime;

            // The time since Update was called last
            float elapsed = (float)elapsedTime.TotalSeconds;

            GameState changeState = GameState.None;

            /*
            if ((keyState.IsKeyDown(Keys.RightAlt) || keyState.IsKeyDown(Keys.LeftAlt)) && keyState.IsKeyDown(Keys.Enter) && !justWentFullScreen)
            {
                ToggleFullScreen();
                justWentFullScreen = true;
            }

            if (keyState.IsKeyUp(Keys.Enter))
            {
                justWentFullScreen = false;
            }
            */
            if (XInputHelper.GamePads[PlayerIndex.One].BackPressed ||
                XInputHelper.GamePads[PlayerIndex.Two].BackPressed) {
                if (gameState == GameState.PlayEvolved || gameState == GameState.PlayRetro) {
                    paused = !paused;
                }

                if (gameState == GameState.LogoSplash) {
                    this.Exit();
                }
            }

            //Reload settings file?
            if (XInputHelper.GamePads[PlayerIndex.One].YPressed) {
                //settings = Settings.Load("settings.xml");
                //GC.Collect();
            }

            if (!paused) {
                //Update everything
                changeState = currentScreen.Update(time, elapsedTime);

                //If either player presses start then reset the game
                if (XInputHelper.GamePads[PlayerIndex.One].StartPressed ||
                    XInputHelper.GamePads[PlayerIndex.Two].StartPressed) {
                    changeState = GameState.LogoSplash;
                }

                if (changeState != GameState.None) {
                    ChangeState(changeState);
                }
            }

            base.Update(gameTime);

            //Update finished, make a copy for the drawing code.
            //This is certainly not the proper way to do this.  However, refactoring
            //the game to read in one state and write to another state would involve
            //a lot of fundamental changes beyond the scope of this project as all of
            //the game components edit their own values on each update.

            //this.drawingScreen = this.currentScreen;
        }

        protected override void Update(GameTime gTime) {
            //Set this first, to prevent the other threads from getting a nanosecond head start.
            this.timeStamp = gTime;

            // update keyboard status
            // This needs to happen within the same thread as the game window
            //  because of Windows stupidnessidity
            //  http://social.msdn.microsoft.com/forums/en-US/xnaframework/thread/0fc4492d-28d5-4f80-a97f-2ecb4c81b380
            keyState = Keyboard.GetState();
            XInputHelper.Update(this, keyState);

            //Set flags for other threads to step.
            this.update = true;
            this.play = true;

            //TimeSpan elapsedTime = gameTime.ElapsedGameTime;
            //TimeSpan time = gameTime.TotalGameTime;

            //// The time since Update was called last
            //float elapsed = (float)elapsedTime.TotalSeconds;

            //GameState changeState = GameState.None;

            //keyState = Keyboard.GetState();
            //XInputHelper.Update(this, keyState);

            //if ((keyState.IsKeyDown(Keys.RightAlt) || keyState.IsKeyDown(Keys.LeftAlt)) && keyState.IsKeyDown(Keys.Enter) && !justWentFullScreen)
            //{
            //    ToggleFullScreen();
            //    justWentFullScreen = true;
            //}

            //if (keyState.IsKeyUp(Keys.Enter))
            //{
            //    justWentFullScreen = false;
            //}

            //if (XInputHelper.GamePads[PlayerIndex.One].BackPressed ||
            //    XInputHelper.GamePads[PlayerIndex.Two].BackPressed)
            //{
            //    if (gameState == GameState.PlayEvolved || gameState == GameState.PlayRetro)
            //    {
            //        paused = !paused;
            //    }

            //    if (gameState == GameState.LogoSplash)
            //    {
            //        this.Exit();
            //    }
            //}

            ////Reload settings file?
            //if (XInputHelper.GamePads[PlayerIndex.One].YPressed)
            //{
            //    //settings = Settings.Load("settings.xml");
            //    //GC.Collect();
            //}

            //if (!paused)
            //{
            //    //Update everything
            //    changeState = currentScreen.Update(time, elapsedTime);

            //    // Update the AudioEngine - MUST call this every frame!!
            //    Sound.Update();

            //    //If either player presses start then reset the game
            //    if (XInputHelper.GamePads[PlayerIndex.One].StartPressed ||
            //        XInputHelper.GamePads[PlayerIndex.Two].StartPressed)
            //    {
            //        changeState = GameState.LogoSplash;
            //    }

            //    if (changeState != GameState.None)
            //    {
            //        ChangeState(changeState);
            //    }
            //}

            //base.Update(gameTime);
        }

        protected override bool BeginDraw() {
            if (!base.BeginDraw())
                return false;

            BeginDrawScaling();

            return true;
        }

        protected override void Draw(GameTime gameTime) {
            graphics.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

            base.Draw(gameTime);

            this.currentScreen.Render();
        }

        protected override void EndDraw() {
            EndDrawScaling();

            base.EndDraw();
        }

        internal void ChangeState(GameState NextState) {
            //Logo spash can come from ANY state since its the place you go when you restart
            if (NextState == GameState.LogoSplash) {
                if (currentScreen != null)
                    currentScreen.Shutdown();

                currentScreen = new TitleScreen(this);
                gameState = GameState.LogoSplash;
            } else if (gameState == GameState.LogoSplash && NextState == GameState.ShipSelection) {
                Sound.PlayCue(Sounds.MenuAdvance);

                //This is really where the game starts so setup the player information
                players = new Player[2] { new Player(), new Player() };

                //Start at level 1
                gameLevel = 1;

                currentScreen.Shutdown();
                currentScreen = new SelectionScreen(this);
                gameState = GameState.ShipSelection;
            } else if (gameState == GameState.PlayEvolved && NextState == GameState.ShipUpgrade) {
                currentScreen.Shutdown();
                currentScreen = new ShipUpgradeScreen(this);
                gameState = GameState.ShipUpgrade;
            } else if ((gameState == GameState.ShipSelection || GameState == GameState.ShipUpgrade) && NextState == GameState.PlayEvolved) {
                Sound.PlayCue(Sounds.MenuAdvance);

                currentScreen.Shutdown();
                currentScreen = new EvolvedScreen(this);
                gameState = GameState.PlayEvolved;
            } else if (gameState == GameState.LogoSplash && NextState == GameState.PlayRetro) {
                //Game starts here for retro
                players = new Player[2] { new Player(), new Player() };

                currentScreen.Shutdown();
                currentScreen = new RetroScreen(this);
                gameState = GameState.PlayRetro;
            } else if (gameState == GameState.PlayEvolved && NextState == GameState.Victory) {
                currentScreen.Shutdown();
                currentScreen = new VictoryScreen(this);
                gameState = GameState.Victory;
            } else {
                //This is a BAD thing and should never happen
                // What does this map to on XBox 360?
                //Debug.Assert(false, String.Format("Invalid State transition {0} to {1}", gameState.ToString(), NextState.ToString()));
            }
        }

        protected override void LoadContent() {
            base.LoadContent();

            contentManager = new ContentManager(Services);

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            if (currentScreen != null)
                currentScreen.OnCreateDevice();

            Font.Init(this);

            if (enableDrawScaling) {
                PresentationParameters pp = graphics.GraphicsDevice.PresentationParameters;

                drawBuffer = new RenderTarget2D(graphics.GraphicsDevice,
                                                FixedDrawingWidth, FixedDrawingHeight, 1,
                                                SurfaceFormat.Color,
                                                pp.MultiSampleType, pp.MultiSampleQuality);

                drawDepthBuffer = new DepthStencilBuffer(graphics.GraphicsDevice,
                                                FixedDrawingWidth, FixedDrawingHeight,
                                                pp.AutoDepthStencilFormat,
                                                pp.MultiSampleType, pp.MultiSampleQuality);    
            }
        }

        protected override void UnloadContent() {
            base.UnloadContent();

            if (drawDepthBuffer != null) {
                drawDepthBuffer.Dispose();
                drawDepthBuffer = null;
            }

            if (drawBuffer != null) {
                drawBuffer.Dispose();
                drawBuffer = null;
            }

            if (spriteBatch != null) {
                spriteBatch.Dispose();
                spriteBatch = null;
            }

            Font.Dispose();

            if (contentManager != null) {
                contentManager.Dispose();
                contentManager = null;
            }

        }

        private void ToggleFullScreen() {
            PresentationParameters presentation = graphics.GraphicsDevice.PresentationParameters;

            if (presentation.IsFullScreen) {   // going windowed
                graphics.PreferredBackBufferWidth = preferredWindowWidth;
                graphics.PreferredBackBufferHeight = preferredWindowHeight;
            } else {
                // going fullscreen, use desktop resolution to minimize display mode changes
                // this also has the nice effect of working around some displays that lie about 
                // supporting 1280x720
                GraphicsAdapter adapter = graphics.GraphicsDevice.CreationParameters.Adapter;
                graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;
            }

            graphics.ToggleFullScreen();
        }

        private void BeginDrawScaling() {
            PresentationParameters presentation = graphics.GraphicsDevice.PresentationParameters;

            if (enableDrawScaling && drawBuffer != null) {
                graphics.GraphicsDevice.SetRenderTarget(0, drawBuffer);
                graphics.GraphicsDevice.DepthStencilBuffer = drawDepthBuffer;
            }
        }

        private void EndDrawScaling() {
            // copy our offscreen surface to the backbuffer with appropriate
            // letterbox bars

            if (!enableDrawScaling || drawBuffer == null)
                return;

            // we disable the depthstencil when setting back to the back buffer
            // because the depthstencil surface (1280x720) might be smaller 
            // than the output window (say 1600x1200) and the graphics device
            // doesn't like that

            graphics.GraphicsDevice.DepthStencilBuffer = null;
            graphics.GraphicsDevice.SetRenderTarget(0, null);

            PresentationParameters presentation = graphics.GraphicsDevice.PresentationParameters;

            float outputAspect = (float)presentation.BackBufferWidth / (float)presentation.BackBufferHeight;
            float preferredAspect = (float)FixedDrawingWidth / (float)FixedDrawingHeight;

            Rectangle dst;

            if (outputAspect <= preferredAspect) {
                // output is taller than it is wider, bars on top/bottom

                int presentHeight = (int)((presentation.BackBufferWidth / preferredAspect) + 0.5f);
                int barHeight = (presentation.BackBufferHeight - presentHeight) / 2;

                dst = new Rectangle(0, barHeight, presentation.BackBufferWidth, presentHeight);
            } else {
                // output is wider than it is tall, bars left/right

                int presentWidth = (int)((presentation.BackBufferHeight * preferredAspect) + 0.5f);
                int barWidth = (presentation.BackBufferWidth - presentWidth) / 2;

                dst = new Rectangle(barWidth, 0, presentWidth, presentation.BackBufferHeight);
            }

            // clear to get black bars
            graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            // draw a quad to get the draw buffer to the back buffer
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            spriteBatch.Draw(drawBuffer.GetTexture(), dst, Color.White);
            spriteBatch.End();
        }
    }
}
