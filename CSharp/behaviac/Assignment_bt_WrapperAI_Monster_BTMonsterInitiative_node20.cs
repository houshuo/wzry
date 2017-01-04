namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterInitiative_node20 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 myForward = ((ObjAgent) pAgent).GetMyForward();
            pAgent.SetVariable<Vector3>("p_orignalDirection", myForward, 0xcd833580);
            return status;
        }
    }
}

