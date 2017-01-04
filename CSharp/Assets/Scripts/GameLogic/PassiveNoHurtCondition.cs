namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.NoDamagePassiveCondition)]
    public class PassiveNoHurtCondition : PassiveCondition
    {
        private bool bNoHurt = true;

        public override bool Fit()
        {
            if (this.sourceActor.handle.ActorControl.IsDeadState)
            {
                return false;
            }
            return this.bNoHurt;
        }

        public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
        {
            this.bNoHurt = true;
            base.Init(_source, _event, ref _config);
            Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
        }

        private void onActorDamage(ref HurtEventResultInfo info)
        {
            if ((info.src == base.sourceActor) && (info.hpChanged < 0))
            {
                this.bNoHurt = false;
            }
        }

        public override void Reset()
        {
            this.bNoHurt = true;
        }

        public override void UnInit()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
        }
    }
}

