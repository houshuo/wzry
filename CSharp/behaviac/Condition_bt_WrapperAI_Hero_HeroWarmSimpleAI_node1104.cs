namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroWarmSimpleAI_node1104 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint myTargetID = ((ObjAgent) pAgent).GetMyTargetID();
            uint variable = (uint) pAgent.GetVariable((uint) 0x4349179f);
            return ((myTargetID != variable) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

