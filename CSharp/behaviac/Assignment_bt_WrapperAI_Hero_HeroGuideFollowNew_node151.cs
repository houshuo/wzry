namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollowNew_node151 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int num = 0x1388;
            pAgent.SetVariable<int>("p_healthRate", num, 0x6a18fe59);
            return status;
        }
    }
}

