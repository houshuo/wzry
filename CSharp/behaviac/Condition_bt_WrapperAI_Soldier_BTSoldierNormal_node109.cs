namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Soldier_BTSoldierNormal_node109 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x4349179f);
            uint num2 = 0;
            return ((variable <= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

