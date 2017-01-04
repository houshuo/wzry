namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class DecoratorLog : DecoratorNode
    {
        protected string m_message;

        protected override BehaviorTask createTask()
        {
            return new DecoratorLogTask();
        }

        ~DecoratorLog()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is DecoratorLog) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if (_t.name == "Log")
                {
                    this.m_message = _t.value;
                }
            }
        }

        private class DecoratorLogTask : DecoratorTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                DecoratorLog node = (DecoratorLog) base.GetNode();
                return status;
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

