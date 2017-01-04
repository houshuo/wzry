namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterBossPassive_node162 : Assignment
    {
        private SkillSlotType opr_p0 = SkillSlotType.SLOT_SKILL_2;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int skillAttackRange = ((ObjAgent) pAgent).GetSkillAttackRange(this.opr_p0);
            pAgent.SetVariable<int>("p_skillAttackRange", skillAttackRange, 0x73e592c4);
            return status;
        }
    }
}

