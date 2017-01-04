namespace behaviac
{
    using System;

    public class BehaviorTreeTask : SingeChildTask
    {
        public override void Clear()
        {
            base.Clear();
            base.m_currentTask = null;
            base.m_returnStatus = EBTStatus.BT_INVALID;
            base.m_root = null;
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);
        }

        public void CopyTo(BehaviorTreeTask target)
        {
            this.copyto(target);
        }

        ~BehaviorTreeTask()
        {
        }

        public string GetName()
        {
            BehaviorTree node = base.m_node as BehaviorTree;
            return node.GetName();
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

        public void Load(ISerializableNode node)
        {
            this.load(node);
        }

        public override bool NeedRestart()
        {
            BehaviorTask root = base.m_root;
            return ((root != null) && root.NeedRestart());
        }

        protected override bool onenter(Agent pAgent)
        {
            return true;
        }

        public override bool onevent(Agent pAgent, string eventName)
        {
            return ((!base.m_node.HasEvents() || !base.m_root.onevent(pAgent, eventName)) || ((((base.m_status == EBTStatus.BT_RUNNING) && base.m_node.HasEvents()) && !base.CheckEvents(eventName, pAgent)) && false));
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
        }

        public EBTStatus resume(Agent pAgent, EBTStatus status)
        {
            BranchTask parent = null;
            BehaviorTask currentTask = base.m_currentTask;
            while (currentTask != null)
            {
                BranchTask task3 = currentTask as BranchTask;
                if (task3 != null)
                {
                    parent = task3;
                    currentTask = task3.GetCurrentTask();
                }
                else
                {
                    parent = currentTask.GetParent();
                    break;
                }
            }
            if (parent != null)
            {
                parent.onexit_action(pAgent, status);
                parent.SetReturnStatus(status);
            }
            return status;
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);
        }

        public void Save(ISerializableNode node)
        {
        }

        public void SetRootTask(BehaviorTask pRoot)
        {
            this.addChild(pRoot);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = base.update(pAgent, childStatus);
            if (status != EBTStatus.BT_RUNNING)
            {
                this.SetCurrentTask(null);
            }
            return status;
        }
    }
}

