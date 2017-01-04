using System;

public interface IApolloVoice
{
    int _CloseMic();
    int _CloseSpeaker();
    int _CreateApolloVoiceEngine(string appID);
    int _DestoryApolloVoiceEngine();
    int _DownloadVoiceFile(string strFullPath, bool bAutoPlay);
    int _ForbidMemberVoice(int nMemberId);
    int _GetFileKey(byte[] fileKey);
    int _GetJoinRoomResult();
    int _GetMemberState(int[] memberState);
    int _GetMicLevel();
    int _GetSpeakerLevel();
    int _JoinRoom(string url1, string url2, string url3, long roomId, long roomKey, short memberId, string OpenId, int nTimeOut);
    int _OpenMic();
    int _OpenSpeaker();
    int _Pause();
    int _PlayFile(string strFullPath);
    int _QuitRoom(long roomId, short memberId, string OpenId);
    int _Resume();
    int _SendRecFile(string strFullPath);
    int _SetMemberCount(int nCount);
    int _SetMode(int nMode);
    int _SetSpeakerVolume(int nVol);
    int _StartRecord(string strFullPath);
    int _StopRecord(bool bAutoSend);
    int _TestMic();
    void Init();
}

