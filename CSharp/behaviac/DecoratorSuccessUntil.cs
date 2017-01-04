namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class DecoratorSuccessUntil : DecoratorCount
    {
        protected override BehaviorTask createTask()
        {
            return new DecoratorSuccessUntilTask();
        }

        ~DecoratorSuccessUntil()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is DecoratorSuccessUntil) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class DecoratorSuccessUntilTask : DecoratorCount.DecoratorCountTask
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
                        return EBTStatus.BT_FAILURE;
                    }
                    return EBTStatus.BT_SUCCESS;
                }
                if (base.m_n == -1)
                {
                    return EBTStatus.BT_SUCCESS;
                }
                return EBTStatus.BT_FAILURE;
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            public override bool NeedRestart()
            {
                return true;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }
        }
    }
}

