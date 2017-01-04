namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class SettlementSystem : Singleton<SettlementSystem>
    {
        private readonly string _achievementsTips = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_SettleAchievement");
        private GameObject _cacheLastReportGo;
        private uint _camp1TotalDamage;
        private uint _camp1TotalKill;
        private uint _camp1TotalTakenDamage;
        private uint _camp1TotalToHeroDamage;
        private uint _camp2TotalDamage;
        private uint _camp2TotalKill;
        private uint _camp2TotalTakenDamage;
        private uint _camp2TotalToHeroDamage;
        private bool _changingGrage;
        private float _coinFrom;
        private LTDescr _coinLtd;
        private float _coinTo;
        private Text _coinTweenText;
        private uint _curDian = 1;
        private uint _curGrade = 1;
        private int _curLeftIndex;
        private uint _curMaxScore = 3;
        private CUIAnimationScript _currentShinningAni;
        private int _curRightIndex;
        private bool _doWangZheSpecial;
        private string _duration;
        private float _expFrom;
        private LTDescr _expLtd;
        private float _expTo;
        private RectTransform _expTweenRect;
        private bool _isDown;
        private bool _isLadderMatch;
        private bool _isSettlementContinue;
        private bool _isUp;
        private Animator _ladderAnimator;
        private CUIFormScript _ladderForm;
        private readonly string _ladderFormName = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_LadderSettle");
        private GameObject _ladderRoot;
        private bool _lastLadderWin;
        private CUIListScript _leftListScript;
        private static uint _lvUpGrade = 0;
        private COM_PLAYERCAMP _myCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
        private bool _neutral;
        private uint _newGrade = 1;
        private uint _newMaxScore = 3;
        private uint _newScore = 1;
        private uint _oldGrade = 1;
        private uint _oldMaxScore = 3;
        private uint _oldScore = 1;
        public readonly string _profitFormName = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_PvpNewProfit.prefab");
        private CUIFormScript _profitFormScript;
        private ulong _reportUid;
        private int _reportWordId;
        private CUIListScript _rightListScript;
        private CUIFormScript _settleFormScript;
        private string _startTime;
        private uint _startTimeInt;
        private bool _win;
        private const string ColorStarGameObjectSubPath = "greyStar/colorStar";
        private const float ExpBarWidth = 327.6f;
        private bool m_bBackShowTimeLine;
        private bool m_bGrade;
        private bool m_bIsDetail = true;
        private bool m_bLastAddFriendBtnState;
        private bool m_bLastDataBtnState;
        private bool m_bLastOverViewBtnState;
        private bool m_bLastReprotBtnState;
        private bool m_bShareDataSucc;
        private bool m_bShareOverView;
        private Transform m_BtnTimeLine;
        private Transform m_PVPBtnGroup;
        private Transform m_PVPShareBtnClose;
        private Transform m_PVPShareDataBtnGroup;
        private Transform m_PVPSwitchOverview;
        private Transform m_PVPSwitchStatistics;
        private Transform m_PVPSwtichAddFriend;
        private GameObject m_ShareDataBtn;
        private PvpAchievementForm m_sharePVPAchieventForm;
        private Text m_timeLineText;
        private Text m_TxtBtnShareCaption;
        private CUIFormScript m_UpdateGradeForm;
        private const int MaxAchievement = 8;
        private int playerNum;
        private static string PlayerWinTimesStr = "PlayerWinTimes";
        private const float ProficientBarWidth = 205f;
        public readonly string SettlementFormName = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_PvpNewSettlement.prefab");
        public const string SHARE_UPDATE_GRADE_FORM = "UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab";
        private static readonly string[] StrHelper = new string[9];
        private const int StrHelperLength = 9;
        private const float TweenTime = 2f;

        private void ChangeSharePVPDataBtnState(bool bShowShare)
        {
            if (this.m_PVPBtnGroup != null)
            {
                this.m_PVPBtnGroup.gameObject.SetActive(!bShowShare);
            }
            if (this.m_PVPSwtichAddFriend != null)
            {
                if (bShowShare)
                {
                    this.m_bLastAddFriendBtnState = this.m_PVPSwtichAddFriend.gameObject.activeSelf;
                    this.m_PVPSwtichAddFriend.gameObject.SetActive(false);
                }
                else
                {
                    this.m_PVPSwtichAddFriend.gameObject.SetActive(this.m_bLastAddFriendBtnState);
                }
            }
            if (this.m_PVPSwitchStatistics != null)
            {
                if (bShowShare)
                {
                    this.m_bLastDataBtnState = this.m_PVPSwitchStatistics.gameObject.activeSelf;
                    this.m_PVPSwitchStatistics.gameObject.SetActive(false);
                }
                else
                {
                    this.m_PVPSwitchStatistics.gameObject.SetActive(this.m_bLastDataBtnState);
                }
            }
            if (this._settleFormScript != null)
            {
                this._settleFormScript.m_formWidgets[9].CustomSetActive(!bShowShare);
                this._settleFormScript.m_formWidgets[10].CustomSetActive(!bShowShare);
                this._settleFormScript.m_formWidgets[0x19].CustomSetActive(!bShowShare);
            }
            if (this.m_PVPSwitchOverview != null)
            {
                if (bShowShare)
                {
                    this.m_bLastOverViewBtnState = this.m_PVPSwitchOverview.gameObject.activeSelf;
                    this.m_PVPSwitchOverview.gameObject.SetActive(false);
                }
                else
                {
                    this.m_PVPSwitchOverview.gameObject.SetActive(this.m_bLastOverViewBtnState);
                }
            }
            if (this.m_bIsDetail)
            {
                if (this.m_bShareDataSucc)
                {
                    this.UpdateTimeBtnState(false);
                }
                else
                {
                    this.UpdateTimeBtnState(true);
                }
            }
            else if (this.m_bShareOverView)
            {
                this.UpdateTimeBtnState(false);
            }
            else
            {
                this.UpdateTimeBtnState(true);
            }
            if (this.m_PVPShareDataBtnGroup != null)
            {
                this.m_PVPShareDataBtnGroup.gameObject.SetActive(bShowShare);
            }
            if (this.m_PVPShareBtnClose != null)
            {
                this.m_PVPShareBtnClose.gameObject.SetActive(bShowShare);
            }
        }

        private void CheckPVPAchievement()
        {
            this.m_sharePVPAchieventForm.Init(this._win);
            if (this._win && this.m_sharePVPAchieventForm.CheckAchievement())
            {
                this.m_sharePVPAchieventForm.ShowVictory();
            }
            else
            {
                this.ShowSettlementPanel(false);
            }
        }

        private void ClearShareData()
        {
            this.m_bLastAddFriendBtnState = false;
            this.m_bLastReprotBtnState = false;
            this.m_bLastOverViewBtnState = false;
            this.m_bLastDataBtnState = false;
            this.m_bShareDataSucc = false;
            this.m_bShareOverView = false;
            this.m_bIsDetail = true;
            this.m_bBackShowTimeLine = false;
            this.m_PVPBtnGroup = null;
            this.m_PVPSwtichAddFriend = null;
            this.m_PVPSwitchStatistics = null;
            this.m_PVPSwitchOverview = null;
            this.m_PVPShareDataBtnGroup = null;
            this.m_PVPShareBtnClose = null;
            this.m_timeLineText = null;
            this.m_BtnTimeLine = null;
            this.m_TxtBtnShareCaption = null;
            this.m_ShareDataBtn = null;
        }

        public void ClosePersonalProfit()
        {
            this.DoCoinTweenEnd();
            this.DoExpTweenEnd();
            this._profitFormScript = null;
            Singleton<CUIManager>.GetInstance().CloseForm(this._profitFormName);
        }

        private void CloseSettlementPanel()
        {
            this._settleFormScript = null;
            Singleton<CUIManager>.GetInstance().CloseForm(this.SettlementFormName);
            Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
            if (!PlayerPrefs.HasKey("NET_ACC_RECOMMENDED") && NetworkAccelerator.getAccelRecommendation())
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX"), enUIEventID.NetworkAccelerator_TurnOn, enUIEventID.NetworkAccelerator_Ignore, Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX_OK"), Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_RECOMMENDED_MSGBOX_CANCEL"), false);
                PlayerPrefs.SetString("NET_ACC_RECOMMENDED", "Y");
            }
            MonoSingleton<ShareSys>.instance.m_bShowTimeline = false;
        }

        private void CollectPlayerKda(PlayerKDA kda)
        {
            ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
            while (enumerator.MoveNext())
            {
                HeroKDA current = enumerator.Current;
                if (current == null)
                {
                    continue;
                }
                COM_PLAYERCAMP playerCamp = kda.PlayerCamp;
                if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                {
                    this._camp1TotalKill += (uint) current.numKill;
                    this._camp1TotalDamage += (uint) current.hurtToEnemy;
                    this._camp1TotalTakenDamage += (uint) current.hurtTakenByEnemy;
                    this._camp1TotalToHeroDamage += (uint) current.hurtToHero;
                }
                else if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                {
                    goto Label_008A;
                }
                break;
            Label_008A:
                this._camp2TotalKill += (uint) current.numKill;
                this._camp2TotalDamage += (uint) current.hurtToEnemy;
                this._camp2TotalTakenDamage += (uint) current.hurtTakenByEnemy;
                this._camp2TotalToHeroDamage += (uint) current.hurtToHero;
                break;
            }
        }

        public void DianXing()
        {
            if (this.NeedDianXing())
            {
                uint num = this.NeedChangeGrade();
                if ((num > 0) && !this._changingGrage)
                {
                    this._changingGrage = true;
                    this._curMaxScore = num;
                    if (this._isUp)
                    {
                        this._curGrade++;
                        this._curDian = 0;
                    }
                    else
                    {
                        this._curGrade--;
                        this._curDian = this._curMaxScore;
                    }
                    if (this._isUp)
                    {
                        this.Ladder_PlayLevelUpStart();
                    }
                    else
                    {
                        this.Ladder_PlayLevelDownStart();
                    }
                }
                else if (!this._changingGrage)
                {
                    if (this._isUp)
                    {
                        this._curDian++;
                        this.PlayXingAnim(this._curDian, this._curMaxScore, false);
                    }
                    else
                    {
                        this.PlayXingAnim(this._curDian, this._curMaxScore, this._isDown);
                    }
                }
            }
            else if (((!this._doWangZheSpecial && (this._oldGrade == this._newGrade)) && ((this._newGrade == GameDataMgr.rankGradeDatabin.count) && (this._oldScore == this._newScore))) && (this._newScore == 0))
            {
                this._doWangZheSpecial = true;
                this.PlayXingAnim(this._curDian, this._curMaxScore, this._isDown);
            }
            else
            {
                this._doWangZheSpecial = false;
                this.LadderAllDisplayEnd();
            }
        }

        private void DoCoinAndExpTween()
        {
            try
            {
                if ((this._coinTweenText != null) && (this._coinTweenText.gameObject != null))
                {
                    this._coinLtd = LeanTween.value(this._coinTweenText.gameObject, delegate (float value) {
                        if ((this._coinTweenText != null) && (this._coinTweenText.gameObject != null))
                        {
                            this._coinTweenText.text = string.Format("+{0}", value.ToString("N0"));
                            if (value >= this._coinTo)
                            {
                                this.DoCoinTweenEnd();
                            }
                        }
                    }, this._coinFrom, this._coinTo, 2f);
                }
                if ((this._expTweenRect != null) && (this._expTweenRect.gameObject != null))
                {
                    this._expLtd = LeanTween.value(this._expTweenRect.gameObject, delegate (float value) {
                        if ((this._expTweenRect != null) && (this._expTweenRect.gameObject != null))
                        {
                            this._expTweenRect.sizeDelta = new Vector2(value * 327.6f, this._expTweenRect.sizeDelta.y);
                            if (value >= this._expTo)
                            {
                                this.DoExpTweenEnd();
                            }
                        }
                    }, this._expFrom, this._expTo, 2f);
                }
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message };
                DebugHelper.Assert(false, "Exceptin in DoCoinAndExpTween, {0}", inParameters);
            }
        }

        public void DoCoinTweenEnd()
        {
            if ((this._coinLtd != null) && (this._coinTweenText != null))
            {
                this._coinTweenText.text = string.Format("+{0}", this._coinTo.ToString("N0"));
                if (Singleton<BattleStatistic>.GetInstance().multiDetail != null)
                {
                    CUICommonSystem.AppendMultipleText(this._coinTweenText, CUseable.GetMultiple(ref Singleton<BattleStatistic>.GetInstance().multiDetail, 0, -1));
                }
                this._coinLtd.cancel();
                this._coinLtd = null;
                this._coinTweenText = null;
            }
        }

        private void DoExpTweenEnd()
        {
            if ((this._expTweenRect != null) && (this._expLtd != null))
            {
                this._expTweenRect.sizeDelta = new Vector2(this._expTo * 327.6f, this._expTweenRect.sizeDelta.y);
                this._expLtd.cancel();
                this._expLtd = null;
                this._expTweenRect = null;
            }
            if (_lvUpGrade > 1)
            {
                CUIEvent event3 = new CUIEvent {
                    m_eventID = enUIEventID.Settle_OpenLvlUp
                };
                event3.m_eventParams.tag = ((int) _lvUpGrade) - 1;
                event3.m_eventParams.tag2 = (int) _lvUpGrade;
                CUIEvent uiEvent = event3;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
            }
            _lvUpGrade = 0;
        }

        private static COMDT_SETTLE_HERO_RESULT_INFO GetHeroSettleInfo(uint heroId)
        {
            COMDT_SETTLE_HERO_RESULT_DETAIL heroSettleInfo = Singleton<BattleStatistic>.GetInstance().heroSettleInfo;
            if (heroSettleInfo != null)
            {
                for (int i = 0; i < heroSettleInfo.bNum; i++)
                {
                    if ((heroSettleInfo.astHeroList[i] != null) && (heroSettleInfo.astHeroList[i].dwHeroConfID == heroId))
                    {
                        return heroSettleInfo.astHeroList[i];
                    }
                }
            }
            return null;
        }

        private static string GetProficiencyLvTxt(int heroType, uint level)
        {
            ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(heroType, (int) level);
            return ((heroProficiency == null) ? string.Empty : Utility.UTF8Convert(heroProficiency.szTitle));
        }

        private GameObject GetXing(uint targetScore, uint targetMax)
        {
            if (this._ladderRoot == null)
            {
                return null;
            }
            Transform transform = this._ladderRoot.transform.FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", targetMax));
            if (transform == null)
            {
                return null;
            }
            GameObject gameObject = transform.gameObject;
            if (gameObject == null)
            {
                return null;
            }
            Transform transform2 = gameObject.transform.FindChild(string.Format("Xing{0}", targetScore));
            return ((transform2 == null) ? null : transform2.gameObject);
        }

        private void ImpAddFriend(CUIEvent uiEvent)
        {
            Singleton<CFriendContoller>.instance.Open_Friend_Verify(uiEvent.m_eventParams.commonUInt64Param1, (uint) uiEvent.m_eventParams.commonUInt64Param2, false);
            uiEvent.m_srcWidget.CustomSetActive(false);
        }

        protected void ImpCloseReport(CUIEvent uiEvent)
        {
            if ((this._settleFormScript != null) && (this._settleFormScript.gameObject != null))
            {
                this._cacheLastReportGo = null;
                this._reportUid = 0L;
                this._reportWordId = 0;
                this._settleFormScript.m_formWidgets[3].CustomSetActive(false);
            }
        }

        protected void ImpDoReport(CUIEvent uiEvent)
        {
            if ((this._settleFormScript != null) && (this._settleFormScript.gameObject != null))
            {
                GameObject obj2 = this._settleFormScript.m_formWidgets[3];
                obj2.CustomSetActive(false);
                Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Report_Report"), false, 1.5f, null, new object[0]);
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x10d1);
                msg.stPkgData.stUserComplaintReq.dwComplaintReason = 1;
                msg.stPkgData.stUserComplaintReq.ullComplaintUserUid = this._reportUid;
                msg.stPkgData.stUserComplaintReq.iComplaintLogicWorldID = this._reportWordId;
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetEnumerator();
                int num = 0;
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    PlayerKDA rkda = current.Value;
                    if (((rkda != null) && (rkda.PlayerUid == this._reportUid)) && (rkda.WorldId == this._reportWordId))
                    {
                        if (!string.IsNullOrEmpty(rkda.PlayerOpenId))
                        {
                            Utility.StringToByteArray(rkda.PlayerOpenId, ref msg.stPkgData.stUserComplaintReq.szComplaintUserOpenId);
                        }
                        byte[] sourceArray = Utility.BytesConvert(rkda.PlayerName);
                        byte[] szComplaintPlayerName = msg.stPkgData.stUserComplaintReq.szComplaintPlayerName;
                        Array.Copy(sourceArray, szComplaintPlayerName, Math.Min(sourceArray.Length, szComplaintPlayerName.Length));
                        szComplaintPlayerName[szComplaintPlayerName.Length - 1] = 0;
                        msg.stPkgData.stUserComplaintReq.iComplaintPlayerCamp = (rkda.PlayerCamp != this._myCamp) ? 2 : 1;
                    }
                    num++;
                }
                GameObject gameObject = obj2.transform.FindChild("ReportToggle").gameObject;
                if (gameObject.transform.FindChild("ReportGuaJi").gameObject.GetComponent<Toggle>().isOn)
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 1;
                }
                else if (gameObject.transform.FindChild("ReportSong").gameObject.GetComponent<Toggle>().isOn)
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 2;
                }
                else if (gameObject.transform.FindChild("ReportXiaoJi").gameObject.GetComponent<Toggle>().isOn)
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 3;
                }
                else if (gameObject.transform.FindChild("ReportMaRen").gameObject.GetComponent<Toggle>().isOn)
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 4;
                }
                else if (gameObject.transform.FindChild("ReportYanYuan").gameObject.GetComponent<Toggle>().isOn)
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 5;
                }
                else if (gameObject.transform.FindChild("ReportGua").gameObject.GetComponent<Toggle>().isOn)
                {
                    msg.stPkgData.stUserComplaintReq.dwComplaintReason = 6;
                }
                Utility.StringToByteArray(CUIUtility.RemoveEmoji(gameObject.transform.FindChild("InputField").gameObject.GetComponent<InputField>().text), ref msg.stPkgData.stUserComplaintReq.szComplaintRemark);
                msg.stPkgData.stUserComplaintReq.dwClientStartTime = this._startTimeInt;
                msg.stPkgData.stUserComplaintReq.iBattlePlayerNumber = num;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                this._reportUid = 0L;
                this._reportWordId = 0;
                if (this._cacheLastReportGo != null)
                {
                    this._cacheLastReportGo.CustomSetActive(false);
                    this._cacheLastReportGo = null;
                }
            }
        }

        private void ImpSettlementTimerEnd()
        {
            Singleton<GameBuilder>.instance.EndGame();
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
            if (this._settleFormScript != null)
            {
                this._settleFormScript.m_formWidgets[2].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[0x10].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[1].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[4].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[5].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[6].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[15].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[11].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[12].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x13].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[20].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[0x11].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x12].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x16].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[0x1b].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[9].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[10].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[0x19].CustomSetActive(!this._neutral);
                uint[] param = new uint[] { !this._win ? 2 : 1, this.playerNum / 2 };
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.PvPShowKDA, param);
                Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Settle, 0L, 0);
                Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Settle);
                Singleton<CChatController>.instance.ShowPanel(true, false);
                Singleton<CChatController>.instance.view.UpView(true);
                Singleton<CChatController>.instance.model.sysData.ClearEntryText();
                GameObject obj2 = this._settleFormScript.m_formWidgets[0x1a];
                if (obj2 != null)
                {
                    Singleton<CRecordUseSDK>.GetInstance().OpenMsgBoxForMomentRecorder(obj2.transform);
                }
            }
        }

        protected void ImpShowReport(CUIEvent uiEvent)
        {
            if ((this._settleFormScript != null) && (this._settleFormScript.gameObject != null))
            {
                GameObject obj2 = this._settleFormScript.m_formWidgets[3];
                obj2.CustomSetActive(true);
                this._cacheLastReportGo = uiEvent.m_srcWidget;
                this._reportUid = uiEvent.m_eventParams.commonUInt64Param1;
                this._reportWordId = (int) uiEvent.m_eventParams.commonUInt64Param2;
                CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
                string playerName = null;
                if (playerKDAStat != null)
                {
                    DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                        if (current.Value.PlayerUid == this._reportUid)
                        {
                            KeyValuePair<uint, PlayerKDA> pair2 = enumerator.Current;
                            if (pair2.Value.WorldId == this._reportWordId)
                            {
                                KeyValuePair<uint, PlayerKDA> pair3 = enumerator.Current;
                                playerName = pair3.Value.PlayerName;
                                break;
                            }
                        }
                    }
                }
                obj2.transform.FindChild("ReportToggle/ReportName").gameObject.GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Report_PlayerName"), playerName);
            }
        }

        private void ImpSwitchAddFriendReportLaHeiDianZan(ShowBtnType btnType)
        {
            if (this._settleFormScript != null)
            {
                Transform transform = this._settleFormScript.m_formWidgets[9].transform.FindChild("light");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive((btnType == ShowBtnType.AddFriend) && !this._neutral);
                }
                transform = this._settleFormScript.m_formWidgets[10].transform.FindChild("light");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive((btnType == ShowBtnType.Report) && !this._neutral);
                }
                transform = this._settleFormScript.m_formWidgets[0x19].transform.FindChild("light");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive((btnType == ShowBtnType.LaHeiDianZan) && !this._neutral);
                }
                if (this._leftListScript != null)
                {
                    int elementAmount = this._leftListScript.GetElementAmount();
                    for (int i = 0; i < elementAmount; i++)
                    {
                        SettlementHelper component = this._leftListScript.GetElemenet(i).gameObject.GetComponent<SettlementHelper>();
                        component.AddFriendRoot.CustomSetActive((btnType == ShowBtnType.AddFriend) && !this._neutral);
                        component.ReportRoot.CustomSetActive((btnType == ShowBtnType.Report) && !this._neutral);
                        component.DianZanLaHeiRoot.CustomSetActive((btnType == ShowBtnType.LaHeiDianZan) && !this._neutral);
                    }
                }
                if (this._rightListScript != null)
                {
                    int num3 = this._rightListScript.GetElementAmount();
                    for (int j = 0; j < num3; j++)
                    {
                        SettlementHelper helper2 = this._rightListScript.GetElemenet(j).gameObject.GetComponent<SettlementHelper>();
                        helper2.AddFriendRoot.CustomSetActive((btnType == ShowBtnType.AddFriend) && !this._neutral);
                        helper2.ReportRoot.CustomSetActive((btnType == ShowBtnType.Report) && !this._neutral);
                        helper2.DianZanLaHeiRoot.CustomSetActive((btnType == ShowBtnType.LaHeiDianZan) && !this._neutral);
                    }
                }
            }
        }

        private void ImpSwitchStatistics()
        {
            if (this._settleFormScript != null)
            {
                bool bActive = true;
                if (this._leftListScript != null)
                {
                    int elementAmount = this._leftListScript.GetElementAmount();
                    for (int i = 0; i < elementAmount; i++)
                    {
                        SettlementHelper component = this._leftListScript.GetElemenet(i).gameObject.GetComponent<SettlementHelper>();
                        if (component.Detail.activeSelf)
                        {
                            component.Detail.CustomSetActive(false);
                            component.Damage.CustomSetActive(true);
                            bActive = false;
                        }
                        else
                        {
                            component.Detail.CustomSetActive(true);
                            component.Damage.CustomSetActive(false);
                            bActive = true;
                        }
                    }
                }
                if (this._rightListScript != null)
                {
                    int num3 = this._rightListScript.GetElementAmount();
                    for (int j = 0; j < num3; j++)
                    {
                        SettlementHelper helper2 = this._rightListScript.GetElemenet(j).gameObject.GetComponent<SettlementHelper>();
                        if (helper2.Detail.activeSelf)
                        {
                            helper2.Detail.CustomSetActive(false);
                            helper2.Damage.CustomSetActive(true);
                        }
                        else
                        {
                            helper2.Detail.CustomSetActive(true);
                            helper2.Damage.CustomSetActive(false);
                        }
                    }
                }
                this._settleFormScript.m_formWidgets[0x11].CustomSetActive(bActive);
                this._settleFormScript.m_formWidgets[0x12].CustomSetActive(bActive);
                this._settleFormScript.m_formWidgets[0x13].CustomSetActive(!bActive);
                this._settleFormScript.m_formWidgets[20].CustomSetActive(!bActive);
                this._settleFormScript.m_formWidgets[11].CustomSetActive(!bActive);
                this._settleFormScript.m_formWidgets[12].CustomSetActive(bActive);
                this.UpdateSharePVPDataCaption(bActive);
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ProfitContinue, new CUIEventManager.OnUIEventHandler(this.OnClickProfitContinue));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_TimerEnd, new CUIEventManager.OnUIEventHandler(this.OnSettlementTimerEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickBack, new CUIEventManager.OnUIEventHandler(this.OnClickBack));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickAgain, new CUIEventManager.OnUIEventHandler(this.OnClickBattleAgain));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_SaveReplay, new CUIEventManager.OnUIEventHandler(this.OnClickSaveReplay));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickStatistics, new CUIEventManager.OnUIEventHandler(this.OnSwitchStatistics));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowReport, new CUIEventManager.OnUIEventHandler(this.OnShowReport));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_CloseReport, new CUIEventManager.OnUIEventHandler(this.OnCloseReport));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_DoReport, new CUIEventManager.OnUIEventHandler(this.OnDoReport));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickAddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickDianLa, new CUIEventManager.OnUIEventHandler(this.OnClickDianZanLaHei));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OnCloseProfit, new CUIEventManager.OnUIEventHandler(this.OnCloseProfit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OnCloseSettlement, new CUIEventManager.OnUIEventHandler(this.OnCloseSettlement));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_SwitchAddFriendReport, new CUIEventManager.OnUIEventHandler(this.OnSwitchAddFriendReport));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickLadderContinue, new CUIEventManager.OnUIEventHandler(this.OnLadderClickContinue));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickShowAchievements, new CUIEventManager.OnUIEventHandler(this.OnShowAchievements));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OpenSharePVPDefeat, new CUIEventManager.OnUIEventHandler(this.OnShowDefeatShare));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickItemDisplay, new CUIEventManager.OnUIEventHandler(this.OnClickItemDisplay));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowPVPSettleData, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleData));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowPVPSettleDataClose, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleDataClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowUpdateGradeShare, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShare));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowUpdateGradeShareClose, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShareClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ContinuousWinLastMarkShinningEnd, new CUIEventManager.OnUIEventHandler(this.OnContinuousWinLastMarkShinningEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ContinuousWinNoLastMarkShinningEnd, new CUIEventManager.OnUIEventHandler(this.OnContinuousWinNoLastMarkShinningEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowAddFriendBtn, new CUIEventManager.OnUIEventHandler(this.OnShowFriendBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowReportBtn, new CUIEventManager.OnUIEventHandler(this.OnShowReportBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowDianLaBtn, new CUIEventManager.OnUIEventHandler(this.OnShowLaHeiDianZanBtn));
            this.m_sharePVPAchieventForm = new PvpAchievementForm();
        }

        private void InitShareDataBtn(CUIFormScript form)
        {
            this.m_PVPBtnGroup = form.gameObject.transform.FindChild("Panel/ButtonGrid");
            this.m_PVPSwtichAddFriend = form.gameObject.transform.FindChild("Panel/SwtichAddFriend");
            this.m_PVPSwitchStatistics = form.gameObject.transform.FindChild("Panel/SwitchStatistics");
            this.m_TxtBtnShareCaption = form.gameObject.transform.FindChild("Panel/ButtonGrid/ButtonShareData/Text").GetComponent<Text>();
            this.m_PVPSwitchOverview = form.gameObject.transform.FindChild("Panel/SwitchOverview");
            this.m_PVPShareDataBtnGroup = form.gameObject.transform.FindChild("Panel/ShareGroup");
            this.m_BtnTimeLine = form.gameObject.transform.FindChild("Panel/ShareGroup/Button_TimeLine");
            this.m_PVPShareBtnClose = form.gameObject.transform.FindChild("Panel/Btn_Share_PVP_DATA_CLOSE");
            this.m_timeLineText = form.gameObject.transform.FindChild("Panel/ShareGroup/Button_TimeLine/ClickText").GetComponent<Text>();
            ShareSys.SetSharePlatfText(this.m_timeLineText);
            this.UpdateSharePVPDataCaption(this.m_bIsDetail);
        }

        public bool IsExistSettleForm()
        {
            bool flag = false;
            if ((((Singleton<CUIManager>.instance.GetForm(FightForm.s_battleUIForm) == null) && (Singleton<CUIManager>.instance.GetForm(WinLose.m_FormPath) == null)) && ((Singleton<CUIManager>.instance.GetForm(this.SettlementFormName) == null) && (Singleton<CUIManager>.instance.GetForm(this._profitFormName) == null))) && ((((Singleton<CUIManager>.instance.GetForm(SingleGameSettleMgr.PATH_BURNING_WINLOSE) == null) && (Singleton<CUIManager>.instance.GetForm(SingleGameSettleMgr.PATH_BURNING_SETTLE) == null)) && ((Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_STAR) == null) && (Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_EXP) == null))) && (((Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_ITEM) == null) && (Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_LOSE) == null)) && (Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_LEVELUP) == null))))
            {
                return flag;
            }
            return true;
        }

        private bool IsGuildProfitGameType()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext == null)
            {
                return false;
            }
            if ((!curLvelContext.IsGameTypeLadder() && !curLvelContext.IsGameTypePvpMatch()) && (!curLvelContext.IsGameTypeRewardMatch() && !curLvelContext.IsGameTypeEntertainment()))
            {
                return false;
            }
            return true;
        }

        private bool IsHaveContinuousWinExtraStar()
        {
            return (this._isUp && (((this._newGrade > this._oldGrade) && (((this._oldMaxScore - this._oldScore) + this._newScore) > 1)) || ((this._newGrade == this._oldGrade) && ((this._newScore - this._oldScore) > 1))));
        }

        public void Ladder_PlayLevelDownEnd()
        {
            if (this._ladderAnimator != null)
            {
                this._ladderAnimator.Play("Base Layer.RankConNow_LevelDownEnd");
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_paiwei_jiangji", null);
            }
        }

        public void Ladder_PlayLevelDownStart()
        {
            if (this._ladderAnimator != null)
            {
                this._ladderAnimator.Play("Base Layer.RankConNow_LevelDownStart");
            }
        }

        public void Ladder_PlayLevelUpEnd()
        {
            if (this._ladderAnimator != null)
            {
                this._ladderAnimator.Play("Base Layer.RankConNow_LevelUpEnd");
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_paiwei_shengji", null);
            }
        }

        public void Ladder_PlayLevelUpStart()
        {
            if (this._ladderAnimator != null)
            {
                this._ladderAnimator.Play("Base Layer.RankConNow_LevelUpStart");
            }
        }

        public void Ladder_PlayShowIn()
        {
            if (this._ladderAnimator != null)
            {
                this._ladderAnimator.Play("Base Layer.RankConNow_ShowIn");
            }
        }

        private void LadderAllDisplayEnd()
        {
            if ((this._ladderForm != null) && (this._ladderForm.gameObject != null))
            {
                this._ladderForm.GetWidget(6).GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Ladder_Inherit_Last_Season_Grade_Finish");
                Transform transform = this._ladderForm.gameObject.transform.FindChild("ShareGroup/Btn_Continue");
                if ((transform != null) && (transform.gameObject != null))
                {
                    transform.gameObject.CustomSetActive(true);
                }
                Transform transform2 = this._ladderForm.gameObject.transform.FindChild("ShareGroup/Btn_Share");
                if (transform2 != null)
                {
                    if (CSysDynamicBlock.bSocialBlocked)
                    {
                        this.m_bGrade = false;
                    }
                    if (this.m_bGrade)
                    {
                        transform2.gameObject.CustomSetActive(true);
                    }
                    else
                    {
                        transform2.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        protected void LadderDisplayProcess(bool isSettle)
        {
            CLadderView.ShowRankDetail(this._ladderRoot.transform.FindChild("RankConNow").gameObject, (byte) this._oldGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
            if (isSettle && this.IsHaveContinuousWinExtraStar())
            {
                this._ladderRoot.transform.FindChild("LianShengTxt").gameObject.CustomSetActive(true);
            }
            this._ladderForm.GetWidget(6).CustomSetActive(!isSettle);
            this.ShowContinuousWinPanel(isSettle);
            this.ResetAllXing(this._curDian, this._curMaxScore, false);
            this.Ladder_PlayShowIn();
        }

        public uint NeedChangeGrade()
        {
            uint dwGradeUpNeedScore = 0;
            if (this._isUp && this.NeedDianXing())
            {
                if (this._curDian == GameDataMgr.rankGradeDatabin.GetDataByKey(this._curGrade).dwGradeUpNeedScore)
                {
                    dwGradeUpNeedScore = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) (this._curGrade + 1)).dwGradeUpNeedScore;
                }
                return dwGradeUpNeedScore;
            }
            if ((this._isDown && this.NeedDianXing()) && (this._curDian == 0))
            {
                dwGradeUpNeedScore = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) (this._curGrade - 1)).dwGradeUpNeedScore;
            }
            return dwGradeUpNeedScore;
        }

        public bool NeedDianXing()
        {
            if (this._isUp)
            {
                return ((this._curGrade < this._newGrade) || ((this._curGrade == this._newGrade) && (this._curDian < this._newScore)));
            }
            return ((this._curGrade > this._newGrade) || ((this._curGrade == this._newGrade) && (this._curDian > this._newScore)));
        }

        private void OnAddFriend(CUIEvent uiEvent)
        {
            this.ImpAddFriend(uiEvent);
        }

        private void OnClickBack(CUIEvent uiEvent)
        {
            this.CloseSettlementPanel();
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && (!curLvelContext.IsGameTypeRewardMatch() && !curLvelContext.IsGameTypeLadder()))
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
            }
        }

        private void OnClickBattleAgain(CUIEvent uiEvent)
        {
            CUIEvent event5;
            this.CloseSettlementPanel();
            if (this._isLadderMatch)
            {
                event5 = new CUIEvent {
                    m_eventID = enUIEventID.Matching_OpenLadder
                };
                CUIEvent event2 = event5;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
            }
            else
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && !curLvelContext.IsGameTypeRewardMatch())
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
                }
                else if ((curLvelContext != null) && curLvelContext.IsGameTypeRewardMatch())
                {
                    CUIEvent event3 = new CUIEvent();
                    event3.m_eventParams.tag = 0;
                    event3.m_eventID = enUIEventID.Union_Battle_BattleEntryGroup_Click;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
                }
                else if ((Singleton<CMatchingSystem>.instance.cacheMathingInfo != null) && (Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId != enUIEventID.None))
                {
                    if (Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId == enUIEventID.Room_CreateRoom)
                    {
                        CRoomSystem.ReqCreateRoom(Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapId, Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapType);
                    }
                    else
                    {
                        event5 = new CUIEvent {
                            m_eventID = Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId
                        };
                        event5.m_eventParams.tagUInt = Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapId;
                        event5.m_eventParams.tag = (int) Singleton<CMatchingSystem>.instance.cacheMathingInfo.AILevel;
                        CUIEvent event4 = event5;
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event4);
                    }
                }
            }
        }

        private void OnClickDianZanLaHei(CUIEvent uiEvent)
        {
            uint num = uiEvent.m_eventParams.commonUInt32Param1;
            uint num2 = uiEvent.m_eventParams.commonUInt16Param1;
            ulong num3 = uiEvent.m_eventParams.commonUInt64Param1;
            int num4 = (int) uiEvent.m_eventParams.commonUInt64Param2;
            string playerName = string.Empty;
            DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                PlayerKDA rkda = current.Value;
                if (((rkda != null) && (rkda.PlayerUid == num3)) && (rkda.WorldId == num4))
                {
                    playerName = rkda.PlayerName;
                    break;
                }
            }
            if (num == num2)
            {
                Singleton<CUIManager>.instance.OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("ZanTeam"), playerName), false, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("ZanEnemyTeam"), playerName), false, 1.5f, null, new object[0]);
            }
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x535);
            msg.stPkgData.stLikeReq.stAcntUin.ullUid = num3;
            msg.stPkgData.stLikeReq.stAcntUin.dwLogicWorldId = (uint) num4;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            uiEvent.m_srcWidget.CustomSetActive(false);
        }

        public void OnClickItemDisplay(CUIEvent uiEvent)
        {
            this.DoCoinAndExpTween();
        }

        private void OnClickProfitContinue(CUIEvent uiEvent)
        {
            this.ClosePersonalProfit();
            this.CheckPVPAchievement();
            MonoSingleton<ShareSys>.GetInstance().m_bShowTimeline = false;
        }

        private void OnClickSaveReplay(CUIEvent uiEvent)
        {
            if (Singleton<GameReplayModule>.HasInstance())
            {
                if (Singleton<GameReplayModule>.GetInstance().FlushRecord())
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("replaySaved", true, 1.5f, null, new object[0]);
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("replaySaveFailed", true, 1.5f, null, new object[0]);
                }
            }
            if (this._settleFormScript != null)
            {
                CUICommonSystem.SetButtonEnable(Utility.GetComponetInChild<Button>(this._settleFormScript.gameObject, "Panel/ButtonGrid/BtnSaveReplay"), false, false, true);
            }
        }

        private void OnCloseProfit(CUIEvent uiEvent)
        {
            this.DoCoinTweenEnd();
            this.DoExpTweenEnd();
            this._profitFormScript = null;
        }

        private void OnCloseReport(CUIEvent uiEvent)
        {
            this.ImpCloseReport(uiEvent);
        }

        private void OnCloseSettlement(CUIEvent uiEvent)
        {
            Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Settle, 0L, 0);
            Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
            Singleton<CChatController>.instance.view.UpView(false);
            Singleton<CChatController>.instance.model.sysData.ClearEntryText();
            Singleton<CChatController>.instance.ShowPanel(false, false);
            CChatNetUT.Send_Leave_Settle();
            this._settleFormScript = null;
            this._leftListScript = null;
            this._rightListScript = null;
            this._cacheLastReportGo = null;
            this.ClearShareData();
            Singleton<GameReplayModule>.GetInstance().ClearRecord();
            Singleton<CRecordUseSDK>.instance.CallGameJoyGenerateWithNothing();
        }

        private void OnContinuousWinLastMarkShinningEnd(CUIEvent uiEvent)
        {
            if (this._ladderForm != null)
            {
                GameObject widget = this._ladderForm.GetWidget(5);
                if (widget != null)
                {
                    widget.CustomSetActive(true);
                }
                GameObject obj3 = this._ladderForm.GetWidget(4);
                if (obj3 != null)
                {
                    CUIAnimationScript component = obj3.GetComponent<CUIAnimationScript>();
                    if (component != null)
                    {
                        component.PlayAnimation("Form_Laddersettle_effect", false);
                    }
                }
                GameObject obj4 = this._ladderForm.GetWidget(0);
                if (obj4 != null)
                {
                    obj4.CustomSetActive(false);
                }
            }
        }

        private void OnContinuousWinNoLastMarkShinningEnd(CUIEvent uiEvent)
        {
            GameObject srcWidget = uiEvent.m_srcWidget;
            if (srcWidget != null)
            {
                Transform transform = srcWidget.transform.Find("greyStar/colorStar");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(true);
                }
            }
        }

        private void OnDoReport(CUIEvent uiEvent)
        {
            this.ImpDoReport(uiEvent);
        }

        protected void OnLadderClickContinue(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(this._ladderFormName);
            this._ladderForm = null;
            this._ladderAnimator = null;
            this._ladderRoot = null;
            if (this._isSettlementContinue)
            {
                this.ShowPersonalProfit(this._lastLadderWin);
                this._lastLadderWin = false;
            }
        }

        public void OnLadderLevelDownEndOver()
        {
            if (this._ladderRoot != null)
            {
                this._changingGrage = false;
                this.DianXing();
            }
        }

        public void OnLadderLevelDownStartOver()
        {
            if (this._ladderRoot != null)
            {
                this.ResetAllXing(this._curMaxScore, this._curMaxScore, true);
                CLadderView.ShowRankDetail(this._ladderRoot.transform.FindChild("RankConNow").gameObject, (byte) this._curGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
                this.Ladder_PlayLevelDownEnd();
            }
        }

        public void OnLadderLevelUpEndOver()
        {
            this._changingGrage = false;
            this.DianXing();
        }

        public void OnLadderLevelUpStartOver()
        {
            if (this._ladderRoot != null)
            {
                this.ResetAllXing(0, this._curMaxScore, false);
                CLadderView.ShowRankDetail(this._ladderRoot.transform.FindChild("RankConNow").gameObject, (byte) this._curGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
                this.Ladder_PlayLevelUpEnd();
            }
        }

        public void OnLadderShowInOver()
        {
            this.DianXing();
        }

        public void OnLadderWangZheXingEndOver()
        {
            this._ladderRoot.transform.FindChild("RankConNow/WangZheXing").gameObject.GetComponent<Animator>().enabled = false;
            this.DianXing();
        }

        public void OnLadderWangZheXingStartOver()
        {
        }

        public void OnLadderXingDownOver()
        {
            GameObject xing = this.GetXing(this._curDian, this._curMaxScore);
            if (xing != null)
            {
                xing.CustomSetActive(true);
                Animator component = xing.GetComponent<Animator>();
                if (component == null)
                {
                    return;
                }
                component.enabled = false;
            }
            this._curDian--;
            this.DianXing();
        }

        public void OnLadderXingUpOver()
        {
            GameObject xing = this.GetXing(this._curDian, this._curMaxScore);
            if (xing != null)
            {
                xing.CustomSetActive(true);
                Animator component = xing.GetComponent<Animator>();
                if (component == null)
                {
                    return;
                }
                component.enabled = false;
            }
            this.DianXing();
        }

        private void OnSettlementTimerEnd(CUIEvent uiEvent)
        {
            this.ImpSettlementTimerEnd();
        }

        private void OnShareTimeLineSucc(Transform btn)
        {
            if ((this.m_BtnTimeLine != null) && (this.m_BtnTimeLine == btn))
            {
                if (this.m_bIsDetail)
                {
                    this.m_bShareDataSucc = true;
                }
                else
                {
                    this.m_bShareOverView = true;
                }
            }
        }

        private void OnShareUpdateGradShare(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.OpenForm("UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab", false, true);
            this.m_UpdateGradeForm = form;
            CLadderView.ShowRankDetail(form.transform.FindChild("ShareFrame/Ladder/RankConNow").gameObject, (byte) this._curGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
            MonoSingleton<ShareSys>.GetInstance().UpdateShareGradeForm(form);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(form.gameObject, "ShareFrame/Ladder/RankConNow/PlayerHead");
                if (componetInChild != null)
                {
                    componetInChild.SetImageUrl(masterRoleInfo.HeadUrl);
                }
                Text text = Utility.GetComponetInChild<Text>(form.gameObject, "ShareFrame/Ladder/RankConNow/PlayerName");
                if (text != null)
                {
                    text.text = masterRoleInfo.Name;
                }
            }
        }

        private void OnShareUpdateGradShareClose(CUIEvent uiEvent)
        {
            if (this.m_UpdateGradeForm != null)
            {
                Singleton<CUIManager>.instance.CloseForm(this.m_UpdateGradeForm);
            }
            this.m_UpdateGradeForm = null;
        }

        private void OnShowAchievements(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.instance.OpenForm(this._achievementsTips, false, true);
        }

        private void OnShowDefeatShare(CUIEvent uiEvent)
        {
            if (!this._win)
            {
                this.m_sharePVPAchieventForm.ShowDefeat();
            }
            Singleton<CChatController>.instance.ShowPanel(false, false);
        }

        private void OnShowFriendBtn(CUIEvent uiEvent)
        {
            this.ImpSwitchAddFriendReportLaHeiDianZan(ShowBtnType.AddFriend);
        }

        private void OnShowLaHeiDianZanBtn(CUIEvent uiEvent)
        {
            this.ImpSwitchAddFriendReportLaHeiDianZan(ShowBtnType.LaHeiDianZan);
        }

        private void OnShowPVPSettleData(CUIEvent uiEvnet)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            uint num = 0;
            uint pvpPlayerNum = 0;
            if (curLvelContext != null)
            {
                num = (uint) (curLvelContext.GetGameType() + 1);
                if (curLvelContext.IsMobaModeWithOutGuide())
                {
                    pvpPlayerNum = (uint) curLvelContext.m_pvpPlayerNum;
                }
            }
            uint[] kShareParam = new uint[] { num, pvpPlayerNum };
            MonoSingleton<ShareSys>.GetInstance().m_ShareActivityParam.set(1, 2, kShareParam);
            this.m_bBackShowTimeLine = MonoSingleton<ShareSys>.GetInstance().m_bShowTimeline;
            this.ChangeSharePVPDataBtnState(true);
            Singleton<EventRouter>.instance.AddEventHandler<Transform>(EventID.SHARE_TIMELINE_SUCC, new Action<Transform>(this.OnShareTimeLineSucc));
            this.ShowElementAddFriendBtn(false);
            Singleton<CChatController>.instance.ShowPanel(false, false);
        }

        private void OnShowPVPSettleDataClose(CUIEvent uiEvnet)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SHARE_PVP_SETTLEDATA_CLOSE);
            this.ChangeSharePVPDataBtnState(false);
            MonoSingleton<ShareSys>.GetInstance().m_bShowTimeline = this.m_bBackShowTimeLine;
            this.ShowElementAddFriendBtn(true);
            Singleton<CChatController>.instance.ShowPanel(true, false);
            MonoSingleton<ShareSys>.GetInstance().m_ShareActivityParam.clear();
        }

        private void OnShowReport(CUIEvent uiEvent)
        {
            this.ImpShowReport(uiEvent);
        }

        private void OnShowReportBtn(CUIEvent uiEvent)
        {
            this.ImpSwitchAddFriendReportLaHeiDianZan(ShowBtnType.Report);
        }

        private void OnSwitchAddFriendReport(CUIEvent uiEvent)
        {
        }

        private void OnSwitchStatistics(CUIEvent uiEvent)
        {
            this.ImpSwitchStatistics();
        }

        private void PlayShiningAnimation(int timerSeq)
        {
            if (this._currentShinningAni != null)
            {
                this._currentShinningAni.PlayAnimation("WinningStreak_Ani", false);
                this._currentShinningAni.SetAnimationEvent(enAnimationEventType.AnimationEnd, enUIEventID.SettlementSys_ContinuousWinLastMarkShinningEnd, new stUIEventParams());
            }
        }

        private void PlayXingAnim(uint targetScore, uint targetMax, bool disappear = false)
        {
            if (this._ladderRoot != null)
            {
                GameObject xing = this.GetXing(targetScore, targetMax);
                if ((xing == null) && (targetMax > 5))
                {
                    if (disappear && (this._curDian > 0))
                    {
                        this._curDian--;
                    }
                    GameObject gameObject = this._ladderRoot.transform.FindChild("RankConNow/WangZheXing").gameObject;
                    gameObject.CustomSetActive(true);
                    gameObject.transform.FindChild("XingNumTxt").gameObject.GetComponent<Text>().text = string.Format("X{0}", this._curDian);
                    Animator component = gameObject.GetComponent<Animator>();
                    component.enabled = true;
                    component.Play("Base Layer.wangzhe_starend");
                    Singleton<CSoundManager>.GetInstance().PostEvent(!disappear ? "UI_paiwei_dexing" : "UI_paiwei_diuxing", null);
                }
                else if (xing != null)
                {
                    xing.CustomSetActive(true);
                    Animator animator2 = xing.GetComponent<Animator>();
                    if (animator2 != null)
                    {
                        animator2.enabled = true;
                        xing.transform.FindChild("LiangXing").gameObject.CustomSetActive(true);
                        animator2.Play(!disappear ? "Base Layer.Start_ShowIn" : "Base Layer.Start_Disappear");
                        Singleton<CSoundManager>.GetInstance().PostEvent(!disappear ? "UI_paiwei_dexing" : "UI_paiwei_diuxing", null);
                    }
                }
            }
        }

        private void ResetAllXing(uint targetScore, uint targetMax, bool inverseShow = false)
        {
            if (this._ladderRoot != null)
            {
                GameObject gameObject = this._ladderRoot.transform.FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", 3)).gameObject;
                GameObject obj3 = this._ladderRoot.transform.FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", 4)).gameObject;
                GameObject obj4 = this._ladderRoot.transform.FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", 5)).gameObject;
                gameObject.CustomSetActive(false);
                obj3.CustomSetActive(false);
                obj4.CustomSetActive(false);
                GameObject obj5 = this._ladderRoot.transform.FindChild("RankConNow/WangZheXing").gameObject;
                if (targetMax > 5)
                {
                    obj5.transform.FindChild("XingNumTxt").gameObject.GetComponent<Text>().text = string.Format("X{0}", this._curDian);
                    obj5.gameObject.GetComponent<Animator>().enabled = false;
                }
                else
                {
                    obj5.CustomSetActive(false);
                }
                Transform transform = this._ladderRoot.transform.FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", targetMax));
                if (transform != null)
                {
                    GameObject obj6 = transform.gameObject;
                    obj6.CustomSetActive(true);
                    for (int i = 1; i <= 5; i++)
                    {
                        Transform transform2 = obj6.transform.FindChild(string.Format("Xing{0}", i));
                        Transform transform3 = obj6.transform.FindChild(string.Format("Xing{0}/LiangXing", i));
                        if ((transform2 != null) && (transform3 != null))
                        {
                            transform2.gameObject.GetComponent<Animator>().enabled = inverseShow;
                            transform3.gameObject.CustomSetActive(i <= targetScore);
                        }
                    }
                }
            }
        }

        private void SetAchievementIcon(GameObject achievements, PvpAchievement type, int index)
        {
            if ((index <= 8) && (achievements != null))
            {
                Transform transform = achievements.transform.FindChild(string.Format("Achievement{0}", index));
                if (transform != null)
                {
                    if (type == PvpAchievement.NULL)
                    {
                        transform.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir, type.ToString());
                        transform.gameObject.CustomSetActive(true);
                        transform.GetComponent<Image>().SetSprite(prefabPath, this._settleFormScript, true, false, false);
                    }
                }
            }
        }

        private void SetBpModeOpenTip(byte oldGrade, byte newGrade)
        {
            GameObject widget = this._ladderForm.GetWidget(7);
            if (widget != null)
            {
                widget.CustomSetActive(!Singleton<CLadderSystem>.GetInstance().IsUseBpMode(oldGrade) && Singleton<CLadderSystem>.GetInstance().IsUseBpMode(newGrade));
            }
        }

        private void SetContinuousWinAllMarkGroup()
        {
            uint continuousWinCountForExtraStar;
            GameObject widget = this._ladderForm.GetWidget(1);
            GameObject obj3 = this._ladderForm.GetWidget(2);
            GameObject obj4 = this._ladderForm.GetWidget(3);
            if (this.IsHaveContinuousWinExtraStar() && (this._newGrade > this._oldGrade))
            {
                continuousWinCountForExtraStar = Singleton<CLadderSystem>.GetInstance().GetContinuousWinCountForExtraStar(this._oldGrade);
            }
            else
            {
                continuousWinCountForExtraStar = Singleton<CLadderSystem>.GetInstance().GetContinuousWinCountForExtraStar();
            }
            switch (continuousWinCountForExtraStar)
            {
                case 3:
                    widget.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    this.SetContinuousWinOneMarkGroup(widget);
                    break;

                case 4:
                    widget.CustomSetActive(false);
                    obj3.CustomSetActive(true);
                    obj4.CustomSetActive(false);
                    this.SetContinuousWinOneMarkGroup(obj3);
                    break;

                case 5:
                    widget.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(true);
                    this.SetContinuousWinOneMarkGroup(obj4);
                    break;
            }
        }

        private void SetContinuousWinOneMarkGroup(GameObject pnlStarGo)
        {
            COMDT_RANKDETAIL currentRankDetail = Singleton<CLadderSystem>.GetInstance().GetCurrentRankDetail();
            if (currentRankDetail != null)
            {
                for (int i = 0; i < pnlStarGo.transform.childCount; i++)
                {
                    Transform child = pnlStarGo.transform.GetChild(i);
                    if (child == null)
                    {
                        return;
                    }
                    GameObject gameObject = child.Find("greyStar/colorStar").gameObject;
                    if (this.IsHaveContinuousWinExtraStar())
                    {
                        gameObject.CustomSetActive(i < (pnlStarGo.transform.childCount - 1));
                        if (i == (pnlStarGo.transform.childCount - 1))
                        {
                            this._currentShinningAni = child.GetComponent<CUIAnimationScript>();
                            if (this._newGrade > this._oldGrade)
                            {
                                Singleton<CTimerManager>.GetInstance().AddTimer(0xbb8, 1, new CTimer.OnTimeUpHandler(this.PlayShiningAnimation));
                            }
                            else
                            {
                                this.PlayShiningAnimation(-1);
                            }
                        }
                    }
                    else
                    {
                        gameObject.CustomSetActive(i < (currentRankDetail.dwAddScoreOfConWinCnt - 1));
                        if (i == (currentRankDetail.dwAddScoreOfConWinCnt - 1))
                        {
                            CUIAnimationScript component = child.GetComponent<CUIAnimationScript>();
                            component.PlayAnimation("WinningStreak_Ani", false);
                            stUIEventParams eventParams = new stUIEventParams();
                            component.SetAnimationEvent(enAnimationEventType.AnimationEnd, enUIEventID.SettlementSys_ContinuousWinNoLastMarkShinningEnd, eventParams);
                        }
                    }
                }
            }
        }

        private void SetCreditSettlement()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                if (acntInfo != null)
                {
                    Text componetInChild = Utility.GetComponetInChild<Text>(this._settleFormScript.m_formWidgets[0x16], "Text");
                    if (acntInfo.iSettleCreditValue > 0)
                    {
                        string[] args = new string[] { masterRoleInfo.creditScore.ToString(), acntInfo.iSettleCreditValue.ToString() };
                        componetInChild.text = Singleton<CTextManager>.instance.GetText("Credit_Change_Tips_1", args);
                    }
                    else if (acntInfo.iSettleCreditValue < 0)
                    {
                        string[] textArray2 = new string[] { masterRoleInfo.creditScore.ToString(), acntInfo.iSettleCreditValue.ToString() };
                        componetInChild.text = Singleton<CTextManager>.instance.GetText("Credit_Change_Tips_2", textArray2);
                    }
                    else
                    {
                        componetInChild.text = masterRoleInfo.creditScore.ToString();
                    }
                }
            }
        }

        private void SetExpProfit()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) masterRoleInfo.PvpLevel));
                if (dataByKey != null)
                {
                    COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                    if (acntInfo != null)
                    {
                        GameObject obj2 = this._profitFormScript.m_formWidgets[1];
                        obj2.transform.FindChild("PlayerName").gameObject.GetComponent<Text>().text = masterRoleInfo.Name;
                        obj2.transform.FindChild("PlayerLv").gameObject.GetComponent<Text>().text = string.Format("Lv.{0}", masterRoleInfo.PvpLevel);
                        obj2.transform.FindChild("ExpMaxTip").gameObject.GetComponent<Text>().text = (acntInfo.bExpDailyLimit <= 0) ? string.Empty : Singleton<CTextManager>.GetInstance().GetText("GetExp_Limit");
                        obj2.transform.FindChild("PvpExpTxt").gameObject.GetComponent<Text>().text = string.Format("{0}/{1}", acntInfo.dwPvpExp, dataByKey.dwNeedExp);
                        obj2.transform.FindChild("AddPvpExpTxt").gameObject.GetComponent<Text>().text = (acntInfo.dwPvpSettleExp <= 0) ? string.Empty : string.Format("+{0}", acntInfo.dwPvpSettleExp);
                        obj2.transform.FindChild("Bar").gameObject.CustomSetActive(acntInfo.dwPvpSettleExp != 0);
                        RectTransform component = obj2.transform.FindChild("PvpExpSliderBg/BasePvpExpSlider").gameObject.GetComponent<RectTransform>();
                        RectTransform transform2 = obj2.transform.FindChild("PvpExpSliderBg/AddPvpExpSlider").gameObject.GetComponent<RectTransform>();
                        if (acntInfo.dwPvpSettleExp > 0)
                        {
                            Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
                        }
                        int num = (int) (acntInfo.dwPvpExp - acntInfo.dwPvpSettleExp);
                        _lvUpGrade = (num >= 0) ? 0 : acntInfo.dwPvpLv;
                        float num2 = Mathf.Max((float) 0f, (float) (((float) num) / ((float) dataByKey.dwNeedExp)));
                        float num3 = Mathf.Max((float) 0f, (float) (((num >= 0) ? ((float) acntInfo.dwPvpSettleExp) : ((float) acntInfo.dwPvpExp)) / ((float) dataByKey.dwNeedExp)));
                        component.sizeDelta = new Vector2(num2 * 327.6f, component.sizeDelta.y);
                        transform2.sizeDelta = new Vector2(num2 * 327.6f, transform2.sizeDelta.y);
                        this._expFrom = num2;
                        this._expTo = num2 + num3;
                        this._expTweenRect = transform2;
                        component.gameObject.CustomSetActive(num >= 0);
                        CUIHttpImageScript script = obj2.transform.FindChild("HeadImage").GetComponent<CUIHttpImageScript>();
                        Image image = obj2.transform.FindChild("NobeIcon").GetComponent<Image>();
                        Image image2 = obj2.transform.FindChild("HeadFrame").GetComponent<Image>();
                        if (!CSysDynamicBlock.bSocialBlocked)
                        {
                            string headUrl = masterRoleInfo.HeadUrl;
                            script.SetImageUrl(headUrl);
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false);
                            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
                        }
                        else
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, 0, false);
                        }
                        GameObject gameObject = obj2.transform.FindChild("DoubleExp").gameObject;
                        gameObject.CustomSetActive(false);
                        COMDT_REWARD_MULTIPLE_DETAIL multiDetail = Singleton<BattleStatistic>.GetInstance().multiDetail;
                        for (int i = 0; i < StrHelper.Length; i++)
                        {
                            StrHelper[i] = null;
                        }
                        if (multiDetail != null)
                        {
                            int num5 = CUseable.GetMultiple(ref multiDetail, 15, -1);
                            COMDT_MULTIPLE_INFO comdt_multiple_info = CUseable.GetMultipleInfo(ref multiDetail, 15, -1);
                            if (comdt_multiple_info != null)
                            {
                                GameObject obj4 = obj2.transform.FindChild("FirstWin").gameObject;
                                if (comdt_multiple_info.dwFirstWinAdd > 0)
                                {
                                    obj4.CustomSetActive(true);
                                    string[] args = new string[] { comdt_multiple_info.dwFirstWinAdd.ToString(CultureInfo.InvariantCulture) };
                                    string text = Singleton<CTextManager>.GetInstance().GetText("Daily_Quest_FirstVictoryName", args);
                                    obj4.GetComponent<Text>().text = text;
                                    obj4.GetComponent<Text>().text = text;
                                }
                                else
                                {
                                    obj4.CustomSetActive(false);
                                }
                                if (num5 > 0)
                                {
                                    string[] textArray2 = new string[] { num5.ToString(CultureInfo.InvariantCulture) };
                                    StrHelper[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_1", textArray2);
                                    if (comdt_multiple_info.dwPvpDailyRatio > 0)
                                    {
                                        string[] textArray3 = new string[] { masterRoleInfo.dailyPvpCnt.ToString(CultureInfo.InvariantCulture), (comdt_multiple_info.dwPvpDailyRatio / 100).ToString(CultureInfo.InvariantCulture) };
                                        StrHelper[1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_2", textArray3);
                                    }
                                    if (comdt_multiple_info.dwQQVIPRatio > 0)
                                    {
                                        if (masterRoleInfo.HasVip(0x10))
                                        {
                                            string[] textArray4 = new string[] { (comdt_multiple_info.dwQQVIPRatio / 100).ToString(CultureInfo.InvariantCulture) };
                                            StrHelper[2] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", textArray4);
                                        }
                                        else if (masterRoleInfo.HasVip(1))
                                        {
                                            string[] textArray5 = new string[] { (comdt_multiple_info.dwQQVIPRatio / 100).ToString(CultureInfo.InvariantCulture) };
                                            StrHelper[2] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", textArray5);
                                        }
                                    }
                                    if (comdt_multiple_info.dwPropRatio > 0)
                                    {
                                        StrHelper[3] = string.Format(Singleton<CTextManager>.GetInstance().GetText("Pvp_settle_Common_Tips_4"), comdt_multiple_info.dwPropRatio / 100, masterRoleInfo.GetExpWinCount(), Math.Ceiling((double) (((float) masterRoleInfo.GetExpExpireHours()) / 24f)));
                                    }
                                    if (comdt_multiple_info.dwWealRatio > 0)
                                    {
                                        string[] textArray6 = new string[] { (comdt_multiple_info.dwWealRatio / 100).ToString(CultureInfo.InvariantCulture) };
                                        StrHelper[4] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_6", textArray6);
                                    }
                                    if (comdt_multiple_info.dwWXGameCenterLoginRatio > 0)
                                    {
                                        string[] textArray7 = new string[] { (comdt_multiple_info.dwWXGameCenterLoginRatio / 100).ToString() };
                                        StrHelper[5] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", textArray7);
                                    }
                                    if (comdt_multiple_info.dwQQGameCenterLoginRatio > 0)
                                    {
                                        string[] textArray8 = new string[] { (comdt_multiple_info.dwQQGameCenterLoginRatio / 100).ToString() };
                                        StrHelper[6] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray8);
                                    }
                                    if (comdt_multiple_info.dwIOSVisitorLoginRatio > 0)
                                    {
                                        string[] textArray9 = new string[] { (comdt_multiple_info.dwIOSVisitorLoginRatio / 100).ToString() };
                                        StrHelper[7] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray9);
                                    }
                                    string str2 = StrHelper[0];
                                    for (int j = 1; j < StrHelper.Length; j++)
                                    {
                                        if (!string.IsNullOrEmpty(StrHelper[j]))
                                        {
                                            str2 = string.Format("{0}\n{1}", str2, StrHelper[j]);
                                        }
                                    }
                                    gameObject.CustomSetActive(true);
                                    gameObject.GetComponentInChildren<Text>().text = string.Format("+{0}%", num5);
                                    CUICommonSystem.SetCommonTipsEvent(this._profitFormScript, gameObject, str2, enUseableTipsPos.enLeft);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetGoldCoinProfit()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                if (acntInfo != null)
                {
                    GameObject obj2 = this._profitFormScript.m_formWidgets[2];
                    Text component = obj2.transform.FindChild("GoldNum").GetComponent<Text>();
                    component.text = "+0";
                    this._coinFrom = 0f;
                    this._coinTo = acntInfo.dwPvpSettleCoin;
                    this._coinTweenText = component;
                    obj2.transform.FindChild("GoldMax").gameObject.CustomSetActive(acntInfo.bReachDailyLimit > 0);
                    GameObject gameObject = obj2.transform.FindChild("DoubleCoin").gameObject;
                    gameObject.CustomSetActive(false);
                    Transform transform = obj2.transform.FindChild("QQVipIcon");
                    transform.gameObject.CustomSetActive(false);
                    for (int i = 0; i < StrHelper.Length; i++)
                    {
                        StrHelper[i] = null;
                    }
                    COMDT_REWARD_MULTIPLE_DETAIL multiDetail = Singleton<BattleStatistic>.GetInstance().multiDetail;
                    if (multiDetail != null)
                    {
                        int num2 = CUseable.GetMultiple(ref multiDetail, 11, -1);
                        COMDT_MULTIPLE_INFO comdt_multiple_info = CUseable.GetMultipleInfo(ref multiDetail, 11, -1);
                        if (comdt_multiple_info != null)
                        {
                            GameObject obj5 = obj2.transform.FindChild("FirstWin").gameObject;
                            if (comdt_multiple_info.dwFirstWinAdd > 0)
                            {
                                obj5.CustomSetActive(true);
                                string[] args = new string[] { comdt_multiple_info.dwFirstWinAdd.ToString(CultureInfo.InvariantCulture) };
                                obj5.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Daily_Quest_FirstVictoryName", args);
                            }
                            else
                            {
                                obj5.CustomSetActive(false);
                            }
                            if (num2 > 0)
                            {
                                string[] textArray2 = new string[] { num2.ToString() };
                                StrHelper[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_7", textArray2);
                                if (comdt_multiple_info.dwPvpDailyRatio > 0)
                                {
                                    string[] textArray3 = new string[] { masterRoleInfo.dailyPvpCnt.ToString(), (comdt_multiple_info.dwPvpDailyRatio / 100).ToString() };
                                    StrHelper[1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_8", textArray3);
                                }
                                if ((comdt_multiple_info.dwQQVIPRatio > 0) && (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ))
                                {
                                    if (masterRoleInfo.HasVip(0x10))
                                    {
                                        string[] textArray4 = new string[] { (comdt_multiple_info.dwQQVIPRatio / 100).ToString() };
                                        StrHelper[2] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", textArray4);
                                    }
                                    else
                                    {
                                        string[] textArray5 = new string[] { (comdt_multiple_info.dwQQVIPRatio / 100).ToString() };
                                        StrHelper[2] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", textArray5);
                                    }
                                    MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(transform.GetComponent<Image>());
                                    transform.FindChild("Text").GetComponent<Text>().text = string.Format("+{0}%", (comdt_multiple_info.dwQQVIPRatio / 100).ToString());
                                    transform.gameObject.CustomSetActive(true);
                                }
                                if (comdt_multiple_info.dwPropRatio > 0)
                                {
                                    StrHelper[3] = string.Format(Singleton<CTextManager>.GetInstance().GetText("Pvp_settle_Common_Tips_10"), comdt_multiple_info.dwPropRatio / 100, masterRoleInfo.GetCoinWinCount(), Math.Ceiling((double) (((float) masterRoleInfo.GetCoinExpireHours()) / 24f)));
                                }
                                if (comdt_multiple_info.dwWealRatio > 0)
                                {
                                    string[] textArray6 = new string[] { (comdt_multiple_info.dwWealRatio / 100).ToString() };
                                    StrHelper[4] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_12", textArray6);
                                }
                                if (comdt_multiple_info.dwWXGameCenterLoginRatio > 0)
                                {
                                    string[] textArray7 = new string[] { (comdt_multiple_info.dwWXGameCenterLoginRatio / 100).ToString() };
                                    StrHelper[5] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", textArray7);
                                    Transform transform2 = obj2.transform.FindChild("WXIcon");
                                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(transform2.gameObject, masterRoleInfo.m_privilegeType, ApolloPlatform.Wechat, false, CSysDynamicBlock.bLobbyEntryBlocked);
                                    transform2.FindChild("Text").GetComponent<Text>().text = string.Format("+{0}%", (comdt_multiple_info.dwWXGameCenterLoginRatio / 100).ToString());
                                }
                                if (comdt_multiple_info.dwQQGameCenterLoginRatio > 0)
                                {
                                    string[] textArray8 = new string[] { (comdt_multiple_info.dwQQGameCenterLoginRatio / 100).ToString() };
                                    StrHelper[6] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray8);
                                    Transform transform3 = obj2.transform.FindChild("QQGameCenterIcon");
                                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(transform3.gameObject, masterRoleInfo.m_privilegeType, ApolloPlatform.QQ, false, CSysDynamicBlock.bLobbyEntryBlocked);
                                    transform3.FindChild("Text").GetComponent<Text>().text = string.Format("+{0}%", (comdt_multiple_info.dwQQGameCenterLoginRatio / 100).ToString());
                                }
                                if (this.IsGuildProfitGameType() && Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
                                {
                                    string[] textArray9 = new string[] { CGuildHelper.GetCoinProfitPercentage(CGuildHelper.GetGuildLevel()).ToString() };
                                    StrHelper[7] = Singleton<CTextManager>.GetInstance().GetText("Guild_Settlement_Guild_Coin_Plus_Tip2", textArray9);
                                }
                                else
                                {
                                    StrHelper[7] = string.Empty;
                                }
                                if (comdt_multiple_info.dwIOSVisitorLoginRatio > 0)
                                {
                                    string[] textArray10 = new string[] { (comdt_multiple_info.dwIOSVisitorLoginRatio / 100).ToString() };
                                    StrHelper[8] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", textArray10);
                                    Transform transform4 = obj2.transform.FindChild("GuestGameCenterIcon");
                                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(transform4.gameObject, masterRoleInfo.m_privilegeType, ApolloPlatform.Guest, false, CSysDynamicBlock.bLobbyEntryBlocked);
                                    transform4.FindChild("Text").GetComponent<Text>().text = string.Format("+{0}%", (comdt_multiple_info.dwIOSVisitorLoginRatio / 100).ToString());
                                }
                                string str = StrHelper[0];
                                for (int j = 1; j < StrHelper.Length; j++)
                                {
                                    if (!string.IsNullOrEmpty(StrHelper[j]))
                                    {
                                        str = string.Format("{0}\n{1}", str, StrHelper[j]);
                                    }
                                }
                                if (CSysDynamicBlock.bLobbyEntryBlocked)
                                {
                                    gameObject.CustomSetActive(false);
                                }
                                else
                                {
                                    gameObject.CustomSetActive(true);
                                    gameObject.GetComponentInChildren<Text>().text = string.Format("+{0}%", num2);
                                    CUICommonSystem.SetCommonTipsEvent(this._profitFormScript, gameObject, str, enUseableTipsPos.enLeft);
                                }
                                GameObject obj6 = obj2.transform.Find("GuildPlusTip").gameObject;
                                bool bActive = ((this.IsGuildProfitGameType() && !Singleton<CGuildSystem>.GetInstance().IsInNormalGuild()) && Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION)) && (masterRoleInfo.PvpLevel <= CGuildSystem.s_showCoinProfitTipMaxLevel);
                                obj6.CustomSetActive(bActive);
                                if (bActive)
                                {
                                    string[] textArray11 = new string[] { CGuildHelper.GetCoinProfitPercentage(0x13).ToString() };
                                    obj6.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Guild_Settlement_Guild_Coin_Plus_Tip1", textArray11);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetGuildInfo()
        {
            GameObject obj2 = this._profitFormScript.m_formWidgets[4];
            obj2.CustomSetActive(false);
            if (((Singleton<BattleStatistic>.GetInstance().acntInfo != null) && Singleton<CGuildSystem>.GetInstance().IsInNormalGuild()) && this.IsGuildProfitGameType())
            {
                GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
                if (playerGuildMemberInfo != null)
                {
                    GameObject obj3 = this._profitFormScript.m_formWidgets[7];
                    Text component = obj2.transform.FindChild("GuildPointTxt").GetComponent<Text>();
                    uint num = playerGuildMemberInfo.RankInfo.byGameRankPoint - CGuildSystem.s_lastByGameRankpoint;
                    object[] inParameters = new object[] { playerGuildMemberInfo.RankInfo.byGameRankPoint, CGuildSystem.s_lastByGameRankpoint };
                    DebugHelper.Assert(playerGuildMemberInfo.RankInfo.byGameRankPoint >= CGuildSystem.s_lastByGameRankpoint, "byGameRankPoint={0}, s_lastByGameRankpoint={1}", inParameters);
                    if (num > 0)
                    {
                        obj3.CustomSetActive(false);
                        component.text = num.ToString(CultureInfo.InvariantCulture);
                    }
                    else if (CGuildSystem.s_lastByGameRankpoint >= CGuildSystem.s_rankpointProfitMax)
                    {
                        obj3.CustomSetActive(true);
                        component.text = string.Empty;
                    }
                    else
                    {
                        obj3.CustomSetActive(false);
                        component.text = string.Empty;
                    }
                    CGuildSystem.s_lastByGameRankpoint = playerGuildMemberInfo.RankInfo.byGameRankPoint;
                    CUICommonSystem.SetCommonTipsEvent(this._profitFormScript, obj2, Singleton<CTextManager>.GetInstance().GetText("Guild_Settlement_Guild_Info_Tip"), enUseableTipsPos.enTop);
                    obj2.CustomSetActive(true);
                }
            }
        }

        public void SetLadderDisplayOldAndNewGrade(uint oldGrade, uint oldScore, uint newGrade, uint newScore)
        {
            this._oldGrade = Math.Max(oldGrade, 1);
            this._oldScore = oldScore;
            this._oldMaxScore = GameDataMgr.rankGradeDatabin.GetDataByKey(this._oldGrade).dwGradeUpNeedScore;
            this._newGrade = Math.Max(newGrade, 1);
            this._newScore = newScore;
            this._newMaxScore = GameDataMgr.rankGradeDatabin.GetDataByKey(this._oldGrade).dwGradeUpNeedScore;
            this._isUp = false;
            this._isDown = false;
            if (this._oldGrade < this._newGrade)
            {
                this.m_bGrade = true;
            }
            else
            {
                this.m_bGrade = false;
            }
            if ((this._oldGrade < this._newGrade) || ((this._oldGrade == this._newGrade) && (this._oldScore < this._newScore)))
            {
                this._isUp = true;
                this._isDown = false;
            }
            else
            {
                this._isDown = true;
                this._isUp = false;
            }
            this._curDian = this._oldScore;
            this._curGrade = this._oldGrade;
            this._curMaxScore = this._oldMaxScore;
        }

        private void SetLadderInfo()
        {
            GameObject obj2 = this._profitFormScript.m_formWidgets[5];
            obj2.CustomSetActive(false);
            this._isLadderMatch = false;
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsGameTypeLadder())
            {
                COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
                if (rankInfo != null)
                {
                    obj2.CustomSetActive(true);
                    this._isLadderMatch = true;
                    Transform transform = obj2.transform.FindChild(string.Format("RankLevelName", new object[0]));
                    if (transform != null)
                    {
                        Text component = transform.gameObject.GetComponent<Text>();
                        ResRankGradeConf dataByKey = GameDataMgr.rankGradeDatabin.GetDataByKey((uint) rankInfo.bNowGrade);
                        component.text = (dataByKey != null) ? StringHelper.UTF8BytesToString(ref dataByKey.szGradeDesc) : string.Empty;
                    }
                    if (obj2.transform.FindChild(string.Format("WangZheXingTxt", new object[0])) != null)
                    {
                        Text text2 = obj2.transform.FindChild(string.Format("WangZheXingTxt", new object[0])).gameObject.GetComponent<Text>();
                        if (rankInfo.bNowGrade == GameDataMgr.rankGradeDatabin.count)
                        {
                            Transform transform2 = obj2.transform.FindChild(string.Format("XingGrid/ImgScore{0}", 1));
                            if (transform2 != null)
                            {
                                transform2.gameObject.CustomSetActive(true);
                            }
                            text2.gameObject.CustomSetActive(true);
                            text2.text = string.Format("X{0}", rankInfo.dwNowScore);
                        }
                        else
                        {
                            text2.gameObject.CustomSetActive(false);
                            for (int i = 1; i <= rankInfo.dwNowScore; i++)
                            {
                                Transform transform3 = obj2.transform.FindChild(string.Format("XingGrid/ImgScore{0}", i));
                                if (transform3 != null)
                                {
                                    transform3.gameObject.CustomSetActive(true);
                                }
                            }
                        }
                        GameObject gameObject = obj2.transform.Find("txtContinuousWinTip").gameObject;
                        bool bActive = this.IsHaveContinuousWinExtraStar();
                        gameObject.CustomSetActive(bActive);
                        this._profitFormScript.m_formWidgets[6].gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public void SetLastMatchDuration(string duration, string startTime, uint startTimeInt)
        {
            this._duration = duration;
            this._startTime = startTime;
            this._startTimeInt = startTimeInt;
        }

        private void SetMapInfo()
        {
            GameObject obj2 = this._profitFormScript.m_formWidgets[6];
            obj2.CustomSetActive(false);
            Text component = obj2.transform.FindChild("GameType").GetComponent<Text>();
            Text text2 = obj2.transform.FindChild("MapName").GetComponent<Text>();
            string text = Singleton<CTextManager>.instance.GetText("Battle_Settle_Game_Type_Single");
            string levelName = string.Empty;
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext != null)
            {
                uint mapID = (uint) curLvelContext.m_mapID;
                if (curLvelContext.IsMobaMode())
                {
                    obj2.CustomSetActive(true);
                    levelName = curLvelContext.m_levelName;
                    if (curLvelContext.IsGameTypeRewardMatch())
                    {
                        text = curLvelContext.m_SecondName;
                    }
                    else
                    {
                        text = Singleton<CTextManager>.instance.GetText(string.Format("Battle_Settle_Game_Type{0}", curLvelContext.m_pvpPlayerNum / 2));
                    }
                }
                component.text = text;
                text2.text = levelName;
            }
        }

        private void SetPlayerSettlement()
        {
            CUIListScript script = null;
            CUIListScript component = null;
            component = this._settleFormScript.m_formWidgets[7].GetComponent<CUIListScript>();
            script = this._settleFormScript.m_formWidgets[8].GetComponent<CUIListScript>();
            int count = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(COM_PLAYERCAMP.COM_PLAYERCAMP_1).Count;
            int amount = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(COM_PLAYERCAMP.COM_PLAYERCAMP_2).Count;
            this.playerNum = count + amount;
            component.SetElementAmount(count);
            script.SetElementAmount(amount);
            this._curLeftIndex = 0;
            this._curRightIndex = 0;
            CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
            if (playerKDAStat != null)
            {
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
                this._camp1TotalDamage = 0;
                this._camp1TotalTakenDamage = 0;
                this._camp1TotalToHeroDamage = 0;
                this._camp2TotalDamage = 0;
                this._camp2TotalTakenDamage = 0;
                this._camp2TotalToHeroDamage = 0;
                this._camp1TotalKill = 0;
                this._camp2TotalKill = 0;
                this._myCamp = Singleton<GamePlayerCenter>.GetInstance().hostPlayerCamp;
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    PlayerKDA kda = current.Value;
                    this.CollectPlayerKda(kda);
                }
                GameObject obj2 = this._settleFormScript.m_formWidgets[4];
                obj2.transform.FindChild("LeftTotalKill").gameObject.GetComponent<Text>().text = this._camp1TotalKill.ToString(CultureInfo.InvariantCulture);
                obj2.transform.FindChild("RightTotalKill").gameObject.GetComponent<Text>().text = this._camp2TotalKill.ToString(CultureInfo.InvariantCulture);
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator2 = playerKDAStat.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> pair2 = enumerator2.Current;
                    PlayerKDA rkda2 = pair2.Value;
                    this.UpdatePlayerKda(rkda2);
                }
            }
        }

        private void SetProficiencyInfo()
        {
            GameObject obj2 = this._profitFormScript.m_formWidgets[3];
            obj2.CustomSetActive(false);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                PlayerKDA hostKDA = null;
                if ((Singleton<BattleLogic>.GetInstance().battleStat != null) && (Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null))
                {
                    hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
                }
                if (hostKDA != null)
                {
                    CHeroInfo info2;
                    RectTransform component = obj2.transform.FindChild("ProficiencySliderBg/BaseProficiencySlider").gameObject.GetComponent<RectTransform>();
                    RectTransform transform2 = obj2.transform.FindChild("ProficiencySliderBg/AddProficiencySlider").gameObject.GetComponent<RectTransform>();
                    Text text = obj2.transform.FindChild("HeroName").GetComponent<Text>();
                    Text text2 = obj2.transform.FindChild("ProficiencyLv").GetComponent<Text>();
                    Text text3 = obj2.transform.FindChild("ProficiencyTxt").GetComponent<Text>();
                    Text text4 = obj2.transform.FindChild("AddProficiencyTxt").GetComponent<Text>();
                    Image image = obj2.transform.FindChild("HeroInfo/HeroHeadIcon").GetComponent<Image>();
                    text4.text = null;
                    obj2.transform.FindChild("Bar").gameObject.CustomSetActive(false);
                    text.text = string.Empty;
                    ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
                    uint id = 0;
                    uint skinId = 0;
                    while (enumerator.MoveNext())
                    {
                        HeroKDA current = enumerator.Current;
                        if (current != null)
                        {
                            id = (uint) current.HeroId;
                            skinId = current.SkinId;
                            break;
                        }
                    }
                    masterRoleInfo.GetHeroInfo(id, out info2, false);
                    ActorMeta actorMeta = new ActorMeta {
                        PlayerId = hostKDA.PlayerId,
                        ConfigId = (int) id
                    };
                    ActorStaticData actorData = new ActorStaticData();
                    Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider).GetActorStaticData(ref actorMeta, ref actorData);
                    COMDT_SETTLE_HERO_RESULT_INFO heroSettleInfo = GetHeroSettleInfo(id);
                    if (heroSettleInfo != null)
                    {
                        ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(actorData.TheHeroOnlyInfo.HeroCapability, (int) heroSettleInfo.dwProficiencyLv);
                        if ((heroProficiency != null) && (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode() && (info2 != null)))
                        {
                            obj2.CustomSetActive(true);
                            text.text = actorData.TheResInfo.Name;
                            text4.text = (heroSettleInfo.dwSettleProficiency <= 0) ? null : string.Format("+{0}", heroSettleInfo.dwSettleProficiency);
                            if (heroSettleInfo.dwSettleProficiency == 0)
                            {
                                obj2.transform.FindChild("Bar").gameObject.CustomSetActive(false);
                            }
                            image.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(id, 0)), obj2.GetComponent<CUIFormScript>(), true, false, false);
                            float num3 = 0f;
                            float num4 = 0f;
                            if (CHeroInfo.GetMaxProficiency() == heroSettleInfo.dwProficiencyLv)
                            {
                                num3 = 1f;
                                num4 = 0f;
                                text3.text = "MAX";
                            }
                            else
                            {
                                int num6 = (int) (heroSettleInfo.dwProficiency - heroSettleInfo.dwSettleProficiency);
                                num3 = Mathf.Max((float) 0f, (float) (((float) num6) / ((float) heroProficiency.dwTopPoint)));
                                num4 = Mathf.Max((float) 0f, (float) (((num6 >= 0) ? ((float) heroSettleInfo.dwSettleProficiency) : ((float) heroSettleInfo.dwProficiency)) / ((float) heroProficiency.dwTopPoint)));
                                text3.text = string.Format("{0} / {1}", heroSettleInfo.dwProficiency, heroProficiency.dwTopPoint);
                            }
                            text2.text = GetProficiencyLvTxt(actorData.TheHeroOnlyInfo.HeroCapability, heroSettleInfo.dwProficiencyLv);
                            transform2.sizeDelta = new Vector2((num3 + num4) * 205f, transform2.sizeDelta.y);
                            component.sizeDelta = new Vector2(num3 * 205f, component.sizeDelta.y);
                            transform2.gameObject.CustomSetActive(num4 > 0f);
                        }
                    }
                }
            }
        }

        private void SetSettlementButton()
        {
            GameObject obj2 = this._settleFormScript.m_formWidgets[1];
            obj2.transform.FindChild("BtnBack").gameObject.CustomSetActive(true);
            obj2.transform.FindChild("BtnAgain").gameObject.CustomSetActive(!this._neutral);
            obj2.transform.FindChild("BtnSaveReplay").gameObject.CustomSetActive(Singleton<GameReplayModule>.GetInstance().HasRecord);
            obj2.transform.FindChild("ButtonShare").gameObject.CustomSetActive((this._win && !this._neutral) && !CSysDynamicBlock.bSocialBlocked);
            obj2.transform.FindChild("ButtonShit").gameObject.CustomSetActive((!this._win && !this._neutral) && !CSysDynamicBlock.bSocialBlocked);
            this.m_ShareDataBtn = obj2.transform.FindChild("ButtonShareData").gameObject;
            this.m_ShareDataBtn.CustomSetActive(!this._neutral && !CSysDynamicBlock.bSocialBlocked);
        }

        private void SetSettlementTitle()
        {
            if (this._settleFormScript != null)
            {
                GameObject obj2 = this._settleFormScript.m_formWidgets[0];
                obj2.transform.FindChild("Win").gameObject.CustomSetActive(this._win && !this._neutral);
                obj2.transform.FindChild("Lose").gameObject.CustomSetActive(!this._win && !this._neutral);
            }
        }

        private void SetSpecialItem()
        {
            COMDT_PVPSPECITEM_OUTPUT specialItemInfo = Singleton<BattleStatistic>.GetInstance().SpecialItemInfo;
            COMDT_REWARD_DETAIL rewards = Singleton<BattleStatistic>.GetInstance().Rewards;
            CUseable[] items = new CUseable[specialItemInfo.bOutputCnt + rewards.bNum];
            int index = 0;
            if (rewards.bNum > 0)
            {
                ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(rewards);
                for (int i = 0; i < useableListFromReward.Count; i++)
                {
                    items[index] = useableListFromReward[i];
                    index++;
                }
            }
            if (specialItemInfo.bOutputCnt > 0)
            {
                DictionaryView<uint, ResPVPSpecItem> pvpSpecialItemDict = GameDataMgr.pvpSpecialItemDict;
                if (pvpSpecialItemDict != null)
                {
                    for (int j = 0; j < specialItemInfo.bOutputCnt; j++)
                    {
                        ResPVPSpecItem item = null;
                        if (pvpSpecialItemDict.TryGetValue(specialItemInfo.astOutputInfo[j].dwPVPSpecItemID, out item))
                        {
                            items[index] = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE) item.wItemType, (int) specialItemInfo.astOutputInfo[j].dwPVPSpecItemCnt, item.dwItemID);
                        }
                        index++;
                    }
                }
            }
            string text = Singleton<CTextManager>.GetInstance().GetText("gotAward");
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((curLvelContext != null) && curLvelContext.IsGameTypeRewardMatch())
            {
                if (this._win)
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips8");
                }
                else
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips9");
                }
            }
            if (items.Length == 0)
            {
                this.DoCoinAndExpTween();
            }
            else if ((curLvelContext != null) && curLvelContext.IsGameTypeRewardMatch())
            {
                Singleton<CUIManager>.GetInstance().OpenAwardTip(items, text, true, enUIEventID.SettlementSys_ClickItemDisplay, false, true, "Form_AwardGold");
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenAwardTip(items, text, true, enUIEventID.SettlementSys_ClickItemDisplay, false, true, "Form_Award");
            }
        }

        private void SetTitle()
        {
            GameObject obj2 = this._profitFormScript.m_formWidgets[0];
            obj2.transform.FindChild("Win").gameObject.CustomSetActive(this._win);
            obj2.transform.FindChild("Lose").gameObject.CustomSetActive(!this._win);
        }

        private void SetVictoryTipsBtnInfo(CUIFormScript form)
        {
            PlayerKDA hostKDA = null;
            if ((Singleton<BattleLogic>.GetInstance().battleStat != null) && (Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null))
            {
                hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
            }
            if (hostKDA != null)
            {
                ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
                uint key = 0;
                while (enumerator.MoveNext())
                {
                    HeroKDA current = enumerator.Current;
                    if (current != null)
                    {
                        key = (uint) current.HeroId;
                        break;
                    }
                }
                if (form != null)
                {
                    GameObject widget = form.GetWidget(0x18);
                    if (CBattleGuideManager.EnableHeroVictoryTips() && !CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        widget.CustomSetActive(true);
                        Transform transform = widget.transform;
                        string[] args = new string[] { key.ToString() };
                        transform.FindChild("Btn").GetComponent<CUIEventScript>().m_onClickEventParams.tagStr = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Url_Result", args);
                        if (!this._win && !MonoSingleton<NewbieGuideManager>.instance.isNewbieGuiding)
                        {
                            int num2 = PlayerPrefs.GetInt(PlayerWinTimesStr, 0) + 1;
                            uint globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_SHOW_WINTRICKTIPS_PVPLOSE_TIMES);
                            if (num2 >= globeValue)
                            {
                                string szName;
                                transform.FindChild("Panel_Guide").gameObject.CustomSetActive(true);
                                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(key);
                                if (dataByKey != null)
                                {
                                    szName = dataByKey.szName;
                                }
                                else
                                {
                                    szName = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
                                }
                                string[] textArray2 = new string[] { szName };
                                transform.FindChild("Panel_Guide/Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_text", textArray2);
                                PlayerPrefs.SetInt(PlayerWinTimesStr, 0);
                            }
                            else
                            {
                                PlayerPrefs.SetInt(PlayerWinTimesStr, num2);
                                transform.FindChild("Panel_Guide").gameObject.CustomSetActive(false);
                            }
                        }
                        else
                        {
                            PlayerPrefs.SetInt(PlayerWinTimesStr, 0);
                            transform.FindChild("Panel_Guide").gameObject.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        widget.CustomSetActive(false);
                    }
                }
            }
        }

        private void ShowContinuousWinPanel(bool isSettle)
        {
            GameObject widget = this._ladderForm.GetWidget(5);
            if (widget != null)
            {
                widget.CustomSetActive(false);
            }
            GameObject obj3 = this._ladderForm.GetWidget(0);
            uint continuousWinCountForExtraStar = Singleton<CLadderSystem>.GetInstance().GetContinuousWinCountForExtraStar();
            if ((isSettle && this._lastLadderWin) && (continuousWinCountForExtraStar > 0))
            {
                obj3.CustomSetActive(true);
                this.SetContinuousWinAllMarkGroup();
            }
            else
            {
                obj3.CustomSetActive(false);
            }
        }

        private void ShowElementAddFriendBtn(bool bShow)
        {
            if (!bShow)
            {
                if (this._leftListScript != null)
                {
                    int elementAmount = this._leftListScript.GetElementAmount();
                    for (int i = 0; i < elementAmount; i++)
                    {
                        SettlementHelper component = this._leftListScript.GetElemenet(i).gameObject.GetComponent<SettlementHelper>();
                        component.AddFriendRoot.CustomSetActive(false);
                        component.ReportRoot.CustomSetActive(false);
                        component.DianZanLaHeiRoot.CustomSetActive(false);
                    }
                }
                if (this._rightListScript != null)
                {
                    int num3 = this._rightListScript.GetElementAmount();
                    for (int j = 0; j < num3; j++)
                    {
                        SettlementHelper helper2 = this._rightListScript.GetElemenet(j).gameObject.GetComponent<SettlementHelper>();
                        helper2.AddFriendRoot.CustomSetActive(false);
                        helper2.ReportRoot.CustomSetActive(false);
                        helper2.DianZanLaHeiRoot.CustomSetActive(false);
                    }
                }
            }
            else
            {
                if (this._leftListScript != null)
                {
                    int num5 = this._leftListScript.GetElementAmount();
                    for (int k = 0; k < num5; k++)
                    {
                        SettlementHelper helper3 = this._leftListScript.GetElemenet(k).gameObject.GetComponent<SettlementHelper>();
                        helper3.AddFriendRoot.CustomSetActive(!this._neutral);
                        helper3.ReportRoot.CustomSetActive(!this._neutral);
                        helper3.DianZanLaHeiRoot.CustomSetActive(!this._neutral);
                    }
                }
                if (this._rightListScript != null)
                {
                    int num7 = this._rightListScript.GetElementAmount();
                    for (int m = 0; m < num7; m++)
                    {
                        SettlementHelper helper4 = this._rightListScript.GetElemenet(m).gameObject.GetComponent<SettlementHelper>();
                        helper4.AddFriendRoot.CustomSetActive(!this._neutral);
                        helper4.ReportRoot.CustomSetActive(!this._neutral);
                        helper4.DianZanLaHeiRoot.CustomSetActive(!this._neutral);
                    }
                }
            }
        }

        public void ShowLadderSettleForm(bool win)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(this._ladderFormName) == null)
            {
                this._ladderForm = Singleton<CUIManager>.GetInstance().OpenForm(this._ladderFormName, false, true);
                this._ladderRoot = this._ladderForm.gameObject.transform.FindChild("Ladder").gameObject;
                this._ladderAnimator = this._ladderRoot.GetComponent<Animator>();
                this._lastLadderWin = win;
                Transform transform = this._ladderForm.gameObject.transform.FindChild("ShareGroup/Btn_Share");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(false);
                }
                COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
                if (rankInfo != null)
                {
                    this.SetLadderDisplayOldAndNewGrade(rankInfo.bOldGrade, rankInfo.dwOldScore, rankInfo.bNowGrade, rankInfo.dwNowScore);
                    this.SetBpModeOpenTip(rankInfo.bOldGrade, rankInfo.bNowGrade);
                }
                this._isSettlementContinue = true;
                this.LadderDisplayProcess(true);
            }
        }

        public void ShowLadderSettleFormWithoutSettle()
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(this._ladderFormName) == null)
            {
                this._ladderForm = Singleton<CUIManager>.GetInstance().OpenForm(this._ladderFormName, false, true);
                this._ladderRoot = this._ladderForm.gameObject.transform.FindChild("Ladder").gameObject;
                this._ladderAnimator = this._ladderRoot.GetComponent<Animator>();
                this._isSettlementContinue = false;
                this.LadderDisplayProcess(false);
            }
        }

        public void ShowPersonalProfit(bool win)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(this._profitFormName) == null)
            {
                this._win = win;
                this._profitFormScript = Singleton<CUIManager>.GetInstance().OpenForm(this._profitFormName, false, true);
                if (this._profitFormScript != null)
                {
                    this.SetTitle();
                    this.SetExpProfit();
                    this.SetGoldCoinProfit();
                    this.SetSpecialItem();
                    this.SetMapInfo();
                    this.SetProficiencyInfo();
                    this.SetGuildInfo();
                    this.SetLadderInfo();
                }
            }
        }

        public void ShowSettlementPanel(bool neutralShow = false)
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(this.SettlementFormName) == null)
            {
                this._neutral = neutralShow;
                this._settleFormScript = Singleton<CUIManager>.GetInstance().OpenForm(this.SettlementFormName, false, true);
                this._settleFormScript.m_formWidgets[2].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x10].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[1].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[4].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[5].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[6].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[15].GetComponent<Text>().text = this._duration;
                this._settleFormScript.m_formWidgets[15].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x15].GetComponent<Text>().text = this._startTime;
                this._settleFormScript.m_formWidgets[0x15].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[11].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[12].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[20].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[0x13].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[0x12].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x11].CustomSetActive(true);
                this._settleFormScript.m_formWidgets[0x16].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[0x1b].CustomSetActive(!this._neutral);
                this._settleFormScript.m_formWidgets[9].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[10].CustomSetActive(false);
                this._settleFormScript.m_formWidgets[0x19].CustomSetActive(false);
                this._leftListScript = this._settleFormScript.m_formWidgets[7].GetComponent<CUIListScript>();
                this._rightListScript = this._settleFormScript.m_formWidgets[8].GetComponent<CUIListScript>();
                this.SetSettlementTitle();
                this.SetSettlementButton();
                this.SetPlayerSettlement();
                this.SetCreditSettlement();
                this.SetVictoryTipsBtnInfo(this._settleFormScript);
                if ((this.m_ShareDataBtn != null) && this.m_ShareDataBtn.activeSelf)
                {
                    this.InitShareDataBtn(this._settleFormScript);
                }
                uint[] param = new uint[] { !this._win ? 2 : 1, this.playerNum / 2 };
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.pvpFin, param);
                this._settleFormScript.m_formWidgets[0x1a].CustomSetActive(false);
            }
        }

        public void SnapScreenShotShowBtn(bool bClose)
        {
            if (this.m_PVPShareDataBtnGroup != null)
            {
                this.m_PVPShareDataBtnGroup.gameObject.SetActive(bClose);
            }
            if (this.m_PVPShareBtnClose != null)
            {
                this.m_PVPShareBtnClose.gameObject.SetActive(bClose);
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ProfitContinue, new CUIEventManager.OnUIEventHandler(this.OnClickProfitContinue));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_TimerEnd, new CUIEventManager.OnUIEventHandler(this.OnSettlementTimerEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickBack, new CUIEventManager.OnUIEventHandler(this.OnClickBack));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickAgain, new CUIEventManager.OnUIEventHandler(this.OnClickBattleAgain));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_SaveReplay, new CUIEventManager.OnUIEventHandler(this.OnClickSaveReplay));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickStatistics, new CUIEventManager.OnUIEventHandler(this.OnSwitchStatistics));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowReport, new CUIEventManager.OnUIEventHandler(this.OnShowReport));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_CloseReport, new CUIEventManager.OnUIEventHandler(this.OnCloseReport));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_DoReport, new CUIEventManager.OnUIEventHandler(this.OnDoReport));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickAddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickDianLa, new CUIEventManager.OnUIEventHandler(this.OnClickDianZanLaHei));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OnCloseProfit, new CUIEventManager.OnUIEventHandler(this.OnCloseProfit));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OnCloseSettlement, new CUIEventManager.OnUIEventHandler(this.OnCloseSettlement));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_SwitchAddFriendReport, new CUIEventManager.OnUIEventHandler(this.OnSwitchAddFriendReport));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickLadderContinue, new CUIEventManager.OnUIEventHandler(this.OnLadderClickContinue));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickShowAchievements, new CUIEventManager.OnUIEventHandler(this.OnShowAchievements));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OpenSharePVPDefeat, new CUIEventManager.OnUIEventHandler(this.OnShowDefeatShare));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickItemDisplay, new CUIEventManager.OnUIEventHandler(this.OnClickItemDisplay));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowPVPSettleData, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleData));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowPVPSettleDataClose, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleDataClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowUpdateGradeShare, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShare));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowUpdateGradeShareClose, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShareClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowAddFriendBtn, new CUIEventManager.OnUIEventHandler(this.OnShowFriendBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowReportBtn, new CUIEventManager.OnUIEventHandler(this.OnShowReportBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowDianLaBtn, new CUIEventManager.OnUIEventHandler(this.OnShowLaHeiDianZanBtn));
        }

        private void UpdateAchievements(GameObject achievements, PlayerKDA kda)
        {
            int index = 1;
            ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
            while (enumerator.MoveNext())
            {
                HeroKDA current = enumerator.Current;
                if (current != null)
                {
                    bool flag = false;
                    for (int i = 1; i < 13; i++)
                    {
                        switch (((PvpAchievement) i))
                        {
                            case PvpAchievement.Legendary:
                                if (current.LegendaryNum > 0)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.Legendary, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.PentaKill:
                                if ((current.PentaKillNum > 0) && !flag)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.PentaKill, index);
                                    index++;
                                    flag = true;
                                }
                                break;

                            case PvpAchievement.QuataryKill:
                                if ((current.QuataryKillNum > 0) && !flag)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.QuataryKill, index);
                                    index++;
                                    flag = true;
                                }
                                break;

                            case PvpAchievement.TripleKill:
                                if ((current.TripleKillNum > 0) && !flag)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.TripleKill, index);
                                    index++;
                                    flag = true;
                                }
                                break;

                            case PvpAchievement.DoubleKill:
                                if (current.DoubleKillNum <= 0)
                                {
                                }
                                break;

                            case PvpAchievement.KillMost:
                                if (current.bKillMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.KillMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.HurtMost:
                                if (current.bHurtMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.HurtMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.HurtTakenMost:
                                if (current.bHurtTakenMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.HurtTakenMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.AsssistMost:
                                if (current.bAsssistMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.AsssistMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.GetCoinMost:
                                if (current.bGetCoinMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.GetCoinMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.KillOrganMost:
                                if (current.bKillOrganMost)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.KillOrganMost, index);
                                    index++;
                                }
                                break;

                            case PvpAchievement.RunAway:
                                if ((kda.bRunaway || kda.bDisconnect) || kda.bHangup)
                                {
                                    this.SetAchievementIcon(achievements, PvpAchievement.RunAway, index);
                                    index++;
                                }
                                break;
                        }
                    }
                    for (int j = index; j <= 8; j++)
                    {
                        this.SetAchievementIcon(achievements, PvpAchievement.NULL, j);
                    }
                    break;
                }
            }
        }

        private void UpdateEquip(GameObject equip, PlayerKDA kda)
        {
            int num = 1;
            ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
            while (enumerator.MoveNext())
            {
                HeroKDA current = enumerator.Current;
                if (current != null)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        ushort equipID = current.Equips[i].m_equipID;
                        Transform transform = equip.transform.FindChild(string.Format("TianFu{0}", num));
                        if ((equipID != 0) && (transform != null))
                        {
                            num++;
                            CUICommonSystem.SetEquipIcon(equipID, transform.gameObject, this._settleFormScript);
                        }
                    }
                    for (int j = num; j <= 6; j++)
                    {
                        Transform transform2 = equip.transform.FindChild(string.Format("TianFu{0}", j));
                        if (transform2 != null)
                        {
                            transform2.gameObject.GetComponent<Image>().SetSprite(string.Format("{0}EquipmentSpace", CUIUtility.s_Sprite_Dynamic_Talent_Dir), this._settleFormScript, true, false, false);
                        }
                    }
                    break;
                }
            }
        }

        private void UpdatePlayerKda(PlayerKDA kda)
        {
            if (kda != null)
            {
                CUIListScript script = null;
                int index = 0;
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                if (curLvelContext != null)
                {
                    switch (kda.PlayerCamp)
                    {
                        case COM_PLAYERCAMP.COM_PLAYERCAMP_1:
                            script = this._leftListScript;
                            index = this._curLeftIndex++;
                            break;

                        case COM_PLAYERCAMP.COM_PLAYERCAMP_2:
                            script = this._rightListScript;
                            index = this._curRightIndex++;
                            break;
                    }
                    if (script != null)
                    {
                        CUIListElementScript elemenet = script.GetElemenet(index);
                        if (elemenet != null)
                        {
                            SettlementHelper component = elemenet.gameObject.GetComponent<SettlementHelper>();
                            this.UpdateEquip(component.Tianfu, kda);
                            this.UpdateAchievements(component.Achievements, kda);
                            bool flag = kda.PlayerCamp == Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
                            if (kda.PlayerId == Singleton<BattleStatistic>.instance.GetMvpPlayer(kda.PlayerCamp, (flag && this._win) || (!flag && !this._win)))
                            {
                                component.Mvp.CustomSetActive(true);
                                string prefabPath = string.Empty;
                                if ((this._win && flag) || (!this._win && !flag))
                                {
                                    prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir, "Img_Icon_Red_Mvp");
                                }
                                else
                                {
                                    prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir, "Img_Icon_Blue_Mvp");
                                }
                                component.Mvp.GetComponent<Image>().SetSprite(prefabPath, this._settleFormScript, true, false, false);
                            }
                            else
                            {
                                component.Mvp.CustomSetActive(false);
                            }
                            component.PlayerName.GetComponent<Text>().text = kda.PlayerName;
                            component.PlayerLv.CustomSetActive(false);
                            if (kda.PlayerId == Singleton<GamePlayerCenter>.GetInstance().HostPlayerId)
                            {
                                component.PlayerName.GetComponent<Text>().color = CUIUtility.s_Text_Color_Self;
                                component.PlayerLv.GetComponent<Text>().color = CUIUtility.s_Text_Color_Self;
                                component.ItsMe.CustomSetActive(true);
                                MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.HeroNobe.GetComponent<Image>(), (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false);
                            }
                            else
                            {
                                component.ItsMe.CustomSetActive(false);
                                MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.HeroNobe.GetComponent<Image>(), (int) kda.PlayerVipLv, false);
                            }
                            if (((kda.PlayerId == Singleton<GamePlayerCenter>.GetInstance().HostPlayerId) || Singleton<CFriendContoller>.instance.model.IsGameFriend(kda.PlayerUid, (uint) kda.WorldId)) || (kda.IsComputer && !curLvelContext.m_isWarmBattle))
                            {
                                component.AddFriend.CustomSetActive(false);
                                component.Report.CustomSetActive(false);
                                component.m_AddfriendBtnShow = false;
                                component.m_ReportRootBtnShow = false;
                            }
                            else
                            {
                                component.AddFriend.CustomSetActive(true);
                                component.Report.CustomSetActive(true);
                                component.m_AddfriendBtnShow = true;
                                component.m_ReportRootBtnShow = true;
                                component.AddFriend.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
                                component.AddFriend.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param2 = (ulong) kda.WorldId;
                                component.Report.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
                                component.Report.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param2 = (ulong) kda.WorldId;
                            }
                            if (((kda.PlayerId != Singleton<GamePlayerCenter>.GetInstance().HostPlayerId) && (!kda.IsComputer || curLvelContext.m_isWarmBattle)) && ((this.playerNum >= 6) && !curLvelContext.IsGameTypePvpRoom()))
                            {
                                component.DianZanLaHei.CustomSetActive(true);
                                component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
                                component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param2 = (ulong) kda.WorldId;
                                component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt32Param1 = (uint) kda.PlayerCamp;
                                component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt16Param1 = (ushort) Singleton<GamePlayerCenter>.GetInstance().hostPlayerCamp;
                            }
                            else
                            {
                                component.DianZanLaHei.CustomSetActive(false);
                            }
                            component.AddFriendRoot.CustomSetActive(!this._neutral);
                            component.ReportRoot.CustomSetActive(false);
                            component.DianZanLaHeiRoot.CustomSetActive(false);
                            component.m_AddfriendBtnShow = true;
                            component.m_ReportRootBtnShow = false;
                            ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                HeroKDA current = enumerator.Current;
                                if (current != null)
                                {
                                    component.HeroIcon.GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint) current.HeroId, 0)), this._settleFormScript, true, false, false);
                                    component.HeroLv.GetComponent<Text>().text = string.Format("{0}", current.SoulLevel);
                                    component.Kill.GetComponent<Text>().text = current.numKill.ToString(CultureInfo.InvariantCulture);
                                    component.Death.GetComponent<Text>().text = current.numDead.ToString(CultureInfo.InvariantCulture);
                                    component.Assist.GetComponent<Text>().text = current.numAssist.ToString(CultureInfo.InvariantCulture);
                                    component.Coin.GetComponent<Text>().text = current.TotalCoin.ToString(CultureInfo.InvariantCulture);
                                    uint num2 = 0;
                                    uint num3 = 0;
                                    uint num4 = 0;
                                    if (kda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                                    {
                                        num2 = this._camp1TotalDamage;
                                        num3 = this._camp1TotalTakenDamage;
                                        num4 = this._camp1TotalToHeroDamage;
                                    }
                                    else
                                    {
                                        num2 = this._camp2TotalDamage;
                                        num3 = this._camp2TotalTakenDamage;
                                        num4 = this._camp2TotalToHeroDamage;
                                    }
                                    num2 = Math.Max(1, num2);
                                    num3 = Math.Max(1, num3);
                                    num4 = Math.Max(1, num4);
                                    component.Damage.transform.FindChild("TotalDamageBg/TotalDamage").gameObject.GetComponent<Text>().text = current.hurtToEnemy.ToString(CultureInfo.InvariantCulture);
                                    component.Damage.transform.FindChild("TotalDamageBg/TotalDamageBar").gameObject.GetComponent<Image>().fillAmount = ((float) current.hurtToEnemy) / ((float) num2);
                                    component.Damage.transform.FindChild("TotalDamageBg/Percent").gameObject.GetComponent<Text>().text = string.Format("{0:P1}", ((float) current.hurtToEnemy) / ((float) num2));
                                    component.Damage.transform.FindChild("TotalTakenDamageBg/TotalTakenDamage").gameObject.GetComponent<Text>().text = current.hurtTakenByEnemy.ToString(CultureInfo.InvariantCulture);
                                    component.Damage.transform.FindChild("TotalTakenDamageBg/TotalTakenDamageBar").gameObject.GetComponent<Image>().fillAmount = ((float) current.hurtTakenByEnemy) / ((float) num3);
                                    component.Damage.transform.FindChild("TotalTakenDamageBg/Percent").gameObject.GetComponent<Text>().text = string.Format("{0:P1}", ((float) current.hurtTakenByEnemy) / ((float) num3));
                                    component.Damage.transform.FindChild("TotalDamageHeroBg/TotalDamageHero").gameObject.GetComponent<Text>().text = current.hurtToHero.ToString(CultureInfo.InvariantCulture);
                                    component.Damage.transform.FindChild("TotalDamageHeroBg/TotalDamageHeroBar").gameObject.GetComponent<Image>().fillAmount = ((float) current.hurtToHero) / ((float) num4);
                                    component.Damage.transform.FindChild("TotalDamageHeroBg/Percent").gameObject.GetComponent<Text>().text = string.Format("{0:P1}", ((float) current.hurtToHero) / ((float) num4));
                                    break;
                                }
                            }
                            component.Detail.CustomSetActive(true);
                            component.Damage.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void UpdateSharePVPDataCaption(bool isDetail)
        {
            if (!CSysDynamicBlock.bSocialBlocked && !this._neutral)
            {
                if (!isDetail)
                {
                    if (this.m_TxtBtnShareCaption != null)
                    {
                        this.m_TxtBtnShareCaption.text = "分享数据";
                    }
                    if (this.m_ShareDataBtn != null)
                    {
                        this.m_ShareDataBtn.CustomSetActive(true);
                    }
                }
                else
                {
                    if (this.m_TxtBtnShareCaption != null)
                    {
                        this.m_TxtBtnShareCaption.text = "分享战绩";
                    }
                    if (this.m_ShareDataBtn != null)
                    {
                        this.m_ShareDataBtn.CustomSetActive(true);
                    }
                }
                this.m_bIsDetail = !isDetail;
            }
        }

        private void UpdateTimeBtnState(bool bShow)
        {
            if (this.m_BtnTimeLine != null)
            {
                this.m_BtnTimeLine.GetComponent<CUIEventScript>().enabled = bShow;
                this.m_BtnTimeLine.GetComponent<Button>().interactable = bShow;
                float a = 0.37f;
                if (bShow)
                {
                    a = 1f;
                }
                this.m_BtnTimeLine.GetComponent<Image>().color = new Color(this.m_BtnTimeLine.GetComponent<Image>().color.r, this.m_BtnTimeLine.GetComponent<Image>().color.g, this.m_BtnTimeLine.GetComponent<Image>().color.b, a);
                if (this.m_timeLineText != null)
                {
                    this.m_timeLineText.color = new Color(this.m_timeLineText.color.r, this.m_timeLineText.color.g, this.m_timeLineText.color.b, a);
                }
            }
        }

        public enum enLadderSettleFormWidget
        {
            ContinuousWinPanel,
            ContinuousWin3StarPanel,
            ContinuousWin4StarPanel,
            ContinuousWin5StarPanel,
            ContinuousWinPanelDisappearEffect,
            ContinuousWinPanelDisappearEffectRoot,
            TipText,
            BpModeOpenTip
        }

        protected enum ProfitWidgets
        {
            CoinInfo = 2,
            ExpInfo = 1,
            GuildInfo = 4,
            GuildPointMaxTip = 7,
            LadderInfo = 5,
            None = -1,
            ProficiencyInfo = 3,
            PvpMapInfo = 6,
            WinLoseTitle = 0
        }

        public enum SettlementWidgets
        {
            AddFriendBtn = 9,
            BtnVictoryTips = 0x18,
            ButtonGrid = 1,
            CreditScore = 0x16,
            DamageBtn = 12,
            DamageCaption = 14,
            DetailBtn = 11,
            DetailCaption = 13,
            DianZanLaHeiBtn = 0x19,
            Duration = 15,
            LeftDamageTitle = 0x13,
            LeftOverViewTitle = 0x11,
            LeftPlayers = 5,
            LeftPlayersList = 7,
            MaxNum = 0x1c,
            None = -1,
            Recorder = 0x1a,
            ReplayKitRecord = 0x17,
            Report = 3,
            ReportBtn = 10,
            RightDamageTitle = 20,
            RightOverViewTitle = 0x12,
            RightPlayers = 6,
            RightPlayersList = 8,
            StartTime = 0x15,
            TimeNode = 0x1b,
            Timer = 2,
            TotalScore = 4,
            WaitNote = 0x10,
            WinLoseTitle = 0
        }

        private enum ShowBtnType
        {
            AddFriend,
            Report,
            LaHeiDianZan
        }
    }
}

