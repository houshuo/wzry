namespace behaviac
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public static class StringUtils
    {
        public static string FindExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public static int FirstToken(string params_, char sep, ref string token)
        {
            int index = params_.IndexOf(sep);
            if (index != -1)
            {
                token = params_.Substring(0, index);
                return index;
            }
            return -1;
        }

        public static object FromString(System.Type type, string valStr, bool bStrIsArrayType)
        {
            if (!string.IsNullOrEmpty(valStr) && (valStr == "null"))
            {
                return null;
            }
            if (type.IsByRef)
            {
                type = type.GetElementType();
            }
            bool flag = Utils.IsArrayType(type);
            if (bStrIsArrayType || flag)
            {
                if (flag)
                {
                    System.Type type2 = type.GetGenericArguments()[0];
                    return FromStringVector(type2, valStr);
                }
                return FromStringVector(type, valStr);
            }
            if (type == typeof(Property))
            {
                return Condition.LoadProperty(valStr);
            }
            if (Utils.IsCustomClassType(type))
            {
                return FromStringStruct(type, valStr);
            }
            return Utils.GetValueFromString(type, valStr);
        }

        private static object FromStringStruct(System.Type type, string src)
        {
            object obj2 = Activator.CreateInstance(type);
            DictionaryView<string, FieldInfo> view = new DictionaryView<string, FieldInfo>();
            foreach (FieldInfo info in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (!info.IsLiteral)
                {
                    view.Add(info.Name, info);
                }
            }
            if (!string.IsNullOrEmpty(src))
            {
                int num2 = SkipPairedBrackets(src, 0);
                int startIndex = 1;
                int index = src.IndexOf(';', startIndex);
                while (index != -1)
                {
                    if (index > startIndex)
                    {
                        string str2;
                        int num5 = src.IndexOf('=', startIndex);
                        int length = num5 - startIndex;
                        string key = src.Substring(startIndex, length);
                        char ch = src[num5 + 1];
                        if (ch != '{')
                        {
                            length = (index - num5) - 1;
                            str2 = src.Substring(num5 + 1, length);
                        }
                        else
                        {
                            int indexBracketBegin = 0;
                            indexBracketBegin += num5 + 1;
                            length = (SkipPairedBrackets(src, indexBracketBegin) - indexBracketBegin) + 1;
                            str2 = src.Substring(num5 + 1, length);
                            index = (num5 + 1) + length;
                        }
                        if (view.ContainsKey(key))
                        {
                            FieldInfo info2 = view[key];
                            object obj3 = FromString(info2.FieldType, str2, false);
                            info2.SetValue(obj2, obj3);
                        }
                    }
                    startIndex = index + 1;
                    index = src.IndexOf(';', startIndex);
                    if (index > num2)
                    {
                        return obj2;
                    }
                }
            }
            return obj2;
        }

        private static object FromStringVector(System.Type type, string src)
        {
            System.Type[] typeArguments = new System.Type[] { type };
            IList list = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(typeArguments));
            if (!string.IsNullOrEmpty(src))
            {
                int index = src.IndexOf(':');
                int num2 = int.Parse(src.Substring(0, index));
                int indexBracketBegin = index + 1;
                int startIndex = indexBracketBegin;
                if ((indexBracketBegin < src.Length) && (src[indexBracketBegin] == '{'))
                {
                    startIndex = SkipPairedBrackets(src, indexBracketBegin);
                }
                for (startIndex = src.IndexOf('|', startIndex); startIndex != -1; startIndex = src.IndexOf('|', startIndex))
                {
                    int length = startIndex - indexBracketBegin;
                    string valStr = src.Substring(indexBracketBegin, length);
                    object obj2 = FromString(type, valStr, false);
                    list.Add(obj2);
                    indexBracketBegin = startIndex + 1;
                    if ((indexBracketBegin < src.Length) && (src[indexBracketBegin] == '{'))
                    {
                        startIndex = SkipPairedBrackets(src, indexBracketBegin);
                    }
                    else
                    {
                        startIndex = indexBracketBegin;
                    }
                }
                if (indexBracketBegin < src.Length)
                {
                    int num6 = src.Length - indexBracketBegin;
                    string str3 = src.Substring(indexBracketBegin, num6);
                    object obj3 = FromString(type, str3, false);
                    list.Add(obj3);
                }
            }
            return list;
        }

        public static bool ParseForStruct(System.Type type, string str, ref string strT, DictionaryView<string, Property> props)
        {
            int num = 0;
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                switch (ch)
                {
                    case ';':
                    case '{':
                    case '}':
                    {
                        int num3 = num;
                        while (num3 <= i)
                        {
                            strT = strT + str[num3++];
                        }
                        num = i + 1;
                        break;
                    }
                    default:
                        if (ch == ' ')
                        {
                            string str2 = string.Empty;
                            int num4 = num;
                            while (str[num4] != '=')
                            {
                                str2 = str2 + str[num4++];
                            }
                            num4++;
                            string typeName = string.Empty;
                            while (str[num4] != ' ')
                            {
                                typeName = typeName + str[num4++];
                            }
                            bool bStatic = false;
                            if (typeName == "static")
                            {
                                num4++;
                                while (str[num4] != ' ')
                                {
                                    typeName = typeName + str[num4++];
                                }
                                bStatic = true;
                            }
                            string variableName = string.Empty;
                            i++;
                            while (str[i] != ';')
                            {
                                variableName = variableName + str[i++];
                            }
                            props[str2] = Property.Create(typeName, variableName, null, bStatic, false);
                            num = i + 1;
                        }
                        break;
                }
            }
            return true;
        }

        private static int SkipPairedBrackets(string src, int indexBracketBegin)
        {
            if (!string.IsNullOrEmpty(src) && (src[indexBracketBegin] == '{'))
            {
                int num = 0;
                for (int i = indexBracketBegin; i < src.Length; i++)
                {
                    if (src[i] == '{')
                    {
                        num++;
                    }
                    else if ((src[i] == '}') && (--num == 0))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static string ToString(object value)
        {
            string str = string.Empty;
            if (value != null)
            {
                System.Type type = value.GetType();
                if (Utils.IsArrayType(type))
                {
                    IList list = value as IList;
                    str = string.Format("{0}:", list.Count);
                    if (list.Count <= 0)
                    {
                        return str;
                    }
                    for (int i = 0; i < (list.Count - 1); i++)
                    {
                        object obj2 = list[i];
                        string str2 = ToString(obj2);
                        str = str + string.Format("{0}|", str2);
                    }
                    object obj3 = list[list.Count - 1];
                    string str3 = ToString(obj3);
                    return (str + string.Format("{0}", str3));
                }
                if (Utils.IsCustomClassType(type))
                {
                    if ((type == typeof(Agent)) || type.IsSubclassOf(typeof(Agent)))
                    {
                        return string.Format("{0:x08}", value.GetHashCode());
                    }
                    str = "{";
                    foreach (FieldInfo info in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    {
                        string str4 = ToString(info.GetValue(value));
                        str = str + string.Format("{0}={1};", info.Name, str4);
                    }
                    return (str + "}");
                }
                return value.ToString();
            }
            return "null";
        }
    }
}

