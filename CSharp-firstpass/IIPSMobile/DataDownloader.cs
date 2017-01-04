namespace IIPSMobile
{
    using System;
    using System.Runtime.InteropServices;

    public class DataDownloader : IIPSMobileDownloaderInterface
    {
        private DownloadCallBack mCallback;
        public IntPtr mDownloader = IntPtr.Zero;

        public DataDownloader(IntPtr downloader)
        {
            this.mDownloader = downloader;
        }

        public bool CancelDownload(uint taskId)
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return false;
            }
            return (CancelDownload(this.mDownloader, taskId) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte CancelDownload(IntPtr dataDownloader, uint taskId);
        public bool DownloadIfsData(uint fileId, byte priority, ref uint taskId)
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return false;
            }
            return (DownloadIfsData(this.mDownloader, fileId, priority, ref taskId) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte DownloadIfsData(IntPtr dataDownloader, uint fileId, byte priority, ref uint taskId);
        public bool DownloadLocalData(string downloadUrl, string savePath, byte priority, ref uint taskID, bool bDoBrokenResume)
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return false;
            }
            byte num = 0;
            if (bDoBrokenResume)
            {
                num = 1;
            }
            return (DownloadLocalData(this.mDownloader, downloadUrl, savePath, priority, ref taskID, num) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte DownloadLocalData(IntPtr dataDownloader, [MarshalAs(UnmanagedType.LPStr)] string downloadurl, [MarshalAs(UnmanagedType.LPStr)] string filepath, byte priority, ref uint TaskID, byte bDoBrokenResume);
        public uint GetDownloadSpeed()
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return 0;
            }
            return GetDownloadSpeed(this.mDownloader);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint GetDownloadSpeed(IntPtr dataDownloader);
        public bool GetDownloadTaskInfo(uint taskId, ref DownloadInfo downloadInfo)
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return false;
            }
            return (GetDownloadTaskInfo(this.mDownloader, taskId, ref downloadInfo) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte GetDownloadTaskInfo(IntPtr dataDownloader, uint taskId, ref DownloadInfo downloadInfo);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint GetLastDownloaderError(IntPtr dataDownloader);
        public uint GetLastError()
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return 0;
            }
            return GetLastDownloaderError(this.mDownloader);
        }

        public bool Init(IIPSMobileDownloadCallbackInterface callback)
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return false;
            }
            this.mCallback = new DownloadCallBack(callback);
            return (InitDataDownloader(this.mDownloader, this.mCallback.mCallBack) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte InitDataDownloader(IntPtr dataDownloader, IntPtr callback);
        public bool PauseDownload()
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return false;
            }
            return (PauseDownload(this.mDownloader) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte PauseDownload(IntPtr dataDownloader);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte ResumeDonload(IntPtr dataDownloader);
        public bool ResumeDownload()
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return false;
            }
            return (ResumeDonload(this.mDownloader) > 0);
        }

        public bool SetDownloadSpeed(uint downloadSpeed)
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return false;
            }
            return (SetDownloadSpeed(this.mDownloader, downloadSpeed) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte SetDownloadSpeed(IntPtr dataDownloader, uint downloadSpeed);
        public bool StartDownload()
        {
            if (this.mDownloader == IntPtr.Zero)
            {
                return false;
            }
            return (StartDownload(this.mDownloader) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte StartDownload(IntPtr dataDownloader);

        [StructLayout(LayoutKind.Sequential)]
        public struct DownloadInfo
        {
            public ulong needDownloadSize;
            public ulong downloadSize;
            public ulong fileSize;
        }
    }
}

