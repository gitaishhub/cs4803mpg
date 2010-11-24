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

namespace Dungeon.Lights {
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Lights : Microsoft.Xna.Framework.GameComponent {
        public Vector4 position;
        public Int32 on;
        public Vector4 attenuation;
        public Vector4 lightDir;
        public int is_pointlight;

        /// <summary>
        /// The rendering cube for this light.  Used for omnidirectional lighting.
        /// </summary>
        public RenderTargetCube LightCube { get; set; }

        public Lights(Game game, int resolution)
            : base(game) {
            this.LightCube = new RenderTargetCube(game.GraphicsDevice, resolution, 1, SurfaceFormat.Single);
        }

        public Vector4 LightDirection {
            get {
                return lightDir;
            }
            set {
                lightDir = value;
            }
        }

        public int Is_PointLight {
            get {
                return is_pointlight;
            }
            set {
                is_pointlight = value;
            }
        }

        public Vector4 Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public void Flipped() {
            on = 1 - on;
        }

        public Int32 Is_on {
            get {
                return on;
            }
            set {
                on = value;
            }
        }

        public Vector4 Attenuation {
            get {
                return attenuation;
            }
            set {
                attenuation = value;
            }
        }

        public override void Initialize() {


            base.Initialize();
        }


        public override void Update(GameTime gameTime) {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        //Stripped from the internet.
        public Matrix GetViewMatrix(CubeMapFace face) {
            Vector3 pos = new Vector3(this.Position.X, this.Position.Y, this.Position.Z);
            Matrix viewMatrix = Matrix.Identity;
            switch (face) {
                case CubeMapFace.NegativeX: {
                        viewMatrix = Matrix.CreateLookAt(pos, Vector3.Left, Vector3.Up);
                        break;
                    }
                case CubeMapFace.NegativeY: {
                        viewMatrix = Matrix.CreateLookAt(pos, Vector3.Down, Vector3.Forward);
                        break;
                    }
                case CubeMapFace.NegativeZ: {
                        viewMatrix = Matrix.CreateLookAt(pos, Vector3.Backward, Vector3.Up);
                        break;
                    }
                case CubeMapFace.PositiveX: {
                        viewMatrix = Matrix.CreateLookAt(pos, Vector3.Right, Vector3.Up);
                        break;
                    }
                case CubeMapFace.PositiveY: {
                        viewMatrix = Matrix.CreateLookAt(pos, Vector3.Up, Vector3.Backward);
                        break;
                    }
                case CubeMapFace.PositiveZ: {
                        viewMatrix = Matrix.CreateLookAt(pos, Vector3.Forward, Vector3.Up);
                        break;
                    }
            }
            return viewMatrix;
        }
    }
}