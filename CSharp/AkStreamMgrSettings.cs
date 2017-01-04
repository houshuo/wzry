using System;

public class AkStreamMgrSettings : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkStreamMgrSettings() : this(AkSoundEnginePINVOKE.CSharp_new_AkStreamMgrSettings(), true)
    {
    }

    internal AkStreamMgrSettings(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkStreamMgrSettings settings = this;
        lock (settings)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkStreamMgrSettings(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkStreamMgrSettings()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkStreamMgrSettings obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public uint uMemorySize
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkStreamMgrSettings_uMemorySize_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkStreamMgrSettings_uMemorySize_set(this.swigCPtr, value);
        }
    }
}

