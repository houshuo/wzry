using Assets.Scripts.UI;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class NetworkAccelerator
{
    private static string key = string.Empty;
    private static int KEY_GET_ACCEL_EFFECT = 0x6b;
    private static int KEY_GET_ACCEL_STAT = 0x66;
    private static int KEY_GET_NETDELAY = 100;
    private static int KEY_LOG_LEVLE = 0x134;
    public static int LOG_LEVEL_DEBUG = 1;
    public static int LOG_LEVEL_ERROR = 4;
    public static int LOG_LEVEL_FATAL = 5;
    public static int LOG_LEVEL_INFO = 2;
    public static int LOG_LEVEL_WARNING = 3;
    public static string PLAYER_PREF_AUTO_NET_ACC = "AUTO_NET_ACC";
    public static string PLAYER_PREF_NET_ACC = "NET_ACC";
    private static bool s_enabled;
    private static bool s_inited;
    private static bool s_started;

    public static void ChangeLogLevel(int level)
    {
        if (s_inited)
        {
            long num = Mathf.Clamp(level, 1, 5);
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                object[] args = new object[] { KEY_LOG_LEVLE, num };
                class2.CallStatic("setLong", args);
            }
        }
    }

    public static void ClearUDPCache()
    {
        if (s_inited)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                class2.CallStatic("clearUDPCache", new object[0]);
            }
        }
    }

    public static bool getAccelRecommendation()
    {
        Debug.Log("getAccelRecommendation :");
        if ((!s_inited || !enabled) || started)
        {
            return false;
        }
        AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
        bool flag = false;
        if (class2 != null)
        {
            flag = class2.CallStatic<int>("getAccelRecommendation", new object[0]) == 1;
            Debug.Log("getAccelRecommendation :" + flag);
        }
        return flag;
    }

    public static long GetDelay()
    {
        if (!s_started)
        {
            return -1L;
        }
        AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
        long num = -1L;
        if (class2 != null)
        {
            object[] args = new object[] { KEY_GET_NETDELAY };
            num = class2.CallStatic<long>("getLong", args);
        }
        return num;
    }

    public static string GetEffect()
    {
        if (!s_started)
        {
            return null;
        }
        AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
        string str = null;
        if (class2 != null)
        {
            object[] args = new object[] { KEY_GET_ACCEL_EFFECT };
            str = class2.CallStatic<string>("getString", args);
        }
        return str;
    }

    public static int GetNetType()
    {
        int num = -1;
        if (s_inited)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                num = class2.CallStatic<int>("getCurrentConnectionType", new object[0]);
            }
        }
        return num;
    }

    public static void Init()
    {
        <Init>c__AnonStorey5C storeyc = new <Init>c__AnonStorey5C();
        Debug.Log("Begin Network Acc");
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.NetworkAccelerator_TurnOn, new CUIEventManager.OnUIEventHandler(NetworkAccelerator.OnEventTurnOn));
        CResource resource = Singleton<CResourceManager>.GetInstance().GetResource("Config/NetAcc.txt", typeof(TextAsset), enResourceType.Numeric, false, true);
        string str = string.Empty;
        if (resource != null)
        {
            CBinaryObject content = resource.m_content as CBinaryObject;
            if ((null != content) && (content.m_data != null))
            {
                string str2 = Encoding.UTF8.GetString(content.m_data);
                if (str2 != null)
                {
                    str = str2.Trim();
                }
            }
        }
        if (string.IsNullOrEmpty(str))
        {
            Debug.LogError("Can not find NetAcc.txt");
        }
        else
        {
            char[] separator = new char[] { ' ' };
            string[] strArray = str.Split(separator);
            if (strArray.Length != 2)
            {
                Debug.LogError("NetAcc.txt format Error:" + str);
            }
            else if ("true" != strArray[1].ToLower())
            {
                Debug.Log("NetAcc closed");
            }
            else
            {
                s_enabled = true;
                key = strArray[0];
                Debug.Log("NetAcc key:" + key);
                storeyc.GMContext = null;
                using (AndroidJavaClass class2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    storeyc.GMContext = class2.GetStatic<AndroidJavaObject>("currentActivity");
                }
                storeyc.GMClass = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
                if (storeyc.GMClass != null)
                {
                    object[] args = new object[] { new AndroidJavaRunnable(storeyc.<>m__2F) };
                    storeyc.GMContext.Call("runOnUiThread", args);
                }
            }
        }
    }

    public static bool isAccerating()
    {
        if (!s_started)
        {
            return false;
        }
        AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
        bool flag = false;
        if (class2 != null)
        {
            flag = class2.CallStatic<bool>("isUDPProxy", new object[0]);
        }
        return flag;
    }

    public static bool IsAutoNetAccConfigOpen()
    {
        return (PlayerPrefs.GetInt(PLAYER_PREF_AUTO_NET_ACC, 0) > 0);
    }

    public static bool IsNetAccConfigOpen()
    {
        return (PlayerPrefs.GetInt(PLAYER_PREF_NET_ACC, 0) > 0);
    }

    private static void OnEventTurnOn(CUIEvent uiEvent)
    {
        SetNetAccConfig(true);
    }

    public static void OnNetDelay(int millis)
    {
        if (s_inited && !started)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                object[] args = new object[] { millis };
                class2.CallStatic("onNetDelay", args);
            }
        }
    }

    public static void SetAutoNetAccConfig(bool open)
    {
        PlayerPrefs.SetInt(PLAYER_PREF_AUTO_NET_ACC, !open ? 0 : 1);
        PlayerPrefs.Save();
    }

    public static void SetEchoPort(int port)
    {
        Debug.Log("Set UD Echo Port to :" + port);
        if (s_inited)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                object[] args = new object[] { port };
                class2.CallStatic("setUdpEchoPort", args);
            }
            Debug.Log("Set UD Echo Port Success!");
        }
    }

    public static void SetNetAccConfig(bool open)
    {
        if (open)
        {
            Start();
        }
        else
        {
            Stop();
        }
        PlayerPrefs.SetInt(PLAYER_PREF_NET_ACC, !s_started ? 0 : 1);
        PlayerPrefs.Save();
    }

    public static void setRecommendationGameIP(string ip, int port)
    {
        Debug.Log(string.Concat(new object[] { "setRecommendationGameIP :", ip, ", port :", port }));
        if (s_inited)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            if (class2 != null)
            {
                object[] args = new object[] { ip, port };
                class2.CallStatic("setRecommendationGameIP", args);
            }
            Debug.Log("Set setRecommendationGameIP Success!");
        }
    }

    private static bool Start()
    {
        if (s_inited)
        {
            if (!enabled)
            {
                return false;
            }
            if (s_started)
            {
                return s_started;
            }
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            bool flag = false;
            if (class2 != null)
            {
                object[] args = new object[] { 0 };
                flag = class2.CallStatic<bool>("start", args);
            }
            if (flag)
            {
                Debug.Log("Start GameMaster Success!");
                s_started = true;
            }
            else
            {
                Debug.LogError("Start GameMaster Fail!");
            }
        }
        return s_started;
    }

    private static bool Stop()
    {
        if (s_inited)
        {
            if (!s_started)
            {
                return s_started;
            }
            AndroidJavaClass class2 = new AndroidJavaClass("com.subao.gamemaster.GameMaster");
            bool flag = false;
            if (class2 != null)
            {
                flag = class2.CallStatic<bool>("stop", new object[0]);
            }
            if (flag)
            {
                Debug.Log("Stop GameMaster Success!");
                ClearUDPCache();
                s_started = false;
            }
            else
            {
                Debug.LogError("Stop GameMaster Fail!");
            }
        }
        return s_started;
    }

    public static bool enabled
    {
        get
        {
            return (s_enabled && MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.useNetAcc);
        }
    }

    public static bool started
    {
        get
        {
            return s_started;
        }
    }

    [CompilerGenerated]
    private sealed class <Init>c__AnonStorey5C
    {
        internal AndroidJavaClass GMClass;
        internal AndroidJavaObject GMContext;

        internal void <>m__2F()
        {
            object[] args = new object[] { this.GMContext, 1, NetworkAccelerator.key, "KingsGlory", "libapollo.so", 0x32c9 };
            int num = this.GMClass.CallStatic<int>("init", args);
            if (num >= 0)
            {
                Debug.Log("Initialize GameMaster Success!");
                NetworkAccelerator.s_inited = true;
                NetworkAccelerator.ChangeLogLevel(NetworkAccelerator.LOG_LEVEL_ERROR);
            }
            else
            {
                Debug.LogError("Initialize GameMaster Fail!, ret:" + num);
            }
        }
    }
}

