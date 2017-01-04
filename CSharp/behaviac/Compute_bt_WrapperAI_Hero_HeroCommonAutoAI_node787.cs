namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Compute_bt_WrapperAI_Hero_HeroCommonAutoAI_node787 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int pvPLevelMaxHeroNum = ((ObjAgent) pAgent).GetPvPLevelMaxHeroNum();
            int num2 = 2;
            int num3 = pvPLevelMaxHeroNum * num2;
            pAgent.SetVariable<int>("p_waitBornFrame", num3, 0x8c2e65af);
            return status;
        }
    }
}

