using System;

public class AkOutputSettings : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkOutputSettings() : this(AkSoundEnginePINVOKE.CSharp_new_AkOutputSettings(), true)
    {
    }

    internal AkOutputSettings(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkOutputSettings settings = this;
        lock (settings)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkOutputSettings(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkOutputSettings()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkOutputSettings obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public AkChannelConfig channelConfig
    {
        get
        {
            IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkOutputSettings_channelConfig_get(this.swigCPtr);
            return (!(cPtr == IntPtr.Zero) ? new AkChannelConfig(cPtr, false) : null);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkOutputSettings_channelConfig_set(this.swigCPtr, AkChannelConfig.getCPtr(value));
        }
    }

    public AkPanningRule ePanningRule
    {
        get
        {
            return (AkPanningRule) AkSoundEnginePINVOKE.CSharp_AkOutputSettings_ePanningRule_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkOutputSettings_ePanningRule_set(this.swigCPtr, (int) value);
        }
    }
}

