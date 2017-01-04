namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroGuideFollowNew_node245 : behaviac.Action
    {
        private int method_p1 = 2;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x7e66728f);
            ((ObjAgent) pAgent).RealMoveToActorRight(variable, this.method_p1);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

