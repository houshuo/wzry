namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class And : ConditionBase
    {
        protected override BehaviorTask createTask()
        {
            return new AndTask();
        }

        ~And()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is And) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }
    }
}

