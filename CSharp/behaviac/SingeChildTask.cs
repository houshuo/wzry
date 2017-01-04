namespace behaviac
{
    using System;

    public class SingeChildTask : BranchTask
    {
        protected BehaviorTask m_root = null;

        protected SingeChildTask()
        {
        }

        protected override void addChild(BehaviorTask pBehavior)
        {
            pBehavior.SetParent(this);
            this.m_root = pBehavior;
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
            SingeChildTask task = target as SingeChildTask;
            if (this.m_root != null)
            {
                if (task.m_root == null)
                {
                    task.m_root = this.m_root.GetNode().CreateAndInitTask();
                }
                this.m_root.copyto(task.m_root);
            }
        }

        ~SingeChildTask()
        {
            this.m_root = null;
        }

        public override ListView<BehaviorNode> GetRunningNodes()
        {
            ListView<BehaviorNode> view = new ListView<BehaviorNode>();
            BehaviorTask currentTask = base.GetCurrentTask();
            if (currentTask == null)
            {
                currentTask = this.m_root;
            }
            if (currentTask != null)
            {
                view.AddRange(currentTask.GetRunningNodes());
            }
            return view;
        }

        public override void Init(BehaviorNode node)
        {
            base.Init(node);
            if (node.GetChildrenCount() == 1)
            {
                BehaviorNode child = node.GetChild(0);
                if (child != null)
                {
                    BehaviorTask pBehavior = child.CreateAndInitTask();
                    this.addChild(pBehavior);
                }
            }
        }

        public override void load(ISerializableNode node)
        {
            base.load(node);
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);
            if (this.m_root != null)
            {
            }
        }

        public override void traverse(NodeHandler_t handler, Agent pAgent, object user_data)
        {
            if (handler(this, pAgent, user_data) && (this.m_root != null))
            {
                this.m_root.traverse(handler, pAgent, user_data);
            }
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            if (base.m_currentTask != null)
            {
                return base.update(pAgent, childStatus);
            }
            if (this.m_root != null)
            {
                return this.m_root.exec(pAgent);
            }
            return EBTStatus.BT_INVALID;
        }
    }
}

