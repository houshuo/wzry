namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Hero_HeroCommonAutoAI_node1030 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x5710b0cf);
            int num2 = 1;
            int num3 = variable - num2;
            pAgent.SetVariable<int>("p_idleShowLast", num3, 0x5710b0cf);
            return status;
        }
    }
}

