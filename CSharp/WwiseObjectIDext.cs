using System;

public class WwiseObjectIDext : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public WwiseObjectIDext() : this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectIDext(), true)
    {
    }

    internal WwiseObjectIDext(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        WwiseObjectIDext dext = this;
        lock (dext)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_WwiseObjectIDext(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~WwiseObjectIDext()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(WwiseObjectIDext obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public AkNodeType GetNodeType()
    {
        return (AkNodeType) AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_GetNodeType(this.swigCPtr);
    }

    public bool IsEqualTo(WwiseObjectIDext in_rOther)
    {
        return AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_IsEqualTo(this.swigCPtr, getCPtr(in_rOther));
    }

    public bool bIsBus
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_bIsBus_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_bIsBus_set(this.swigCPtr, value);
        }
    }

    public uint id
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_id_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_id_set(this.swigCPtr, value);
        }
    }
}

