namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroWarmSimpleAI_node308 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x4349179f);
            ActorTypeDef actorType = ((ObjAgent) pAgent).GetActorType(variable);
            ActorTypeDef def2 = ActorTypeDef.Actor_Type_Hero;
            return ((actorType == def2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

