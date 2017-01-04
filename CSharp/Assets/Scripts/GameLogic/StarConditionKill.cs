namespace Assets.Scripts.GameLogic
{
    using System;

    [StarCondition(1)]
    internal class StarConditionKill : StarConditionKillBase
    {
        protected override void OnStatChanged()
        {
            this.TriggerChangedEvent();
        }

        protected override bool isSelfCamp
        {
            get
            {
                return (base.ConditionInfo.KeyDetail[2] == 0);
            }
        }

        protected override int targetCount
        {
            get
            {
                return base.ConditionInfo.ValueDetail[0];
            }
        }

        protected override int targetID
        {
            get
            {
                return base.ConditionInfo.KeyDetail[1];
            }
        }

        protected override ActorTypeDef targetType
        {
            get
            {
                return (ActorTypeDef) base.ConditionInfo.KeyDetail[0];
            }
        }
    }
}

