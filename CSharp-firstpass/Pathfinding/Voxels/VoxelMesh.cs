namespace Pathfinding.Voxels
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct VoxelMesh
    {
        public VInt3[] verts;
        public int[] tris;
    }
}

