namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Hero_HeroWarmNormalAI_node575 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0xb8ff8feb);
            int num2 = 1;
            int num3 = variable + num2;
            pAgent.SetVariable<int>("p_pursueFrame", num3, 0xb8ff8feb);
            return status;
        }
    }
}

