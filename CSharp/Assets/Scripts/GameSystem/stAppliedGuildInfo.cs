namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct stAppliedGuildInfo
    {
        public stGuildBriefInfo stBriefInfo;
        public uint dwApplyTime;
    }
}

