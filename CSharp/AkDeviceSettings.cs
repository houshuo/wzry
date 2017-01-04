using System;

public class AkDeviceSettings : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkDeviceSettings() : this(AkSoundEnginePINVOKE.CSharp_new_AkDeviceSettings(), true)
    {
    }

    internal AkDeviceSettings(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkDeviceSettings settings = this;
        lock (settings)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkDeviceSettings(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkDeviceSettings()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkDeviceSettings obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public int ePoolAttributes
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_ePoolAttributes_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_ePoolAttributes_set(this.swigCPtr, value);
        }
    }

    public float fMaxCacheRatio
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_fMaxCacheRatio_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_fMaxCacheRatio_set(this.swigCPtr, value);
        }
    }

    public float fTargetAutoStmBufferLength
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_fTargetAutoStmBufferLength_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_fTargetAutoStmBufferLength_set(this.swigCPtr, value);
        }
    }

    public IntPtr pIOMemory
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_pIOMemory_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_pIOMemory_set(this.swigCPtr, value);
        }
    }

    public AkThreadProperties threadProperties
    {
        get
        {
            IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_threadProperties_get(this.swigCPtr);
            return (!(cPtr == IntPtr.Zero) ? new AkThreadProperties(cPtr, false) : null);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_threadProperties_set(this.swigCPtr, AkThreadProperties.getCPtr(value));
        }
    }

    public uint uGranularity
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uGranularity_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uGranularity_set(this.swigCPtr, value);
        }
    }

    public uint uIOMemoryAlignment
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemoryAlignment_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemoryAlignment_set(this.swigCPtr, value);
        }
    }

    public uint uIOMemorySize
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemorySize_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemorySize_set(this.swigCPtr, value);
        }
    }

    public uint uMaxCachePinnedBytes
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxCachePinnedBytes_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxCachePinnedBytes_set(this.swigCPtr, value);
        }
    }

    public uint uMaxConcurrentIO
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxConcurrentIO_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxConcurrentIO_set(this.swigCPtr, value);
        }
    }

    public uint uSchedulerTypeFlags
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uSchedulerTypeFlags_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uSchedulerTypeFlags_set(this.swigCPtr, value);
        }
    }
}

