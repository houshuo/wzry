namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Hero_HeroWarmSimpleAI_node1050 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x8c2e65af);
            int num2 = 2;
            int num3 = variable * num2;
            pAgent.SetVariable<int>("p_waitBornFrame", num3, 0x8c2e65af);
            return status;
        }
    }
}

