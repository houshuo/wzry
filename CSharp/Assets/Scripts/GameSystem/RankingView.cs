namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using UnityEngine;
    using UnityEngine.UI;

    public class RankingView
    {
        public static string s_ChooseHeroPath = "UGUI/Form/System/CustomRecommendEquip/Form_ChooseHero.prefab";
        private static Vector2 s_ListPos1 = new Vector2(10f, 0f);
        private static Vector2 s_ListPos2 = new Vector2(10f, -100f);
        private static Vector2 s_ListSize1 = new Vector2(-20f, 0f);
        private static Vector2 s_ListSize2 = new Vector2(-20f, -100f);

        public static void HideAllRankMenu()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
            if (form != null)
            {
                form.m_formWidgets[14].CustomSetActive(false);
            }
        }

        public static void InitRankGodDetailTab()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingGodDetailForm);
            if (form != null)
            {
                GameObject p = Utility.FindChild(form.GetWidget(0), "TitleTab");
                string[] titleList = new string[] { Singleton<CTextManager>.instance.GetText("Ranking_God_Tips_3"), Singleton<CTextManager>.instance.GetText("Ranking_God_Tips_4") };
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(p, "TitleTab");
                CUICommonSystem.InitMenuPanel(p, titleList, 0);
            }
        }

        public static bool IsRankWidgetActive(RankingFormWidget widget)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
            if (form == null)
            {
                return false;
            }
            return form.GetWidget((int) widget).activeSelf;
        }

        public static void OnHeroItemEnable(CUIEvent uiEvent, ResHeroCfgInfo heroCfgInfo)
        {
            if ((heroCfgInfo != null) && (uiEvent.m_srcWidget != null))
            {
                GameObject item = Utility.FindChild(uiEvent.m_srcWidget, "heroItemCell");
                if (item != null)
                {
                    CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, item, heroCfgInfo.szImagePath, enHeroHeadType.enIcon, false);
                    CUIEventScript component = item.GetComponent<CUIEventScript>();
                    if (component != null)
                    {
                        stUIEventParams eventParams = new stUIEventParams {
                            heroId = heroCfgInfo.dwCfgID
                        };
                        component.SetUIEvent(enUIEventType.Click, enUIEventID.Ranking_HeroChg_Hero_Click, eventParams);
                    }
                    Utility.FindChild(item, "TxtFree").CustomSetActive(false);
                    Utility.FindChild(item, "equipedPanel").CustomSetActive(false);
                }
            }
        }

        public static void OnRankGodDetailEquipClick(CEquipInfo equipInfo, string playerName, string heroName)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingGodDetailForm);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                if (widget != null)
                {
                    GameObject p = Utility.FindChild(widget, "Panel_EquipInfo");
                    Text componetInChild = Utility.GetComponetInChild<Text>(p, "heroEquipText");
                    Text text2 = Utility.GetComponetInChild<Text>(p, "equipNameText");
                    Text text3 = Utility.GetComponetInChild<Text>(p, "Panel_euipProperty/equipPropertyDescText");
                    text2.text = (equipInfo != null) ? equipInfo.m_equipName : string.Empty;
                    text3.text = (equipInfo != null) ? equipInfo.m_equipPropertyDesc : string.Empty;
                    string[] args = new string[] { playerName, heroName };
                    componetInChild.text = Singleton<CTextManager>.instance.GetText("RankGodHeroEquipDesc", args);
                }
            }
        }

        public static void OnRankGodDetailTab(int tabIndex, COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO masterHeroInfo, uint heroId)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingGodDetailForm);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                if (widget != null)
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                    if (dataByKey != null)
                    {
                        GameObject obj3 = Utility.FindChild(widget, "Panel_EquipInfo");
                        GameObject obj4 = Utility.FindChild(widget, "Panel_SymbolInfo");
                        string str = Utility.UTF8Convert(masterHeroInfo.stAcntInfo.szPlayerName);
                        string szName = dataByKey.szName;
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                        if (tabIndex == 0)
                        {
                            obj3.CustomSetActive(true);
                            CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(obj3, "List");
                            int bEquipNum = masterHeroInfo.stEquipList.bEquipNum;
                            ushort[] defaultRecommendEquipInfo = new ushort[6];
                            if (bEquipNum > 0)
                            {
                                for (int j = 0; j < bEquipNum; j++)
                                {
                                    defaultRecommendEquipInfo[j] = (ushort) masterHeroInfo.stEquipList.EquipID[j];
                                }
                            }
                            else
                            {
                                defaultRecommendEquipInfo = Singleton<CEquipSystem>.instance.GetDefaultRecommendEquipInfo(heroId);
                                bEquipNum = defaultRecommendEquipInfo.Length;
                            }
                            componetInChild.SetElementAmount(bEquipNum);
                            for (int i = 0; i < bEquipNum; i++)
                            {
                                GameObject gameObject = componetInChild.GetElemenet(i).gameObject;
                                CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
                                ushort equipID = defaultRecommendEquipInfo[i];
                                CEquipInfo equipInfo = Singleton<CEquipSystem>.instance.GetEquipInfo(equipID);
                                component.m_onClickEventParams.battleEquipPar.equipInfo = Singleton<CEquipSystem>.instance.GetEquipInfo(equipID);
                                component.m_onClickEventParams.tagStr = str;
                                component.m_onClickEventParams.tagStr1 = szName;
                                CUICommonSystem.SetEquipIcon(equipID, gameObject, form);
                            }
                            if (bEquipNum > 0)
                            {
                                componetInChild.SelectElement(0, true);
                                componetInChild.GetElemenet(0).GetComponent<CUIEventScript>().OnPointerClick(null);
                                CUIEventScript script4 = componetInChild.GetElemenet(0).GetComponent<CUIEventScript>();
                                Singleton<CUIEventManager>.instance.DispatchUIEvent(script4.m_onClickEventID, script4.m_onClickEventParams);
                            }
                            else
                            {
                                componetInChild.SelectElement(-1, true);
                            }
                        }
                        else if (tabIndex == 1)
                        {
                            ListView<CSymbolItem> symbolList = new ListView<CSymbolItem>();
                            for (int k = 0; k < masterHeroInfo.stSymbolPageInfo.bSymbolPosNum; k++)
                            {
                                bool flag = false;
                                for (int n = 0; n < symbolList.Count; n++)
                                {
                                    if (symbolList[n].m_baseID == masterHeroInfo.stSymbolPageInfo.SymbolId[k])
                                    {
                                        CSymbolItem local1 = symbolList[n];
                                        local1.m_stackCount++;
                                        flag = true;
                                        break;
                                    }
                                }
                                if (!flag)
                                {
                                    CSymbolItem item = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, masterHeroInfo.stSymbolPageInfo.SymbolId[k], 1) as CSymbolItem;
                                    symbolList.Add(item);
                                }
                            }
                            CSymbolWearController.SortSymbolList(ref symbolList);
                            obj4.CustomSetActive(true);
                            CUIListScript script5 = Utility.GetComponetInChild<CUIListScript>(obj4, "List");
                            script5.SetElementAmount(symbolList.Count);
                            int num7 = 0;
                            for (int m = 0; m < symbolList.Count; m++)
                            {
                                GameObject p = script5.GetElemenet(m).gameObject;
                                Utility.GetComponetInChild<Image>(p, "imgIcon").SetSprite(symbolList[m].GetIconPath(), form, true, false, false);
                                Utility.GetComponetInChild<Text>(p, "SymbolName").text = symbolList[m].m_name;
                                char[] trimChars = new char[] { '\n' };
                                Utility.GetComponetInChild<Text>(p, "SymbolDesc").text = CSymbolSystem.GetSymbolAttString(symbolList[m], true).TrimEnd(trimChars);
                                Utility.GetComponetInChild<Text>(p, "lblIconCount").text = string.Format("x{0}", symbolList[m].m_stackCount);
                                num7 += symbolList[m].m_SymbolData.wLevel;
                            }
                            Utility.GetComponetInChild<Text>(obj4, "symbolPageLvlText").text = num7.ToString();
                            string[] args = new string[] { str, szName };
                            Utility.GetComponetInChild<Text>(obj4, "heroSymbolText").text = Singleton<CTextManager>.instance.GetText("RankGodHeroSymbolDesc", args);
                        }
                    }
                }
            }
        }

        public static void OnRankingArenaElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            GameObject obj3 = Utility.FindChild(srcWidget, "addFriendBtn");
            GameObject obj4 = Utility.FindChild(srcWidget, "sendButton");
            if (Singleton<CArenaSystem>.instance.m_rankInfoList.astFigterDetail[srcWidgetIndexInBelongedList].stFigterData.bMemberType == 1)
            {
                COMDT_ARENA_MEMBER_OF_ACNT fighterInfo = CArenaSystem.GetFighterInfo(Singleton<CArenaSystem>.instance.m_rankInfoList.astFigterDetail[srcWidgetIndexInBelongedList].stFigterData);
                ulong ullUid = fighterInfo.ullUid;
                uint logicWorldID = (uint) MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
                COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.GameFriend, ullUid, logicWorldID);
                COMDT_FRIEND_INFO comdt_friend_info2 = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.SNS, ullUid, logicWorldID);
                bool flag = comdt_friend_info != null;
                bool flag2 = comdt_friend_info2 != null;
                if (!flag && !flag2)
                {
                    ulong playerUllUID = (uint) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID;
                    obj3.CustomSetActive(playerUllUID != fighterInfo.ullUid);
                    obj4.CustomSetActive(false);
                    CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(obj3, "AddFriend");
                    componetInChild.m_onClickEventID = enUIEventID.Ranking_ArenaAddFriend;
                    componetInChild.m_onClickEventParams.tag = (int) logicWorldID;
                    componetInChild.m_onClickEventParams.commonUInt64Param1 = fighterInfo.ullUid;
                }
                else
                {
                    COMDT_ACNT_UNIQ uniq = (comdt_friend_info != null) ? comdt_friend_info.stUin : comdt_friend_info2.stUin;
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(true);
                    bool flag3 = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(uniq, COM_FRIEND_TYPE.COM_FRIEND_TYPE_GAME);
                    bool flag4 = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(uniq, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
                    bool isEnable = flag3 && flag4;
                    CUICommonSystem.SetButtonEnableWithShader(obj4.GetComponent<Button>(), isEnable, true);
                    if (isEnable)
                    {
                        CUIEventScript component = obj4.GetComponent<CUIEventScript>();
                        if (flag)
                        {
                            component.m_onClickEventID = enUIEventID.Ranking_Friend_GAME_SendCoin;
                        }
                        else
                        {
                            component.m_onClickEventID = enUIEventID.Friend_SendCoin;
                        }
                        component.m_onClickEventParams.tag = srcWidgetIndexInBelongedList;
                        component.m_onClickEventParams.commonUInt64Param1 = uniq.ullUid;
                        component.m_onClickEventParams.commonUInt64Param2 = uniq.dwLogicWorldId;
                    }
                }
            }
            else
            {
                obj3.CustomSetActive(false);
                obj4.CustomSetActive(false);
            }
            CArenaSystem.Arena_RankElementEnable(uiEvent);
        }

        public static void OpenHeroChooseForm()
        {
            enHeroJobType all = enHeroJobType.All;
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_ChooseHeroPath, false, true);
            GameObject widget = script.GetWidget(2);
            GameObject listObj = script.GetWidget(0);
            if (listObj != null)
            {
                Utility.GetComponetInChild<CUIListElementScript>(widget, "ScrollRect/Content/ListElement_Template").m_onEnableEventID = enUIEventID.Ranking_HeroChg_Hero_Item_Enable;
            }
            if (listObj != null)
            {
                script.GetWidget(1).CustomSetActive(false);
                string text = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
                string str2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
                string str3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
                string str4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
                string str5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
                string str6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
                string str7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
                string[] titleList = new string[] { text, str2, str3, str4, str5, str6, str7 };
                CUICommonSystem.InitMenuPanel(listObj, titleList, (int) all);
                listObj.GetComponent<CUIListScript>().m_listSelectChangedEventID = enUIEventID.Ranking_HeroChg_Title_Click;
            }
        }

        public static void RankingNumSet(uint rankNumber, RankingItemHelper rankingHelper)
        {
            rankingHelper.RankingNumText.CustomSetActive(false);
            rankingHelper.No1.CustomSetActive(false);
            rankingHelper.No2.CustomSetActive(false);
            rankingHelper.No3.CustomSetActive(false);
            rankingHelper.No1BG.CustomSetActive(false);
            rankingHelper.No1IconFrame.CustomSetActive(false);
            if (rankNumber == 0)
            {
                if (rankingHelper.NoRankingText != null)
                {
                    rankingHelper.NoRankingText.CustomSetActive(true);
                }
            }
            else
            {
                if (rankingHelper.NoRankingText != null)
                {
                    rankingHelper.NoRankingText.CustomSetActive(false);
                }
                switch (rankNumber)
                {
                    case 1:
                        rankingHelper.No1.CustomSetActive(true);
                        if ((rankingHelper.No1BG != null) && (rankingHelper.No1IconFrame != null))
                        {
                            rankingHelper.No1BG.CustomSetActive(true);
                            rankingHelper.No1IconFrame.CustomSetActive(true);
                        }
                        return;

                    case 2:
                        rankingHelper.No2.CustomSetActive(true);
                        return;

                    case 3:
                        rankingHelper.No3.CustomSetActive(true);
                        return;
                }
                rankingHelper.RankingNumText.CustomSetActive(true);
                rankingHelper.RankingNumText.GetComponent<Text>().text = string.Format("{0}", rankNumber);
            }
        }

        public static void RefreshGodHeroForm(ListView<ResHeroCfgInfo> heroList)
        {
            if (heroList != null)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_ChooseHeroPath);
                if (form != null)
                {
                    GameObject widget = form.GetWidget(2);
                    if (widget != null)
                    {
                        widget.GetComponent<CUIListScript>().SetElementAmount(heroList.Count);
                    }
                }
            }
        }

        public static void RefreshRankArena()
        {
            if ((Singleton<CArenaSystem>.instance.m_rankInfoList != null) && (Singleton<CArenaSystem>.instance.m_fightHeroInfoList != null))
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(RankingSystem.s_rankingForm);
                if (form != null)
                {
                    GameObject widget = form.GetWidget(0x11);
                    if (widget != null)
                    {
                        CUIListScript component = widget.GetComponent<CUIListScript>();
                        component.SetElementAmount(Singleton<CArenaSystem>.instance.m_rankInfoList.bFigterNum);
                        component.MoveElementInScrollArea(0, true);
                    }
                }
            }
        }

        public static void ResetRankListPos(RankingSystem.RankingType rankingType)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
            if (form != null)
            {
                GameObject obj2 = form.m_formWidgets[3];
                if (obj2 != null)
                {
                    if (rankingType != RankingSystem.RankingType.God)
                    {
                        obj2.GetComponent<RectTransform>().anchoredPosition = s_ListPos1;
                        obj2.GetComponent<RectTransform>().sizeDelta = s_ListSize1;
                    }
                    else
                    {
                        obj2.GetComponent<RectTransform>().anchoredPosition = s_ListPos2;
                        obj2.GetComponent<RectTransform>().sizeDelta = s_ListSize2;
                    }
                }
            }
        }

        public static void SetGameObjChildText(GameObject parentObj, string childName, string text)
        {
            if (parentObj != null)
            {
                parentObj.transform.FindChild(childName).gameObject.GetComponent<Text>().text = text;
            }
        }

        public static void SetHostUrlHeadIcon(GameObject headIcon)
        {
            if (!CSysDynamicBlock.bSocialBlocked)
            {
                string headUrl = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().HeadUrl;
                headIcon.GetComponent<CUIHttpImageScript>().SetImageUrl(headUrl);
            }
        }

        public static void SetPlatChannel(GameObject parentObj, uint logicWorldId)
        {
            if (parentObj != null)
            {
                Transform transform = parentObj.transform.Find("NameGroup/PlatChannelIcon");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(!Utility.IsSamePlatform(logicWorldId));
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        transform.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public static void SetUrlHeadIcon(GameObject headIcon, string serverUrl)
        {
            if (!CSysDynamicBlock.bSocialBlocked)
            {
                headIcon.GetComponent<CUIHttpImageScript>().SetImageUrl(serverUrl);
            }
        }

        public static void ShowAllRankMenu()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
            if (form != null)
            {
                GameObject obj2 = form.m_formWidgets[14];
                obj2.CustomSetActive(true);
                Utility.FindChild(obj2, "ListElement8/Lock").CustomSetActive(!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA));
                Singleton<CMiShuSystem>.GetInstance().SetNewFlagForArenaRankBtn(true);
                Singleton<CMiShuSystem>.GetInstance().SetNewFlagForGodRankBtn(true);
            }
        }

        public static void ShowRankGodDetailPanel()
        {
            GameObject widget = Singleton<CUIManager>.instance.OpenForm(RankingSystem.s_rankingGodDetailForm, false, true).GetWidget(0);
            InitRankGodDetailTab();
            Utility.GetComponetInChild<CUIListScript>(widget, "TitleTab").SelectElement(0, true);
        }

        public static void UpdateArenaSelfInfo()
        {
            if (Singleton<CArenaSystem>.instance.m_fightHeroInfoList != null)
            {
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
                if (form != null)
                {
                    uint pvpLevel = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel;
                    List<uint> arenaDefHeroList = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList;
                    GameObject widget = form.GetWidget(0x12);
                    RankingItemHelper component = widget.GetComponent<RankingItemHelper>();
                    SetGameObjChildText(widget, "NameGroup/PlayerName", Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Name);
                    SetGameObjChildText(widget, "PlayerLv", string.Format("Lv.{0}", pvpLevel.ToString(CultureInfo.InvariantCulture)));
                    SetHostUrlHeadIcon(Utility.FindChild(widget, "HeadIcon"));
                    MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(Utility.GetComponetInChild<Image>(widget, "NameGroup/QQVipIcon"));
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(Utility.GetComponetInChild<Image>(widget, "NobeIcon"), (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false);
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(Utility.GetComponetInChild<Image>(widget, "HeadFrame"), (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "NameGroup/WXIcon"), Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Wechat, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "NameGroup/QQGameCenterIcon"), Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.QQ, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "NameGroup/GuestGameCenterIcon"), Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Guest, true, false);
                    RankingNumSet(Singleton<CArenaSystem>.instance.m_fightHeroInfoList.stArenaInfo.dwSelfRank, component);
                    for (int i = 0; i < 3; i++)
                    {
                        int num5 = i + 1;
                        GameObject obj3 = Utility.FindChild(widget, string.Format("listHero/heroItemCell{0}", num5.ToString()));
                        if (arenaDefHeroList.Count > i)
                        {
                            obj3.CustomSetActive(true);
                            IHeroData data = CHeroDataFactory.CreateHeroData(arenaDefHeroList[i]);
                            CUICommonSystem.SetHeroItemData(form, obj3, data, enHeroHeadType.enIcon, false);
                        }
                        else
                        {
                            obj3.CustomSetActive(false);
                        }
                    }
                    ListView<COMDT_ARENA_FIGHT_RECORD> recordList = Singleton<CArenaSystem>.instance.m_recordList;
                    ulong playerUllUID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
                    int num4 = 0;
                    if ((recordList != null) && (recordList.Count > 0))
                    {
                        if (recordList[0].ullAtkerUid == playerUllUID)
                        {
                            if (recordList[0].bResult == 1)
                            {
                                num4 = (int) (recordList[0].dwAtkerRank - recordList[0].dwTargetRank);
                            }
                        }
                        else if (recordList[0].bResult == 1)
                        {
                            num4 = (int) (recordList[0].dwAtkerRank - recordList[0].dwTargetRank);
                        }
                    }
                    GameObject obj4 = Utility.FindChild(widget, "ChangeIcon");
                    if (num4 == 0)
                    {
                        obj4.CustomSetActive(false);
                        SetGameObjChildText(widget, "ChangeNum", "--");
                    }
                    else if (num4 > 0)
                    {
                        obj4.CustomSetActive(true);
                        obj4.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                        SetGameObjChildText(widget, "ChangeNum", num4.ToString(CultureInfo.InvariantCulture));
                    }
                    else if (num4 < 0)
                    {
                        obj4.CustomSetActive(true);
                        obj4.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                        SetGameObjChildText(widget, "ChangeNum", num4.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        public static void UpdateGodFindBtns(CUIListScript uiList, int index)
        {
            if (uiList != null)
            {
                int elementAmount = uiList.GetElementAmount();
                for (int i = 0; i < elementAmount; i++)
                {
                    if (((uiList.GetElemenet(i) != null) && (uiList.GetElemenet(i).gameObject != null)) && uiList.IsElementInScrollArea(i))
                    {
                        Utility.FindChild(uiList.GetElemenet(i).gameObject, "FindBtn/Select").CustomSetActive(index == i);
                    }
                }
            }
        }

        public static void UpdateOneGodElement(GameObject objElement, int viewIndex, CSDT_RANKING_LIST_SUCC curRankingList)
        {
            if ((curRankingList != null) && (objElement != null))
            {
                RankingItemHelper component = objElement.GetComponent<RankingItemHelper>();
                if (component != null)
                {
                    CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = curRankingList.astItemDetail[viewIndex];
                    if (csdt_ranking_list_item_info != null)
                    {
                        string text = string.Empty;
                        uint dwPvpLevel = 1;
                        string serverUrl = null;
                        ulong ullUid = 0L;
                        uint logicWorldId = 0;
                        uint dwCurLevel = 0;
                        uint dwHeadIconId = 0;
                        uint dwVipLevel = 0;
                        COM_PRIVILEGE_TYPE privilegeType = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
                        COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stAcntInfo = csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stMasterHero.stAcntInfo;
                        if (stAcntInfo != null)
                        {
                            text = StringHelper.UTF8BytesToString(ref stAcntInfo.szPlayerName);
                            dwPvpLevel = stAcntInfo.dwPvpLevel;
                            serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref stAcntInfo.szHeadUrl);
                            ullUid = stAcntInfo.ullUid;
                            logicWorldId = (uint) stAcntInfo.iLogicWorldId;
                            dwCurLevel = stAcntInfo.stGameVip.dwCurLevel;
                            dwHeadIconId = stAcntInfo.stGameVip.dwHeadIconId;
                            privilegeType = (COM_PRIVILEGE_TYPE) stAcntInfo.bPrivilege;
                            dwVipLevel = stAcntInfo.dwVipLevel;
                        }
                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, privilegeType, ApolloPlatform.Wechat, true, false);
                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, privilegeType, ApolloPlatform.QQ, true, false);
                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, privilegeType, ApolloPlatform.Guest, true, false);
                        SetGameObjChildText(objElement, "NameGroup/PlayerName", text);
                        SetGameObjChildText(objElement, "PlayerLv", string.Format("Lv.{0}", Math.Max(1, dwPvpLevel)));
                        SetUrlHeadIcon(component.HeadIcon, serverUrl);
                        SetPlatChannel(objElement, logicWorldId);
                        component.LadderGo.CustomSetActive(false);
                        objElement.transform.FindChild("Value").gameObject.CustomSetActive(true);
                        objElement.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
                        component.FindBtn.CustomSetActive(true);
                        component.FindBtn.GetComponent<CUIEventScript>().m_onClickEventParams.tag = viewIndex;
                        Utility.FindChild(component.FindBtn, "Select").CustomSetActive(false);
                        uint num7 = (csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stMasterHero.dwWinCnt * 100) / csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stMasterHero.dwGameCnt;
                        string[] args = new string[] { num7.ToString(), csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stMasterHero.dwWinCnt.ToString() };
                        SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroMasterName", args));
                        SetGameObjChildText(objElement, "Value", string.Empty);
                        uint rankNumber = (uint) (viewIndex + 1);
                        RankingNumSet(rankNumber, component);
                        component.AddFriend.CustomSetActive(false);
                        component.SendCoin.CustomSetActive(false);
                        component.Online.CustomSetActive(false);
                        if (dwVipLevel == 0xdf1f9)
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component.QqVip.GetComponent<Image>());
                        }
                        else
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(component.QqVip.GetComponent<Image>(), (int) dwVipLevel);
                        }
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.VipIcon.GetComponent<Image>(), (int) dwCurLevel, false);
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component.HeadIconFrame.GetComponent<Image>(), (int) dwHeadIconId);
                    }
                }
            }
        }

        public static void UpdateRankGodTitle(ResHeroCfgInfo heroCfgInfo)
        {
            if (heroCfgInfo != null)
            {
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(RankingSystem.s_rankingForm);
                if (form != null)
                {
                    GameObject p = form.m_formWidgets[0x13];
                    if (p != null)
                    {
                        GameObject item = Utility.FindChild(p, "heroItemCell");
                        CUICommonSystem.SetHeroItemImage(form, item, heroCfgInfo.szImagePath, enHeroHeadType.enIcon, false);
                        string[] args = new string[] { heroCfgInfo.szHeroTitle, heroCfgInfo.szName };
                        Utility.GetComponetInChild<Text>(p, "PlayerName").text = Singleton<CTextManager>.instance.GetText("RankGodHeroName", args);
                    }
                }
            }
        }

        public static void UpdateSymbolItem(CSymbolItem symbol, GameObject element, CUIFormScript form)
        {
            if (((symbol != null) && (element != null)) && (form != null))
            {
                Image component = element.transform.Find("iconImage").GetComponent<Image>();
                Text text = element.transform.Find("countText").GetComponent<Text>();
                Text text2 = element.transform.Find("nameText").GetComponent<Text>();
                Text text3 = element.transform.Find("descText").GetComponent<Text>();
                component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, symbol.m_SymbolData.dwIcon), form, true, false, false);
                text.text = string.Format("x{0}", symbol.m_stackCount.ToString());
                text2.text = symbol.m_SymbolData.szName;
                text3.text = CSymbolSystem.GetSymbolAttString(symbol.m_baseID, true);
            }
        }
    }
}

