namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stPrepareGuildBriefInfo
    {
        public ulong uulUid;
        public int dwLogicWorldId;
        public string sName;
        public byte bMemCnt;
        public uint dwHeadId;
        public uint dwRequestTime;
        public stGuildMemBriefInfo stCreatePlayer;
        public string sBulletin;
        public bool IsOnlyFriend;
        public void Reset()
        {
            this.uulUid = 0L;
            this.dwLogicWorldId = 0;
            this.sName = null;
            this.bMemCnt = 0;
            this.dwHeadId = 0;
            this.dwRequestTime = 0;
            this.sBulletin = null;
            this.IsOnlyFriend = false;
            this.stCreatePlayer.Reset();
        }
    }
}

