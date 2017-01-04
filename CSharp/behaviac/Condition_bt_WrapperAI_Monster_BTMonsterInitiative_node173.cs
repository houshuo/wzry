namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;

    internal class Condition_bt_WrapperAI_Monster_BTMonsterInitiative_node173 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            SkillSlotType variable = (SkillSlotType) ((int) pAgent.GetVariable((uint) 0x6c745b));
            SkillTargetRule skillTargetRule = ((ObjAgent) pAgent).GetSkillTargetRule(variable);
            SkillTargetRule nearestEnermy = SkillTargetRule.NearestEnermy;
            return ((skillTargetRule != nearestEnermy) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

