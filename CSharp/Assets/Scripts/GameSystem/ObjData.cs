namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjData
    {
        public GameObject Object;
        public CActorInfo ActorInfo;
    }
}

