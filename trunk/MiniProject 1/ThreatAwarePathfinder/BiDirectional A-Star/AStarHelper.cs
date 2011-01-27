using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BiDirectional_A_Star
{
    static class AStarHelper
    {
        public static float Dist(Node a, Node b)
        {
            return Vector2.Distance(a.Pos, b.Pos);
        }

        public static float CalcDistByThreat(Node origNode, Node destNode, List<Agent> enemies, List<Agent> allies)
        {
            float d = AStarHelper.Dist(origNode, destNode);

            List<float> penalties = new List<float>();
            foreach (Agent e in enemies)
            {
                float p = e.getThreat(origNode, destNode);
                if (p > 0) { penalties.Add(p); }
            }

            List<float> bonuses = new List<float>();
            foreach (Agent a in allies)
            {
                float b = a.getThreat(origNode, destNode);
                if (b > 0) { bonuses.Add(b); }
            }

            // figure it out
            return d;
        }
    }
}
