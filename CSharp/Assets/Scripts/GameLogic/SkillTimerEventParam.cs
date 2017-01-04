namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SkillTimerEventParam
    {
        public int totalTime;
        public ulong starTime;
        public PoolObjHandle<ActorRoot> src;
        public SkillTimerEventParam(int _totalTime, ulong _starTime, PoolObjHandle<ActorRoot> _src)
        {
            this.totalTime = _totalTime;
            this.starTime = _starTime;
            this.src = _src;
        }
    }
}

