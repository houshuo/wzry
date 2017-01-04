namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node659 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int num = 0x7530;
            pAgent.SetVariable<int>("p_srchRange", num, 0x921d0d6a);
            return status;
        }
    }
}

