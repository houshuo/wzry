namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node441 : behaviac.Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0xeddd618f);
            ((ObjAgent) pAgent).RealMoveToActor(variable);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

