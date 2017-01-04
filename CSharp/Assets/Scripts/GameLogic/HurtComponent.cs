namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;

    public class HurtComponent : LogicComponent
    {
        private ResBattleParam battleParam;

        public int CommonDamagePart(ref HurtDataInfo hurt)
        {
            int num = 0;
            num = (((((hurt.hurtValue + ((hurt.adValue * hurt.attackInfo.iActorATT) / 0x2710)) + ((hurt.apValue * hurt.attackInfo.iActorINT) / 0x2710)) + ((hurt.hpValue * hurt.attackInfo.iActorMaxHp) / 0x2710)) + this.GetExtraHurtValue(ref hurt)) * this.GetBaseHurtValue(ref hurt)) / this.GetBaseHurtRate(ref hurt);
            if ((hurt.atkSlot == SkillSlotType.SLOT_SKILL_0) && (hurt.atker != 0))
            {
                num += hurt.atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_BASEHURTADD].totalValue;
            }
            return num;
        }

        private int CriticalDamagePart(ref HurtDataInfo hurt)
        {
            bool flag = false;
            int num = 0;
            if ((hurt.atkSlot == SkillSlotType.SLOT_SKILL_0) && (hurt.iCanSkillCrit != 0))
            {
                int num2 = (int) ((hurt.attackInfo.iCritStrikeValue + (hurt.attackInfo.iActorLvl * this.battleParam.dwM_Critical)) + this.battleParam.dwN_Critical);
                int num3 = 0;
                int num4 = 0;
                if (num2 > 0)
                {
                    num3 = ((hurt.attackInfo.iCritStrikeValue * 0x2710) / num2) + hurt.attackInfo.iCritStrikeRate;
                }
                int num5 = (int) ((hurt.attackInfo.iReduceCritStrikeValue + (base.actor.ValueComponent.mActorValue.actorLvl * this.battleParam.dwM_ReduceCritical)) + this.battleParam.dwN_ReduceCritical);
                if (num5 > 0)
                {
                    num4 = (hurt.attackInfo.iReduceCritStrikeValue * 0x2710) / num5;
                    num4 += hurt.attackInfo.iReduceCritStrikeRate;
                }
                if (!base.actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneCrit))
                {
                    flag = FrameRandom.Random(0x2710) < (num3 - num4);
                }
                num = !flag ? 0 : hurt.attackInfo.iCritStrikeEff;
                if (flag)
                {
                    DefaultGameEventParam prm = new DefaultGameEventParam(hurt.atker, hurt.target);
                    Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorCrit, ref prm);
                }
            }
            return num;
        }

        private int GetBaseHurtRate(ref HurtDataInfo hurt)
        {
            int num = 1;
            if (hurt.extraHurtType != ExtraHurtTypeDef.ExtraHurt_Value)
            {
                num = 100;
            }
            return num;
        }

        private int GetBaseHurtValue(ref HurtDataInfo hurt)
        {
            int totalValue = hurt.target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
            int actorHp = hurt.target.handle.ValueComponent.actorHp;
            switch (hurt.extraHurtType)
            {
                case ExtraHurtTypeDef.ExtraHurt_Value:
                    return 1;

                case ExtraHurtTypeDef.ExtraHurt_MaxHp:
                    return totalValue;

                case ExtraHurtTypeDef.ExtraHurt_CurHp:
                    return (totalValue - actorHp);

                case ExtraHurtTypeDef.ExtraHurt_LoseHp:
                    return actorHp;
            }
            return 1;
        }

        private int GetExtraHurtValue(ref HurtDataInfo hurt)
        {
            int totalValue = hurt.target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
            int actorHp = hurt.target.handle.ValueComponent.actorHp;
            return ((hurt.loseHpValue * (totalValue - actorHp)) / 0x2710);
        }

        private int Hemophagia(ref HurtDataInfo hurt, int hurtValue)
        {
            int nAddHp = 0;
            if (hurt.atker != 0)
            {
                if (hurt.hurtType == HurtTypeDef.PhysHurt)
                {
                    int num2 = 0;
                    int num3 = (int) ((hurt.attackInfo.iPhysicsHemophagia + (hurt.attackInfo.iActorLvl * this.battleParam.dwM_PhysicsHemophagia)) + this.battleParam.dwN_PhysicsHemophagia);
                    if (num3 > 0)
                    {
                        num2 = (hurt.attackInfo.iPhysicsHemophagia * 0x2710) / num3;
                    }
                    nAddHp = (hurtValue * (num2 + hurt.attackInfo.iPhysicsHemophagiaRate)) / 0x2710;
                    nAddHp = (nAddHp * hurt.hemoFadeRate) / 0x2710;
                }
                else if (hurt.hurtType == HurtTypeDef.MagicHurt)
                {
                    int num4 = 0;
                    int num5 = (int) ((hurt.attackInfo.iMagicHemophagia + (hurt.attackInfo.iActorLvl * this.battleParam.dwM_MagicHemophagia)) + this.battleParam.dwN_MagicHemophagia);
                    if (num5 > 0)
                    {
                        num4 = (hurt.attackInfo.iMagicHemophagia * 0x2710) / num5;
                    }
                    nAddHp = (hurtValue * (num4 + hurt.attackInfo.iMagicHemophagiaRate)) / 0x2710;
                    nAddHp = (nAddHp * hurt.hemoFadeRate) / 0x2710;
                }
                if (nAddHp > 0)
                {
                    int actorHp = hurt.atker.handle.ValueComponent.actorHp;
                    hurt.atker.handle.ActorControl.ReviveHp(nAddHp);
                    actorHp = hurt.atker.handle.ValueComponent.actorHp - actorHp;
                    HemophagiaEventResultInfo prm = new HemophagiaEventResultInfo(hurt.atker, actorHp);
                    Singleton<GameEventSys>.instance.SendEvent<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, ref prm);
                }
            }
            return nAddHp;
        }

        public bool ImmuneDamage(ref HurtDataInfo hurt)
        {
            if (((hurt.atkSlot == SkillSlotType.SLOT_SKILL_0) && (hurt.atker != 0)) && ((hurt.atker.handle.ActorControl != null) && hurt.atker.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Blindness)))
            {
                ActorSkillEventParam param = new ActorSkillEventParam(hurt.target, SkillSlotType.SLOT_SKILL_0);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.AllEvent_Blindess, hurt.target, ref param, GameSkillEventChannel.Channel_AllActor);
                return true;
            }
            return ((hurt.target != 0) && hurt.target.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative));
        }

        public override void Init()
        {
            base.Init();
            this.battleParam = GameDataMgr.battleParam.GetAnyData();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.battleParam = null;
        }

        private int ReduceDamagePart(ref HurtDataInfo hurt, HurtTypeDef hurtType)
        {
            int num = 0;
            if (hurtType == HurtTypeDef.PhysHurt)
            {
                int num2 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue - hurt.attackInfo.iDEFStrike;
                num2 = (num2 * (0x2710 - hurt.attackInfo.iDEFStrikeRate)) / 0x2710;
                num2 = (num2 <= 0) ? 0 : num2;
                int num3 = (int) ((num2 + (this.battleParam.dwM_PhysicsDefend * base.actor.ValueComponent.mActorValue.actorLvl)) + this.battleParam.dwN_PhysicsDefend);
                if (num3 != 0)
                {
                    num = (num2 * 0x2710) / num3;
                }
                return num;
            }
            if (hurtType == HurtTypeDef.MagicHurt)
            {
                int num4 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue - hurt.attackInfo.iRESStrike;
                num4 = (num4 * (0x2710 - hurt.attackInfo.iRESStrikeRate)) / 0x2710;
                num4 = (num4 <= 0) ? 0 : num4;
                int num5 = (int) ((num4 + (this.battleParam.dwM_MagicDefend * base.actor.ValueComponent.mActorValue.actorLvl)) + this.battleParam.dwN_MagicDefend);
                if (num5 != 0)
                {
                    num = (num4 * 0x2710) / num5;
                }
            }
            return num;
        }

        public int TakeDamage(ref HurtDataInfo hurt)
        {
            int nAddHp = 0;
            int actorHp = 0;
            int num3 = this.CriticalDamagePart(ref hurt);
            int num4 = 0x26ac + FrameRandom.Random(200);
            base.actor.BuffHolderComp.OnDamageExtraValueEffect(ref hurt, hurt.atker, hurt.atkSlot);
            if (hurt.hurtType == HurtTypeDef.Therapic)
            {
                nAddHp = (this.CommonDamagePart(ref hurt) * num4) / 0x2710;
                nAddHp = (nAddHp * hurt.iOverlayFadeRate) / 0x2710;
                if (hurt.iAddTotalHurtType == 1)
                {
                    nAddHp += (nAddHp * hurt.iAddTotalHurtValue) / 0x2710;
                }
                else
                {
                    nAddHp += hurt.iAddTotalHurtValue;
                }
                actorHp = base.actor.ValueComponent.actorHp;
                base.actor.ActorControl.ReviveHp(nAddHp);
                actorHp = base.actor.ValueComponent.actorHp - actorHp;
                if (((hurt.atker != 0) && (base.actor.TheActorMeta.ActorCamp == hurt.atker.handle.TheActorMeta.ActorCamp)) && (base.actor.ValueComponent.actorHp > 0))
                {
                    base.actor.ActorControl.AddHelpSelfActor(hurt.atker);
                }
            }
            else
            {
                if (this.ImmuneDamage(ref hurt))
                {
                    return 0;
                }
                bool flag = (hurt.atker != 0) && hurt.atker.handle.bOneKiller;
                int num5 = base.actor.ValueComponent.actorHp;
                nAddHp = (((((((this.CommonDamagePart(ref hurt) * (0x2710 - this.ReduceDamagePart(ref hurt, hurt.hurtType))) / 0x2710) * (0x2710 + num3)) / 0x2710) * num4) / 0x2710) + hurt.attackInfo.iFinalHurt) - base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURTLESS].totalValue;
                nAddHp = (nAddHp * (0x2710 - base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_HURTREDUCERATE].totalValue)) / 0x2710;
                int extraHurtOutputRate = base.actor.BuffHolderComp.GetExtraHurtOutputRate(hurt.atker);
                nAddHp = ((nAddHp * ((0x2710 + hurt.attackInfo.iHurtOutputRate) + extraHurtOutputRate)) / 0x2710) * hurt.gatherTime;
                if (nAddHp < 0)
                {
                    nAddHp = 0;
                }
                if (hurt.atker != 0)
                {
                    nAddHp = (nAddHp * Singleton<BattleLogic>.GetInstance().clashAddition.CalcDamageAddition(hurt.atker.handle.TheStaticData.TheBaseAttribute.ClashMark, base.actor.TheStaticData.TheBaseAttribute.ClashMark)) / 0x2710;
                }
                nAddHp = base.actor.BuffHolderComp.OnDamage(ref hurt, nAddHp);
                if (((hurt.atker != 0) && (hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)) && ((hurt.atker.handle.TheStaticData.TheHeroOnlyInfo.AttackDistanceType == 2) && (hurt.iLongRangeReduction > 0)))
                {
                    nAddHp = (nAddHp * hurt.iLongRangeReduction) / 0x2710;
                }
                int iDamageLimit = hurt.iDamageLimit;
                if ((hurt.target != 0) && (hurt.target.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster))
                {
                    if ((iDamageLimit > 0) && (hurt.iMonsterDamageLimit > 0))
                    {
                        iDamageLimit = (iDamageLimit >= hurt.iMonsterDamageLimit) ? hurt.iMonsterDamageLimit : iDamageLimit;
                    }
                    else if (hurt.iMonsterDamageLimit > 0)
                    {
                        iDamageLimit = hurt.iMonsterDamageLimit;
                    }
                }
                if (hurt.iReduceDamage > 0)
                {
                    nAddHp -= hurt.iReduceDamage;
                    nAddHp = (nAddHp >= 0) ? nAddHp : 0;
                }
                if (iDamageLimit > 0)
                {
                    nAddHp = (nAddHp >= iDamageLimit) ? iDamageLimit : nAddHp;
                }
                if ((hurt.target != 0) && (hurt.target.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ))
                {
                    this.Hemophagia(ref hurt, nAddHp);
                }
                actorHp = base.actor.ValueComponent.actorHp;
                base.actor.ValueComponent.actorHp -= nAddHp;
                actorHp = base.actor.ValueComponent.actorHp - actorHp;
                if (flag)
                {
                    base.actor.ValueComponent.actorHp = 0;
                    actorHp = num5 * -1;
                }
                if (((hurt.atker != 0) && (base.actor.TheActorMeta.ActorCamp != hurt.atker.handle.TheActorMeta.ActorCamp)) && (base.actor.ValueComponent.actorHp > 0))
                {
                    base.actor.ActorControl.AddHurtSelfActor(hurt.atker);
                }
            }
            HurtEventResultInfo prm = new HurtEventResultInfo(base.GetActor(), hurt.atker, hurt, nAddHp, actorHp, num3);
            Singleton<GameEventSys>.instance.SendEvent<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, ref prm);
            return actorHp;
        }
    }
}

