namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1010 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint heroWhoAttackSelf = ((ObjAgent) pAgent).GetHeroWhoAttackSelf();
            pAgent.SetVariable<uint>("p_targetID", heroWhoAttackSelf, 0x4349179f);
            return status;
        }
    }
}

