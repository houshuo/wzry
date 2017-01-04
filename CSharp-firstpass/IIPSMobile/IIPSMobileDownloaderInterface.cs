namespace IIPSMobile
{
    using System;

    public interface IIPSMobileDownloaderInterface
    {
        bool CancelDownload(uint taskId);
        bool DownloadIfsData(uint fileId, byte priority, ref uint taskId);
        bool DownloadLocalData(string downloadUrl, string savePath, byte priority, ref uint taskID, bool bDoBrokenResume);
        uint GetDownloadSpeed();
        bool GetDownloadTaskInfo(uint taskId, ref DataDownloader.DownloadInfo downloadInfo);
        uint GetLastError();
        bool Init(IIPSMobileDownloadCallbackInterface callback);
        bool PauseDownload();
        bool ResumeDownload();
        bool SetDownloadSpeed(uint downloadSpeed);
        bool StartDownload();
    }
}

