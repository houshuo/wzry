namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node916 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint needHelpTarget = ((ObjAgent) pAgent).GetNeedHelpTarget();
            pAgent.SetVariable<uint>("p_helpTargtID", needHelpTarget, 0x3e931a5c);
            return status;
        }
    }
}

