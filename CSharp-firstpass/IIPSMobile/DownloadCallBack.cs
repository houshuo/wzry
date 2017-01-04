namespace IIPSMobile
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class DownloadCallBack
    {
        private OnDownloadErrorFunc errFunc;
        public IntPtr mCallBack = IntPtr.Zero;
        public static IIPSMobileDownloadCallbackInterface mCBImp;
        private OnDownloadProgressFunc progressFunc;
        private OnDownloadSuccessFunc succFunc;

        public DownloadCallBack(IIPSMobileDownloadCallbackInterface CBImp)
        {
            IntPtr callback = GCHandle.ToIntPtr(GCHandle.Alloc(CBImp, GCHandleType.Normal));
            this.errFunc = new OnDownloadErrorFunc(DownloadCallBack.OnDownloadError);
            this.succFunc = new OnDownloadSuccessFunc(DownloadCallBack.OnDownloadSuccess);
            this.progressFunc = new OnDownloadProgressFunc(DownloadCallBack.OnDownloadProgress);
            this.mCallBack = CreateDownlaodMgrCallBack(this.errFunc, this.succFunc, this.progressFunc, callback);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern IntPtr CreateDownlaodMgrCallBack(OnDownloadErrorFunc onDownloadError, OnDownloadSuccessFunc onDownloadSuccess, OnDownloadProgressFunc onDownloadProgress, IntPtr callback);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern void DestroyDownlaodMgrCallBack(IntPtr callback);
        ~DownloadCallBack()
        {
            if (this.mCallBack != IntPtr.Zero)
            {
                GCHandle.FromIntPtr(GetDownloadCallbackGCHandle(this.mCallBack)).Free();
                DestroyDownlaodMgrCallBack(this.mCallBack);
            }
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern IntPtr GetDownloadCallbackGCHandle(IntPtr callback);
        [MonoPInvokeCallback(typeof(OnDownloadErrorFunc))]
        public static void OnDownloadError(IntPtr callback, uint taskId, uint errorCode)
        {
            mCBImp = (IIPSMobileDownloadCallbackInterface) GCHandle.FromIntPtr(callback).Target;
            mCBImp.OnDownloadError(taskId, errorCode);
        }

        [MonoPInvokeCallback(typeof(OnDownloadProgressFunc))]
        public static void OnDownloadProgress(IntPtr callback, uint taskId, DataDownloader.DownloadInfo info)
        {
            mCBImp = (IIPSMobileDownloadCallbackInterface) GCHandle.FromIntPtr(callback).Target;
            mCBImp.OnDownloadProgress(taskId, info);
        }

        [MonoPInvokeCallback(typeof(OnDownloadSuccessFunc))]
        public static void OnDownloadSuccess(IntPtr callback, uint taskId)
        {
            mCBImp = (IIPSMobileDownloadCallbackInterface) GCHandle.FromIntPtr(callback).Target;
            mCBImp.OnDownloadSuccess(taskId);
        }

        internal delegate void OnDownloadErrorFunc(IntPtr callback, uint taskId, uint errorCode);

        internal delegate void OnDownloadProgressFunc(IntPtr callback, uint taskId, DataDownloader.DownloadInfo info);

        internal delegate void OnDownloadSuccessFunc(IntPtr callback, uint taskId);
    }
}

