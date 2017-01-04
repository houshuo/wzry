namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    [StarConditionAttrContext(2)]
    internal class StarConditionHealthStat : StarCondition
    {
        private bool bHasFailure;
        private int LoweastHealthPercent = 100;

        public override void Dispose()
        {
            base.Dispose();
            Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
        }

        public override void Initialize(ResDT_ConditionInfo InConditionInfo)
        {
            base.Initialize(InConditionInfo);
            Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
            Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
        }

        private void OnActorDamage(ref HurtEventResultInfo info)
        {
            if (!this.bHasFailure && ActorHelper.IsHostActor(ref info.src))
            {
                int actorHp = info.src.handle.ValueComponent.actorHp;
                int actorHpTotal = info.src.handle.ValueComponent.actorHpTotal;
                int inFirst = (actorHp * 100) / actorHpTotal;
                if (inFirst < this.LoweastHealthPercent)
                {
                    this.LoweastHealthPercent = inFirst;
                }
                bool flag = !SmartCompare.Compare<int>(inFirst, this.healthPercent, this.operation);
                if (this.bHasFailure != flag)
                {
                    this.bHasFailure = flag;
                    this.TriggerChangedEvent();
                }
            }
        }

        private int healthPercent
        {
            get
            {
                return base.ConditionInfo.ValueDetail[0];
            }
        }

        private int loweastHealthPercent
        {
            get
            {
                return this.LoweastHealthPercent;
            }
        }

        public override StarEvaluationStatus status
        {
            get
            {
                return (!this.bHasFailure ? StarEvaluationStatus.Success : StarEvaluationStatus.Failure);
            }
        }

        private ActorTypeDef targetActorType
        {
            get
            {
                return (ActorTypeDef) base.ConditionInfo.KeyDetail[1];
            }
        }

        public override int[] values
        {
            get
            {
                return new int[] { this.loweastHealthPercent };
            }
        }
    }
}

