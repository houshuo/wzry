namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.KilledPassiveCondition)]
    public class PassiveKilledCondition : PassiveCondition
    {
        private bool bTrigger;

        public override bool Fit()
        {
            return this.bTrigger;
        }

        public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
        {
            this.bTrigger = false;
            base.Init(_source, _event, ref _config);
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            if (((prm.logicAtker == base.sourceActor) && !prm.bImmediateRevive) && base.CheckTargetSubType(prm.src, base.localParams[0], base.localParams[1]))
            {
                this.bTrigger = true;
                base.rootEvent.SetTriggerActor(prm.src);
            }
        }

        public override void Reset()
        {
            this.bTrigger = false;
        }

        public override void UnInit()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
        }
    }
}

