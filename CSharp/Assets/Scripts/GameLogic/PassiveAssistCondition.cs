namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.AssistPassiveCondition)]
    public class PassiveAssistCondition : PassiveCondition
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
            this.sourceActor.handle.ActorControl.eventActorAssist += new ActorEventHandler(this.onActorAssist);
        }

        private void onActorAssist(ref DefaultGameEventParam prm)
        {
            if ((prm.orignalAtker == base.sourceActor) && base.CheckTargetSubType(prm.src, base.localParams[0], base.localParams[1]))
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
            if (base.sourceActor != 0)
            {
                this.sourceActor.handle.ActorControl.eventActorAssist -= new ActorEventHandler(this.onActorAssist);
            }
        }
    }
}

