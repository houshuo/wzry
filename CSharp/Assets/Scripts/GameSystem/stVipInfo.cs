namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stVipInfo
    {
        public uint score;
        public uint level;
        public uint headIconId;
        public void Reset()
        {
            this.score = 0;
            this.level = 0;
            this.headIconId = 0;
        }
    }
}

