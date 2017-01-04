namespace Pathfinding
{
    using Pathfinding.Serialization;
    using Pathfinding.Serialization.JsonFx;
    using Pathfinding.Util;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, JsonOptIn]
    public class NavMeshGraph : NavGraph, IUpdatableGraph, IRaycastableGraph, INavmesh, INavmeshHolder
    {
        private BBTree _bbTree;
        [NonSerialized]
        private VInt3[] _vertices;
        [JsonMember]
        public bool accurateNearestNode = true;
        public TriangleMeshNode[] nodes;
        [JsonMember]
        public Vector3 offset;
        [NonSerialized]
        private Vector3[] originalVertices;
        [JsonMember]
        public Vector3 rotation;
        [JsonMember]
        public float scale = 1f;
        [JsonMember]
        public Mesh sourceMesh;
        [NonSerialized]
        public int[] triangles;

        public GraphUpdateThreading CanUpdateAsync(GraphUpdateObject o)
        {
            return GraphUpdateThreading.UnityThread;
        }

        public static Vector3 ClosestPointOnNode(TriangleMeshNode node, VInt3[] vertices, Vector3 pos)
        {
            return Polygon.ClosestPointOnTriangle((Vector3) vertices[node.v0], (Vector3) vertices[node.v1], (Vector3) vertices[node.v2], pos);
        }

        public bool ContainsPoint(TriangleMeshNode node, Vector3 pos)
        {
            return ((Polygon.IsClockwise((Vector3) this.vertices[node.v0], (Vector3) this.vertices[node.v1], pos) && Polygon.IsClockwise((Vector3) this.vertices[node.v1], (Vector3) this.vertices[node.v2], pos)) && Polygon.IsClockwise((Vector3) this.vertices[node.v2], (Vector3) this.vertices[node.v0], pos));
        }

        public static bool ContainsPoint(TriangleMeshNode node, Vector3 pos, VInt3[] vertices)
        {
            if (!Polygon.IsClockwiseMargin((Vector3) vertices[node.v0], (Vector3) vertices[node.v1], (Vector3) vertices[node.v2]))
            {
                Debug.LogError("Noes!");
            }
            return ((Polygon.IsClockwiseMargin((Vector3) vertices[node.v0], (Vector3) vertices[node.v1], pos) && Polygon.IsClockwiseMargin((Vector3) vertices[node.v1], (Vector3) vertices[node.v2], pos)) && Polygon.IsClockwiseMargin((Vector3) vertices[node.v2], (Vector3) vertices[node.v0], pos));
        }

        public override void CreateNodes(int number)
        {
            TriangleMeshNode[] nodeArray = new TriangleMeshNode[number];
            for (int i = 0; i < number; i++)
            {
                nodeArray[i] = new TriangleMeshNode(base.active);
                nodeArray[i].Penalty = base.initialPenalty;
            }
        }

        public override void DeserializeExtraInfo(GraphSerializationContext ctx)
        {
            uint graphIndex = (uint) base.active.astarData.GetGraphIndex(this);
            TriangleMeshNode.SetNavmeshHolder(0, (int) graphIndex, this);
            int num2 = ctx.reader.ReadInt32();
            int num3 = ctx.reader.ReadInt32();
            if (num2 == -1)
            {
                this.nodes = new TriangleMeshNode[0];
                this._vertices = new VInt3[0];
                this.originalVertices = new Vector3[0];
            }
            this.nodes = new TriangleMeshNode[num2];
            this._vertices = new VInt3[num3];
            this.originalVertices = new Vector3[num3];
            for (int i = 0; i < num3; i++)
            {
                this._vertices[i] = new VInt3(ctx.reader.ReadInt32(), ctx.reader.ReadInt32(), ctx.reader.ReadInt32());
                this.originalVertices[i] = new Vector3(ctx.reader.ReadSingle(), ctx.reader.ReadSingle(), ctx.reader.ReadSingle());
            }
            this.bbTree = new BBTree(this);
            for (int j = 0; j < num2; j++)
            {
                this.nodes[j] = new TriangleMeshNode(base.active);
                TriangleMeshNode node = this.nodes[j];
                node.DeserializeNode(ctx);
                node.GraphIndex = graphIndex;
                node.UpdatePositionFromVertices();
                this.bbTree.Insert(node);
            }
        }

        public static void DeserializeMeshNodes(NavMeshGraph graph, GraphNode[] nodes, byte[] bytes)
        {
            MemoryStream input = new MemoryStream(bytes);
            BinaryReader reader = new BinaryReader(input);
            for (int i = 0; i < nodes.Length; i++)
            {
                TriangleMeshNode node = nodes[i] as TriangleMeshNode;
                if (node == null)
                {
                    Debug.LogError("Serialization Error : Couldn't cast the node to the appropriate type - NavMeshGenerator");
                    return;
                }
                node.v0 = reader.ReadInt32();
                node.v1 = reader.ReadInt32();
                node.v2 = reader.ReadInt32();
            }
            int num2 = reader.ReadInt32();
            graph.vertices = new VInt3[num2];
            for (int j = 0; j < num2; j++)
            {
                int num4 = reader.ReadInt32();
                int num5 = reader.ReadInt32();
                int num6 = reader.ReadInt32();
                graph.vertices[j] = new VInt3(num4, num5, num6);
            }
            RebuildBBTree(graph);
        }

        public override void DeserializeSettings(GraphSerializationContext ctx)
        {
            base.DeserializeSettings(ctx);
            this.sourceMesh = ctx.DeserializeUnityObject() as Mesh;
            this.offset = ctx.DeserializeVector3();
            this.rotation = ctx.DeserializeVector3();
            this.scale = ctx.reader.ReadSingle();
            this.accurateNearestNode = ctx.reader.ReadBoolean();
        }

        public void GenerateMatrix()
        {
            base.SetMatrix(Matrix4x4.TRS(this.offset, Quaternion.Euler(this.rotation), new Vector3(this.scale, this.scale, this.scale)));
        }

        private void GenerateNodes(Vector3[] vectorVertices, int[] triangles, out Vector3[] originalVertices, out VInt3[] vertices)
        {
            if ((vectorVertices.Length == 0) || (triangles.Length == 0))
            {
                originalVertices = vectorVertices;
                vertices = new VInt3[0];
                this.nodes = new TriangleMeshNode[0];
            }
            else
            {
                vertices = new VInt3[vectorVertices.Length];
                int index = 0;
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = (VInt3) this.matrix.MultiplyPoint3x4(vectorVertices[i]);
                }
                Dictionary<VInt3, int> dictionary = new Dictionary<VInt3, int>();
                int[] numArray = new int[vertices.Length];
                for (int j = 0; j < vertices.Length; j++)
                {
                    if (!dictionary.ContainsKey(vertices[j]))
                    {
                        numArray[index] = j;
                        dictionary.Add(vertices[j], index);
                        index++;
                    }
                }
                for (int k = 0; k < triangles.Length; k++)
                {
                    VInt3 num5 = vertices[triangles[k]];
                    triangles[k] = dictionary[num5];
                }
                VInt3[] numArray2 = vertices;
                vertices = new VInt3[index];
                originalVertices = new Vector3[index];
                for (int m = 0; m < index; m++)
                {
                    vertices[m] = numArray2[numArray[m]];
                    originalVertices[m] = vectorVertices[numArray[m]];
                }
                this.nodes = new TriangleMeshNode[triangles.Length / 3];
                int graphIndex = base.active.astarData.GetGraphIndex(this);
                for (int n = 0; n < this.nodes.Length; n++)
                {
                    this.nodes[n] = new TriangleMeshNode(base.active);
                    TriangleMeshNode node = this.nodes[n];
                    node.GraphIndex = (uint) graphIndex;
                    node.Penalty = base.initialPenalty;
                    node.Walkable = true;
                    node.v0 = triangles[n * 3];
                    node.v1 = triangles[(n * 3) + 1];
                    node.v2 = triangles[(n * 3) + 2];
                    if (!Polygon.IsClockwise(vertices[node.v0], vertices[node.v1], vertices[node.v2]))
                    {
                        int num9 = node.v0;
                        node.v0 = node.v2;
                        node.v2 = num9;
                    }
                    if (Polygon.IsColinear(vertices[node.v0], vertices[node.v1], vertices[node.v2]))
                    {
                        Debug.DrawLine((Vector3) vertices[node.v0], (Vector3) vertices[node.v1], Color.red);
                        Debug.DrawLine((Vector3) vertices[node.v1], (Vector3) vertices[node.v2], Color.red);
                        Debug.DrawLine((Vector3) vertices[node.v2], (Vector3) vertices[node.v0], Color.red);
                    }
                    node.UpdatePositionFromVertices();
                }
                DictionaryView<VInt2, TriangleMeshNode> view = new DictionaryView<VInt2, TriangleMeshNode>();
                int num10 = 0;
                int num11 = 0;
                while (num10 < triangles.Length)
                {
                    view[new VInt2(triangles[num10], triangles[num10 + 1])] = this.nodes[num11];
                    view[new VInt2(triangles[num10 + 1], triangles[num10 + 2])] = this.nodes[num11];
                    view[new VInt2(triangles[num10 + 2], triangles[num10])] = this.nodes[num11];
                    num11++;
                    num10 += 3;
                }
                ListLinqView<MeshNode> view2 = new ListLinqView<MeshNode>();
                List<uint> list = new List<uint>();
                int num12 = 0;
                int num13 = 0;
                int num14 = 0;
                while (num13 < triangles.Length)
                {
                    view2.Clear();
                    list.Clear();
                    TriangleMeshNode node2 = this.nodes[num14];
                    for (int num15 = 0; num15 < 3; num15++)
                    {
                        TriangleMeshNode node3;
                        if (view.TryGetValue(new VInt2(triangles[num13 + ((num15 + 1) % 3)], triangles[num13 + num15]), out node3))
                        {
                            view2.Add(node3);
                            VInt3 num16 = node2.position - node3.position;
                            list.Add((uint) num16.costMagnitude);
                        }
                    }
                    node2.connections = view2.ToArray();
                    node2.connectionCosts = list.ToArray();
                    num14++;
                    num13 += 3;
                }
                if (num12 > 0)
                {
                    Debug.LogError("One or more triangles are identical to other triangles, this is not a good thing to have in a navmesh\nIncreasing the scale of the mesh might help\nNumber of triangles with error: " + num12 + "\n");
                }
                RebuildBBTree(this);
            }
        }

        public override NNInfo GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
        {
            return GetNearest(this, this.nodes, position, constraint, this.accurateNearestNode);
        }

        public NNInfo GetNearest(VInt3 position, NNConstraint constraint, GraphNode hint)
        {
            return GetNearest(this, this.nodes, (Vector3) position, constraint, this.accurateNearestNode);
        }

        public static NNInfo GetNearest(NavMeshGraph graph, GraphNode[] nodes, Vector3 position, NNConstraint constraint, bool accurateNearestNode)
        {
            if ((nodes == null) || (nodes.Length == 0))
            {
                Debug.LogError("NavGraph hasn't been generated yet or does not contain any nodes");
                return new NNInfo();
            }
            if (constraint == null)
            {
                constraint = NNConstraint.None;
            }
            VInt3[] vertices = graph.vertices;
            if (graph.bbTree == null)
            {
                return GetNearestForce(graph, graph, position, constraint, accurateNearestNode);
            }
            float radius = ((graph.bbTree.Size.width + graph.bbTree.Size.height) * 0.5f) * 0.02f;
            NNInfo info = graph.bbTree.QueryCircle(position, radius, constraint);
            if (info.node == null)
            {
                for (int i = 1; i <= 8; i++)
                {
                    info = graph.bbTree.QueryCircle(position, (i * i) * radius, constraint);
                    if ((info.node != null) || ((((i - 1) * (i - 1)) * radius) > (AstarPath.active.maxNearestNodeDistance * 2f)))
                    {
                        break;
                    }
                }
            }
            if (info.node != null)
            {
                info.clampedPosition = ClosestPointOnNode(info.node as TriangleMeshNode, vertices, position);
            }
            if (info.constrainedNode != null)
            {
                if (constraint.constrainDistance)
                {
                    Vector3 vector = ((Vector3) info.constrainedNode.position) - position;
                    if (vector.sqrMagnitude > AstarPath.active.maxNearestNodeDistanceSqr)
                    {
                        info.constrainedNode = null;
                        return info;
                    }
                }
                info.constClampedPosition = ClosestPointOnNode(info.constrainedNode as TriangleMeshNode, vertices, position);
            }
            return info;
        }

        public override NNInfo GetNearestForce(Vector3 position, NNConstraint constraint)
        {
            return GetNearestForce(this, this, position, constraint, this.accurateNearestNode);
        }

        public static NNInfo GetNearestForce(NavGraph graph, INavmeshHolder navmesh, Vector3 position, NNConstraint constraint, bool accurateNearestNode)
        {
            NNInfo info = GetNearestForceBoth(graph, navmesh, position, constraint, accurateNearestNode);
            info.node = info.constrainedNode;
            info.clampedPosition = info.constClampedPosition;
            return info;
        }

        public static NNInfo GetNearestForceBoth(NavGraph graph, INavmeshHolder navmesh, Vector3 position, NNConstraint constraint, bool accurateNearestNode)
        {
            <GetNearestForceBoth>c__AnonStorey30 storey;
            storey = new <GetNearestForceBoth>c__AnonStorey30 {
                accurateNearestNode = accurateNearestNode,
                position = position,
                constraint = constraint,
                pos = (VInt3) storey.position,
                minDist = -1.0,
                minNode = null,
                minConstDist = -1.0,
                minConstNode = null,
                maxDistSqr = !storey.constraint.constrainDistance ? float.PositiveInfinity : AstarPath.active.maxNearestNodeDistanceSqr
            };
            GraphNodeDelegateCancelable del = new GraphNodeDelegateCancelable(storey.<>m__24);
            graph.GetNodes(del);
            NNInfo info = new NNInfo(storey.minNode);
            if (info.node != null)
            {
                Vector3 vector = (info.node as TriangleMeshNode).ClosestPointOnNode(storey.position);
                info.clampedPosition = vector;
            }
            info.constrainedNode = storey.minConstNode;
            if (info.constrainedNode != null)
            {
                Vector3 vector2 = (info.constrainedNode as TriangleMeshNode).ClosestPointOnNode(storey.position);
                info.constClampedPosition = vector2;
            }
            return info;
        }

        public override void GetNodes(GraphNodeDelegateCancelable del)
        {
            if (this.nodes != null)
            {
                for (int i = 0; (i < this.nodes.Length) && del(this.nodes[i]); i++)
                {
                }
            }
        }

        public void GetTileCoordinates(int tileIndex, out int x, out int z)
        {
            x = z = 0;
        }

        public VInt3 GetVertex(int index)
        {
            return this.vertices[index];
        }

        public int GetVertexArrayIndex(int index)
        {
            return index;
        }

        public bool Linecast(VInt3 origin, VInt3 end)
        {
            return this.Linecast(origin, end, base.GetNearest(origin, NNConstraint.None).node);
        }

        public bool Linecast(VInt3 origin, VInt3 end, GraphNode hint)
        {
            GraphHitInfo info;
            return Linecast(this, origin, end, hint, out info, null);
        }

        public bool Linecast(VInt3 origin, VInt3 end, GraphNode hint, out GraphHitInfo hit)
        {
            return Linecast(this, origin, end, hint, out hit, null);
        }

        public static bool Linecast(INavmesh graph, VInt3 tmp_origin, VInt3 tmp_end, GraphNode hint, out GraphHitInfo hit)
        {
            return Linecast(graph, tmp_origin, tmp_end, hint, out hit, null);
        }

        public bool Linecast(VInt3 origin, VInt3 end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace)
        {
            return Linecast(this, origin, end, hint, out hit, trace);
        }

        public static bool Linecast(INavmesh graph, VInt3 tmp_origin, VInt3 tmp_end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace)
        {
            VInt3 p = tmp_end;
            VInt3 num2 = tmp_origin;
            hit = new GraphHitInfo();
            if (float.IsNaN((float) ((tmp_origin.x + tmp_origin.y) + tmp_origin.z)))
            {
                throw new ArgumentException("origin is NaN");
            }
            if (float.IsNaN((float) ((tmp_end.x + tmp_end.y) + tmp_end.z)))
            {
                throw new ArgumentException("end is NaN");
            }
            TriangleMeshNode item = hint as TriangleMeshNode;
            if (item == null)
            {
                item = (graph as NavGraph).GetNearest(tmp_origin, NNConstraint.None).node as TriangleMeshNode;
                if (item == null)
                {
                    Debug.LogError("Could not find a valid node to start from");
                    hit.point = tmp_origin;
                    return true;
                }
            }
            if (num2 == p)
            {
                hit.node = item;
                return false;
            }
            num2 = (VInt3) item.ClosestPointOnNode((Vector3) num2);
            hit.origin = num2;
            if (!item.Walkable)
            {
                hit.point = num2;
                hit.tangentOrigin = num2;
                return true;
            }
            List<VInt3> list = ListPool<VInt3>.Claim();
            List<VInt3> list2 = ListPool<VInt3>.Claim();
            int num3 = 0;
            while (true)
            {
                num3++;
                if (num3 > 0x7d0)
                {
                    Debug.LogError("Linecast was stuck in infinite loop. Breaking.");
                    ListPool<VInt3>.Release(list);
                    ListPool<VInt3>.Release(list2);
                    return true;
                }
                TriangleMeshNode node2 = null;
                if (trace != null)
                {
                    trace.Add(item);
                }
                if (item.ContainsPoint(p))
                {
                    ListPool<VInt3>.Release(list);
                    ListPool<VInt3>.Release(list2);
                    return false;
                }
                for (int i = 0; i < item.connections.Length; i++)
                {
                    if (item.connections[i].GraphIndex == item.GraphIndex)
                    {
                        list.Clear();
                        list2.Clear();
                        if (item.GetPortal(item.connections[i], list, list2, false))
                        {
                            float num7;
                            float num8;
                            VInt3 a = list[0];
                            VInt3 b = list2[0];
                            if (((Polygon.LeftNotColinear(a, b, hit.origin) || !Polygon.LeftNotColinear(a, b, tmp_end)) && Polygon.IntersectionFactor(a, b, hit.origin, tmp_end, out num7, out num8)) && ((num8 >= 0f) && ((num7 >= 0f) && (num7 <= 1f))))
                            {
                                node2 = item.connections[i] as TriangleMeshNode;
                                break;
                            }
                        }
                    }
                }
                if (node2 == null)
                {
                    int vertexCount = item.GetVertexCount();
                    for (int j = 0; j < vertexCount; j++)
                    {
                        VFactor factor;
                        VFactor factor2;
                        VInt3 vertex = item.GetVertex(j);
                        VInt3 num12 = item.GetVertex((j + 1) % vertexCount);
                        if (((Polygon.LeftNotColinear(vertex, num12, hit.origin) || !Polygon.LeftNotColinear(vertex, num12, tmp_end)) && Polygon.IntersectionFactor(vertex, num12, hit.origin, tmp_end, out factor, out factor2)) && (!factor2.IsNegative && (!factor.IsNegative && ((factor.nom / factor.den) <= 1L))))
                        {
                            VInt3 num13 = (VInt3) ((num12 - vertex) * factor.nom);
                            num13 = IntMath.Divide(num13, factor.den) + vertex;
                            hit.point = num13;
                            hit.node = item;
                            hit.tangent = num12 - vertex;
                            hit.tangentOrigin = vertex;
                            ListPool<VInt3>.Release(list);
                            ListPool<VInt3>.Release(list2);
                            return true;
                        }
                    }
                    Debug.LogWarning("Linecast failing because point not inside node, and line does not hit any edges of it");
                    ListPool<VInt3>.Release(list);
                    ListPool<VInt3>.Release(list2);
                    return false;
                }
                item = node2;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            TriangleMeshNode.SetNavmeshHolder(0, base.active.astarData.GetGraphIndex(this), null);
        }

        public override void OnDrawGizmos(bool drawNodes)
        {
            if (drawNodes)
            {
                Matrix4x4 matrix = base.matrix;
                this.GenerateMatrix();
                if (this.nodes == null)
                {
                }
                if (this.nodes != null)
                {
                    if (this.bbTree != null)
                    {
                        this.bbTree.OnDrawGizmos();
                    }
                    if (matrix != base.matrix)
                    {
                        this.RelocateNodes(matrix, base.matrix);
                    }
                    PathHandler debugPathData = AstarPath.active.debugPathData;
                    for (int i = 0; i < this.nodes.Length; i++)
                    {
                        TriangleMeshNode node = this.nodes[i];
                        Gizmos.color = this.NodeColor(node, AstarPath.active.debugPathData);
                        if (node.Walkable)
                        {
                            if ((AstarPath.active.showSearchTree && (debugPathData != null)) && (debugPathData.GetPathNode(node).parent != null))
                            {
                                Gizmos.DrawLine((Vector3) node.position, (Vector3) debugPathData.GetPathNode(node).parent.node.position);
                            }
                            else
                            {
                                for (int j = 0; j < node.connections.Length; j++)
                                {
                                    Gizmos.DrawLine((Vector3) node.position, Vector3.Lerp((Vector3) node.position, (Vector3) node.connections[j].position, 0.45f));
                                }
                            }
                            Gizmos.color = AstarColor.MeshEdgeColor;
                        }
                        else
                        {
                            Gizmos.color = Color.red;
                        }
                        Gizmos.DrawLine((Vector3) this.vertices[node.v0], (Vector3) this.vertices[node.v1]);
                        Gizmos.DrawLine((Vector3) this.vertices[node.v1], (Vector3) this.vertices[node.v2]);
                        Gizmos.DrawLine((Vector3) this.vertices[node.v2], (Vector3) this.vertices[node.v0]);
                    }
                }
            }
        }

        public void PostProcess()
        {
        }

        public static void RebuildBBTree(NavMeshGraph graph)
        {
            NavMeshGraph graph2 = graph;
            BBTree bbTree = graph2.bbTree;
            if (bbTree == null)
            {
                bbTree = new BBTree(graph);
            }
            bbTree.Clear();
            TriangleMeshNode[] triNodes = graph2.TriNodes;
            for (int i = triNodes.Length - 1; i >= 0; i--)
            {
                bbTree.Insert(triNodes[i]);
            }
            graph2.bbTree = bbTree;
        }

        public override void RelocateNodes(Matrix4x4 oldMatrix, Matrix4x4 newMatrix)
        {
            if (((this.vertices != null) && (this.vertices.Length != 0)) && ((this.originalVertices != null) && (this.originalVertices.Length == this.vertices.Length)))
            {
                for (int i = 0; i < this._vertices.Length; i++)
                {
                    this._vertices[i] = (VInt3) newMatrix.MultiplyPoint3x4(this.originalVertices[i]);
                }
                for (int j = 0; j < this.nodes.Length; j++)
                {
                    TriangleMeshNode node = this.nodes[j];
                    node.UpdatePositionFromVertices();
                    if (node.connections != null)
                    {
                        for (int m = 0; m < node.connections.Length; m++)
                        {
                            VInt3 num5 = node.position - node.connections[m].position;
                            node.connectionCosts[m] = (uint) num5.costMagnitude;
                        }
                    }
                }
                base.SetMatrix(newMatrix);
                this.bbTree = new BBTree(this);
                for (int k = 0; k < this.nodes.Length; k++)
                {
                    this.bbTree.Insert(this.nodes[k]);
                }
            }
        }

        public override void ScanInternal(OnScanStatus statusCallback)
        {
            if (this.sourceMesh != null)
            {
                this.GenerateMatrix();
                Vector3[] vertices = this.sourceMesh.vertices;
                this.triangles = this.sourceMesh.triangles;
                TriangleMeshNode.SetNavmeshHolder(0, base.active.astarData.GetGraphIndex(this), this);
                this.GenerateNodes(vertices, this.triangles, out this.originalVertices, out this._vertices);
            }
        }

        public void ScanInternal(string objMeshPath)
        {
            Mesh mesh = ObjImporter.ImportFile(objMeshPath);
            if (mesh == null)
            {
                Debug.LogError("Couldn't read .obj file at '" + objMeshPath + "'");
            }
            else
            {
                this.sourceMesh = mesh;
                base.ScanInternal();
            }
        }

        public override void SerializeExtraInfo(GraphSerializationContext ctx)
        {
            if (((this.nodes == null) || (this.originalVertices == null)) || ((this._vertices == null) || (this.originalVertices.Length != this._vertices.Length)))
            {
                ctx.writer.Write(-1);
                ctx.writer.Write(-1);
            }
            else
            {
                ctx.writer.Write(this.nodes.Length);
                ctx.writer.Write(this._vertices.Length);
                for (int i = 0; i < this._vertices.Length; i++)
                {
                    ctx.writer.Write(this._vertices[i].x);
                    ctx.writer.Write(this._vertices[i].y);
                    ctx.writer.Write(this._vertices[i].z);
                    ctx.writer.Write(this.originalVertices[i].x);
                    ctx.writer.Write(this.originalVertices[i].y);
                    ctx.writer.Write(this.originalVertices[i].z);
                }
                for (int j = 0; j < this.nodes.Length; j++)
                {
                    this.nodes[j].SerializeNode(ctx);
                }
            }
        }

        public override void SerializeSettings(GraphSerializationContext ctx)
        {
            base.SerializeSettings(ctx);
            ctx.SerializeUnityObject(this.sourceMesh);
            ctx.SerializeVector3(this.offset);
            ctx.SerializeVector3(this.rotation);
            ctx.writer.Write(this.scale);
            ctx.writer.Write(this.accurateNearestNode);
        }

        public void Sort(Vector3[] a)
        {
            bool flag = true;
            while (flag)
            {
                flag = false;
                for (int i = 0; i < (a.Length - 1); i++)
                {
                    if ((a[i].x > a[i + 1].x) || ((a[i].x == a[i + 1].x) && ((a[i].y > a[i + 1].y) || ((a[i].y == a[i + 1].y) && (a[i].z > a[i + 1].z)))))
                    {
                        Vector3 vector = a[i];
                        a[i] = a[i + 1];
                        a[i + 1] = vector;
                        flag = true;
                    }
                }
            }
        }

        public void UpdateArea(GraphUpdateObject o)
        {
            UpdateArea(o, this);
        }

        public static void UpdateArea(GraphUpdateObject o, INavmesh graph)
        {
            <UpdateArea>c__AnonStorey31 storey = new <UpdateArea>c__AnonStorey31 {
                o = o
            };
            Bounds bounds = storey.o.bounds;
            storey.r = Rect.MinMaxRect(bounds.min.x, bounds.min.z, bounds.max.x, bounds.max.z);
            int xmin = Mathf.FloorToInt(bounds.min.x * 1000f);
            int ymin = Mathf.FloorToInt(bounds.min.z * 1000f);
            int xmax = Mathf.FloorToInt(bounds.max.x * 1000f);
            storey.r2 = new IntRect(xmin, ymin, xmax, Mathf.FloorToInt(bounds.max.z * 1000f));
            storey.a = new VInt3(storey.r2.xmin, 0, storey.r2.ymin);
            storey.b = new VInt3(storey.r2.xmin, 0, storey.r2.ymax);
            storey.c = new VInt3(storey.r2.xmax, 0, storey.r2.ymin);
            storey.d = new VInt3(storey.r2.xmax, 0, storey.r2.ymax);
            storey.ia = storey.a;
            storey.ib = storey.b;
            storey.ic = storey.c;
            storey.id = storey.d;
            graph.GetNodes(new GraphNodeDelegateCancelable(storey.<>m__25));
        }

        public void UpdateAreaInit(GraphUpdateObject o)
        {
        }

        public BBTree bbTree
        {
            get
            {
                return this._bbTree;
            }
            set
            {
                this._bbTree = value;
            }
        }

        public TriangleMeshNode[] TriNodes
        {
            get
            {
                return this.nodes;
            }
        }

        public VInt3[] vertices
        {
            get
            {
                return this._vertices;
            }
            set
            {
                this._vertices = value;
            }
        }

        [CompilerGenerated]
        private sealed class <GetNearestForceBoth>c__AnonStorey30
        {
            internal bool accurateNearestNode;
            internal NNConstraint constraint;
            internal float maxDistSqr;
            internal double minConstDist;
            internal GraphNode minConstNode;
            internal double minDist;
            internal GraphNode minNode;
            internal VInt3 pos;
            internal Vector3 position;

            internal bool <>m__24(GraphNode _node)
            {
                TriangleMeshNode node = _node as TriangleMeshNode;
                if (this.accurateNearestNode)
                {
                    Vector3 vector = node.ClosestPointOnNode(this.position);
                    Vector3 vector2 = ((Vector3) this.pos) - vector;
                    float sqrMagnitude = vector2.sqrMagnitude;
                    if ((this.minNode == null) || (sqrMagnitude < this.minDist))
                    {
                        this.minDist = sqrMagnitude;
                        this.minNode = node;
                    }
                    if (((sqrMagnitude < this.maxDistSqr) && this.constraint.Suitable(node)) && ((this.minConstNode == null) || (sqrMagnitude < this.minConstDist)))
                    {
                        this.minConstDist = sqrMagnitude;
                        this.minConstNode = node;
                    }
                }
                else if (!node.ContainsPoint((VInt3) this.position))
                {
                    VInt3 num4 = node.position - this.pos;
                    double num2 = num4.sqrMagnitude;
                    if ((this.minNode == null) || (num2 < this.minDist))
                    {
                        this.minDist = num2;
                        this.minNode = node;
                    }
                    if (((num2 < this.maxDistSqr) && this.constraint.Suitable(node)) && ((this.minConstNode == null) || (num2 < this.minConstDist)))
                    {
                        this.minConstDist = num2;
                        this.minConstNode = node;
                    }
                }
                else
                {
                    int num3 = AstarMath.Abs((int) (node.position.y - this.pos.y));
                    if ((this.minNode == null) || (num3 < this.minDist))
                    {
                        this.minDist = num3;
                        this.minNode = node;
                    }
                    if (((num3 < this.maxDistSqr) && this.constraint.Suitable(node)) && ((this.minConstNode == null) || (num3 < this.minConstDist)))
                    {
                        this.minConstDist = num3;
                        this.minConstNode = node;
                    }
                }
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateArea>c__AnonStorey31
        {
            internal VInt3 a;
            internal VInt3 b;
            internal VInt3 c;
            internal VInt3 d;
            internal VInt3 ia;
            internal VInt3 ib;
            internal VInt3 ic;
            internal VInt3 id;
            internal GraphUpdateObject o;
            internal Rect r;
            internal IntRect r2;

            internal bool <>m__25(GraphNode _node)
            {
                TriangleMeshNode node = _node as TriangleMeshNode;
                bool flag = false;
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                int num4 = 0;
                for (int i = 0; i < 3; i++)
                {
                    VInt3 vertex = node.GetVertex(i);
                    Vector3 vector = (Vector3) vertex;
                    if (this.r2.Contains(vertex.x, vertex.z))
                    {
                        flag = true;
                        break;
                    }
                    if (vector.x < this.r.xMin)
                    {
                        num++;
                    }
                    if (vector.x > this.r.xMax)
                    {
                        num2++;
                    }
                    if (vector.z < this.r.yMin)
                    {
                        num3++;
                    }
                    if (vector.z > this.r.yMax)
                    {
                        num4++;
                    }
                }
                if (flag || (((num != 3) && (num2 != 3)) && ((num3 != 3) && (num4 != 3))))
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int num8 = (j <= 1) ? (j + 1) : 0;
                        VInt3 num9 = node.GetVertex(j);
                        VInt3 num10 = node.GetVertex(num8);
                        if (Polygon.Intersects(this.a, this.b, num9, num10))
                        {
                            flag = true;
                            break;
                        }
                        if (Polygon.Intersects(this.a, this.c, num9, num10))
                        {
                            flag = true;
                            break;
                        }
                        if (Polygon.Intersects(this.c, this.d, num9, num10))
                        {
                            flag = true;
                            break;
                        }
                        if (Polygon.Intersects(this.d, this.b, num9, num10))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if ((node.ContainsPoint(this.ia) || node.ContainsPoint(this.ib)) || (node.ContainsPoint(this.ic) || node.ContainsPoint(this.id)))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        this.o.WillUpdateNode(node);
                        this.o.Apply(node);
                    }
                }
                return true;
            }
        }
    }
}

