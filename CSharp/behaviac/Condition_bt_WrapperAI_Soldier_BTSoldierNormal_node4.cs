namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node4 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0x2abf5ad6);
            int num2 = 10;
            return ((variable >= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

