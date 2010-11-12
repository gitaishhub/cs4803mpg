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

    public class Teapot : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Model thisTeapot;
        private Effect teapotEffect;
        private Matrix worldMatrix;
        private Matrix WVP;

        private Vector3 init_position;
        private Vector3 rotation;
        private Vector3 offset;
        public Vector4 a_material;
        public Vector4 d_material;
        public Vector4 s_material;
        public Vector3 scaling;

        private int wireFrame;

        public Texture2D texture_metal;

        public Teapot(Game game)
            : base(game)
        {
            thisTeapot = game.Content.Load<Model>("teapot");

            texture_metal = game.Content.Load<Texture2D>("metal");
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

        public int WireFrame
        {
            get
            {
                return wireFrame;
            }
            set
            {
                wireFrame = value;
            }
        }

        public Vector3 Scaling
        {
            get
            {
                return scaling;
            }
            set
            {
                scaling = value;
            }
        }

        public override void Initialize()
        {
          
            rotation = new Vector3(0.0f, 0.0f, 0.0f);
            offset = new Vector3(0.02f, 0.018f, 0.033f);

            base.Initialize();
        }

        public Effect TeapotEffect
        {
            get
            {
                return teapotEffect;
            }
            set
            {
                teapotEffect = value;
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


        public override void Update(GameTime gameTime)
        {
            rotation += offset;

            worldMatrix = Matrix.CreateScale(scaling) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateRotationY(rotation.Y)
                * Matrix.CreateTranslation(init_position);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            if (wireFrame == 1)
            {
                teapotEffect.CurrentTechnique = teapotEffect.Techniques["myPlainWireTech"];
            }
            else
            {
                teapotEffect.CurrentTechnique = teapotEffect.Techniques["myPlainTech"];
            }
            teapotEffect.Parameters["gWVP"].SetValue(WVP);
            teapotEffect.Parameters["gWorld"].SetValue(worldMatrix);
            teapotEffect.Parameters["material"].StructureMembers["a_material"].SetValue(a_material);
            teapotEffect.Parameters["material"].StructureMembers["d_material"].SetValue(d_material);
            teapotEffect.Parameters["material"].StructureMembers["s_material"].SetValue(s_material);
        
   
            foreach (ModelMesh mesh in thisTeapot.Meshes)
            {

                teapotEffect.Begin();
                foreach (EffectPass pass in teapotEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    pass.End();
                }

                teapotEffect.End();

                foreach (ModelMeshPart part in thisTeapot.Meshes[0].MeshParts)
                {

                    part.Effect = teapotEffect;

                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}