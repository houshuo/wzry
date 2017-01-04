namespace behaviac
{
    using System;

    internal class DecoratorWeight_bt_WrapperAI_Hero_HeroWarmNormalAI_node380 : DecoratorWeight
    {
        public DecoratorWeight_bt_WrapperAI_Hero_HeroWarmNormalAI_node380()
        {
            base.m_bDecorateWhenChildEnds = false;
        }

        protected override int GetWeight(Agent pAgent)
        {
            return (int) pAgent.GetVariable((uint) 0xf25eb1eb);
        }
    }
}

