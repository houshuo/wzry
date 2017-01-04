namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Hero_HeroGuideFollow_node136 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0xd5f60189);
            int num2 = 1;
            int num3 = variable + num2;
            pAgent.SetVariable<int>("p_waitFrame", num3, 0xd5f60189);
            return status;
        }
    }
}

