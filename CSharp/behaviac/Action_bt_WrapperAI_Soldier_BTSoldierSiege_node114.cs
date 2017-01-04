namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Soldier_BTSoldierSiege_node114 : behaviac.Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0x921d0d6a);
            return ((ObjAgent) pAgent).MoveToSkillTargetWithRange(variable);
        }
    }
}

