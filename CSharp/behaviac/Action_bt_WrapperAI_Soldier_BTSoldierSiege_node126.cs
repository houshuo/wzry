namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Action_bt_WrapperAI_Soldier_BTSoldierSiege_node126 : Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ((ObjAgent) pAgent).TerminateMove();
            return EBTStatus.BT_SUCCESS;
        }
    }
}

