namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Action_bt_WrapperAI_Soldier_BTSoldierNormal_node30 : Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ((ObjAgent) pAgent).ChangeTargetToAtker();
            return EBTStatus.BT_SUCCESS;
        }
    }
}

