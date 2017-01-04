namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ActorSkillEventParam
    {
        public SkillSlotType slot;
        public PoolObjHandle<ActorRoot> src;
        public ActorSkillEventParam(PoolObjHandle<ActorRoot> _src, SkillSlotType _slot = 0)
        {
            this.src = _src;
            this.slot = _slot;
        }
    }
}

