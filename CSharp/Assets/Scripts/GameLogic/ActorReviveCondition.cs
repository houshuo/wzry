namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.ActorReviveCondition)]
    public class ActorReviveCondition : PassiveCondition
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
            this.sourceActor.handle.ActorControl.eventActorRevive += new ActorEventHandler(this.onActorRevive);
        }

        private void onActorRevive(ref DefaultGameEventParam _prm)
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
            if (base.sourceActor != 0)
            {
                this.sourceActor.handle.ActorControl.eventActorRevive -= new ActorEventHandler(this.onActorRevive);
            }
        }
    }
}

