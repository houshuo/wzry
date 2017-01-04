namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Hero_HeroGuideFollow_node102 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x8c2e65af);
            int num2 = 0x1a;
            int num3 = variable + num2;
            pAgent.SetVariable<int>("p_waitBornFrame", num3, 0x8c2e65af);
            return status;
        }
    }
}

