namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SGroupDeadEventParam
    {
        public CommonSpawnGroup csg;
        public SpawnGroup sg;
    }
}

