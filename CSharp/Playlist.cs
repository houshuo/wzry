using System;

public class Playlist : AkPlaylistArray
{
    private IntPtr swigCPtr;

    public Playlist() : this(AkSoundEnginePINVOKE.CSharp_new_Playlist(), true)
    {
    }

    internal Playlist(IntPtr cPtr, bool cMemoryOwn) : base(AkSoundEnginePINVOKE.CSharp_Playlist_SWIGUpcast(cPtr), cMemoryOwn)
    {
        this.swigCPtr = cPtr;
    }

    public override void Dispose()
    {
        Playlist playlist = this;
        lock (playlist)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (base.swigCMemOwn)
                {
                    base.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_Playlist(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }

    public AKRESULT Enqueue(uint in_audioNodeID)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_Playlist_Enqueue__SWIG_4(this.swigCPtr, in_audioNodeID);
    }

    public AKRESULT Enqueue(uint in_audioNodeID, int in_msDelay)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_Playlist_Enqueue__SWIG_3(this.swigCPtr, in_audioNodeID, in_msDelay);
    }

    public AKRESULT Enqueue(uint in_audioNodeID, int in_msDelay, IntPtr in_pCustomInfo)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_Playlist_Enqueue__SWIG_2(this.swigCPtr, in_audioNodeID, in_msDelay, in_pCustomInfo);
    }

    public AKRESULT Enqueue(uint in_audioNodeID, int in_msDelay, IntPtr in_pCustomInfo, uint in_cExternals)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_Playlist_Enqueue__SWIG_1(this.swigCPtr, in_audioNodeID, in_msDelay, in_pCustomInfo, in_cExternals);
    }

    public AKRESULT Enqueue(uint in_audioNodeID, int in_msDelay, IntPtr in_pCustomInfo, uint in_cExternals, AkExternalSourceInfo in_pExternalSources)
    {
        return (AKRESULT) AkSoundEnginePINVOKE.CSharp_Playlist_Enqueue__SWIG_0(this.swigCPtr, in_audioNodeID, in_msDelay, in_pCustomInfo, in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources));
    }

    ~Playlist()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(Playlist obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }
}

