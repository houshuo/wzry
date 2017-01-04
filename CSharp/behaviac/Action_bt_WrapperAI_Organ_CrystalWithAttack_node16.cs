namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Action_bt_WrapperAI_Organ_CrystalWithAttack_node16 : Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ((ObjAgent) pAgent).HelpToAttack();
            return EBTStatus.BT_SUCCESS;
        }
    }
}

