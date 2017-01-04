namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollowNew_node130 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            bool flag = true;
            pAgent.SetVariable<bool>("p_followSoldierLine", flag, 0xed860278);
            return status;
        }
    }
}

