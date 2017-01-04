using com.tencent.pandora;
using System;

public class EnumWrap
{
    private static System.Type classType = typeof(Enum);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateEnum(IntPtr L)
    {
        LuaDLL.luaL_error(L, "Enum class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CompareTo(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Enum enum2 = (Enum) LuaScriptMgr.GetNetObjectSelf(L, 1, "Enum");
        object varObject = LuaScriptMgr.GetVarObject(L, 2);
        int d = enum2.CompareTo(varObject);
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Equals(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Enum varObject = LuaScriptMgr.GetVarObject(L, 1) as Enum;
        object obj2 = LuaScriptMgr.GetVarObject(L, 2);
        bool b = (varObject == 0) ? (obj2 == null) : varObject.Equals(obj2);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Format(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
        object varObject = LuaScriptMgr.GetVarObject(L, 2);
        string luaString = LuaScriptMgr.GetLuaString(L, 3);
        string str = Enum.Format(typeObject, varObject, luaString);
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetHashCode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int hashCode = ((Enum) LuaScriptMgr.GetNetObjectSelf(L, 1, "Enum")).GetHashCode();
        LuaScriptMgr.Push(L, hashCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetName(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
        object varObject = LuaScriptMgr.GetVarObject(L, 2);
        string name = Enum.GetName(typeObject, varObject);
        LuaScriptMgr.Push(L, name);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetNames(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string[] names = Enum.GetNames(LuaScriptMgr.GetTypeObject(L, 1));
        LuaScriptMgr.PushArray(L, names);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTypeCode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        TypeCode typeCode = ((Enum) LuaScriptMgr.GetNetObjectSelf(L, 1, "Enum")).GetTypeCode();
        LuaScriptMgr.Push(L, typeCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetUnderlyingType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type underlyingType = Enum.GetUnderlyingType(LuaScriptMgr.GetTypeObject(L, 1));
        LuaScriptMgr.Push(L, underlyingType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetValues(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Array values = Enum.GetValues(LuaScriptMgr.GetTypeObject(L, 1));
        LuaScriptMgr.PushObject(L, values);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsDefined(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
        object varObject = LuaScriptMgr.GetVarObject(L, 2);
        bool b = Enum.IsDefined(typeObject, varObject);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Eq(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Enum luaObject = LuaScriptMgr.GetLuaObject(L, 1) as Enum;
        Enum enum3 = LuaScriptMgr.GetLuaObject(L, 2) as Enum;
        bool b = luaObject == enum3;
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
            LuaScriptMgr.Push(L, "Table: System.Enum");
        }
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Parse(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                object o = Enum.Parse(typeObject, luaString);
                LuaScriptMgr.PushVarObject(L, o);
                return 1;
            }
            case 3:
            {
                System.Type enumType = LuaScriptMgr.GetTypeObject(L, 1);
                string str2 = LuaScriptMgr.GetLuaString(L, 2);
                bool boolean = LuaScriptMgr.GetBoolean(L, 3);
                object obj3 = Enum.Parse(enumType, str2, boolean);
                LuaScriptMgr.PushVarObject(L, obj3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Enum.Parse");
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("GetTypeCode", new LuaCSFunction(EnumWrap.GetTypeCode)), new LuaMethod("GetValues", new LuaCSFunction(EnumWrap.GetValues)), new LuaMethod("GetNames", new LuaCSFunction(EnumWrap.GetNames)), new LuaMethod("GetName", new LuaCSFunction(EnumWrap.GetName)), new LuaMethod("IsDefined", new LuaCSFunction(EnumWrap.IsDefined)), new LuaMethod("GetUnderlyingType", new LuaCSFunction(EnumWrap.GetUnderlyingType)), new LuaMethod("Parse", new LuaCSFunction(EnumWrap.Parse)), new LuaMethod("CompareTo", new LuaCSFunction(EnumWrap.CompareTo)), new LuaMethod("ToString", new LuaCSFunction(EnumWrap.ToString)), new LuaMethod("ToObject", new LuaCSFunction(EnumWrap.ToObject)), new LuaMethod("Format", new LuaCSFunction(EnumWrap.Format)), new LuaMethod("GetHashCode", new LuaCSFunction(EnumWrap.GetHashCode)), new LuaMethod("Equals", new LuaCSFunction(EnumWrap.Equals)), new LuaMethod("New", new LuaCSFunction(EnumWrap._CreateEnum)), new LuaMethod("GetClassType", new LuaCSFunction(EnumWrap.GetClassType)), new LuaMethod("__tostring", new LuaCSFunction(EnumWrap.Lua_ToString)), 
            new LuaMethod("__eq", new LuaCSFunction(EnumWrap.Lua_Eq))
         };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "System.Enum", typeof(Enum), regs, fields, null);
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToObject(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(System.Type), typeof(long)))
        {
            System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
            long num2 = (long) LuaDLL.lua_tonumber(L, 2);
            object o = Enum.ToObject(typeObject, num2);
            LuaScriptMgr.PushVarObject(L, o);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(System.Type), typeof(object)))
        {
            System.Type enumType = LuaScriptMgr.GetTypeObject(L, 1);
            object varObject = LuaScriptMgr.GetVarObject(L, 2);
            object obj4 = Enum.ToObject(enumType, varObject);
            LuaScriptMgr.PushVarObject(L, obj4);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Enum.ToObject");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToString(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                string str = ((Enum) LuaScriptMgr.GetNetObjectSelf(L, 1, "Enum")).ToString();
                LuaScriptMgr.Push(L, str);
                return 1;
            }
            case 2:
            {
                Enum enum3 = (Enum) LuaScriptMgr.GetNetObjectSelf(L, 1, "Enum");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                string str3 = enum3.ToString(luaString);
                LuaScriptMgr.Push(L, str3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Enum.ToString");
        return 0;
    }
}

