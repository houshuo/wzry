namespace behaviac
{
    using Assets.Scripts.Framework;
    using System;
    using System.Collections.Generic;

    public class SelectorProbability : BehaviorNode
    {
        protected CMethodBase m_method;

        public override void AddChild(BehaviorNode pBehavior)
        {
            if (((DecoratorWeight) pBehavior) != null)
            {
                base.AddChild(pBehavior);
            }
        }

        protected override BehaviorTask createTask()
        {
            return new SelectorProbabilityTask();
        }

        ~SelectorProbability()
        {
            this.m_method = null;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is SelectorProbability) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if ((_t.name == "RandomGenerator") && (_t.value[0] != '\0'))
                {
                    this.m_method = behaviac.Action.LoadMethod(_t.value);
                }
            }
        }

        private class SelectorProbabilityTask : CompositeTask
        {
            private int m_totalSum;
            private List<int> m_weightingMap = new List<int>();

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~SelectorProbabilityTask()
            {
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                this.m_weightingMap.Clear();
                this.m_totalSum = 0;
                for (int i = 0; i < base.m_children.Count; i++)
                {
                    BehaviorTask task = base.m_children[i];
                    int weight = ((DecoratorWeight.DecoratorWeightTask) task).GetWeight(pAgent);
                    this.m_weightingMap.Add(weight);
                    this.m_totalSum += weight;
                }
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                base.m_activeChildIndex = -1;
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
                if (base.m_activeChildIndex != -1)
                {
                    BehaviorTask task = base.m_children[base.m_activeChildIndex];
                    return task.exec(pAgent);
                }
                int num = FrameRandom.Random((ushort) this.m_totalSum);
                float num2 = 0f;
                for (int i = 0; i < base.m_children.Count; i++)
                {
                    int num4 = this.m_weightingMap[i];
                    num2 += num4;
                    if ((num4 > 0) && (num2 >= num))
                    {
                        EBTStatus status2 = base.m_children[i].exec(pAgent);
                        if (status2 == EBTStatus.BT_RUNNING)
                        {
                            base.m_activeChildIndex = i;
                            return status2;
                        }
                        base.m_activeChildIndex = -1;
                        return status2;
                    }
                }
                return EBTStatus.BT_FAILURE;
            }
        }
    }
}

