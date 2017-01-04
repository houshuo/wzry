namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Compute : BehaviorNode
    {
        protected EComputeOperator m_operator;
        protected Property m_opl;
        protected Property m_opr1;
        protected CMethodBase m_opr1_m;
        protected Property m_opr2;
        protected CMethodBase m_opr2_m;

        protected override BehaviorTask createTask()
        {
            return new ComputeTask();
        }

        ~Compute()
        {
            this.m_opl = null;
            this.m_opr1 = null;
            this.m_opr1_m = null;
            this.m_opr2 = null;
            this.m_opr2_m = null;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is Compute) && base.IsValid(pAgent, pTask));
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
                else if (_t.name == "Operator")
                {
                    if (_t.value == "Add")
                    {
                        this.m_operator = EComputeOperator.E_ADD;
                    }
                    else if (_t.value == "Sub")
                    {
                        this.m_operator = EComputeOperator.E_SUB;
                    }
                    else if (_t.value == "Mul")
                    {
                        this.m_operator = EComputeOperator.E_MUL;
                    }
                    else if (_t.value == "Div")
                    {
                        this.m_operator = EComputeOperator.E_DIV;
                    }
                }
                else if (_t.name == "Opr1")
                {
                    if (_t.value.IndexOf('(') == -1)
                    {
                        string typeName = null;
                        this.m_opr1 = Condition.LoadRight(_t.value, propertyName, ref typeName);
                    }
                    else
                    {
                        this.m_opr1_m = behaviac.Action.LoadMethod(_t.value);
                    }
                }
                else if (_t.name == "Opr2")
                {
                    if (_t.value.IndexOf('(') == -1)
                    {
                        string str3 = null;
                        this.m_opr2 = Condition.LoadRight(_t.value, propertyName, ref str3);
                    }
                    else
                    {
                        this.m_opr2_m = behaviac.Action.LoadMethod(_t.value);
                    }
                }
            }
        }

        private class ComputeTask : LeafTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~ComputeTask()
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
                Compute node = (Compute) base.GetNode();
                bool flag = false;
                object obj2 = null;
                if (node.m_opl != null)
                {
                    if (node.m_opr1_m != null)
                    {
                        flag = true;
                        ParentType parentType = node.m_opr1_m.GetParentType();
                        Agent parent = pAgent;
                        if (parentType == ParentType.PT_INSTANCE)
                        {
                            parent = Agent.GetInstance(node.m_opr1_m.GetInstanceNameString(), parent.GetContextId());
                        }
                        obj2 = node.m_opr1_m.run(parent, pAgent);
                    }
                    else if (node.m_opr1 != null)
                    {
                        flag = true;
                        Agent pAgentTo = pAgent;
                        Agent pAgentFrom = pAgent;
                        if (node.m_opl.GetParentType() == ParentType.PT_INSTANCE)
                        {
                            pAgentTo = Agent.GetInstance(node.m_opl.GetInstanceNameString(), pAgentTo.GetContextId());
                        }
                        if (node.m_opr1.GetParentType() == ParentType.PT_INSTANCE)
                        {
                            pAgentFrom = Agent.GetInstance(node.m_opr1.GetInstanceNameString(), pAgentFrom.GetContextId());
                            if (pAgentFrom == null)
                            {
                                pAgentFrom = pAgentTo;
                            }
                        }
                        node.m_opl.SetFrom(pAgentFrom, node.m_opr1, pAgentTo);
                        obj2 = node.m_opl.GetValue(pAgentTo);
                    }
                    if (node.m_opr2_m != null)
                    {
                        flag = true;
                        ParentType type4 = node.m_opr2_m.GetParentType();
                        Agent instance = pAgent;
                        if (type4 == ParentType.PT_INSTANCE)
                        {
                            instance = Agent.GetInstance(node.m_opr2_m.GetInstanceNameString(), instance.GetContextId());
                        }
                        object obj3 = node.m_opr2_m.run(instance, pAgent);
                        ParentType type5 = node.m_opl.GetParentType();
                        Agent agent5 = pAgent;
                        if (type5 == ParentType.PT_INSTANCE)
                        {
                            agent5 = Agent.GetInstance(node.m_opl.GetInstanceNameString(), agent5.GetContextId());
                        }
                        object v = Details.ComputeValue(obj2, obj3, node.m_operator);
                        node.m_opl.SetValue(agent5, v);
                    }
                    else if (node.m_opr2 != null)
                    {
                        flag = true;
                        Agent agent6 = pAgent;
                        Agent agent7 = pAgent;
                        if (node.m_opl.GetParentType() == ParentType.PT_INSTANCE)
                        {
                            agent6 = Agent.GetInstance(node.m_opl.GetInstanceNameString(), agent6.GetContextId());
                        }
                        if (node.m_opr2.GetParentType() == ParentType.PT_INSTANCE)
                        {
                            agent7 = Agent.GetInstance(node.m_opr2.GetInstanceNameString(), agent7.GetContextId());
                            if (agent7 == null)
                            {
                                agent7 = agent6;
                            }
                        }
                        object obj5 = node.m_opr2.GetValue(agent7);
                        object obj6 = Details.ComputeValue(obj2, obj5, node.m_operator);
                        node.m_opl.SetValue(agent6, obj6);
                    }
                }
                if (!flag)
                {
                    status = node.update_impl(pAgent, childStatus);
                }
                return status;
            }
        }
    }
}

