namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public abstract class DecoratorCount : DecoratorNode
    {
        private Property m_count_var;

        ~DecoratorCount()
        {
            this.m_count_var = null;
        }

        protected virtual int GetCount(Agent pAgent)
        {
            if (this.m_count_var != null)
            {
                return (int) this.m_count_var.GetValue(pAgent);
            }
            return 0;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if (_t.name == "Count")
                {
                    string typeName = null;
                    string propertyName = null;
                    this.m_count_var = Condition.LoadRight(_t.value, propertyName, ref typeName);
                }
            }
        }

        protected abstract class DecoratorCountTask : DecoratorTask
        {
            protected int m_n;

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
                DecoratorCount.DecoratorCountTask task = (DecoratorCount.DecoratorCountTask) target;
                task.m_n = this.m_n;
            }

            ~DecoratorCountTask()
            {
            }

            public int GetCount(Agent pAgent)
            {
                DecoratorCount node = (DecoratorCount) base.GetNode();
                return ((node == null) ? 0 : node.GetCount(pAgent));
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                base.onenter(pAgent);
                if ((this.m_n == 0) || !this.NeedRestart())
                {
                    int count = this.GetCount(pAgent);
                    if (count == 0)
                    {
                        return false;
                    }
                    this.m_n = count;
                }
                return true;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
                CSerializationID attrId = new CSerializationID("count");
                node.setAttr<int>(attrId, this.m_n);
            }
        }
    }
}

