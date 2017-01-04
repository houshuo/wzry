namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node361 : Condition
    {
        private int opl_p1 = 0x2af8;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x7e66728f);
            bool flag = ((ObjAgent) pAgent).IsDistanceToActorMoreThanRange(variable, this.opl_p1);
            bool flag2 = true;
            return ((flag != flag2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

