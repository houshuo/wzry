namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LockTargetEventParam
    {
        public uint lockTargetID;
        public LockTargetEventParam(uint _targetID)
        {
            this.lockTargetID = _targetID;
        }
    }
}

