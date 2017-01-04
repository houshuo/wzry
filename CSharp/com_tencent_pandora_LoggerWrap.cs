using com.tencent.pandora;
using System;

public class com_tencent_pandora_LoggerWrap
{
    private static System.Type classType = typeof(Logger);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_Logger(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            Logger o = new Logger();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.Logger.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int d(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Logger.d(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int e(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Logger.e(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_DEBUG(IntPtr L)
    {
        LuaScriptMgr.Push(L, Logger.DEBUG);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_ERROR(IntPtr L)
    {
        LuaScriptMgr.Push(L, Logger.ERROR);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_FATAL(IntPtr L)
    {
        LuaScriptMgr.Push(L, Logger.FATAL);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_INEED_LOG_TEXT(IntPtr L)
    {
        LuaScriptMgr.Push(L, Logger.INEED_LOG_TEXT);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_INFO(IntPtr L)
    {
        LuaScriptMgr.Push(L, Logger.INFO);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_LOG_LEVEL(IntPtr L)
    {
        LuaScriptMgr.Push(L, Logger.LOG_LEVEL);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_WARN(IntPtr L)
    {
        LuaScriptMgr.Push(L, Logger.WARN);
        return 1;
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
        LuaScriptMgr.CheckArgsCount(L, 3);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        int iErrorCode = (int) LuaScriptMgr.GetNumber(L, 2);
        string luaString = LuaScriptMgr.GetLuaString(L, 3);
        Logger.Log(number, iErrorCode, luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LogCommInfo(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Logger.LogCommInfo(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LogNet(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        Logger.LogNet(number, luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LogNetError(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        Logger.LogNetError(number, luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LogText(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        Logger.LogText(number, luaString);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Log", new LuaCSFunction(com_tencent_pandora_LoggerWrap.Log)), new LuaMethod("LogNetError", new LuaCSFunction(com_tencent_pandora_LoggerWrap.LogNetError)), new LuaMethod("LogCommInfo", new LuaCSFunction(com_tencent_pandora_LoggerWrap.LogCommInfo)), new LuaMethod("LogText", new LuaCSFunction(com_tencent_pandora_LoggerWrap.LogText)), new LuaMethod("LogNet", new LuaCSFunction(com_tencent_pandora_LoggerWrap.LogNet)), new LuaMethod("e", new LuaCSFunction(com_tencent_pandora_LoggerWrap.e)), new LuaMethod("d", new LuaCSFunction(com_tencent_pandora_LoggerWrap.d)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_LoggerWrap._Createcom_tencent_pandora_Logger)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_LoggerWrap.GetClassType)) };
        LuaField[] fields = new LuaField[] { new LuaField("INEED_LOG_TEXT", new LuaCSFunction(com_tencent_pandora_LoggerWrap.get_INEED_LOG_TEXT), new LuaCSFunction(com_tencent_pandora_LoggerWrap.set_INEED_LOG_TEXT)), new LuaField("LOG_LEVEL", new LuaCSFunction(com_tencent_pandora_LoggerWrap.get_LOG_LEVEL), new LuaCSFunction(com_tencent_pandora_LoggerWrap.set_LOG_LEVEL)), new LuaField("FATAL", new LuaCSFunction(com_tencent_pandora_LoggerWrap.get_FATAL), new LuaCSFunction(com_tencent_pandora_LoggerWrap.set_FATAL)), new LuaField("ERROR", new LuaCSFunction(com_tencent_pandora_LoggerWrap.get_ERROR), new LuaCSFunction(com_tencent_pandora_LoggerWrap.set_ERROR)), new LuaField("WARN", new LuaCSFunction(com_tencent_pandora_LoggerWrap.get_WARN), new LuaCSFunction(com_tencent_pandora_LoggerWrap.set_WARN)), new LuaField("INFO", new LuaCSFunction(com_tencent_pandora_LoggerWrap.get_INFO), new LuaCSFunction(com_tencent_pandora_LoggerWrap.set_INFO)), new LuaField("DEBUG", new LuaCSFunction(com_tencent_pandora_LoggerWrap.get_DEBUG), new LuaCSFunction(com_tencent_pandora_LoggerWrap.set_DEBUG)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.Logger", typeof(Logger), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_DEBUG(IntPtr L)
    {
        Logger.DEBUG = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_ERROR(IntPtr L)
    {
        Logger.ERROR = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_FATAL(IntPtr L)
    {
        Logger.FATAL = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_INEED_LOG_TEXT(IntPtr L)
    {
        Logger.INEED_LOG_TEXT = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_INFO(IntPtr L)
    {
        Logger.INFO = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_LOG_LEVEL(IntPtr L)
    {
        Logger.LOG_LEVEL = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_WARN(IntPtr L)
    {
        Logger.WARN = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }
}

