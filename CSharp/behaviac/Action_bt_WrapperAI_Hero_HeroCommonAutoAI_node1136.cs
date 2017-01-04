namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Action_bt_WrapperAI_Hero_HeroCommonAutoAI_node1136 : Action
    {
        private ObjBehaviMode method_p0 = ObjBehaviMode.State_Idle;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ((ObjAgent) pAgent).SetCurBehavior(this.method_p0);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

