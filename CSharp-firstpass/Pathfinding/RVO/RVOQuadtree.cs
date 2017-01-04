namespace Pathfinding.RVO
{
    using Pathfinding.RVO.Sampled;
    using System;
    using System.Runtime.InteropServices;

    public class RVOQuadtree
    {
        private VRect bounds;
        private int filledNodes = 1;
        private const int LeafSize = 15;
        private long maxRadius;
        private Node[] nodes = new Node[0x2a];

        public void Clear()
        {
            this.nodes[0] = new Node();
            this.filledNodes = 1;
            this.maxRadius = 0L;
        }

        public int GetNodeIndex()
        {
            if (this.filledNodes == this.nodes.Length)
            {
                Node[] nodeArray = new Node[this.nodes.Length * 2];
                for (int i = 0; i < this.nodes.Length; i++)
                {
                    nodeArray[i] = this.nodes[i];
                }
                this.nodes = nodeArray;
            }
            this.nodes[this.filledNodes] = new Node();
            this.nodes[this.filledNodes].child00 = this.filledNodes;
            this.filledNodes++;
            return (this.filledNodes - 1);
        }

        public void Insert(Agent agent)
        {
            int index = 0;
            VRect bounds = this.bounds;
            VInt2 xz = agent.position.xz;
            agent.next = null;
            this.maxRadius = IntMath.Max((long) agent.radius.i, this.maxRadius);
            int num3 = 0;
            while (true)
            {
                num3++;
                if (this.nodes[index].child00 == index)
                {
                    if ((this.nodes[index].count < 15) || (num3 > 10))
                    {
                        this.nodes[index].Add(agent);
                        this.nodes[index].count = (byte) (this.nodes[index].count + 1);
                        return;
                    }
                    Node node = this.nodes[index];
                    node.child00 = this.GetNodeIndex();
                    node.child01 = this.GetNodeIndex();
                    node.child10 = this.GetNodeIndex();
                    node.child11 = this.GetNodeIndex();
                    this.nodes[index] = node;
                    this.nodes[index].Distribute(this.nodes, bounds);
                }
                if (this.nodes[index].child00 != index)
                {
                    VInt2 center = bounds.center;
                    if (xz.x > center.x)
                    {
                        if (xz.y > center.y)
                        {
                            index = this.nodes[index].child11;
                            bounds = VRect.MinMaxRect(center.x, center.y, bounds.xMax, bounds.yMax);
                        }
                        else
                        {
                            index = this.nodes[index].child10;
                            bounds = VRect.MinMaxRect(center.x, bounds.yMin, bounds.xMax, center.y);
                        }
                    }
                    else if (xz.y > center.y)
                    {
                        index = this.nodes[index].child01;
                        bounds = VRect.MinMaxRect(bounds.xMin, center.y, center.x, bounds.yMax);
                    }
                    else
                    {
                        index = this.nodes[index].child00;
                        bounds = VRect.MinMaxRect(bounds.xMin, bounds.yMin, center.x, center.y);
                    }
                }
            }
        }

        public void Query(VInt2 p, long radius, Agent agent)
        {
            this.QueryRec(0, p, radius, agent, this.bounds);
        }

        private long QueryRec(int i, VInt2 p, long radius, Agent agent, VRect r)
        {
            if (this.nodes[i].child00 == i)
            {
                for (Agent agent2 = this.nodes[i].linkedList; agent2 != null; agent2 = agent2.next)
                {
                    long a = agent.InsertAgentNeighbour(agent2, radius * radius);
                    if (a < (radius * radius))
                    {
                        radius = IntMath.Sqrt(a);
                    }
                }
                return radius;
            }
            VInt2 center = r.center;
            if ((p.x - radius) < center.x)
            {
                if ((p.y - radius) < center.y)
                {
                    radius = this.QueryRec(this.nodes[i].child00, p, radius, agent, VRect.MinMaxRect(r.xMin, r.yMin, center.x, center.y));
                }
                if ((p.y + radius) > center.y)
                {
                    radius = this.QueryRec(this.nodes[i].child01, p, radius, agent, VRect.MinMaxRect(r.xMin, center.y, center.x, r.yMax));
                }
            }
            if ((p.x + radius) > center.x)
            {
                if ((p.y - radius) < center.y)
                {
                    radius = this.QueryRec(this.nodes[i].child10, p, radius, agent, VRect.MinMaxRect(center.x, r.yMin, r.xMax, center.y));
                }
                if ((p.y + radius) > center.y)
                {
                    radius = this.QueryRec(this.nodes[i].child11, p, radius, agent, VRect.MinMaxRect(center.x, center.y, r.xMax, r.yMax));
                }
            }
            return radius;
        }

        public void SetBounds(VRect r)
        {
            this.bounds = r;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Node
        {
            public int child00;
            public int child01;
            public int child10;
            public int child11;
            public byte count;
            public Agent linkedList;
            public void Add(Agent agent)
            {
                agent.next = this.linkedList;
                this.linkedList = agent;
            }

            public void Distribute(RVOQuadtree.Node[] nodes, VRect r)
            {
                VInt2 center = r.center;
                while (this.linkedList != null)
                {
                    Agent next = this.linkedList.next;
                    if (this.linkedList.position.x > center.x)
                    {
                        if (this.linkedList.position.z > center.y)
                        {
                            nodes[this.child11].Add(this.linkedList);
                        }
                        else
                        {
                            nodes[this.child10].Add(this.linkedList);
                        }
                    }
                    else if (this.linkedList.position.z > center.y)
                    {
                        nodes[this.child01].Add(this.linkedList);
                    }
                    else
                    {
                        nodes[this.child00].Add(this.linkedList);
                    }
                    this.linkedList = next;
                }
                this.count = 0;
            }
        }
    }
}

