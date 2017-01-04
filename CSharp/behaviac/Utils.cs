namespace behaviac
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class Utils
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map6;
        private static Dictionary<string, bool> ms_staticClasses;
        private static Dictionary<string, string> ms_type_mapping;

        static Utils()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("Boolean", "bool");
            dictionary.Add("System.Boolean", "bool");
            dictionary.Add("Int32", "int");
            dictionary.Add("System.Int32", "int");
            dictionary.Add("UInt32", "uint");
            dictionary.Add("System.UInt32", "uint");
            dictionary.Add("Int16", "short");
            dictionary.Add("System.Int16", "short");
            dictionary.Add("UInt16", "ushort");
            dictionary.Add("System.UInt16", "ushort");
            dictionary.Add("Int8", "sbyte");
            dictionary.Add("System.Int8", "sbyte");
            dictionary.Add("SByte", "sbyte");
            dictionary.Add("System.SByte", "sbyte");
            dictionary.Add("UInt8", "ubyte");
            dictionary.Add("System.UInt8", "ubyte");
            dictionary.Add("Byte", "ubyte");
            dictionary.Add("System.Byte", "ubyte");
            dictionary.Add("Char", "char");
            dictionary.Add("Int64", "long");
            dictionary.Add("System.Int64", "long");
            dictionary.Add("UInt64", "ulong");
            dictionary.Add("System.UInt64", "ulong");
            dictionary.Add("Single", "float");
            dictionary.Add("System.Single", "float");
            dictionary.Add("Double", "double");
            dictionary.Add("System.Double", "double");
            dictionary.Add("String", "string");
            dictionary.Add("System.String", "string");
            dictionary.Add("Void", "void");
            ms_type_mapping = dictionary;
        }

        public static void AddStaticClass(System.Type type)
        {
            if (IsStaticType(type))
            {
                StaticClasses[type.FullName] = true;
            }
        }

        public static void ConvertFromInteger<T>(int v, ref T ret)
        {
        }

        public static uint ConvertToInteger<T>(T v)
        {
            return 0;
        }

        public static int GetClassTypeNumberId<T>()
        {
            return 0;
        }

        private static object GetDefaultValue(System.Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }
            return null;
        }

        public static string GetNameWithoutClassName(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                return null;
            }
            int num = variableName.LastIndexOf(':');
            if ((num > 0) && (variableName[num - 1] == ':'))
            {
                return variableName.Substring(num + 1);
            }
            return variableName;
        }

        public static string GetNativeTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return string.Empty;
            }
            foreach (KeyValuePair<string, string> pair in ms_type_mapping)
            {
                if (pair.Key == typeName)
                {
                    return pair.Value;
                }
                if ((pair.Key + "&") == typeName)
                {
                    return (pair.Value + "&");
                }
            }
            char[] separator = new char[] { '.' };
            string[] strArray = typeName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            return strArray[strArray.Length - 1];
        }

        public static string GetNativeTypeName(System.Type type)
        {
            if (IsArrayType(type))
            {
                System.Type type2 = type.GetGenericArguments()[0];
                return string.Format("vector<{0}>", GetNativeTypeName(type2));
            }
            return GetNativeTypeName(type.Name);
        }

        public static System.Type GetType(string typeName)
        {
            System.Type type = Utility.GetType(typeName);
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
            if (typeName.StartsWith("System.Collections.Generic.List"))
            {
                int index = typeName.IndexOf("[[");
                if (index > -1)
                {
                    int num3 = typeName.IndexOf(",");
                    if (num3 < 0)
                    {
                        num3 = typeName.IndexOf("]]");
                    }
                    if (num3 > index)
                    {
                        type = GetType(typeName.Substring(index + 2, (num3 - index) - 2));
                        if (type != null)
                        {
                            System.Type[] typeArguments = new System.Type[] { type };
                            return typeof(List<>).MakeGenericType(typeArguments);
                        }
                    }
                }
            }
            return null;
        }

        public static System.Type GetTypeFromName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            string key = typeName;
            if (key != null)
            {
                int num;
                if (<>f__switch$map6 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(14);
                    dictionary.Add("bool", 0);
                    dictionary.Add("int", 1);
                    dictionary.Add("uint", 2);
                    dictionary.Add("short", 3);
                    dictionary.Add("ushort", 4);
                    dictionary.Add("char", 5);
                    dictionary.Add("sbyte", 6);
                    dictionary.Add("ubyte", 7);
                    dictionary.Add("byte", 7);
                    dictionary.Add("long", 8);
                    dictionary.Add("ulong", 9);
                    dictionary.Add("float", 10);
                    dictionary.Add("double", 11);
                    dictionary.Add("string", 12);
                    <>f__switch$map6 = dictionary;
                }
                if (<>f__switch$map6.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            return typeof(bool);

                        case 1:
                            return typeof(int);

                        case 2:
                            return typeof(uint);

                        case 3:
                            return typeof(short);

                        case 4:
                            return typeof(ushort);

                        case 5:
                            return typeof(char);

                        case 6:
                            return typeof(sbyte);

                        case 7:
                            return typeof(byte);

                        case 8:
                            return typeof(long);

                        case 9:
                            return typeof(ulong);

                        case 10:
                            return typeof(float);

                        case 11:
                            return typeof(double);

                        case 12:
                            return typeof(string);
                    }
                }
            }
            return GetType(typeName);
        }

        public static object GetValueFromString(System.Type type, string value)
        {
            if (value != null)
            {
                if ((((type == typeof(string)) && !string.IsNullOrEmpty(value)) && ((value.Length > 1) && (value[0] == '"'))) && (value[value.Length - 1] == '"'))
                {
                    value = value.Substring(1, value.Length - 2);
                }
                try
                {
                    return TypeDescriptor.GetConverter(type).ConvertFromString(value);
                }
                catch
                {
                    if (type == typeof(bool))
                    {
                        bool flag;
                        if (bool.TryParse(value, out flag))
                        {
                            return flag;
                        }
                    }
                    else if (type == typeof(int))
                    {
                        int num;
                        if (int.TryParse(value, out num))
                        {
                            return num;
                        }
                    }
                    else if (type == typeof(uint))
                    {
                        uint num2;
                        if (uint.TryParse(value, out num2))
                        {
                            return num2;
                        }
                    }
                    else if (type == typeof(short))
                    {
                        short num3;
                        if (short.TryParse(value, out num3))
                        {
                            return num3;
                        }
                    }
                    else if (type == typeof(ushort))
                    {
                        ushort num4;
                        if (ushort.TryParse(value, out num4))
                        {
                            return num4;
                        }
                    }
                    else if (type == typeof(char))
                    {
                        char ch;
                        if (char.TryParse(value, out ch))
                        {
                            return ch;
                        }
                    }
                    else if (type == typeof(sbyte))
                    {
                        sbyte num5;
                        if (sbyte.TryParse(value, out num5))
                        {
                            return num5;
                        }
                    }
                    else if (type == typeof(byte))
                    {
                        byte num6;
                        if (byte.TryParse(value, out num6))
                        {
                            return num6;
                        }
                    }
                    else if (type == typeof(long))
                    {
                        long num7;
                        if (long.TryParse(value, out num7))
                        {
                            return num7;
                        }
                    }
                    else if (type == typeof(ulong))
                    {
                        ulong num8;
                        if (ulong.TryParse(value, out num8))
                        {
                            return num8;
                        }
                    }
                    else if (type == typeof(float))
                    {
                        float num9;
                        if (float.TryParse(value, out num9))
                        {
                            return num9;
                        }
                    }
                    else if (type == typeof(double))
                    {
                        double num10;
                        if (double.TryParse(value, out num10))
                        {
                            return num10;
                        }
                    }
                    else if (type == typeof(string))
                    {
                        return value;
                    }
                }
            }
            return GetDefaultValue(type);
        }

        public static bool IsArrayType(System.Type type)
        {
            return (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(List<>)));
        }

        public static bool IsCustomClassType(System.Type type)
        {
            return (((((type != null) && !type.IsByRef) && (type.IsClass || type.IsValueType)) && (((type != typeof(void)) && !type.IsEnum) && (!type.IsPrimitive && !IsStringType(type)))) && !IsArrayType(type));
        }

        public static bool IsEnumType(System.Type type)
        {
            return ((type != null) && type.IsEnum);
        }

        public static bool IsNull(object aObj)
        {
            return ((aObj == null) || aObj.Equals(null));
        }

        public static bool IsParVar(string variableName)
        {
            int num = variableName.LastIndexOf(':');
            if ((num != -1) && (variableName[num - 1] == ':'))
            {
                return false;
            }
            return true;
        }

        public static bool IsStaticClass(string className)
        {
            return StaticClasses.ContainsKey(className);
        }

        public static bool IsStaticType(System.Type type)
        {
            return (((type != null) && type.IsAbstract) && type.IsSealed);
        }

        public static bool IsStringType(System.Type type)
        {
            return (type == typeof(string));
        }

        public static uint MakeVariableId(string idstring)
        {
            return CRC32.CalcCRC(idstring);
        }

        private static Dictionary<string, bool> StaticClasses
        {
            get
            {
                if (ms_staticClasses == null)
                {
                    ms_staticClasses = new Dictionary<string, bool>();
                }
                return ms_staticClasses;
            }
        }
    }
}

