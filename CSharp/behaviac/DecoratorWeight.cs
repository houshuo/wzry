namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class DecoratorWeight : DecoratorNode
    {
        private Property m_weight_var;

        protected override BehaviorTask createTask()
        {
            return new DecoratorWeightTask();
        }

        ~DecoratorWeight()
        {
            this.m_weight_var = null;
        }

        protected virtual int GetWeight(Agent pAgent)
        {
            if (this.m_weight_var != null)
            {
                return (int) this.m_weight_var.GetValue(pAgent);
            }
            return 0;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if (_t.name == "Weight")
                {
                    string typeName = null;
                    string propertyName = null;
                    this.m_weight_var = Condition.LoadRight(_t.value, propertyName, ref typeName);
                }
            }
        }

        public class DecoratorWeightTask : DecoratorTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                return status;
            }

            public int GetWeight(Agent pAgent)
            {
                DecoratorWeight node = (DecoratorWeight) base.GetNode();
                return ((node == null) ? 0 : node.GetWeight(pAgent));
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }
        }
    }
}

