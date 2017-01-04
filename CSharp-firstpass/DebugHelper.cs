using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    private static string CachedLogRootPath;
    public SLogTypeDef cfgMode = SLogTypeDef.LogType_System;
    private static DebugHelper instance = null;
    private static SLogTypeDef logMode = SLogTypeDef.LogType_Custom;
    private static SLogObj[] s_logers = new SLogObj[8];

    [Conditional("UNITY_ANDROID"), Conditional("FORCE_LOG"), Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE_WIN")]
    public static void Assert(bool InCondition)
    {
        Assert(InCondition, null, null);
    }

    [Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_ANDROID"), Conditional("UNITY_STANDALONE_WIN")]
    public static void Assert(bool InCondition, string InFormat)
    {
        Assert(InCondition, InFormat, null);
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_ANDROID"), Conditional("FORCE_LOG"), Conditional("UNITY_EDITOR")]
    public static void Assert(bool InCondition, string InFormat, params object[] InParameters)
    {
        if (!InCondition)
        {
            try
            {
                string str = null;
                if (!string.IsNullOrEmpty(InFormat))
                {
                    try
                    {
                        if (InParameters != null)
                        {
                            str = string.Format(InFormat, InParameters);
                        }
                        else
                        {
                            str = InFormat;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    str = string.Format(" no assert detail, stacktrace is :{0}", Environment.StackTrace);
                }
                if (str != null)
                {
                    CustomLog("Assert failed! " + str);
                }
                else
                {
                    Debug.LogWarning("Assert failed!");
                }
            }
            catch (Exception)
            {
            }
        }
    }

    private void Awake()
    {
        Assert(instance == null);
        instance = this;
        logMode = this.cfgMode;
        int num = 8;
        for (int i = 0; i < num; i++)
        {
            s_logers[i] = new SLogObj();
        }
    }

    public static void BeginLogs()
    {
        CloseLogs();
        string logRootPath = DebugHelper.logRootPath;
        string str2 = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        OpenLoger(SLogCategory.Normal, string.Format("{0}/{1}_normal.log", logRootPath, str2));
        OpenLoger(SLogCategory.Skill, string.Format("{0}/{1}_skill.log", logRootPath, str2));
        OpenLoger(SLogCategory.Misc, string.Format("{0}/{1}_misc.log", logRootPath, str2));
        OpenLoger(SLogCategory.Msg, string.Format("{0}/{1}_msg.log", logRootPath, str2));
        OpenLoger(SLogCategory.Actor, string.Format("{0}/{1}_actor.log", logRootPath, str2));
        OpenLoger(SLogCategory.Rvo, string.Format("{0}/{1}_rvo.log", logRootPath, str2));
        OpenLoger(SLogCategory.Fow, string.Format("{0}/{1}_fow.log", logRootPath, str2));
    }

    [Conditional("UNITY_EDITOR")]
    public static void ClearConsole()
    {
    }

    public static void ClearLogs(int passedMinutes = 60)
    {
        DateTime now = DateTime.Now;
        DirectoryInfo info = new DirectoryInfo(logRootPath);
        if (info.Exists)
        {
            string[] strArray = Directory.GetFiles(info.FullName, "*.log", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < strArray.Length; i++)
            {
                try
                {
                    string fileName = strArray[i];
                    FileInfo info2 = new FileInfo(fileName);
                    if (info2.Exists)
                    {
                        TimeSpan span = (TimeSpan) (now - info2.CreationTime);
                        if (span.TotalMinutes > passedMinutes)
                        {
                            File.Delete(fileName);
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }

    public static void CloseLoger(SLogCategory logType)
    {
        int index = (int) logType;
        s_logers[index].Flush();
        s_logers[index].Close();
        s_logers[index].TargetPath = null;
    }

    public static void CloseLogs()
    {
        CloseLoger(SLogCategory.Normal);
        CloseLoger(SLogCategory.Skill);
        CloseLoger(SLogCategory.Misc);
        CloseLoger(SLogCategory.Msg);
        CloseLoger(SLogCategory.Actor);
        CloseLoger(SLogCategory.Rvo);
        CloseLoger(SLogCategory.Fow);
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    public static void ConsoleLog(string logmsg)
    {
        Debug.Log(logmsg);
    }

    [Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN")]
    public static void ConsoleLogError(string logmsg)
    {
        Debug.LogError(logmsg);
    }

    [Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN")]
    public static void ConsoleLogWarning(string logmsg)
    {
        Debug.LogWarning(logmsg);
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("UNITY_ANDROID"), Conditional("FORCE_LOG")]
    public static void CustomLog(string str)
    {
        CustomLog(str, null);
    }

    [Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_ANDROID")]
    public static void CustomLog(string str, params object[] InParameters)
    {
        try
        {
            string className = "com.tencent.tmgp.sgame.SGameUtility";
            if (InParameters != null)
            {
                str = string.Format(str, InParameters);
            }
            str = DateTime.Now.ToString("yyyyMMdd_HHmmss ") + str;
            Debug.Log(str);
            AndroidJavaClass class2 = new AndroidJavaClass(className);
            object[] args = new object[] { str };
            class2.CallStatic("dtLog", args);
            class2.Dispose();
        }
        catch (Exception)
        {
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG")]
    public static void EditorAssert(bool InCondition, string InFormat = null, params object[] InParameters)
    {
        Assert(InCondition, InFormat, InParameters);
    }

    public static SLogObj GetLoger(SLogCategory logType)
    {
        return s_logers[(int) logType];
    }

    public static string GetLogerPath(SLogCategory logType)
    {
        return s_logers[(int) logType].LastTargetPath;
    }

    [Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR")]
    public static void Log(string logmsg)
    {
        if (logMode == SLogTypeDef.LogType_Custom)
        {
        }
    }

    [Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN")]
    public static void LogError(string errmsg)
    {
        Debug.LogError(errmsg);
    }

    [Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN")]
    public static void LogInternal(SLogCategory logType, string logmsg)
    {
        s_logers[(int) logType].Log(logmsg);
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG"), Conditional("UNITY_EDITOR")]
    public static void LogMisc(string logmsg)
    {
        if ((logMode != SLogTypeDef.LogType_System) && (logMode == SLogTypeDef.LogType_Custom))
        {
        }
    }

    [Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR")]
    public static void LogSkill(string logmsg)
    {
        if ((logMode != SLogTypeDef.LogType_System) && (logMode == SLogTypeDef.LogType_Custom))
        {
        }
    }

    [Conditional("FORCE_LOG"), Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR")]
    public static void LogWarning(string warmsg)
    {
        Debug.LogWarning(warmsg);
    }

    protected void OnDestroy()
    {
        int num = 8;
        for (int i = 0; i < num; i++)
        {
            s_logers[i].Flush();
            s_logers[i].Close();
        }
        Singleton<BackgroundWorker>.DestroyInstance();
    }

    public static void OpenLoger(SLogCategory logType, string logFile)
    {
        int index = (int) logType;
        s_logers[index].Flush();
        s_logers[index].Close();
        s_logers[index].TargetPath = logFile;
    }

    public static string logRootPath
    {
        get
        {
            if (CachedLogRootPath == null)
            {
                string path = string.Format("{0}/Replay/", Application.persistentDataPath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                CachedLogRootPath = path;
            }
            return CachedLogRootPath;
        }
    }
}

