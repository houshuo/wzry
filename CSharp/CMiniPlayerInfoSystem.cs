using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CMiniPlayerInfoSystem : Singleton<CMiniPlayerInfoSystem>
{
    private CSPkg m_BackPlayeInfoMsg;
    private bool m_bUp;
    private int m_CurSelectedLogicWorld;
    private ulong m_CurSelectedUuid;
    private OpenSrc m_OpenSrc;
    private CPlayerProfile m_PlayerProfile = new CPlayerProfile();
    public static string sPlayerInfoFormPath = "UGUI/Form/System/Player/Form_Mini_Player_Info.prefab";

    public override void Init()
    {
        base.Init();
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_Open_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenMiniProfile));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_Profile, new CUIEventManager.OnUIEventHandler(this.OnOpenPlayerInfoForm));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_Invite_3v3, new CUIEventManager.OnUIEventHandler(this.OnInvite3v3));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mini_Player_Info_Invite_5v5, new CUIEventManager.OnUIEventHandler(this.OnInvite5v5));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Chat_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Block, new CUIEventManager.OnUIEventHandler(this.OnBlockFriend));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Block_Ok, new CUIEventManager.OnUIEventHandler(this.OnBlockOk));
        Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>(EventID.PlayerInfoSystem_Info_Received, new Action<CSPkg>(this.OnPlayerInfoSystemRecivedMsg));
        Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.PlayerBlock_Success, new System.Action(this.OnPlayerBlockSuccess));
    }

    private void OnAddFriend(CUIEvent uiEvent)
    {
        if ((this.m_CurSelectedUuid > 0L) && (this.m_CurSelectedLogicWorld > 0))
        {
            Singleton<CFriendContoller>.instance.Open_Friend_Verify(this.m_CurSelectedUuid, (uint) this.m_CurSelectedLogicWorld, false);
        }
    }

    private void OnBlockFriend(CUIEvent uiEvent)
    {
        string name = Singleton<CFriendContoller>.instance.model.GetName(this.m_CurSelectedUuid, (uint) this.m_CurSelectedLogicWorld);
        string[] args = new string[] { name };
        string strContent = string.Format(Singleton<CTextManager>.instance.GetText("Black_BlockTip", args), new object[0]);
        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Friend_Block_Ok, enUIEventID.Friend_Block_Cancle, false);
    }

    private void OnBlockOk(CUIEvent uiEvent)
    {
        FriendSysNetCore.Send_Block(this.m_CurSelectedUuid, (uint) this.m_CurSelectedLogicWorld);
    }

    private void OnCloseForm(CUIEvent uiEvent)
    {
        Singleton<CUIManager>.GetInstance().CloseForm(sPlayerInfoFormPath);
        this.m_CurSelectedUuid = 0L;
        this.m_CurSelectedLogicWorld = 0;
        this.m_BackPlayeInfoMsg = null;
    }

    private void OnInvite3v3(CUIEvent uiEvent)
    {
        if ((this.m_CurSelectedUuid > 0L) && (this.m_CurSelectedLogicWorld > 0))
        {
            CUIEvent event2 = new CUIEvent {
                m_eventID = enUIEventID.Matching_OpenEntry
            };
            event2.m_eventParams.tag = 3;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
            if (form == null)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
                return;
            }
            CUIEvent event3 = new CUIEvent {
                m_eventID = enUIEventID.Matching_BtnGroup_Click,
                m_srcFormScript = form
            };
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
            event3 = new CUIEvent {
                m_eventID = enUIEventID.Matching_Begin3v3Team
            };
            uint result = 0;
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_3V3"), out result);
            event3.m_eventParams.tagUInt = result;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
        }
        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
    }

    private void OnInvite5v5(CUIEvent uiEvent)
    {
        if ((this.m_CurSelectedUuid > 0L) && (this.m_CurSelectedLogicWorld > 0))
        {
            CUIEvent event2 = new CUIEvent {
                m_eventID = enUIEventID.Matching_OpenEntry
            };
            event2.m_eventParams.tag = 3;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
            if (form == null)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
                return;
            }
            CUIEvent event3 = new CUIEvent {
                m_eventID = enUIEventID.Matching_BtnGroup_Click,
                m_srcFormScript = form
            };
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
            event3 = new CUIEvent {
                m_eventID = enUIEventID.Matching_Begin5v5Team
            };
            uint result = 0;
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_5V5"), out result);
            event3.m_eventParams.tagUInt = result;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
        }
        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
    }

    private void OnInviteEntertainment(CUIEvent uiEvent)
    {
        if ((this.m_CurSelectedUuid > 0L) && (this.m_CurSelectedLogicWorld > 0))
        {
            CUIEvent event2 = new CUIEvent {
                m_eventID = enUIEventID.Matching_OpenEntry
            };
            event2.m_eventParams.tag = 3;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
            if (form == null)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
                return;
            }
            CUIEvent event3 = new CUIEvent {
                m_eventID = enUIEventID.Matching_BtnGroup_Click
            };
            event3.m_eventParams.tag = 1;
            event3.m_srcFormScript = form;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
            event3 = new CUIEvent {
                m_eventID = enUIEventID.MatchingExt_BeginMelee
            };
            uint result = 0;
            uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_MELEE"), out result);
            event3.m_eventParams.tagUInt = result;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
        }
        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
    }

    private void OnOpenMiniProfile(CUIEvent uiEvent)
    {
        ulong ullUid = 0L;
        int iLogicWorldId = 0;
        this.m_bUp = false;
        this.m_OpenSrc = (OpenSrc) uiEvent.m_eventParams.tag;
        switch (this.m_OpenSrc)
        {
            case OpenSrc.Rank:
                ullUid = uiEvent.m_eventParams.commonUInt64Param1;
                iLogicWorldId = uiEvent.m_eventParams.tag2;
                break;

            case OpenSrc.Chat:
                if (Singleton<CChatController>.GetInstance().view.CurTab == EChatChannel.Lobby)
                {
                    ullUid = uiEvent.m_eventParams.commonUInt64Param1;
                    iLogicWorldId = uiEvent.m_eventParams.tag2;
                    break;
                }
                if (Singleton<CChatController>.GetInstance().view.CurTab == EChatChannel.Guild)
                {
                    ullUid = uiEvent.m_eventParams.commonUInt64Param1;
                    iLogicWorldId = uiEvent.m_eventParams.tag2;
                    break;
                }
                if (Singleton<CChatController>.GetInstance().view.CurTab == EChatChannel.Settle)
                {
                    ullUid = uiEvent.m_eventParams.commonUInt64Param1;
                    iLogicWorldId = uiEvent.m_eventParams.tag2;
                    this.m_bUp = true;
                }
                else if (Singleton<CChatController>.GetInstance().view.CurTab == EChatChannel.Friend_Chat)
                {
                    CChatSysData sysData = Singleton<CChatController>.GetInstance().model.sysData;
                    if (sysData == null)
                    {
                        Debug.LogError("Open mini profile failed, CChatSysData is null");
                        return;
                    }
                    ullUid = sysData.ullUid;
                    iLogicWorldId = (int) sysData.dwLogicWorldId;
                }
                break;
        }
        if ((ullUid > 0L) && ((ullUid != this.m_CurSelectedUuid) || (iLogicWorldId != this.m_CurSelectedLogicWorld)))
        {
            this.m_CurSelectedUuid = ullUid;
            this.m_CurSelectedLogicWorld = iLogicWorldId;
            Singleton<CPlayerInfoSystem>.GetInstance().ReqOtherPlayerDetailInfo(ullUid, iLogicWorldId, false, true);
        }
    }

    private void OnOpenPlayerInfoForm(CUIEvent uiEvent)
    {
        if ((this.m_CurSelectedUuid > 0L) && (this.m_CurSelectedLogicWorld > 0))
        {
            if (this.m_BackPlayeInfoMsg != null)
            {
                Singleton<CPlayerInfoSystem>.GetInstance().ImpResDetailInfo(this.m_BackPlayeInfoMsg);
            }
            else
            {
                Singleton<CPlayerInfoSystem>.GetInstance().ReqOtherPlayerDetailInfo(this.m_CurSelectedUuid, this.m_CurSelectedLogicWorld, true, true);
            }
        }
    }

    private void OnPlayerBlockSuccess()
    {
        if (this.m_BackPlayeInfoMsg != null)
        {
            this.OnPlayerInfoSystemRecivedMsg(this.m_BackPlayeInfoMsg);
        }
    }

    private void OnPlayerInfoSystemRecivedMsg(CSPkg msg)
    {
        Text text3;
        if (msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode != 0)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format("Error Code {0}", msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode), false);
            return;
        }
        this.m_BackPlayeInfoMsg = msg;
        this.m_PlayerProfile.ConvertServerDetailData(msg.stPkgData.stGetAcntDetailInfoRsp.stAcntDetail.stOfSucc);
        CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(sPlayerInfoFormPath, false, true);
        if (script == null)
        {
            return;
        }
        if (this.m_bUp)
        {
            script.SetPriority(enFormPriority.Priority5);
        }
        else
        {
            script.RestorePriority();
        }
        GameObject widget = script.GetWidget(0);
        RectTransform transform = script.transform.Find("panel") as RectTransform;
        if (transform == null)
        {
            Debug.LogError("mini player info form's panel is null");
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
        }
        OpenSrc openSrc = this.m_OpenSrc;
        if (openSrc == OpenSrc.Rank)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(RankingSystem.s_rankingForm);
            if (form == null)
            {
                Debug.LogError("can't get ranking form");
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
                return;
            }
            RectTransform transform3 = form.transform.Find("bg") as RectTransform;
            if (transform3 == null)
            {
                Debug.LogError("ranking form's bg is null");
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
                return;
            }
            widget.CustomSetActive(true);
            Vector3[] fourCornersArray = new Vector3[4];
            transform3.GetWorldCorners(fourCornersArray);
            Vector2 vector3 = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, fourCornersArray[2]);
            Vector2 screenPoint = new Vector2(0f, vector3.y - 100f);
            if (((vector3.x + 80f) + transform.rect.width) > Screen.width)
            {
                screenPoint.x = (Screen.width - transform.rect.width) - 15f;
            }
            else
            {
                screenPoint.x = vector3.x + 80f;
            }
            transform.position = CUIUtility.ScreenToWorldPoint(script.GetCamera(), screenPoint, transform.position.z);
        }
        else if (openSrc == OpenSrc.Chat)
        {
            CUIFormScript script2 = Singleton<CUIManager>.GetInstance().GetForm(CChatController.ChatFormPath);
            if (script2 == null)
            {
                Debug.LogError("can't get chat form");
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
                return;
            }
            RectTransform transform2 = script2.transform.Find("node/null") as RectTransform;
            if (transform2 == null)
            {
                Debug.LogError("chat form's close btn is null");
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mini_Player_Info_CloseForm);
                return;
            }
            widget.CustomSetActive(true);
            Vector3[] vectorArray = new Vector3[4];
            transform2.GetWorldCorners(vectorArray);
            Vector2 vector = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, vectorArray[3]);
            Vector2 vector2 = new Vector2(0f, vector.y);
            if (((vector.x + transform.rect.width) + 100f) > Screen.width)
            {
                vector2.x = (Screen.width - transform.rect.width) - 15f;
            }
            else
            {
                vector2 = new Vector2(vector.x + 20f, vector.y);
            }
            transform.position = CUIUtility.ScreenToWorldPoint(script.GetCamera(), vector2, transform.position.z);
        }
        Text componetInChild = Utility.GetComponetInChild<Text>(script.gameObject, "panel/Name/Text");
        if (componetInChild != null)
        {
            componetInChild.text = this.m_PlayerProfile.Name();
        }
        COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.GameFriend, this.m_CurSelectedUuid, (uint) this.m_CurSelectedLogicWorld);
        COMDT_FRIEND_INFO comdt_friend_info2 = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.SNS, this.m_CurSelectedUuid, (uint) this.m_CurSelectedLogicWorld);
        Text text2 = Utility.GetComponetInChild<Text>(script.gameObject, "panel/Online/Text");
        if (text2 != null)
        {
            if (this.m_PlayerProfile.IsOnLine())
            {
                if (((comdt_friend_info != null) ? comdt_friend_info : ((comdt_friend_info2 != null) ? comdt_friend_info2 : null)) != null)
                {
                    COM_ACNT_GAME_STATE friendInGamingState = Singleton<CFriendContoller>.instance.model.GetFriendInGamingState(this.m_CurSelectedUuid, (uint) this.m_CurSelectedLogicWorld);
                    switch (friendInGamingState)
                    {
                        case COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE:
                            text2.text = string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online"));
                            goto Label_0558;

                        case COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_SINGLEGAME:
                        case COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_MULTIGAME:
                            text2.text = string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Gaming"));
                            goto Label_0558;
                    }
                    if (friendInGamingState == COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_TEAM)
                    {
                        text2.text = string.Format("<color=#ffff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Teaming"));
                    }
                }
                else
                {
                    text2.text = string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online"));
                }
            }
            else
            {
                string text = Singleton<CTextManager>.GetInstance().GetText("Common_Offline");
                text2.text = text;
                text2.text = text;
            }
        }
    Label_0558:
        text3 = Utility.GetComponetInChild<Text>(script.gameObject, "panel/DuanWei/Text");
        if (text3 != null)
        {
            string rankTitle = CLadderView.GetRankTitle(this.m_PlayerProfile.GetRankGrade(), this.m_PlayerProfile.GetRankClass());
            text3.text = !string.IsNullOrEmpty(rankTitle) ? rankTitle : Singleton<CTextManager>.GetInstance().GetText("Common_NoData");
        }
        Text text4 = Utility.GetComponetInChild<Text>(script.gameObject, "panel/Team/Text");
        Text text5 = Utility.GetComponetInChild<Text>(script.gameObject, "panel/Position/Text");
        if (!CGuildSystem.IsInNormalGuild(this.m_PlayerProfile.GuildState) || string.IsNullOrEmpty(this.m_PlayerProfile.GuildName))
        {
            if (text4 != null)
            {
                text4.text = Singleton<CTextManager>.GetInstance().GetText("PlayerInfo_Guild");
            }
            if (text5 != null)
            {
                text5.text = Singleton<CTextManager>.GetInstance().GetText("PlayerInfo_Guild");
            }
        }
        else
        {
            if (text4 != null)
            {
                text4.text = this.m_PlayerProfile.GuildName;
            }
            if (text5 != null)
            {
                text5.text = CGuildHelper.GetPositionName(this.m_PlayerProfile.GuildState);
            }
        }
        GameObject obj3 = Utility.FindChild(script.gameObject, "panel/Btn/AddFriend");
        GameObject obj4 = Utility.FindChild(script.gameObject, "panel/Btn/Profile");
        GameObject obj5 = Utility.FindChild(script.gameObject, "panel/Btn/3v3");
        GameObject obj6 = Utility.FindChild(script.gameObject, "panel/Btn/5v5");
        GameObject obj7 = Utility.FindChild(script.gameObject, "panel/Btn/Block");
        obj4.CustomSetActive(true);
        switch (this.m_OpenSrc)
        {
            case OpenSrc.Rank:
                if ((comdt_friend_info != null) || (comdt_friend_info2 != null))
                {
                    obj3.CustomSetActive(false);
                    obj5.CustomSetActive(true);
                    obj6.CustomSetActive(true);
                }
                else
                {
                    obj3.CustomSetActive(true);
                    obj5.CustomSetActive(false);
                    obj6.CustomSetActive(false);
                }
                obj7.CustomSetActive(false);
                break;

            case OpenSrc.Chat:
                switch (Singleton<CChatController>.GetInstance().view.CurTab)
                {
                    case EChatChannel.Lobby:
                        obj5.CustomSetActive(false);
                        obj6.CustomSetActive(false);
                        if ((comdt_friend_info == null) && (comdt_friend_info2 == null))
                        {
                            obj3.CustomSetActive(true);
                        }
                        else
                        {
                            obj3.CustomSetActive(false);
                        }
                        obj7.CustomSetActive(true);
                        this.SetBlockButtonBlocked(obj7, Singleton<CFriendContoller>.instance.model.IsBlack(this.m_CurSelectedUuid, (uint) this.m_CurSelectedLogicWorld));
                        return;

                    case EChatChannel.Friend:
                    case EChatChannel.Select_Hero:
                    case EChatChannel.Speaker:
                        return;

                    case EChatChannel.Guild:
                        if ((comdt_friend_info == null) && (comdt_friend_info2 == null))
                        {
                            obj3.CustomSetActive(true);
                            obj5.CustomSetActive(false);
                            obj6.CustomSetActive(false);
                        }
                        else
                        {
                            obj3.CustomSetActive(false);
                            obj5.CustomSetActive(true);
                            obj6.CustomSetActive(true);
                        }
                        obj7.CustomSetActive(true);
                        this.SetBlockButtonBlocked(obj7, Singleton<CFriendContoller>.instance.model.IsBlack(this.m_CurSelectedUuid, (uint) this.m_CurSelectedLogicWorld));
                        return;

                    case EChatChannel.Friend_Chat:
                        obj3.CustomSetActive(false);
                        obj5.CustomSetActive(true);
                        obj6.CustomSetActive(true);
                        obj7.CustomSetActive(true);
                        this.SetBlockButtonBlocked(obj7, Singleton<CFriendContoller>.instance.model.IsBlack(this.m_CurSelectedUuid, (uint) this.m_CurSelectedLogicWorld));
                        return;

                    case EChatChannel.Settle:
                        obj3.CustomSetActive(false);
                        obj5.CustomSetActive(false);
                        obj6.CustomSetActive(false);
                        obj7.CustomSetActive(true);
                        this.SetBlockButtonBlocked(obj7, Singleton<CFriendContoller>.instance.model.IsBlack(this.m_CurSelectedUuid, (uint) this.m_CurSelectedLogicWorld));
                        return;
                }
                break;
        }
    }

    private void SetBlockButtonBlocked(GameObject blockNode, bool bHasBlocked)
    {
        if (blockNode != null)
        {
            CUIEventScript component = blockNode.GetComponent<CUIEventScript>();
            if (component != null)
            {
                component.enabled = !bHasBlocked;
            }
            Button btn = blockNode.GetComponent<Button>();
            if (btn != null)
            {
                CUICommonSystem.SetButtonEnableWithShader(btn, !bHasBlocked, true);
            }
            string str = !bHasBlocked ? "屏蔽" : "已屏蔽";
            Text text = blockNode.transform.Find("Text").GetComponent<Text>();
            if (text != null)
            {
                text.text = str;
            }
        }
    }

    public override void UnInit()
    {
        base.UnInit();
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_Open_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenMiniProfile));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_Profile, new CUIEventManager.OnUIEventHandler(this.OnOpenPlayerInfoForm));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_Invite_3v3, new CUIEventManager.OnUIEventHandler(this.OnInvite3v3));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mini_Player_Info_Invite_5v5, new CUIEventManager.OnUIEventHandler(this.OnInvite5v5));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Chat_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Chat_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Block, new CUIEventManager.OnUIEventHandler(this.OnBlockFriend));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Block_Ok, new CUIEventManager.OnUIEventHandler(this.OnBlockOk));
        Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>(EventID.PlayerInfoSystem_Info_Received, new Action<CSPkg>(this.OnPlayerInfoSystemRecivedMsg));
        Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.PlayerBlock_Success, new System.Action(this.OnPlayerBlockSuccess));
    }

    public enum OpenSrc
    {
        None,
        Rank,
        Chat
    }
}

