namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class WaitFrames : BehaviorNode
    {
        private CMethodBase m_frames_method;
        private Property m_frames_var;

        protected override BehaviorTask createTask()
        {
            return new WaitFramesTask();
        }

        ~WaitFrames()
        {
        }

        protected virtual int GetFrames(Agent pAgent)
        {
            if (this.m_frames_var != null)
            {
                return (int) this.m_frames_var.GetValue(pAgent);
            }
            if (this.m_frames_method == null)
            {
                return 0;
            }
            ParentType parentType = this.m_frames_method.GetParentType();
            Agent parent = pAgent;
            if (parentType == ParentType.PT_INSTANCE)
            {
                parent = Agent.GetInstance(this.m_frames_method.GetInstanceNameString(), parent.GetContextId());
            }
            return (int) this.m_frames_method.run(parent, pAgent);
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if (_t.name == "Frames")
                {
                    string propertyName = null;
                    if (_t.value.IndexOf('(') == -1)
                    {
                        string typeName = null;
                        this.m_frames_var = Condition.LoadRight(_t.value, propertyName, ref typeName);
                    }
                    else
                    {
                        this.m_frames_method = behaviac.Action.LoadMethod(_t.value);
                    }
                }
            }
        }

        private class WaitFramesTask : LeafTask
        {
            private int m_frames;
            private int m_start;

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
                WaitFrames.WaitFramesTask task = (WaitFrames.WaitFramesTask) target;
                task.m_start = this.m_start;
                task.m_frames = this.m_frames;
            }

            ~WaitFramesTask()
            {
            }

            private int GetFrames(Agent pAgent)
            {
                WaitFrames node = (WaitFrames) base.GetNode();
                return ((node == null) ? 0 : node.GetFrames(pAgent));
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                this.m_start = 0;
                this.m_frames = this.GetFrames(pAgent);
                if (this.m_frames <= 0)
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
                node.setAttr<int>(attrId, this.m_start);
                CSerializationID nid2 = new CSerializationID("frames");
                node.setAttr<int>(nid2, this.m_frames);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                this.m_start += Workspace.GetDeltaFrames();
                if (this.m_start >= this.m_frames)
                {
                    return EBTStatus.BT_SUCCESS;
                }
                return EBTStatus.BT_RUNNING;
            }
        }
    }
}

