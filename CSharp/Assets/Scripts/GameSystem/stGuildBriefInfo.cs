namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct stGuildBriefInfo
    {
        public ulong uulUid;
        public string sName;
        public byte bLevel;
        public byte bMemCnt;
        public string sBulletin;
        public uint dwHeadId;
        public uint dwSettingMask;
        public uint Rank;
        public uint Ability;
        public void Reset()
        {
            this.uulUid = 0L;
            this.sName = null;
            this.bLevel = 0;
            this.bMemCnt = 0;
            this.sBulletin = null;
            this.dwHeadId = 0;
            this.dwSettingMask = 0;
            this.Rank = 0;
            this.Ability = 0;
        }
    }
}

