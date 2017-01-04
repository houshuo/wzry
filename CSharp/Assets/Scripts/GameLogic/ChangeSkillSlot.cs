namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ChangeSkillSlot
    {
        public int changeCount;
        public int initSkillID;
        public int changeSkillID;
    }
}

