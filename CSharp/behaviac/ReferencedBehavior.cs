namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class ReferencedBehavior : BehaviorNode
    {
        protected string m_referencedBehaviorPath;

        protected override BehaviorTask createTask()
        {
            return new ReferencedBehaviorTask();
        }

        ~ReferencedBehavior()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is ReferencedBehavior) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if (_t.name == "ReferenceFilename")
                {
                    this.m_referencedBehaviorPath = _t.value;
                    bool flag = Workspace.Load(this.m_referencedBehaviorPath);
                }
            }
        }

        private class ReferencedBehaviorTask : SingeChildTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~ReferencedBehaviorTask()
            {
            }

            public override void Init(BehaviorNode node)
            {
                base.Init(node);
            }

            protected override bool isContinueTicking()
            {
                return true;
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                ReferencedBehavior node = base.GetNode() as ReferencedBehavior;
                if (node != null)
                {
                    string name = pAgent.btgetcurrent().GetName();
                    string btMsg = string.Format("{0}[{1}] {2}", name, node.GetId(), node.m_referencedBehaviorPath);
                    LogManager.Log(pAgent, btMsg, EActionResult.EAR_none, LogMode.ELM_jump);
                    pAgent.btreferencetree(node.m_referencedBehaviorPath);
                }
                return EBTStatus.BT_RUNNING;
            }
        }
    }
}

