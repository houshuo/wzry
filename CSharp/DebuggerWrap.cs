using com.tencent.pandora;
using System;

public class DebuggerWrap
{
    private static System.Type classType = typeof(Debugger);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateDebugger(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            Debugger o = new Debugger();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Debugger.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Log(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        object[] args = LuaScriptMgr.GetParamsObject(L, 2, num - 1);
        Debugger.Log(luaString, args);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LogError(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        object[] args = LuaScriptMgr.GetParamsObject(L, 2, num - 1);
        Debugger.LogError(luaString, args);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LogWarning(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        object[] args = LuaScriptMgr.GetParamsObject(L, 2, num - 1);
        Debugger.LogWarning(luaString, args);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Log", new LuaCSFunction(DebuggerWrap.Log)), new LuaMethod("LogWarning", new LuaCSFunction(DebuggerWrap.LogWarning)), new LuaMethod("LogError", new LuaCSFunction(DebuggerWrap.LogError)), new LuaMethod("New", new LuaCSFunction(DebuggerWrap._CreateDebugger)), new LuaMethod("GetClassType", new LuaCSFunction(DebuggerWrap.GetClassType)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "Debugger", typeof(Debugger), regs, fields, typeof(Logger));
    }
}

