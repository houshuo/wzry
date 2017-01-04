namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Query : BehaviorNode
    {
        private List<Descriptor_t> m_descriptors;
        protected string m_domain;

        private float ComputeSimilarity(List<Descriptor_t> q, List<BehaviorTree.Descriptor_t> c)
        {
            float num = 0f;
            for (int i = 0; i < q.Count; i++)
            {
                Descriptor_t _t = q[i];
                Property other = FindProperty(_t, c);
                if (other != null)
                {
                    float num3 = _t.Attribute.DifferencePercentage(other);
                    num += (1f - num3) * _t.Weight;
                }
            }
            return num;
        }

        protected override BehaviorTask createTask()
        {
            return new QueryTask();
        }

        ~Query()
        {
        }

        private static Property FindProperty(Descriptor_t q, List<BehaviorTree.Descriptor_t> c)
        {
            for (int i = 0; i < c.Count; i++)
            {
                BehaviorTree.Descriptor_t _t = c[i];
                if (_t.Descriptor.GetVariableId() == q.Attribute.GetVariableId())
                {
                    return _t.Descriptor;
                }
            }
            return null;
        }

        private List<Descriptor_t> GetDescriptors()
        {
            return this.m_descriptors;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            if (properties.Count > 0)
            {
                foreach (property_t _t in properties)
                {
                    if (_t.name == "Domain")
                    {
                        this.m_domain = _t.value;
                    }
                    else if (_t.name == "Descriptors")
                    {
                        this.SetDescriptors(_t.value);
                    }
                }
            }
        }

        protected void SetDescriptors(string descriptors)
        {
            this.m_descriptors = (List<Descriptor_t>) StringUtils.FromString(typeof(List<Descriptor_t>), descriptors, false);
            for (int i = 0; i < this.m_descriptors.Count; i++)
            {
                Descriptor_t _t = this.m_descriptors[i];
                _t.Attribute.SetDefaultValue(_t.Reference);
            }
        }

        private class Descriptor_t
        {
            public Property Attribute;
            public Property Reference;
            public float Weight;

            public Descriptor_t()
            {
                this.Attribute = null;
                this.Reference = null;
                this.Weight = 0f;
            }

            public Descriptor_t(Query.Descriptor_t copy)
            {
                this.Attribute = copy.Attribute.clone();
                this.Reference = copy.Reference.clone();
                this.Weight = copy.Weight;
            }

            ~Descriptor_t()
            {
                this.Attribute = null;
                this.Reference = null;
            }
        }

        private class QueryTask : SingeChildTask
        {
            public override bool CheckPredicates(Agent pAgent)
            {
                bool flag = false;
                if (base.m_attachments != null)
                {
                    flag = base.CheckPredicates(pAgent);
                }
                if (flag)
                {
                    this.ReQuery(pAgent);
                }
                return flag;
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~QueryTask()
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
                return this.ReQuery(pAgent);
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            private bool ReQuery(Agent pAgent)
            {
                Query node = base.GetNode() as Query;
                if (node != null)
                {
                    List<Query.Descriptor_t> descriptors = node.GetDescriptors();
                    if (descriptors.Count > 0)
                    {
                        DictionaryView<string, BehaviorTree> behaviorTrees = Workspace.GetBehaviorTrees();
                        BehaviorTree tree = null;
                        float num = -1f;
                        foreach (KeyValuePair<string, BehaviorTree> pair in behaviorTrees)
                        {
                            BehaviorTree tree2 = pair.Value;
                            string domains = tree2.GetDomains();
                            if (string.IsNullOrEmpty(node.m_domain) || (!string.IsNullOrEmpty(domains) && (domains.IndexOf(node.m_domain) != -1)))
                            {
                                List<BehaviorTree.Descriptor_t> c = tree2.GetDescriptors();
                                float num2 = node.ComputeSimilarity(descriptors, c);
                                if (num2 > num)
                                {
                                    num = num2;
                                    tree = tree2;
                                }
                            }
                        }
                        if (tree != null)
                        {
                            pAgent.btreferencetree(tree.GetName());
                            return true;
                        }
                    }
                }
                return false;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                return EBTStatus.BT_RUNNING;
            }
        }
    }
}

