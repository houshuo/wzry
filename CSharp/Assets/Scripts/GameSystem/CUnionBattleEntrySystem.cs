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
    public class CUnionBattleEntrySystem : Singleton<CUnionBattleEntrySystem>
    {
        public SCPKG_GETAWARDPOOL_RSP m_awardPoolInfo = new SCPKG_GETAWARDPOOL_RSP();
        public SCPKG_GET_MATCHINFO_RSP m_baseInfo = new SCPKG_GET_MATCHINFO_RSP();
        public SCPKG_MATCHPOINT_NTF m_personInfo = new SCPKG_MATCHPOINT_NTF();
        private const RES_BATTLE_MAP_TYPE m_ResMapType = RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH;
        private uint m_selectMapID;
        private ResRewardMatchLevelInfo m_selectMapRes;
        private COMDT_REWARDMATCH_RECORD m_selectStateInfo;
        private ResRewardMatchTimeInfo m_selectTimeInfo;
        public COMDT_REWARDMATCH_DATA m_stateInfo = new COMDT_REWARDMATCH_DATA();
        private readonly int m_unionBattleRuleId = 10;
        private readonly int m_unionConfirmEntryRuleId = 13;
        public stLastWinLoseCntChginfo[] m_WinLoseChgInfo;
        public static byte MAX_LOSE_TIME = 3;
        private uint maxMatchRoundCnt;
        private static readonly uint SECOND_ONE_DAY = 0x15180;
        public static string UNION_CONFIRM_ENTRY_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionBattleConfirmEntry";
        public static string UNION_ENTRY_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionBattleEntry";
        public static string UNION_ENTRY_REWARDINTRO_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionBattleRewardIntro";
        public static string UNION_ENTRY_SECOND_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionBattleEntrySecond";
        public static string UNION_ENTRY_THIRD_PATH = "UGUI/Form/System/PvP/UnionBattle/Form_UnionBattleEntryThird";

        private COMDT_MATCHPOINT GetMapPersonMatchPoint()
        {
            if (this.m_personInfo == null)
            {
                return null;
            }
            uint selectMapID = this.m_selectMapID;
            COMDT_MATCHPOINT[] astPointList = this.m_personInfo.astPointList;
            for (int i = 0; i < this.m_personInfo.dwCount; i++)
            {
                if (selectMapID == astPointList[i].dwMapId)
                {
                    return astPointList[i];
                }
            }
            return null;
        }

        private COMDT_REWARDMATCH_RECORD GetMapStateInfo(uint mapId)
        {
            COMDT_REWARDMATCH_RECORD comdt_rewardmatch_record = null;
            for (int i = 0; i < this.m_stateInfo.bRecordCnt; i++)
            {
                if (mapId == this.m_stateInfo.astRecord[i].dwMapId)
                {
                    comdt_rewardmatch_record = this.m_stateInfo.astRecord[i];
                }
            }
            return comdt_rewardmatch_record;
        }

        public ResCommReward GetResCommonReward(uint awardID)
        {
            return GameDataMgr.commonRewardDatabin.GetDataByKey(awardID);
        }

        public static COMDT_REWARDMATCH_RECORD GetRewardMatchStateByMapId(uint mapId)
        {
            COMDT_REWARDMATCH_DATA stateInfo = Singleton<CUnionBattleEntrySystem>.GetInstance().m_stateInfo;
            int length = stateInfo.astRecord.Length;
            COMDT_REWARDMATCH_RECORD comdt_rewardmatch_record = new COMDT_REWARDMATCH_RECORD();
            for (int i = 0; i < length; i++)
            {
                if (stateInfo.astRecord[i].dwMapId == mapId)
                {
                    return (comdt_rewardmatch_record = stateInfo.astRecord[i]);
                }
            }
            return comdt_rewardmatch_record;
        }

        public static int GetUnionBattleMapCount()
        {
            return GameDataMgr.uinionBattleLevelDatabin.count;
        }

        public static ResRewardMatchLevelInfo GetUnionBattleMapInfoByIndex(int mapIndex)
        {
            return GameDataMgr.uinionBattleLevelDatabin.GetDataByIndex(mapIndex);
        }

        public static ResRewardMatchLevelInfo GetUnionBattleMapInfoByMapID(uint mapID)
        {
            return GameDataMgr.uinionBattleLevelDatabin.GetDataByKey(mapID);
        }

        public static void GetWinloSeCntChgInfo(uint dwMapId, ref stLastWinLoseCntChginfo outWinLoseChginfo)
        {
            stLastWinLoseCntChginfo[] winLoseChgInfo = Singleton<CUnionBattleEntrySystem>.instance.m_WinLoseChgInfo;
            for (int i = 0; i < winLoseChgInfo.Length; i++)
            {
                if (winLoseChgInfo[i].mapId == dwMapId)
                {
                    outWinLoseChginfo = winLoseChgInfo[i];
                    break;
                }
            }
        }

        public static bool HasMatchInActiveTime()
        {
            int unionBattleMapCount = GetUnionBattleMapCount();
            for (int i = 0; i < unionBattleMapCount; i++)
            {
                if (CUICommonSystem.GetMatchOpenState(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, GetUnionBattleMapInfoByIndex(i).dwMapId).matchState == enMatchOpenState.enMatchOpen_InActiveTime)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_ClickEntry, new CUIEventManager.OnUIEventHandler(this.Open_BattleEntry));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_BattleEntryGroup_Click, new CUIEventManager.OnUIEventHandler(this.Open_SecondBattleEntry));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_SubBattleEntryGroup_Click, new CUIEventManager.OnUIEventHandler(this.OnClickSecondUIBattleEntry));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_SingleStartMatch, new CUIEventManager.OnUIEventHandler(this.OnClickStartSingleMatch));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_ConfirmBuyItem, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_BuyTiketClick, new CUIEventManager.OnUIEventHandler(this.OnBuyTiketClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_Rule, new CUIEventManager.OnUIEventHandler(this.OnClickRule));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_RewardMatch_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnRewardMatchTimeUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_battle_Click_StartOneMatchRound, new CUIEventManager.OnUIEventHandler(this.OnClickStartMatch));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_RewardIntro, new CUIEventManager.OnUIEventHandler(this.OnClickRewardIntro));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_GetReward, new CUIEventManager.OnUIEventHandler(this.OnClickGetReward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_GotReward, new CUIEventManager.OnUIEventHandler(this.OnGotReward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_RewardIntro_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnRewardElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_ExChgRewwad, new CUIEventManager.OnUIEventHandler(this.OnClickExChgReward));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_CliCk_LoseTips, new CUIEventManager.OnUIEventHandler(this.OnClickLoseTips));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Union_Battle_Click_GetRewardTips, new CUIEventManager.OnUIEventHandler(this.OnClickGetRewardTips));
            int unionBattleMapCount = GetUnionBattleMapCount();
            this.m_WinLoseChgInfo = new stLastWinLoseCntChginfo[unionBattleMapCount];
            for (int i = 0; i < unionBattleMapCount; i++)
            {
                this.m_WinLoseChgInfo[i].mapId = GetUnionBattleMapInfoByIndex(i).dwMapId;
            }
            this.maxMatchRoundCnt = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 200).dwConfValue;
        }

        private void initConfirmFormWidget()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(UNION_CONFIRM_ENTRY_PATH);
            if (form != null)
            {
                Text component = form.GetWidget(0).GetComponent<Text>();
                ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((long) this.m_unionConfirmEntryRuleId);
                component.text = dataByKey.szContent;
                Text text3 = form.GetWidget(1).GetComponent<Text>();
                text3.text = string.Format(text3.text, this.m_selectStateInfo.bMatchCnt, this.maxMatchRoundCnt);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    GameObject widget = form.GetWidget(3);
                    GameObject gameObject = widget.transform.FindChild("lblIconCount").gameObject;
                    Text text4 = gameObject.GetComponent<Text>();
                    int useableStackCount = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID);
                    CUseable itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID, useableStackCount);
                    CUICommonSystem.SetItemCell(form, widget, itemUseable, true, false);
                    text4.text = string.Format("x{0}", useableStackCount);
                    gameObject.CustomSetActive(true);
                }
            }
        }

        private void initFirstFormWidget()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(UNION_ENTRY_PATH);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                GameObject obj3 = form.GetWidget(1);
                if ((widget != null) && (obj3 != null))
                {
                    widget.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 0;
                    obj3.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 1;
                    int dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xab).dwConfValue;
                    if (this.m_baseInfo.dwPlayerNum >= dwConfValue)
                    {
                        string[] args = new string[] { this.m_baseInfo.dwPlayerNum.ToString() };
                        string text = Singleton<CTextManager>.instance.GetText("Union_Battle_Tips12", args);
                        CUICommonSystem.SetTextContent(form.GetWidget(2), text);
                        CUICommonSystem.SetObjActive(form.GetWidget(2), true);
                    }
                    else
                    {
                        CUICommonSystem.SetObjActive(form.GetWidget(2), false);
                    }
                    if (CSysDynamicBlock.bLobbyEntryBlocked && (obj3 != null))
                    {
                        obj3.CustomSetActive(false);
                    }
                }
            }
        }

        private void initRewardIntroFormWidget()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(UNION_ENTRY_REWARDINTRO_PATH);
            if (form != null)
            {
                CUIListScript component = form.GetWidget(0).transform.GetComponent<CUIListScript>();
                int amount = this.m_selectMapRes.bWinCnt + 2;
                component.SetElementAmount(amount);
                for (int i = 0; i <= amount; i++)
                {
                    if ((component.GetElemenet(i) != null) && component.IsElementInScrollArea(i))
                    {
                        this.refreshOneRewardElement(form, component.GetElemenet(i).gameObject, i);
                    }
                }
            }
        }

        private void initSecondFormWidget()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(UNION_ENTRY_SECOND_PATH);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                GameObject btn = form.GetWidget(1);
                GameObject obj4 = form.GetWidget(2);
                if (((widget != null) && (btn != null)) && (obj4 != null))
                {
                    uint[] numArray = new uint[10];
                    bool[] flagArray = new bool[10];
                    uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Union_1"), out numArray[0]);
                    uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_Union_2"), out numArray[1]);
                    flagArray[0] = CUICommonSystem.IsMatchOpened(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, numArray[0]);
                    flagArray[1] = CUICommonSystem.IsMatchOpened(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, numArray[1]);
                    widget.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 0;
                    btn.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 1;
                    obj4.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tag = 2;
                    widget.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = numArray[0];
                    btn.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt = numArray[1];
                    widget.GetComponent<CUIMiniEventScript>().m_onClickEventParams.commonBool = flagArray[0];
                    btn.GetComponent<CUIMiniEventScript>().m_onClickEventParams.commonBool = flagArray[1];
                    widget.transform.FindChild("Lock").gameObject.CustomSetActive(!flagArray[0]);
                    btn.transform.FindChild("Lock").gameObject.CustomSetActive(!flagArray[1]);
                    this.ShowCountDownTime(widget, flagArray[0]);
                    this.ShowCountDownTime(btn, flagArray[1]);
                    widget.transform.FindChild("Desc/MapNameTxt").GetComponent<Text>().text = GetUnionBattleMapInfoByMapID(widget.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt).stLevelCommonInfo.szName;
                    btn.transform.FindChild("Desc/MapNameTxt").GetComponent<Text>().text = GetUnionBattleMapInfoByMapID(btn.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt).stLevelCommonInfo.szName;
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        if (obj4 != null)
                        {
                            obj4.CustomSetActive(false);
                        }
                        if (btn != null)
                        {
                            btn.CustomSetActive(false);
                        }
                        GameObject gameObject = form.transform.FindChild("panelBottom/Button").gameObject;
                        if (gameObject != null)
                        {
                            gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void initThirdFormWidget()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(UNION_ENTRY_THIRD_PATH);
            if ((form != null) && (this.m_selectStateInfo != null))
            {
                Transform transform = form.GetWidget(2).transform;
                int bLossCnt = this.m_selectStateInfo.bLossCnt;
                GameObject[] objArray = new GameObject[MAX_LOSE_TIME];
                objArray[0] = transform.FindChild("loseicon_1").gameObject;
                objArray[1] = transform.FindChild("loseicon_2").gameObject;
                objArray[2] = transform.FindChild("loseicon_3").gameObject;
                for (int i = 0; i < MAX_LOSE_TIME; i++)
                {
                    if ((i + 1) <= bLossCnt)
                    {
                        objArray[i].CustomSetActive(true);
                    }
                    else
                    {
                        objArray[i].CustomSetActive(false);
                    }
                }
                if ((bLossCnt == 2) && (this.m_selectStateInfo.bState != 2))
                {
                    CUICommonSystem.PlayAnimator(form.GetWidget(9).gameObject, "TipsLose_Anim_In_2");
                    form.GetWidget(10).transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips20");
                }
                Transform transform2 = form.GetWidget(0).transform;
                stLastWinLoseCntChginfo outWinLoseChginfo = new stLastWinLoseCntChginfo();
                GetWinloSeCntChgInfo(this.m_selectMapID, ref outWinLoseChginfo);
                if ((outWinLoseChginfo.winLoseChgType == enLastWinLoseCntChgType.enChgType_Win) && (this.m_selectStateInfo.bWinCnt != 0))
                {
                    int num4 = this.m_selectStateInfo.bWinCnt - 1;
                    CUICommonSystem.PlayAnimator(transform2.gameObject, string.Format("VictoryTimes_{0}-{1}", num4.ToString(), this.m_selectStateInfo.bWinCnt.ToString()));
                    SetWinLoseCntChgInfo(this.m_selectMapID, enLastWinLoseCntChgType.enChgType_None);
                    transform2.FindChild("Img_WinCnt").gameObject.CustomSetActive(false);
                }
                else
                {
                    GameObject gameObject = transform2.FindChild("Img_WinCnt").gameObject;
                    gameObject.GetComponent<Image>().SetSprite(string.Format("{0}{1}{2}", "UGUI/Sprite/System/", "UnionBattle/", this.m_selectStateInfo.bWinCnt), form, true, false, false);
                    gameObject.CustomSetActive(true);
                    SetWinLoseCntChgInfo(this.m_selectMapID, enLastWinLoseCntChgType.enChgType_None);
                }
                if (outWinLoseChginfo.winLoseChgType == enLastWinLoseCntChgType.enChgType_Lose)
                {
                    CUICommonSystem.PlayAnimator(transform.gameObject, string.Format("Lose_Anim_{0}", this.m_selectStateInfo.bLossCnt));
                    SetWinLoseCntChgInfo(this.m_selectMapID, enLastWinLoseCntChgType.enChgType_None);
                }
                if (this.m_selectMapRes != null)
                {
                    form.GetWidget(7).GetComponent<Text>().text = this.m_selectMapRes.stLevelCommonInfo.szName;
                    form.GetWidget(8).GetComponent<Text>().text = this.m_selectMapRes.szMatchName;
                }
                ResRewardMatchTimeInfo info = null;
                GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey(5, this.m_selectMapID), out info);
                if (info != null)
                {
                    form.GetWidget(1).GetComponent<Text>().text = info.szTimeTips;
                }
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                    if (this.m_selectMapRes != null)
                    {
                        int useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID);
                        CUseable itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID, useableStackCount);
                        GameObject widget = form.GetWidget(6);
                        Text component = widget.transform.Find("lblIconCount").GetComponent<Text>();
                        CUICommonSystem.SetItemCell(form, widget, itemUseable, true, false);
                        component.text = "x" + useableStackCount.ToString();
                        component.gameObject.CustomSetActive(true);
                        CUICommonSystem.SetHostHeadItemCell(form.GetWidget(5));
                        GameObject obj6 = form.GetWidget(3);
                        GameObject obj7 = form.GetWidget(4);
                        bool bActive = this.m_selectStateInfo.bState == 2;
                        obj6.CustomSetActive(!bActive);
                        obj7.CustomSetActive(bActive);
                        if (bActive)
                        {
                            CUICommonSystem.PlayAnimator(form.GetWidget(9).gameObject, "TipsAward_Anim_In_2");
                            Text text4 = obj7.transform.FindChild("Tips/Text").GetComponent<Text>();
                            if (this.m_selectStateInfo.bWinCnt == this.m_selectMapRes.bWinCnt)
                            {
                                text4.text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips18");
                            }
                            else
                            {
                                text4.text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips19");
                            }
                        }
                    }
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        Transform transform3 = form.transform.FindChild("Root/PanelRight/Btn_award");
                        if (transform3 != null)
                        {
                            transform3.gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void initTirdFormTicket()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(UNION_ENTRY_THIRD_PATH);
            if (form != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    int useableStackCount = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID);
                    CUseable itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID, useableStackCount);
                    GameObject widget = form.GetWidget(6);
                    Text component = widget.transform.Find("lblIconCount").GetComponent<Text>();
                    CUICommonSystem.SetItemCell(form, widget, itemUseable, true, false);
                    component.text = "x" + useableStackCount.ToString();
                    component.gameObject.CustomSetActive(true);
                }
            }
        }

        public bool IsUnionFuncLocked()
        {
            return !Singleton<CFunctionUnlockSys>.GetInstance().FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_REWARDMATCH);
        }

        private void OnBuyPickDialogConfirm(CUIEvent uiEvent, uint count)
        {
            int bCount = (int) count;
            uint tagUInt = uiEvent.m_eventParams.tagUInt;
            int tag = uiEvent.m_eventParams.tag;
            CUseable useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, tagUInt, bCount);
            if (useable != null)
            {
                int num4 = (int) (useable.GetBuyPrice((RES_SHOPBUY_COINTYPE) tag) * bCount);
                enPayType payType = CMallSystem.ResBuyTypeToPayType(tag);
                stUIEventParams confirmEventParams = new stUIEventParams {
                    tag = bCount
                };
                string[] args = new string[] { bCount.ToString(), useable.m_name };
                CMallSystem.TryToPay(enPayPurpose.Buy, Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips5", args), payType, (uint) num4, enUIEventID.Union_Battle_ConfirmBuyItem, ref confirmEventParams, enUIEventID.None, true, true, false);
            }
        }

        private void OnBuyTiketClick(CUIEvent uiEvt)
        {
            CUIEvent uieventPars = new CUIEvent();
            stUIEventParams @params = new stUIEventParams {
                tagUInt = this.m_selectMapRes.dwConsumPayItemID,
                tag = this.m_selectMapRes.bCoinType
            };
            uieventPars.m_srcFormScript = uiEvt.m_srcFormScript;
            uieventPars.m_srcWidget = uiEvt.m_srcWidget;
            uieventPars.m_eventParams = @params;
            BuyPickDialog.Show(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, @params.tagUInt, (RES_SHOPBUY_COINTYPE) @params.tag, 10000f, 0x63, null, null, new BuyPickDialog.OnConfirmBuyCommonDelegate(this.OnBuyPickDialogConfirm), uieventPars);
        }

        private void OnClickExChgReward(CUIEvent uiEvt)
        {
            CUICommonSystem.JumpForm(RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_NOTICE);
        }

        private void OnClickGetReward(CUIEvent uiEvt)
        {
            SendChgMatchStateReq(this.m_selectMapID, 0);
        }

        private void OnClickGetRewardTips(CUIEvent uiEvt)
        {
            CUIAnimatorScript component = uiEvt.m_srcFormScript.GetWidget(9).GetComponent<CUIAnimatorScript>();
            if (uiEvt.m_srcWidget.activeInHierarchy && (component.m_currentAnimatorStateName != "TipsAward_Anim_Out"))
            {
                component.PlayAnimator("TipsAward_Anim_Out");
            }
        }

        private void OnClickLoseTips(CUIEvent uiEvt)
        {
            CUIAnimatorScript component = uiEvt.m_srcFormScript.GetWidget(9).GetComponent<CUIAnimatorScript>();
            if (uiEvt.m_srcWidget.activeInHierarchy && (component.m_currentAnimatorStateName != "TipsLose_Anim_Out"))
            {
                component.PlayAnimator("TipsLose_Anim_Out");
            }
        }

        private void OnClickRewardIntro(CUIEvent uiEvt)
        {
            Singleton<CUIManager>.GetInstance().OpenForm(UNION_ENTRY_REWARDINTRO_PATH, false, true);
            this.initRewardIntroFormWidget();
        }

        private void OnClickRule(CUIEvent uiEvt)
        {
            int unionBattleRuleId = this.m_unionBattleRuleId;
            Singleton<CUIManager>.GetInstance().OpenInfoForm(unionBattleRuleId);
        }

        private void OnClickSecondUIBattleEntry(CUIEvent uiEvt)
        {
            this.m_selectMapID = uiEvt.m_eventParams.tagUInt;
            this.m_selectMapRes = GetUnionBattleMapInfoByMapID(this.m_selectMapID);
            this.m_selectStateInfo = this.GetMapStateInfo(this.m_selectMapID);
            GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey(5, this.m_selectMapID), out this.m_selectTimeInfo);
            switch (uiEvt.m_eventParams.tag)
            {
                case 0:
                case 1:
                    if (uiEvt.m_eventParams.commonBool)
                    {
                        if (this.m_selectStateInfo == null)
                        {
                            return;
                        }
                        if (this.m_selectStateInfo.bState == 0)
                        {
                            Singleton<CUIManager>.GetInstance().OpenForm(UNION_CONFIRM_ENTRY_PATH, false, true);
                            this.initConfirmFormWidget();
                        }
                        else
                        {
                            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(UNION_ENTRY_THIRD_PATH, false, true);
                            this.initThirdFormWidget();
                        }
                        break;
                    }
                    Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips15", true, 1.5f, null, new object[0]);
                    return;

                case 2:
                    Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips3", true, 1.5f, null, new object[0]);
                    break;
            }
        }

        private void OnClickStartMatch(CUIEvent uiEvt)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                stMatchOpenInfo matchOpenState = CUICommonSystem.GetMatchOpenState(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, this.m_selectMapID);
                if ((matchOpenState.matchState == enMatchOpenState.enMatchOpen_InActiveTime) && (this.m_selectStateInfo.bMatchCnt < this.maxMatchRoundCnt))
                {
                    int num = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID) + useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumFreeItemID);
                    int dwCousumItemNum = (int) this.m_selectMapRes.dwCousumItemNum;
                    if (num >= dwCousumItemNum)
                    {
                        int num3 = 10;
                        if ((this.m_selectStateInfo.bMatchCnt < num3) || (this.m_selectStateInfo.dwClearTime < Utility.GetGlobalRefreshTimeSeconds()))
                        {
                            SendChgMatchStateReq(this.m_selectMapID, 1);
                        }
                    }
                    else
                    {
                        int bCount = dwCousumItemNum - num;
                        CUseable useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_selectMapRes.dwConsumPayItemID, bCount);
                        if (useable != null)
                        {
                            int num5 = (int) (useable.GetBuyPrice((RES_SHOPBUY_COINTYPE) this.m_selectMapRes.bCoinType) * bCount);
                            enPayType payType = CMallSystem.ResBuyTypeToPayType(this.m_selectMapRes.bCoinType);
                            stUIEventParams confirmEventParams = new stUIEventParams {
                                tag = bCount
                            };
                            string[] args = new string[] { bCount.ToString(), useable.m_name };
                            CMallSystem.TryToPay(enPayPurpose.Buy, Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips5", args), payType, (uint) num5, enUIEventID.Union_Battle_ConfirmBuyItem, ref confirmEventParams, enUIEventID.None, true, true, false);
                        }
                    }
                }
                else if (matchOpenState.matchState != enMatchOpenState.enMatchOpen_InActiveTime)
                {
                    Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips4", true, 1.5f, null, new object[0]);
                }
                else if (this.m_selectStateInfo.bMatchCnt >= this.maxMatchRoundCnt)
                {
                    Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips17", true, 1.5f, null, new object[0]);
                }
            }
        }

        private void OnClickStartSingleMatch(CUIEvent uiEvt)
        {
            this.SendBeginMatchReq();
        }

        private void OnConfirmBuyItem(CUIEvent uiEvt)
        {
            SendBuyTicketRequest(this.m_selectMapID, (uint) uiEvt.m_eventParams.tag);
        }

        private void OnGotReward(CUIEvent uiEvt)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(UNION_ENTRY_THIRD_PATH);
        }

        private void OnRewardElementEnable(CUIEvent uiEvt)
        {
            this.refreshOneRewardElement(uiEvt.m_srcFormScript, uiEvt.m_srcWidget, uiEvt.m_srcWidgetIndexInBelongedList);
        }

        private void OnRewardMatchTimeUp(CUIEvent uiEvt)
        {
            this.initSecondFormWidget();
        }

        private void Open_BattleEntry(CUIEvent uiEvt)
        {
            if (Singleton<CMatchingSystem>.instance.IsInMatching)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
            }
            else if (this.IsUnionFuncLocked())
            {
                Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips2", true, 1.5f, null, new object[0]);
            }
            else
            {
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(UNION_ENTRY_PATH, false, true);
                this.initFirstFormWidget();
                SendGetUnionBattleBaseInfoReq();
                Singleton<CMiShuSystem>.instance.SetNewFlagForUnionBattleEntry(false);
            }
        }

        private void Open_SecondBattleEntry(CUIEvent uiEvt)
        {
            switch (uiEvt.m_eventParams.tag)
            {
                case 0:
                {
                    CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(UNION_ENTRY_SECOND_PATH, false, true);
                    SendGetUnionBattleStateReq();
                    this.initSecondFormWidget();
                    Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(4, null);
                    break;
                }
                case 1:
                    Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips3", true, 1.5f, null, new object[0]);
                    break;
            }
        }

        [MessageHandler(0x13ee)]
        public static void ReciveAwardPoolInfo(CSPkg msg)
        {
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
            Singleton<CUnionBattleEntrySystem>.instance.m_awardPoolInfo = msg.stPkgData.stGetAwardPoolRsp;
        }

        [MessageHandler(0x13f1)]
        public static void ReciveBuyTicketInfo(CSPkg msg)
        {
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
            if (msg.stPkgData.stBuyMatchTicketRsp.iResult == 0)
            {
                Singleton<CUnionBattleEntrySystem>.instance.initConfirmFormWidget();
                Singleton<CUnionBattleEntrySystem>.instance.initTirdFormTicket();
                Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips14", true, 1.5f, null, new object[0]);
            }
            else if (msg.stPkgData.stBuyMatchTicketRsp.iResult == 1)
            {
                Singleton<CUIManager>.instance.OpenTips("Union_Battle_Tips13", true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x13ef)]
        public static void RecivePersonInfo(CSPkg msg)
        {
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
            Singleton<CUnionBattleEntrySystem>.instance.m_personInfo = msg.stPkgData.stMatchPointNtf;
        }

        [MessageHandler(0x13f3)]
        public static void ReciveUnionBattleBaseInfo(CSPkg msg)
        {
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
            Singleton<CUnionBattleEntrySystem>.instance.m_baseInfo = msg.stPkgData.stGetMatchInfoRsp;
            Singleton<CUnionBattleEntrySystem>.instance.initFirstFormWidget();
        }

        [MessageHandler(0x13f5)]
        public static void ReciveUnionBattleStateRsp(CSPkg msg)
        {
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
            Singleton<CUnionBattleEntrySystem>.instance.m_stateInfo = msg.stPkgData.stGetRewardMatchInfoRsp.stRewardMatchInfo;
        }

        private void refreshOneRewardElement(CUIFormScript form, GameObject element, int index)
        {
            if ((element != null) && (form != null))
            {
                int num = index - 1;
                element.transform.FindChild("PanelReward").gameObject.CustomSetActive(index != 0);
                element.transform.FindChild("PanelTitle").gameObject.CustomSetActive(index == 0);
                if (index != 0)
                {
                    string[] args = new string[] { num.ToString() };
                    element.transform.FindChild("PanelReward/Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips16", args);
                    CUIListScript component = element.transform.FindChild("PanelReward/IconContainer").GetComponent<CUIListScript>();
                    GameObject gameObject = element.transform.FindChild("PanelReward/itemBaoXiang").gameObject;
                    ResRewardMatchReward dataByKey = GameDataMgr.unionBattleWinCntRewardDatabin.GetDataByKey(GameDataMgr.GetDoubleKey(this.m_selectMapID, (uint) num));
                    if ((dataByKey == null) || (dataByKey.astRewardItem[0].bRewardType == 0))
                    {
                        component.SetElementAmount(0);
                        gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        ResRewardInfo info = dataByKey.astRewardItem[0];
                        CUseable itemUseable = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE) info.bRewardType, (int) info.dwRewardNum, info.dwRewardID);
                        CUICommonSystem.SetItemCell(form, gameObject, itemUseable, true, false);
                        gameObject.CustomSetActive(true);
                        ResPropInfo info2 = GameDataMgr.itemDatabin.GetDataByKey(info.dwRewardID);
                        if (info2 != null)
                        {
                            ResRandomRewardStore store = GameDataMgr.randomRewardDB.GetDataByKey((long) ((int) info2.EftParam[0]));
                            if (store != null)
                            {
                                ListView<CUseable> view = new ListView<CUseable>();
                                for (int i = 0; i < store.astRewardDetail.Length; i++)
                                {
                                    if (store.astRewardDetail[i].bItemType == 0)
                                    {
                                        break;
                                    }
                                    ResDT_RandomRewardInfo info3 = store.astRewardDetail[i];
                                    view.Add(CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) info3.bItemType, (int) info3.dwLowCnt, info3.dwItemID));
                                }
                                component.SetElementAmount(view.Count);
                                int count = view.Count;
                                for (int j = 0; j < count; j++)
                                {
                                    CUIListElementScript elemenet = component.GetElemenet(j);
                                    CUICommonSystem.SetItemCell(form, elemenet.gameObject, view[j], true, false);
                                    Text text2 = elemenet.transform.FindChild("lblIconCount").GetComponent<Text>();
                                    ResDT_RandomRewardInfo info4 = store.astRewardDetail[j];
                                    if (info4.dwLowCnt != info4.dwHighCnt)
                                    {
                                        text2.text = string.Format("{0}~{1}", info4.dwLowCnt, info4.dwHighCnt);
                                    }
                                    if ((j + 1) >= count)
                                    {
                                        elemenet.gameObject.transform.FindChild("Add").gameObject.CustomSetActive(false);
                                    }
                                    else
                                    {
                                        elemenet.gameObject.transform.FindChild("Add").gameObject.CustomSetActive(true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void SendAwartPoolReq(uint mapID)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x13ed);
            msg.stPkgData.stGetAwardPoolReq.dwMapId = mapID;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void SendBeginMatchReq()
        {
            CMatchingSystem.ReqStartSingleMatching(this.m_selectMapID, false, COM_BATTLE_MAP_TYPE.COM_BATTLE_MAP_TYPE_REWARDMATCH);
        }

        public static void SendBuyTicketRequest(uint mapID, uint itemNum)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x13f0);
            msg.stPkgData.stBuyMatchTicketReq.dwMapId = mapID;
            msg.stPkgData.stBuyMatchTicketReq.dwNum = itemNum;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendChgMatchStateReq(uint mapId, byte bState)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x13f6);
            msg.stPkgData.stRewardMatchStateChgReq.dwMapId = mapId;
            msg.stPkgData.stRewardMatchStateChgReq.bState = bState;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendGetUnionBattleBaseInfoReq()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x13f2);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendGetUnionBattleStateReq()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x13f4);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SetWinLoseCntChgInfo(uint dwMapId, enLastWinLoseCntChgType type)
        {
            stLastWinLoseCntChginfo[] winLoseChgInfo = Singleton<CUnionBattleEntrySystem>.instance.m_WinLoseChgInfo;
            for (int i = 0; i < winLoseChgInfo.Length; i++)
            {
                if (winLoseChgInfo[i].mapId == dwMapId)
                {
                    winLoseChgInfo[i].winLoseChgType = type;
                    break;
                }
            }
        }

        private void ShowCountDownTime(GameObject Btn, bool bMapOpened)
        {
            if (Btn != null)
            {
                Transform transform = Btn.transform;
                uint tagUInt = Btn.GetComponent<CUIMiniEventScript>().m_onClickEventParams.tagUInt;
                Transform transform2 = transform.FindChild("Desc");
                if (transform2 != null)
                {
                    GameObject gameObject = transform2.FindChild("Text").gameObject;
                    GameObject obj3 = transform2.FindChild("Timer").gameObject;
                    if (!bMapOpened)
                    {
                        obj3.CustomSetActive(false);
                        gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        int utilOpenDay = 0;
                        uint utilOpenSec = 0;
                        CUICommonSystem.GetTimeUtilOpen(RES_BATTLE_MAP_TYPE.RES_BATTLE_MAP_TYPE_REWARDMATCH, tagUInt, out utilOpenSec, out utilOpenDay);
                        if ((utilOpenDay == 0) && (utilOpenSec == 0))
                        {
                            obj3.CustomSetActive(false);
                            gameObject.CustomSetActive(true);
                            gameObject.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips6");
                        }
                        else if (utilOpenSec < SECOND_ONE_DAY)
                        {
                            gameObject.CustomSetActive(false);
                            obj3.CustomSetActive(true);
                            CUITimerScript component = obj3.transform.FindChild("Text").GetComponent<CUITimerScript>();
                            component.SetTotalTime((float) utilOpenSec);
                            component.StartTimer();
                        }
                        else
                        {
                            int num4 = utilOpenDay;
                            gameObject.CustomSetActive(true);
                            obj3.CustomSetActive(false);
                            string text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips7");
                            gameObject.GetComponent<Text>().text = string.Format(text, num4);
                        }
                    }
                }
            }
        }

        private void ShowReward(COMDT_REWARDMATCH_RECORD matchState)
        {
            ResRewardMatchReward dataByKey = GameDataMgr.unionBattleWinCntRewardDatabin.GetDataByKey(GameDataMgr.GetDoubleKey(this.m_selectStateInfo.dwMapId, this.m_selectStateInfo.bWinCnt));
            ListView<CUseable> inList = new ListView<CUseable>();
            for (int i = 0; i < dataByKey.astRewardItem.Length; i++)
            {
                if (dataByKey.astRewardItem[i].bRewardType == 0)
                {
                    break;
                }
                if ((dataByKey.astRewardItem[i].bRewardType != 20) || ((Singleton<HeadIconSys>.GetInstance().GetInfo(dataByKey.astRewardItem[i].dwRewardID) == null) && !CSysDynamicBlock.bSocialBlocked))
                {
                    ResRewardInfo info = dataByKey.astRewardItem[i];
                    inList.Add(CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE) info.bRewardType, (int) info.dwRewardNum, info.dwRewardID));
                }
            }
            string text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips8");
            Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(inList), text, true, enUIEventID.Union_Battle_GotReward, false, true, "Form_AwardGold");
        }

        private void StartARoundRewardMatch()
        {
            Singleton<CUIManager>.GetInstance().OpenForm(UNION_ENTRY_THIRD_PATH, false, true);
            this.initThirdFormWidget();
            Singleton<CUIManager>.GetInstance().CloseForm(UNION_CONFIRM_ENTRY_PATH);
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_ClickEntry, new CUIEventManager.OnUIEventHandler(this.Open_BattleEntry));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_BattleEntryGroup_Click, new CUIEventManager.OnUIEventHandler(this.Open_SecondBattleEntry));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_SubBattleEntryGroup_Click, new CUIEventManager.OnUIEventHandler(this.OnClickSecondUIBattleEntry));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_SingleStartMatch, new CUIEventManager.OnUIEventHandler(this.OnClickStartSingleMatch));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_ConfirmBuyItem, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyItem));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_BuyTiketClick, new CUIEventManager.OnUIEventHandler(this.OnBuyTiketClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_Rule, new CUIEventManager.OnUIEventHandler(this.OnClickRule));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_RewardMatch_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnRewardMatchTimeUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_battle_Click_StartOneMatchRound, new CUIEventManager.OnUIEventHandler(this.OnClickStartMatch));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_RewardIntro, new CUIEventManager.OnUIEventHandler(this.OnClickRewardIntro));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_GetReward, new CUIEventManager.OnUIEventHandler(this.OnClickGetReward));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_GotReward, new CUIEventManager.OnUIEventHandler(this.OnGotReward));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_RewardIntro_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnRewardElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_ExChgRewwad, new CUIEventManager.OnUIEventHandler(this.OnClickExChgReward));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_CliCk_LoseTips, new CUIEventManager.OnUIEventHandler(this.OnClickLoseTips));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Union_Battle_Click_GetRewardTips, new CUIEventManager.OnUIEventHandler(this.OnClickGetRewardTips));
            this.m_awardPoolInfo = null;
            this.m_personInfo = null;
            this.m_baseInfo = null;
            this.m_stateInfo = null;
            this.m_WinLoseChgInfo = null;
        }

        [MessageHandler(0x13f7)]
        public static void UnionBattleStateChg(CSPkg msg)
        {
            COMDT_REWARDMATCH_RECORD stChgInfo = msg.stPkgData.stRewardMatchInfoChgNtf.stChgInfo;
            COMDT_REWARDMATCH_DATA stateInfo = Singleton<CUnionBattleEntrySystem>.instance.m_stateInfo;
            for (int i = 0; i < stateInfo.bRecordCnt; i++)
            {
                if (stateInfo.astRecord[i].dwMapId == stChgInfo.dwMapId)
                {
                    if ((stateInfo.astRecord[i].bState == 0) && (stChgInfo.bState == 1))
                    {
                        Singleton<CUnionBattleEntrySystem>.instance.StartARoundRewardMatch();
                    }
                    else if ((stateInfo.astRecord[i].bState == 2) && (stChgInfo.bState == 0))
                    {
                        Singleton<CUnionBattleEntrySystem>.instance.ShowReward(stChgInfo);
                    }
                    if (stateInfo.astRecord[i].bWinCnt != stChgInfo.bWinCnt)
                    {
                        SetWinLoseCntChgInfo(stChgInfo.dwMapId, enLastWinLoseCntChgType.enChgType_Win);
                    }
                    else if (stateInfo.astRecord[i].bLossCnt != stChgInfo.bLossCnt)
                    {
                        SetWinLoseCntChgInfo(stChgInfo.dwMapId, enLastWinLoseCntChgType.enChgType_Lose);
                    }
                    stateInfo.astRecord[i] = stChgInfo;
                    break;
                }
            }
        }

        public enum enLastWinLoseCntChgType
        {
            enChgType_None,
            enChgType_Win,
            enChgType_Lose
        }

        private enum enUnionBattleEntryWidget
        {
            enEntry_Btn1,
            enEntry_Btn2,
            enEntry_PlayerCount
        }

        private enum enUnionConfirmEntryWidget
        {
            enIntroTxt,
            enOpenTimesTxt,
            enBtnStart,
            enTicketItem
        }

        private enum enUnionRewardIntroWidget
        {
            enEntry_RewardItems
        }

        private enum enUnionSecBattleEntryWidget
        {
            enEntry_Btn1,
            enEntry_Btn2,
            enEntry_Btn3
        }

        private enum enUnionThirdEntryWidget
        {
            enEntry_WinCountTxt,
            enEntry_OpenTimeTxt,
            enEntry_PanelLoseTimes,
            enEntry_StartFightBtn,
            enEntry_GetAwardBtn,
            enEntry_HeadItemCell,
            enEntry_TicketItemCell,
            enEntry_MapName,
            enEntry_MatchName,
            enEntry_PanelRight,
            enEntry_LoseTwoTimesTips
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stLastWinLoseCntChginfo
        {
            public uint mapId;
            public CUnionBattleEntrySystem.enLastWinLoseCntChgType winLoseChgType;
        }
    }
}

