namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;

    [MessageHandlerClass]
    public class CLicenseInfo
    {
        public ListView<CLicenseItem> m_licenseList = new ListView<CLicenseItem>();

        public bool CheckGetLicense(uint cfgId)
        {
            ResLicenseInfo dataByKey = GameDataMgr.licenseDatabin.GetDataByKey(cfgId);
            bool flag = false;
            if (dataByKey != null)
            {
                for (int i = 0; i < dataByKey.UnlockArray.Length; i++)
                {
                    if (dataByKey.UnlockArray[i] > 0)
                    {
                        if (dataByKey.bIsAnd > 0)
                        {
                            flag &= Singleton<CFunctionUnlockSys>.GetInstance().CheckUnlock(dataByKey.UnlockArray[i]);
                        }
                        else
                        {
                            flag |= Singleton<CFunctionUnlockSys>.GetInstance().CheckUnlock(dataByKey.UnlockArray[i]);
                        }
                    }
                }
            }
            return flag;
        }

        public CLicenseItem GetLicenseItemByIndex(int index)
        {
            if ((index >= 0) && (index < this.m_licenseList.Count))
            {
                return this.m_licenseList[index];
            }
            return null;
        }

        public void InitLicenseCfgInfo()
        {
            this.m_licenseList.Clear();
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.licenseDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResLicenseInfo info = (ResLicenseInfo) current.Value;
                CLicenseItem item = new CLicenseItem(info.dwLicenseID);
                this.m_licenseList.Add(item);
            }
        }

        [MessageHandler(0x547)]
        public static void ReceiveLicenseGetRsp(CSPkg msg)
        {
            SCPKG_CMD_LICENSE_RSP stLicenseGetRsp = msg.stPkgData.stLicenseGetRsp;
            switch (stLicenseGetRsp.iResult)
            {
                case 0:
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        masterRoleInfo.m_licenseInfo.SetLicenseItemData(stLicenseGetRsp.dwLicenseID, stLicenseGetRsp.dwLicenseTime);
                    }
                    break;
                }
            }
        }

        public static void ReqGetLicense(uint licenseId)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x546);
            msg.stPkgData.stLicenseGetReq.dwLicenseID = licenseId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void ReviewLicenseList()
        {
            for (int i = 0; i < this.m_licenseList.Count; i++)
            {
                if ((this.m_licenseList[i].m_getSecond == 0) && this.CheckGetLicense(this.m_licenseList[i].m_licenseId))
                {
                    ReqGetLicense(this.m_licenseList[i].m_licenseId);
                }
            }
        }

        public void SetLicenseItemData(uint licenseId, uint getSec)
        {
            for (int i = 0; i < this.m_licenseList.Count; i++)
            {
                if (licenseId == this.m_licenseList[i].m_licenseId)
                {
                    this.m_licenseList[i].m_getSecond = getSec;
                    return;
                }
            }
        }

        public void SetSvrLicenseData(COMDT_ACNT_LICENSE svrLicenseData)
        {
            if (this.m_licenseList.Count == 0)
            {
                this.InitLicenseCfgInfo();
            }
            for (int i = 0; i < svrLicenseData.bLicenseCnt; i++)
            {
                this.SetLicenseItemData(svrLicenseData.astLicenseList[i].dwLicenseID, svrLicenseData.astLicenseList[i].dwGetSecond);
            }
        }
    }
}

