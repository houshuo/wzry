namespace Pathfinding
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class PathHandler
    {
        private Stack<PathNode[]> bucketCache = new Stack<PathNode[]>();
        private bool[] bucketCreated = new bool[0];
        private const int BucketIndexMask = 0x3ff;
        private bool[] bucketNew = new bool[0];
        private const int BucketSize = 0x400;
        private const int BucketSizeLog2 = 10;
        public readonly StringBuilder DebugStringBuilder = new StringBuilder();
        private int filledBuckets;
        private BinaryHeapM heap = new BinaryHeapM(0x80);
        public PathNode[][] nodes = new PathNode[0][];
        private ushort pathID;
        public readonly int threadID;
        public readonly int totalThreadCount;

        public PathHandler(int threadID, int totalThreadCount)
        {
            this.threadID = threadID;
            this.totalThreadCount = totalThreadCount;
        }

        public void ClearPathIDs()
        {
            for (int i = 0; i < this.nodes.Length; i++)
            {
                PathNode[] nodeArray = this.nodes[i];
                if (this.nodes[i] != null)
                {
                    for (int j = 0; j < 0x400; j++)
                    {
                        nodeArray[j].pathID = 0;
                    }
                }
            }
        }

        public void DestroyNode(GraphNode node)
        {
            PathNode pathNode = this.GetPathNode(node);
            pathNode.node = null;
            pathNode.parent = null;
        }

        public BinaryHeapM GetHeap()
        {
            return this.heap;
        }

        public PathNode GetPathNode(GraphNode node)
        {
            int nodeIndex = node.NodeIndex;
            return this.nodes[nodeIndex >> 10][nodeIndex & 0x3ff];
        }

        public PathNode GetPathNode(int nodeIndex)
        {
            return this.nodes[nodeIndex >> 10][nodeIndex & 0x3ff];
        }

        public bool HeapEmpty()
        {
            return (this.heap.numberOfItems <= 0);
        }

        public void InitializeForPath(Path p)
        {
            this.pathID = p.pathID;
            this.heap.Clear();
        }

        public void InitializeNode(GraphNode node)
        {
            int nodeIndex = node.NodeIndex;
            int index = nodeIndex >> 10;
            int num3 = nodeIndex & 0x3ff;
            if (index >= this.nodes.Length)
            {
                PathNode[][] nodeArray = new PathNode[Math.Max(Math.Max((int) ((this.nodes.Length * 3) / 2), (int) (index + 1)), this.nodes.Length + 2)][];
                for (int i = 0; i < this.nodes.Length; i++)
                {
                    nodeArray[i] = this.nodes[i];
                }
                bool[] flagArray = new bool[nodeArray.Length];
                for (int j = 0; j < this.nodes.Length; j++)
                {
                    flagArray[j] = this.bucketNew[j];
                }
                bool[] flagArray2 = new bool[nodeArray.Length];
                for (int k = 0; k < this.nodes.Length; k++)
                {
                    flagArray2[k] = this.bucketCreated[k];
                }
                this.nodes = nodeArray;
                this.bucketNew = flagArray;
                this.bucketCreated = flagArray2;
            }
            if (this.nodes[index] == null)
            {
                PathNode[] nodeArray2;
                if (this.bucketCache.Count > 0)
                {
                    nodeArray2 = this.bucketCache.Pop();
                }
                else
                {
                    nodeArray2 = new PathNode[0x400];
                    for (int m = 0; m < 0x400; m++)
                    {
                        nodeArray2[m] = new PathNode();
                    }
                }
                this.nodes[index] = nodeArray2;
                if (!this.bucketCreated[index])
                {
                    this.bucketNew[index] = true;
                    this.bucketCreated[index] = true;
                }
                this.filledBuckets++;
            }
            PathNode node2 = this.nodes[index][num3];
            node2.node = node;
        }

        public PathNode PopNode()
        {
            return this.heap.Remove();
        }

        public void PushNode(PathNode node)
        {
            this.heap.Add(node);
        }

        public void RebuildHeap()
        {
            this.heap.Rebuild();
        }

        public ushort PathID
        {
            get
            {
                return this.pathID;
            }
        }
    }
}

