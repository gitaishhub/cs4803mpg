#region File Description
//-----------------------------------------------------------------------------
// RetroProjectiles.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
#endregion

namespace Spacewar
{
    /// <summary>
    /// A collection to batch render the bullets for retro mode
    /// </summary>
    public class RetroProjectiles : Projectiles
    {
        private Effect effect;
        private EffectParameter worldViewProjectionParam;

        private VertexBuffer buffer;
        private VertexDeclaration vertexDecl;

        private int maxProjectileCount = 200;
        VertexPositionColor[] data;

        public RetroProjectiles(Game game)
            : base(game)
        {
            Create();
        }

        /// <summary>
        /// Creates a group of projectiles
        /// </summary>
        /// <param name="player">Which player shot the bullet</param>
        /// <param name="position">Start position of projectile</param>
        /// <param name="velocity">Initial velocity of projectile</param>
        /// <param name="angle">Direction projectile is facing</param>
        /// <param name="time">Game time that this projectile was shot</param>
        /// <param name="particles">The particles to add to for effects</param>
        public override void Add(PlayerIndex player, Vector3 position, Vector3 velocity, float angle, TimeSpan time, Particles particles, SceneItem readSource)
        {
            Add(new Projectile(GameInstance, player, position, velocity, angle, time, null), readSource);
        }

        /// <summary>
        /// Renders all bullets in a single batch
        /// </summary>
        public override void Render()
        {
            if (Count > 0)
            {
                base.Render();

                //RETRO
                //All bullets are rendered in a single draw call.

                //Build a new vertex buffer with all the bullets
                IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)GameInstance.Services.GetService(typeof(IGraphicsDeviceService));
                GraphicsDevice device = graphicsService.GraphicsDevice;

                // Don't exceed the maximum number of vertices in our default Vertex Buffer!
                int totalCount = (Count > maxProjectileCount) ? maxProjectileCount : Count;
                int position = 0;
                foreach (Projectile bullet in this)
                {
                    if (position < maxProjectileCount)
                    {
                        Vector3 bulletPosition = bullet.Position;
                        Vector3 bulletPosition1 = bullet.Position;

                        data[position++].Position = bulletPosition;

                        bulletPosition.X += .5f;
                        data[position++].Position = bulletPosition;

                        bulletPosition1.Y += .5f;
                        data[position++].Position = bulletPosition1;

                        bulletPosition.Y += .5f;
                        data[position++].Position = bulletPosition;
                    }
                }

                device.VertexDeclaration = vertexDecl;
                device.RenderState.CullMode = CullMode.None;

                effect.Begin();
                effect.Techniques[0].Passes[0].Begin();
                worldViewProjectionParam.SetValue(SpacewarGame.Camera.View * SpacewarGame.Camera.Projection);
                effect.CommitChanges();

                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.PointList, data, 0, totalCount * 4);

                effect.Techniques[0].Passes[0].End();
                effect.End();

                //We DO NOT call the base class here since we have handled all the children
            }
        }

        public void Create()
        {
            //Load the correct shader and set up the parameters
            if (effect == null || effect.IsDisposed)
            {
                OnCreateDevice();
            }
        }

        public override void OnCreateDevice()
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)GameInstance.Services.GetService(typeof(IGraphicsDeviceService));
            effect = SpacewarGame.ContentManager.Load<Effect>(SpacewarGame.Settings.MediaPath + @"shaders\simple");

            worldViewProjectionParam = effect.Parameters["worldViewProjection"];

            buffer = new VertexBuffer(graphicsService.GraphicsDevice, typeof(VertexPositionColor), maxProjectileCount * 4, BufferUsage.WriteOnly);
            vertexDecl = new VertexDeclaration(graphicsService.GraphicsDevice, VertexPositionColor.VertexElements);

            data = new VertexPositionColor[maxProjectileCount * 4];

            // initialize the Projectile Pool
            for (int position = 0; position < maxProjectileCount * 4; position++)
            {
                data[position] = new VertexPositionColor(new Vector3(0.0f, 0.0f, 0.0f), Color.White);
            }
        }
    }
}
