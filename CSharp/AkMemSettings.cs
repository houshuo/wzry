using System;

public class AkMemSettings : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkMemSettings() : this(AkSoundEnginePINVOKE.CSharp_new_AkMemSettings(), true)
    {
    }

    internal AkMemSettings(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkMemSettings settings = this;
        lock (settings)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkMemSettings(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkMemSettings()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkMemSettings obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public uint uMaxNumPools
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkMemSettings_uMaxNumPools_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkMemSettings_uMaxNumPools_set(this.swigCPtr, value);
        }
    }
}

