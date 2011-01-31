using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using BiDirectional_A_Star;

namespace ThreatAwarePathfinder {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AbstractDemo : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private const int WIDTH = 1024;
        private const int HEIGHT = 1024 / 2;

        private List<Agent> threats;
        private List<Agent> allies;
        private Node[,] nodeArray;

        private Texture2D threatTex;
        private Texture2D allyTex;
        private Vector2 agentTexOrigin;

        private Texture2D influenceTex;
        private Vector2 influenceTexOrigin;

        private Texture2D nodeTex;
        private Vector2 nodeTexOrigin;
        private float nodeScale;

        private float delta = 16;

        private BiDirectionAStar pather;

        private KeyboardState currKeyboard;
        private KeyboardState lastKeyborad;

        public AbstractDemo() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = AbstractDemo.WIDTH;
            this.graphics.PreferredBackBufferHeight = AbstractDemo.HEIGHT;
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            //Discretize space and create nodes.
            int nodeWidth = (int)(AbstractDemo.WIDTH / this.delta);
            int nodeHeight = (int)(AbstractDemo.HEIGHT / this.delta);
            this.nodeArray = new Node[nodeWidth, nodeHeight];

            Vector2 v = Vector2.Zero;
            for (int x = 0; x < nodeWidth; x++) {
                for (int y = 0; y < nodeHeight; y++) {
                    v.X = (x + 0.5f) * this.delta;
                    v.Y = (y + 0.5f) * this.delta;

                    Node node = new Node();
                    node.Pos = v;

                    this.nodeArray[x, y] = node;
                }
            }

            //Link nodes to their neighbors.
            for (int i = 0; i < nodeWidth; i++) {
                for (int j = 0; j < nodeHeight; j++) {
                    Node node = this.nodeArray[i, j];

                    bool topEdge = j - 1 < 0;
                    bool botEdge = j + 1 >= nodeHeight;
                    bool ltEdge = i - 1 < 0;
                    bool rtEdge = i + 1 >= nodeWidth;

                    //NW
                    if (!topEdge && !ltEdge) node.Neighbors.Add(this.nodeArray[i - 1, j - 1]);
                    //N
                    if (!topEdge) node.Neighbors.Add(this.nodeArray[i, j - 1]);
                    //NE
                    if (!topEdge && !rtEdge) node.Neighbors.Add(this.nodeArray[i + 1, j - 1]);
                    //E
                    if (!rtEdge) node.Neighbors.Add(this.nodeArray[i + 1, j]);
                    //SE
                    if (!botEdge && !rtEdge) node.Neighbors.Add(this.nodeArray[i + 1, j + 1]);
                    //S
                    if (!botEdge) node.Neighbors.Add(this.nodeArray[i, j + 1]);
                    //SW
                    if (!botEdge && !ltEdge) node.Neighbors.Add(this.nodeArray[i - 1, j + 1]);
                    //W
                    if (!ltEdge) node.Neighbors.Add(this.nodeArray[i - 1, j]);
                }
            }

            //Create agents.
            this.threats = new List<Agent>();
            this.allies = new List<Agent>();

            Agent threat1 = new Agent(new Vector2(1 * AbstractDemo.WIDTH / 4f, 3 * AbstractDemo.HEIGHT / 4f), 64);
            //Agent threat2 = new Agent(new Vector2(3 * AbstractDemo.WIDTH / 4f, 1 * AbstractDemo.HEIGHT / 4f), 64);
            this.threats.Add(threat1);
            //this.threats.Add(threat2);

            //Create search engine.
            this.pather = new BiDirectionAStar(this.nodeArray[0, 0], this.nodeArray[nodeWidth - 1, nodeHeight - 1]);
            this.pather.Enemies = this.threats;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            this.nodeTex = this.Content.Load<Texture2D>("node");
            this.nodeTexOrigin = new Vector2(nodeTex.Width / 2f, nodeTex.Height / 2f);

            this.nodeScale = (this.delta / 2) / this.nodeTex.Width;

            this.threatTex = Content.Load<Texture2D>("threat");
            this.allyTex = Content.Load<Texture2D>("ally");
            this.agentTexOrigin = new Vector2(allyTex.Width / 2f, allyTex.Height / 2f);

            this.influenceTex = Content.Load<Texture2D>("influence");
            this.influenceTexOrigin = new Vector2(influenceTex.Width / 2f, influenceTex.Height / 2f);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            this.currKeyboard = Keyboard.GetState();

            // Allows the game to exit
            if (this.currKeyboard.IsKeyDown(Keys.Escape)) this.Exit();

            // TODO: Add your update logic here
            if (this.currKeyboard.IsKeyDown(Keys.Space) && this.lastKeyborad.IsKeyUp(Keys.Space)) {
                this.pather.Step();
            }
            if (this.currKeyboard.IsKeyDown(Keys.OemPeriod)) {
                this.pather.Step();
            }
            if (this.currKeyboard.IsKeyDown(Keys.Enter) && this.lastKeyborad.IsKeyUp(Keys.Enter)) {
                this.pather.Solve();
            }

            base.Update(gameTime);

            this.lastKeyborad = this.currKeyboard;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(new Color(0.95f, 0.95f, 0.95f));

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            Color std = Color.Blue;
            Color dts = Color.Green;
            Color threat = Color.Orange;

            foreach (Agent agent in this.threats) {
                Color c = new Color(threat, 0.5f);
                float scale = agent.Radius / (this.influenceTex.Width / 2f);

                spriteBatch.Draw(this.threatTex, agent.Pos, null, threat, 0f, this.agentTexOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(this.influenceTex, agent.Pos, null, c, 0f, this.influenceTexOrigin, scale, SpriteEffects.None, 0f);
            }

            foreach (Node node in this.pather.stdSoFar) {
                Color c = new Color(std, 0.8f);
                spriteBatch.Draw(this.nodeTex, node.Pos, null, c, 0f, this.nodeTexOrigin, 1.5f * this.nodeScale, SpriteEffects.None, 0f);
            }

            foreach (Node node in this.pather.stdFrontier) {
                Color c = new Color(std, 0.4f);
                spriteBatch.Draw(this.nodeTex, node.Pos, null, c, 0f, this.nodeTexOrigin, 1.5f * this.nodeScale, SpriteEffects.None, 0f);
            }

            foreach (Node node in this.pather.dtsSoFar) {
                Color c = new Color(dts, 0.8f);
                spriteBatch.Draw(this.nodeTex, node.Pos, null, c, 0f, this.nodeTexOrigin, 1.5f * this.nodeScale, SpriteEffects.None, 0f);
            }

            foreach (Node node in this.pather.dtsFrontier) {
                Color c = new Color(dts, 0.4f);
                spriteBatch.Draw(this.nodeTex, node.Pos, null, c, 0f, this.nodeTexOrigin, 1.5f * this.nodeScale, SpriteEffects.None, 0f);
            }

            foreach (Node node in this.pather.answer)
            {
                Color c = new Color(Color.Red, 1.0f);
                spriteBatch.Draw(this.nodeTex, node.Pos, null, c, 0f, this.nodeTexOrigin, 1.5f * this.nodeScale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
