namespace behaviac
{
    using System;

    internal class DecoratorLoopUntil_bt_WrapperAI_Hero_HeroWarmNormalAI_node444 : DecoratorLoopUntil
    {
        public DecoratorLoopUntil_bt_WrapperAI_Hero_HeroWarmNormalAI_node444()
        {
            base.m_bDecorateWhenChildEnds = true;
            base.m_until = true;
        }

        protected override int GetCount(Agent pAgent)
        {
            return 7;
        }
    }
}

