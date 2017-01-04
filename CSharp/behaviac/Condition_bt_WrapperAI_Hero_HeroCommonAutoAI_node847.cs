namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node847 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            OutOfControlType outOfControlType = ((ObjAgent) pAgent).GetOutOfControlType();
            OutOfControlType terror = OutOfControlType.Terror;
            return ((outOfControlType != terror) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

