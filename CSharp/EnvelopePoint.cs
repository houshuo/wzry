using System;

public class EnvelopePoint : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public EnvelopePoint() : this(AkSoundEnginePINVOKE.CSharp_new_EnvelopePoint(), true)
    {
    }

    internal EnvelopePoint(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        EnvelopePoint point = this;
        lock (point)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_EnvelopePoint(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~EnvelopePoint()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(EnvelopePoint obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public ushort uAttenuation
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uAttenuation_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uAttenuation_set(this.swigCPtr, value);
        }
    }

    public uint uPosition
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uPosition_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uPosition_set(this.swigCPtr, value);
        }
    }
}

