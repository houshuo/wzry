namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;

    public class CLicenseItem
    {
        public uint m_getSecond;
        public uint m_licenseId;
        public ResLicenseInfo m_resLicenseInfo;

        public CLicenseItem(uint cfgId)
        {
            this.m_licenseId = cfgId;
            this.m_resLicenseInfo = GameDataMgr.licenseDatabin.GetDataByKey(cfgId);
            if (this.m_resLicenseInfo != null)
            {
                this.m_getSecond = 0;
            }
        }
    }
}

