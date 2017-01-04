using System;

public class AkListenerPosition : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkListenerPosition() : this(AkSoundEnginePINVOKE.CSharp_new_AkListenerPosition(), true)
    {
    }

    internal AkListenerPosition(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkListenerPosition position = this;
        lock (position)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkListenerPosition(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkListenerPosition()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkListenerPosition obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public AkVector OrientationFront
    {
        get
        {
            IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkListenerPosition_OrientationFront_get(this.swigCPtr);
            return (!(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : null);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkListenerPosition_OrientationFront_set(this.swigCPtr, AkVector.getCPtr(value));
        }
    }

    public AkVector OrientationTop
    {
        get
        {
            IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkListenerPosition_OrientationTop_get(this.swigCPtr);
            return (!(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : null);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkListenerPosition_OrientationTop_set(this.swigCPtr, AkVector.getCPtr(value));
        }
    }

    public AkVector Position
    {
        get
        {
            IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkListenerPosition_Position_get(this.swigCPtr);
            return (!(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : null);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkListenerPosition_Position_set(this.swigCPtr, AkVector.getCPtr(value));
        }
    }
}

