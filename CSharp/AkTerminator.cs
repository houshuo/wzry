using System;
using System.Threading;
using UnityEngine;

[AddComponentMenu("Wwise/AkTerminator")]
public class AkTerminator : MonoBehaviour
{
    private static AkTerminator ms_Instance;

    private void Awake()
    {
        if (ms_Instance != null)
        {
            if (ms_Instance != this)
            {
                UnityEngine.Object.DestroyImmediate(this);
            }
        }
        else
        {
            UnityEngine.Object.DontDestroyOnLoad(this);
            ms_Instance = this;
        }
    }

    private void OnApplicationQuit()
    {
        this.Terminate();
    }

    private void OnDestroy()
    {
        if (ms_Instance == this)
        {
            ms_Instance = null;
        }
    }

    private void Terminate()
    {
        if (((ms_Instance != null) && (ms_Instance == this)) && AkSoundEngine.IsInitialized())
        {
            AkSoundEngine.StopAll();
            AkSoundEngine.RenderAudio();
            for (uint i = 0; i < 50; i++)
            {
                AkCallbackManager.PostCallbacks();
                using (EventWaitHandle handle = new ManualResetEvent(false))
                {
                    handle.WaitOne(TimeSpan.FromMilliseconds(1.0));
                }
            }
            AkSoundEngine.Term();
            ms_Instance = null;
            AkCallbackManager.Term();
        }
    }
}

