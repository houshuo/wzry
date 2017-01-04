namespace behaviac
{
    using System;

    public class CompositeTask : BranchTask
    {
        protected const int InvalidChildIndex = -1;
        protected int m_activeChildIndex = -1;
        protected ListView<BehaviorTask> m_children = new ListView<BehaviorTask>();

        protected CompositeTask()
        {
            this.m_activeChildIndex = -1;
        }

        protected override void addChild(BehaviorTask pBehavior)
        {
            pBehavior.SetParent(this);
            this.m_children.Add(pBehavior);
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
            CompositeTask task = target as CompositeTask;
            task.m_activeChildIndex = this.m_activeChildIndex;
            int count = this.m_children.Count;
            for (int i = 0; i < count; i++)
            {
                BehaviorTask task2 = this.m_children[i];
                BehaviorTask task3 = task.m_children[i];
                task2.copyto(task3);
            }
        }

        ~CompositeTask()
        {
            this.m_children.Clear();
        }

        public override ListView<BehaviorNode> GetRunningNodes()
        {
            ListView<BehaviorNode> view = new ListView<BehaviorNode>();
            foreach (BehaviorTask task in this.m_children)
            {
                view.AddRange(task.GetRunningNodes());
            }
            return view;
        }

        public override void Init(BehaviorNode node)
        {
            base.Init(node);
            int childrenCount = node.GetChildrenCount();
            for (int i = 0; i < childrenCount; i++)
            {
                BehaviorTask pBehavior = node.GetChild(i).CreateAndInitTask();
                this.addChild(pBehavior);
            }
        }

        public override void load(ISerializableNode node)
        {
            base.load(node);
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);
        }

        public override void traverse(NodeHandler_t handler, Agent pAgent, object user_data)
        {
            if (handler(this, pAgent, user_data))
            {
                for (int i = 0; i < this.m_children.Count; i++)
                {
                    this.m_children[i].traverse(handler, pAgent, user_data);
                }
            }
        }
    }
}

