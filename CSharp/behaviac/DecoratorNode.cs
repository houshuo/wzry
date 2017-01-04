namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public abstract class DecoratorNode : BehaviorNode
    {
        public bool m_bDecorateWhenChildEnds = false;

        ~DecoratorNode()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is DecoratorNode) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if ((_t.name == "DecorateWhenChildEnds") && (_t.value == "true"))
                {
                    this.m_bDecorateWhenChildEnds = true;
                }
            }
        }
    }
}

