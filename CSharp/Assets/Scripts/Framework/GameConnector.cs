namespace Assets.Scripts.Framework
{
    using Apollo;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using tsf4g_tdr_csharp;
    using UnityEngine;

    public class GameConnector : BaseConnector
    {
        private NetworkState changedNetState;
        private List<CSPkg> confirmSendQueue = new List<CSPkg>();
        public NetworkReachability curNetworkReachability;
        private List<CSPkg> gameMsgSendQueue = new List<CSPkg>();
        private int nBuffSize = 0x32000;
        private bool netStateChanged;
        private ReconnectPolicy reconPolicy = new ReconnectPolicy();
        private uint reportCount;
        private byte[] szSendBuffer = new byte[0x32000];

        public void CleanUp()
        {
            this.gameMsgSendQueue.Clear();
            this.confirmSendQueue.Clear();
            this.reconPolicy.StopPolicy();
            this.ClearBuffer();
        }

        private void ClearBuffer()
        {
            this.szSendBuffer.Initialize();
        }

        protected override void DealConnectClose(ApolloResult result)
        {
        }

        protected override void DealConnectError(ApolloResult result)
        {
            this.reconPolicy.StartPolicy(result, 6);
            MonoSingleton<Reconnection>.instance.QueryIsRelayGaming(result);
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                new KeyValuePair<string, string>("openid", "NULL"),
                new KeyValuePair<string, string>("status", "0"),
                new KeyValuePair<string, string>("type", "challenge"),
                new KeyValuePair<string, string>("errorCode", result.ToString())
            };
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", events, true);
        }

        protected override void DealConnectFail(ApolloResult result, ApolloLoginInfo loginInfo)
        {
            this.reconPolicy.StartPolicy(result, 6);
            MonoSingleton<Reconnection>.instance.QueryIsRelayGaming(result);
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                new KeyValuePair<string, string>("openid", "NULL"),
                new KeyValuePair<string, string>("status", "0"),
                new KeyValuePair<string, string>("type", "challenge"),
                new KeyValuePair<string, string>("errorCode", result.ToString())
            };
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", events, true);
        }

        protected override void DealConnectSucc()
        {
            this.reportCount = 0;
            this.reconPolicy.StopPolicy();
            Singleton<ReconnectIpSelect>.instance.SetRelaySuccessUrl(base.initParam.ip);
            MonoSingleton<Reconnection>.GetInstance().OnConnectSuccess();
            List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                new KeyValuePair<string, string>("openid", "NULL"),
                new KeyValuePair<string, string>("status", "0"),
                new KeyValuePair<string, string>("type", "challenge"),
                new KeyValuePair<string, string>("errorCode", "SUCC")
            };
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_SvrConnectFail", events, true);
        }

        public void Disconnect()
        {
            ApolloNetworkService.Intance.NetworkChangedEvent -= new Apollo.NetworkStateChanged(this.NetworkStateChanged);
            base.DestroyConnector();
            this.reconPolicy.StopPolicy();
            this.reconPolicy.SetConnector(null, null, 0);
            base.initParam = null;
        }

        ~GameConnector()
        {
            base.DestroyConnector();
            this.reconPolicy = null;
        }

        public void ForceReconnect()
        {
            this.reconPolicy.UpdatePolicy(true);
        }

        public void HandleMsg(CSPkg msg)
        {
            if ((msg.stPkgHead.dwMsgID == 0x433) || (msg.stPkgHead.dwMsgID == 0x435))
            {
                Singleton<GameReplayModule>.instance.CacheRecord(msg);
            }
            NetMsgDelegate msgHandler = Singleton<NetworkModule>.instance.GetMsgHandler(msg.stPkgHead.dwMsgID);
            if (msgHandler != null)
            {
                msgHandler(msg);
            }
        }

        public void HandleSending()
        {
            if (base.connected)
            {
                for (int i = 0; i < this.confirmSendQueue.Count; i++)
                {
                    CSPkg msg = this.confirmSendQueue[i];
                    if ((Singleton<GameLogic>.instance.GameRunningTick - msg.stPkgHead.dwSvrPkgSeq) > 0x1388)
                    {
                        this.SendPackage(msg);
                        msg.stPkgHead.dwSvrPkgSeq = Singleton<GameLogic>.instance.GameRunningTick;
                    }
                }
                while (base.connected && (this.gameMsgSendQueue.Count > 0))
                {
                    CSPkg pkg2 = this.gameMsgSendQueue[0];
                    if (!this.SendPackage(pkg2))
                    {
                        break;
                    }
                    if (pkg2.stPkgHead.dwReserve > 0)
                    {
                        pkg2.stPkgHead.dwSvrPkgSeq = Singleton<GameLogic>.instance.GameRunningTick;
                        this.confirmSendQueue.Add(pkg2);
                    }
                    else
                    {
                        pkg2.Release();
                    }
                    this.gameMsgSendQueue.RemoveAt(0);
                }
            }
            else
            {
                MonoSingleton<Reconnection>.instance.UpdateReconnect();
            }
        }

        public bool Init(ConnectorParam para)
        {
            this.reconPolicy.SetConnector(this, new tryReconnectDelegate(this.onTryReconnect), 8);
            ApolloNetworkService.Intance.NetworkChangedEvent -= new Apollo.NetworkStateChanged(this.NetworkStateChanged);
            ApolloNetworkService.Intance.NetworkChangedEvent += new Apollo.NetworkStateChanged(this.NetworkStateChanged);
            this.curNetworkReachability = Application.internetReachability;
            return base.CreateConnector(para);
        }

        private void NetworkStateChanged(NetworkState state)
        {
            this.changedNetState = state;
            this.netStateChanged = true;
        }

        private uint onTryReconnect(uint nCount, uint nMax)
        {
            if ((this.curNetworkReachability != Application.internetReachability) && MonoSingleton<CTongCaiSys>.instance.isTongCaiValid)
            {
                string connectUrl = Singleton<ReconnectIpSelect>.instance.GetConnectUrl(ConnectorType.Relay, nCount);
                base.initParam.SetVip(connectUrl);
                this.curNetworkReachability = Application.internetReachability;
            }
            if (nCount >= 2)
            {
                try
                {
                    MonoSingleton<Reconnection>.GetInstance().ShowReconnectMsgAlert(((int) nCount) - 1, ((int) nMax) - 1);
                }
                catch (Exception exception)
                {
                    object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                    DebugHelper.Assert(false, "Exception In GameConnector Try Reconnect, {0} {1}", inParameters);
                }
            }
            if (nCount == 2)
            {
                List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                    new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                    new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                    new KeyValuePair<string, string>("ping", Singleton<FrameSynchr>.instance.GameSvrPing.ToString())
                };
                if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                {
                    events.Add(new KeyValuePair<string, string>("Network", "3G or 4G"));
                }
                else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    events.Add(new KeyValuePair<string, string>("Network", "WIFI"));
                }
                else
                {
                    events.Add(new KeyValuePair<string, string>("Network", "NoSignal"));
                }
                events.Add(new KeyValuePair<string, string>("FrameNum", Singleton<FrameSynchr>.instance.CurFrameNum.ToString()));
                events.Add(new KeyValuePair<string, string>("IsFighting", Singleton<BattleLogic>.instance.isFighting.ToString()));
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("GameConnector.onTryReconnect", events, true);
            }
            NetworkModule instance = Singleton<NetworkModule>.GetInstance();
            instance.m_GameReconnetCount++;
            return nCount;
        }

        public void PushSendMsg(CSPkg msg)
        {
            this.gameMsgSendQueue.Add(msg);
        }

        public CSPkg RecvPackage()
        {
            if (base.connected && (base.connector != null))
            {
                byte[] buffer;
                int num;
                if (base.connector.ReadUdpData(out buffer, out num) == ApolloResult.Success)
                {
                    int usedSize = 0;
                    CSPkg pkg = CSPkg.New();
                    TdrError.ErrorType type = pkg.unpack(ref buffer, num, ref usedSize, 0);
                    if ((type == TdrError.ErrorType.TDR_NO_ERROR) && (usedSize > 0))
                    {
                        return pkg;
                    }
                    object[] objArray1 = new object[] { "UnpackUdpError: BufLen:", num, " TdrError:", type, " nParseSize:", usedSize };
                    this.ReportException(new UnpackError(string.Concat(objArray1)));
                }
                if (base.connector.ReadData(out buffer, out num) == ApolloResult.Success)
                {
                    int num3 = 0;
                    CSPkg pkg2 = CSPkg.New();
                    TdrError.ErrorType type2 = pkg2.unpack(ref buffer, num, ref num3, 0);
                    if ((type2 == TdrError.ErrorType.TDR_NO_ERROR) && (num3 > 0))
                    {
                        int index = 0;
                        while (index < this.confirmSendQueue.Count)
                        {
                            CSPkg pkg3 = this.confirmSendQueue[index];
                            if ((pkg3.stPkgHead.dwReserve > 0) && (pkg3.stPkgHead.dwReserve == pkg2.stPkgHead.dwMsgID))
                            {
                                pkg3.Release();
                                this.confirmSendQueue.RemoveAt(index);
                            }
                            else
                            {
                                index++;
                            }
                        }
                        return pkg2;
                    }
                    object[] objArray2 = new object[] { "UnpackError: BufLen:", num, " TdrError:", type2, " nParseSize:", num3 };
                    this.ReportException(new UnpackError(string.Concat(objArray2)));
                }
            }
            return null;
        }

        private void ReportException(Exception ecp)
        {
            if (++this.reportCount <= 15)
            {
                BuglyAgent.ReportException(ecp, ecp.Message);
            }
        }

        public bool SendPackage(CSPkg msg)
        {
            if (base.connected && (base.connector != null))
            {
                int usedSize = 0;
                TdrError.ErrorType type = msg.pack(ref this.szSendBuffer, this.nBuffSize, ref usedSize, 0);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    ApolloResult result;
                    byte[] destinationArray = new byte[usedSize];
                    Array.Copy(this.szSendBuffer, destinationArray, usedSize);
                    if (base.initParam.bIsUDP && ((msg.stPkgHead.dwMsgID == 0x3ed) || (msg.stPkgHead.dwMsgID == 0x4ec)))
                    {
                        result = base.connector.WriteUdpData(destinationArray, -1);
                    }
                    else
                    {
                        result = base.connector.WriteData(destinationArray, -1);
                    }
                    if (result == ApolloResult.Success)
                    {
                        return true;
                    }
                    if (msg.stPkgHead.dwMsgID == 0x438)
                    {
                        object[] objArray1 = new object[] { "WriteError: MsgID:", msg.stPkgHead.dwMsgID, " PackSize:", usedSize, " ApolloResult:", result };
                        this.ReportException(new WriteError(string.Concat(objArray1)));
                    }
                }
                else
                {
                    object[] objArray2 = new object[] { "PackError: MsgID:", msg.stPkgHead.dwMsgID, " TdrError:", type };
                    this.ReportException(new PackError(string.Concat(objArray2)));
                }
            }
            return false;
        }

        public void Update()
        {
            this.reconPolicy.UpdatePolicy(false);
            if (this.netStateChanged)
            {
                if (this.changedNetState == NetworkState.NotReachable)
                {
                    Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(Singleton<CTextManager>.GetInstance().GetText("NetworkConnecting"), 10, enUIEventID.None);
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                }
                this.netStateChanged = false;
            }
        }

        private class PackError : Exception
        {
            public PackError(string msg) : base(msg)
            {
            }
        }

        private class ReadError : Exception
        {
            public ReadError(string msg) : base(msg)
            {
            }
        }

        private class UnpackError : Exception
        {
            public UnpackError(string msg) : base(msg)
            {
            }
        }

        private class WriteError : Exception
        {
            public WriteError(string msg) : base(msg)
            {
            }
        }
    }
}

