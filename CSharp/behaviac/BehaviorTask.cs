namespace behaviac
{
    using System;

    public abstract class BehaviorTask
    {
        private static NodeHandler_t abort_handler_ = new NodeHandler_t(BehaviorTask.abort_handler);
        protected ListView<AttachmentTask> m_attachments = null;
        private int m_id;
        protected BehaviorNode m_node = null;
        protected BranchTask m_parent = null;
        public EBTStatus m_status = EBTStatus.BT_INVALID;
        private static EBTStatus ms_lastExitStatus_ = EBTStatus.BT_INVALID;
        private static NodeHandler_t reset_handler_ = new NodeHandler_t(BehaviorTask.reset_handler);

        protected BehaviorTask()
        {
        }

        public void abort(Agent pAgent)
        {
            this.traverse(abort_handler_, pAgent, null);
        }

        private static bool abort_handler(BehaviorTask node, Agent pAgent, object user_data)
        {
            if (node.m_status == EBTStatus.BT_RUNNING)
            {
                node.onexit_action(pAgent, EBTStatus.BT_FAILURE);
                node.m_status = EBTStatus.BT_FAILURE;
                node.SetCurrentTask(null);
            }
            return true;
        }

        private void Attach(AttachmentTask pAttachment)
        {
            if (this.m_attachments == null)
            {
                this.m_attachments = new ListView<AttachmentTask>();
            }
            this.m_attachments.Add(pAttachment);
        }

        public bool CheckEvents(string eventName, Agent pAgent)
        {
            if (this.m_attachments != null)
            {
                for (int i = 0; i < this.m_attachments.Count; i++)
                {
                    AttachmentTask task = this.m_attachments[i];
                    Event.EventTask task2 = task as Event.EventTask;
                    if ((task2 != null) && !string.IsNullOrEmpty(eventName))
                    {
                        string str = task2.GetEventName();
                        if (!string.IsNullOrEmpty(str) && (str == eventName))
                        {
                            EBTStatus status = task.GetStatus();
                            if ((status == EBTStatus.BT_RUNNING) || (status == EBTStatus.BT_INVALID))
                            {
                                status = task.exec(pAgent);
                            }
                            switch (status)
                            {
                                case EBTStatus.BT_SUCCESS:
                                    if (!task2.TriggeredOnce())
                                    {
                                        break;
                                    }
                                    return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public virtual bool CheckPredicates(Agent pAgent)
        {
            if ((this.m_attachments == null) || (this.m_attachments.Count == 0))
            {
                return true;
            }
            bool flag = false;
            for (int i = 0; i < this.m_attachments.Count; i++)
            {
                AttachmentTask task = this.m_attachments[i];
                Predicate.PredicateTask task2 = task as Predicate.PredicateTask;
                if (task2 != null)
                {
                    EBTStatus status = task2.GetStatus();
                    switch (status)
                    {
                        case EBTStatus.BT_RUNNING:
                        case EBTStatus.BT_INVALID:
                            status = task2.exec(pAgent);
                            break;
                    }
                    bool flag2 = this.getBooleanFromStatus(status);
                    if (i == 0)
                    {
                        flag = flag2;
                    }
                    else if (task2.IsAnd())
                    {
                        flag = flag && flag2;
                    }
                    else
                    {
                        flag = flag || flag2;
                    }
                }
            }
            return flag;
        }

        public virtual void Clear()
        {
            this.m_status = EBTStatus.BT_INVALID;
            this.m_parent = null;
            this.m_id = -1;
            this.FreeAttachments();
            this.m_node = null;
        }

        public virtual void copyto(BehaviorTask target)
        {
            target.m_status = this.m_status;
        }

        public static void DestroyTask(BehaviorTask task)
        {
        }

        public EBTStatus exec(Agent pAgent)
        {
            bool flag = false;
            if (this.m_status == EBTStatus.BT_RUNNING)
            {
                flag = true;
            }
            else
            {
                this.m_status = EBTStatus.BT_INVALID;
                flag = this.onenter_action(pAgent);
                if (this.isContinueTicking())
                {
                    BranchTask parentBranch = this.GetParentBranch();
                    if ((parentBranch != null) && (parentBranch != this))
                    {
                        parentBranch.SetCurrentTask(this);
                    }
                }
            }
            if (flag)
            {
                EBTStatus returnStatus = this.GetReturnStatus();
                if (returnStatus == EBTStatus.BT_INVALID)
                {
                    this.m_status = this.update(pAgent, EBTStatus.BT_RUNNING);
                }
                else
                {
                    this.m_status = returnStatus;
                }
                if (this.m_status != EBTStatus.BT_RUNNING)
                {
                    if (this.isContinueTicking())
                    {
                        BranchTask task2 = this.GetParentBranch();
                        if ((task2 != null) && (task2 != this))
                        {
                            task2.SetCurrentTask(null);
                        }
                    }
                    this.onexit_action(pAgent, this.m_status);
                }
            }
            else
            {
                this.m_status = EBTStatus.BT_FAILURE;
            }
            EBTStatus status2 = this.m_status;
            if ((this.m_status != EBTStatus.BT_RUNNING) && this.NeedRestart())
            {
                this.m_status = EBTStatus.BT_INVALID;
                this.SetReturnStatus(EBTStatus.BT_INVALID);
            }
            return status2;
        }

        ~BehaviorTask()
        {
        }

        protected void FreeAttachments()
        {
            if (this.m_attachments != null)
            {
                this.m_attachments.Clear();
                this.m_attachments = null;
            }
        }

        private bool getBooleanFromStatus(EBTStatus status)
        {
            if (status == EBTStatus.BT_FAILURE)
            {
                return false;
            }
            return (status == EBTStatus.BT_SUCCESS);
        }

        public string GetClassNameString()
        {
            if (this.m_node != null)
            {
                return this.m_node.GetClassNameString();
            }
            return "SubBT";
        }

        public int GetId()
        {
            return this.m_id;
        }

        public BehaviorNode GetNode()
        {
            return this.m_node;
        }

        public static EBTStatus GetNodeExitStatus()
        {
            return ms_lastExitStatus_;
        }

        public BranchTask GetParent()
        {
            return this.m_parent;
        }

        public BranchTask GetParentBranch()
        {
            for (BehaviorTask task = this.m_parent; task != null; task = task.m_parent)
            {
                BranchTask task2 = task as BranchTask;
                if ((task2 != null) && task2.isContinueTicking())
                {
                    return task2;
                }
            }
            return null;
        }

        public virtual EBTStatus GetReturnStatus()
        {
            return EBTStatus.BT_INVALID;
        }

        public virtual ListView<BehaviorNode> GetRunningNodes()
        {
            return new ListView<BehaviorNode>();
        }

        public EBTStatus GetStatus()
        {
            return this.m_status;
        }

        public virtual void Init(BehaviorNode node)
        {
            this.m_node = node;
            this.m_id = this.m_node.GetId();
            int attachmentsCount = node.GetAttachmentsCount();
            if (attachmentsCount > 0)
            {
                for (int i = 0; i < attachmentsCount; i++)
                {
                    AttachmentTask pAttachment = (AttachmentTask) node.GetAttachment(i).CreateAndInitTask();
                    this.Attach(pAttachment);
                }
            }
        }

        private void InstantiatePars(Agent pAgent)
        {
            BehaviorNode node = this.m_node;
            if ((node != null) && (node.m_pars != null))
            {
                for (int i = 0; i < node.m_pars.Count; i++)
                {
                    node.m_pars[i].Instantiate(pAgent);
                }
            }
        }

        protected virtual bool isContinueTicking()
        {
            return false;
        }

        public virtual void load(ISerializableNode node)
        {
        }

        public virtual bool NeedRestart()
        {
            return false;
        }

        protected virtual bool onenter(Agent pAgent)
        {
            return true;
        }

        public bool onenter_action(Agent pAgent)
        {
            this.InstantiatePars(pAgent);
            bool flag = this.onenter(pAgent);
            if (((this.m_node != null) && !this.m_node.enteraction_impl(pAgent)) && (this.m_node.m_enterAction != null))
            {
                ParentType parentType = this.m_node.m_enterAction.GetParentType();
                Agent parent = pAgent;
                if (parentType == ParentType.PT_INSTANCE)
                {
                    parent = Agent.GetInstance(this.m_node.m_enterAction.GetInstanceNameString(), parent.GetContextId());
                }
                this.m_node.m_enterAction.run(parent, pAgent);
            }
            if (!flag)
            {
                this.UnInstantiatePars(pAgent);
            }
            return flag;
        }

        public virtual bool onevent(Agent pAgent, string eventName)
        {
            if (((this.m_status == EBTStatus.BT_RUNNING) && this.m_node.HasEvents()) && !this.CheckEvents(eventName, pAgent))
            {
                return false;
            }
            return true;
        }

        protected virtual void onexit(Agent pAgent, EBTStatus status)
        {
        }

        public void onexit_action(Agent pAgent, EBTStatus status)
        {
            this.onexit(pAgent, status);
            this.SetReturnStatus(EBTStatus.BT_INVALID);
            if (this.m_node != null)
            {
                bool flag = this.m_node.exitaction_impl(pAgent);
                if (flag || (this.m_node.m_exitAction != null))
                {
                    Agent parent = pAgent;
                    if ((!flag && (this.m_node.m_exitAction != null)) && (this.m_node.m_exitAction.GetParentType() == ParentType.PT_INSTANCE))
                    {
                        parent = Agent.GetInstance(this.m_node.m_exitAction.GetInstanceNameString(), parent.GetContextId());
                    }
                    if (!flag && (this.m_node.m_exitAction != null))
                    {
                        ms_lastExitStatus_ = status;
                        this.m_node.m_exitAction.run(parent, pAgent);
                        ms_lastExitStatus_ = EBTStatus.BT_INVALID;
                    }
                }
            }
            this.UnInstantiatePars(pAgent);
        }

        public void reset(Agent pAgent)
        {
            this.traverse(reset_handler_, pAgent, null);
        }

        private static bool reset_handler(BehaviorTask node, Agent pAgent, object user_data)
        {
            node.m_status = EBTStatus.BT_INVALID;
            node.SetCurrentTask(null);
            return true;
        }

        public virtual void save(ISerializableNode node)
        {
        }

        public virtual void SetCurrentTask(BehaviorTask node)
        {
        }

        public void SetId(int id)
        {
            this.m_id = id;
        }

        public void SetParent(BranchTask parent)
        {
            this.m_parent = parent;
        }

        public virtual void SetReturnStatus(EBTStatus status)
        {
        }

        public void SetStatus(EBTStatus s)
        {
            this.m_status = s;
        }

        public abstract void traverse(NodeHandler_t handler, Agent pAgent, object user_data);
        private void UnInstantiatePars(Agent pAgent)
        {
            BehaviorNode node = this.m_node;
            if ((node != null) && (node.m_pars != null))
            {
                for (int i = 0; i < node.m_pars.Count; i++)
                {
                    node.m_pars[i].UnInstantiate(pAgent);
                }
            }
        }

        protected virtual EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            return EBTStatus.BT_SUCCESS;
        }
    }
}

