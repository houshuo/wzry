namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroGuideFollow_node360 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            bool variable = (bool) pAgent.GetVariable((uint) 0x88b8dd9a);
            bool flag2 = true;
            return ((variable != flag2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

