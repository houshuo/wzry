using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public sealed class BuglyAgent
{
    private static bool _autoQuitApplicationAfterReport;
    private static LogSeverity _autoReportLogLevel = LogSeverity.LogError;
    private static string _configChannel;
    private static long _configDelayTime;
    private static string _configUser;
    private static string _configVersion;
    private static bool _debugMode;
    private static bool _isInitialized;
    private static readonly string _pluginVersion = "1.2.5";
    private static bool _uncaughtAutoReportOnce;
    private static AndroidJavaObject _unityAgent;
    private static readonly string CLASS_UNITYAGENT = "com.tencent.bugly.unity.UnityAgent";
    private static readonly int EXCEPTION_TYPE_CAUGHT = 2;
    private static readonly int EXCEPTION_TYPE_UNCAUGHT = 1;

    private static  event LogCallbackDelegate LogCallbackEventHandler;

    private static void _HandleException(Exception e, string message, bool uncaught)
    {
        if ((e != null) && IsInitialized)
        {
            string name = e.GetType().Name;
            string str2 = e.Message;
            if (!string.IsNullOrEmpty(message))
            {
                str2 = string.Format("{0}{1}***{2}", str2, Environment.NewLine, message);
            }
            StringBuilder builder = new StringBuilder(string.Empty);
            StackTrace trace = new StackTrace(e, true);
            int frameCount = trace.FrameCount;
            for (int i = 0; i < frameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                builder.AppendFormat("{0}.{1}", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name);
                ParameterInfo[] parameters = frame.GetMethod().GetParameters();
                if ((parameters == null) || (parameters.Length == 0))
                {
                    builder.Append(" () ");
                }
                else
                {
                    builder.Append(" (");
                    int length = parameters.Length;
                    ParameterInfo info = null;
                    for (int j = 0; j < length; j++)
                    {
                        info = parameters[j];
                        builder.AppendFormat("{0} {1}", info.ParameterType.Name, info.Name);
                        if (j != (length - 1))
                        {
                            builder.Append(", ");
                        }
                    }
                    info = null;
                    builder.Append(") ");
                }
                string fileName = frame.GetFileName();
                if (!string.IsNullOrEmpty(fileName) && !fileName.ToLower().Equals("unknown"))
                {
                    fileName = fileName.Replace(@"\", "/");
                    int index = fileName.ToLower().IndexOf("/assets/");
                    if (index < 0)
                    {
                        index = fileName.ToLower().IndexOf("assets/");
                    }
                    if (index > 0)
                    {
                        fileName = fileName.Substring(index);
                    }
                    builder.AppendFormat("(at {0}:{1})", fileName, frame.GetFileLineNumber());
                }
                builder.AppendLine();
            }
            _reportException(uncaught, name, str2, builder.ToString());
        }
    }

    private static void _HandleException(LogSeverity logLevel, string name, string message, string stackTrace, bool uncaught)
    {
        if (!IsInitialized)
        {
            PrintLog(LogSeverity.Log, "It has not been initialized.", new object[0]);
        }
        else if ((logLevel == LogSeverity.Log) || (uncaught && (logLevel < _autoReportLogLevel)))
        {
            object[] args = new object[] { logLevel.ToString() };
            PrintLog(LogSeverity.Log, "Not report exception for level {0}", args);
        }
        else
        {
            string str = null;
            string str2 = null;
            if (!string.IsNullOrEmpty(message))
            {
                try
                {
                    if ((logLevel == LogSeverity.LogException) && message.Contains("Exception"))
                    {
                        Match match = new Regex(@"^(?<errorType>\S+):\s*(?<errorMessage>.*)").Match(message);
                        if (match.Success)
                        {
                            str = match.Groups["errorType"].Value;
                            str2 = match.Groups["errorMessage"].Value.Trim();
                        }
                    }
                }
                catch
                {
                }
                if (string.IsNullOrEmpty(str2))
                {
                    str2 = message;
                }
            }
            if (string.IsNullOrEmpty(name))
            {
                if (string.IsNullOrEmpty(str))
                {
                    str = string.Format("Unity{0}", logLevel.ToString());
                }
            }
            else
            {
                str = name;
            }
            _reportException(uncaught, str, str2, stackTrace);
        }
    }

    private static void _OnLogCallbackHandler(string condition, string stackTrace, LogType type)
    {
        if (LogCallbackEventHandler != null)
        {
            LogCallbackEventHandler(condition, stackTrace, type);
        }
        if ((IsInitialized && (string.IsNullOrEmpty(condition) || !condition.Contains("[BuglyAgent] <Log>"))) && !_uncaughtAutoReportOnce)
        {
            LogSeverity log = LogSeverity.Log;
            switch (type)
            {
                case LogType.Error:
                    log = LogSeverity.LogError;
                    break;

                case LogType.Assert:
                    log = LogSeverity.LogAssert;
                    break;

                case LogType.Warning:
                    log = LogSeverity.LogWarning;
                    break;

                case LogType.Log:
                    log = LogSeverity.LogDebug;
                    break;

                case LogType.Exception:
                    log = LogSeverity.LogException;
                    break;
            }
            if (log != LogSeverity.Log)
            {
                _HandleException(log, null, condition, stackTrace, true);
            }
        }
    }

    private static void _OnUncaughtExceptionHandler(object sender, UnhandledExceptionEventArgs args)
    {
        if ((args != null) && (args.ExceptionObject != null))
        {
            try
            {
                if (args.ExceptionObject.GetType() != typeof(Exception))
                {
                    return;
                }
            }
            catch
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("BuglyAgent: Failed to report uncaught exception");
                }
                return;
            }
            if (IsInitialized && !_uncaughtAutoReportOnce)
            {
                _HandleException((Exception) args.ExceptionObject, null, true);
            }
        }
    }

    private static void _RegisterExceptionHandler()
    {
        try
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(BuglyAgent._OnUncaughtExceptionHandler);
            Application.RegisterLogCallback(new Application.LogCallback(BuglyAgent._OnLogCallbackHandler));
            _isInitialized = true;
            PrintLog(LogSeverity.Log, "Register the log callback", new object[0]);
        }
        catch
        {
        }
    }

    private static void _reportException(bool uncaught, string name, string reason, string stackTrace)
    {
        if (!string.IsNullOrEmpty(name))
        {
            if (string.IsNullOrEmpty(stackTrace))
            {
                stackTrace = StackTraceUtility.ExtractStackTrace();
            }
            if (string.IsNullOrEmpty(stackTrace))
            {
                stackTrace = "Empty";
            }
            else
            {
                try
                {
                    char[] separator = new char[] { '\n' };
                    string[] strArray = stackTrace.Split(separator);
                    if ((strArray != null) && (strArray.Length > 0))
                    {
                        StringBuilder builder = new StringBuilder();
                        string str = null;
                        int length = strArray.Length;
                        for (int i = 0; i < length; i++)
                        {
                            str = strArray[i];
                            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(str.Trim()))
                            {
                                str = str.Trim();
                                if ((!str.StartsWith("System.Collections.Generic.") && !str.StartsWith("ShimEnumerator")) && (!str.Contains("Bugly") && !str.Contains("..ctor")))
                                {
                                    str = str.Replace(":", ".");
                                    int index = str.ToLower().IndexOf("(at");
                                    int startIndex = str.ToLower().IndexOf("/assets/");
                                    if ((index > 0) && (startIndex > 0))
                                    {
                                        builder.AppendFormat("{0}(at {1}", str.Substring(0, index), str.Substring(startIndex));
                                    }
                                    else
                                    {
                                        builder.Append(str);
                                    }
                                    builder.AppendLine();
                                }
                            }
                        }
                        stackTrace = builder.ToString();
                    }
                }
                catch
                {
                }
            }
            object[] args = new object[] { name, reason, stackTrace };
            PrintLog(LogSeverity.Log, "\n*********\n{0} {1}\n{2}\n*********", args);
            _uncaughtAutoReportOnce = uncaught && _autoQuitApplicationAfterReport;
            ReportException(!uncaught ? EXCEPTION_TYPE_CAUGHT : EXCEPTION_TYPE_UNCAUGHT, name, reason, stackTrace, uncaught && _autoQuitApplicationAfterReport);
        }
    }

    private static void _UnregisterExceptionHandler()
    {
        try
        {
            AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(BuglyAgent._OnUncaughtExceptionHandler);
            Application.RegisterLogCallback(null);
            PrintLog(LogSeverity.Log, "Unregister the log callback", new object[0]);
        }
        catch
        {
        }
    }

    private static void AddKeyAndValueInScene(string key, string value)
    {
        try
        {
            object[] args = new object[] { key, value };
            UnityAgent.Call("addSceneValue", args);
        }
        catch
        {
        }
    }

    public static void AddSceneData(string key, string value)
    {
        if (IsInitialized)
        {
            object[] args = new object[] { key, value };
            DebugLog(null, "Add scene data: [{0}, {1}]", args);
            AddKeyAndValueInScene(key, value);
        }
    }

    public static void ConfigAutoQuitApplication(bool autoQuit)
    {
        _autoQuitApplicationAfterReport = autoQuit;
    }

    public static void ConfigAutoReportLogLevel(LogSeverity level)
    {
        _autoReportLogLevel = level;
    }

    public static void ConfigDebugMode(bool enable)
    {
        EnableDebugMode(enable);
    }

    public static void ConfigDefault(string channel, string version, string user, long delay)
    {
        ConfigDefaultBeforeInit(channel, version, user, delay);
    }

    private static void ConfigDefaultBeforeInit(string channel, string version, string user, long delay)
    {
        _configChannel = channel;
        _configVersion = version;
        _configUser = user;
        _configDelayTime = delay;
    }

    public static void DebugLog(string tag, string format, params object[] args)
    {
        if (!string.IsNullOrEmpty(format))
        {
            Console.WriteLine("[BuglyAgent] <{0}> - {1}", tag, string.Format(format, args));
        }
    }

    private static void EnableDebugMode(bool enable)
    {
        _debugMode = enable;
        try
        {
            object[] args = new object[] { enable };
            UnityAgent.Call("setLogEnable", args);
        }
        catch
        {
        }
    }

    public static void EnableExceptionHandler()
    {
        if (IsInitialized)
        {
            DebugLog(null, "BuglyAgent has already been initialized.", new object[0]);
        }
        else
        {
            PrintLog(LogSeverity.Log, "Only enable the exception handler, please make sure you has initialized the sdk in the native code in associated Android or iOS project.", new object[0]);
            _RegisterExceptionHandler();
        }
    }

    private static void InitUnityAgent(string appId)
    {
        if (!IsInitialized)
        {
            try
            {
                object[] args = new object[] { appId, _configChannel, _configVersion, _configUser, _configDelayTime };
                UnityAgent.Call("initWithConfiguration", args);
                _isInitialized = true;
            }
            catch
            {
            }
        }
    }

    public static void InitWithAppId(string appId)
    {
        if (IsInitialized)
        {
            DebugLog(null, "BuglyAgent has already been initialized.", new object[0]);
        }
        else if (!string.IsNullOrEmpty(appId))
        {
            InitUnityAgent(appId);
            _RegisterExceptionHandler();
        }
    }

    private static void LogToConsole(LogSeverity level, string message)
    {
        if ((_debugMode || (level == LogSeverity.Log)) || (level >= LogSeverity.LogInfo))
        {
            try
            {
                object[] args = new object[] { string.Format("[BuglyAgent] <{0}> - {1}", level.ToString(), message) };
                UnityAgent.Call("printLog", args);
            }
            catch
            {
            }
        }
    }

    public static void PrintLog(LogSeverity level, string format, params object[] args)
    {
        if (!string.IsNullOrEmpty(format))
        {
            LogToConsole(level, string.Format(format, args));
        }
    }

    public static void RegisterLogCallback(LogCallbackDelegate handler)
    {
        if (handler != null)
        {
            DebugLog(null, "Add log callback handler", new object[0]);
            LogCallbackEventHandler = (LogCallbackDelegate) Delegate.Combine(LogCallbackEventHandler, handler);
        }
    }

    private static void ReportAttachmentWithException(string log)
    {
    }

    public static void ReportException(Exception e, string message)
    {
        if (IsInitialized)
        {
            object[] args = new object[] { message, e };
            DebugLog(null, "Report exception: {0}\n------------\n{1}\n------------", args);
            _HandleException(e, message, false);
        }
    }

    public static void ReportException(string name, string message, string stackTrace)
    {
        if (IsInitialized)
        {
            object[] args = new object[] { name, message, stackTrace };
            DebugLog(null, "Report exception: {0} {1} \n{2}", args);
            _HandleException(LogSeverity.LogException, name, message, stackTrace, false);
        }
    }

    private static void ReportException(int type, string name, string reason, string stackTrace, bool quitProgram)
    {
        try
        {
            object[] args = new object[] { name, reason, stackTrace, quitProgram };
            UnityAgent.Call("traceException", args);
        }
        catch
        {
        }
    }

    private static void ReportExtrasWithException(string key, string value)
    {
    }

    private static void SetCurrentScene(int sceneId)
    {
        try
        {
            object[] args = new object[] { sceneId };
            UnityAgent.Call("setScene", args);
        }
        catch
        {
        }
    }

    public static void SetScene(int sceneId)
    {
        if (IsInitialized)
        {
            object[] args = new object[] { sceneId };
            DebugLog(null, "Set scene: {0}", args);
            SetCurrentScene(sceneId);
        }
    }

    public static void SetUserId(string userId)
    {
        if (IsInitialized)
        {
            object[] args = new object[] { userId };
            DebugLog(null, "Set user id: {0}", args);
            SetUserInfo(userId);
        }
    }

    private static void SetUserInfo(string userInfo)
    {
        try
        {
            object[] args = new object[] { userInfo };
            UnityAgent.Call("setUserId", args);
        }
        catch
        {
        }
    }

    public static void UnregisterLogCallback(LogCallbackDelegate handler)
    {
        if (handler != null)
        {
            DebugLog(null, "Remove log callback handler", new object[0]);
            LogCallbackEventHandler = (LogCallbackDelegate) Delegate.Remove(LogCallbackEventHandler, handler);
        }
    }

    public static bool AutoQuitApplicationAfterReport
    {
        get
        {
            return _autoQuitApplicationAfterReport;
        }
    }

    public static bool IsInitialized
    {
        get
        {
            return _isInitialized;
        }
    }

    public static string PluginVersion
    {
        get
        {
            return _pluginVersion;
        }
    }

    public static AndroidJavaObject UnityAgent
    {
        get
        {
            if (_unityAgent == null)
            {
                using (AndroidJavaClass class2 = new AndroidJavaClass(CLASS_UNITYAGENT))
                {
                    _unityAgent = class2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
                }
            }
            return _unityAgent;
        }
    }

    public delegate void LogCallbackDelegate(string condition, string stackTrace, LogType type);
}

