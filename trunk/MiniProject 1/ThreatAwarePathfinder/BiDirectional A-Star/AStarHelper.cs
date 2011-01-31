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

        public static float g(Node origNode, Node destNode, List<Agent> enemies, List<Agent> allies)
        {
            return h(origNode, destNode, enemies, allies);
        }

        public static float h(Node origNode, Node destNode, List<Agent> enemies, List<Agent> allies)
        {
            float d = AStarHelper.Dist(origNode, destNode);

            float penaltyDist = 0f;
            foreach (Agent e in enemies)
            {
                penaltyDist += e.getThreat(origNode, destNode) * (e.Radius * 2);
            }

            float bonusDist = 0f;
            foreach (Agent a in allies)
            {
                bonusDist += a.getThreat(origNode, destNode) * (a.Radius * 2);
            }

            float c1 = 0.1f;
            float c2 = 0.9f;
            return (d - penaltyDist - bonusDist) * c1 + c2 * penaltyDist;
        }
    }
}
