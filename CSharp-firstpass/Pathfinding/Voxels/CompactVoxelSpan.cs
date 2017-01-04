namespace Pathfinding.Voxels
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct CompactVoxelSpan
    {
        public ushort y;
        public uint con;
        public uint h;
        public int reg;
        public CompactVoxelSpan(ushort bottom, uint height)
        {
            this.con = 0x18;
            this.y = bottom;
            this.h = height;
            this.reg = 0;
        }

        public void SetConnection(int dir, uint value)
        {
            int num = dir * 6;
            this.con = (uint) ((this.con & ~(((int) 0x3f) << num)) | ((ulong) ((value & 0x3f) << (num & 0x1f))));
        }

        public int GetConnection(int dir)
        {
            return (((int) (this.con >> (dir * 6))) & 0x3f);
        }
    }
}

