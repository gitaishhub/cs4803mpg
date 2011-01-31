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

        public List<Node> answer { get; private set; }
        public List<Node> stdSoFar { get; private set; }
        public List<Node> stdFrontier { get; private set; }
        public List<Node> dtsSoFar { get; private set; }
        public List<Node> dtsFrontier { get; private set; }

        private int counter;

        public BiDirectionAStar(Node start, Node dest)
        {
            this.startNode = start;
            this.destNode = dest;
            this.startToDest = new AStar(start, dest);
            this.destToStart = new AStar(dest, start);

            answer = new List<Node>();
            stdSoFar = new List<Node>();
            dtsSoFar = new List<Node>();
            stdFrontier = this.startToDest.FrontierNodes;
            dtsFrontier = this.destToStart.FrontierNodes;

            counter = 0;
        }

        public List<Node> Step()
        {
            if (answer.Count > 0) { return answer; }

            float stdBest = startToDest.CalcBestDistance();
            float dtsBest = destToStart.CalcBestDistance();

            Node expandedNode = null;
            String chosen = "";
            if (counter % 2 == 0/*stdBest <= dtsBest*/)
            {
                chosen = "std";
                expandedNode = startToDest.Step(chosen);
                stdFrontier = startToDest.FrontierNodes;
            }
            else
            {
                chosen = "dts";
                expandedNode = destToStart.Step(chosen);
                dtsFrontier = destToStart.FrontierNodes;
            }
            counter++;

            List<Node> stdFrom = new List<Node>();
            if (expandedNode.CameFrom.ContainsKey("std")) {
                stdFrom = getCameFrom(expandedNode.CameFrom["std"], "std");
                stdSoFar = stdFrom;
            }
            List<Node> dtsFrom = new List<Node>();
            if (expandedNode.CameFrom.ContainsKey("dts")) {
                dtsFrom = getCameFrom(expandedNode.CameFrom["dts"], "dts");
                dtsSoFar = dtsFrom;
            }

            List<Node> soFar = new List<Node>();
            soFar.AddRange(stdFrom);
            soFar.Add(expandedNode);
            soFar.AddRange(dtsFrom);

            if (expandedNode.CameFrom.Count > 1)
            {
                answer = soFar;
            }

            return soFar;
        }

        private List<Node> getCameFrom(Node n, string index)
        {
            List<Node> temp = new List<Node>();

            if (index.Equals("std"))
            {
                temp.Add(n);
                if (n.CameFrom.ContainsKey(index))
                {
                    temp.AddRange(getCameFrom(n.CameFrom[index], index));
                }
            }
            else if (index.Equals("dts"))
            {
                if (n.CameFrom.ContainsKey(index))
                {
                    temp.AddRange(getCameFrom(n.CameFrom[index], index));
                }
                temp.Add(n);
            }

            return temp;
        }

        public List<Node> Solve()
        {
            while (answer.Count == 0)
            {
                Step();
            }
            return answer;
        }
    }
}
