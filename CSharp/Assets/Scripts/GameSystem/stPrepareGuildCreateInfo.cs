namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stPrepareGuildCreateInfo
    {
        public string sName;
        public uint dwHeadId;
        public string sBulletin;
        public bool isOnlyFriend;
    }
}

