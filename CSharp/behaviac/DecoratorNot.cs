namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class DecoratorNot : DecoratorNode
    {
        protected override BehaviorTask createTask()
        {
            return new DecoratorNotTask();
        }

        ~DecoratorNot()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is DecoratorNot) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class DecoratorNotTask : DecoratorTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                if (status == EBTStatus.BT_FAILURE)
                {
                    return EBTStatus.BT_SUCCESS;
                }
                if (status == EBTStatus.BT_SUCCESS)
                {
                    return EBTStatus.BT_FAILURE;
                }
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

