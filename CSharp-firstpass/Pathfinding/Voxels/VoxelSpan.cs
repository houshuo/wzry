namespace Pathfinding.Voxels
{
    using System;

    public class VoxelSpan
    {
        public int area;
        public uint bottom;
        public VoxelSpan next;
        public uint top;

        public VoxelSpan(uint b, uint t, int area)
        {
            this.bottom = b;
            this.top = t;
            this.area = area;
        }
    }
}

