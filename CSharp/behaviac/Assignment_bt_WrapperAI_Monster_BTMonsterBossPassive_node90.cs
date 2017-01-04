namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterBossPassive_node90 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint terrorMeActor = ((ObjAgent) pAgent).GetTerrorMeActor();
            pAgent.SetVariable<uint>("p_terrorMeActor", terrorMeActor, 0x3d4ae8e1);
            return status;
        }
    }
}

