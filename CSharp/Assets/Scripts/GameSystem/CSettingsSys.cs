namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CSettingsSys : Singleton<CSettingsSys>
    {
        private SettingType[] _availableSettingTypes;
        private int _availableSettingTypesCnt;
        private CUIFormScript _form;
        private CUIListScript _tabList;
        private readonly string PLAYCEHOLDER = "---";
        public static string SETTING_FORM = "UGUI/Form/System/Settings/Form_Settings.prefab";

        private void ChangeImageColor(Transform imageTransform, Color color)
        {
            if (imageTransform != null)
            {
                Image component = imageTransform.GetComponent<Image>();
                if (component != null)
                {
                    component.color = color;
                }
            }
        }

        private void ChangeText(CUIListElementScript element, string InText)
        {
            DebugHelper.Assert(element != null);
            if (element != null)
            {
                Transform transform = element.gameObject.transform;
                DebugHelper.Assert(transform != null);
                Transform transform2 = transform.FindChild("Text");
                DebugHelper.Assert(transform2 != null);
                Text text = (transform2 == null) ? null : transform2.GetComponent<Text>();
                DebugHelper.Assert(text != null);
                if (text != null)
                {
                    text.text = InText;
                }
            }
        }

        private CUISliderEventScript GetSliderBarScript(GameObject bar)
        {
            Transform transform = bar.transform.FindChild("Slider");
            if (transform != null)
            {
                return transform.GetComponent<CUISliderEventScript>();
            }
            return bar.GetComponent<CUISliderEventScript>();
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OpenForm, new CUIEventManager.OnUIEventHandler(this.onOpenSetting));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ReqLogout, new CUIEventManager.OnUIEventHandler(this.onReqLogout));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ConfirmLogout, new CUIEventManager.OnUIEventHandler(this.onConfirmLogout));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_SettingTypeChange, new CUIEventManager.OnUIEventHandler(this.OnSettingTabChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSetting));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OpenNetworkAccelerator, new CUIEventManager.OnUIEventHandler(this.OnNetAccChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_AutomaticOpenNetworkAccelerator, new CUIEventManager.OnUIEventHandler(this.OnAutoNetAccChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_PrivacyPolicy, new CUIEventManager.OnUIEventHandler(this.OnClickPrivacyPolicy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_TermOfService, new CUIEventManager.OnUIEventHandler(this.OnClickTermOfService));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Contract, new CUIEventManager.OnUIEventHandler(this.OnClickContract));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_UpdateTimer, new CUIEventManager.OnUIEventHandler(this.OnUpdateTimer));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_SurrenderCDReady, new CUIEventManager.OnUIEventHandler(this.OnSurrenderCDReady));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ConfirmQuality_Accept, new CUIEventManager.OnUIEventHandler(this.onQualitySettingAccept));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ConfirmQuality_Cancel, new CUIEventManager.OnUIEventHandler(this.onQualitySettingCancel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ClickMoveCameraGuide, new CUIEventManager.OnUIEventHandler(this.onClickMoveCameraGuide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ClickSkillCancleTypeHelp, new CUIEventManager.OnUIEventHandler(this.onClickSkillCancleTypeHelp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnSmartCastChange, new CUIEventManager.OnUIEventHandler(this.OnSmartCastChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnLunPanCastChange, new CUIEventManager.OnUIEventHandler(this.OnLunPanCastChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnPickNearestChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnPickNearestChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnPickMinHpChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnPickMinHpChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnCommonAttackType1Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnCommonAttackType1Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnCommonAttackType2Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnCommonAttackType2Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnSkillCanleType1Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnSkillCanleType1Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnSkillCanleType2Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnSkillCanleType2Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnJoyStickMoveChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnJoyStickMoveChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnJoyStickNoMoveChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnJoyStickNoMoveChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnRightJoyStickBtnLocChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnRightJoyStickBtnLocChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_OnRightJoyStickFingerLocChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnRightJoyStickFingerLocChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onLunpanSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onModelLODChange, new CUIEventManager.OnUIEventHandler(this.OnModeLODChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onParticleLODChange, new CUIEventManager.OnUIEventHandler(this.OnParticleLODChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onSkillTipChange, new CUIEventManager.OnUIEventHandler(this.OnSkillTipChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onFpsChange, new CUIEventManager.OnUIEventHandler(this.OnFpsChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onInputChatChange, new CUIEventManager.OnUIEventHandler(this.OnInputChatChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onMoveCameraChange, new CUIEventManager.OnUIEventHandler(this.OnMoveCameraChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onHDBarChange, new CUIEventManager.OnUIEventHandler(this.OnHDBarChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_CameraHeight, new CUIEventManager.OnUIEventHandler(this.OnCameraHeightChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onCameraSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnCameraSensitivityChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onMusicChange, new CUIEventManager.OnUIEventHandler(this.OnMiusicChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onSoundEffectChange, new CUIEventManager.OnUIEventHandler(this.OnSoundEffectChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onVoiceChange, new CUIEventManager.OnUIEventHandler(this.OnVoiceChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onVibrateChange, new CUIEventManager.OnUIEventHandler(this.OnVibrateChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onMusicSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeMusic));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onSoundSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeSound));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onVoiceSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeVoice));
            this._availableSettingTypes = new SettingType[5];
            this._availableSettingTypesCnt = 0;
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_ReplayKitCourse, new CUIEventManager.OnUIEventHandler(this.OnClickOBFormOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Settings_Slider_onRecordKingTimeEnableChange, new CUIEventManager.OnUIEventHandler(this.OnRecordKingTimeEnableChange));
        }

        private void InitRecorderWidget()
        {
            GameObject obj2 = this._form.m_formWidgets[0x39];
            if (obj2 != null)
            {
                if (!Singleton<CRecordUseSDK>.instance.GetRecorderGlobalCfgEnableFlag())
                {
                    obj2.CustomSetActive(false);
                }
                else
                {
                    Transform transform = obj2.transform.FindChild("Text");
                    if (transform != null)
                    {
                        Text component = transform.gameObject.GetComponent<Text>();
                        if (component != null)
                        {
                            component.text = Singleton<CTextManager>.GetInstance().GetText("RecordKingTimeName");
                        }
                    }
                    Transform transform2 = obj2.transform.FindChild("Desc");
                    if (transform2 != null)
                    {
                        Text text2 = transform2.gameObject.GetComponent<Text>();
                        if (text2 != null)
                        {
                            text2.text = Singleton<CTextManager>.GetInstance().GetText("RecordKingTimeDesc");
                        }
                    }
                    CUISliderEventScript sliderBarScript = this.GetSliderBarScript(obj2);
                    int num = !GameSettings.EnableKingTimeMode ? 0 : 1;
                    if ((sliderBarScript != null) && (((int) sliderBarScript.value) != num))
                    {
                        sliderBarScript.value = num;
                    }
                    obj2.CustomSetActive(true);
                }
            }
        }

        private void InitVoiceSetting()
        {
            this.ShowSoundSettingLevel(GameSettings.EnableMusic, null, SettingFormWidget.MusicLevel);
            this.ShowSoundSettingLevel(GameSettings.EnableSound, null, SettingFormWidget.SoundEffectLevel);
            this.ShowSoundSettingLevel(GameSettings.EnableVoice, null, SettingFormWidget.VoiceEffectLevel);
        }

        private void InitWidget(string formPath)
        {
            this._form = Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
            bool isRuning = Singleton<BattleLogic>.instance.isRuning;
            this._form.m_formWidgets[0x37].CustomSetActive(!isRuning);
            this._form.m_formWidgets[0x38].CustomSetActive(isRuning);
            if (isRuning)
            {
                this._tabList = this._form.m_formWidgets[0x12].GetComponent<CUIListScript>();
            }
            else
            {
                this._tabList = this._form.m_formWidgets[3].GetComponent<CUIListScript>();
            }
            DebugHelper.Assert(this._tabList != null);
            this.SetAvailableTabs();
            DebugHelper.Assert(this._availableSettingTypesCnt != 0, "Available Setting Type Array's Length Is 0 ?!");
            this._tabList.SetElementAmount(this._availableSettingTypesCnt);
            for (int i = 0; i < this._availableSettingTypesCnt; i++)
            {
                SettingType type = this._availableSettingTypes[i];
                CUIListElementScript elemenet = this._tabList.GetElemenet(i);
                switch (type)
                {
                    case SettingType.Basic:
                        this.ChangeText(elemenet, "基础设置");
                        Singleton<CMiShuSystem>.GetInstance().ShowNewFlag(elemenet.gameObject, enNewFlagKey.New_BasicSettingTab_V1);
                        break;

                    case SettingType.Operation:
                        this.ChangeText(elemenet, "操作设置");
                        break;

                    case SettingType.VoiceSetting:
                        this.ChangeText(elemenet, "音效设置");
                        break;

                    case SettingType.NetAcc:
                        this.ChangeText(elemenet, "网络加速");
                        break;

                    case SettingType.ReplayKit:
                        this.ChangeText(elemenet, "录像设置");
                        Singleton<CMiShuSystem>.GetInstance().ShowNewFlag(elemenet.gameObject, enNewFlagKey.New_ReplayKitTab_V2);
                        break;
                }
            }
            this._tabList.SelectElement(0, true);
            if (isRuning)
            {
                this._form.m_formWidgets[0x36].CustomSetActive(false);
                this._form.m_formWidgets[0x35].CustomSetActive(true);
                this.GetSliderBarScript(this._form.m_formWidgets[0x16]).value = GameSettings.FpsShowType;
                this.GetSliderBarScript(this._form.m_formWidgets[0x2a]).value = (float) GameSettings.TheCameraMoveType;
                this.UpdateCameraSensitivitySlider();
                GameObject widget = this._form.GetWidget(0x1a);
                if (widget == null)
                {
                    return;
                }
                if (Singleton<CSurrenderSystem>.instance.CanSurrender())
                {
                    widget.CustomSetActive(true);
                    GameObject p = Utility.FindChild(widget, "Button_Surrender");
                    if (p == null)
                    {
                        return;
                    }
                    Button component = p.GetComponent<Button>();
                    if (component == null)
                    {
                        return;
                    }
                    GameObject obj4 = Utility.FindChild(p, "CountDown");
                    if (obj4 == null)
                    {
                        return;
                    }
                    CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(obj4, "timerSurrender");
                    if (componetInChild == null)
                    {
                        return;
                    }
                    uint time = 0;
                    if (Singleton<CSurrenderSystem>.instance.InSurrenderCD(out time))
                    {
                        obj4.CustomSetActive(true);
                        CUICommonSystem.SetButtonEnable(component, false, false, true);
                        componetInChild.SetTotalTime((float) time);
                        componetInChild.ReStartTimer();
                    }
                    else
                    {
                        obj4.CustomSetActive(false);
                        CUICommonSystem.SetButtonEnable(component, true, true, true);
                    }
                }
                else
                {
                    widget.CustomSetActive(false);
                }
            }
            else
            {
                this._form.m_formWidgets[0x36].CustomSetActive(true);
                this._form.m_formWidgets[0x35].CustomSetActive(false);
                this.SetHDBarShow();
                CUISliderEventScript sliderBarScript = this.GetSliderBarScript(this._form.m_formWidgets[6]);
                CUISliderEventScript script4 = this.GetSliderBarScript(this._form.m_formWidgets[7]);
                sliderBarScript.value = sliderBarScript.MaxValue - GameSettings.ModelLOD;
                script4.value = script4.MaxValue - GameSettings.ParticleLOD;
                this.GetSliderBarScript(this._form.m_formWidgets[0x31]).value = !GameSettings.EnableHDMode ? ((float) 0) : ((float) 1);
                Text text = this._form.m_formWidgets[9].transform.FindChild("Text").gameObject.GetComponent<Text>();
                ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
                if (accountInfo != null)
                {
                    if (accountInfo.Platform == ApolloPlatform.QQ)
                    {
                        text.text = Singleton<CTextManager>.GetInstance().GetText("Common_Login_QQ");
                    }
                    else if (accountInfo.Platform == ApolloPlatform.Wechat)
                    {
                        text.text = Singleton<CTextManager>.GetInstance().GetText("Common_Login_Weixin");
                    }
                    else if (accountInfo.Platform == ApolloPlatform.WTLogin)
                    {
                        text.text = Singleton<CTextManager>.GetInstance().GetText("Common_Login_PC");
                    }
                    else if (accountInfo.Platform == ApolloPlatform.Guest)
                    {
                        text.text = Singleton<CTextManager>.GetInstance().GetText("Common_Login_Guest");
                    }
                }
            }
            this.GetSliderBarScript(this._form.m_formWidgets[0x10]).value = GameSettings.CameraHeight;
            this.GetSliderBarScript(this._form.m_formWidgets[8]).value = !GameSettings.EnableOutline ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[0x26]).value = GameSettings.InBattleInputChatEnable;
            if (GameSettings.TheCastType == CastType.LunPanCast)
            {
                this._form.m_formWidgets[11].GetComponent<Toggle>().isOn = false;
                this._form.m_formWidgets[12].GetComponent<Toggle>().isOn = true;
                this.LunPanSettingsStatusChange(true);
            }
            else
            {
                this._form.m_formWidgets[11].GetComponent<Toggle>().isOn = true;
                this._form.m_formWidgets[12].GetComponent<Toggle>().isOn = false;
                this.LunPanSettingsStatusChange(false);
            }
            this._form.m_formWidgets[14].GetComponent<Toggle>().isOn = GameSettings.TheSelectType == SelectEnemyType.SelectNearest;
            this._form.m_formWidgets[15].GetComponent<Toggle>().isOn = GameSettings.TheSelectType == SelectEnemyType.SelectLowHp;
            this._form.m_formWidgets[0x18].GetComponent<Toggle>().isOn = GameSettings.TheCommonAttackType == CommonAttactType.Type1;
            this._form.m_formWidgets[0x19].GetComponent<Toggle>().isOn = GameSettings.TheCommonAttackType == CommonAttactType.Type2;
            this._form.m_formWidgets[20].GetComponent<Toggle>().isOn = GameSettings.JoyStickShowType == 0;
            this._form.m_formWidgets[0x15].GetComponent<Toggle>().isOn = GameSettings.JoyStickShowType == 1;
            this._form.m_formWidgets[0x27].GetComponent<Toggle>().isOn = GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle;
            this._form.m_formWidgets[40].GetComponent<Toggle>().isOn = GameSettings.TheSkillCancleType == SkillCancleType.DisitanceCancle;
            this.GetSliderBarScript(this._form.m_formWidgets[13]).value = GameSettings.LunPanSensitivity;
            this.GetSliderBarScript(this._form.m_formWidgets[0x1c]).value = GameSettings.MusicEffectLevel * 0.01f;
            this.GetSliderBarScript(this._form.m_formWidgets[0x1d]).value = GameSettings.SoundEffectLevel * 0.01f;
            this.GetSliderBarScript(this._form.m_formWidgets[30]).value = GameSettings.VoiceEffectLevel * 0.01f;
            this.GetSliderBarScript(this._form.m_formWidgets[4]).value = !GameSettings.EnableMusic ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[5]).value = !GameSettings.EnableSound ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[0x17]).value = !GameSettings.EnableVoice ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[0x29]).value = !GameSettings.EnableVibrate ? ((float) 0) : ((float) 1);
            this.InitVoiceSetting();
            this.GetSliderBarScript(this._form.m_formWidgets[0x23]).value = !NetworkAccelerator.IsNetAccConfigOpen() ? ((float) 0) : ((float) 1);
            this.GetSliderBarScript(this._form.m_formWidgets[0x24]).value = !NetworkAccelerator.IsAutoNetAccConfigOpen() ? ((float) 0) : ((float) 1);
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                Transform transform = this._form.transform.FindChild("BasicSetting/HelpMe");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(false);
                }
            }
        }

        private void LunPanSettingsStatusChange(bool bEnable)
        {
            this.SliderStatusChange(bEnable, null, SettingFormWidget.OpLunPanSensi);
            this.ToggleStatusChange(bEnable, SettingFormWidget.RightJoyStickBtnLoc);
            this.ToggleStatusChange(bEnable, SettingFormWidget.RightJoyStickFingerLoc);
        }

        private void OnAutoNetAccChange(CUIEvent uiEvent)
        {
            int sliderValue = (int) uiEvent.m_eventParams.sliderValue;
            int num2 = !NetworkAccelerator.IsAutoNetAccConfigOpen() ? 0 : 1;
            NetworkAccelerator.SetAutoNetAccConfig(sliderValue > 0);
            if ((num2 != sliderValue) && NetworkAccelerator.IsAutoNetAccConfigOpen())
            {
                this.GetSliderBarScript(this._form.m_formWidgets[0x23]).value = 1f;
            }
        }

        private void OnCameraHeightChange(CUIEvent uiEvent)
        {
            GameSettings.CameraHeight = Convert.ToInt32(uiEvent.m_eventParams.sliderValue);
        }

        private void OnCameraSensitivityChange(CUIEvent uiEvent)
        {
            GameSettings.SetCurCameraSensitivity(uiEvent.m_eventParams.sliderValue);
            this.UpdateCameraSensitivitySlider();
        }

        private void OnClickContract(CUIEvent uiEvent)
        {
            CUICommonSystem.OpenUrl("http://game.qq.com/contract.shtml", false);
        }

        private void onClickMoveCameraGuide(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(1, null);
        }

        private void OnClickOBFormOpen(CUIEvent uiEvent)
        {
            if (this._form != null)
            {
                Singleton<CUIManager>.instance.CloseForm(this._form);
            }
            Singleton<COBSystem>.instance.OnOBFormOpen(uiEvent);
        }

        private void OnClickPrivacyPolicy(CUIEvent uiEvent)
        {
            CUICommonSystem.OpenUrl("http://www.tencent.com/en-us/zc/privacypolicy.shtml", false);
        }

        private void onClickReplayKitHelp(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(8, null);
        }

        private void onClickSkillCancleTypeHelp(CUIEvent uiEvent)
        {
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(6, null);
        }

        private void OnClickTermOfService(CUIEvent uiEvent)
        {
            CUICommonSystem.OpenUrl("http://www.tencent.com/en-us/zc/termsofservice.shtml", false);
        }

        protected void OnCloseSetting(CUIEvent uiEvent)
        {
            this._availableSettingTypesCnt = 0;
            this.UnInitWidget();
            GameSettings.Save();
        }

        private static void OnCommonAttackType1Change(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheCommonAttackType = CommonAttactType.Type1;
            }
        }

        private static void OnCommonAttackType2Change(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                sliderMoveCameraAdjustment();
                GameSettings.TheCommonAttackType = CommonAttactType.Type2;
            }
        }

        private void onConfirmLogout(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x3f8);
            msg.stPkgData.stGameLogoutReq.iLogoutType = 0;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void OnFpsChange(CUIEvent uiEvent)
        {
            GameSettings.FpsShowType = (int) uiEvent.m_eventParams.sliderValue;
        }

        private void OnHDBarChange(CUIEvent uiEvent)
        {
            GameSettings.EnableHDMode = uiEvent.m_eventParams.sliderValue == 1f;
        }

        private void OnInputChatChange(CUIEvent uiEvent)
        {
            GameSettings.InBattleInputChatEnable = (int) uiEvent.m_eventParams.sliderValue;
        }

        private static void OnJoyStickMoveChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.JoyStickMoveType = 0;
            }
        }

        private static void OnJoyStickNoMoveChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.JoyStickMoveType = 1;
            }
        }

        private void OnLunPanCastChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                this.LunPanSettingsStatusChange(true);
                GameSettings.TheCastType = CastType.LunPanCast;
            }
        }

        private void OnMiusicChange(CUIEvent uiEvent)
        {
            GameSettings.EnableMusic = uiEvent.m_eventParams.sliderValue == 1f;
            this.ShowSoundSettingLevel(GameSettings.EnableMusic, null, SettingFormWidget.MusicLevel);
        }

        private void OnModeLODChange(CUIEvent uiEvent)
        {
            int num = Convert.ToInt32(uiEvent.m_eventParams.sliderValue);
            if ((((uiEvent.m_srcWidgetScript as CUISliderEventScript).MaxValue - num) < GameSettings.ModelLOD) && (PlayerPrefs.GetInt("degrade", 0) == 1))
            {
                stUIEventParams par = new stUIEventParams {
                    tag = 0,
                    tag2 = num
                };
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Setting_Quality_Confirm"), enUIEventID.Settings_ConfirmQuality_Accept, enUIEventID.Settings_ConfirmQuality_Cancel, par, false);
            }
            else
            {
                CUISliderEventScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUISliderEventScript;
                if (srcWidgetScript != null)
                {
                    GameSettings.ModelLOD = (uiEvent.m_srcWidgetScript as CUISliderEventScript).MaxValue - num;
                }
            }
        }

        private void OnMoveCameraChange(CUIEvent uiEvent)
        {
            int sliderValue = (int) uiEvent.m_eventParams.sliderValue;
            if ((GameSettings.TheCommonAttackType == CommonAttactType.Type2) && (sliderValue == 1))
            {
                sliderValue = 2;
            }
            GameSettings.TheCameraMoveType = (CameraMoveType) sliderValue;
            this.UpdateCameraSensitivitySlider();
        }

        private void OnNetAccChange(CUIEvent uiEvent)
        {
            NetworkAccelerator.SetNetAccConfig(Convert.ToInt32(uiEvent.m_eventParams.sliderValue) > 0);
        }

        private void onOpenSetting(CUIEvent uiEvent)
        {
            this.InitWidget(SETTING_FORM);
        }

        private void OnParticleLODChange(CUIEvent uiEvent)
        {
            int num = Convert.ToInt32(uiEvent.m_eventParams.sliderValue);
            if ((((uiEvent.m_srcWidgetScript as CUISliderEventScript).MaxValue - num) < GameSettings.ParticleLOD) && (PlayerPrefs.GetInt("degrade", 0) == 1))
            {
                stUIEventParams par = new stUIEventParams {
                    tag = 1,
                    tag2 = num
                };
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Setting_Quality_Confirm"), enUIEventID.Settings_ConfirmQuality_Accept, enUIEventID.Settings_ConfirmQuality_Cancel, par, false);
            }
            else
            {
                CUISliderEventScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUISliderEventScript;
                if (srcWidgetScript != null)
                {
                    GameSettings.ParticleLOD = (uiEvent.m_srcWidgetScript as CUISliderEventScript).MaxValue - num;
                }
            }
        }

        private static void OnPickMinHpChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheSelectType = SelectEnemyType.SelectLowHp;
            }
        }

        private static void OnPickNearestChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheSelectType = SelectEnemyType.SelectNearest;
            }
        }

        private void onQualitySettingAccept(CUIEvent uiEvent)
        {
            stUIEventParams eventParams = uiEvent.m_eventParams;
            switch (eventParams.tag)
            {
                case 0:
                    GameSettings.ModelLOD = this.GetSliderBarScript(this._form.m_formWidgets[6]).MaxValue - eventParams.tag2;
                    break;

                case 1:
                    GameSettings.ParticleLOD = this.GetSliderBarScript(this._form.m_formWidgets[7]).MaxValue - eventParams.tag2;
                    break;

                case 2:
                    GameSettings.EnableOutline = eventParams.tag2 != 0;
                    break;
            }
            PlayerPrefs.SetInt("degrade", 0);
            PlayerPrefs.Save();
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Force_Modify_Quality", null, true);
        }

        private void onQualitySettingCancel(CUIEvent uiEvent)
        {
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Give_Up_Modify_Quality", null, true);
            CUISliderEventScript sliderBarScript = this.GetSliderBarScript(this._form.m_formWidgets[6]);
            CUISliderEventScript script2 = this.GetSliderBarScript(this._form.m_formWidgets[7]);
            sliderBarScript.value = sliderBarScript.MaxValue - GameSettings.ModelLOD;
            script2.value = script2.MaxValue - GameSettings.ParticleLOD;
            this.GetSliderBarScript(this._form.m_formWidgets[8]).value = !GameSettings.EnableOutline ? ((float) 0) : ((float) 1);
        }

        private void OnRecordKingTimeEnableChange(CUIEvent uiEvent)
        {
            bool flag = uiEvent.m_eventParams.sliderValue == 1f;
            if (flag)
            {
                if (!Singleton<CRecordUseSDK>.instance.OpenRecorderCheck(this._form.m_formWidgets[0x39]))
                {
                    flag = false;
                    CUISliderEventScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUISliderEventScript;
                    if (srcWidgetScript != null)
                    {
                        srcWidgetScript.value = 0f;
                    }
                }
                else
                {
                    Singleton<CTaskSys>.instance.Increse(5);
                }
            }
            GameSettings.EnableKingTimeMode = flag;
        }

        private void OnReplayKitEnableAutoModeChange(CUIEvent uiEvent)
        {
            if (((uiEvent.m_eventParams.sliderValue != 1f) ? 0 : 1) != 0)
            {
                if (!Singleton<CReplayKitSys>.GetInstance().Cap || !GameSettings.EnableReplayKit)
                {
                    this.GetSliderBarScript(this._form.m_formWidgets[0x2d]).value = 0f;
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Enable_First"), false, 1.5f, null, new object[0]);
                    return;
                }
                Singleton<CReplayKitSys>.GetInstance().CheckStorage(true);
            }
            GameSettings.EnableReplayKitAutoMode = uiEvent.m_eventParams.sliderValue == 1f;
        }

        private void OnReplayKitEnableChange(CUIEvent uiEvent)
        {
            bool flag = uiEvent.m_eventParams.sliderValue == 1f;
            if (flag)
            {
                CUISliderEventScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUISliderEventScript;
                if (!Singleton<CReplayKitSys>.GetInstance().Cap)
                {
                    srcWidgetScript.value = 0f;
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Not_Support"), false, 1.5f, null, new object[0]);
                    return;
                }
                if (Singleton<CReplayKitSys>.GetInstance().CheckStorage(true) == CReplayKitSys.StorageStatus.Disable)
                {
                    srcWidgetScript.value = 0f;
                    return;
                }
            }
            GameSettings.EnableReplayKit = flag;
            if (!GameSettings.EnableReplayKit)
            {
                this.GetSliderBarScript(this._form.m_formWidgets[0x2d]).value = 0f;
            }
        }

        private void onReqLogout(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Common_Exit_Tip"), enUIEventID.Settings_ConfirmLogout, enUIEventID.None, false);
        }

        private static void OnRightJoyStickBtnLocChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.JoyStickShowType = 0;
            }
        }

        private static void OnRightJoyStickFingerLocChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.JoyStickShowType = 1;
            }
        }

        private void OnSensitivityChange(CUIEvent uiEvent)
        {
            GameSettings.LunPanSensitivity = uiEvent.m_eventParams.sliderValue;
        }

        private void OnSensitivityChangeMusic(CUIEvent uiEvent)
        {
            GameSettings.MusicEffectLevel = uiEvent.m_eventParams.sliderValue * 100f;
        }

        private void OnSensitivityChangeSound(CUIEvent uiEvent)
        {
            GameSettings.SoundEffectLevel = uiEvent.m_eventParams.sliderValue * 100f;
        }

        private void OnSensitivityChangeVoice(CUIEvent uiEvent)
        {
            GameSettings.VoiceEffectLevel = uiEvent.m_eventParams.sliderValue * 100f;
        }

        protected void OnSettingTabChange(CUIEvent uiEvent)
        {
            if ((this._form != null) && (this._tabList != null))
            {
                int selectedIndex = this._tabList.GetSelectedIndex();
                if ((selectedIndex >= 0) && (selectedIndex < this._availableSettingTypesCnt))
                {
                    switch (this._availableSettingTypes[selectedIndex])
                    {
                        case SettingType.Basic:
                            this._form.m_formWidgets[1].CustomSetActive(true);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            break;

                        case SettingType.Operation:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(true);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            break;

                        case SettingType.VoiceSetting:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(true);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            break;

                        case SettingType.NetAcc:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(true);
                            this._form.m_formWidgets[0x2b].CustomSetActive(false);
                            break;

                        case SettingType.ReplayKit:
                            this._form.m_formWidgets[1].CustomSetActive(false);
                            this._form.m_formWidgets[2].CustomSetActive(false);
                            this._form.m_formWidgets[0x1b].CustomSetActive(false);
                            this._form.m_formWidgets[0x25].CustomSetActive(false);
                            this._form.m_formWidgets[0x2b].CustomSetActive(true);
                            this.InitRecorderWidget();
                            Singleton<CMiShuSystem>.GetInstance().HideNewFlag(this._tabList.GetElemenet(selectedIndex).gameObject, enNewFlagKey.New_ReplayKitTab_V2);
                            if (CSysDynamicBlock.bLobbyEntryBlocked)
                            {
                                Transform transform = this._form.m_formWidgets[0x2b].transform.FindChild("Button_Course");
                                if (transform != null)
                                {
                                    transform.gameObject.CustomSetActive(false);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private static void OnSkillCanleType1Change(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheSkillCancleType = SkillCancleType.AreaCancle;
            }
        }

        private static void OnSkillCanleType2Change(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                GameSettings.TheSkillCancleType = SkillCancleType.DisitanceCancle;
            }
        }

        private void OnSkillTipChange(CUIEvent uiEvent)
        {
            int sliderValue = (int) uiEvent.m_eventParams.sliderValue;
            if (((sliderValue != 0) && !GameSettings.EnableOutline) && (PlayerPrefs.GetInt("degrade", 0) == 1))
            {
                stUIEventParams par = new stUIEventParams {
                    tag = 2,
                    tag2 = sliderValue
                };
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Setting_Quality_Confirm"), enUIEventID.Settings_ConfirmQuality_Accept, enUIEventID.Settings_ConfirmQuality_Cancel, par, false);
            }
            else
            {
                GameSettings.EnableOutline = sliderValue != 0;
            }
        }

        private void OnSmartCastChange(CUIEvent uiEvent)
        {
            if (uiEvent.m_eventParams.togleIsOn)
            {
                this.LunPanSettingsStatusChange(false);
                GameSettings.TheCastType = CastType.SmartCast;
            }
        }

        private void OnSoundEffectChange(CUIEvent uiEvent)
        {
            GameSettings.EnableSound = uiEvent.m_eventParams.sliderValue == 1f;
            this.ShowSoundSettingLevel(GameSettings.EnableSound, null, SettingFormWidget.SoundEffectLevel);
        }

        private void OnSurrenderCDReady(CUIEvent uiEvent)
        {
            if (Singleton<BattleLogic>.instance.isRuning && (this._form != null))
            {
                GameObject widget = this._form.GetWidget(0x1a);
                if (widget != null)
                {
                    GameObject p = Utility.FindChild(widget, "Button_Surrender");
                    if (p != null)
                    {
                        Button component = p.GetComponent<Button>();
                        if (component != null)
                        {
                            GameObject obj4 = Utility.FindChild(p, "CountDown");
                            if (obj4 != null)
                            {
                                obj4.CustomSetActive(false);
                                CUICommonSystem.SetButtonEnable(component, true, true, true);
                            }
                        }
                    }
                }
            }
        }

        private void OnUpdateTimer(CUIEvent uiEvent)
        {
            if ((this._form != null) && NetworkAccelerator.enabled)
            {
                int netType = NetworkAccelerator.GetNetType();
                string pLAYCEHOLDER = this.PLAYCEHOLDER;
                switch (netType)
                {
                    case 1:
                        pLAYCEHOLDER = "WIFI";
                        break;

                    case 2:
                        pLAYCEHOLDER = "2G";
                        break;

                    case 3:
                        pLAYCEHOLDER = "3G";
                        break;

                    case 4:
                        pLAYCEHOLDER = "4G";
                        break;
                }
                this._form.m_formWidgets[0x21].GetComponent<Text>().text = pLAYCEHOLDER;
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                if ((curLvelContext == null) || !curLvelContext.IsMobaMode())
                {
                    if (NetworkAccelerator.started)
                    {
                        this._form.m_formWidgets[0x22].GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_LEBEL_NETWORK_DETAIL_ACC_STATE_UNKNOWN");
                    }
                    else
                    {
                        this._form.m_formWidgets[0x22].GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_LEBEL_NETWORK_DETAIL_ACC_STATE_CLOSE");
                    }
                    this._form.m_formWidgets[0x1f].GetComponent<Text>().text = this.PLAYCEHOLDER;
                }
                else
                {
                    if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
                    {
                        this._form.m_formWidgets[0x1f].GetComponent<Text>().text = Singleton<CBattleSystem>.GetInstance().FightForm.GetDisplayPing() + "ms";
                    }
                    if (!NetworkAccelerator.started)
                    {
                        this._form.m_formWidgets[0x22].GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_LEBEL_NETWORK_DETAIL_ACC_STATE_CLOSE");
                    }
                    else if (Singleton<FrameSynchr>.GetInstance().bActive && NetworkAccelerator.isAccerating())
                    {
                        this._form.m_formWidgets[0x22].GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_LEBEL_NETWORK_DETAIL_ACC_STATE_ACC");
                    }
                    else
                    {
                        this._form.m_formWidgets[0x22].GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("NETWORK_ACCELERATOR_LEBEL_NETWORK_DETAIL_ACC_STATE_DIRECT");
                    }
                }
            }
        }

        private void OnVibrateChange(CUIEvent uiEvent)
        {
            GameSettings.EnableVibrate = uiEvent.m_eventParams.sliderValue == 1f;
        }

        private void OnVoiceChange(CUIEvent uiEvent)
        {
            GameSettings.EnableVoice = uiEvent.m_eventParams.sliderValue == 1f;
            if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
            {
                Singleton<CBattleSystem>.GetInstance().FightForm.ChangeSpeakerBtnState();
            }
            this.ShowSoundSettingLevel(GameSettings.EnableVoice, null, SettingFormWidget.VoiceEffectLevel);
        }

        private void SetAvailableTabs()
        {
            this._availableSettingTypesCnt = 0;
            int num = 5;
            for (int i = 0; i < num; i++)
            {
                SettingType type = (SettingType) i;
                switch (type)
                {
                    case SettingType.Basic:
                    case SettingType.Operation:
                    case SettingType.VoiceSetting:
                        this._availableSettingTypes[this._availableSettingTypesCnt++] = type;
                        break;

                    case SettingType.NetAcc:
                        if (NetworkAccelerator.enabled)
                        {
                            this._availableSettingTypes[this._availableSettingTypesCnt++] = type;
                        }
                        break;

                    case SettingType.ReplayKit:
                        if ((!Singleton<BattleLogic>.instance.isRuning && Singleton<CRecordUseSDK>.instance.GetRecorderGlobalCfgEnableFlag()) && !CSysDynamicBlock.bLobbyEntryBlocked)
                        {
                            this._availableSettingTypes[this._availableSettingTypesCnt++] = type;
                        }
                        break;
                }
            }
        }

        private void SetHDBarShow()
        {
            if (this._form != null)
            {
                GameObject obj2 = this._form.m_formWidgets[0x31];
                bool bActive = GameSettings.SupportHDMode();
                if (Singleton<BattleLogic>.HasInstance())
                {
                    bActive &= !Singleton<BattleLogic>.GetInstance().isRuning;
                }
                obj2.CustomSetActive(bActive);
            }
        }

        private void ShowSoundSettingLevel(bool bEnable, Slider soundObj, SettingFormWidget widgetEnum)
        {
            if (bEnable)
            {
                if ((soundObj == null) && (this._form != null))
                {
                    soundObj = this._form.m_formWidgets[(int) widgetEnum].transform.FindChild("Slider").gameObject.GetComponent<Slider>();
                }
                if (soundObj != null)
                {
                    soundObj.interactable = true;
                    Transform transform = soundObj.transform.Find("Background");
                    if (transform != null)
                    {
                        Image component = transform.GetComponent<Image>();
                        if (component != null)
                        {
                            component.color = new Color(1f, 1f, 1f, 1f);
                        }
                    }
                    transform = soundObj.transform.Find("Handle Slide Area/Handle");
                    if (transform != null)
                    {
                        Image image2 = transform.GetComponent<Image>();
                        if (image2 != null)
                        {
                            image2.color = new Color(1f, 1f, 1f, 1f);
                        }
                    }
                }
            }
            else
            {
                if ((soundObj == null) && (this._form != null))
                {
                    soundObj = this._form.m_formWidgets[(int) widgetEnum].transform.FindChild("Slider").gameObject.GetComponent<Slider>();
                }
                if (soundObj != null)
                {
                    soundObj.interactable = false;
                    Transform transform2 = soundObj.transform.Find("Background");
                    if (transform2 != null)
                    {
                        Image image3 = transform2.GetComponent<Image>();
                        if (image3 != null)
                        {
                            image3.color = new Color(0f, 1f, 1f, 1f);
                        }
                    }
                    transform2 = soundObj.transform.Find("Handle Slide Area/Handle");
                    if (transform2 != null)
                    {
                        Image image4 = transform2.GetComponent<Image>();
                        if (image4 != null)
                        {
                            image4.color = new Color(0f, 1f, 1f, 1f);
                        }
                    }
                }
            }
        }

        private static void sliderMoveCameraAdjustment()
        {
            CSettingsSys instance = Singleton<CSettingsSys>.GetInstance();
            CUISliderEventScript script = (instance._form == null) ? null : instance._form.m_formWidgets[0x2a].transform.FindChild("Slider").GetComponent<CUISliderEventScript>();
            if (Singleton<BattleLogic>.instance.isRuning)
            {
                if ((script != null) && (script.value == 1f))
                {
                    CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                    uIEvent.m_eventID = enUIEventID.Settings_Slider_onMoveCameraChange;
                    uIEvent.m_eventParams.sliderValue = 2f;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
                }
            }
            else if (GameSettings.TheCameraMoveType == CameraMoveType.JoyStick)
            {
                GameSettings.TheCameraMoveType = CameraMoveType.Slide;
            }
        }

        private void SliderStatusChange(bool bEnable, Slider slliderObj, SettingFormWidget widgetEnum)
        {
            if ((slliderObj == null) && (this._form != null))
            {
                slliderObj = this._form.m_formWidgets[(int) widgetEnum].transform.FindChild("Slider").gameObject.GetComponent<Slider>();
            }
            if (slliderObj != null)
            {
                slliderObj.interactable = bEnable;
                Color color = !bEnable ? new Color(0f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 1f);
                Transform imageTransform = slliderObj.transform.Find("Background");
                this.ChangeImageColor(imageTransform, color);
                imageTransform = slliderObj.transform.Find("Handle Slide Area/Handle");
                this.ChangeImageColor(imageTransform, color);
            }
        }

        private void ToggleStatusChange(bool bEnable, SettingFormWidget widgetEnum)
        {
            Toggle component = this._form.m_formWidgets[(int) widgetEnum].GetComponent<Toggle>();
            if (component != null)
            {
                component.interactable = bEnable;
                Color color = !bEnable ? new Color(0f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 1f);
                Transform imageTransform = component.transform.Find("Background/Bg");
                this.ChangeImageColor(imageTransform, color);
                imageTransform = component.transform.Find("Background/Checkmark");
                this.ChangeImageColor(imageTransform, color);
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OpenForm, new CUIEventManager.OnUIEventHandler(this.onOpenSetting));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ReqLogout, new CUIEventManager.OnUIEventHandler(this.onReqLogout));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ConfirmLogout, new CUIEventManager.OnUIEventHandler(this.onConfirmLogout));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_SettingTypeChange, new CUIEventManager.OnUIEventHandler(this.OnSettingTabChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseSetting));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OpenNetworkAccelerator, new CUIEventManager.OnUIEventHandler(this.OnNetAccChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_AutomaticOpenNetworkAccelerator, new CUIEventManager.OnUIEventHandler(this.OnAutoNetAccChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_PrivacyPolicy, new CUIEventManager.OnUIEventHandler(this.OnClickPrivacyPolicy));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_TermOfService, new CUIEventManager.OnUIEventHandler(this.OnClickTermOfService));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Contract, new CUIEventManager.OnUIEventHandler(this.OnClickContract));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_UpdateTimer, new CUIEventManager.OnUIEventHandler(this.OnUpdateTimer));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_SurrenderCDReady, new CUIEventManager.OnUIEventHandler(this.OnSurrenderCDReady));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ConfirmQuality_Accept, new CUIEventManager.OnUIEventHandler(this.onQualitySettingAccept));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ConfirmQuality_Cancel, new CUIEventManager.OnUIEventHandler(this.onQualitySettingCancel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ClickMoveCameraGuide, new CUIEventManager.OnUIEventHandler(this.onClickMoveCameraGuide));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ClickSkillCancleTypeHelp, new CUIEventManager.OnUIEventHandler(this.onClickSkillCancleTypeHelp));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnSmartCastChange, new CUIEventManager.OnUIEventHandler(this.OnSmartCastChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnLunPanCastChange, new CUIEventManager.OnUIEventHandler(this.OnLunPanCastChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnPickNearestChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnPickNearestChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnPickMinHpChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnPickMinHpChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnCommonAttackType1Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnCommonAttackType1Change));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnCommonAttackType2Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnCommonAttackType2Change));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnSkillCanleType1Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnSkillCanleType1Change));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnSkillCanleType2Change, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnSkillCanleType2Change));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnJoyStickMoveChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnJoyStickMoveChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnJoyStickNoMoveChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnJoyStickNoMoveChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnRightJoyStickBtnLocChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnRightJoyStickBtnLocChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_OnRightJoyStickFingerLocChange, new CUIEventManager.OnUIEventHandler(CSettingsSys.OnRightJoyStickFingerLocChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onLunpanSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onModelLODChange, new CUIEventManager.OnUIEventHandler(this.OnModeLODChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onParticleLODChange, new CUIEventManager.OnUIEventHandler(this.OnParticleLODChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onSkillTipChange, new CUIEventManager.OnUIEventHandler(this.OnSkillTipChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onFpsChange, new CUIEventManager.OnUIEventHandler(this.OnFpsChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onInputChatChange, new CUIEventManager.OnUIEventHandler(this.OnInputChatChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onMoveCameraChange, new CUIEventManager.OnUIEventHandler(this.OnMoveCameraChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onHDBarChange, new CUIEventManager.OnUIEventHandler(this.OnHDBarChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_CameraHeight, new CUIEventManager.OnUIEventHandler(this.OnCameraHeightChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onCameraSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnCameraSensitivityChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onMusicChange, new CUIEventManager.OnUIEventHandler(this.OnMiusicChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onSoundEffectChange, new CUIEventManager.OnUIEventHandler(this.OnSoundEffectChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onVoiceChange, new CUIEventManager.OnUIEventHandler(this.OnVoiceChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onVibrateChange, new CUIEventManager.OnUIEventHandler(this.OnVibrateChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onMusicSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeMusic));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onSoundSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeSound));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onVoiceSensitivityChange, new CUIEventManager.OnUIEventHandler(this.OnSensitivityChangeVoice));
            this._availableSettingTypes = null;
            this._availableSettingTypesCnt = 0;
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_ReplayKitCourse, new CUIEventManager.OnUIEventHandler(this.OnClickOBFormOpen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Settings_Slider_onRecordKingTimeEnableChange, new CUIEventManager.OnUIEventHandler(this.OnRecordKingTimeEnableChange));
        }

        private void UnInitWidget()
        {
            if (this._tabList != null)
            {
                Singleton<CMiShuSystem>.GetInstance().HideNewFlag(this._tabList.GetElemenet(0).gameObject, enNewFlagKey.New_BasicSettingTab_V1);
            }
            this._tabList = null;
            this._form = null;
        }

        private void UpdateCameraSensitivitySlider()
        {
            float curCameraSensitivity = GameSettings.GetCurCameraSensitivity();
            if (curCameraSensitivity < 0f)
            {
                this.SliderStatusChange(false, null, SettingFormWidget.CameraSensitivity);
                this.GetSliderBarScript(this._form.m_formWidgets[0x30]).value = curCameraSensitivity;
            }
            else
            {
                this.SliderStatusChange(true, null, SettingFormWidget.CameraSensitivity);
                this.GetSliderBarScript(this._form.m_formWidgets[0x30]).value = curCameraSensitivity;
            }
        }

        protected enum SettingType
        {
            Basic,
            Operation,
            VoiceSetting,
            NetAcc,
            ReplayKit,
            TypeCount
        }
    }
}

