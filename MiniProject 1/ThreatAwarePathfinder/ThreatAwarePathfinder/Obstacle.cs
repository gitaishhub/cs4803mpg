using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace ThreatAwarePathfinder {
    public class PolygonObstacle {

        private List<Vector2> vertices;
        private Vector2 center;

        public PolygonObstacle(Vector2 center) {
            this.center = center;
            this.ConstructRandomVertices();
        }

        private void ConstructRandomVertices() {
            Random rng = new Random();

            //Get number of vertices, 3 - 8;
            int vertexCount = rng.Next(3, 9);
            this.vertices = new List<Vector2>(vertexCount);

            float offset = MathHelper.TwoPi * (float)rng.NextDouble();
            float delta = MathHelper.TwoPi / vertexCount;
            Vector2 v = Vector2.Zero;

            for (int i = 0; i < vertexCount; i++) {
                v.X = (float)Math.Cos(i * delta + offset);
                v.Y = (float)Math.Sin(i * delta + offset);

                v *= 32 + 96 * (float)rng.NextDouble();

                this.vertices.Add(v);
            }
        }

        public bool Contains(Vector2 point) {
            //Create direction vector.
            Vector2 dir = Vector2.UnitX;
            int intersections = 0;

            for (int i = 0; i < this.vertices.Count; i++) {
                //Test ray direction dot product against both endpoints of edge.
                //If both dot products have the same sign, no intersection.
                
            }

            return (intersections / 2f == intersections / 2);
        }
    }
}
