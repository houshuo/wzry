namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Condition_bt_WrapperAI_Hero_HeroGuideFollow_node1037 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = ((ObjAgent) pAgent).IsPlayOnNetwork();
            EBTStatus status2 = EBTStatus.BT_FAILURE;
            return ((status != status2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

