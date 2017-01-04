namespace IIPSMobile
{
    using System;
    using System.Text;

    public class IIPSMobileVersion
    {
        private IIPSMobileVersionCallBack mCallback;
        private uint mLastErr;
        private VersionMgr vMgr;

        public IIPSMobileVersionMgrInterface CreateVersionMgr(IIPSMobileVersionCallBackInterface CallbackImp, string config)
        {
            if (this.vMgr == null)
            {
                this.vMgr = new VersionMgr();
                this.vMgr.CreateCppVersionManager();
                this.mCallback = new IIPSMobileVersionCallBack(CallbackImp);
                this.mCallback.CreateCppVersionInfoCallBack();
                if (!this.vMgr.MgrInitVersionManager(this.mCallback, (uint) config.Length, Encoding.ASCII.GetBytes(config)))
                {
                    this.mLastErr = this.vMgr.MgrGetVersionMgrLastError();
                    return null;
                }
            }
            return this.vMgr;
        }

        public void DeleteVersionMgr()
        {
            if (this.vMgr != null)
            {
                this.vMgr.DeleteCppVersionManager();
                this.vMgr = null;
            }
            if (this.mCallback != null)
            {
                this.mCallback.DeleteCppVersionCallBack();
                this.mCallback = null;
            }
        }

        public uint GetLastErr()
        {
            return this.mLastErr;
        }
    }
}

