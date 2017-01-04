namespace behaviac
{
    using Assets.Scripts.Framework;
    using System;
    using System.Collections.Generic;

    public abstract class CompositeStochastic : BehaviorNode
    {
        protected CMethodBase m_method;

        ~CompositeStochastic()
        {
            this.m_method = null;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is CompositeStochastic) && base.IsValid(pAgent, pTask));
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

        public class CompositeStochasticTask : CompositeTask
        {
            protected List<int> m_set = new List<int>();
            public static uint RandomMax = 0x2710;

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
                CompositeStochastic.CompositeStochasticTask task = (CompositeStochastic.CompositeStochasticTask) target;
                task.m_set = this.m_set;
            }

            ~CompositeStochasticTask()
            {
            }

            public static int GetRandomValue(CMethodBase method, Agent pAgent)
            {
                if (method != null)
                {
                    ParentType parentType = method.GetParentType();
                    Agent parent = pAgent;
                    if (parentType == ParentType.PT_INSTANCE)
                    {
                        parent = Agent.GetInstance(method.GetInstanceNameString(), parent.GetContextId());
                    }
                    return (int) method.run(parent, pAgent);
                }
                return FrameRandom.Random(RandomMax);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                this.random_child(pAgent);
                base.m_activeChildIndex = 0;
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            private void random_child(Agent pAgent)
            {
                CompositeStochastic node = (CompositeStochastic) base.GetNode();
                int count = base.m_children.Count;
                if (this.m_set.Count != count)
                {
                    this.m_set.Clear();
                    for (int j = 0; j < count; j++)
                    {
                        this.m_set.Add(j);
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    int num4 = (int) (((long) (count * GetRandomValue((node == null) ? null : node.m_method, pAgent))) / ((ulong) RandomMax));
                    int num5 = (int) (((long) (count * GetRandomValue((node == null) ? null : node.m_method, pAgent))) / ((ulong) RandomMax));
                    if (num4 != num5)
                    {
                        int num6 = this.m_set[num4];
                        this.m_set[num4] = this.m_set[num5];
                        this.m_set[num5] = num6;
                    }
                }
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
                CSerializationID attrId = new CSerializationID("set");
                node.setAttr<List<int>>(attrId, this.m_set);
            }
        }
    }
}

