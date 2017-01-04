namespace behaviac
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Wait : BehaviorNode
    {
        protected bool m_ignoreTimeScale = false;
        protected Property m_time_var = null;

        protected override BehaviorTask createTask()
        {
            return new WaitTask();
        }

        ~Wait()
        {
            this.m_time_var = null;
        }

        protected virtual float GetTime(Agent pAgent)
        {
            if (this.m_time_var != null)
            {
                return Convert.ToSingle(this.m_time_var.GetValue(pAgent));
            }
            return 0f;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if (_t.name == "IgnoreTimeScale")
                {
                    this.m_ignoreTimeScale = _t.value == "true";
                }
                else if (_t.name == "Time")
                {
                    string typeName = null;
                    string propertyName = null;
                    this.m_time_var = Condition.LoadRight(_t.value, propertyName, ref typeName);
                }
            }
        }

        private class WaitTask : LeafTask
        {
            private float m_start = 0f;
            private float m_time = 0f;

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
                Wait.WaitTask task = (Wait.WaitTask) target;
                task.m_start = this.m_start;
                task.m_time = this.m_time;
            }

            private bool GetIgnoreTimeScale()
            {
                Wait node = base.GetNode() as Wait;
                return ((node != null) && node.m_ignoreTimeScale);
            }

            private float GetTime(Agent pAgent)
            {
                Wait node = base.GetNode() as Wait;
                return ((node == null) ? 0f : node.GetTime(pAgent));
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                if (this.GetIgnoreTimeScale())
                {
                    this.m_start = Time.realtimeSinceStartup * 1000f;
                }
                else
                {
                    this.m_start = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                }
                this.m_time = this.GetTime(pAgent);
                if (this.m_time <= 0f)
                {
                    return false;
                }
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
                CSerializationID attrId = new CSerializationID("start");
                node.setAttr<float>(attrId, this.m_start);
                CSerializationID nid2 = new CSerializationID("time");
                node.setAttr<float>(nid2, this.m_time);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                if (this.GetIgnoreTimeScale())
                {
                    if (((Time.realtimeSinceStartup * 1000f) - this.m_start) >= this.m_time)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }
                }
                else if ((Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.m_start) >= this.m_time)
                {
                    return EBTStatus.BT_SUCCESS;
                }
                return EBTStatus.BT_RUNNING;
            }
        }
    }
}

