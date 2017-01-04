namespace Pathfinding
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class QuadtreeGraph : NavGraph
    {
        public Vector3 center;
        public int editorHeightLog2 = 6;
        public int editorWidthLog2 = 6;
        public LayerMask layerMask = -1;
        private BitArray map;
        public int minDepth = 3;
        public float nodeSize = 1f;
        private QuadtreeNodeHolder root;

        public void AddNeighboursRec(ListView<QuadtreeNode> arr, QuadtreeNodeHolder holder, int depth, int x, int y, IntRect bounds, QuadtreeNode dontInclude)
        {
            int num = ((int) 1) << (Math.Min(this.editorHeightLog2, this.editorWidthLog2) - depth);
            IntRect a = new IntRect(x, y, x + num, y + num);
            if (IntRect.Intersects(a, bounds))
            {
                if (holder.node != null)
                {
                    if (holder.node != dontInclude)
                    {
                        arr.Add(holder.node);
                    }
                }
                else
                {
                    this.AddNeighboursRec(arr, holder.c0, depth + 1, x, y, bounds, dontInclude);
                    this.AddNeighboursRec(arr, holder.c1, depth + 1, x + (num / 2), y, bounds, dontInclude);
                    this.AddNeighboursRec(arr, holder.c2, depth + 1, x + (num / 2), y + (num / 2), bounds, dontInclude);
                    this.AddNeighboursRec(arr, holder.c3, depth + 1, x, y + (num / 2), bounds, dontInclude);
                }
            }
        }

        public bool CheckCollision(int x, int y)
        {
            return !Physics.CheckSphere(this.LocalToWorldPosition(x, y, 1), this.nodeSize * 1.4142f, (int) this.layerMask);
        }

        public int CheckNode(int xs, int ys, int width)
        {
            Debug.Log(string.Concat(new object[] { "Checking Node ", xs, " ", ys, " width: ", width }));
            bool flag = this.map[xs + (ys * this.Width)];
            for (int i = xs; i < (xs + width); i++)
            {
                for (int j = ys; j < (ys + width); j++)
                {
                    if (this.map[i + (j * this.Width)] != flag)
                    {
                        return -1;
                    }
                }
            }
            return (!flag ? 0 : 1);
        }

        public void CreateNodeRec(QuadtreeNodeHolder holder, int depth, int x, int y)
        {
            int num2;
            int width = ((int) 1) << (Math.Min(this.editorHeightLog2, this.editorWidthLog2) - depth);
            if (depth < this.minDepth)
            {
                num2 = -1;
            }
            else
            {
                num2 = this.CheckNode(x, y, width);
            }
            if (((num2 == 1) || (num2 == 0)) || (width == 1))
            {
                QuadtreeNode node = new QuadtreeNode(base.active);
                node.SetPosition((VInt3) this.LocalToWorldPosition(x, y, width));
                node.Walkable = num2 == 1;
                holder.node = node;
            }
            else
            {
                holder.c0 = new QuadtreeNodeHolder();
                holder.c1 = new QuadtreeNodeHolder();
                holder.c2 = new QuadtreeNodeHolder();
                holder.c3 = new QuadtreeNodeHolder();
                this.CreateNodeRec(holder.c0, depth + 1, x, y);
                this.CreateNodeRec(holder.c1, depth + 1, x + (width / 2), y);
                this.CreateNodeRec(holder.c2, depth + 1, x + (width / 2), y + (width / 2));
                this.CreateNodeRec(holder.c3, depth + 1, x, y + (width / 2));
            }
        }

        public void DrawRec(QuadtreeNodeHolder h, int depth, int x, int y, Vector3 parentPos)
        {
            int width = ((int) 1) << (Math.Min(this.editorHeightLog2, this.editorWidthLog2) - depth);
            Vector3 start = this.LocalToWorldPosition(x, y, width);
            Debug.DrawLine(start, parentPos, Color.red);
            if (h.node != null)
            {
                Debug.DrawRay(start, Vector3.down, !h.node.Walkable ? Color.yellow : Color.green);
            }
            else
            {
                this.DrawRec(h.c0, depth + 1, x, y, start);
                this.DrawRec(h.c1, depth + 1, x + (width / 2), y, start);
                this.DrawRec(h.c2, depth + 1, x + (width / 2), y + (width / 2), start);
                this.DrawRec(h.c3, depth + 1, x, y + (width / 2), start);
            }
        }

        public override void GetNodes(GraphNodeDelegateCancelable del)
        {
            if (this.root != null)
            {
                this.root.GetNodes(del);
            }
        }

        public Vector3 LocalToWorldPosition(int x, int y, int width)
        {
            return new Vector3((x + (width * 0.5f)) * this.nodeSize, 0f, (y + (width * 0.5f)) * this.nodeSize);
        }

        public override void OnDrawGizmos(bool drawNodes)
        {
            base.OnDrawGizmos(drawNodes);
            if (drawNodes && (this.root != null))
            {
                this.DrawRec(this.root, 0, 0, 0, Vector3.zero);
            }
        }

        public QuadtreeNode QueryPoint(int qx, int qy)
        {
            if (this.root == null)
            {
                return null;
            }
            QuadtreeNodeHolder root = this.root;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            while (root.node == null)
            {
                int num4 = ((int) 1) << (Math.Min(this.editorHeightLog2, this.editorWidthLog2) - num);
                if (qx >= (num2 + (num4 / 2)))
                {
                    num2 += num4 / 2;
                    if (qy >= (num3 + (num4 / 2)))
                    {
                        num3 += num4 / 2;
                        root = root.c2;
                    }
                    else
                    {
                        root = root.c1;
                    }
                }
                else if (qy >= (num3 + (num4 / 2)))
                {
                    num3 += num4 / 2;
                    root = root.c3;
                }
                else
                {
                    root = root.c0;
                }
                num++;
            }
            return root.node;
        }

        public void RecalculateConnections(QuadtreeNodeHolder holder, int depth, int x, int y)
        {
            if (this.root == null)
            {
                throw new InvalidOperationException("Graph contains no nodes");
            }
            if (holder.node == null)
            {
                throw new ArgumentException("No leaf node specified. Holder has no node.");
            }
            int num = ((int) 1) << (Math.Min(this.editorHeightLog2, this.editorWidthLog2) - depth);
            ListLinqView<QuadtreeNode> arr = new ListLinqView<QuadtreeNode>();
            IntRect rect = new IntRect(x, y, x + num, y + num);
            this.AddNeighboursRec(arr, this.root, 0, 0, 0, rect.Expand(0), holder.node);
            holder.node.connections = arr.ToArray();
            holder.node.connectionCosts = new uint[arr.Count];
            for (int i = 0; i < arr.Count; i++)
            {
                VInt3 num4 = arr[i].position - holder.node.position;
                holder.node.connectionCosts[i] = (uint) num4.costMagnitude;
            }
        }

        public void RecalculateConnectionsRec(QuadtreeNodeHolder holder, int depth, int x, int y)
        {
            if (holder.node != null)
            {
                this.RecalculateConnections(holder, depth, x, y);
            }
            else
            {
                int num = ((int) 1) << (Math.Min(this.editorHeightLog2, this.editorWidthLog2) - depth);
                this.RecalculateConnectionsRec(holder.c0, depth + 1, x, y);
                this.RecalculateConnectionsRec(holder.c1, depth + 1, x + (num / 2), y);
                this.RecalculateConnectionsRec(holder.c2, depth + 1, x + (num / 2), y + (num / 2));
                this.RecalculateConnectionsRec(holder.c3, depth + 1, x, y + (num / 2));
            }
        }

        public override void ScanInternal(OnScanStatus statusCallback)
        {
            this.Width = ((int) 1) << this.editorWidthLog2;
            this.Height = ((int) 1) << this.editorHeightLog2;
            this.map = new BitArray(this.Width * this.Height);
            for (int i = 0; i < this.Width; i++)
            {
                for (int j = 0; j < this.Height; j++)
                {
                    this.map.Set(i + (j * this.Width), this.CheckCollision(i, j));
                }
            }
            QuadtreeNodeHolder holder = new QuadtreeNodeHolder();
            this.CreateNodeRec(holder, 0, 0, 0);
            this.root = holder;
            this.RecalculateConnectionsRec(this.root, 0, 0, 0);
        }

        public int Height { get; protected set; }

        public int Width { get; protected set; }
    }
}

