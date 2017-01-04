namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Monster_BTMonsterPassive_zhouwang_node4 : behaviac.Action
    {
        private string method_p0 = "zhouwang_die";

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            return ((ObjAgent) pAgent).PlayHelperAgeAction(this.method_p0);
        }
    }
}

