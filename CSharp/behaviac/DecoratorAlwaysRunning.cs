namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class DecoratorAlwaysRunning : DecoratorNode
    {
        protected override BehaviorTask createTask()
        {
            return new DecoratorAlwaysRunningTask();
        }

        ~DecoratorAlwaysRunning()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is DecoratorAlwaysRunning) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class DecoratorAlwaysRunningTask : DecoratorTask
        {
            protected override void addChild(BehaviorTask pBehavior)
            {
                base.addChild(pBehavior);
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                return EBTStatus.BT_RUNNING;
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

