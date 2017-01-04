namespace Assets.Scripts.GameLogic.DataCenter
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ActorServerSkillData
    {
        public ActorMeta TheActorMeta;
        public ActorSkillSlot SkillSlot;
        public bool IsUnlocked;
        public uint SkillLevel;
        public uint SelfSkill;
    }
}

