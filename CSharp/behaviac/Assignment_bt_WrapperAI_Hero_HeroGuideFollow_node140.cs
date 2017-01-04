namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollow_node140 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            bool flag = false;
            pAgent.SetVariable<bool>("p_mustToKillDragon", flag, 0xd6d2067e);
            return status;
        }
    }
}

