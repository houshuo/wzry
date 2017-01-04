namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollow_node422 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 restoredHpPos = ((ObjAgent) pAgent).GetRestoredHpPos();
            pAgent.SetVariable<Vector3>("p_restorePos", restoredHpPos, 0x9bc0c9a2);
            return status;
        }
    }
}

