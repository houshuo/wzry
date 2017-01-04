namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct DuraEftPlayParam
    {
        public GameObject EftObj;
        public int RemainMSec;
    }
}

