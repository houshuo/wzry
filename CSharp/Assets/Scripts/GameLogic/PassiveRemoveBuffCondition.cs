namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.RemoveBuffPassiveCondition)]
    public class PassiveRemoveBuffCondition : PassiveCondition
    {
        private bool bTrigger;

        public override bool Fit()
        {
            return this.bTrigger;
        }

        public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
        {
            this.bTrigger = true;
            base.Init(_source, _event, ref _config);
            if (((_source != 0) && (_source.handle.BuffHolderComp != null)) && (_source.handle.BuffHolderComp.SpawnedBuffList != null))
            {
                for (int i = 0; i < _source.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    BuffSkill skill = _source.handle.BuffHolderComp.SpawnedBuffList[i];
                    if (((skill != null) && (skill.cfgData != null)) && (skill.cfgData.iCfgID == base.localParams[0]))
                    {
                        this.bTrigger = false;
                        break;
                    }
                }
            }
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnPlayerBuffChange));
        }

        private void OnPlayerBuffChange(ref BuffChangeEventParam prm)
        {
            if (((prm.target == base.sourceActor) && (prm.stBuffSkill != 0)) && ((prm.stBuffSkill.handle.cfgData != null) && (prm.stBuffSkill.handle.cfgData.iCfgID == base.localParams[0])))
            {
                if (prm.bIsAdd)
                {
                    this.bTrigger = false;
                }
                else
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
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnPlayerBuffChange));
        }
    }
}

