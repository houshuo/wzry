using System;

public class AkSoundPosition : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkSoundPosition() : this(AkSoundEnginePINVOKE.CSharp_new_AkSoundPosition(), true)
    {
    }

    internal AkSoundPosition(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkSoundPosition position = this;
        lock (position)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkSoundPosition(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkSoundPosition()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkSoundPosition obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public AkVector Orientation
    {
        get
        {
            IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Orientation_get(this.swigCPtr);
            return (!(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : null);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Orientation_set(this.swigCPtr, AkVector.getCPtr(value));
        }
    }

    public AkVector Position
    {
        get
        {
            IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Position_get(this.swigCPtr);
            return (!(cPtr == IntPtr.Zero) ? new AkVector(cPtr, false) : null);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Position_set(this.swigCPtr, AkVector.getCPtr(value));
        }
    }
}

