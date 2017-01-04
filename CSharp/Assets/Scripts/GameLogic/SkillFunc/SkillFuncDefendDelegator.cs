namespace Assets.Scripts.GameLogic.SkillFunc
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [SkillFuncHandlerClass]
    internal class SkillFuncDefendDelegator
    {
        [SkillFuncHandler(0x22, new int[] {  })]
        public static bool OnSkillFuncClearSkillEffect(ref SSkillFuncContext inContext)
        {
            if (inContext.inStage == ESkillFuncStage.Enter)
            {
                PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
                if (inTargetObj != 0)
                {
                    int skillFuncParam = inContext.GetSkillFuncParam(0, false);
                    inTargetObj.handle.BuffHolderComp.ClearEffectTypeBuff(skillFuncParam);
                }
            }
            return true;
        }

        [SkillFuncHandler(0x1f, new int[] {  })]
        public static bool OnSkillFuncImmuneControlSkillEffect(ref SSkillFuncContext inContext)
        {
            if (inContext.inStage == ESkillFuncStage.Enter)
            {
                PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
                if (inTargetObj != 0)
                {
                    inTargetObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
                    if (inContext.GetSkillFuncParam(0, false) == 1)
                    {
                        int num = 4;
                        inTargetObj.handle.BuffHolderComp.ClearEffectTypeBuff(num);
                    }
                }
            }
            else if (inContext.inStage == ESkillFuncStage.Leave)
            {
                PoolObjHandle<ActorRoot> handle2 = inContext.inTargetObj;
                if (handle2 != 0)
                {
                    handle2.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
                }
            }
            return true;
        }

        [SkillFuncHandler(0x36, new int[] {  })]
        public static bool OnSkillFuncImmuneDeadHurt(ref SSkillFuncContext inContext)
        {
            return true;
        }

        [SkillFuncHandler(30, new int[] {  })]
        public static bool OnSkillFuncImmuneHurt(ref SSkillFuncContext inContext)
        {
            if ((inContext.inStage == ESkillFuncStage.Enter) && (inContext.inTargetObj != 0))
            {
                int skillFuncParam = inContext.GetSkillFuncParam(1, false);
                int num2 = inContext.GetSkillFuncParam(2, false);
                inContext.inBuffSkill.handle.CustomParams[3] = skillFuncParam;
                inContext.inBuffSkill.handle.CustomParams[4] = num2;
            }
            return true;
        }

        [SkillFuncHandler(15, new int[] {  })]
        public static bool OnSkillFuncImmuneNegativeSkillEffect(ref SSkillFuncContext inContext)
        {
            if (inContext.inStage == ESkillFuncStage.Enter)
            {
                PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
                if (inTargetObj != 0)
                {
                    inTargetObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative);
                    if (inContext.GetSkillFuncParam(0, false) == 1)
                    {
                        int num = 2;
                        num += 4;
                        inTargetObj.handle.BuffHolderComp.ClearEffectTypeBuff(num);
                    }
                }
            }
            else if (inContext.inStage == ESkillFuncStage.Leave)
            {
                PoolObjHandle<ActorRoot> handle2 = inContext.inTargetObj;
                if (handle2 != 0)
                {
                    handle2.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative);
                }
            }
            return true;
        }
    }
}

