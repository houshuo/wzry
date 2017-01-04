namespace IIPSMobile
{
    using System;

    public interface IIPSMobileVersionMgrInterface
    {
        bool InstallApk(string path);
        void MgrCancelUpdate();
        bool MgrCheckAppUpdate();
        uint MgrGetActionDownloadSpeed();
        short MgrGetCurDataVersion();
        ulong MgrGetMemorySize();
        uint MgrGetVersionMgrLastError();
        bool MgrPoll();
        bool MgrSetNextStage(bool goonWork);
        bool MgrUnitVersionManager();
    }
}

