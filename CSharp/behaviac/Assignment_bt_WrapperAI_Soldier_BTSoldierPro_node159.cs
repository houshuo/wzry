namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Soldier_BTSoldierPro_node159 : Assignment
    {
        private SkillSlotType opr_p0 = SkillSlotType.SLOT_SKILL_3;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int skillAttackRange = ((ObjAgent) pAgent).GetSkillAttackRange(this.opr_p0);
            pAgent.SetVariable<int>("p_skillAttackRange", skillAttackRange, 0x73e592c4);
            return status;
        }
    }
}

