using com.tencent.pandora;
using System;

public class com_tencent_pandora_DelegateFactoryWrap
{
    private static System.Type classType = typeof(DelegateFactory);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_DelegateFactory(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.DelegateFactory class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Action(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.Action(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Action_AssetBundle(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.Action_AssetBundle(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Action_bool(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.Action_bool(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Action_GameObject(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.Action_GameObject(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Action_WWW_string(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.Action_WWW_string(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Application_LogCallback(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.Application_LogCallback(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Clear(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        DelegateFactory.Clear();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int com_tencent_pandora_GetNewsImage_Callback(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.com_tencent_pandora_GetNewsImage_Callback(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int com_tencent_pandora_GetNewsImage_callbackFuc(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.com_tencent_pandora_GetNewsImage_callbackFuc(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int com_tencent_pandora_NetProxcy_CallBack(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.com_tencent_pandora_NetProxcy_CallBack(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Action_GameObject", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.Action_GameObject)), new LuaMethod("Action", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.Action)), new LuaMethod("UnityEngine_Events_UnityAction", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.UnityEngine_Events_UnityAction)), new LuaMethod("System_Reflection_MemberFilter", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.System_Reflection_MemberFilter)), new LuaMethod("System_Reflection_TypeFilter", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.System_Reflection_TypeFilter)), new LuaMethod("Action_bool", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.Action_bool)), new LuaMethod("Action_AssetBundle", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.Action_AssetBundle)), new LuaMethod("Action_WWW_string", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.Action_WWW_string)), new LuaMethod("Application_LogCallback", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.Application_LogCallback)), new LuaMethod("com_tencent_pandora_GetNewsImage_callbackFuc", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.com_tencent_pandora_GetNewsImage_callbackFuc)), new LuaMethod("com_tencent_pandora_GetNewsImage_Callback", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.com_tencent_pandora_GetNewsImage_Callback)), new LuaMethod("com_tencent_pandora_NetProxcy_CallBack", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.com_tencent_pandora_NetProxcy_CallBack)), new LuaMethod("Clear", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.Clear)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap._Createcom_tencent_pandora_DelegateFactory)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.GetClassType)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.DelegateFactory", regs);
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int System_Reflection_MemberFilter(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.System_Reflection_MemberFilter(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int System_Reflection_TypeFilter(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.System_Reflection_TypeFilter(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int UnityEngine_Events_UnityAction(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Delegate o = DelegateFactory.UnityEngine_Events_UnityAction(LuaScriptMgr.GetLuaFunction(L, 1));
        LuaScriptMgr.Push(L, o);
        return 1;
    }
}

