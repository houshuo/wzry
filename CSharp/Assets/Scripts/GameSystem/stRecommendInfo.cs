namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stRecommendInfo
    {
        public int recommendTime;
        public ulong uid;
        public string headUrl;
        public string name;
        public uint level;
        public uint ability;
        public string recommendName;
        public stVipInfo stVip;
    }
}

