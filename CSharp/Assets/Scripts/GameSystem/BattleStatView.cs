namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class BattleStatView
    {
        private HeroItem[] _heroList0;
        private HeroItem[] _heroList1;
        private GameObject _root;
        private const int HERO_MAX_NUM = 5;
        private GameObject heroView;
        private CUIListScript list;
        private bool m_battleHeroPropertyChange;
        private bool m_battleKDAChanged;
        private bool m_bListCampInited;
        private List<HeroKDA> m_heroListCamp1 = new List<HeroKDA>();
        private List<HeroKDA> m_heroListCamp2 = new List<HeroKDA>();
        private List<Player> m_playerListCamp1 = new List<Player>();
        private List<Player> m_playerListCamp2 = new List<Player>();
        private bool m_sortByCoin;
        private GameObject matchInfo;
        public static string s_battleStateViewUIForm = "UGUI/Form/Battle/Form_BattleStateView.prefab";
        private GameObject sortByCoinBtn;
        private GameObject valueInfo;

        public void Clear()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_BattleStatViewSortClick, new CUIEventManager.OnUIEventHandler(this.OnSortClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_CloseStatView, new CUIEventManager.OnUIEventHandler(this.onCloseClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleMatchInfo_InfoTypeChange, new CUIEventManager.OnUIEventHandler(this.OnBttleMatchInfoTabChange));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_KDA_CHANGED, new System.Action(this.OnBattleKDAChanged));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_HERO_PROPERTY_CHANGED, new System.Action(this.OnBattleHeroPropertyChange));
            Singleton<CUIManager>.GetInstance().CloseForm(s_battleStateViewUIForm);
            this._root = null;
            this._heroList0 = null;
            this._heroList1 = null;
            this.m_heroListCamp1.Clear();
            this.m_heroListCamp2.Clear();
            this.m_playerListCamp1.Clear();
            this.m_playerListCamp2.Clear();
            this.m_bListCampInited = false;
            this.sortByCoinBtn = null;
        }

        public void Hide()
        {
            if (null != this._root)
            {
                Singleton<CUIManager>.GetInstance().GetForm(s_battleStateViewUIForm).Hide(enFormHideFlag.HideByCustom, true);
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ReviveTimeChange, new CUIEventManager.OnUIEventHandler(this.UpdateBattleState));
                Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.onSoulLvlChange));
                Singleton<CUIParticleSystem>.instance.Show(null);
            }
        }

        public void Init()
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(s_battleStateViewUIForm) == null)
            {
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_battleStateViewUIForm, false, true);
                this._root = script.gameObject.transform.Find("BattleStatView").gameObject;
                if (this._root != null)
                {
                    this._heroList0 = new HeroItem[5];
                    this._heroList1 = new HeroItem[5];
                    this.heroView = Utility.FindChild(this._root, "HeroView");
                    this.matchInfo = Utility.FindChild(this._root, "BattleMatchInfo");
                    this.valueInfo = Utility.FindChild(this._root, "HeroValueInfo");
                    this.sortByCoinBtn = Utility.FindChild(this._root, "SortByCoin");
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        this.m_sortByCoin = PlayerPrefs.GetInt(string.Format("Sgmae_Battle_SortByCoin_{0}", masterRoleInfo.playerUllUID)) > 0;
                    }
                    this.UpdateSortBtn();
                    if (((this.heroView != null) && (this.matchInfo != null)) && (this.valueInfo != null))
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            this._heroList0[i] = new HeroItem(Utility.FindChild(this.heroView, "HeroList_0/" + i), Utility.FindChild(this.matchInfo, "HeroList_0/" + i), Utility.FindChild(this.valueInfo, "HeroList_0/" + i));
                            this._heroList1[i] = new HeroItem(Utility.FindChild(this.heroView, "HeroList_1/" + i), Utility.FindChild(this.matchInfo, "HeroList_1/" + i), Utility.FindChild(this.valueInfo, "HeroList_1/" + i));
                        }
                        GameObject gameObject = script.gameObject.transform.Find("TopCommon/Panel_Menu/ListMenu").gameObject;
                        if (gameObject != null)
                        {
                            this.list = gameObject.GetComponent<CUIListScript>();
                            string[] titleList = new string[] { Singleton<CTextManager>.GetInstance().GetText("BattleStateView_MatchInfo"), Singleton<CTextManager>.GetInstance().GetText("BattleStateView_HeroInfo") };
                            CUICommonSystem.InitMenuPanel(gameObject, titleList, 0);
                            this.heroView.CustomSetActive(true);
                            this.matchInfo.CustomSetActive(true);
                            this.valueInfo.CustomSetActive(false);
                            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_CloseStatView, new CUIEventManager.OnUIEventHandler(this.onCloseClick));
                            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleMatchInfo_InfoTypeChange, new CUIEventManager.OnUIEventHandler(this.OnBttleMatchInfoTabChange));
                            Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_KDA_CHANGED, new System.Action(this.OnBattleKDAChanged));
                            Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_HERO_PROPERTY_CHANGED, new System.Action(this.OnBattleHeroPropertyChange));
                            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_BattleStatViewSortClick, new CUIEventManager.OnUIEventHandler(this.OnSortClick));
                            this.Hide();
                        }
                    }
                }
            }
        }

        public void LateUpdate()
        {
            if (this.m_battleKDAChanged)
            {
                this.UpdateKDAView();
                this.m_battleKDAChanged = false;
            }
        }

        private void OnBattleHeroPropertyChange()
        {
            this.m_battleHeroPropertyChange = true;
        }

        private void OnBattleKDAChanged()
        {
            this.m_battleKDAChanged = true;
        }

        private void OnBttleMatchInfoTabChange(CUIEvent uiEvent)
        {
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            if (component != null)
            {
                int selectedIndex = component.GetSelectedIndex();
                if ((Singleton<CUIManager>.GetInstance().GetForm(s_battleStateViewUIForm) != null) && (((this.heroView != null) || (this.matchInfo != null)) || (this.valueInfo != null)))
                {
                    this.SortHeroAndPlayer();
                    if (selectedIndex == 0)
                    {
                        this.heroView.CustomSetActive(true);
                        this.matchInfo.CustomSetActive(true);
                        this.valueInfo.CustomSetActive(false);
                        this.UpdateKDAView();
                    }
                    else
                    {
                        this.heroView.CustomSetActive(true);
                        this.matchInfo.CustomSetActive(false);
                        this.valueInfo.CustomSetActive(true);
                        this.m_battleHeroPropertyChange = true;
                        this.UpdateBattleState(null);
                    }
                }
            }
        }

        private void onCloseClick(CUIEvent evt)
        {
            this.Hide();
        }

        private void OnSortClick(CUIEvent uievent)
        {
            this.SortByCoin = !this.SortByCoin;
        }

        private void onSoulLvlChange(PoolObjHandle<ActorRoot> act, int curVal)
        {
            if (this._root != null)
            {
                HeroItem[] itemArray = this._heroList0;
                for (int i = 0; i < itemArray.Length; i++)
                {
                    HeroItem item = itemArray[i];
                    if (((item != null) && item.Visible) && ((item.kdaData != null) && (item.kdaData.actorHero == act)))
                    {
                        item.level.text = curVal.ToString();
                    }
                    if (((i + 1) == itemArray.Length) && (itemArray == this._heroList0))
                    {
                        itemArray = this._heroList1;
                        i = -1;
                    }
                }
            }
        }

        public void RefreshVoiceStateIfNess()
        {
            if (!Singleton<CUIManager>.GetInstance().GetForm(s_battleStateViewUIForm).IsHided())
            {
                int index = 0;
                Player curPlayer = null;
                HeroItem item = null;
                for (index = 0; index < this.m_playerListCamp1.Count; index++)
                {
                    if (index >= this._heroList0.Length)
                    {
                        break;
                    }
                    curPlayer = this.m_playerListCamp1[index];
                    item = this._heroList0[index];
                    item.updateHeroVoiceState(curPlayer);
                }
                for (index = 0; index < this.m_playerListCamp2.Count; index++)
                {
                    if (index >= this._heroList1.Length)
                    {
                        break;
                    }
                    curPlayer = this.m_playerListCamp2[index];
                    this._heroList1[index].updateHeroVoiceState(curPlayer);
                }
            }
        }

        public void Show()
        {
            if (null != this._root)
            {
                Singleton<CUIManager>.GetInstance().GetForm(s_battleStateViewUIForm).Appear(enFormHideFlag.HideByCustom, true);
                if (this.list != null)
                {
                    this.list.SelectElement(0, true);
                }
                this.SortHeroAndPlayer();
                this.UpdateBattleState(null);
                this.UpdateKDAView();
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ReviveTimeChange, new CUIEventManager.OnUIEventHandler(this.UpdateBattleState));
                Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.onSoulLvlChange));
                this.RefreshVoiceStateIfNess();
                Singleton<CUIParticleSystem>.instance.Hide(null);
            }
        }

        private int SortByCoinAndPos(Player left, Player right)
        {
            PlayerKDA playerKDA = Singleton<BattleStatistic>.instance.m_playerKDAStat.GetPlayerKDA(left.PlayerId);
            return (((Singleton<BattleStatistic>.instance.m_playerKDAStat.GetPlayerKDA(right.PlayerId).TotalCoin - playerKDA.TotalCoin) * 10) + (left.CampPos - right.CampPos));
        }

        private int SortByCoinAndPos(HeroKDA left, HeroKDA right)
        {
            return (((right.TotalCoin - left.TotalCoin) * 10) + (left.CampPos - right.CampPos));
        }

        private int SortByPos(Player left, Player right)
        {
            return (left.CampPos - right.CampPos);
        }

        private int SortByPos(HeroKDA left, HeroKDA right)
        {
            return (left.CampPos - right.CampPos);
        }

        private void SortHeroAndPlayer()
        {
            bool forceUpdate = true;
            if (((this.m_heroListCamp1.Count > 0) || (this.m_heroListCamp2.Count > 0)) || ((this.m_playerListCamp1.Count > 0) || (this.m_playerListCamp2.Count > 0)))
            {
                forceUpdate = false;
            }
            this.UpdateListCamp(forceUpdate);
            if (this.m_sortByCoin)
            {
                this.m_heroListCamp1.Sort(new Comparison<HeroKDA>(this.SortByCoinAndPos));
                this.m_heroListCamp2.Sort(new Comparison<HeroKDA>(this.SortByCoinAndPos));
                this.m_playerListCamp1.Sort(new Comparison<Player>(this.SortByCoinAndPos));
                this.m_playerListCamp2.Sort(new Comparison<Player>(this.SortByCoinAndPos));
            }
            else
            {
                this.m_heroListCamp1.Sort(new Comparison<HeroKDA>(this.SortByPos));
                this.m_heroListCamp2.Sort(new Comparison<HeroKDA>(this.SortByPos));
                this.m_playerListCamp1.Sort(new Comparison<Player>(this.SortByPos));
                this.m_playerListCamp2.Sort(new Comparison<Player>(this.SortByPos));
            }
        }

        public void Toggle()
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        private void UpdateBattleState(CUIEvent evt = null)
        {
            if ((null != this._root) && !Singleton<CUIManager>.GetInstance().GetForm(s_battleStateViewUIForm).IsHided())
            {
                int index = 0;
                Player curPlayer = null;
                HeroItem item = null;
                for (index = 0; index < this.m_playerListCamp1.Count; index++)
                {
                    if (index >= this._heroList0.Length)
                    {
                        break;
                    }
                    curPlayer = this.m_playerListCamp1[index];
                    item = this._heroList0[index];
                    item.updateReviceCD(curPlayer);
                    item.updateTalentSkillCD(curPlayer);
                    if (this.m_battleHeroPropertyChange)
                    {
                        item.updateHeroValue(curPlayer);
                    }
                }
                for (index = 0; index < this.m_playerListCamp2.Count; index++)
                {
                    if (index >= this._heroList1.Length)
                    {
                        break;
                    }
                    curPlayer = this.m_playerListCamp2[index];
                    item = this._heroList1[index];
                    item.updateReviceCD(curPlayer);
                    item.updateTalentSkillCD(curPlayer);
                    if (this.m_battleHeroPropertyChange)
                    {
                        item.updateHeroValue(curPlayer);
                    }
                }
                if (this.m_battleHeroPropertyChange)
                {
                    this.m_battleHeroPropertyChange = false;
                }
            }
        }

        private void UpdateKDAView()
        {
            if (null != this._root)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_battleStateViewUIForm);
                if ((form != null) && !form.IsHided())
                {
                    int index = 0;
                    index = 0;
                    while (index < this.m_heroListCamp1.Count)
                    {
                        if (index < this._heroList0.Length)
                        {
                            this._heroList0[index].Visible = true;
                            this._heroList0[index].Validate(this.m_heroListCamp1[index]);
                        }
                        index++;
                    }
                    while (index < this._heroList0.Length)
                    {
                        this._heroList0[index].Visible = false;
                        index++;
                    }
                    index = 0;
                    index = 0;
                    while (index < this.m_heroListCamp2.Count)
                    {
                        if (index < this._heroList1.Length)
                        {
                            this._heroList1[index].Visible = true;
                            this._heroList1[index].Validate(this.m_heroListCamp2[index]);
                        }
                        index++;
                    }
                    while (index < this._heroList1.Length)
                    {
                        this._heroList1[index].Visible = false;
                        index++;
                    }
                }
            }
        }

        private void UpdateListCamp(bool forceUpdate)
        {
            if (forceUpdate || !this.m_bListCampInited)
            {
                this.m_playerListCamp1.Clear();
                this.m_playerListCamp2.Clear();
                List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
                for (int i = 0; i < allPlayers.Count; i++)
                {
                    if (allPlayers[i].PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                    {
                        this.m_playerListCamp1.Add(allPlayers[i]);
                    }
                    else if (allPlayers[i].PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                    {
                        this.m_playerListCamp2.Add(allPlayers[i]);
                    }
                }
                this.m_heroListCamp1.Clear();
                this.m_heroListCamp2.Clear();
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    PlayerKDA rkda = current.Value;
                    if (rkda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                    {
                        ListView<HeroKDA>.Enumerator enumerator2 = rkda.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            this.m_heroListCamp1.Add(enumerator2.Current);
                        }
                    }
                    else if (rkda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                    {
                        ListView<HeroKDA>.Enumerator enumerator3 = rkda.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            this.m_heroListCamp2.Add(enumerator3.Current);
                        }
                    }
                }
            }
        }

        private void UpdateSortBtn()
        {
            if (this.sortByCoinBtn != null)
            {
                Text componetInChild = Utility.GetComponetInChild<Text>(this.sortByCoinBtn, "Text");
                if (componetInChild != null)
                {
                    if (this.m_sortByCoin)
                    {
                        componetInChild.text = Singleton<CTextManager>.instance.GetText("Battle_Statistic_Sort_Coin");
                    }
                    else
                    {
                        componetInChild.text = Singleton<CTextManager>.instance.GetText("Battle_Statistic_Sort_Common");
                    }
                }
            }
        }

        public bool SortByCoin
        {
            get
            {
                return this.m_sortByCoin;
            }
            set
            {
                this.m_sortByCoin = value;
                this.m_battleHeroPropertyChange = true;
                this.UpdateSortBtn();
                this.SortHeroAndPlayer();
                this.UpdateKDAView();
                this.UpdateBattleState(null);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    PlayerPrefs.SetInt(string.Format("Sgmae_Battle_SortByCoin_{0}", masterRoleInfo.playerUllUID), !value ? 0 : 1);
                    PlayerPrefs.Save();
                    CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.SortBYCoinBtnClick);
                }
            }
        }

        public bool Visible
        {
            get
            {
                return !Singleton<CUIManager>.GetInstance().GetForm(s_battleStateViewUIForm).IsHided();
            }
        }

        private class HeroItem
        {
            public Text assistNum;
            public Text deadNum;
            public Image[] equipList = new Image[6];
            public Text heroAD;
            public Text heroADDef;
            public Text heroAP;
            public Text heroAPDef;
            public Text heroHP;
            public Text heroName;
            public Image icon;
            public HeroKDA kdaData;
            public Text killMon;
            public Text killNum;
            public Text level;
            public GameObject mineBg;
            public Text playerName;
            public Text reviveTime;
            public GameObject rootHeroView;
            public GameObject rootMatchInfo;
            public GameObject rootValueInfo;
            public GameObject talentSkill;
            public Text talentSkillCD;
            public Image talentSkillImage;
            public GameObject voiceIconsNode;

            public HeroItem(GameObject heroNode, GameObject matchNode, GameObject valueNode)
            {
                this.rootHeroView = heroNode;
                this.rootMatchInfo = matchNode;
                this.rootValueInfo = valueNode;
                this.icon = Utility.GetComponetInChild<Image>(heroNode, "HeadIcon");
                this.mineBg = Utility.FindChild(heroNode, "MineBg");
                this.level = Utility.GetComponetInChild<Text>(heroNode, "Level");
                this.playerName = Utility.GetComponetInChild<Text>(heroNode, "PlayerName");
                this.heroName = Utility.GetComponetInChild<Text>(heroNode, "HeroName");
                this.reviveTime = Utility.GetComponetInChild<Text>(heroNode, "ReviveTime");
                this.voiceIconsNode = Utility.FindChild(heroNode, "Voice");
                this.killNum = Utility.GetComponetInChild<Text>(matchNode, "KillNum");
                this.deadNum = Utility.GetComponetInChild<Text>(matchNode, "DeadNum");
                this.killMon = Utility.GetComponetInChild<Text>(matchNode, "KillMon");
                this.assistNum = Utility.GetComponetInChild<Text>(matchNode, "AssistNum");
                GameObject p = Utility.FindChild(matchNode, "TalentIcon");
                this.equipList[0] = Utility.GetComponetInChild<Image>(p, "img1");
                this.equipList[1] = Utility.GetComponetInChild<Image>(p, "img2");
                this.equipList[2] = Utility.GetComponetInChild<Image>(p, "img3");
                this.equipList[3] = Utility.GetComponetInChild<Image>(p, "img4");
                this.equipList[4] = Utility.GetComponetInChild<Image>(p, "img5");
                this.equipList[5] = Utility.GetComponetInChild<Image>(p, "img6");
                this.talentSkill = Utility.FindChild(matchNode, "TalentSkill");
                this.talentSkillImage = Utility.GetComponetInChild<Image>(this.talentSkill, "Image");
                this.talentSkillCD = Utility.GetComponetInChild<Text>(this.talentSkill, "TimeCD");
                this.heroHP = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/HP");
                this.heroAD = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/AD");
                this.heroAP = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/AP");
                this.heroADDef = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/ADDef");
                this.heroAPDef = Utility.GetComponetInChild<Text>(valueNode, "ValueInfo/APDef");
                heroNode.transform.FindChild("ReviveTime").gameObject.SetActive(true);
                matchNode.transform.FindChild("TalentSkill/TimeCD").gameObject.SetActive(true);
                this.kdaData = null;
            }

            private void _updateVoiceIcon(GameObject node, CS_VOICESTATE_TYPE value)
            {
                if (node != null)
                {
                    GameObject gameObject = node.transform.Find("AllOpen").gameObject;
                    GameObject obj3 = node.transform.Find("AllClose").gameObject;
                    GameObject obj4 = node.transform.Find("HalfOpen").gameObject;
                    switch (value)
                    {
                        case CS_VOICESTATE_TYPE.CS_VOICESTATE_NONE:
                            gameObject.CustomSetActive(false);
                            obj3.CustomSetActive(true);
                            obj4.CustomSetActive(false);
                            return;

                        case CS_VOICESTATE_TYPE.CS_VOICESTATE_PART:
                            gameObject.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(true);
                            return;

                        case CS_VOICESTATE_TYPE.CS_VOICESTATE_FULL:
                            gameObject.CustomSetActive(true);
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            return;
                    }
                }
            }

            public void updateHeroValue(Player curPlayer)
            {
                if (((curPlayer != null) && (curPlayer.Captain != 0)) && (curPlayer.Captain.handle.ValueComponent.mActorValue != null))
                {
                    ValueDataInfo[] actorValue = curPlayer.Captain.handle.ValueComponent.mActorValue.GetActorValue();
                    this.heroHP.text = string.Format("{0}", actorValue[5].totalValue);
                    this.heroAD.text = string.Format("{0}", actorValue[1].totalValue);
                    this.heroAP.text = string.Format("{0}", actorValue[2].totalValue);
                    this.heroADDef.text = string.Format("{0}", actorValue[3].totalValue);
                    this.heroAPDef.text = string.Format("{0}", actorValue[4].totalValue);
                }
            }

            public void updateHeroVoiceState(Player curPlayer)
            {
                if (((curPlayer != null) && (curPlayer.Captain != 0)) && (curPlayer.Captain.handle.ValueComponent.mActorValue != null))
                {
                    Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                    if (hostPlayer != null)
                    {
                        bool bActive = Singleton<GamePlayerCenter>.instance.IsAtSameCamp(hostPlayer.PlayerId, curPlayer.PlayerId);
                        this.voiceIconsNode.CustomSetActive(bActive);
                        if (bActive)
                        {
                            if (curPlayer.Computer && !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_isWarmBattle)
                            {
                                this.voiceIconsNode.CustomSetActive(false);
                            }
                            if (hostPlayer.PlayerUId == curPlayer.PlayerUId)
                            {
                                this._updateVoiceIcon(this.voiceIconsNode, MonoSingleton<VoiceSys>.instance.curVoiceState);
                            }
                            else
                            {
                                CS_VOICESTATE_TYPE cs_voicestate_type = MonoSingleton<VoiceSys>.GetInstance().TryGetVoiceState(curPlayer.PlayerUId);
                                this._updateVoiceIcon(this.voiceIconsNode, cs_voicestate_type);
                            }
                        }
                    }
                }
            }

            public void updateReviceCD(Player curPlayer)
            {
                if (((curPlayer != null) && (curPlayer.Captain != 0)) && (curPlayer.Captain.handle.ActorControl != null))
                {
                    if (curPlayer.Captain.handle.ActorControl.IsDeadState)
                    {
                        this.reviveTime.text = SimpleNumericString.GetNumeric(Mathf.FloorToInt(curPlayer.Captain.handle.ActorControl.ReviveCooldown * 0.001f));
                        this.icon.color = CUIUtility.s_Color_Grey;
                    }
                    else
                    {
                        this.reviveTime.text = string.Empty;
                        this.icon.color = CUIUtility.s_Color_Full;
                    }
                }
            }

            public void updateTalentSkillCD(Player curPlayer)
            {
                if (((curPlayer != null) && (curPlayer.Captain != 0)) && (curPlayer.Captain.handle.SkillControl != null))
                {
                    SkillSlot slot = curPlayer.Captain.handle.SkillControl.SkillSlotArray[5];
                    if (slot == null)
                    {
                        this.talentSkill.CustomSetActive(false);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(slot.SkillObj.IconName))
                        {
                            this.talentSkillImage.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + slot.SkillObj.IconName, Singleton<CUIManager>.GetInstance().GetForm(BattleStatView.s_battleStateViewUIForm), true, false, false);
                        }
                        this.talentSkill.CustomSetActive(true);
                        if (slot.CurSkillCD > 0)
                        {
                            this.talentSkillCD.text = SimpleNumericString.GetNumeric(Mathf.FloorToInt(((float) slot.CurSkillCD) * 0.001f));
                            this.talentSkillImage.color = CUIUtility.s_Color_Grey;
                        }
                        else
                        {
                            this.talentSkillCD.text = string.Empty;
                            this.talentSkillImage.color = CUIUtility.s_Color_Full;
                        }
                    }
                }
            }

            public void Validate(HeroKDA kdaData)
            {
                if (kdaData != null)
                {
                    this.kdaData = kdaData;
                }
                if (this.kdaData != null)
                {
                    this.icon.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint) this.kdaData.HeroId, 0)), Singleton<CBattleSystem>.instance.FormScript, true, false, false);
                    Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.kdaData.actorHero);
                    this.playerName.text = ownerPlayer.Name;
                    this.heroName.text = this.kdaData.actorHero.handle.TheStaticData.TheResInfo.Name;
                    this.level.text = this.kdaData.actorHero.handle.ValueComponent.actorSoulLevel.ToString();
                    this.killNum.text = this.kdaData.numKill.ToString();
                    this.deadNum.text = this.kdaData.numDead.ToString();
                    this.killMon.text = (this.kdaData.numKillMonster + this.kdaData.numKillSoldier).ToString();
                    this.killMon.text = this.kdaData.TotalCoin.ToString();
                    this.assistNum.text = this.kdaData.numAssist.ToString();
                    int num = 0;
                    for (int i = 0; i < 6; i++)
                    {
                        ushort equipID = this.kdaData.Equips[i].m_equipID;
                        if (equipID != 0)
                        {
                            CUICommonSystem.SetEquipIcon(equipID, this.equipList[num++].gameObject, Singleton<CBattleSystem>.instance.FormScript);
                        }
                    }
                    for (int j = num; j < 6; j++)
                    {
                        this.equipList[j].gameObject.GetComponent<Image>().SetSprite(string.Format("{0}EquipmentSpace", CUIUtility.s_Sprite_Dynamic_Talent_Dir), Singleton<CBattleSystem>.instance.FormScript, true, false, false);
                    }
                    if (ownerPlayer == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer())
                    {
                        this.playerName.color = CUIUtility.s_Text_Color_Self;
                        this.mineBg.CustomSetActive(true);
                    }
                    else
                    {
                        if (ownerPlayer.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                        {
                            this.playerName.color = CUIUtility.s_Text_Color_Camp_1;
                        }
                        else
                        {
                            this.playerName.color = CUIUtility.s_Text_Color_Camp_2;
                        }
                        this.mineBg.CustomSetActive(false);
                    }
                }
            }

            public bool Visible
            {
                get
                {
                    return ((((this.rootHeroView != null) && (this.rootMatchInfo != null)) && (this.rootValueInfo != null)) && ((this.rootHeroView.activeSelf && this.rootMatchInfo.activeSelf) && this.rootValueInfo.activeSelf));
                }
                set
                {
                    if (((this.rootHeroView != null) && (this.rootMatchInfo != null)) && (this.rootValueInfo != null))
                    {
                        this.rootHeroView.CustomSetActive(value);
                        this.rootMatchInfo.CustomSetActive(value);
                        this.rootValueInfo.CustomSetActive(value);
                    }
                }
            }
        }
    }
}

