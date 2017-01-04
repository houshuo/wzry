namespace Assets.Scripts.GameSystem
{
    using IIPSMobile;
    using System;
    using System.Runtime.CompilerServices;

    public class CVersionUpdateCallback : IIPSMobileVersionCallBackInterface
    {
        public OnActionMsgDelegate m_onActionMsgDelegate;
        public OnErrorDelegate m_onErrorDelegate;
        public OnGetNewVersionInfoDelegate m_onGetNewVersionInfoDelegate;
        public OnNoticeInstallApkDelegate m_onNoticeInstallApkDelegate;
        public OnProgressDelegate m_onProgressDelegate;
        public OnSuccessDelegate m_onSuccessDelegate;

        public byte OnActionMsg(string msg)
        {
            if (this.m_onActionMsgDelegate != null)
            {
                this.m_onActionMsgDelegate(msg);
            }
            return 1;
        }

        public void OnError(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode)
        {
            if (this.m_onErrorDelegate != null)
            {
                this.m_onErrorDelegate(curVersionStage, errorCode);
            }
        }

        public byte OnGetNewVersionInfo(IIPSMobileVersionCallBack.VERSIONINFO newVersionInfo)
        {
            if (this.m_onGetNewVersionInfoDelegate != null)
            {
                this.m_onGetNewVersionInfoDelegate(newVersionInfo);
            }
            return 1;
        }

        public byte OnNoticeInstallApk(string path)
        {
            if (this.m_onNoticeInstallApkDelegate != null)
            {
                this.m_onNoticeInstallApkDelegate(path);
            }
            return 1;
        }

        public void OnProgress(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize)
        {
            if (this.m_onProgressDelegate != null)
            {
                this.m_onProgressDelegate(curVersionStage, totalSize, nowSize);
            }
        }

        public void OnSuccess()
        {
            if (this.m_onSuccessDelegate != null)
            {
                this.m_onSuccessDelegate();
            }
        }

        public void SaveConfig(uint bufferSize, IntPtr configBuffer)
        {
        }

        public delegate byte OnActionMsgDelegate(string msg);

        public delegate void OnErrorDelegate(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode);

        public delegate byte OnGetNewVersionInfoDelegate(IIPSMobileVersionCallBack.VERSIONINFO newVersionInfo);

        public delegate byte OnNoticeInstallApkDelegate(string path);

        public delegate void OnProgressDelegate(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize);

        public delegate void OnSuccessDelegate();
    }
}

