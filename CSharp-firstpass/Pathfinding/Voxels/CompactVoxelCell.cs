namespace Pathfinding.Voxels
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct CompactVoxelCell
    {
        public uint index;
        public uint count;
        public CompactVoxelCell(uint i, uint c)
        {
            this.index = i;
            this.count = c;
        }
    }
}

