using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

using Dungeon.Weapons;

namespace Dungeon
{

    public class Enemy : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //public Vector3 lookAtVec; // I maintain this as a unit looking vector 
        //public Vector3 turn;
        //public float radius;
        public float move, updown;
        public Vector3 walk;
        private Matrix world;
        private Model model;
        public Effect Effect { get; set; }

        public Enemy(Game game)
            : base(game)
        {
            //gameP = game;

            //this.Position = 4 * Vector3.Backward;
        }

        public Matrix WorldMatrix
        {
            get { return this.world; }
            set { this.world = value; }
        }

        public Matrix ViewMatrix
        {
            get;
            set;
        }

        public Matrix ProjectionMatrix
        {
            get;
            set;
        }

        public Vector3 Position {
            get
            {
                return this.world.Translation;
            }
            set
            {
                this.world.Translation = value;
            }
        }

        public override void Initialize()
        {
            this.model = this.Game.Content.Load<Model>("Project4/Enemy/dino videogame");
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Vector3 cur_pos;

            //// update for bullet's view
            //for (int i = 0; i < Game.Components.Count; i++)
            //{
            //    if (Game.Components[i] is Bullet)
            //    {
            //        cur_pos = ((Bullet)(Game.Components[i])).Position;
            //        // means outstanding bullet in the room
            //        if (wall_collision(cur_pos.X) || wall_collision(cur_pos.Y) || wall_collision(cur_pos.X))
            //        {
            //            // bullet out of bound
            //            Game.Components.RemoveAt(i);
            //            break;
            //        }
            //        else
            //        {
            //            bullet.ViewMatrix = viewMatrix;
            //            bullet.ProjectionMatrix = projectionMatrix;
            //        }
            //    }
            //}

            base.Update(gameTime);
        }

        public void Move()
        {

        }

        public override void Draw(GameTime gameTime)
        {
            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = this.ProjectionMatrix;
                    effect.View = this.ViewMatrix;
                    effect.World = this.WorldMatrix;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}