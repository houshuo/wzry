namespace behaviac
{
    using System;

    public class VariableComparator
    {
        protected E_VariableComparisonType m_comparisonType;
        protected Property m_lhs;
        protected Property m_rhs;

        private VariableComparator(VariableComparator copy)
        {
            this.m_lhs = copy.m_lhs;
            this.m_rhs = copy.m_rhs;
        }

        public VariableComparator(Property lhs, Property rhs)
        {
            this.m_lhs = lhs;
            this.m_rhs = rhs;
        }

        public VariableComparator clone()
        {
            return new VariableComparator(this);
        }

        public static VariableComparator Create(string typeName, Property lhs, Property rhs)
        {
            return new VariableComparator(lhs, rhs);
        }

        public bool Execute(Agent agentL, Agent agentR)
        {
            object objA = this.m_lhs.GetValue(agentL);
            object obj3 = this.m_rhs.GetValue(agentR);
            switch (this.m_comparisonType)
            {
                case E_VariableComparisonType.VariableComparisonType_Equal:
                    if (!object.ReferenceEquals(objA, null))
                    {
                        return objA.Equals(obj3);
                    }
                    return object.ReferenceEquals(obj3, null);

                case E_VariableComparisonType.VariableComparisonType_NotEqual:
                    if (!object.ReferenceEquals(objA, null))
                    {
                        return !objA.Equals(obj3);
                    }
                    return !object.ReferenceEquals(obj3, null);

                case E_VariableComparisonType.VariableComparisonType_Greater:
                    return Details.Greater(objA, obj3);

                case E_VariableComparisonType.VariableComparisonType_GreaterEqual:
                    return Details.GreaterEqual(objA, obj3);

                case E_VariableComparisonType.VariableComparisonType_Less:
                    return Details.Less(objA, obj3);

                case E_VariableComparisonType.VariableComparisonType_LessEqual:
                    return Details.LessEqual(objA, obj3);
            }
            return false;
        }

        public bool Execute(object lhs, Agent parent, Agent agentR)
        {
            object obj2 = this.m_rhs.GetValue(agentR);
            switch (this.m_comparisonType)
            {
                case E_VariableComparisonType.VariableComparisonType_Equal:
                    return lhs.Equals(obj2);

                case E_VariableComparisonType.VariableComparisonType_NotEqual:
                    return !lhs.Equals(obj2);

                case E_VariableComparisonType.VariableComparisonType_Greater:
                    return Details.Greater(lhs, obj2);

                case E_VariableComparisonType.VariableComparisonType_GreaterEqual:
                    return Details.GreaterEqual(lhs, obj2);

                case E_VariableComparisonType.VariableComparisonType_Less:
                    return Details.Less(lhs, obj2);

                case E_VariableComparisonType.VariableComparisonType_LessEqual:
                    return Details.LessEqual(lhs, obj2);
            }
            return false;
        }

        ~VariableComparator()
        {
            this.m_lhs = null;
            this.m_rhs = null;
        }

        public static E_VariableComparisonType ParseComparisonType(string comparionOperator)
        {
            if (comparionOperator != "Equal")
            {
                if (comparionOperator == "NotEqual")
                {
                    return E_VariableComparisonType.VariableComparisonType_NotEqual;
                }
                if (comparionOperator == "Greater")
                {
                    return E_VariableComparisonType.VariableComparisonType_Greater;
                }
                if (comparionOperator == "GreaterEqual")
                {
                    return E_VariableComparisonType.VariableComparisonType_GreaterEqual;
                }
                if (comparionOperator == "Less")
                {
                    return E_VariableComparisonType.VariableComparisonType_Less;
                }
                if (comparionOperator == "LessEqual")
                {
                    return E_VariableComparisonType.VariableComparisonType_LessEqual;
                }
            }
            return E_VariableComparisonType.VariableComparisonType_Equal;
        }

        public void SetComparisonType(E_VariableComparisonType type)
        {
            this.m_comparisonType = type;
        }
    }
}

