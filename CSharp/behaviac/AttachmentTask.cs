namespace behaviac
{
    using System;

    public class AttachmentTask : BehaviorTask
    {
        protected AttachmentTask()
        {
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
        }

        ~AttachmentTask()
        {
        }

        public override void Init(BehaviorNode node)
        {
            base.Init(node);
        }

        public override void traverse(NodeHandler_t handler, Agent pAgent, object user_data)
        {
            handler(this, pAgent, user_data);
        }
    }
}

