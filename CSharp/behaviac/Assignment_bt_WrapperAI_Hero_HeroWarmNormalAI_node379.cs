namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node379 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            bool flag = false;
            pAgent.SetVariable<bool>("p_needGoHome", flag, 0x5152913b);
            return status;
        }
    }
}

