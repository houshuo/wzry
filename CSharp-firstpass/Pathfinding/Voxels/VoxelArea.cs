namespace Pathfinding.Voxels
{
    using Pathfinding;
    using System;
    using UnityEngine;

    public class VoxelArea
    {
        public int[] areaTypes;
        public const float AvgSpanLayerCountEstimate = 8f;
        public CompactVoxelCell[] compactCells;
        public int compactSpanCount;
        public CompactVoxelSpan[] compactSpans;
        public readonly int depth;
        public int[] DirectionX;
        public int[] DirectionZ;
        public ushort[] dist;
        public const uint InvalidSpanValue = uint.MaxValue;
        private int linkedSpanCount;
        public LinkedVoxelSpan[] linkedSpans;
        public ushort maxDistance;
        public const uint MaxHeight = 0x10000;
        public const int MaxHeightInt = 0x10000;
        public int maxRegions;
        private int[] removedStack = new int[0x80];
        private int removedStackCount;
        public ushort[] tmpUShortArr;
        public Vector3[] VectorDirection;
        public readonly int width;

        public VoxelArea(int width, int depth)
        {
            this.width = width;
            this.depth = depth;
            int num = width * depth;
            this.compactCells = new CompactVoxelCell[num];
            this.linkedSpans = new LinkedVoxelSpan[(((int) (num * 8f)) + 15) & -16];
            this.ResetLinkedVoxelSpans();
            int[] numArray1 = new int[4];
            numArray1[0] = -1;
            numArray1[2] = 1;
            this.DirectionX = numArray1;
            int[] numArray2 = new int[4];
            numArray2[1] = width;
            numArray2[3] = -width;
            this.DirectionZ = numArray2;
            this.VectorDirection = new Vector3[] { Vector3.left, Vector3.forward, Vector3.right, Vector3.back };
        }

        public void AddLinkedSpan(int index, uint bottom, uint top, int area, int voxelWalkableClimb)
        {
            if (this.linkedSpans[index].bottom == uint.MaxValue)
            {
                this.linkedSpans[index] = new LinkedVoxelSpan(bottom, top, area);
            }
            else
            {
                int num7;
                int num = -1;
                int num2 = index;
                while (index != -1)
                {
                    if (this.linkedSpans[index].bottom > top)
                    {
                        break;
                    }
                    if (this.linkedSpans[index].top < bottom)
                    {
                        num = index;
                        index = this.linkedSpans[index].next;
                    }
                    else
                    {
                        if (this.linkedSpans[index].bottom < bottom)
                        {
                            bottom = this.linkedSpans[index].bottom;
                        }
                        if (this.linkedSpans[index].top > top)
                        {
                            top = this.linkedSpans[index].top;
                        }
                        if (AstarMath.Abs((int) (top - this.linkedSpans[index].top)) <= voxelWalkableClimb)
                        {
                            area = AstarMath.Max(area, this.linkedSpans[index].area);
                        }
                        int next = this.linkedSpans[index].next;
                        if (num != -1)
                        {
                            this.linkedSpans[num].next = next;
                            if (this.removedStackCount == this.removedStack.Length)
                            {
                                int[] dst = new int[this.removedStackCount * 4];
                                Buffer.BlockCopy(this.removedStack, 0, dst, 0, this.removedStackCount * 4);
                                this.removedStack = dst;
                            }
                            this.removedStack[this.removedStackCount] = index;
                            this.removedStackCount++;
                            index = next;
                            continue;
                        }
                        if (next != -1)
                        {
                            this.linkedSpans[num2] = this.linkedSpans[next];
                            if (this.removedStackCount == this.removedStack.Length)
                            {
                                int[] numArray2 = new int[this.removedStackCount * 4];
                                Buffer.BlockCopy(this.removedStack, 0, numArray2, 0, this.removedStackCount * 4);
                                this.removedStack = numArray2;
                            }
                            this.removedStack[this.removedStackCount] = next;
                            this.removedStackCount++;
                            index = this.linkedSpans[num2].next;
                            continue;
                        }
                        this.linkedSpans[num2] = new LinkedVoxelSpan(bottom, top, area);
                        return;
                    }
                }
                if (this.linkedSpanCount >= this.linkedSpans.Length)
                {
                    LinkedVoxelSpan[] linkedSpans = this.linkedSpans;
                    int linkedSpanCount = this.linkedSpanCount;
                    int removedStackCount = this.removedStackCount;
                    this.linkedSpans = new LinkedVoxelSpan[this.linkedSpans.Length * 2];
                    this.ResetLinkedVoxelSpans();
                    this.linkedSpanCount = linkedSpanCount;
                    this.removedStackCount = removedStackCount;
                    for (int i = 0; i < this.linkedSpanCount; i++)
                    {
                        this.linkedSpans[i] = linkedSpans[i];
                    }
                    Debug.Log("Layer estimate too low, doubling size of buffer.\nThis message is harmless.");
                }
                if (this.removedStackCount > 0)
                {
                    this.removedStackCount--;
                    num7 = this.removedStack[this.removedStackCount];
                }
                else
                {
                    num7 = this.linkedSpanCount;
                    this.linkedSpanCount++;
                }
                if (num != -1)
                {
                    this.linkedSpans[num7] = new LinkedVoxelSpan(bottom, top, area, this.linkedSpans[num].next);
                    this.linkedSpans[num].next = num7;
                }
                else
                {
                    this.linkedSpans[num7] = this.linkedSpans[num2];
                    this.linkedSpans[num2] = new LinkedVoxelSpan(bottom, top, area, num7);
                }
            }
        }

        public int GetSpanCount()
        {
            int num = 0;
            int num2 = this.width * this.depth;
            for (int i = 0; i < num2; i++)
            {
                for (int j = i; (j != -1) && (this.linkedSpans[j].bottom != uint.MaxValue); j = this.linkedSpans[j].next)
                {
                    if (this.linkedSpans[j].area != 0)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public int GetSpanCountAll()
        {
            int num = 0;
            int num2 = this.width * this.depth;
            for (int i = 0; i < num2; i++)
            {
                for (int j = i; (j != -1) && (this.linkedSpans[j].bottom != uint.MaxValue); j = this.linkedSpans[j].next)
                {
                    num++;
                }
            }
            return num;
        }

        public void Reset()
        {
            this.ResetLinkedVoxelSpans();
            for (int i = 0; i < this.compactCells.Length; i++)
            {
                this.compactCells[i].count = 0;
                this.compactCells[i].index = 0;
            }
        }

        private void ResetLinkedVoxelSpans()
        {
            int length = this.linkedSpans.Length;
            this.linkedSpanCount = this.width * this.depth;
            LinkedVoxelSpan span = new LinkedVoxelSpan(uint.MaxValue, uint.MaxValue, -1, -1);
            for (int i = 0; i < length; i++)
            {
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
                i++;
                this.linkedSpans[i] = span;
            }
            this.removedStackCount = 0;
        }
    }
}

