namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Or : ConditionBase
    {
        protected override BehaviorTask createTask()
        {
            return new OrTask();
        }

        ~Or()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is Or) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class OrTask : Selector.SelectorTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~OrTask()
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
                for (int i = 0; i < base.m_children.Count; i++)
                {
                    EBTStatus status = base.m_children[i].exec(pAgent);
                    if (status == EBTStatus.BT_SUCCESS)
                    {
                        return status;
                    }
                }
                return EBTStatus.BT_FAILURE;
            }
        }
    }
}

