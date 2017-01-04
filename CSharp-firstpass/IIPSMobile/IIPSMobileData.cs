namespace IIPSMobile
{
    using System;
    using System.Text;

    public class IIPSMobileData
    {
        public IIPSMobileDataMgrInterface CreateDataMgr(string config)
        {
            IIPSMobileDataManager manager = new IIPSMobileDataManager();
            if (!manager.Init((uint) config.Length, Encoding.ASCII.GetBytes(config)))
            {
                return null;
            }
            return manager;
        }
    }
}

