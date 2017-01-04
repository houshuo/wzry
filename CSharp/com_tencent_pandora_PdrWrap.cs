using com.tencent.pandora;
using System;
using UnityEngine;

public class com_tencent_pandora_PdrWrap
{
    private static System.Type classType = typeof(Pdr);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_Pdr(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.Pdr class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DoString(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Pdr.DoString(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isInitUlua(IntPtr L)
    {
        LuaScriptMgr.Push(L, Pdr.isInitUlua);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetSDKVer(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        string sDKVer = Pdr.GetSDKVer();
        LuaScriptMgr.Push(L, sDKVer);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTempPath(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string tempPath = ((Pdr) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.Pdr")).GetTempPath();
        LuaScriptMgr.Push(L, tempPath);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Log(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Pdr pdr = (Pdr) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.Pdr");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        pdr.Log(luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Eq(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        UnityEngine.Object luaObject = LuaScriptMgr.GetLuaObject(L, 1) as UnityEngine.Object;
        UnityEngine.Object obj3 = LuaScriptMgr.GetLuaObject(L, 2) as UnityEngine.Object;
        bool b = luaObject == obj3;
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LuaReady(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Pdr.LuaReady(LuaScriptMgr.GetBoolean(L, 1));
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("GetTempPath", new LuaCSFunction(com_tencent_pandora_PdrWrap.GetTempPath)), new LuaMethod("Log", new LuaCSFunction(com_tencent_pandora_PdrWrap.Log)), new LuaMethod("SendPandoraLibCmd", new LuaCSFunction(com_tencent_pandora_PdrWrap.SendPandoraLibCmd)), new LuaMethod("DoString", new LuaCSFunction(com_tencent_pandora_PdrWrap.DoString)), new LuaMethod("GetSDKVer", new LuaCSFunction(com_tencent_pandora_PdrWrap.GetSDKVer)), new LuaMethod("LuaReady", new LuaCSFunction(com_tencent_pandora_PdrWrap.LuaReady)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_PdrWrap._Createcom_tencent_pandora_Pdr)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_PdrWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_PdrWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("isInitUlua", new LuaCSFunction(com_tencent_pandora_PdrWrap.get_isInitUlua), new LuaCSFunction(com_tencent_pandora_PdrWrap.set_isInitUlua)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.Pdr", typeof(Pdr), regs, fields, typeof(MonoBehaviour));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SendPandoraLibCmd(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 4);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        int iLength = (int) LuaScriptMgr.GetNumber(L, 3);
        int iFlag = (int) LuaScriptMgr.GetNumber(L, 4);
        Pdr.SendPandoraLibCmd(number, luaString, iLength, iFlag);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_isInitUlua(IntPtr L)
    {
        Pdr.isInitUlua = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }
}

