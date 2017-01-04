namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class IfElse : BehaviorNode
    {
        protected override BehaviorTask createTask()
        {
            return new IfElseTask();
        }

        ~IfElse()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is IfElse) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class IfElseTask : CompositeTask
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
                base.m_activeChildIndex = -1;
                return (base.m_children.Count == 3);
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
                if (childStatus != EBTStatus.BT_RUNNING)
                {
                    return childStatus;
                }
                if (base.m_activeChildIndex == -1)
                {
                    BehaviorTask task = base.m_children[0];
                    switch (task.exec(pAgent))
                    {
                        case EBTStatus.BT_SUCCESS:
                            base.m_activeChildIndex = 1;
                            break;

                        case EBTStatus.BT_FAILURE:
                            base.m_activeChildIndex = 2;
                            break;
                    }
                }
                if (base.m_activeChildIndex != -1)
                {
                    BehaviorTask task2 = base.m_children[base.m_activeChildIndex];
                    return task2.exec(pAgent);
                }
                return EBTStatus.BT_RUNNING;
            }
        }
    }
}

