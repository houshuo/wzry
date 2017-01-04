namespace Pathfinding
{
    using Pathfinding.Util;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [Serializable]
    public class EuclideanEmbedding
    {
        private uint[] costs = new uint[8];
        [NonSerialized]
        public bool dirty;
        private object lockObj = new object();
        private int maxNodeIndex;
        public HeuristicOptimizationMode mode;
        private int pivotCount;
        public Transform pivotPointRoot;
        private GraphNode[] pivots;
        private uint ra = 0xc39ec3;
        private uint rc = 0x43fd43fd;
        private uint rval;
        public int seed;
        public int spreadOutCount = 1;

        private void EnsureCapacity(int index)
        {
            if ((index > this.maxNodeIndex) && (index > this.maxNodeIndex))
            {
                if (index >= this.costs.Length)
                {
                    uint[] numArray = new uint[Math.Max((int) (index * 2), (int) (this.pivots.Length * 2))];
                    for (int i = 0; i < this.costs.Length; i++)
                    {
                        numArray[i] = this.costs[i];
                    }
                    this.costs = numArray;
                }
                this.maxNodeIndex = index;
            }
        }

        private void GetClosestWalkableNodesToChildrenRecursively(Transform tr, List<GraphNode> nodes)
        {
            IEnumerator enumerator = tr.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    NNInfo nearest = AstarPath.active.GetNearest(current.position, NNConstraint.Default);
                    if ((nearest.node != null) && nearest.node.Walkable)
                    {
                        nodes.Add(nearest.node);
                    }
                    this.GetClosestWalkableNodesToChildrenRecursively(tr, nodes);
                }
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

        public uint GetHeuristic(int nodeIndex1, int nodeIndex2)
        {
            nodeIndex1 *= this.pivotCount;
            nodeIndex2 *= this.pivotCount;
            if ((nodeIndex1 >= this.costs.Length) || (nodeIndex2 >= this.costs.Length))
            {
                this.EnsureCapacity((nodeIndex1 <= nodeIndex2) ? nodeIndex2 : nodeIndex1);
            }
            uint num = 0;
            for (int i = 0; i < this.pivotCount; i++)
            {
                uint num3 = (uint) Math.Abs((int) (this.costs[nodeIndex1 + i] - this.costs[nodeIndex2 + i]));
                if (num3 > num)
                {
                    num = num3;
                }
            }
            return num;
        }

        public uint GetRandom()
        {
            this.rval = (this.ra * this.rval) + this.rc;
            return this.rval;
        }

        public void OnDrawGizmos()
        {
            if (this.pivots != null)
            {
                for (int i = 0; i < this.pivots.Length; i++)
                {
                    Gizmos.color = new Color(0.6235294f, 0.3686275f, 0.7607843f, 0.8f);
                    if ((this.pivots[i] != null) && !this.pivots[i].Destroyed)
                    {
                        Gizmos.DrawCube((Vector3) this.pivots[i].position, Vector3.one);
                    }
                }
            }
        }

        public void RecalculateCosts()
        {
            <RecalculateCosts>c__AnonStorey39 storey = new <RecalculateCosts>c__AnonStorey39 {
                <>f__this = this
            };
            if (this.pivots == null)
            {
                this.RecalculatePivots();
            }
            if (this.mode != HeuristicOptimizationMode.None)
            {
                this.pivotCount = 0;
                DebugHelper.Assert(this.pivots != null);
                for (int i = 0; i < this.pivots.Length; i++)
                {
                    if ((this.pivots[i] != null) && (this.pivots[i].Destroyed || !this.pivots[i].Walkable))
                    {
                        throw new Exception("Invalid pivot nodes (destroyed or unwalkable)");
                    }
                }
                if (this.mode != HeuristicOptimizationMode.RandomSpreadOut)
                {
                    for (int j = 0; j < this.pivots.Length; j++)
                    {
                        if (this.pivots[j] == null)
                        {
                            throw new Exception("Invalid pivot nodes (null)");
                        }
                    }
                }
                Debug.Log("Recalculating costs...");
                this.pivotCount = this.pivots.Length;
                storey.startCostCalculation = null;
                storey.startCostCalculation = new Action<int>(storey.<>m__2A);
                if (this.mode != HeuristicOptimizationMode.RandomSpreadOut)
                {
                    for (int k = 0; k < this.pivots.Length; k++)
                    {
                        storey.startCostCalculation(k);
                    }
                }
                else
                {
                    storey.startCostCalculation(0);
                }
                this.dirty = false;
            }
        }

        public void RecalculatePivots()
        {
            <RecalculatePivots>c__AnonStorey35 storey = new <RecalculatePivots>c__AnonStorey35 {
                <>f__this = this
            };
            if (this.mode == HeuristicOptimizationMode.None)
            {
                this.pivotCount = 0;
                this.pivots = null;
            }
            else
            {
                this.rval = (uint) this.seed;
                NavGraph[] graphs = AstarPath.active.graphs;
                storey.pivotList = ListPool<GraphNode>.Claim();
                if (this.mode == HeuristicOptimizationMode.Custom)
                {
                    if (this.pivotPointRoot == null)
                    {
                        throw new Exception("Grid Graph -> heuristicOptimizationMode is HeuristicOptimizationMode.Custom, but no 'customHeuristicOptimizationPivotsRoot' is set");
                    }
                    this.GetClosestWalkableNodesToChildrenRecursively(this.pivotPointRoot, storey.pivotList);
                }
                else if (this.mode == HeuristicOptimizationMode.Random)
                {
                    <RecalculatePivots>c__AnonStorey34 storey2 = new <RecalculatePivots>c__AnonStorey34 {
                        <>f__ref$53 = storey,
                        <>f__this = this,
                        n = 0
                    };
                    for (int i = 0; i < graphs.Length; i++)
                    {
                        graphs[i].GetNodes(new GraphNodeDelegateCancelable(storey2.<>m__28));
                    }
                }
                else
                {
                    if (this.mode != HeuristicOptimizationMode.RandomSpreadOut)
                    {
                        throw new Exception("Invalid HeuristicOptimizationMode: " + this.mode);
                    }
                    <RecalculatePivots>c__AnonStorey36 storey3 = new <RecalculatePivots>c__AnonStorey36 {
                        first = null
                    };
                    if (this.pivotPointRoot != null)
                    {
                        this.GetClosestWalkableNodesToChildrenRecursively(this.pivotPointRoot, storey.pivotList);
                    }
                    else
                    {
                        for (int k = 0; k < graphs.Length; k++)
                        {
                            graphs[k].GetNodes(new GraphNodeDelegateCancelable(storey3.<>m__29));
                        }
                        if (storey3.first != null)
                        {
                            storey.pivotList.Add(storey3.first);
                        }
                        else
                        {
                            Debug.LogError("Could not find any walkable node in any of the graphs.");
                            ListPool<GraphNode>.Release(storey.pivotList);
                            return;
                        }
                    }
                    for (int j = 0; j < this.spreadOutCount; j++)
                    {
                        storey.pivotList.Add(null);
                    }
                }
                this.pivots = storey.pivotList.ToArray();
                ListPool<GraphNode>.Release(storey.pivotList);
            }
        }

        [CompilerGenerated]
        private sealed class <RecalculateCosts>c__AnonStorey39
        {
            internal EuclideanEmbedding <>f__this;
            internal Action<int> startCostCalculation;

            internal void <>m__2A(int k)
            {
                <RecalculateCosts>c__AnonStorey37 storey;
                storey = new <RecalculateCosts>c__AnonStorey37 {
                    <>f__ref$57 = this,
                    k = k,
                    pivot = this.<>f__this.pivots[storey.k],
                    fp = null,
                    fp = FloodPath.Construct(storey.pivot, null)
                };
                storey.fp.immediateCallback = new OnPathDelegate(storey.<>m__2B);
                AstarPath.StartPath(storey.fp, true);
            }

            private sealed class <RecalculateCosts>c__AnonStorey37
            {
                internal EuclideanEmbedding.<RecalculateCosts>c__AnonStorey39 <>f__ref$57;
                internal FloodPath fp;
                internal int k;
                internal GraphNode pivot;

                internal void <>m__2B(Path _p)
                {
                    <RecalculateCosts>c__AnonStorey38 storey = new <RecalculateCosts>c__AnonStorey38 {
                        <>f__ref$57 = this.<>f__ref$57,
                        <>f__ref$55 = this
                    };
                    _p.Claim(this.<>f__ref$57.<>f__this);
                    MeshNode pivot = this.pivot as MeshNode;
                    storey.costOffset = 0;
                    if ((pivot != null) && (pivot.connectionCosts != null))
                    {
                        for (int j = 0; j < pivot.connectionCosts.Length; j++)
                        {
                            storey.costOffset = Math.Max(storey.costOffset, pivot.connectionCosts[j]);
                        }
                    }
                    NavGraph[] graphs = AstarPath.active.graphs;
                    for (int i = graphs.Length - 1; i >= 0; i--)
                    {
                        graphs[i].GetNodes(new GraphNodeDelegateCancelable(storey.<>m__2C));
                    }
                    if ((this.<>f__ref$57.<>f__this.mode == HeuristicOptimizationMode.RandomSpreadOut) && (this.k < (this.<>f__ref$57.<>f__this.pivots.Length - 1)))
                    {
                        int nodeIndex = -1;
                        uint num4 = 0;
                        int num5 = this.<>f__ref$57.<>f__this.maxNodeIndex / this.<>f__ref$57.<>f__this.pivotCount;
                        for (int k = 1; k < num5; k++)
                        {
                            uint num7 = 0x40000000;
                            for (int m = 0; m <= this.k; m++)
                            {
                                num7 = Math.Min(num7, this.<>f__ref$57.<>f__this.costs[(k * this.<>f__ref$57.<>f__this.pivotCount) + m]);
                            }
                            GraphNode node = this.fp.pathHandler.GetPathNode(k).node;
                            if (((num7 > num4) || (nodeIndex == -1)) && (((node != null) && !node.Destroyed) && node.Walkable))
                            {
                                nodeIndex = k;
                                num4 = num7;
                            }
                        }
                        if (nodeIndex == -1)
                        {
                            Debug.LogError("Failed generating random pivot points for heuristic optimizations");
                            return;
                        }
                        this.<>f__ref$57.<>f__this.pivots[this.k + 1] = this.fp.pathHandler.GetPathNode(nodeIndex).node;
                        Debug.Log(string.Concat(new object[] { "Found node at ", this.<>f__ref$57.<>f__this.pivots[this.k + 1].position, " with score ", num4 }));
                        this.<>f__ref$57.startCostCalculation(this.k + 1);
                    }
                    _p.Release(this.<>f__ref$57.<>f__this);
                }

                private sealed class <RecalculateCosts>c__AnonStorey38
                {
                    internal EuclideanEmbedding.<RecalculateCosts>c__AnonStorey39.<RecalculateCosts>c__AnonStorey37 <>f__ref$55;
                    internal EuclideanEmbedding.<RecalculateCosts>c__AnonStorey39 <>f__ref$57;
                    internal uint costOffset;

                    internal bool <>m__2C(GraphNode node)
                    {
                        int index = (node.NodeIndex * this.<>f__ref$57.<>f__this.pivotCount) + this.<>f__ref$55.k;
                        this.<>f__ref$57.<>f__this.EnsureCapacity(index);
                        PathNode pathNode = this.<>f__ref$55.fp.pathHandler.GetPathNode(node);
                        if (this.costOffset > 0)
                        {
                            this.<>f__ref$57.<>f__this.costs[index] = ((pathNode.pathID != this.<>f__ref$55.fp.pathID) || (pathNode.parent == null)) ? 0 : Math.Max((uint) (pathNode.parent.G - this.costOffset), (uint) 0);
                        }
                        else
                        {
                            this.<>f__ref$57.<>f__this.costs[index] = (pathNode.pathID != this.<>f__ref$55.fp.pathID) ? 0 : pathNode.G;
                        }
                        return true;
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <RecalculatePivots>c__AnonStorey34
        {
            internal EuclideanEmbedding.<RecalculatePivots>c__AnonStorey35 <>f__ref$53;
            internal EuclideanEmbedding <>f__this;
            internal int n;

            internal bool <>m__28(GraphNode node)
            {
                if (!node.Destroyed && node.Walkable)
                {
                    this.n++;
                    if ((((ulong) this.<>f__this.GetRandom()) % ((long) this.n)) < this.<>f__this.spreadOutCount)
                    {
                        if (this.<>f__ref$53.pivotList.Count < this.<>f__this.spreadOutCount)
                        {
                            this.<>f__ref$53.pivotList.Add(node);
                        }
                        else
                        {
                            this.<>f__ref$53.pivotList[(int) (((ulong) this.<>f__this.GetRandom()) % ((long) this.<>f__ref$53.pivotList.Count))] = node;
                        }
                    }
                }
                return true;
            }
        }

        [CompilerGenerated]
        private sealed class <RecalculatePivots>c__AnonStorey35
        {
            internal EuclideanEmbedding <>f__this;
            internal List<GraphNode> pivotList;
        }

        [CompilerGenerated]
        private sealed class <RecalculatePivots>c__AnonStorey36
        {
            internal GraphNode first;

            internal bool <>m__29(GraphNode node)
            {
                if ((node != null) && node.Walkable)
                {
                    this.first = node;
                    return false;
                }
                return true;
            }
        }
    }
}

