using com.tencent.pandora;
using System;
using System.Collections.Generic;

public class com_tencent_pandora_CUserDataWrap
{
    private static System.Type classType = typeof(CUserData);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_CUserData(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            CUserData o = new CUserData();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.CUserData.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_user(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, CUserData.user);
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("setUserInfo", new LuaCSFunction(com_tencent_pandora_CUserDataWrap.setUserInfo)), new LuaMethod("SetPara", new LuaCSFunction(com_tencent_pandora_CUserDataWrap.SetPara)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_CUserDataWrap._Createcom_tencent_pandora_CUserData)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_CUserDataWrap.GetClassType)) };
        LuaField[] fields = new LuaField[] { new LuaField("user", new LuaCSFunction(com_tencent_pandora_CUserDataWrap.get_user), new LuaCSFunction(com_tencent_pandora_CUserDataWrap.set_user)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.CUserData", typeof(CUserData), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_user(IntPtr L)
    {
        CUserData.user = (CUserInfoData) LuaScriptMgr.GetNetObject(L, 3, typeof(CUserInfoData));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetPara(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Dictionary<string, string> dicPara = (Dictionary<string, string>) LuaScriptMgr.GetNetObject(L, 1, typeof(Dictionary<string, string>));
        CUserData.SetPara(dicPara);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int setUserInfo(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        CUserData data = (CUserData) LuaScriptMgr.GetNetObjectSelf(L, 1, "com.tencent.pandora.CUserData");
        CUserInfoData u = (CUserInfoData) LuaScriptMgr.GetNetObject(L, 2, typeof(CUserInfoData));
        data.setUserInfo(u);
        return 0;
    }
}

