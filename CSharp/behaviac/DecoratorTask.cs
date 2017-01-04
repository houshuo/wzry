namespace behaviac
{
    using System;

    public abstract class DecoratorTask : SingeChildTask
    {
        private bool m_bDecorateWhenChildEnds = false;

        protected DecoratorTask()
        {
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
        }

        protected abstract EBTStatus decorate(EBTStatus status);
        ~DecoratorTask()
        {
        }

        public override void Init(BehaviorNode node)
        {
            base.Init(node);
            DecoratorNode node2 = node as DecoratorNode;
            this.m_bDecorateWhenChildEnds = node2.m_bDecorateWhenChildEnds;
        }

        protected override bool isContinueTicking()
        {
            return true;
        }

        public override void load(ISerializableNode node)
        {
            base.load(node);
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = base.update(pAgent, childStatus);
            if (this.m_bDecorateWhenChildEnds && (status == EBTStatus.BT_RUNNING))
            {
                return EBTStatus.BT_RUNNING;
            }
            EBTStatus status2 = this.decorate(status);
            if (status != EBTStatus.BT_RUNNING)
            {
                BehaviorTask root = base.m_root;
                if (root != null)
                {
                    root.SetStatus(EBTStatus.BT_INVALID);
                }
                this.SetCurrentTask(null);
            }
            return status2;
        }
    }
}

