using Pathfinding;
using Pathfinding.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

[AddComponentMenu("Pathfinding/Pathfinder")]
public class AstarPath : MonoBehaviour
{
    [CompilerGenerated]
    private static GraphNodeDelegateCancelable <>f__am$cache4E;
    [CompilerGenerated]
    private static GraphNodeDelegateCancelable <>f__am$cache4F;
    public static AstarPath active;
    public AstarData astarData;
    public AstarData[] astarDataArray;
    public static readonly string Branch = "master_Pro";
    public AstarColor colorSettings;
    public float debugFloor;
    public GraphDebugMode debugMode;
    public Path debugPath;
    public float debugRoof = 20000f;
    public static readonly AstarDistribution Distribution = AstarDistribution.AssetStore;
    public EuclideanEmbedding euclideanEmbedding = new EuclideanEmbedding();
    private Stack<GraphNode> floodStack;
    public bool fullGetNearestSearch;
    [NonSerialized, HideInInspector]
    public int gizmosDataIndex;
    private AutoResetEvent graphUpdateAsyncEvent = new AutoResetEvent(false);
    private Queue<GraphUpdateObject> graphUpdateQueue;
    private Queue<GUOSingle> graphUpdateQueueAsync = new Queue<GUOSingle>();
    private Queue<GUOSingle> graphUpdateQueueRegular = new Queue<GUOSingle>();
    private bool graphUpdateRoutineRunning;
    private Thread graphUpdateThread;
    public static readonly bool HasPro = true;
    public Heuristic heuristic = Heuristic.Euclidean;
    public float heuristicScale = 1f;
    public string inGameDebugPath;
    public static bool isEditor = true;
    private bool isRegisteredForUpdate;
    public bool isScanning;
    private float lastGraphUpdate = -9999f;
    public float lastScanTime;
    public uint lastUniqueAreaIndex;
    public bool limitGraphUpdates = true;
    public PathLog logPathResults = PathLog.OnlyErrors;
    public bool manualDebugFloorRoof;
    public float maxFrameTime = 1f;
    public float maxGraphUpdateFreq = 0.2f;
    public float maxNearestNodeDistance = 100f;
    public int minAreaSize;
    private ushort nextFreePathID = 1;
    private int nextNodeIndex = 1;
    private Stack<int> nodeIndexPool = new Stack<int>();
    public static OnVoidDelegate On65KOverflow;
    public static OnVoidDelegate OnAwakeSettings;
    public OnVoidDelegate OnDrawGizmosCallback;
    public static OnGraphDelegate OnGraphPostScan;
    public static OnGraphDelegate OnGraphPreScan;
    public static OnScanDelegate OnGraphsUpdated;
    [Obsolete]
    public OnVoidDelegate OnGraphsWillBeUpdated;
    [Obsolete]
    public OnVoidDelegate OnGraphsWillBeUpdated2;
    public static OnScanDelegate OnLatePostScan;
    public static OnPathDelegate OnPathPostSearch;
    public static OnPathDelegate OnPathPreSearch;
    public static OnScanDelegate OnPostScan;
    public static OnScanDelegate OnPreScan;
    private static OnVoidDelegate OnThreadSafeCallback;
    private ThreadControlQueue pathQueue = new ThreadControlQueue(0);
    private Path pathReturnPop;
    private static LockFreeStack pathReturnStack = new LockFreeStack();
    public static int PathsCompleted = 0;
    public bool prioritizeGraphs;
    public float prioritizeGraphsLimit = 1f;
    private ManualResetEvent processingGraphUpdatesAsync = new ManualResetEvent(true);
    private bool processingWorkItems;
    private bool queuedWorkItemFloodFill;
    private static readonly object safeUpdateLock = new object();
    public bool scanOnStartup = true;
    public bool showGraphs;
    public bool showNavGraphs = true;
    public bool showSearchTree;
    public bool showUnwalkableNodes = true;
    [SerializeField]
    protected string[] tagNames;
    public ThreadCount threadCount;
    private static IEnumerator threadEnumerator;
    private static PathThreadInfo[] threadInfos = new PathThreadInfo[0];
    private static Thread[] threads;
    public float unwalkableNodeDebugSize = 0.3f;
    private static int waitForPathDepth = 0;
    private Queue<AstarWorkItem> workItems = new Queue<AstarWorkItem>();
    private bool workItemsQueued;

    public void AddWorkItem(AstarWorkItem itm)
    {
        this.workItems.Enqueue(itm);
        if (!this.workItemsQueued)
        {
            this.workItemsQueued = true;
            if (!this.isScanning)
            {
                InterruptPathfinding();
            }
        }
    }

    public void ApplyLinks()
    {
        if ((this.astarData.userConnections != null) && (this.astarData.userConnections.Length > 0))
        {
            Debug.LogWarning("<b>Deleting all links now</b>, but saving graph data in backup variable.\nCreating replacement links using the new system, stored under the <i>Links</i> GameObject.");
            GameObject obj2 = new GameObject("Links");
            Dictionary<VInt3, GameObject> dictionary = new Dictionary<VInt3, GameObject>();
            for (int i = 0; i < this.astarData.userConnections.Length; i++)
            {
                UserConnection connection = this.astarData.userConnections[i];
                GameObject obj3 = !dictionary.ContainsKey((VInt3) connection.p1) ? new GameObject("Link " + i) : dictionary[(VInt3) connection.p1];
                GameObject obj4 = !dictionary.ContainsKey((VInt3) connection.p2) ? new GameObject("Link " + i) : dictionary[(VInt3) connection.p2];
                obj3.transform.parent = obj2.transform;
                obj4.transform.parent = obj2.transform;
                dictionary[(VInt3) connection.p1] = obj3;
                dictionary[(VInt3) connection.p2] = obj4;
                obj3.transform.position = connection.p1;
                obj4.transform.position = connection.p2;
                NodeLink link = obj3.AddComponent<NodeLink>();
                link.end = obj4.transform;
                link.deleteConnection = !connection.enable;
            }
            this.astarData.userConnections = null;
            this.astarData.data_backup = this.astarData.GetData();
            throw new NotSupportedException("<b>Links have been deprecated</b>. Please use the component <b>NodeLink</b> instead. Create two GameObjects around the points you want to link, then press <b>Cmd+Alt+L</b> ( <b>Ctrl+Alt+L</b> on windows) to link them. See <b>Menubar -> Edit -> Pathfinding</b>.");
        }
    }

    private static void AstarLog(string s)
    {
        if (object.ReferenceEquals(active, null))
        {
            Debug.Log("No AstarPath object was found : " + s);
        }
        else if ((active.logPathResults != PathLog.None) && (active.logPathResults != PathLog.OnlyErrors))
        {
            Debug.Log(s);
        }
    }

    private static void AstarLogError(string s)
    {
        if (active == null)
        {
            Debug.Log("No AstarPath object was found : " + s);
        }
        else if (active.logPathResults != PathLog.None)
        {
            Debug.LogError(s);
        }
    }

    public void Awake()
    {
        active = this;
        if (UnityEngine.Object.FindObjectsOfType(typeof(AstarPath)).Length > 1)
        {
            Debug.LogError("You should NOT have more than one AstarPath component in the scene at any time.\nThis can cause serious errors since the AstarPath component builds around a singleton pattern.");
        }
        base.useGUILayout = false;
        isEditor = Application.isEditor;
        if (OnAwakeSettings != null)
        {
            OnAwakeSettings();
        }
        GraphModifier.FindAllModifiers();
        RelevantGraphSurface.FindAllGraphSurfaces();
        int num = CalculateThreadCount(this.threadCount);
        threads = new Thread[num];
        threadInfos = new PathThreadInfo[Math.Max(num, 1)];
        this.pathQueue = new ThreadControlQueue(threadInfos.Length);
        for (int i = 0; i < threadInfos.Length; i++)
        {
            threadInfos[i] = new PathThreadInfo(i, this, new PathHandler(i, threadInfos.Length));
        }
        for (int j = 0; j < threads.Length; j++)
        {
            threads[j] = new Thread(new ParameterizedThreadStart(AstarPath.CalculatePathsThreaded));
            threads[j].Name = "Pathfinding Thread " + j;
            threads[j].IsBackground = true;
        }
        if (num == 0)
        {
            threadEnumerator = CalculatePaths(threadInfos[0]);
        }
        else
        {
            threadEnumerator = null;
        }
        for (int k = 0; k < threads.Length; k++)
        {
            if (this.logPathResults == PathLog.Heavy)
            {
                Debug.Log("Starting pathfinding thread " + k);
            }
            threads[k].Start(threadInfos[k]);
        }
        if (num != 0)
        {
            this.graphUpdateThread = new Thread(new ParameterizedThreadStart(this.ProcessGraphUpdatesAsync));
            this.graphUpdateThread.IsBackground = true;
            this.graphUpdateThread.Priority = System.Threading.ThreadPriority.Lowest;
            this.graphUpdateThread.Start(this);
        }
        this.Initialize();
        this.FlushWorkItems(true, false);
        this.euclideanEmbedding.dirty = true;
        if (this.scanOnStartup && (!this.astarData.cacheStartup || (this.astarData.data_cachedStartup == null)))
        {
            this.Scan();
        }
        this.astarData.RasterizeGraphNodes();
    }

    public void BlockUntilPathQueueBlocked()
    {
        if (this.pathQueue != null)
        {
            this.pathQueue.Block();
            while (!this.pathQueue.AllReceiversBlocked)
            {
                if (IsUsingMultithreading)
                {
                    Thread.Sleep(1);
                }
                else
                {
                    threadEnumerator.MoveNext();
                }
            }
        }
    }

    [DebuggerHidden]
    private static IEnumerator CalculatePaths(object _threadInfo)
    {
        return new <CalculatePaths>c__Iterator6 { _threadInfo = _threadInfo, <$>_threadInfo = _threadInfo };
    }

    private static void CalculatePathsThreaded(object _threadInfo)
    {
        PathThreadInfo info;
        try
        {
            info = (PathThreadInfo) _threadInfo;
        }
        catch (Exception exception)
        {
            Debug.LogError("Arguments to pathfinding threads must be of type ThreadStartInfo\n" + exception);
            throw new ArgumentException("Argument must be of type ThreadStartInfo", exception);
        }
        AstarPath astar = info.astar;
        try
        {
            PathHandler runData = info.runData;
            if (runData.nodes == null)
            {
                throw new NullReferenceException("NodeRuns must be assigned to the threadInfo.runData.nodes field before threads are started\nthreadInfo is an argument to the thread functions");
            }
            long num = (long) (astar.maxFrameTime * 10000f);
            long targetTick = DateTime.UtcNow.Ticks + num;
            while (true)
            {
                Path p = astar.pathQueue.Pop();
                num = (long) (astar.maxFrameTime * 10000f);
                p.PrepareBase(runData);
                p.AdvanceState(PathState.Processing);
                if (OnPathPreSearch != null)
                {
                    OnPathPreSearch(p);
                }
                long ticks = DateTime.UtcNow.Ticks;
                long num4 = 0L;
                p.Prepare();
                if (!p.IsDone())
                {
                    astar.debugPath = p;
                    p.Initialize();
                    while (!p.IsDone())
                    {
                        p.CalculateStep(targetTick);
                        p.searchIterations++;
                        if (p.IsDone())
                        {
                            break;
                        }
                        num4 += DateTime.UtcNow.Ticks - ticks;
                        Thread.Sleep(0);
                        ticks = DateTime.UtcNow.Ticks;
                        targetTick = ticks + num;
                        if (astar.pathQueue.IsTerminating)
                        {
                            p.Error();
                        }
                    }
                    num4 += DateTime.UtcNow.Ticks - ticks;
                    p.duration = num4 * 0.0001f;
                }
                p.Cleanup();
                astar.LogPathResults(p);
                if (p.immediateCallback != null)
                {
                    p.immediateCallback(p);
                }
                if (OnPathPostSearch != null)
                {
                    OnPathPostSearch(p);
                }
                pathReturnStack.Push(p);
                p.AdvanceState(PathState.ReturnQueue);
                if (DateTime.UtcNow.Ticks > targetTick)
                {
                    Thread.Sleep(1);
                    targetTick = DateTime.UtcNow.Ticks + num;
                }
            }
        }
        catch (Exception exception2)
        {
            if ((exception2 is ThreadAbortException) || (exception2 is ThreadControlQueue.QueueTerminationException))
            {
                if (astar.logPathResults == PathLog.Heavy)
                {
                    Debug.LogWarning("Shutting down pathfinding thread #" + info.threadIndex + " with Thread.Abort call");
                }
                return;
            }
            Debug.LogException(exception2);
            Debug.LogError("Unhandled exception during pathfinding. Terminating.");
            astar.pathQueue.TerminateReceivers();
        }
        Debug.LogError("Error : This part should never be reached.");
        astar.pathQueue.ReceiverTerminated();
    }

    public static int CalculateThreadCount(ThreadCount count)
    {
        if ((count != ThreadCount.AutomaticLowLoad) && (count != ThreadCount.AutomaticHighLoad))
        {
            return (int) count;
        }
        int num = Mathf.Max(1, SystemInfo.processorCount);
        int systemMemorySize = SystemInfo.systemMemorySize;
        if (systemMemorySize <= 0)
        {
            Debug.LogError("Machine reporting that is has <= 0 bytes of RAM. This is definitely not true, assuming 1 GiB");
            systemMemorySize = 0x400;
        }
        if (num <= 1)
        {
            return 0;
        }
        if (systemMemorySize <= 0x200)
        {
            return 0;
        }
        if (count == ThreadCount.AutomaticHighLoad)
        {
            if (systemMemorySize <= 0x400)
            {
                num = Math.Min(num, 2);
            }
            return num;
        }
        num /= 2;
        num = Mathf.Max(1, num);
        if (systemMemorySize <= 0x400)
        {
            num = Math.Min(num, 2);
        }
        return Math.Min(num, 6);
    }

    [DebuggerHidden]
    private IEnumerator DelayedGraphUpdate()
    {
        return new <DelayedGraphUpdate>c__Iterator4 { <>f__this = this };
    }

    public void DestroyNode(GraphNode node)
    {
        if (node.NodeIndex != -1)
        {
            this.nodeIndexPool.Push(node.NodeIndex);
            if (threadInfos == null)
            {
                threadInfos = new PathThreadInfo[0];
            }
            for (int i = 0; i < threadInfos.Length; i++)
            {
                threadInfos[i].runData.DestroyNode(node);
            }
        }
    }

    private bool DrawUnwalkableNode(GraphNode node)
    {
        if (!node.Walkable)
        {
            Gizmos.DrawCube((Vector3) node.position, (Vector3) (Vector3.one * this.unwalkableNodeDebugSize));
        }
        return true;
    }

    public void EnsureValidFloodFill()
    {
        if (this.queuedWorkItemFloodFill)
        {
            this.FloodFill();
        }
    }

    public static string[] FindTagNames()
    {
        if (active != null)
        {
            return active.GetTagNames();
        }
        AstarPath path = UnityEngine.Object.FindObjectOfType(typeof(AstarPath)) as AstarPath;
        if (path != null)
        {
            active = path;
            return path.GetTagNames();
        }
        return new string[] { "There is no AstarPath component in the scene" };
    }

    [ContextMenu("Flood Fill Graphs")]
    public void FloodFill()
    {
        <FloodFill>c__AnonStorey1B storeyb = new <FloodFill>c__AnonStorey1B {
            <>f__this = this
        };
        this.queuedWorkItemFloodFill = false;
        if (this.astarData.graphs != null)
        {
            storeyb.area = 0;
            this.lastUniqueAreaIndex = 0;
            if (this.floodStack == null)
            {
                this.floodStack = new Stack<GraphNode>(0x400);
            }
            storeyb.stack = this.floodStack;
            for (int i = 0; i < this.graphs.Length; i++)
            {
                NavGraph graph = this.graphs[i];
                if (graph != null)
                {
                    if (<>f__am$cache4E == null)
                    {
                        <>f__am$cache4E = delegate (GraphNode node) {
                            node.Area = 0;
                            return true;
                        };
                    }
                    graph.GetNodes(<>f__am$cache4E);
                }
            }
            storeyb.smallAreasDetected = 0;
            storeyb.warnAboutAreas = false;
            storeyb.smallAreaList = ListPool<GraphNode>.Claim();
            for (int j = 0; j < this.graphs.Length; j++)
            {
                NavGraph graph2 = this.graphs[j];
                if (graph2 != null)
                {
                    GraphNodeDelegateCancelable del = new GraphNodeDelegateCancelable(storeyb.<>m__B);
                    graph2.GetNodes(del);
                }
            }
            this.lastUniqueAreaIndex = storeyb.area;
            if (storeyb.warnAboutAreas)
            {
                Debug.LogError("Too many areas - The maximum number of areas is " + 0x1ffff + ". Try raising the A* Inspector -> Settings -> Min Area Size value. Enable the optimization ASTAR_MORE_AREAS under the Optimizations tab.");
            }
            if (storeyb.smallAreasDetected > 0)
            {
                AstarLog(string.Concat(new object[] { storeyb.smallAreasDetected, " small areas were detected (fewer than ", this.minAreaSize, " nodes),these might have the same IDs as other areas, but it shouldn't affect pathfinding in any significant way (you might get All Nodes Searched as a reason for path failure).\nWhich areas are defined as 'small' is controlled by the 'Min Area Size' variable, it can be changed in the A* inspector-->Settings-->Min Area Size\nThe small areas will use the area id ", 0x1ffff }));
            }
            ListPool<GraphNode>.Release(storeyb.smallAreaList);
        }
    }

    public void FloodFill(GraphNode seed)
    {
        this.FloodFill(seed, this.lastUniqueAreaIndex + 1);
        this.lastUniqueAreaIndex++;
    }

    public void FloodFill(GraphNode seed, uint area)
    {
        if (area > 0x1ffff)
        {
            Debug.LogError("Too high area index - The maximum area index is " + 0x1ffff);
        }
        else if (area < 0)
        {
            Debug.LogError("Too low area index - The minimum area index is 0");
        }
        else
        {
            if (this.floodStack == null)
            {
                this.floodStack = new Stack<GraphNode>(0x400);
            }
            Stack<GraphNode> floodStack = this.floodStack;
            floodStack.Clear();
            floodStack.Push(seed);
            seed.Area = area;
            while (floodStack.Count > 0)
            {
                floodStack.Pop().FloodFill(floodStack, area);
            }
        }
    }

    public void FlushGraphUpdates()
    {
        if (this.IsAnyGraphUpdatesQueued)
        {
            this.QueueGraphUpdates();
            this.FlushWorkItems(true, true);
        }
    }

    public void FlushThreadSafeCallbacks()
    {
        if (OnThreadSafeCallback != null)
        {
            this.BlockUntilPathQueueBlocked();
            this.PerformBlockingActions(false, true);
        }
    }

    public void FlushWorkItems(bool unblockOnComplete = true, bool block = false)
    {
        this.BlockUntilPathQueueBlocked();
        this.PerformBlockingActions(block, unblockOnComplete);
    }

    public AstarData GetData(int campIndex)
    {
        if (((this.astarDataArray != null) && (campIndex >= 0)) && (campIndex < this.astarDataArray.Length))
        {
            return this.astarDataArray[campIndex];
        }
        return this.astarData;
    }

    public TriangleMeshNode GetLocatedByRasterizer(VInt3 position, int campIndex)
    {
        return this.GetData(campIndex).GetLocatedByRasterizer(position);
    }

    public GraphNode GetNearest(Ray ray)
    {
        <GetNearest>c__AnonStorey1F storeyf = new <GetNearest>c__AnonStorey1F();
        if (this.graphs == null)
        {
            return null;
        }
        storeyf.minDist = float.PositiveInfinity;
        storeyf.nearestNode = null;
        storeyf.lineDirection = ray.direction;
        storeyf.lineOrigin = ray.origin;
        for (int i = 0; i < this.graphs.Length; i++)
        {
            this.graphs[i].GetNodes(new GraphNodeDelegateCancelable(storeyf.<>m__F));
        }
        return storeyf.nearestNode;
    }

    public NNInfo GetNearest(Vector3 position)
    {
        return this.GetNearest(position, NNConstraint.None);
    }

    public NNInfo GetNearest(Vector3 position, NNConstraint constraint)
    {
        return this.GetNearest(position, constraint, null);
    }

    public NNInfo GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
    {
        if (this.graphs != null)
        {
            float positiveInfinity = float.PositiveInfinity;
            NNInfo info = new NNInfo();
            int index = -1;
            for (int i = 0; i < this.graphs.Length; i++)
            {
                NavGraph graph = this.graphs[i];
                if ((graph != null) && constraint.SuitableGraph(i, graph))
                {
                    NNInfo nearestForce;
                    if (this.fullGetNearestSearch)
                    {
                        nearestForce = graph.GetNearestForce(position, constraint);
                    }
                    else
                    {
                        nearestForce = graph.GetNearest(position, constraint);
                    }
                    if (nearestForce.node != null)
                    {
                        Vector3 vector = nearestForce.clampedPosition - position;
                        float magnitude = vector.magnitude;
                        if (this.prioritizeGraphs && (magnitude < this.prioritizeGraphsLimit))
                        {
                            positiveInfinity = magnitude;
                            info = nearestForce;
                            index = i;
                            break;
                        }
                        if (magnitude < positiveInfinity)
                        {
                            positiveInfinity = magnitude;
                            info = nearestForce;
                            index = i;
                        }
                    }
                }
            }
            if (index == -1)
            {
                return info;
            }
            if (info.constrainedNode != null)
            {
                info.node = info.constrainedNode;
                info.clampedPosition = info.constClampedPosition;
            }
            if ((!this.fullGetNearestSearch && (info.node != null)) && !constraint.Suitable(info.node))
            {
                NNInfo info3 = this.graphs[index].GetNearestForce(position, constraint);
                if (info3.node != null)
                {
                    info = info3;
                }
            }
            if (constraint.Suitable(info.node))
            {
                if (!constraint.constrainDistance)
                {
                    return info;
                }
                Vector3 vector2 = info.clampedPosition - position;
                if (vector2.sqrMagnitude <= this.maxNearestNodeDistanceSqr)
                {
                    return info;
                }
            }
        }
        return new NNInfo();
    }

    public int GetNewNodeIndex()
    {
        if (this.nodeIndexPool.Count > 0)
        {
            return this.nodeIndexPool.Pop();
        }
        return this.nextNodeIndex++;
    }

    public ushort GetNextPathID()
    {
        ushort num;
        if (this.nextFreePathID == 0)
        {
            this.nextFreePathID = (ushort) (this.nextFreePathID + 1);
            Debug.Log("65K cleanup");
            if (On65KOverflow != null)
            {
                OnVoidDelegate delegate2 = On65KOverflow;
                On65KOverflow = null;
                delegate2();
            }
        }
        this.nextFreePathID = (ushort) ((num = this.nextFreePathID) + 1);
        return num;
    }

    public string[] GetTagNames()
    {
        if ((this.tagNames == null) || (this.tagNames.Length != 0x20))
        {
            this.tagNames = new string[0x20];
            for (int i = 0; i < this.tagNames.Length; i++)
            {
                this.tagNames[i] = string.Empty + i;
            }
            this.tagNames[0] = "Basic Ground";
        }
        return this.tagNames;
    }

    private void Initialize()
    {
        this.SetUpReferences();
        this.astarData.FindGraphTypes();
        this.astarData.Awake();
        this.astarData.UpdateShortcuts();
        for (int i = 0; i < this.astarData.graphs.Length; i++)
        {
            if (this.astarData.graphs[i] != null)
            {
                this.astarData.graphs[i].Awake();
            }
        }
    }

    public void InitializeNode(GraphNode node)
    {
        if (threadInfos == null)
        {
            threadInfos = new PathThreadInfo[0];
        }
        for (int i = 0; i < threadInfos.Length; i++)
        {
            threadInfos[i].runData.InitializeNode(node);
        }
    }

    private static void InterruptPathfinding()
    {
        active.pathQueue.Block();
    }

    private void LogPathResults(Path p)
    {
        if ((this.logPathResults != PathLog.None) && ((this.logPathResults != PathLog.OnlyErrors) || p.error))
        {
            string str = p.DebugString(this.logPathResults);
            if (this.logPathResults == PathLog.InGame)
            {
                this.inGameDebugPath = str;
            }
        }
    }

    [ContextMenu("Log Profiler")]
    public void LogProfiler()
    {
    }

    public void OnApplicationQuit()
    {
        if (this.logPathResults == PathLog.Heavy)
        {
            Debug.Log("+++ Application Quitting - Cleaning Up +++");
        }
        this.OnDestroy();
        if (threads != null)
        {
            for (int i = 0; i < threads.Length; i++)
            {
                if ((threads[i] != null) && threads[i].IsAlive)
                {
                    threads[i].Abort();
                }
            }
        }
    }

    public void OnDestroy()
    {
        if (this.logPathResults == PathLog.Heavy)
        {
            Debug.Log("+++ AstarPath Component Destroyed - Cleaning Up Pathfinding Data +++");
        }
        if (active == this)
        {
            this.pathQueue.TerminateReceivers();
            this.BlockUntilPathQueueBlocked();
            this.euclideanEmbedding.dirty = false;
            this.FlushWorkItems(true, false);
            if (this.logPathResults == PathLog.Heavy)
            {
                Debug.Log("Processing Eventual Work Items");
            }
            this.graphUpdateAsyncEvent.Set();
            if (threads != null)
            {
                for (int i = 0; i < threads.Length; i++)
                {
                    if (!threads[i].Join(50))
                    {
                        Debug.LogError("Could not terminate pathfinding thread[" + i + "] in 50ms, trying Thread.Abort");
                        threads[i].Abort();
                    }
                }
            }
            if (this.logPathResults == PathLog.Heavy)
            {
                Debug.Log("Returning Paths");
            }
            this.ReturnPaths(false);
            pathReturnStack.PopAll();
            if (this.logPathResults == PathLog.Heavy)
            {
                Debug.Log("Destroying Graphs");
            }
            this.astarData.OnDestroy();
            if (this.astarDataArray != null)
            {
                for (int j = 1; j < this.astarDataArray.Length; j++)
                {
                    this.astarDataArray[j].OnDestroy();
                }
                this.astarDataArray = null;
            }
            if (this.logPathResults == PathLog.Heavy)
            {
                Debug.Log("Cleaning up variables");
            }
            this.floodStack = null;
            this.graphUpdateQueue = null;
            this.OnDrawGizmosCallback = null;
            OnAwakeSettings = null;
            OnGraphPreScan = null;
            OnGraphPostScan = null;
            OnPathPreSearch = null;
            OnPathPostSearch = null;
            OnPreScan = null;
            OnPostScan = null;
            OnLatePostScan = null;
            On65KOverflow = null;
            OnGraphsUpdated = null;
            OnThreadSafeCallback = null;
            threads = null;
            threadInfos = null;
            PathsCompleted = 0;
            active = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (active == null)
        {
            active = this;
        }
        else if (active != this)
        {
            return;
        }
        AstarData data = this.GetData(this.gizmosDataIndex);
        NavGraph[] graphs = data.graphs;
        if ((graphs != null) && (((this.pathQueue == null) || !this.pathQueue.AllReceiversBlocked) || (this.workItems.Count <= 0)))
        {
            if (this.showNavGraphs && !this.manualDebugFloorRoof)
            {
                this.debugFloor = float.PositiveInfinity;
                this.debugRoof = float.NegativeInfinity;
                for (int j = 0; j < this.graphs.Length; j++)
                {
                    if ((graphs[j] != null) && graphs[j].drawGizmos)
                    {
                        graphs[j].GetNodes(delegate (GraphNode node) {
                            if ((!active.showSearchTree || (this.debugPathData == null)) || NavGraph.InSearchTree(node, this.debugPath))
                            {
                                PathNode node2 = (this.debugPathData == null) ? null : this.debugPathData.GetPathNode(node);
                                if ((node2 != null) || (this.debugMode == GraphDebugMode.Penalty))
                                {
                                    switch (this.debugMode)
                                    {
                                        case GraphDebugMode.G:
                                            this.debugFloor = Mathf.Min(this.debugFloor, (float) node2.G);
                                            this.debugRoof = Mathf.Max(this.debugRoof, (float) node2.G);
                                            break;

                                        case GraphDebugMode.H:
                                            this.debugFloor = Mathf.Min(this.debugFloor, (float) node2.H);
                                            this.debugRoof = Mathf.Max(this.debugRoof, (float) node2.H);
                                            break;

                                        case GraphDebugMode.F:
                                            this.debugFloor = Mathf.Min(this.debugFloor, (float) node2.F);
                                            this.debugRoof = Mathf.Max(this.debugRoof, (float) node2.F);
                                            break;

                                        case GraphDebugMode.Penalty:
                                            this.debugFloor = Mathf.Min(this.debugFloor, (float) node.Penalty);
                                            this.debugRoof = Mathf.Max(this.debugRoof, (float) node.Penalty);
                                            break;
                                    }
                                }
                            }
                            return true;
                        });
                    }
                }
                if (float.IsInfinity(this.debugFloor))
                {
                    this.debugFloor = 0f;
                    this.debugRoof = 1f;
                }
                if ((this.debugRoof - this.debugFloor) < 1f)
                {
                    this.debugRoof++;
                }
            }
            for (int i = 0; i < graphs.Length; i++)
            {
                if ((graphs[i] != null) && graphs[i].drawGizmos)
                {
                    graphs[i].OnDrawGizmos(this.showNavGraphs);
                }
            }
            if (this.showNavGraphs)
            {
                this.euclideanEmbedding.OnDrawGizmos();
            }
            if (this.showUnwalkableNodes && this.showNavGraphs)
            {
                Gizmos.color = AstarColor.UnwalkableNode;
                GraphNodeDelegateCancelable del = new GraphNodeDelegateCancelable(this.DrawUnwalkableNode);
                for (int k = 0; k < graphs.Length; k++)
                {
                    if ((graphs[k] != null) && graphs[k].drawGizmos)
                    {
                        graphs[k].GetNodes(del);
                    }
                }
            }
            if (this.OnDrawGizmosCallback != null)
            {
                this.OnDrawGizmosCallback();
            }
            if ((data != null) && (data.rasterizer != null))
            {
                data.rasterizer.DrawGizmos();
            }
        }
    }

    private void PerformBlockingActions(bool force = false, bool unblockOnComplete = true)
    {
        if (this.pathQueue.AllReceiversBlocked)
        {
            this.ReturnPaths(false);
            if (OnThreadSafeCallback != null)
            {
                OnVoidDelegate onThreadSafeCallback = OnThreadSafeCallback;
                OnThreadSafeCallback = null;
                onThreadSafeCallback();
            }
            if (this.ProcessWorkItems(force) == 2)
            {
                this.workItemsQueued = false;
                if (unblockOnComplete)
                {
                    if (this.euclideanEmbedding.dirty)
                    {
                        this.euclideanEmbedding.RecalculateCosts();
                    }
                    this.pathQueue.Unblock();
                }
            }
        }
    }

    private bool ProcessGraphUpdates(bool force)
    {
        if (force)
        {
            this.processingGraphUpdatesAsync.WaitOne();
        }
        else if (!this.processingGraphUpdatesAsync.WaitOne(0))
        {
            return false;
        }
        if (this.graphUpdateQueueAsync.Count != 0)
        {
            throw new Exception("Queue should be empty at this stage");
        }
        while (this.graphUpdateQueueRegular.Count > 0)
        {
            GUOSingle item = this.graphUpdateQueueRegular.Peek();
            GraphUpdateThreading threading = (item.order != GraphUpdateOrder.FloodFill) ? item.graph.CanUpdateAsync(item.obj) : GraphUpdateThreading.SeparateThread;
            bool flag = force;
            if ((!Application.isPlaying || (this.graphUpdateThread == null)) || !this.graphUpdateThread.IsAlive)
            {
                flag = true;
            }
            if (!flag && (threading == GraphUpdateThreading.SeparateAndUnityInit))
            {
                if (this.graphUpdateQueueAsync.Count > 0)
                {
                    this.processingGraphUpdatesAsync.Reset();
                    this.graphUpdateAsyncEvent.Set();
                    return false;
                }
                item.graph.UpdateAreaInit(item.obj);
                this.graphUpdateQueueRegular.Dequeue();
                this.graphUpdateQueueAsync.Enqueue(item);
                this.processingGraphUpdatesAsync.Reset();
                this.graphUpdateAsyncEvent.Set();
                return false;
            }
            if (!flag && (threading == GraphUpdateThreading.SeparateThread))
            {
                this.graphUpdateQueueRegular.Dequeue();
                this.graphUpdateQueueAsync.Enqueue(item);
            }
            else
            {
                if (this.graphUpdateQueueAsync.Count > 0)
                {
                    if (force)
                    {
                        throw new Exception("This should not happen");
                    }
                    this.processingGraphUpdatesAsync.Reset();
                    this.graphUpdateAsyncEvent.Set();
                    return false;
                }
                this.graphUpdateQueueRegular.Dequeue();
                if (item.order == GraphUpdateOrder.FloodFill)
                {
                    this.FloodFill();
                    continue;
                }
                if (threading == GraphUpdateThreading.SeparateAndUnityInit)
                {
                    try
                    {
                        item.graph.UpdateAreaInit(item.obj);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError("Error while initializing GraphUpdates\n" + exception);
                    }
                }
                try
                {
                    item.graph.UpdateArea(item.obj);
                    continue;
                }
                catch (Exception exception2)
                {
                    Debug.LogError("Error while updating graphs\n" + exception2);
                    continue;
                }
            }
        }
        if (this.graphUpdateQueueAsync.Count > 0)
        {
            this.processingGraphUpdatesAsync.Reset();
            this.graphUpdateAsyncEvent.Set();
            return false;
        }
        GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
        if (OnGraphsUpdated != null)
        {
            OnGraphsUpdated(this);
        }
        return true;
    }

    private void ProcessGraphUpdatesAsync(object _astar)
    {
        AstarPath objA = _astar as AstarPath;
        if (object.ReferenceEquals(objA, null))
        {
            Debug.LogError("ProcessGraphUpdatesAsync started with invalid parameter _astar (was no AstarPath object)");
        }
        else
        {
            while (!objA.pathQueue.IsTerminating)
            {
                this.graphUpdateAsyncEvent.WaitOne();
                if (objA.pathQueue.IsTerminating)
                {
                    this.graphUpdateQueueAsync.Clear();
                    this.processingGraphUpdatesAsync.Set();
                    return;
                }
                while (this.graphUpdateQueueAsync.Count > 0)
                {
                    GUOSingle num = this.graphUpdateQueueAsync.Dequeue();
                    try
                    {
                        if (num.order != GraphUpdateOrder.GraphUpdate)
                        {
                            if (num.order != GraphUpdateOrder.FloodFill)
                            {
                                throw new NotSupportedException(string.Empty + num.order);
                            }
                            objA.FloodFill();
                        }
                        else
                        {
                            num.graph.UpdateArea(num.obj);
                        }
                        continue;
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError("Exception while updating graphs:\n" + exception);
                        continue;
                    }
                }
                this.processingGraphUpdatesAsync.Set();
            }
        }
    }

    private int ProcessWorkItems(bool force)
    {
        if (!this.pathQueue.AllReceiversBlocked)
        {
            return 0;
        }
        if (this.processingWorkItems)
        {
            throw new Exception("Processing work items recursively. Please do not wait for other work items to be completed inside work items. If you think this is not caused by any of your scripts, this might be a bug.");
        }
        this.processingWorkItems = true;
        while (this.workItems.Count > 0)
        {
            bool flag;
            AstarWorkItem item = this.workItems.Peek();
            if (item.init != null)
            {
                item.init();
                item.init = null;
            }
            try
            {
                flag = (item.update != null) ? item.update(force) : true;
            }
            catch
            {
                this.workItems.Dequeue();
                this.processingWorkItems = false;
                throw;
            }
            if (!flag)
            {
                if (force)
                {
                    Debug.LogError("Misbehaving WorkItem. 'force'=true but the work item did not complete.\nIf force=true is passed to a WorkItem it should always return true.");
                }
                this.processingWorkItems = false;
                return 1;
            }
            this.workItems.Dequeue();
        }
        this.EnsureValidFloodFill();
        this.processingWorkItems = false;
        return 2;
    }

    public void QueueGraphUpdates()
    {
        if (!this.isRegisteredForUpdate)
        {
            this.isRegisteredForUpdate = true;
            AstarWorkItem itm = new AstarWorkItem {
                init = new OnVoidDelegate(this.QueueGraphUpdatesInternal),
                update = new Func<bool, bool>(this.ProcessGraphUpdates)
            };
            this.AddWorkItem(itm);
        }
    }

    private void QueueGraphUpdatesInternal()
    {
        this.isRegisteredForUpdate = false;
        bool flag = false;
        while (this.graphUpdateQueue.Count > 0)
        {
            GraphUpdateObject obj2 = this.graphUpdateQueue.Dequeue();
            if (obj2.requiresFloodFill)
            {
                flag = true;
            }
            IEnumerator enumerator = this.astarData.GetUpdateableGraphs().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IUpdatableGraph current = (IUpdatableGraph) enumerator.Current;
                    NavGraph graph = current as NavGraph;
                    if ((obj2.nnConstraint == null) || obj2.nnConstraint.SuitableGraph(active.astarData.GetGraphIndex(graph), graph))
                    {
                        GUOSingle item = new GUOSingle {
                            order = GraphUpdateOrder.GraphUpdate,
                            obj = obj2,
                            graph = current
                        };
                        this.graphUpdateQueueRegular.Enqueue(item);
                    }
                }
                continue;
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }
        if (flag)
        {
            GUOSingle num2 = new GUOSingle {
                order = GraphUpdateOrder.FloodFill
            };
            this.graphUpdateQueueRegular.Enqueue(num2);
        }
        this.debugPath = null;
        GraphModifier.TriggerEvent(GraphModifier.EventType.PreUpdate);
    }

    public void QueueWorkItemFloodFill()
    {
        if (!this.pathQueue.AllReceiversBlocked)
        {
            throw new Exception("You are calling QueueWorkItemFloodFill from outside a WorkItem. This might cause unexpected behaviour.");
        }
        this.queuedWorkItemFloodFill = true;
    }

    public static void RegisterSafeUpdate(OnVoidDelegate callback)
    {
        if ((callback != null) && Application.isPlaying)
        {
            if (active.pathQueue.AllReceiversBlocked)
            {
                active.pathQueue.Lock();
                try
                {
                    if (active.pathQueue.AllReceiversBlocked)
                    {
                        callback();
                        return;
                    }
                }
                finally
                {
                    active.pathQueue.Unlock();
                }
            }
            OnThreadSafeCallback = (OnVoidDelegate) Delegate.Combine(OnThreadSafeCallback, callback);
            active.pathQueue.Block();
        }
    }

    [Obsolete("The threadSafe parameter has been deprecated")]
    public static void RegisterSafeUpdate(OnVoidDelegate callback, bool threadSafe)
    {
        RegisterSafeUpdate(callback);
    }

    [ContextMenu("Reset Profiler")]
    public void ResetProfiler()
    {
    }

    public void ReturnPaths(bool timeSlice)
    {
        Path path = pathReturnStack.PopAll();
        if (this.pathReturnPop == null)
        {
            this.pathReturnPop = path;
        }
        else
        {
            Path pathReturnPop = this.pathReturnPop;
            while (pathReturnPop.next != null)
            {
                pathReturnPop = pathReturnPop.next;
            }
            pathReturnPop.next = path;
        }
        long num = !timeSlice ? 0L : (DateTime.UtcNow.Ticks + 0x2710L);
        int num2 = 0;
        while (this.pathReturnPop != null)
        {
            Path path3 = this.pathReturnPop;
            this.pathReturnPop = this.pathReturnPop.next;
            path3.next = null;
            path3.ReturnPath();
            path3.AdvanceState(PathState.Returned);
            path3.ReleaseSilent(this);
            num2++;
            if ((num2 > 5) && timeSlice)
            {
                num2 = 0;
                if (DateTime.UtcNow.Ticks >= num)
                {
                    return;
                }
            }
        }
    }

    public void Scan()
    {
        this.ScanLoop(null);
    }

    public void ScanLoop(OnScanStatus statusCallback)
    {
        <ScanLoop>c__AnonStorey1D storeyd = new <ScanLoop>c__AnonStorey1D {
            statusCallback = statusCallback
        };
        if (this.graphs != null)
        {
            this.isScanning = true;
            this.euclideanEmbedding.dirty = false;
            this.VerifyIntegrity();
            this.BlockUntilPathQueueBlocked();
            if (!Application.isPlaying)
            {
                GraphModifier.FindAllModifiers();
                RelevantGraphSurface.FindAllGraphSurfaces();
            }
            RelevantGraphSurface.UpdateAllPositions();
            this.astarData.UpdateShortcuts();
            if (storeyd.statusCallback != null)
            {
                storeyd.statusCallback(new Progress(0.05f, "Pre processing graphs"));
            }
            if (OnPreScan != null)
            {
                OnPreScan(this);
            }
            GraphModifier.TriggerEvent(GraphModifier.EventType.PreScan);
            DateTime utcNow = DateTime.UtcNow;
            for (int i = 0; i < this.graphs.Length; i++)
            {
                if (this.graphs[i] != null)
                {
                    if (<>f__am$cache4F == null)
                    {
                        <>f__am$cache4F = delegate (GraphNode node) {
                            node.Destroy();
                            return true;
                        };
                    }
                    this.graphs[i].GetNodes(<>f__am$cache4F);
                }
            }
            <ScanLoop>c__AnonStorey1E storeye = new <ScanLoop>c__AnonStorey1E {
                i = 0
            };
            while (storeye.i < this.graphs.Length)
            {
                <ScanLoop>c__AnonStorey1C storeyc = new <ScanLoop>c__AnonStorey1C {
                    <>f__ref$29 = storeyd,
                    <>f__ref$30 = storeye
                };
                NavGraph graph = this.graphs[storeye.i];
                if (graph == null)
                {
                    if (storeyd.statusCallback != null)
                    {
                        object[] objArray1 = new object[] { "Skipping graph ", storeye.i + 1, " of ", this.graphs.Length, " because it is null" };
                        storeyd.statusCallback(new Progress(AstarMath.MapTo(0.05f, 0.7f, (storeye.i + 0.5f) / ((float) (this.graphs.Length + 1))), string.Concat(objArray1)));
                    }
                }
                else
                {
                    if (OnGraphPreScan != null)
                    {
                        if (storeyd.statusCallback != null)
                        {
                            object[] objArray2 = new object[] { "Scanning graph ", storeye.i + 1, " of ", this.graphs.Length, " - Pre processing" };
                            storeyd.statusCallback(new Progress(AstarMath.MapToRange(0.1f, 0.7f, ((float) storeye.i) / ((float) this.graphs.Length)), string.Concat(objArray2)));
                        }
                        OnGraphPreScan(graph);
                    }
                    storeyc.minp = AstarMath.MapToRange(0.1f, 0.7f, ((float) storeye.i) / ((float) this.graphs.Length));
                    storeyc.maxp = AstarMath.MapToRange(0.1f, 0.7f, (storeye.i + 0.95f) / ((float) this.graphs.Length));
                    if (storeyd.statusCallback != null)
                    {
                        object[] objArray3 = new object[] { "Scanning graph ", storeye.i + 1, " of ", this.graphs.Length };
                        storeyd.statusCallback(new Progress(storeyc.minp, string.Concat(objArray3)));
                    }
                    OnScanStatus status = null;
                    if (storeyd.statusCallback != null)
                    {
                        status = new OnScanStatus(storeyc.<>m__D);
                    }
                    graph.ScanInternal(status);
                    graph.GetNodes(new GraphNodeDelegateCancelable(storeyc.<>m__E));
                    if (OnGraphPostScan != null)
                    {
                        if (storeyd.statusCallback != null)
                        {
                            object[] objArray4 = new object[] { "Scanning graph ", storeye.i + 1, " of ", this.graphs.Length, " - Post processing" };
                            storeyd.statusCallback(new Progress(AstarMath.MapToRange(0.1f, 0.7f, (storeye.i + 0.95f) / ((float) this.graphs.Length)), string.Concat(objArray4)));
                        }
                        OnGraphPostScan(graph);
                    }
                }
                storeye.i++;
            }
            if (storeyd.statusCallback != null)
            {
                storeyd.statusCallback(new Progress(0.8f, "Post processing graphs"));
            }
            if (OnPostScan != null)
            {
                OnPostScan(this);
            }
            GraphModifier.TriggerEvent(GraphModifier.EventType.PostScan);
            this.ApplyLinks();
            try
            {
                this.FlushWorkItems(false, true);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            this.isScanning = false;
            if (storeyd.statusCallback != null)
            {
                storeyd.statusCallback(new Progress(0.9f, "Computing areas"));
            }
            this.FloodFill();
            this.VerifyIntegrity();
            if (storeyd.statusCallback != null)
            {
                storeyd.statusCallback(new Progress(0.95f, "Late post processing"));
            }
            if (OnLatePostScan != null)
            {
                OnLatePostScan(this);
            }
            GraphModifier.TriggerEvent(GraphModifier.EventType.LatePostScan);
            this.euclideanEmbedding.dirty = true;
            this.euclideanEmbedding.RecalculatePivots();
            this.PerformBlockingActions(true, true);
            TimeSpan span = (TimeSpan) (DateTime.UtcNow - utcNow);
            this.lastScanTime = (float) span.TotalSeconds;
            GC.Collect();
            AstarLog("Scanning - Process took " + ((this.lastScanTime * 1000f)).ToString("0") + " ms to complete");
        }
    }

    public void SetUpReferences()
    {
        active = this;
        if (this.astarData == null)
        {
            this.astarData = new AstarData();
        }
        if (this.astarData.userConnections == null)
        {
            this.astarData.userConnections = new UserConnection[0];
        }
        if (this.colorSettings == null)
        {
            this.colorSettings = new AstarColor();
        }
        this.colorSettings.OnEnable();
    }

    public static bool StartPath(Path p, bool pushToFront = false)
    {
        if (object.ReferenceEquals(active, null))
        {
            Debug.LogError("There is no AstarPath object in the scene");
            return false;
        }
        if (p.GetState() != PathState.Created)
        {
            Debug.LogError(string.Concat(new object[] { "The path has an invalid state. Expected ", PathState.Created, " found ", p.GetState(), "\nMake sure you are not requesting the same path twice" }));
            return false;
        }
        if (active.pathQueue.IsTerminating)
        {
            p.Error();
            p.LogError("No new paths are accepted");
            return false;
        }
        if ((active.graphs == null) || (active.graphs.Length == 0))
        {
            Debug.LogError("There are no graphs in the scene");
            p.Error();
            p.LogError("There are no graphs in the scene");
            Debug.LogError(p.errorLog);
            return false;
        }
        p.Claim(active);
        p.AdvanceState(PathState.PathQueue);
        if (pushToFront)
        {
            active.pathQueue.PushFront(p);
        }
        else
        {
            active.pathQueue.Push(p);
        }
        return true;
    }

    private void Update()
    {
        this.PerformBlockingActions(false, true);
        if (threadEnumerator != null)
        {
            try
            {
                threadEnumerator.MoveNext();
            }
            catch (Exception exception)
            {
                threadEnumerator = null;
                if (!(exception is ThreadControlQueue.QueueTerminationException))
                {
                    Debug.LogException(exception);
                    Debug.LogError("Unhandled exception during pathfinding. Terminating.");
                    this.pathQueue.TerminateReceivers();
                    try
                    {
                        this.pathQueue.PopNoBlock(false);
                    }
                    catch
                    {
                    }
                }
            }
        }
        this.ReturnPaths(true);
    }

    public void UpdateGraphs(GraphUpdateObject ob)
    {
        if (this.graphUpdateQueue == null)
        {
            this.graphUpdateQueue = new Queue<GraphUpdateObject>();
        }
        this.graphUpdateQueue.Enqueue(ob);
        if (this.limitGraphUpdates && ((Time.time - this.lastGraphUpdate) < this.maxGraphUpdateFreq))
        {
            if (!this.graphUpdateRoutineRunning)
            {
                base.StartCoroutine(this.DelayedGraphUpdate());
            }
        }
        else
        {
            this.QueueGraphUpdates();
        }
    }

    public void UpdateGraphs(Bounds bounds)
    {
        this.UpdateGraphs(new GraphUpdateObject(bounds));
    }

    public void UpdateGraphs(GraphUpdateObject ob, float t)
    {
        base.StartCoroutine(this.UpdateGraphsInteral(ob, t));
    }

    public void UpdateGraphs(Bounds bounds, float t)
    {
        this.UpdateGraphs(new GraphUpdateObject(bounds), t);
    }

    [DebuggerHidden]
    private IEnumerator UpdateGraphsInteral(GraphUpdateObject ob, float t)
    {
        return new <UpdateGraphsInteral>c__Iterator5 { t = t, ob = ob, <$>t = t, <$>ob = ob, <>f__this = this };
    }

    public void VerifyIntegrity()
    {
        if (active != this)
        {
            throw new Exception("Singleton pattern broken. Make sure you only have one AstarPath object in the scene");
        }
        if (this.astarData == null)
        {
            throw new NullReferenceException("AstarData is null... Astar not set up correctly?");
        }
        if (this.astarData.graphs == null)
        {
            this.astarData.graphs = new NavGraph[0];
        }
        if ((this.pathQueue == null) && !Application.isPlaying)
        {
            this.pathQueue = new ThreadControlQueue(0);
        }
        if ((threadInfos == null) && !Application.isPlaying)
        {
            threadInfos = new PathThreadInfo[0];
        }
        if (IsUsingMultithreading)
        {
        }
    }

    public static void WaitForPath(Path p)
    {
        if (active == null)
        {
            Debug.LogError("Pathfinding is not correctly initialized in this scene (yet?). AstarPath.active is null.\nDo not call this function in Awake");
        }
        else if (p == null)
        {
            Debug.LogError("Path must not be null");
        }
        else if (!active.pathQueue.IsTerminating)
        {
            if (p.GetState() == PathState.Created)
            {
                Debug.LogError("The specified path has not been started yet.");
            }
            else
            {
                waitForPathDepth++;
                if (waitForPathDepth == 5)
                {
                    Debug.LogError("You are calling the WaitForPath function recursively (maybe from a path callback). Please don't do this.");
                }
                if (p.GetState() < PathState.ReturnQueue)
                {
                    if (IsUsingMultithreading)
                    {
                        while (p.GetState() < PathState.ReturnQueue)
                        {
                            if (active.pathQueue.IsTerminating)
                            {
                                waitForPathDepth--;
                                Debug.LogError("Pathfinding Threads seems to have crashed.");
                                return;
                            }
                            Thread.Sleep(1);
                            active.PerformBlockingActions(false, true);
                        }
                    }
                    else
                    {
                        while (p.GetState() < PathState.ReturnQueue)
                        {
                            if (active.pathQueue.IsEmpty && (p.GetState() != PathState.Processing))
                            {
                                waitForPathDepth--;
                                Debug.LogError("Critical error. Path Queue is empty but the path state is '" + p.GetState() + "'");
                                return;
                            }
                            threadEnumerator.MoveNext();
                            active.PerformBlockingActions(false, true);
                        }
                    }
                }
                active.ReturnPaths(false);
                waitForPathDepth--;
            }
        }
    }

    public PathHandler debugPathData
    {
        get
        {
            if (this.debugPath == null)
            {
                return null;
            }
            return this.debugPath.pathHandler;
        }
    }

    public NavGraph[] graphs
    {
        get
        {
            if (this.astarData == null)
            {
                this.astarData = new AstarData();
            }
            return this.astarData.graphs;
        }
        set
        {
            if (this.astarData == null)
            {
                this.astarData = new AstarData();
            }
            this.astarData.graphs = value;
        }
    }

    [Obsolete]
    public System.Type[] graphTypes
    {
        get
        {
            return this.astarData.graphTypes;
        }
    }

    public bool IsAnyGraphUpdatesQueued
    {
        get
        {
            return ((this.graphUpdateQueue != null) && (this.graphUpdateQueue.Count > 0));
        }
    }

    public static bool IsUsingMultithreading
    {
        get
        {
            if ((threads != null) && (threads.Length > 0))
            {
                return true;
            }
            if ((((threads == null) || (threads.Length != 0)) || (threadEnumerator == null)) && Application.isPlaying)
            {
                object[] objArray1 = new object[] { "Not 'using threading' and not 'not using threading'... Are you sure pathfinding is set up correctly?\nIf scripts are reloaded in unity editor during play this could happen.\n", (threads == null) ? "NULL" : (string.Empty + threads.Length), " ", threadEnumerator != null };
                throw new Exception(string.Concat(objArray1));
            }
            return false;
        }
    }

    public float maxNearestNodeDistanceSqr
    {
        get
        {
            return (this.maxNearestNodeDistance * this.maxNearestNodeDistance);
        }
    }

    public static int NumParallelThreads
    {
        get
        {
            return ((threadInfos == null) ? 0 : threadInfos.Length);
        }
    }

    public static System.Version Version
    {
        get
        {
            return new System.Version(3, 6, 0, 0);
        }
    }

    [CompilerGenerated]
    private sealed class <CalculatePaths>c__Iterator6 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal object _threadInfo;
        internal object <$>_threadInfo;
        internal AstarPath <astar>__4;
        internal bool <blockedBefore>__8;
        internal Exception <e>__1;
        internal long <maxTicks>__5;
        internal int <numPaths>__2;
        internal Path <p>__7;
        internal PathHandler <runData>__3;
        internal long <startTicks>__9;
        internal long <targetTick>__6;
        internal PathThreadInfo <threadInfo>__0;
        internal long <totalTicks>__10;

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
                    try
                    {
                        this.<threadInfo>__0 = (PathThreadInfo) this._threadInfo;
                    }
                    catch (Exception exception)
                    {
                        this.<e>__1 = exception;
                        Debug.LogError("Arguments to pathfinding threads must be of type ThreadStartInfo\n" + this.<e>__1);
                        throw new ArgumentException("Argument must be of type ThreadStartInfo", this.<e>__1);
                    }
                    this.<numPaths>__2 = 0;
                    this.<runData>__3 = this.<threadInfo>__0.runData;
                    this.<astar>__4 = this.<threadInfo>__0.astar;
                    if (this.<runData>__3.nodes == null)
                    {
                        throw new NullReferenceException("NodeRuns must be assigned to the threadInfo.runData.nodes field before threads are started\nthreadInfo is an argument to the thread functions");
                    }
                    this.<maxTicks>__5 = (long) (AstarPath.active.maxFrameTime * 10000f);
                    this.<targetTick>__6 = DateTime.UtcNow.Ticks + this.<maxTicks>__5;
                    break;

                case 1:
                    goto Label_0156;

                case 2:
                    goto Label_0281;

                case 3:
                    this.<targetTick>__6 = DateTime.UtcNow.Ticks + this.<maxTicks>__5;
                    this.<numPaths>__2 = 0;
                    break;

                default:
                    goto Label_03EB;
            }
        Label_00E7:
            this.<p>__7 = null;
            this.<blockedBefore>__8 = false;
        Label_0156:
            while (this.<p>__7 == null)
            {
                try
                {
                    this.<p>__7 = this.<astar>__4.pathQueue.PopNoBlock(this.<blockedBefore>__8);
                    if (this.<p>__7 == null)
                    {
                        this.<blockedBefore>__8 = true;
                    }
                }
                catch (ThreadControlQueue.QueueTerminationException)
                {
                    goto Label_03EB;
                }
                if (this.<p>__7 == null)
                {
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_03ED;
                }
            }
            this.<maxTicks>__5 = (long) (AstarPath.active.maxFrameTime * 10000f);
            this.<p>__7.PrepareBase(this.<runData>__3);
            this.<p>__7.AdvanceState(PathState.Processing);
            if (AstarPath.OnPathPreSearch != null)
            {
                AstarPath.OnPathPreSearch(this.<p>__7);
            }
            this.<numPaths>__2++;
            this.<startTicks>__9 = DateTime.UtcNow.Ticks;
            this.<totalTicks>__10 = 0L;
            this.<p>__7.Prepare();
            if (!this.<p>__7.IsDone())
            {
                AstarPath.active.debugPath = this.<p>__7;
                this.<p>__7.Initialize();
                while (!this.<p>__7.IsDone())
                {
                    this.<p>__7.CalculateStep(this.<targetTick>__6);
                    this.<p>__7.searchIterations++;
                    if (this.<p>__7.IsDone())
                    {
                        break;
                    }
                    this.<totalTicks>__10 += DateTime.UtcNow.Ticks - this.<startTicks>__9;
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_03ED;
                Label_0281:
                    this.<startTicks>__9 = DateTime.UtcNow.Ticks;
                    if (this.<astar>__4.pathQueue.IsTerminating)
                    {
                        this.<p>__7.Error();
                    }
                    this.<targetTick>__6 = DateTime.UtcNow.Ticks + this.<maxTicks>__5;
                }
                this.<totalTicks>__10 += DateTime.UtcNow.Ticks - this.<startTicks>__9;
                this.<p>__7.duration = this.<totalTicks>__10 * 0.0001f;
            }
            this.<p>__7.Cleanup();
            AstarPath.active.LogPathResults(this.<p>__7);
            if (this.<p>__7.immediateCallback != null)
            {
                this.<p>__7.immediateCallback(this.<p>__7);
            }
            if (AstarPath.OnPathPostSearch != null)
            {
                AstarPath.OnPathPostSearch(this.<p>__7);
            }
            AstarPath.pathReturnStack.Push(this.<p>__7);
            this.<p>__7.AdvanceState(PathState.ReturnQueue);
            if (DateTime.UtcNow.Ticks <= this.<targetTick>__6)
            {
                goto Label_00E7;
            }
            this.$current = null;
            this.$PC = 3;
            goto Label_03ED;
            this.$PC = -1;
        Label_03EB:
            return false;
        Label_03ED:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
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
    private sealed class <DelayedGraphUpdate>c__Iterator4 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal AstarPath <>f__this;

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
                    this.<>f__this.graphUpdateRoutineRunning = true;
                    this.$current = new WaitForSeconds(this.<>f__this.maxGraphUpdateFreq - (Time.time - this.<>f__this.lastGraphUpdate));
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.QueueGraphUpdates();
                    this.<>f__this.graphUpdateRoutineRunning = false;
                    this.$PC = -1;
                    break;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
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
    private sealed class <FloodFill>c__AnonStorey1B
    {
        internal AstarPath <>f__this;
        internal uint area;
        internal List<GraphNode> smallAreaList;
        internal int smallAreasDetected;
        internal Stack<GraphNode> stack;
        internal bool warnAboutAreas;

        internal bool <>m__B(GraphNode node)
        {
            if (node.Walkable && (node.Area == 0))
            {
                this.area++;
                uint area = this.area;
                if (this.area > 0x1ffff)
                {
                    if (this.smallAreaList.Count > 0)
                    {
                        GraphNode t = this.smallAreaList[this.smallAreaList.Count - 1];
                        area = t.Area;
                        this.smallAreaList.RemoveAt(this.smallAreaList.Count - 1);
                        this.stack.Clear();
                        this.stack.Push(t);
                        t.Area = 0x1ffff;
                        while (this.stack.Count > 0)
                        {
                            this.stack.Pop().FloodFill(this.stack, 0x1ffff);
                        }
                        this.smallAreasDetected++;
                    }
                    else
                    {
                        this.area--;
                        area = this.area;
                        this.warnAboutAreas = true;
                    }
                }
                this.stack.Clear();
                this.stack.Push(node);
                int num2 = 1;
                node.Area = area;
                while (this.stack.Count > 0)
                {
                    num2++;
                    this.stack.Pop().FloodFill(this.stack, area);
                }
                if (num2 < this.<>f__this.minAreaSize)
                {
                    this.smallAreaList.Add(node);
                }
            }
            return true;
        }
    }

    [CompilerGenerated]
    private sealed class <GetNearest>c__AnonStorey1F
    {
        internal Vector3 lineDirection;
        internal Vector3 lineOrigin;
        internal float minDist;
        internal GraphNode nearestNode;

        internal bool <>m__F(GraphNode node)
        {
            Vector3 position = (Vector3) node.position;
            Vector3 vector2 = this.lineOrigin + ((Vector3) (Vector3.Dot(position - this.lineOrigin, this.lineDirection) * this.lineDirection));
            float num = Mathf.Abs((float) (vector2.x - position.x));
            num *= num;
            if (num <= this.minDist)
            {
                num = Mathf.Abs((float) (vector2.z - position.z));
                num *= num;
                if (num > this.minDist)
                {
                    return true;
                }
                Vector3 vector3 = vector2 - position;
                float sqrMagnitude = vector3.sqrMagnitude;
                if (sqrMagnitude < this.minDist)
                {
                    this.minDist = sqrMagnitude;
                    this.nearestNode = node;
                }
            }
            return true;
        }
    }

    [CompilerGenerated]
    private sealed class <ScanLoop>c__AnonStorey1C
    {
        internal AstarPath.<ScanLoop>c__AnonStorey1D <>f__ref$29;
        internal AstarPath.<ScanLoop>c__AnonStorey1E <>f__ref$30;
        internal float maxp;
        internal float minp;

        internal void <>m__D(Progress p)
        {
            p.progress = AstarMath.MapToRange(this.minp, this.maxp, p.progress);
            this.<>f__ref$29.statusCallback(p);
        }

        internal bool <>m__E(GraphNode node)
        {
            node.GraphIndex = (uint) this.<>f__ref$30.i;
            return true;
        }
    }

    [CompilerGenerated]
    private sealed class <ScanLoop>c__AnonStorey1D
    {
        internal OnScanStatus statusCallback;
    }

    [CompilerGenerated]
    private sealed class <ScanLoop>c__AnonStorey1E
    {
        internal int i;
    }

    [CompilerGenerated]
    private sealed class <UpdateGraphsInteral>c__Iterator5 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GraphUpdateObject <$>ob;
        internal float <$>t;
        internal AstarPath <>f__this;
        internal GraphUpdateObject ob;
        internal float t;

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
                    this.$current = new WaitForSeconds(this.t);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.UpdateGraphs(this.ob);
                    this.$PC = -1;
                    break;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
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

    public enum AstarDistribution
    {
        WebsiteDownload,
        AssetStore
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AstarWorkItem
    {
        public OnVoidDelegate init;
        public Func<bool, bool> update;
        public AstarWorkItem(Func<bool, bool> update)
        {
            this.init = null;
            this.update = update;
        }

        public AstarWorkItem(OnVoidDelegate init, Func<bool, bool> update)
        {
            this.init = init;
            this.update = update;
        }
    }

    private enum GraphUpdateOrder
    {
        GraphUpdate,
        FloodFill
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct GUOSingle
    {
        public AstarPath.GraphUpdateOrder order;
        public IUpdatableGraph graph;
        public GraphUpdateObject obj;
    }
}

