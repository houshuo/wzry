namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node82 : Assignment
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

