using System;

public class _ArrayPoolDefault : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public _ArrayPoolDefault() : this(AkSoundEnginePINVOKE.CSharp_new__ArrayPoolDefault(), true)
    {
    }

    internal _ArrayPoolDefault(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        _ArrayPoolDefault default2 = this;
        lock (default2)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete__ArrayPoolDefault(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~_ArrayPoolDefault()
    {
        this.Dispose();
    }

    public static int Get()
    {
        return AkSoundEnginePINVOKE.CSharp__ArrayPoolDefault_Get();
    }

    internal static IntPtr getCPtr(_ArrayPoolDefault obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }
}

