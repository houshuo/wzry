namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroGuideFollowNew_node336 : Assignment
    {
        private int opr_p0 = 0x2710;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint nearestEnemyWithoutNotInBattleJungleMonster = ((ObjAgent) pAgent).GetNearestEnemyWithoutNotInBattleJungleMonster(this.opr_p0);
            pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithoutNotInBattleJungleMonster, 0x4349179f);
            return status;
        }
    }
}

