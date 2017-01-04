namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;

    internal class Condition_bt_WrapperAI_Monster_BTMonsterRob_node74 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            SkillSlotType variable = (SkillSlotType) ((int) pAgent.GetVariable((uint) 0x6c745b));
            SkillTargetRule skillTargetRule = ((ObjAgent) pAgent).GetSkillTargetRule(variable);
            SkillTargetRule lowerHpFriendly = SkillTargetRule.LowerHpFriendly;
            return ((skillTargetRule != lowerHpFriendly) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

