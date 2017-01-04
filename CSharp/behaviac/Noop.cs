namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Noop : BehaviorNode
    {
        protected override BehaviorTask createTask()
        {
            return new NoopTask();
        }

        ~Noop()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return (pTask.GetNode() is Noop);
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class NoopTask : LeafTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                return EBTStatus.BT_SUCCESS;
            }
        }
    }
}

