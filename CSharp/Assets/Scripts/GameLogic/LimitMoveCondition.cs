namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.LimitMoveCondition)]
    public class LimitMoveCondition : PassiveCondition
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
            Singleton<GameSkillEventSys>.instance.AddEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_LimitMove, new GameSkillEvent<LimitMoveEventParam>(this.onActorLimitMove));
            Singleton<GameSkillEventSys>.instance.AddEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, new GameSkillEvent<LimitMoveEventParam>(this.onActorCancelLimitMove));
        }

        private void onActorCancelLimitMove(ref LimitMoveEventParam _param)
        {
            if (_param.src == base.sourceActor)
            {
                this.bTrigger = false;
            }
        }

        private void onActorLimitMove(ref LimitMoveEventParam _param)
        {
            if (_param.src == base.sourceActor)
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
            Singleton<GameSkillEventSys>.instance.RmvEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_LimitMove, new GameSkillEvent<LimitMoveEventParam>(this.onActorLimitMove));
            Singleton<GameSkillEventSys>.instance.RmvEventHandler<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, new GameSkillEvent<LimitMoveEventParam>(this.onActorCancelLimitMove));
        }
    }
}

