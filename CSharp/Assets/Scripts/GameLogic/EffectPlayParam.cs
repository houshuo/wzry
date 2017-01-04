namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct EffectPlayParam
    {
        public GameObject eftObj;
        public EffectFollowType followType;
        public float y_offset;
    }
}

