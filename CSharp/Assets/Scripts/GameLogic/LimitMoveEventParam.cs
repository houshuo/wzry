namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LimitMoveEventParam
    {
        public int totalTime;
        public int combineID;
        public PoolObjHandle<ActorRoot> src;
        public LimitMoveEventParam(int _time, int _combineID, PoolObjHandle<ActorRoot> _src)
        {
            this.totalTime = _time;
            this.combineID = _combineID;
            this.src = _src;
        }
    }
}

