namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct stInvitedFriend
    {
        public ulong uulUid;
        public int dwInviteTime;
    }
}

