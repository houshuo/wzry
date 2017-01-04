namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class SelectorLoop : BehaviorNode
    {
        protected override BehaviorTask createTask()
        {
            return new SelectorLoopTask();
        }

        ~SelectorLoop()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is SelectorLoop) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public class SelectorLoopTask : CompositeTask
        {
            protected override void addChild(BehaviorTask pBehavior)
            {
                base.addChild(pBehavior);
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
                SelectorLoop.SelectorLoopTask task = (SelectorLoop.SelectorLoopTask) target;
                task.m_activeChildIndex = base.m_activeChildIndex;
            }

            ~SelectorLoopTask()
            {
            }

            public override void Init(BehaviorNode node)
            {
                base.Init(node);
            }

            protected override bool isContinueTicking()
            {
                return true;
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                base.m_activeChildIndex = -1;
                return base.onenter(pAgent);
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                base.onexit(pAgent, s);
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
                CSerializationID attrId = new CSerializationID("activeChild");
                node.setAttr<int>(attrId, base.m_activeChildIndex);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                int num = -1;
                for (int i = 0; i < base.m_children.Count; i++)
                {
                    WithPreconditionTask task = (WithPreconditionTask) base.m_children[i];
                    if (task.PreconditionNode().exec(pAgent) == EBTStatus.BT_SUCCESS)
                    {
                        num = i;
                        break;
                    }
                }
                if (num != -1)
                {
                    if (base.m_activeChildIndex != -1)
                    {
                        WithPreconditionTask task3 = (WithPreconditionTask) base.m_children[base.m_activeChildIndex];
                        BehaviorTask task4 = task3.Action();
                        BehaviorTask task6 = ((WithPreconditionTask) base.m_children[num]).Action();
                        if (task4 != task6)
                        {
                            task4.abort(pAgent);
                            task3.abort(pAgent);
                            base.m_activeChildIndex = num;
                        }
                    }
                    for (int j = 0; j < base.m_children.Count; j++)
                    {
                        WithPreconditionTask task7 = (WithPreconditionTask) base.m_children[j];
                        EBTStatus status2 = task7.exec(pAgent);
                        if ((j >= num) && ((j <= num) || (task7.PreconditionNode().exec(pAgent) == EBTStatus.BT_SUCCESS)))
                        {
                            EBTStatus status4 = task7.Action().exec(pAgent);
                            switch (status4)
                            {
                                case EBTStatus.BT_RUNNING:
                                    base.m_activeChildIndex = num;
                                    return status4;

                                case EBTStatus.BT_FAILURE:
                                case EBTStatus.BT_INVALID:
                                {
                                    continue;
                                }
                            }
                            return status4;
                        }
                    }
                }
                return EBTStatus.BT_FAILURE;
            }
        }
    }
}

