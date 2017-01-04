namespace Pathfinding
{
    using Pathfinding.Util;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class ConstantPath : Path
    {
        public List<GraphNode> allNodes;
        public PathEndingCondition endingCondition;
        public Vector3 originalStartPoint;
        public GraphNode startNode;
        public Vector3 startPoint;

        public ConstantPath()
        {
        }

        [Obsolete("Please use the Construct method instead")]
        public ConstantPath(Vector3 start, OnPathDelegate callbackDelegate)
        {
            throw new Exception("This constructor is obsolete, please use the Construct method instead");
        }

        [Obsolete("Please use the Construct method instead")]
        public ConstantPath(Vector3 start, int maxGScore, OnPathDelegate callbackDelegate)
        {
            throw new Exception("This constructor is obsolete, please use the Construct method instead");
        }

        public override void CalculateStep(long targetTick)
        {
            for (int i = 0; base.CompleteState == PathCompleteState.NotCalculated; i++)
            {
                base.searchedNodes++;
                if (this.endingCondition.TargetFound(base.currentR))
                {
                    base.CompleteState = PathCompleteState.Complete;
                    break;
                }
                if (!base.currentR.flag1)
                {
                    this.allNodes.Add(base.currentR.node);
                    base.currentR.flag1 = true;
                }
                base.currentR.node.Open(this, base.currentR, base.pathHandler);
                if (base.pathHandler.HeapEmpty())
                {
                    base.CompleteState = PathCompleteState.Complete;
                    break;
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
        }

        public override void Cleanup()
        {
            int count = this.allNodes.Count;
            for (int i = 0; i < count; i++)
            {
                base.pathHandler.GetPathNode(this.allNodes[i]).flag1 = false;
            }
        }

        public static ConstantPath Construct(Vector3 start, int maxGScore, OnPathDelegate callback = null)
        {
            ConstantPath path = PathPool<ConstantPath>.GetPath();
            path.Setup(start, maxGScore, callback);
            return path;
        }

        public override void Initialize()
        {
            PathNode pathNode = base.pathHandler.GetPathNode(this.startNode);
            pathNode.node = this.startNode;
            pathNode.pathID = base.pathHandler.PathID;
            pathNode.parent = null;
            pathNode.cost = 0;
            pathNode.G = base.GetTraversalCost(this.startNode);
            pathNode.H = base.CalculateHScore(this.startNode);
            this.startNode.Open(this, pathNode, base.pathHandler);
            base.searchedNodes++;
            pathNode.flag1 = true;
            this.allNodes.Add(this.startNode);
            if (base.pathHandler.HeapEmpty())
            {
                base.CompleteState = PathCompleteState.Complete;
            }
            else
            {
                base.currentR = base.pathHandler.PopNode();
            }
        }

        public override void OnEnterPool()
        {
            base.OnEnterPool();
            if (this.allNodes != null)
            {
                ListPool<GraphNode>.Release(this.allNodes);
            }
        }

        public override void Prepare()
        {
            base.nnConstraint.tags = base.enabledTags;
            NNInfo nearest = AstarPath.active.GetNearest(this.startPoint, base.nnConstraint);
            this.startNode = nearest.node;
            if (this.startNode == null)
            {
                base.Error();
                base.LogError("Could not find close node to the start point");
            }
        }

        protected override void Recycle()
        {
            PathPool<ConstantPath>.Recycle(this);
        }

        public override void Reset()
        {
            base.Reset();
            this.allNodes = ListPool<GraphNode>.Claim();
            this.endingCondition = null;
            this.originalStartPoint = Vector3.zero;
            this.startPoint = Vector3.zero;
            this.startNode = null;
            base.heuristic = Heuristic.None;
        }

        protected void Setup(Vector3 start, int maxGScore, OnPathDelegate callback)
        {
            base.callback = callback;
            this.startPoint = start;
            this.originalStartPoint = this.startPoint;
            this.endingCondition = new EndingConditionDistance(this, maxGScore);
        }

        public override bool FloodingPath
        {
            get
            {
                return true;
            }
        }
    }
}

