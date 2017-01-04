namespace behaviac
{
    using System;

    internal class DecoratorLoopUntil_bt_WrapperAI_Hero_HeroLowAI_node546 : DecoratorLoopUntil
    {
        public DecoratorLoopUntil_bt_WrapperAI_Hero_HeroLowAI_node546()
        {
            base.m_bDecorateWhenChildEnds = false;
            base.m_until = true;
        }

        protected override int GetCount(Agent pAgent)
        {
            return 180;
        }
    }
}

