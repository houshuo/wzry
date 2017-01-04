namespace IIPSMobile
{
    using System;
    using System.Runtime.InteropServices;

    public class IIPSMobileDataManager : IIPSMobileDataMgrInterface
    {
        protected IntPtr mDataManager = IntPtr.Zero;

        public IIPSMobileDataManager()
        {
            this.mDataManager = CreateDataManager();
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern IntPtr CreateDataManager();
        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte DataMgrPollCallback(IntPtr dataManager);
        ~IIPSMobileDataManager()
        {
            if (this.mDataManager != IntPtr.Zero)
            {
                ReleaseDataManager(this.mDataManager);
                this.mDataManager = IntPtr.Zero;
            }
        }

        public IIPSMobileDownloaderInterface GetDataDownloader(bool openProgressCallBack = false)
        {
            if (this.mDataManager == IntPtr.Zero)
            {
                return null;
            }
            byte num = 0;
            if (openProgressCallBack)
            {
                num = 1;
            }
            return new DataDownloader(GetDataDownloader(this.mDataManager, num));
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern IntPtr GetDataDownloader(IntPtr dataManager, byte openProgressCallBack);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern ulong GetDataMgrMemorySize(IntPtr dataManager);
        public IIPSMobileDataQueryInterface GetDataQuery()
        {
            if (this.mDataManager == IntPtr.Zero)
            {
                return null;
            }
            return new DataQuery(GetDataQuery(this.mDataManager));
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern IntPtr GetDataQuery(IntPtr dataManager);
        public IIPSMobileDataReaderInterface GetDataReader()
        {
            if (this.mDataManager == IntPtr.Zero)
            {
                return null;
            }
            return new DataReader(GetDataReader(this.mDataManager));
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern IntPtr GetDataReader(IntPtr dataManager);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint GetLastDataMgrError(IntPtr dataManager);
        public bool Init(uint bufferSize, byte[] configBuffer)
        {
            if (this.mDataManager == IntPtr.Zero)
            {
                return false;
            }
            GCHandle handle = GCHandle.Alloc(configBuffer, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();
            handle.Free();
            return (InitDataManager(this.mDataManager, bufferSize, ptr) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte InitDataManager(IntPtr dataManager, uint bufferSize, IntPtr configBuffer);
        public uint MgrGetDataMgrLastError()
        {
            if (this.mDataManager == IntPtr.Zero)
            {
                return 0;
            }
            return GetLastDataMgrError(this.mDataManager);
        }

        public ulong MgrGetMemorySize()
        {
            if (this.mDataManager == IntPtr.Zero)
            {
                return 0L;
            }
            return GetDataMgrMemorySize(this.mDataManager);
        }

        public bool PollCallBack()
        {
            if (this.mDataManager == IntPtr.Zero)
            {
                return false;
            }
            return (DataMgrPollCallback(this.mDataManager) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern void ReleaseDataManager(IntPtr dataManager);
        public bool Uninit()
        {
            if (this.mDataManager == IntPtr.Zero)
            {
                return false;
            }
            return (UnitDataManager(this.mDataManager) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte UnitDataManager(IntPtr dataManager);
    }
}

