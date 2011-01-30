﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiDirectional_A_Star
{
    public class AStar
    {
        private Node startNode;
        private Node destNode;

        public List<Node> FrontierNodes { get; set; }
        private List<Node> visitedNodes;
        public List<Agent> Allies { get; set; }
        public List<Agent> Enemies { get; set; }

        public AStar(Node start, Node dest)
        {
            this.startNode = start;
            this.destNode = dest;
            this.Allies = new List<Agent>();
            this.Enemies = new List<Agent>();

            this.FrontierNodes = new List<Node>();
            this.FrontierNodes.Add(start);
            start.G = 0;

            this.visitedNodes = new List<Node>();
        }

        public float CalcBestDistance()
        {
            float min = float.PositiveInfinity;
            foreach (Node node in FrontierNodes)
            {
                float d = (float)node.G + node.h(destNode, Enemies, Allies);
                if (d < min)
                {
                    min = d;
                }
            }
            return min;
        }

        public Node Step()
        {
            float min = float.PositiveInfinity;
            Node best = null;
            foreach (Node node in FrontierNodes)
            {
                float d = (float)node.G + node.h(destNode, Enemies, Allies);
                if (d < min)
                {
                    min = d;
                    best = node;
                }
            }

            FrontierNodes.Remove(best);
            visitedNodes.Add(best);
            foreach (Node neighbor in best.Neighbors)
            {
                if (!FrontierNodes.Contains(neighbor) && !visitedNodes.Contains(neighbor))
                {
                    float h = AStarHelper.CalcDistByThreat(best, neighbor, Enemies, Allies);
                    neighbor.G = best.G + h;
                    FrontierNodes.Add(neighbor);
                    neighbor.CameFrom.Add(best);
                }
            }

            return best;
        }
    }
}
