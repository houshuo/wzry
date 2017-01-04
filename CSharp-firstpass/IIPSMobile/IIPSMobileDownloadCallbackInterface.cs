namespace IIPSMobile
{
    using System;

    public interface IIPSMobileDownloadCallbackInterface
    {
        void OnDownloadError(uint taskId, uint errorCode);
        void OnDownloadProgress(uint taskId, DataDownloader.DownloadInfo info);
        void OnDownloadSuccess(uint taskId);
    }
}

