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

using Dungeon.Weapons;

namespace Dungeon
{
   
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Vector3 position;
        public Vector3 lookAtVec; // I maintain this as a unit looking vector 
        public Vector3 turn;
        public float radius;
        public float move, updown;
        public Vector3 walk;
        private Random rng;

        public Game gameP;
        private Bullet bullet;
        private AudioComponent audioComponent;
        private KeyboardState oldState;

        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        public Player(Game game)
            : base(game)
        {
            gameP = game;
            this.rng = new Random();
            audioComponent = new AudioComponent(game);
            game.Components.Add(audioComponent);
        }

        public bool wall_collision(float future_loc)
        {
            if (future_loc > 195.0f || future_loc < -195.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
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


        // not used for now
        public Vector3 NormalizeXZ(Vector3 vec)
        {
            Vector2 vecXZ;

            vecXZ = new Vector2(vec.X, vec.Z);
            vecXZ = Vector2.Normalize(vecXZ);
            return new Vector3(vecXZ.X, vec.Y, vecXZ.Y);

        }

        public void Shoot()
        {

            audioComponent.PlayCue("shoot");
            bullet = new Bullet(gameP);

            bullet.Coefficient_A_Materials = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            bullet.Coefficient_D_Materials = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            bullet.Coefficient_S_Materials = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            bullet.Position = position;
            bullet.ViewMatrix = viewMatrix;
            bullet.ProjectionMatrix = projectionMatrix;
            bullet.Trajectory = Vector3.Normalize(lookAtVec);
            bullet.WorldMatrix = Matrix.Identity*Matrix.CreateTranslation(bullet.Position);
            bullet.gWVP = bullet.WorldMatrix * viewMatrix * projectionMatrix;

            bullet.BulletEffect = gameP.Content.Load<Effect>("DungeonEffect");
            bullet.BulletEffect.CurrentTechnique = bullet.BulletEffect.Techniques["myTech"];
            gameP.Components.Add(bullet);            
        }

        //public void checkBulletCollision()
        //{
        //    //Check to see if bullets have hit the enemy.
        //    //Kill it, and respawn.
        //    bool hit = false;
        //    BoundingSphere[] bounds = ((Game1)this.Game).enemy.Bounds;
        //    foreach (BoundingSphere sphere in bounds)
        //    {
        //        hit |= bullet.Bounds.Intersects(sphere);
        //    }
        //    if (hit)
        //    {
        //        Console.Out.WriteLine("Trex dead");
        //    } 
        //}

        public void Move()
        {
            KeyboardState keyboard = Keyboard.GetState();
            Vector3 tmpcam = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 new_position;
            float speed = 3;

            if (keyboard.IsKeyDown(Keys.Space) != oldState.IsKeyDown(Keys.Space))
            {
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    Shoot();
                }
                oldState = keyboard;
            }
            else if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
            {
                move -= 0.05f;
                // note that these 2 components are normalized
                // so the outcome of this will be normalized too
                lookAtVec.X = lookAtVec.X * (float)Math.Cos(move) - lookAtVec.Z * (float)Math.Sin(move);
                lookAtVec.Z = lookAtVec.X * (float)Math.Sin(move) + lookAtVec.Z * (float)Math.Cos(move);
            }
            else if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
            {
                move += 0.05f;
                lookAtVec.X = lookAtVec.X * (float)Math.Cos(move) - lookAtVec.Z * (float)Math.Sin(move);
                lookAtVec.Z = lookAtVec.X * (float)Math.Sin(move) + lookAtVec.Z * (float)Math.Cos(move);
            }
            else if (keyboard.IsKeyDown(Keys.Space))
            {
                move = 0.0f;
            }
            else if (keyboard.IsKeyDown(Keys.U))
            {
                lookAtVec.Y += (float)Math.Sin(updown);// up;              
            }
            else if (keyboard.IsKeyDown(Keys.D))
            {
                lookAtVec.Y -= (float)Math.Sin(updown);
            }
            else if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
            {
                // walk forward

                new_position.X = position.X + speed * lookAtVec.X; //*0.5f;
                new_position.Z = position.Z + speed * lookAtVec.Z; //*0.5f;

                if (wall_collision(new_position.X))
                {
                    if (wall_collision(new_position.Z))
                    {
                        // both exceed
                        return;
                    }
                    else
                    {
                        // Z is ok
                        position.Z = new_position.Z;

                    }
                }
                else
                {
                    // X is ok
                    if (wall_collision(new_position.Z))
                    {
                        position.X = new_position.X;

                        return;
                    }
                    else
                    {
                        position.X = new_position.X;
                        position.Z = new_position.Z;

                    }
                }
            }
            else if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
            {
                // walk backward

                new_position.X = position.X - speed * lookAtVec.X; // *0.5f;
                new_position.Z = position.Z - speed * lookAtVec.Z; // *0.5f;

                if (wall_collision(new_position.X))
                {
                    // X has collided
                    if (wall_collision(new_position.Z))
                    {
                        // both exceed
                        return;
                    }
                    else
                    {
                        // Z is ok
                        position.Z = new_position.Z;

                    }
                }
                else
                {
                    // X is ok
                    if (wall_collision(new_position.Z))
                    {
                        position.X = new_position.X;

                        return;
                    }
                    else
                    {
                        position.X = new_position.X;
                        position.Z = new_position.Z;

                    }
                }
            }
            else if (keyboard.IsKeyDown(Keys.Q))
            {
                // strafe left
                Vector3 left = new Vector3(lookAtVec.Z, 0, -lookAtVec.X);

                new_position.X = position.X + speed * left.X; //*0.5f;
                new_position.Z = position.Z + speed * left.Z; //*0.5f;

                if (wall_collision(new_position.X))
                {
                    if (wall_collision(new_position.Z))
                    {
                        // both exceed
                        return;
                    }
                    else
                    {
                        // Z is ok
                        position.Z = new_position.Z;

                    }
                }
                else
                {
                    // X is ok
                    if (wall_collision(new_position.Z))
                    {
                        position.X = new_position.X;

                        return;
                    }
                    else
                    {
                        position.X = new_position.X;
                        position.Z = new_position.Z;

                    }
                }
            }
            else if (keyboard.IsKeyDown(Keys.E))
            {
                // strafe left
                Vector3 right = new Vector3(-lookAtVec.Z, 0, lookAtVec.X);

                new_position.X = position.X + speed * right.X; //*0.5f;
                new_position.Z = position.Z + speed * right.Z; //*0.5f;

                if (wall_collision(new_position.X))
                {
                    if (wall_collision(new_position.Z))
                    {
                        // both exceed
                        return;
                    }
                    else
                    {
                        // Z is ok
                        position.Z = new_position.Z;

                    }
                }
                else
                {
                    // X is ok
                    if (wall_collision(new_position.Z))
                    {
                        position.X = new_position.X;

                        return;
                    }
                    else
                    {
                        position.X = new_position.X;
                        position.Z = new_position.Z;

                    }
                }
            }

            
            move = 0.0f;
            
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

        public Vector3 LookAtVec
        {
            get
            {
                return lookAtVec;
            }
            set
            {
                // normalize X and Z for future movement
                Vector2 tmpXZ;
                tmpXZ = new Vector2(value.X, value.Z);
                tmpXZ = Vector2.Normalize(tmpXZ);
                value.X = tmpXZ.X;
                value.Z = tmpXZ.Y;
                lookAtVec = value;
            }
        }

        public BoundingSphere Bounds
        {
            get
            {
                BoundingSphere bounds = new BoundingSphere(position,radius);

               return bounds;
            }
        }

        public override void Initialize()
        {
            radius = 15.0f;
            move   = 0.0f;
            updown = 0.02f;
            
            base.Initialize();
        }

        
        public override void Update(GameTime gameTime)
        {
            Vector3 cur_pos;
            
            // update for bullet's view
            for (int i = 0; i < gameP.Components.Count; i++)
            {
                if (gameP.Components[i] is Bullet)
                {
                    bullet = (Bullet)gameP.Components[i];
                    cur_pos = bullet.Position;
                    //cur_pos = ((Bullet)(gameP.Components[i])).Position;
                    // means outstanding bullet in the room
                    if (wall_collision(cur_pos.X) || wall_collision(cur_pos.Y) || wall_collision(cur_pos.X))
                    {
                        // bullet out of bound
                        gameP.Components.RemoveAt(i);
                        break;
                    }
                    else 
                    {
                        bullet.ViewMatrix = viewMatrix;
                        bullet.ProjectionMatrix = projectionMatrix;
                    }
                }
            }
            
            //Check to see if dinosaur has eaten me.
            bool hit = false;
            BoundingSphere[] bounds = ((Game1)this.Game).enemy.Bounds;
            foreach (BoundingSphere sphere in bounds)
            {
                hit |= this.Bounds.Intersects(sphere);
            }
            if (hit)
            {
                //Get an x and z at a safe distance from enemy.
                Vector3 enemyPosition = (this.Game as Game1).enemy.Position;
                Vector3 newPosition = Vector3.Zero;
                do
                {
                    newPosition.X = 200 * (float)this.rng.NextDouble();
                    newPosition.Z = 200 * (float)this.rng.NextDouble();

                } while (Vector3.DistanceSquared(enemyPosition, newPosition) < 2500);

                float x = 200 * (float)this.rng.NextDouble();
                float z = 200 * (float)this.rng.NextDouble();
                this.Position = new Vector3(x, this.Position.Y, z);
                Console.WriteLine("You Died!");
            }

            base.Update(gameTime);
        }
    }
}