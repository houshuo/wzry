namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Variables
    {
        public DictionaryView<uint, IVariable> m_variables = new DictionaryView<uint, IVariable>();

        public void Clear()
        {
            this.m_variables.Clear();
        }

        public void CopyTo(Agent pAgent, Variables target)
        {
            target.m_variables.Clear();
            foreach (KeyValuePair<uint, IVariable> pair in this.m_variables)
            {
                IVariable variable = pair.Value.clone();
                target.m_variables[variable.GetId()] = variable;
            }
            if (!object.ReferenceEquals(pAgent, null))
            {
                foreach (KeyValuePair<uint, IVariable> pair2 in target.m_variables)
                {
                    pair2.Value.CopyTo(pAgent);
                }
            }
        }

        ~Variables()
        {
            this.Clear();
        }

        public object Get(Agent pAgent, uint varId)
        {
            IVariable variable = null;
            if (!this.m_variables.TryGetValue(varId, out variable))
            {
                CMemberBase base2 = pAgent.FindMember(varId);
                if (base2 != null)
                {
                    return base2.Get(pAgent);
                }
            }
            else
            {
                Property property = variable.GetProperty();
                if ((property != null) && !string.IsNullOrEmpty(property.GetRefName()))
                {
                    return this.Get(pAgent, property.GetRefNameId());
                }
                return variable.GetValue(pAgent);
            }
            return null;
        }

        public void Instantiate(Property property_, object value)
        {
            uint variableId = property_.GetVariableId();
            IVariable variable = null;
            if (!this.m_variables.TryGetValue(variableId, out variable))
            {
                variable = new IVariable(null, property_);
                variable.SetValue(value, null);
                this.m_variables.Add(variableId, variable);
            }
            else
            {
                if (variable.m_instantiated == 0)
                {
                    variable.SetProperty(property_);
                }
                variable.m_instantiated = (byte) (variable.m_instantiated + 1);
            }
        }

        public bool IsExisting(uint varId)
        {
            return this.m_variables.ContainsKey(varId);
        }

        public void Log(Agent pAgent, bool bForce)
        {
        }

        public void Reset()
        {
            foreach (KeyValuePair<uint, IVariable> pair in this.m_variables)
            {
                pair.Value.Reset();
            }
        }

        public void Set(Agent pAgent, CMemberBase pMember, string variableName, object value, uint varId)
        {
            if (varId == 0)
            {
                varId = Utils.MakeVariableId(variableName);
            }
            IVariable variable = null;
            if (!this.m_variables.TryGetValue(varId, out variable))
            {
                if (pMember == null)
                {
                    if (pAgent != null)
                    {
                        pMember = pAgent.FindMember(variableName);
                    }
                    else
                    {
                        pMember = Agent.FindMemberBase(variableName);
                    }
                }
                variable = new IVariable(pMember, variableName, varId);
                this.m_variables.Add(varId, variable);
            }
            variable.SetValue(value, pAgent);
        }

        public void SetFromString(Agent pAgent, string variableName, string valueStr)
        {
            string nameWithoutClassName = Utils.GetNameWithoutClassName(variableName);
            CMemberBase pMember = pAgent.FindMember(nameWithoutClassName);
            uint key = Utils.MakeVariableId(nameWithoutClassName);
            IVariable variable = null;
            if (this.m_variables.TryGetValue(key, out variable))
            {
                variable.SetFromString(pAgent, pMember, valueStr);
            }
        }

        public void UnInstantiate(string variableName)
        {
            uint key = Utils.MakeVariableId(variableName);
            IVariable variable = null;
            if (this.m_variables.TryGetValue(key, out variable))
            {
                variable.m_instantiated = (byte) (variable.m_instantiated - 1);
                if (variable.m_instantiated == 0)
                {
                    variable.SetProperty(null);
                }
            }
        }

        public void Unload()
        {
            this.m_variables.Clear();
        }

        public void UnLoad(string variableName)
        {
            uint key = Utils.MakeVariableId(variableName);
            this.m_variables.Remove(key);
        }
    }
}

