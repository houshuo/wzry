using System;

public class AkMusicPlaylistCallbackInfo : IDisposable
{
    protected bool swigCMemOwn;
    private IntPtr swigCPtr;

    public AkMusicPlaylistCallbackInfo() : this(AkSoundEnginePINVOKE.CSharp_new_AkMusicPlaylistCallbackInfo(), true)
    {
    }

    internal AkMusicPlaylistCallbackInfo(IntPtr cPtr, bool cMemoryOwn)
    {
        this.swigCMemOwn = cMemoryOwn;
        this.swigCPtr = cPtr;
    }

    public virtual void Dispose()
    {
        AkMusicPlaylistCallbackInfo info = this;
        lock (info)
        {
            if (this.swigCPtr != IntPtr.Zero)
            {
                if (this.swigCMemOwn)
                {
                    this.swigCMemOwn = false;
                    AkSoundEnginePINVOKE.CSharp_delete_AkMusicPlaylistCallbackInfo(this.swigCPtr);
                }
                this.swigCPtr = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
    }

    ~AkMusicPlaylistCallbackInfo()
    {
        this.Dispose();
    }

    internal static IntPtr getCPtr(AkMusicPlaylistCallbackInfo obj)
    {
        return ((obj != null) ? obj.swigCPtr : IntPtr.Zero);
    }

    public uint playlistID
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_playlistID_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_playlistID_set(this.swigCPtr, value);
        }
    }

    public uint uNumPlaylistItems
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uNumPlaylistItems_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uNumPlaylistItems_set(this.swigCPtr, value);
        }
    }

    public uint uPlaylistItemDone
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uPlaylistItemDone_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uPlaylistItemDone_set(this.swigCPtr, value);
        }
    }

    public uint uPlaylistSelection
    {
        get
        {
            return AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uPlaylistSelection_get(this.swigCPtr);
        }
        set
        {
            AkSoundEnginePINVOKE.CSharp_AkMusicPlaylistCallbackInfo_uPlaylistSelection_set(this.swigCPtr, value);
        }
    }
}

