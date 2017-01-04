namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [MessageHandlerClass]
    public class TssdkSys : MonoSingleton<TssdkSys>
    {
        private bool bInit;
        private float m_LastTime;
        private string m_OpenID = string.Empty;

        public void CreateTssSDKSys(string openId, int nPlatform)
        {
            Debug.Log("TssdkSys CreateTssSDKSys, nplatf = " + nPlatform);
            if (openId != null)
            {
                if (nPlatform == 2)
                {
                    TssSdk.TssSdkSetUserInfo((TssSdk.EENTRYID) nPlatform, openId, ApolloConfig.WXAppID);
                }
                else
                {
                    TssSdk.TssSdkSetUserInfo((TssSdk.EENTRYID) nPlatform, openId, ApolloConfig.QQAppID);
                }
            }
        }

        protected override void Init()
        {
            base.Init();
            try
            {
                TssSdk.TssSdkInit(0xa11);
                this.bInit = true;
                this.m_LastTime = Time.time;
                Debug.Log("TssdkSys init");
            }
            catch (Exception)
            {
                this.bInit = false;
            }
        }

        private void InitTssSdk()
        {
            int nPlatform = 1;
            if (ApolloConfig.platform == ApolloPlatform.Wechat)
            {
                nPlatform = 2;
            }
            ApolloAccountInfo accountInfo = new ApolloAccountInfo();
            IApollo.Instance.GetAccountService().GetRecord(ref accountInfo);
            this.m_OpenID = accountInfo.OpenId;
            this.CreateTssSDKSys(this.m_OpenID, nPlatform);
        }

        [MessageHandler(0xbb9)]
        public static void On_GetAntiData(CSPkg msg)
        {
            TssSdk.TssSdkRcvAntiData(msg.stPkgData.stAntiDataSyn.szBuff, msg.stPkgData.stAntiDataSyn.wLen);
        }

        public void OnAccountLogin()
        {
            this.InitTssSdk();
            Debug.Log("TssdkSys OnAccountLogin");
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                TssSdk.TssSdkSetGameStatus(TssSdk.EGAMESTATUS.GAME_STATUS_BACKEND);
            }
            else
            {
                TssSdk.TssSdkSetGameStatus(TssSdk.EGAMESTATUS.GAME_STATUS_FRONTEND);
            }
            Singleton<BeaconHelper>.GetInstance().Event_ApplicationPause(pause);
        }

        public void Update()
        {
            if (this.bInit && ((Time.time - this.m_LastTime) > 20f))
            {
                IntPtr ptr = TssSdk.tss_get_report_data();
                if (ptr != IntPtr.Zero)
                {
                    TssSdk.AntiDataInfo info = new TssSdk.AntiDataInfo();
                    if (TssSdk.Is64bit())
                    {
                        short num = Marshal.ReadInt16(ptr, 0);
                        long num2 = Marshal.ReadInt64(ptr, 2);
                        info.anti_data_len = (ushort) num;
                        info.anti_data = new IntPtr(num2);
                    }
                    else if (TssSdk.Is32bit())
                    {
                        short num3 = Marshal.ReadInt16(ptr, 0);
                        int num4 = Marshal.ReadInt32(ptr, 2);
                        info.anti_data_len = (ushort) num3;
                        info.anti_data = new IntPtr(num4);
                    }
                    if (info.anti_data != IntPtr.Zero)
                    {
                        CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xbb8);
                        msg.stPkgData.stAntiDataReq.wLen = info.anti_data_len;
                        Marshal.Copy(info.anti_data, msg.stPkgData.stAntiDataReq.szBuff, 0, info.anti_data_len);
                        Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                    }
                    TssSdk.tss_del_report_data(ptr);
                }
                this.m_LastTime = Time.time;
            }
        }
    }
}

