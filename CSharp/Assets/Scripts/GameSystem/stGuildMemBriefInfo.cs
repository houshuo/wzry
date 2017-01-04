namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct stGuildMemBriefInfo
    {
        public ulong uulUid;
        public int dwLogicWorldId;
        public string sName;
        public uint dwLevel;
        public uint dwAbility;
        public string szHeadUrl;
        public uint dwGameEntity;
        public stVipInfo stVip;
        public uint dwScoreOfRank;
        public uint dwClassOfRank;
        public void Reset()
        {
            this.uulUid = 0L;
            this.dwLogicWorldId = 0;
            this.sName = null;
            this.dwLevel = 0;
            this.dwAbility = 0;
            this.dwGameEntity = 0;
            this.stVip.Reset();
            this.dwScoreOfRank = 0;
            this.dwClassOfRank = 0;
        }
    }
}

