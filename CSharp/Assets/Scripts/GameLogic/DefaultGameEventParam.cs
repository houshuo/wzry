namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DefaultGameEventParam
    {
        public PoolObjHandle<ActorRoot> src;
        public PoolObjHandle<ActorRoot> atker;
        public PoolObjHandle<ActorRoot> orignalAtker;
        public DefaultGameEventParam(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker)
        {
            this.src = _src;
            this.atker = _atker;
            this.orignalAtker = _atker;
        }

        public DefaultGameEventParam(PoolObjHandle<ActorRoot> _src, PoolObjHandle<ActorRoot> _atker, ref PoolObjHandle<ActorRoot> _orignalAtker)
        {
            this.src = _src;
            this.atker = _atker;
            this.orignalAtker = _orignalAtker;
        }
    }
}

