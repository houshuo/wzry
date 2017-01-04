namespace Assets.Scripts.Framework
{
    using Apollo;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Runtime.CompilerServices;
    using tsf4g_tdr_csharp;

    public class LobbyConnector : BaseConnector
    {
        private List<CSPkg> confirmSendQueue = new List<CSPkg>();
        public uint curCltPkgSeq;
        public uint curSvrPkgSeq;
        public DelegateGetTryReconnect GetTryReconnect;
        public ApolloResult lastResult;
        private List<CSPkg> lobbySendQueue = new List<CSPkg>();
        private static int nBuffSize = 0x32000;
        private ReconnectPolicy reconPolicy = new ReconnectPolicy();
        private byte[] szSendBuffer = new byte[0x32000];

        public event NetConnectedEvent ConnectedEvent;

        public event NetDisconnectEvent DisconnectEvent;

        public bool CanSendPing()
        {
            return ((base.connected && (this.lobbySendQueue.Count == 0)) && (this.curSvrPkgSeq > 0));
        }

        public void CleanUp()
        {
            this.lobbySendQueue.Clear();
            this.confirmSendQueue.Clear();
            this.reconPolicy.StopPolicy();
            this.szSendBuffer.Initialize();
            this.curSvrPkgSeq = 0;
            this.curCltPkgSeq = 0;
        }

        protected override void DealConnectClose(ApolloResult result)
        {
            if (this.DisconnectEvent != null)
            {
                this.DisconnectEvent(this);
            }
        }

        protected override void DealConnectError(ApolloResult result)
        {
            this.lastResult = result;
            this.reconPolicy.StartPolicy(result, 10);
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                new KeyValuePair<string, string>("openid", "NULL"),
                new KeyValuePair<string, string>("status", "0"),
                new KeyValuePair<string, string>("type", "platform"),
                new KeyValuePair<string, string>("errorCode", result.ToString())
            };
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", events, true);
        }

        protected override void DealConnectFail(ApolloResult result, ApolloLoginInfo loginInfo)
        {
            this.lastResult = result;
            if ((result == ApolloResult.StayInQueue) || (result == ApolloResult.SvrIsFull))
            {
                MonoSingleton<TdirMgr>.GetInstance().ConnectLobby();
            }
            else
            {
                this.reconPolicy.StartPolicy(result, 10);
            }
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                new KeyValuePair<string, string>("openid", "NULL"),
                new KeyValuePair<string, string>("status", "0"),
                new KeyValuePair<string, string>("type", "platform"),
                new KeyValuePair<string, string>("errorCode", result.ToString())
            };
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", events, true);
        }

        protected override void DealConnectSucc()
        {
            if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
            {
                LookUpDNSOfServerTongcai(ApolloConfig.loginOnlyIpOrUrl, 0);
            }
            this.reconPolicy.StopPolicy();
            Singleton<ReconnectIpSelect>.instance.SetLobbySuccessUrl(base.initParam.ip);
            if (this.ConnectedEvent != null)
            {
                this.ConnectedEvent(this);
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            MonoSingleton<TssdkSys>.GetInstance().OnAccountLogin();
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                new KeyValuePair<string, string>("openid", "NULL"),
                new KeyValuePair<string, string>("status", "0"),
                new KeyValuePair<string, string>("type", "platform"),
                new KeyValuePair<string, string>("errorCode", "SUCC")
            };
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", events, true);
        }

        public void Disconnect()
        {
            base.DestroyConnector();
            this.reconPolicy.StopPolicy();
            this.reconPolicy.SetConnector(null, null, 0);
        }

        private static void DNSAsyncCallbackTongcai(IAsyncResult ar)
        {
            int asyncState = (int) ar.AsyncState;
            IPAddress[] addressArray = Dns.EndGetHostAddresses(ar);
            if ((addressArray != null) && (addressArray.Length != 0))
            {
                ApolloConfig.loginOnlyIpTongCai = addressArray[0].ToString();
            }
        }

        ~LobbyConnector()
        {
            base.DestroyConnector();
            this.reconPolicy = null;
        }

        public void HandleSending()
        {
            if (base.connected)
            {
                int index = 0;
                while (base.connected && (index < this.lobbySendQueue.Count))
                {
                    CSPkg msg = this.lobbySendQueue[index];
                    if (this.SendPackage(msg))
                    {
                        this.confirmSendQueue.Add(msg);
                        this.lobbySendQueue.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
            else
            {
                this.reconPolicy.UpdatePolicy(false);
            }
        }

        public bool Init(ConnectorParam para)
        {
            this.reconPolicy.SetConnector(this, new tryReconnectDelegate(this.onTryReconnect), 8);
            return base.CreateConnector(para);
        }

        private static void LookUpDNSOfServerTongcai(string host, int port)
        {
            ApolloConfig.loginOnlyIpTongCai = string.Empty;
            try
            {
                Dns.BeginGetHostAddresses(host, new AsyncCallback(LobbyConnector.DNSAsyncCallbackTongcai), port);
            }
            catch (Exception)
            {
            }
        }

        private uint onTryReconnect(uint nCount, uint nMax)
        {
            ListView<CSPkg> view = new ListView<CSPkg>();
            for (int i = 0; i < this.lobbySendQueue.Count; i++)
            {
                view.Add(this.lobbySendQueue[i]);
            }
            this.lobbySendQueue.Clear();
            for (int j = 0; j < this.confirmSendQueue.Count; j++)
            {
                this.lobbySendQueue.Add(this.confirmSendQueue[j]);
            }
            this.confirmSendQueue.Clear();
            for (int k = 0; k < view.Count; k++)
            {
                this.lobbySendQueue.Add(view[k]);
            }
            NetworkModule instance = Singleton<NetworkModule>.GetInstance();
            instance.m_lobbyReconnetCount++;
            if (this.GetTryReconnect != null)
            {
                return this.GetTryReconnect(nCount, nMax);
            }
            return 0;
        }

        public void PostRecvPackage(CSPkg msg)
        {
            if ((msg != null) && (msg.stPkgHead.dwReserve <= this.curCltPkgSeq))
            {
                int index = 0;
                while (index < this.confirmSendQueue.Count)
                {
                    CSPkg pkg = this.confirmSendQueue[index];
                    if ((pkg.stPkgHead.dwReserve > 0) && (pkg.stPkgHead.dwReserve <= msg.stPkgHead.dwReserve))
                    {
                        this.confirmSendQueue.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }

        public void PushSendMsg(CSPkg msg)
        {
            this.curCltPkgSeq++;
            msg.stPkgHead.dwReserve = this.curCltPkgSeq;
            this.lobbySendQueue.Add(msg);
        }

        public CSPkg RecvPackage()
        {
            byte[] buffer;
            int num;
            if ((base.connected && (base.connector != null)) && (base.connector.ReadData(out buffer, out num) == ApolloResult.Success))
            {
                int usedSize = 0;
                CSPkg pkg = CSPkg.New();
                TdrError.ErrorType type = pkg.unpack(ref buffer, num, ref usedSize, 0);
                if ((type == TdrError.ErrorType.TDR_NO_ERROR) && (usedSize > 0))
                {
                    if (pkg.stPkgHead.dwMsgID == 0x3f6)
                    {
                        this.curSvrPkgSeq = 0;
                    }
                    if (pkg.stPkgHead.dwSvrPkgSeq > this.curSvrPkgSeq)
                    {
                        if (pkg.stPkgHead.dwMsgID != 0x410)
                        {
                            this.curSvrPkgSeq = pkg.stPkgHead.dwSvrPkgSeq;
                        }
                        return pkg;
                    }
                }
                else
                {
                    object[] inParameters = new object[] { type };
                    DebugHelper.Assert(false, "TDR Unpack lobbyMsg Error -- {0}", inParameters);
                }
            }
            return null;
        }

        public bool RedirectNewPort(ushort nPort)
        {
            base.initParam.SetVPort(nPort);
            this.reconPolicy.SetConnector(this, new tryReconnectDelegate(this.onTryReconnect), 8);
            return base.CreateConnector(base.initParam);
        }

        public void ResetSending(bool bResetSeq)
        {
            this.lobbySendQueue.Clear();
            this.confirmSendQueue.Clear();
            this.szSendBuffer.Initialize();
            if (bResetSeq)
            {
                this.curCltPkgSeq = 0;
            }
        }

        private bool SendPackage(CSPkg msg)
        {
            if (base.connected && (base.connector != null))
            {
                msg.stPkgHead.dwSvrPkgSeq = this.curSvrPkgSeq;
                int usedSize = 0;
                if (msg.pack(ref this.szSendBuffer, nBuffSize, ref usedSize, 0) == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    byte[] destinationArray = new byte[usedSize];
                    Array.Copy(this.szSendBuffer, destinationArray, usedSize);
                    return (base.connector.WriteData(destinationArray, -1) == ApolloResult.Success);
                }
            }
            return false;
        }

        public delegate uint DelegateGetTryReconnect(uint curConnectTime, uint maxCount);
    }
}

