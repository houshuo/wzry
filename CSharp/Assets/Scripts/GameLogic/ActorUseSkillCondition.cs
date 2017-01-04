namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.UseSkillCondition)]
    public class ActorUseSkillCondition : PassiveCondition
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
            Singleton<GameSkillEventSys>.instance.AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onActorUseSkill));
        }

        private void onActorUseSkill(ref ActorSkillEventParam _prm)
        {
            if (_prm.src == base.sourceActor)
            {
                if (base.localParams[0] == 0)
                {
                    this.bTrigger = true;
                }
                else if ((base.localParams[0] & (((int) 1) << _prm.slot)) > 0)
                {
                    this.bTrigger = true;
                }
            }
        }

        public override void Reset()
        {
            this.bTrigger = false;
        }

        public override void UnInit()
        {
            Singleton<GameSkillEventSys>.instance.RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onActorUseSkill));
        }
    }
}

