namespace Assets.Scripts.GameLogic
{
    using System;

    public class DistanceSearchCondition
    {
        public static bool Fit(ActorRoot _actor, ActorRoot _mainActor, int _dis)
        {
            ulong num = (ulong) (_dis * _dis);
            VInt3 num3 = _actor.location - _mainActor.location;
            ulong num2 = (ulong) num3.sqrMagnitudeLong2D;
            return (num >= num2);
        }

        public static bool Fit(VInt3 _curPos, ActorRoot _mainActor, int _dis)
        {
            ulong num = (ulong) (_dis * _dis);
            VInt3 num3 = _curPos - _mainActor.location;
            ulong num2 = (ulong) num3.sqrMagnitudeLong2D;
            return (num >= num2);
        }
    }
}

