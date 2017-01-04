namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Soldier_BTSoldierPro_node432 : behaviac.Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint variable = (uint) pAgent.GetVariable((uint) 0x4349179f);
            ((ObjAgent) pAgent).RealMoveToActor(variable);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

