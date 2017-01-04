namespace behaviac
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class IVariable
    {
        protected uint m_id;
        public byte m_instantiated;
        protected string m_name;
        protected CMemberBase m_pMember;
        protected Property m_property;
        private object m_value;

        public IVariable(IVariable copy)
        {
            this.m_id = copy.m_id;
            this.m_name = copy.m_name;
            this.m_property = copy.m_property;
            this.m_pMember = copy.m_pMember;
            this.m_instantiated = copy.m_instantiated;
            this.m_value = copy.m_value;
        }

        public IVariable(CMemberBase pMember, Property property_)
        {
            this.m_property = property_;
            this.m_pMember = pMember;
            this.m_instantiated = 1;
            this.m_name = this.m_property.GetVariableName();
            this.m_id = this.m_property.GetVariableId();
        }

        public IVariable(CMemberBase pMember, string variableName, uint id)
        {
            this.m_id = id;
            this.m_name = variableName;
            this.m_property = null;
            this.m_pMember = pMember;
            this.m_instantiated = 1;
        }

        public IVariable clone()
        {
            return new IVariable(this);
        }

        public void CopyTo(Agent pAgent)
        {
            if (this.m_pMember != null)
            {
                this.m_pMember.Set(pAgent, this.m_value);
            }
        }

        private static void DeepCopy(out object result, object obj)
        {
            if (obj == null)
            {
                result = obj;
            }
            else
            {
                System.Type conversionType = obj.GetType();
                if (conversionType.IsValueType || (conversionType == typeof(string)))
                {
                    result = obj;
                }
                else if (conversionType.IsArray)
                {
                    System.Type elementType = conversionType.GetElementType();
                    if (elementType == null)
                    {
                        elementType = Utility.GetType(conversionType.FullName.Replace("[]", string.Empty));
                    }
                    Array array = obj as Array;
                    Array array2 = Array.CreateInstance(elementType, array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        object obj2;
                        DeepCopy(out obj2, array.GetValue(i));
                        array2.SetValue(obj2, i);
                    }
                    result = Convert.ChangeType(array2, conversionType);
                }
                else if (conversionType.IsClass)
                {
                    result = obj;
                    if ((conversionType == typeof(Agent)) || conversionType.IsSubclassOf(typeof(Agent)))
                    {
                        result = obj;
                    }
                    else
                    {
                        object obj3 = Activator.CreateInstance(conversionType);
                        foreach (FieldInfo info in conversionType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                        {
                            object obj4 = info.GetValue(obj);
                            if (obj4 != null)
                            {
                                object obj5;
                                DeepCopy(out obj5, obj4);
                                info.SetValue(obj3, obj5);
                            }
                        }
                        result = obj3;
                    }
                }
                else
                {
                    result = obj;
                }
            }
        }

        ~IVariable()
        {
        }

        public uint GetId()
        {
            return this.m_id;
        }

        public Property GetProperty()
        {
            return this.m_property;
        }

        public object GetValue(Agent pAgent)
        {
            if (this.m_pMember != null)
            {
                return this.m_pMember.Get(pAgent);
            }
            return this.m_value;
        }

        public void Load(ISerializableNode node)
        {
        }

        public void Log(Agent pAgent)
        {
            string str = StringUtils.ToString(this.m_value);
            string typeName = string.Empty;
            if (!object.ReferenceEquals(this.m_value, null))
            {
                typeName = Utils.GetNativeTypeName(this.m_value.GetType());
            }
            else
            {
                typeName = "Agent";
            }
            string name = this.m_name;
            if (!object.ReferenceEquals(pAgent, null))
            {
                CMemberBase base2 = pAgent.FindMember(this.m_name);
                if (base2 != null)
                {
                    string str4 = base2.GetClassNameString().Replace(".", "::");
                    name = string.Format("{0}::{1}", str4, this.m_name);
                }
            }
            LogManager.Log(pAgent, typeName, name, str);
        }

        public void Reset()
        {
        }

        public void Save(ISerializableNode node)
        {
            CSerializationID chidlId = new CSerializationID("var");
            ISerializableNode node2 = node.newChild(chidlId);
            CSerializationID attrId = new CSerializationID("name");
            node2.setAttr(attrId, this.m_name);
            CSerializationID nid3 = new CSerializationID("value");
            node2.setAttr<object>(nid3, this.m_value);
        }

        public void SetFromString(Agent pAgent, CMemberBase pMember, string valueString)
        {
            if (!string.IsNullOrEmpty(valueString) && (pMember != null))
            {
                object rhs = StringUtils.FromString(pMember.MemberType, valueString, false);
                if (!Details.Equal(this.m_value, rhs))
                {
                    this.m_value = rhs;
                    if (!object.ReferenceEquals(pAgent, null) && (pMember != null))
                    {
                        pMember.Set(pAgent, rhs);
                    }
                }
            }
        }

        public void SetProperty(Property p)
        {
            if (p != null)
            {
            }
            this.m_property = p;
        }

        public void SetValue(object value, Agent pAgent)
        {
            bool flag = false;
            if (this.m_pMember != null)
            {
                this.m_pMember.Set(pAgent, value);
                flag = true;
            }
            if (!flag && !Details.Equal(this.m_value, value))
            {
                this.m_value = value;
            }
        }
    }
}

