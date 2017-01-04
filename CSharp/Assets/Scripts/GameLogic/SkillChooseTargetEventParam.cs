namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SkillChooseTargetEventParam
    {
        public PoolObjHandle<ActorRoot> src;
        public PoolObjHandle<ActorRoot> atker;
        public int iTargetCount;
        public SkillChooseTargetEventParam(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker, int _iTargetCount)
        {
            this.src = _src;
            this.atker = _atker;
            this.iTargetCount = _iTargetCount;
        }
    }
}

