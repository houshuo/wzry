namespace behaviac
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class AgentExtra_Generated
    {
        private static Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();
        private static Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
        private static Dictionary<string, PropertyInfo> _properties = new Dictionary<string, PropertyInfo>();

        public static object ExecuteMethod(Agent agent, string method, object[] args)
        {
            Type baseType = agent.GetType();
            string key = baseType.FullName + method;
            if (_methods.ContainsKey(key))
            {
                return _methods[key].Invoke(agent, args);
            }
            while (baseType != typeof(object))
            {
                MethodInfo info = baseType.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (info != null)
                {
                    _methods[key] = info;
                    return info.Invoke(agent, args);
                }
                baseType = baseType.BaseType;
            }
            return null;
        }

        public static object GetProperty(Agent agent, string property)
        {
            Type baseType = agent.GetType();
            string key = baseType.FullName + property;
            if (_fields.ContainsKey(key))
            {
                return _fields[key].GetValue(agent);
            }
            if (_properties.ContainsKey(key))
            {
                return _properties[key].GetValue(agent, null);
            }
            while (baseType != typeof(object))
            {
                FieldInfo field = baseType.GetField(property, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (field != null)
                {
                    _fields[key] = field;
                    return field.GetValue(agent);
                }
                PropertyInfo info2 = baseType.GetProperty(property, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (info2 != null)
                {
                    _properties[key] = info2;
                    return info2.GetValue(agent, null);
                }
                baseType = baseType.BaseType;
            }
            return null;
        }

        public static void SetProperty(Agent agent, string property, object value)
        {
            Type baseType = agent.GetType();
            string key = baseType.FullName + property;
            if (_fields.ContainsKey(key))
            {
                _fields[key].SetValue(agent, value);
            }
            else if (_properties.ContainsKey(key))
            {
                _properties[key].SetValue(agent, value, null);
            }
            else
            {
                while (baseType != typeof(object))
                {
                    FieldInfo field = baseType.GetField(property, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                    if (field != null)
                    {
                        _fields[key] = field;
                        field.SetValue(agent, value);
                        return;
                    }
                    PropertyInfo info2 = baseType.GetProperty(property, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                    if (info2 != null)
                    {
                        _properties[key] = info2;
                        info2.SetValue(agent, value, null);
                        return;
                    }
                    baseType = baseType.BaseType;
                }
            }
        }
    }
}

