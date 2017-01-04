using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class GameStoreExtend
{
    private static Type[] s_bdtMap;

    public static string GetPathName(this Type type, bool withGeneName = true)
    {
        if (type.IsGenericParameter)
        {
            return (!withGeneName ? string.Empty : type.Name);
        }
        if (type.IsArray)
        {
            string str = string.Empty;
            int arrayRank = type.GetArrayRank();
            for (int i = 0; i < arrayRank; i++)
            {
                if (i == 0)
                {
                    str = str + "[";
                }
                else
                {
                    str = str + ",";
                }
                if ((i + 1) == arrayRank)
                {
                    str = str + "]";
                }
            }
            return (type.GetElementType().GetPathName(withGeneName) + str);
        }
        string name = type.Name;
        if (type.IsGenericType)
        {
            Type[] genericArguments = type.GetGenericArguments();
            Type[] typeArray2 = null;
            if ((type.IsNested && (type.DeclaringType != null)) && type.DeclaringType.IsGenericType)
            {
                typeArray2 = type.DeclaringType.GetGenericArguments();
            }
            string str3 = string.Empty;
            for (int j = (typeArray2 == null) ? 0 : typeArray2.Length; j < genericArguments.Length; j++)
            {
                if (str3 != string.Empty)
                {
                    str3 = str3 + ",";
                }
                str3 = str3 + genericArguments[j].GetPathName(withGeneName);
            }
            int index = name.IndexOf('`');
            if (index >= 0)
            {
                name = name.Substring(0, index);
            }
            if (str3 != string.Empty)
            {
                name = name + "<" + str3 + ">";
            }
        }
        if (type.IsNested && (type.DeclaringType != null))
        {
            return (type.DeclaringType.GetPathName(withGeneName) + "." + name);
        }
        if (!string.IsNullOrEmpty(type.Namespace))
        {
            name = type.Namespace + "." + name;
        }
        return name;
    }

    public static bool IsAbsolutePublic(this Type type)
    {
        if (!type.IsGenericParameter)
        {
            if (!type.IsPublic && !type.IsNestedPublic)
            {
                return false;
            }
            if (type.IsArray && !type.GetElementType().IsAbsolutePublic())
            {
                return false;
            }
            if (type.IsNested && !type.DeclaringType.IsAbsolutePublic())
            {
                return false;
            }
            if (type.IsGenericType)
            {
                Type[] genericArguments = type.GetGenericArguments();
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    if (!genericArguments[i].IsAbsolutePublic())
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public static bool IsInterfaceImplied(this Type type, Type interfaceType)
    {
        Type[] typeArray = type.FindInterfaces(new System.Reflection.TypeFilter(GameStoreExtend.TypeFilter), interfaceType);
        return ((typeArray != null) && (typeArray.Length > 0));
    }

    public static void Start()
    {
        if (s_bdtMap == null)
        {
            s_bdtMap = new Type[] { typeof(int), typeof(uint), typeof(bool), typeof(string), typeof(byte), typeof(short), typeof(long), typeof(ushort), typeof(ulong), typeof(float), typeof(double), typeof(char), typeof(sbyte), typeof(decimal) };
        }
    }

    public static BaseDataType ToBDT(this Type type)
    {
        if (string.IsNullOrEmpty(type.Name) || (type.Name[0] == '<'))
        {
            return BaseDataType.BUG;
        }
        for (int i = 0; i < s_bdtMap.Length; i++)
        {
            if (type == s_bdtMap[i])
            {
                return (BaseDataType) i;
            }
        }
        if (type.IsValueType)
        {
            if (type.BaseType == typeof(Enum))
            {
                return BaseDataType.Enum;
            }
            return BaseDataType.Struct;
        }
        if (type.IsArray)
        {
            return BaseDataType.Array;
        }
        if (type.IsInterface)
        {
            return BaseDataType.Interface;
        }
        if (type.IsSubclassOf(typeof(Delegate)))
        {
            return BaseDataType.Delegate;
        }
        if (type.IsGenericParameter)
        {
            return BaseDataType.Generic;
        }
        return BaseDataType.Class;
    }

    public static bool TypeFilter(Type type, object typeObj)
    {
        return (type == (typeObj as Type));
    }
}

