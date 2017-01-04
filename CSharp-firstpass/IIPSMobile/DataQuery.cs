namespace IIPSMobile
{
    using System;
    using System.Runtime.InteropServices;

    public class DataQuery : IIPSMobileDataQueryInterface
    {
        public IntPtr mDataQuery = IntPtr.Zero;

        public DataQuery(IntPtr Query)
        {
            this.mDataQuery = Query;
        }

        public uint GetFileId(string fileName)
        {
            if (this.mDataQuery == IntPtr.Zero)
            {
                return 0;
            }
            return GetIFileId(this.mDataQuery, fileName);
        }

        public string GetFileName(uint fileId)
        {
            if (this.mDataQuery == IntPtr.Zero)
            {
                return null;
            }
            return Marshal.PtrToStringAnsi(GetIFileName(this.mDataQuery, fileId));
        }

        public uint GetFileSize(uint fileId)
        {
            if (this.mDataQuery == IntPtr.Zero)
            {
                return 0;
            }
            return GetIFileSize(this.mDataQuery, fileId);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint GetIFileId(IntPtr dataQuery, [MarshalAs(UnmanagedType.LPStr)] string szFileName);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern IntPtr GetIFileName(IntPtr dataQuery, uint fileId);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint GetIFileSize(IntPtr dataQuery, uint fileId);
        public uint GetIfsPackagesInfo(ref IIPS_PACKAGE_INFO pInfo, uint count)
        {
            if (this.mDataQuery == IntPtr.Zero)
            {
                return 0;
            }
            return GetIfsPackagesInfo(this.mDataQuery, ref pInfo, count);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint GetIfsPackagesInfo(IntPtr dataQuery, ref IIPS_PACKAGE_INFO pInfo, uint count);
        public uint GetLastDataQueryError()
        {
            if (this.mDataQuery == IntPtr.Zero)
            {
                return 0;
            }
            return GetLastDataQueryError(this.mDataQuery);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint GetLastDataQueryError(IntPtr dataQuery);
        public bool IIPSFindClose(uint findHandle)
        {
            if (this.mDataQuery == IntPtr.Zero)
            {
                return false;
            }
            return (IIPSFindClose(this.mDataQuery, findHandle) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte IIPSFindClose(IntPtr dataQuery, uint findHandle);
        public uint IIPSFindFirstFile(uint fileId, ref IIPS_FIND_FILE_INFO pInfo)
        {
            if (this.mDataQuery == IntPtr.Zero)
            {
                return 0;
            }
            return IIPSFindFirstFile(this.mDataQuery, fileId, ref pInfo);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern uint IIPSFindFirstFile(IntPtr dataQuery, uint fileId, ref IIPS_FIND_FILE_INFO pInfo);
        public bool IIPSFindNextFile(uint findHandle, ref IIPS_FIND_FILE_INFO pInfo)
        {
            if (this.mDataQuery == IntPtr.Zero)
            {
                return false;
            }
            return (IIPSFindNextFile(this.mDataQuery, findHandle, ref pInfo) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte IIPSFindNextFile(IntPtr dataQuery, uint findHandle, ref IIPS_FIND_FILE_INFO pInfo);
        public bool IsDirectory(uint fileId)
        {
            if (this.mDataQuery == IntPtr.Zero)
            {
                return false;
            }
            return (IsIFileDir(this.mDataQuery, fileId) > 0);
        }

        public bool IsFileReady(uint fileId)
        {
            if (this.mDataQuery == IntPtr.Zero)
            {
                return false;
            }
            return (IsIFileReady(this.mDataQuery, fileId) > 0);
        }

        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte IsIFileDir(IntPtr dataQuery, uint fileId);
        [DllImport("apollo", ExactSpelling=true)]
        private static extern byte IsIFileReady(IntPtr dataQuery, uint fileId);

        [StructLayout(LayoutKind.Sequential)]
        public struct IIPS_FIND_FILE_INFO
        {
            public uint fileId;
            public uint fileSize;
            public byte isDirectory;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IIPS_PACKAGE_INFO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=260)]
            public byte[] szPackageName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=260)]
            public byte[] szPackageFilePath;
            public ulong currentSize;
            public ulong totalSize;
        }
    }
}

