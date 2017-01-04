namespace Pathfinding
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public class ABPath : Path
    {
        public bool calculatePartial;
        public GraphNode endHint;
        public GraphNode endNode;
        protected int[] endNodeCosts;
        public VInt3 endPoint;
        protected bool hasEndPoint;
        public VInt3 originalEndPoint;
        public VInt3 originalStartPoint;
        protected PathNode partialBestTarget;
        public bool recalcStartEndCosts;
        public GraphNode startHint;
        public GraphNode startNode;
        public VInt3 startPoint;

        public ABPath()
        {
            this.recalcStartEndCosts = true;
            this.hasEndPoint = true;
        }

        [Obsolete("Use PathPool<T>.GetPath instead")]
        public ABPath(VInt3 start, VInt3 end, OnPathDelegate callbackDelegate)
        {
            this.recalcStartEndCosts = true;
            this.hasEndPoint = true;
            this.Reset();
            this.Setup(ref start, ref end, callbackDelegate);
        }

        public override void CalculateStep(long targetTick)
        {
            for (int i = 0; base.CompleteState == PathCompleteState.NotCalculated; i++)
            {
                base.searchedNodes++;
                if (base.currentR.node == this.endNode)
                {
                    base.CompleteState = PathCompleteState.Complete;
                    break;
                }
                if (base.currentR.H < this.partialBestTarget.H)
                {
                    this.partialBestTarget = base.currentR;
                }
                base.currentR.node.Open(this, base.currentR, base.pathHandler);
                if (base.pathHandler.HeapEmpty())
                {
                    base.Error();
                    base.LogError("No open points, whole area searched");
                    return;
                }
                base.currentR = base.pathHandler.PopNode();
                if (i > 500)
                {
                    if (DateTime.UtcNow.Ticks >= targetTick)
                    {
                        return;
                    }
                    i = 0;
                    if (base.searchedNodes > 0xf4240)
                    {
                        throw new Exception("Probable infinite loop. Over 1,000,000 nodes searched");
                    }
                }
            }
            if (base.CompleteState == PathCompleteState.Complete)
            {
                this.Trace(base.currentR);
            }
            else if (this.calculatePartial && (this.partialBestTarget != null))
            {
                base.CompleteState = PathCompleteState.Partial;
                this.Trace(this.partialBestTarget);
            }
        }

        public override void Cleanup()
        {
            if (this.startNode != null)
            {
                base.pathHandler.GetPathNode(this.startNode).flag2 = false;
            }
            if (this.endNode != null)
            {
                base.pathHandler.GetPathNode(this.endNode).flag2 = false;
            }
        }

        public static ABPath Construct(ref VInt3 start, ref VInt3 end, OnPathDelegate callback = null)
        {
            ABPath path = PathPool<ABPath>.GetPath();
            path.Setup(ref start, ref end, callback);
            return path;
        }

        public override string DebugString(PathLog logMode)
        {
            if ((logMode == PathLog.None) || (!base.error && (logMode == PathLog.OnlyErrors)))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(!base.error ? "Path Completed : " : "Path Failed : ");
            builder.Append("Computation Time ");
            builder.Append(this.duration.ToString((logMode != PathLog.Heavy) ? "0.00" : "0.000"));
            builder.Append(" ms Searched Nodes ");
            builder.Append(base.searchedNodes);
            if (!base.error)
            {
                builder.Append(" Path Length ");
                builder.Append((base.path != null) ? base.path.Count.ToString() : "Null");
                if (logMode == PathLog.Heavy)
                {
                    builder.Append("\nSearch Iterations " + base.searchIterations);
                    if (this.hasEndPoint && (this.endNode != null))
                    {
                        PathNode pathNode = base.pathHandler.GetPathNode(this.endNode);
                        builder.Append("\nEnd Node\n\tG: ");
                        builder.Append(pathNode.G);
                        builder.Append("\n\tH: ");
                        builder.Append(pathNode.H);
                        builder.Append("\n\tF: ");
                        builder.Append(pathNode.F);
                        builder.Append("\n\tPoint: ");
                        builder.Append(((Vector3) this.endPoint).ToString());
                        builder.Append("\n\tGraph: ");
                        builder.Append(this.endNode.GraphIndex);
                    }
                    builder.Append("\nStart Node");
                    builder.Append("\n\tPoint: ");
                    builder.Append(((Vector3) this.startPoint).ToString());
                    builder.Append("\n\tGraph: ");
                    if (this.startNode != null)
                    {
                        builder.Append(this.startNode.GraphIndex);
                    }
                    else
                    {
                        builder.Append("< null startNode >");
                    }
                }
            }
            if (base.error)
            {
                builder.Append("\nError: ");
                builder.Append(base.errorLog);
            }
            if ((logMode == PathLog.Heavy) && !AstarPath.IsUsingMultithreading)
            {
                builder.Append("\nCallback references ");
                if (base.callback != null)
                {
                    builder.Append(base.callback.Target.GetType().FullName).AppendLine();
                }
                else
                {
                    builder.AppendLine("NULL");
                }
            }
            builder.Append("\nPath Number ");
            builder.Append(base.pathID);
            return builder.ToString();
        }

        public override uint GetConnectionSpecialCost(GraphNode a, GraphNode b, uint currentCost)
        {
            if ((this.startNode != null) && (this.endNode != null))
            {
                if (a == this.startNode)
                {
                    VInt3 num = this.startPoint - ((b != this.endNode) ? b.position : base.hTarget);
                    VInt3 num2 = a.position - b.position;
                    return (uint) (num.costMagnitude * ((currentCost * 1.0) / ((double) num2.costMagnitude)));
                }
                if (b == this.startNode)
                {
                    VInt3 num3 = this.startPoint - ((a != this.endNode) ? a.position : base.hTarget);
                    VInt3 num4 = a.position - b.position;
                    return (uint) (num3.costMagnitude * ((currentCost * 1.0) / ((double) num4.costMagnitude)));
                }
                if (a == this.endNode)
                {
                    VInt3 num5 = base.hTarget - b.position;
                    VInt3 num6 = a.position - b.position;
                    return (uint) (num5.costMagnitude * ((currentCost * 1.0) / ((double) num6.costMagnitude)));
                }
                if (b != this.endNode)
                {
                    return currentCost;
                }
                VInt3 num7 = base.hTarget - a.position;
                VInt3 num8 = a.position - b.position;
                return (uint) (num7.costMagnitude * ((currentCost * 1.0) / ((double) num8.costMagnitude)));
            }
            if (a == this.startNode)
            {
                VInt3 num9 = this.startPoint - b.position;
                VInt3 num10 = a.position - b.position;
                return (uint) (num9.costMagnitude * ((currentCost * 1.0) / ((double) num10.costMagnitude)));
            }
            if (b == this.startNode)
            {
                VInt3 num11 = this.startPoint - a.position;
                VInt3 num12 = a.position - b.position;
                return (uint) (num11.costMagnitude * ((currentCost * 1.0) / ((double) num12.costMagnitude)));
            }
            return currentCost;
        }

        public override void Initialize()
        {
            if (this.startNode != null)
            {
                base.pathHandler.GetPathNode(this.startNode).flag2 = true;
            }
            if (this.endNode != null)
            {
                base.pathHandler.GetPathNode(this.endNode).flag2 = true;
            }
            if (this.hasEndPoint && (this.startNode == this.endNode))
            {
                PathNode pathNode = base.pathHandler.GetPathNode(this.endNode);
                pathNode.node = this.endNode;
                pathNode.parent = null;
                pathNode.H = 0;
                pathNode.G = 0;
                this.Trace(pathNode);
                base.CompleteState = PathCompleteState.Complete;
            }
            else
            {
                PathNode node2 = base.pathHandler.GetPathNode(this.startNode);
                node2.node = this.startNode;
                node2.pathID = base.pathHandler.PathID;
                node2.parent = null;
                node2.cost = 0;
                node2.G = (this.startNode == null) ? 0 : base.GetTraversalCost(this.startNode);
                node2.H = (this.startNode == null) ? 0 : base.CalculateHScore(this.startNode);
                DebugHelper.Assert(this.startNode != null, "startNode != null");
                if (this.startNode != null)
                {
                    this.startNode.Open(this, node2, base.pathHandler);
                }
                base.searchedNodes++;
                this.partialBestTarget = node2;
                if (base.pathHandler.HeapEmpty())
                {
                    if (!this.calculatePartial)
                    {
                        base.Error();
                        base.LogError("No open points, the start node didn't open any nodes");
                        return;
                    }
                    base.CompleteState = PathCompleteState.Partial;
                    this.Trace(this.partialBestTarget);
                }
                base.currentR = base.pathHandler.PopNode();
            }
        }

        public override void Prepare()
        {
            VInt3 num;
            AstarData targetAstarData = this.targetAstarData;
            this.startNode = targetAstarData.GetNearestByRasterizer(this.startPoint, out num);
            if (this.startNode != null)
            {
                this.startPoint = num;
            }
            if (this.hasEndPoint)
            {
                this.endNode = targetAstarData.GetNearestByRasterizer(this.endPoint, out num);
                if (this.endNode != null)
                {
                    this.endPoint = num;
                }
                base.hTarget = this.endPoint;
                base.hTargetNode = this.endNode;
            }
            if (((this.startNode == null) && this.hasEndPoint) && (this.endNode == null))
            {
                base.Error();
                base.LogError("Couldn't find close nodes to the start point or the end point");
            }
            else if (this.startNode == null)
            {
                base.Error();
                base.LogError("Couldn't find a close node to the start point");
            }
            else if ((this.endNode == null) && this.hasEndPoint)
            {
                base.Error();
                base.LogError("Couldn't find a close node to the end point");
            }
            else if (!this.startNode.Walkable)
            {
                base.Error();
                base.LogError("The node closest to the start point is not walkable");
            }
            else if (this.hasEndPoint && !this.endNode.Walkable)
            {
                base.Error();
                base.LogError("The node closest to the end point is not walkable");
            }
            else if (this.hasEndPoint && (this.startNode.Area != this.endNode.Area))
            {
                base.Error();
                base.LogError(string.Concat(new object[] { "There is no valid path to the target (start area: ", this.startNode.Area, ", target area: ", this.endNode.Area, ")" }));
            }
        }

        protected override void Recycle()
        {
            PathPool<ABPath>.Recycle(this);
        }

        public override void Reset()
        {
            base.Reset();
            this.startNode = null;
            this.endNode = null;
            this.startHint = null;
            this.endHint = null;
            this.originalStartPoint = VInt3.zero;
            this.originalEndPoint = VInt3.zero;
            this.startPoint = VInt3.zero;
            this.endPoint = VInt3.zero;
            this.calculatePartial = false;
            this.partialBestTarget = null;
            this.hasEndPoint = true;
            base.hTarget = new VInt3();
            this.endNodeCosts = null;
        }

        public void ResetCosts(Path p)
        {
        }

        protected void Setup(ref VInt3 start, ref VInt3 end, OnPathDelegate callbackDelegate)
        {
            base.callback = callbackDelegate;
            this.UpdateStartEnd(ref start, ref end);
        }

        protected void UpdateStartEnd(ref VInt3 start, ref VInt3 end)
        {
            this.originalStartPoint = start;
            this.originalEndPoint = end;
            this.startPoint = start;
            this.endPoint = end;
            base.hTarget = end;
        }

        public AstarData targetAstarData
        {
            get
            {
                if (((AstarPath.active.astarDataArray != null) && (base.astarDataIndex >= 0)) && (base.astarDataIndex < AstarPath.active.astarDataArray.Length))
                {
                    return AstarPath.active.astarDataArray[base.astarDataIndex];
                }
                return AstarPath.active.astarData;
            }
        }
    }
}

