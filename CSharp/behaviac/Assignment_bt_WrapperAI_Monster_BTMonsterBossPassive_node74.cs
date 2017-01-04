namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterBossPassive_node74 : Assignment
    {
        private int opr_p1 = 0xbb8;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x73e592c4);
            SkillSlotType inSlot = (SkillSlotType) ((int) pAgent.GetVariable((uint) 0x6c745b));
            uint num2 = ((ObjAgent) pAgent).GetLowHpTeamMember(variable, this.opr_p1, inSlot);
            pAgent.SetVariable<uint>("p_teamTarget", num2, 0x407b5a10);
            return status;
        }
    }
}

