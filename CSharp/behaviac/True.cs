namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class True : ConditionBase
    {
        protected override BehaviorTask createTask()
        {
            return new TrueTask();
        }

        ~True()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is True) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class TrueTask : ConditionBaseTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~TrueTask()
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
                return EBTStatus.BT_SUCCESS;
            }
        }
    }
}

