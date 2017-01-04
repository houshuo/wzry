namespace behaviac
{
    using System;

    public abstract class BranchTask : BehaviorTask
    {
        protected BehaviorTask m_currentTask;
        protected EBTStatus m_returnStatus = EBTStatus.BT_INVALID;

        protected BranchTask()
        {
        }

        protected abstract void addChild(BehaviorTask pBehavior);
        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
        }

        ~BranchTask()
        {
        }

        public BehaviorTask GetCurrentTask()
        {
            return this.m_currentTask;
        }

        public override EBTStatus GetReturnStatus()
        {
            return this.m_returnStatus;
        }

        public override void Init(BehaviorNode node)
        {
            base.Init(node);
        }

        protected override bool isContinueTicking()
        {
            return false;
        }

        public override void load(ISerializableNode node)
        {
            base.load(node);
        }

        public override bool onevent(Agent pAgent, string eventName)
        {
            bool flag = true;
            if ((base.m_status == EBTStatus.BT_RUNNING) && base.m_node.HasEvents())
            {
                flag = this.oneventCurrentNode(pAgent, eventName);
            }
            return flag;
        }

        private bool oneventCurrentNode(Agent pAgent, string eventName)
        {
            EBTStatus status = this.m_currentTask.GetStatus();
            bool flag = this.m_currentTask.onevent(pAgent, eventName);
            if (flag)
            {
                for (BranchTask task = this.m_currentTask.GetParent(); (task != null) && (task != this); task = task.GetParent())
                {
                    flag = task.onevent(pAgent, eventName);
                    if (!flag)
                    {
                        return false;
                    }
                }
            }
            return flag;
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);
        }

        public override void SetCurrentTask(BehaviorTask node)
        {
            this.m_currentTask = node;
        }

        public override void SetReturnStatus(EBTStatus status)
        {
            this.m_returnStatus = status;
        }

        protected EBTStatus tickCurrentNode(Agent pAgent)
        {
            EBTStatus status = this.m_currentTask.GetStatus();
            if ((status != EBTStatus.BT_INVALID) && (status != EBTStatus.BT_RUNNING))
            {
                return status;
            }
            BehaviorTask currentTask = this.m_currentTask;
            EBTStatus childStatus = this.m_currentTask.exec(pAgent);
            if (childStatus != EBTStatus.BT_RUNNING)
            {
                BranchTask parent = currentTask.GetParent();
                this.SetCurrentTask(null);
                while ((parent != null) && (parent != this))
                {
                    childStatus = parent.update(pAgent, childStatus);
                    if (childStatus == EBTStatus.BT_RUNNING)
                    {
                        return EBTStatus.BT_RUNNING;
                    }
                    parent.onexit_action(pAgent, childStatus);
                    parent.m_status = childStatus;
                    parent = parent.GetParent();
                }
            }
            return childStatus;
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_INVALID;
            if ((this.m_currentTask != null) && (this.m_currentTask.GetStatus() != EBTStatus.BT_RUNNING))
            {
                this.SetCurrentTask(null);
            }
            if (this.m_currentTask != null)
            {
                status = this.tickCurrentNode(pAgent);
            }
            return status;
        }
    }
}

