namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CArenaSystem : Singleton<CArenaSystem>
    {
        public SCPKG_CHGARENAFIGHTERRSP m_fightHeroInfoList;
        public uint m_lastFighterInfoRequestTime;
        public uint m_lastRankRequestTime;
        public uint m_lastRecordRequestTime;
        public int m_nextCanFightTimes;
        private bool m_openArenaForm;
        public COMDT_ARENA_FIGHTER_INFO m_rankInfoList;
        public ListView<COMDT_ARENA_FIGHT_RECORD> m_recordList = new ListView<COMDT_ARENA_FIGHT_RECORD>();
        public CSDT_ACNT_ARENADATA m_serverInfo;
        public byte m_tarIndex;
        public COMDT_ARENA_MEMBER_OF_ACNT m_tarInfo;
        public ulong m_tarObjID;
        public uint m_tarRank;
        public byte m_tarType;
        public static int s_Arena_RULE_ID = 5;
        public static string s_arenaFightRecordFormPath = "UGUI/Form/System/Arena/Form_Arena_FightRecord.prefab";
        public static string s_arenaFightResultFormPath = "UGUI/Form/System/Arena/Form_Arena_Result.prefab";
        public static string s_arenaFormPath = "UGUI/Form/System/Arena/Form_Arena.prefab";
        public static string s_arenaPlayerInfoFormPath = "UGUI/Form/System/Arena/Form_Arena_PlayerInfo.prefab";
        public static string s_arenaRankChangeFormPath = "UGUI/Form/System/Arena/Form_Arena_RankChange.prefab";
        public static string s_arenaRankInfoFormPath = "UGUI/Form/System/Arena/Form_Arena_RankInfo.prefab";
        public const int s_arenaRankMaxNum = 50;
        public static int s_mapKey = 0x7789;

        private void Arena_BuyChangeTimes(CUIEvent uiEvent)
        {
            uint shopInfoCfgId = CPurchaseSys.GetShopInfoCfgId(RES_SHOPBUY_TYPE.RES_BUYTYPE_ARENACHALLENGECNT, this.m_serverInfo.bBuyChallengeCnt + 1);
            ResShopInfo dataByKey = GameDataMgr.resShopInfoDatabin.GetDataByKey((uint) ((ushort) shopInfoCfgId));
            if (dataByKey != null)
            {
                uint dwCoinPrice = dataByKey.dwCoinPrice;
                int dwValue = (int) dataByKey.dwValue;
                enPayType payType = CMallSystem.ResBuyTypeToPayType(dataByKey.bCoinType);
                string[] args = new string[] { dwValue.ToString(), (this.m_serverInfo.bBuyChallengeCnt + 1).ToString() };
                CMallSystem.TryToPay(enPayPurpose.Buy, Singleton<CTextManager>.GetInstance().GetText("Arena_Arena_Count_Buy", args), payType, dwCoinPrice, enUIEventID.Arena_ConfirmBuyChangeTimes, ref uiEvent.m_eventParams, enUIEventID.None, true, true, false);
            }
        }

        private void Arena_CDTimeEnd(CUIEvent uiEvent)
        {
            this.RefreshArenaForm();
        }

        private void Arena_ChangeHeroList(CUIEvent uiEvent)
        {
            SendGetFightHeroListMSG(true);
        }

        private void Arena_CloseTps(CUIEvent uiEvent)
        {
            uiEvent.m_srcFormScript.gameObject.transform.Find("panelTips").gameObject.CustomSetActive(false);
        }

        private void Arena_ConfirmBuyChangeTimes(CUIEvent uiEvent)
        {
            SendBuyTimesMSG();
        }

        private void Arena_ConfirmResetCD(CUIEvent uiEvent)
        {
            SendResetCDMSG();
        }

        private void Arena_ConfirmResutlForm(CUIEvent uiEvent)
        {
            if ((this.m_tarRank < this.m_serverInfo.dwTopRank) || (this.m_serverInfo.dwTopRank == 0))
            {
                long num = Math.Abs((int) (this.m_serverInfo.dwTopRank - this.m_tarRank));
                this.m_serverInfo.dwTopRank = this.m_tarRank;
                CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_arenaRankChangeFormPath, false, true);
                Text component = formScript.gameObject.transform.Find("resultCell/lblRank").GetComponent<Text>();
                Text text2 = formScript.gameObject.transform.Find("resultCell/lblRankChange").GetComponent<Text>();
                Transform[] transformArray = new Transform[] { formScript.gameObject.transform.Find("Award/ListAward/itemCell1"), formScript.gameObject.transform.Find("Award/ListAward/itemCell2"), formScript.gameObject.transform.Find("Award/ListAward/itemCell3"), formScript.gameObject.transform.Find("Award/ListAward/itemCell4"), formScript.gameObject.transform.Find("Award/ListAward/itemCell5"), formScript.gameObject.transform.Find("Award/ListAward/itemCell6"), formScript.gameObject.transform.Find("Award/ListAward/itemCell7"), formScript.gameObject.transform.Find("Award/ListAward/itemCell8") };
                component.text = this.m_tarRank.ToString();
                text2.text = num.ToString();
                ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(Singleton<SingleGameSettleMgr>.GetInstance().m_settleData.stDetail.stReward);
                for (int i = 0; i < transformArray.Length; i++)
                {
                    if (i < useableListFromReward.Count)
                    {
                        useableListFromReward[i].SetMultiple(ref Singleton<SingleGameSettleMgr>.GetInstance().m_settleData.stDetail.stMultipleDetail, true);
                        CUICommonSystem.SetItemCell(formScript, transformArray[i].gameObject, useableListFromReward[i], true, false);
                    }
                    else
                    {
                        transformArray[i].gameObject.CustomSetActive(false);
                    }
                }
            }
            this.m_fightHeroInfoList.stArenaInfo.dwSelfRank = this.m_tarRank;
        }

        private void Arena_OnCloseForm(CUIEvent uiEvent)
        {
            CExploreView.RefreshExploreList();
        }

        private void Arena_OpenForm(CUIEvent uiEvent)
        {
            if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA))
            {
                if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
                }
                else if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.Count == 0)
                {
                    SendJoinArenaMSG();
                }
                else
                {
                    MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(15, true, false);
                    this.Reset();
                    SendGetFightHeroListMSG(true);
                    this.m_openArenaForm = true;
                }
            }
            else
            {
                ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 9);
                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
            }
        }

        private void Arena_OpenPlayerInfoForm(CUIEvent uiEvent)
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.Count == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Arena_ARENAADDMEM_ERR_BATTLELISTISNULL"), false, 1.5f, null, new object[0]);
                Singleton<CUIManager>.GetInstance().CloseForm(s_arenaPlayerInfoFormPath);
            }
            else
            {
                int num = this.m_nextCanFightTimes - CRoleInfo.GetElapseSecondsSinceLogin();
                if (num > 0)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Arena_ERR_LIMIT_CD"), false, 1.5f, null, new object[0]);
                    Singleton<CUIManager>.GetInstance().CloseForm(s_arenaPlayerInfoFormPath);
                }
                else if (this.m_serverInfo.chAlreadyFightCnt >= GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x30).dwConfValue)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Arena_ERR_LIMIT_CNT"), false, 1.5f, null, new object[0]);
                    Singleton<CUIManager>.GetInstance().CloseForm(s_arenaPlayerInfoFormPath);
                }
                else
                {
                    int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                    COMDT_ARENA_MEMBER_OF_ACNT fighterInfo = GetFighterInfo(this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[srcWidgetIndexInBelongedList].stFigterData);
                    this.m_tarObjID = fighterInfo.ullUid;
                    this.m_tarIndex = (byte) srcWidgetIndexInBelongedList;
                    this.m_tarInfo = fighterInfo;
                    this.m_tarType = this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[srcWidgetIndexInBelongedList].stFigterData.bMemberType;
                    this.m_tarRank = this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[srcWidgetIndexInBelongedList].dwRank;
                    CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_arenaPlayerInfoFormPath, false, true);
                    GameObject gameObject = formScript.gameObject.transform.Find("Root/heroItemCell").gameObject;
                    Text component = formScript.gameObject.transform.Find("Root/lblName").gameObject.GetComponent<Text>();
                    Text text2 = formScript.gameObject.transform.Find("Root/lblFight").gameObject.GetComponent<Text>();
                    Text text3 = formScript.gameObject.transform.Find("Root/lblRank").gameObject.GetComponent<Text>();
                    CUIListScript script2 = formScript.gameObject.transform.Find("Root/List").gameObject.GetComponent<CUIListScript>();
                    component.text = StringHelper.UTF8BytesToString(ref fighterInfo.szName);
                    string[] args = new string[] { fighterInfo.dwPVPLevel.ToString() };
                    text2.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Fighting_Power", args);
                    string[] textArray2 = new string[] { this.m_tarRank.ToString() };
                    text3.text = Singleton<CTextManager>.GetInstance().GetText("Arena_13", textArray2);
                    this.SetPlayerHead(gameObject, StringHelper.UTF8BytesToString(ref fighterInfo.szHeadUrl), this.m_tarType, formScript);
                    Image image = gameObject.transform.Find("NobeIcon").GetComponent<Image>();
                    Image image2 = gameObject.transform.Find("NobeImag").GetComponent<Image>();
                    if (image != null)
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) fighterInfo.stVip.dwCurLevel, false);
                    }
                    if (image2 != null)
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) fighterInfo.stVip.dwHeadIconId);
                    }
                    script2.SetElementAmount(fighterInfo.stBattleHero.astHero.Length);
                    for (int i = 0; i < script2.GetElementAmount(); i++)
                    {
                        GameObject item = script2.GetElemenet(i).gameObject.transform.Find("heroItemCell").gameObject;
                        COMDT_ARENA_HERODETAIL comdt_arena_herodetail = fighterInfo.stBattleHero.astHero[i];
                        if (comdt_arena_herodetail.dwHeroId != 0)
                        {
                            CCustomHeroData data = CHeroDataFactory.CreateCustomHeroData(comdt_arena_herodetail.dwHeroId) as CCustomHeroData;
                            data.m_star = comdt_arena_herodetail.wHeroStar;
                            data.m_level = comdt_arena_herodetail.wHeroLevel;
                            data.m_quaility = comdt_arena_herodetail.stHeroQuality.wQuality;
                            data.m_subQualility = comdt_arena_herodetail.stHeroQuality.wSubQuality;
                            CUICommonSystem.SetHeroItemData(formScript, item, data, enHeroHeadType.enIcon, false);
                        }
                        else
                        {
                            item.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void Arena_OpenRankInfoForm(CUIEvent uiEvent)
        {
            SendGetRankListMSG(true);
        }

        private void Arena_OpenRecordForm(CUIEvent uiEvent)
        {
            if (((CRoleInfo.GetElapseSecondsSinceLogin() - this.m_lastRecordRequestTime) > 60L) || (this.m_lastRecordRequestTime == 0))
            {
                SendGetRecordMSG(true);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenForm(s_arenaFightRecordFormPath, false, true);
                Singleton<CArenaSystem>.GetInstance().RefreshRecordForm();
            }
        }

        private void Arena_OpenShopForm(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_OpenArenaShop);
        }

        private void Arena_OpenTips(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenInfoForm(s_Arena_RULE_ID);
        }

        public static void Arena_RankElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            Text component = srcWidget.transform.Find("lbTop").GetComponent<Text>();
            Text text2 = srcWidget.transform.Find("lblTitle").GetComponent<Text>();
            Text text3 = srcWidget.transform.Find("lblFight").GetComponent<Text>();
            GameObject[] objArray = new GameObject[] { srcWidget.transform.Find("listHero/heroItemCell1").gameObject, srcWidget.transform.Find("listHero/heroItemCell2").gameObject, srcWidget.transform.Find("listHero/heroItemCell3").gameObject };
            Image image = srcWidget.transform.Find("img1").GetComponent<Image>();
            Image image2 = srcWidget.transform.Find("img2").GetComponent<Image>();
            Image image3 = srcWidget.transform.Find("img3").GetComponent<Image>();
            COMDT_ARENA_MEMBER_OF_ACNT fighterInfo = GetFighterInfo(Singleton<CArenaSystem>.instance.m_rankInfoList.astFigterDetail[srcWidgetIndexInBelongedList].stFigterData);
            image.gameObject.CustomSetActive(false);
            image2.gameObject.CustomSetActive(false);
            image3.gameObject.CustomSetActive(false);
            component.gameObject.CustomSetActive(false);
            switch (srcWidgetIndexInBelongedList)
            {
                case 0:
                    image.gameObject.CustomSetActive(true);
                    break;

                case 1:
                    image2.gameObject.CustomSetActive(true);
                    break;

                case 2:
                    image3.gameObject.CustomSetActive(true);
                    break;

                default:
                    component.gameObject.CustomSetActive(true);
                    break;
            }
            string[] args = new string[] { (srcWidgetIndexInBelongedList + 1).ToString() };
            component.text = Singleton<CTextManager>.GetInstance().GetText("Arena_13", args);
            text2.text = StringHelper.UTF8BytesToString(ref fighterInfo.szName);
            string[] textArray2 = new string[] { fighterInfo.dwPVPLevel.ToString() };
            text3.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Fighting_Power", textArray2);
            for (int i = 0; i < objArray.Length; i++)
            {
                if (i < fighterInfo.stBattleHero.astHero.Length)
                {
                    COMDT_ARENA_HERODETAIL comdt_arena_herodetail = fighterInfo.stBattleHero.astHero[i];
                    if (comdt_arena_herodetail.dwHeroId != 0)
                    {
                        objArray[i].CustomSetActive(true);
                        CCustomHeroData data = CHeroDataFactory.CreateCustomHeroData(comdt_arena_herodetail.dwHeroId) as CCustomHeroData;
                        data.m_star = comdt_arena_herodetail.wHeroStar;
                        data.m_level = comdt_arena_herodetail.wHeroLevel;
                        data.m_quaility = comdt_arena_herodetail.stHeroQuality.wQuality;
                        data.m_subQualility = comdt_arena_herodetail.stHeroQuality.wSubQuality;
                        CUICommonSystem.SetHeroItemData(uiEvent.m_srcFormScript, objArray[i], data, enHeroHeadType.enIcon, false);
                    }
                    else
                    {
                        objArray[i].CustomSetActive(false);
                    }
                }
                else
                {
                    objArray[i].CustomSetActive(false);
                }
            }
        }

        private void Arena_RankInfoMenuClick(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            GameObject gameObject = srcFormScript.gameObject.transform.Find("Root/panelRank").gameObject;
            GameObject obj3 = srcFormScript.gameObject.transform.Find("Root/panelAward").gameObject;
            gameObject.CustomSetActive(false);
            obj3.CustomSetActive(false);
            switch (selectedIndex)
            {
                case 0:
                    gameObject.CustomSetActive(true);
                    gameObject.transform.Find("List").gameObject.GetComponent<CUIListScript>().SetElementAmount(Math.Min(this.m_rankInfoList.bFigterNum, 50));
                    break;

                case 1:
                {
                    obj3.CustomSetActive(true);
                    CUIListScript component = obj3.transform.Find("List").gameObject.GetComponent<CUIListScript>();
                    component.SetElementAmount(10);
                    for (int i = 0; i < component.GetElementAmount(); i++)
                    {
                        CUIListElementScript elemenet = component.GetElemenet(i);
                        Image image = elemenet.gameObject.transform.Find("img1").GetComponent<Image>();
                        Image image2 = elemenet.gameObject.transform.Find("img2").GetComponent<Image>();
                        Image image3 = elemenet.gameObject.transform.Find("img3").GetComponent<Image>();
                        Text text = elemenet.gameObject.transform.Find("lbTop").GetComponent<Text>();
                        GameObject[] objArray = new GameObject[] { elemenet.gameObject.transform.Find("ListAward/itemCell1").gameObject, elemenet.gameObject.transform.Find("ListAward/itemCell2").gameObject, elemenet.gameObject.transform.Find("ListAward/itemCell3").gameObject, elemenet.gameObject.transform.Find("ListAward/itemCell4").gameObject };
                        image.gameObject.CustomSetActive(false);
                        image2.gameObject.CustomSetActive(false);
                        image3.gameObject.CustomSetActive(false);
                        text.gameObject.CustomSetActive(false);
                        switch (i)
                        {
                            case 0:
                                image.gameObject.CustomSetActive(true);
                                break;

                            case 1:
                                image2.gameObject.CustomSetActive(true);
                                break;

                            case 2:
                                image3.gameObject.CustomSetActive(true);
                                break;

                            default:
                                text.gameObject.CustomSetActive(true);
                                break;
                        }
                        string[] textArray1 = new string[] { (i + 1).ToString() };
                        text.text = Singleton<CTextManager>.GetInstance().GetText("Arena_13", textArray1);
                        ListView<CUseable> view = this.GetRewardUseableList(i + 1);
                        for (int k = 0; k < objArray.Length; k++)
                        {
                            if (k > (view.Count - 1))
                            {
                                objArray[k].CustomSetActive(false);
                            }
                            else
                            {
                                CUICommonSystem.SetItemCell(uiEvent.m_srcFormScript, objArray[k], view[k], true, false);
                                if (view[k].m_stackCount == 0)
                                {
                                    objArray[k].CustomSetActive(false);
                                }
                                else
                                {
                                    objArray[k].CustomSetActive(true);
                                }
                            }
                        }
                    }
                    Text text2 = obj3.transform.Find("panelSelfInfo/lblRank").GetComponent<Text>();
                    GameObject[] objArray2 = new GameObject[] { obj3.transform.Find("panelSelfInfo/ListAward/itemCell1").gameObject, obj3.transform.Find("panelSelfInfo/ListAward/itemCell2").gameObject, obj3.transform.Find("panelSelfInfo/ListAward/itemCell3").gameObject, obj3.transform.Find("panelSelfInfo/ListAward/itemCell4").gameObject };
                    string[] args = new string[] { this.m_fightHeroInfoList.stArenaInfo.dwSelfRank.ToString() };
                    text2.text = Singleton<CTextManager>.GetInstance().GetText("Arena_My_Rank", args);
                    ListView<CUseable> rewardUseableList = this.GetRewardUseableList((int) this.m_fightHeroInfoList.stArenaInfo.dwSelfRank);
                    for (int j = 0; j < objArray2.Length; j++)
                    {
                        if (j > (rewardUseableList.Count - 1))
                        {
                            objArray2[j].CustomSetActive(false);
                        }
                        else
                        {
                            CUICommonSystem.SetItemCell(uiEvent.m_srcFormScript, objArray2[j], rewardUseableList[j], true, false);
                            if (rewardUseableList[j].m_stackCount == 0)
                            {
                                objArray2[j].CustomSetActive(false);
                            }
                            else
                            {
                                objArray2[j].CustomSetActive(true);
                            }
                        }
                    }
                    break;
                }
            }
        }

        private void Arena_ReciveDefTeamInfo(CUIEvent uiEvent)
        {
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.Clear();
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.AddRange(uiEvent.m_eventParams.heroIdList);
            SendSetDefTeamConfigMSG(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList);
        }

        private void Arena_RecordlementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            ulong playerUllUID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
            Text component = srcWidget.transform.Find("lblTitle").GetComponent<Text>();
            Text text2 = srcWidget.transform.Find("lblFight").GetComponent<Text>();
            Text text3 = srcWidget.transform.Find("lblRankChange").GetComponent<Text>();
            Image image = srcWidget.transform.Find("imgUp").GetComponent<Image>();
            Image image2 = srcWidget.transform.Find("imgDown").GetComponent<Image>();
            Image image3 = srcWidget.transform.Find("imgWin").GetComponent<Image>();
            Image image4 = srcWidget.transform.Find("imgLose").GetComponent<Image>();
            Image image5 = srcWidget.transform.Find("RedFlag").GetComponent<Image>();
            Image image6 = srcWidget.transform.Find("BlueFlag").GetComponent<Image>();
            text3.text = "---";
            image.gameObject.CustomSetActive(false);
            image2.gameObject.CustomSetActive(false);
            image3.gameObject.CustomSetActive(false);
            image4.gameObject.CustomSetActive(false);
            image5.gameObject.CustomSetActive(false);
            image6.gameObject.CustomSetActive(false);
            GameObject[] objArray = new GameObject[] { srcWidget.transform.Find("listHero/heroItemCell1").gameObject, srcWidget.transform.Find("listHero/heroItemCell2").gameObject, srcWidget.transform.Find("listHero/heroItemCell3").gameObject };
            COMDT_ARENA_MEMBER_OF_ACNT fighterInfo = GetFighterInfo(this.m_recordList[srcWidgetIndexInBelongedList].stTargetInfo);
            component.text = StringHelper.UTF8BytesToString(ref fighterInfo.szName);
            string[] args = new string[] { fighterInfo.dwPVPLevel.ToString() };
            text2.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Fighting_Power", args);
            image3.gameObject.CustomSetActive(false);
            image4.gameObject.CustomSetActive(false);
            if (this.m_recordList[srcWidgetIndexInBelongedList].ullAtkerUid == playerUllUID)
            {
                image5.gameObject.CustomSetActive(true);
                if (this.m_recordList[srcWidgetIndexInBelongedList].bResult == 1)
                {
                    text3.text = Math.Abs((int) (this.m_recordList[srcWidgetIndexInBelongedList].dwAtkerRank - this.m_recordList[srcWidgetIndexInBelongedList].dwTargetRank)).ToString();
                    image3.gameObject.CustomSetActive(true);
                    image.gameObject.CustomSetActive(true);
                }
                else
                {
                    image4.gameObject.CustomSetActive(true);
                }
            }
            else
            {
                image6.gameObject.CustomSetActive(true);
                if (this.m_recordList[srcWidgetIndexInBelongedList].bResult == 1)
                {
                    text3.text = Math.Abs((int) (this.m_recordList[srcWidgetIndexInBelongedList].dwAtkerRank - this.m_recordList[srcWidgetIndexInBelongedList].dwTargetRank)).ToString();
                    image4.gameObject.CustomSetActive(true);
                    image2.gameObject.CustomSetActive(true);
                }
                else
                {
                    image3.gameObject.CustomSetActive(true);
                }
            }
            for (int i = 0; i < objArray.Length; i++)
            {
                if (i < fighterInfo.stBattleHero.astHero.Length)
                {
                    COMDT_ARENA_HERODETAIL comdt_arena_herodetail = fighterInfo.stBattleHero.astHero[i];
                    if (comdt_arena_herodetail.dwHeroId != 0)
                    {
                        CCustomHeroData data = CHeroDataFactory.CreateCustomHeroData(comdt_arena_herodetail.dwHeroId) as CCustomHeroData;
                        data.m_star = comdt_arena_herodetail.wHeroStar;
                        data.m_level = comdt_arena_herodetail.wHeroLevel;
                        data.m_quaility = comdt_arena_herodetail.stHeroQuality.wQuality;
                        data.m_subQualility = comdt_arena_herodetail.stHeroQuality.wSubQuality;
                        CUICommonSystem.SetHeroItemData(uiEvent.m_srcFormScript, objArray[i], data, enHeroHeadType.enIcon, false);
                    }
                    else
                    {
                        objArray[i].CustomSetActive(false);
                    }
                }
                else
                {
                    objArray[i].CustomSetActive(false);
                }
            }
        }

        private void Arena_ResetByNewDay(CUIEvent uiEvent)
        {
            this.m_serverInfo.bChallengeLimitCnt = (byte) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x30).dwConfValue;
            this.m_serverInfo.bBuyChallengeCnt = 0;
            this.m_serverInfo.dwNextRefreshLeftTime = ((uint) CRoleInfo.GetElapseSecondsSinceLogin()) + Utility.s_daySecond;
            this.m_serverInfo.dwLeftFightCoolTime = 0;
            this.m_serverInfo.chAlreadyFightCnt = 0;
            this.m_nextCanFightTimes = CRoleInfo.GetElapseSecondsSinceLogin();
            this.RefreshArenaForm();
        }

        private void Arena_ResetCD(CUIEvent uiEvent)
        {
            uint payValue = 10;
            int num2 = this.m_nextCanFightTimes - CRoleInfo.GetElapseSecondsSinceLogin();
            if (num2 > 0)
            {
                ResClrCD dataByKey = GameDataMgr.cdDatabin.GetDataByKey((uint) 1);
                payValue = (uint) ((((long) num2) / ((ulong) dataByKey.dwConsumeUnit)) * dataByKey.dwUnitPrice);
                if (payValue == 0)
                {
                    payValue = dataByKey.dwUnitPrice;
                }
                enPayType payType = CMallSystem.ResBuyTypeToPayType((int) dataByKey.dwConsumeType);
                CMallSystem.TryToPay(enPayPurpose.Buy, Singleton<CTextManager>.GetInstance().GetText("Arena_Cd_End"), payType, payValue, enUIEventID.Arena_ConfirmBuyResetCD, ref uiEvent.m_eventParams, enUIEventID.None, true, true, false);
            }
        }

        private void Arena_StartFight(CUIEvent uiEvent)
        {
            CSDT_SINGLE_GAME_OF_ARENA reportInfo = new CSDT_SINGLE_GAME_OF_ARENA {
                iLevelID = s_mapKey,
                bTargetIndex = this.m_tarIndex
            };
            ResLevelCfgInfo dataByKey = GameDataMgr.arenaLevelDatabin.GetDataByKey((long) s_mapKey);
            DebugHelper.Assert(dataByKey != null);
            Singleton<CHeroSelectBaseSystem>.instance.SetPveDataWithArena(dataByKey.dwBattleListID, reportInfo, Singleton<CTextManager>.GetInstance().GetText("Arena_Arena"));
            Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enArena, (byte) dataByKey.iHeroNum, 0, 0, 0);
        }

        private void Arena_TeamConfig(CUIEvent uiEvent)
        {
            Singleton<CHeroSelectBaseSystem>.instance.SetPveDataWithArena(0, null, Singleton<CTextManager>.GetInstance().GetText("Arena_Arena_Defensive_Team"));
            Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enArenaDefTeamConfig, 3, 0, 0, 0);
        }

        public void BattleReturn(bool isWin)
        {
            if (!isWin)
            {
                Singleton<CArenaSystem>.GetInstance().m_nextCanFightTimes = CRoleInfo.GetElapseSecondsSinceLogin() + ((int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x31).dwConfValue);
            }
            Singleton<CArenaSystem>.GetInstance().m_serverInfo.chAlreadyFightCnt = (sbyte) (Singleton<CArenaSystem>.GetInstance().m_serverInfo.chAlreadyFightCnt + 1);
        }

        public void Clear()
        {
            this.m_lastRankRequestTime = 0;
            this.m_lastRecordRequestTime = 0;
            this.m_lastFighterInfoRequestTime = 0;
        }

        private int GetDefTeamBattleValue()
        {
            return 0;
        }

        public static COMDT_ARENA_MEMBER_OF_ACNT GetFighterInfo(COMDT_ARENA_BLOCKDATA rankInfo)
        {
            COMDT_ARENA_MEMBER_OF_ACNT comdt_arena_member_of_acnt = new COMDT_ARENA_MEMBER_OF_ACNT();
            if (rankInfo.bMemberType == 1)
            {
                return rankInfo.stMember.stAcnt;
            }
            comdt_arena_member_of_acnt.ullUid = rankInfo.stMember.stNpc.dwObjId;
            ResNpcOfArena dataByKey = GameDataMgr.npcOfArena.GetDataByKey((uint) comdt_arena_member_of_acnt.ullUid);
            ResRobotBattleList list = GameDataMgr.robotBattleListInfo.GetDataByKey(dataByKey.dwBattleList);
            ListView<COMDT_ARENA_HERODETAIL> inList = new ListView<COMDT_ARENA_HERODETAIL>();
            uint[] numArray = new uint[] { (uint) (dataByKey.dwForceValue * 0.33), (uint) (dataByKey.dwForceValue * 0.33), (uint) (dataByKey.dwForceValue * 0.34) };
            uint num = 0;
            for (int i = 0; i < numArray.Length; i++)
            {
                ResRobotPower robotHeroInfo = GetRobotHeroInfo(numArray[i]);
                COMDT_ARENA_HERODETAIL item = new COMDT_ARENA_HERODETAIL {
                    dwHeroId = list.HeroList[i]
                };
                item.stHeroQuality.wQuality = (ushort) robotHeroInfo.iQuality;
                item.stHeroQuality.wSubQuality = (ushort) robotHeroInfo.iSubQuality;
                item.wHeroStar = (ushort) robotHeroInfo.iStar;
                item.wHeroLevel = robotHeroInfo.wLevel;
                if (item.wHeroLevel > dataByKey.dwNpcLevel)
                {
                    item.wHeroLevel = (ushort) dataByKey.dwNpcLevel;
                }
                num += robotHeroInfo.dwPower;
                inList.Add(item);
            }
            StringHelper.StringToUTF8Bytes(dataByKey.szNpcName, ref comdt_arena_member_of_acnt.szName);
            comdt_arena_member_of_acnt.dwPVPLevel = dataByKey.dwNpcLevel;
            comdt_arena_member_of_acnt.dwForceValue = num;
            StringHelper.StringToUTF8Bytes(dataByKey.dwNpcHeadId.ToString(), ref comdt_arena_member_of_acnt.szHeadUrl);
            comdt_arena_member_of_acnt.stBattleHero = new COMDT_ARENA_HEROINFO();
            comdt_arena_member_of_acnt.stBattleHero.astHero = LinqS.ToArray<COMDT_ARENA_HERODETAIL>(inList);
            return comdt_arena_member_of_acnt;
        }

        public int GetLessFightTimes()
        {
            return (this.m_serverInfo.bChallengeLimitCnt - this.m_serverInfo.chAlreadyFightCnt);
        }

        public ListView<CUseable> GetRewardUseableList(int rank)
        {
            <GetRewardUseableList>c__AnonStorey67 storey = new <GetRewardUseableList>c__AnonStorey67 {
                rank = rank
            };
            ListView<CUseable> view = new ListView<CUseable>();
            ResArenaOneDayReward reward = GameDataMgr.arenaRewardDatabin.FindIf(new Func<ResArenaOneDayReward, bool>(storey.<>m__4A));
            if (reward != null)
            {
                view.Add(CUseableManager.CreateVirtualUseable(enVirtualItemType.enDiamond, (int) reward.dwRewardDiamond));
                view.Add(CUseableManager.CreateVirtualUseable(enVirtualItemType.enGoldCoin, (int) reward.dwRewardCoin));
                view.Add(CUseableManager.CreateVirtualUseable(enVirtualItemType.enArenaCoin, (int) reward.dwRewardArenaCoin));
                view.Add(CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, reward.dwRewardPropId, (int) reward.dwRewardPropCnt));
            }
            return view;
        }

        public static ResRobotPower GetRobotHeroInfo(uint forceValue)
        {
            <GetRobotHeroInfo>c__AnonStorey68 storey = new <GetRobotHeroInfo>c__AnonStorey68 {
                forceValue = forceValue,
                res = null
            };
            GameDataMgr.robotHeroInfo.Accept(new Action<ResRobotPower>(storey.<>m__4B));
            if (storey.res == null)
            {
                GameDataMgr.robotHeroInfo.Accept(new Action<ResRobotPower>(storey.<>m__4C));
            }
            return storey.res;
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenForm, new CUIEventManager.OnUIEventHandler(this.Arena_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_TeamConfig, new CUIEventManager.OnUIEventHandler(this.Arena_TeamConfig));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ChangeHeroList, new CUIEventManager.OnUIEventHandler(this.Arena_ChangeHeroList));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_BuyChangeTimes, new CUIEventManager.OnUIEventHandler(this.Arena_BuyChangeTimes));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ResetCD, new CUIEventManager.OnUIEventHandler(this.Arena_ResetCD));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ConfirmBuyChangeTimes, new CUIEventManager.OnUIEventHandler(this.Arena_ConfirmBuyChangeTimes));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ConfirmBuyResetCD, new CUIEventManager.OnUIEventHandler(this.Arena_ConfirmResetCD));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_CDTimeEnd, new CUIEventManager.OnUIEventHandler(this.Arena_CDTimeEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenPlayerInfoForm, new CUIEventManager.OnUIEventHandler(this.Arena_OpenPlayerInfoForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenRankInfoForm, new CUIEventManager.OnUIEventHandler(this.Arena_OpenRankInfoForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_RankElementEnable, new CUIEventManager.OnUIEventHandler(CArenaSystem.Arena_RankElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenRecordForm, new CUIEventManager.OnUIEventHandler(this.Arena_OpenRecordForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_RecordElementEnable, new CUIEventManager.OnUIEventHandler(this.Arena_RecordlementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenShopForm, new CUIEventManager.OnUIEventHandler(this.Arena_OpenShopForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_StartFight, new CUIEventManager.OnUIEventHandler(this.Arena_StartFight));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_RankInfoMenuClick, new CUIEventManager.OnUIEventHandler(this.Arena_RankInfoMenuClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ReciveDefTeamInfo, new CUIEventManager.OnUIEventHandler(this.Arena_ReciveDefTeamInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ResetByNewDay, new CUIEventManager.OnUIEventHandler(this.Arena_ResetByNewDay));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_ConfirmResutlForm, new CUIEventManager.OnUIEventHandler(this.Arena_ConfirmResutlForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OpenTips, new CUIEventManager.OnUIEventHandler(this.Arena_OpenTips));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_CloseTps, new CUIEventManager.OnUIEventHandler(this.Arena_CloseTps));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Arena_OnClose, new CUIEventManager.OnUIEventHandler(this.Arena_OnCloseForm));
            s_mapKey = GameDataMgr.arenaLevelDatabin.GetAnyData().iCfgID;
        }

        public void InitServerData(CSDT_ACNT_ARENADATA serverData)
        {
            this.m_serverInfo = serverData;
            this.m_nextCanFightTimes = CRoleInfo.GetElapseSecondsSinceLogin() + ((int) this.m_serverInfo.dwLeftFightCoolTime);
        }

        [MessageHandler(0xb55)]
        public static void ReciveDefTeamListInfo(CSPkg msg)
        {
            SCPKG_SETBATTLELISTOFARENA_RSP stSetBattleListOfArenaRsp = msg.stPkgData.stSetBattleListOfArenaRsp;
            if (stSetBattleListOfArenaRsp.bErrCode == 0)
            {
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.Clear();
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_arenaDefHeroList.AddRange(stSetBattleListOfArenaRsp.stResult.stSucc.stBattleList.BattleHeroList);
                Singleton<CArenaSystem>.GetInstance().RefreshArenaForm();
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0xb55, stSetBattleListOfArenaRsp.bErrCode), false, 1.5f, null, new object[0]);
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        [MessageHandler(0xb5c)]
        public static void ReciveFightHeroListInfo(CSPkg msg)
        {
            SCPKG_CHGARENAFIGHTERRSP stChgArenaFighterRsp = msg.stPkgData.stChgArenaFighterRsp;
            Singleton<CArenaSystem>.GetInstance().m_fightHeroInfoList = stChgArenaFighterRsp;
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (Singleton<CArenaSystem>.instance.m_openArenaForm)
            {
                Singleton<CArenaSystem>.instance.m_openArenaForm = false;
                if (Singleton<CUIManager>.GetInstance().GetForm(s_arenaFormPath) == null)
                {
                    Singleton<CUIManager>.GetInstance().OpenForm(s_arenaFormPath, false, true);
                }
            }
            if (Singleton<CUIManager>.GetInstance().GetForm(s_arenaFormPath) != null)
            {
                Singleton<CArenaSystem>.GetInstance().RefreshArenaForm();
            }
            Singleton<EventRouter>.instance.BroadCastEvent("Arena_Fighter_Changed");
        }

        [MessageHandler(0xb58)]
        public static void ReciveJoinArenaInfo(CSPkg msg)
        {
            SCPKG_JOINARENA_RSP stJoinArenaRsp = msg.stPkgData.stJoinArenaRsp;
            if (stJoinArenaRsp.stResult.bErrCode == 0)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Arena_OpenForm);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0xb58, stJoinArenaRsp.stResult.bErrCode), false, 1.5f, null, new object[0]);
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        [MessageHandler(0xb5e)]
        public static void ReciveRankListInfo(CSPkg msg)
        {
            SCPKG_GETTOPFIGHTEROFARENA_RSP stGetTopFighterOfArenaRsp = msg.stPkgData.stGetTopFighterOfArenaRsp;
            Singleton<CArenaSystem>.GetInstance().m_rankInfoList = stGetTopFighterOfArenaRsp.stTopFighter;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            for (int i = 0; i < stGetTopFighterOfArenaRsp.stTopFighter.bFigterNum; i++)
            {
                if ((stGetTopFighterOfArenaRsp.stTopFighter.astFigterDetail[i].stFigterData.bMemberType == 1) && (masterRoleInfo.playerUllUID == stGetTopFighterOfArenaRsp.stTopFighter.astFigterDetail[i].stFigterData.stMember.stAcnt.ullUid))
                {
                    stGetTopFighterOfArenaRsp.stTopFighter.astFigterDetail[i].stFigterData.stMember.stAcnt.stBattleHero.astHero = new COMDT_ARENA_HERODETAIL[3];
                    for (int j = 0; j < masterRoleInfo.m_arenaDefHeroList.Count; j++)
                    {
                        stGetTopFighterOfArenaRsp.stTopFighter.astFigterDetail[i].stFigterData.stMember.stAcnt.stBattleHero.astHero[j] = new COMDT_ARENA_HERODETAIL { dwHeroId = masterRoleInfo.m_arenaDefHeroList[j] };
                    }
                    break;
                }
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (Singleton<CUIManager>.GetInstance().GetForm(s_arenaFormPath) != null)
            {
                Singleton<CUIManager>.GetInstance().OpenForm(s_arenaRankInfoFormPath, false, true);
                Singleton<CArenaSystem>.GetInstance().RefreshRankForm();
            }
            Singleton<EventRouter>.instance.BroadCastEvent("Rank_Arena_List");
        }

        [MessageHandler(0xb60)]
        public static void ReciveRecordListInfo(CSPkg msg)
        {
            SCPKG_GETARENAFIGHTHISTORY_RSP stGetArenaFightHistoryRsp = msg.stPkgData.stGetArenaFightHistoryRsp;
            Singleton<CArenaSystem>.GetInstance().m_recordList.Clear();
            for (int i = 0; i < stGetArenaFightHistoryRsp.bNum; i++)
            {
                Singleton<CArenaSystem>.GetInstance().m_recordList.Add(stGetArenaFightHistoryRsp.astRecord[i]);
            }
            Singleton<CArenaSystem>.GetInstance().m_recordList.Sort(new Comparison<COMDT_ARENA_FIGHT_RECORD>(CArenaSystem.SortCompareRecord));
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (Singleton<CUIManager>.GetInstance().GetForm(s_arenaFormPath) != null)
            {
                Singleton<CUIManager>.GetInstance().OpenForm(s_arenaFightRecordFormPath, false, true);
                Singleton<CArenaSystem>.GetInstance().RefreshRecordForm();
            }
            Singleton<EventRouter>.instance.BroadCastEvent("Arena_Record_List");
        }

        [MessageHandler(0x4ef)]
        public static void ReciveResetCD(CSPkg msg)
        {
            if (msg.stPkgData.stClrCdLimitRsp.bResult == 0)
            {
                Singleton<CArenaSystem>.GetInstance().m_nextCanFightTimes = CRoleInfo.GetElapseSecondsSinceLogin();
                Singleton<CArenaSystem>.GetInstance().RefreshArenaForm();
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Reset CD Fail", false, 1.5f, null, new object[0]);
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        public void RefreshArenaForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_arenaFormPath);
            if (form != null)
            {
                CUITimerScript component = form.gameObject.transform.Find("Timer").gameObject.GetComponent<CUITimerScript>();
                component.SetTotalTime((float) (this.m_serverInfo.dwNextRefreshLeftTime - CRoleInfo.GetElapseSecondsSinceLogin()));
                component.StartTimer();
                CUIListScript script3 = form.gameObject.transform.Find("Root/panelTop/List").gameObject.GetComponent<CUIListScript>();
                Text text = form.gameObject.transform.Find("Root/panelTop/lblFightValue").gameObject.GetComponent<Text>();
                Text text2 = form.gameObject.transform.Find("Root/panelRight/lblTitle").gameObject.GetComponent<Text>();
                Text text3 = form.gameObject.transform.Find("Root/panelCenter/lblTitle").gameObject.GetComponent<Text>();
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                List<uint> arenaDefHeroList = masterRoleInfo.m_arenaDefHeroList;
                string[] args = new string[] { masterRoleInfo.PvpLevel.ToString() };
                text.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Fighting_Power", args);
                if (this.m_fightHeroInfoList.stArenaInfo.dwSelfRank > 0)
                {
                    string[] textArray2 = new string[] { this.m_fightHeroInfoList.stArenaInfo.dwSelfRank.ToString() };
                    text2.text = Singleton<CTextManager>.GetInstance().GetText("Arena_My_Rank", textArray2);
                }
                else
                {
                    text2.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Not_In_Rank");
                }
                if (this.m_fightHeroInfoList.stArenaInfo.dwSelfRank == 1)
                {
                    text3.gameObject.CustomSetActive(true);
                }
                script3.SetElementAmount(arenaDefHeroList.Count);
                for (int i = 0; i < arenaDefHeroList.Count; i++)
                {
                    GameObject item = script3.GetElemenet(i).gameObject.transform.Find("heroItemCell").gameObject;
                    IHeroData data = CHeroDataFactory.CreateHeroData(arenaDefHeroList[i]);
                    CUICommonSystem.SetHeroItemData(form, item, data, enHeroHeadType.enIcon, false);
                }
                script3 = form.gameObject.transform.Find("Root/panelCenter/List").gameObject.GetComponent<CUIListScript>();
                script3.SetElementAmount(this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.bFigterNum);
                for (int j = 0; j < this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.bFigterNum; j++)
                {
                    CUIListElementScript elemenet = script3.GetElemenet(j);
                    GameObject cell = elemenet.gameObject.transform.Find("heroItemCell").gameObject;
                    Text text4 = elemenet.gameObject.transform.Find("lblName").gameObject.GetComponent<Text>();
                    Text text5 = elemenet.gameObject.transform.Find("lblRank").gameObject.GetComponent<Text>();
                    Text text6 = elemenet.gameObject.transform.Find("lblFight").gameObject.GetComponent<Text>();
                    Text text7 = elemenet.gameObject.transform.Find("lblLevel").gameObject.GetComponent<Text>();
                    COMDT_ARENA_MEMBER_OF_ACNT fighterInfo = GetFighterInfo(this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[j].stFigterData);
                    int bMemberType = this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[j].stFigterData.bMemberType;
                    text4.text = StringHelper.UTF8BytesToString(ref fighterInfo.szName);
                    string[] textArray3 = new string[] { this.m_fightHeroInfoList.stArenaInfo.stMatchFighter.astFigterDetail[j].dwRank.ToString() };
                    text5.text = Singleton<CTextManager>.GetInstance().GetText("Arena_13", textArray3);
                    string[] textArray4 = new string[] { fighterInfo.dwPVPLevel.ToString() };
                    text6.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Fighting_Power", textArray4);
                    text7.text = fighterInfo.dwPVPLevel.ToString();
                    text7.gameObject.CustomSetActive(false);
                    Image image = cell.transform.Find("NobeIcon").GetComponent<Image>();
                    Image image2 = cell.transform.Find("NobeImag").GetComponent<Image>();
                    if (image != null)
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) fighterInfo.stVip.dwCurLevel, false);
                    }
                    if (image2 != null)
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) fighterInfo.stVip.dwHeadIconId);
                    }
                    this.SetPlayerHead(cell, StringHelper.UTF8BytesToString(ref fighterInfo.szHeadUrl), bMemberType, form);
                }
                Text text8 = form.gameObject.transform.Find("Root/panelLeft/lblTitle").gameObject.GetComponent<Text>();
                Button button = form.gameObject.transform.Find("Root/panelLeft/btnChangeHeroList").gameObject.GetComponent<Button>();
                Button button2 = form.gameObject.transform.Find("Root/panelLeft/btnBuyTimes").gameObject.GetComponent<Button>();
                GameObject gameObject = form.gameObject.transform.Find("Root/panelLeft/panelCD").gameObject;
                CUITimerScript script6 = form.gameObject.transform.Find("Root/panelLeft/panelCD/TimerCD").gameObject.GetComponent<CUITimerScript>();
                string[] textArray5 = new string[] { (this.m_serverInfo.bChallengeLimitCnt - this.m_serverInfo.chAlreadyFightCnt).ToString(), this.m_serverInfo.bChallengeLimitCnt.ToString() };
                text8.text = Singleton<CTextManager>.GetInstance().GetText("Arena_Today_Challenge_Count", textArray5);
                int num4 = this.m_nextCanFightTimes - CRoleInfo.GetElapseSecondsSinceLogin();
                if (num4 <= 0)
                {
                    gameObject.CustomSetActive(false);
                    script6.EndTimer();
                }
                else
                {
                    gameObject.CustomSetActive(true);
                    script6.SetTotalTime((float) num4);
                    script6.StartTimer();
                }
                if (this.m_serverInfo.chAlreadyFightCnt >= this.m_serverInfo.bChallengeLimitCnt)
                {
                    button.gameObject.CustomSetActive(false);
                    uint key = (uint) ((9 + this.m_serverInfo.bBuyChallengeCnt) + 1);
                    if (GameDataMgr.shopRefreshCostDatabin.GetDataByKey(key) != null)
                    {
                        button2.gameObject.CustomSetActive(true);
                    }
                }
                else
                {
                    button.gameObject.CustomSetActive(true);
                    button2.gameObject.CustomSetActive(false);
                }
            }
        }

        private void RefreshRankForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_arenaRankInfoFormPath);
            if (form != null)
            {
                GameObject gameObject = form.gameObject.transform.Find("Root/panelRank").gameObject;
                GameObject obj3 = form.gameObject.transform.Find("Root/panelAward").gameObject;
                gameObject.CustomSetActive(false);
                obj3.CustomSetActive(false);
                CUIListScript component = form.gameObject.transform.Find("Root/ListMenu").gameObject.GetComponent<CUIListScript>();
                string[] strArray = new string[] { Singleton<CTextManager>.GetInstance().GetText("Arena_Ranking"), Singleton<CTextManager>.GetInstance().GetText("Arena_Ranking_Award") };
                component.SetElementAmount(strArray.Length);
                for (int i = 0; i < component.m_elementAmount; i++)
                {
                    component.GetElemenet(i).gameObject.transform.Find("Text").GetComponent<Text>().text = strArray[i];
                }
                component.SelectElement(0, true);
            }
        }

        public void RefreshRecordForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_arenaFightRecordFormPath);
            if (form != null)
            {
                form.gameObject.transform.Find("Root/List").gameObject.GetComponent<CUIListScript>().SetElementAmount(this.m_recordList.Count);
            }
        }

        public void Reset()
        {
            this.m_tarObjID = 0L;
            this.m_fightHeroInfoList = null;
            this.m_rankInfoList = null;
        }

        public void ResetFightTimes(int haveBuyTimes, int maxTimes)
        {
            this.m_serverInfo.bBuyChallengeCnt = (byte) haveBuyTimes;
            this.m_serverInfo.bChallengeLimitCnt = (byte) maxTimes;
            this.m_serverInfo.chAlreadyFightCnt = 0;
            this.m_serverInfo.dwLeftFightCoolTime = 0;
            this.RefreshArenaForm();
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        public static void SendBuyTimesMSG()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x459);
            msg.stPkgData.stShopBuyReq = new CSPKG_CMD_SHOPBUY();
            msg.stPkgData.stShopBuyReq.iBuyType = 9;
            msg.stPkgData.stShopBuyReq.iBuySubType = Singleton<CArenaSystem>.GetInstance().m_serverInfo.bBuyChallengeCnt + 1;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendGetFightHeroListMSG(bool bForceSend = true)
        {
            if ((bForceSend || ((CRoleInfo.GetElapseSecondsSinceLogin() - Singleton<CArenaSystem>.GetInstance().m_lastFighterInfoRequestTime) > 60L)) || (Singleton<CArenaSystem>.GetInstance().m_fightHeroInfoList == null))
            {
                Singleton<CArenaSystem>.GetInstance().m_lastFighterInfoRequestTime = (uint) CRoleInfo.GetElapseSecondsSinceLogin();
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xb5b);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        public static void SendGetRankListMSG(bool bForceSend = true)
        {
            if ((bForceSend || ((CRoleInfo.GetElapseSecondsSinceLogin() - Singleton<CArenaSystem>.GetInstance().m_lastRankRequestTime) > 60L)) || (Singleton<CArenaSystem>.GetInstance().m_rankInfoList == null))
            {
                Singleton<CArenaSystem>.GetInstance().m_lastRankRequestTime = (uint) CRoleInfo.GetElapseSecondsSinceLogin();
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xb5d);
                msg.stPkgData.stGetTopFighterOfArenaReq.bTopNum = 100;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        public static void SendGetRecordMSG(bool bForceSend = true)
        {
            if ((bForceSend || ((CRoleInfo.GetElapseSecondsSinceLogin() - Singleton<CArenaSystem>.GetInstance().m_lastRecordRequestTime) > 60L)) || (Singleton<CArenaSystem>.GetInstance().m_recordList == null))
            {
                Singleton<CArenaSystem>.GetInstance().m_lastRecordRequestTime = (uint) CRoleInfo.GetElapseSecondsSinceLogin();
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xb5f);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        public static void SendJoinArenaMSG()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xb57);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendResetCDMSG()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4ee);
            msg.stPkgData.stClrCdLimitReq.dwCdType = 1;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendSetDefTeamConfigMSG(List<uint> heroList)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xb54);
            msg.stPkgData.stSetBattleListOfArenaReq.stBattleList.wHeroCnt = (ushort) heroList.Count;
            msg.stPkgData.stSetBattleListOfArenaReq.stBattleList.BattleHeroList = heroList.ToArray();
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void SetPlayerHead(GameObject cell, string headPath, int memberType, CUIFormScript formScript)
        {
            if (!CSysDynamicBlock.bSocialBlocked)
            {
                CUIHttpImageScript component = cell.transform.Find("httpIcon").GetComponent<CUIHttpImageScript>();
                Image image = cell.transform.Find("npcIcon").GetComponent<Image>();
                if (memberType == 2)
                {
                    component.gameObject.CustomSetActive(false);
                    image.gameObject.CustomSetActive(true);
                    image.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + headPath, formScript, true, false, false);
                }
                else
                {
                    component.gameObject.CustomSetActive(true);
                    image.gameObject.CustomSetActive(false);
                    component.SetImageUrl(headPath);
                }
            }
        }

        private void SetResultCell(GameObject root, string name, uint pvpLevel, int battleValue, uint rank, int memberType, string headPath, CUIFormScript form)
        {
            Text component = root.transform.Find("lblName").GetComponent<Text>();
            Text text2 = root.transform.Find("lblFight").GetComponent<Text>();
            Text text3 = root.transform.Find("lblRank").GetComponent<Text>();
            GameObject gameObject = root.transform.Find("heroItemCell").gameObject;
            component.text = name;
            text2.text = pvpLevel.ToString();
            text3.text = rank.ToString();
            this.SetPlayerHead(gameObject, headPath, memberType, form);
        }

        public void ShowBattleResult(SCPKG_SINGLEGAMEFINRSP settleData)
        {
            if (settleData.stDetail.stGameInfo.bGameResult == 1)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_arenaFightResultFormPath, false, true);
                GameObject gameObject = form.gameObject.transform.Find("Root/resultCell1").gameObject;
                GameObject root = form.gameObject.transform.Find("Root/resultCell2").gameObject;
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                uint pvpLevel = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel;
                this.SetResultCell(gameObject, masterRoleInfo.Name, pvpLevel, this.GetDefTeamBattleValue(), this.m_tarRank, 1, masterRoleInfo.HeadUrl, form);
                this.SetResultCell(root, StringHelper.UTF8BytesToString(ref this.m_tarInfo.szName), this.m_tarInfo.dwPVPLevel, (int) this.m_tarInfo.dwForceValue, this.m_fightHeroInfoList.stArenaInfo.dwSelfRank, this.m_tarType, StringHelper.UTF8BytesToString(ref this.m_tarInfo.szHeadUrl), form);
            }
        }

        public static int SortCompareRecord(COMDT_ARENA_FIGHT_RECORD info1, COMDT_ARENA_FIGHT_RECORD info2)
        {
            if (info2.dwFightTime > info1.dwFightTime)
            {
                return 1;
            }
            if (info2.dwFightTime == info1.dwFightTime)
            {
                return 0;
            }
            return -1;
        }

        [CompilerGenerated]
        private sealed class <GetRewardUseableList>c__AnonStorey67
        {
            internal int rank;

            internal bool <>m__4A(ResArenaOneDayReward x)
            {
                return ((this.rank >= x.dwRankStart) && (this.rank <= x.dwRankEnd));
            }
        }

        [CompilerGenerated]
        private sealed class <GetRobotHeroInfo>c__AnonStorey68
        {
            internal uint forceValue;
            internal ResRobotPower res;

            internal void <>m__4B(ResRobotPower x)
            {
                if ((x.dwPower > this.forceValue) && ((this.res == null) || (this.res.dwPower > x.dwPower)))
                {
                    this.res = x;
                }
            }

            internal void <>m__4C(ResRobotPower x)
            {
                if ((x.dwPower < this.forceValue) && ((this.res == null) || (this.res.dwPower < x.dwPower)))
                {
                    this.res = x;
                }
            }
        }
    }
}

