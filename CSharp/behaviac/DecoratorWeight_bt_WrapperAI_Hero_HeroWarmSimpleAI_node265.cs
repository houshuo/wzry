namespace behaviac
{
    using System;

    internal class DecoratorWeight_bt_WrapperAI_Hero_HeroWarmSimpleAI_node265 : DecoratorWeight
    {
        public DecoratorWeight_bt_WrapperAI_Hero_HeroWarmSimpleAI_node265()
        {
            base.m_bDecorateWhenChildEnds = false;
        }

        protected override int GetWeight(Agent pAgent)
        {
            return 10;
        }
    }
}

