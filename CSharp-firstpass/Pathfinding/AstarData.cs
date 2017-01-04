namespace Pathfinding
{
    using Pathfinding.Serialization;
    using Pathfinding.Util;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;

    [Serializable]
    public class AstarData
    {
        [SerializeField]
        public bool cacheStartup;
        [SerializeField]
        private byte[] data;
        public byte[] data_backup;
        public byte[] data_cachedStartup;
        public uint dataChecksum;
        public int DataGroupIndex;
        [NonSerialized]
        public NavGraph[] graphs = new NavGraph[0];
        public System.Type[] graphTypes;
        public bool hasBeenReverted;
        [NonSerialized]
        public NavMeshGraph navmesh;
        public GraphNodeRasterizer rasterizer;
        [NonSerialized]
        public RecastGraph recastGraph;
        public byte[] revertData;
        [NonSerialized]
        public UserConnection[] userConnections = new UserConnection[0];

        public void AddGraph(NavGraph graph)
        {
            AstarPath.active.BlockUntilPathQueueBlocked();
            for (int i = 0; i < this.graphs.Length; i++)
            {
                if (this.graphs[i] == null)
                {
                    this.graphs[i] = graph;
                    graph.active = this.active;
                    graph.Awake();
                    graph.graphIndex = (uint) i;
                    this.UpdateShortcuts();
                    return;
                }
            }
            if ((this.graphs != null) && (this.graphs.Length >= 0xffL))
            {
                throw new Exception("Graph Count Limit Reached. You cannot have more than " + 0xff + " graphs. Some compiler directives can change this limit, e.g ASTAR_MORE_AREAS, look under the 'Optimizations' tab in the A* Inspector");
            }
            this.graphs = new List<NavGraph>(this.graphs) { graph }.ToArray();
            this.UpdateShortcuts();
            graph.active = this.active;
            graph.Awake();
            graph.graphIndex = (uint) (this.graphs.Length - 1);
        }

        public NavGraph AddGraph(string type)
        {
            NavGraph graph = null;
            for (int i = 0; i < this.graphTypes.Length; i++)
            {
                if (this.graphTypes[i].Name == type)
                {
                    graph = this.CreateGraph(this.graphTypes[i]);
                }
            }
            if (graph == null)
            {
                Debug.LogError("No NavGraph of type '" + type + "' could be found");
                return null;
            }
            this.AddGraph(graph);
            return graph;
        }

        public NavGraph AddGraph(System.Type type)
        {
            NavGraph graph = null;
            for (int i = 0; i < this.graphTypes.Length; i++)
            {
                if (object.Equals(this.graphTypes[i], type))
                {
                    graph = this.CreateGraph(this.graphTypes[i]);
                }
            }
            if (graph == null)
            {
                Debug.LogError(string.Concat(new object[] { "No NavGraph of type '", type, "' could be found, ", this.graphTypes.Length, " graph types are avaliable" }));
                return null;
            }
            this.AddGraph(graph);
            return graph;
        }

        public void Awake()
        {
            this.userConnections = new UserConnection[0];
            this.graphs = new NavGraph[0];
            if (this.cacheStartup && (this.data_cachedStartup != null))
            {
                this.LoadFromCache();
            }
            else
            {
                this.DeserializeGraphs();
            }
        }

        private TriangleMeshNode checkObjIntersects(ref int edge, VInt3 start, VInt3 end, int gridX, int gridY)
        {
            List<object> objs = this.rasterizer.GetObjs(gridX, gridY);
            if ((objs != null) && (objs.Count != 0))
            {
                VInt3[] numArray = new VInt3[3];
                TriangleMeshNode node = null;
                int num = -1;
                long num2 = 0x7fffffffffffffffL;
                for (int i = 0; i < objs.Count; i++)
                {
                    TriangleMeshNode node2 = objs[i] as TriangleMeshNode;
                    node2.GetPoints(out numArray[0], out numArray[1], out numArray[2]);
                    for (int j = 0; j < 3; j++)
                    {
                        int index = j;
                        int num6 = (j + 1) % 3;
                        if (Polygon.Intersects(numArray[index], numArray[num6], start, end))
                        {
                            bool flag;
                            VInt3 rhs = Polygon.IntersectionPoint(ref numArray[index], ref numArray[num6], ref start, ref end, out flag);
                            DebugHelper.Assert(flag);
                            long num8 = start.XZSqrMagnitude(ref rhs);
                            if (num8 < num2)
                            {
                                num2 = num8;
                                node = node2;
                                num = j;
                            }
                        }
                    }
                }
                if ((num != -1) && (node != null))
                {
                    edge = num;
                    return node;
                }
            }
            return null;
        }

        private void ClearGraphs()
        {
            if (this.graphs != null)
            {
                for (int i = 0; i < this.graphs.Length; i++)
                {
                    if (this.graphs[i] != null)
                    {
                        this.graphs[i].OnDestroy();
                    }
                }
                this.graphs = null;
                this.UpdateShortcuts();
            }
        }

        public NavGraph CreateGraph(string type)
        {
            Debug.Log("Creating Graph of type '" + type + "'");
            for (int i = 0; i < this.graphTypes.Length; i++)
            {
                if (this.graphTypes[i].Name == type)
                {
                    return this.CreateGraph(this.graphTypes[i]);
                }
            }
            Debug.LogError("Graph type (" + type + ") wasn't found");
            return null;
        }

        public NavGraph CreateGraph(System.Type type)
        {
            NavGraph graph = Activator.CreateInstance(type) as NavGraph;
            graph.active = this.active;
            return graph;
        }

        public void DeserializeGraphs()
        {
            if (this.data != null)
            {
                this.DeserializeGraphs(this.data);
            }
        }

        public void DeserializeGraphs(byte[] bytes)
        {
            AstarPath.active.BlockUntilPathQueueBlocked();
            try
            {
                if (bytes == null)
                {
                    throw new ArgumentNullException("Bytes should not be null when passed to DeserializeGraphs");
                }
                AstarSerializer sr = new AstarSerializer(this);
                if (sr.OpenDeserialize(bytes))
                {
                    this.DeserializeGraphsPart(sr);
                    sr.CloseDeserialize();
                    this.UpdateShortcuts();
                }
                else
                {
                    Debug.Log("Invalid data file (cannot read zip).\nThe data is either corrupt or it was saved using a 3.0.x or earlier version of the system");
                }
                this.active.VerifyIntegrity();
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Caught exception while deserializing data.\n" + exception);
                this.data_backup = bytes;
            }
        }

        public void DeserializeGraphsAdditive(byte[] bytes)
        {
            AstarPath.active.BlockUntilPathQueueBlocked();
            try
            {
                if (bytes == null)
                {
                    throw new ArgumentNullException("Bytes should not be null when passed to DeserializeGraphs");
                }
                AstarSerializer sr = new AstarSerializer(this);
                if (sr.OpenDeserialize(bytes))
                {
                    this.DeserializeGraphsPartAdditive(sr);
                    sr.CloseDeserialize();
                }
                else
                {
                    Debug.Log("Invalid data file (cannot read zip).");
                }
                this.active.VerifyIntegrity();
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Caught exception while deserializing data.\n" + exception);
            }
        }

        public void DeserializeGraphsPart(AstarSerializer sr)
        {
            this.ClearGraphs();
            this.graphs = sr.DeserializeGraphs();
            if (this.graphs != null)
            {
                for (int i = 0; i < this.graphs.Length; i++)
                {
                    if (this.graphs[i] != null)
                    {
                        this.graphs[i].graphIndex = (uint) i;
                    }
                }
            }
            this.userConnections = sr.DeserializeUserConnections();
            sr.DeserializeExtraInfo();
            <DeserializeGraphsPart>c__AnonStorey19 storey = new <DeserializeGraphsPart>c__AnonStorey19 {
                i = 0
            };
            while (storey.i < this.graphs.Length)
            {
                if (this.graphs[storey.i] != null)
                {
                    this.graphs[storey.i].GetNodes(new GraphNodeDelegateCancelable(storey.<>m__7));
                }
                storey.i++;
            }
            sr.PostDeserialization();
        }

        public void DeserializeGraphsPartAdditive(AstarSerializer sr)
        {
            if (this.graphs == null)
            {
                this.graphs = new NavGraph[0];
            }
            if (this.userConnections == null)
            {
                this.userConnections = new UserConnection[0];
            }
            List<NavGraph> list = new List<NavGraph>(this.graphs);
            list.AddRange(sr.DeserializeGraphs());
            this.graphs = list.ToArray();
            if (this.graphs != null)
            {
                for (int j = 0; j < this.graphs.Length; j++)
                {
                    if (this.graphs[j] != null)
                    {
                        this.graphs[j].graphIndex = (uint) j;
                    }
                }
            }
            List<UserConnection> list2 = new List<UserConnection>(this.userConnections);
            list2.AddRange(sr.DeserializeUserConnections());
            this.userConnections = list2.ToArray();
            sr.DeserializeNodes();
            DebugHelper.Assert(this.graphs != null, "不能为空");
            <DeserializeGraphsPartAdditive>c__AnonStorey1A storeya = new <DeserializeGraphsPartAdditive>c__AnonStorey1A {
                i = 0
            };
            while (storeya.i < this.graphs.Length)
            {
                if (this.graphs[storeya.i] != null)
                {
                    this.graphs[storeya.i].GetNodes(new GraphNodeDelegateCancelable(storeya.<>m__8));
                }
                storeya.i++;
            }
            sr.DeserializeExtraInfo();
            sr.PostDeserialization();
            for (int i = 0; i < this.graphs.Length; i++)
            {
                for (int k = i + 1; k < this.graphs.Length; k++)
                {
                    if (((this.graphs[i] != null) && (this.graphs[k] != null)) && (this.graphs[i].guid == this.graphs[k].guid))
                    {
                        Debug.LogWarning("Guid Conflict when importing graphs additively. Imported graph will get a new Guid.\nThis message is (relatively) harmless.");
                        this.graphs[i].guid = Pathfinding.Util.Guid.NewGuid();
                        break;
                    }
                }
            }
        }

        public NavGraph FindGraphOfType(System.Type type)
        {
            if (this.graphs != null)
            {
                for (int i = 0; i < this.graphs.Length; i++)
                {
                    if ((this.graphs[i] != null) && object.Equals(this.graphs[i].GetType(), type))
                    {
                        return this.graphs[i];
                    }
                }
            }
            return null;
        }

        [DebuggerHidden]
        public IEnumerable FindGraphsOfType(System.Type type)
        {
            return new <FindGraphsOfType>c__Iterator1 { type = type, <$>type = type, <>f__this = this, $PC = -2 };
        }

        public void FindGraphTypes()
        {
            System.Type[] types = Assembly.GetAssembly(typeof(AstarPath)).GetTypes();
            List<System.Type> list = new List<System.Type>();
            foreach (System.Type type in types)
            {
                for (System.Type type2 = type.BaseType; type2 != null; type2 = type2.BaseType)
                {
                    if (object.Equals(type2, typeof(NavGraph)))
                    {
                        list.Add(type);
                        break;
                    }
                }
            }
            this.graphTypes = list.ToArray();
        }

        public TriangleMeshNode FindNearestByRasterizer(VInt3 position, int maxRange = -1)
        {
            int num;
            int num2;
            if (this.rasterizer == null)
            {
                return null;
            }
            this.rasterizer.GetCellPosClamped(out num, out num2, position);
            long minDist = 0x7fffffffffffffffL;
            TriangleMeshNode nearestNode = null;
            List<object> objs = this.rasterizer.GetObjs(num, num2);
            if (!this.getNearest(position, objs, ref minDist, ref nearestNode))
            {
                int num4 = 1;
                int max = Mathf.Max(this.rasterizer.numCellsX, this.rasterizer.numCellsY);
                if (maxRange == -1)
                {
                    maxRange = max;
                }
                else
                {
                    maxRange = Mathf.Clamp(maxRange, 1, max);
                }
                while (num4 < maxRange)
                {
                    int x = Mathf.Max(num - num4, 0);
                    int num7 = Mathf.Min((int) (num + num4), (int) (this.rasterizer.numCellsX - 1));
                    int y = Mathf.Max(num2 - num4, 0);
                    int num9 = Mathf.Min((int) (num2 + num4), (int) (this.rasterizer.numCellsY - 1));
                    if ((num - num4) == x)
                    {
                        for (int i = y; i <= num9; i++)
                        {
                            this.getNearest(position, this.rasterizer.GetObjs(x, i), ref minDist, ref nearestNode);
                        }
                    }
                    if ((num + num4) == num7)
                    {
                        for (int j = y; j <= num9; j++)
                        {
                            this.getNearest(position, this.rasterizer.GetObjs(num7, j), ref minDist, ref nearestNode);
                        }
                    }
                    if ((num2 - num4) == y)
                    {
                        for (int k = x + 1; k < num7; k++)
                        {
                            this.getNearest(position, this.rasterizer.GetObjs(k, y), ref minDist, ref nearestNode);
                        }
                    }
                    if ((num2 + num4) == num9)
                    {
                        for (int m = x + 1; m < num7; m++)
                        {
                            this.getNearest(position, this.rasterizer.GetObjs(m, num9), ref minDist, ref nearestNode);
                        }
                    }
                    if (nearestNode != null)
                    {
                        return nearestNode;
                    }
                    num4++;
                }
            }
            return nearestNode;
        }

        public byte[] GetData()
        {
            return this.data;
        }

        public static NavGraph GetGraph(GraphNode node)
        {
            if (node == null)
            {
                return null;
            }
            AstarPath active = AstarPath.active;
            if (active == null)
            {
                return null;
            }
            AstarData astarData = active.astarData;
            if (astarData == null)
            {
                return null;
            }
            if (astarData.graphs == null)
            {
                return null;
            }
            uint graphIndex = node.GraphIndex;
            if (graphIndex >= astarData.graphs.Length)
            {
                return null;
            }
            return astarData.graphs[graphIndex];
        }

        public int GetGraphIndex(NavGraph graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }
            if (this.graphs != null)
            {
                for (int i = 0; i < this.graphs.Length; i++)
                {
                    if (graph == this.graphs[i])
                    {
                        return i;
                    }
                }
            }
            Debug.LogError("Graph doesn't exist");
            return -1;
        }

        public System.Type GetGraphType(string type)
        {
            for (int i = 0; i < this.graphTypes.Length; i++)
            {
                if (this.graphTypes[i].Name == type)
                {
                    return this.graphTypes[i];
                }
            }
            return null;
        }

        public TriangleMeshNode GetLocatedByRasterizer(VInt3 position)
        {
            TriangleMeshNode node = null;
            if (this.rasterizer != null)
            {
                List<object> located = this.rasterizer.GetLocated(position);
                if (located == null)
                {
                    return node;
                }
                for (int i = 0; i < located.Count; i++)
                {
                    VInt3 num;
                    VInt3 num2;
                    VInt3 num3;
                    TriangleMeshNode node2 = located[i] as TriangleMeshNode;
                    if (node2 == null)
                    {
                        return node;
                    }
                    node2.GetPoints(out num, out num2, out num3);
                    if (Polygon.ContainsPoint(num, num2, num3, position))
                    {
                        return node2;
                    }
                }
            }
            return node;
        }

        private bool getNearest(VInt3 position, List<object> objs, ref long minDist, ref TriangleMeshNode nearestNode)
        {
            if ((objs == null) || (objs.Count == 0))
            {
                return false;
            }
            bool flag = false;
            for (int i = 0; i < objs.Count; i++)
            {
                TriangleMeshNode node = objs[i] as TriangleMeshNode;
                if (node == null)
                {
                    return false;
                }
                long num2 = position.XZSqrMagnitude(node.position);
                if (num2 < minDist)
                {
                    minDist = num2;
                    nearestNode = node;
                    flag = true;
                }
            }
            return flag;
        }

        public TriangleMeshNode GetNearestByRasterizer(VInt3 position, out VInt3 clampedPosition)
        {
            clampedPosition = VInt3.zero;
            if (this.rasterizer == null)
            {
                return null;
            }
            TriangleMeshNode locatedByRasterizer = this.GetLocatedByRasterizer(position);
            if (locatedByRasterizer != null)
            {
                clampedPosition = position;
                return locatedByRasterizer;
            }
            locatedByRasterizer = this.FindNearestByRasterizer(position, -1);
            if (locatedByRasterizer == null)
            {
                return null;
            }
            clampedPosition = locatedByRasterizer.ClosestPointOnNodeXZ(position);
            return locatedByRasterizer;
        }

        public GraphNode GetNode(int graphIndex, int nodeIndex)
        {
            return this.GetNode(graphIndex, nodeIndex, this.graphs);
        }

        public GraphNode GetNode(int graphIndex, int nodeIndex, NavGraph[] graphs)
        {
            throw new NotImplementedException();
        }

        [DebuggerHidden]
        public IEnumerable GetRaycastableGraphs()
        {
            return new <GetRaycastableGraphs>c__Iterator3 { <>f__this = this, $PC = -2 };
        }

        [DebuggerHidden]
        public IEnumerable GetUpdateableGraphs()
        {
            return new <GetUpdateableGraphs>c__Iterator2 { <>f__this = this, $PC = -2 };
        }

        public NavGraph GuidToGraph(Pathfinding.Util.Guid guid)
        {
            if (this.graphs != null)
            {
                for (int i = 0; i < this.graphs.Length; i++)
                {
                    if ((this.graphs[i] != null) && (this.graphs[i].guid == guid))
                    {
                        return this.graphs[i];
                    }
                }
            }
            return null;
        }

        public int GuidToIndex(Pathfinding.Util.Guid guid)
        {
            if (this.graphs != null)
            {
                for (int i = 0; i < this.graphs.Length; i++)
                {
                    if ((this.graphs[i] != null) && (this.graphs[i].guid == guid))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public GraphNodeRasterizer InitRasterizer(int inCellSize, bool bPot)
        {
            <InitRasterizer>c__AnonStorey18 storey = new <InitRasterizer>c__AnonStorey18();
            if ((this.graphs == null) || (this.graphs.Length == 0))
            {
                return null;
            }
            storey.min = new VInt2(0x7fffffff, 0x7fffffff);
            storey.max = new VInt2(-2147483648, -2147483648);
            for (int i = 0; i < this.graphs.Length; i++)
            {
                RecastGraph graph = this.graphs[i] as RecastGraph;
                if (graph == null)
                {
                    return null;
                }
                graph.GetNodes(new GraphNodeDelegateCancelable(storey.<>m__5));
            }
            storey.r = new GraphNodeRasterizer();
            if (!bPot)
            {
                storey.r.Init(storey.min, storey.max.x - storey.min.x, storey.max.y - storey.min.y, inCellSize);
            }
            else
            {
                storey.r.InitPot(storey.min, storey.max.x - storey.min.x, storey.max.y - storey.min.y, inCellSize);
            }
            for (int j = 0; j < this.graphs.Length; j++)
            {
                RecastGraph graph2 = this.graphs[j] as RecastGraph;
                if (graph2 != null)
                {
                    graph2.GetNodes(new GraphNodeDelegateCancelable(storey.<>m__6));
                }
            }
            return storey.r;
        }

        public TriangleMeshNode IntersectByRasterizer(VInt3 start, VInt3 end, out int edge)
        {
            edge = -1;
            if (this.rasterizer != null)
            {
                int num = end.x - start.x;
                int num2 = end.z - start.z;
                int num3 = start.x - this.rasterizer.origin.x;
                int num4 = start.z - this.rasterizer.origin.y;
                int num5 = Mathf.Abs(num);
                int num6 = num3 % this.rasterizer.cellSize;
                int num7 = (num <= 0) ? (-num6 - 1) : (this.rasterizer.cellSize - num6);
                int cellSize = Mathf.Abs(num7);
                int num9 = this.rasterizer.numCellsX * this.rasterizer.cellSize;
                int num10 = this.rasterizer.numCellsY * this.rasterizer.cellSize;
                int num11 = num3;
                int num12 = num4;
                while (((num5 >= 0) && (num11 >= 0)) && (num11 < num9))
                {
                    int gridX = num11 / this.rasterizer.cellSize;
                    int num14 = Mathf.Abs((num == 0) ? num2 : IntMath.Divide((int) (num2 * num7), num));
                    int num15 = num12 % this.rasterizer.cellSize;
                    int num16 = (num2 < 0) ? (-num15 - 1) : (this.rasterizer.cellSize - num15);
                    int num17 = Mathf.Abs(num16);
                    int num18 = num12;
                    while (((num14 >= 0) && (num12 >= 0)) && (num12 < num10))
                    {
                        int gridY = num12 / this.rasterizer.cellSize;
                        TriangleMeshNode node = this.checkObjIntersects(ref edge, start, end, gridX, gridY);
                        if (node != null)
                        {
                            return node;
                        }
                        num12 += num16;
                        num14 -= num17;
                        num16 = (num2 < 0) ? -this.rasterizer.cellSize : this.rasterizer.cellSize;
                        num17 = this.rasterizer.cellSize;
                    }
                    num11 += num7;
                    num5 -= cellSize;
                    num7 = (num < 0) ? -this.rasterizer.cellSize : this.rasterizer.cellSize;
                    cellSize = this.rasterizer.cellSize;
                    if (num != 0)
                    {
                        num12 = ((num18 * num) + (num2 * num7)) / num;
                    }
                }
            }
            return null;
        }

        public void LoadFromCache()
        {
            AstarPath.active.BlockUntilPathQueueBlocked();
            if ((this.data_cachedStartup != null) && (this.data_cachedStartup.Length > 0))
            {
                this.DeserializeGraphs(this.data_cachedStartup);
                GraphModifier.TriggerEvent(GraphModifier.EventType.PostCacheLoad);
            }
            else
            {
                Debug.LogError("Can't load from cache since the cache is empty");
            }
        }

        public void OnDestroy()
        {
            this.ClearGraphs();
        }

        public void RasterizeGraphNodes()
        {
            this.RasterizeGraphNodesInternal();
        }

        private void RasterizeGraphNodesInternal()
        {
            this.rasterizer = this.InitRasterizer(0xfa0, false);
        }

        public bool RemoveGraph(NavGraph graph)
        {
            graph.SafeOnDestroy();
            int index = 0;
            while (index < this.graphs.Length)
            {
                if (this.graphs[index] == graph)
                {
                    break;
                }
                index++;
            }
            if (index == this.graphs.Length)
            {
                return false;
            }
            this.graphs[index] = null;
            this.UpdateShortcuts();
            return true;
        }

        public void SaveCacheData(SerializeSettings settings)
        {
            this.data_cachedStartup = this.SerializeGraphs(settings);
            this.cacheStartup = true;
        }

        public byte[] SerializeGraphs()
        {
            return this.SerializeGraphs(SerializeSettings.Settings);
        }

        public byte[] SerializeGraphs(SerializeSettings settings)
        {
            uint num;
            return this.SerializeGraphs(settings, out num);
        }

        public byte[] SerializeGraphs(SerializeSettings settings, out uint checksum)
        {
            AstarPath.active.BlockUntilPathQueueBlocked();
            AstarSerializer sr = new AstarSerializer(this, settings);
            sr.OpenSerialize();
            this.SerializeGraphsPart(sr);
            byte[] buffer = sr.CloseSerialize();
            checksum = sr.GetChecksum();
            return buffer;
        }

        public byte[] SerializeGraphsExtra(SerializeSettings settings)
        {
            AstarPath.active.BlockUntilPathQueueBlocked();
            AstarSerializer serializer = new AstarSerializer(this, settings);
            serializer.OpenSerialize();
            serializer.graphs = this.graphs;
            byte[] buffer = serializer.SerializeExtraInfoBytes();
            serializer.CloseSerialize();
            return buffer;
        }

        public void SerializeGraphsPart(AstarSerializer sr)
        {
            sr.SerializeGraphs(this.graphs);
            sr.SerializeUserConnections(this.userConnections);
            sr.SerializeNodes();
            sr.SerializeExtraInfo();
        }

        public void SetData(byte[] data, uint checksum)
        {
            this.data = data;
            this.dataChecksum = checksum;
        }

        public void UpdateShortcuts()
        {
            this.navmesh = (NavMeshGraph) this.FindGraphOfType(typeof(NavMeshGraph));
            this.recastGraph = (RecastGraph) this.FindGraphOfType(typeof(RecastGraph));
        }

        public AstarPath active
        {
            get
            {
                return AstarPath.active;
            }
        }

        [CompilerGenerated]
        private sealed class <DeserializeGraphsPart>c__AnonStorey19
        {
            internal int i;

            internal bool <>m__7(GraphNode node)
            {
                node.GraphIndex = (uint) this.i;
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <DeserializeGraphsPartAdditive>c__AnonStorey1A
        {
            internal int i;

            internal bool <>m__8(GraphNode node)
            {
                node.GraphIndex = (uint) this.i;
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <FindGraphsOfType>c__Iterator1 : IDisposable, IEnumerable, IEnumerator, IEnumerator<object>, IEnumerable<object>
        {
            internal object $current;
            internal int $PC;
            internal System.Type <$>type;
            internal AstarData <>f__this;
            internal int <i>__0;
            internal System.Type type;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (this.<>f__this.graphs != null)
                        {
                            this.<i>__0 = 0;
                            while (this.<i>__0 < this.<>f__this.graphs.Length)
                            {
                                if ((this.<>f__this.graphs[this.<i>__0] != null) && object.Equals(this.<>f__this.graphs[this.<i>__0].GetType(), this.type))
                                {
                                    this.$current = this.<>f__this.graphs[this.<i>__0];
                                    this.$PC = 1;
                                    return true;
                                }
                            Label_00A4:
                                this.<i>__0++;
                            }
                            this.$PC = -1;
                            break;
                        }
                        break;

                    case 1:
                        goto Label_00A4;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<object> IEnumerable<object>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AstarData.<FindGraphsOfType>c__Iterator1 { <>f__this = this.<>f__this, type = this.<$>type };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<object>.GetEnumerator();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetRaycastableGraphs>c__Iterator3 : IDisposable, IEnumerable, IEnumerator, IEnumerator<object>, IEnumerable<object>
        {
            internal object $current;
            internal int $PC;
            internal AstarData <>f__this;
            internal int <i>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (this.<>f__this.graphs != null)
                        {
                            this.<i>__0 = 0;
                            while (this.<i>__0 < this.<>f__this.graphs.Length)
                            {
                                if ((this.<>f__this.graphs[this.<i>__0] != null) && (this.<>f__this.graphs[this.<i>__0] is IRaycastableGraph))
                                {
                                    this.$current = this.<>f__this.graphs[this.<i>__0];
                                    this.$PC = 1;
                                    return true;
                                }
                            Label_0099:
                                this.<i>__0++;
                            }
                            this.$PC = -1;
                            break;
                        }
                        break;

                    case 1:
                        goto Label_0099;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<object> IEnumerable<object>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AstarData.<GetRaycastableGraphs>c__Iterator3 { <>f__this = this.<>f__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<object>.GetEnumerator();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetUpdateableGraphs>c__Iterator2 : IDisposable, IEnumerable, IEnumerator, IEnumerator<object>, IEnumerable<object>
        {
            internal object $current;
            internal int $PC;
            internal AstarData <>f__this;
            internal int <i>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (this.<>f__this.graphs != null)
                        {
                            this.<i>__0 = 0;
                            while (this.<i>__0 < this.<>f__this.graphs.Length)
                            {
                                if ((this.<>f__this.graphs[this.<i>__0] != null) && (this.<>f__this.graphs[this.<i>__0] is IUpdatableGraph))
                                {
                                    this.$current = this.<>f__this.graphs[this.<i>__0];
                                    this.$PC = 1;
                                    return true;
                                }
                            Label_0099:
                                this.<i>__0++;
                            }
                            this.$PC = -1;
                            break;
                        }
                        break;

                    case 1:
                        goto Label_0099;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<object> IEnumerable<object>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AstarData.<GetUpdateableGraphs>c__Iterator2 { <>f__this = this.<>f__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<object>.GetEnumerator();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <InitRasterizer>c__AnonStorey18
        {
            internal VInt2 max;
            internal VInt2 min;
            internal GraphNodeRasterizer r;

            internal bool <>m__5(GraphNode node)
            {
                TriangleMeshNode node2 = node as TriangleMeshNode;
                if (node2 != null)
                {
                    VInt2 xz = node2.GetVertex(0).xz;
                    VInt2 r = node2.GetVertex(1).xz;
                    VInt2 num3 = node2.GetVertex(2).xz;
                    this.min.Min(ref xz);
                    this.min.Min(ref r);
                    this.min.Min(ref num3);
                    this.max.Max(ref xz);
                    this.max.Max(ref r);
                    this.max.Max(ref num3);
                }
                return true;
            }

            internal bool <>m__6(GraphNode node)
            {
                TriangleMeshNode data = node as TriangleMeshNode;
                if (data != null)
                {
                    VInt2 xz = data.GetVertex(0).xz;
                    VInt2 num2 = data.GetVertex(1).xz;
                    VInt2 num3 = data.GetVertex(2).xz;
                    this.r.AddTriangle(ref xz, ref num2, ref num3, data);
                }
                return true;
            }
        }
    }
}

