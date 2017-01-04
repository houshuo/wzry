using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameJoyAndroidPreRender : MonoBehaviour
{
    private int bIsNewVersionSDK = -1;
    private bool bLogPostRender = true;
    private double dCaptureFrameUsedMS;
    private double dRenderCameraUsedMS;
    private long lTotalCaptureFrames;
    private long lTotalFrames;

    [DebuggerHidden]
    private IEnumerator CoroutineCaptureFrame()
    {
        return new <CoroutineCaptureFrame>c__IteratorE { <>f__this = this };
    }

    private bool DoCaptureFrame()
    {
        int num = 0;
        bool flag = false;
        try
        {
            num = Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_OnRecordeFrame(IntPtr.Zero, IntPtr.Zero);
            if (Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_BeginDraw(IntPtr.Zero, IntPtr.Zero) == 1)
            {
                if (this.bLogPostRender)
                {
                }
                this.RerenderCameraFrame();
                if (this.bLogPostRender)
                {
                    this.bLogPostRender = false;
                }
                flag = true;
            }
            num = Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_EndDraw(IntPtr.Zero, IntPtr.Zero);
        }
        catch (DllNotFoundException)
        {
        }
        return flag;
    }

    [DllImport("app_plugins_extra/lib/5cd271c9cb4817fa5c284ff4a782a511/libMMCodecSdk")]
    private static extern int Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_BeginDraw(IntPtr JNIEnv, IntPtr thiz);
    [DllImport("app_plugins_extra/lib/5cd271c9cb4817fa5c284ff4a782a511/libMMCodecSdk")]
    private static extern int Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_CallMethod(IntPtr JNIEnv, IntPtr thiz, int nMethodID, int nParam1, int nParam2, int nParam3, int nParam4, int nParam5);
    [DllImport("app_plugins_extra/lib/5cd271c9cb4817fa5c284ff4a782a511/libMMCodecSdk")]
    private static extern int Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_EndDraw(IntPtr JNIEnv, IntPtr thiz);
    [DllImport("app_plugins_extra/lib/5cd271c9cb4817fa5c284ff4a782a511/libMMCodecSdk")]
    private static extern int Java_com_tencent_qqgamemi_srp_agent_sdk_MMCodecSdk_OnRecordeFrame(IntPtr JNIEnv, IntPtr thiz);
    private void OnCaptureFrame()
    {
        if (this.lTotalFrames == 0)
        {
        }
        this.lTotalFrames += 1L;
        if ((this.lTotalFrames % 2L) != 0)
        {
            if (this.bIsNewVersionSDK == -1)
            {
                this.bIsNewVersionSDK = GameJoySDK.getGameJoyInstance().IsNewSDKVersion();
                if (this.bIsNewVersionSDK == -1)
                {
                }
            }
            if (((this.bIsNewVersionSDK != -1) && (this.bIsNewVersionSDK != 0)) && (GameJoySDK.getGameJoyInstance().GetRecorderStatus() == GameJoySDK.RECORER_STATUS.RS_STARTED))
            {
                base.StartCoroutine(this.CoroutineCaptureFrame());
            }
        }
    }

    private void OnPostRender()
    {
        this.OnCaptureFrame();
    }

    private void RerenderCameraFrame()
    {
        TimeSpan timeOfDay = DateTime.Now.TimeOfDay;
        Camera[] allCameras = Camera.allCameras;
        if (allCameras != null)
        {
            foreach (Camera camera in allCameras)
            {
                if (((camera != null) && camera.enabled) && (camera.targetTexture == null))
                {
                    camera.Render();
                }
            }
        }
    }

    private void Start()
    {
        this.bIsNewVersionSDK = -1;
        this.lTotalFrames = 0L;
        this.dCaptureFrameUsedMS = 0.0;
        this.lTotalCaptureFrames = 0L;
        this.dRenderCameraUsedMS = 0.0;
    }

    [CompilerGenerated]
    private sealed class <CoroutineCaptureFrame>c__IteratorE : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GameJoyAndroidPreRender <>f__this;
        internal TimeSpan <m_BeginTimeSpan>__0;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<m_BeginTimeSpan>__0 = DateTime.Now.TimeOfDay;
                    if (this.<>f__this.DoCaptureFrame())
                    {
                        this.<>f__this.lTotalCaptureFrames += 1L;
                    }
                    this.$PC = -1;
                    break;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

