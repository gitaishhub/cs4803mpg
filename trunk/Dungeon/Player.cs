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

        public void Move()
        {
            KeyboardState keyboard = Keyboard.GetState();
            Vector3 tmpcam = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 new_position;


            if (keyboard.IsKeyDown(Keys.Space) != oldState.IsKeyDown(Keys.Space))
            {
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    Shoot();
                }
                oldState = keyboard;
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                move -= 0.01f;
                // note that these 2 components are normalized
                // so the outcome of this will be normalized too
                lookAtVec.X = lookAtVec.X * (float)Math.Cos(move) - lookAtVec.Z * (float)Math.Sin(move);
                lookAtVec.Z = lookAtVec.X * (float)Math.Sin(move) + lookAtVec.Z * (float)Math.Cos(move);
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                move += 0.01f;
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
            else if (keyboard.IsKeyDown(Keys.Up))
            {
                // walk forward

                new_position.X = position.X + lookAtVec.X; // *0.5f;
                new_position.Z = position.Z + lookAtVec.Z; // *0.5f;

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
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                // walk backward

                new_position.X = position.X - lookAtVec.X; // *0.5f;
                new_position.Z = position.Z - lookAtVec.Z; // *0.5f;

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

        public override void Initialize()
        {
            radius = 200.0f;
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
                    cur_pos = ((Bullet)(gameP.Components[i])).Position;
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

            


            base.Update(gameTime);
        }
    }
}