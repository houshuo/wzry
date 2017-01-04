namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class DecoratorCountLimit : DecoratorCount
    {
        protected override BehaviorTask createTask()
        {
            return new DecoratorCountLimitTask();
        }

        ~DecoratorCountLimit()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is DecoratorCountLimit) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        private class DecoratorCountLimitTask : DecoratorCount.DecoratorCountTask
        {
            private bool m_bInited;

            public override bool CheckPredicates(Agent pAgent)
            {
                bool flag = false;
                if (base.m_attachments != null)
                {
                    flag = base.CheckPredicates(pAgent);
                }
                return flag;
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
                DecoratorCountLimit.DecoratorCountLimitTask task = (DecoratorCountLimit.DecoratorCountLimitTask) target;
                task.m_bInited = this.m_bInited;
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                return status;
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                if (this.CheckPredicates(pAgent))
                {
                    this.m_bInited = false;
                }
                if (!this.m_bInited)
                {
                    this.m_bInited = true;
                    int count = base.GetCount(pAgent);
                    base.m_n = count;
                }
                if (base.m_n > 0)
                {
                    base.m_n--;
                    return true;
                }
                if (base.m_n == 0)
                {
                    return false;
                }
                return (base.m_n == -1);
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
                CSerializationID attrId = new CSerializationID("inited");
                node.setAttr<bool>(attrId, this.m_bInited);
            }
        }
    }
}

