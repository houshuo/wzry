namespace IIPSMobile
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class IIPSMobileVersionCallBack
    {
        private OnActionMsgFunc actionMsg;
        private OnErrorFunc errFunc;
        private OnNoticeInstallApkFunc installApk;
        public IntPtr mCallBack = IntPtr.Zero;
        private static IIPSMobileVersionCallBackInterface mImpCB;
        private IntPtr pManagedObject = IntPtr.Zero;
        private OnProgressFunc progressFunc;
        private SaveConfigFunc saveFUnc;
        private OnSuccessFunc succFUnc;
        private OnGetNewVersionInfoFunc versionFunc;

        public IIPSMobileVersionCallBack(IIPSMobileVersionCallBackInterface imp)
        {
            this.pManagedObject = GCHandle.ToIntPtr(GCHandle.Alloc(imp, GCHandleType.Normal));
            this.versionFunc = new OnGetNewVersionInfoFunc(IIPSMobileVersionCallBack.OnGetNewVersionInfo);
            this.progressFunc = new OnProgressFunc(IIPSMobileVersionCallBack.OnProgress);
            this.errFunc = new OnErrorFunc(IIPSMobileVersionCallBack.OnError);
            this.succFUnc = new OnSuccessFunc(IIPSMobileVersionCallBack.OnSuccess);
            this.saveFUnc = new SaveConfigFunc(IIPSMobileVersionCallBack.SaveConfig);
            this.installApk = new OnNoticeInstallApkFunc(IIPSMobileVersionCallBack.OnNoticeInstallApk);
            this.actionMsg = new OnActionMsgFunc(IIPSMobileVersionCallBack.OnActionMsg);
        }

        public void CreateCppVersionInfoCallBack()
        {
            this.mCallBack = CreateVersionInfoCallBack(new OnGetNewVersionInfoFunc(IIPSMobileVersionCallBack.OnGetNewVersionInfo), new OnProgressFunc(IIPSMobileVersionCallBack.OnProgress), new OnErrorFunc(IIPSMobileVersionCallBack.OnError), new OnSuccessFunc(IIPSMobileVersionCallBack.OnSuccess), new SaveConfigFunc(IIPSMobileVersionCallBack.SaveConfig), new OnNoticeInstallApkFunc(IIPSMobileVersionCallBack.OnNoticeInstallApk), new OnActionMsgFunc(IIPSMobileVersionCallBack.OnActionMsg), this.pManagedObject);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern IntPtr CreateVersionInfoCallBack(OnGetNewVersionInfoFunc onGetNewVersionInfoFunc, OnProgressFunc onProgressFunc, OnErrorFunc onErrorFunc, OnSuccessFunc onSuccessFunc, SaveConfigFunc saveConfigFunc, OnNoticeInstallApkFunc noticeInstallApk, OnActionMsgFunc msg, IntPtr callback);
        public void DeleteCppVersionCallBack()
        {
            if (this.mCallBack != IntPtr.Zero)
            {
                GCHandle.FromIntPtr(GetCallBackGCHandle(this.mCallBack)).Free();
                DestroyVersionInfoCallBack(this.mCallBack);
                this.mCallBack = IntPtr.Zero;
            }
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern void DestroyVersionInfoCallBack(IntPtr callback);
        ~IIPSMobileVersionCallBack()
        {
            if (this.mCallBack != IntPtr.Zero)
            {
                GCHandle.FromIntPtr(GetCallBackGCHandle(this.mCallBack)).Free();
                DestroyVersionInfoCallBack(this.mCallBack);
                this.mCallBack = IntPtr.Zero;
            }
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern IntPtr GetCallBackGCHandle(IntPtr callback);
        [MonoPInvokeCallback(typeof(OnActionMsgFunc))]
        public static byte OnActionMsg(IntPtr callback, [MarshalAs(UnmanagedType.LPStr)] string url)
        {
            mImpCB = (IIPSMobileVersionCallBackInterface) GCHandle.FromIntPtr(callback).Target;
            return mImpCB.OnActionMsg(url);
        }

        [MonoPInvokeCallback(typeof(OnErrorFunc))]
        public static void OnError(IntPtr callback, VERSIONSTAGE curVersionStage, uint errorCode)
        {
            mImpCB = (IIPSMobileVersionCallBackInterface) GCHandle.FromIntPtr(callback).Target;
            mImpCB.OnError(curVersionStage, errorCode);
        }

        [MonoPInvokeCallback(typeof(OnGetNewVersionInfoFunc))]
        public static byte OnGetNewVersionInfo(IntPtr callback, IntPtr newVersionInfo)
        {
            mImpCB = (IIPSMobileVersionCallBackInterface) GCHandle.FromIntPtr(callback).Target;
            VERSIONINFO versioninfo = (VERSIONINFO) Marshal.PtrToStructure(newVersionInfo, typeof(VERSIONINFO));
            return mImpCB.OnGetNewVersionInfo(versioninfo);
        }

        [MonoPInvokeCallback(typeof(OnNoticeInstallApkFunc))]
        public static byte OnNoticeInstallApk(IntPtr callback, [MarshalAs(UnmanagedType.LPStr)] string url)
        {
            mImpCB = (IIPSMobileVersionCallBackInterface) GCHandle.FromIntPtr(callback).Target;
            return mImpCB.OnNoticeInstallApk(url);
        }

        [MonoPInvokeCallback(typeof(OnProgressFunc))]
        public static void OnProgress(IntPtr callback, VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize)
        {
            mImpCB = (IIPSMobileVersionCallBackInterface) GCHandle.FromIntPtr(callback).Target;
            mImpCB.OnProgress(curVersionStage, totalSize, nowSize);
        }

        [MonoPInvokeCallback(typeof(OnSuccessFunc))]
        public static void OnSuccess(IntPtr callback)
        {
            mImpCB = (IIPSMobileVersionCallBackInterface) GCHandle.FromIntPtr(callback).Target;
            mImpCB.OnSuccess();
        }

        [MonoPInvokeCallback(typeof(SaveConfigFunc))]
        public static void SaveConfig(IntPtr callback, uint bufferSize, IntPtr configBuffer)
        {
            mImpCB = (IIPSMobileVersionCallBackInterface) GCHandle.FromIntPtr(callback).Target;
            mImpCB.SaveConfig(bufferSize, configBuffer);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPVERSION
        {
            public IIPSMobileVersionCallBack.PROGRAMMEVERSION programmeVersion;
            public IIPSMobileVersionCallBack.DATAVERSION dataVersion;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DATAVERSION
        {
            public ushort DataVersion;
        }

        internal delegate byte OnActionMsgFunc(IntPtr callback, [MarshalAs(UnmanagedType.LPStr)] string url);

        internal delegate void OnErrorFunc(IntPtr callback, IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode);

        internal delegate byte OnGetNewVersionInfoFunc(IntPtr callback, IntPtr newVersionInfo);

        internal delegate byte OnNoticeInstallApkFunc(IntPtr callback, [MarshalAs(UnmanagedType.LPStr)] string url);

        internal delegate void OnProgressFunc(IntPtr callback, IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize);

        internal delegate void OnSuccessFunc(IntPtr callback);

        [StructLayout(LayoutKind.Sequential)]
        public struct PROGRAMMEVERSION
        {
            public ushort MajorVersion_Number;
            public ushort MinorVersion_Number;
            public ushort Revision_Number;
        }

        internal delegate void SaveConfigFunc(IntPtr callback, uint bufferSize, IntPtr configBuffer);

        [StructLayout(LayoutKind.Sequential)]
        public struct VERSIONINFO
        {
            public byte isAppUpdating;
            public byte isAppDiffUpdating;
            public byte isForcedUpdating;
            public IIPSMobileVersionCallBack.APPVERSION newAppVersion;
            public ulong needDownloadSize;
        }

        public enum VERSIONSTAGE
        {
            VS_CheckApkMd5 = 0x11,
            VS_CreateApk = 0x10,
            VS_DiffUpdata = 8,
            VS_DownApkConfig = 15,
            VS_DownloadData = 5,
            VS_ExtractData = 7,
            VS_Fail = 100,
            VS_FirstExtract = 10,
            VS_FullUpdate = 9,
            VS_FullUpdate_CompareMetaFile = 14,
            VS_FullUpdate_CreateTask = 0x12,
            VS_FullUpdate_Extract = 11,
            VS_FullUpdate_GetFileList = 12,
            VS_FullUpdate_GetMetaFile = 13,
            VS_GetDownloadVersion = 4,
            VS_GetVersionInfo = 3,
            VS_MergeData = 6,
            VS_SelfDataCheck = 1,
            VS_SelfDataRepair = 2,
            VS_SourceAnalyseDiff = 0x5d,
            VS_SourceDownload = 0x5e,
            VS_SourceExtract = 0x5f,
            VS_SourcePrepareUpdate = 0x5c,
            VS_SourceUpdateCures = 90,
            VS_SourceUpdateDownloadList = 0x5b,
            VS_Start = 0,
            VS_Success = 0x63
        }
    }
}

