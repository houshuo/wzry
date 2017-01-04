namespace behaviac
{
    using System;

    public class LeafTask : BehaviorTask
    {
        protected LeafTask()
        {
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
        }

        ~LeafTask()
        {
        }

        public override ListView<BehaviorNode> GetRunningNodes()
        {
            ListView<BehaviorNode> view = new ListView<BehaviorNode>();
            if (this.isContinueTicking() && (base.GetStatus() == EBTStatus.BT_RUNNING))
            {
                view.Add(base.GetNode());
            }
            return view;
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

        public override bool onevent(Agent pAgent, string eventName)
        {
            return base.onevent(pAgent, eventName);
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);
        }

        public override void traverse(NodeHandler_t handler, Agent pAgent, object user_data)
        {
            handler(this, pAgent, user_data);
        }
    }
}

