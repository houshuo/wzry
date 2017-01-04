namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.HpPassiveCondition)]
    public class PassiveHpCondition : PassiveCondition
    {
        private bool bHpTrigger;

        public override bool Fit()
        {
            return this.bHpTrigger;
        }

        public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
        {
            this.bHpTrigger = false;
            base.Init(_source, _event, ref _config);
            if (_source != 0)
            {
                _source.handle.ValueComponent.HpChgEvent += new ValueChangeDelegate(this.OnHpChange);
            }
        }

        private void OnHpChange()
        {
            int actorHp = this.sourceActor.handle.ValueComponent.actorHp;
            int totalValue = this.sourceActor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
            int num3 = (actorHp * 0x2710) / totalValue;
            this.bHpTrigger = false;
            if (base.localParams[0] == 1)
            {
                if (num3 <= base.localParams[1])
                {
                    this.bHpTrigger = true;
                }
            }
            else if ((base.localParams[0] == 4) && (num3 >= base.localParams[1]))
            {
                this.bHpTrigger = true;
            }
        }

        public override void Reset()
        {
            this.bHpTrigger = false;
        }

        public override void UnInit()
        {
            if (base.sourceActor != 0)
            {
                this.sourceActor.handle.ValueComponent.HpChgEvent -= new ValueChangeDelegate(this.OnHpChange);
            }
        }
    }
}

