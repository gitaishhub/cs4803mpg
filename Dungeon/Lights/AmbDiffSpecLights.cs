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

namespace Dungeon.Lights
{
    
    public class AmbDiffSpecLights : Lights
    {
        public Vector4 c_ambient;
        public Vector4 c_diffuse;
        public Vector4 c_specular;
        public float shininess;

        public AmbDiffSpecLights(Game game)
            : base(game)
        {
            
        }

        public Vector4 Coefficient_ambient
        {
            get
            {
                return c_ambient;
            }
            set
            {
                c_ambient = value;
            }
        }

        public Vector4 Coefficient_diffuse
        {
            get
            {
                return c_diffuse;
            }
            set
            {
                c_diffuse = value;
            }
        }

        public Vector4 Coefficient_specular
        {
            get
            {
                return c_specular;
            }
            set
            {
                c_specular = value;
            }
        }

        public float Shininess
        {
            get
            {
                return shininess;
            }
            set
            {
                shininess = value;
            }
        }

        public override void Initialize()
        {
            

            base.Initialize();
        }

      
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}