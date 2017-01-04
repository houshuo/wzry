namespace Pathfinding.Voxels
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct VoxelContour
    {
        public int nverts;
        public int[] verts;
        public int[] rverts;
        public int reg;
        public int area;
    }
}

