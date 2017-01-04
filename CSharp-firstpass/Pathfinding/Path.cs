namespace Pathfinding
{
    using Pathfinding.Util;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;

    public abstract class Path
    {
        private string _errorLog = string.Empty;
        public int astarDataIndex = -1;
        public OnPathDelegate callback;
        public DateTime callTime;
        private List<object> claimed = new List<object>();
        protected PathNode currentR;
        public float duration;
        public int enabledTags = -1;
        protected bool hasBeenReset;
        public int height;
        public Heuristic heuristic;
        public float heuristicScale = 1f;
        protected VInt3 hTarget;
        protected GraphNode hTargetNode;
        public OnPathDelegate immediateCallback;
        protected int[] internalTagPenalties;
        protected int[] manualTagPenalties;
        protected float maxFrameTime;
        public Path next;
        public NNConstraint nnConstraint = PathNNConstraint.Default;
        public List<GraphNode> path;
        private PathCompleteState pathCompleteState;
        public PathHandler pathHandler;
        public ushort pathID;
        public int radius;
        public bool recycled;
        private bool releasedNotSilent;
        public int searchedNodes;
        public int searchIterations;
        public int speed;
        private PathState state;
        private object stateLock = new object();
        public int turnRadius;
        public List<VInt3> vectorPath;
        public int walkabilityMask = -1;
        private static readonly int[] ZeroTagPenalties = new int[0x20];

        protected Path()
        {
        }

        public void AdvanceState(PathState s)
        {
            this.state = (PathState) Math.Max((int) this.state, (int) s);
        }

        public uint CalculateHScore(GraphNode node)
        {
            uint num;
            uint num2;
            switch (this.heuristic)
            {
                case Heuristic.Manhattan:
                {
                    VInt3 position = node.position;
                    int introduced8 = Math.Abs((int) (this.hTarget.x - position.x));
                    num = (uint) (((introduced8 + Math.Abs((int) (this.hTarget.y - position.y))) + Math.Abs((int) (this.hTarget.z - position.z))) * this.heuristicScale);
                    num2 = (this.hTargetNode == null) ? 0 : AstarPath.active.euclideanEmbedding.GetHeuristic(node.NodeIndex, this.hTargetNode.NodeIndex);
                    return Math.Max(num, num2);
                }
                case Heuristic.DiagonalManhattan:
                {
                    VInt3 num4 = this.GetHTarget() - node.position;
                    num4.x = Math.Abs(num4.x);
                    num4.y = Math.Abs(num4.y);
                    num4.z = Math.Abs(num4.z);
                    int num5 = Math.Min(num4.x, num4.z);
                    int num6 = Math.Max(num4.x, num4.z);
                    num = (uint) (((((14 * num5) / 10) + (num6 - num5)) + num4.y) * this.heuristicScale);
                    num2 = (this.hTargetNode == null) ? 0 : AstarPath.active.euclideanEmbedding.GetHeuristic(node.NodeIndex, this.hTargetNode.NodeIndex);
                    return Math.Max(num, num2);
                }
                case Heuristic.Euclidean:
                {
                    VInt3 num7 = this.GetHTarget() - node.position;
                    num = (uint) (num7.costMagnitude * this.heuristicScale);
                    num2 = (this.hTargetNode == null) ? 0 : AstarPath.active.euclideanEmbedding.GetHeuristic(node.NodeIndex, this.hTargetNode.NodeIndex);
                    return Math.Max(num, num2);
                }
            }
            return 0;
        }

        public abstract void CalculateStep(long targetTick);
        public bool CanTraverse(GraphNode node)
        {
            return (node.Walkable && (((this.enabledTags >> node.Tag) & 1) != 0));
        }

        public void Claim(object o)
        {
            if (object.ReferenceEquals(o, null))
            {
                throw new ArgumentNullException("o");
            }
            for (int i = 0; i < this.claimed.Count; i++)
            {
                if (object.ReferenceEquals(this.claimed[i], o))
                {
                    throw new ArgumentException("You have already claimed the path with that object (" + o.ToString() + "). Are you claiming the path with the same object twice?");
                }
            }
            this.claimed.Add(o);
        }

        public virtual void Cleanup()
        {
        }

        public virtual string DebugString(PathLog logMode)
        {
            if ((logMode == PathLog.None) || (!this.error && (logMode == PathLog.OnlyErrors)))
            {
                return string.Empty;
            }
            StringBuilder debugStringBuilder = this.pathHandler.DebugStringBuilder;
            debugStringBuilder.Length = 0;
            debugStringBuilder.Append(!this.error ? "Path Completed : " : "Path Failed : ");
            debugStringBuilder.Append("Computation Time ");
            debugStringBuilder.Append(this.duration.ToString((logMode != PathLog.Heavy) ? "0.00 ms " : "0.000 ms "));
            debugStringBuilder.Append("Searched Nodes ");
            debugStringBuilder.Append(this.searchedNodes);
            if (!this.error)
            {
                debugStringBuilder.Append(" Path Length ");
                debugStringBuilder.Append((this.path != null) ? this.path.Count.ToString() : "Null");
                if (logMode == PathLog.Heavy)
                {
                    debugStringBuilder.Append("\nSearch Iterations " + this.searchIterations);
                }
            }
            if (this.error)
            {
                debugStringBuilder.Append("\nError: ");
                debugStringBuilder.Append(this.errorLog);
            }
            if ((logMode == PathLog.Heavy) && !AstarPath.IsUsingMultithreading)
            {
                debugStringBuilder.Append("\nCallback references ");
                if (this.callback != null)
                {
                    debugStringBuilder.Append(this.callback.Target.GetType().FullName).AppendLine();
                }
                else
                {
                    debugStringBuilder.AppendLine("NULL");
                }
            }
            debugStringBuilder.Append("\nPath Number ");
            debugStringBuilder.Append(this.pathID);
            return debugStringBuilder.ToString();
        }

        public void Error()
        {
            this.CompleteState = PathCompleteState.Error;
        }

        private void ErrorCheck()
        {
            if (!this.hasBeenReset)
            {
                throw new Exception("The path has never been reset. Use pooling API or call Reset() after creating the path with the default constructor.");
            }
            if (this.recycled)
            {
                throw new Exception("The path is currently in a path pool. Are you sending the path for calculation twice?");
            }
            if (this.pathHandler == null)
            {
                throw new Exception("Field pathHandler is not set. Please report this bug.");
            }
            if (this.GetState() > PathState.Processing)
            {
                throw new Exception("This path has already been processed. Do not request a path with the same path object twice.");
            }
        }

        public void ForceLogError(string msg)
        {
            this.Error();
            this._errorLog = this._errorLog + msg;
            Debug.LogError(msg);
        }

        public virtual uint GetConnectionSpecialCost(GraphNode a, GraphNode b, uint currentCost)
        {
            return currentCost;
        }

        public VInt3 GetHTarget()
        {
            return this.hTarget;
        }

        public PathState GetState()
        {
            return this.state;
        }

        public uint GetTagPenalty(int tag)
        {
            return (uint) this.internalTagPenalties[tag];
        }

        public float GetTotalLength()
        {
            if (this.vectorPath == null)
            {
                return float.PositiveInfinity;
            }
            float num = 0f;
            for (int i = 0; i < (this.vectorPath.Count - 1); i++)
            {
                num += Vector3.Distance((Vector3) this.vectorPath[i], (Vector3) this.vectorPath[i + 1]);
            }
            return num;
        }

        public uint GetTraversalCost(GraphNode node)
        {
            return (this.GetTagPenalty((int) node.Tag) + node.Penalty);
        }

        protected bool HasExceededTime(int searchedNodes, long targetTime)
        {
            return (DateTime.UtcNow.Ticks >= targetTime);
        }

        public abstract void Initialize();
        public bool IsDone()
        {
            return (this.CompleteState != PathCompleteState.NotCalculated);
        }

        public void Log(string msg)
        {
            if (AstarPath.isEditor || (AstarPath.active.logPathResults != PathLog.None))
            {
                this._errorLog = this._errorLog + msg;
            }
        }

        public void LogError(string msg)
        {
            if (AstarPath.isEditor || (AstarPath.active.logPathResults != PathLog.None))
            {
                this._errorLog = this._errorLog + msg;
            }
            if ((AstarPath.active.logPathResults != PathLog.None) && (AstarPath.active.logPathResults != PathLog.InGame))
            {
                Debug.LogWarning(msg);
            }
        }

        public virtual void OnEnterPool()
        {
            if (this.vectorPath != null)
            {
                ListPool<VInt3>.Release(this.vectorPath);
            }
            if (this.path != null)
            {
                ListPool<GraphNode>.Release(this.path);
            }
            this.vectorPath = null;
            this.path = null;
        }

        public abstract void Prepare();
        public void PrepareBase(PathHandler pathHandler)
        {
            if (pathHandler.PathID > this.pathID)
            {
                pathHandler.ClearPathIDs();
            }
            this.pathHandler = pathHandler;
            pathHandler.InitializeForPath(this);
            if ((this.internalTagPenalties == null) || (this.internalTagPenalties.Length != 0x20))
            {
                this.internalTagPenalties = ZeroTagPenalties;
            }
            try
            {
                this.ErrorCheck();
            }
            catch (Exception exception)
            {
                this.ForceLogError(string.Concat(new object[] { "Exception in path ", this.pathID, "\n", exception.ToString() }));
            }
        }

        protected abstract void Recycle();
        public void Release(object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }
            for (int i = 0; i < this.claimed.Count; i++)
            {
                if (object.ReferenceEquals(this.claimed[i], o))
                {
                    this.claimed.RemoveAt(i);
                    this.releasedNotSilent = true;
                    if (this.claimed.Count == 0)
                    {
                        this.Recycle();
                    }
                    return;
                }
            }
            if (this.claimed.Count == 0)
            {
                throw new ArgumentException("You are releasing a path which is not claimed at all (most likely it has been pooled already). Are you releasing the path with the same object (" + o.ToString() + ") twice?");
            }
            throw new ArgumentException("You are releasing a path which has not been claimed with this object (" + o.ToString() + "). Are you releasing the path with the same object twice?");
        }

        public void ReleaseSilent(object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }
            for (int i = 0; i < this.claimed.Count; i++)
            {
                if (object.ReferenceEquals(this.claimed[i], o))
                {
                    this.claimed.RemoveAt(i);
                    if (this.releasedNotSilent && (this.claimed.Count == 0))
                    {
                        this.Recycle();
                    }
                    return;
                }
            }
            if (this.claimed.Count == 0)
            {
                throw new ArgumentException("You are releasing a path which is not claimed at all (most likely it has been pooled already). Are you releasing the path with the same object (" + o.ToString() + ") twice?");
            }
            throw new ArgumentException("You are releasing a path which has not been claimed with this object (" + o.ToString() + "). Are you releasing the path with the same object twice?");
        }

        public virtual void Reset()
        {
            if (object.ReferenceEquals(AstarPath.active, null))
            {
                Debug.LogError("No AstarPath object found in the scene. Make sure there is one or do not create paths in Awake");
            }
            this.hasBeenReset = true;
            this.state = PathState.Created;
            this.releasedNotSilent = false;
            this.pathHandler = null;
            this.callback = null;
            this._errorLog = string.Empty;
            this.pathCompleteState = PathCompleteState.NotCalculated;
            this.path = ListPool<GraphNode>.Claim();
            this.vectorPath = ListPool<VInt3>.Claim();
            this.currentR = null;
            this.duration = 0f;
            this.searchIterations = 0;
            this.searchedNodes = 0;
            this.nnConstraint = PathNNConstraint.Default;
            this.next = null;
            this.radius = 0;
            this.walkabilityMask = -1;
            this.height = 0;
            this.turnRadius = 0;
            this.speed = 0;
            this.pathID = 0;
            this.enabledTags = -1;
            this.tagPenalties = null;
            this.callTime = DateTime.UtcNow;
            if (AstarPath.active != null)
            {
                this.pathID = AstarPath.active.GetNextPathID();
                this.heuristic = AstarPath.active.heuristic;
                this.heuristicScale = AstarPath.active.heuristicScale;
            }
            else
            {
                this.heuristic = Heuristic.Manhattan;
                this.heuristicScale = 1f;
            }
            this.hTarget = VInt3.zero;
            this.hTargetNode = null;
        }

        public virtual void ReturnPath()
        {
            if (this.callback != null)
            {
                this.callback(this);
            }
        }

        protected virtual void Trace(PathNode from)
        {
            int num = 0;
            PathNode parent = from;
            while (parent != null)
            {
                parent = parent.parent;
                num++;
                if (num > 0x400)
                {
                    Debug.LogWarning("Inifinity loop? >1024 node path. Remove this message if you really have that long paths (Path.cs, Trace function)");
                    break;
                }
            }
            if (this.path.Capacity < num)
            {
                this.path.Capacity = num;
            }
            if (this.vectorPath.Capacity < num)
            {
                this.vectorPath.Capacity = num;
            }
            parent = from;
            for (int i = 0; i < num; i++)
            {
                this.path.Add(parent.node);
                parent = parent.parent;
            }
            int num3 = num / 2;
            for (int j = 0; j < num3; j++)
            {
                GraphNode node2 = this.path[j];
                this.path[j] = this.path[(num - j) - 1];
                this.path[(num - j) - 1] = node2;
            }
            for (int k = 0; k < num; k++)
            {
                this.vectorPath.Add(this.path[k].position);
            }
        }

        [DebuggerHidden]
        public IEnumerator WaitForPath()
        {
            return new <WaitForPath>c__Iterator7 { <>f__this = this };
        }

        public PathCompleteState CompleteState
        {
            get
            {
                return this.pathCompleteState;
            }
            protected set
            {
                this.pathCompleteState = value;
            }
        }

        public bool error
        {
            get
            {
                return (this.CompleteState == PathCompleteState.Error);
            }
        }

        public string errorLog
        {
            get
            {
                return this._errorLog;
            }
        }

        public virtual bool FloodingPath
        {
            get
            {
                return false;
            }
        }

        public int[] tagPenalties
        {
            get
            {
                return this.manualTagPenalties;
            }
            set
            {
                if ((value == null) || (value.Length != 0x20))
                {
                    this.manualTagPenalties = null;
                    this.internalTagPenalties = ZeroTagPenalties;
                }
                else
                {
                    this.manualTagPenalties = value;
                    this.internalTagPenalties = value;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitForPath>c__Iterator7 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Path <>f__this;

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
                        if (this.<>f__this.GetState() == PathState.Created)
                        {
                            throw new InvalidOperationException("This path has not been started yet");
                        }
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_006C;
                }
                while (this.<>f__this.GetState() != PathState.Returned)
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.$PC = -1;
            Label_006C:
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
    }
}

