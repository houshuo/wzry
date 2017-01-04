namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollow_node110 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint dragonId = ((ObjAgent) pAgent).GetDragonId();
            pAgent.SetVariable<uint>("p_targetID", dragonId, 0x4349179f);
            return status;
        }
    }
}

