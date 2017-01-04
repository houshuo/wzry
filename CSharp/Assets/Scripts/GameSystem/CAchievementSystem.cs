namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CAchievementSystem : Singleton<CAchievementSystem>
    {
        private const string AchievementListFormPrefabPath = "UGUI/Form/System/Achieve/Form_Achievement_List.prefab";
        private const string AchievementShareFormPrefabPath = "UGUI/Form/System/Achieve/Form_Achievement_ShareNewAchievement.prefab";
        private const string AchievementTypeFormPrefabPath = "UGUI/Form/System/Achieve/Form_Achievement_Type.prefab";
        private int m_curAchievementType;
        private ListView<CAchieveItem> m_oneTypeAchieveItems = new ListView<CAchieveItem>();
        private uint m_worldRank;

        private float GetProcessCircleFillAmount(int finish, int total)
        {
            if (total == 0)
            {
                return 0f;
            }
            double num = 0.05;
            double num2 = 0.9;
            return (float) (num + ((((float) finish) / ((float) total)) * num2));
        }

        private void HandleAchieveGetRankingAccountInfo(SCPKG_GET_RANKING_ACNT_INFO_RSP rsp)
        {
            if (rsp.stAcntRankingDetail.stOfSucc.bNumberType == 8)
            {
                this.m_worldRank = rsp.stAcntRankingDetail.stOfSucc.dwRankNo;
            }
        }

        private void HandleLobbyStateEnter()
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_STATE_UPDATE);
        }

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Open_Type_Form, new CUIEventManager.OnUIEventHandler(this.OnAchievementOpenTypeForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Open_List_Form, new CUIEventManager.OnUIEventHandler(this.OnAchievementOpenListForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.OnAchievementListElementEnabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_Get_Award, new CUIEventManager.OnUIEventHandler(this.OnAchievementGetAward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Achievement_ShowShareBtn, new CUIEventManager.OnUIEventHandler(this.OnAchievementShowShareBtn));
            Singleton<EventRouter>.instance.AddEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>(EventID.ACHIEVE_GET_RANKING_ACCOUNT_INFO, new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.HandleAchieveGetRankingAccountInfo));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.LOBBY_STATE_ENTER, new System.Action(this.HandleLobbyStateEnter));
        }

        private void OnAchievementGetAward(CUIEvent uiEvent)
        {
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListScript;
            if (srcWidgetScript == null)
            {
                DebugHelper.Assert(false, "CAchievementSystem.OnAchievementGetAward(): listScript is null");
            }
            else if ((srcWidgetScript.GetSelectedIndex() >= this.m_oneTypeAchieveItems.Count) || (srcWidgetScript.GetSelectedIndex() < 0))
            {
                object[] inParameters = new object[] { srcWidgetScript.GetSelectedIndex() };
                DebugHelper.Assert(false, "CAchievementSystem.OnAchievementGetAward(): list index out of range: {0}", inParameters);
            }
            else
            {
                CAchieveItem item = this.m_oneTypeAchieveItems[srcWidgetScript.GetSelectedIndex()];
                if (item.IsCanGetReward())
                {
                    SendGetAchieveRewardReq(item.m_cfgId);
                }
            }
        }

        private void OnAchievementListElementEnabled(CUIEvent uiEvent)
        {
            CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
            if ((uiEvent.m_srcWidgetIndexInBelongedList >= this.m_oneTypeAchieveItems.Count) || (uiEvent.m_srcWidgetIndexInBelongedList < 0))
            {
                object[] inParameters = new object[] { uiEvent.m_srcWidgetIndexInBelongedList };
                DebugHelper.Assert(false, "CAchievementSystem.OnAchievementListElementEnabled(): m_srcWidgetIndexInBelongedList out of range: {0}", inParameters);
            }
            else
            {
                CAchieveItem achieveItem = this.m_oneTypeAchieveItems[uiEvent.m_srcWidgetIndexInBelongedList];
                this.SetAchievementListItem(srcWidgetScript, achieveItem);
            }
        }

        private void OnAchievementOpenListForm(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.tag == 1)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Common_System_Not_Open_Tip", true, 1.5f, null, new object[0]);
            }
            else
            {
                this.m_curAchievementType = uiEvent.m_srcWidget.transform.GetSiblingIndex() + 1;
                this.OpenListForm();
            }
        }

        private void OnAchievementOpenTypeForm(CUIEvent uiEvent)
        {
            this.OpenTypeForm();
        }

        private void OnAchievementShowShareBtn(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                srcFormScript.GetWidget(2).CustomSetActive(true);
                srcFormScript.GetWidget(1).CustomSetActive(false);
            }
        }

        [MessageHandler(0x1133)]
        public static void OnNotifyAchieveDoneDataChange(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_ACHIEVEMENT_DONE_DATA_CHG_NTF stAchievementDoneDataChgNtf = msg.stPkgData.stAchievementDoneDataChgNtf;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "role is null OnNotifyAchieveDoneDataChange");
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_achieveInfo.OnAchieveDoneDataChange(stAchievementDoneDataChgNtf.stAchievementDoneData);
            }
        }

        [MessageHandler(0x1132)]
        public static void OnNotifyAchieveStateChange(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_ACHIEVEMENT_STATE_CHG_NTF stAchievementStateChgNtf = msg.stPkgData.stAchievementStateChgNtf;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "role is null OnNotifyAchieveStateChange");
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_achieveInfo.OnAchieveStateChange(stAchievementStateChgNtf.stAchievementData);
                if (Singleton<GameStateCtrl>.GetInstance().isLobbyState)
                {
                    Singleton<CAchievementSystem>.GetInstance().RefreshListForm(null);
                    Singleton<CAchievementSystem>.GetInstance().RefreshTypeForm(null);
                    Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.ACHIEVE_STATE_UPDATE);
                }
            }
        }

        [MessageHandler(0x1135)]
        public static void OnNotifyGetAchieveReward(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_GET_ACHIEVEMENT_REWARD_RSP stGetAchievementRewardRsp = msg.stPkgData.stGetAchievementRewardRsp;
            if (stGetAchievementRewardRsp.iResult == 0)
            {
                Singleton<CAchievementSystem>.GetInstance().OpenShareForm(stGetAchievementRewardRsp.dwAchievementID);
            }
        }

        [MessageHandler(0x1131)]
        public static void OnRecieveAchieveInfo(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_ACHIEVEMENT_INFO_NTF stGetAchievememtInfoNtf = msg.stPkgData.stGetAchievememtInfoNtf;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "role is null OnRecieveAchieveInfo");
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_achieveInfo.InitAchieveInfo(stGetAchievememtInfoNtf.stAchievementInfo);
            }
        }

        private void OpenListForm()
        {
            CUIFormScript listForm = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Achievement_List.prefab", false, true);
            if (listForm != null)
            {
                this.RefreshListForm(listForm);
            }
        }

        private void OpenShareForm(uint achievementId)
        {
            CUIFormScript shareForm = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Achievement_ShareNewAchievement.prefab", false, true);
            if (shareForm != null)
            {
                this.RefreshShareForm(shareForm, achievementId);
            }
        }

        private void OpenTypeForm()
        {
            CUIFormScript typeForm = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_Achievement_Type.prefab", false, true);
            if (typeForm != null)
            {
                this.RefreshTypeForm(typeForm);
            }
        }

        private void RefreshAwardPanel(CUIFormScript shareForm, uint achievementId)
        {
            ResAchievement dataByKey = GameDataMgr.achieveDatabin.GetDataByKey(achievementId);
            if (dataByKey != null)
            {
                ListView<CUseable> view = new ListView<CUseable>();
                CUseable item = CUseableManager.CreateVirtualUseable(enVirtualItemType.enAchievementPoint, (int) dataByKey.dwPoint);
                view.Add(item);
                for (int i = 0; i < dataByKey.astReward.Length; i++)
                {
                    if ((dataByKey.astReward[i].bRewardType != 0) && (dataByKey.astReward[i].dwRewardNum > 0))
                    {
                        CUseable useable2 = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE) dataByKey.astReward[i].bRewardType, (int) dataByKey.astReward[i].dwRewardNum, dataByKey.astReward[i].dwRewardID);
                        if (useable2 != null)
                        {
                            view.Add(useable2);
                        }
                    }
                }
                CUIListScript component = shareForm.GetWidget(11).GetComponent<CUIListScript>();
                component.SetElementAmount(view.Count);
                for (int j = 0; j < view.Count; j++)
                {
                    CUIListElementScript elemenet = component.GetElemenet(j);
                    if (elemenet != null)
                    {
                        GameObject gameObject = elemenet.gameObject;
                        CUICommonSystem.SetItemCell(shareForm, gameObject, view[j], true, false);
                    }
                }
            }
        }

        private void RefreshListForm(CUIFormScript listForm = null)
        {
            if (listForm == null)
            {
                listForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Achievement_List.prefab");
            }
            if (listForm != null)
            {
                listForm.GetWidget(1).GetComponent<Text>().text = CAchieveItem.GetAchievementTypeName(this.m_curAchievementType);
                this.m_oneTypeAchieveItems = CAchieveInfo.GetAchieveInfo().GetNeedShowAchieveItemsByType(this.m_curAchievementType);
                this.m_oneTypeAchieveItems.Sort();
                listForm.GetWidget(0).GetComponent<CUIListScript>().SetElementAmount(this.m_oneTypeAchieveItems.Count);
            }
        }

        private void RefreshShareForm(CUIFormScript shareForm, uint achievementId)
        {
            if (shareForm == null)
            {
                shareForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Achievement_ShareNewAchievement.prefab");
            }
            if (shareForm != null)
            {
                CAchieveItem achieveItemById = CAchieveInfo.GetAchieveInfo().GetAchieveItemById(achievementId);
                if (achieveItemById != null)
                {
                    shareForm.GetWidget(0).GetComponent<Text>().text = achieveItemById.GetAchievementName();
                    shareForm.GetWidget(5).GetComponent<Text>().text = achieveItemById.GetAchievementDesc();
                    shareForm.GetWidget(7).GetComponent<Text>().text = achieveItemById.GetAchievementTips();
                    shareForm.GetWidget(9).GetComponent<Image>().SetSprite(achieveItemById.GetAchievementBgIconPath(), shareForm, true, false, false);
                    GameObject widget = shareForm.GetWidget(6);
                    if (achieveItemById.IsHideForegroundIcon())
                    {
                        widget.CustomSetActive(false);
                    }
                    else
                    {
                        widget.CustomSetActive(true);
                        widget.GetComponent<Image>().SetSprite(achieveItemById.GetAchievementIconPath(), shareForm, true, false, false);
                    }
                    this.RefreshAwardPanel(shareForm, achievementId);
                    ShareSys.SetSharePlatfText(shareForm.GetWidget(10).GetComponent<Text>());
                    if (CSysDynamicBlock.bSocialBlocked)
                    {
                        Transform transform = shareForm.transform.Find("Panel_ShareAchievement_Btn");
                        if (transform != null)
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                        Transform transform2 = shareForm.transform.Find("Panel_NewAchievement_Btn/Btn_Share");
                        if (transform2 != null)
                        {
                            transform2.gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void RefreshTypeForm(CUIFormScript typeForm = null)
        {
            if (typeForm == null)
            {
                typeForm = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Achieve/Form_Achievement_Type.prefab");
            }
            if (typeForm != null)
            {
                CAchieveInfo achieveInfo = CAchieveInfo.GetAchieveInfo();
                int gotRewardAchievePoint = achieveInfo.GetGotRewardAchievePoint(0);
                int totalAchievePoint = achieveInfo.GetTotalAchievePoint(0);
                string[] args = new string[] { gotRewardAchievePoint.ToString(), totalAchievePoint.ToString() };
                typeForm.GetWidget(0).GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Achievement_My_Point", args);
                string[] textArray2 = new string[] { (this.m_worldRank != 0) ? this.m_worldRank.ToString() : Singleton<CTextManager>.GetInstance().GetText("Common_Not_In_Rank") };
                typeForm.GetWidget(1).GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Achievement_World_Rank", textArray2);
                Transform transform = typeForm.GetWidget(2).transform;
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    int achieveType = i + 1;
                    int finish = achieveInfo.GetGotRewardAchievePoint(achieveType);
                    int total = achieveInfo.GetTotalAchievePoint(achieveType);
                    string[] textArray3 = new string[] { finish.ToString(), total.ToString() };
                    child.Find("txtProcess").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Achievement_Type_Process_Format", textArray3);
                    if ((finish < total) || (total == 0))
                    {
                        child.Find("imgProcessCircle").GetComponent<Image>().fillAmount = this.GetProcessCircleFillAmount(finish, total);
                        if (total == 0)
                        {
                            child.Find("imgNotFinishBg").GetComponent<Image>().color = Color.grey;
                            child.Find("imgIcon").GetComponent<Image>().color = Color.grey;
                            child.GetComponent<CUIEventScript>().m_onClickEventParams.tag = 1;
                            if (CSysDynamicBlock.bUnfinishBlock)
                            {
                                child.gameObject.CustomSetActive(false);
                            }
                        }
                    }
                    else
                    {
                        child.Find("imgNotFinishBg").gameObject.CustomSetActive(false);
                        child.Find("imgProcessCircle").gameObject.CustomSetActive(false);
                        child.Find("imgFinishBg").gameObject.CustomSetActive(true);
                        child.Find("imgAllComplishMark").gameObject.CustomSetActive(true);
                    }
                    child.Find("imgRedPoint").gameObject.CustomSetActive(achieveInfo.IsHaveFinishButNotGetRewardAchievement(achieveType));
                }
            }
        }

        public static void SendGetAchieveRewardReq(uint achieveId)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1134);
            msg.stPkgData.stGetAchievementRewardReq = new CSPKG_GET_ACHIEVEMENT_REWARD_REQ();
            msg.stPkgData.stGetAchievementRewardReq.dwAchievementID = achieveId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void SendReqGetRankingAcountInfo()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2c);
            msg.stPkgData.stGetRankingAcntInfoReq.bNumberType = 8;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void SetAchievementAwardTipsEvent(CUIEventScript eventScript, CUseable useable)
        {
            stUIEventParams eventParams = new stUIEventParams {
                iconUseable = useable,
                tag = 0
            };
            eventScript.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
            eventScript.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
            eventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
            eventScript.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
        }

        private void SetAchievementListItem(CUIListElementScript listElement, CAchieveItem achieveItem)
        {
            Transform transform = listElement.transform;
            GameObject gameObject = transform.Find("Panel_NotGotAward").gameObject;
            GameObject obj3 = transform.Find("Panel_GotAward").gameObject;
            GameObject obj4 = transform.Find("imgIconBg/imgIcon").gameObject;
            Image component = transform.Find("imgIconBg").GetComponent<Image>();
            Image image = transform.Find("imgIconBg/imgIcon").GetComponent<Image>();
            Text text = transform.Find("txtName").GetComponent<Text>();
            Text text2 = transform.Find("txtDesc").GetComponent<Text>();
            Text text3 = transform.Find("txtProcess").GetComponent<Text>();
            if (achieveItem.GetAchieveState() == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_REWARD)
            {
                obj3.CustomSetActive(true);
                gameObject.CustomSetActive(false);
            }
            else
            {
                gameObject.CustomSetActive(true);
                obj3.CustomSetActive(false);
            }
            component.SetSprite(achieveItem.GetAchievementBgIconPath(), listElement.m_belongedFormScript, true, false, false);
            if (achieveItem.IsHideForegroundIcon())
            {
                obj4.CustomSetActive(false);
            }
            else
            {
                obj4.CustomSetActive(true);
                image.SetSprite(achieveItem.GetAchievementIconPath(), listElement.m_belongedFormScript, true, false, false);
            }
            text.text = achieveItem.GetAchievementName();
            text2.text = achieveItem.GetAchievementDesc();
            if (achieveItem.m_cfgInfo.dwClassification == 2)
            {
                text3.gameObject.CustomSetActive(true);
                if (achieveItem.IsFinish())
                {
                    string[] args = new string[] { achieveItem.m_cfgInfo.dwDoneCondi.ToString(), achieveItem.m_cfgInfo.dwDoneCondi.ToString() };
                    text3.text = Singleton<CTextManager>.GetInstance().GetText("Achievement_Type_Process_Format", args);
                }
                else
                {
                    string[] textArray2 = new string[] { achieveItem.GetAchieveDoneCnt().ToString(), achieveItem.m_cfgInfo.dwDoneCondi.ToString() };
                    text3.text = Singleton<CTextManager>.GetInstance().GetText("Achievement_Type_Process_Format", textArray2);
                }
            }
            else
            {
                text3.gameObject.CustomSetActive(false);
            }
            this.SetAchievementListItemState(listElement, achieveItem);
            this.SetAchievementListItemAward(listElement, achieveItem);
        }

        private void SetAchievementListItemAward(CUIListElementScript listElement, CAchieveItem achieveItem)
        {
            Transform transform = listElement.transform;
            GameObject gameObject = transform.Find("pnlAward/pnlPoint").gameObject;
            GameObject obj3 = transform.Find("pnlAward/pnlCoin").gameObject;
            GameObject obj4 = transform.Find("pnlAward/pnlDiamond").gameObject;
            GameObject obj5 = transform.Find("pnlAward/pnlItem").gameObject;
            GameObject obj6 = transform.Find("pnlAward/pnlSkin").gameObject;
            uint dwPoint = achieveItem.m_cfgInfo.dwPoint;
            uint achievementAwardCnt = achieveItem.GetAchievementAwardCnt(RES_REWARDS_TYPE.RES_REWARDS_TYPE_HONOUR);
            uint num3 = achieveItem.GetAchievementAwardCnt(RES_REWARDS_TYPE.RES_REWARDS_TYPE_DIAMOND);
            uint num4 = achieveItem.GetAchievementAwardCnt(RES_REWARDS_TYPE.RES_REWARDS_TYPE_ITEM);
            uint num5 = achieveItem.GetAchievementAwardCnt(RES_REWARDS_TYPE.RES_REWARDS_TYPE_SKIN);
            gameObject.transform.Find("txtNum").GetComponent<Text>().text = dwPoint.ToString();
            CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
            CUseable useable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enAchievementPoint, (int) dwPoint);
            this.SetAchievementAwardTipsEvent(component, useable);
            if (achievementAwardCnt > 0)
            {
                obj3.CustomSetActive(true);
                obj3.GetComponent<LayoutElement>().ignoreLayout = false;
                obj3.transform.Find("txtNum").GetComponent<Text>().text = achievementAwardCnt.ToString();
                CUIEventScript eventScript = obj3.GetComponent<CUIEventScript>();
                CUseable useable2 = CUseableManager.CreateVirtualUseable(enVirtualItemType.enGoldCoin, (int) achievementAwardCnt);
                this.SetAchievementAwardTipsEvent(eventScript, useable2);
            }
            else
            {
                obj3.CustomSetActive(false);
                obj3.GetComponent<LayoutElement>().ignoreLayout = true;
            }
            if (num3 > 0)
            {
                obj4.CustomSetActive(true);
                obj4.GetComponent<LayoutElement>().ignoreLayout = false;
                obj4.transform.Find("txtNum").GetComponent<Text>().text = num3.ToString();
                CUIEventScript script3 = obj4.GetComponent<CUIEventScript>();
                CUseable useable3 = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDiamond, (int) num3);
                this.SetAchievementAwardTipsEvent(script3, useable3);
            }
            else
            {
                obj4.CustomSetActive(false);
                obj4.GetComponent<LayoutElement>().ignoreLayout = true;
            }
            if (num4 > 0)
            {
                obj5.CustomSetActive(true);
                obj5.GetComponent<LayoutElement>().ignoreLayout = false;
                obj5.transform.Find("txtNum").GetComponent<Text>().text = num4.ToString();
                string prefabPath = CUIUtility.s_Sprite_Dynamic_Icon_Dir + achieveItem.GetAchievementAwardId(RES_REWARDS_TYPE.RES_REWARDS_TYPE_ITEM);
                obj5.transform.Find("imgIcon").GetComponent<Image>().SetSprite(prefabPath, listElement.m_belongedFormScript, true, false, false);
            }
            else
            {
                obj5.CustomSetActive(false);
                obj5.GetComponent<LayoutElement>().ignoreLayout = true;
            }
            if (num5 > 0)
            {
                obj6.CustomSetActive(true);
                obj6.GetComponent<LayoutElement>().ignoreLayout = false;
                obj6.transform.Find("txtNum").GetComponent<Text>().text = num5.ToString();
                string str2 = CUIUtility.s_Sprite_Dynamic_Icon_Dir + achieveItem.GetAchievementAwardId(RES_REWARDS_TYPE.RES_REWARDS_TYPE_SKIN);
                obj6.transform.Find("imgIcon").GetComponent<Image>().SetSprite(str2, listElement.m_belongedFormScript, true, false, false);
            }
            else
            {
                obj6.CustomSetActive(false);
                obj6.GetComponent<LayoutElement>().ignoreLayout = true;
            }
        }

        private void SetAchievementListItemState(CUIListElementScript listElement, CAchieveItem achieveItem)
        {
            Transform transform = listElement.transform;
            GameObject gameObject = transform.Find("btnGet").gameObject;
            GameObject obj3 = transform.Find("imgAwardMark").gameObject;
            GameObject obj4 = transform.Find("txtInProcess").gameObject;
            if (achieveItem.GetAchieveState() == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_REWARD)
            {
                obj3.CustomSetActive(true);
                gameObject.CustomSetActive(false);
                obj4.CustomSetActive(false);
            }
            else if (achieveItem.GetAchieveState() == COM_ACHIEVEMENT_STATE.COM_ACHIEVEMENT_STATE_FIN)
            {
                obj3.CustomSetActive(false);
                gameObject.CustomSetActive(true);
                obj4.CustomSetActive(false);
            }
            else
            {
                obj3.CustomSetActive(false);
                gameObject.CustomSetActive(false);
                obj4.CustomSetActive(true);
                if (achieveItem.m_cfgInfo.dwClassification == 2)
                {
                    obj4.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Achievement_State_In_Process");
                }
                else
                {
                    obj4.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Achievement_State_Unfinish");
                }
            }
        }

        public enum enAchievementListFormWidget
        {
            Achievement_List,
            Title_Text
        }

        public enum enAchievementShareFormWidget
        {
            Name_Text,
            New_Achievement_Btn_Panel,
            Share_Achievement_Btn_Panel,
            Close_Btn,
            ScreenShotFrame_Panel,
            Desc_Text,
            Icon_Image,
            Tips_Text,
            Confirm_Btn,
            Icon_Bg_Image,
            ShareToFriendArea_Text,
            Award_List
        }

        public enum enAchievementTypeFormWidget
        {
            My_Point_Text,
            World_Rank_Text,
            Type_Panel
        }
    }
}

