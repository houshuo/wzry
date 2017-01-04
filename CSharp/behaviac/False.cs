namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class False : ConditionBase
    {
        protected override BehaviorTask createTask()
        {
            return new FalseTask();
        }

        ~False()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is False) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class FalseTask : ConditionBaseTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~FalseTask()
            {
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                return EBTStatus.BT_FAILURE;
            }
        }
    }
}

