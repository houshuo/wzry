namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Soldier_BTSoldierNormal_node38 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint variable = (uint) pAgent.GetVariable((uint) 0x4349179f);
            pAgent.SetVariable<uint>("p_abandonTargetID", variable, 0xb81a7cc);
            return status;
        }
    }
}

