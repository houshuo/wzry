namespace behaviac
{
    using System;

    internal class DecoratorWeight_bt_WrapperAI_Hero_HeroWarmNormalAI_node360 : DecoratorWeight
    {
        public DecoratorWeight_bt_WrapperAI_Hero_HeroWarmNormalAI_node360()
        {
            base.m_bDecorateWhenChildEnds = false;
        }

        protected override int GetWeight(Agent pAgent)
        {
            return 4;
        }
    }
}

