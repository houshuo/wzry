using System;

public class _ArrayPoolLEngineDefault : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public _ArrayPoolLEngineDefault() : this(AkSoundEnginePINVOKE.CSharp_new__ArrayPoolLEngineDefault(), true)
    {
    }

    internal _ArrayPoolLEngineDefault(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        _ArrayPoolLEngineDefault default2 = this;
        lock (default2)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete__ArrayPoolLEngineDefault(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~_ArrayPoolLEngineDefault()
    {
        this.Dispose();
    }

    public static int Get()
    {
        return AkSoundEnginePINVOKE.CSharp__ArrayPoolLEngineDefault_Get();
    }

    internal static IntPtr getCPtr(_ArrayPoolLEngineDefault obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }
}

