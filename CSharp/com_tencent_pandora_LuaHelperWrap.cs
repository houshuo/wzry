using com.tencent.pandora;
using System;

public class com_tencent_pandora_LuaHelperWrap
{
    private static System.Type classType = typeof(LuaHelper);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_LuaHelper(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.LuaHelper class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Action(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Action o = LuaHelper.Action(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetPanelManager(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        PanelManager panelManager = LuaHelper.GetPanelManager();
        LuaScriptMgr.Push(L, panelManager);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetResManager(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        ResourceManager resManager = LuaHelper.GetResManager();
        LuaScriptMgr.Push(L, resManager);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type o = LuaHelper.GetType(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnCallLuaFunc(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        LuaStringBuffer stringBuffer = LuaScriptMgr.GetStringBuffer(L, 1);
        LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 2);
        LuaHelper.OnCallLuaFunc(stringBuffer, luaFunction);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnJsonCallFunc(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 2);
        LuaHelper.OnJsonCallFunc(luaString, luaFunction);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("GetType", new LuaCSFunction(com_tencent_pandora_LuaHelperWrap.GetType)), new LuaMethod("GetPanelManager", new LuaCSFunction(com_tencent_pandora_LuaHelperWrap.GetPanelManager)), new LuaMethod("GetResManager", new LuaCSFunction(com_tencent_pandora_LuaHelperWrap.GetResManager)), new LuaMethod("Action", new LuaCSFunction(com_tencent_pandora_LuaHelperWrap.Action)), new LuaMethod("OnCallLuaFunc", new LuaCSFunction(com_tencent_pandora_LuaHelperWrap.OnCallLuaFunc)), new LuaMethod("OnJsonCallFunc", new LuaCSFunction(com_tencent_pandora_LuaHelperWrap.OnJsonCallFunc)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_LuaHelperWrap._Createcom_tencent_pandora_LuaHelper)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_LuaHelperWrap.GetClassType)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.LuaHelper", regs);
    }
}

