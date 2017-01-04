namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterBossInitiative_node234 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint myTargetID = ((ObjAgent) pAgent).GetMyTargetID();
            pAgent.SetVariable<uint>("p_targetID", myTargetID, 0x4349179f);
            return status;
        }
    }
}

