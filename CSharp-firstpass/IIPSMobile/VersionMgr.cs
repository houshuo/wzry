namespace IIPSMobile
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class VersionMgr : IIPSMobileVersionMgrInterface
    {
        private IntPtr mVersionMgr = IntPtr.Zero;

        public VersionMgr()
        {
            this.mVersionMgr = IntPtr.Zero;
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern void CancelUpdate(IntPtr versionMgr);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte CheckAppUpdate(IntPtr versionMgr);
        public void CreateCppVersionManager()
        {
            this.mVersionMgr = CreateVersionManager();
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern IntPtr CreateVersionManager();
        public void DeleteCppVersionManager()
        {
            if (this.mVersionMgr != IntPtr.Zero)
            {
                ReleaseVersionManager(this.mVersionMgr);
                this.mVersionMgr = IntPtr.Zero;
            }
        }

        ~VersionMgr()
        {
            this.mVersionMgr = IntPtr.Zero;
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint GetActionDownloadSpeed(IntPtr versionMgr);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern short GetCurDataVersion(IntPtr versionMgr);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern ulong GetMemorySize(IntPtr versionMgr);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint GetVersionMgrLastError(IntPtr versionMgr);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte InitVersionMgr(IntPtr versionMgr, IntPtr callback, uint bufferSize, IntPtr configBuffer);
        public bool InstallApk(string path)
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (class2 == null)
            {
                return false;
            }
            AndroidJavaObject @static = class2.GetStatic<AndroidJavaObject>("currentActivity");
            if (@static == null)
            {
                return false;
            }
            AndroidJavaObject obj3 = new AndroidJavaObject("cu_iipsmobile.CuIIPSMobile", new object[0]);
            if (obj3 == null)
            {
                return false;
            }
            object[] args = new object[] { path, @static };
            if (obj3.Call<int>("installAPK", args) != 0)
            {
                return false;
            }
            return true;
        }

        public void MgrCancelUpdate()
        {
            if (this.mVersionMgr != IntPtr.Zero)
            {
                CancelUpdate(this.mVersionMgr);
            }
        }

        public bool MgrCheckAppUpdate()
        {
            if (this.mVersionMgr == IntPtr.Zero)
            {
                return false;
            }
            return (CheckAppUpdate(this.mVersionMgr) > 0);
        }

        public uint MgrGetActionDownloadSpeed()
        {
            if (this.mVersionMgr == IntPtr.Zero)
            {
                return 0;
            }
            return GetActionDownloadSpeed(this.mVersionMgr);
        }

        public short MgrGetCurDataVersion()
        {
            if (this.mVersionMgr == IntPtr.Zero)
            {
                return 0;
            }
            return GetCurDataVersion(this.mVersionMgr);
        }

        public ulong MgrGetMemorySize()
        {
            if (this.mVersionMgr == IntPtr.Zero)
            {
                return 0L;
            }
            return GetMemorySize(this.mVersionMgr);
        }

        public uint MgrGetVersionMgrLastError()
        {
            if (this.mVersionMgr == IntPtr.Zero)
            {
                return 1;
            }
            return GetVersionMgrLastError(this.mVersionMgr);
        }

        public bool MgrInitVersionManager(IIPSMobileVersionCallBack callBack, uint bufferSize, byte[] configBuffer)
        {
            if (this.mVersionMgr == IntPtr.Zero)
            {
                return false;
            }
            GCHandle handle = GCHandle.Alloc(configBuffer, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();
            handle.Free();
            return (InitVersionMgr(this.mVersionMgr, callBack.mCallBack, bufferSize, ptr) > 0);
        }

        public bool MgrPoll()
        {
            if (this.mVersionMgr == IntPtr.Zero)
            {
                return false;
            }
            PoolVersionManager(this.mVersionMgr);
            return true;
        }

        public bool MgrSetNextStage(bool goonWork)
        {
            if (this.mVersionMgr == IntPtr.Zero)
            {
                return false;
            }
            byte num = 0;
            if (goonWork)
            {
                num = 1;
            }
            return (SetNextStage(this.mVersionMgr, num) > 0);
        }

        public bool MgrUnitVersionManager()
        {
            if (this.mVersionMgr == IntPtr.Zero)
            {
                return false;
            }
            return (UnitVersionMgr(this.mVersionMgr) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern void PoolVersionManager(IntPtr versionMgr);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern void ReleaseVersionManager(IntPtr versionMgr);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte SetNextStage(IntPtr versionMgr, byte goonWork);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte UnitVersionMgr(IntPtr versionMgr);
    }
}

