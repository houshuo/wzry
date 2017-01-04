namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class DecoratorLoop : DecoratorCount
    {
        protected override BehaviorTask createTask()
        {
            return new DecoratorLoopTask();
        }

        ~DecoratorLoop()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is DecoratorLoop) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class DecoratorLoopTask : DecoratorCount.DecoratorCountTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                if (base.m_n > 0)
                {
                    base.m_n--;
                    if (base.m_n == 0)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }
                    return EBTStatus.BT_RUNNING;
                }
                if (base.m_n == -1)
                {
                    return EBTStatus.BT_RUNNING;
                }
                return EBTStatus.BT_SUCCESS;
            }

            ~DecoratorLoopTask()
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
        }
    }
}

