namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroLowAI_node594 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint teamMemberTarget = ((ObjAgent) pAgent).GetTeamMemberTarget();
            pAgent.SetVariable<uint>("p_targetID", teamMemberTarget, 0x4349179f);
            return status;
        }
    }
}

