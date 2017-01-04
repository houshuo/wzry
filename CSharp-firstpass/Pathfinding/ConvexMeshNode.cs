namespace Pathfinding
{
    using System;
    using UnityEngine;

    public class ConvexMeshNode : MeshNode
    {
        private int[] indices;
        protected static INavmeshHolder[] navmeshHolders = new INavmeshHolder[0];

        public ConvexMeshNode(AstarPath astar) : base(astar)
        {
            this.indices = new int[0];
        }

        public override Vector3 ClosestPointOnNode(Vector3 p)
        {
            throw new NotImplementedException();
        }

        public override Vector3 ClosestPointOnNodeXZ(Vector3 p)
        {
            throw new NotImplementedException();
        }

        public override VInt3 ClosestPointOnNodeXZ(VInt3 p)
        {
            throw new NotImplementedException();
        }

        public override void GetConnections(GraphNodeDelegate del)
        {
            if (base.connections != null)
            {
                for (int i = 0; i < base.connections.Length; i++)
                {
                    del(base.connections[i]);
                }
            }
        }

        protected static INavmeshHolder GetNavmeshHolder(uint graphIndex)
        {
            return navmeshHolders[graphIndex];
        }

        public override VInt3 GetVertex(int i)
        {
            return GetNavmeshHolder(base.GraphIndex).GetVertex(this.GetVertexIndex(i));
        }

        public override int GetVertexCount()
        {
            return this.indices.Length;
        }

        public int GetVertexIndex(int i)
        {
            return this.indices[i];
        }

        public override void Open(Path path, PathNode pathNode, PathHandler handler)
        {
            if (base.connections != null)
            {
                for (int i = 0; i < base.connections.Length; i++)
                {
                    GraphNode node = base.connections[i];
                    if (path.CanTraverse(node))
                    {
                        PathNode node2 = handler.GetPathNode(node);
                        if (node2.pathID != handler.PathID)
                        {
                            node2.parent = pathNode;
                            node2.pathID = handler.PathID;
                            node2.cost = base.connectionCosts[i];
                            node2.H = path.CalculateHScore(node);
                            node.UpdateG(path, node2);
                            handler.PushNode(node2);
                        }
                        else
                        {
                            uint num2 = base.connectionCosts[i];
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

        public void SetPosition(VInt3 p)
        {
            base.position = p;
        }
    }
}

