namespace behaviac
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class DecoratorTime : DecoratorNode
    {
        private Property m_time_var;

        protected override BehaviorTask createTask()
        {
            return new DecoratorTimeTask();
        }

        ~DecoratorTime()
        {
            this.m_time_var = null;
        }

        protected virtual int GetTime(Agent pAgent)
        {
            if (this.m_time_var != null)
            {
                return (int) this.m_time_var.GetValue(pAgent);
            }
            return 0;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if (_t.name == "Time")
                {
                    string typeName = null;
                    string propertyName = null;
                    this.m_time_var = Condition.LoadRight(_t.value, propertyName, ref typeName);
                }
            }
        }

        private class DecoratorTimeTask : DecoratorTask
        {
            private int m_start;
            private int m_time;

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
                DecoratorTime.DecoratorTimeTask task = (DecoratorTime.DecoratorTimeTask) target;
                task.m_start = this.m_start;
                task.m_time = this.m_time;
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                this.m_start += (int) (Time.deltaTime * 1000f);
                if (this.m_start >= this.m_time)
                {
                    return EBTStatus.BT_SUCCESS;
                }
                return EBTStatus.BT_RUNNING;
            }

            ~DecoratorTimeTask()
            {
            }

            private int GetTime(Agent pAgent)
            {
                DecoratorTime node = (DecoratorTime) base.GetNode();
                return ((node == null) ? 0 : node.GetTime(pAgent));
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                base.onenter(pAgent);
                this.m_start = 0;
                this.m_time = this.GetTime(pAgent);
                if (this.m_time <= 0)
                {
                    return false;
                }
                return true;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
                CSerializationID attrId = new CSerializationID("start");
                node.setAttr<int>(attrId, this.m_start);
                CSerializationID nid2 = new CSerializationID("time");
                node.setAttr<int>(nid2, this.m_time);
            }
        }
    }
}

