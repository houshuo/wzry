namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class COBSystem : Singleton<COBSystem>
    {
        private enStatus curStatus;
        public static int EXPERT_DETAL_SEC = 60;
        public static int FRIEND_DETAL_SEC = 60;
        private static int m_lastGetExpertListTime = 0;
        private static int m_lastGetFriendStateTime = 0;
        public static readonly string OB_FORM_PATH = "UGUI/Form/System/OB/Form_OB.prefab";
        private List<stOBExpert> OBExpertList = new List<stOBExpert>();
        private List<stOBFriend> OBFriendList = new List<stOBFriend>();
        private ListView<GameReplayModule.ReplayFileInfo> OBLocalList = new ListView<GameReplayModule.ReplayFileInfo>();
        private static Vector2 s_content_pos = new Vector2(0f, -30f);
        private static Vector2 s_content_size = new Vector2(0f, -60f);

        public void Clear()
        {
            this.OBFriendList.Clear();
            this.OBExpertList.Clear();
            this.OBLocalList.Clear();
            m_lastGetExpertListTime = 0;
            m_lastGetFriendStateTime = 0;
        }

        public static void GetFriendsState()
        {
            if ((CRoleInfo.GetCurrentUTCTime() - m_lastGetFriendStateTime) > FRIEND_DETAL_SEC)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1471);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                m_lastGetFriendStateTime = CRoleInfo.GetCurrentUTCTime();
            }
        }

        public static void GetGreatMatch(bool bForce = false)
        {
            if (bForce || ((CRoleInfo.GetCurrentUTCTime() - m_lastGetExpertListTime) > EXPERT_DETAL_SEC))
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1466);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                m_lastGetExpertListTime = CRoleInfo.GetCurrentUTCTime();
            }
        }

        public override void Init()
        {
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_STATE_NTF", new Action<CSPkg>(this.On_Friend_SNS_STATE_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_List", new Action<CSPkg>(this.On_FriendSys_Friend_List));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_GAME_STATE_NTF", new Action<CSPkg>(this.On_Friend_GAME_STATE_NTF));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnOBFormOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnOBFormClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Tab_Click, new CUIEventManager.OnUIEventHandler(this.OnOBVideoTabClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Enter, new CUIEventManager.OnUIEventHandler(this.OnVideoEnter));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Enter_Confirm, new CUIEventManager.OnUIEventHandler(this.OnVideoEnterConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Element_Enable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Editor_Click, new CUIEventManager.OnUIEventHandler(this.OnEditorClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Delete, new CUIEventManager.OnUIEventHandler(this.OnVideoDelete));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Delete_Confirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmDelete));
            base.Init();
        }

        private void InitTab(CUIListScript list)
        {
            if (list != null)
            {
                int amount = Enum.GetValues(typeof(enOBTab)).Length - 1;
                list.SetElementAmount(amount);
                string text = string.Empty;
                for (int i = 0; i < amount; i++)
                {
                    switch (((enOBTab) (i + 1)))
                    {
                        case enOBTab.Expert:
                            text = Singleton<CTextManager>.instance.GetText("OB_Expert");
                            break;

                        case enOBTab.Friend:
                            text = Singleton<CTextManager>.instance.GetText("OB_Freind");
                            break;

                        case enOBTab.Local:
                            text = Singleton<CTextManager>.instance.GetText("OB_Local");
                            break;
                    }
                    Utility.GetComponetInChild<Text>(list.GetElemenet(i).gameObject, "Text").text = text;
                }
                list.SelectElement(0, true);
            }
        }

        public bool IsInOBFriendList(ulong uid)
        {
            for (int i = 0; i < this.OBFriendList.Count; i++)
            {
                stOBFriend friend = this.OBFriendList[i];
                if (friend.uin.ullUid == uid)
                {
                    return true;
                }
            }
            return false;
        }

        public void OBFriend(COMDT_ACNT_UNIQ uin)
        {
            bool flag = false;
            for (int i = 0; i < this.OBFriendList.Count; i++)
            {
                stOBFriend friend = this.OBFriendList[i];
                if (uin.ullUid == friend.uin.ullUid)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                SendOBServeFriend(uin);
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips("OB_Error_1", false, 1.5f, null, new object[0]);
            }
        }

        private void On_Friend_GAME_STATE_NTF(CSPkg msg)
        {
            stOBFriend friend;
            SCPKG_CMD_NTF_FRIEND_GAME_STATE stNtfFriendGameState = msg.stPkgData.stNtfFriendGameState;
            for (int i = 0; i < this.OBFriendList.Count; i++)
            {
                stOBFriend friend2 = this.OBFriendList[i];
                if (friend2.uin.ullUid == stNtfFriendGameState.stAcntUniq.ullUid)
                {
                    if (stNtfFriendGameState.bVideoState == 0)
                    {
                        this.OBFriendList.RemoveAt(i);
                    }
                    else if (stNtfFriendGameState.bVideoState == 1)
                    {
                        friend = this.OBFriendList[i];
                        friend.gameDetail = stNtfFriendGameState.stGameInfo.stDetail;
                        this.OBFriendList[i] = friend;
                    }
                    this.UpdateView();
                    return;
                }
            }
            if (stNtfFriendGameState.bVideoState != 0)
            {
                if (stNtfFriendGameState.stGameInfo.stDetail == null)
                {
                    DebugHelper.Assert(false, "SCPKG_CMD_NTF_FRIEND_GAME_STATE [bMultGameSubState == COM_ACNT_MULTIGAME_GAMING] and  [stGameInfo.stDetail == null] , this is sever' guo!");
                }
                else
                {
                    COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByUid(stNtfFriendGameState.stAcntUniq.ullUid, CFriendModel.FriendType.GameFriend);
                    if (comdt_friend_info == null)
                    {
                        comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByUid(stNtfFriendGameState.stAcntUniq.ullUid, CFriendModel.FriendType.SNS);
                    }
                    if (comdt_friend_info != null)
                    {
                        friend = new stOBFriend {
                            uin = stNtfFriendGameState.stAcntUniq,
                            friendName = Utility.UTF8Convert(comdt_friend_info.szUserName),
                            headUrl = Utility.UTF8Convert(comdt_friend_info.szHeadUrl),
                            gameDetail = stNtfFriendGameState.stGameInfo.stDetail
                        };
                        this.OBFriendList.Add(friend);
                        this.UpdateView();
                    }
                }
            }
        }

        private void On_Friend_SNS_STATE_NTF(CSPkg msg)
        {
            SCPKG_NTF_SNS_FRIEND stNtfSnsFriend = msg.stPkgData.stNtfSnsFriend;
            for (int i = 0; i < stNtfSnsFriend.dwSnsFriendNum; i++)
            {
                CSDT_SNS_FRIEND_INFO csdt_sns_friend_info = stNtfSnsFriend.astSnsFriendList[i];
                if ((csdt_sns_friend_info != null) && (csdt_sns_friend_info.bVideoState != 0))
                {
                    stOBFriend friend;
                    if (csdt_sns_friend_info.stGameInfo.stDetail == null)
                    {
                        DebugHelper.Assert(false, "SCPKG_NTF_SNS_FRIEND [bMultGameSubState == COM_ACNT_MULTIGAME_GAMING] and  [stGameInfo.stDetail == null] , this is sever' guo!");
                        continue;
                    }
                    bool flag = false;
                    for (int j = 0; j < this.OBFriendList.Count; j++)
                    {
                        stOBFriend friend2 = this.OBFriendList[j];
                        if (stNtfSnsFriend.astSnsFriendList[i].stSnsFrindInfo.stUin.ullUid == friend2.uin.ullUid)
                        {
                            friend = this.OBFriendList[j];
                            friend.gameDetail = stNtfSnsFriend.astSnsFriendList[i].stGameInfo.stDetail;
                            this.OBFriendList[j] = friend;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        friend = new stOBFriend {
                            uin = stNtfSnsFriend.astSnsFriendList[i].stSnsFrindInfo.stUin,
                            friendName = Utility.UTF8Convert(stNtfSnsFriend.astSnsFriendList[i].stSnsFrindInfo.szUserName),
                            headUrl = Utility.UTF8Convert(stNtfSnsFriend.astSnsFriendList[i].stSnsFrindInfo.szHeadUrl),
                            gameDetail = stNtfSnsFriend.astSnsFriendList[i].stGameInfo.stDetail
                        };
                        this.OBFriendList.Add(friend);
                    }
                }
            }
            this.UpdateView();
        }

        private void On_FriendSys_Friend_List(CSPkg msg)
        {
            SCPKG_CMD_LIST_FRIEND stFriendListRsp = msg.stPkgData.stFriendListRsp;
            for (int i = 0; i < stFriendListRsp.dwFriendNum; i++)
            {
                CSDT_FRIEND_INFO csdt_friend_info = stFriendListRsp.astFrindList[i];
                if ((csdt_friend_info != null) && (csdt_friend_info.bVideoState != 0))
                {
                    stOBFriend friend;
                    if (csdt_friend_info.stGameInfo.stDetail == null)
                    {
                        DebugHelper.Assert(false, "CSDT_FRIEND_INFO [bMultGameSubState == COM_ACNT_MULTIGAME_GAMING] and  [stGameInfo.stDetail == null] , this is sever' guo!");
                        continue;
                    }
                    bool flag = false;
                    for (int j = 0; i < this.OBFriendList.Count; j++)
                    {
                        stOBFriend friend2 = this.OBFriendList[j];
                        if (stFriendListRsp.astFrindList[i].stFriendInfo.stUin.ullUid == friend2.uin.ullUid)
                        {
                            friend = this.OBFriendList[j];
                            friend.gameDetail = stFriendListRsp.astFrindList[i].stGameInfo.stDetail;
                            this.OBFriendList[j] = friend;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        friend = new stOBFriend {
                            uin = stFriendListRsp.astFrindList[i].stFriendInfo.stUin,
                            friendName = Utility.UTF8Convert(stFriendListRsp.astFrindList[i].stFriendInfo.szUserName),
                            headUrl = Utility.UTF8Convert(stFriendListRsp.astFrindList[i].stFriendInfo.szHeadUrl),
                            gameDetail = stFriendListRsp.astFrindList[i].stGameInfo.stDetail
                        };
                        this.OBFriendList.Add(friend);
                    }
                }
            }
            this.UpdateView();
        }

        [MessageHandler(0x1467)]
        public static void ON_GET_GREATMATCH_RSP(CSPkg msg)
        {
            Singleton<COBSystem>.instance.OnGetGreatMatch(msg);
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
        }

        [MessageHandler(0x1463)]
        public static void ON_OBSERVE_FRIEND_RSP(CSPkg msg)
        {
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
            if (msg.stPkgData.stObserveFriendRsp.iResult == 0)
            {
                if (Singleton<WatchController>.GetInstance().StartObserve(msg.stPkgData.stObserveFriendRsp.stTgwinfo))
                {
                    Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                }
            }
            else
            {
                Singleton<COBSystem>.instance.UpdateView();
                Singleton<CUIManager>.instance.OpenTips(string.Format("OB_Error_{0}", msg.stPkgData.stObserveFriendRsp.iResult), true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x1465)]
        public static void ON_OBSERVE_GREAT_RSP(CSPkg msg)
        {
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
            if (msg.stPkgData.stObserveGreatRsp.iResult == 0)
            {
                if (Singleton<WatchController>.GetInstance().StartObserve(null))
                {
                    Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                }
            }
            else
            {
                GetGreatMatch(true);
                Singleton<CUIManager>.instance.OpenTips(string.Format("OB_Error_{0}", msg.stPkgData.stObserveGreatRsp.iResult), true, 1.5f, null, new object[0]);
            }
        }

        private void OnConfirmDelete(CUIEvent cuiEvent)
        {
            int tag = cuiEvent.m_eventParams.tag;
            if ((tag >= 0) && (tag < this.OBLocalList.Count))
            {
                string path = this.OBLocalList[tag].path;
                try
                {
                    File.Delete(path);
                    this.OBLocalList.RemoveAt(tag);
                    Singleton<CUIManager>.instance.OpenTips("OB_Desc_8", true, 1.5f, null, new object[0]);
                }
                catch
                {
                    Singleton<CUIManager>.instance.OpenTips("OB_Desc_9", true, 1.5f, null, new object[0]);
                }
            }
            this.UpdateView();
        }

        private void OnEditorClick(CUIEvent cuiEvent)
        {
            if (this.CurTab == enOBTab.Local)
            {
                if (this.curStatus == enStatus.Normal)
                {
                    this.curStatus = enStatus.Editor;
                }
                else
                {
                    this.curStatus = enStatus.Normal;
                }
                this.UpdateView();
            }
        }

        private void OnElementEnable(CUIEvent cuiEvent)
        {
            if (this.CurTab == enOBTab.Expert)
            {
                this.UpdateElement(cuiEvent.m_srcWidget, this.OBExpertList[cuiEvent.m_srcWidgetIndexInBelongedList]);
            }
            else if (this.CurTab == enOBTab.Friend)
            {
                this.UpdateElement(cuiEvent.m_srcWidget, this.OBFriendList[cuiEvent.m_srcWidgetIndexInBelongedList]);
            }
            else if (this.CurTab == enOBTab.Local)
            {
                this.UpdateElement(cuiEvent.m_srcWidget, this.OBLocalList[cuiEvent.m_srcWidgetIndexInBelongedList]);
            }
        }

        private void OnGetGreatMatch(CSPkg msg)
        {
            this.OBExpertList.Clear();
            for (int i = 0; i < msg.stPkgData.stGetGreatMatchRsp.dwCount; i++)
            {
                for (int j = 0; j < msg.stPkgData.stGetGreatMatchRsp.astList[i].dwLabelNum; j++)
                {
                    stOBExpert item = new stOBExpert {
                        desk = msg.stPkgData.stGetGreatMatchRsp.astList[i].stDesk,
                        startTime = msg.stPkgData.stGetGreatMatchRsp.astList[i].dwStartTime,
                        observeNum = msg.stPkgData.stGetGreatMatchRsp.astList[i].dwObserveNum,
                        heroLabel = msg.stPkgData.stGetGreatMatchRsp.astList[i].astLabel[j]
                    };
                    this.OBExpertList.Add(item);
                }
            }
            this.OBExpertList.Sort(new Comparison<stOBExpert>(this.SortByObserveNum));
            this.UpdateView();
        }

        private void OnOBFormClose(CUIEvent cuiEvent)
        {
        }

        public void OnOBFormOpen(CUIEvent cuiEvent)
        {
            if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
            }
            else
            {
                CUIFormScript script = Singleton<CUIManager>.instance.OpenForm(OB_FORM_PATH, false, true);
                if (script != null)
                {
                    CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(script.gameObject, "Panel_Menu/List");
                    this.InitTab(componetInChild);
                    this.OBLocalList = Singleton<GameReplayModule>.instance.ListReplayFiles(true);
                    Singleton<CMiShuSystem>.instance.SetNewFlagForOBBtn(false);
                    Transform transform = script.gameObject.transform.FindChild("Panel_Menu/BtnVideoMgr");
                    if ((transform != null) && (transform.gameObject != null))
                    {
                        if (!Singleton<CRecordUseSDK>.instance.GetRecorderGlobalCfgEnableFlag())
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                        else
                        {
                            transform.gameObject.CustomSetActive(true);
                        }
                    }
                }
            }
        }

        private void OnOBVideoTabClick(CUIEvent cuiEvent)
        {
            switch (this.CurTab)
            {
                case enOBTab.Expert:
                    GetGreatMatch(false);
                    break;

                case enOBTab.Friend:
                    GetFriendsState();
                    break;
            }
            this.curStatus = enStatus.Normal;
            this.UpdateView();
        }

        private void OnVideoDelete(CUIEvent cuiEvent)
        {
            int srcWidgetIndexInBelongedList = cuiEvent.m_srcWidgetIndexInBelongedList;
            stUIEventParams par = new stUIEventParams {
                tag = srcWidgetIndexInBelongedList
            };
            Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(Singleton<CTextManager>.instance.GetText("OB_Desc_7"), enUIEventID.OB_Video_Delete_Confirm, enUIEventID.None, par, false);
        }

        private void OnVideoEnter(CUIEvent cuiEvent)
        {
            if (this.curStatus != enStatus.Editor)
            {
                if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
                }
                else
                {
                    Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(Singleton<CTextManager>.instance.GetText("OB_Desc_11"), enUIEventID.OB_Video_Enter_Confirm, enUIEventID.None, false);
                }
            }
        }

        private void OnVideoEnterConfirm(CUIEvent cuiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(OB_FORM_PATH);
            if (form != null)
            {
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "ContentList");
                if (componetInChild != null)
                {
                    int selectedIndex = componetInChild.GetSelectedIndex();
                    int count = 0;
                    switch (this.CurTab)
                    {
                        case enOBTab.Expert:
                            if ((selectedIndex >= 0) && (selectedIndex < this.OBExpertList.Count))
                            {
                                stOBExpert expert = this.OBExpertList[selectedIndex];
                                Singleton<WatchController>.GetInstance().TargetUID = expert.heroLabel.ullUid;
                                stOBExpert expert2 = this.OBExpertList[selectedIndex];
                                SendOBServeGreat(expert2.desk);
                            }
                            count = this.OBExpertList.Count;
                            break;

                        case enOBTab.Friend:
                            if ((selectedIndex >= 0) && (selectedIndex < this.OBFriendList.Count))
                            {
                                stOBFriend friend = this.OBFriendList[selectedIndex];
                                Singleton<WatchController>.GetInstance().TargetUID = friend.uin.ullUid;
                                stOBFriend friend2 = this.OBFriendList[selectedIndex];
                                SendOBServeFriend(friend2.uin);
                            }
                            count = this.OBFriendList.Count;
                            break;

                        case enOBTab.Local:
                            Singleton<WatchController>.GetInstance().TargetUID = Singleton<CRoleInfoManager>.instance.masterUUID;
                            if ((selectedIndex >= 0) && (selectedIndex < this.OBLocalList.Count))
                            {
                                Singleton<WatchController>.GetInstance().StartReplay(this.OBLocalList[selectedIndex].path);
                            }
                            count = this.OBLocalList.Count;
                            break;
                    }
                }
            }
        }

        public static void SendOBServeFriend(COMDT_ACNT_UNIQ uin)
        {
            if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
            }
            else
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1462);
                msg.stPkgData.stObserveFriendReq.bType = !Singleton<CFriendContoller>.GetInstance().model.IsGameFriend(uin.ullUid, uin.dwLogicWorldId) ? ((byte) 2) : ((byte) 1);
                msg.stPkgData.stObserveFriendReq.stFriendUniq = uin;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        public static void SendOBServeGreat(COMDT_OB_DESK desk)
        {
            if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
            }
            else
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1464);
                msg.stPkgData.stObserveGreatReq.stDesk = desk;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private int SortByObserveNum(stOBExpert left, stOBExpert right)
        {
            return (int) (right.observeNum - left.observeNum);
        }

        public override void UnInit()
        {
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_SNS_STATE_NTF", new Action<CSPkg>(this.On_Friend_SNS_STATE_NTF));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_List", new Action<CSPkg>(this.On_FriendSys_Friend_List));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_GAME_STATE_NTF", new Action<CSPkg>(this.On_Friend_GAME_STATE_NTF));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnOBFormOpen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnOBFormClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Tab_Click, new CUIEventManager.OnUIEventHandler(this.OnOBVideoTabClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Enter, new CUIEventManager.OnUIEventHandler(this.OnVideoEnter));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Enter_Confirm, new CUIEventManager.OnUIEventHandler(this.OnVideoEnterConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Element_Enable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Editor_Click, new CUIEventManager.OnUIEventHandler(this.OnEditorClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Delete, new CUIEventManager.OnUIEventHandler(this.OnVideoDelete));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Delete_Confirm, new CUIEventManager.OnUIEventHandler(this.OnConfirmDelete));
            base.UnInit();
        }

        private void UpdateElement(GameObject element, GameReplayModule.ReplayFileInfo OBLocal)
        {
            this.UpdateElement(element, OBLocal.userName, OBLocal.userHead, OBLocal.userRankGrade, OBLocal.userRankClass, OBLocal.heroId, enOBTab.Local, 0, this.curStatus, OBLocal.startTime, OBLocal.mapType, OBLocal.mapId);
        }

        private void UpdateElement(GameObject element, stOBExpert OBExpert)
        {
            this.UpdateElement(element, Utility.UTF8Convert(OBExpert.heroLabel.szRoleName), Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(Utility.UTF8Convert(OBExpert.heroLabel.szHealUrl)), (byte) OBExpert.heroLabel.dwGrade, OBExpert.heroLabel.dwClass, OBExpert.heroLabel.dwHeroID, enOBTab.Expert, (int) OBExpert.observeNum, this.curStatus, 0L, 0, 0);
        }

        private void UpdateElement(GameObject element, stOBFriend OBFriend)
        {
            if ((CFriendModel.RemarkNames != null) && CFriendModel.RemarkNames.ContainsKey(OBFriend.uin.ullUid))
            {
                string str = string.Empty;
                if (CFriendModel.RemarkNames.TryGetValue(OBFriend.uin.ullUid, out str))
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        this.UpdateElement(element, str, Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(OBFriend.headUrl), OBFriend.gameDetail.bGrade, OBFriend.gameDetail.dwClass, OBFriend.gameDetail.dwHeroID, enOBTab.Friend, (int) OBFriend.gameDetail.dwObserveNum, this.curStatus, 0L, 0, 0);
                    }
                    else
                    {
                        this.UpdateElement(element, OBFriend.friendName, Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(OBFriend.headUrl), OBFriend.gameDetail.bGrade, OBFriend.gameDetail.dwClass, OBFriend.gameDetail.dwHeroID, enOBTab.Friend, (int) OBFriend.gameDetail.dwObserveNum, this.curStatus, 0L, 0, 0);
                    }
                }
            }
            else
            {
                this.UpdateElement(element, OBFriend.friendName, Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(OBFriend.headUrl), OBFriend.gameDetail.bGrade, OBFriend.gameDetail.dwClass, OBFriend.gameDetail.dwHeroID, enOBTab.Friend, (int) OBFriend.gameDetail.dwObserveNum, this.curStatus, 0L, 0, 0);
            }
        }

        private void UpdateElement(GameObject element, string name, string headUrl, byte bGrade, uint subGrade, uint heroId, enOBTab curTab, int onlineNum, enStatus status = 0, long localTicks = 0, byte mapType = 0, uint mapId = 0)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(OB_FORM_PATH);
            if (form != null)
            {
                CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(element, "HeadImg");
                Image image = Utility.GetComponetInChild<Image>(element, "HeroImg");
                Image image2 = Utility.GetComponetInChild<Image>(element, "RankImg");
                Image image3 = Utility.GetComponetInChild<Image>(element, "RankImg/SubRankImg");
                Text text = Utility.GetComponetInChild<Text>(element, "PlayerName");
                Text text2 = Utility.GetComponetInChild<Text>(element, "HeroName");
                GameObject obj2 = Utility.FindChild(element, "WatchImg");
                Text text3 = Utility.GetComponetInChild<Text>(element, "LocalTime");
                Text text4 = Utility.GetComponetInChild<Text>(element, "LocalMap");
                Text text5 = Utility.GetComponetInChild<Text>(element, "WatchImg/OnlineCount");
                GameObject obj3 = Utility.FindChild(element, "DeleteBtn");
                componetInChild.SetImageUrl(headUrl);
                if (bGrade > 0)
                {
                    image2.gameObject.CustomSetActive(true);
                    image2.SetSprite(CLadderView.GetRankSmallIconPath(bGrade, subGrade), form, true, false, false);
                    image3.SetSprite(CLadderView.GetSubRankSmallIconPath(bGrade, subGrade), form, true, false, false);
                }
                else
                {
                    image2.gameObject.CustomSetActive(false);
                }
                text.text = name;
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                if (dataByKey != null)
                {
                    string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, CSkinInfo.GetHeroSkinPic(heroId, 0));
                    image.SetSprite(prefabPath, form, false, true, true);
                    text2.text = dataByKey.szName;
                }
                else
                {
                    text2.text = string.Empty;
                    DebugHelper.Assert(false, string.Format("COBSystem UpdateElement hero cfg[{0}] can not be found!", heroId));
                }
                if (curTab != enOBTab.Local)
                {
                    obj2.CustomSetActive(true);
                    string[] args = new string[] { onlineNum.ToString() };
                    text5.text = Singleton<CTextManager>.instance.GetText("OB_Desc_3", args);
                    text3.gameObject.SetActive(false);
                    obj3.CustomSetActive(false);
                    text4.gameObject.CustomSetActive(false);
                }
                else
                {
                    obj2.CustomSetActive(false);
                    text3.gameObject.SetActive(true);
                    DateTime time = new DateTime(localTicks);
                    string[] textArray2 = new string[] { time.Month.ToString(), time.Day.ToString(), time.Hour.ToString("D2"), time.Minute.ToString("D2") };
                    text3.text = Singleton<CTextManager>.instance.GetText("OB_Desc_12", textArray2);
                    obj3.CustomSetActive(status == enStatus.Editor);
                    text4.gameObject.CustomSetActive(true);
                    ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(mapType, mapId);
                    if (pvpMapCommonInfo != null)
                    {
                        text4.text = pvpMapCommonInfo.szName;
                    }
                    else
                    {
                        text4.text = string.Empty;
                    }
                }
            }
        }

        public void UpdateView()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(OB_FORM_PATH);
            if (form != null)
            {
                GameObject obj2 = Utility.FindChild(form.gameObject, "ContentList/BtnEditor");
                Text componetInChild = Utility.GetComponetInChild<Text>(form.gameObject, "ContentList/BtnEditor/Text");
                CUIListScript script2 = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "ContentList");
                RectTransform component = Utility.FindChild(script2.gameObject, "ScrollRect").GetComponent<RectTransform>();
                if (script2 != null)
                {
                    switch (this.CurTab)
                    {
                        case enOBTab.Expert:
                            obj2.CustomSetActive(false);
                            component.anchoredPosition = Vector2.zero;
                            component.sizeDelta = Vector2.zero;
                            script2.SetElementAmount(this.OBExpertList.Count);
                            break;

                        case enOBTab.Friend:
                            obj2.CustomSetActive(false);
                            component.anchoredPosition = Vector2.zero;
                            component.sizeDelta = Vector2.zero;
                            script2.SetElementAmount(this.OBFriendList.Count);
                            break;

                        case enOBTab.Local:
                            obj2.CustomSetActive(this.OBLocalList.Count > 0);
                            component.sizeDelta = s_content_size;
                            component.anchoredPosition = s_content_pos;
                            componetInChild.text = Singleton<CTextManager>.instance.GetText((this.curStatus != enStatus.Normal) ? "Common_Close" : "Common_Edit");
                            script2.SetElementAmount(this.OBLocalList.Count);
                            break;
                    }
                }
            }
        }

        private enOBTab CurTab
        {
            get
            {
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(OB_FORM_PATH);
                if (form == null)
                {
                    return enOBTab.Null;
                }
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "Panel_Menu/List");
                if (componetInChild == null)
                {
                    return enOBTab.Null;
                }
                return (enOBTab) (componetInChild.GetSelectedIndex() + 1);
            }
        }

        private enum enOBTab
        {
            Null,
            Expert,
            Friend,
            Local
        }

        private enum enStatus
        {
            Normal,
            Editor
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct stOBExpert
        {
            public COMDT_OB_DESK desk;
            public uint startTime;
            public uint observeNum;
            public COMDT_HEROLABEL heroLabel;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct stOBFriend
        {
            public COMDT_ACNT_UNIQ uin;
            public string friendName;
            public string headUrl;
            public COMDT_GAMEINFO_DETAIL gameDetail;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct stOBLocal
        {
            public string path;
            public uint heroId;
        }
    }
}

