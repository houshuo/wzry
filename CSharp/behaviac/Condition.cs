namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Condition : ConditionBase
    {
        private VariableComparator m_comparator;
        protected Property m_opl;
        private CMethodBase m_opl_m;
        private Property m_opr;

        public static VariableComparator Create(string typeName, string comparionOperator, Property lhs, Property rhs)
        {
            E_VariableComparisonType type = VariableComparator.ParseComparisonType(comparionOperator);
            if (Agent.IsAgentClassName(typeName))
            {
                typeName = "void*";
            }
            VariableComparator comparator = VariableComparator.Create(typeName, lhs, rhs);
            comparator.SetComparisonType(type);
            return comparator;
        }

        protected override BehaviorTask createTask()
        {
            return new ConditionTask();
        }

        public static bool DoCompare(Agent pAgent, VariableComparator comparator, Property opl, CMethodBase opl_m, Property opr)
        {
            bool flag = false;
            if (opl != null)
            {
                Agent agentL = pAgent;
                if (opl.GetParentType() == ParentType.PT_INSTANCE)
                {
                    agentL = Agent.GetInstance(opl.GetInstanceNameString(), agentL.GetContextId());
                }
                Agent instance = pAgent;
                if (opr.GetParentType() == ParentType.PT_INSTANCE)
                {
                    instance = Agent.GetInstance(opr.GetInstanceNameString(), agentL.GetContextId());
                }
                return comparator.Execute(agentL, instance);
            }
            if (opl_m == null)
            {
                return flag;
            }
            ParentType parentType = opl_m.GetParentType();
            Agent parent = pAgent;
            if (parentType == ParentType.PT_INSTANCE)
            {
                parent = Agent.GetInstance(opl_m.GetInstanceNameString(), parent.GetContextId());
            }
            object lhs = opl_m.run(parent, pAgent);
            Agent agentR = pAgent;
            if (opr.GetParentType() == ParentType.PT_INSTANCE)
            {
                agentR = Agent.GetInstance(opr.GetInstanceNameString(), agentR.GetContextId());
            }
            return comparator.Execute(lhs, parent, agentR);
        }

        ~Condition()
        {
            this.m_opl = null;
            this.m_opr = null;
            this.m_opl_m = null;
            this.m_comparator = null;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is Condition) && base.IsValid(pAgent, pTask));
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
                        this.m_opl = LoadLeft(_t.value, ref propertyName, null);
                    }
                    else
                    {
                        this.m_opl_m = behaviac.Action.LoadMethod(_t.value);
                    }
                }
                else if (_t.name == "Opr")
                {
                    this.m_opr = LoadRight(_t.value, propertyName, ref typeName);
                }
            }
            if ((!string.IsNullOrEmpty(str3) && ((this.m_opl != null) || (this.m_opl_m != null))) && (this.m_opr != null))
            {
                this.m_comparator = Create(typeName, str3, this.m_opl, this.m_opr);
            }
        }

        public static Property LoadLeft(string value, ref string propertyName, string constValue)
        {
            Property property = null;
            if (string.IsNullOrEmpty(value))
            {
                return property;
            }
            char[] separator = new char[] { ' ' };
            string[] strArray = value.Split(separator);
            if ((strArray != null) && (strArray.Length == 2))
            {
                string str = strArray[0].Replace("::", ".");
                propertyName = strArray[1];
                return Property.Create(str, strArray[1], constValue, false, false);
            }
            DebugHelper.Assert(((strArray != null) && (strArray.Length > 0)) && (strArray[0] == "static"));
            string typeName = strArray[1].Replace("::", ".");
            propertyName = strArray[2];
            return Property.Create(typeName, strArray[2], constValue, true, false);
        }

        public static Property LoadProperty(string value)
        {
            string propertyName = null;
            string typeName = null;
            return LoadRight(value, propertyName, ref typeName);
        }

        public static Property LoadRight(string value, string propertyName, ref string typeName)
        {
            Property property = null;
            if (string.IsNullOrEmpty(value))
            {
                return property;
            }
            if (value.StartsWith("const"))
            {
                string str = value.Substring(6);
                int num2 = StringUtils.FirstToken(str, ' ', ref typeName);
                typeName = typeName.Replace("::", ".");
                string str2 = str.Substring(num2 + 1);
                return Property.Create(typeName, propertyName, str2, false, true);
            }
            char[] separator = new char[] { ' ' };
            string[] strArray = value.Split(separator);
            if (strArray[0] == "static")
            {
                typeName = strArray[1].Replace("::", ".");
                return Property.Create(typeName, strArray[2], null, true, false);
            }
            typeName = strArray[0].Replace("::", ".");
            return Property.Create(typeName, strArray[1], null, false, false);
        }

        public static bool Register<T>(string typeName)
        {
            return true;
        }

        public static void RegisterBasicTypes()
        {
        }

        public static void UnRegister<T>(string typeName)
        {
        }

        public static void UnRegisterBasicTypes()
        {
        }

        private class ConditionTask : ConditionBaseTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~ConditionTask()
            {
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
                EBTStatus status = EBTStatus.BT_FAILURE;
                Condition node = (Condition) base.GetNode();
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

