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
    
    public class Room : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Vector3 FrontNormal = new Vector3(0.0f, 0.0f, 1.0f);
        private Vector3 RightNormal = Vector3.Right;
        private Vector3 TopNormal = Vector3.Up;
        private Vector3 LeftNormal = Vector3.Left;
        private Vector3 BackNormal = Vector3.Forward;  // Sounds weird, but it is pointing inward
        private Vector3 BottomNormal = Vector3.Down;
        private Vector3 initial_position;
        public  float morphrate;
        private int  morphdir;

        private VertexDeclaration WallVertexDecl;
        private VertexPositionNormalTexture[] vertex;
        private int vertexcount;
        private short[] triangleListIndices;


        private Texture2D texture_sky, texture_brick, texture_lightmap;
        private Texture2D flagstoneDiffuse, flagstoneSpecular, flagstoneNormal;
        //private Texture2D texture_monalisa, texture_monalisa_spook, texture_monalisa_warp;

        public Vector4 a_material;
        public Vector4 d_material;
        public Vector4 s_material;

        public Effect wallEffect;
        public EffectTechnique renderingTech;
        public Matrix worldMatrix;
        public Matrix spinMatrix;
        public Matrix WVP;
        public Matrix spinWVP;
        
        public int numLights;

        public Room(Game game)
            : base(game)
        {
           
            vertex = new VertexPositionNormalTexture[44];
            vertexcount = 44;

            WallVertexDecl = new VertexDeclaration(game.GraphicsDevice,
                                                VertexPositionNormalTexture.VertexElements);        
            
            // Left 
            vertex[0] = new VertexPositionNormalTexture(new Vector3(-200.0f, 200.0f, -200.0f), RightNormal, new Vector2(15.0f,  0.0f));
            vertex[1] = new VertexPositionNormalTexture(new Vector3(-200.0f,   0.0f, -200.0f), RightNormal, new Vector2(15.0f, 15.0f));
            vertex[2] = new VertexPositionNormalTexture(new Vector3(-200.0f,   0.0f,  200.0f), RightNormal, new Vector2( 0.0f, 15.0f));
            vertex[3] = new VertexPositionNormalTexture(new Vector3(-200.0f, 200.0f,  200.0f), RightNormal, new Vector2( 0.0f,  0.0f));

            // Near 
            vertex[4] = new VertexPositionNormalTexture(new Vector3(-200.0f, 200.0f,  200.0f), BackNormal, new Vector2( 0.0f,  0.0f));
            vertex[5] = new VertexPositionNormalTexture(new Vector3(-200.0f,   0.0f,  200.0f), BackNormal, new Vector2( 0.0f, 15.0f));
            vertex[6] = new VertexPositionNormalTexture(new Vector3( 200.0f,   0.0f,  200.0f), BackNormal, new Vector2(15.0f, 15.0f));
            vertex[7] = new VertexPositionNormalTexture(new Vector3( 200.0f, 200.0f,  200.0f), BackNormal, new Vector2(15.0f,  0.0f));

            // Right
            vertex[8]  = new VertexPositionNormalTexture(new Vector3( 200.0f, 200.0f, 200.0f), LeftNormal, new Vector2(15.0f,  0.0f));
            vertex[9]  = new VertexPositionNormalTexture(new Vector3( 200.0f, 0.0f,   200.0f), LeftNormal, new Vector2(15.0f, 15.0f));
            vertex[10] = new VertexPositionNormalTexture(new Vector3( 200.0f, 0.0f,  -200.0f), LeftNormal, new Vector2( 0.0f, 15.0f));
            vertex[11] = new VertexPositionNormalTexture(new Vector3( 200.0f, 200.0f,-200.0f), LeftNormal, new Vector2( 0.0f,  0.0f));

            // Far
            vertex[12] = new VertexPositionNormalTexture(new Vector3( 200.0f, 200.0f, -200.0f), FrontNormal, new Vector2(15.0f,  0.0f));
            vertex[13] = new VertexPositionNormalTexture(new Vector3( 200.0f,   0.0f, -200.0f), FrontNormal, new Vector2(15.0f, 15.0f));
            vertex[14] = new VertexPositionNormalTexture(new Vector3(-200.0f,   0.0f, -200.0f), FrontNormal, new Vector2( 0.0f, 15.0f));
            vertex[15] = new VertexPositionNormalTexture(new Vector3(-200.0f, 200.0f, -200.0f), FrontNormal, new Vector2( 0.0f,  0.0f));

            // Ground
            vertex[16] = new VertexPositionNormalTexture(new Vector3( 200.0f,   0.0f, -200.0f), TopNormal, new Vector2(10.0f,  0.0f));
            vertex[17] = new VertexPositionNormalTexture(new Vector3( 200.0f,   0.0f,  200.0f), TopNormal, new Vector2(10.0f, 10.0f));
            vertex[18] = new VertexPositionNormalTexture(new Vector3(-200.0f,   0.0f,  200.0f), TopNormal, new Vector2( 0.0f, 10.0f));
            vertex[19] = new VertexPositionNormalTexture(new Vector3(-200.0f,   0.0f, -200.0f), TopNormal, new Vector2( 0.0f,  0.0f));

            // Sky
            vertex[20] = new VertexPositionNormalTexture(new Vector3( 200.0f,  200.0f, -200.0f), BottomNormal, new Vector2(1.0f, 1.0f));
            vertex[21] = new VertexPositionNormalTexture(new Vector3( 200.0f,  200.0f,  200.0f), BottomNormal, new Vector2(1.0f, 0.0f));
            vertex[22] = new VertexPositionNormalTexture(new Vector3(-200.0f,  200.0f,  200.0f), BottomNormal, new Vector2(0.0f, 0.0f));
            vertex[23] = new VertexPositionNormalTexture(new Vector3(-200.0f,  200.0f, -200.0f), BottomNormal, new Vector2(0.0f, 1.0f));

            // Glowing Mona Lisa
            vertex[24] = new VertexPositionNormalTexture(new Vector3(15.0f, 90.0f, -185.0f), FrontNormal, new Vector2(0.0f, 0.0f));
            vertex[25] = new VertexPositionNormalTexture(new Vector3(65.0f, 90.0f, -185.0f), FrontNormal, new Vector2(1.0f, 0.0f));
            vertex[26] = new VertexPositionNormalTexture(new Vector3(65.0f, 15.0f, -185.0f),  FrontNormal, new Vector2(1.0f, 1.0f));
            vertex[27] = new VertexPositionNormalTexture(new Vector3(15.0f, 15.0f, -185.0f),  FrontNormal, new Vector2(0.0f, 1.0f));

            // Mona Lisa
            vertex[28] = new VertexPositionNormalTexture(new Vector3(-65.0f, 90.0f, -185.0f), FrontNormal, new Vector2(0.0f, 0.0f));
            vertex[29] = new VertexPositionNormalTexture(new Vector3(-15.0f, 90.0f, -185.0f), FrontNormal, new Vector2(1.0f, 0.0f));
            vertex[30] = new VertexPositionNormalTexture(new Vector3(-15.0f, 15.0f, -185.0f), FrontNormal, new Vector2(1.0f, 1.0f));
            vertex[31] = new VertexPositionNormalTexture(new Vector3(-65.0f, 15.0f, -185.0f), FrontNormal, new Vector2(0.0f, 1.0f));

            // Mona Lisa Spikey
            vertex[32] = new VertexPositionNormalTexture(new Vector3(25.0f, 90.0f, 185.0f), BackNormal, new Vector2(0.0f, 0.0f));
            vertex[33] = new VertexPositionNormalTexture(new Vector3(-25.0f, 90.0f, 185.0f), BackNormal, new Vector2(1.0f, 0.0f));
            vertex[34] = new VertexPositionNormalTexture(new Vector3(-25.0f, 15.0f, 185.0f), BackNormal, new Vector2(1.0f, 1.0f));
            vertex[35] = new VertexPositionNormalTexture(new Vector3(25.0f, 15.0f, 185.0f), BackNormal, new Vector2(0.0f, 1.0f));

            // Rotating Mona Lisa front face
            // this one is declared in its object space, not the coordinates of the world space
            // Not the best way to insert this object.. ..
            vertex[36] = new VertexPositionNormalTexture(new Vector3(-25.0f,  38.0f, 0.0f), FrontNormal, new Vector2(0.0f, 0.0f));
            vertex[37] = new VertexPositionNormalTexture(new Vector3( 25.0f,  38.0f, 0.0f), FrontNormal, new Vector2(1.0f, 0.0f));
            vertex[38] = new VertexPositionNormalTexture(new Vector3( 25.0f, -38.0f, 0.0f), FrontNormal, new Vector2(1.0f, 1.0f));
            vertex[39] = new VertexPositionNormalTexture(new Vector3(-25.0f, -38.0f, 0.0f), FrontNormal, new Vector2(0.0f, 1.0f));
            vertex[40] = new VertexPositionNormalTexture(new Vector3( 25.0f,  38.0f, 0.0f), BackNormal, new Vector2(0.0f, 0.0f));
            vertex[41] = new VertexPositionNormalTexture(new Vector3( 25.0f, -38.0f, 0.0f), BackNormal, new Vector2(0.0f, 1.0f));
            vertex[42] = new VertexPositionNormalTexture(new Vector3(-25.0f, -38.0f, 0.0f), BackNormal, new Vector2(1.0f, 1.0f));
            vertex[43] = new VertexPositionNormalTexture(new Vector3(-25.0f,  38.0f, 0.0f), BackNormal, new Vector2(1.0f, 0.0f));

            
            triangleListIndices = new short[66] {   0, 1, 3, 
                                                    1, 2, 3, 
                                                    4, 6, 7, 
                                                    4, 5, 6, 
                                                    8, 9, 11, 
                                                    11,  9, 10,
                                                    12, 13, 14,
                                                    14, 15, 12,
                                                    16, 17, 18,
                                                    16, 18, 19,
                                                    20, 22, 21,
                                                    20, 23, 22,
                                                    24, 25, 26,
                                                    24, 26, 27,
                                                    28, 29, 30,
                                                    28, 30, 31,
                                                    32, 33, 34,
                                                    32, 34, 35,
                                                    36, 37, 38,
                                                    36, 38, 39,
                                                    40, 43, 41,
                                                    43, 42, 41
                                                };

             

            texture_brick = game.Content.Load<Texture2D>("brick");
            texture_sky = game.Content.Load<Texture2D>("sky");
            //texture_stone = game.Content.Load<Texture2D>("stone");
            texture_lightmap = game.Content.Load<Texture2D>("lightmap");
            //texture_monalisa = game.Content.Load<Texture2D>("monalisa");
            //texture_monalisa_spook = game.Content.Load<Texture2D>("monalisa_horn");
            //texture_monalisa_warp = game.Content.Load<Texture2D>("monalisa_warp");

            this.flagstoneDiffuse = game.Content.Load<Texture2D>("Project4/flagstoneDiffuse");
            this.flagstoneNormal = game.Content.Load<Texture2D>("Project4/flagstoneNormal");
            this.flagstoneSpecular = game.Content.Load<Texture2D>("Project4/flagstoneSpecular");
        }

        public float MorphRate
        {
            get
            {
                return morphrate;
            }
            set
            {
                morphrate = value;
            }
        }


        public int NumOfLights
        {
            get
            {
                return numLights;
            }
            set
            {
                numLights = value;
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
        
        public EffectTechnique RenderingTech
        {
            get
            {
                return renderingTech;
            }
            set
            {
                renderingTech = value;
            }
        }

        public Effect WallEffect
        {
            get
            {
                return wallEffect;
            }
            set
            {
                wallEffect = value;
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

        public Matrix SpinMatrix
        {
            get
            {
                return spinMatrix;
            }
            set
            {
                spinMatrix = value;
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

        public Matrix gSpinWVP
        {
            get
            {
                return spinWVP;
            }
            set
            {
                spinWVP = value;
            }
        }


        protected void Morphing()
        {
            morphrate += morphdir * 0.006f;
            if (morphrate > 1.79f)
            {
                morphdir = morphdir - 2;
            }
            else if (morphrate <= -0.5f)
            {
                morphdir = morphdir + 2;
            }
        }


        protected void Spin()
        {
                
        }

        public override void Initialize()
        {
            morphdir = 1;

            base.Initialize();
        }

      
        public override void Update(GameTime gameTime)
        {

            Morphing();
            Spin();


            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            wallEffect.CurrentTechnique = WallEffect.Techniques["myTech"];
            wallEffect.GraphicsDevice.VertexDeclaration = WallVertexDecl;

            wallEffect.Parameters["gWVP"].SetValue(WVP);
            wallEffect.Parameters["gWorld"].SetValue(WorldMatrix);

            wallEffect.Parameters["material"].StructureMembers["a_material"].SetValue(a_material);
            wallEffect.Parameters["material"].StructureMembers["d_material"].SetValue(d_material);
            wallEffect.Parameters["material"].StructureMembers["s_material"].SetValue(s_material);

            wallEffect.Parameters["map"].SetValue(texture_brick); // will change inside the pass
            //wallEffect.Parameters["map2"].SetValue(texture_monalisa_spook);
            wallEffect.Parameters["morphrate"].SetValue(morphrate);
           
            
            wallEffect.Begin();           
            foreach (EffectPass pass in wallEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
    
                 // drawing walls
                wallEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                    vertex,
                                                    0,
                                                    16,
                                                    triangleListIndices,
                                                    0,
                                                    8);

                // Change texture to stone picture
                wallEffect.Parameters["map"].SetValue(this.flagstoneDiffuse);
                wallEffect.CommitChanges();

                // drawing ground
                wallEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                    vertex,
                                                    0,
                                                    24,
                                                    triangleListIndices,
                                                    24,
                                                    2);


                // Change texture to cosmo picture
                wallEffect.Parameters["map"].SetValue(texture_sky);
                wallEffect.CommitChanges();
                // drawing sky
                wallEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                    vertex,
                                                    0,
                                                    24,
                                                    triangleListIndices,
                                                    30,
                                                    2);

                // Change texture to mona lisa
                //wallEffect.Parameters["map"].SetValue(texture_monalisa);
                //wallEffect.CommitChanges();
               

                //// drawing the regular mona lisa
                //wallEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                //                                    vertex,
                //                                    0,
                //                                    32,
                //                                    triangleListIndices,
                //                                    42,
                //                                    2);                
                
                pass.End();
            }
            wallEffect.End();

            //wallEffect.CurrentTechnique = WallEffect.Techniques["LightMapTech"];

            //wallEffect.Parameters["map2"].SetValue(texture_lightmap);
            //wallEffect.Begin();
            //foreach (EffectPass pass in wallEffect.CurrentTechnique.Passes)
            //{
            //    pass.Begin();

            //    // drawing the growling mona lisa
            //    wallEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            //                                        vertex,
            //                                        0,
            //                                        28,
            //                                        triangleListIndices,
            //                                        36,
            //                                        2);


            //    pass.End();
            //}
            //wallEffect.End();


            // Drawing the spooky mona lisa
            //wallEffect.CurrentTechnique = WallEffect.Techniques["MorphTech"];
            //wallEffect.Parameters["map2"].SetValue(texture_monalisa_spook);

            //wallEffect.Begin();
            //foreach (EffectPass pass in wallEffect.CurrentTechnique.Passes)
            //{
            //    pass.Begin();

            //    wallEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            //                                        vertex,
            //                                        0,
            //                                        36,
            //                                        triangleListIndices,
            //                                        48,
            //                                        2);


            //    pass.End();
            //}
            //wallEffect.End();


            // Drawing the rotating mona lisa
            //wallEffect.CurrentTechnique = WallEffect.Techniques["myTech"];
            //wallEffect.Parameters["gWorld"].SetValue(spinMatrix);
            //wallEffect.Parameters["gWVP"].SetValue(spinWVP);
            //wallEffect.Begin();
            //foreach (EffectPass pass in wallEffect.CurrentTechnique.Passes)
            //{
            //    pass.Begin();

            //    wallEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            //                                        vertex,
            //                                        0,
            //                                        40,
            //                                        triangleListIndices,
            //                                        54,
            //                                        2);


            //    // Change texture to cosmo picture
            //    wallEffect.Parameters["map"].SetValue(texture_monalisa_warp);
            //    wallEffect.CommitChanges();
            //    wallEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            //                                       vertex,
            //                                       0,
            //                                       44,
            //                                       triangleListIndices,
            //                                       60,
            //                                       2);


            //    pass.End();
            //}
            //wallEffect.End();

            base.Draw(gameTime);
        }
    }
}