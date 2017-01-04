namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node1021 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x7206a9b3);
            int numB = (int) pAgent.GetVariable((uint) 0x8bd66236);
            int mod = ((BTBaseAgent) pAgent).GetMod(variable, numB);
            int num4 = 0;
            return ((mod != num4) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

