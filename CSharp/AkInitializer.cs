using System;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("Wwise/AkInitializer"), RequireComponent(typeof(AkTerminator))]
public class AkInitializer : MonoBehaviour
{
    public string basePath = AkBankPathUtil.GetDefaultPath();
    public const int c_DefaultPoolSize = 0x1000;
    public const string c_Language = "English(US)";
    public const int c_LowerPoolSize = 0x800;
    public const float c_MemoryCutoffThreshold = 0.95f;
    public const int c_StreamingPoolSize = 0x400;
    public int defaultPoolSize = 0x1000;
    public string language = "English(US)";
    public int lowerPoolSize = 0x800;
    public float memoryCutoffThreshold = 0.95f;
    private static AkInitializer ms_Instance;
    public static bool s_loadBankFromMemory = true;
    public int streamingPoolSize = 0x400;

    private void Awake()
    {
        if (ms_Instance != null)
        {
            if (ms_Instance != this)
            {
                UnityEngine.Object.DestroyImmediate(base.gameObject);
            }
        }
        else
        {
            Debug.Log("WwiseUnity: Initialize sound engine ...");
            DebugHelper.CustomLog("WwiseUnity: Initialize sound engine ...");
            AkMemSettings settings = new AkMemSettings {
                uMaxNumPools = 40
            };
            AkDeviceSettings settings2 = new AkDeviceSettings();
            AkSoundEngine.GetDefaultDeviceSettings(settings2);
            AkStreamMgrSettings settings3 = new AkStreamMgrSettings {
                uMemorySize = (uint) (this.streamingPoolSize * 0x400)
            };
            AkInitSettings settings4 = new AkInitSettings();
            AkSoundEngine.GetDefaultInitSettings(settings4);
            settings4.uDefaultPoolSize = (uint) (this.defaultPoolSize * 0x400);
            AkPlatformInitSettings settings5 = new AkPlatformInitSettings();
            AkSoundEngine.GetDefaultPlatformInitSettings(settings5);
            settings5.uLEngineDefaultPoolSize = (uint) (this.lowerPoolSize * 0x400);
            settings5.fLEngineDefaultPoolRatioThreshold = this.memoryCutoffThreshold;
            AkMusicSettings settings6 = new AkMusicSettings();
            AkSoundEngine.GetDefaultMusicSettings(settings6);
            if (AkSoundEngine.Init(settings, settings3, settings2, settings4, settings5, settings6) != AKRESULT.AK_Success)
            {
                Debug.LogError("WwiseUnity: Failed to initialize the sound engine. Abort.");
            }
            else
            {
                ms_Instance = this;
                AkBankPathUtil.UsePlatformSpecificPath();
                string platformBasePath = AkBankPathUtil.GetPlatformBasePath();
                if (!s_loadBankFromMemory)
                {
                }
                AkSoundEngine.SetBasePath(platformBasePath);
                AkSoundEngine.SetCurrentLanguage(this.language);
                if (AkCallbackManager.Init() != AKRESULT.AK_Success)
                {
                    Debug.LogError("WwiseUnity: Failed to initialize Callback Manager. Terminate sound engine.");
                    AkSoundEngine.Term();
                    ms_Instance = null;
                }
                else
                {
                    AKRESULT akresult;
                    uint num;
                    Debug.Log("WwiseUnity: Sound engine initialized.");
                    UnityEngine.Object.DontDestroyOnLoad(this);
                    if (s_loadBankFromMemory)
                    {
                        string soundBankPathInResources = GetSoundBankPathInResources("Init.bytes");
                        CBinaryObject content = Singleton<CResourceManager>.GetInstance().GetResource(soundBankPathInResources, typeof(TextAsset), enResourceType.Sound, false, false).m_content as CBinaryObject;
                        GCHandle handle = GCHandle.Alloc(content.m_data, GCHandleType.Pinned);
                        IntPtr ptr = handle.AddrOfPinnedObject();
                        if (ptr != IntPtr.Zero)
                        {
                            akresult = AkSoundEngine.LoadBank(ptr, (uint) content.m_data.Length, -1, out num);
                            handle.Free();
                        }
                        else
                        {
                            akresult = AKRESULT.AK_Fail;
                        }
                        Singleton<CResourceManager>.GetInstance().RemoveCachedResource(soundBankPathInResources);
                    }
                    else
                    {
                        akresult = AkSoundEngine.LoadBank("Init.bnk", -1, out num);
                    }
                    if (akresult != AKRESULT.AK_Success)
                    {
                        Debug.LogError("WwiseUnity: Failed load Init.bnk with result: " + akresult.ToString());
                    }
                }
            }
        }
    }

    public static string GetBasePath()
    {
        return ms_Instance.basePath;
    }

    public static string GetCurrentLanguage()
    {
        return ms_Instance.language;
    }

    public static string GetSoundBankPathInResources(string bankName)
    {
        string str = string.Empty;
        str = "Sound/Android/";
        return CFileManager.CombinePath(str, bankName);
    }

    private void LateUpdate()
    {
        if (ms_Instance != null)
        {
            AkCallbackManager.PostCallbacks();
            AkSoundEngine.RenderAudio();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (ms_Instance != null)
        {
            if (!pauseStatus)
            {
                AkSoundEngine.WakeupFromSuspend();
            }
            else
            {
                AkSoundEngine.Suspend();
            }
            AkSoundEngine.RenderAudio();
        }
    }

    private void OnDestroy()
    {
        if (ms_Instance == this)
        {
            AkCallbackManager.SetMonitoringCallback((ErrorLevel) 0, null);
            ms_Instance = null;
        }
    }

    private void OnEnable()
    {
        if ((ms_Instance == null) && AkSoundEngine.IsInitialized())
        {
            ms_Instance = this;
        }
    }
}

