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

        private const int WIDTH = 1048;
        private const int HEIGHT = 768;

        private Node[,] nodeArray;

        private Texture2D nodeTex;
        private Vector2 nodeTexOrigin;
        private float nodeScale;

        private float delta = 16;

        private List<Node> path;

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

            //Search!
            BiDirectionAStar pather = new BiDirectionAStar(this.nodeArray[0, 0], this.nodeArray[nodeWidth - 1, nodeHeight - 1]);
            this.path = pather.Solve();
            List<Node> firstHalf = pather.answer0;
            List<Node> secondHalf = pather.answer1;
            Console.Out.WriteLine("Search complete.");

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
            KeyboardState keyboard = Keyboard.GetState();

            // Allows the game to exit
            if (keyboard.IsKeyDown(Keys.Escape)) this.Exit();

            // TODO: Add your update logic here
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(new Color(0.95f, 0.95f, 0.95f));

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            foreach (Node node in this.nodeArray) {
                spriteBatch.Draw(this.nodeTex, node.Pos, null, Color.White, 0f, this.nodeTexOrigin, this.nodeScale, SpriteEffects.None, 0f);
            }

            foreach (Node node in this.path) {
                spriteBatch.Draw(this.nodeTex, node.Pos, null, Color.Blue, 0f, this.nodeTexOrigin, 1.5f * this.nodeScale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
