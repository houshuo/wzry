namespace Assets.Scripts.GameLogic.DataCenter
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ActorStaticSkillData
    {
        public ActorMeta TheActorMeta;
        public ActorSkillSlot SkillSlot;
        public int SkillId;
        public int PassiveSkillId;
    }
}

