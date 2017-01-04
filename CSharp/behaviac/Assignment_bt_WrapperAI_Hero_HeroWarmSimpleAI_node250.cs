namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node250 : Assignment
    {
        private int opr_p0 = 0x7530;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint nearestEnemyIgnoreVisible = ((ObjAgent) pAgent).GetNearestEnemyIgnoreVisible(this.opr_p0);
            pAgent.SetVariable<uint>("p_targetID", nearestEnemyIgnoreVisible, 0x4349179f);
            return status;
        }
    }
}

