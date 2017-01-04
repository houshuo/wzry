namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveCondition(PassiveConditionType.DamagePassiveCondition)]
    public class PassiveHurtCondition : PassiveCondition
    {
        private bool bHurt;

        private bool CheckAttackType(ref HurtEventResultInfo info)
        {
            bool flag = false;
            if (base.localParams[0] == 0)
            {
                return true;
            }
            if (base.localParams[0] == 1)
            {
                if ((info.atker != 0) && (info.atker.handle.TheStaticData.TheHeroOnlyInfo.AttackDistanceType == 1))
                {
                    flag = this.CheckSkillSlot(ref info);
                }
                return flag;
            }
            if (((base.localParams[0] == 2) && (info.atker != 0)) && (info.atker.handle.TheStaticData.TheHeroOnlyInfo.AttackDistanceType == 2))
            {
                flag = this.CheckSkillSlot(ref info);
            }
            return flag;
        }

        private bool CheckDamageRateForHp(ref HurtEventResultInfo info)
        {
            bool flag = false;
            int num = base.localParams[3];
            if (num <= 0)
            {
                return true;
            }
            if ((info.src != 0) && (info.src.handle.ValueComponent != null))
            {
                int num2 = -info.hpChanged;
                if (num2 > ((num * (info.src.handle.ValueComponent.actorHp + num2)) / 100))
                {
                    flag = true;
                }
            }
            return flag;
        }

        private bool CheckSkillSlot(ref HurtEventResultInfo info)
        {
            bool flag = false;
            if (base.localParams[1] == info.hurtInfo.atkSlot)
            {
                flag = true;
            }
            return flag;
        }

        public override bool Fit()
        {
            if (this.sourceActor.handle.ActorControl.IsDeadState)
            {
                return false;
            }
            return this.bHurt;
        }

        public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
        {
            this.bHurt = false;
            base.Init(_source, _event, ref _config);
            Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
        }

        private void onActorDamage(ref HurtEventResultInfo info)
        {
            if ((((info.src == base.sourceActor) && (info.hpChanged < 0)) && (!info.hurtInfo.bBounceHurt || (base.localParams[2] != 1))) && (this.CheckDamageRateForHp(ref info) && this.CheckAttackType(ref info)))
            {
                this.bHurt = true;
                base.rootEvent.SetTriggerActor(info.atker);
            }
        }

        public override void Reset()
        {
            this.bHurt = false;
        }

        public override void UnInit()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
        }
    }
}

