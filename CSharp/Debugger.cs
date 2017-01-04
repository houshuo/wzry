using com.tencent.pandora;
using System;

public class Debugger : Logger
{
    public static void Log(string str, params object[] args)
    {
        str = string.Format(str, args);
        Logger.d(str);
    }

    public static void LogError(string str, params object[] args)
    {
        str = string.Format(str, args);
        Logger.d(str);
    }

    public static void LogWarning(string str, params object[] args)
    {
        str = string.Format(str, args);
        Logger.d(str);
    }
}

