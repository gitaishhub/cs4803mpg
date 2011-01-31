using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BiDirectional_A_Star
{
    public class Agent
    {
        public Vector2 Pos { get; set; }
        public float Radius { get; set; }
        public BoundingSphere ThreatArea {
            get {
                return new BoundingSphere(new Vector3(this.Pos, 0), this.Radius);
            }
        }

        public Agent(Vector2 position, float radius)
        {
            this.Pos = position;
            this.Radius = radius;
        }

        public float getThreat(Node origin, Node dest)
        {
            Vector2 d = dest.Pos - origin.Pos;
            Vector2 f = origin.Pos - this.Pos;

            float a = d.X * d.X + d.Y * d.Y;
            float b = 2 * (f.X * d.X + f.Y * d.Y);
            float c = f.X * f.X + f.Y * f.Y - Radius * Radius;

            float discr = b * b - 4 * a * c;
            if (discr < 0) { return 0; }

            discr = (float)Math.Sqrt(discr);

            float t1 = (-b + discr) / (2 * a);
            float t2 = (-b - discr) / (2 * a);

            Vector2 dir = dest.Pos - origin.Pos;

            Vector2 inter0 = origin.Pos;
            if (t1 >= 0 && t1 <= 1)
            {
                inter0 = origin.Pos + dir * t1;
            }
            Vector2 inter1 = origin.Pos;
            if (t2 >= 0 && t2 <= 1)
            {
                inter1 = origin.Pos + dir * t2;
            }

            return (inter0 - inter1).Length() / (ThreatArea.Radius * 2.0f);
        }
    }
}
