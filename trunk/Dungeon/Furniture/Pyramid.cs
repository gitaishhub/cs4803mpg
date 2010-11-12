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
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Dungeon.Furniture
{
 
    public class Pyramid : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Vector3 FrontNormal = new Vector3(0.0f, 0.0f, 1.0f);
        private Vector3 RightNormal = Vector3.Right;
        private Vector3 TopNormal = Vector3.Up;
        private Vector3 LeftNormal = Vector3.Left;
        private Vector3 BackNormal = Vector3.Forward;  // Sounds weird, but it is pointing inward
        private Vector3 BottomNormal = Vector3.Down;

        private VertexDeclaration PyramidVertexDecl;
        private VertexPositionNormalTexture[] vertex;
        private int vertexcount;
        private short[] triangleListIndices;
        private int transparent;
        private int is_bullet;

        public Vector4 a_material;
        public Vector4 d_material;
        public Vector4 s_material;

        public Matrix worldMatrix;
        public Matrix WVP;

        private Vector3 init_position;
        private Vector3 rotation;
        private Vector3 offset;

        public Effect  pyramidEffect;

        public Texture2D texture_metal;

        public Pyramid(Game game)
            : base(game)
        {
            vertex = new VertexPositionNormalTexture[12];
            vertexcount = 12;

            PyramidVertexDecl = new VertexDeclaration(game.GraphicsDevice,
                                                VertexPositionNormalTexture.VertexElements);

            // Base
            vertex[0] = new VertexPositionNormalTexture(new Vector3(-20.0f,  0.0f, -20.0f), BottomNormal, new Vector2(0.0f, 3.0f));
            vertex[1] = new VertexPositionNormalTexture(new Vector3( 20.0f,  0.0f, -20.0f), BottomNormal, new Vector2(3.0f, 3.0f));
            vertex[2] = new VertexPositionNormalTexture(new Vector3( 20.0f,  0.0f,  20.0f), BottomNormal, new Vector2(3.0f, 0.0f));
            vertex[3] = new VertexPositionNormalTexture(new Vector3(-20.0f,  0.0f,  20.0f), BottomNormal, new Vector2(0.0f, 0.0f));
            
            // Face 1 and 2
            vertex[4] = new VertexPositionNormalTexture(new Vector3(  0.0f, 30.0f,   0.0f), TopNormal, new Vector2(0.0f, 0.0f));
            vertex[5] = new VertexPositionNormalTexture(new Vector3( 20.0f,  0.0f, -20.0f), new Vector3(1, 0, -1), new Vector2(3.0f, 3.0f));
            vertex[6] = new VertexPositionNormalTexture(new Vector3(20.0f, 0.0f, 20.0f), new Vector3(1, 0, 1), new Vector2(1.5f, 3.0f));
            vertex[7] = new VertexPositionNormalTexture(new Vector3(-20.0f,  0.0f,  20.0f), new Vector3(-1, 0, 1), new Vector2(0.0f, 3.0f));

            // Face 3 and 4
            vertex[8] = new VertexPositionNormalTexture(new Vector3(0.0f, 30.0f, 0.0f), TopNormal, new Vector2(0.0f, 0.0f));
            vertex[9] = new VertexPositionNormalTexture(new Vector3(-20.0f, 0.0f, 20.0f), new Vector3(-1, 0, 1), new Vector2(3.0f, 3.0f));
            vertex[10] = new VertexPositionNormalTexture(new Vector3(-20.0f, 0.0f, -20.0f), new Vector3(-1, 0, -1), new Vector2(1.5f, 3.0f));
            vertex[11] = new VertexPositionNormalTexture(new Vector3(20.0f, 0.0f, -20.0f), new Vector3(1, 0, -1), new Vector2(0.0f, 3.0f));




            triangleListIndices = new short[18] {   0, 2, 1, 
                                                  0, 3, 2, 
                                                  4, 5, 6,
                                                  4, 6, 7,
                                                  8, 9, 10, 
                                                  8, 10, 11
            };

            texture_metal = game.Content.Load<Texture2D>("metal2");
        }

        public Vector3 Init_position
        {
            get
            {
                return init_position;
            }
            set
            {
                init_position = value;
            }
        }


        public int Transparent
        {
            get
            {
                return transparent;
            }
            set
            {
                transparent = value;
            }
        }

        public Matrix WorldMatrix
        {
            get
            {
                return worldMatrix;
            }
            set
            {
                worldMatrix = value;
            }
        }

        public Matrix gWVP
        {
            get
            {
                return WVP;
            }
            set
            {
                WVP = value;
            }
        }

        public Vector4 Coefficient_A_Materials
        {
            get
            {
                return a_material;
            }
            set
            {
                a_material = value;
            }
        }

        public Vector4 Coefficient_D_Materials
        {
            get
            {
                return d_material;
            }
            set
            {
                d_material = value;
            }
        }


        public Vector4 Coefficient_S_Materials
        {
            get
            {
                return s_material;
            }
            set
            {
                s_material = value;
            }
        }

        public Effect PyramidEffect
        {
            get
            {
                return pyramidEffect;
            }
            set
            {
                pyramidEffect = value;
            }
        }


        public override void Initialize()
        {
            rotation = new Vector3(0.0f, 0.0f, 0.0f);
            offset   = new Vector3(0.01f, 0.015f, 0.023f);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            rotation += offset;

            worldMatrix = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateRotationY(rotation.Y) 
                * Matrix.CreateTranslation(init_position);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            pyramidEffect.CurrentTechnique = pyramidEffect.Techniques["myTech"];
            pyramidEffect.GraphicsDevice.VertexDeclaration = PyramidVertexDecl;
            pyramidEffect.Parameters["gWVP"].SetValue(WVP);
            pyramidEffect.Parameters["gWorld"].SetValue(worldMatrix);
            pyramidEffect.Parameters["material"].StructureMembers["a_material"].SetValue(a_material);
            pyramidEffect.Parameters["material"].StructureMembers["d_material"].SetValue(d_material);
            pyramidEffect.Parameters["material"].StructureMembers["s_material"].SetValue(s_material);
            pyramidEffect.Parameters["map"].SetValue(texture_metal); // will change inside the pass

            if (transparent == 1)
            {
                GraphicsDevice.RenderState.AlphaBlendEnable = true;
                GraphicsDevice.RenderState.DepthBufferEnable = false;
                GraphicsDevice.RenderState.SourceBlend = Blend.One;
                GraphicsDevice.RenderState.DestinationBlend = Blend.One;
                GraphicsDevice.RenderState.CullMode = CullMode.None;
                GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
            }
            pyramidEffect.Begin();
            foreach (EffectPass pass in pyramidEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                pyramidEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                    vertex,
                                                    0,
                                                    12,
                                                    triangleListIndices,
                                                    0,
                                                    6);
                pass.End();
            }
            pyramidEffect.End();

            if (transparent == 1)
            {
                GraphicsDevice.RenderState.AlphaBlendEnable = false;
                GraphicsDevice.RenderState.DepthBufferEnable = true;
                GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            }
        }

    }
}