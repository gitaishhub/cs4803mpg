using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BiDirectional_A_Star
{
    public class BiDirectionAStar
    {
        private Node startNode;
        private Node destNode;

        public List<Agent> Allies
        {
            set {
                startToDest.Allies = value;
                destToStart.Allies = value;
            }
        }
        public List<Agent> Enemies
        {
            set
            {
                startToDest.Enemies = value;
                destToStart.Enemies = value;
            }
        }

        private AStar startToDest;
        private AStar destToStart;

        private List<Node> answer;
        public List<Node> answer0;
        public List<Node> answer1;

        public BiDirectionAStar(Node start, Node dest)
        {
            this.startNode = start;
            this.destNode = dest;
            this.startToDest = new AStar(start, dest);
            this.destToStart = new AStar(dest, start);
        }

        public List<Node> Step()
        {
            if (answer != null) { return answer; }

            float stdBest = startToDest.CalcBestDistance();
            float dtsBest = destToStart.CalcBestDistance();

            Node expandedNode = null;
            if (stdBest <= dtsBest)
            {
                expandedNode = startToDest.Step();
            }
            else
            {
                expandedNode = destToStart.Step();
            }

            List<Node> soFar = new List<Node>();
            int count = 0;
            foreach (Node cameFrom in expandedNode.CameFrom)
            {
                if (count == 0)
                {
                    answer0 = getCameFrom(cameFrom, count);
                    answer0.Add(expandedNode);
                    soFar.AddRange(answer0);
                }
                else if (count == 1)
                {
                    answer1 = getCameFrom(cameFrom, count);
                    soFar.AddRange(answer1);
                }
                count++;
            }
            if (count > 1)
            {
                answer = soFar;
            }

            // reconstruct path based on frontier nodes from the two AStar objects
            return soFar;
        }

        private List<Node> getCameFrom(Node n, int count)
        {
            List<Node> temp = new List<Node>();
            if (count == 1)
            {
                temp.Add(n);
            }
            if (n.CameFrom.Count > 0)
            {
                temp.AddRange(getCameFrom(n.CameFrom[0], count));
            }
            if (count == 0)
            {
                temp.Add(n);
            }
            return temp;
        }

        public List<Node> Solve()
        {
            while (answer == null)
            {
                Step();
            }
            return answer;
        }
    }
}
