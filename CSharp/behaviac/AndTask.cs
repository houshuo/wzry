namespace behaviac
{
    using System;

    internal class AndTask : Sequence.SequenceTask
    {
        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
        }

        ~AndTask()
        {
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
            for (int i = 0; i < base.m_children.Count; i++)
            {
                EBTStatus status = base.m_children[i].exec(pAgent);
                if (status == EBTStatus.BT_FAILURE)
                {
                    return status;
                }
            }
            return EBTStatus.BT_SUCCESS;
        }
    }
}

