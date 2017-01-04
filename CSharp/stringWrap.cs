using com.tencent.pandora;
using System;
using System.Globalization;
using System.Text;

public class stringWrap
{
    private static System.Type classType = typeof(string);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createstring(IntPtr L)
    {
        if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TSTRING)
        {
            string o = LuaDLL.lua_tostring(L, 1);
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: String.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Clone(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        object o = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).Clone();
        LuaScriptMgr.PushVarObject(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Compare(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 2)
        {
            string luaString = LuaScriptMgr.GetLuaString(L, 1);
            string strB = LuaScriptMgr.GetLuaString(L, 2);
            int d = string.Compare(luaString, strB);
            LuaScriptMgr.Push(L, d);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(StringComparison)))
        {
            string strA = LuaScriptMgr.GetString(L, 1);
            string str4 = LuaScriptMgr.GetString(L, 2);
            StringComparison luaObject = (StringComparison) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            int num3 = string.Compare(strA, str4, luaObject);
            LuaScriptMgr.Push(L, num3);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(bool)))
        {
            string str5 = LuaScriptMgr.GetString(L, 1);
            string str6 = LuaScriptMgr.GetString(L, 2);
            bool ignoreCase = LuaDLL.lua_toboolean(L, 3);
            int num4 = string.Compare(str5, str6, ignoreCase);
            LuaScriptMgr.Push(L, num4);
            return 1;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(CultureInfo), typeof(CompareOptions)))
        {
            string str7 = LuaScriptMgr.GetString(L, 1);
            string str8 = LuaScriptMgr.GetString(L, 2);
            CultureInfo culture = (CultureInfo) LuaScriptMgr.GetLuaObject(L, 3);
            CompareOptions options = (CompareOptions) ((int) LuaScriptMgr.GetLuaObject(L, 4));
            int num5 = string.Compare(str7, str8, culture, options);
            LuaScriptMgr.Push(L, num5);
            return 1;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(bool), typeof(CultureInfo)))
        {
            string str9 = LuaScriptMgr.GetString(L, 1);
            string str10 = LuaScriptMgr.GetString(L, 2);
            bool flag2 = LuaDLL.lua_toboolean(L, 3);
            CultureInfo info2 = (CultureInfo) LuaScriptMgr.GetLuaObject(L, 4);
            int num6 = string.Compare(str9, str10, flag2, info2);
            LuaScriptMgr.Push(L, num6);
            return 1;
        }
        if (num == 5)
        {
            string str11 = LuaScriptMgr.GetLuaString(L, 1);
            int number = (int) LuaScriptMgr.GetNumber(L, 2);
            string str12 = LuaScriptMgr.GetLuaString(L, 3);
            int indexB = (int) LuaScriptMgr.GetNumber(L, 4);
            int length = (int) LuaScriptMgr.GetNumber(L, 5);
            int num10 = string.Compare(str11, number, str12, indexB, length);
            LuaScriptMgr.Push(L, num10);
            return 1;
        }
        if ((num == 6) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(int), typeof(string), typeof(int), typeof(int), typeof(StringComparison)))
        {
            string str13 = LuaScriptMgr.GetString(L, 1);
            int indexA = (int) LuaDLL.lua_tonumber(L, 2);
            string str14 = LuaScriptMgr.GetString(L, 3);
            int num12 = (int) LuaDLL.lua_tonumber(L, 4);
            int num13 = (int) LuaDLL.lua_tonumber(L, 5);
            StringComparison comparisonType = (StringComparison) ((int) LuaScriptMgr.GetLuaObject(L, 6));
            int num14 = string.Compare(str13, indexA, str14, num12, num13, comparisonType);
            LuaScriptMgr.Push(L, num14);
            return 1;
        }
        if ((num == 6) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(int), typeof(string), typeof(int), typeof(int), typeof(bool)))
        {
            string str15 = LuaScriptMgr.GetString(L, 1);
            int num15 = (int) LuaDLL.lua_tonumber(L, 2);
            string str16 = LuaScriptMgr.GetString(L, 3);
            int num16 = (int) LuaDLL.lua_tonumber(L, 4);
            int num17 = (int) LuaDLL.lua_tonumber(L, 5);
            bool flag3 = LuaDLL.lua_toboolean(L, 6);
            int num18 = string.Compare(str15, num15, str16, num16, num17, flag3);
            LuaScriptMgr.Push(L, num18);
            return 1;
        }
        if ((num == 7) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(int), typeof(string), typeof(int), typeof(int), typeof(CultureInfo), typeof(CompareOptions)))
        {
            string str17 = LuaScriptMgr.GetString(L, 1);
            int num19 = (int) LuaDLL.lua_tonumber(L, 2);
            string str18 = LuaScriptMgr.GetString(L, 3);
            int num20 = (int) LuaDLL.lua_tonumber(L, 4);
            int num21 = (int) LuaDLL.lua_tonumber(L, 5);
            CultureInfo info3 = (CultureInfo) LuaScriptMgr.GetLuaObject(L, 6);
            CompareOptions options2 = (CompareOptions) ((int) LuaScriptMgr.GetLuaObject(L, 7));
            int num22 = string.Compare(str17, num19, str18, num20, num21, info3, options2);
            LuaScriptMgr.Push(L, num22);
            return 1;
        }
        if ((num == 7) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(int), typeof(string), typeof(int), typeof(int), typeof(bool), typeof(CultureInfo)))
        {
            string str19 = LuaScriptMgr.GetString(L, 1);
            int num23 = (int) LuaDLL.lua_tonumber(L, 2);
            string str20 = LuaScriptMgr.GetString(L, 3);
            int num24 = (int) LuaDLL.lua_tonumber(L, 4);
            int num25 = (int) LuaDLL.lua_tonumber(L, 5);
            bool flag4 = LuaDLL.lua_toboolean(L, 6);
            CultureInfo info4 = (CultureInfo) LuaScriptMgr.GetLuaObject(L, 7);
            int num26 = string.Compare(str19, num23, str20, num24, num25, flag4, info4);
            LuaScriptMgr.Push(L, num26);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Compare");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CompareOrdinal(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string luaString = LuaScriptMgr.GetLuaString(L, 1);
                string strB = LuaScriptMgr.GetLuaString(L, 2);
                int d = string.CompareOrdinal(luaString, strB);
                LuaScriptMgr.Push(L, d);
                return 1;
            }
            case 5:
            {
                string strA = LuaScriptMgr.GetLuaString(L, 1);
                int number = (int) LuaScriptMgr.GetNumber(L, 2);
                string str4 = LuaScriptMgr.GetLuaString(L, 3);
                int indexB = (int) LuaScriptMgr.GetNumber(L, 4);
                int length = (int) LuaScriptMgr.GetNumber(L, 5);
                int num6 = string.CompareOrdinal(strA, number, str4, indexB, length);
                LuaScriptMgr.Push(L, num6);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.CompareOrdinal");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CompareTo(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
        {
            string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string strB = LuaScriptMgr.GetString(L, 2);
            int d = str.CompareTo(strB);
            LuaScriptMgr.Push(L, d);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(object)))
        {
            string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            object varObject = LuaScriptMgr.GetVarObject(L, 2);
            int num3 = str3.CompareTo(varObject);
            LuaScriptMgr.Push(L, num3);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.CompareTo");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Concat(IntPtr L)
    {
        int count = LuaDLL.lua_gettop(L);
        if (count == 1)
        {
            string str = LuaScriptMgr.GetVarObject(L, 1);
            LuaScriptMgr.Push(L, str);
            return 1;
        }
        if ((count == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
        {
            string str2 = LuaScriptMgr.GetString(L, 1);
            string str3 = LuaScriptMgr.GetString(L, 2);
            string str4 = str2 + str3;
            LuaScriptMgr.Push(L, str4);
            return 1;
        }
        if ((count == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(object), typeof(object)))
        {
            object varObject = LuaScriptMgr.GetVarObject(L, 1);
            object obj4 = LuaScriptMgr.GetVarObject(L, 2);
            string str5 = varObject + obj4;
            LuaScriptMgr.Push(L, str5);
            return 1;
        }
        if ((count == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(string)))
        {
            string str6 = LuaScriptMgr.GetString(L, 1);
            string str7 = LuaScriptMgr.GetString(L, 2);
            string str8 = LuaScriptMgr.GetString(L, 3);
            string str9 = str6 + str7 + str8;
            LuaScriptMgr.Push(L, str9);
            return 1;
        }
        if ((count == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(object), typeof(object), typeof(object)))
        {
            object obj5 = LuaScriptMgr.GetVarObject(L, 1);
            object obj6 = LuaScriptMgr.GetVarObject(L, 2);
            object obj7 = LuaScriptMgr.GetVarObject(L, 3);
            string str10 = obj5 + obj6 + obj7;
            LuaScriptMgr.Push(L, str10);
            return 1;
        }
        if ((count == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(string), typeof(string)))
        {
            string str11 = LuaScriptMgr.GetString(L, 1);
            string str12 = LuaScriptMgr.GetString(L, 2);
            string str13 = LuaScriptMgr.GetString(L, 3);
            string str14 = LuaScriptMgr.GetString(L, 4);
            string str15 = str11 + str12 + str13 + str14;
            LuaScriptMgr.Push(L, str15);
            return 1;
        }
        if ((count == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(object), typeof(object), typeof(object), typeof(object)))
        {
            object obj8 = LuaScriptMgr.GetVarObject(L, 1);
            object obj9 = LuaScriptMgr.GetVarObject(L, 2);
            object obj10 = LuaScriptMgr.GetVarObject(L, 3);
            object obj11 = LuaScriptMgr.GetVarObject(L, 4);
            object[] objArray1 = new object[] { obj8, obj9, obj10, obj11 };
            string str16 = string.Concat(objArray1);
            LuaScriptMgr.Push(L, str16);
            return 1;
        }
        if (LuaScriptMgr.CheckParamsType(L, typeof(string), 1, count))
        {
            string str17 = string.Concat(LuaScriptMgr.GetParamsString(L, 1, count));
            LuaScriptMgr.Push(L, str17);
            return 1;
        }
        if (LuaScriptMgr.CheckParamsType(L, typeof(object), 1, count))
        {
            string str18 = string.Concat(LuaScriptMgr.GetParamsObject(L, 1, count));
            LuaScriptMgr.Push(L, str18);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Concat");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Contains(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        bool b = str.Contains(luaString);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Copy(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = string.Copy(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CopyTo(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 5);
        string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 3);
        int destinationIndex = (int) LuaScriptMgr.GetNumber(L, 4);
        int count = (int) LuaScriptMgr.GetNumber(L, 5);
        str.CopyTo(number, arrayNumber, destinationIndex, count);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int EndsWith(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                bool b = str.EndsWith(luaString);
                LuaScriptMgr.Push(L, b);
                return 1;
            }
            case 3:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                string str4 = LuaScriptMgr.GetLuaString(L, 2);
                StringComparison comparisonType = (StringComparison) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(StringComparison)));
                bool flag2 = str3.EndsWith(str4, comparisonType);
                LuaScriptMgr.Push(L, flag2);
                return 1;
            }
            case 4:
            {
                string str5 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                string str6 = LuaScriptMgr.GetLuaString(L, 2);
                bool boolean = LuaScriptMgr.GetBoolean(L, 3);
                CultureInfo culture = (CultureInfo) LuaScriptMgr.GetNetObject(L, 4, typeof(CultureInfo));
                bool flag4 = str5.EndsWith(str6, boolean, culture);
                LuaScriptMgr.Push(L, flag4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.EndsWith");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Equals(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 2, typeof(string)))
        {
            string varObject = LuaScriptMgr.GetVarObject(L, 1) as string;
            string str2 = LuaScriptMgr.GetString(L, 2);
            bool b = (varObject == null) ? (str2 == null) : varObject.Equals(str2);
            LuaScriptMgr.Push(L, b);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 2, typeof(object)))
        {
            string str3 = LuaScriptMgr.GetVarObject(L, 1) as string;
            object obj2 = LuaScriptMgr.GetVarObject(L, 2);
            bool flag2 = (str3 == null) ? (obj2 == null) : str3.Equals(obj2);
            LuaScriptMgr.Push(L, flag2);
            return 1;
        }
        if (num == 3)
        {
            string str4 = LuaScriptMgr.GetVarObject(L, 1) as string;
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            StringComparison comparisonType = (StringComparison) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(StringComparison)));
            bool flag3 = (str4 == null) ? (luaString == null) : str4.Equals(luaString, comparisonType);
            LuaScriptMgr.Push(L, flag3);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Equals");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Format(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 2)
        {
            string luaString = LuaScriptMgr.GetLuaString(L, 1);
            object varObject = LuaScriptMgr.GetVarObject(L, 2);
            string str = string.Format(luaString, varObject);
            LuaScriptMgr.Push(L, str);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(object), typeof(object)))
        {
            string format = LuaScriptMgr.GetString(L, 1);
            object obj3 = LuaScriptMgr.GetVarObject(L, 2);
            object obj4 = LuaScriptMgr.GetVarObject(L, 3);
            string str4 = string.Format(format, obj3, obj4);
            LuaScriptMgr.Push(L, str4);
            return 1;
        }
        if (num == 4)
        {
            string str5 = LuaScriptMgr.GetLuaString(L, 1);
            object obj5 = LuaScriptMgr.GetVarObject(L, 2);
            object obj6 = LuaScriptMgr.GetVarObject(L, 3);
            object obj7 = LuaScriptMgr.GetVarObject(L, 4);
            string str6 = string.Format(str5, obj5, obj6, obj7);
            LuaScriptMgr.Push(L, str6);
            return 1;
        }
        if (LuaScriptMgr.CheckTypes(L, 1, typeof(IFormatProvider), typeof(string)) && LuaScriptMgr.CheckParamsType(L, typeof(object), 3, num - 2))
        {
            IFormatProvider luaObject = (IFormatProvider) LuaScriptMgr.GetLuaObject(L, 1);
            string str7 = LuaScriptMgr.GetString(L, 2);
            object[] args = LuaScriptMgr.GetParamsObject(L, 3, num - 2);
            string str8 = string.Format(luaObject, str7, args);
            LuaScriptMgr.Push(L, str8);
            return 1;
        }
        if (LuaScriptMgr.CheckTypes(L, 1, typeof(string)) && LuaScriptMgr.CheckParamsType(L, typeof(object), 2, num - 1))
        {
            string str9 = LuaScriptMgr.GetString(L, 1);
            object[] objArray2 = LuaScriptMgr.GetParamsObject(L, 2, num - 1);
            string str10 = string.Format(str9, objArray2);
            LuaScriptMgr.Push(L, str10);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Format");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Empty(IntPtr L)
    {
        LuaScriptMgr.Push(L, string.Empty);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Length(IntPtr L)
    {
        string luaObject = (string) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Length");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Length on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.Length);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetEnumerator(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        CharEnumerator o = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).GetEnumerator();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetHashCode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int hashCode = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).GetHashCode();
        LuaScriptMgr.Push(L, hashCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTypeCode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        TypeCode typeCode = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).GetTypeCode();
        LuaScriptMgr.Push(L, typeCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IndexOf(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char)))
        {
            string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char ch = (char) ((ushort) LuaDLL.lua_tonumber(L, 2));
            int index = str.IndexOf(ch);
            LuaScriptMgr.Push(L, index);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
        {
            string str2 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string str3 = LuaScriptMgr.GetString(L, 2);
            int d = str2.IndexOf(str3);
            LuaScriptMgr.Push(L, d);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int)))
        {
            string str4 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string str5 = LuaScriptMgr.GetString(L, 2);
            int startIndex = (int) LuaDLL.lua_tonumber(L, 3);
            int num5 = str4.IndexOf(str5, startIndex);
            LuaScriptMgr.Push(L, num5);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char), typeof(int)))
        {
            string str6 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char ch2 = (char) ((ushort) LuaDLL.lua_tonumber(L, 2));
            int num6 = (int) LuaDLL.lua_tonumber(L, 3);
            int num7 = str6.IndexOf(ch2, num6);
            LuaScriptMgr.Push(L, num7);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(StringComparison)))
        {
            string str7 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string str8 = LuaScriptMgr.GetString(L, 2);
            StringComparison luaObject = (StringComparison) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            int num8 = str7.IndexOf(str8, luaObject);
            LuaScriptMgr.Push(L, num8);
            return 1;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int), typeof(int)))
        {
            string str9 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string str10 = LuaScriptMgr.GetString(L, 2);
            int num9 = (int) LuaDLL.lua_tonumber(L, 3);
            int count = (int) LuaDLL.lua_tonumber(L, 4);
            int num11 = str9.IndexOf(str10, num9, count);
            LuaScriptMgr.Push(L, num11);
            return 1;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int), typeof(StringComparison)))
        {
            string str11 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string str12 = LuaScriptMgr.GetString(L, 2);
            int num12 = (int) LuaDLL.lua_tonumber(L, 3);
            StringComparison comparisonType = (StringComparison) ((int) LuaScriptMgr.GetLuaObject(L, 4));
            int num13 = str11.IndexOf(str12, num12, comparisonType);
            LuaScriptMgr.Push(L, num13);
            return 1;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char), typeof(int), typeof(int)))
        {
            string str13 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char ch3 = (char) ((ushort) LuaDLL.lua_tonumber(L, 2));
            int num14 = (int) LuaDLL.lua_tonumber(L, 3);
            int num15 = (int) LuaDLL.lua_tonumber(L, 4);
            int num16 = str13.IndexOf(ch3, num14, num15);
            LuaScriptMgr.Push(L, num16);
            return 1;
        }
        if (num == 5)
        {
            string str14 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            int number = (int) LuaScriptMgr.GetNumber(L, 3);
            int num18 = (int) LuaScriptMgr.GetNumber(L, 4);
            StringComparison comparison3 = (StringComparison) ((int) LuaScriptMgr.GetNetObject(L, 5, typeof(StringComparison)));
            int num19 = str14.IndexOf(luaString, number, num18, comparison3);
            LuaScriptMgr.Push(L, num19);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.IndexOf");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IndexOfAny(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
                int d = str.IndexOfAny(arrayNumber);
                LuaScriptMgr.Push(L, d);
                return 1;
            }
            case 3:
            {
                string str2 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                char[] anyOf = LuaScriptMgr.GetArrayNumber<char>(L, 2);
                int number = (int) LuaScriptMgr.GetNumber(L, 3);
                int num4 = str2.IndexOfAny(anyOf, number);
                LuaScriptMgr.Push(L, num4);
                return 1;
            }
            case 4:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                char[] chArray3 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
                int startIndex = (int) LuaScriptMgr.GetNumber(L, 3);
                int count = (int) LuaScriptMgr.GetNumber(L, 4);
                int num7 = str3.IndexOfAny(chArray3, startIndex, count);
                LuaScriptMgr.Push(L, num7);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.IndexOfAny");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Insert(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        string luaString = LuaScriptMgr.GetLuaString(L, 3);
        string str3 = str.Insert(number, luaString);
        LuaScriptMgr.Push(L, str3);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Intern(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = string.Intern(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsInterned(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = string.IsInterned(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsNormalized(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                bool b = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).IsNormalized();
                LuaScriptMgr.Push(L, b);
                return 1;
            }
            case 2:
            {
                string str2 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                NormalizationForm normalizationForm = (NormalizationForm) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(NormalizationForm)));
                bool flag2 = str2.IsNormalized(normalizationForm);
                LuaScriptMgr.Push(L, flag2);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.IsNormalized");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsNullOrEmpty(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        bool b = string.IsNullOrEmpty(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Join(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string luaString = LuaScriptMgr.GetLuaString(L, 1);
                string[] arrayString = LuaScriptMgr.GetArrayString(L, 2);
                string str = string.Join(luaString, arrayString);
                LuaScriptMgr.Push(L, str);
                return 1;
            }
            case 4:
            {
                string separator = LuaScriptMgr.GetLuaString(L, 1);
                string[] strArray2 = LuaScriptMgr.GetArrayString(L, 2);
                int number = (int) LuaScriptMgr.GetNumber(L, 3);
                int count = (int) LuaScriptMgr.GetNumber(L, 4);
                string str4 = string.Join(separator, strArray2, number, count);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Join");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LastIndexOf(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char)))
        {
            string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char ch = (char) ((ushort) LuaDLL.lua_tonumber(L, 2));
            int d = str.LastIndexOf(ch);
            LuaScriptMgr.Push(L, d);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
        {
            string str2 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string str3 = LuaScriptMgr.GetString(L, 2);
            int num3 = str2.LastIndexOf(str3);
            LuaScriptMgr.Push(L, num3);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int)))
        {
            string str4 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string str5 = LuaScriptMgr.GetString(L, 2);
            int startIndex = (int) LuaDLL.lua_tonumber(L, 3);
            int num5 = str4.LastIndexOf(str5, startIndex);
            LuaScriptMgr.Push(L, num5);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char), typeof(int)))
        {
            string str6 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char ch2 = (char) ((ushort) LuaDLL.lua_tonumber(L, 2));
            int num6 = (int) LuaDLL.lua_tonumber(L, 3);
            int num7 = str6.LastIndexOf(ch2, num6);
            LuaScriptMgr.Push(L, num7);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(StringComparison)))
        {
            string str7 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string str8 = LuaScriptMgr.GetString(L, 2);
            StringComparison luaObject = (StringComparison) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            int num8 = str7.LastIndexOf(str8, luaObject);
            LuaScriptMgr.Push(L, num8);
            return 1;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int), typeof(int)))
        {
            string str9 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string str10 = LuaScriptMgr.GetString(L, 2);
            int num9 = (int) LuaDLL.lua_tonumber(L, 3);
            int count = (int) LuaDLL.lua_tonumber(L, 4);
            int num11 = str9.LastIndexOf(str10, num9, count);
            LuaScriptMgr.Push(L, num11);
            return 1;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(int), typeof(StringComparison)))
        {
            string str11 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string str12 = LuaScriptMgr.GetString(L, 2);
            int num12 = (int) LuaDLL.lua_tonumber(L, 3);
            StringComparison comparisonType = (StringComparison) ((int) LuaScriptMgr.GetLuaObject(L, 4));
            int num13 = str11.LastIndexOf(str12, num12, comparisonType);
            LuaScriptMgr.Push(L, num13);
            return 1;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char), typeof(int), typeof(int)))
        {
            string str13 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char ch3 = (char) ((ushort) LuaDLL.lua_tonumber(L, 2));
            int num14 = (int) LuaDLL.lua_tonumber(L, 3);
            int num15 = (int) LuaDLL.lua_tonumber(L, 4);
            int num16 = str13.LastIndexOf(ch3, num14, num15);
            LuaScriptMgr.Push(L, num16);
            return 1;
        }
        if (num == 5)
        {
            string str14 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            int number = (int) LuaScriptMgr.GetNumber(L, 3);
            int num18 = (int) LuaScriptMgr.GetNumber(L, 4);
            StringComparison comparison3 = (StringComparison) ((int) LuaScriptMgr.GetNetObject(L, 5, typeof(StringComparison)));
            int num19 = str14.LastIndexOf(luaString, number, num18, comparison3);
            LuaScriptMgr.Push(L, num19);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.LastIndexOf");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LastIndexOfAny(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
                int d = str.LastIndexOfAny(arrayNumber);
                LuaScriptMgr.Push(L, d);
                return 1;
            }
            case 3:
            {
                string str2 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                char[] anyOf = LuaScriptMgr.GetArrayNumber<char>(L, 2);
                int number = (int) LuaScriptMgr.GetNumber(L, 3);
                int num4 = str2.LastIndexOfAny(anyOf, number);
                LuaScriptMgr.Push(L, num4);
                return 1;
            }
            case 4:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                char[] chArray3 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
                int startIndex = (int) LuaScriptMgr.GetNumber(L, 3);
                int count = (int) LuaScriptMgr.GetNumber(L, 4);
                int num7 = str3.LastIndexOfAny(chArray3, startIndex, count);
                LuaScriptMgr.Push(L, num7);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.LastIndexOfAny");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Eq(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        string str2 = LuaScriptMgr.GetLuaString(L, 2);
        bool b = luaString == str2;
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_ToString(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject != null)
        {
            LuaScriptMgr.Push(L, luaObject.ToString());
        }
        else
        {
            LuaScriptMgr.Push(L, "Table: System.String");
        }
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Normalize(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                string str = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).Normalize();
                LuaScriptMgr.Push(L, str);
                return 1;
            }
            case 2:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                NormalizationForm normalizationForm = (NormalizationForm) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(NormalizationForm)));
                string str4 = str3.Normalize(normalizationForm);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Normalize");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int PadLeft(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                int number = (int) LuaScriptMgr.GetNumber(L, 2);
                string str2 = str.PadLeft(number);
                LuaScriptMgr.Push(L, str2);
                return 1;
            }
            case 3:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                int totalWidth = (int) LuaScriptMgr.GetNumber(L, 2);
                char paddingChar = (char) ((ushort) LuaScriptMgr.GetNumber(L, 3));
                string str4 = str3.PadLeft(totalWidth, paddingChar);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.PadLeft");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int PadRight(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                int number = (int) LuaScriptMgr.GetNumber(L, 2);
                string str2 = str.PadRight(number);
                LuaScriptMgr.Push(L, str2);
                return 1;
            }
            case 3:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                int totalWidth = (int) LuaScriptMgr.GetNumber(L, 2);
                char paddingChar = (char) ((ushort) LuaScriptMgr.GetNumber(L, 3));
                string str4 = str3.PadRight(totalWidth, paddingChar);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.PadRight");
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("Clone", new LuaCSFunction(stringWrap.Clone)), new LuaMethod("GetTypeCode", new LuaCSFunction(stringWrap.GetTypeCode)), new LuaMethod("CopyTo", new LuaCSFunction(stringWrap.CopyTo)), new LuaMethod("ToCharArray", new LuaCSFunction(stringWrap.ToCharArray)), new LuaMethod("Split", new LuaCSFunction(stringWrap.Split)), new LuaMethod("Substring", new LuaCSFunction(stringWrap.Substring)), new LuaMethod("Trim", new LuaCSFunction(stringWrap.Trim)), new LuaMethod("TrimStart", new LuaCSFunction(stringWrap.TrimStart)), new LuaMethod("TrimEnd", new LuaCSFunction(stringWrap.TrimEnd)), new LuaMethod("Compare", new LuaCSFunction(stringWrap.Compare)), new LuaMethod("CompareTo", new LuaCSFunction(stringWrap.CompareTo)), new LuaMethod("CompareOrdinal", new LuaCSFunction(stringWrap.CompareOrdinal)), new LuaMethod("EndsWith", new LuaCSFunction(stringWrap.EndsWith)), new LuaMethod("IndexOfAny", new LuaCSFunction(stringWrap.IndexOfAny)), new LuaMethod("IndexOf", new LuaCSFunction(stringWrap.IndexOf)), new LuaMethod("LastIndexOf", new LuaCSFunction(stringWrap.LastIndexOf)), 
            new LuaMethod("LastIndexOfAny", new LuaCSFunction(stringWrap.LastIndexOfAny)), new LuaMethod("Contains", new LuaCSFunction(stringWrap.Contains)), new LuaMethod("IsNullOrEmpty", new LuaCSFunction(stringWrap.IsNullOrEmpty)), new LuaMethod("Normalize", new LuaCSFunction(stringWrap.Normalize)), new LuaMethod("IsNormalized", new LuaCSFunction(stringWrap.IsNormalized)), new LuaMethod("Remove", new LuaCSFunction(stringWrap.Remove)), new LuaMethod("PadLeft", new LuaCSFunction(stringWrap.PadLeft)), new LuaMethod("PadRight", new LuaCSFunction(stringWrap.PadRight)), new LuaMethod("StartsWith", new LuaCSFunction(stringWrap.StartsWith)), new LuaMethod("Replace", new LuaCSFunction(stringWrap.Replace)), new LuaMethod("ToLower", new LuaCSFunction(stringWrap.ToLower)), new LuaMethod("ToLowerInvariant", new LuaCSFunction(stringWrap.ToLowerInvariant)), new LuaMethod("ToUpper", new LuaCSFunction(stringWrap.ToUpper)), new LuaMethod("ToUpperInvariant", new LuaCSFunction(stringWrap.ToUpperInvariant)), new LuaMethod("ToString", new LuaCSFunction(stringWrap.ToString)), new LuaMethod("Format", new LuaCSFunction(stringWrap.Format)), 
            new LuaMethod("Copy", new LuaCSFunction(stringWrap.Copy)), new LuaMethod("Concat", new LuaCSFunction(stringWrap.Concat)), new LuaMethod("Insert", new LuaCSFunction(stringWrap.Insert)), new LuaMethod("Intern", new LuaCSFunction(stringWrap.Intern)), new LuaMethod("IsInterned", new LuaCSFunction(stringWrap.IsInterned)), new LuaMethod("Join", new LuaCSFunction(stringWrap.Join)), new LuaMethod("GetEnumerator", new LuaCSFunction(stringWrap.GetEnumerator)), new LuaMethod("GetHashCode", new LuaCSFunction(stringWrap.GetHashCode)), new LuaMethod("Equals", new LuaCSFunction(stringWrap.Equals)), new LuaMethod("New", new LuaCSFunction(stringWrap._Createstring)), new LuaMethod("GetClassType", new LuaCSFunction(stringWrap.GetClassType)), new LuaMethod("__tostring", new LuaCSFunction(stringWrap.Lua_ToString)), new LuaMethod("__eq", new LuaCSFunction(stringWrap.Lua_Eq))
         };
        LuaField[] fields = new LuaField[] { new LuaField("Empty", new LuaCSFunction(stringWrap.get_Empty), null), new LuaField("Length", new LuaCSFunction(stringWrap.get_Length), null) };
        LuaScriptMgr.RegisterLib(L, "System.String", typeof(string), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Remove(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                int number = (int) LuaScriptMgr.GetNumber(L, 2);
                string str2 = str.Remove(number);
                LuaScriptMgr.Push(L, str2);
                return 1;
            }
            case 3:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                int startIndex = (int) LuaScriptMgr.GetNumber(L, 2);
                int count = (int) LuaScriptMgr.GetNumber(L, 3);
                string str4 = str3.Remove(startIndex, count);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Remove");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Replace(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string), typeof(string)))
        {
            string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string oldValue = LuaScriptMgr.GetString(L, 2);
            string newValue = LuaScriptMgr.GetString(L, 3);
            string str4 = str.Replace(oldValue, newValue);
            LuaScriptMgr.Push(L, str4);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char), typeof(char)))
        {
            string str5 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char oldChar = (char) ((ushort) LuaDLL.lua_tonumber(L, 2));
            char newChar = (char) ((ushort) LuaDLL.lua_tonumber(L, 3));
            string str6 = str5.Replace(oldChar, newChar);
            LuaScriptMgr.Push(L, str6);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Replace");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Split(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char[]), typeof(StringSplitOptions)))
        {
            string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
            StringSplitOptions luaObject = (StringSplitOptions) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            string[] o = str.Split(arrayNumber, luaObject);
            LuaScriptMgr.PushArray(L, o);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char[]), typeof(int)))
        {
            string str2 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char[] separator = LuaScriptMgr.GetArrayNumber<char>(L, 2);
            int count = (int) LuaDLL.lua_tonumber(L, 3);
            string[] strArray2 = str2.Split(separator, count);
            LuaScriptMgr.PushArray(L, strArray2);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string[]), typeof(StringSplitOptions)))
        {
            string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string[] arrayString = LuaScriptMgr.GetArrayString(L, 2);
            StringSplitOptions options = (StringSplitOptions) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            string[] strArray4 = str3.Split(arrayString, options);
            LuaScriptMgr.PushArray(L, strArray4);
            return 1;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string[]), typeof(int), typeof(StringSplitOptions)))
        {
            string str4 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            string[] strArray5 = LuaScriptMgr.GetArrayString(L, 2);
            int num3 = (int) LuaDLL.lua_tonumber(L, 3);
            StringSplitOptions options3 = (StringSplitOptions) ((int) LuaScriptMgr.GetLuaObject(L, 4));
            string[] strArray6 = str4.Split(strArray5, num3, options3);
            LuaScriptMgr.PushArray(L, strArray6);
            return 1;
        }
        if ((num == 4) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(char[]), typeof(int), typeof(StringSplitOptions)))
        {
            string str5 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char[] chArray3 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
            int num4 = (int) LuaDLL.lua_tonumber(L, 3);
            StringSplitOptions options4 = (StringSplitOptions) ((int) LuaScriptMgr.GetLuaObject(L, 4));
            string[] strArray7 = str5.Split(chArray3, num4, options4);
            LuaScriptMgr.PushArray(L, strArray7);
            return 1;
        }
        if (LuaScriptMgr.CheckParamsType(L, typeof(char), 2, num - 1))
        {
            string str6 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char[] chArray4 = LuaScriptMgr.GetArrayNumber<char>(L, 2);
            string[] strArray8 = str6.Split(chArray4);
            LuaScriptMgr.PushArray(L, strArray8);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Split");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int StartsWith(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                bool b = str.StartsWith(luaString);
                LuaScriptMgr.Push(L, b);
                return 1;
            }
            case 3:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                string str4 = LuaScriptMgr.GetLuaString(L, 2);
                StringComparison comparisonType = (StringComparison) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(StringComparison)));
                bool flag2 = str3.StartsWith(str4, comparisonType);
                LuaScriptMgr.Push(L, flag2);
                return 1;
            }
            case 4:
            {
                string str5 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                string str6 = LuaScriptMgr.GetLuaString(L, 2);
                bool boolean = LuaScriptMgr.GetBoolean(L, 3);
                CultureInfo culture = (CultureInfo) LuaScriptMgr.GetNetObject(L, 4, typeof(CultureInfo));
                bool flag4 = str5.StartsWith(str6, boolean, culture);
                LuaScriptMgr.Push(L, flag4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.StartsWith");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Substring(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                int number = (int) LuaScriptMgr.GetNumber(L, 2);
                string str2 = str.Substring(number);
                LuaScriptMgr.Push(L, str2);
                return 1;
            }
            case 3:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                int startIndex = (int) LuaScriptMgr.GetNumber(L, 2);
                int length = (int) LuaScriptMgr.GetNumber(L, 3);
                string str4 = str3.Substring(startIndex, length);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Substring");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToCharArray(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                char[] o = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).ToCharArray();
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
            case 3:
            {
                string str2 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                int number = (int) LuaScriptMgr.GetNumber(L, 2);
                int length = (int) LuaScriptMgr.GetNumber(L, 3);
                char[] chArray2 = str2.ToCharArray(number, length);
                LuaScriptMgr.PushArray(L, chArray2);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.ToCharArray");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToLower(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                string str = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).ToLower();
                LuaScriptMgr.Push(L, str);
                return 1;
            }
            case 2:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                CultureInfo culture = (CultureInfo) LuaScriptMgr.GetNetObject(L, 2, typeof(CultureInfo));
                string str4 = str3.ToLower(culture);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.ToLower");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToLowerInvariant(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).ToLowerInvariant();
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToString(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                string str = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).ToString();
                LuaScriptMgr.Push(L, str);
                return 1;
            }
            case 2:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                IFormatProvider provider = (IFormatProvider) LuaScriptMgr.GetNetObject(L, 2, typeof(IFormatProvider));
                string str4 = str3.ToString(provider);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.ToString");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToUpper(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                string str = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).ToUpper();
                LuaScriptMgr.Push(L, str);
                return 1;
            }
            case 2:
            {
                string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
                CultureInfo culture = (CultureInfo) LuaScriptMgr.GetNetObject(L, 2, typeof(CultureInfo));
                string str4 = str3.ToUpper(culture);
                LuaScriptMgr.Push(L, str4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.ToUpper");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToUpperInvariant(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).ToUpperInvariant();
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Trim(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 1)
        {
            string str = ((string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string")).Trim();
            LuaScriptMgr.Push(L, str);
            return 1;
        }
        if (LuaScriptMgr.CheckParamsType(L, typeof(char), 2, num - 1))
        {
            string str3 = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
            char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
            string str4 = str3.Trim(arrayNumber);
            LuaScriptMgr.Push(L, str4);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: string.Trim");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int TrimEnd(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
        char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
        string str2 = str.TrimEnd(arrayNumber);
        LuaScriptMgr.Push(L, str2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int TrimStart(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        string str = (string) LuaScriptMgr.GetNetObjectSelf(L, 1, "string");
        char[] arrayNumber = LuaScriptMgr.GetArrayNumber<char>(L, 2);
        string str2 = str.TrimStart(arrayNumber);
        LuaScriptMgr.Push(L, str2);
        return 1;
    }
}

