namespace Pathfinding
{
    using Pathfinding.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class TriangleMeshNode : MeshNode
    {
        protected static ListView<INavmeshHolder[]> _navmeshHolders = new ListView<INavmeshHolder[]>();
        private static VInt3[] _staticVerts = new VInt3[3];
        public int v0;
        public int v1;
        public int v2;

        protected TriangleMeshNode()
        {
        }

        public TriangleMeshNode(AstarPath astar) : base(astar)
        {
        }

        private void CalcNearestPoint(out VInt3 cp, ref VInt3 start, ref VInt3 end, ref VInt3 p)
        {
            VInt2 b = new VInt2(end.x - start.x, end.z - start.z);
            long sqrMagnitudeLong = b.sqrMagnitudeLong;
            VInt2 a = new VInt2(p.x - start.x, p.z - start.z);
            cp = new VInt3();
            cp.y = p.y;
            long num4 = VInt2.DotLong(ref a, ref b);
            if (sqrMagnitudeLong != 0)
            {
                long num5 = (end.x - start.x) * num4;
                long num6 = (end.z - start.z) * num4;
                cp.x = (int) IntMath.Divide(num5, sqrMagnitudeLong);
                cp.z = (int) IntMath.Divide(num6, sqrMagnitudeLong);
                cp.x += start.x;
                cp.z += start.z;
            }
            else
            {
                int num7 = (int) num4;
                cp.x = start.x + ((end.x - start.x) * num7);
                cp.z = start.z + ((end.z - start.z) * num7);
            }
        }

        public TriangleMeshNode Clone()
        {
            TriangleMeshNode graphNode = new TriangleMeshNode();
            this.Duplicate(graphNode);
            return graphNode;
        }

        public override Vector3 ClosestPointOnNode(Vector3 p)
        {
            INavmeshHolder navmeshHolder = GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex);
            return Polygon.ClosestPointOnTriangle((Vector3) navmeshHolder.GetVertex(this.v0), (Vector3) navmeshHolder.GetVertex(this.v1), (Vector3) navmeshHolder.GetVertex(this.v2), p);
        }

        public override Vector3 ClosestPointOnNodeXZ(Vector3 _p)
        {
            INavmeshHolder navmeshHolder = GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex);
            VInt3 vertex = navmeshHolder.GetVertex(this.v0);
            VInt3 lineEnd = navmeshHolder.GetVertex(this.v1);
            VInt3 num3 = navmeshHolder.GetVertex(this.v2);
            VInt3 point = (VInt3) _p;
            int y = point.y;
            vertex.y = 0;
            lineEnd.y = 0;
            num3.y = 0;
            point.y = 0;
            if ((((lineEnd.x - vertex.x) * (point.z - vertex.z)) - ((point.x - vertex.x) * (lineEnd.z - vertex.z))) > 0L)
            {
                float num6 = Mathf.Clamp01(AstarMath.NearestPointFactorXZ(vertex, lineEnd, point));
                return (Vector3) (new Vector3(vertex.x + ((lineEnd.x - vertex.x) * num6), (float) y, vertex.z + ((lineEnd.z - vertex.z) * num6)) * 0.001f);
            }
            if ((((num3.x - lineEnd.x) * (point.z - lineEnd.z)) - ((point.x - lineEnd.x) * (num3.z - lineEnd.z))) > 0L)
            {
                float num7 = Mathf.Clamp01(AstarMath.NearestPointFactorXZ(lineEnd, num3, point));
                return (Vector3) (new Vector3(lineEnd.x + ((num3.x - lineEnd.x) * num7), (float) y, lineEnd.z + ((num3.z - lineEnd.z) * num7)) * 0.001f);
            }
            if ((((vertex.x - num3.x) * (point.z - num3.z)) - ((point.x - num3.x) * (vertex.z - num3.z))) > 0L)
            {
                float num8 = Mathf.Clamp01(AstarMath.NearestPointFactorXZ(num3, vertex, point));
                return (Vector3) (new Vector3(num3.x + ((vertex.x - num3.x) * num8), (float) y, num3.z + ((vertex.z - num3.z) * num8)) * 0.001f);
            }
            return _p;
        }

        public override VInt3 ClosestPointOnNodeXZ(VInt3 p)
        {
            VInt3 num4;
            INavmeshHolder navmeshHolder = GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex);
            VInt3 vertex = navmeshHolder.GetVertex(this.v0);
            VInt3 end = navmeshHolder.GetVertex(this.v1);
            VInt3 num3 = navmeshHolder.GetVertex(this.v2);
            vertex.y = 0;
            end.y = 0;
            num3.y = 0;
            if ((((end.x - vertex.x) * (p.z - vertex.z)) - ((p.x - vertex.x) * (end.z - vertex.z))) > 0L)
            {
                this.CalcNearestPoint(out num4, ref vertex, ref end, ref p);
                return num4;
            }
            if ((((num3.x - end.x) * (p.z - end.z)) - ((p.x - end.x) * (num3.z - end.z))) > 0L)
            {
                this.CalcNearestPoint(out num4, ref end, ref num3, ref p);
                return num4;
            }
            if ((((vertex.x - num3.x) * (p.z - num3.z)) - ((p.x - num3.x) * (vertex.z - num3.z))) > 0L)
            {
                this.CalcNearestPoint(out num4, ref num3, ref vertex, ref p);
                return num4;
            }
            return p;
        }

        public override bool ContainsPoint(VInt3 p)
        {
            INavmeshHolder navmeshHolder = GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex);
            VInt3 vertex = navmeshHolder.GetVertex(this.v0);
            VInt3 num2 = navmeshHolder.GetVertex(this.v1);
            VInt3 num3 = navmeshHolder.GetVertex(this.v2);
            if ((((num2.x - vertex.x) * (p.z - vertex.z)) - ((p.x - vertex.x) * (num2.z - vertex.z))) > 0L)
            {
                return false;
            }
            if ((((num3.x - num2.x) * (p.z - num2.z)) - ((p.x - num2.x) * (num3.z - num2.z))) > 0L)
            {
                return false;
            }
            if ((((vertex.x - num3.x) * (p.z - num3.z)) - ((p.x - num3.x) * (vertex.z - num3.z))) > 0L)
            {
                return false;
            }
            return true;
        }

        public override void DeserializeNode(GraphSerializationContext ctx)
        {
            base.DeserializeNode(ctx);
            this.v0 = ctx.reader.ReadInt32();
            this.v1 = ctx.reader.ReadInt32();
            this.v2 = ctx.reader.ReadInt32();
        }

        protected override void Duplicate(GraphNode graphNode)
        {
            base.Duplicate(graphNode);
            TriangleMeshNode node = (TriangleMeshNode) graphNode;
            node.v0 = this.v0;
            node.v1 = this.v1;
            node.v2 = this.v2;
        }

        public int EdgeIntersect(VInt3 a, VInt3 b)
        {
            VInt3 num;
            VInt3 num2;
            VInt3 num3;
            this.GetPoints(out num, out num2, out num3);
            if (Polygon.Intersects(num, num2, a, b))
            {
                return 0;
            }
            if (Polygon.Intersects(num2, num3, a, b))
            {
                return 1;
            }
            if (Polygon.Intersects(num3, num, a, b))
            {
                return 2;
            }
            return -1;
        }

        public int EdgeIntersect(VInt3 a, VInt3 b, int startEdge, int count)
        {
            VInt3[] numArray = _staticVerts;
            this.GetPoints(out numArray[0], out numArray[1], out numArray[2]);
            for (int i = 0; i < count; i++)
            {
                int index = (startEdge + i) % 3;
                int num3 = (index + 1) % 3;
                if (Polygon.Intersects(numArray[index], numArray[num3], a, b))
                {
                    return index;
                }
            }
            return -1;
        }

        public int GetColinearEdge(VInt3 a, VInt3 b)
        {
            VInt3 num;
            VInt3 num2;
            VInt3 num3;
            this.GetPoints(out num, out num2, out num3);
            if (Polygon.IsColinear(num, num2, a) && Polygon.IsColinear(num, num2, b))
            {
                return 0;
            }
            if (Polygon.IsColinear(num2, num3, a) && Polygon.IsColinear(num2, num3, b))
            {
                return 1;
            }
            if (Polygon.IsColinear(num3, num, a) && Polygon.IsColinear(num3, num, b))
            {
                return 2;
            }
            return -1;
        }

        public int GetColinearEdge(VInt3 a, VInt3 b, int startEdge, int count)
        {
            VInt3[] numArray = _staticVerts;
            this.GetPoints(out numArray[0], out numArray[1], out numArray[2]);
            for (int i = 0; i < count; i++)
            {
                int index = (startEdge + i) % 3;
                int num3 = (index + 1) % 3;
                if (Polygon.IsColinear(numArray[index], numArray[num3], a) && Polygon.IsColinear(numArray[index], numArray[num3], b))
                {
                    return index;
                }
            }
            return -1;
        }

        public static INavmeshHolder GetNavmeshHolder(int dataGroupIndex, uint graphIndex)
        {
            return _navmeshHolders[dataGroupIndex][graphIndex];
        }

        public TriangleMeshNode GetNeighborByEdge(int edge, out int otherEdge)
        {
            otherEdge = -1;
            if (((edge < 0) || (edge > 2)) || (base.connections == null))
            {
                return null;
            }
            int vertexIndex = this.GetVertexIndex(edge % 3);
            int num2 = this.GetVertexIndex((edge + 1) % 3);
            for (int i = 0; i < base.connections.Length; i++)
            {
                TriangleMeshNode node2 = base.connections[i] as TriangleMeshNode;
                if ((node2 != null) && (node2.GraphIndex == base.GraphIndex))
                {
                    if ((node2.v1 == vertexIndex) && (node2.v0 == num2))
                    {
                        otherEdge = 0;
                    }
                    else if ((node2.v2 == vertexIndex) && (node2.v1 == num2))
                    {
                        otherEdge = 1;
                    }
                    else if ((node2.v0 == vertexIndex) && (node2.v2 == num2))
                    {
                        otherEdge = 2;
                    }
                    if (otherEdge != -1)
                    {
                        return node2;
                    }
                }
            }
            return null;
        }

        public void GetPoints(out Vector3 a, out Vector3 b, out Vector3 c)
        {
            INavmeshHolder navmeshHolder = GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex);
            a = (Vector3) navmeshHolder.GetVertex(this.v0);
            b = (Vector3) navmeshHolder.GetVertex(this.v1);
            c = (Vector3) navmeshHolder.GetVertex(this.v2);
        }

        public void GetPoints(out VInt3 a, out VInt3 b, out VInt3 c)
        {
            INavmeshHolder navmeshHolder = GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex);
            a = navmeshHolder.GetVertex(this.v0);
            b = navmeshHolder.GetVertex(this.v1);
            c = navmeshHolder.GetVertex(this.v2);
        }

        public override bool GetPortal(GraphNode _other, List<VInt3> left, List<VInt3> right, bool backwards)
        {
            int num;
            int num2;
            return this.GetPortal(_other, left, right, backwards, out num, out num2);
        }

        public bool GetPortal(GraphNode _other, List<VInt3> left, List<VInt3> right, bool backwards, out int aIndex, out int bIndex)
        {
            aIndex = -1;
            bIndex = -1;
            if (_other.GraphIndex != base.GraphIndex)
            {
                return false;
            }
            TriangleMeshNode node = _other as TriangleMeshNode;
            int tileIndex = (this.GetVertexIndex(0) >> 12) & 0x7ffff;
            int num2 = (node.GetVertexIndex(0) >> 12) & 0x7ffff;
            if ((tileIndex != num2) && (GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex) is RecastGraph))
            {
                int num3;
                int num4;
                int num5;
                int num6;
                int num7;
                INavmeshHolder navmeshHolder = GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex);
                navmeshHolder.GetTileCoordinates(tileIndex, out num3, out num5);
                navmeshHolder.GetTileCoordinates(num2, out num4, out num6);
                if (Math.Abs((int) (num3 - num4)) == 1)
                {
                    num7 = 0;
                }
                else if (Math.Abs((int) (num5 - num6)) == 1)
                {
                    num7 = 2;
                }
                else
                {
                    object[] objArray1 = new object[] { "Tiles not adjacent (", num3, ", ", num5, ") (", num4, ", ", num6, ")" };
                    throw new Exception(string.Concat(objArray1));
                }
                int vertexCount = this.GetVertexCount();
                int num9 = node.GetVertexCount();
                int i = -1;
                int num11 = -1;
                for (int j = 0; j < vertexCount; j++)
                {
                    int num13 = this.GetVertex(j)[num7];
                    for (int k = 0; k < num9; k++)
                    {
                        if ((num13 == node.GetVertex((k + 1) % num9)[num7]) && (this.GetVertex((j + 1) % vertexCount)[num7] == node.GetVertex(k)[num7]))
                        {
                            i = j;
                            num11 = k;
                            j = vertexCount;
                            break;
                        }
                    }
                }
                aIndex = i;
                bIndex = num11;
                if (i != -1)
                {
                    VInt3 vertex = this.GetVertex(i);
                    VInt3 item = this.GetVertex((i + 1) % vertexCount);
                    int num17 = (num7 != 2) ? 2 : 0;
                    int num18 = Math.Min(vertex[num17], item[num17]);
                    int num19 = Math.Max(vertex[num17], item[num17]);
                    num18 = Math.Max(num18, Math.Min(node.GetVertex(num11)[num17], node.GetVertex((num11 + 1) % num9)[num17]));
                    num19 = Math.Min(num19, Math.Max(node.GetVertex(num11)[num17], node.GetVertex((num11 + 1) % num9)[num17]));
                    if (vertex[num17] < item[num17])
                    {
                        vertex[num17] = num18;
                        item[num17] = num19;
                    }
                    else
                    {
                        vertex[num17] = num19;
                        item[num17] = num18;
                    }
                    if (left != null)
                    {
                        left.Add(vertex);
                        right.Add(item);
                    }
                    return true;
                }
            }
            else if (!backwards)
            {
                int num20 = -1;
                int num21 = -1;
                int num22 = this.GetVertexCount();
                int num23 = node.GetVertexCount();
                for (int m = 0; m < num22; m++)
                {
                    int vertexIndex = this.GetVertexIndex(m);
                    for (int n = 0; n < num23; n++)
                    {
                        if ((vertexIndex == node.GetVertexIndex((n + 1) % num23)) && (this.GetVertexIndex((m + 1) % num22) == node.GetVertexIndex(n)))
                        {
                            num20 = m;
                            num21 = n;
                            m = num22;
                            break;
                        }
                    }
                }
                aIndex = num20;
                bIndex = num21;
                if (num20 == -1)
                {
                    return false;
                }
                if (left != null)
                {
                    left.Add(this.GetVertex(num20));
                    right.Add(this.GetVertex((num20 + 1) % num22));
                }
            }
            return true;
        }

        public override VInt3 GetVertex(int i)
        {
            VInt3 zero = VInt3.zero;
            try
            {
                zero = GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex).GetVertex(this.GetVertexIndex(i));
            }
            catch (Exception)
            {
            }
            return zero;
        }

        public int GetVertexArrayIndex(int i)
        {
            return GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex).GetVertexArrayIndex((i != 0) ? ((i != 1) ? this.v2 : this.v1) : this.v0);
        }

        public override int GetVertexCount()
        {
            return 3;
        }

        public int GetVertexIndex(int i)
        {
            return ((i != 0) ? ((i != 1) ? this.v2 : this.v1) : this.v0);
        }

        public bool IsVertex(VInt3 p, out int index)
        {
            INavmeshHolder navmeshHolder = GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex);
            index = -1;
            if (navmeshHolder.GetVertex(this.v0).IsEqualXZ(ref p))
            {
                index = 0;
            }
            else if (navmeshHolder.GetVertex(this.v1).IsEqualXZ(ref p))
            {
                index = 1;
            }
            else if (navmeshHolder.GetVertex(this.v2).IsEqualXZ(ref p))
            {
                index = 2;
            }
            return (index != -1);
        }

        public override void Open(Path path, PathNode pathNode, PathHandler handler)
        {
            if (base.connections != null)
            {
                bool flag = pathNode.flag2;
                for (int i = base.connections.Length - 1; i >= 0; i--)
                {
                    GraphNode node = base.connections[i];
                    if (path.CanTraverse(node))
                    {
                        PathNode node2 = handler.GetPathNode(node);
                        if (node2 != pathNode.parent)
                        {
                            uint currentCost = base.connectionCosts[i];
                            if (flag || node2.flag2)
                            {
                                currentCost = path.GetConnectionSpecialCost(this, node, currentCost);
                            }
                            if (node2.pathID != handler.PathID)
                            {
                                node2.node = node;
                                node2.parent = pathNode;
                                node2.pathID = handler.PathID;
                                node2.cost = currentCost;
                                node2.H = path.CalculateHScore(node);
                                node.UpdateG(path, node2);
                                handler.PushNode(node2);
                            }
                            else if (((pathNode.G + currentCost) + path.GetTraversalCost(node)) < node2.G)
                            {
                                node2.cost = currentCost;
                                node2.parent = pathNode;
                                node.UpdateRecursiveG(path, node2, handler);
                            }
                            else if ((((node2.G + currentCost) + path.GetTraversalCost(this)) < pathNode.G) && node.ContainsConnection(this))
                            {
                                pathNode.parent = node2;
                                pathNode.cost = currentCost;
                                this.UpdateRecursiveG(path, pathNode, handler);
                            }
                        }
                    }
                }
            }
        }

        public override void SerializeNode(GraphSerializationContext ctx)
        {
            base.SerializeNode(ctx);
            ctx.writer.Write(this.v0);
            ctx.writer.Write(this.v1);
            ctx.writer.Write(this.v2);
        }

        public static void SetNavmeshHolder(int dataGroupIndex, int graphIndex, INavmeshHolder graph)
        {
            if (dataGroupIndex >= _navmeshHolders.Count)
            {
                for (int i = _navmeshHolders.Count; i <= dataGroupIndex; i++)
                {
                    _navmeshHolders.Add(new INavmeshHolder[0]);
                }
            }
            if (_navmeshHolders[dataGroupIndex].Length <= graphIndex)
            {
                INavmeshHolder[] holderArray = new INavmeshHolder[graphIndex + 1];
                INavmeshHolder[] holderArray2 = _navmeshHolders[dataGroupIndex];
                for (int j = 0; j < holderArray2.Length; j++)
                {
                    holderArray[j] = holderArray2[j];
                }
                _navmeshHolders[dataGroupIndex] = holderArray;
            }
            _navmeshHolders[dataGroupIndex][graphIndex] = (INavmeshHolder[]) graph;
        }

        public int SharedEdge(GraphNode other)
        {
            int num;
            int num2;
            this.GetPortal(other, null, null, false, out num, out num2);
            return num;
        }

        public void UpdatePositionFromVertices()
        {
            INavmeshHolder navmeshHolder = GetNavmeshHolder(base.DataGroupIndex, base.GraphIndex);
            base.position = (VInt3) (((navmeshHolder.GetVertex(this.v0) + navmeshHolder.GetVertex(this.v1)) + navmeshHolder.GetVertex(this.v2)) * 0.333333f);
        }

        public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
        {
            base.UpdateG(path, pathNode);
            handler.PushNode(pathNode);
            if (base.connections != null)
            {
                for (int i = 0; i < base.connections.Length; i++)
                {
                    GraphNode node = base.connections[i];
                    PathNode node2 = handler.GetPathNode(node);
                    if ((node2.parent == pathNode) && (node2.pathID == handler.PathID))
                    {
                        node.UpdateRecursiveG(path, node2, handler);
                    }
                }
            }
        }
    }
}

