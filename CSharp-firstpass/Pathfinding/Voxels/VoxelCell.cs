namespace Pathfinding.Voxels
{
    using Pathfinding;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct VoxelCell
    {
        public VoxelSpan firstSpan;
        public void AddSpan(uint bottom, uint top, int area, int voxelWalkableClimb)
        {
            VoxelSpan span = new VoxelSpan(bottom, top, area);
            if (this.firstSpan == null)
            {
                this.firstSpan = span;
            }
            else
            {
                VoxelSpan span2 = null;
                VoxelSpan firstSpan = this.firstSpan;
                while (firstSpan != null)
                {
                    if (firstSpan.bottom > span.top)
                    {
                        break;
                    }
                    if (firstSpan.top < span.bottom)
                    {
                        span2 = firstSpan;
                        firstSpan = firstSpan.next;
                    }
                    else
                    {
                        if (firstSpan.bottom < bottom)
                        {
                            span.bottom = firstSpan.bottom;
                        }
                        if (firstSpan.top > top)
                        {
                            span.top = firstSpan.top;
                        }
                        if (AstarMath.Abs((int) (span.top - firstSpan.top)) <= voxelWalkableClimb)
                        {
                            span.area = AstarMath.Max(span.area, firstSpan.area);
                        }
                        VoxelSpan next = firstSpan.next;
                        if (span2 != null)
                        {
                            span2.next = next;
                        }
                        else
                        {
                            this.firstSpan = next;
                        }
                        firstSpan = next;
                    }
                }
                if (span2 != null)
                {
                    span.next = span2.next;
                    span2.next = span;
                }
                else
                {
                    span.next = this.firstSpan;
                    this.firstSpan = span;
                }
            }
        }
    }
}

