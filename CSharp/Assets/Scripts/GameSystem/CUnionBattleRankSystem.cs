namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [MessageHandlerClass]
    public class CUnionBattleRankSystem : Singleton<CUnionBattleRankSystem>
    {
        private const int DAY_REWARD_NUM = 6;
        private ResRewardMatchLevelInfo m_CurMapInfo = new ResRewardMatchLevelInfo();
        private uint m_CurSelMapId;
        private int m_CurSelRankItemIndex = -1;
        private enUnionRankMatchType m_CurSelRankMatchType = enUnionRankMatchType.enRankMatchType_None;
        private enUnionRankType m_CurSelRankType = enUnionRankType.enRankType_None;
        private COMDT_GUILD_REWARDPOINT_LIST m_CurTeamRewardPoint = new COMDT_GUILD_REWARDPOINT_LIST();
        private stUnionRankInfo[] m_UnionRankInfo = new stUnionRankInfo[4];
        public static string UNION_RANK_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionRank";

        public void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                this.m_UnionRankInfo[i].lastRetrieveTime = 0;
                this.m_UnionRankInfo[i].listInfo = null;
                this.m_UnionRankInfo[i].selfIndex = -1;
            }
        }

        public static COM_APOLLO_TRANK_SCORE_TYPE ConvertLocalToSeverRankType(enUnionRankType rankType)
        {
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_NULL;
            if ((rankType > enUnionRankType.enRankType_None) && (rankType < enUnionRankType.enRankType_Count))
            {
                com_apollo_trank_score_type = (COM_APOLLO_TRANK_SCORE_TYPE) (rankType + 60);
            }
            return com_apollo_trank_score_type;
        }

        public static enUnionRankType ConvertSeverToLocalRankType(COM_APOLLO_TRANK_SCORE_TYPE rankType)
        {
            enUnionRankType type = enUnionRankType.enRankType_None;
            if ((rankType >= COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_REWARDMATCH_LOW_COIN_WIN) && (rankType <= COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_REWARDMATCH_HIGH_DIAMOND_WIN))
            {
                type = (enUnionRankType) (rankType - 60);
            }
            return type;
        }

        private CSDT_RANKING_LIST_ITEM_INFO GetActRankInfo(enUnionRankType rankType)
        {
            CSDT_RANKING_LIST_SUCC listInfo = this.m_UnionRankInfo[(int) this.m_CurSelRankType].listInfo;
            int selfIndex = this.m_UnionRankInfo[(int) this.m_CurSelRankType].selfIndex;
            if (((listInfo != null) && (selfIndex >= 0)) && ((selfIndex < listInfo.astItemDetail.Length) && (selfIndex < listInfo.dwItemNum)))
            {
                return listInfo.astItemDetail[selfIndex];
            }
            return new CSDT_RANKING_LIST_ITEM_INFO();
        }

        private COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER GetRankItemDetailInfo(enUnionRankType rankType, int listIndex)
        {
            CSDT_RANKING_LIST_SUCC listInfo = this.m_UnionRankInfo[(int) this.m_CurSelRankType].listInfo;
            if (((listInfo != null) && (listIndex < listInfo.astItemDetail.Length)) && (listIndex < listInfo.dwItemNum))
            {
                switch (rankType)
                {
                    case enUnionRankType.enRankMatchType_WinCntCoinCoinMatchLow:
                        return listInfo.astItemDetail[listIndex].stExtraInfo.stDetailInfo.stLowCoinWin;

                    case enUnionRankType.enRankMatchType_WinCntCoinMatchHigh:
                        return listInfo.astItemDetail[listIndex].stExtraInfo.stDetailInfo.stHighCoinWin;

                    case enUnionRankType.enRankMatchType_WinCntDiamondMatchLow:
                        return listInfo.astItemDetail[listIndex].stExtraInfo.stDetailInfo.stLowDiamondWin;

                    case enUnionRankType.enRankMatchType_WinCntDiamondMatchHigh:
                        return listInfo.astItemDetail[listIndex].stExtraInfo.stDetailInfo.stHighDiamondWin;
                }
            }
            return new COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER();
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_Rank, new CUIEventManager.OnUIEventHandler(this.OnClickRank));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_MatchType_Menu, new CUIEventManager.OnUIEventHandler(this.OnClickMatchTypeMenu));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Rank_ClickDetail, new CUIEventManager.OnUIEventHandler(this.OnClickDetail));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Rank_DateList_Element_Enable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("UnionRank_Get_Rank_List", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetRankList));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>("UnionRank_Get_Rank_Account_Info", new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.OnGetAccountInfo));
        }

        public void initWidget()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(UNION_RANK_PATH);
            if (form != null)
            {
                this.m_CurSelRankType = enUnionRankType.enRankType_None;
                this.m_CurSelRankMatchType = enUnionRankMatchType.enRankMatchType_None;
                this.m_CurSelRankItemIndex = -1;
                this.m_CurSelMapId = 0;
                CUIListScript component = form.GetWidget(2).GetComponent<CUIListScript>();
                int unionBattleMapCount = CUnionBattleEntrySystem.GetUnionBattleMapCount();
                component.SetElementAmount(unionBattleMapCount);
                int index = 0;
                for (int i = 0; i < unionBattleMapCount; i++)
                {
                    ResRewardMatchLevelInfo unionBattleMapInfoByIndex = CUnionBattleEntrySystem.GetUnionBattleMapInfoByIndex(i);
                    if (CUICommonSystem.IsMatchOpened(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, unionBattleMapInfoByIndex.dwMapId))
                    {
                        CUIListElementScript elemenet = component.GetElemenet(index);
                        if (elemenet != null)
                        {
                            CUIEventScript script4 = elemenet.GetComponent<CUIEventScript>();
                            elemenet.transform.FindChild("Text").GetComponent<Text>().text = unionBattleMapInfoByIndex.szMatchName;
                            script4.m_onClickEventParams.tagUInt = unionBattleMapInfoByIndex.dwMapId;
                            script4.m_onClickEventParams.commonUInt32Param1 = unionBattleMapInfoByIndex.dwMatchType;
                        }
                        index++;
                    }
                }
                if (index != unionBattleMapCount)
                {
                    component.SetElementAmount(index);
                }
            }
        }

        public bool IsNeedToRetrieveRankTypeInfo(enUnionRankType rankType)
        {
            if ((rankType <= enUnionRankType.enRankType_None) || (rankType >= enUnionRankType.enRankType_Count))
            {
                return false;
            }
            int index = (int) rankType;
            return (((this.m_UnionRankInfo[index].listInfo == null) || (this.m_UnionRankInfo[index].lastRetrieveTime == 0)) || (CRoleInfo.GetCurrentUTCTime() >= (this.m_UnionRankInfo[index].lastRetrieveTime + this.m_UnionRankInfo[index].listInfo.dwTimeLimit)));
        }

        private void OnClickDetail(CUIEvent uiEvt)
        {
            int selectedIndex = uiEvt.m_srcWidgetBelongedListScript.GetComponent<CUIListScript>().GetSelectedIndex();
            this.m_CurSelRankItemIndex = selectedIndex;
            COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER rankItemDetailInfo = this.GetRankItemDetailInfo(this.m_CurSelRankType, this.m_CurSelRankItemIndex);
            ulong ullUid = 0L;
            int iLogicWorldId = 0;
            if (rankItemDetailInfo != null)
            {
                ullUid = rankItemDetailInfo.ullUid;
                iLogicWorldId = rankItemDetailInfo.iLogicWorldId;
                if ((ullUid == Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID) && (iLogicWorldId == MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID))
                {
                    Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(ullUid, iLogicWorldId, CPlayerInfoSystem.DetailPlayerInfoSource.Self);
                }
                else
                {
                    Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(ullUid, iLogicWorldId, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers);
                }
            }
        }

        private void OnClickMatchTypeMenu(CUIEvent uiEvt)
        {
            this.SelectRankMatchType(uiEvt);
        }

        private void OnClickRank(CUIEvent uiEvt)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(UNION_RANK_PATH, false, true);
            if (script != null)
            {
                this.Clear();
                this.initWidget();
                CUIListScript component = script.GetWidget(2).GetComponent<CUIListScript>();
                if ((component != null) && (component.GetElementAmount() > 0))
                {
                    component.SelectElement(0, true);
                    CUIEventScript script3 = component.GetElemenet(0).GetComponent<CUIEventScript>();
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(script3.m_onClickEventID, script3.m_onClickEventParams);
                }
            }
        }

        private void OnElementEnable(CUIEvent uiEvt)
        {
            this.RefreshOneWinCntElement(uiEvt.m_srcWidget, uiEvt.m_srcWidgetIndexInBelongedList);
        }

        private void OnGetAccountInfo(SCPKG_GET_RANKING_ACNT_INFO_RSP acntInfo)
        {
            enUnionRankType type = ConvertSeverToLocalRankType((COM_APOLLO_TRANK_SCORE_TYPE) acntInfo.stAcntRankingDetail.stOfSucc.bNumberType);
            if (type != enUnionRankType.enRankType_None)
            {
                this.m_UnionRankInfo[(int) type].lastRetrieveTime = (uint) CRoleInfo.GetCurrentUTCTime();
                this.RefreshAcntInfo();
            }
        }

        private void OnGetRankList(SCPKG_GET_RANKING_LIST_RSP rankList)
        {
            enUnionRankType rankType = ConvertSeverToLocalRankType((COM_APOLLO_TRANK_SCORE_TYPE) rankList.stRankingListDetail.stOfSucc.bNumberType);
            if (rankType != enUnionRankType.enRankType_None)
            {
                this.m_UnionRankInfo[(int) rankType].lastRetrieveTime = (uint) CRoleInfo.GetCurrentUTCTime();
                this.m_UnionRankInfo[(int) rankType].listInfo = rankList.stRankingListDetail.stOfSucc;
                CSDT_RANKING_LIST_SUCC listInfo = this.m_UnionRankInfo[(int) rankType].listInfo;
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    this.m_UnionRankInfo[(int) rankType].selfIndex = -1;
                    for (int i = 0; i < listInfo.dwItemNum; i++)
                    {
                        COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER rankItemDetailInfo = this.GetRankItemDetailInfo(rankType, i);
                        if (masterRoleInfo.playerUllUID == rankItemDetailInfo.ullUid)
                        {
                            this.m_UnionRankInfo[(int) rankType].selfIndex = i;
                        }
                    }
                    this.RefreshWinCntRankList();
                    this.RefreshAcntInfo();
                }
            }
        }

        private static void RankNobSet(uint rankNumber, RankingItemHelper rankingHelper)
        {
            rankingHelper.RankingNumText.CustomSetActive(false);
            rankingHelper.No1.CustomSetActive(false);
            rankingHelper.No2.CustomSetActive(false);
            rankingHelper.No3.CustomSetActive(false);
            rankingHelper.No1BG.CustomSetActive(false);
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
                        if (rankingHelper.No1BG != null)
                        {
                            rankingHelper.No1BG.CustomSetActive(true);
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

        private void RefreshAcntInfo()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(UNION_RANK_PATH);
            if (form != null)
            {
                CSDT_RANKING_LIST_ITEM_INFO actRankInfo = this.GetActRankInfo(this.m_CurSelRankType);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                GameObject widget = form.GetWidget(1);
                RankingItemHelper component = widget.GetComponent<RankingItemHelper>();
                uint rankNumber = 0;
                if ((actRankInfo != null) && (masterRoleInfo != null))
                {
                    widget.CustomSetActive(true);
                    string name = masterRoleInfo.Name;
                    uint level = masterRoleInfo.Level;
                    rankNumber = actRankInfo.dwRankNo;
                    uint dwPerfectCnt = 0;
                    if (rankNumber == 0)
                    {
                        dwPerfectCnt = CUnionBattleEntrySystem.GetRewardMatchStateByMapId(this.m_CurSelMapId).dwPerfectCnt;
                    }
                    else
                    {
                        dwPerfectCnt = actRankInfo.dwRankScore;
                    }
                    widget.transform.FindChild("Value").gameObject.CustomSetActive(true);
                    widget.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
                    SetGameObjChildText(widget, "Value", dwPerfectCnt.ToString(CultureInfo.InvariantCulture));
                    SetGameObjChildText(widget, "NameGroup/PlayerName", masterRoleInfo.Name);
                    SetGameObjChildText(widget, "PlayerLv", string.Format("Lv.{0}", level.ToString(CultureInfo.InvariantCulture)));
                }
                RankNobSet(rankNumber, component);
                MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component.QqVip.GetComponent<Image>());
                CUICommonSystem.SetHostHeadItemCell(widget.transform.FindChild("HeadItemCell").gameObject);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Wechat, true, false);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.QQ, true, false);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Guest, true, false);
            }
        }

        private void RefreshOneWinCntElement(GameObject element, int index)
        {
            CSDT_RANKING_LIST_SUCC listInfo = this.m_UnionRankInfo[(int) this.m_CurSelRankType].listInfo;
            int num = index;
            if (((element != null) && (listInfo != null)) && ((num < listInfo.astItemDetail.Length) && (num < listInfo.dwItemNum)))
            {
                RankingItemHelper component = element.GetComponent<RankingItemHelper>();
                uint dwRankScore = 0;
                string text = string.Empty;
                uint dwPvpLevel = 1;
                string serverUrl = null;
                uint dwCurLevel = 0;
                uint dwHeadIconId = 0;
                uint dwVipLevel = 0;
                COM_PRIVILEGE_TYPE privilegeType = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
                dwRankScore = listInfo.astItemDetail[num].dwRankScore;
                COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER rankItemDetailInfo = this.GetRankItemDetailInfo(this.m_CurSelRankType, num);
                text = StringHelper.UTF8BytesToString(ref rankItemDetailInfo.szPlayerName);
                dwPvpLevel = rankItemDetailInfo.dwPvpLevel;
                serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref rankItemDetailInfo.szHeadUrl);
                dwCurLevel = rankItemDetailInfo.stGameVip.dwCurLevel;
                dwHeadIconId = rankItemDetailInfo.stGameVip.dwHeadIconId;
                privilegeType = (COM_PRIVILEGE_TYPE) rankItemDetailInfo.bPrivilege;
                dwVipLevel = rankItemDetailInfo.dwVipLevel;
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, privilegeType, ApolloPlatform.Wechat, true, false);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, privilegeType, ApolloPlatform.QQ, true, false);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, privilegeType, ApolloPlatform.Guest, true, false);
                SetGameObjChildText(element, "NameGroup/PlayerName", text);
                SetGameObjChildText(element, "PlayerLv", string.Format("Lv.{0}", Math.Max(1, dwPvpLevel)));
                element.transform.FindChild("Value").gameObject.CustomSetActive(true);
                SetGameObjChildText(element, "Value", dwRankScore.ToString(CultureInfo.InvariantCulture));
                uint rankNumber = (uint) (num + 1);
                RankNobSet(rankNumber, component);
                if (!CSysDynamicBlock.bSocialBlocked)
                {
                    if (rankItemDetailInfo.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component.QqVip.GetComponent<Image>());
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.VipIcon.GetComponent<Image>(), (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false);
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component.HeadIconFrame.GetComponent<Image>(), (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Wechat, true, false);
                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.QQ, true, false);
                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Guest, true, false);
                        RankingView.SetHostUrlHeadIcon(component.HeadIcon);
                    }
                    else
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(component.QqVip.GetComponent<Image>(), (int) dwVipLevel);
                        MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.VipIcon.GetComponent<Image>(), (int) dwCurLevel, false);
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component.HeadIconFrame.GetComponent<Image>(), (int) dwHeadIconId);
                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, privilegeType, ApolloPlatform.Wechat, true, false);
                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, privilegeType, ApolloPlatform.QQ, true, false);
                        MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, privilegeType, ApolloPlatform.Guest, true, false);
                        RankingView.SetUrlHeadIcon(component.HeadIcon, serverUrl);
                    }
                }
            }
        }

        private void RefreshWinCntRankList()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(UNION_RANK_PATH);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                CSDT_RANKING_LIST_SUCC listInfo = this.m_UnionRankInfo[(int) this.m_CurSelRankType].listInfo;
                Transform transform = widget.transform.FindChild("RankingList");
                Transform transform2 = widget.transform.FindChild("NoRankTxt");
                if ((listInfo == null) || (listInfo.dwItemNum == 0))
                {
                    transform.gameObject.CustomSetActive(false);
                    transform2.gameObject.CustomSetActive(true);
                }
                else
                {
                    transform.gameObject.CustomSetActive(true);
                    transform2.gameObject.CustomSetActive(false);
                    int dwItemNum = (int) listInfo.dwItemNum;
                    CUIListScript component = transform.GetComponent<CUIListScript>();
                    component.SetElementAmount(dwItemNum);
                    component.MoveElementInScrollArea(0, true);
                    for (int i = 0; i < dwItemNum; i++)
                    {
                        if ((component.GetElemenet(i) != null) && component.IsElementInScrollArea(i))
                        {
                            this.RefreshOneWinCntElement(component.GetElemenet(i).gameObject, i);
                        }
                    }
                }
            }
        }

        public static void ReqRankListInfo(COM_APOLLO_TRANK_SCORE_TYPE rankType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2a);
            msg.stPkgData.stGetRankingListReq.bNumberType = (byte) rankType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        [MessageHandler(0x1459)]
        public static void ReqTeamRewardAcntPoint(CSPkg msg)
        {
            Singleton<CUnionBattleRankSystem>.instance.m_CurTeamRewardPoint = msg.stPkgData.stNtfGuildRewardPoint.stGuildPoint;
        }

        private void RetrieveRankTypeInfo(enUnionRankType rankType)
        {
            ReqRankListInfo(ConvertLocalToSeverRankType(rankType));
        }

        private void SelectRankMatchType(CUIEvent uiEvt)
        {
            uint tagUInt = uiEvt.m_eventParams.tagUInt;
            enUnionRankMatchType type = (enUnionRankMatchType) uiEvt.m_eventParams.commonUInt32Param1;
            if (this.m_CurSelMapId != tagUInt)
            {
                this.m_CurSelMapId = tagUInt;
                this.m_CurSelRankMatchType = type;
                this.m_CurMapInfo = CUnionBattleEntrySystem.GetUnionBattleMapInfoByMapID(this.m_CurSelMapId);
                this.m_CurSelRankType = ConvertSeverToLocalRankType((COM_APOLLO_TRANK_SCORE_TYPE) this.m_CurMapInfo.dwRankType);
                if (Singleton<CUIManager>.GetInstance().GetForm(UNION_RANK_PATH) != null)
                {
                    if (this.IsNeedToRetrieveRankTypeInfo(this.m_CurSelRankType))
                    {
                        this.RetrieveRankTypeInfo(this.m_CurSelRankType);
                    }
                    this.RefreshWinCntRankList();
                    this.RefreshAcntInfo();
                }
            }
        }

        private static void SetGameObjChildText(GameObject parentObj, string childName, string text)
        {
            if (parentObj != null)
            {
                parentObj.transform.FindChild(childName).gameObject.GetComponent<Text>().text = text;
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_Rank, new CUIEventManager.OnUIEventHandler(this.OnClickRank));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_MatchType_Menu, new CUIEventManager.OnUIEventHandler(this.OnClickMatchTypeMenu));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Rank_ClickDetail, new CUIEventManager.OnUIEventHandler(this.OnClickDetail));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Rank_DateList_Element_Enable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<SCPKG_GET_RANKING_LIST_RSP>("UnionRank_Get_Rank_List", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetRankList));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>("UnionRank_Get_Rank_Account_Info", new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.OnGetAccountInfo));
        }

        public enum enWidget
        {
            enWidGet_RankList,
            enWidGet_SelfInfo,
            enWidGet_MatchTypeMenu
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct stUnionRankInfo
        {
            public uint lastRetrieveTime;
            public CSDT_RANKING_LIST_SUCC listInfo;
            public int selfIndex;
        }
    }
}

