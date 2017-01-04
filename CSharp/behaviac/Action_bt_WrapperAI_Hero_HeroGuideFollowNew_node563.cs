namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroGuideFollowNew_node563 : behaviac.Action
    {
        private int method_p0 = 0x2af8;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            return ((ObjAgent) pAgent).HasEnemyInRange(this.method_p0);
        }
    }
}

