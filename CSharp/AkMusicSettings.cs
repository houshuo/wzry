using System;

public class AkMusicSettings : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkMusicSettings() : this(AkSoundEnginePINVOKE.CSharp_new_AkMusicSettings(), true)
    {
    }

    internal AkMusicSettings(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkMusicSettings settings = this;
        lock (settings)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkMusicSettings(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkMusicSettings()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkMusicSettings obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public float fStreamingLookAheadRatio
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkMusicSettings_fStreamingLookAheadRatio_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkMusicSettings_fStreamingLookAheadRatio_set(this.swigCPtr, value);
        }
    }
}

