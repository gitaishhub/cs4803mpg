﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BiDirectional_A_Star
{
    class Agent
    {
        public Vector2 Pos { get; set; }
        public BoundingSphere ThreatArea { get; set; }

        public Agent(BoundingSphere threatArea)
        {
            this.ThreatArea = threatArea;
        }

        public float getThreat(Node origin, Node dest)
        {
            Vector3 dir = new Vector3(dest.Pos - origin.Pos, 0);

            //Get intersection A.
            Ray ray1 = new Ray(new Vector3(origin.Pos, 0), dir);
            float? enter = this.ThreatArea.Intersects(ray1);
            if (enter == null) return 0;
            Vector3 intersectionA = ray1.Position + ray1.Direction * (float)enter;

            //Get intersection B.
            Ray ray2 = new Ray(intersectionA + 0.001f * dir, dir);
            float? exit = this.ThreatArea.Intersects(ray2);
            if (exit == null) return 0;
            Vector3 intersectionB = ray2.Position + ray2.Direction * (float)exit;

            return (intersectionA - intersectionB).Length() / (ThreatArea.Radius * 2.0f);
        }
    }
}
