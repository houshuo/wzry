namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroGuideFollow_node141 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            bool variable = (bool) pAgent.GetVariable((uint) 0xd6d2067e);
            bool flag2 = false;
            return ((variable != flag2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

