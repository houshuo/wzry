namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class FightForm : IBattleForm
    {
        public CUIFormScript _formScript = null;
        private CUIJoystickScript _joystick;
        private uint _lastFps;
        private BattleTaskView battleTaskView;
        private GameObject BuffDesc;
        private static enOtherFloatTextContent[] energyShortageFloatText = new enOtherFloatTextContent[] { enOtherFloatTextContent.InEnergyShortage, enOtherFloatTextContent.InEnergyShortage, enOtherFloatTextContent.InEnergyShortage, enOtherFloatTextContent.InEnergyShortage, enOtherFloatTextContent.MadnessShortage };
        private HeroHeadHud heroHeadHud;
        private Text m_AdTxt;
        private Text m_ApTxt;
        private BattleMisc m_battleMisc;
        private bool m_bOpenMic;
        private bool m_bOpenSpeak;
        private int m_displayPing;
        private BattleDragonView m_dragonView;
        private Image m_EpImg;
        private HeroInfoPanel m_heroInfoPanel;
        private Image m_HpImg;
        private Text m_HpTxt;
        public bool m_isInBattle;
        private bool m_isSkillDecShow;
        private Text m_MgcDefTxt;
        private GameObject m_objHeroHead;
        private Transform m_OpeankBigMap;
        private Transform m_OpeankSpeakAnim;
        private Transform m_OpenMicObj;
        private Transform m_OpenMicTipObj;
        private Text m_OpenMicTipText;
        private Transform m_OpenSpeakerObj;
        private Transform m_OpenSpeakerTipObj;
        private Text m_OpenSpeakerTipText;
        private CanvasGroup m_panelHeroCanvas;
        private GameObject m_panelHeroInfo;
        private Text m_PhyDefTxt;
        private PoolObjHandle<ActorRoot> m_selectedHero;
        private CBattleShowBuffDesc m_showBuffDesc;
        private SignalPanel m_signalPanel;
        public CSkillButtonManager m_skillButtonManager;
        public ListView<SLOTINFO> m_SkillSlotList = new ListView<SLOTINFO>();
        private int m_Vocetimer;
        private int m_VocetimerFirst;
        private int m_VoiceMictime;
        private int m_VoiceTipsShowTime = 0x7d0;
        private string microphone_path = "UGUI/Sprite/Battle/Battle_btn_Microphone.prefab";
        private string no_microphone_path = "UGUI/Sprite/Battle/Battle_btn_No_Microphone.prefab";
        private string no_voiceIcon_path = "UGUI/Sprite/Battle/Battle_btn_No_voice.prefab";
        public static string s_battleDragonTipForm = "UGUI/Form/Battle/Form_Battle_Dragon_Tips.prefab";
        public static string s_battleUIForm = "UGUI/Form/Battle/Form_Battle.prefab";
        public ScoreBoard scoreBoard;
        private ScoreboardPvE scoreboardPvE;
        private GameObject skillTipDesc;
        private Assets.Scripts.GameSystem.SoldierWave soldierWaveView;
        private CStarEvalPanel starEvalPanel;
        private float timeEnergyShortage;
        private float timeNoSkillTarget;
        private float timeSkillCooldown;
        private CTreasureHud treasureHud;
        public Vector2 UI_world_Factor_Big;
        public Vector2 UI_world_Factor_Small;
        private string voiceIcon_path = "UGUI/Sprite/Battle/Battle_btn_voice.prefab";
        public Vector2 world_UI_Factor_Big;
        public Vector2 world_UI_Factor_Small;

        private void Battle_ActivateForm(CUIEvent uiEvent)
        {
            if (this._formScript != null)
            {
                this._formScript.Appear(enFormHideFlag.HideByCustom, true);
            }
        }

        private void BattleOpenSpeak(CUIEvent uiEvent, bool bInit = false)
        {
            if (uiEvent != null)
            {
                CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Voice_OpenSpeak);
            }
            if (this.m_OpenSpeakerTipObj != null)
            {
                if (!CFakePvPHelper.bInFakeSelect && MonoSingleton<VoiceSys>.GetInstance().IsBattleSupportVoice())
                {
                    if (!MonoSingleton<VoiceSys>.GetInstance().GlobalVoiceSetting)
                    {
                        if (!bInit)
                        {
                            if (this.m_OpenSpeakerTipText != null)
                            {
                                this.m_OpenSpeakerTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Server_Not_Open_Tips;
                            }
                            this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(true);
                            Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
                        }
                        return;
                    }
                    if (!MonoSingleton<VoiceSys>.GetInstance().IsInVoiceRoom())
                    {
                        if (this.m_OpenSpeakerTipText != null)
                        {
                            this.m_OpenSpeakerTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Cannot_JoinVoiceRoom;
                        }
                        this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(true);
                        Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
                        return;
                    }
                }
                if (this.m_bOpenSpeak)
                {
                    if (this.m_OpenSpeakerTipText != null)
                    {
                        this.m_OpenSpeakerTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_CloseSpeaker;
                    }
                    MonoSingleton<VoiceSys>.GetInstance().ClosenSpeakers();
                    MonoSingleton<VoiceSys>.GetInstance().CloseMic();
                    this.m_bOpenMic = false;
                    if (this.m_OpenSpeakerObj != null)
                    {
                        CUIUtility.SetImageSprite(this.m_OpenSpeakerObj.GetComponent<Image>(), this.no_voiceIcon_path, null, true, false, false);
                    }
                    if (this.m_OpenMicObj != null)
                    {
                        CUIUtility.SetImageSprite(this.m_OpenMicObj.GetComponent<Image>(), this.no_microphone_path, null, true, false, false);
                    }
                }
                else
                {
                    if (this.m_OpenSpeakerTipText != null)
                    {
                        this.m_OpenSpeakerTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_OpenSpeaker;
                    }
                    MonoSingleton<VoiceSys>.GetInstance().OpenSpeakers();
                    if (this.m_OpenSpeakerObj != null)
                    {
                        CUIUtility.SetImageSprite(this.m_OpenSpeakerObj.GetComponent<Image>(), this.voiceIcon_path, null, true, false, false);
                    }
                }
                this.m_bOpenSpeak = !this.m_bOpenSpeak;
                if (this.m_bOpenSpeak)
                {
                    if (!GameSettings.EnableVoice)
                    {
                        GameSettings.EnableVoice = true;
                    }
                    if (bInit)
                    {
                        if (MonoSingleton<VoiceSys>.GetInstance().UseMicOnUser)
                        {
                            this.OnBattleOpenMic(null);
                        }
                    }
                    else
                    {
                        this.OnBattleOpenMic(null);
                    }
                }
                else if (GameSettings.EnableVoice)
                {
                    GameSettings.EnableVoice = false;
                }
                this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(true);
                Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
            }
            if (this.m_OpeankSpeakAnim != null)
            {
                this.m_OpeankSpeakAnim.gameObject.CustomSetActive(false);
            }
        }

        public void BattleStart()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((this.scoreBoard != null) && this.scoreBoard.IsShown())
            {
                this.scoreBoard.RegiseterEvent();
                this.scoreBoard.Show();
            }
            if (!curLvelContext.IsMobaMode())
            {
                Utility.FindChild(this._formScript.gameObject, "HeroHeadHud").CustomSetActive(true);
                this.heroHeadHud = Utility.FindChild(this._formScript.gameObject, "HeroHeadHud").GetComponent<HeroHeadHud>();
                this.heroHeadHud.Init();
                this._formScript.GetWidget(0x48).CustomSetActive(false);
            }
            else
            {
                Utility.FindChild(this._formScript.gameObject, "HeroHeadHud").CustomSetActive(false);
                GameObject widget = this._formScript.GetWidget(0x48);
                widget.CustomSetActive(true);
                this.m_heroInfoPanel = new HeroInfoPanel();
                this.m_heroInfoPanel.Init(widget);
            }
            this.ResetSkillButtonManager(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain);
            if (this.m_skillButtonManager != null)
            {
                this.m_skillButtonManager.InitializeCampHeroInfo(this._formScript);
            }
            SLevelContext context2 = Singleton<BattleLogic>.instance.GetCurLvelContext();
            SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_5);
            SkillButton button2 = this.GetButton(SkillSlotType.SLOT_SKILL_7);
            SkillButton button3 = this.GetButton(SkillSlotType.SLOT_SKILL_6);
            if ((((context2 != null) && (button != null)) && ((button.m_button != null) && (button3 != null))) && (((button3.m_button != null) && (button2 != null)) && (button2.m_button != null)))
            {
                bool flag = false;
                if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL) && context2.IsMobaModeWithOutGuide())
                {
                    button.m_button.CustomSetActive(true);
                }
                else
                {
                    button.m_button.CustomSetActive(false);
                    SkillButton button4 = this.GetButton(SkillSlotType.SLOT_SKILL_4);
                    if ((button4 != null) && (button4.m_button != null))
                    {
                        bool flag2 = false;
                        if (curLvelContext.IsGameTypeGuide())
                        {
                            if (curLvelContext.m_mapID == CBattleGuideManager.GuideLevelID5v5)
                            {
                                flag2 = true;
                            }
                        }
                        else if (curLvelContext.IsMobaModeWithOutGuide() && (curLvelContext.m_pvpPlayerNum == 10))
                        {
                            flag2 = true;
                        }
                        if (flag2)
                        {
                            button3.m_button.transform.position = button2.m_button.transform.position;
                            button2.m_button.transform.position = button4.m_button.transform.position;
                        }
                        button4.m_button.transform.position = button.m_button.transform.position;
                        flag = true;
                    }
                }
                button2.m_button.CustomSetActive(context2.m_bEnableOrnamentSlot);
                if (!context2.m_bEnableOrnamentSlot)
                {
                    flag = true;
                    button3.m_button.transform.position = button2.m_button.transform.position;
                }
                if (flag && (this._formScript.m_sgameGraphicRaycaster != null))
                {
                    this._formScript.m_sgameGraphicRaycaster.UpdateTiles();
                }
            }
            if (this.m_OpeankBigMap != null)
            {
                MiniMapSysUT.RefreshMapPointerBig(this.m_OpeankBigMap.gameObject);
            }
            if (this.m_OpenMicObj != null)
            {
                MiniMapSysUT.RefreshMapPointerBig(this.m_OpenMicObj.gameObject);
            }
            if (this.m_OpenSpeakerObj != null)
            {
                MiniMapSysUT.RefreshMapPointerBig(this.m_OpenSpeakerObj.gameObject);
            }
        }

        public void ChangeSpeakerBtnState()
        {
            if (this.m_isInBattle && (this.m_bOpenSpeak != GameSettings.EnableVoice))
            {
                this.OnBattleOpenSpeaker(null);
            }
        }

        private void CheckAndUpdateLearnSkill(PoolObjHandle<ActorRoot> hero)
        {
            if (hero != 0)
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (Singleton<BattleLogic>.GetInstance().IsMatchLearnSkillRule(hero, (SkillSlotType) i))
                    {
                        this.UpdateLearnSkillBtnState(i, true);
                    }
                    else
                    {
                        this.UpdateLearnSkillBtnState(i, false);
                    }
                }
            }
        }

        public void ClearSkillLvlStates(int iSkillSlotType)
        {
            SkillButton button = this.GetButton((SkillSlotType) iSkillSlotType);
            if (button != null)
            {
                GameObject skillLvlImg = button.GetSkillLvlImg(1);
                if (skillLvlImg != null)
                {
                    ListView<GameObject> view = new ListView<GameObject>();
                    Transform parent = skillLvlImg.transform.parent;
                    int childCount = parent.childCount;
                    for (int i = 0; i < childCount; i++)
                    {
                        GameObject gameObject = parent.GetChild(i).gameObject;
                        if (gameObject.name.Contains("SkillLvlImg") && gameObject.activeSelf)
                        {
                            view.Add(gameObject);
                        }
                    }
                    childCount = view.Count;
                    for (int j = 0; j < childCount; j++)
                    {
                        view[j].CustomSetActive(false);
                    }
                    view.Clear();
                }
            }
        }

        public void CloseForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_battleUIForm);
        }

        public void DisableCameraDragPanelForRevive()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((this._formScript != null) && (curLvelContext != null))
            {
                GameObject obj2 = Utility.FindChild(this._formScript.gameObject, "CameraDragPanel");
                if (curLvelContext.IsMobaMode())
                {
                    obj2.CustomSetActive(false);
                }
                if ((curLvelContext.IsMobaMode() && (Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain != 0)) && (this._formScript != null))
                {
                    Transform transform = (obj2 == null) ? null : obj2.transform.Find("panelDeadInfo");
                    if (transform != null)
                    {
                        Transform transform2 = transform.Find("Timer");
                        if (transform2 != null)
                        {
                            CUITimerScript component = transform2.GetComponent<CUITimerScript>();
                            if (component != null)
                            {
                                component.EndTimer();
                            }
                        }
                        transform.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public void DisableUIEvent()
        {
            if ((this._formScript != null) && (this._formScript.gameObject != null))
            {
                GraphicRaycaster component = this._formScript.gameObject.GetComponent<GraphicRaycaster>();
                if (component != null)
                {
                    component.enabled = false;
                }
            }
        }

        public void EnableCameraDragPanelForDead()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((this._formScript != null) && (curLvelContext != null))
            {
                GameObject obj2 = Utility.FindChild(this._formScript.gameObject, "CameraDragPanel");
                if (curLvelContext.IsMobaMode() && (obj2 != null))
                {
                    obj2.CustomSetActive(true);
                }
                if (curLvelContext.IsMobaMode())
                {
                    PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                    if (((captain != 0) && (this._formScript != null)) && (obj2 != null))
                    {
                        Transform transform = obj2.transform.Find("panelDeadInfo");
                        if (transform != null)
                        {
                            Transform transform2 = transform.Find("Timer");
                            if (transform2 != null)
                            {
                                CUITimerScript component = transform2.GetComponent<CUITimerScript>();
                                if (component != null)
                                {
                                    float time = captain.handle.ActorControl.ReviveCooldown * 0.001f;
                                    component.SetTotalTime(time);
                                    component.StartTimer();
                                    this.OnReviveTimerChange(null);
                                }
                            }
                            transform.gameObject.CustomSetActive(true);
                        }
                    }
                }
            }
        }

        public BattleMisc GetBattleMisc()
        {
            return this.m_battleMisc;
        }

        public SkillButton GetButton(SkillSlotType skillSlotType)
        {
            return ((this.m_skillButtonManager == null) ? null : this.m_skillButtonManager.GetButton(skillSlotType));
        }

        public SkillSlotType GetCurSkillSlotType()
        {
            return this.m_skillButtonManager.GetCurSkillSlotType();
        }

        public int GetDisplayPing()
        {
            return this.m_displayPing;
        }

        public CUIJoystickScript GetJoystick()
        {
            return this._joystick;
        }

        public SignalPanel GetSignalPanel()
        {
            return this.m_signalPanel;
        }

        public void HideHeroInfoPanel()
        {
            if (this.m_panelHeroInfo == null)
            {
                if (this._formScript == null)
                {
                    return;
                }
                this.m_panelHeroInfo = Utility.FindChild(this._formScript.gameObject, "PanelHeroInfo");
                if (this.m_panelHeroInfo == null)
                {
                    return;
                }
            }
            if (this.m_panelHeroCanvas == null)
            {
                this.m_panelHeroCanvas = this.m_panelHeroInfo.GetComponent<CanvasGroup>();
            }
            if (this.m_panelHeroCanvas != null)
            {
                this.m_panelHeroCanvas.alpha = 0f;
            }
            if (Singleton<CBattleSelectTarget>.GetInstance() != null)
            {
                Singleton<CBattleSelectTarget>.GetInstance().CloseForm();
            }
            if (this.m_selectedHero != 0)
            {
                this.m_selectedHero.Release();
            }
        }

        public void HideSkillDescInfo()
        {
            if (this.skillTipDesc == null)
            {
                this.skillTipDesc = Utility.FindChild(this._formScript.gameObject, "Panel_SkillTip");
                if (this.skillTipDesc == null)
                {
                    return;
                }
            }
            if (this.skillTipDesc != null)
            {
                this.skillTipDesc.CustomSetActive(false);
                this.m_isSkillDecShow = false;
            }
        }

        private bool IsSkillTipsActive()
        {
            if ((this.skillTipDesc == null) && (this._formScript != null))
            {
                this.skillTipDesc = Utility.FindChild(this._formScript.gameObject, "Panel_SkillTip");
            }
            return ((this.skillTipDesc != null) && this.skillTipDesc.activeSelf);
        }

        public void LateUpdate()
        {
            if (this.m_isInBattle && (this.scoreBoard != null))
            {
                this.scoreBoard.LateUpdate();
            }
        }

        public void OnAcceptTrusteeship(CUIEvent uiEvent)
        {
            this.SendTrusteeshipResult(uiEvent.m_eventParams.commonUInt32Param1, ACCEPT_AIPLAYER_RSP.ACCEPT_AIPLAEYR_YES);
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            if (ActorHelper.IsHostCtrlActor(ref prm.src) && (this.m_skillButtonManager != null))
            {
                this.m_skillButtonManager.SkillButtonUp(this._formScript);
            }
        }

        private void OnActorGoldCoinInBattleChanged(PoolObjHandle<ActorRoot> actor, int changeValue, int currentValue, bool isIncome)
        {
            if ((Singleton<BattleLogic>.GetInstance().m_GameInfo.gameContext.levelContext.IsMobaMode() && ((actor != 0) && ActorHelper.IsHostCtrlActor(ref actor))) && (this._formScript != null))
            {
                GameObject widget = this._formScript.GetWidget(0x2e);
                if (widget != null)
                {
                    Text component = widget.GetComponent<Text>();
                    if (component != null)
                    {
                        component.text = currentValue.ToString();
                    }
                }
            }
        }

        private void OnActorHurtAbsorb(ref DefaultGameEventParam _param)
        {
            if ((_param.src != 0) && ActorHelper.IsHostActor(ref _param.src))
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Absorb, (Vector3) _param.src.handle.location, new string[0]);
            }
            else if ((_param.atker != 0) && ActorHelper.IsHostActor(ref _param.atker))
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Absorb, (Vector3) _param.src.handle.location, new string[0]);
            }
        }

        private void OnActorImmune(ref DefaultGameEventParam _param)
        {
            if ((_param.src != 0) && ActorHelper.IsHostActor(ref _param.src))
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Immunity, (Vector3) _param.src.handle.location, new string[0]);
            }
            else if ((_param.atker != 0) && ActorHelper.IsHostActor(ref _param.atker))
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Immunity, (Vector3) _param.src.handle.location, new string[0]);
            }
        }

        private void OnBattleAtkSelectHeroBtnDown(CUIEvent uiEvent)
        {
            SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
            if ((button != null) && !button.bDisableFlag)
            {
                Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_HERO);
                Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, uiEvent.m_srcWidget, uiEvent.m_srcFormScript);
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_hero", null);
            }
        }

        private void OnBattleAtkSelectSoldierBtnDown(CUIEvent uiEvent)
        {
            SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
            if ((button != null) && !button.bDisableFlag)
            {
                Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_SOLDIER);
                Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, uiEvent.m_srcWidget, uiEvent.m_srcFormScript);
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_xiaobing", null);
            }
        }

        private void OnBattleEquipBoughtEffectPlayEnd(CUIEvent uiEvent)
        {
            uiEvent.m_srcWidget.CustomSetActive(false);
        }

        private void OnBattleEquipQuicklyBuy(CUIEvent uiEvent)
        {
            if (this._formScript != null)
            {
                GameObject widget = this._formScript.GetWidget(50 + uiEvent.m_eventParams.battleEquipPar.m_indexInQuicklyBuyList);
                if (widget != null)
                {
                    widget.CustomSetActive(true);
                    CUIAnimationScript component = widget.GetComponent<CUIAnimationScript>();
                    if (component != null)
                    {
                        component.PlayAnimation("Battle_UI_ZhuangBei_01", true);
                    }
                }
                GameObject obj3 = this._formScript.GetWidget(0x34 + uiEvent.m_eventParams.battleEquipPar.m_indexInQuicklyBuyList);
                if (obj3 != null)
                {
                    obj3.CustomSetActive(true);
                    CUIAnimationScript script2 = obj3.GetComponent<CUIAnimationScript>();
                    if (script2 != null)
                    {
                        script2.PlayAnimation("Battle_UI_ZhuangBei_01", true);
                    }
                }
            }
        }

        private void OnBattleHeroSkillTipOpen(SkillSlot skillSlot, Vector3 Pos)
        {
            if (null != this._formScript)
            {
                if (this.skillTipDesc == null)
                {
                    this.skillTipDesc = Utility.FindChild(this._formScript.gameObject, "Panel_SkillTip");
                    if (this.skillTipDesc == null)
                    {
                        return;
                    }
                }
                if (skillSlot != null)
                {
                    PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                    if (captain != 0)
                    {
                        IHeroData data = CHeroDataFactory.CreateHeroData((uint) captain.handle.TheActorMeta.ConfigId);
                        Text component = this.skillTipDesc.transform.Find("skillNameText").GetComponent<Text>();
                        component.text = StringHelper.UTF8BytesToString(ref skillSlot.SkillObj.cfgData.szSkillName);
                        Text text2 = this.skillTipDesc.transform.Find("SkillDescribeText").GetComponent<Text>();
                        ValueDataInfo[] actorValue = captain.handle.ValueComponent.mActorValue.GetActorValue();
                        if ((text2 != null) && (skillSlot.SkillObj.cfgData.szSkillDesc.Length > 0))
                        {
                            text2.text = CUICommonSystem.GetSkillDesc(skillSlot.SkillObj.cfgData.szSkillDesc, actorValue, skillSlot.GetSkillLevel(), captain.handle.ValueComponent.actorSoulLevel);
                        }
                        Text text3 = this.skillTipDesc.transform.Find("SkillCDText").GetComponent<Text>();
                        string[] args = new string[] { (skillSlot.GetSkillCDMax() / 0x3e8).ToString() };
                        text3.text = Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", args);
                        string[] textArray2 = new string[] { skillSlot.NextSkillEnergyCostTotal().ToString() };
                        text3.transform.Find("SkillEnergyCostText").GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText(CUICommonSystem.GetEnergyMaxOrCostText((uint) captain.handle.ValueComponent.mActorValue.EnergyType, EnergyShowType.CostValue), textArray2);
                        uint[] skillEffectType = skillSlot.SkillObj.cfgData.SkillEffectType;
                        GameObject gameObject = null;
                        for (int i = 1; i <= 2; i++)
                        {
                            gameObject = component.transform.Find(string.Format("EffectNode{0}", i)).gameObject;
                            if ((i <= skillEffectType.Length) && (skillEffectType[i - 1] != 0))
                            {
                                gameObject.CustomSetActive(true);
                                gameObject.GetComponent<Image>().SetSprite(CSkillData.GetEffectSlotBg((SkillEffectType) skillEffectType[i - 1]), this._formScript, true, false, false);
                                gameObject.transform.Find("Text").GetComponent<Text>().text = CSkillData.GetEffectDesc((SkillEffectType) skillEffectType[i - 1]);
                            }
                            else
                            {
                                gameObject.CustomSetActive(false);
                            }
                        }
                        Vector3 vector = Pos;
                        vector.x -= 4f;
                        vector.y += 4f;
                        this.skillTipDesc.transform.position = vector;
                        this.skillTipDesc.CustomSetActive(true);
                        this.m_isSkillDecShow = true;
                    }
                }
            }
        }

        private void OnBattleLearnSkillButtonClick(CUIEvent uiEvent)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (hostPlayer != null)
            {
                PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                if ((captain != 0) && (((uiEvent != null) && (uiEvent.m_srcWidget != null)) && ((uiEvent.m_srcWidget.transform != null) && (uiEvent.m_srcWidget.transform.parent != null))))
                {
                    string name = uiEvent.m_srcWidget.transform.parent.name;
                    int index = int.Parse(name.Substring(name.Length - 1));
                    if ((index >= 1) && (index <= 3))
                    {
                        byte bSkillLvl = 0;
                        if ((captain.handle.SkillControl != null) && (captain.handle.SkillControl.SkillSlotArray[index] != null))
                        {
                            bSkillLvl = (byte) captain.handle.SkillControl.SkillSlotArray[index].GetSkillLevel();
                        }
                        this.SendLearnSkillCommand(captain, (SkillSlotType) index, bSkillLvl);
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_junei_ani_jinengxuexi", null);
                        Transform transform = uiEvent.m_srcWidget.transform.parent.Find("LearnEffect");
                        if (transform != null)
                        {
                            transform.gameObject.CustomSetActive(true);
                            CUIAnimationScript component = transform.gameObject.GetComponent<CUIAnimationScript>();
                            if (component != null)
                            {
                                component.PlayAnimation("Battle_UI_Skill_01", true);
                            }
                        }
                    }
                }
            }
        }

        private void OnBattleOpenMic(CUIEvent uiEvent)
        {
            bool flag = true;
            if (uiEvent == null)
            {
                flag = false;
            }
            if (flag)
            {
                CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Voice_OpenMic);
            }
            if (this.m_OpenMicTipObj != null)
            {
                if (!this.m_bOpenSpeak)
                {
                    if (this.m_OpenMicTipText != null)
                    {
                        this.m_OpenMicTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_FIrstOPenSpeak;
                    }
                    if (flag)
                    {
                        this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
                    }
                    Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
                }
                else
                {
                    if (!CFakePvPHelper.bInFakeSelect && MonoSingleton<VoiceSys>.GetInstance().IsBattleSupportVoice())
                    {
                        if (!MonoSingleton<VoiceSys>.GetInstance().GlobalVoiceSetting)
                        {
                            if (this.m_OpenMicTipText != null)
                            {
                                this.m_OpenMicTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Server_Not_Open_Tips;
                            }
                            if (flag)
                            {
                                this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
                            }
                            Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
                            return;
                        }
                        if (!MonoSingleton<VoiceSys>.GetInstance().IsInVoiceRoom())
                        {
                            if (this.m_OpenMicTipText != null)
                            {
                                this.m_OpenMicTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Cannot_JoinVoiceRoom;
                            }
                            if (flag)
                            {
                                this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
                            }
                            Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
                            return;
                        }
                    }
                    if (this.m_bOpenMic)
                    {
                        if (this.m_OpenMicTipText != null)
                        {
                            this.m_OpenMicTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_CloseMic;
                        }
                        MonoSingleton<VoiceSys>.GetInstance().CloseMic();
                        if (this.m_OpenMicObj != null)
                        {
                            CUIUtility.SetImageSprite(this.m_OpenMicObj.GetComponent<Image>(), this.no_microphone_path, null, true, false, false);
                        }
                    }
                    else
                    {
                        if (this.m_OpenMicTipText != null)
                        {
                            this.m_OpenMicTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_OpenMic;
                        }
                        MonoSingleton<VoiceSys>.GetInstance().OpenMic();
                        if (this.m_OpenMicObj != null)
                        {
                            CUIUtility.SetImageSprite(this.m_OpenMicObj.GetComponent<Image>(), this.microphone_path, null, true, false, false);
                        }
                    }
                    this.m_bOpenMic = !this.m_bOpenMic;
                    if (flag)
                    {
                        this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
                    }
                    Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
                }
            }
        }

        private void OnBattleOpenSpeaker(CUIEvent uiEvent)
        {
            this.BattleOpenSpeak(uiEvent, false);
        }

        public void OnBattlePauseGame(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.instance.PauseGame(this, true);
            if (this._formScript != null)
            {
                this._formScript.GetWidget(0x27).CustomSetActive(false);
                this._formScript.GetWidget(40).CustomSetActive(true);
                this._formScript.GetWidget(0x29).CustomSetActive(true);
            }
        }

        public void OnBattleResumeGame(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.instance.ResumeGame(this);
            if (this._formScript != null)
            {
                this._formScript.GetWidget(40).CustomSetActive(false);
                this._formScript.GetWidget(0x27).CustomSetActive(true);
                this._formScript.GetWidget(0x29).CustomSetActive(false);
            }
        }

        private void OnBattleSkillBtnHold(CUIEvent uiEvent)
        {
            if (!this.m_isSkillDecShow && !this.m_skillButtonManager.CurrentSkillTipsResponed)
            {
                this.OnBattleSkillDecShow(uiEvent);
            }
            else if (this.m_isSkillDecShow && this.m_skillButtonManager.CurrentSkillTipsResponed)
            {
                this.HideSkillDescInfo();
            }
        }

        private void OnBattleSkillButtonDown(CUIEvent uiEvent)
        {
            if (this.m_signalPanel != null)
            {
                this.m_signalPanel.CancelSelectedSignalButton();
            }
            stUIEventParams par = new stUIEventParams();
            par = uiEvent.m_eventParams;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Newbie_CloseSkillGesture, par);
            if (this.IsSkillTipsActive())
            {
                this.HideSkillDescInfo();
            }
            this.m_skillButtonManager.SkillButtonDown(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, uiEvent.m_pointerEventData.position);
        }

        private void OnBattleSkillButtonDragged(CUIEvent uiEvent)
        {
            this.m_skillButtonManager.DragSkillButton(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, uiEvent.m_pointerEventData.position);
        }

        private void OnBattleSkillButtonHoldEnd(CUIEvent uiEvent)
        {
            if (this.m_isSkillDecShow)
            {
                this.HideSkillDescInfo();
                this.m_skillButtonManager.SkillButtonUp(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, false, new Vector2());
            }
        }

        private void OnBattleSkillButtonUp(CUIEvent uiEvent)
        {
            this.m_skillButtonManager.SkillButtonUp(uiEvent.m_srcFormScript, uiEvent.m_eventParams.m_skillSlotType, true, uiEvent.m_pointerEventData.position);
        }

        private void OnBattleSkillDecShow(CUIEvent uiEvent)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && ((hostPlayer.Captain.handle.ActorControl == null) || !hostPlayer.Captain.handle.ActorControl.IsDeadState))
            {
                int num;
                SkillSlot slot;
                string name = uiEvent.m_srcWidget.transform.parent.name;
                Vector3 position = uiEvent.m_srcWidget.transform.parent.transform.position;
                if (!int.TryParse(name.Substring(name.Length - 1), out num))
                {
                    name = uiEvent.m_srcWidget.transform.name;
                    position = uiEvent.m_srcWidget.transform.position;
                    if (!int.TryParse(name.Substring(name.Length - 1), out num))
                    {
                        return;
                    }
                }
                SkillSlotType type = (SkillSlotType) num;
                if (hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(type, out slot))
                {
                    string str2 = Utility.UTF8Convert(slot.SkillObj.cfgData.szSkillDesc);
                    this.OnBattleHeroSkillTipOpen(slot, position);
                }
            }
        }

        private void OnBattleSkillDisableAlert(CUIEvent uiEvent)
        {
            this.m_skillButtonManager.OnBattleSkillDisableAlert(uiEvent.m_eventParams.m_skillSlotType);
        }

        private void OnBattleSkillDisableBtnDown(CUIEvent uiEvent)
        {
            this.OnBattleSkillDecShow(uiEvent);
        }

        private void OnBattleSkillDisableBtnUp(CUIEvent uiEvent)
        {
            this.HideSkillDescInfo();
        }

        private void OnBattleSkillLevelUpEffectPlayEnd(CUIEvent uiEvent)
        {
            uiEvent.m_srcWidget.CustomSetActive(false);
        }

        private void OnBigMap_Open_BigMap(CUIEvent uievent)
        {
        }

        public void OnCancelTrusteeship(CUIEvent uiEvent)
        {
            this.SendTrusteeshipResult(uiEvent.m_eventParams.commonUInt32Param1, ACCEPT_AIPLAYER_RSP.ACCEPT_AIPLAYER_NO);
        }

        private void onChangeOperateMode(CUIEvent uiEvent)
        {
        }

        private void OnClearLockTarget(ref LockTargetEventParam prm)
        {
            this.HideHeroInfoPanel();
        }

        private void OnClearTarget(ref SelectTargetEventParam prm)
        {
            this.HideHeroInfoPanel();
        }

        private void OnClickBattleScene(CUIEvent uievent)
        {
            Singleton<LockModeScreenSelector>.GetInstance().OnClickBattleScene(uievent.m_pointerEventData.position);
        }

        public void OnCloseHeorInfoPanel(CUIEvent uiEvent)
        {
            Singleton<CBattleHeroInfoPanel>.GetInstance().Hide();
        }

        private void OnCommon_BattleShowOrHideWifiInfo(CUIEvent uiEvent)
        {
            GameObject widget = this._formScript.GetWidget(0x25);
            DebugHelper.Assert(widget != null);
            if (widget != null)
            {
                widget.CustomSetActive(!widget.activeInHierarchy);
            }
        }

        private void OnCommon_BattleWifiCheckTimer(CUIEvent uiEvent)
        {
            if (this._formScript != null)
            {
                GameObject widget = this._formScript.GetWidget(0x23);
                GameObject obj3 = this._formScript.GetWidget(0x24);
                GameObject obj4 = this._formScript.GetWidget(0x25);
                DebugHelper.Assert(((widget != null) && (obj3 != null)) && (obj4 != null));
                this.m_displayPing = (GameSettings.FpsShowType != 1) ? Singleton<FrameSynchr>.GetInstance().GameSvrPing : Singleton<FrameSynchr>.instance.RealSvrPing;
                this.m_displayPing = (this.m_displayPing <= 100) ? this.m_displayPing : ((((this.m_displayPing - 100) * 7) / 10) + 100);
                this.m_displayPing = Mathf.Clamp(this.m_displayPing, 0, 460);
                uint index = 0;
                if (this.m_displayPing == 0)
                {
                    this.m_displayPing = 10;
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if (((curLvelContext != null) && curLvelContext.m_isWarmBattle) && curLvelContext.IsGameTypeComBat())
                    {
                        int num2 = UnityEngine.Random.Range(0, 10);
                        if (num2 == 0)
                        {
                            this.m_displayPing = 50 + UnityEngine.Random.Range(0, 100);
                        }
                        else if (num2 < 3)
                        {
                            this.m_displayPing = 50 + UnityEngine.Random.Range(0, 50);
                        }
                        else if (num2 < 6)
                        {
                            this.m_displayPing = 50 + UnityEngine.Random.Range(0, 30);
                        }
                        else
                        {
                            this.m_displayPing = 50 + UnityEngine.Random.Range(0, 15);
                        }
                    }
                }
                if (this.m_displayPing < 100)
                {
                    index = 2;
                }
                else if (this.m_displayPing < 200)
                {
                    index = 1;
                }
                else
                {
                    index = 0;
                }
                if (obj4 != null)
                {
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        obj4.GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_System_Wifi_Dir + CLobbySystem.s_noNetStateName, this._formScript, true, false, false);
                    }
                    else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                    {
                        obj4.GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_System_Wifi_Dir + CLobbySystem.s_wifiStateName[index], this._formScript, true, false, false);
                    }
                    else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                    {
                        obj4.GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_System_Wifi_Dir + CLobbySystem.s_netStateName[index], this._formScript, true, false, false);
                    }
                }
                if (((obj4 != null) && obj4.activeInHierarchy) && (obj3 != null))
                {
                    Text component = obj3.transform.GetComponent<Text>();
                    if (component != null)
                    {
                        component.text = string.Format("{0}ms", this.m_displayPing);
                        component.color = CLobbySystem.s_WifiStateColor[index];
                    }
                }
            }
        }

        private void onConfirmReturnLobby(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(CSettingsSys.SETTING_FORM);
            if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsGameTypeGuide() && !Singleton<CBattleGuideManager>.instance.bTrainingAdv)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Tutorial_Level_Qiut_Tip"), false, 1.5f, null, new object[0]);
            }
            else
            {
                if (Singleton<CBattleGuideManager>.instance.bPauseGame)
                {
                    Singleton<CBattleGuideManager>.instance.ResumeGame(this);
                }
                if (Singleton<LobbyLogic>.instance.inMultiGame)
                {
                    Singleton<LobbyLogic>.instance.ReqMultiGameRunaway();
                }
                else
                {
                    Singleton<BattleLogic>.instance.DoFightOver(false);
                    Singleton<BattleLogic>.instance.SingleReqLoseGame();
                }
            }
        }

        public void OnDragonTipFormClose(CUIEvent cuiEvent)
        {
            Singleton<CUIManager>.instance.CloseForm(s_battleDragonTipForm);
        }

        public void OnDragonTipFormOpen(CUIEvent cuiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.instance.OpenForm(s_battleDragonTipForm, false, true);
            Text component = Utility.FindChild(script.gameObject, "DragonBuffx1Text").GetComponent<Text>();
            Text text2 = Utility.FindChild(script.gameObject, "DragonBuffx2Text").GetComponent<Text>();
            Text text3 = Utility.FindChild(script.gameObject, "DragonBuffx3Text").GetComponent<Text>();
            ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long) Singleton<BattleLogic>.instance.GetDragonBuffId(RES_SKILL_SRC_TYPE.RES_SKILL_SRC_DRAGON_ONE));
            ResSkillCombineCfgInfo info2 = GameDataMgr.skillCombineDatabin.GetDataByKey((long) Singleton<BattleLogic>.instance.GetDragonBuffId(RES_SKILL_SRC_TYPE.RES_SKILL_SRC_DRAGON_TWO));
            ResSkillCombineCfgInfo info3 = GameDataMgr.skillCombineDatabin.GetDataByKey((long) Singleton<BattleLogic>.instance.GetDragonBuffId(RES_SKILL_SRC_TYPE.RES_SKILL_SRC_DRAGON_THREE));
            if (dataByKey != null)
            {
                component.text = Utility.UTF8Convert(dataByKey.szSkillCombineDesc);
            }
            if (info2 != null)
            {
                text2.text = Utility.UTF8Convert(info2.szSkillCombineDesc);
            }
            if (info3 != null)
            {
                text3.text = Utility.UTF8Convert(info3.szSkillCombineDesc);
            }
        }

        private void OnDropCamera(CUIEvent uiEvent)
        {
            MonoSingleton<CameraSystem>.GetInstance().MoveCamera(-uiEvent.m_pointerEventData.delta.x, -uiEvent.m_pointerEventData.delta.y);
        }

        private void OnFormClosed(CUIEvent uiEvent)
        {
            Singleton<CBattleSystem>.GetInstance().OnFormClosed();
            this.UnregisterEvents();
            Singleton<InBattleMsgMgr>.instance.Clear();
            this.m_isInBattle = false;
            this.m_bOpenSpeak = false;
            this.m_bOpenMic = false;
            MonoSingleton<VoiceSys>.GetInstance().LeaveRoom();
            Singleton<CTimerManager>.instance.RemoveTimer(this.m_Vocetimer);
            Singleton<CTimerManager>.instance.RemoveTimer(this.m_VoiceMictime);
            Singleton<CTimerManager>.instance.RemoveTimer(this.m_VocetimerFirst);
            this.m_SkillSlotList.Clear();
            if (this.scoreBoard != null)
            {
                this.scoreBoard.Clear();
                this.scoreBoard = null;
            }
            if (this.m_heroInfoPanel != null)
            {
                this.m_heroInfoPanel.Clear();
                this.m_heroInfoPanel = null;
            }
            if (this.scoreboardPvE != null)
            {
                this.scoreboardPvE.Clear();
                this.scoreboardPvE = null;
            }
            if (this.treasureHud != null)
            {
                this.treasureHud.Clear();
                this.treasureHud = null;
            }
            if (this.starEvalPanel != null)
            {
                this.starEvalPanel.Clear();
                this.starEvalPanel = null;
            }
            if (this.battleTaskView != null)
            {
                this.battleTaskView.Clear();
                this.battleTaskView = null;
            }
            if (this.soldierWaveView != null)
            {
                this.soldierWaveView.Clear();
                this.soldierWaveView = null;
            }
            if (this.heroHeadHud != null)
            {
                this.heroHeadHud.Clear();
                this.heroHeadHud = null;
            }
            if (this.m_dragonView != null)
            {
                this.m_dragonView.Clear();
                this.m_dragonView = null;
            }
            if (this.m_signalPanel != null)
            {
                this.m_signalPanel.Clear();
                this.m_signalPanel = null;
            }
            if (this.m_battleMisc != null)
            {
                this.m_battleMisc.Uninit();
                this.m_battleMisc.Clear();
                this.m_battleMisc = null;
            }
            if (this.m_skillButtonManager != null)
            {
                this.m_skillButtonManager.Clear();
                this.m_skillButtonManager = null;
            }
            this._joystick = null;
            this.m_bOpenSpeak = false;
            this.m_bOpenMic = false;
            this.m_VocetimerFirst = 0;
            this.m_Vocetimer = 0;
            this.m_VoiceMictime = 0;
            this.m_OpenSpeakerObj = null;
            this.m_OpenSpeakerTipObj = null;
            this.m_OpenSpeakerTipText = null;
            this.m_OpeankSpeakAnim = null;
            this.m_OpeankBigMap = null;
            this.m_OpenMicObj = null;
            this.m_OpenMicTipObj = null;
            this.m_OpenMicTipText = null;
            Singleton<CUIManager>.GetInstance().CloseForm(CSettingsSys.SETTING_FORM);
            Singleton<CUIManager>.GetInstance().CloseForm(string.Format("{0}{1}", "UGUI/Form/Common/", "Form_SmallMessageBox.prefab"));
            Singleton<CBattleHeroInfoPanel>.GetInstance().Hide();
            this._formScript = null;
            if (this.m_showBuffDesc != null)
            {
                this.m_showBuffDesc.UnInit();
            }
        }

        private void OnGameSettingCommonAttackTypeChange(CommonAttactType byAtkType)
        {
            if (Singleton<BattleLogic>.instance.isRuning)
            {
                this.m_skillButtonManager.SetCommonAtkBtnState(byAtkType);
            }
        }

        private void onHeroEnergyChange(PoolObjHandle<ActorRoot> actor, int curVal, int maxVal)
        {
            if (this._formScript != null)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (hostPlayer != null)
                {
                    PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                    if ((actor != 0) && (actor == captain))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int index = i + 1;
                            SkillSlot slot = actor.handle.SkillControl.SkillSlotArray[index];
                            SkillButton button = this.GetButton((SkillSlotType) index);
                            if ((slot != null) && slot.CanEnableSkillSlotByEnergy())
                            {
                                if (!slot.IsEnergyEnough)
                                {
                                    if (!button.bDisableFlag)
                                    {
                                        this.m_skillButtonManager.SetEnergyDisableButton((SkillSlotType) index);
                                    }
                                }
                                else if (button.bDisableFlag)
                                {
                                    this.m_skillButtonManager.SetEnableButton((SkillSlotType) index);
                                }
                            }
                        }
                    }
                    this.OnHeroEpChange(actor, curVal, maxVal);
                }
            }
        }

        private void onHeroEnergyMax(PoolObjHandle<ActorRoot> actor, int curVal, int maxVal)
        {
            if (this._formScript != null)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (hostPlayer != null)
                {
                    PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                    if ((actor != 0) && (actor == captain))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int index = i + 1;
                            SkillSlot slot = actor.handle.SkillControl.SkillSlotArray[index];
                            SkillButton button = this.GetButton((SkillSlotType) index);
                            if (((slot != null) && slot.CanEnableSkillSlotByEnergy()) && button.bDisableFlag)
                            {
                                this.m_skillButtonManager.SetEnableButton((SkillSlotType) index);
                            }
                        }
                    }
                }
            }
        }

        private void OnHeroEpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
        {
            if ((this.m_selectedHero != 0) && (hero == this.m_selectedHero))
            {
                this.UpdateEpInfo();
            }
        }

        private void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
        {
            if ((this.m_selectedHero != 0) && (hero == this.m_selectedHero))
            {
                this.UpdateHpInfo();
            }
        }

        private void OnHeroSkillLvlup(PoolObjHandle<ActorRoot> hero, byte bSlotType, byte bSkillLevel)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain == hero))
            {
                this.UpdateSkillLvlState(bSlotType, bSkillLevel);
                this.CheckAndUpdateLearnSkill(hero);
                if (bSkillLevel == 1)
                {
                    this.ResetSkillButtonManager(hero);
                }
            }
        }

        private void OnHeroSoulLvlChange(PoolObjHandle<ActorRoot> hero, int level)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (hostPlayer != null)
            {
                PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                if (((captain != 0) && (hero != 0)) && (((hero.handle.ActorAgent != null) && !hero.handle.ActorAgent.IsAutoAI()) && (captain == hero)))
                {
                    this.CheckAndUpdateLearnSkill(hero);
                }
            }
        }

        private void OnLockTarget(ref LockTargetEventParam prm)
        {
            Player hostPlayer = null;
            if (Singleton<GamePlayerCenter>.GetInstance().HostPlayerId != 0)
            {
                hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (hostPlayer == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
            if (hostPlayer.GetOperateMode() == OperateMode.LockMode)
            {
                this.ShowTargetInfoByTargetId(prm.lockTargetID);
            }
        }

        private void onMultiHashNotSync(CUIEvent uiEvent)
        {
            ConnectorParam connectionParam = Singleton<NetworkModule>.instance.gameSvr.GetConnectionParam();
            Singleton<GameBuilder>.instance.EndGame();
            if (connectionParam != null)
            {
                Singleton<NetworkModule>.instance.InitGameServerConnect(connectionParam);
            }
        }

        public void OnOpenHeorInfoPanel(CUIEvent uiEvent)
        {
            CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_ButtonViewSkillInfo);
            Singleton<CBattleHeroInfoPanel>.GetInstance().Show();
        }

        private void OnPlayerBlindess(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.Blindess, (Vector3) _param.src.handle.location, new string[0]);
            }
        }

        private void OnPlayerCancelLimitSkill(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.CancelLimitButton(_param.slot);
            }
        }

        private void OnPlayerEnergyShortage(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                float time = Time.time;
                if (((time - this.timeEnergyShortage) > 2f) && (_param.src.handle.ValueComponent != null))
                {
                    int energyType = (int) _param.src.handle.ValueComponent.mActorValue.EnergyType;
                    if ((energyType < 0) || (energyType >= energyShortageFloatText.Length))
                    {
                        energyType = 0;
                    }
                    Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(energyShortageFloatText[energyType], (Vector3) _param.src.handle.location, new string[0]);
                    this.timeEnergyShortage = time;
                }
            }
        }

        private void OnPlayerLimitSkill(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.SetLimitButton(_param.slot);
            }
        }

        private void OnPlayerNoSkillTarget(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                float time = Time.time;
                if ((time - this.timeNoSkillTarget) > 2f)
                {
                    Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.NoTarget, (Vector3) _param.src.handle.location, new string[0]);
                    this.timeNoSkillTarget = time;
                }
            }
        }

        private void OnPlayerProtectDisappear(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.ShieldDisappear, (Vector3) _param.src.handle.location, new string[0]);
            }
        }

        private void OnPlayerSkillCDChanged(ref DefaultSkillEventParam _param)
        {
            if ((this._formScript != null) && ((_param.actor != 0) && ActorHelper.IsHostCtrlActor(ref _param.actor)))
            {
                this.m_skillButtonManager.UpdateButtonCD(_param.slot, _param.param);
            }
        }

        private void OnPlayerSkillCDEnd(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.SetButtonCDOver(_param.slot, true);
            }
        }

        private void OnPlayerSkillCDStart(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.SetButtonCDStart(_param.slot);
            }
        }

        private void OnPlayerSkillChanged(ref ChangeSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.ChangeSkill(_param.slot, ref _param);
            }
        }

        private void OnPlayerSkillCooldown(ref ActorSkillEventParam _param)
        {
            if (_param.src != 0)
            {
                float time = Time.time;
                if ((time - this.timeSkillCooldown) > 2f)
                {
                    Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.InCooldown, (Vector3) _param.src.handle.location, new string[0]);
                    this.timeSkillCooldown = time;
                }
            }
        }

        private void OnPlayerSkillDisable(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.SetDisableButton(_param.slot);
            }
        }

        private void OnPlayerSkillEnable(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.SetEnableButton(_param.slot);
            }
        }

        private void OnPlayerSkillRecovered(ref DefaultSkillEventParam _param)
        {
            if (this._formScript != null)
            {
                this.m_skillButtonManager.RecoverSkill(_param.slot, ref _param);
            }
        }

        private void OnPlayerUpdateSkill(ref DefaultSkillEventParam _param)
        {
            if ((this._formScript != null) && (_param.slot != SkillSlotType.SLOT_SKILL_0))
            {
                PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
                this.m_skillButtonManager.Initialise(captain);
            }
        }

        public void onQuitAppClick()
        {
            SGameApplication.Quit();
        }

        private void onQuitGame(CUIEvent uiEvent)
        {
            Utility.FindChild(this._formScript.gameObject, "SysMenu").CustomSetActive(false);
            SGameApplication.Quit();
        }

        private void onReturnGame(CUIEvent uiEvent)
        {
            Utility.FindChild(this._formScript.gameObject, "SysMenu").CustomSetActive(false);
        }

        private void onReturnLobby(CUIEvent uiEvent)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (curLvelContext != null)
            {
                if (curLvelContext.IsMultilModeWithWarmBattle())
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("多人对战不能退出游戏。"), false, 1.5f, null, new object[0]);
                }
                else
                {
                    this.onConfirmReturnLobby(null);
                }
            }
        }

        private void OnReviveTimerChange(CUIEvent uiEvent)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if ((this._formScript != null) && (curLvelContext != null))
            {
                Text component = Utility.FindChild(this._formScript.gameObject, "CameraDragPanel").transform.Find("panelDeadInfo/lblReviveTime").GetComponent<Text>();
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (hostPlayer != null)
                {
                    PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                    if (captain != 0)
                    {
                        float num = captain.handle.ActorControl.ReviveCooldown * 0.001f;
                        component.text = string.Format("{0}", Mathf.FloorToInt(num + 0.2f));
                    }
                }
            }
        }

        private void OnSelectTarget(ref SelectTargetEventParam prm)
        {
            Player hostPlayer = null;
            if (Singleton<GamePlayerCenter>.GetInstance().HostPlayerId != 0)
            {
                hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (hostPlayer == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
            if (hostPlayer.GetOperateMode() == OperateMode.DefaultMode)
            {
                this.ShowTargetInfoByTargetId(prm.commonAttackTargetID);
            }
        }

        public void OnSwitchAutoAI(ref DefaultGameEventParam param)
        {
            if (((Singleton<GamePlayerCenter>.instance != null) && (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null)) && ((Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain != 0) && (param.src == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain)))
            {
                GameObject obj2 = (this._formScript == null) ? null : Utility.FindChild(this._formScript.gameObject, "PanelBtn/ToggleAutoBtn");
                if (obj2 != null)
                {
                    Transform transform = obj2.transform.Find("imgAuto");
                    if (transform != null)
                    {
                        transform.gameObject.CustomSetActive(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ActorControl.m_isAutoAI);
                        MonoSingleton<DialogueProcessor>.GetInstance().bAutoNextPage = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ActorControl.m_isAutoAI;
                    }
                }
            }
        }

        public void OnSwitchHeroInfoPanel(CUIEvent uiEvent)
        {
        }

        public void OnToggleAutoAI(CUIEvent uiEvent)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle.ActorControl != null))
            {
                FrameCommand<SwitchActorAutoAICommand> command = FrameCommandFactory.CreateFrameCommand<SwitchActorAutoAICommand>();
                command.cmdData.IsAutoAI = !hostPlayer.Captain.handle.ActorControl.m_isAutoAI ? ((sbyte) 1) : ((sbyte) 0);
                command.Send();
                uiEvent.m_srcWidget.gameObject.transform.Find("imgAuto").gameObject.CustomSetActive(!hostPlayer.Captain.handle.ActorControl.m_isAutoAI);
                MonoSingleton<DialogueProcessor>.GetInstance().bAutoNextPage = command.cmdData.IsAutoAI != 0;
            }
        }

        public void OnToggleFreeCamera(CUIEvent uiEvent)
        {
            MonoSingleton<CameraSystem>.instance.ToggleFreeCamera();
        }

        public void OnUpdateDragonUI(int delta)
        {
            if (this.m_dragonView != null)
            {
                this.m_dragonView.UpdateDragon(delta);
            }
        }

        private void onUseSkill(ref ActorSkillEventParam prm)
        {
            if (ActorHelper.IsHostCtrlActor(ref prm.src))
            {
                int configId = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.TheActorMeta.ConfigId;
                SLOTINFO item = null;
                for (int i = 0; i < this.m_SkillSlotList.Count; i++)
                {
                    if (this.m_SkillSlotList[i].id == configId)
                    {
                        item = this.m_SkillSlotList[i];
                        break;
                    }
                }
                if (item == null)
                {
                    item = new SLOTINFO {
                        id = configId
                    };
                    this.m_SkillSlotList.Add(item);
                }
                item.m_SKillSlot[(int) prm.slot]++;
            }
        }

        private void OnVoiceMicTimeEnd(int timersequence)
        {
            if (this.m_OpenMicTipObj != null)
            {
                Singleton<CTimerManager>.instance.PauseTimer(this.m_VoiceMictime);
                Singleton<CTimerManager>.instance.ResetTimer(this.m_VoiceMictime);
                this.m_OpenMicTipObj.gameObject.CustomSetActive(false);
            }
        }

        private void OnVoiceTimeEnd(int timersequence)
        {
            if (this.m_OpenSpeakerTipObj != null)
            {
                Singleton<CTimerManager>.instance.PauseTimer(this.m_Vocetimer);
                Singleton<CTimerManager>.instance.ResetTimer(this.m_Vocetimer);
                this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(false);
            }
        }

        private void OnVoiceTimeEndFirst(int timersequence)
        {
            if (this.m_OpenSpeakerTipObj != null)
            {
                Singleton<CTimerManager>.instance.PauseTimer(this.m_Vocetimer);
                Singleton<CTimerManager>.instance.ResetTimer(this.m_Vocetimer);
                this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(false);
            }
            if (this.m_OpeankSpeakAnim != null)
            {
                this.m_OpeankSpeakAnim.gameObject.CustomSetActive(false);
            }
        }

        public bool OpenForm()
        {
            this._formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_battleUIForm, false, true);
            if (null == this._formScript)
            {
                return false;
            }
            this.m_isInBattle = true;
            this.m_SkillSlotList.Clear();
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            this.m_skillButtonManager = new CSkillButtonManager();
            SGameGraphicRaycaster component = this._formScript.GetComponent<SGameGraphicRaycaster>();
            if (component != null)
            {
                DebugHelper.Assert(component.raycastMode == SGameGraphicRaycaster.RaycastMode.Sgame_tile, "Form_Battle RayCast Mode 应该设置为Sgame_tile,请检查...");
            }
            this.m_Vocetimer = Singleton<CTimerManager>.instance.AddTimer(this.m_VoiceTipsShowTime, -1, new CTimer.OnTimeUpHandler(this.OnVoiceTimeEnd));
            Singleton<CTimerManager>.instance.PauseTimer(this.m_Vocetimer);
            Singleton<CTimerManager>.instance.ResetTimer(this.m_Vocetimer);
            this.m_VoiceMictime = Singleton<CTimerManager>.instance.AddTimer(this.m_VoiceTipsShowTime, -1, new CTimer.OnTimeUpHandler(this.OnVoiceMicTimeEnd));
            Singleton<CTimerManager>.instance.PauseTimer(this.m_VoiceMictime);
            Singleton<CTimerManager>.instance.ResetTimer(this.m_VoiceMictime);
            this.m_OpenSpeakerObj = this._formScript.transform.Find("MapPanel/Mini/Voice_OpenSpeaker");
            this.m_OpenSpeakerTipObj = this._formScript.transform.Find("MapPanel/Mini/Voice_OpenSpeaker/info");
            this.m_OpeankSpeakAnim = this._formScript.transform.Find("MapPanel/Mini/Voice_OpenSpeaker/voice_anim");
            this.m_OpeankBigMap = this._formScript.transform.Find("MapPanel/Mini/Button_OpenBigMap");
            this.m_OpenMicObj = this._formScript.transform.Find("MapPanel/Mini/Voice_OpenMic");
            this.m_OpenMicTipObj = this._formScript.transform.Find("MapPanel/Mini/Voice_OpenMic/info");
            this.m_OpenSpeakerTipText = this.m_OpenSpeakerTipObj.Find("Text").GetComponent<Text>();
            if (this.m_OpeankBigMap != null)
            {
                this.m_OpeankBigMap.gameObject.CustomSetActive(true);
            }
            if (this.m_OpenMicTipObj != null)
            {
                this.m_OpenMicTipObj.gameObject.CustomSetActive(false);
                this.m_OpenMicTipText = this.m_OpenMicTipObj.Find("Text").GetComponent<Text>();
            }
            if (this.m_OpenSpeakerObj != null)
            {
                this.m_OpenSpeakerObj.gameObject.CustomSetActive(true);
            }
            if (this.m_OpenMicObj != null)
            {
                this.m_OpenMicObj.gameObject.CustomSetActive(true);
            }
            try
            {
                MonoSingleton<VoiceSys>.GetInstance().UpdateMyVoiceIcon(0);
                if (MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
                {
                    this.BattleOpenSpeak(null, true);
                }
                else
                {
                    MonoSingleton<VoiceSys>.GetInstance().ClosenSpeakers();
                    MonoSingleton<VoiceSys>.GetInstance().CloseMic();
                }
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                DebugHelper.Assert(false, "exception for closen speakers... {0} {1}", inParameters);
            }
            this.SetJoyStickMoveType(GameSettings.JoyStickMoveType);
            this.SetJoyStickShowType(GameSettings.JoyStickShowType);
            this.SetFpsShowType(GameSettings.FpsShowType);
            this.SetCameraMoveMode(GameSettings.TheCameraMoveType);
            this.treasureHud = new CTreasureHud();
            this.treasureHud.Init(Utility.FindChild(this._formScript.gameObject, "TreasurePanel"));
            this.treasureHud.Hide();
            this.starEvalPanel = new CStarEvalPanel();
            this.starEvalPanel.Init(Utility.FindChild(this._formScript.gameObject, "StarEvalPanel"));
            this.starEvalPanel.reset();
            this.m_battleMisc = new BattleMisc();
            this.m_battleMisc.Init(Utility.FindChild(this._formScript.gameObject, "mis"), this._formScript);
            this.battleTaskView = new BattleTaskView();
            this.battleTaskView.Init(Utility.FindChild(this._formScript.gameObject, "TaskView"));
            GameObject obj2 = Utility.FindChild(this._formScript.gameObject, "PanelBtn/ToggleAutoBtn");
            obj2.CustomSetActive(!curLvelContext.IsMobaMode());
            if ((Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_GUIDE) || !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().canAutoAI)
            {
                obj2.CustomSetActive(false);
            }
            GameObject widget = this._formScript.GetWidget(0x4b);
            if (widget != null)
            {
                Singleton<CReplayKitSys>.GetInstance().InitReplayKit(widget.transform, false, true);
            }
            Utility.FindChild(this._formScript.gameObject, "PanelBtn/btnViewBattleInfo").CustomSetActive(curLvelContext.IsMobaMode());
            GameObject obj5 = Utility.FindChild(this._formScript.gameObject, "panelTopRight/SignalPanel");
            this.m_signalPanel = new SignalPanel();
            this.m_signalPanel.Init(this._formScript, this._formScript.GetWidget(6), null, this._formScript.GetWidget(0x2b), curLvelContext.IsMobaMode());
            Singleton<InBattleMsgMgr>.instance.InitView(Utility.FindChild(this._formScript.gameObject, "panelTopRight/SignalPanel/Button_Chat"), this._formScript);
            if (!curLvelContext.IsMobaMode())
            {
                if (this.m_OpenSpeakerObj != null)
                {
                    this.m_OpenSpeakerObj.gameObject.CustomSetActive(false);
                }
                if (this.m_OpenMicObj != null)
                {
                    this.m_OpenMicObj.gameObject.CustomSetActive(false);
                }
                if (this.m_OpeankSpeakAnim != null)
                {
                    this.m_OpeankSpeakAnim.gameObject.CustomSetActive(false);
                }
                if (this.m_OpeankBigMap != null)
                {
                    this.m_OpeankBigMap.gameObject.CustomSetActive(false);
                }
            }
            obj5.CustomSetActive(curLvelContext.IsMobaMode());
            if (curLvelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_GUIDE)
            {
                if (this.m_OpenSpeakerObj != null)
                {
                    this.m_OpenSpeakerObj.gameObject.CustomSetActive(false);
                }
                if (this.m_OpenMicObj != null)
                {
                    this.m_OpenMicObj.gameObject.CustomSetActive(false);
                }
            }
            if (curLvelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_DEFEND)
            {
                this.soldierWaveView = new Assets.Scripts.GameSystem.SoldierWave();
                this.soldierWaveView.Init(Utility.FindChild(this._formScript.gameObject, "WaveStatistics"));
                this.soldierWaveView.Show();
            }
            GameObject obj6 = Utility.FindChild(this._formScript.gameObject, "ScoreBoard");
            GameObject obj7 = Utility.FindChild(this._formScript.gameObject, "ScoreBoardPvE");
            GameObject obj8 = Utility.FindChild(this._formScript.gameObject, "MapPanel/DragonInfo");
            if (curLvelContext.IsMobaMode())
            {
                this.scoreBoard = new ScoreBoard();
                this.scoreBoard.Init(obj6);
                this.scoreBoard.RegiseterEvent();
                this.scoreBoard.Show();
            }
            else
            {
                obj6.CustomSetActive(false);
                if (curLvelContext.IsGameTypeAdventure())
                {
                    this.scoreboardPvE = new ScoreboardPvE();
                    this.scoreboardPvE.Init(obj7);
                    this.scoreboardPvE.Show();
                }
            }
            if (Singleton<BattleLogic>.instance.m_dragonSpawn != null)
            {
                if (curLvelContext.IsMobaModeWithOutGuide() && (curLvelContext.m_pvpPlayerNum == 10))
                {
                    obj8.CustomSetActive(false);
                }
                else
                {
                    obj8.CustomSetActive(true);
                    this.m_dragonView = new BattleDragonView();
                    this.m_dragonView.Init(obj8, Singleton<BattleLogic>.instance.m_dragonSpawn);
                }
            }
            else
            {
                obj8.CustomSetActive(false);
            }
            GameObject obj9 = this._formScript.GetWidget(0x27);
            GameObject obj10 = this._formScript.GetWidget(40);
            GameObject obj11 = this._formScript.GetWidget(0x29);
            if (curLvelContext.IsMultilModeWithWarmBattle())
            {
                obj9.CustomSetActive(false);
            }
            else
            {
                obj9.CustomSetActive(true);
            }
            obj10.CustomSetActive(false);
            obj11.CustomSetActive(false);
            if (this._formScript != null)
            {
                this._formScript.Hide(enFormHideFlag.HideByCustom, true);
            }
            Singleton<InBattleMsgMgr>.instance.RegInBattleEvent();
            if (CSysDynamicBlock.bUnfinishBlock)
            {
                Utility.FindChild(this._formScript.gameObject, "panelTopRight/SignalPanel/Button_Chat").CustomSetActive(false);
            }
            this.m_showBuffDesc = new CBattleShowBuffDesc();
            this.m_showBuffDesc.Init(Utility.FindChild(this._formScript.gameObject, "BuffSkill"));
            GameObject obj13 = this._formScript.GetWidget(0x2d);
            if (obj13 != null)
            {
                obj13.CustomSetActive(curLvelContext.IsMobaMode());
            }
            GameObject obj14 = this._formScript.GetWidget(0x2e);
            if (obj14 != null)
            {
                Text text = obj14.GetComponent<Text>();
                if (text != null)
                {
                    text.text = 0.ToString();
                }
            }
            this._joystick = this._formScript.GetWidget(0x26).transform.GetComponent<CUIJoystickScript>();
            this.RegisterEvents();
            return true;
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddMesh(CUIParticleSystem.s_particleSkillBtnEffect_Path);
        }

        private void RegisterEvents()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_MultiHashInvalid, new CUIEventManager.OnUIEventHandler(this.onMultiHashNotSync));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ActivateForm, new CUIEventManager.OnUIEventHandler(this.Battle_ActivateForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCloseForm, new CUIEventManager.OnUIEventHandler(this.OnFormClosed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OpenSysMenu, new CUIEventManager.OnUIEventHandler(this.ShowSysMenu));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onReturnLobby));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ConfirmSysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onConfirmReturnLobby));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SysQuitGame, new CUIEventManager.OnUIEventHandler(this.onQuitGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SysReturnGame, new CUIEventManager.OnUIEventHandler(this.onReturnGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnOpMode, new CUIEventManager.OnUIEventHandler(this.onChangeOperateMode));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SwitchAutoAI, new CUIEventManager.OnUIEventHandler(this.OnToggleAutoAI));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ChgFreeCamera, new CUIEventManager.OnUIEventHandler(this.OnToggleFreeCamera));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_HeroInfoSwitch, new CUIEventManager.OnUIEventHandler(this.OnSwitchHeroInfoPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_HeroInfoPanelOpen, new CUIEventManager.OnUIEventHandler(this.OnOpenHeorInfoPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_HeroInfoPanelClose, new CUIEventManager.OnUIEventHandler(this.OnCloseHeorInfoPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_FPSAndLagUpdate, new CUIEventManager.OnUIEventHandler(this.UpdateFpsAndLag));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnOpenDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnCloseDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Disable_Alert, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableAlert));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonDragged, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDragged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Disable_Down, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Disable_Up, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonHold, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillBtnHold));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonHoldEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonHoldEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAtkSelectHeroDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectHeroBtnDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAtkSelectSoldierDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectSoldierBtnDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_LearnSkillBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleLearnSkillButtonClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_SkillLevelUpEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillLevelUpEffectPlayEnd));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.onHeroEnergyChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyMax", new Action<PoolObjHandle<ActorRoot>, int, int>(this.onHeroEnergyMax));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_BattleWifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleWifiCheckTimer));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_BattleShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleShowOrHideWifiInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnDropCamera));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_RevivieTimer, new CUIEventManager.OnUIEventHandler(this.OnReviveTimerChange));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ChangeSkillEventParam>(GameSkillEventDef.Event_ChangeSkill, new GameSkillEvent<ChangeSkillEventParam>(this.OnPlayerSkillChanged));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_RecoverSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillRecovered));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDChanged));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDStart, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDStart));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDEnd));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillEnable));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_DisableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillDisable));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_ProtectDisappear, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerProtectDisappear));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_SkillCooldown, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerSkillCooldown));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Enent_EnergyShortage, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerEnergyShortage));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_NoSkillTarget, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerNoSkillTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_Blindess, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerBlindess));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_LimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerLimitSkill));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_CancelLimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerCancelLimitSkill));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerUpdateSkill));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorImmune, new RefAction<DefaultGameEventParam>(this.OnActorImmune));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorHurtAbsorb, new RefAction<DefaultGameEventParam>(this.OnActorHurtAbsorb));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, new RefAction<DefaultGameEventParam>(this.OnSwitchAutoAI));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onUseSkill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Pause_Game, new CUIEventManager.OnUIEventHandler(this.OnBattlePauseGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Resume_Game, new CUIEventManager.OnUIEventHandler(this.OnBattleResumeGame));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_RecommendEquip_Buy, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipQuicklyBuy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_EquipBoughtEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBoughtEffectPlayEnd));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnActorGoldCoinInBattleChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Trusteeship_Accept, new CUIEventManager.OnUIEventHandler(this.OnAcceptTrusteeship));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Trusteeship_Cancel, new CUIEventManager.OnUIEventHandler(this.OnCancelTrusteeship));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnHeroSkillLvlup));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_OpenSpeaker, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenSpeaker));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VOICE_OpenMic, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenMic));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CommonAttactType>(EventID.GAME_SETTING_COMMONATTACK_TYPE_CHANGE, new Action<CommonAttactType>(this.OnGameSettingCommonAttackTypeChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
            this.SetCommonAttackTargetEvent();
        }

        public void ResetHostPlayerSkillIndicatorSensitivity()
        {
            if (Singleton<BattleLogic>.GetInstance().isFighting)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (hostPlayer != null)
                {
                    PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
                    if ((captain != 0) && (captain.handle.SkillControl != null))
                    {
                        float spd = 0f;
                        float angularSpd = 0f;
                        GameSettings.GetLunPanSensitivity(out spd, out angularSpd);
                        for (int i = 0; i < captain.handle.SkillControl.SkillSlotArray.Length; i++)
                        {
                            SkillSlot slot = captain.handle.SkillControl.SkillSlotArray[i];
                            if ((slot != null) && (slot.skillIndicator != null))
                            {
                                slot.skillIndicator.SetIndicatorSpeed(spd, angularSpd);
                            }
                        }
                    }
                }
            }
        }

        public void ResetSkillButtonManager(PoolObjHandle<ActorRoot> actor)
        {
            if ((actor.handle != null) && (this.m_skillButtonManager != null))
            {
                this.m_skillButtonManager.Initialise(actor);
                Singleton<EventRouter>.GetInstance().BroadCastEvent("ResetSkillButtonManager");
            }
        }

        private void SendLearnSkillCommand(PoolObjHandle<ActorRoot> actor, SkillSlotType enmSkillSlotType, byte bSkillLvl)
        {
            FrameCommand<LearnSkillCommand> command = FrameCommandFactory.CreateFrameCommand<LearnSkillCommand>();
            command.cmdData.dwHeroID = actor.handle.ObjID;
            command.cmdData.bSkillLevel = bSkillLvl;
            command.cmdData.bSlotType = (byte) enmSkillSlotType;
            command.Send();
        }

        public void SendTrusteeshipResult(uint objID, ACCEPT_AIPLAYER_RSP trusteeshipRsp)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x415);
            msg.stPkgData.stIsAcceptAiPlayerRsp.dwAiPlayerObjID = objID;
            msg.stPkgData.stIsAcceptAiPlayerRsp.bResult = (byte) trusteeshipRsp;
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
        }

        public void SetButtonHighLight(GameObject button, bool highLight)
        {
            this.m_skillButtonManager.SetButtonHighLight(button, highLight);
        }

        public void SetCameraMoveMode(CameraMoveType cameraMoveType)
        {
            if (this._formScript != null)
            {
                bool bActive = false;
                bool flag2 = false;
                switch (cameraMoveType)
                {
                    case CameraMoveType.Close:
                        bActive = false;
                        flag2 = false;
                        break;

                    case CameraMoveType.JoyStick:
                        bActive = true;
                        flag2 = false;
                        break;

                    case CameraMoveType.Slide:
                        bActive = false;
                        flag2 = true;
                        break;
                }
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext != null) && !curLvelContext.m_isCanRightJoyStickCameraDrag)
                {
                    bActive = false;
                    flag2 = false;
                }
                GameObject widget = this._formScript.GetWidget(0x49);
                GameObject obj3 = this._formScript.GetWidget(0x4a);
                if (widget != null)
                {
                    widget.CustomSetActive(bActive);
                }
                if (obj3 != null)
                {
                    obj3.CustomSetActive(flag2);
                }
            }
        }

        public void SetCommonAttackTargetEvent()
        {
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnSelectTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnClearTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnLockTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnClearLockTarget));
        }

        public void SetDragonUINum(COM_PLAYERCAMP camp, byte num)
        {
            if (this.m_dragonView != null)
            {
                this.m_dragonView.SetDrgonNum(camp, num);
            }
        }

        public void SetFpsShowType(int showType)
        {
            if (this._formScript != null)
            {
                bool bActive = showType == 1;
                this._formScript.GetWidget(0x20).CustomSetActive(bActive);
                GameObject widget = this._formScript.GetWidget(0x2c);
                if (widget != null)
                {
                    CUITimerScript component = widget.GetComponent<CUITimerScript>();
                    if (component != null)
                    {
                        component.m_onChangedIntervalTime = !bActive ? ((double) 2f) : ((double) 0.3f);
                    }
                }
            }
        }

        public void SetJoyStickMoveType(int moveType)
        {
            if (this._formScript != null)
            {
                CUIJoystickScript component = this._formScript.GetWidget(0x26).transform.GetComponent<CUIJoystickScript>();
                if (component != null)
                {
                    if ((moveType == 0) || CCheatSystem.IsJoystickForceMoveable())
                    {
                        component.m_isAxisMoveable = true;
                    }
                    else
                    {
                        component.m_isAxisMoveable = false;
                    }
                }
            }
        }

        public void SetJoyStickShowType(int showType)
        {
            if (this._formScript != null)
            {
                if (showType == 0)
                {
                    this.m_skillButtonManager.SetSkillIndicatorMode(enSkillIndicatorMode.FixedPosition);
                }
                else
                {
                    this.m_skillButtonManager.SetSkillIndicatorMode(enSkillIndicatorMode.General);
                }
            }
        }

        public void SetLearnBtnHighLight(GameObject button, bool highLight)
        {
            this.m_skillButtonManager.SetlearnBtnHighLight(button, highLight);
        }

        private void SetSelectedHeroInfo(PoolObjHandle<ActorRoot> hero)
        {
            if (this.m_panelHeroInfo == null)
            {
                this.m_panelHeroInfo = Utility.FindChild(this._formScript.gameObject, "PanelHeroInfo");
            }
            if ((this.m_panelHeroInfo != null) && (hero != 0))
            {
                if (this.m_objHeroHead == null)
                {
                    this.m_objHeroHead = Utility.FindChild(this.m_panelHeroInfo, "HeroHead");
                }
                if (this.m_objHeroHead != null)
                {
                    uint configId = (uint) hero.handle.TheActorMeta.ConfigId;
                    KillDetailInfo detail = new KillDetailInfo {
                        Killer = hero
                    };
                    KillInfo info2 = KillNotifyUT.Convert_DetailInfo_KillInfo(detail);
                    this.m_objHeroHead.transform.Find("imageIcon").GetComponent<Image>().SetSprite(info2.KillerImgSrc, this._formScript, true, false, false);
                }
                Singleton<CBattleSelectTarget>.GetInstance().OpenForm(hero);
            }
        }

        public void ShowArenaTimer()
        {
            GameObject obj2 = Utility.FindChild(this._formScript.gameObject, "ArenaTimer63s");
            if (obj2 != null)
            {
                Transform transform = obj2.transform.Find("Timer");
                if (transform != null)
                {
                    CUITimerScript component = transform.GetComponent<CUITimerScript>();
                    if (component != null)
                    {
                        component.ReStartTimer();
                    }
                }
                obj2.CustomSetActive(true);
            }
        }

        public void ShowHeroInfoPanel(PoolObjHandle<ActorRoot> hero)
        {
            if (this.m_panelHeroInfo == null)
            {
                this.m_panelHeroInfo = Utility.FindChild(this._formScript.gameObject, "PanelHeroInfo");
                if (this.m_panelHeroInfo != null)
                {
                    this.m_panelHeroInfo.CustomSetActive(true);
                }
            }
            if (this.m_panelHeroCanvas == null)
            {
                this.m_panelHeroCanvas = this.m_panelHeroInfo.GetComponent<CanvasGroup>();
            }
            if (((this.m_panelHeroInfo != null) && (this.m_panelHeroCanvas != null)) && ((hero != 0) && (hero.handle.ValueComponent != null)))
            {
                this.m_selectedHero = hero;
                this.SetSelectedHeroInfo(hero);
                this.m_panelHeroCanvas.alpha = 1f;
            }
        }

        private void ShowSkillDescInfo(string strSkillDesc, Vector3 Pos)
        {
            if (this.BuffDesc == null)
            {
                this.BuffDesc = Utility.FindChild(this._formScript.gameObject, "SkillDesc");
                if (this.BuffDesc == null)
                {
                    return;
                }
            }
            GameObject obj2 = Utility.FindChild(this.BuffDesc, "Text");
            if (obj2 != null)
            {
                Text component = obj2.GetComponent<Text>();
                component.text = strSkillDesc;
                float preferredHeight = component.preferredHeight;
                Image componetInChild = Utility.GetComponetInChild<Image>(this.BuffDesc, "bg");
                if (componetInChild != null)
                {
                    Vector2 sizeDelta = componetInChild.rectTransform.sizeDelta;
                    preferredHeight += ((componetInChild.gameObject.transform.localPosition.y - component.gameObject.transform.localPosition.y) * 2f) + 10f;
                    componetInChild.rectTransform.sizeDelta = new Vector2(sizeDelta.x, preferredHeight);
                    RectTransform transform = this.BuffDesc.GetComponent<RectTransform>();
                    if (transform != null)
                    {
                        transform.sizeDelta = componetInChild.rectTransform.sizeDelta;
                    }
                    Vector3 vector2 = Pos;
                    vector2.x -= 4f;
                    vector2.y += 4f;
                    this.BuffDesc.transform.position = vector2;
                    this.BuffDesc.CustomSetActive(true);
                    this.m_isSkillDecShow = true;
                }
            }
        }

        public void ShowSysMenu(CUIEvent uiEvent)
        {
            Singleton<CUIParticleSystem>.instance.Hide(null);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Settings_OpenForm);
        }

        private void ShowTargetInfoByTargetId(uint uilockTargetID)
        {
            uint objID = uilockTargetID;
            if (Singleton<GameObjMgr>.instance != null)
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(objID);
                if (actor != 0)
                {
                    this.ShowHeroInfoPanel(actor);
                }
            }
        }

        public void ShowTaskView(bool show)
        {
            if (this.battleTaskView != null)
            {
                this.battleTaskView.Visible = show;
            }
        }

        public void ShowVoiceTips()
        {
            if (this.m_OpenSpeakerTipObj != null)
            {
                if (!MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
                {
                    this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(true);
                    if (this.m_OpenSpeakerTipText != null)
                    {
                        this.m_OpenSpeakerTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_FirstTips;
                    }
                    if (this.m_OpeankSpeakAnim != null)
                    {
                        this.m_OpeankSpeakAnim.gameObject.CustomSetActive(true);
                    }
                    this.m_VocetimerFirst = Singleton<CTimerManager>.instance.AddTimer(0x2710, 1, new CTimer.OnTimeUpHandler(this.OnVoiceTimeEndFirst));
                }
                else
                {
                    this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(false);
                    if (this.m_OpeankSpeakAnim != null)
                    {
                        this.m_OpeankSpeakAnim.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public void ShowWinLosePanel(bool bWin)
        {
            Singleton<WinLose>.GetInstance().ShowPanel(bWin);
        }

        private void UnRegisteredCommonAttackTargetEvent()
        {
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnSelectTarget));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnClearTarget));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnLockTarget));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnClearLockTarget));
        }

        private void UnregisterEvents()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_MultiHashInvalid, new CUIEventManager.OnUIEventHandler(this.onMultiHashNotSync));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ActivateForm, new CUIEventManager.OnUIEventHandler(this.Battle_ActivateForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCloseForm, new CUIEventManager.OnUIEventHandler(this.OnFormClosed));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OpenSysMenu, new CUIEventManager.OnUIEventHandler(this.ShowSysMenu));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onReturnLobby));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ConfirmSysReturnLobby, new CUIEventManager.OnUIEventHandler(this.onConfirmReturnLobby));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SysQuitGame, new CUIEventManager.OnUIEventHandler(this.onQuitGame));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SysReturnGame, new CUIEventManager.OnUIEventHandler(this.onReturnGame));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnOpMode, new CUIEventManager.OnUIEventHandler(this.onChangeOperateMode));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SwitchAutoAI, new CUIEventManager.OnUIEventHandler(this.OnToggleAutoAI));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ChgFreeCamera, new CUIEventManager.OnUIEventHandler(this.OnToggleFreeCamera));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_HeroInfoSwitch, new CUIEventManager.OnUIEventHandler(this.OnSwitchHeroInfoPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_HeroInfoPanelOpen, new CUIEventManager.OnUIEventHandler(this.OnOpenHeorInfoPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_HeroInfoPanelClose, new CUIEventManager.OnUIEventHandler(this.OnCloseHeorInfoPanel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_FPSAndLagUpdate, new CUIEventManager.OnUIEventHandler(this.UpdateFpsAndLag));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnOpenDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormOpen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnCloseDragonTip, new CUIEventManager.OnUIEventHandler(this.OnDragonTipFormClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Disable_Alert, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableAlert));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonDragged, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonDragged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Disable_Down, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Disable_Up, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillDisableBtnUp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonHold, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillBtnHold));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonHoldEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillButtonHoldEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAtkSelectHeroDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectHeroBtnDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAtkSelectSoldierDown, new CUIEventManager.OnUIEventHandler(this.OnBattleAtkSelectSoldierBtnDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_LearnSkillBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleLearnSkillButtonClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_SkillLevelUpEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillLevelUpEffectPlayEnd));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnHeroSkillLvlup));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.onHeroEnergyChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyMax", new Action<PoolObjHandle<ActorRoot>, int, int>(this.onHeroEnergyMax));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_BattleWifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleWifiCheckTimer));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_BattleShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.OnCommon_BattleShowOrHideWifiInfo));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnDropCamera));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_RevivieTimer, new CUIEventManager.OnUIEventHandler(this.OnReviveTimerChange));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ChangeSkillEventParam>(GameSkillEventDef.Event_ChangeSkill, new GameSkillEvent<ChangeSkillEventParam>(this.OnPlayerSkillChanged));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_RecoverSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillRecovered));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDChanged));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDStart, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDStart));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillCDEnd));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillEnable));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_DisableSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerSkillDisable));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_ProtectDisappear, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerProtectDisappear));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_SkillCooldown, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerSkillCooldown));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Enent_EnergyShortage, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerEnergyShortage));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.Event_NoSkillTarget, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerNoSkillTarget));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_Blindess, new GameSkillEvent<ActorSkillEventParam>(this.OnPlayerBlindess));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_LimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerLimitSkill));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_CancelLimitSkill, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerCancelLimitSkill));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.Event_UpdateSkillUI, new GameSkillEvent<DefaultSkillEventParam>(this.OnPlayerUpdateSkill));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorImmune, new RefAction<DefaultGameEventParam>(this.OnActorImmune));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorHurtAbsorb, new RefAction<DefaultGameEventParam>(this.OnActorHurtAbsorb));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, new RefAction<DefaultGameEventParam>(this.OnSwitchAutoAI));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, new GameSkillEvent<ActorSkillEventParam>(this.onUseSkill));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Pause_Game, new CUIEventManager.OnUIEventHandler(this.OnBattlePauseGame));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Resume_Game, new CUIEventManager.OnUIEventHandler(this.OnBattleResumeGame));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_RecommendEquip_Buy, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipQuicklyBuy));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_EquipBoughtEffectPlayEnd, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBoughtEffectPlayEnd));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnActorGoldCoinInBattleChanged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Trusteeship_Accept, new CUIEventManager.OnUIEventHandler(this.OnAcceptTrusteeship));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Trusteeship_Cancel, new CUIEventManager.OnUIEventHandler(this.OnCancelTrusteeship));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.VOICE_OpenSpeaker, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenSpeaker));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.VOICE_OpenMic, new CUIEventManager.OnUIEventHandler(this.OnBattleOpenMic));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CommonAttactType>(EventID.GAME_SETTING_COMMONATTACK_TYPE_CHANGE, new Action<CommonAttactType>(this.OnGameSettingCommonAttackTypeChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
            this.UnRegisteredCommonAttackTargetEvent();
        }

        public void Update()
        {
            if (this.m_isInBattle)
            {
                if (this.scoreBoard != null)
                {
                    this.scoreBoard.Update();
                }
                if (this.m_heroInfoPanel != null)
                {
                    this.m_heroInfoPanel.Update();
                }
                if (this.scoreboardPvE != null)
                {
                    this.scoreboardPvE.Update();
                }
                if (this.soldierWaveView != null)
                {
                    this.soldierWaveView.Update();
                }
                if (this.m_signalPanel != null)
                {
                    this.m_signalPanel.Update();
                }
                if (Singleton<InBattleMsgMgr>.instance != null)
                {
                    Singleton<InBattleMsgMgr>.instance.Update();
                }
                if (Singleton<CBattleSystem>.instance.TheMinimapSys != null)
                {
                    Singleton<CBattleSystem>.instance.TheMinimapSys.Update();
                }
            }
        }

        public void UpdateAdValueInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].totalValue;
                if (this.m_AdTxt == null)
                {
                    GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/AdTxt");
                    if (obj2 != null)
                    {
                        this.m_AdTxt = obj2.GetComponent<Text>();
                    }
                }
                if (this.m_AdTxt != null)
                {
                    this.m_AdTxt.text = totalValue.ToString();
                }
            }
        }

        public void UpdateApValueInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT].totalValue;
                if (this.m_ApTxt == null)
                {
                    GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/ApTxt");
                    if (obj2 != null)
                    {
                        this.m_ApTxt = obj2.GetComponent<Text>();
                    }
                }
                if (this.m_ApTxt != null)
                {
                    this.m_ApTxt.text = totalValue.ToString();
                }
            }
        }

        public void UpdateEpInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int actorEp = this.m_selectedHero.handle.ValueComponent.actorEp;
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue;
                if (this.m_EpImg == null)
                {
                    GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "EpImg");
                    if (obj2 != null)
                    {
                        this.m_EpImg = obj2.GetComponent<Image>();
                    }
                }
                if (this.m_EpImg != null)
                {
                    float num3 = 0f;
                    if (totalValue > 0)
                    {
                        num3 = ((float) actorEp) / ((float) totalValue);
                    }
                    this.m_EpImg.CustomFillAmount(num3);
                }
            }
        }

        protected void UpdateFpsAndLag(CUIEvent uiEvent)
        {
            if (this._formScript != null)
            {
                GameObject widget = this._formScript.GetWidget(0x20);
                uint fFps = (uint) GameFramework.m_fFps;
                if (widget != null)
                {
                    Text component = widget.transform.FindChild("FPSText").gameObject.GetComponent<Text>();
                    component.text = string.Format("FPS {0}", fFps);
                    if (CheatCommandCommonEntry.CPU_CLOCK_ENABLE)
                    {
                        component.text = string.Format("FPS {0}\n{1}Mhz-{2}Mhz", fFps, Utility.GetCpuCurrentClock(), Utility.GetCpuMinClock());
                    }
                }
                this._lastFps = fFps;
            }
        }

        public void UpdateHpInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int actorHp = this.m_selectedHero.handle.ValueComponent.actorHp;
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                if (this.m_HpImg == null)
                {
                    GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "HpImg");
                    if (obj2 != null)
                    {
                        this.m_HpImg = obj2.GetComponent<Image>();
                    }
                }
                if (this.m_HpImg != null)
                {
                    this.m_HpImg.CustomFillAmount(((float) actorHp) / ((float) totalValue));
                }
                if (this.m_HpTxt == null)
                {
                    GameObject obj3 = Utility.FindChild(this.m_panelHeroInfo, "HpTxt");
                    if (obj3 != null)
                    {
                        this.m_HpTxt = obj3.GetComponent<Text>();
                    }
                }
                if (this.m_HpTxt != null)
                {
                    string str = string.Format("{0}/{1}", actorHp, totalValue);
                    this.m_HpTxt.text = str;
                }
            }
        }

        private void UpdateLearnSkillBtnState(int iSkillSlotType, bool bIsShow)
        {
            SkillButton button = this.GetButton((SkillSlotType) iSkillSlotType);
            if (button != null)
            {
                GameObject learnSkillButton = button.GetLearnSkillButton();
                if (learnSkillButton != null)
                {
                    learnSkillButton.CustomSetActive(bIsShow);
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<int, bool>("HeroSkillLearnButtonStateChange", iSkillSlotType, bIsShow);
                }
            }
        }

        public void UpdateLogic(int delta)
        {
            if (this.m_isInBattle)
            {
                this.m_skillButtonManager.UpdateLogic(delta);
                if (this.m_showBuffDesc != null)
                {
                    this.m_showBuffDesc.UpdateBuffCD(delta);
                }
                Singleton<CBattleSelectTarget>.GetInstance().Update(this.m_selectedHero);
            }
        }

        public void UpdateMgcDefValueInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue;
                GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/MgcDefTxt");
                if (obj2 != null)
                {
                    this.m_MgcDefTxt = obj2.GetComponent<Text>();
                }
                if (this.m_MgcDefTxt != null)
                {
                    this.m_MgcDefTxt.text = totalValue.ToString();
                }
            }
        }

        public void UpdatePhyDefValueInfo()
        {
            if (this.m_selectedHero != 0)
            {
                int totalValue = this.m_selectedHero.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue;
                if (this.m_PhyDefTxt == null)
                {
                    GameObject obj2 = Utility.FindChild(this.m_panelHeroInfo, "InfoPanel/TxtPanel/PhyDefTxt");
                    if (obj2 != null)
                    {
                        this.m_PhyDefTxt = obj2.GetComponent<Text>();
                    }
                }
                if (this.m_PhyDefTxt != null)
                {
                    this.m_PhyDefTxt.text = totalValue.ToString();
                }
            }
        }

        private void UpdateSkillLvlState(int iSkillSlotType, int iSkillLvl)
        {
            SkillButton button = this.GetButton((SkillSlotType) iSkillSlotType);
            if (button != null)
            {
                GameObject skillLvlImg = button.GetSkillLvlImg(iSkillLvl);
                if (skillLvlImg != null)
                {
                    skillLvlImg.CustomSetActive(true);
                }
            }
        }

        public CUIFormScript FormScript
        {
            get
            {
                return this._formScript;
            }
        }

        public bool OpenSpeakInBattle
        {
            get
            {
                return this.m_bOpenSpeak;
            }
        }

        public CUIContainerScript TextHudContainer
        {
            get
            {
                if (this._formScript != null)
                {
                    GameObject widget = this._formScript.GetWidget(0x18);
                    if (widget != null)
                    {
                        return widget.GetComponent<CUIContainerScript>();
                    }
                }
                return null;
            }
        }
    }
}

