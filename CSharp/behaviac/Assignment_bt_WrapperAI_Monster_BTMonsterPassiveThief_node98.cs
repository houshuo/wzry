namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterPassiveThief_node98 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 myCurPos = ((ObjAgent) pAgent).GetMyCurPos();
            pAgent.SetVariable<Vector3>("p_randomPos", myCurPos, 0xcb5cf5ca);
            return status;
        }
    }
}

