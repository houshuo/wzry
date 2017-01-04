namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class SequenceStochastic : CompositeStochastic
    {
        protected override BehaviorTask createTask()
        {
            return new SequenceStochasticTask();
        }

        ~SequenceStochastic()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is SequenceStochastic) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class SequenceStochasticTask : CompositeStochastic.CompositeStochasticTask
        {
            protected override void addChild(BehaviorTask pBehavior)
            {
                base.addChild(pBehavior);
            }

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
                base.onenter(pAgent);
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                base.onexit(pAgent, s);
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                bool flag = true;
                do
                {
                    EBTStatus status = childStatus;
                    if (!flag || (status == EBTStatus.BT_RUNNING))
                    {
                        int num = base.m_set[base.m_activeChildIndex];
                        status = base.m_children[num].exec(pAgent);
                    }
                    flag = false;
                    if (status != EBTStatus.BT_SUCCESS)
                    {
                        return status;
                    }
                    base.m_activeChildIndex++;
                    if (base.m_activeChildIndex >= base.m_children.Count)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }
                }
                while (this.CheckPredicates(pAgent));
                return EBTStatus.BT_FAILURE;
            }
        }
    }
}

