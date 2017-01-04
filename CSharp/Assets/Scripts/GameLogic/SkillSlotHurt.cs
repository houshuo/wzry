namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SkillSlotHurt
    {
        public int curTotalHurt;
        public int nextTotalHurt;
        public uint skillUseCount;
        public int cdTime;
        public ulong recordTime;
    }
}

