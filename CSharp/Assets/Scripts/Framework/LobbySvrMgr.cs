namespace Assets.Scripts.Framework
{
    using Apollo;
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class LobbySvrMgr : Singleton<LobbySvrMgr>
    {
        public ChooseSvrPolicy chooseSvrPol;
        public NetworkReachability curNetworkReachability;
        public bool isFirstLogin;
        public bool isLogin;
        private bool isLoginingByDefault = true;

        public event ConnectFailHandler connectFailHandler;

        private void ConnectFailed()
        {
            if (this.chooseSvrPol == ChooseSvrPolicy.DeviceID)
            {
                MonoSingleton<TdirMgr>.GetInstance().ConnectLobbyWithSnsNameChooseSvr();
            }
            else if (this.chooseSvrPol == ChooseSvrPolicy.NickName)
            {
                MonoSingleton<TdirMgr>.GetInstance().ConnectLobbyRandomChooseSvr(ChooseSvrPolicy.Random1);
            }
            else if (this.chooseSvrPol == ChooseSvrPolicy.Random1)
            {
                MonoSingleton<TdirMgr>.GetInstance().ConnectLobbyRandomChooseSvr(ChooseSvrPolicy.Random2);
            }
            else
            {
                Debug.Log("LobbyConnector ConnectFailed called!");
                if (this.connectFailHandler != null)
                {
                    this.connectFailHandler(Singleton<NetworkModule>.GetInstance().lobbySvr.lastResult);
                }
            }
        }

        public bool ConnectServer()
        {
            if (!Singleton<NetworkModule>.GetInstance().isOnlineMode || this.isLogin)
            {
                return false;
            }
            Singleton<NetworkModule>.GetInstance().lobbySvr.ConnectedEvent -= new NetConnectedEvent(this.onLobbyConnected);
            Singleton<NetworkModule>.GetInstance().lobbySvr.DisconnectEvent -= new NetDisconnectEvent(this.onLobbyDisconnected);
            Singleton<NetworkModule>.GetInstance().lobbySvr.ConnectedEvent += new NetConnectedEvent(this.onLobbyConnected);
            Singleton<NetworkModule>.GetInstance().lobbySvr.DisconnectEvent += new NetDisconnectEvent(this.onLobbyDisconnected);
            Singleton<NetworkModule>.GetInstance().lobbySvr.GetTryReconnect = (LobbyConnector.DelegateGetTryReconnect) Delegate.Remove(Singleton<NetworkModule>.GetInstance().lobbySvr.GetTryReconnect, new LobbyConnector.DelegateGetTryReconnect(this.OnTryReconnect));
            Singleton<NetworkModule>.GetInstance().lobbySvr.GetTryReconnect = (LobbyConnector.DelegateGetTryReconnect) Delegate.Combine(Singleton<NetworkModule>.GetInstance().lobbySvr.GetTryReconnect, new LobbyConnector.DelegateGetTryReconnect(this.OnTryReconnect));
            ConnectorParam para = new ConnectorParam {
                url = ApolloConfig.loginUrl,
                ip = ApolloConfig.loginOnlyIpOrUrl,
                vPort = ApolloConfig.loginOnlyVPort
            };
            bool flag = Singleton<NetworkModule>.GetInstance().InitLobbyServerConnect(para);
            this.curNetworkReachability = Application.internetReachability;
            if (flag)
            {
                Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(null, 10, enUIEventID.None);
            }
            return flag;
        }

        private void ConnectServerWithTdirCandidate(int index)
        {
            bool flag = false;
            this.isLoginingByDefault = false;
            IPAddrInfo info = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index];
            ApolloConfig.loginOnlyVPort = ushort.Parse(info.port);
            if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
            {
                flag = true;
                ApolloConfig.ISPType = MonoSingleton<TdirMgr>.GetInstance().GetISP();
                ApolloConfig.loginOnlyIpOrUrl = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[ApolloConfig.ISPType];
                IPAddrInfo info2 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index];
                ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", ApolloConfig.loginOnlyIpOrUrl, info2.port);
            }
            else
            {
                object param = 0;
                if (MonoSingleton<TdirMgr>.GetInstance().ParseNodeAppAttr(MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.attr, TdirNodeAttrType.ISPChoose, ref param))
                {
                    Dictionary<string, int> dictionary = (Dictionary<string, int>) param;
                    if (dictionary != null)
                    {
                        foreach (KeyValuePair<string, int> pair in dictionary)
                        {
                            if (pair.Value == MonoSingleton<TdirMgr>.GetInstance().GetISP())
                            {
                                IPAddrInfo info3 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index];
                                ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", pair.Key, info3.port);
                                ApolloConfig.loginOnlyIpOrUrl = pair.Key;
                                ApolloConfig.ISPType = MonoSingleton<TdirMgr>.GetInstance().GetISP();
                                flag = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (flag)
            {
                this.ConnectServer();
            }
            else
            {
                this.ConnectFailed();
            }
        }

        public void ConnectServerWithTdirDefault(int index, ChooseSvrPolicy policy)
        {
            this.chooseSvrPol = policy;
            this.isLoginingByDefault = true;
            IPAddrInfo info = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index];
            ApolloConfig.loginOnlyVPort = ushort.Parse(info.port);
            ApolloConfig.ISPType = MonoSingleton<TdirMgr>.GetInstance().GetISP();
            if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
            {
                IPAddrInfo info2 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index];
                ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", MonoSingleton<CTongCaiSys>.instance.TongcaiUrl, info2.port);
                ApolloConfig.loginOnlyIpOrUrl = MonoSingleton<CTongCaiSys>.instance.TongcaiUrl;
            }
            else
            {
                IPAddrInfo info3 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index];
                IPAddrInfo info4 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index];
                ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", info3.ip, info4.port);
                IPAddrInfo info5 = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.addrs[index];
                ApolloConfig.loginOnlyIpOrUrl = info5.ip;
            }
            this.ConnectServer();
            Singleton<BeaconHelper>.GetInstance().Event_CommonReport("Event_LoginMsgSend");
        }

        private void onLobbyConnected(object sender)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        private void onLobbyDisconnected(object sender)
        {
        }

        public uint OnTryReconnect(uint curConnectTime, uint maxcount)
        {
            if (this.isLogin)
            {
                if ((this.curNetworkReachability != Application.internetReachability) && MonoSingleton<CTongCaiSys>.instance.isTongCaiValid)
                {
                    ApolloConfig.loginOnlyIpOrUrl = Singleton<ReconnectIpSelect>.instance.GetConnectUrl(ConnectorType.Lobby, curConnectTime);
                    ApolloConfig.loginUrl = string.Format("tcp://{0}:{1}", ApolloConfig.loginOnlyIpOrUrl, ApolloConfig.loginOnlyVPort);
                    Singleton<NetworkModule>.instance.lobbySvr.initParam.SetVip(ApolloConfig.loginOnlyIpOrUrl);
                    this.curNetworkReachability = Application.internetReachability;
                }
                if (!Singleton<LobbyLogic>.instance.inMultiGame && (Singleton<BattleLogic>.instance.isGameOver || Singleton<BattleLogic>.instance.m_bIsPayStat))
                {
                    if (curConnectTime > maxcount)
                    {
                        if (curConnectTime == (maxcount + 1))
                        {
                            Singleton<LobbyLogic>.instance.OnSendSingleGameFinishFail();
                        }
                        return curConnectTime;
                    }
                    Singleton<CUIManager>.GetInstance().OpenSendMsgAlert("尝试重新连接服务器...", 5, enUIEventID.None);
                    return curConnectTime;
                }
                if (!Singleton<BattleLogic>.instance.isRuning)
                {
                    Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                }
                if (curConnectTime >= maxcount)
                {
                    return 0;
                }
                return curConnectTime;
            }
            if (this.isLoginingByDefault)
            {
                this.ConnectServerWithTdirCandidate(MonoSingleton<TdirMgr>.GetInstance().m_connectIndex);
            }
            else
            {
                this.ConnectFailed();
            }
            return (curConnectTime + maxcount);
        }

        public delegate void ConnectFailHandler(ApolloResult result);
    }
}

