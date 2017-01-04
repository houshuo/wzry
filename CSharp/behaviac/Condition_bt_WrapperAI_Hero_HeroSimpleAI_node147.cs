namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroSimpleAI_node147 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int hPPercent = ((ObjAgent) pAgent).GetHPPercent();
            int variable = (int) pAgent.GetVariable((uint) 0x6a18fe59);
            return ((hPPercent >= variable) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

