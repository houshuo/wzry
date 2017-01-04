namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class DecoratorFrames : DecoratorNode
    {
        private Property m_frames_var;

        protected override BehaviorTask createTask()
        {
            return new DecoratorFramesTask();
        }

        ~DecoratorFrames()
        {
            this.m_frames_var = null;
        }

        protected virtual int GetFrames(Agent pAgent)
        {
            if (this.m_frames_var != null)
            {
                return (int) this.m_frames_var.GetValue(pAgent);
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
                    this.m_frames_var = Condition.LoadRight(_t.value, propertyName, ref typeName);
                }
            }
        }

        private class DecoratorFramesTask : DecoratorTask
        {
            private int m_frames;
            private int m_start;

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
                DecoratorFrames.DecoratorFramesTask task = (DecoratorFrames.DecoratorFramesTask) target;
                task.m_start = this.m_start;
                task.m_frames = this.m_frames;
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                this.m_start += Workspace.GetDeltaFrames();
                if (this.m_start >= this.m_frames)
                {
                    return EBTStatus.BT_SUCCESS;
                }
                return EBTStatus.BT_RUNNING;
            }

            ~DecoratorFramesTask()
            {
            }

            private int GetFrames(Agent pAgent)
            {
                DecoratorFrames node = (DecoratorFrames) base.GetNode();
                return ((node == null) ? 0 : node.GetFrames(pAgent));
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                base.onenter(pAgent);
                this.m_start = 0;
                this.m_frames = this.GetFrames(pAgent);
                return (this.m_frames > 0);
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
                CSerializationID attrId = new CSerializationID("start");
                node.setAttr<int>(attrId, this.m_start);
                CSerializationID nid2 = new CSerializationID("frames");
                node.setAttr<int>(nid2, this.m_frames);
            }
        }
    }
}

