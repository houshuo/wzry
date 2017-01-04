namespace Assets.Scripts.Framework
{
    using Apollo;
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using tsf4g_tdr_csharp;
    using UnityEngine;

    [MessageHandlerClass]
    public class Reconnection : MonoSingleton<Reconnection>, IGameModule
    {
        public float g_fBeginReconnectTime = -1f;
        private bool m_bShouldSpin = true;
        private bool m_bUpdateReconnect;
        private bool ms_bChaseupGameFrames;
        private bool ms_bDealRelayCachePkg;
        private bool ms_bProcessingRelaySync;
        private bool ms_bWaitingRelaySync;
        private int ms_cachedPkgLen;
        private ListView<CSPkg> ms_cachePkgList = new ListView<CSPkg>();
        private float ms_fWaitVideoTimeout;
        private ListView<CSPkg> ms_laterPkgList = new ListView<CSPkg>();
        private uint ms_nRelayCacheEndFrameNum;
        private uint ms_recvVideoPieceIdx;
        private int nRecvBuffSize = 0x64000;
        private int nRecvByteSize;
        private byte[] szRecvBuffer = new byte[0x64000];

        private void AddCachePkg(CSPkg msg)
        {
            DebugHelper.Assert(msg != null);
            if (!this.ms_bDealRelayCachePkg)
            {
                this.ms_cachePkgList.Add(msg);
            }
            else
            {
                msg.Release();
            }
        }

        private void CacheFramesData(CSPkg inFrapMsg)
        {
            int dwBufLen = (int) inFrapMsg.stPkgData.stRecoverFrapRsp.dwBufLen;
            byte[] dst = new byte[dwBufLen];
            Buffer.BlockCopy(inFrapMsg.stPkgData.stRecoverFrapRsp.szBuf, 0, dst, 0, dwBufLen);
            if ((this.nRecvByteSize + dst.Length) > this.nRecvBuffSize)
            {
                Array.Resize<byte>(ref this.szRecvBuffer, this.nRecvByteSize + dst.Length);
                Buffer.BlockCopy(dst, 0, this.szRecvBuffer, this.nRecvByteSize, dst.Length);
                this.nRecvBuffSize = this.nRecvByteSize + dst.Length;
                this.nRecvByteSize = this.nRecvBuffSize;
            }
            else
            {
                Buffer.BlockCopy(dst, 0, this.szRecvBuffer, this.nRecvByteSize, dst.Length);
                this.nRecvByteSize += dst.Length;
            }
        }

        private void ExitMultiGame()
        {
            this.m_bShouldSpin = false;
            this.m_bUpdateReconnect = false;
            this.ms_bChaseupGameFrames = false;
            Singleton<GameBuilder>.instance.EndGame();
            Singleton<CUIManager>.instance.CloseAllFormExceptLobby(true);
        }

        public bool FilterRelaySvrPackage(CSPkg msg)
        {
            if (msg.stPkgHead.dwMsgID == 0x43a)
            {
                this.m_bShouldSpin = false;
            }
            if (msg.stPkgHead.dwMsgID == 0x444)
            {
                this.onReconnectGame(msg);
                return true;
            }
            if (!this.isProcessingRelaySync)
            {
                return false;
            }
            if (msg.stPkgHead.dwMsgID == 0x443)
            {
                this.onRelaySyncCacheFrames(msg);
            }
            else
            {
                this.ms_laterPkgList.Add(msg);
            }
            return true;
        }

        private void HeroSelectReconectBanStep(CSDT_RECONN_BANINFO banInfo)
        {
            DebugHelper.CustomLog("HeroSelectReconectBanStep");
            Singleton<GameStateCtrl>.GetInstance().GotoState("LobbyState");
            Singleton<CRoomSystem>.instance.SetRoomType(1);
            CSDT_CAMPINFO[] campInfo = new CSDT_CAMPINFO[banInfo.astCampInfo.Length];
            for (int i = 0; i < banInfo.astCampInfo.Length; i++)
            {
                campInfo[i] = new CSDT_CAMPINFO();
                campInfo[i].dwPlayerNum = banInfo.astCampInfo[i].dwPlayerNum;
                campInfo[i].astCampPlayerInfo = new CSDT_CAMPPLAYERINFO[banInfo.astCampInfo[i].astCampPlayerInfo.Length];
                for (int j = 0; j < banInfo.astCampInfo[i].astCampPlayerInfo.Length; j++)
                {
                    campInfo[i].astCampPlayerInfo[j] = banInfo.astCampInfo[i].astCampPlayerInfo[j];
                }
            }
            CHeroSelectBaseSystem.StartPvpHeroSelectSystem(banInfo.stDeskInfo, campInfo, banInfo.stFreeHero, banInfo.stFreeHeroSymbol);
            Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep = enBanPickStep.enBan;
            if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
            {
                MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
                if (masterMemberInfo != null)
                {
                    Singleton<CHeroSelectBaseSystem>.instance.ClearBanHero();
                    Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_1, banInfo.stStateInfo.Camp1BanList);
                    Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_2, banInfo.stStateInfo.Camp2BanList);
                    Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState = banInfo.stStateInfo.stCurState;
                    Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stNextState = banInfo.stStateInfo.stNextState;
                    Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount = banInfo.stStateInfo.bBanPosNum;
                    Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(false);
                    Singleton<CHeroSelectBanPickSystem>.instance.RefreshAll();
                    Singleton<CHeroSelectBanPickSystem>.instance.PlayStepTitleAnimation();
                    Singleton<CHeroSelectBanPickSystem>.instance.PlayCurrentBgAnimation();
                    MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
                    if (memberInfo != null)
                    {
                        if (masterMemberInfo == memberInfo)
                        {
                            Utility.VibrateHelper();
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_MyTurn", null);
                        }
                        Singleton<CSoundManager>.GetInstance().PostEvent("Play_Music_BanPick", null);
                    }
                }
            }
        }

        private void HeroSelectReconectPickStep(CSDT_RECONN_PICKINFO pickInfo)
        {
            DebugHelper.CustomLog("HeroSelectReconectPickStep");
            Singleton<GameStateCtrl>.GetInstance().GotoState("LobbyState");
            Singleton<CRoomSystem>.instance.SetRoomType(1);
            CSDT_CAMPINFO[] campInfo = new CSDT_CAMPINFO[pickInfo.astCampInfo.Length];
            for (int i = 0; i < pickInfo.astCampInfo.Length; i++)
            {
                campInfo[i] = new CSDT_CAMPINFO();
                campInfo[i].dwPlayerNum = pickInfo.astCampInfo[i].dwPlayerNum;
                campInfo[i].astCampPlayerInfo = new CSDT_CAMPPLAYERINFO[pickInfo.astCampInfo[i].astPlayerInfo.Length];
                for (int k = 0; k < pickInfo.astCampInfo[i].astPlayerInfo.Length; k++)
                {
                    campInfo[i].astCampPlayerInfo[k] = pickInfo.astCampInfo[i].astPlayerInfo[k].stPickHeroInfo;
                }
            }
            CHeroSelectBaseSystem.StartPvpHeroSelectSystem(pickInfo.stDeskInfo, campInfo, pickInfo.stFreeHero, pickInfo.stFreeHeroSymbol);
            Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep = enBanPickStep.enPick;
            for (int j = 0; j < pickInfo.astCampInfo.Length; j++)
            {
                for (int m = 0; m < pickInfo.astCampInfo[j].astPlayerInfo.Length; m++)
                {
                    uint dwObjId = pickInfo.astCampInfo[j].astPlayerInfo[m].stPickHeroInfo.stPlayerInfo.dwObjId;
                    byte bIsPickOK = pickInfo.astCampInfo[j].astPlayerInfo[m].bIsPickOK;
                    MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo(dwObjId);
                    if (memberInfo != null)
                    {
                        memberInfo.isPrepare = bIsPickOK == 1;
                    }
                }
            }
            MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
            if (masterMemberInfo != null)
            {
                Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                if (masterMemberInfo.isPrepare)
                {
                    Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm = true;
                }
            }
            else
            {
                return;
            }
            if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
            {
                Singleton<CHeroSelectNormalSystem>.GetInstance().m_showHeroID = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
                if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
                {
                    Singleton<CHeroSelectNormalSystem>.instance.SwitchSkinMenuSelect();
                }
                Singleton<CHeroSelectNormalSystem>.GetInstance().RefreshHeroPanel(false, true);
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
            {
                CSDT_RECONN_BAN_PICK_STATE_INFO stBanPickInfo = pickInfo.stPickStateExtra.stPickDetail.stBanPickInfo;
                Singleton<CHeroSelectBaseSystem>.instance.ClearBanHero();
                if (stBanPickInfo != null)
                {
                    Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_1, stBanPickInfo.Camp1BanList);
                    Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_2, stBanPickInfo.Camp2BanList);
                    Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState = stBanPickInfo.stCurState;
                    Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stNextState = stBanPickInfo.stNextState;
                    Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount = stBanPickInfo.bBanPosNum;
                    Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(false);
                    Singleton<CHeroSelectBanPickSystem>.instance.RefreshAll();
                    Singleton<CHeroSelectBanPickSystem>.instance.PlayStepTitleAnimation();
                    Singleton<CHeroSelectBanPickSystem>.instance.PlayCurrentBgAnimation();
                    MemberInfo info3 = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo((COM_PLAYERCAMP) Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bCamp, Singleton<CHeroSelectBaseSystem>.instance.m_curBanPickInfo.stCurState.bPos);
                    if (info3 != null)
                    {
                        if (masterMemberInfo == info3)
                        {
                            Utility.VibrateHelper();
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_MyTurn", null);
                            Singleton<CSoundManager>.GetInstance().PostEvent("Set_Segment2", null);
                        }
                        else
                        {
                            Singleton<CSoundManager>.GetInstance().PostEvent("Set_Segment1", null);
                        }
                    }
                }
            }
        }

        private void HeroSelectReconectSwapStep(CSDT_RECONN_ADJUSTINFO swapInfo)
        {
            DebugHelper.CustomLog("HeroSelectReconectSwapStep");
            Singleton<GameStateCtrl>.GetInstance().GotoState("LobbyState");
            Singleton<CRoomSystem>.instance.SetRoomType(1);
            CSDT_CAMPINFO[] campInfo = new CSDT_CAMPINFO[swapInfo.astCampInfo.Length];
            for (int i = 0; i < swapInfo.astCampInfo.Length; i++)
            {
                campInfo[i] = new CSDT_CAMPINFO();
                campInfo[i].dwPlayerNum = swapInfo.astCampInfo[i].dwPlayerNum;
                campInfo[i].astCampPlayerInfo = new CSDT_CAMPPLAYERINFO[swapInfo.astCampInfo[i].astPlayerInfo.Length];
                for (int k = 0; k < swapInfo.astCampInfo[i].astPlayerInfo.Length; k++)
                {
                    campInfo[i].astCampPlayerInfo[k] = swapInfo.astCampInfo[i].astPlayerInfo[k].stPickHeroInfo;
                }
            }
            CHeroSelectBaseSystem.StartPvpHeroSelectSystem(swapInfo.stDeskInfo, campInfo, swapInfo.stFreeHero, swapInfo.stFreeHeroSymbol);
            Singleton<CHeroSelectBaseSystem>.instance.m_banPickStep = enBanPickStep.enSwap;
            for (int j = 0; j < swapInfo.astCampInfo.Length; j++)
            {
                for (int m = 0; m < swapInfo.astCampInfo[j].astPlayerInfo.Length; m++)
                {
                    uint dwObjId = swapInfo.astCampInfo[j].astPlayerInfo[m].stPickHeroInfo.stPlayerInfo.dwObjId;
                    byte bIsPickOK = swapInfo.astCampInfo[j].astPlayerInfo[m].bIsPickOK;
                    MemberInfo memberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMemberInfo(dwObjId);
                    if (memberInfo != null)
                    {
                        memberInfo.isPrepare = bIsPickOK == 1;
                    }
                }
            }
            MemberInfo masterMemberInfo = Singleton<CRoomSystem>.instance.roomInfo.GetMasterMemberInfo();
            if (masterMemberInfo != null)
            {
                Singleton<CHeroSelectBaseSystem>.instance.SetPvpHeroSelect(masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID);
                if (masterMemberInfo.isPrepare)
                {
                    Singleton<CHeroSelectBaseSystem>.instance.m_isSelectConfirm = true;
                }
            }
            else
            {
                return;
            }
            if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
            {
                Singleton<CHeroSelectNormalSystem>.GetInstance().m_showHeroID = masterMemberInfo.ChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
                if (Singleton<CHeroSelectBaseSystem>.instance.selectType == enSelectType.enRandom)
                {
                    Singleton<CHeroSelectNormalSystem>.instance.SwitchSkinMenuSelect();
                }
                Singleton<CHeroSelectNormalSystem>.instance.RefreshHeroPanel(false, true);
                Singleton<CHeroSelectNormalSystem>.instance.StartEndTimer((int) (swapInfo.dwLeftMs / 0x3e8));
            }
            else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
            {
                Singleton<CHeroSelectBaseSystem>.instance.ClearBanHero();
                Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_1, swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.Camp1BanList);
                Singleton<CHeroSelectBaseSystem>.instance.AddBanHero(COM_PLAYERCAMP.COM_PLAYERCAMP_2, swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.Camp2BanList);
                Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwActiveObjID = swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.dwActiveObjID;
                Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwPassiveObjID = swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.dwPassiveObjID;
                Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.iErrCode = 0;
                Singleton<CHeroSelectBaseSystem>.instance.m_banHeroTeamMaxCount = swapInfo.stHeroSwapInfo.stPickDetail.stHeroSwapInfo.bBanPosNum;
                if (masterMemberInfo.dwObjId == Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwActiveObjID)
                {
                    Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enReqing;
                }
                else if (masterMemberInfo.dwObjId == Singleton<CHeroSelectBaseSystem>.instance.m_swapInfo.dwPassiveObjID)
                {
                    Singleton<CHeroSelectBaseSystem>.instance.m_swapState = enSwapHeroState.enSwapAllow;
                }
                Singleton<CHeroSelectBanPickSystem>.instance.InitMenu(false);
                Singleton<CHeroSelectBanPickSystem>.instance.RefreshAll();
                Singleton<CHeroSelectBanPickSystem>.instance.StartEndTimer((int) (swapInfo.dwLeftMs / 0x3e8));
                Singleton<CSoundManager>.GetInstance().PostEvent("Set_BanPickEnd", null);
            }
        }

        protected override void Init()
        {
            Singleton<CUICommonSystem>.GetInstance();
        }

        private void OnCancelReconnecting(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectConfirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmReconnecting));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectCancel, new CUIEventManager.OnUIEventHandler(this.OnCancelReconnecting));
            this.ExitMultiGame();
        }

        private void OnConfirmReconnecting(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectConfirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmReconnecting));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectCancel, new CUIEventManager.OnUIEventHandler(this.OnCancelReconnecting));
            if (!Singleton<NetworkModule>.instance.gameSvr.connected)
            {
                Singleton<NetworkModule>.instance.gameSvr.ForceReconnect();
                Singleton<CUIManager>.GetInstance().OpenSendMsgAlert("手动重连游戏尝试...", 10, enUIEventID.None);
            }
        }

        private void OnConnectionClosedExitGame(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_ReconnectClosed, new CUIEventManager.OnUIEventHandler(this.OnConnectionClosedExitGame));
            this.ExitMultiGame();
        }

        public void OnConnectSuccess()
        {
        }

        [MessageHandler(0x425)]
        public static void onQueryIsRelayGaming(CSPkg msg)
        {
            if (MonoSingleton<Reconnection>.instance.shouldReconnect && !Singleton<WatchController>.GetInstance().IsWatching)
            {
                if (msg.stPkgData.stAskInMultGameRsp.bYes != 0)
                {
                    MonoSingleton<Reconnection>.instance.m_bUpdateReconnect = true;
                }
                else
                {
                    MonoSingleton<Reconnection>.instance.PopMsgBoxConnectionClosed();
                }
            }
        }

        private void onReconnectGame(CSPkg msg)
        {
            switch (msg.stPkgData.stReconnGameNtf.bState)
            {
                case 1:
                    this.HeroSelectReconectBanStep(msg.stPkgData.stReconnGameNtf.stStateData.stBanInfo);
                    Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                    break;

                case 2:
                    this.HeroSelectReconectPickStep(msg.stPkgData.stReconnGameNtf.stStateData.stPickInfo);
                    Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                    break;

                case 3:
                    this.HeroSelectReconectSwapStep(msg.stPkgData.stReconnGameNtf.stStateData.stAdjustInfo);
                    Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                    break;

                case 4:
                    if (!Singleton<LobbyLogic>.instance.inMultiGame)
                    {
                        Singleton<LobbyLogic>.instance.inMultiGame = true;
                        Singleton<LobbyLogic>.instance.inMultiRoom = true;
                        SCPKG_MULTGAME_BEGINLOAD stBeginLoad = msg.stPkgData.stReconnGameNtf.stStateData.stLoadingInfo.stBeginLoad;
                        Singleton<GameBuilder>.instance.StartGame(new MultiGameContext(stBeginLoad));
                        break;
                    }
                    return;

                case 5:
                    this.g_fBeginReconnectTime = Time.time;
                    Singleton<LobbyLogic>.GetInstance().inMultiRoom = true;
                    MonoSingleton<Reconnection>.GetInstance().RequestRelaySyncCacheFrames(false);
                    break;

                case 6:
                    MonoSingleton<Reconnection>.GetInstance().ExitMultiGame();
                    break;

                default:
                    DebugHelper.Assert(false);
                    break;
            }
        }

        private void onRelaySyncCacheFrames(CSPkg inFrapMsg)
        {
            DebugHelper.Assert(this.ms_bWaitingRelaySync);
            if (this.ms_bWaitingRelaySync)
            {
                if (inFrapMsg.stPkgData.stRecoverFrapRsp.dwThisPos != this.ms_recvVideoPieceIdx)
                {
                    this.RequestRelaySyncCacheFrames(true);
                }
                else
                {
                    this.ms_recvVideoPieceIdx++;
                    if (inFrapMsg.stPkgData.stRecoverFrapRsp.dwBufLen > 0)
                    {
                        this.CacheFramesData(inFrapMsg);
                    }
                    if (inFrapMsg.stPkgData.stRecoverFrapRsp.dwTotalNum == this.ms_recvVideoPieceIdx)
                    {
                        this.ParseFramesData();
                        DebugHelper.Assert(this.nRecvByteSize == 0);
                        this.ms_bWaitingRelaySync = false;
                        this.ms_bProcessingRelaySync = true;
                        this.ms_nRelayCacheEndFrameNum = inFrapMsg.stPkgData.stRecoverFrapRsp.dwCurKFrapsNo;
                        base.StartCoroutine("ProcessRelaySyncCache");
                    }
                    else
                    {
                        CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x442);
                        msg.stPkgData.stRecoverFrapReq.bIsNew = 0;
                        Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
                        this.ms_fWaitVideoTimeout = 20f;
                    }
                }
            }
        }

        private void ParseFramesData()
        {
            try
            {
                while (this.nRecvByteSize > 0)
                {
                    int usedSize = 0;
                    CSPkg msg = CSPkg.New();
                    if ((msg.unpack(ref this.szRecvBuffer, this.nRecvByteSize, ref usedSize, 0) != TdrError.ErrorType.TDR_NO_ERROR) || (usedSize <= 0))
                    {
                        return;
                    }
                    Buffer.BlockCopy(this.szRecvBuffer, usedSize, this.szRecvBuffer, 0, this.nRecvByteSize - usedSize);
                    this.nRecvByteSize -= usedSize;
                    this.AddCachePkg(msg);
                }
            }
            catch (Exception exception)
            {
                BugLocateLogSys.Log("ParseFramesCacheData " + exception.Message);
            }
        }

        private void PopConfirmingReconnecting()
        {
            if (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/Common/Form_MessageBox.prefab") == null)
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel("重连服务器失败，请检测手机网络环境。", enUIEventID.Net_ReconnectConfirm, enUIEventID.Net_ReconnectCancel, false);
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Net_ReconnectConfirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmReconnecting));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Net_ReconnectCancel, new CUIEventManager.OnUIEventHandler(this.OnCancelReconnecting));
            }
        }

        public void PopMsgBoxConnectionClosed()
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBox("游戏已经结束，点击确定返回游戏大厅。", enUIEventID.Net_ReconnectClosed, false);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Net_ReconnectClosed, new CUIEventManager.OnUIEventHandler(this.OnConnectionClosedExitGame));
        }

        [DebuggerHidden]
        private IEnumerator ProcessRelaySyncCache()
        {
            return new <ProcessRelaySyncCache>c__Iterator9 { <>f__this = this };
        }

        public void QueryIsRelayGaming(ApolloResult result)
        {
            if (this.m_bShouldSpin && Singleton<BattleLogic>.instance.isRuning)
            {
                if ((result == ApolloResult.PeerStopSession) || (result == ApolloResult.PeerCloseConnection))
                {
                    CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x424);
                    Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                }
                else if (result == ApolloResult.TokenInvalid)
                {
                    Singleton<ApolloHelper>.GetInstance().Login(ApolloConfig.platform, 0L, null);
                    Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("AccessTokenExpired", null, true);
                }
                else
                {
                    this.m_bUpdateReconnect = true;
                }
            }
        }

        public bool RequestRelaySyncCacheFrames(bool force = false)
        {
            if (Singleton<WatchController>.GetInstance().IsWatching)
            {
                return false;
            }
            if (this.isProcessingRelayRecover && !force)
            {
                return false;
            }
            base.StopCoroutine("ProcessRelaySyncCache");
            this.ms_cachePkgList.Clear();
            this.ms_laterPkgList.Clear();
            this.ms_nRelayCacheEndFrameNum = 0;
            this.ms_bDealRelayCachePkg = false;
            this.ms_bProcessingRelaySync = false;
            this.ms_bChaseupGameFrames = false;
            this.m_bShouldSpin = true;
            this.m_bUpdateReconnect = true;
            this.ms_recvVideoPieceIdx = 0;
            this.nRecvByteSize = 0;
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x442);
            msg.stPkgData.stRecoverFrapReq.dwCurLen = (uint) this.ms_cachedPkgLen;
            msg.stPkgData.stRecoverFrapReq.bIsNew = 1;
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            this.ms_bWaitingRelaySync = true;
            this.ms_fWaitVideoTimeout = 20f;
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert("断线重连数据恢复中，嗷了个嗷...", 10, enUIEventID.None);
            return true;
        }

        public void ResetRelaySyncCache()
        {
            this.nRecvByteSize = 0;
            this.ms_cachedPkgLen = 0;
            this.ms_recvVideoPieceIdx = 0;
            this.ms_bWaitingRelaySync = false;
            this.ms_fWaitVideoTimeout = 0f;
            this.ms_bDealRelayCachePkg = false;
            this.ms_bProcessingRelaySync = false;
            this.ms_bChaseupGameFrames = false;
            this.m_bShouldSpin = true;
            this.m_bUpdateReconnect = true;
            this.ms_nRelayCacheEndFrameNum = 0;
            base.StopCoroutine("ProcessRelaySyncCache");
            this.ms_cachePkgList.Clear();
            this.ms_laterPkgList.Clear();
        }

        public bool SendReconnectSucceeded()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x445);
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            return true;
        }

        public void ShowReconnectMsgAlert(int nCount, int nMax)
        {
            if (nCount > nMax)
            {
                if (nCount == (nMax + 1))
                {
                    this.PopConfirmingReconnecting();
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(string.Format("自动重连 第[{0}/{1}]尝试...", nCount, nMax), 10, enUIEventID.None);
            }
        }

        public void UpdateCachedLen(CSPkg msg)
        {
            DebugHelper.Assert(msg != null);
            if (msg.stPkgHead.dwReserve > this.ms_cachedPkgLen)
            {
                this.ms_cachedPkgLen = (int) msg.stPkgHead.dwReserve;
            }
        }

        public void UpdateFrame()
        {
            if (this.ms_bChaseupGameFrames)
            {
                if ((Singleton<FrameSynchr>.instance.EndFrameNum > 0) && (Singleton<FrameSynchr>.instance.CurFrameNum > 0))
                {
                    CUILoadingSystem.OnSelfLoadProcess((0.99f * Singleton<FrameSynchr>.instance.CurFrameNum) / ((float) Singleton<FrameSynchr>.instance.EndFrameNum));
                }
                this.ms_bChaseupGameFrames = Singleton<FrameSynchr>.instance.CurFrameNum < (Singleton<FrameSynchr>.instance.EndFrameNum - 15);
                if (!this.ms_bChaseupGameFrames)
                {
                    this.SendReconnectSucceeded();
                    ProtocolObjectPool.Clear(50);
                    Singleton<GameEventSys>.instance.SendEvent(GameEventDef.Event_MultiRecoverFin);
                }
            }
            if (this.ms_bWaitingRelaySync)
            {
                this.ms_fWaitVideoTimeout -= Time.unscaledDeltaTime;
                if (this.ms_fWaitVideoTimeout < 0f)
                {
                    this.RequestRelaySyncCacheFrames(true);
                }
            }
        }

        public void UpdateReconnect()
        {
            if ((Singleton<NetworkModule>.GetInstance().isOnlineMode && this.m_bShouldSpin) && this.m_bUpdateReconnect)
            {
                Singleton<NetworkModule>.instance.gameSvr.Update();
            }
        }

        public bool isExcuteCacheMsgData
        {
            get
            {
                return this.ms_bDealRelayCachePkg;
            }
        }

        public bool isProcessingRelayRecover
        {
            get
            {
                return ((this.ms_bWaitingRelaySync || this.ms_bProcessingRelaySync) || this.ms_bChaseupGameFrames);
            }
        }

        public bool isProcessingRelaySync
        {
            get
            {
                return (this.ms_bWaitingRelaySync || this.ms_bProcessingRelaySync);
            }
        }

        public bool shouldReconnect
        {
            get
            {
                return this.m_bShouldSpin;
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessRelaySyncCache>c__Iterator9 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Reconnection <>f__this;
            internal int <i>__0;
            internal int <j>__2;
            internal CSPkg <msg>__1;
            internal CSPkg <msg1>__3;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this.ms_bDealRelayCachePkg = true;
                        this.<i>__0 = 0;
                        goto Label_00A3;

                    case 1:
                        break;

                    default:
                        goto Label_0179;
                }
            Label_0071:
                while (MonoSingleton<GameLoader>.instance.isLoadStart)
                {
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    return true;
                }
                Singleton<NetworkModule>.instance.gameSvr.HandleMsg(this.<msg>__1);
                this.<i>__0++;
            Label_00A3:
                if (this.<i>__0 < this.<>f__this.ms_cachePkgList.Count)
                {
                    this.<msg>__1 = this.<>f__this.ms_cachePkgList[this.<i>__0];
                    goto Label_0071;
                }
                this.<>f__this.ms_bDealRelayCachePkg = false;
                this.<>f__this.ms_cachePkgList.Clear();
                this.<j>__2 = 0;
                while (this.<j>__2 < this.<>f__this.ms_laterPkgList.Count)
                {
                    this.<msg1>__3 = this.<>f__this.ms_laterPkgList[this.<j>__2];
                    Singleton<NetworkModule>.instance.gameSvr.HandleMsg(this.<msg1>__3);
                    this.<j>__2++;
                }
                this.<>f__this.ms_laterPkgList.Clear();
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                this.<>f__this.ms_bChaseupGameFrames = true;
                this.<>f__this.ms_bProcessingRelaySync = false;
                this.$PC = -1;
            Label_0179:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

