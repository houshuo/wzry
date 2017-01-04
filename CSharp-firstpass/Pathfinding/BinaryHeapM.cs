namespace Pathfinding
{
    using System;
    using System.Runtime.InteropServices;

    public class BinaryHeapM
    {
        private Tuple[] binaryHeap;
        public const int D = 4;
        public float growthFactor = 2f;
        public int numberOfItems;
        private const bool SortGScores = true;

        public BinaryHeapM(int numberOfElements)
        {
            this.binaryHeap = new Tuple[numberOfElements];
            this.numberOfItems = 0;
        }

        public void Add(PathNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("Sending null node to BinaryHeap");
            }
            if (this.numberOfItems == this.binaryHeap.Length)
            {
                int num = Math.Max(this.binaryHeap.Length + 4, (int) Math.Round((double) (this.binaryHeap.Length * this.growthFactor)));
                if (num > 0x40000)
                {
                    throw new Exception("Binary Heap Size really large (2^18). A heap size this large is probably the cause of pathfinding running in an infinite loop. \nRemove this check (in BinaryHeap.cs) if you are sure that it is not caused by a bug");
                }
                Tuple[] tupleArray = new Tuple[num];
                for (int i = 0; i < this.binaryHeap.Length; i++)
                {
                    tupleArray[i] = this.binaryHeap[i];
                }
                this.binaryHeap = tupleArray;
            }
            Tuple tuple = new Tuple(node.F, node);
            this.binaryHeap[this.numberOfItems] = tuple;
            int numberOfItems = this.numberOfItems;
            uint f = node.F;
            uint g = node.G;
            while (numberOfItems != 0)
            {
                int index = (numberOfItems - 1) / 4;
                if ((f >= this.binaryHeap[index].F) && ((f != this.binaryHeap[index].F) || (g <= this.binaryHeap[index].node.G)))
                {
                    break;
                }
                this.binaryHeap[numberOfItems] = this.binaryHeap[index];
                this.binaryHeap[index] = tuple;
                numberOfItems = index;
            }
            this.numberOfItems++;
        }

        public void Clear()
        {
            this.numberOfItems = 0;
        }

        internal PathNode GetNode(int i)
        {
            return this.binaryHeap[i].node;
        }

        public void Rebuild()
        {
            for (int i = 2; i < this.numberOfItems; i++)
            {
                int index = i;
                Tuple tuple = this.binaryHeap[i];
                uint f = tuple.F;
                while (index != 1)
                {
                    int num4 = index / 4;
                    if (f >= this.binaryHeap[num4].F)
                    {
                        break;
                    }
                    this.binaryHeap[index] = this.binaryHeap[num4];
                    this.binaryHeap[num4] = tuple;
                    index = num4;
                }
            }
        }

        public PathNode Remove()
        {
            this.numberOfItems--;
            PathNode node = this.binaryHeap[0].node;
            this.binaryHeap[0] = this.binaryHeap[this.numberOfItems];
            int index = 0;
            int num2 = 0;
            while (true)
            {
                num2 = index;
                uint f = this.binaryHeap[index].F;
                int num5 = (num2 * 4) + 1;
                if ((num5 <= this.numberOfItems) && ((this.binaryHeap[num5].F < f) || ((this.binaryHeap[num5].F == f) && (this.binaryHeap[num5].node.G < this.binaryHeap[index].node.G))))
                {
                    f = this.binaryHeap[num5].F;
                    index = num5;
                }
                if (((num5 + 1) <= this.numberOfItems) && ((this.binaryHeap[num5 + 1].F < f) || ((this.binaryHeap[num5 + 1].F == f) && (this.binaryHeap[num5 + 1].node.G < this.binaryHeap[index].node.G))))
                {
                    f = this.binaryHeap[num5 + 1].F;
                    index = num5 + 1;
                }
                if (((num5 + 2) <= this.numberOfItems) && ((this.binaryHeap[num5 + 2].F < f) || ((this.binaryHeap[num5 + 2].F == f) && (this.binaryHeap[num5 + 2].node.G < this.binaryHeap[index].node.G))))
                {
                    f = this.binaryHeap[num5 + 2].F;
                    index = num5 + 2;
                }
                if (((num5 + 3) <= this.numberOfItems) && ((this.binaryHeap[num5 + 3].F < f) || ((this.binaryHeap[num5 + 3].F == f) && (this.binaryHeap[num5 + 3].node.G < this.binaryHeap[index].node.G))))
                {
                    f = this.binaryHeap[num5 + 3].F;
                    index = num5 + 3;
                }
                if (num2 == index)
                {
                    return node;
                }
                Tuple tuple = this.binaryHeap[num2];
                this.binaryHeap[num2] = this.binaryHeap[index];
                this.binaryHeap[index] = tuple;
            }
        }

        internal void SetF(int i, uint F)
        {
            this.binaryHeap[i].F = F;
        }

        private void Validate()
        {
            for (int i = 1; i < this.numberOfItems; i++)
            {
                int index = (i - 1) / 4;
                if (this.binaryHeap[index].F > this.binaryHeap[i].F)
                {
                    object[] objArray1 = new object[] { "Invalid state at ", i, ":", index, " ( ", this.binaryHeap[index].F, " > ", this.binaryHeap[i].F, " ) " };
                    throw new Exception(string.Concat(objArray1));
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Tuple
        {
            public uint F;
            public PathNode node;
            public Tuple(uint F, PathNode node)
            {
                this.F = F;
                this.node = node;
            }
        }
    }
}

