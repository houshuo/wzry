namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct HemophagiaEventResultInfo
    {
        public PoolObjHandle<ActorRoot> src;
        public int hpChanged;
        public HemophagiaEventResultInfo(PoolObjHandle<ActorRoot> _src, int _hpChanged)
        {
            this.src = _src;
            this.hpChanged = _hpChanged;
        }
    }
}

