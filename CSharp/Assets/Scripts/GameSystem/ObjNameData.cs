namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjNameData
    {
        public string ObjectName;
        public CActorInfo ActorInfo;
    }
}

