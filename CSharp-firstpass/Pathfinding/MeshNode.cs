namespace Pathfinding
{
    using Pathfinding.Serialization;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class MeshNode : GraphNode
    {
        public uint[] connectionCosts;
        public GraphNode[] connections;

        protected MeshNode()
        {
        }

        public MeshNode(AstarPath astar) : base(astar)
        {
        }

        public override void AddConnection(GraphNode node, uint cost)
        {
            if (this.connections != null)
            {
                for (int i = 0; i < this.connections.Length; i++)
                {
                    if (this.connections[i] == node)
                    {
                        this.connectionCosts[i] = cost;
                        return;
                    }
                }
            }
            int index = (this.connections == null) ? 0 : this.connections.Length;
            GraphNode[] nodeArray = new GraphNode[index + 1];
            uint[] numArray = new uint[index + 1];
            if (this.connections != null)
            {
                for (int j = 0; j < index; j++)
                {
                    nodeArray[j] = this.connections[j];
                    numArray[j] = this.connectionCosts[j];
                }
            }
            nodeArray[index] = node;
            numArray[index] = cost;
            this.connections = nodeArray;
            this.connectionCosts = numArray;
        }

        public override void ClearConnections(bool alsoReverse)
        {
            if (alsoReverse && (this.connections != null))
            {
                for (int i = 0; i < this.connections.Length; i++)
                {
                    this.connections[i].RemoveConnection(this);
                }
            }
            this.connections = null;
            this.connectionCosts = null;
        }

        public abstract Vector3 ClosestPointOnNode(Vector3 p);
        public abstract Vector3 ClosestPointOnNodeXZ(Vector3 p);
        public abstract VInt3 ClosestPointOnNodeXZ(VInt3 p);
        public override bool ContainsConnection(GraphNode node)
        {
            for (int i = 0; i < this.connections.Length; i++)
            {
                if (this.connections[i] == node)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool ContainsPoint(VInt3 p)
        {
            bool flag = false;
            int vertexCount = this.GetVertexCount();
            int i = 0;
            for (int j = vertexCount - 1; i < vertexCount; j = i++)
            {
                if ((((this.GetVertex(i).z <= p.z) && (p.z < this.GetVertex(j).z)) || ((this.GetVertex(j).z <= p.z) && (p.z < this.GetVertex(i).z))) && (p.x < ((((this.GetVertex(j).x - this.GetVertex(i).x) * (p.z - this.GetVertex(i).z)) / (this.GetVertex(j).z - this.GetVertex(i).z)) + this.GetVertex(i).x)))
                {
                    flag = !flag;
                }
            }
            return flag;
        }

        public override void DeserializeReferences(GraphSerializationContext ctx)
        {
            int num = ctx.reader.ReadInt32();
            if (num == -1)
            {
                this.connections = null;
                this.connectionCosts = null;
            }
            else
            {
                this.connections = new GraphNode[num];
                this.connectionCosts = new uint[num];
                for (int i = 0; i < num; i++)
                {
                    this.connections[i] = ctx.GetNodeFromIdentifier(ctx.reader.ReadInt32());
                    this.connectionCosts[i] = ctx.reader.ReadUInt32();
                }
            }
        }

        protected override void Duplicate(GraphNode graphNode)
        {
            base.Duplicate(graphNode);
            MeshNode node = (MeshNode) graphNode;
            if (this.connectionCosts != null)
            {
                node.connectionCosts = new uint[this.connectionCosts.Length];
                Array.Copy(this.connectionCosts, node.connectionCosts, this.connectionCosts.Length);
            }
        }

        public override void FloodFill(Stack<GraphNode> stack, uint region)
        {
            if (this.connections != null)
            {
                for (int i = 0; i < this.connections.Length; i++)
                {
                    GraphNode t = this.connections[i];
                    if (t.Area != region)
                    {
                        t.Area = region;
                        stack.Push(t);
                    }
                }
            }
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

        public abstract VInt3 GetVertex(int i);
        public abstract int GetVertexCount();
        public override void RemoveConnection(GraphNode node)
        {
            if (this.connections != null)
            {
                for (int i = 0; i < this.connections.Length; i++)
                {
                    if (this.connections[i] == node)
                    {
                        int length = this.connections.Length;
                        GraphNode[] nodeArray = new GraphNode[length - 1];
                        uint[] numArray = new uint[length - 1];
                        for (int j = 0; j < i; j++)
                        {
                            nodeArray[j] = this.connections[j];
                            numArray[j] = this.connectionCosts[j];
                        }
                        for (int k = i + 1; k < length; k++)
                        {
                            nodeArray[k - 1] = this.connections[k];
                            numArray[k - 1] = this.connectionCosts[k];
                        }
                        this.connections = nodeArray;
                        this.connectionCosts = numArray;
                        return;
                    }
                }
            }
        }

        public override void SerializeReferences(GraphSerializationContext ctx)
        {
            if (this.connections == null)
            {
                ctx.writer.Write(-1);
            }
            else
            {
                ctx.writer.Write(this.connections.Length);
                for (int i = 0; i < this.connections.Length; i++)
                {
                    ctx.writer.Write(ctx.GetNodeIdentifier(this.connections[i]));
                    ctx.writer.Write(this.connectionCosts[i]);
                }
            }
        }

        public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
        {
            base.UpdateG(path, pathNode);
            handler.PushNode(pathNode);
            for (int i = 0; i < this.connections.Length; i++)
            {
                GraphNode node = this.connections[i];
                PathNode node2 = handler.GetPathNode(node);
                if ((node2.parent == pathNode) && (node2.pathID == handler.PathID))
                {
                    node.UpdateRecursiveG(path, node2, handler);
                }
            }
        }
    }
}

