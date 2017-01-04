namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroSimpleAI_node500 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus variable = (EBTStatus) ((int) pAgent.GetVariable((uint) 0xb8871cee));
            EBTStatus status2 = EBTStatus.BT_SUCCESS;
            return ((variable != status2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

