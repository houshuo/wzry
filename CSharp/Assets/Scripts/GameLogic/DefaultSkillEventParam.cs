namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DefaultSkillEventParam
    {
        public SkillSlotType slot;
        public int param;
        public PoolObjHandle<ActorRoot> actor;
        public DefaultSkillEventParam(SkillSlotType _slot, int _param, PoolObjHandle<ActorRoot> _actor)
        {
            this.slot = _slot;
            this.param = _param;
            this.actor = _actor;
        }
    }
}

