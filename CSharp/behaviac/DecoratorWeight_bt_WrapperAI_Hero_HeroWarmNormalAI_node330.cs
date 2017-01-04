namespace behaviac
{
    using System;

    internal class DecoratorWeight_bt_WrapperAI_Hero_HeroWarmNormalAI_node330 : DecoratorWeight
    {
        public DecoratorWeight_bt_WrapperAI_Hero_HeroWarmNormalAI_node330()
        {
            base.m_bDecorateWhenChildEnds = false;
        }

        protected override int GetWeight(Agent pAgent)
        {
            return 200;
        }
    }
}

