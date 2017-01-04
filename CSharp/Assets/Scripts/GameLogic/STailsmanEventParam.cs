namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct STailsmanEventParam
    {
        public PoolObjHandle<CTailsman> tailsman;
        public PoolObjHandle<ActorRoot> user;
        public STailsmanEventParam(PoolObjHandle<CTailsman> inTailsman, PoolObjHandle<ActorRoot> inUser)
        {
            this.tailsman = inTailsman;
            this.user = inUser;
        }
    }
}

