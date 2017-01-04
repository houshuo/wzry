namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Soldier_BTSoldierNormal_node20 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x2abf5ad6);
            int num2 = 1;
            int num3 = variable + num2;
            pAgent.SetVariable<int>("p_pursueTime", num3, 0x2abf5ad6);
            return status;
        }
    }
}

