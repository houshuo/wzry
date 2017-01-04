using System;

public class AkObjectInfo : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkObjectInfo() : this(AkSoundEnginePINVOKE.CSharp_new_AkObjectInfo(), true)
    {
    }

    internal AkObjectInfo(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkObjectInfo info = this;
        lock (info)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkObjectInfo(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkObjectInfo()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkObjectInfo obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public int iDepth
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkObjectInfo_iDepth_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkObjectInfo_iDepth_set(this.swigCPtr, value);
        }
    }

    public uint objID
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkObjectInfo_objID_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkObjectInfo_objID_set(this.swigCPtr, value);
        }
    }

    public uint parentID
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkObjectInfo_parentID_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkObjectInfo_parentID_set(this.swigCPtr, value);
        }
    }
}

