namespace behaviac
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Security;

    public abstract class BehaviorNode
    {
        protected ListView<BehaviorNode> m_attachments;
        protected bool m_bHasEvents;
        protected ListView<BehaviorNode> m_children;
        private string m_className;
        public CMethodBase m_enterAction;
        public CMethodBase m_exitAction;
        private int m_id;
        protected BehaviorNode m_parent;
        public ListView<Property> m_pars;

        protected BehaviorNode()
        {
        }

        public virtual void AddChild(BehaviorNode pChild)
        {
            pChild.m_parent = this;
            if (this.m_children == null)
            {
                this.m_children = new ListView<BehaviorNode>();
            }
            this.m_children.Add(pChild);
        }

        public void AddPar(string type, string name, string value, string eventParam)
        {
            Property item = Property.Create(type, name, value, false, false);
            if (!string.IsNullOrEmpty(eventParam))
            {
                item.SetRefName(eventParam);
            }
            if (this.m_pars == null)
            {
                this.m_pars = new ListView<Property>();
            }
            this.m_pars.Add(item);
        }

        public void Attach(BehaviorNode pAttachment)
        {
            if (this.m_attachments == null)
            {
                this.m_attachments = new ListView<BehaviorNode>();
            }
            this.m_attachments.Add(pAttachment);
        }

        public void Clear()
        {
            this.m_enterAction = null;
            this.m_exitAction = null;
            if (this.m_pars != null)
            {
                foreach (Property property in this.m_pars)
                {
                    Property.DeleteFromCache(property);
                }
                this.m_pars.Clear();
                this.m_pars = null;
            }
            if (this.m_attachments != null)
            {
                this.m_attachments.Clear();
                this.m_attachments = null;
            }
            if (this.m_children != null)
            {
                this.m_children.Clear();
                this.m_children = null;
            }
        }

        protected static BehaviorNode Create(string className)
        {
            return Workspace.CreateBehaviorNode(className);
        }

        public BehaviorTask CreateAndInitTask()
        {
            BehaviorTask task = this.createTask();
            task.Init(this);
            return task;
        }

        protected abstract BehaviorTask createTask();
        public virtual bool enteraction_impl(Agent pAgent)
        {
            return false;
        }

        public virtual bool exitaction_impl(Agent pAgent)
        {
            return false;
        }

        ~BehaviorNode()
        {
            this.Clear();
        }

        public BehaviorNode GetAttachment(int index)
        {
            if ((this.m_attachments != null) && (index < this.m_attachments.Count))
            {
                return this.m_attachments[index];
            }
            return null;
        }

        public int GetAttachmentsCount()
        {
            if (this.m_attachments != null)
            {
                return this.m_attachments.Count;
            }
            return 0;
        }

        public BehaviorNode GetChild(int index)
        {
            if ((this.m_children != null) && (index < this.m_children.Count))
            {
                return this.m_children[index];
            }
            return null;
        }

        public int GetChildrenCount()
        {
            if (this.m_children != null)
            {
                return this.m_children.Count;
            }
            return 0;
        }

        public string GetClassNameString()
        {
            return this.m_className;
        }

        public int GetId()
        {
            return this.m_id;
        }

        public bool HasEvents()
        {
            return this.m_bHasEvents;
        }

        public virtual bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return true;
        }

        protected static BehaviorNode load(string agentType, SecurityElement node)
        {
            int version = int.Parse(node.Attribute("version"));
            string className = node.Attribute("class");
            BehaviorNode node2 = Create(className);
            if (node2 != null)
            {
                node2.SetClassNameString(className);
                string str2 = node.Attribute("id");
                node2.SetId(Convert.ToInt32(str2));
                node2.load_properties_pars_attachments_children(true, version, agentType, node);
            }
            return node2;
        }

        protected virtual void load(int version, string agentType, List<property_t> properties)
        {
            foreach (property_t _t in properties)
            {
                if (_t.name == "EnterAction")
                {
                    if (!string.IsNullOrEmpty(_t.value))
                    {
                        this.m_enterAction = behaviac.Action.LoadMethod(_t.value);
                    }
                }
                else if ((_t.name == "ExitAction") && !string.IsNullOrEmpty(_t.value))
                {
                    this.m_exitAction = behaviac.Action.LoadMethod(_t.value);
                }
            }
            Workspace.HandleNodeLoaded(this.GetClassNameString().Replace(".", "::"), properties);
        }

        private void load_par(int version, string agentType, SecurityElement node)
        {
            if (node.Tag == "par")
            {
                string name = node.Attribute("name");
                string type = node.Attribute("type").Replace("::", ".");
                string str3 = node.Attribute("value");
                string eventParam = node.Attribute("eventParam");
                this.AddPar(type, name, str3, eventParam);
            }
        }

        protected void load_properties_pars_attachments_children(bool bNode, int version, string agentType, SecurityElement node)
        {
            bool flag = this.HasEvents();
            if (node.Children != null)
            {
                List<property_t> properties = new List<property_t>();
                IEnumerator enumerator = node.Children.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        SecurityElement current = (SecurityElement) enumerator.Current;
                        if (!this.load_property_pars(ref properties, current, version, agentType) && bNode)
                        {
                            if (current.Tag == "attachment")
                            {
                                string className = current.Attribute("class");
                                BehaviorNode pAttachment = Create(className);
                                if (pAttachment != null)
                                {
                                    pAttachment.SetClassNameString(className);
                                    string str2 = current.Attribute("id");
                                    pAttachment.SetId(Convert.ToInt32(str2));
                                    pAttachment.load_properties_pars_attachments_children(false, version, agentType, current);
                                    this.Attach(pAttachment);
                                    flag |= pAttachment is Event;
                                }
                            }
                            else if (current.Tag == "node")
                            {
                                BehaviorNode pChild = load(agentType, current);
                                flag |= pChild.m_bHasEvents;
                                this.AddChild(pChild);
                            }
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
                if (properties.Count > 0)
                {
                    this.load(version, agentType, properties);
                }
            }
            this.m_bHasEvents |= flag;
        }

        private bool load_property_pars(ref List<property_t> properties, SecurityElement c, int version, string agentType)
        {
            if (!(c.Tag == "property"))
            {
                if (!(c.Tag == "pars"))
                {
                    return false;
                }
                if (c.Children != null)
                {
                    IEnumerator enumerator2 = c.Children.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            SecurityElement current = (SecurityElement) enumerator2.Current;
                            if (current.Tag == "par")
                            {
                                this.load_par(version, agentType, current);
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable2 = enumerator2 as IDisposable;
                        if (disposable2 == null)
                        {
                        }
                        disposable2.Dispose();
                    }
                }
                return true;
            }
            if (c.Attributes.Count == 1)
            {
                IEnumerator enumerator = c.Attributes.Keys.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string n = (string) enumerator.Current;
                        string v = (string) c.Attributes[n];
                        property_t item = new property_t(n, v);
                        properties.Add(item);
                        goto Label_0095;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
        Label_0095:
            return true;
        }

        public void SetClassNameString(string className)
        {
            this.m_className = className;
        }

        public void SetHasEvents(bool hasEvents)
        {
            this.m_bHasEvents = hasEvents;
        }

        public void SetId(int id)
        {
            this.m_id = id;
        }

        protected virtual EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            return EBTStatus.BT_FAILURE;
        }
    }
}

