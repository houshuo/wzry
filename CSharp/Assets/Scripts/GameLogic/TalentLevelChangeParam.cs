namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TalentLevelChangeParam
    {
        public PoolObjHandle<ActorRoot> src;
        public int SoulLevel;
        public int TalentLevel;
    }
}

