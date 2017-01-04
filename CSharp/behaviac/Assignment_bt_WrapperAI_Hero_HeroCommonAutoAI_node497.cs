namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node497 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            EBTStatus status2 = ((ObjAgent) pAgent).IsLowAI();
            pAgent.SetVariable<EBTStatus>("p_LowAI", status2, 0xb8871cee);
            return status;
        }
    }
}

