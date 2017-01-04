namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class WithPrecondition : BehaviorNode
    {
        protected override BehaviorTask createTask()
        {
            return new WithPreconditionTask();
        }

        ~WithPrecondition()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is WithPrecondition) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }
    }
}

