namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    public class TargetProperty
    {
        public static ulong GetPropertyHpRate(ActorRoot _inActor, RES_FUNCEFT_TYPE _type)
        {
            int num = _inActor.ValueComponent.actorHp * 100;
            int totalValue = _inActor.ValueComponent.mActorValue[_type].totalValue;
            if (totalValue != 0)
            {
                num /= totalValue;
            }
            else
            {
                object[] inParameters = new object[] { (_inActor.gameObject != null) ? _inActor.name : "null" };
                DebugHelper.Assert(false, "Gameobj MaxHp = 0 Exception,  ActorName:{0}", inParameters);
            }
            return (ulong) num;
        }

        public static ulong GetPropertyValue(ActorRoot _inActor, RES_FUNCEFT_TYPE _type)
        {
            return (ulong) _inActor.ValueComponent.mActorValue[_type].totalValue;
        }
    }
}

