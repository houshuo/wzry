namespace IIPSMobile
{
    using System;
    using System.Runtime.InteropServices;

    public interface IIPSMobileDataMgrInterface
    {
        IIPSMobileDownloaderInterface GetDataDownloader(bool openProgressCallBack = false);
        IIPSMobileDataQueryInterface GetDataQuery();
        IIPSMobileDataReaderInterface GetDataReader();
        uint MgrGetDataMgrLastError();
        ulong MgrGetMemorySize();
        bool PollCallBack();
        bool Uninit();
    }
}

