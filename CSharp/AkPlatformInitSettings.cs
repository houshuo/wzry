using System;

public class AkPlatformInitSettings : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkPlatformInitSettings() : this(AkSoundEnginePINVOKE.CSharp_new_AkPlatformInitSettings(), true)
    {
    }

    internal AkPlatformInitSettings(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkPlatformInitSettings settings = this;
        lock (settings)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkPlatformInitSettings(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkPlatformInitSettings()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkPlatformInitSettings obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public float fLEngineDefaultPoolRatioThreshold
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_fLEngineDefaultPoolRatioThreshold_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_fLEngineDefaultPoolRatioThreshold_set(this.swigCPtr, value);
        }
    }

    public AkThreadProperties threadBankManager
    {
        get
        {
            IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadBankManager_get(this.swigCPtr);
            return (!(cPtr == IntPtr.Zero) ? new AkThreadProperties(cPtr, false) : null);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadBankManager_set(this.swigCPtr, AkThreadProperties.getCPtr(value));
        }
    }

    public AkThreadProperties threadLEngine
    {
        get
        {
            IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadLEngine_get(this.swigCPtr);
            return (!(cPtr == IntPtr.Zero) ? new AkThreadProperties(cPtr, false) : null);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadLEngine_set(this.swigCPtr, AkThreadProperties.getCPtr(value));
        }
    }

    public AkThreadProperties threadMonitor
    {
        get
        {
            IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadMonitor_get(this.swigCPtr);
            return (!(cPtr == IntPtr.Zero) ? new AkThreadProperties(cPtr, false) : null);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_threadMonitor_set(this.swigCPtr, AkThreadProperties.getCPtr(value));
        }
    }

    public uint uChannelMask
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uChannelMask_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uChannelMask_set(this.swigCPtr, value);
        }
    }

    public uint uLEngineDefaultPoolSize
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uLEngineDefaultPoolSize_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uLEngineDefaultPoolSize_set(this.swigCPtr, value);
        }
    }

    public ushort uNumRefillsInVoice
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uNumRefillsInVoice_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uNumRefillsInVoice_set(this.swigCPtr, value);
        }
    }

    public uint uSampleRate
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uSampleRate_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkPlatformInitSettings_uSampleRate_set(this.swigCPtr, value);
        }
    }
}

