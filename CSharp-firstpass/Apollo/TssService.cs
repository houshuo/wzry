namespace Apollo
{
    using apollo_tss;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class TssService : ApolloObject, IApolloServiceBase, ITssService
    {
        [CompilerGenerated]
        private static TalkerMessageWithoutReceiptHandler<ApolloTSS> <>f__am$cache5;
        private uint gameId;
        public static readonly TssService Instance = new TssService();
        private float intervalBetweenCollections;
        private ApolloTalker talker;
        private TssTransferBase transfer;

        private TssService() : base(true, true)
        {
            this.intervalBetweenCollections = 20f;
        }

        public void Intialize(uint gameId)
        {
            this.gameId = gameId;
            TssSdk.TssSdkInit(gameId);
        }

        public override void OnApplicationPause(bool pauseStatus)
        {
        }

        protected override void OnTimeOut()
        {
            if ((this.talker == null) && (this.transfer == null))
            {
                throw new Exception("Talker or Transfer must not be null !");
            }
            if (this.connected)
            {
                this.OnTssCollected();
            }
            this.ResetTimeInterval();
        }

        private void OnTssCollected()
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
                    long num4 = Marshal.ReadInt32(ptr, 2);
                    info.anti_data_len = (ushort) num3;
                    info.anti_data = new IntPtr(num4);
                }
                if (info.anti_data == IntPtr.Zero)
                {
                    ADebug.Log("OnTssCollected aniti data is null");
                }
                else
                {
                    ApolloTSS otss;
                    otss = new ApolloTSS {
                        wLen = info.anti_data_len,
                        szData = new byte[otss.wLen]
                    };
                    Marshal.Copy(info.anti_data, otss.szData, 0, otss.wLen);
                    ADebug.Log("begin send tss data len:" + otss.wLen);
                    if (this.talker != null)
                    {
                        this.talker.Send(TalkerCommand.CommandDomain.TSS, otss);
                    }
                    else if (this.transfer != null)
                    {
                        this.transfer.OnTssDataCollected(otss.szData);
                    }
                    TssSdk.tss_del_report_data(ptr);
                }
            }
            else
            {
                ADebug.Log("tss tss_get_report_data addr is null");
            }
        }

        public void ReportUserInfo()
        {
            this.ReportUserInfo(0, string.Empty);
        }

        public void ReportUserInfo(uint wordId, string roleId)
        {
            IApolloAccountService accountService = IApollo.Instance.GetAccountService();
            ApolloAccountInfo accountInfo = new ApolloAccountInfo();
            if (accountService.GetRecord(ref accountInfo) == ApolloResult.Success)
            {
                this.setUserInfo(accountInfo, wordId, roleId);
            }
        }

        private void ResetTimeInterval()
        {
            base.UpdateTimeLeft = this.intervalBetweenCollections;
        }

        internal void SetAntiData(byte[] data, int len = 0)
        {
            if (data != null)
            {
                if (len == 0)
                {
                    len = data.Length;
                }
                TssSdk.TssSdkRcvAntiData(data, (ushort) len);
            }
        }

        private void setUserInfo(ApolloAccountInfo accountInfo, uint worldId, string roleId)
        {
            if (accountInfo == null)
            {
                ADebug.Log("TssService account info is null");
            }
            else
            {
                TssSdk.EENTRYID entryId = TssSdk.EENTRYID.ENTRY_ID_OTHERS;
                string openId = accountInfo.OpenId;
                string appId = null;
                if (accountInfo != null)
                {
                    if (accountInfo.Platform == ApolloPlatform.Wechat)
                    {
                        entryId = TssSdk.EENTRYID.ENTRY_ID_MM;
                        appId = ApolloCommon.ApolloInfo.WXAppId;
                    }
                    else
                    {
                        entryId = TssSdk.EENTRYID.ENTRY_ID_QZONE;
                        appId = ApolloCommon.ApolloInfo.QQAppId;
                    }
                }
                TssSdk.TssSdkSetUserInfoEx(entryId, openId, appId, worldId, roleId);
            }
        }

        public void StartWithTalker(IApolloTalker talker, float intervalBetweenCollections = 2)
        {
            this.intervalBetweenCollections = intervalBetweenCollections;
            this.ResetTimeInterval();
            this.talker = talker as ApolloTalker;
            if (this.talker == null)
            {
                throw new Exception("Talker must not be null !");
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = delegate (ApolloTSS resp) {
                    if (resp != null)
                    {
                        ADebug.Log("tss recv data len:" + resp.wLen);
                        TssSdk.TssSdkRcvAntiData(resp.szData, resp.wLen);
                    }
                    else
                    {
                        ADebug.Log("Tss resp  is null");
                    }
                };
            }
            this.talker.Register<ApolloTSS>(TalkerCommand.CommandDomain.TSS, <>f__am$cache5);
        }

        public void StartWithTransfer(TssTransferBase transfer, float intervalBetweenCollections = 2)
        {
            this.intervalBetweenCollections = intervalBetweenCollections;
            this.ResetTimeInterval();
            this.transfer = transfer;
            if (this.transfer == null)
            {
                throw new Exception("Transfer must not be null !");
            }
            this.transfer.tssService = this;
        }

        private bool connected
        {
            get
            {
                if (this.talker != null)
                {
                    ApolloConnector connector = this.talker.connector as ApolloConnector;
                    if (connector != null)
                    {
                        return connector.Connected;
                    }
                }
                else if (this.transfer != null)
                {
                    return this.transfer.IsConnected();
                }
                return false;
            }
        }
    }
}

