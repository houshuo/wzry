namespace behaviac
{
    using System;

    internal class DecoratorLoopUntil_bt_WrapperAI_Hero_HeroGuideFollow_node99 : DecoratorLoopUntil
    {
        public DecoratorLoopUntil_bt_WrapperAI_Hero_HeroGuideFollow_node99()
        {
            base.m_bDecorateWhenChildEnds = true;
            base.m_until = true;
        }

        protected override int GetCount(Agent pAgent)
        {
            return -1;
        }
    }
}

