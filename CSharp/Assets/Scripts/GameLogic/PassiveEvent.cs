namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    public class PassiveEvent
    {
        protected ResSkillPassiveCfgInfo cfgData;
        protected ListView<PassiveCondition> conditions = new ListView<PassiveCondition>();
        protected int deltaTime;
        protected int[] localParams = new int[5];
        private const int MAX_EVENT_PARAM = 5;
        protected PassiveSkill passiveSkill;
        protected PoolObjHandle<ActorRoot> sourceActor;
        protected PoolObjHandle<ActorRoot> triggerActor = new PoolObjHandle<ActorRoot>(null);

        public void AddCondition(PassiveCondition _condition)
        {
            this.conditions.Add(_condition);
        }

        public bool ChangeEventParam(int _index, int _value)
        {
            if ((_index < 0) && (_index >= 5))
            {
                return false;
            }
            this.localParams[_index] = _value;
            return true;
        }

        protected bool Fit()
        {
            if ((this.conditions.Count == 0) && (this.deltaTime <= 0))
            {
                return true;
            }
            for (int i = 0; i < this.conditions.Count; i++)
            {
                PassiveCondition condition = this.conditions[i];
                if (condition.Fit() && (this.deltaTime <= 0))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual int GetCDTime()
        {
            return this.deltaTime;
        }

        public virtual void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
        {
            this.sourceActor = _actor;
            this.triggerActor.Release();
            this.passiveSkill = _skill;
            this.cfgData = _skill.cfgData;
            this.deltaTime = 0;
            for (int i = 0; i < this.conditions.Count; i++)
            {
                this.conditions[i].Init(this.sourceActor, this, ref this.cfgData.astPassiveConditon[i]);
            }
            this.SetEventParam();
        }

        public virtual void InitCDTime(int _cdTime)
        {
            this.deltaTime = _cdTime;
        }

        protected void Reset()
        {
            for (int i = 0; i < this.conditions.Count; i++)
            {
                this.conditions[i].Reset();
            }
        }

        private void SetEventParam()
        {
            this.localParams[0] = this.cfgData.iPassiveEventParam1;
            this.localParams[1] = this.cfgData.iPassiveEventParam2;
            this.localParams[2] = this.cfgData.iPassiveEventParam3;
            this.localParams[3] = this.cfgData.iPassiveEventParam4;
            this.localParams[4] = this.cfgData.iPassiveEventParam5;
        }

        public void SetTriggerActor(PoolObjHandle<ActorRoot> _actor)
        {
            this.triggerActor = _actor;
        }

        protected void Trigger()
        {
            SkillUseParam param = new SkillUseParam();
            param.Init(this.passiveSkill.SlotType);
            param.SetOriginator(this.sourceActor);
            if (this.triggerActor == 0)
            {
                param.TargetActor = this.sourceActor;
            }
            else
            {
                param.TargetActor = this.triggerActor;
            }
            this.passiveSkill.Use(this.sourceActor, ref param);
            this.deltaTime = this.cfgData.iCoolDown;
        }

        public virtual void UnInit()
        {
            for (int i = 0; i < this.conditions.Count; i++)
            {
                this.conditions[i].UnInit();
            }
        }

        public virtual void UpdateLogic(int _delta)
        {
            if (this.deltaTime > 0)
            {
                this.deltaTime -= _delta;
                this.deltaTime = (this.deltaTime <= 0) ? 0 : this.deltaTime;
            }
        }
    }
}

