namespace behaviac
{
    using System;

    public class Property
    {
        protected readonly bool m_bIsConst;
        private bool m_bValidDefaultValue;
        protected object m_defaultValue;
        protected string m_instanceName;
        protected readonly CMemberBase m_memberBase;
        protected ParentType m_pt;
        protected string m_refParName;
        protected uint m_refParNameId;
        protected string m_varaibleFullName;
        protected string m_varaibleName;
        protected uint m_variableId;
        private static DictionaryView<string, Property> ms_properties = new DictionaryView<string, Property>();

        protected Property(Property copy)
        {
            this.m_varaibleName = copy.m_varaibleName;
            this.m_varaibleFullName = copy.m_varaibleFullName;
            this.m_variableId = copy.m_variableId;
            this.m_refParName = copy.m_refParName;
            this.m_refParNameId = copy.m_refParNameId;
            this.m_memberBase = copy.m_memberBase;
            this.m_pt = copy.m_pt;
            this.m_instanceName = copy.m_instanceName;
            this.m_bValidDefaultValue = copy.m_bValidDefaultValue;
            this.m_defaultValue = copy.m_defaultValue;
            this.m_bIsConst = copy.m_bIsConst;
        }

        public Property(CMemberBase pMemberBase, bool bIsConst)
        {
            this.m_memberBase = pMemberBase;
            this.m_variableId = 0;
            this.m_bValidDefaultValue = false;
            this.m_bIsConst = bIsConst;
            if (this.m_memberBase != null)
            {
                this.m_pt = this.m_memberBase.GetParentType();
            }
            else
            {
                this.m_pt = ParentType.PT_PAR;
            }
        }

        public static void Cleanup()
        {
            ms_properties.Clear();
        }

        public Property clone()
        {
            return new Property(this);
        }

        private static Property create(bool bParVar, bool bConst, string typeName, string variableName, string instanceName, string valueStr)
        {
            bool flag = !string.IsNullOrEmpty(variableName);
            if (flag && !bParVar)
            {
                Property property = Agent.CreateProperty(typeName, variableName, valueStr);
                if ((flag && (property != null)) && !bConst)
                {
                    property.SetVariableName(variableName);
                    property.SetInstanceNameString(instanceName);
                }
                return property;
            }
            bool flag2 = IsAgentPtr(typeName, valueStr);
            Property property2 = new Property(null, bConst);
            object v = null;
            if (!flag2)
            {
                bool bStrIsArrayType = false;
                if (typeName.StartsWith("vector<"))
                {
                    bStrIsArrayType = true;
                }
                System.Type typeFromName = null;
                if (bStrIsArrayType)
                {
                    int index = typeName.IndexOf('<');
                    int length = (typeName.IndexOf('>') - index) - 1;
                    typeFromName = GetTypeFromName(typeName.Substring(index + 1, length));
                }
                else
                {
                    typeFromName = GetTypeFromName(typeName);
                }
                v = StringUtils.FromString(typeFromName, valueStr, bStrIsArrayType);
            }
            else
            {
                v = null;
            }
            property2.SetDefaultValue(v);
            if ((flag && (property2 != null)) && !bConst)
            {
                property2.SetVariableName(variableName);
            }
            return property2;
        }

        public static Property Create(string value, CMemberBase pMemberBase, bool bConst)
        {
            Property property = new Property(pMemberBase, bConst);
            if (!string.IsNullOrEmpty(value) && property.SetDefaultValue(value))
            {
            }
            return property;
        }

        public static Property Create(string typeName, string variableName, string value, bool bStatic, bool bConst)
        {
            if (!bConst)
            {
                string str = null;
                string str2 = null;
                bool flag = Utils.IsParVar(variableName);
                if (flag)
                {
                    str2 = string.Format("{0}::{1}", typeName, variableName);
                }
                else
                {
                    str2 = ParseInstanceNameProperty(variableName, ref str);
                }
                bool flag2 = false;
                if (!string.IsNullOrEmpty(str2))
                {
                    flag2 = ms_properties.ContainsKey(str2);
                }
                if (!flag2)
                {
                    Property property = create(flag, bConst, typeName, str2, str, value);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        ms_properties[str2] = property;
                    }
                    return property.clone();
                }
                Property property4 = ms_properties[str2].clone();
                if (!string.IsNullOrEmpty(value) && !IsAgentPtr(typeName, value))
                {
                    property4.SetDefaultValue(value);
                }
                return property4;
            }
            CMemberBase base2 = null;
            bool flag4 = !string.IsNullOrEmpty(variableName);
            string instanceName = null;
            string propertyName = variableName;
            if (flag4)
            {
                propertyName = ParseInstanceNameProperty(variableName, ref instanceName);
                base2 = Agent.FindMemberBase(propertyName);
            }
            if (base2 != null)
            {
                return base2.CreateProperty(value, true);
            }
            bool bParVar = flag4 && Utils.IsParVar(variableName);
            return create(bParVar, bConst, typeName, propertyName, instanceName, value);
        }

        public static void DeleteFromCache(Property property_)
        {
            string variableFullName = property_.GetVariableFullName();
            if (!string.IsNullOrEmpty(variableFullName) && ms_properties.ContainsKey(variableFullName))
            {
                ms_properties.Remove(variableFullName);
            }
        }

        public float DifferencePercentage(Property other)
        {
            object defaultValue = this.GetDefaultValue();
            object obj3 = other.GetDefaultValue();
            float range = this.m_memberBase.GetRange();
            float num2 = 0f;
            float num3 = 0f;
            if (defaultValue.GetType() == typeof(float))
            {
                num2 = (float) defaultValue;
                num3 = (float) obj3;
            }
            else if (defaultValue.GetType() == typeof(long))
            {
                num2 = (long) defaultValue;
                num3 = (long) obj3;
            }
            else if (defaultValue.GetType() == typeof(int))
            {
                num2 = (int) defaultValue;
                num3 = (int) obj3;
            }
            else if (defaultValue.GetType() == typeof(short))
            {
                num2 = (short) defaultValue;
                num3 = (short) obj3;
            }
            else if (defaultValue.GetType() == typeof(sbyte))
            {
                num2 = (sbyte) defaultValue;
                num3 = (sbyte) obj3;
            }
            else if (defaultValue.GetType() == typeof(ulong))
            {
                num2 = (ulong) defaultValue;
                num3 = (ulong) obj3;
            }
            else if (defaultValue.GetType() == typeof(uint))
            {
                num2 = (uint) defaultValue;
                num3 = (uint) obj3;
            }
            else if (defaultValue.GetType() == typeof(ushort))
            {
                num2 = (ushort) defaultValue;
                num3 = (ushort) obj3;
            }
            else if (defaultValue.GetType() == typeof(byte))
            {
                num2 = (byte) defaultValue;
                num3 = (byte) obj3;
            }
            float num4 = num2 - num3;
            if (num4 < 0f)
            {
                num4 = -num4;
            }
            return (num4 / range);
        }

        ~Property()
        {
        }

        public string GetClassNameString()
        {
            return ((this.m_memberBase == null) ? null : this.m_memberBase.GetClassNameString());
        }

        public uint GetDefaultInteger()
        {
            return 0;
        }

        private object GetDefaultValue()
        {
            return this.m_defaultValue;
        }

        public string GetInstanceNameString()
        {
            if (!string.IsNullOrEmpty(this.m_instanceName))
            {
                return this.m_instanceName;
            }
            return ((this.m_memberBase == null) ? null : this.m_memberBase.GetInstanceNameString());
        }

        public ParentType GetParentType()
        {
            return this.m_pt;
        }

        public string GetRefName()
        {
            return this.m_refParName;
        }

        public uint GetRefNameId()
        {
            return this.m_refParNameId;
        }

        private static System.Type GetTypeFromName(string typeName)
        {
            if (typeName == "void*")
            {
                return typeof(Agent);
            }
            System.Type typeFromName = Agent.GetTypeFromName(typeName);
            if (typeFromName == null)
            {
                typeName = typeName.Replace("::", ".");
                typeFromName = Utils.GetType(typeName);
                if (typeFromName == null)
                {
                    typeFromName = Utils.GetTypeFromName(typeName);
                }
            }
            return typeFromName;
        }

        public object GetValue(Agent parent)
        {
            if ((parent == null) || this.m_bIsConst)
            {
                return this.GetDefaultValue();
            }
            if (this.m_memberBase == null)
            {
                return parent.GetVariable(this.m_varaibleName);
            }
            if (this.m_pt == ParentType.PT_INSTANCE)
            {
                parent = Agent.GetInstance(this.GetInstanceNameString(), parent.GetContextId());
            }
            return this.m_memberBase.Get(parent);
        }

        public object GetValue(Agent parent, Agent parHolder)
        {
            if ((parent == null) || this.m_bIsConst)
            {
                return this.GetDefaultValue();
            }
            if (this.m_memberBase == null)
            {
                return parHolder.GetVariable(this.m_varaibleName);
            }
            if (this.m_pt == ParentType.PT_INSTANCE)
            {
                parHolder = Agent.GetInstance(this.GetInstanceNameString(), parHolder.GetContextId());
            }
            return this.m_memberBase.Get(parHolder);
        }

        public string GetVariableFullName()
        {
            return this.m_varaibleFullName;
        }

        public uint GetVariableId()
        {
            return this.m_variableId;
        }

        public string GetVariableName()
        {
            return this.m_varaibleName;
        }

        public void Instantiate(Agent pAgent)
        {
            object defaultValue = this.GetDefaultValue();
            pAgent.Instantiate<object>(defaultValue, this);
        }

        private static bool IsAgentPtr(string typeName, string valueStr)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(valueStr) && (valueStr == "null"))
            {
                flag = true;
            }
            return flag;
        }

        private static string ParseInstanceNameProperty(string fullName, ref string instanceName)
        {
            int index = fullName.IndexOf('.');
            if (index != -1)
            {
                instanceName = fullName.Substring(0, index).Replace("::", ".");
                return fullName.Substring(index + 1);
            }
            return fullName;
        }

        public void SetDefaultInteger(int count)
        {
            this.m_bValidDefaultValue = true;
            Utils.ConvertFromInteger<object>(count, ref this.m_defaultValue);
        }

        public void SetDefaultValue(Property r)
        {
            object defaultValue = r.GetDefaultValue();
            this.SetDefaultValue(defaultValue);
        }

        public void SetDefaultValue(object v)
        {
            this.m_bValidDefaultValue = true;
            this.m_defaultValue = v;
        }

        public bool SetDefaultValue(string valStr)
        {
            System.Type memberType = null;
            if (this.m_memberBase != null)
            {
                memberType = this.m_memberBase.MemberType;
            }
            else if (this.m_bValidDefaultValue)
            {
                memberType = this.m_defaultValue.GetType();
            }
            this.m_defaultValue = StringUtils.FromString(memberType, valStr, false);
            if (this.m_defaultValue != null)
            {
                this.m_bValidDefaultValue = true;
                return true;
            }
            return false;
        }

        public bool SetDefaultValue(string valStr, System.Type type)
        {
            this.m_defaultValue = StringUtils.FromString(type, valStr, false);
            if (this.m_defaultValue != null)
            {
                this.m_bValidDefaultValue = true;
                return true;
            }
            return false;
        }

        public void SetFrom(Agent pAgentFrom, Property from, Agent pAgentTo)
        {
            object v = from.GetValue(pAgentFrom);
            this.SetValue(pAgentTo, v);
        }

        public void SetInstanceNameString(string agentIntanceName)
        {
            if (!string.IsNullOrEmpty(agentIntanceName))
            {
                if (Agent.IsNameRegistered(agentIntanceName))
                {
                    this.m_pt = ParentType.PT_INSTANCE;
                    this.m_instanceName = agentIntanceName;
                }
                else
                {
                    this.m_pt = ParentType.PT_AGENT;
                }
            }
        }

        public void SetRefName(string refParName)
        {
            this.m_refParName = refParName;
            this.m_refParNameId = Utils.MakeVariableId(this.m_refParName);
        }

        public void SetValue(Agent parent, object v)
        {
            string staticClassName = null;
            if (this.m_memberBase != null)
            {
                if (this.m_pt == ParentType.PT_INSTANCE)
                {
                    parent = Agent.GetInstance(this.GetInstanceNameString(), parent.GetContextId());
                }
                if (this.m_memberBase.ISSTATIC())
                {
                    staticClassName = this.m_memberBase.GetClassNameString();
                }
            }
            parent.SetVariableRegistry(this.m_memberBase, this.m_varaibleName, v, staticClassName, this.m_variableId);
        }

        public void SetVariableName(string variableName)
        {
            this.m_varaibleFullName = variableName;
            string nameWithoutClassName = Utils.GetNameWithoutClassName(variableName);
            this.m_variableId = Utils.MakeVariableId(nameWithoutClassName);
            this.m_varaibleName = nameWithoutClassName;
        }

        public void UnInstantiate(Agent pAgent)
        {
            pAgent.UnInstantiate(this.m_varaibleName);
        }

        public void UnLoad(Agent pAgent)
        {
            pAgent.UnLoad(this.m_varaibleName);
        }
    }
}

