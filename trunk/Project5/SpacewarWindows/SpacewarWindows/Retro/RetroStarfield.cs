#region File Description
//-----------------------------------------------------------------------------
// RetroStarfield.cs
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
#endregion

namespace Spacewar
{
    /// <summary>
    /// The stars for the background of retro mode
    /// </summary>
    public class RetroStarfield : Shape
    {
        private const int numberOfPoints = 800;
        private const int percentBigStars = 20;
        private Effect effect;

        public RetroStarfield(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Creates vertex buffer full of points
        /// </summary>
        public override void Create()
        {
            OnCreateDevice();
        }

        /// <summary>
        /// Renders the starfield
        /// </summary>
        public override void Render()
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)GameInstance.Services.GetService(typeof(IGraphicsDeviceService));

            GraphicsDevice device = graphicsService.GraphicsDevice;
            device.VertexDeclaration = vertexDecl;
            device.Vertices[0].SetSource(buffer, 0, VertexPositionColor.SizeInBytes);

            effect.Begin();
            effect.Techniques[0].Passes[0].Begin();

            device.DrawPrimitives(PrimitiveType.PointList, 0, numberOfPoints);

            effect.Techniques[0].Passes[0].End();
            effect.End();
        }

        public override void OnCreateDevice()
        {
            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)GameInstance.Services.GetService(typeof(IGraphicsDeviceService));

            effect = SpacewarGame.ContentManager.Load<Effect>(SpacewarGame.Settings.MediaPath + @"shaders\simplescreen");

            vertexDecl = new VertexDeclaration(graphicsService.GraphicsDevice, VertexPositionColor.VertexElements);

            buffer = new VertexBuffer(graphicsService.GraphicsDevice, typeof(VertexPositionColor), numberOfPoints, BufferUsage.WriteOnly);

            VertexPositionColor[] data = new VertexPositionColor[numberOfPoints];

            int pointCount = 0;
            Random random = new Random();
            while (pointCount < numberOfPoints)
            {
                byte greyValue = (byte)(random.Next(200) + 56); // 56-255
                Color color = new Color(greyValue, greyValue, greyValue);
                Vector2 position = new Vector2(random.Next(960) + 160, random.Next(720));

                //Add a big star if the time is right and there is room in the buffer
                if (random.Next(100) < percentBigStars && (pointCount + 4) < numberOfPoints)
                {
                    //Big stars are just 4 points
                    data[pointCount++] = new VertexPositionColor(new Vector3(position.X, position.Y, 0), color);
                    data[pointCount++] = new VertexPositionColor(new Vector3(position.X + 1, position.Y, 0), color);
                    data[pointCount++] = new VertexPositionColor(new Vector3(position.X, position.Y + 1, 0), color);
                    data[pointCount++] = new VertexPositionColor(new Vector3(position.X + 1, position.Y + 1, 0), color);
                }
                else
                {
                    data[pointCount++] = new VertexPositionColor(new Vector3(position.X, position.Y, 0), color);
                }
            }

            buffer.SetData<VertexPositionColor>(data);
        }
    }
}
