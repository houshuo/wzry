using System;
using System.Reflection;

public static class UtilityPlugin
{
    public static Type GetType(string typeName)
    {
        if (!string.IsNullOrEmpty(typeName))
        {
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
        }
        return null;
    }
}

