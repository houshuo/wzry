using System;

public class AkRamp : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkRamp() : this(AkSoundEnginePINVOKE.CSharp_new_AkRamp__SWIG_0(), true)
    {
    }

    internal AkRamp(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public AkRamp(float in_fPrev, float in_fNext) : this(AkSoundEnginePINVOKE.CSharp_new_AkRamp__SWIG_1(in_fPrev, in_fNext), true)
    {
    }

    public virtual void Dispose()
    {
        AkRamp ramp = this;
        lock (ramp)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkRamp(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkRamp()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkRamp obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public float fNext
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkRamp_fNext_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkRamp_fNext_set(this.swigCPtr, value);
        }
    }

    public float fPrev
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkRamp_fPrev_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkRamp_fPrev_set(this.swigCPtr, value);
        }
    }
}

