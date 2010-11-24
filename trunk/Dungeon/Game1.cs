/*
 * ECE4893
 * Multicore and GPU Programming for Video Games
 * 
 * Author: Hsien-Hsin Sean Lee
 * 
 * School of Electrical and Computer Engineering
 * Georgia Tech
 * 
 * leehs@gatech.edu
 * 
 * */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Dungeon.Furniture;
using Dungeon.Lights;

namespace Dungeon {
    public class Game1 : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private AudioComponent audioComponent;
        private Cue backmusic;

        public Player Arnold;
        public Enemy enemy;
        public Room masterRoom;

        public int numberdiffLights, numberspecLights;
        public AmbDiffSpecLights[] LiteSource;
        public KeyboardState oldState;

        //public Pyramid[] nugget;
        //public Teapot[] teapot;

        public Matrix viewMatrix;
        public Matrix projectionMatrix;
        public Matrix lightProjectionMatrix;
        private Vector3 spin;

        private RenderTarget2D sceneTarget;
        private RenderTarget2D depthTarget;
        public Effect P4Effect { get; set; }

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.IsFullScreen = false;
        }


        protected void SetupRoom() {
            if (masterRoom == null) {
                masterRoom = new Room(this);
                Components.Add(masterRoom);
            }
            masterRoom.Coefficient_A_Materials = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            masterRoom.Coefficient_D_Materials = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            masterRoom.Coefficient_S_Materials = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            masterRoom.MorphRate = -0.5f;
            masterRoom.SpinMatrix = Matrix.Identity;
            masterRoom.gSpinWVP = masterRoom.SpinMatrix * viewMatrix * projectionMatrix;

            //Changed this to something more granular.
            masterRoom.WorldMatrix = Matrix.Identity;
            masterRoom.View = this.viewMatrix;
            masterRoom.Projection = this.projectionMatrix;
        }

        protected void InstallLights() {

            masterRoom.NumOfLights = 4;
            LiteSource = new AmbDiffSpecLights[4];

            // Set up a diffuse/specular light source
            // Due to the room model is too simplified, the light will 
            // have pretty much similar effect on all pixls
            LiteSource[0] = new AmbDiffSpecLights(this, 256);
            LiteSource[0].Position = new Vector4(0.0f, 128f, 0.0f, 1.0f);
            LiteSource[0].Coefficient_ambient = new Vector4(0.2f, 0.2f, 0.2f, 1.0f);
            LiteSource[0].Coefficient_diffuse = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
            LiteSource[0].Coefficient_specular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            LiteSource[0].Shininess = 1.0f;
            LiteSource[0].Is_on = 1;
            LiteSource[0].Is_PointLight = 1;
            LiteSource[0].LightDirection = new Vector4(0.0f, 0.0f, 0.0f, 0.0f); // Not used if this is a point light.
            LiteSource[0].Attenuation = new Vector4(1.0f, 0.000005f, 0.0f, 0.0f); // no attenuation

            // Set up a point light source

            LiteSource[1] = new AmbDiffSpecLights(this, 256);
            LiteSource[1].Position = new Vector4(-180.0f, 20.0f, -180.0f, 1.0f);
            LiteSource[1].Coefficient_ambient = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            LiteSource[1].Coefficient_diffuse = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            LiteSource[1].Coefficient_specular = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            LiteSource[1].Shininess = 4.0f;
            LiteSource[1].Is_on = 0;
            LiteSource[1].Is_PointLight = 1;
            LiteSource[1].LightDirection = new Vector4(-180.0f, 100.0f, -180.0f, 0.0f); // Not used if this is a point light.
            LiteSource[1].Attenuation = new Vector4(1.0f, 0.000075f, 0.0f, 0.0f); // smooth attenuation

            // Set up a second point light source

            LiteSource[2] = new AmbDiffSpecLights(this, 256);
            LiteSource[2].Position = new Vector4(180.0f, 180.0f, 180.0f, 1.0f);
            LiteSource[2].Coefficient_ambient = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            LiteSource[2].Coefficient_diffuse = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            LiteSource[2].Coefficient_specular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            LiteSource[2].Shininess = 8.0f;
            LiteSource[2].Is_on = 0;
            LiteSource[2].Is_PointLight = 1;
            LiteSource[2].LightDirection = new Vector4(1.0f, 0.0f, 1.0f, 0.0f); // Not used if this is a point light.
            LiteSource[2].Attenuation = new Vector4(1.0f, 0.000075f, 0.0f, 0.0f); // smooth attenuation

            // Set up a third point light source

            LiteSource[3] = new AmbDiffSpecLights(this, 256);
            LiteSource[3].Position = new Vector4(0.0f, 60.0f, 170.0f, 1.0f);
            LiteSource[3].Coefficient_ambient = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            LiteSource[3].Coefficient_diffuse = new Vector4(0.3f, 0.3f, 1.0f, 1.0f);
            LiteSource[3].Coefficient_specular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            LiteSource[3].Shininess = 8.0f;
            LiteSource[3].Is_on = 0;
            LiteSource[3].Is_PointLight = 1;
            LiteSource[3].LightDirection = new Vector4(0.0f, 0.0f, 1.0f, 0.0f); // Not used if this is a point light.
            LiteSource[3].Attenuation = new Vector4(1.0f, 0.000075f, 0.0f, 0.0f); // smooth attenuation

            for (int i = 0; i < masterRoom.NumOfLights; i++) {
                Components.Add(LiteSource[i]);
            }
        }

        protected void InstallFurniture() {

            //nugget = new Pyramid[2];

            //nugget[0] = new Pyramid(this);
            //nugget[0].Coefficient_A_Materials = new Vector4(0.1412f, 0.1412f, 0.1412f, 1.0f);
            //nugget[0].Coefficient_D_Materials = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            //nugget[0].Coefficient_S_Materials = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            //nugget[0].Transparent = 0;
            //nugget[0].Init_position = new Vector3(100.0f, 40.0f, -100.0f);
            //nugget[0].WorldMatrix = Matrix.CreateTranslation(nugget[0].Init_position);
            //nugget[0].gWVP = nugget[0].WorldMatrix * viewMatrix * projectionMatrix;

            //nugget[1] = new Pyramid(this);
            //nugget[1].Coefficient_A_Materials = new Vector4(0.1412f, 0.1412f, 0.1412f, 1.0f);
            //nugget[1].Coefficient_D_Materials = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            //nugget[1].Coefficient_S_Materials = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            //nugget[1].Transparent = 1;
            //nugget[1].Init_position = new Vector3(-100.0f, 60.0f, 100.0f);
            //nugget[1].WorldMatrix = Matrix.CreateTranslation(nugget[1].Init_position);
            //nugget[1].gWVP = nugget[1].WorldMatrix * viewMatrix * projectionMatrix;

            //Components.Add(nugget[0]);
            //Components.Add(nugget[1]);

            //teapot = new Teapot[2];

            //teapot[0] = new Teapot(this);
            //teapot[0].Coefficient_A_Materials = new Vector4(0.05f, 0.05f, 0.05f, 1.0f);
            //teapot[0].Coefficient_D_Materials = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            //teapot[0].Coefficient_S_Materials = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            //teapot[0].Init_position = new Vector3(-70.0f, 30.0f, -120.0f);
            //teapot[0].Scaling = new Vector3(10.0f, 10.0f, 10.0f);
            //teapot[0].WireFrame = 0;
            //teapot[0].WorldMatrix = Matrix.CreateTranslation(teapot[0].Init_position);
            //teapot[0].gWVP = teapot[0].WorldMatrix * viewMatrix * projectionMatrix;

            //teapot[1] = new Teapot(this);
            //teapot[1].Coefficient_A_Materials = new Vector4(0.05f, 0.05f, 0.05f, 1.0f);
            //teapot[1].Coefficient_D_Materials = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            //teapot[1].Coefficient_S_Materials = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            //teapot[1].Init_position = new Vector3(80.0f, 50.0f, 100.0f);
            //teapot[1].Scaling = new Vector3(12.0f, 12.0f, 12.0f);
            //teapot[1].WireFrame = 1;
            //teapot[1].WorldMatrix = Matrix.CreateTranslation(teapot[0].Init_position);
            //teapot[1].gWVP = teapot[1].WorldMatrix * viewMatrix * projectionMatrix;

            //Components.Add(teapot[0]);
            //Components.Add(teapot[1]);

        }

        protected void InitPlayer() {
            Arnold = new Player(this);
            Arnold.Position = new Vector3(0.0f, 35.0f, 0.0f);
            // The LookAtVec.XZ will be normalized for turning purposes
            Arnold.LookAtVec = new Vector3(0.0f, 0.0f, -1.0f);  // this is a vector for only X, Z component, 
            // Y will be accounted for when call Matrix.CreateLookAt()
            Components.Add(Arnold);
        }

        protected void InitEnemy() {
            this.enemy = new Enemy(this);

            Components.Add(enemy);
        }

        protected override void Initialize() {

            // Audio
            audioComponent = new AudioComponent(this);
            Components.Add(audioComponent);

            // initialize the player
            // a better programming style would put player into one class
            InitPlayer();
            InitEnemy();

            spin = new Vector3(0.0f, 0.0f, 0.0f);

            viewMatrix = Matrix.CreateLookAt(Arnold.Position, (Arnold.Position + Arnold.LookAtVec), Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.ToRadians(45),
                    (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height,
                    1f, 1024f);

            //Setup the view matrix for shadow mapping.
            this.lightProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1, 0.1f, 1024f);


            Arnold.ViewMatrix = viewMatrix;
            Arnold.ProjectionMatrix = projectionMatrix;

            //This should be done in LoadContent(), but the enemy needs it now-ish so I'm just moving it here.
            this.P4Effect = Content.Load<Effect>("Project4/P4Shader");
            this.enemy.ViewMatrix = viewMatrix;
            this.enemy.ProjectionMatrix = projectionMatrix;

            // initialize the room
            SetupRoom();

            // Add lights
            InstallLights();

            // Add furniture
            InstallFurniture();

            base.Initialize();

            backmusic = audioComponent.GetCue("footsteps1");
            backmusic.Play();

        }

        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            masterRoom.WallEffect = Content.Load<Effect>("DungeonEffect");
            masterRoom.WallEffect.CurrentTechnique = masterRoom.WallEffect.Techniques["myTech"];

            //nugget[0].PyramidEffect = Content.Load<Effect>("DungeonEffect");
            //nugget[0].PyramidEffect.CurrentTechnique = nugget[0].PyramidEffect.Techniques["myTech"];
            //nugget[1].PyramidEffect = Content.Load<Effect>("DungeonEffect");
            //nugget[1].PyramidEffect.CurrentTechnique = nugget[1].PyramidEffect.Techniques["myTech"];

            //teapot[0].TeapotEffect = Content.Load<Effect>("DungeonEffect");
            //teapot[0].TeapotEffect.CurrentTechnique = teapot[0].TeapotEffect.Techniques["myTech"];
            //teapot[1].TeapotEffect = Content.Load<Effect>("DungeonEffect");
            //teapot[1].TeapotEffect.CurrentTechnique = teapot[1].TeapotEffect.Techniques["myTech"];

            this.sceneTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1, GraphicsDevice.DisplayMode.Format);
            this.depthTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1, GraphicsDevice.DisplayMode.Format);

            base.LoadContent();
        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }


        protected void LightSwitch() {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.F1) != oldState.IsKeyDown(Keys.F1)) {
                if (keyboard.IsKeyDown(Keys.F1)) {
                    // key F1 is pressed
                    // turn off Light 0
                    LiteSource[0].Flipped();
                } else {
                    // key F1 is released
                }
                oldState = keyboard;
            } else if (keyboard.IsKeyDown(Keys.F2) != oldState.IsKeyDown(Keys.F2)) {
                if (keyboard.IsKeyDown(Keys.F2)) {
                    // key F2 is pressed
                    // turn off Light 1
                    LiteSource[1].Flipped();
                } else {
                    // key F2 is released
                }
                oldState = keyboard;
            } else if (keyboard.IsKeyDown(Keys.F3) != oldState.IsKeyDown(Keys.F3)) {
                if (keyboard.IsKeyDown(Keys.F3)) {
                    // key F3 is pressed
                    // turn off Light 2
                    LiteSource[2].Flipped();
                } else {
                    // key F3 is released
                }
                oldState = keyboard;
            } else if (keyboard.IsKeyDown(Keys.F4) != oldState.IsKeyDown(Keys.F4)) {
                if (keyboard.IsKeyDown(Keys.F4)) {
                    // key F4 is pressed
                    // turn off Light 3
                    LiteSource[3].Flipped();
                } else {
                    // key F4 is released
                }
                oldState = keyboard;
            }
        }


        protected override void Update(GameTime gameTime) {
            Vector3 offset = new Vector3(0.0f, 0.005f, 0.0f);

            KeyboardState keyboard = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();


            Arnold.Move();
            LightSwitch();

            viewMatrix = Matrix.CreateLookAt(Arnold.Position, (Arnold.Position + Arnold.LookAtVec), Vector3.Up);
            Arnold.ViewMatrix = viewMatrix;
            enemy.ViewMatrix = viewMatrix;

            //Again, I wanted this more granular.
            masterRoom.WorldMatrix = Matrix.Identity;
            masterRoom.View = this.viewMatrix;
            masterRoom.Projection = this.projectionMatrix;

            // create a spinning matrix
            // This is not a good programming style, try to avoid in the future
            spin = spin + offset;
            masterRoom.SpinMatrix = Matrix.CreateRotationY(spin.Y) * Matrix.CreateTranslation(new Vector3(195, 53, 0)); // rotate around Y
            masterRoom.gSpinWVP = masterRoom.SpinMatrix * viewMatrix * projectionMatrix; // this does not appear to be a good programming style, better done inside the Room class

            //// place the pyramid
            //nugget[0].gWVP = nugget[0].WorldMatrix * viewMatrix * projectionMatrix;
            //nugget[1].gWVP = nugget[1].WorldMatrix * viewMatrix * projectionMatrix;

            //// place the teapot
            //teapot[0].gWVP = teapot[0].WorldMatrix * viewMatrix * projectionMatrix;

            //// place the teapot2
            //teapot[1].gWVP = teapot[1].WorldMatrix * viewMatrix * projectionMatrix;


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            //Send most of what we need to the shaders.
            this.SendDataToShaders();

            #region DepthBuffer Rendering
            //Render the scene for each light.
            this.P4Effect.CurrentTechnique = this.P4Effect.Techniques["DepthMapped"];
            this.P4Effect.Parameters["World"].SetValue(Matrix.Identity);
            this.P4Effect.Parameters["Projection"].SetValue(this.lightProjectionMatrix);
            //this.P4Effect.Parameters["LightIndex"].SetValue(0);

            this.GraphicsDevice.Clear(Color.TransparentBlack);

            //The values 0-5 are the int casts of the enumerations CubeMapFace.  Basically, this saves typing.
            for (int f = 0; f < 6; f++) {
                //Cast to something more useful.
                CubeMapFace face = (CubeMapFace)f;

                //Set view direction and cube face.
                this.P4Effect.Parameters["View"].SetValue(LiteSource[0].GetViewMatrix(face));
                this.GraphicsDevice.SetRenderTarget(0, LiteSource[0].LightCube, face);

                this.masterRoom.DrawFloor();
                this.enemy.DrawShadowEffect(this.GraphicsDevice);
            }
            
            //Render depth buffer for camera.
            this.GraphicsDevice.SetRenderTarget(0, this.depthTarget);

            this.P4Effect.Parameters["World"].SetValue(Matrix.Identity);
            this.P4Effect.Parameters["View"].SetValue(this.viewMatrix);
            this.P4Effect.Parameters["Projection"].SetValue(this.projectionMatrix);

            this.GraphicsDevice.Clear(Color.TransparentBlack);
            this.masterRoom.DrawFloor();
            this.enemy.DrawShadowEffect(this.GraphicsDevice);

            #endregion

            #region SceneRendering
            //Set effect to scene rendering.
            this.P4Effect.CurrentTechnique = this.P4Effect.Techniques["BumpMapped"];
            this.P4Effect.Parameters["World"].SetValue(Matrix.Identity);
            this.P4Effect.Parameters["View"].SetValue(this.viewMatrix);
            this.P4Effect.Parameters["Projection"].SetValue(this.projectionMatrix);

            //Set light shadow cubes.
            this.P4Effect.Parameters["ShadowCube"].SetValue(LiteSource[0].LightCube.GetTexture());

            //Set render target.
            this.GraphicsDevice.SetRenderTarget(0, this.sceneTarget);

            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            #endregion

            #region PostProcessing
            //Reset render target.
            this.GraphicsDevice.SetRenderTarget(0, null);
            
            this.P4Effect.CurrentTechnique = this.P4Effect.Techniques["MotionBlurred"];

            
            //Finally put shit on screen.
            this.spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            this.spriteBatch.Draw(this.sceneTarget.GetTexture(), Vector2.Zero, Color.White);

            this.spriteBatch.End();

            #endregion


        }

        private void SendDataToShaders() {
            Effect ObjEffect = masterRoom.WallEffect;

            // maybe bad programming planning 
            ObjEffect.Parameters["eyePosition"].SetValue(Arnold.Position);
            ObjEffect.Parameters["numLights"].SetValue(masterRoom.NumOfLights);

            // set up lights in shader code
            for (int i = 0; i < masterRoom.NumOfLights; i++) {
                ObjEffect.Parameters["light"].Elements[i].StructureMembers["position"].SetValue(LiteSource[i].Position);
                ObjEffect.Parameters["light"].Elements[i].StructureMembers["ambientLight"].SetValue(LiteSource[i].Coefficient_ambient);
                ObjEffect.Parameters["light"].Elements[i].StructureMembers["diffuseLight"].SetValue(LiteSource[i].Coefficient_diffuse);
                ObjEffect.Parameters["light"].Elements[i].StructureMembers["specLight"].SetValue(LiteSource[i].Coefficient_specular);
                ObjEffect.Parameters["light"].Elements[i].StructureMembers["shininess"].SetValue(LiteSource[i].Shininess);
                ObjEffect.Parameters["light"].Elements[i].StructureMembers["on"].SetValue(LiteSource[i].Is_on);
                ObjEffect.Parameters["light"].Elements[i].StructureMembers["attenuation"].SetValue(LiteSource[i].Attenuation);
                ObjEffect.Parameters["light"].Elements[i].StructureMembers["is_pointLight"].SetValue(LiteSource[i].Is_PointLight);
                ObjEffect.Parameters["light"].Elements[i].StructureMembers["lightDir"].SetValue(LiteSource[i].LightDirection);
            }

            //Set parameters for our shader.
            ObjEffect = this.P4Effect;
            ObjEffect.Parameters["Viewpoint"].SetValue(Arnold.Position);
            ObjEffect.Parameters["NumLights"].SetValue(masterRoom.NumOfLights);


            for (int i = 0; i < masterRoom.NumOfLights; i++) {
                ObjEffect.Parameters["Lights"].Elements[i].StructureMembers["Position"].SetValue(LiteSource[i].Position);
                ObjEffect.Parameters["Lights"].Elements[i].StructureMembers["AmbientLight"].SetValue(LiteSource[i].Coefficient_ambient);
                ObjEffect.Parameters["Lights"].Elements[i].StructureMembers["DiffuseLight"].SetValue(LiteSource[i].Coefficient_diffuse);
                ObjEffect.Parameters["Lights"].Elements[i].StructureMembers["SpecLight"].SetValue(LiteSource[i].Coefficient_specular);
                ObjEffect.Parameters["Lights"].Elements[i].StructureMembers["Shininess"].SetValue(LiteSource[i].Shininess);
                ObjEffect.Parameters["Lights"].Elements[i].StructureMembers["On"].SetValue(LiteSource[i].Is_on);
                ObjEffect.Parameters["Lights"].Elements[i].StructureMembers["Attenuation"].SetValue(LiteSource[i].Attenuation);
                ObjEffect.Parameters["Lights"].Elements[i].StructureMembers["IsPointLight"].SetValue(LiteSource[i].Is_PointLight);
                ObjEffect.Parameters["Lights"].Elements[i].StructureMembers["Direction"].SetValue(LiteSource[i].LightDirection);
            }
        }


    }
}
