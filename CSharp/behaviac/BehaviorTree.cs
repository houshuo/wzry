namespace behaviac
{
    using Mono.Xml;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Text;

    public class BehaviorTree : BehaviorNode
    {
        private List<Descriptor_t> m_descriptorRefs;
        protected string m_domains;
        protected string m_name;

        public static void Cleanup()
        {
            Workspace.UnLoadAll();
        }

        protected override BehaviorTask createTask()
        {
            return new BehaviorTreeTask();
        }

        ~BehaviorTree()
        {
            if (this.m_descriptorRefs != null)
            {
                this.m_descriptorRefs.Clear();
            }
        }

        public List<Descriptor_t> GetDescriptors()
        {
            return this.m_descriptorRefs;
        }

        public string GetDomains()
        {
            return this.m_domains;
        }

        public string GetName()
        {
            return this.m_name;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            if (properties.Count > 0)
            {
                foreach (property_t _t in properties)
                {
                    if (_t.name == "Domains")
                    {
                        this.m_domains = _t.value;
                    }
                    else if (_t.name == "DescriptorRefs")
                    {
                        this.m_descriptorRefs = (List<Descriptor_t>) StringUtils.FromString(typeof(List<Descriptor_t>), _t.value, false);
                        if (this.m_descriptorRefs != null)
                        {
                            for (int i = 0; i < this.m_descriptorRefs.Count; i++)
                            {
                                Descriptor_t _t2 = this.m_descriptorRefs[i];
                                if (_t2.Descriptor != null)
                                {
                                    _t2.Descriptor.SetDefaultValue(_t2.Reference);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool load_xml(byte[] pBuffer)
        {
            try
            {
                string xml = Encoding.UTF8.GetString(pBuffer);
                Mono.Xml.SecurityParser parser = new Mono.Xml.SecurityParser();
                parser.LoadXml(xml);
                SecurityElement node = parser.ToXml();
                if ((node.Tag != "behavior") && ((node.Children == null) || (node.Children.Count != 1)))
                {
                    return false;
                }
                this.m_name = node.Attribute("name");
                string agentType = node.Attribute("agenttype");
                int version = int.Parse(node.Attribute("version"));
                base.SetClassNameString("BehaviorTree");
                base.SetId(-1);
                base.load_properties_pars_attachments_children(true, version, agentType, node);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public void SetDescriptors(string descriptors)
        {
            this.m_descriptorRefs = (List<Descriptor_t>) StringUtils.FromString(typeof(List<Descriptor_t>), descriptors, false);
            if (this.m_descriptorRefs != null)
            {
                for (int i = 0; i < this.m_descriptorRefs.Count; i++)
                {
                    Descriptor_t _t = this.m_descriptorRefs[i];
                    _t.Descriptor.SetDefaultValue(_t.Reference);
                }
            }
        }

        public void SetDomains(string domains)
        {
            this.m_domains = domains;
        }

        public void SetName(string name)
        {
            this.m_name = name;
        }

        public class Descriptor_t
        {
            public Property Descriptor;
            public Property Reference;

            public Descriptor_t()
            {
            }

            public Descriptor_t(BehaviorTree.Descriptor_t copy)
            {
                this.Descriptor = (copy.Descriptor == null) ? null : copy.Descriptor.clone();
                this.Reference = (copy.Reference == null) ? null : copy.Reference.clone();
            }

            ~Descriptor_t()
            {
                this.Descriptor = null;
                this.Reference = null;
            }
        }
    }
}

