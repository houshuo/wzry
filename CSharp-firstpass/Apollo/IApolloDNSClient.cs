namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    public interface IApolloDNSClient
    {
        DNSErrCode GetErrorCode();
        string GetErrorString();
        DNSErrCode Init(bool callbackMode = false, int cacheTime = 120);
        void Poll(int timeout = 10);
        DNSErrCode Query(string domainName, ProcessQueryResult process, string OId = null, string SId = null, string version = null, string userData = null);
        DNSErrCode SetCurrentAPN(string APN);
        DNSErrCode SetEnableCache(bool enable = true);
        DNSErrCode SetEnableLog(bool enable = false);
        DNSErrCode SetFileSys(IApolloDNSFileSys fileSys);
    }
}

