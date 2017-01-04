namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SpawnBuffEventParam
    {
        public uint showType;
        public uint floatTextID;
        public PoolObjHandle<ActorRoot> src;
        public SpawnBuffEventParam(uint _showType, uint _floatTextID, PoolObjHandle<ActorRoot> _src)
        {
            this.showType = _showType;
            this.floatTextID = _floatTextID;
            this.src = _src;
        }
    }
}

