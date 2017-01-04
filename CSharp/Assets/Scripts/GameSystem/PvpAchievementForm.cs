namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class PvpAchievementForm
    {
        private ListView<string> barrageList = new ListView<string>();
        private bool[] m_allAchievements;
        private RES_SHOW_ACHIEVEMENT_TYPE m_curAchievemnt = RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_COUNT;
        public static string s_formSharePVPDefeatPath = "UGUI/Form/System/PvP/Settlement/Form_SharePVPDefeat.prefab";
        public static string s_formSharePVPVictoryPath = "UGUI/Form/System/PvP/Settlement/Form_SharePVPVictory.prefab";
        public static string s_imageSharePVPBadge = (CUIUtility.s_Sprite_Dynamic_PvpAchievementShare_Dir + "Img_PVP_ShareIcon_");

        public bool CheckAchievement()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (masterRoleInfo.PvpLevel >= GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9f).dwConfValue))
            {
                for (int i = 0; i < this.m_allAchievements.Length; i++)
                {
                    if (this.m_allAchievements[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int GetAchievementCount()
        {
            int num = 0;
            for (int i = 0; i < this.m_allAchievements.Length; i++)
            {
                if (this.m_allAchievements[i])
                {
                    num++;
                }
            }
            return num;
        }

        private int GetAchievementCount(RES_SHOW_ACHIEVEMENT_TYPE achievement)
        {
            switch (achievement)
            {
                case RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_LEGENDARY:
                    return this.GetKVDataCount(RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_GODLIKE_CNT);

                case RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_TRIPLEKILL:
                    return this.GetKVDataCount(RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_TRIPLE_KILL_CNT);

                case RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_MVP:
                    return this.GetKVDataCount(RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_MVP_CNT);
            }
            return 0;
        }

        private int GetKVDataCount(RES_STATISTIC_SETTLE_DATA_TYPE type)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                for (int i = 0; i < masterRoleInfo.pvpDetail.stKVDetail.dwNum; i++)
                {
                    COMDT_STATISTIC_KEY_VALUE_INFO comdt_statistic_key_value_info = masterRoleInfo.pvpDetail.stKVDetail.astKVDetail[i];
                    if (((RES_STATISTIC_SETTLE_DATA_TYPE) comdt_statistic_key_value_info.dwKey) == type)
                    {
                        return (int) comdt_statistic_key_value_info.dwValue;
                    }
                }
            }
            return 0;
        }

        private void HideDefeat()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_formSharePVPDefeatPath);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_CloseSharePVPDefeat, new CUIEventManager.OnUIEventHandler(this.OnCloseSharePVPDefeat));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShareDefeatAddBarrage, new CUIEventManager.OnUIEventHandler(this.OnSharePVPDefeatAddBarrage));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShareDefeatSelectBarrage, new CUIEventManager.OnUIEventHandler(this.OnSharePVPDefeatSelectBarrage));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShareDefeatBarrageEnable, new CUIEventManager.OnUIEventHandler(this.OnBarrageEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_CloseShareDefeatBarrage, new CUIEventManager.OnUIEventHandler(this.OnCloseBarrage));
        }

        private void HideVictory()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_formSharePVPVictoryPath);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_LeftSwitchPVPAchievement, new CUIEventManager.OnUIEventHandler(this.LeftSwitchAchievementHandle));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_RightSwitchPVPAchievement, new CUIEventManager.OnUIEventHandler(this.RightSwitchAchievementHandle));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OpenSharePVPAchievement, new CUIEventManager.OnUIEventHandler(this.PvpAchievementShareBtnHandle));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ClosePVPAchievement, new CUIEventManager.OnUIEventHandler(this.OnPVPAchievementCloseHandle));
        }

        public void Init(bool bWin)
        {
            this.m_allAchievements = new bool[8];
            this.m_curAchievemnt = RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_COUNT;
            if ((Singleton<BattleLogic>.GetInstance().battleStat != null) && (Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null))
            {
                PlayerKDA hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
                if (hostKDA != null)
                {
                    uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(hostKDA.PlayerCamp, bWin);
                    if ((mvpPlayer != 0) && (mvpPlayer == hostKDA.PlayerId))
                    {
                        this.m_allAchievements[7] = true;
                    }
                    COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
                    if (acntInfo != null)
                    {
                        if (acntInfo.dwCurWeekContinousWinNum == 15)
                        {
                            this.m_allAchievements[0] = true;
                        }
                        else if (acntInfo.dwCurWeekContinousWinNum == 10)
                        {
                            this.m_allAchievements[2] = true;
                        }
                        else if (acntInfo.dwCurWeekContinousWinNum == 5)
                        {
                            this.m_allAchievements[4] = true;
                        }
                    }
                    ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        HeroKDA current = enumerator.Current;
                        if (current != null)
                        {
                            if (current.LegendaryNum > 0)
                            {
                                this.m_allAchievements[5] = true;
                            }
                            if (current.PentaKillNum > 0)
                            {
                                this.m_allAchievements[1] = true;
                            }
                            if (current.QuataryKillNum > 0)
                            {
                                this.m_allAchievements[3] = true;
                            }
                            if (current.TripleKillNum > 0)
                            {
                                this.m_allAchievements[6] = true;
                            }
                        }
                    }
                }
            }
        }

        private bool IsVictoryStreak(RES_SHOW_ACHIEVEMENT_TYPE achievement)
        {
            return (((achievement == RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_FIFTEENVICTORY) || (achievement == RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_FIVEVICTORY)) || (achievement == RES_SHOW_ACHIEVEMENT_TYPE.RES_SHOW_ACHIEVEMENT_TENVICTORY));
        }

        private void LeftSwitchAchievementHandle(CUIEvent ievent)
        {
            int curAchievemnt = (int) this.m_curAchievemnt;
            for (int i = curAchievemnt - 1; i > 0; i--)
            {
                if (this.m_allAchievements[i])
                {
                    this.SwitchAchievement((RES_SHOW_ACHIEVEMENT_TYPE) i);
                    return;
                }
            }
            for (int j = this.m_allAchievements.Length - 1; j > curAchievemnt; j--)
            {
                if (this.m_allAchievements[j])
                {
                    this.SwitchAchievement((RES_SHOW_ACHIEVEMENT_TYPE) j);
                    return;
                }
            }
        }

        private void OnBarrageEnable(CUIEvent uiEvent)
        {
            this.UpdateOneBarrageElement(uiEvent.m_srcWidget, uiEvent.m_srcWidgetIndexInBelongedList, false);
        }

        private void OnCloseBarrage(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_formSharePVPDefeatPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(1);
                if ((widget != null) && widget.activeSelf)
                {
                    widget.CustomSetActive(false);
                }
            }
        }

        private void OnCloseSharePVPDefeat(CUIEvent uiEvent)
        {
            Singleton<CChatController>.instance.ShowPanel(true, false);
            this.HideDefeat();
        }

        private void OnPVPAchievementCloseHandle(CUIEvent uiEvent)
        {
            this.HideVictory();
            Singleton<SettlementSystem>.GetInstance().ShowSettlementPanel(false);
            MonoSingleton<ShareSys>.instance.m_bShowTimeline = false;
        }

        private void OnSharePVPDefeatAddBarrage(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_formSharePVPDefeatPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(1);
                if ((widget != null) && !widget.activeSelf)
                {
                    widget.CustomSetActive(true);
                }
                GameObject obj3 = Utility.FindChild(widget, "BarrageList");
                if (obj3 != null)
                {
                    CUIListScript component = obj3.GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        component.SetElementAmount(this.barrageList.Count);
                        component.MoveElementInScrollArea(0, true);
                        for (int i = 0; i < this.barrageList.Count; i++)
                        {
                            if ((component.GetElemenet(i) != null) && component.IsElementInScrollArea(i))
                            {
                                this.UpdateOneBarrageElement(component.GetElemenet(i).gameObject, i, false);
                            }
                        }
                        if (component.GetSelectedIndex() == -1)
                        {
                            component.SelectElement(0, true);
                        }
                        if (!obj3.activeSelf)
                        {
                            obj3.CustomSetActive(true);
                        }
                    }
                }
            }
        }

        private void OnSharePVPDefeatSelectBarrage(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_formSharePVPDefeatPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(1);
                if (widget != null)
                {
                    GameObject obj3 = Utility.FindChild(widget, "BarrageList");
                    if (obj3 != null)
                    {
                        if (obj3.activeSelf)
                        {
                            obj3.CustomSetActive(false);
                        }
                        GameObject obj4 = Utility.FindChild(widget, "BarrageBg/BarrageText");
                        int selectedIndex = obj3.GetComponent<CUIListScript>().GetSelectedIndex();
                        if ((obj4 != null) && (selectedIndex < this.barrageList.Count))
                        {
                            obj4.GetComponent<Text>().text = this.barrageList[selectedIndex];
                        }
                    }
                }
            }
        }

        private void PvpAchievementShareBtnHandle(CUIEvent ievent)
        {
            MonoSingleton<ShareSys>.GetInstance().OpenShowSharePVPFrom(this.m_curAchievemnt);
        }

        private void RightSwitchAchievementHandle(CUIEvent ievent)
        {
            int curAchievemnt = (int) this.m_curAchievemnt;
            for (int i = curAchievemnt + 1; i < this.m_allAchievements.Length; i++)
            {
                if (this.m_allAchievements[i])
                {
                    this.SwitchAchievement((RES_SHOW_ACHIEVEMENT_TYPE) i);
                    return;
                }
            }
            for (int j = 0; j < curAchievemnt; j++)
            {
                if (this.m_allAchievements[j])
                {
                    this.SwitchAchievement((RES_SHOW_ACHIEVEMENT_TYPE) j);
                    return;
                }
            }
        }

        public void ShowDefeat()
        {
            if ((this.m_allAchievements != null) && (this.m_allAchievements.Length == 8))
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(s_formSharePVPDefeatPath, false, true);
                if (form != null)
                {
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_CloseSharePVPDefeat, new CUIEventManager.OnUIEventHandler(this.OnCloseSharePVPDefeat));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShareDefeatAddBarrage, new CUIEventManager.OnUIEventHandler(this.OnSharePVPDefeatAddBarrage));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShareDefeatSelectBarrage, new CUIEventManager.OnUIEventHandler(this.OnSharePVPDefeatSelectBarrage));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShareDefeatBarrageEnable, new CUIEventManager.OnUIEventHandler(this.OnBarrageEnable));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_CloseShareDefeatBarrage, new CUIEventManager.OnUIEventHandler(this.OnCloseBarrage));
                    DatabinTable<ResDefeatBarrageText, ushort> table = new DatabinTable<ResDefeatBarrageText, ushort>("Databin/Client/Text/DefeatBarrageText.bytes", "wID");
                    for (int i = 0; i < this.m_allAchievements.Length; i++)
                    {
                        if (this.m_allAchievements[i])
                        {
                            this.m_curAchievemnt = (RES_SHOW_ACHIEVEMENT_TYPE) i;
                            break;
                        }
                    }
                    this.barrageList.Clear();
                    if (table != null)
                    {
                        Dictionary<long, object>.Enumerator enumerator = table.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<long, object> current = enumerator.Current;
                            ResDefeatBarrageText text = (ResDefeatBarrageText) current.Value;
                            if (((RES_SHOW_ACHIEVEMENT_TYPE) text.wAchievementType) == this.m_curAchievemnt)
                            {
                                this.barrageList.Add(text.szContent);
                            }
                        }
                    }
                    MonoSingleton<ShareSys>.GetInstance().UpdateSharePVPForm(form, form.GetWidget(2));
                }
            }
        }

        public void ShowVictory()
        {
            if ((this.m_allAchievements != null) && (this.m_allAchievements.Length == 8))
            {
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_formSharePVPVictoryPath, false, true);
                if (script != null)
                {
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_LeftSwitchPVPAchievement, new CUIEventManager.OnUIEventHandler(this.LeftSwitchAchievementHandle));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_RightSwitchPVPAchievement, new CUIEventManager.OnUIEventHandler(this.RightSwitchAchievementHandle));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OpenSharePVPAchievement, new CUIEventManager.OnUIEventHandler(this.PvpAchievementShareBtnHandle));
                    Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ClosePVPAchievement, new CUIEventManager.OnUIEventHandler(this.OnPVPAchievementCloseHandle));
                    for (int i = 0; i < this.m_allAchievements.Length; i++)
                    {
                        if (this.m_allAchievements[i])
                        {
                            this.SwitchAchievement((RES_SHOW_ACHIEVEMENT_TYPE) i);
                            break;
                        }
                    }
                    if (CSysDynamicBlock.bSocialBlocked)
                    {
                        Transform transform = script.transform.Find("AchievementRoot/ButtonGrid/Button_Share");
                        if (transform != null)
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void SwitchAchievement(RES_SHOW_ACHIEVEMENT_TYPE achievement)
        {
            this.m_curAchievemnt = achievement;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_formSharePVPVictoryPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0);
                Utility.FindChild(widget.gameObject, "Title/Text").GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("Share_PvpAchievement_Title" + ((int) achievement));
                Utility.FindChild(widget.gameObject, "Grid").CustomSetActive(this.GetAchievementCount() > 1);
                if (this.IsVictoryStreak(achievement))
                {
                    CUIUtility.SetImageSprite(form.GetWidget(1).GetComponent<Image>(), s_imageSharePVPBadge + 0 + ".prefab", form, true, false, false);
                }
                else
                {
                    CUIUtility.SetImageSprite(form.GetWidget(1).GetComponent<Image>(), s_imageSharePVPBadge + ((int) achievement) + ".prefab", form, true, false, false);
                }
                GameObject gameObject = form.GetWidget(2).gameObject;
                if (gameObject != null)
                {
                    int achievementCount = this.GetAchievementCount(achievement);
                    if (achievementCount != 0)
                    {
                        Utility.FindChild(gameObject, "Number").GetComponent<Text>().text = achievementCount.ToString();
                        Utility.FindChild(gameObject, "Text").GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("Share_PvpAchievement_Desc_" + ((int) achievement));
                    }
                    gameObject.CustomSetActive(achievementCount != 0);
                }
            }
        }

        private void UpdateOneBarrageElement(GameObject go, int index, bool selected = false)
        {
            if ((go != null) && (this.barrageList.Count > index))
            {
                Utility.FindChild(go, "Text").GetComponent<Text>().text = this.barrageList[index];
            }
        }

        public enum ShareDefeatWidget
        {
            ButtonClose,
            BarragePanel,
            DisplayRect
        }

        private enum ShareVictoryWidget
        {
            AchievementRoot,
            BadgeIcon,
            Times
        }
    }
}

