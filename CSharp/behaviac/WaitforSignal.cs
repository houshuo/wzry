namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class WaitforSignal : BehaviorNode
    {
        protected override BehaviorTask createTask()
        {
            return new WaitforSignalTask();
        }

        ~WaitforSignal()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is WaitforSignal) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }
    }
}

