using System;
using System.Collections.Generic;
using UnityEngine;

public class GameJoy : Singleton<GameJoy>
{
    public static void CheckRecorderAvailability()
    {
        try
        {
            GameJoySDK.CheckSupportRecord();
        }
        catch (Exception exception)
        {
            object[] inParameters = new object[] { exception.Message, exception.StackTrace, exception.GetType().ToString() };
            DebugHelper.Assert(false, "CheckRecorderAvailability {2} {0} {1}", inParameters);
            onUnSupport();
        }
    }

    public static void checkSDKPermission()
    {
        OnFinishCheckSDKPremission(GameJoySDK.checkFloatWindowPermission());
    }

    public void EndMomentsRecording()
    {
        GameJoySDK.instance.endMomentRecording();
    }

    public void GenerateMomentsVideo(List<TimeStamp> timeStampList, string defaultGameTag, Dictionary<string, string> extraInfo)
    {
        GameJoySDK.instance.generateMomentVideo(timeStampList, defaultGameTag, extraInfo);
    }

    public void HideRecorder()
    {
        GameJoySDK.instance.dismissGameJoyRecorder();
    }

    public override void Init()
    {
        base.Init();
        if (GameJoySDK.SetupGameJoySDK() == null)
        {
            Debug.LogError("GameJoySDK Failed Setup Gamejoysdk!");
        }
    }

    public static void OnFinishCheckSDKPremission(bool bResult)
    {
        Singleton<EventRouter>.instance.BroadCastEvent<bool>(EventID.GAMEJOY_SDK_PERMISSION_CHECK_RESULT, bResult);
    }

    public static void OnStartMomentsRecording(bool bResult)
    {
        Singleton<EventRouter>.instance.BroadCastEvent<bool>(EventID.GAMEJOY_STARTRECORDING_RESULT, bResult);
    }

    public static void onSupport()
    {
        Singleton<EventRouter>.instance.BroadCastEvent<bool>(EventID.GAMEJOY_AVAILABILITY_CHECK_RESULT, true);
    }

    public static void onUnSupport()
    {
        Singleton<EventRouter>.instance.BroadCastEvent<bool>(EventID.GAMEJOY_AVAILABILITY_CHECK_RESULT, false);
    }

    public void SetDefaultStartPosition(float x, float y)
    {
        if ((y >= 0f) && (y <= 1f))
        {
            y = 1f - y;
        }
        GameJoySDK.instance.setDefaultStartPosition(x, y);
    }

    public void SetDefaultUploadShareDialogPosition(float x, float y)
    {
        if ((y >= 0f) && (y <= 1f))
        {
            y = 1f - y;
        }
        GameJoySDK.instance.setUploadShareDialogDefaultPosition(x, y);
    }

    public void ShowRecorder()
    {
        GameJoySDK.instance.showGameJoyRecorder();
    }

    public void ShowVideoListDialog()
    {
        GameJoySDK.instance.showVideoListDialog();
    }

    public void StartMomentsRecording()
    {
        GameJoySDK.instance.startMomentRecording();
    }

    public bool isRecording
    {
        get
        {
            return GameJoySDK.instance.IsRecording();
        }
    }

    public bool isRecordingMoments
    {
        get
        {
            return GameJoySDK.instance.isRecordingMoments();
        }
    }

    public bool isShowed
    {
        get
        {
            return GameJoySDK.instance.IsShowed();
        }
    }

    public string sdkVersion
    {
        get
        {
            return string.Empty;
        }
    }
}

