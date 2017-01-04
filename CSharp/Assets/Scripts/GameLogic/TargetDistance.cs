namespace Assets.Scripts.GameLogic
{
    using System;

    public class TargetDistance
    {
        public static ulong GetDistance(VInt3 _curPos, VInt3 _inPos)
        {
            VInt3 num2 = _curPos - _inPos;
            return (ulong) num2.sqrMagnitudeLong2D;
        }
    }
}

