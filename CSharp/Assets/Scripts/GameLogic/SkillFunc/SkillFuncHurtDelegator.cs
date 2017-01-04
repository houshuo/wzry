namespace Assets.Scripts.GameLogic.SkillFunc
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [SkillFuncHandlerClass]
    public class SkillFuncHurtDelegator
    {
        private static int GetEffectFadeRate(ref SSkillFuncContext inContext)
        {
            int iNextDeltaFadeRate = inContext.inBuffSkill.handle.cfgData.iNextDeltaFadeRate;
            int iNextLowFadeRate = inContext.inBuffSkill.handle.cfgData.iNextLowFadeRate;
            int num3 = 0x2710 - ((inContext.inEffectCount - 1) * iNextDeltaFadeRate);
            return ((num3 >= iNextLowFadeRate) ? num3 : iNextLowFadeRate);
        }

        private static int GetOverlayFadeRate(ref SSkillFuncContext inContext)
        {
            PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
            int num = 0x2710 - inContext.inBuffSkill.handle.cfgData.iOverlayFadeRate;
            if ((num != 0x2710) && (((num < 0x2710) && (num >= 0)) && ((inTargetObj != 0) && (inTargetObj.handle.BuffHolderComp != null))))
            {
                int skillID = inContext.inBuffSkill.handle.SkillID;
                if ((inContext.inBuffSkill != 0) && inContext.inBuffSkill.handle.bFirstEffect)
                {
                    inContext.inBuffSkill.handle.bFirstEffect = false;
                    return 0x2710;
                }
                if (inTargetObj.handle.BuffHolderComp.FindBuffCount(skillID) > 1)
                {
                    return num;
                }
            }
            return 0x2710;
        }

        private static int GetSkillFuncProtectValue(ref SSkillFuncContext inContext)
        {
            int num = 0;
            if (inContext.inStage == ESkillFuncStage.Enter)
            {
                PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
                PoolObjHandle<ActorRoot> inOriginator = inContext.inOriginator;
                if ((inTargetObj != 0) && (inOriginator != 0))
                {
                    HurtDataInfo info;
                    inContext.inCustomData = new HurtAttackerInfo();
                    inContext.inCustomData.Init(inOriginator, inTargetObj);
                    info.atker = inOriginator;
                    info.atkerName = inOriginator.handle.name;
                    info.target = inTargetObj;
                    info.attackInfo = inContext.inCustomData;
                    info.atkSlot = inContext.inUseContext.SlotType;
                    info.hurtType = HurtTypeDef.PhysHurt;
                    info.extraHurtType = ExtraHurtTypeDef.ExtraHurt_Value;
                    info.hurtValue = inContext.GetSkillFuncParam(1, true);
                    info.adValue = inContext.GetSkillFuncParam(2, true);
                    info.apValue = inContext.GetSkillFuncParam(3, true);
                    info.hpValue = inContext.GetSkillFuncParam(4, true);
                    info.loseHpValue = 0;
                    info.hurtCount = inContext.inDoCount;
                    info.hemoFadeRate = 0x2710;
                    info.bExtraBuff = false;
                    info.gatherTime = inContext.inUseContext.GatherTime;
                    info.bBounceHurt = false;
                    info.bLastHurt = inContext.inLastEffect;
                    info.iAddTotalHurtType = 0;
                    info.iAddTotalHurtValue = 0;
                    info.iCanSkillCrit = inContext.inBuffSkill.handle.cfgData.iCanSkillCrit;
                    info.iDamageLimit = inContext.inBuffSkill.handle.cfgData.iDamageLimit;
                    info.iMonsterDamageLimit = inContext.inBuffSkill.handle.cfgData.iMonsterDamageLimit;
                    info.iLongRangeReduction = inContext.inBuffSkill.handle.cfgData.iLongRangeReduction;
                    info.iEffectiveTargetType = inContext.inBuffSkill.handle.cfgData.iEffectiveTargetType;
                    info.iOverlayFadeRate = 0x2710;
                    info.iEffectFadeRate = 0x2710;
                    info.iReduceDamage = 0;
                    num = inTargetObj.handle.ActorControl.actor.HurtControl.CommonDamagePart(ref info);
                }
            }
            return num;
        }

        private static bool HandleSkillFuncHurt(ref SSkillFuncContext inContext, HurtTypeDef hurtType)
        {
            int num = 0;
            if (inContext.inStage == ESkillFuncStage.Enter)
            {
                PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
                PoolObjHandle<ActorRoot> inOriginator = inContext.inOriginator;
                if ((inTargetObj != 0) && !inTargetObj.handle.ActorControl.IsDeadState)
                {
                    HurtDataInfo info;
                    bool flag = false;
                    inContext.inCustomData = new HurtAttackerInfo();
                    inContext.inCustomData.Init(inOriginator, inTargetObj);
                    info.atker = inOriginator;
                    info.atkerName = "HurtOrHealAttacker";
                    if (inOriginator != 0)
                    {
                        info.atkerName = inOriginator.handle.name;
                    }
                    info.target = inTargetObj;
                    info.attackInfo = inContext.inCustomData;
                    info.atkSlot = inContext.inUseContext.SlotType;
                    info.hurtType = hurtType;
                    info.extraHurtType = (ExtraHurtTypeDef) inContext.GetSkillFuncParam(0, false);
                    info.hurtValue = inContext.GetSkillFuncParam(1, true);
                    info.adValue = inContext.GetSkillFuncParam(2, true);
                    info.apValue = inContext.GetSkillFuncParam(3, true);
                    info.hpValue = inContext.GetSkillFuncParam(4, true);
                    info.loseHpValue = inContext.GetSkillFuncParam(5, true);
                    info.hurtCount = inContext.inDoCount;
                    info.hemoFadeRate = inContext.inBuffSkill.handle.cfgData.iEffectFadeRate;
                    info.bExtraBuff = inContext.inBuffSkill.handle.bExtraBuff;
                    info.gatherTime = inContext.inUseContext.GatherTime;
                    flag = inContext.inBuffSkill.handle.cfgData.dwEffectSubType == 9;
                    info.bBounceHurt = flag;
                    info.bLastHurt = inContext.inLastEffect;
                    info.iAddTotalHurtType = 0;
                    info.iAddTotalHurtValue = 0;
                    info.iCanSkillCrit = inContext.inBuffSkill.handle.cfgData.iCanSkillCrit;
                    info.iDamageLimit = inContext.inBuffSkill.handle.cfgData.iDamageLimit;
                    info.iMonsterDamageLimit = inContext.inBuffSkill.handle.cfgData.iMonsterDamageLimit;
                    info.iLongRangeReduction = inContext.inBuffSkill.handle.cfgData.iLongRangeReduction;
                    info.iEffectiveTargetType = inContext.inBuffSkill.handle.cfgData.iEffectiveTargetType;
                    info.iOverlayFadeRate = GetOverlayFadeRate(ref inContext);
                    info.iEffectFadeRate = GetEffectFadeRate(ref inContext);
                    info.iReduceDamage = 0;
                    num = inTargetObj.handle.ActorControl.TakeDamage(ref info);
                    inContext.inAction.handle.refParams.AddRefParam("HurtValue", -num);
                }
            }
            else if (inContext.inStage == ESkillFuncStage.Update)
            {
                PoolObjHandle<ActorRoot> handle3 = inContext.inTargetObj;
                PoolObjHandle<ActorRoot> handle4 = inContext.inOriginator;
                if ((handle3 != 0) && !handle3.handle.ActorControl.IsDeadState)
                {
                    HurtDataInfo info2;
                    bool flag2 = false;
                    info2.atker = handle4;
                    info2.atkerName = (handle4 == 0) ? string.Empty : handle4.handle.name;
                    info2.target = handle3;
                    info2.attackInfo = inContext.inCustomData;
                    info2.atkSlot = inContext.inUseContext.SlotType;
                    info2.hurtType = hurtType;
                    info2.extraHurtType = (ExtraHurtTypeDef) inContext.GetSkillFuncParam(0, false);
                    info2.hurtValue = inContext.GetSkillFuncParam(1, true);
                    info2.adValue = inContext.GetSkillFuncParam(2, true);
                    info2.apValue = inContext.GetSkillFuncParam(3, true);
                    info2.hpValue = inContext.GetSkillFuncParam(4, true);
                    info2.loseHpValue = inContext.GetSkillFuncParam(5, true);
                    info2.hurtCount = inContext.inDoCount;
                    info2.hemoFadeRate = inContext.inBuffSkill.handle.cfgData.iEffectFadeRate;
                    info2.bExtraBuff = inContext.inBuffSkill.handle.bExtraBuff;
                    info2.gatherTime = inContext.inUseContext.GatherTime;
                    flag2 = inContext.inBuffSkill.handle.cfgData.dwEffectSubType == 9;
                    info2.bBounceHurt = flag2;
                    info2.bLastHurt = inContext.inLastEffect;
                    info2.iAddTotalHurtType = 0;
                    info2.iAddTotalHurtValue = 0;
                    info2.iCanSkillCrit = inContext.inBuffSkill.handle.cfgData.iCanSkillCrit;
                    info2.iDamageLimit = inContext.inBuffSkill.handle.cfgData.iDamageLimit;
                    info2.iMonsterDamageLimit = inContext.inBuffSkill.handle.cfgData.iMonsterDamageLimit;
                    info2.iLongRangeReduction = inContext.inBuffSkill.handle.cfgData.iLongRangeReduction;
                    info2.iEffectiveTargetType = inContext.inBuffSkill.handle.cfgData.iEffectiveTargetType;
                    info2.iOverlayFadeRate = GetOverlayFadeRate(ref inContext);
                    info2.iEffectFadeRate = GetEffectFadeRate(ref inContext);
                    info2.iReduceDamage = 0;
                    num = handle3.handle.ActorControl.TakeDamage(ref info2);
                    inContext.inAction.handle.refParams.AddRefParam("HurtValue", -num);
                }
            }
            return (num != 0);
        }

        [SkillFuncHandler(50, new int[] {  })]
        public static bool OnSkillFuncAddExp(ref SSkillFuncContext inContext)
        {
            if (inContext.inStage != ESkillFuncStage.Leave)
            {
                PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
                if (inTargetObj != 0)
                {
                    int skillFuncParam = inContext.GetSkillFuncParam(0, false);
                    inTargetObj.handle.ValueComponent.AddSoulExp(skillFuncParam, true, AddSoulType.SkillFunc);
                }
            }
            return true;
        }

        [SkillFuncHandler(3, new int[] {  })]
        public static bool OnSkillFuncAddHp(ref SSkillFuncContext inContext)
        {
            return ((inContext.inStage != ESkillFuncStage.Leave) && HandleSkillFuncHurt(ref inContext, HurtTypeDef.Therapic));
        }

        [SkillFuncHandler(0x45, new int[] {  })]
        public static bool OnSkillFuncBlindess(ref SSkillFuncContext inContext)
        {
            PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
            if (inTargetObj != 0)
            {
                if (inContext.inStage == ESkillFuncStage.Enter)
                {
                    inTargetObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Blindness);
                }
                else if (inContext.inStage == ESkillFuncStage.Leave)
                {
                    inTargetObj.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Blindness);
                }
            }
            return true;
        }

        [SkillFuncHandler(0x3d, new int[] {  })]
        public static bool OnSkillFuncCommonAtkWithMagicHurt(ref SSkillFuncContext inContext)
        {
            return true;
        }

        [SkillFuncHandler(0x2c, new int[] {  })]
        public static bool OnSkillFuncConditionHurtOut(ref SSkillFuncContext inContext)
        {
            return true;
        }

        [SkillFuncHandler(0x43, new int[] {  })]
        public static bool OnSkillFuncDecHurtRate(ref SSkillFuncContext inContext)
        {
            return true;
        }

        [SkillFuncHandler(0x21, new int[] {  })]
        public static bool OnSkillFuncExtraEffect(ref SSkillFuncContext inContext)
        {
            return true;
        }

        [SkillFuncHandler(0x44, new int[] {  })]
        public static bool OnSkillFuncExtraHurtWithLowHp(ref SSkillFuncContext inContext)
        {
            return true;
        }

        [SkillFuncHandler(0x34, new int[] {  })]
        public static bool OnSkillFuncImmuneCrit(ref SSkillFuncContext inContext)
        {
            PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
            if (inTargetObj != 0)
            {
                if (inContext.inStage == ESkillFuncStage.Enter)
                {
                    inTargetObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneCrit);
                }
                else if (inContext.inStage == ESkillFuncStage.Leave)
                {
                    inTargetObj.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneCrit);
                }
            }
            return true;
        }

        [SkillFuncHandler(0x35, new int[] {  })]
        public static bool OnSkillFuncLimiteMaxHurt(ref SSkillFuncContext inContext)
        {
            PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
            if (inTargetObj != 0)
            {
                if (inContext.inStage == ESkillFuncStage.Enter)
                {
                    int skillFuncParam = inContext.GetSkillFuncParam(0, false);
                    inTargetObj.handle.BuffHolderComp.protectRule.SetLimiteMaxHurt(true, skillFuncParam);
                }
                else if (inContext.inStage == ESkillFuncStage.Leave)
                {
                    inTargetObj.handle.BuffHolderComp.protectRule.SetLimiteMaxHurt(false, 0);
                }
            }
            return true;
        }

        [SkillFuncHandler(1, new int[] {  })]
        public static bool OnSkillFuncMagicHurt(ref SSkillFuncContext inContext)
        {
            return ((inContext.inStage != ESkillFuncStage.Leave) && HandleSkillFuncHurt(ref inContext, HurtTypeDef.MagicHurt));
        }

        [SkillFuncHandler(0, new int[] {  })]
        public static bool OnSkillFuncPhysHurt(ref SSkillFuncContext inContext)
        {
            return ((inContext.inStage != ESkillFuncStage.Leave) && HandleSkillFuncHurt(ref inContext, HurtTypeDef.PhysHurt));
        }

        [SkillFuncHandler(0x1b, new int[] {  })]
        public static bool OnSkillFuncProtect(ref SSkillFuncContext inContext)
        {
            if (inContext.inStage != ESkillFuncStage.Enter)
            {
                return false;
            }
            int skillFuncProtectValue = GetSkillFuncProtectValue(ref inContext);
            int skillFuncParam = inContext.GetSkillFuncParam(0, false);
            int num3 = inContext.GetSkillFuncParam(5, false);
            switch (skillFuncParam)
            {
                case 1:
                    inContext.inBuffSkill.handle.CustomParams[0] += skillFuncProtectValue;
                    SendProtectEvent(ref inContext, 0, skillFuncProtectValue);
                    break;

                case 2:
                    inContext.inBuffSkill.handle.CustomParams[1] += skillFuncProtectValue;
                    SendProtectEvent(ref inContext, 1, skillFuncProtectValue);
                    break;

                case 3:
                    inContext.inBuffSkill.handle.CustomParams[2] += skillFuncProtectValue;
                    SendProtectEvent(ref inContext, 2, skillFuncProtectValue);
                    break;
            }
            inContext.inBuffSkill.handle.CustomParams[3] = num3;
            return true;
        }

        [SkillFuncHandler(2, new int[] {  })]
        public static bool OnSkillFuncRealHurt(ref SSkillFuncContext inContext)
        {
            return ((inContext.inStage != ESkillFuncStage.Leave) && HandleSkillFuncHurt(ref inContext, HurtTypeDef.RealHurt));
        }

        [SkillFuncHandler(10, new int[] {  })]
        public static bool OnSkillFuncSuckBlood(ref SSkillFuncContext inContext)
        {
            PoolObjHandle<ActorRoot> inOriginator = inContext.inOriginator;
            if (inOriginator == 0)
            {
                return false;
            }
            if (inContext.inStage != ESkillFuncStage.Leave)
            {
                int num = 0;
                if (inContext.inAction.handle.refParams.GetRefParam("HurtValue", ref num))
                {
                    int nAddHp = (num * inContext.inSkillFunc.astSkillFuncParam[0].iParam) / 0x2710;
                    inOriginator.handle.ActorControl.ReviveHp(nAddHp);
                }
            }
            return true;
        }

        [SkillFuncHandler(0x47, new int[] {  })]
        public static bool OnSkillFuncTargetExtraCoin(ref SSkillFuncContext inContext)
        {
            return true;
        }

        [SkillFuncHandler(0x31, new int[] {  })]
        public static bool OnSkillFuncTargetExtraExp(ref SSkillFuncContext inContext)
        {
            return true;
        }

        [SkillFuncHandler(0x30, new int[] {  })]
        public static bool OnSkillFuncTargetExtraHurt(ref SSkillFuncContext inContext)
        {
            return true;
        }

        private static void SendProtectEvent(ref SSkillFuncContext inContext, int type, int changeValue)
        {
            if (((changeValue != 0) && (inContext.inTargetObj != 0)) && (inContext.inTargetObj.handle.BuffHolderComp != null))
            {
                inContext.inTargetObj.handle.BuffHolderComp.protectRule.SendProtectEvent(type, changeValue);
            }
        }
    }
}

