using com.tencent.pandora;
using System;

public class com_tencent_pandora_ThirdSDKWrap
{
    private static System.Type classType = typeof(ThirdSDK);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_ThirdSDK(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            ThirdSDK o = new ThirdSDK();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.ThirdSDK.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int BuyGoods(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 5);
        ThirdSDK dsdk = (ThirdSDK) LuaScriptMgr.GetNetObjectSelf(L, 1, "com.tencent.pandora.ThirdSDK");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        string iActionId = LuaScriptMgr.GetLuaString(L, 3);
        string payType = LuaScriptMgr.GetLuaString(L, 4);
        int number = (int) LuaScriptMgr.GetNumber(L, 5);
        dsdk.BuyGoods(luaString, iActionId, payType, number);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetInstance(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        ThirdSDK instance = ThirdSDK.GetInstance();
        LuaScriptMgr.PushObject(L, instance);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int midasPay(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 13);
        ThirdSDK dsdk = (ThirdSDK) LuaScriptMgr.GetNetObjectSelf(L, 1, "com.tencent.pandora.ThirdSDK");
        bool boolean = LuaScriptMgr.GetBoolean(L, 2);
        string luaString = LuaScriptMgr.GetLuaString(L, 3);
        string pf = LuaScriptMgr.GetLuaString(L, 4);
        string goodsTokenUrl = LuaScriptMgr.GetLuaString(L, 5);
        string accType = LuaScriptMgr.GetLuaString(L, 6);
        string payToken = LuaScriptMgr.GetLuaString(L, 7);
        string zoneId = LuaScriptMgr.GetLuaString(L, 8);
        string pfKey = LuaScriptMgr.GetLuaString(L, 9);
        string openid = LuaScriptMgr.GetLuaString(L, 10);
        string str9 = LuaScriptMgr.GetLuaString(L, 11);
        string method = LuaScriptMgr.GetLuaString(L, 12);
        int number = (int) LuaScriptMgr.GetNumber(L, 13);
        dsdk.midasPay(boolean, luaString, pf, goodsTokenUrl, accType, payToken, zoneId, pfKey, openid, str9, method, number);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("GetInstance", new LuaCSFunction(com_tencent_pandora_ThirdSDKWrap.GetInstance)), new LuaMethod("midasPay", new LuaCSFunction(com_tencent_pandora_ThirdSDKWrap.midasPay)), new LuaMethod("BuyGoods", new LuaCSFunction(com_tencent_pandora_ThirdSDKWrap.BuyGoods)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_ThirdSDKWrap._Createcom_tencent_pandora_ThirdSDK)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_ThirdSDKWrap.GetClassType)) };
        LuaField[] fields = new LuaField[0];
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.ThirdSDK", typeof(ThirdSDK), regs, fields, typeof(object));
    }
}

