namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Condition_bt_WrapperAI_Hero_HeroLowAI_node84 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            OutOfControlType outOfControlType = ((ObjAgent) pAgent).GetOutOfControlType();
            OutOfControlType taunt = OutOfControlType.Taunt;
            return ((outOfControlType != taunt) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

