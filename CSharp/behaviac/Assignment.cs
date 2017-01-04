namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Assignment : BehaviorNode
    {
        protected Property m_opl;
        protected Property m_opr;
        protected CMethodBase m_opr_m;

        protected override BehaviorTask createTask()
        {
            return new AssignmentTask();
        }

        ~Assignment()
        {
            this.m_opl = null;
            this.m_opr = null;
            this.m_opr_m = null;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is Assignment) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            string propertyName = null;
            foreach (property_t _t in properties)
            {
                if (_t.name == "Opl")
                {
                    this.m_opl = Condition.LoadLeft(_t.value, ref propertyName, null);
                }
                else if (_t.name == "Opr")
                {
                    if (_t.value.IndexOf('(') == -1)
                    {
                        string typeName = null;
                        this.m_opr = Condition.LoadRight(_t.value, propertyName, ref typeName);
                    }
                    else
                    {
                        this.m_opr_m = behaviac.Action.LoadMethod(_t.value);
                    }
                }
            }
        }

        private class AssignmentTask : LeafTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~AssignmentTask()
            {
            }

            protected override bool isContinueTicking()
            {
                return false;
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
                EBTStatus status = EBTStatus.BT_SUCCESS;
                Assignment node = (Assignment) base.GetNode();
                if ((node.m_opr_m != null) && (node.m_opl != null))
                {
                    ParentType parentType = node.m_opr_m.GetParentType();
                    Agent parent = pAgent;
                    if (parentType == ParentType.PT_INSTANCE)
                    {
                        parent = Agent.GetInstance(node.m_opr_m.GetInstanceNameString(), parent.GetContextId());
                    }
                    object v = node.m_opr_m.run(parent, pAgent);
                    ParentType type2 = node.m_opl.GetParentType();
                    Agent instance = pAgent;
                    if (type2 == ParentType.PT_INSTANCE)
                    {
                        instance = Agent.GetInstance(node.m_opl.GetInstanceNameString(), instance.GetContextId());
                    }
                    node.m_opl.SetValue(instance, v);
                    return status;
                }
                if ((node.m_opr != null) && (node.m_opl != null))
                {
                    Agent pAgentTo = pAgent;
                    Agent pAgentFrom = pAgent;
                    if (node.m_opl.GetParentType() == ParentType.PT_INSTANCE)
                    {
                        pAgentTo = Agent.GetInstance(node.m_opl.GetInstanceNameString(), pAgentTo.GetContextId());
                    }
                    if (node.m_opr.GetParentType() == ParentType.PT_INSTANCE)
                    {
                        pAgentFrom = Agent.GetInstance(node.m_opr.GetInstanceNameString(), pAgentFrom.GetContextId());
                        if (pAgentFrom == null)
                        {
                            pAgentFrom = pAgentTo;
                        }
                    }
                    node.m_opl.SetFrom(pAgentFrom, node.m_opr, pAgentTo);
                    return status;
                }
                return node.update_impl(pAgent, childStatus);
            }
        }
    }
}

