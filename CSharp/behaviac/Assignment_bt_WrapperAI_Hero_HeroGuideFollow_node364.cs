namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollow_node364 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint hostPlayrCaptain = ((ObjAgent) pAgent).GetHostPlayrCaptain();
            pAgent.SetVariable<uint>("p_captainID", hostPlayrCaptain, 0x7e66728f);
            return status;
        }
    }
}

