namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node322 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            SkillSlotType curSkillSlotType = ((ObjAgent) pAgent).GetCurSkillSlotType();
            pAgent.SetVariable<SkillSlotType>("p_curSlotType", curSkillSlotType, 0x6c745b);
            return status;
        }
    }
}

