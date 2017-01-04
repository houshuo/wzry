namespace IIPSMobile
{
    using System;

    public interface IIPSMobileVersionCallBackInterface
    {
        byte OnActionMsg(string msg);
        void OnError(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode);
        byte OnGetNewVersionInfo(IIPSMobileVersionCallBack.VERSIONINFO newVersionInfo);
        byte OnNoticeInstallApk(string path);
        void OnProgress(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize);
        void OnSuccess();
        void SaveConfig(uint bufferSize, IntPtr configBuffer);
    }
}

