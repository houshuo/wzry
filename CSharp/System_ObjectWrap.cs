using com.tencent.pandora;
using System;

public class System_ObjectWrap
{
    private static System.Type classType = typeof(object);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateSystem_Object(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            object o = new object();
            LuaScriptMgr.PushVarObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: object.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Destroy(IntPtr L)
    {
        LuaScriptMgr.__gc(L);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Equals(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        object varObject = LuaScriptMgr.GetVarObject(L, 1);
        object obj3 = LuaScriptMgr.GetVarObject(L, 2);
        bool b = (varObject == null) ? (obj3 == null) : varObject.Equals(obj3);
        LuaScriptMgr.Push(L, b);
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
        int hashCode = LuaScriptMgr.GetNetObjectSelf(L, 1, "object").GetHashCode();
        LuaScriptMgr.Push(L, hashCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type o = LuaScriptMgr.GetNetObjectSelf(L, 1, "object").GetType();
        LuaScriptMgr.Push(L, o);
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
            LuaScriptMgr.Push(L, "Table: System.Object");
        }
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ReferenceEquals(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        object varObject = LuaScriptMgr.GetVarObject(L, 1);
        object objB = LuaScriptMgr.GetVarObject(L, 2);
        bool b = object.ReferenceEquals(varObject, objB);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Equals", new LuaCSFunction(System_ObjectWrap.Equals)), new LuaMethod("GetHashCode", new LuaCSFunction(System_ObjectWrap.GetHashCode)), new LuaMethod("GetType", new LuaCSFunction(System_ObjectWrap.GetType)), new LuaMethod("ToString", new LuaCSFunction(System_ObjectWrap.ToString)), new LuaMethod("ReferenceEquals", new LuaCSFunction(System_ObjectWrap.ReferenceEquals)), new LuaMethod("Destroy", new LuaCSFunction(System_ObjectWrap.Destroy)), new LuaMethod("New", new LuaCSFunction(System_ObjectWrap._CreateSystem_Object)), new LuaMethod("GetClassType", new LuaCSFunction(System_ObjectWrap.GetClassType)), new LuaMethod("__tostring", new LuaCSFunction(System_ObjectWrap.Lua_ToString)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "System.Object", typeof(object), regs, fields, null);
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToString(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = LuaScriptMgr.GetNetObjectSelf(L, 1, "object").ToString();
        LuaScriptMgr.Push(L, str);
        return 1;
    }
}

