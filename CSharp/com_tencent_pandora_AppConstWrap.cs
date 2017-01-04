using com.tencent.pandora;
using System;

public class com_tencent_pandora_AppConstWrap
{
    private static System.Type classType = typeof(AppConst);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_AppConst(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            AppConst o = new AppConst();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.AppConst.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_AppName(IntPtr L)
    {
        LuaScriptMgr.Push(L, "6l");
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_AppPrefix(IntPtr L)
    {
        LuaScriptMgr.Push(L, "6l_");
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_AutoWrapMode(IntPtr L)
    {
        LuaScriptMgr.Push(L, true);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_DebugMode(IntPtr L)
    {
        LuaScriptMgr.Push(L, false);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_ExampleMode(IntPtr L)
    {
        LuaScriptMgr.Push(L, true);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_GameFrameRate(IntPtr L)
    {
        LuaScriptMgr.Push(L, 30);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_LuaBasePath(IntPtr L)
    {
        LuaScriptMgr.Push(L, AppConst.LuaBasePath);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_LuaEncode(IntPtr L)
    {
        LuaScriptMgr.Push(L, false);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_LuaWrapPath(IntPtr L)
    {
        LuaScriptMgr.Push(L, AppConst.LuaWrapPath);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_SocketAddress(IntPtr L)
    {
        LuaScriptMgr.Push(L, AppConst.SocketAddress);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_SocketPort(IntPtr L)
    {
        LuaScriptMgr.Push(L, AppConst.SocketPort);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_TimerInterval(IntPtr L)
    {
        LuaScriptMgr.Push(L, 1);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_UpdateMode(IntPtr L)
    {
        LuaScriptMgr.Push(L, false);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_UseCJson(IntPtr L)
    {
        LuaScriptMgr.Push(L, true);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_UseLpeg(IntPtr L)
    {
        LuaScriptMgr.Push(L, false);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_UsePbc(IntPtr L)
    {
        LuaScriptMgr.Push(L, false);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_UsePbLua(IntPtr L)
    {
        LuaScriptMgr.Push(L, false);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_UserId(IntPtr L)
    {
        LuaScriptMgr.Push(L, AppConst.UserId);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_UseSproto(IntPtr L)
    {
        LuaScriptMgr.Push(L, false);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_WebUrl(IntPtr L)
    {
        LuaScriptMgr.Push(L, "http://localhost:6688/");
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_AppConstWrap._Createcom_tencent_pandora_AppConst)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_AppConstWrap.GetClassType)) };
        LuaField[] fields = new LuaField[] { 
            new LuaField("DebugMode", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_DebugMode), null), new LuaField("ExampleMode", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_ExampleMode), null), new LuaField("UpdateMode", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_UpdateMode), null), new LuaField("AutoWrapMode", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_AutoWrapMode), null), new LuaField("UsePbc", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_UsePbc), null), new LuaField("UseLpeg", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_UseLpeg), null), new LuaField("UsePbLua", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_UsePbLua), null), new LuaField("UseCJson", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_UseCJson), null), new LuaField("UseSproto", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_UseSproto), null), new LuaField("LuaEncode", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_LuaEncode), null), new LuaField("TimerInterval", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_TimerInterval), null), new LuaField("GameFrameRate", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_GameFrameRate), null), new LuaField("AppName", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_AppName), null), new LuaField("AppPrefix", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_AppPrefix), null), new LuaField("WebUrl", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_WebUrl), null), new LuaField("UserId", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_UserId), new LuaCSFunction(com_tencent_pandora_AppConstWrap.set_UserId)), 
            new LuaField("SocketPort", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_SocketPort), new LuaCSFunction(com_tencent_pandora_AppConstWrap.set_SocketPort)), new LuaField("SocketAddress", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_SocketAddress), new LuaCSFunction(com_tencent_pandora_AppConstWrap.set_SocketAddress)), new LuaField("LuaBasePath", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_LuaBasePath), null), new LuaField("LuaWrapPath", new LuaCSFunction(com_tencent_pandora_AppConstWrap.get_LuaWrapPath), null)
         };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.AppConst", typeof(AppConst), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_SocketAddress(IntPtr L)
    {
        AppConst.SocketAddress = LuaScriptMgr.GetString(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_SocketPort(IntPtr L)
    {
        AppConst.SocketPort = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_UserId(IntPtr L)
    {
        AppConst.UserId = LuaScriptMgr.GetString(L, 3);
        return 0;
    }
}

