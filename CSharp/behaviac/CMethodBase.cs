namespace behaviac
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class CMethodBase
    {
        private MethodMetaInfoAttribute descAttrbute_;
        private CStringID m_id;
        private string m_instanceName;
        private string m_name;
        private object[] m_param_values;
        private Param_t[] m_params;
        private ParentType m_parentType;
        private MethodBase method_;

        protected CMethodBase(CMethodBase copy)
        {
            this.m_id = new CStringID();
            this.m_instanceName = copy.m_instanceName;
            this.m_parentType = copy.m_parentType;
            this.method_ = copy.method_;
            this.descAttrbute_ = copy.descAttrbute_;
            this.m_name = copy.m_name;
            this.m_id = copy.m_id;
        }

        public CMethodBase(MethodBase m, MethodMetaInfoAttribute a, string methodNameOverride)
        {
            this.m_id = new CStringID();
            this.method_ = m;
            this.descAttrbute_ = a;
            this.m_name = string.IsNullOrEmpty(methodNameOverride) ? this.method_.Name : methodNameOverride;
            this.m_id.SetId(this.m_name);
        }

        public virtual CMethodBase clone()
        {
            return new CMethodBase(this);
        }

        public virtual Property CreateProperty(string defaultValue, bool bConst)
        {
            return Property.Create(defaultValue, null, bConst);
        }

        public string GetClassNameString()
        {
            if (this.IsNamedEvent())
            {
                return this.method_.DeclaringType.DeclaringType.FullName;
            }
            return this.method_.DeclaringType.FullName;
        }

        public CStringID GetId()
        {
            return this.m_id;
        }

        public string GetInstanceNameString()
        {
            return this.m_instanceName;
        }

        public string GetName()
        {
            return this.m_name;
        }

        public ParentType GetParentType()
        {
            return this.m_parentType;
        }

        public virtual bool IsNamedEvent()
        {
            return false;
        }

        public bool IsStatic()
        {
            return false;
        }

        public void Load(Agent parent, List<string> paramsToken)
        {
            ParameterInfo[] parameters = this.method_.GetParameters();
            this.m_param_values = new object[parameters.Length];
            if (paramsToken.Count == parameters.Length)
            {
                this.m_params = new Param_t[parameters.Length];
                for (int i = 0; i < paramsToken.Count; i++)
                {
                    ParameterInfo info = parameters[i];
                    if (paramsToken[i][0] == '{')
                    {
                        DictionaryView<string, Property> props = new DictionaryView<string, Property>();
                        string strT = string.Empty;
                        if (StringUtils.ParseForStruct(info.ParameterType, paramsToken[i], ref strT, props))
                        {
                            this.m_param_values[i] = StringUtils.FromString(info.ParameterType, strT, false);
                            this.m_params[i].paramStructMembers = props;
                        }
                    }
                    else
                    {
                        bool flag2 = paramsToken[i][0] == '"';
                        if (flag2 || (paramsToken[i].IndexOf(' ') == -1))
                        {
                            string valStr = !flag2 ? paramsToken[i] : paramsToken[i].Substring(1, paramsToken[i].Length - 2);
                            this.m_param_values[i] = StringUtils.FromString(info.ParameterType, valStr, false);
                        }
                        else
                        {
                            char[] separator = new char[] { ' ' };
                            string[] strArray = paramsToken[i].Split(separator);
                            if (strArray.Length == 2)
                            {
                                Property property = Property.Create(strArray[0].Replace("::", "."), strArray[1], null, false, false);
                                this.m_params[i].paramProperty = property;
                            }
                            else if (strArray.Length == 3)
                            {
                                Property property2 = Property.Create(strArray[1].Replace("::", "."), strArray[2], null, true, false);
                                this.m_params[i].paramProperty = property2;
                            }
                        }
                    }
                }
            }
        }

        public object run(Agent parent, Agent parHolder)
        {
            if (this.m_params != null)
            {
                for (int i = 0; i < this.m_params.Length; i++)
                {
                    Property paramProperty = this.m_params[i].paramProperty;
                    if (paramProperty != null)
                    {
                        this.m_param_values[i] = paramProperty.GetValue(parent, parHolder);
                    }
                    if (this.m_params[i].paramStructMembers != null)
                    {
                        Agent.CTagObjectDescriptor descriptorByName = Agent.GetDescriptorByName(this.m_param_values[i].GetType().FullName);
                        foreach (KeyValuePair<string, Property> pair in this.m_params[i].paramStructMembers)
                        {
                            CMemberBase member = descriptorByName.GetMember(pair.Key);
                            if (member != null)
                            {
                                object v = pair.Value.GetValue(parent, parHolder);
                                member.Set(this.m_param_values[i], v);
                            }
                        }
                    }
                }
            }
            object obj3 = this.method_.Invoke(parent, this.m_param_values);
            if (this.m_params != null)
            {
                for (int j = 0; j < this.m_params.Length; j++)
                {
                    Property property2 = this.m_params[j].paramProperty;
                    if (property2 != null)
                    {
                        object obj4 = this.m_param_values[j];
                        property2.SetValue(parHolder, obj4);
                    }
                    if (this.m_params[j].paramStructMembers != null)
                    {
                        Agent.CTagObjectDescriptor descriptor2 = Agent.GetDescriptorByName(this.m_param_values[j].GetType().FullName);
                        foreach (KeyValuePair<string, Property> pair2 in this.m_params[j].paramStructMembers)
                        {
                            CMemberBase base3 = descriptor2.GetMember(pair2.Key);
                            if (base3 != null)
                            {
                                object obj5 = base3.Get(this.m_param_values[j]);
                                pair2.Value.SetValue(parHolder, obj5);
                            }
                        }
                    }
                }
            }
            return obj3;
        }

        public object run(Agent parent, Agent parHolder, object param)
        {
            if (this.m_param_values.Length == 1)
            {
                this.m_param_values[0] = param;
            }
            return this.method_.Invoke(parent, this.m_param_values);
        }

        public void SetInstanceNameString(string agentInstanceName, ParentType pt)
        {
            this.m_instanceName = agentInstanceName;
            this.m_parentType = pt;
        }

        public string Name
        {
            get
            {
                return this.m_name;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Param_t
        {
            public Property paramProperty;
            public DictionaryView<string, Property> paramStructMembers;
        }
    }
}

