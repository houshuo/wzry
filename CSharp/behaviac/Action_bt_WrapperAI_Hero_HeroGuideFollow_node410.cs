namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroGuideFollow_node410 : behaviac.Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x7e66728f);
            ((ObjAgent) pAgent).RealMoveToActor(variable);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

