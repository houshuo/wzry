namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stFriendVerifyContent
    {
        public ulong ullUid;
        public uint dwLogicWorldID;
        public string content;
        public stFriendVerifyContent(ulong ullUid, uint dwLogicWorldID, string content)
        {
            this.ullUid = ullUid;
            this.dwLogicWorldID = dwLogicWorldID;
            this.content = content;
        }
    }
}

