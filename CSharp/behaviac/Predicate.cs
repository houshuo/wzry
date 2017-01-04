namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Predicate : ConditionBase
    {
        protected bool m_bAnd = false;
        protected VariableComparator m_comparator;
        protected Property m_opl;
        protected CMethodBase m_opl_m;
        protected Property m_opr;

        protected override BehaviorTask createTask()
        {
            return new PredicateTask();
        }

        ~Predicate()
        {
            this.m_opl = null;
            this.m_opr = null;
            this.m_opl_m = null;
            this.m_comparator = null;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is Predicate) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            string typeName = null;
            string propertyName = null;
            string str3 = null;
            foreach (property_t _t in properties)
            {
                if (_t.name == "Operator")
                {
                    str3 = _t.value;
                }
                else if (_t.name == "Opl")
                {
                    if (_t.value.IndexOf('(') == -1)
                    {
                        this.m_opl = Condition.LoadLeft(_t.value, ref propertyName, null);
                    }
                    else
                    {
                        this.m_opl_m = behaviac.Action.LoadMethod(_t.value);
                    }
                }
                else if (_t.name == "Opr")
                {
                    this.m_opr = Condition.LoadRight(_t.value, propertyName, ref typeName);
                }
                else if (_t.name == "BinaryOperator")
                {
                    if (_t.value == "Or")
                    {
                        this.m_bAnd = false;
                    }
                    else if (_t.value == "And")
                    {
                        this.m_bAnd = true;
                    }
                }
            }
            if ((!string.IsNullOrEmpty(str3) && ((this.m_opl != null) || (this.m_opl_m != null))) && (this.m_opr != null))
            {
                this.m_comparator = Condition.Create(typeName, str3, this.m_opl, this.m_opr);
            }
        }

        public class PredicateTask : AttachmentTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~PredicateTask()
            {
            }

            public bool IsAnd()
            {
                Predicate node = base.GetNode() as Predicate;
                return node.m_bAnd;
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            public override bool NeedRestart()
            {
                return true;
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
                EBTStatus status = EBTStatus.BT_FAILURE;
                Predicate node = base.GetNode() as Predicate;
                if (node.m_comparator != null)
                {
                    if (Condition.DoCompare(pAgent, node.m_comparator, node.m_opl, node.m_opl_m, node.m_opr))
                    {
                        status = EBTStatus.BT_SUCCESS;
                    }
                    return status;
                }
                return node.update_impl(pAgent, childStatus);
            }
        }
    }
}

