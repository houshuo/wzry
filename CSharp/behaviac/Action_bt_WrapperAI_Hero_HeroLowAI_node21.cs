namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroLowAI_node21 : behaviac.Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            SkillSlotType variable = (SkillSlotType) ((int) pAgent.GetVariable((uint) 0x6c745b));
            uint objID = (uint) pAgent.GetVariable((uint) 0x4349179f);
            return ((ObjAgent) pAgent).CheckSkillFilter(variable, objID);
        }
    }
}

