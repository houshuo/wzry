namespace Assets.Scripts.Framework
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class NetworkModule : Singleton<NetworkModule>
    {
        private bool _bOnlineMode = true;
        private float _preGameHeartTime;
        private float _preLobbyHeartTime;
        public GameConnector gameSvr = new GameConnector();
        public LobbyConnector lobbySvr = new LobbyConnector();
        public int m_GameReconnetCount;
        public int m_lobbyReconnetCount;
        private uint m_uiRecvGameMsgCount;
        private Dictionary<uint, NetMsgDelegate> mNetMsgHandlers = new Dictionary<uint, NetMsgDelegate>();

        public void CloseAllServerConnect()
        {
            this.CloseLobbyServerConnect();
            this.CloseGameServerConnect(true);
        }

        public void CloseGameServerConnect(bool switchLocal = true)
        {
            if (switchLocal)
            {
                Singleton<FrameSynchr>.instance.SwitchSynchrLocal();
            }
            MonoSingleton<Reconnection>.instance.ResetRelaySyncCache();
            Singleton<FrameWindow>.GetInstance().Reset();
            this.gameSvr.CleanUp();
            this.gameSvr.Disconnect();
            this.m_GameReconnetCount = 0;
        }

        public void CloseLobbyServerConnect()
        {
            this.lobbySvr.CleanUp();
            this.lobbySvr.Disconnect();
            this.lobbyPing = 0;
            this.m_lobbyReconnetCount = 0;
        }

        public static CSPkg CreateDefaultCSPKG(uint msgID)
        {
            CSPkg pkg = CSPkg.New();
            pkg.stPkgHead.dwMsgID = msgID;
            pkg.stPkgHead.iVersion = MetaLib.getVersion();
            pkg.stPkgData.construct((long) msgID);
            return pkg;
        }

        private static void DNSAsyncCallback(IAsyncResult ar)
        {
            int asyncState = (int) ar.AsyncState;
            IPAddress[] addressArray = Dns.EndGetHostAddresses(ar);
            if ((addressArray != null) && (addressArray.Length != 0))
            {
                ApolloConfig.loginOnlyIp = addressArray[0].ToString();
            }
        }

        public NetMsgDelegate GetMsgHandler(uint msgId)
        {
            NetMsgDelegate delegate2;
            this.mNetMsgHandlers.TryGetValue(msgId, out delegate2);
            return delegate2;
        }

        private void HandleGameMsgRecv()
        {
            if (this.gameSvr != null)
            {
                CSPkg msg = null;
                while ((msg = this.gameSvr.RecvPackage()) != null)
                {
                    this.m_uiRecvGameMsgCount++;
                    if (!MonoSingleton<Reconnection>.GetInstance().FilterRelaySvrPackage(msg))
                    {
                        this.gameSvr.HandleMsg(msg);
                    }
                }
            }
        }

        public void HandleGameMsgSend()
        {
            if (this.isOnlineMode)
            {
                this.gameSvr.HandleSending();
            }
        }

        private void HandleLobbyMsgRecv()
        {
            if (this.lobbySvr != null)
            {
                for (CSPkg pkg = this.lobbySvr.RecvPackage(); pkg != null; pkg = this.lobbySvr.RecvPackage())
                {
                    NetMsgDelegate delegate2 = null;
                    if (this.mNetMsgHandlers.TryGetValue(pkg.stPkgHead.dwMsgID, out delegate2))
                    {
                        delegate2(pkg);
                    }
                    else if (pkg.stPkgHead.dwMsgID == 0x4ed)
                    {
                        uint num = ((uint) (Time.realtimeSinceStartup * 1000f)) - pkg.stPkgData.stGameSvrPing.dwTime;
                        this.lobbyPing = (num + this.lobbyPing) / 2;
                    }
                    this.lobbySvr.PostRecvPackage(pkg);
                }
            }
        }

        private void HandleLobbyMsgSend()
        {
            if (this.isOnlineMode)
            {
                this.lobbySvr.HandleSending();
            }
        }

        public override void Init()
        {
            this.isOnlineMode = true;
            ClassEnumerator enumerator = new ClassEnumerator(typeof(MessageHandlerClassAttribute), null, typeof(NetworkModule).Assembly, true, false, false);
            ListView<System.Type>.Enumerator enumerator2 = enumerator.results.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                MethodInfo[] methods = enumerator2.Current.GetMethods();
                for (int i = 0; (methods != null) && (i < methods.Length); i++)
                {
                    MethodInfo method = methods[i];
                    if (method.IsStatic)
                    {
                        object[] customAttributes = method.GetCustomAttributes(typeof(MessageHandlerAttribute), true);
                        for (int j = 0; j < customAttributes.Length; j++)
                        {
                            MessageHandlerAttribute attribute = customAttributes[j] as MessageHandlerAttribute;
                            if (attribute != null)
                            {
                                this.RegisterMsgHandler(attribute.ID, (NetMsgDelegate) Delegate.CreateDelegate(typeof(NetMsgDelegate), method));
                                if (attribute.AdditionalIdList != null)
                                {
                                    int length = attribute.AdditionalIdList.Length;
                                    for (int k = 0; k < length; k++)
                                    {
                                        this.RegisterMsgHandler(attribute.AdditionalIdList[k], (NetMsgDelegate) Delegate.CreateDelegate(typeof(NetMsgDelegate), method));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool InitGameServerConnect(ConnectorParam para)
        {
            this.m_GameReconnetCount = 0;
            MonoSingleton<Reconnection>.GetInstance().ResetRelaySyncCache();
            Singleton<FrameWindow>.GetInstance().Reset();
            return this.gameSvr.Init(para);
        }

        public bool InitLobbyServerConnect(ConnectorParam para)
        {
            this.m_lobbyReconnetCount = 0;
            this.isOnlineMode = true;
            return this.lobbySvr.Init(para);
        }

        public static void InitRelayConnnecting(COMDT_TGWINFO inRelayTgw)
        {
            if (inRelayTgw.dwVipCnt > 0)
            {
                string host = null;
                if (MonoSingleton<CTongCaiSys>.GetInstance().IsCanUseTongCai())
                {
                    host = ApolloConfig.loginOnlyIpOrUrl;
                }
                else if ((inRelayTgw.szRelayUrl.Length > 0) && (inRelayTgw.szRelayUrl[0] != 0))
                {
                    host = StringHelper.UTF8BytesToString(ref inRelayTgw.szRelayUrl);
                }
                else
                {
                    host = ApolloConfig.loginOnlyIpOrUrl;
                }
                Singleton<ReconnectIpSelect>.instance.SetRelayTgw(inRelayTgw);
                LookUpDNSOfServerAndConfigNetAcc(host, inRelayTgw.wEchoPort);
                ConnectorParam para = new ConnectorParam {
                    bIsUDP = (inRelayTgw.bIsUDP <= 0) ? false : true,
                    ip = host
                };
                para.SetVPort(inRelayTgw.wVPort);
                NetworkAccelerator.ClearUDPCache();
                NetworkAccelerator.SetEchoPort(inRelayTgw.wEchoPort);
                ApolloConfig.echoPort = inRelayTgw.wEchoPort;
                Singleton<NetworkModule>.GetInstance().InitGameServerConnect(para);
            }
        }

        private static void LookUpDNSOfServerAndConfigNetAcc(string host, int port)
        {
            ApolloConfig.loginOnlyIp = string.Empty;
            try
            {
                Dns.BeginGetHostAddresses(host, new AsyncCallback(NetworkModule.DNSAsyncCallback), port);
            }
            catch (Exception)
            {
            }
        }

        public void RegisterMsgHandler(uint cmdID, NetMsgDelegate handler)
        {
            if (!this.mNetMsgHandlers.ContainsKey(cmdID))
            {
                this.mNetMsgHandlers.Add(cmdID, handler);
            }
        }

        public void ResetLobbySending()
        {
            this.lobbySvr.ResetSending(true);
        }

        public bool SendGameMsg(ref CSPkg msg, uint confirmMsgID = 0)
        {
            if (this.isOnlineMode && this.gameSvr.connected)
            {
                msg.stPkgHead.dwReserve = confirmMsgID;
                this.gameSvr.PushSendMsg(msg);
                return true;
            }
            return false;
        }

        public bool SendLobbyMsg(ref CSPkg msg, bool isShowAlert = false)
        {
            if (!this.isOnlineMode)
            {
                return false;
            }
            if (isShowAlert && !Singleton<BattleLogic>.instance.isRuning)
            {
                Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            }
            this.lobbySvr.PushSendMsg(msg);
            return true;
        }

        public void UpdateFrame()
        {
            if (this.isOnlineMode)
            {
                this.UpdateLobbyConnection();
                this.UpdateGameConnection();
                try
                {
                    this.HandleLobbyMsgSend();
                }
                catch (Exception exception)
                {
                    object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                    DebugHelper.Assert(false, "Error In HandleLobbyMsgSend: {0}, Call stack : {1}", inParameters);
                }
                try
                {
                    this.HandleGameMsgSend();
                }
                catch (Exception exception2)
                {
                    object[] objArray2 = new object[] { exception2.Message, exception2.StackTrace };
                    DebugHelper.Assert(false, "Error In HandleGameMsgSend: {0}, Call stack : {1}", objArray2);
                }
                try
                {
                    this.HandleLobbyMsgRecv();
                }
                catch (Exception exception3)
                {
                    object[] objArray3 = new object[] { exception3.Message, exception3.StackTrace };
                    DebugHelper.Assert(false, "Error In HandleLobbyMsgRecv: {0}, Call stack : {1}", objArray3);
                }
                try
                {
                    this.HandleGameMsgRecv();
                }
                catch (Exception exception4)
                {
                    object[] objArray4 = new object[] { exception4.Message, exception4.StackTrace };
                    DebugHelper.Assert(false, "Error In HandleGameMsgRecv: {0}, Call stack : {1}", objArray4);
                }
            }
        }

        private void UpdateGameConnection()
        {
            MonoSingleton<Reconnection>.instance.UpdateFrame();
            if (this.gameSvr.connected && ((Time.unscaledTime - this._preGameHeartTime) > 3f))
            {
                this._preGameHeartTime = Time.unscaledTime;
                CSPkg msg = CreateDefaultCSPKG(0x4ec);
                msg.stPkgData.stRelaySvrPing.dwTime = (uint) (Time.realtimeSinceStartup * 1000f);
                msg.stPkgData.stRelaySvrPing.dwSeqNo = (uint) Singleton<FrameSynchr>.GetInstance().m_SendHeartSeq;
                FrameSynchr instance = Singleton<FrameSynchr>.GetInstance();
                instance.m_SendHeartSeq += 1L;
                this.gameSvr.PushSendMsg(msg);
            }
        }

        private void UpdateLobbyConnection()
        {
            if (this.lobbySvr.CanSendPing() && ((Time.unscaledTime - this._preLobbyHeartTime) > 5f))
            {
                this._preLobbyHeartTime = Time.unscaledTime;
                CSPkg msg = CreateDefaultCSPKG(0x4ed);
                msg.stPkgData.stGameSvrPing.dwTime = (uint) (Time.realtimeSinceStartup * 1000f);
                this.lobbySvr.PushSendMsg(msg);
            }
        }

        public bool isOnlineMode
        {
            get
            {
                return this._bOnlineMode;
            }
            set
            {
                this._bOnlineMode = value;
            }
        }

        public uint lobbyPing { get; private set; }

        public uint RecvGameMsgCount
        {
            get
            {
                return this.m_uiRecvGameMsgCount;
            }
            set
            {
                this.m_uiRecvGameMsgCount = value;
            }
        }
    }
}

