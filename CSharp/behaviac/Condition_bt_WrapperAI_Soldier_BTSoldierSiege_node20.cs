namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Condition_bt_WrapperAI_Soldier_BTSoldierSiege_node20 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            OutOfControlType outOfControlType = ((ObjAgent) pAgent).GetOutOfControlType();
            OutOfControlType taunt = OutOfControlType.Taunt;
            return ((outOfControlType != taunt) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

