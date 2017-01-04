using System;

public class AkVector : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkVector() : this(AkSoundEnginePINVOKE.CSharp_new_AkVector(), true)
    {
    }

    internal AkVector(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkVector vector = this;
        lock (vector)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkVector(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkVector()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkVector obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public float X
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkVector_X_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkVector_X_set(this.swigCPtr, value);
        }
    }

    public float Y
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkVector_Y_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkVector_Y_set(this.swigCPtr, value);
        }
    }

    public float Z
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkVector_Z_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkVector_Z_set(this.swigCPtr, value);
        }
    }
}

