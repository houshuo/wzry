using System;

public class AkAuxSendValue : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    internal AkAuxSendValue(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkAuxSendValue value2 = this;
        lock (value2)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkAuxSendValue(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkAuxSendValue()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkAuxSendValue obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public uint auxBusID
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_auxBusID_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_auxBusID_set(this.swigCPtr, value);
        }
    }

    public float fControlValue
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_fControlValue_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_fControlValue_set(this.swigCPtr, value);
        }
    }
}

