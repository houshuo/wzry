namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Soldier_BTSoldierPro_node9 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x921d0d6a);
            uint nearestEnemy = ((ObjAgent) pAgent).GetNearestEnemy(variable);
            pAgent.SetVariable<uint>("p_targetID", nearestEnemy, 0x4349179f);
            return status;
        }
    }
}

