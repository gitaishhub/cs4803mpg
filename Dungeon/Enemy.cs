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
        //public Vector3 turn;
        //public float radius;
        private Random rng;
        private Matrix world;
        private Matrix rotation;
        private Model model;
        public Effect Effect { get; set; }

        public Enemy(Game game)
            : base(game)
        {
            this.world = Matrix.CreateScale(64) * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateTranslation(new Vector3(0, 26, -128));
            this.rng = new Random();
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

        public Vector3 Position
        {
            get
            {
                return this.world.Translation;
            }
            set
            {
                this.world.Translation = value;
            }
        }

        public BoundingSphere[] Bounds
        {
            get
            {
                BoundingSphere[] bounds = new BoundingSphere[this.model.Meshes.Count];

                //This is a pretty dirty way to do this, but whatever.
                //These are bounding spheres, so there's no need to worry about any rotations.
                Matrix[] transforms = new Matrix[this.model.Bones.Count];
                this.model.CopyAbsoluteBoneTransformsTo(transforms);
                Matrix translation = Matrix.CreateTranslation(this.Position);

                //Add each sphere to the array and return.
                for (int i = 0; i < this.model.Meshes.Count; i++)
                {
                    bounds[i] = this.model.Meshes[i].BoundingSphere.Transform(transforms[this.model.Meshes[i].ParentBone.Index] * this.rotation * this.world);
                }

                return bounds;
            }
        }

        public override void Initialize()
        {
            this.model = this.Game.Content.Load<Model>("Project4/Enemy/dino videogame");
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            this.Move();
            this.checkCollision();

            base.Update(gameTime);
        }

        public void Move()
        {
            Vector3 direction = ((Game1)this.Game).Arnold.Position - this.Position;
            this.Position += 0.001f * direction;

            float phi = (float)Math.Atan2(direction.X, direction.Z);
            this.rotation = Matrix.CreateRotationY(phi);
        }

        public void checkCollision()
        {
            //Check to see if bullets have hit me.
            //Kill me, respawn me.
            foreach (GameComponent component in this.Game.Components)
            {
                if (!(component is Bullet)) { continue; }

                Bullet bullet = component as Bullet;
                Enemy enemy = component as Enemy;
                bool hit = false;
                BoundingSphere bounds = bullet.Bounds;
                foreach (BoundingSphere sphere in this.Bounds)
                {
                    hit |= sphere.Intersects(bounds);
                }
                if (hit)
                {
                    Console.Out.WriteLine("Trex Died");
                    Game.Components.Remove(bullet);

                    //Get an x and z at a safe distance from player.
                    Vector3 playerPosition = (this.Game as Game1).Arnold.Position;
                    Vector3 newPosition = Vector3.Zero;
                    do{
                        newPosition.X = 200 * (float)this.rng.NextDouble();
                        newPosition.Z = 200 * (float)this.rng.NextDouble();
                        
                    } while (Vector3.DistanceSquared(playerPosition, newPosition) < 2500);

                    float x = 200 * (float)this.rng.NextDouble();
                    float z = 200 * (float)this.rng.NextDouble();
                    this.Position = new Vector3(x, 26, z);
                    break;                    
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[this.model.Bones.Count];
            this.model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = this.ProjectionMatrix;
                    effect.View = this.ViewMatrix;
                    effect.World = transforms[mesh.ParentBone.Index] * this.rotation * this.world;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}