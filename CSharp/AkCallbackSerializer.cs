using System;

public class AkCallbackSerializer : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkCallbackSerializer() : this(AkSoundEnginePINVOKE.CSharp_new_AkCallbackSerializer(), true)
    {
    }

    internal AkCallbackSerializer(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkCallbackSerializer serializer = this;
        lock (serializer)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkCallbackSerializer(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkCallbackSerializer()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkCallbackSerializer obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public static AKRESULT Init(IntPtr in_pMemory, uint in_uSize)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_Init(in_pMemory, in_uSize);
    }

    public static IntPtr Lock()
    {
        return AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_Lock();
    }

    public static void SetLocalOutput(uint in_uErrorLevel)
    {
        AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_SetLocalOutput(in_uErrorLevel);
    }

    public static void Term()
    {
        AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_Term();
    }

    public static void Unlock()
    {
        AkSoundEnginePINVOKE.CSharp_AkCallbackSerializer_Unlock();
    }
}

