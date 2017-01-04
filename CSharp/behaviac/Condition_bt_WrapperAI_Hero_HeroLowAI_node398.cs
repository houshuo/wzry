namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using ResData;

    internal class Condition_bt_WrapperAI_Hero_HeroLowAI_node398 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            RES_LEVEL_HEROAITYPE mapAIMode = ((ObjAgent) pAgent).GetMapAIMode();
            RES_LEVEL_HEROAITYPE res_level_heroaitype2 = RES_LEVEL_HEROAITYPE.RES_LEVEL_HEROAITYPE_CAPTAIN;
            return ((mapAIMode == res_level_heroaitype2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

