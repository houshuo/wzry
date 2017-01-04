namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterInitiative_node124 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            SkillSlotType type = SkillSlotType.SLOT_SKILL_0;
            pAgent.SetVariable<SkillSlotType>("p_curSlotType", type, 0x6c745b);
            return status;
        }
    }
}

