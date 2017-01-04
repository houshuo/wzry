using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameJoySDK : MonoBehaviour
{
    [CompilerGenerated]
    private static Func<KeyValuePair<string, string>, string> <>f__am$cache8;
    public bool enableOnStart = true;
    public const string GAMEJOY_UNITY3D_CS_VERSION = "2015-09-22-0002-Camera-Render";
    private static AndroidJavaClass mQMiObj;
    private static RECORER_STATUS mRecorderStatus = RECORER_STATUS.RS_UNUSED;
    private static int mSDKVersion = -1;
    private static AndroidJavaObject playerActivityContext;
    public static float recorderX = -1f;
    public static float recorderY = -1f;
    private static GameJoySDK singletonInstance;

    private void Awake()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            this.InitializeRenderCamera("Pre");
        }
    }

    public int BeginDraw()
    {
        if (mRecorderStatus != RECORER_STATUS.RS_STARTED)
        {
            return 0;
        }
        int num = 0;
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            num = mQMiObj.CallStatic<int>("beginDraw", new object[0]);
        }
        return num;
    }

    public int CallMethod(int nMethodID, int nParam1, int nParam2, int nParam3, int nParam4, int nParam5)
    {
        int num = 0;
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            object[] args = new object[] { nMethodID, nParam1, nParam2, nParam3, nParam4, nParam5 };
            num = mQMiObj.CallStatic<int>("callMethod", args);
        }
        return num;
    }

    public static bool checkFloatWindowPermission()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            if (playerActivityContext == null)
            {
                playerActivityContext = getActivityContext();
            }
            if (playerActivityContext != null)
            {
                object[] args = new object[] { playerActivityContext };
                return mQMiObj.CallStatic<bool>("checkFloatWindowPermission", args);
            }
        }
        return false;
    }

    public static void CheckSupportRecord()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            playerActivityContext = getActivityContext();
            if (playerActivityContext != null)
            {
                RecordEnableCallback callback = new RecordEnableCallback();
                object[] args = new object[] { playerActivityContext, callback };
                mQMiObj.CallStatic("isSupportRecord", args);
            }
        }
    }

    public void dismissGameJoyRecorder()
    {
        this.StopQMi();
    }

    public int EndDraw()
    {
        if (mRecorderStatus != RECORER_STATUS.RS_STARTED)
        {
            return 0;
        }
        int num = 0;
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            num = mQMiObj.CallStatic<int>("endDraw", new object[0]);
        }
        return num;
    }

    public void endMomentRecording()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            mQMiObj.CallStatic("endMomentRecording", new object[0]);
        }
    }

    public void generateMomentVideo(List<TimeStamp> timeStampList, string defaultGameTag, Dictionary<string, string> extraInfo)
    {
        if ((timeStampList != null) && (timeStampList.Count > 0))
        {
            this.showUploadShareDialog();
        }
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            string str = null;
            if (extraInfo != null)
            {
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = pair => string.Format("{0}'{1}", pair.Key.ToString(), pair.Value.ToString());
                }
                str = string.Join("^", extraInfo.Select<KeyValuePair<string, string>, string>(<>f__am$cache8).ToArray<string>());
            }
            long[] numArray = null;
            long[] numArray2 = null;
            if (timeStampList != null)
            {
                int count = timeStampList.Count;
                if (count != 0)
                {
                    TimeStamp[] stampArray = timeStampList.ToArray();
                    numArray = new long[count];
                    numArray2 = new long[count];
                    for (int i = 0; i < count; i++)
                    {
                        numArray[i] = (long) stampArray[i].startTime;
                        numArray2[i] = (long) stampArray[i].endTime;
                    }
                    for (int j = 0; j < count; j++)
                    {
                    }
                }
            }
            object[] args = new object[] { numArray, numArray2, defaultGameTag, str };
            mQMiObj.CallStatic("generateMomentVideo", args);
        }
    }

    private static AndroidJavaObject getActivityContext()
    {
        if (playerActivityContext == null)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (class2 == null)
            {
                return null;
            }
            playerActivityContext = class2.GetStatic<AndroidJavaObject>("currentActivity");
            if (playerActivityContext == null)
            {
                return null;
            }
        }
        return playerActivityContext;
    }

    public void GetGameEngineType()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            string str = "Unity3D_" + Application.unityVersion;
            object[] args = new object[] { str };
            mQMiObj.CallStatic("setGameEngineType", args);
        }
    }

    public static GameJoySDK getGameJoyInstance()
    {
        return instance;
    }

    public RECORER_STATUS GetRecorderStatus()
    {
        return mRecorderStatus;
    }

    public void HideQMi()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            playerActivityContext = getActivityContext();
            if (playerActivityContext != null)
            {
                object[] args = new object[] { playerActivityContext };
                mQMiObj.CallStatic("hideQMi", args);
            }
        }
    }

    private void InitializeRenderCamera(string type)
    {
        if (type.Equals("Pre") && (GameObject.Find("GameJoy" + type + "Camera") == null))
        {
            GameObject target = new GameObject();
            Camera camera = (Camera) target.AddComponent("Camera");
            camera.name = "GameJoy" + type + "Camera";
            camera.clearFlags = CameraClearFlags.Nothing;
            camera.cullingMask = 0;
            if (type.Equals("Pre"))
            {
                camera.depth = float.MinValue;
            }
            camera.gameObject.AddComponent("GameJoyAndroid" + type + "Render");
            UnityEngine.Object.DontDestroyOnLoad(target);
        }
    }

    public void initQMi()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            playerActivityContext = getActivityContext();
            if (playerActivityContext != null)
            {
            }
        }
    }

    public void InitRecordPlugin()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            playerActivityContext = getActivityContext();
            if (playerActivityContext != null)
            {
                object[] args = new object[] { playerActivityContext };
                mQMiObj.CallStatic("initQMi", args);
            }
        }
    }

    public int IsNewSDKVersion()
    {
        if ((mSDKVersion == -1) && (mRecorderStatus != RECORER_STATUS.RS_STARTED))
        {
            return -1;
        }
        if (mSDKVersion >= 0x1f)
        {
            return 1;
        }
        return 0;
    }

    public bool IsRecording()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            return mQMiObj.CallStatic<bool>("isRecording", new object[0]);
        }
        return false;
    }

    public bool isRecordingMoments()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            return mQMiObj.CallStatic<bool>("isRecordingMoment", new object[0]);
        }
        return false;
    }

    public bool IsShowed()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            return mQMiObj.CallStatic<bool>("isShowed", new object[0]);
        }
        return false;
    }

    public void IsStartRecordingSuccess(string success)
    {
        bool bResult = false;
        if ((success != null) && success.Equals("true"))
        {
            bResult = true;
        }
        GameJoy.OnStartMomentsRecording(bResult);
    }

    [Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE_WIN"), Conditional("FORCE_LOG")]
    public static void Log(string msglog)
    {
        Debug.Log(msglog);
    }

    private static AndroidJavaClass mQMiObjJavaClass()
    {
        if (mQMiObj == null)
        {
            mQMiObj = new AndroidJavaClass("com.tencent.qqgamemi.QmiSdkApi");
        }
        if (mQMiObj == null)
        {
        }
        return mQMiObj;
    }

    private void OnApplicationFocus()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            mQMiObj.CallStatic("onGameEnterForeground", new object[0]);
        }
    }

    private void OnApplicationPause()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            mQMiObj.CallStatic("onGameEnterBackground", new object[0]);
        }
    }

    private void OnApplicationQuit()
    {
        this.StopRecord();
        this.StopQMi();
    }

    public void ScollToSide()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            playerActivityContext = getActivityContext();
            if (playerActivityContext != null)
            {
                object[] args = new object[] { playerActivityContext };
                mQMiObj.CallStatic("scollToSide", args);
            }
        }
    }

    public void setDefaultStartPosition(float x, float y)
    {
        recorderX = x;
        recorderY = y;
    }

    public static GameJoySDK SetupGameJoySDK()
    {
        if (singletonInstance != null)
        {
            return singletonInstance;
        }
        GameObject target = new GameObject("GameJoySDK");
        UnityEngine.Object.DontDestroyOnLoad(target);
        singletonInstance = target.AddComponent<GameJoySDK>();
        return instance;
    }

    public void setUploadShareDialogDefaultPosition(float x, float y)
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            object[] args = new object[] { x, y };
            mQMiObj.CallStatic("setUploadShareDialogPosition", args);
        }
    }

    public void showGameJoyRecorder()
    {
        if (this.enableOnStart)
        {
            this.StartQMi(recorderX, recorderY);
        }
    }

    public void showUploadShareDialog()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            mQMiObj.CallStatic("showUploadShareVideoDialog", new object[0]);
        }
    }

    public void showVideoListDialog()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            if (playerActivityContext == null)
            {
                playerActivityContext = getActivityContext();
            }
            if (playerActivityContext != null)
            {
                object[] args = new object[] { playerActivityContext };
                mQMiObj.CallStatic("showVideoListDialog", args);
            }
        }
    }

    private void Start()
    {
        this.InitRecordPlugin();
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }

    public void startMomentRecording()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            if (playerActivityContext == null)
            {
                playerActivityContext = getActivityContext();
            }
            if (playerActivityContext != null)
            {
                string str = "Unity3D_" + Application.unityVersion;
                object[] args = new object[] { playerActivityContext, str };
                mQMiObj.CallStatic("startMomentRecording", args);
            }
        }
    }

    public void StartQMi(float x, float y)
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            if (playerActivityContext == null)
            {
                playerActivityContext = getActivityContext();
            }
            if (playerActivityContext != null)
            {
                string str = "Unity3D_" + Application.unityVersion;
                object[] args = new object[] { playerActivityContext, str, x, y };
                mQMiObj.CallStatic("showQMi", args);
            }
        }
    }

    public void StartRecord()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            mQMiObj.CallStatic("onStartRecordVideo", new object[0]);
            mSDKVersion = mQMiObj.CallStatic<int>("getSRPpluginVersionCode", new object[0]);
            mRecorderStatus = RECORER_STATUS.RS_STARTED;
        }
    }

    public void StopQMi()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            playerActivityContext = getActivityContext();
            if (playerActivityContext != null)
            {
                object[] args = new object[] { playerActivityContext };
                mQMiObj.CallStatic("stopQMi", args);
            }
        }
    }

    public void StopRecord()
    {
        if (mQMiObj == null)
        {
            mQMiObj = mQMiObjJavaClass();
        }
        if (mQMiObj != null)
        {
            mQMiObj.CallStatic("onStopRecordVideo", new object[0]);
            mRecorderStatus = RECORER_STATUS.RS_STOPED;
        }
    }

    public void Update()
    {
    }

    public static GameJoySDK instance
    {
        get
        {
            if (singletonInstance == null)
            {
            }
            return singletonInstance;
        }
    }

    public class RecordEnableCallback : AndroidJavaProxy
    {
        public RecordEnableCallback() : base("com.tencent.qqgamemi.RecordEnableCallback")
        {
        }

        private void onFail()
        {
            GameJoy.onUnSupport();
        }

        private void onSuccess()
        {
            GameJoy.onSupport();
        }
    }

    public enum RECORER_STATUS
    {
        RS_STARTED = 1,
        RS_STOPED = 0,
        RS_UNUSED = -1
    }
}

