namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SCampScoreUpdateParam
    {
        public int HeadCount;
        public int HeadPoints;
        public PoolObjHandle<ActorRoot> src;
        public PoolObjHandle<ActorRoot> atker;
        public COM_PLAYERCAMP CampType;
        public SCampScoreUpdateParam(int inHeadCount, int inHeadPoints, PoolObjHandle<ActorRoot> inSrc, PoolObjHandle<ActorRoot> inAtker, COM_PLAYERCAMP inCampType)
        {
            this.HeadCount = inHeadCount;
            this.HeadPoints = inHeadPoints;
            this.src = inSrc;
            this.atker = inAtker;
            this.CampType = inCampType;
        }
    }
}

