namespace behaviac
{
    using System;

    internal class WithPreconditionTask : Sequence.SequenceTask
    {
        public BehaviorTask Action()
        {
            return base.m_children[1];
        }

        protected override void addChild(BehaviorTask pBehavior)
        {
            base.addChild(pBehavior);
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
        }

        public override void load(ISerializableNode node)
        {
            base.load(node);
        }

        protected override bool onenter(Agent pAgent)
        {
            BehaviorTask parent = base.GetParent();
            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
            BehaviorTask parent = base.GetParent();
        }

        public BehaviorTask PreconditionNode()
        {
            return base.m_children[0];
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            BehaviorTask parent = base.GetParent();
            return EBTStatus.BT_RUNNING;
        }
    }
}

