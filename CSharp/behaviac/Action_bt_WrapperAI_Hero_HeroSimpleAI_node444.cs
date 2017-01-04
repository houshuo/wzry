namespace behaviac
{
    using Assets.Scripts.GameLogic;

    internal class Action_bt_WrapperAI_Hero_HeroSimpleAI_node444 : Action
    {
        private SkillAbortType method_p0 = SkillAbortType.TYPE_MOVE;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            return ((ObjAgent) pAgent).AbortCurUseSkillByType(this.method_p0);
        }
    }
}

