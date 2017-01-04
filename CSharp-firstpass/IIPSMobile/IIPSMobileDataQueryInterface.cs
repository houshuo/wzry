namespace IIPSMobile
{
    using System;

    public interface IIPSMobileDataQueryInterface
    {
        uint GetFileId(string fileName);
        string GetFileName(uint fileId);
        uint GetFileSize(uint fileId);
        uint GetIfsPackagesInfo(ref DataQuery.IIPS_PACKAGE_INFO pInfo, uint count);
        uint GetLastDataQueryError();
        bool IIPSFindClose(uint findHandle);
        uint IIPSFindFirstFile(uint fileId, ref DataQuery.IIPS_FIND_FILE_INFO pInfo);
        bool IIPSFindNextFile(uint findHandle, ref DataQuery.IIPS_FIND_FILE_INFO pInfo);
        bool IsDirectory(uint fileId);
        bool IsFileReady(uint fileId);
    }
}

