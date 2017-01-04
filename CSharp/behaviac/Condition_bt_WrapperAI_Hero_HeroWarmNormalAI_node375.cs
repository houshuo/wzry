namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node375 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            bool variable = (bool) pAgent.GetVariable((uint) 0x5152913b);
            bool flag2 = true;
            return ((variable != flag2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

