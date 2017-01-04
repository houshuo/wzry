namespace behaviac
{
    using System;

    internal class WaitforSignalTask : SingeChildTask
    {
        private bool m_bTriggered = false;

        public override bool CheckPredicates(Agent pAgent)
        {
            bool flag = false;
            if (base.m_attachments != null)
            {
                flag = base.CheckPredicates(pAgent);
            }
            return flag;
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
            WaitforSignalTask task = (WaitforSignalTask) target;
            task.m_bTriggered = this.m_bTriggered;
        }

        ~WaitforSignalTask()
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
            this.m_bTriggered = false;
            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);
            CSerializationID attrId = new CSerializationID("triggered");
            node.setAttr<bool>(attrId, this.m_bTriggered);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            if (childStatus != EBTStatus.BT_RUNNING)
            {
                return childStatus;
            }
            if (!this.m_bTriggered)
            {
                this.m_bTriggered = this.CheckPredicates(pAgent);
            }
            if (!this.m_bTriggered)
            {
                return EBTStatus.BT_RUNNING;
            }
            if (base.m_root == null)
            {
                return EBTStatus.BT_SUCCESS;
            }
            return base.update(pAgent, childStatus);
        }
    }
}

