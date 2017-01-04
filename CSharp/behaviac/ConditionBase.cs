namespace behaviac
{
    using System;

    public abstract class ConditionBase : BehaviorNode
    {
        ~ConditionBase()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is ConditionBase) && base.IsValid(pAgent, pTask));
        }
    }
}

