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

namespace Dungeon.Weapons
{

    public class Bullet : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Vector3 FrontNormal = new Vector3(0.0f, 0.0f, 1.0f);
        private Vector3 RightNormal = Vector3.Right;
        private Vector3 TopNormal = Vector3.Up;
        private Vector3 LeftNormal = Vector3.Left;
        private Vector3 BackNormal = Vector3.Forward;  // Sounds weird, but it is pointing inward
        private Vector3 BottomNormal = Vector3.Down;

        private VertexDeclaration BulletVertexDecl;
        private VertexPositionNormalTexture[] vertex;
        private int vertexcount;
        private short[] triangleListIndices;
        
        public Vector4 a_material;
        public Vector4 d_material;
        public Vector4 s_material;

        public Matrix worldMatrix;
        public Matrix WVP;

        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        private Vector3 position;
        private float radius;
        private Vector3 trajectory;

        public Effect bulletEffect;

        public Texture2D texture_metal;

        public Bullet(Game game)
            : base(game)
        {
            vertex = new VertexPositionNormalTexture[12];
            vertexcount = 12;

            BulletVertexDecl = new VertexDeclaration(game.GraphicsDevice,
                                                VertexPositionNormalTexture.VertexElements);

            // Base
            vertex[0] = new VertexPositionNormalTexture(new Vector3(-2.0f, 0.0f, -2.0f), BottomNormal, new Vector2(0.0f, 1.0f));
            vertex[1] = new VertexPositionNormalTexture(new Vector3(2.0f, 0.0f, -2.0f), BottomNormal, new Vector2(1.0f, 1.0f));
            vertex[2] = new VertexPositionNormalTexture(new Vector3(2.0f, 0.0f, 2.0f), BottomNormal, new Vector2(1.0f, 0.0f));
            vertex[3] = new VertexPositionNormalTexture(new Vector3(-2.0f, 0.0f, 2.0f), BottomNormal, new Vector2(0.0f, 0.0f));

            // Face 1 and 2
            vertex[4] = new VertexPositionNormalTexture(new Vector3(0.0f, 3.0f, 0.0f), TopNormal, new Vector2(0.0f, 0.0f));
            vertex[5] = new VertexPositionNormalTexture(new Vector3(2.0f, 0.0f, -2.0f), new Vector3(1, 0, -1), new Vector2(1.0f, 1.0f));
            vertex[6] = new VertexPositionNormalTexture(new Vector3(2.0f, 0.0f, 2.0f), new Vector3(1, 0, 1), new Vector2(0.5f, 1.0f));
            vertex[7] = new VertexPositionNormalTexture(new Vector3(-2.0f, 0.0f, 2.0f), new Vector3(-1, 0, 1), new Vector2(0.0f, 1.0f));

            // Face 3 and 4
            vertex[8] = new VertexPositionNormalTexture(new Vector3(0.0f, 3.0f, 0.0f), TopNormal, new Vector2(0.0f, 0.0f));
            vertex[9] = new VertexPositionNormalTexture(new Vector3(-2.0f, 0.0f, 2.0f), new Vector3(-1, 0, 1), new Vector2(1.0f, 1.0f));
            vertex[10] = new VertexPositionNormalTexture(new Vector3(-2.0f, 0.0f, -2.0f), new Vector3(-1, 0, -1), new Vector2(0.5f, 1.0f));
            vertex[11] = new VertexPositionNormalTexture(new Vector3(2.0f, 0.0f, -2.0f), new Vector3(1, 0, -1), new Vector2(0.0f, 1.0f));




            triangleListIndices = new short[18] {   0, 2, 1, 
                                                  0, 3, 2, 
                                                  4, 5, 6,
                                                  4, 6, 7,
                                                  8, 9, 10, 
                                                  8, 10, 11
            };

            texture_metal = game.Content.Load<Texture2D>("metal3");
        }

        public Matrix ViewMatrix
        {
            get
            {
                return viewMatrix;
            }
            set
            {
                viewMatrix = value;
            }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                return projectionMatrix;
            }
            set
            {
                projectionMatrix = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public Vector3 Trajectory
        {
            get
            {
                return trajectory;
            }
            set
            {
                trajectory = value;
            }
        }

        public BoundingSphere Bounds
        {
            get
            {
                BoundingSphere bounds = new BoundingSphere(position, radius);

                return bounds;
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

        public Effect BulletEffect
        {
            get
            {
                return bulletEffect;
            }
            set
            {
                bulletEffect = value;
            }
        }


        public override void Initialize()
        {
            radius = 10;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            position = position + trajectory * 6.0f;
            worldMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(90))*Matrix.CreateTranslation(position);
            WVP = worldMatrix * viewMatrix * projectionMatrix;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            bulletEffect.CurrentTechnique = bulletEffect.Techniques["myTech"];
            bulletEffect.GraphicsDevice.VertexDeclaration = BulletVertexDecl;
            bulletEffect.Parameters["gWVP"].SetValue(WVP);
            bulletEffect.Parameters["gWorld"].SetValue(worldMatrix);
            bulletEffect.Parameters["material"].StructureMembers["a_material"].SetValue(a_material);
            bulletEffect.Parameters["material"].StructureMembers["d_material"].SetValue(d_material);
            bulletEffect.Parameters["material"].StructureMembers["s_material"].SetValue(s_material);
            bulletEffect.Parameters["map"].SetValue(texture_metal); // will change inside the pass

            
            BulletEffect.Begin();
            foreach (EffectPass pass in BulletEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                bulletEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                    vertex,
                                                    0,
                                                    12,
                                                    triangleListIndices,
                                                    0,
                                                    6);
                pass.End();
            }
            bulletEffect.End();

            
        }

    }
}