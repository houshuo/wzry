namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Monster_BTMonsterInitiative_node36 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0x1fd679ba);
            int num2 = 4;
            return ((variable >= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

