namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ChangeSkillEventParam
    {
        public SkillSlotType slot;
        public int skillID;
        public int changeTime;
        public ChangeSkillEventParam(SkillSlotType _slot, int _id, int _time)
        {
            this.slot = _slot;
            this.skillID = _id;
            this.changeTime = _time;
        }
    }
}

