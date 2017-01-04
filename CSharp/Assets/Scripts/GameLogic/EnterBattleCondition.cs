namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.EnterBattlePassiveCondition)]
    public class EnterBattleCondition : PassiveCondition
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
            this.sourceActor.handle.ActorControl.eventActorEnterCombat += new ActorEventHandler(this.onActorEnterBattle);
            this.sourceActor.handle.ActorControl.eventActorExitCombat += new ActorEventHandler(this.onActorExitBattle);
        }

        private void onActorEnterBattle(ref DefaultGameEventParam _prm)
        {
            if (_prm.src == base.sourceActor)
            {
                this.bTrigger = true;
            }
        }

        private void onActorExitBattle(ref DefaultGameEventParam _prm)
        {
            if (_prm.src == base.sourceActor)
            {
                this.bTrigger = false;
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
                this.sourceActor.handle.ActorControl.eventActorEnterCombat -= new ActorEventHandler(this.onActorEnterBattle);
                this.sourceActor.handle.ActorControl.eventActorExitCombat -= new ActorEventHandler(this.onActorExitBattle);
            }
        }
    }
}

