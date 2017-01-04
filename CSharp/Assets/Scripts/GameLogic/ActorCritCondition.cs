namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.ActorCritCondition)]
    public class ActorCritCondition : PassiveCondition
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
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorCrit, new RefAction<DefaultGameEventParam>(this.onActorCrit));
        }

        private void onActorCrit(ref DefaultGameEventParam _prm)
        {
            if (_prm.src == base.sourceActor)
            {
                this.bTrigger = true;
            }
        }

        public override void Reset()
        {
            this.bTrigger = false;
        }

        public override void UnInit()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorCrit, new RefAction<DefaultGameEventParam>(this.onActorCrit));
        }
    }
}

