namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollow_node183 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int num = 0x3e8;
            pAgent.SetVariable<int>("p_healthRate", num, 0x6a18fe59);
            return status;
        }
    }
}

