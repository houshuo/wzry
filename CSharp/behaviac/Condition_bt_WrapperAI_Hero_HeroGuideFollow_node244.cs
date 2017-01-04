namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroGuideFollow_node244 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            uint myObjID = ((ObjAgent) pAgent).GetMyObjID();
            uint variable = (uint) pAgent.GetVariable((uint) 0xa01cd192);
            return ((myObjID >= variable) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

