namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class DecoratorLoopUntil : DecoratorCount
    {
        protected bool m_until = true;

        protected override BehaviorTask createTask()
        {
            return new DecoratorLoopUntilTask();
        }

        ~DecoratorLoopUntil()
        {
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if (_t.name == "Until")
                {
                    if (_t.value == "true")
                    {
                        this.m_until = true;
                    }
                    else if (_t.value == "false")
                    {
                        this.m_until = false;
                    }
                }
            }
        }

        private class DecoratorLoopUntilTask : DecoratorCount.DecoratorCountTask
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
                }
                if (base.m_n == 0)
                {
                    return EBTStatus.BT_SUCCESS;
                }
                DecoratorLoopUntil node = (DecoratorLoopUntil) base.GetNode();
                if (node.m_until)
                {
                    if (status == EBTStatus.BT_SUCCESS)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }
                }
                else if (status == EBTStatus.BT_FAILURE)
                {
                    return EBTStatus.BT_FAILURE;
                }
                return EBTStatus.BT_RUNNING;
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

