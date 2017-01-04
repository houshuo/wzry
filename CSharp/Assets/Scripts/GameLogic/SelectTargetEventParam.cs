namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SelectTargetEventParam
    {
        public uint commonAttackTargetID;
        public SelectTargetEventParam(uint _targetID)
        {
            this.commonAttackTargetID = _targetID;
        }
    }
}

