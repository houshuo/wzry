namespace Assets.Scripts.GameSystem
{
    using System;

    public enum enTMAssistantDownloadTaskState
    {
        DownloadSDKTaskState_DELETE = 6,
        DownloadSDKTaskState_DOWNLOADING = 2,
        DownloadSDKTaskState_FAILED = 5,
        DownloadSDKTaskState_PAUSED = 3,
        DownloadSDKTaskState_SUCCEED = 4,
        DownloadSDKTaskState_WAITING = 1
    }
}

