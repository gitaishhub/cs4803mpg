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
    
    public class DiffuseLights : Lights 
    {
        public Vector4 c_ambient;
        public Vector4 c_diffuse;
     
        public DiffuseLights(Game game)
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

        
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}