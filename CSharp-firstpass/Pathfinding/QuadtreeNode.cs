namespace Pathfinding
{
    using System;

    public class QuadtreeNode : GraphNode
    {
        public uint[] connectionCosts;
        public GraphNode[] connections;

        public QuadtreeNode(AstarPath astar) : base(astar)
        {
        }

        public override void AddConnection(GraphNode node, uint cost)
        {
            throw new NotImplementedException("QuadTree Nodes do not have support for adding manual connections");
        }

        public override void ClearConnections(bool alsoReverse)
        {
            if (alsoReverse)
            {
                for (int i = 0; i < this.connections.Length; i++)
                {
                    this.connections[i].RemoveConnection(this);
                }
            }
            this.connections = null;
            this.connectionCosts = null;
        }

        public override void GetConnections(GraphNodeDelegate del)
        {
            if (this.connections != null)
            {
                for (int i = 0; i < this.connections.Length; i++)
                {
                    del(this.connections[i]);
                }
            }
        }

        public override void Open(Path path, PathNode pathNode, PathHandler handler)
        {
            if (this.connections != null)
            {
                for (int i = 0; i < this.connections.Length; i++)
                {
                    GraphNode node = this.connections[i];
                    if (path.CanTraverse(node))
                    {
                        PathNode node2 = handler.GetPathNode(node);
                        if (node2.pathID != handler.PathID)
                        {
                            node2.node = node;
                            node2.parent = pathNode;
                            node2.pathID = handler.PathID;
                            node2.cost = this.connectionCosts[i];
                            node2.H = path.CalculateHScore(node);
                            node.UpdateG(path, node2);
                            handler.PushNode(node2);
                        }
                        else
                        {
                            uint num2 = this.connectionCosts[i];
                            if (((pathNode.G + num2) + path.GetTraversalCost(node)) < node2.G)
                            {
                                node2.cost = num2;
                                node2.parent = pathNode;
                                node.UpdateRecursiveG(path, node2, handler);
                            }
                            else if ((((node2.G + num2) + path.GetTraversalCost(this)) < pathNode.G) && node.ContainsConnection(this))
                            {
                                pathNode.parent = node2;
                                pathNode.cost = num2;
                                this.UpdateRecursiveG(path, pathNode, handler);
                            }
                        }
                    }
                }
            }
        }

        public override void RemoveConnection(GraphNode node)
        {
            throw new NotImplementedException("QuadTree Nodes do not have support for adding manual connections");
        }

        public void SetPosition(VInt3 value)
        {
            base.position = value;
        }
    }
}

