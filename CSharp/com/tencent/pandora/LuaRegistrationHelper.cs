namespace com.tencent.pandora
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    public static class LuaRegistrationHelper
    {
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification="The type parameter is used to select an enum type")]
        public static void Enumeration<T>(LuaState lua)
        {
            if (lua == null)
            {
                throw new ArgumentNullException("lua");
            }
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("The type must be an enumeration!");
            }
            string[] names = Enum.GetNames(enumType);
            T[] values = (T[]) Enum.GetValues(enumType);
            lua.NewTable(enumType.Name);
            for (int i = 0; i < names.Length; i++)
            {
                string str = enumType.Name + "." + names[i];
                lua[str] = values[i];
            }
        }

        public static void TaggedInstanceMethods(LuaState lua, object o)
        {
            if (lua == null)
            {
                throw new ArgumentNullException("lua");
            }
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }
            foreach (MethodInfo info in o.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (LuaGlobalAttribute attribute in info.GetCustomAttributes(typeof(LuaGlobalAttribute), true))
                {
                    if (string.IsNullOrEmpty(attribute.Name))
                    {
                        lua.RegisterFunction(info.Name, o, info);
                    }
                    else
                    {
                        lua.RegisterFunction(attribute.Name, o, info);
                    }
                }
            }
        }

        public static void TaggedStaticMethods(LuaState lua, Type type)
        {
            if (lua == null)
            {
                throw new ArgumentNullException("lua");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsClass)
            {
                throw new ArgumentException("The type must be a class!", "type");
            }
            foreach (MethodInfo info in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                foreach (LuaGlobalAttribute attribute in info.GetCustomAttributes(typeof(LuaGlobalAttribute), false))
                {
                    if (string.IsNullOrEmpty(attribute.Name))
                    {
                        lua.RegisterFunction(info.Name, null, info);
                    }
                    else
                    {
                        lua.RegisterFunction(attribute.Name, null, info);
                    }
                }
            }
        }
    }
}

