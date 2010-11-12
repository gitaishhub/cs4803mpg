using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace Dungeon
{
    
    public class MonaLisa : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Vector3 FrontNormal = new Vector3(0.0f, 0.0f, 1.0f);
        private Vector3 RightNormal = Vector3.Right;
        private Vector3 TopNormal = Vector3.Up;
        private Vector3 LeftNormal = Vector3.Left;
        private Vector3 BackNormal = Vector3.Forward;  // Sounds weird, but it is pointing inward
        private Vector3 BottomNormal = Vector3.Down;

        private VertexDeclaration MLVertexDecl;
        private VertexPositionNormalTexture[] vertex;
        private int vertexcount;
        private short[] triangleListIndices;

        public Vector4 a_material;
        public Vector4 d_material;

        public Effect monalisaEffect;
        public EffectTechnique renderingTech;
        public Matrix worldMatrix;
        public Matrix WVP;

        private Texture2D texture_ML;

        public MonaLisa(Game game)
            : base(game)
        {
            vertex = new VertexPositionNormalTexture[4];
            vertexcount = 4;

            MLVertexDecl = new VertexDeclaration(game.GraphicsDevice,
                                                VertexPositionNormalTexture.VertexElements);

            // Mona Lisa with LightMap
            // Front
            vertex[0] = new VertexPositionNormalTexture(new Vector3( 15.0f, 100.0f, -185.0f), FrontNormal, new Vector2(0.0f, 0.0f));
            vertex[1] = new VertexPositionNormalTexture(new Vector3( 65.0f, 100.0f, -185.0f), FrontNormal, new Vector2(1.0f, 0.0f));
            vertex[2] = new VertexPositionNormalTexture(new Vector3( 65.0f,  25.0f, -185.0f), FrontNormal, new Vector2(1.0f, 1.0f));
            vertex[3] = new VertexPositionNormalTexture(new Vector3( 15.0f,  25.0f, -185.0f), FrontNormal, new Vector2(0.0f, 1.0f));

            triangleListIndices = new short[6] {   0, 1, 2, 
                                                   0, 2, 3,                                                   
                                                };

             

            texture_ML = game.Content.Load<Texture2D>("monalisa");
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

        public Effect MLEffect
        {
            get
            {
                return monalisaEffect;
            }
            set
            {
                monalisaEffect = value;
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
       
        public override void Initialize()
        {
          

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            

            monalisaEffect.Begin();

            monalisaEffect.Parameters["gWVP"].SetValue(WVP);
            monalisaEffect.Parameters["gWorld"].SetValue(WorldMatrix);
            monalisaEffect.Parameters["material"].StructureMembers["a_material"].SetValue(a_material);
            monalisaEffect.Parameters["material"].StructureMembers["d_material"].SetValue(d_material);
            monalisaEffect.Parameters["map"].SetValue(texture_ML);

            monalisaEffect.GraphicsDevice.VertexDeclaration = MLVertexDecl;

            foreach (EffectPass pass in monalisaEffect.CurrentTechnique.Passes)
            {
                pass.Begin();


                // drawing walls
                monalisaEffect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                                                    vertex,
                                                    0,
                                                    4,
                                                    triangleListIndices,
                                                    0,
                                                    2);

                // Change texture to stone picture
                //monalisEffect.Parameters["map"].SetValue(texture_stone);
                //monalisEffect.CommitChanges();

              
                pass.End();
            }
            monalisaEffect.End();

            base.Update(gameTime);
        }
    }
}