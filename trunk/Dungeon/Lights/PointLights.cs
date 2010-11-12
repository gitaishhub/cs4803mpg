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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PointLight : Lights
    {
        public Vector4 lightDir;
        public int is_pointlight;

        public PointLight(Game game)
            : base(game)
        {
            
        }

        public Vector4 LightDirection
        {
            get
            {
                return lightDir;
            }
            set
            {
                lightDir = value;
            }
        }

        public int Is_PointLight
        {
            get
            {
                return is_pointlight;
            }
            set
            {
                is_pointlight = value;
            }
        }
       
       
        public override void Initialize()
        {
            

            base.Initialize();
        }

        
        public override void Update(GameTime gameTime)
        {
            

            base.Update(gameTime);
        }
    }
}