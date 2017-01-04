namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CSkillButtonManager
    {
        private SkillButton[] _skillButtons = new SkillButton[s_maxButtonCount];
        private const float c_directionalSkillIndicatorRespondMinRadius = 30f;
        private const float c_skillICancleRadius = 270f;
        private const float c_skillIndicatorMoveRadius = 30f;
        private const float c_skillIndicatorRadius = 110f;
        private const float c_skillIndicatorRespondMinRadius = 15f;
        private const string c_skillJoystickTargetNormalBorderPath = "UGUI/Sprite/Battle/Battle_skillFrameBg_new.prefab";
        private const string c_skillJoystickTargetSelectedBorderPath = "UGUI/Sprite/Battle/Battle_ComboCD.prefab";
        private stCampHeroInfo[] m_campHeroInfos = new stCampHeroInfo[0];
        private bool m_commonAtkSlide;
        private Vector2 m_currentSkillDownScreenPosition = Vector2.zero;
        private bool m_currentSkillIndicatorEnabled;
        private bool m_currentSkillIndicatorInCancelArea;
        private bool m_currentSkillIndicatorJoystickEnabled;
        private bool m_currentSkillIndicatorResponed;
        private Vector2 m_currentSkillIndicatorScreenPosition = Vector2.zero;
        private enSkillJoystickMode m_currentSkillJoystickMode;
        private int m_currentSkillJoystickSelectedIndex = -1;
        private SkillSlotType m_currentSkillSlotType = SkillSlotType.SLOT_SKILL_COUNT;
        private bool m_currentSkillTipsResponed;
        private enSkillIndicatorMode m_skillIndicatorMode = enSkillIndicatorMode.FixedPosition;
        private static byte s_maxButtonCount = 8;
        private static enBattleFormWidget[] s_skillButtons = new enBattleFormWidget[] { enBattleFormWidget.Button_Attack, enBattleFormWidget.Button_Skill_1, enBattleFormWidget.Button_Skill_2, enBattleFormWidget.Button_Skill_3, enBattleFormWidget.Button_Recover, enBattleFormWidget.Button_Talent, enBattleFormWidget.Button_Skill_6, enBattleFormWidget.Button_Skill_Ornament, enBattleFormWidget.Button_ComAtkSelectSoldier };
        private static enBattleFormWidget[] s_skillCDTexts = new enBattleFormWidget[] { enBattleFormWidget.None, enBattleFormWidget.Text_Skill_1_CD, enBattleFormWidget.Text_Skill_2_CD, enBattleFormWidget.Text_Skill_3_CD, enBattleFormWidget.Text_Skill_4_CD, enBattleFormWidget.Text_Skill_5_CD, enBattleFormWidget.Text_Skill_6_CD, enBattleFormWidget.Text_Skill_Ornament_CD, enBattleFormWidget.None };
        private static Color s_skillCursorBGColorBlue = new Color(0.1686275f, 0.7607843f, 1f, 1f);
        private static Color s_skillCursorBGColorRed = new Color(0.972549f, 0.1843137f, 0.1843137f, 1f);

        public CSkillButtonManager()
        {
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.onActorRevive));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
        }

        public void CancelLimitButton(SkillSlotType skillSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            SkillButton button = this.GetButton(skillSlotType);
            DebugHelper.Assert(button != null);
            if (button != null)
            {
                button.bLimitedFlag = false;
                if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
                {
                    if (!button.bDisableFlag)
                    {
                        GameObject animationPresent = button.GetAnimationPresent();
                        if (animationPresent != null)
                        {
                            Image component = animationPresent.GetComponent<Image>();
                            if (component != null)
                            {
                                component.color = CUIUtility.s_Color_Full;
                            }
                        }
                        GameObject disableButton = button.GetDisableButton();
                        if (disableButton != null)
                        {
                            disableButton.CustomSetActive(false);
                        }
                        this.SetSelectTargetBtnState(false);
                    }
                }
                else
                {
                    if (button.m_button != null)
                    {
                        SkillSlot slot;
                        CUIEventScript script = button.m_button.GetComponent<CUIEventScript>();
                        if (((script != null) && (hostPlayer != null)) && ((hostPlayer.Captain != 0) && hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot)))
                        {
                            if (slot.EnableButtonFlag)
                            {
                                script.enabled = true;
                            }
                            else
                            {
                                script.enabled = false;
                            }
                        }
                    }
                    GameObject gameObject = button.GetAnimationPresent().transform.Find("disableFrame").gameObject;
                    DebugHelper.Assert(gameObject != null);
                    if (gameObject != null)
                    {
                        gameObject.CustomSetActive(false);
                    }
                    if (!button.bDisableFlag)
                    {
                        CUICommonSystem.PlayAnimation(button.GetAnimationPresent(), enSkillButtonAnimationName.normal.ToString(), false);
                    }
                }
            }
        }

        public void CancelUseSkillSlot(SkillSlotType skillSlotType, enSkillJoystickMode mode = 0)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                hostPlayer.Captain.handle.SkillControl.HostCancelUseSkillSlot(skillSlotType, mode);
            }
        }

        public void ChangeSkill(SkillSlotType skillSlotType, ref ChangeSkillEventParam skillParam)
        {
            if ((skillSlotType > SkillSlotType.SLOT_SKILL_0) && (skillParam.skillID > 0))
            {
                SkillButton button = this._skillButtons[(int) skillSlotType];
                if (button != null)
                {
                    button.ChangeSkillIcon(skillParam.skillID);
                }
                this.SetComboEffect(skillSlotType, skillParam.changeTime, skillParam.changeTime);
            }
        }

        private void ChangeSkillCursorBGSprite(CUIFormScript battleFormScript, GameObject skillJoystick, bool isSkillCursorInCancelArea)
        {
            if (skillJoystick != null)
            {
                Image component = skillJoystick.GetComponent<Image>();
                if (component != null)
                {
                    component.color = GetCursorBGColor(isSkillCursorInCancelArea);
                }
            }
        }

        private void ChangeSkillJoystickSelectedTarget(CUIFormScript battleFormScript, GameObject skillJoystick, int selectedIndex)
        {
            if (this.m_currentSkillJoystickSelectedIndex != selectedIndex)
            {
                int currentSkillJoystickSelectedIndex = this.m_currentSkillJoystickSelectedIndex;
                this.m_currentSkillJoystickSelectedIndex = selectedIndex;
                this.SetSkillIndicatorSelectedTarget(this.m_currentSkillJoystickSelectedIndex);
                if ((battleFormScript != null) && (skillJoystick != null))
                {
                    CUIComponent component = skillJoystick.GetComponent<CUIComponent>();
                    if (((component != null) && (component.m_widgets != null)) && (component.m_widgets.Length >= this.m_campHeroInfos.Length))
                    {
                        if ((this.m_currentSkillJoystickSelectedIndex >= 0) && (this.m_currentSkillJoystickSelectedIndex < this.m_campHeroInfos.Length))
                        {
                            this.SetSkillJoystickTargetHead(battleFormScript, component.m_widgets[this.m_currentSkillJoystickSelectedIndex], true);
                            Transform transform = skillJoystick.transform.FindChild("HighLight");
                            if (transform != null)
                            {
                                transform.gameObject.CustomSetActive(true);
                                transform.eulerAngles = new Vector3(0f, 0f, (float) (0x2d * this.m_currentSkillJoystickSelectedIndex));
                            }
                        }
                        else
                        {
                            Transform transform2 = skillJoystick.transform.FindChild("HighLight");
                            if (transform2 != null)
                            {
                                transform2.gameObject.CustomSetActive(false);
                            }
                        }
                        if ((currentSkillJoystickSelectedIndex >= 0) && (currentSkillJoystickSelectedIndex < this.m_campHeroInfos.Length))
                        {
                            this.SetSkillJoystickTargetHead(battleFormScript, component.m_widgets[currentSkillJoystickSelectedIndex], false);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            if (this._skillButtons != null)
            {
                for (int i = 0; i < this._skillButtons.Length; i++)
                {
                    if (this._skillButtons[i] != null)
                    {
                        this._skillButtons[i].Clear();
                    }
                }
            }
        }

        public bool ClearButtonInput(SkillSlotType CurSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                for (int i = 0; i < 8; i++)
                {
                    if (CurSlotType != i)
                    {
                        SkillSlot slot;
                        if (!hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot((SkillSlotType) i, out slot))
                        {
                            return false;
                        }
                        SkillButton button = this.GetButton((SkillSlotType) i);
                        if (button != null)
                        {
                            if (button.m_button != null)
                            {
                                CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                                if (component != null)
                                {
                                    component.ClearInputStatus();
                                }
                            }
                            GameObject disableButton = button.GetDisableButton();
                            if (disableButton != null)
                            {
                                CUIEventScript script2 = disableButton.GetComponent<CUIEventScript>();
                                if (script2 != null)
                                {
                                    script2.ClearInputStatus();
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void CommonAtkSlide(CUIFormScript battleFormScript, Vector2 screenPosition)
        {
            if (GameSettings.TheCommonAttackType == CommonAttactType.Type2)
            {
                SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
                if ((button != null) && !button.bDisableFlag)
                {
                    GameObject widget = battleFormScript.GetWidget(0x44);
                    GameObject targetObj = battleFormScript.GetWidget(0x39);
                    if (this.IsSkillCursorInTargetArea(battleFormScript, ref screenPosition, widget))
                    {
                        Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_HERO);
                        Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, widget, battleFormScript);
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_hero", null);
                    }
                    else if (this.IsSkillCursorInTargetArea(battleFormScript, ref screenPosition, targetObj))
                    {
                        Singleton<LockModeKeySelector>.GetInstance().OnHandleClickSelectTargetBtn(AttackTargetType.ATTACK_TARGET_SOLDIER);
                        Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, targetObj, battleFormScript);
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_signal_Change_xiaobing", null);
                    }
                }
            }
        }

        public void DisableSkillCursor(CUIFormScript battleFormScript)
        {
            this.m_currentSkillIndicatorEnabled = false;
            this.m_currentSkillIndicatorJoystickEnabled = false;
            this.m_currentSkillIndicatorResponed = false;
            this.m_currentSkillTipsResponed = false;
            this.m_currentSkillIndicatorInCancelArea = false;
            DebugHelper.Assert(battleFormScript != null);
            if (battleFormScript != null)
            {
                this.DisableSkillJoystick(battleFormScript, this.m_currentSkillJoystickMode);
                if (GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle)
                {
                    GameObject widget = battleFormScript.GetWidget(0x1f);
                    if (widget != null)
                    {
                        widget.CustomSetActive(false);
                    }
                }
            }
        }

        private void DisableSkillJoystick(CUIFormScript battleFormScript, enSkillJoystickMode skillJoystickMode)
        {
            if (battleFormScript != null)
            {
                if (skillJoystickMode == enSkillJoystickMode.General)
                {
                    GameObject widget = battleFormScript.GetWidget(0x19);
                    if ((widget != null) && widget.activeSelf)
                    {
                        widget.CustomSetActive(false);
                        RectTransform transform = widget.transform.FindChild("Cursor") as RectTransform;
                        if (transform != null)
                        {
                            transform.anchoredPosition = Vector2.zero;
                        }
                    }
                }
                else if (skillJoystickMode == enSkillJoystickMode.SelectTarget)
                {
                    GameObject obj3 = battleFormScript.GetWidget(0x4c);
                    if ((obj3 != null) && obj3.activeSelf)
                    {
                        obj3.CustomSetActive(false);
                        RectTransform transform2 = obj3.transform.FindChild("Cursor") as RectTransform;
                        if (transform2 != null)
                        {
                            transform2.anchoredPosition = Vector2.zero;
                        }
                    }
                }
            }
        }

        public void DragSkillButton(CUIFormScript formScript, SkillSlotType skillSlotType, Vector2 dragScreenPosition)
        {
            if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
            {
                this.m_commonAtkSlide = true;
            }
            if ((this.m_currentSkillSlotType == skillSlotType) && this.m_currentSkillIndicatorEnabled)
            {
                bool currentSkillIndicatorInCancelArea = this.m_currentSkillIndicatorInCancelArea;
                if (formScript != null)
                {
                    GameObject widget = formScript.GetWidget((this.m_currentSkillJoystickMode != enSkillJoystickMode.General) ? 0x4c : 0x19);
                    Vector2 cursor = this.MoveSkillCursor(formScript, widget, skillSlotType, this.m_currentSkillJoystickMode, ref dragScreenPosition, out this.m_currentSkillIndicatorInCancelArea);
                    if (currentSkillIndicatorInCancelArea != this.m_currentSkillIndicatorInCancelArea)
                    {
                        this.ChangeSkillCursorBGSprite(formScript, widget, this.m_currentSkillIndicatorInCancelArea);
                    }
                    if (this.m_currentSkillJoystickMode == enSkillJoystickMode.General)
                    {
                        this.MoveSkillCursorInScene(skillSlotType, ref cursor, this.m_currentSkillIndicatorInCancelArea);
                    }
                    else if ((this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget) && (this.m_currentSkillJoystickSelectedIndex != -1))
                    {
                        uint objectID = this.m_campHeroInfos[this.m_currentSkillJoystickSelectedIndex].m_objectID;
                        PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objectID);
                        if (actor != 0)
                        {
                            MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(actor);
                        }
                    }
                }
            }
        }

        public void EnableSkillCursor(CUIFormScript battleFormScript, ref Vector2 screenPosition, bool enableSkillCursorJoystick, SkillSlotType skillSlotType, bool isSkillCanBeCancled)
        {
            this.m_currentSkillIndicatorEnabled = true;
            this.m_currentSkillIndicatorResponed = false;
            this.m_currentSkillTipsResponed = false;
            if (enableSkillCursorJoystick)
            {
                this.m_currentSkillIndicatorJoystickEnabled = true;
                this.m_currentSkillJoystickMode = this.GetSkillJoystickMode(skillSlotType);
                if (battleFormScript != null)
                {
                    if (this.m_currentSkillJoystickMode == enSkillJoystickMode.General)
                    {
                        this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.SelectTarget);
                        GameObject widget = battleFormScript.GetWidget(0x19);
                        if (widget != null)
                        {
                            widget.CustomSetActive(true);
                            Vector3 skillIndicatorFixedPosition = this.GetButton(skillSlotType).m_skillIndicatorFixedPosition;
                            if ((this.m_skillIndicatorMode == enSkillIndicatorMode.General) || (skillIndicatorFixedPosition == Vector3.zero))
                            {
                                widget.transform.position = CUIUtility.ScreenToWorldPoint(battleFormScript.GetCamera(), screenPosition, widget.transform.position.z);
                                this.m_currentSkillIndicatorScreenPosition = screenPosition;
                            }
                            else
                            {
                                widget.transform.position = skillIndicatorFixedPosition;
                                this.m_currentSkillIndicatorScreenPosition = CUIUtility.WorldToScreenPoint(battleFormScript.GetCamera(), skillIndicatorFixedPosition);
                            }
                        }
                        this.ChangeSkillCursorBGSprite(battleFormScript, widget, this.m_currentSkillIndicatorInCancelArea);
                    }
                    else if (this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget)
                    {
                        this.DisableSkillJoystick(battleFormScript, enSkillJoystickMode.General);
                        GameObject obj3 = battleFormScript.GetWidget(0x4c);
                        if (obj3 != null)
                        {
                            obj3.CustomSetActive(true);
                            obj3.transform.position = this.GetButton(skillSlotType).m_button.transform.position;
                            this.m_currentSkillIndicatorScreenPosition = CUIUtility.WorldToScreenPoint(battleFormScript.GetCamera(), obj3.transform.position);
                            CUIAnimationScript component = obj3.GetComponent<CUIAnimationScript>();
                            if (component != null)
                            {
                                component.PlayAnimation("Head_In2", true);
                            }
                            this.ResetSkillJoystickSelectedTarget(battleFormScript);
                            this.ChangeSkillCursorBGSprite(battleFormScript, obj3, this.m_currentSkillIndicatorInCancelArea);
                        }
                    }
                }
            }
            if ((battleFormScript != null) && (GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle))
            {
                GameObject obj4 = battleFormScript.GetWidget(0x1f);
                if (obj4 != null)
                {
                    obj4.CustomSetActive(isSkillCanBeCancled);
                }
            }
        }

        ~CSkillButtonManager()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.onActorRevive));
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
        }

        public SkillButton GetButton(SkillSlotType skillSlotType)
        {
            int index = (int) skillSlotType;
            if ((index < 0) || (index >= this._skillButtons.Length))
            {
                return null;
            }
            SkillButton button = this._skillButtons[index];
            if (button == null)
            {
                CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
                button = new SkillButton {
                    m_button = (fightFormScript != null) ? fightFormScript.GetWidget((int) s_skillButtons[index]) : null,
                    m_cdText = (fightFormScript != null) ? fightFormScript.GetWidget((int) s_skillCDTexts[index]) : null
                };
                this._skillButtons[index] = button;
                if (button.m_button != null)
                {
                    Transform transform = button.m_button.transform.FindChild("IndicatorPosition");
                    if (transform != null)
                    {
                        button.m_skillIndicatorFixedPosition = transform.position;
                    }
                }
            }
            return button;
        }

        public SkillSlotType GetCurSkillSlotType()
        {
            return this.m_currentSkillSlotType;
        }

        public static Color GetCursorBGColor(bool cancel)
        {
            return (!cancel ? s_skillCursorBGColorBlue : s_skillCursorBGColorRed);
        }

        private static void GetJoystickHeadAreaFan(ref stFan headAreaFan, GameObject head, GameObject preHead, GameObject backHead)
        {
            if ((preHead == null) && (backHead == null))
            {
                headAreaFan.m_minRadian = 0f;
                headAreaFan.m_maxRadian = 0f;
            }
            float radian = GetRadian((head.transform as RectTransform).anchoredPosition);
            if (preHead != null)
            {
                headAreaFan.m_minRadian = GetRadian((Vector2) (((head.transform as RectTransform).anchoredPosition + (preHead.transform as RectTransform).anchoredPosition) / 2f));
                if (backHead == null)
                {
                    headAreaFan.m_maxRadian = radian + (radian - headAreaFan.m_minRadian);
                    return;
                }
            }
            if (backHead != null)
            {
                headAreaFan.m_maxRadian = GetRadian((Vector2) (((head.transform as RectTransform).anchoredPosition + (backHead.transform as RectTransform).anchoredPosition) / 2f));
                if (preHead == null)
                {
                    headAreaFan.m_minRadian = radian - (headAreaFan.m_maxRadian - radian);
                }
            }
        }

        private static void GetJoystickHeadAreaInScreen(ref Rect targetArea, CUIFormScript formScript, RectTransform joystickRectTransform, RectTransform targetRectTransform)
        {
            if ((joystickRectTransform == null) || (targetRectTransform == null))
            {
                targetArea = new Rect(0f, 0f, 0f, 0f);
            }
            else
            {
                Vector2 vector = new Vector2(formScript.ChangeFormValueToScreen(targetRectTransform.anchoredPosition.x), formScript.ChangeFormValueToScreen(targetRectTransform.anchoredPosition.y));
                float width = formScript.ChangeFormValueToScreen(targetRectTransform.rect.width);
                float height = formScript.ChangeFormValueToScreen(targetRectTransform.rect.height);
                float x = vector.x - (width / 2f);
                float y = vector.y - (height / 2f);
                targetArea = new Rect(x, y, width, height);
            }
        }

        private static float GetRadian(Vector2 point)
        {
            float num = Mathf.Atan2(point.y, point.x);
            if (num < 0f)
            {
                num += 6.283185f;
            }
            return num;
        }

        public enSkillJoystickMode GetSkillJoystickMode(SkillSlotType skillSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                SkillSlot slot = null;
                if (hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot) && (slot != null))
                {
                    Skill skill = (slot.NextSkillObj == null) ? slot.SkillObj : slot.NextSkillObj;
                    if ((skill != null) && (skill.cfgData != null))
                    {
                        return (enSkillJoystickMode) skill.cfgData.bWheelType;
                    }
                }
            }
            return enSkillJoystickMode.General;
        }

        public void Initialise(PoolObjHandle<ActorRoot> actor)
        {
            if (((actor != 0) && (actor.handle.SkillControl != null)) && (actor.handle.SkillControl.SkillSlotArray != null))
            {
                SkillSlot[] skillSlotArray = actor.handle.SkillControl.SkillSlotArray;
                this._skillButtons = new SkillButton[s_maxButtonCount];
                for (int i = 0; i < s_maxButtonCount; i++)
                {
                    SkillSlotType skillSlotType = (SkillSlotType) i;
                    SkillButton button = this.GetButton(skillSlotType);
                    SkillSlot slot = skillSlotArray[i];
                    DebugHelper.Assert(button != null);
                    if (button == null)
                    {
                        continue;
                    }
                    button.bDisableFlag = false;
                    button.bLimitedFlag = false;
                    if (slot != null)
                    {
                        if (!slot.EnableButtonFlag)
                        {
                            button.bDisableFlag = true;
                        }
                        else
                        {
                            button.bDisableFlag = false;
                        }
                    }
                    if (actor.handle.SkillControl.DisableSkill[i] == 1)
                    {
                        button.bLimitedFlag = true;
                    }
                    else
                    {
                        button.bLimitedFlag = false;
                    }
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if (skillSlotType == SkillSlotType.SLOT_SKILL_6)
                    {
                        if (curLvelContext.IsGameTypeGuide())
                        {
                            if (((curLvelContext.m_mapID == CBattleGuideManager.GuideLevelID5v5) && (slot != null)) && ((slot.SkillObj != null) && (slot.SkillObj.cfgData != null)))
                            {
                                goto Label_019D;
                            }
                            button.m_button.CustomSetActive(false);
                            continue;
                        }
                        if ((!curLvelContext.IsMobaModeWithOutGuide() || (curLvelContext.m_pvpPlayerNum != 10)) || (((slot == null) || (slot.SkillObj == null)) || (slot.SkillObj.cfgData == null)))
                        {
                            button.m_button.CustomSetActive(false);
                            continue;
                        }
                    }
                Label_019D:
                    if ((skillSlotType == SkillSlotType.SLOT_SKILL_7) && !curLvelContext.m_bEnableOrnamentSlot)
                    {
                        button.m_button.CustomSetActive(false);
                        button.m_cdText.CustomSetActive(false);
                        continue;
                    }
                    if ((Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeBurning() || Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeArena()) || Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeAdventure())
                    {
                        if ((Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeBurning() || Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeArena()) && ((skillSlotType == SkillSlotType.SLOT_SKILL_4) || (skillSlotType == SkillSlotType.SLOT_SKILL_5)))
                        {
                            if ((button.m_button != null) && (button.m_cdText != null))
                            {
                                button.m_button.CustomSetActive(false);
                                button.m_cdText.CustomSetActive(false);
                            }
                            continue;
                        }
                        if ((skillSlotType >= SkillSlotType.SLOT_SKILL_1) && (skillSlotType <= SkillSlotType.SLOT_SKILL_3))
                        {
                            if (button.m_button != null)
                            {
                                GameObject skillLvlFrameImg = button.GetSkillLvlFrameImg(true);
                                if (skillLvlFrameImg != null)
                                {
                                    skillLvlFrameImg.CustomSetActive(false);
                                }
                                GameObject obj3 = button.GetSkillLvlFrameImg(false);
                                if (obj3 != null)
                                {
                                    obj3.CustomSetActive(false);
                                }
                                GameObject skillFrameImg = button.GetSkillFrameImg();
                                if (skillFrameImg != null)
                                {
                                    skillFrameImg.CustomSetActive(true);
                                }
                            }
                            if (slot != null)
                            {
                                int dwConfValue = 0;
                                switch (skillSlotType)
                                {
                                    case SkillSlotType.SLOT_SKILL_1:
                                    case SkillSlotType.SLOT_SKILL_2:
                                        dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x8e).dwConfValue;
                                        break;

                                    default:
                                        if (skillSlotType == SkillSlotType.SLOT_SKILL_3)
                                        {
                                            dwConfValue = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x8f).dwConfValue;
                                        }
                                        break;
                                }
                                slot.SetSkillLevel(dwConfValue);
                            }
                        }
                    }
                    if (button.m_button != null)
                    {
                        CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                        CUIEventScript script2 = button.GetDisableButton().GetComponent<CUIEventScript>();
                        if (slot != null)
                        {
                            component.enabled = true;
                            script2.m_onClickEventID = enUIEventID.Battle_Disable_Alert;
                            script2.enabled = true;
                            switch (skillSlotType)
                            {
                                case SkillSlotType.SLOT_SKILL_1:
                                case SkillSlotType.SLOT_SKILL_2:
                                case SkillSlotType.SLOT_SKILL_3:
                                    if (slot.EnableButtonFlag)
                                    {
                                        component.enabled = true;
                                    }
                                    else
                                    {
                                        component.enabled = false;
                                    }
                                    break;
                            }
                            if (button.GetDisableButton() != null)
                            {
                                if (slot.GetSkillLevel() > 0)
                                {
                                    this.SetEnableButton(skillSlotType);
                                    script2.m_onDownEventID = enUIEventID.None;
                                    script2.m_onUpEventID = enUIEventID.None;
                                    if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                                    {
                                        script2.m_onHoldEventID = enUIEventID.Battle_OnSkillButtonHold;
                                        script2.m_onHoldEndEventID = enUIEventID.Battle_OnSkillButtonHoldEnd;
                                    }
                                    if (((actor.handle.ActorControl != null) && actor.handle.ActorControl.IsDeadState) || ((slot.SlotType != SkillSlotType.SLOT_SKILL_0) && !slot.IsCDReady))
                                    {
                                        this.SetDisableButton(skillSlotType);
                                    }
                                }
                                else
                                {
                                    this.SetDisableButton(skillSlotType);
                                }
                            }
                            if (button.m_button != null)
                            {
                                button.m_button.CustomSetActive(true);
                            }
                            if (button.m_cdText != null)
                            {
                                button.m_cdText.CustomSetActive(true);
                            }
                            if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                            {
                                Image image = button.m_button.transform.Find("Present/SkillImg").GetComponent<Image>();
                                image.gameObject.CustomSetActive(true);
                                image.SetSprite(CUIUtility.s_Sprite_Dynamic_Skill_Dir + slot.SkillObj.IconName, Singleton<CBattleSystem>.GetInstance().FightFormScript, true, false, false);
                                if (((skillSlotType == SkillSlotType.SLOT_SKILL_4) || (skillSlotType == SkillSlotType.SLOT_SKILL_5)) || ((skillSlotType == SkillSlotType.SLOT_SKILL_6) || (skillSlotType == SkillSlotType.SLOT_SKILL_7)))
                                {
                                    Transform transform = button.m_button.transform.Find("lblName");
                                    if (transform != null)
                                    {
                                        if (slot.SkillObj.cfgData != null)
                                        {
                                            transform.GetComponent<Text>().text = slot.SkillObj.cfgData.szSkillName;
                                        }
                                        transform.gameObject.CustomSetActive(true);
                                    }
                                }
                                if (actor.handle.SkillControl.IsDisableSkillSlot(skillSlotType))
                                {
                                    this.SetLimitButton(skillSlotType);
                                }
                                else if ((slot.GetSkillLevel() > 0) && slot.IsEnergyEnough)
                                {
                                    this.CancelLimitButton(skillSlotType);
                                }
                                if (slot.GetSkillLevel() > 0)
                                {
                                    this.UpdateButtonCD(skillSlotType, (int) slot.CurSkillCD);
                                }
                                else if (button.m_cdText != null)
                                {
                                    button.m_cdText.CustomSetActive(false);
                                }
                            }
                            component.m_onDownEventParams.m_skillSlotType = skillSlotType;
                            component.m_onUpEventParams.m_skillSlotType = skillSlotType;
                            component.m_onHoldEventParams.m_skillSlotType = skillSlotType;
                            component.m_onHoldEndEventParams.m_skillSlotType = skillSlotType;
                            component.m_onDragStartEventParams.m_skillSlotType = skillSlotType;
                            component.m_onDragEventParams.m_skillSlotType = skillSlotType;
                            script2.m_onClickEventParams.m_skillSlotType = skillSlotType;
                            if (slot.skillChangeEvent.IsDisplayUI())
                            {
                                this.SetComboEffect(skillSlotType, slot.skillChangeEvent.GetEffectTIme(), slot.skillChangeEvent.GetEffectMaxTime());
                            }
                        }
                        else
                        {
                            component.enabled = false;
                            script2.enabled = false;
                            if (button.GetDisableButton() != null)
                            {
                                this.SetDisableButton(skillSlotType);
                            }
                            if (button.m_cdText != null)
                            {
                                button.m_cdText.CustomSetActive(false);
                            }
                            if (skillSlotType == SkillSlotType.SLOT_SKILL_7)
                            {
                                Transform transform2 = button.m_button.transform.Find("Present/SkillImg");
                                if (transform2 != null)
                                {
                                    transform2.gameObject.CustomSetActive(false);
                                }
                                Transform transform3 = button.m_button.transform.Find("lblName");
                                if (transform3 != null)
                                {
                                    transform3.gameObject.CustomSetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void InitializeCampHeroInfo(CUIFormScript battleFormScript)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (hostPlayer != null)
            {
                List<Player> allCampPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(hostPlayer.PlayerCamp);
                if (allCampPlayers != null)
                {
                    this.m_campHeroInfos = new stCampHeroInfo[allCampPlayers.Count - 1];
                    int index = 0;
                    for (int i = 0; i < allCampPlayers.Count; i++)
                    {
                        if (allCampPlayers[i] != hostPlayer)
                        {
                            this.m_campHeroInfos[index].m_headIconPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic((uint) allCampPlayers[i].Captain.handle.TheActorMeta.ConfigId, 0);
                            this.m_campHeroInfos[index].m_objectID = allCampPlayers[i].Captain.handle.ObjID;
                            index++;
                        }
                    }
                }
                this.m_currentSkillJoystickSelectedIndex = -1;
                if (battleFormScript != null)
                {
                    GameObject widget = battleFormScript.GetWidget(0x4c);
                    if (widget != null)
                    {
                        CUIComponent component = widget.GetComponent<CUIComponent>();
                        if (((component != null) && (component.m_widgets != null)) && (component.m_widgets.Length >= this.m_campHeroInfos.Length))
                        {
                            for (int j = 0; j < component.m_widgets.Length; j++)
                            {
                                GameObject obj3 = component.m_widgets[j];
                                if (obj3 != null)
                                {
                                    if (j >= this.m_campHeroInfos.Length)
                                    {
                                        obj3.CustomSetActive(false);
                                    }
                                    else
                                    {
                                        obj3.CustomSetActive(true);
                                        obj3.transform.localScale = new Vector3(1f, 1f, 1f);
                                        GetJoystickHeadAreaInScreen(ref this.m_campHeroInfos[j].m_headAreaInScreen, battleFormScript, widget.transform as RectTransform, obj3.transform as RectTransform);
                                        GetJoystickHeadAreaFan(ref this.m_campHeroInfos[j].m_headAreaFan, obj3, (j != 0) ? component.m_widgets[j - 1] : null, (j != (component.m_widgets.Length - 1)) ? component.m_widgets[j + 1] : null);
                                        CUIComponent component2 = obj3.GetComponent<CUIComponent>();
                                        if (((component2 != null) && (component2.m_widgets != null)) && (component2.m_widgets.Length >= 2))
                                        {
                                            GameObject obj4 = component2.m_widgets[0];
                                            if (obj4 != null)
                                            {
                                                Image image = obj4.GetComponent<Image>();
                                                if (image != null)
                                                {
                                                    image.SetSprite(this.m_campHeroInfos[j].m_headIconPath, battleFormScript, true, false, false);
                                                }
                                            }
                                            GameObject obj5 = component2.m_widgets[1];
                                            if (obj5 != null)
                                            {
                                                Image image2 = obj5.GetComponent<Image>();
                                                if (image2 != null)
                                                {
                                                    image2.SetSprite("UGUI/Sprite/Battle/Battle_skillFrameBg_new.prefab", battleFormScript, true, false, false);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        widget.CustomSetActive(false);
                    }
                }
            }
        }

        private bool IsSkillCursorInCanceledArea(CUIFormScript battleFormScript, ref Vector2 screenPosition)
        {
            if (GameSettings.TheSkillCancleType == SkillCancleType.AreaCancle)
            {
                GameObject widget = battleFormScript.GetWidget(0x1f);
                return this.IsSkillCursorInTargetArea(battleFormScript, ref screenPosition, widget);
            }
            Vector2 vector = screenPosition - this.m_currentSkillDownScreenPosition;
            return (battleFormScript.ChangeScreenValueToForm(vector.magnitude) > 270f);
        }

        private bool IsSkillCursorInTargetArea(CUIFormScript battleFormScript, ref Vector2 screenPosition, GameObject targetObj)
        {
            DebugHelper.Assert(battleFormScript != null, "battleFormScript!=null");
            if (battleFormScript != null)
            {
                DebugHelper.Assert((targetObj != null) && (targetObj.transform is RectTransform), "targetObj != null && targetObj.transform is RectTransform");
                if (((targetObj != null) && targetObj.activeInHierarchy) && (targetObj.transform is RectTransform))
                {
                    Vector2 vector = CUIUtility.WorldToScreenPoint(battleFormScript.GetCamera(), targetObj.transform.position);
                    float width = battleFormScript.ChangeFormValueToScreen((targetObj.transform as RectTransform).sizeDelta.x);
                    float height = battleFormScript.ChangeFormValueToScreen((targetObj.transform as RectTransform).sizeDelta.y);
                    Rect rect = new Rect(vector.x - (width / 2f), vector.y - (height / 2f), width, height);
                    return rect.Contains(screenPosition);
                }
            }
            return false;
        }

        public bool IsUseSkillCursorJoystick(SkillSlotType skillSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            return (((hostPlayer != null) && (hostPlayer.Captain != 0)) && hostPlayer.Captain.handle.SkillControl.IsUseSkillJoystick(skillSlotType));
        }

        public Vector2 MoveSkillCursor(CUIFormScript battleFormScript, GameObject skillJoystick, SkillSlotType skillSlotType, enSkillJoystickMode skillJoystickMode, ref Vector2 screenPosition, out bool skillCanceled)
        {
            skillCanceled = this.IsSkillCursorInCanceledArea(battleFormScript, ref screenPosition);
            if (!this.m_currentSkillIndicatorJoystickEnabled)
            {
                return Vector2.zero;
            }
            if (!this.m_currentSkillIndicatorResponed)
            {
                Vector2 vector = screenPosition - this.m_currentSkillDownScreenPosition;
                if (battleFormScript.ChangeScreenValueToForm(vector.magnitude) > 15f)
                {
                    this.m_currentSkillIndicatorResponed = true;
                }
            }
            if (!this.m_currentSkillTipsResponed)
            {
                Vector2 vector2 = screenPosition - this.m_currentSkillDownScreenPosition;
                if (battleFormScript.ChangeScreenValueToForm(vector2.magnitude) > 30f)
                {
                    this.m_currentSkillTipsResponed = true;
                }
            }
            if (!this.m_currentSkillIndicatorResponed)
            {
                return Vector2.zero;
            }
            Vector2 vector3 = screenPosition - this.m_currentSkillIndicatorScreenPosition;
            Vector2 vector4 = vector3;
            float magnitude = vector3.magnitude;
            float num2 = magnitude;
            num2 = battleFormScript.ChangeScreenValueToForm(magnitude);
            vector4.x = battleFormScript.ChangeScreenValueToForm(vector3.x);
            vector4.y = battleFormScript.ChangeScreenValueToForm(vector3.y);
            if (num2 > 110f)
            {
                vector4 = (Vector2) (vector4.normalized * 110f);
            }
            if (skillJoystick != null)
            {
                Transform transform = skillJoystick.transform.FindChild("Cursor");
                if (transform != null)
                {
                    (transform as RectTransform).anchoredPosition = vector4;
                }
            }
            if (skillJoystickMode == enSkillJoystickMode.General)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && ((hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType].SkillObj.cfgData.dwRangeAppointType == 3) && (num2 < 30f)))
                {
                    return Vector2.zero;
                }
            }
            else if (skillJoystickMode == enSkillJoystickMode.SelectTarget)
            {
                int selectedIndex = this.SkillJoystickSelectTarget(battleFormScript, skillJoystick, ref screenPosition);
                this.ChangeSkillJoystickSelectedTarget(battleFormScript, skillJoystick, selectedIndex);
            }
            return (Vector2) (vector4 / 110f);
        }

        public void MoveSkillCursorInScene(SkillSlotType skillSlotType, ref Vector2 cursor, bool isSkillCursorInCancelArea)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                hostPlayer.Captain.handle.SkillControl.SelectSkillTarget(skillSlotType, cursor, isSkillCursorInCancelArea);
            }
        }

        private void onActorDead(ref GameDeadEventParam prm)
        {
            if (Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain == prm.src)
            {
                for (int i = 0; i < 8; i++)
                {
                    this.SetDisableButton((SkillSlotType) i);
                }
            }
        }

        private void onActorRevive(ref DefaultGameEventParam prm)
        {
            if (Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain == prm.src)
            {
                for (int i = 0; i < 8; i++)
                {
                    this.SetEnableButton((SkillSlotType) i);
                }
            }
        }

        public void OnBattleSkillDisableAlert(SkillSlotType skillSlotType)
        {
            SkillSlot slot;
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && (hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot) && slot.IsUnLock()))
            {
                if (!slot.IsCDReady)
                {
                    slot.SendSkillCooldownEvent();
                }
                else if (!slot.IsEnergyEnough)
                {
                    slot.SendSkillShortageEvent();
                }
            }
        }

        private void OnCaptainSwitched(ref DefaultGameEventParam prm)
        {
            Singleton<CBattleSystem>.GetInstance().FightForm.ResetSkillButtonManager(prm.src);
        }

        public static void Preload(ref ActorPreloadTab result)
        {
        }

        public void ReadyUseSkillSlot(SkillSlotType skillSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                hostPlayer.Captain.handle.SkillControl.ReadyUseSkillSlot(skillSlotType);
            }
        }

        public void RecoverSkill(SkillSlotType skillSlotType, ref DefaultSkillEventParam skillParam)
        {
            if ((skillSlotType > SkillSlotType.SLOT_SKILL_0) && (skillParam.param > 0))
            {
                SkillButton button = this._skillButtons[(int) skillSlotType];
                if (button != null)
                {
                    button.ChangeSkillIcon(skillParam.param);
                }
                this.SetComboEffect(skillSlotType, 0, 0);
            }
        }

        public void RequestUseSkillSlot(SkillSlotType skillSlotType, enSkillJoystickMode mode = 0, uint objID = 0)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                hostPlayer.Captain.handle.SkillControl.RequestUseSkillSlot(skillSlotType, mode, objID);
            }
        }

        private void ResetSkillJoystickSelectedTarget(CUIFormScript battleFormScript)
        {
            this.m_currentSkillJoystickSelectedIndex = -1;
            this.SetSkillIndicatorSelectedTarget(this.m_currentSkillJoystickSelectedIndex);
            if (battleFormScript != null)
            {
                GameObject widget = battleFormScript.GetWidget(0x4c);
                if (widget != null)
                {
                    CUIComponent component = widget.GetComponent<CUIComponent>();
                    if (((component != null) && (component.m_widgets != null)) && (component.m_widgets.Length >= this.m_campHeroInfos.Length))
                    {
                        for (int i = 0; i < this.m_campHeroInfos.Length; i++)
                        {
                            this.SetSkillJoystickTargetHead(battleFormScript, component.m_widgets[i], false);
                        }
                    }
                    Transform transform = widget.transform.FindChild("HighLight");
                    if (transform != null)
                    {
                        transform.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public void SendUseCommonAttack(sbyte Start, uint ObjID)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (hostPlayer.Captain != 0)) && !hostPlayer.Captain.handle.ActorControl.IsDeadState)
            {
                FrameCommand<UseCommonAttackCommand> command = FrameCommandFactory.CreateFrameCommand<UseCommonAttackCommand>();
                command.cmdData.Start = Start;
                command.cmdData.ObjID = ObjID;
                command.Send();
            }
        }

        public void SetButtonCDOver(SkillSlotType skillSlotType, bool isPlayMusic = true)
        {
            if ((skillSlotType != SkillSlotType.SLOT_SKILL_0) && this.SetEnableButton(skillSlotType))
            {
                SkillButton button = this.GetButton(skillSlotType);
                GameObject target = (button == null) ? null : button.GetAnimationCD();
                CUICommonSystem.PlayAnimation(target, enSkillButtonAnimationName.CD_End.ToString(), false);
                if (isPlayMusic)
                {
                    Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("UI_prompt_jineng");
                }
            }
        }

        public void SetButtonCDStart(SkillSlotType skillSlotType)
        {
            if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
            {
                this.SetDisableButton(skillSlotType);
                SkillButton button = this.GetButton(skillSlotType);
                GameObject target = (button == null) ? null : button.GetAnimationCD();
                CUICommonSystem.PlayAnimation(target, enSkillButtonAnimationName.CD_Star.ToString(), false);
            }
        }

        public void SetButtonHighLight(SkillSlotType skillSlotType, bool highLight)
        {
            SkillButton button = this.GetButton(skillSlotType);
            if ((button != null) && (button.m_button != null))
            {
                this.SetButtonHighLight(button.m_button, highLight);
            }
        }

        public void SetButtonHighLight(GameObject button, bool highLight)
        {
            Transform transform = button.transform.FindChild("Present/highlighter");
            if (transform != null)
            {
                transform.gameObject.CustomSetActive(highLight);
            }
        }

        private void SetComboEffect(SkillSlotType skillSlotType, int leftTime, int totalTime)
        {
            SkillButton button = this.GetButton(skillSlotType);
            if ((button != null) && (null != button.m_button))
            {
                button.effectTimeTotal = totalTime;
                button.effectTimeLeft = leftTime;
                GameObject obj2 = Utility.FindChildSafe(button.m_button, "Present/comboCD");
                if (obj2 != null)
                {
                    if ((button.effectTimeLeft > 0) && (button.effectTimeTotal > 0))
                    {
                        obj2.CustomSetActive(true);
                        button.effectTimeImage = obj2.GetComponent<Image>();
                    }
                    else
                    {
                        obj2.CustomSetActive(false);
                        button.effectTimeImage = null;
                    }
                }
            }
        }

        public void SetCommonAtkBtnState(CommonAttactType byAtkType)
        {
            CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
            if (fightFormScript != null)
            {
                GameObject widget = fightFormScript.GetWidget(0x44);
                GameObject obj3 = fightFormScript.GetWidget(0x39);
                if (((widget != null) && (obj3 != null)) && (obj3.GetComponent<CUIEventScript>() != null))
                {
                    if (byAtkType == CommonAttactType.Type1)
                    {
                        widget.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                    }
                    else if (byAtkType == CommonAttactType.Type2)
                    {
                        widget.CustomSetActive(true);
                        obj3.CustomSetActive(true);
                        bool bActive = false;
                        SkillButton button = this.GetButton(SkillSlotType.SLOT_SKILL_0);
                        if (button != null)
                        {
                            GameObject disableButton = button.GetDisableButton();
                            if (disableButton != null)
                            {
                                bActive = disableButton.activeSelf;
                            }
                        }
                        this.SetSelectTargetBtnState(bActive);
                    }
                    Singleton<EventRouter>.GetInstance().BroadCastEvent("CommonAttack_Type_Changed");
                }
            }
        }

        public void SetDisableButton(SkillSlotType skillSlotType)
        {
            CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
            if (fightFormScript != null)
            {
                if (this.m_currentSkillSlotType == skillSlotType)
                {
                    this.SkillButtonUp(fightFormScript, skillSlotType, false, new Vector2());
                }
                SkillButton button = this.GetButton(skillSlotType);
                if (button != null)
                {
                    if (button.m_button != null)
                    {
                        CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                        if (component != null)
                        {
                            if (component.ClearInputStatus())
                            {
                                Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
                            }
                            component.enabled = false;
                        }
                    }
                    button.bDisableFlag = true;
                    if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                    {
                        CUICommonSystem.PlayAnimation(button.GetAnimationPresent(), enSkillButtonAnimationName.disable.ToString(), false);
                    }
                    else
                    {
                        GameObject animationPresent = button.GetAnimationPresent();
                        if (animationPresent != null)
                        {
                            Image image = animationPresent.GetComponent<Image>();
                            if (image != null)
                            {
                                image.color = CUIUtility.s_Color_DisableGray;
                            }
                        }
                        GameObject obj4 = button.GetDisableButton();
                        if (obj4 != null)
                        {
                            obj4.CustomSetActive(true);
                        }
                        this.SetSelectTargetBtnState(true);
                    }
                    GameObject disableButton = button.GetDisableButton();
                    if (disableButton != null)
                    {
                        disableButton.CustomSetActive(true);
                    }
                }
            }
        }

        public bool SetEnableButton(SkillSlotType skillSlotType)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                SkillSlot slot;
                if (!hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out slot))
                {
                    return false;
                }
                if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
                {
                    if (hostPlayer.Captain.handle.ActorControl.IsDeadState)
                    {
                        return false;
                    }
                }
                else if (!slot.EnableButtonFlag)
                {
                    return false;
                }
            }
            SkillButton button = this.GetButton(skillSlotType);
            if (button != null)
            {
                if (!button.bLimitedFlag && (button.m_button != null))
                {
                    CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                    if (component != null)
                    {
                        if (component.ClearInputStatus())
                        {
                            Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
                        }
                        component.enabled = true;
                    }
                }
                button.bDisableFlag = false;
                if (!button.bLimitedFlag && (skillSlotType != SkillSlotType.SLOT_SKILL_0))
                {
                    CUICommonSystem.PlayAnimation(button.GetAnimationPresent(), enSkillButtonAnimationName.normal.ToString(), false);
                }
                else if (!button.bLimitedFlag && (skillSlotType == SkillSlotType.SLOT_SKILL_0))
                {
                    GameObject animationPresent = button.GetAnimationPresent();
                    if (animationPresent != null)
                    {
                        Image image = animationPresent.GetComponent<Image>();
                        if (image != null)
                        {
                            image.color = CUIUtility.s_Color_Full;
                        }
                    }
                    GameObject obj4 = button.GetDisableButton();
                    if (obj4 != null)
                    {
                        obj4.CustomSetActive(false);
                    }
                    this.SetSelectTargetBtnState(false);
                }
                GameObject disableButton = button.GetDisableButton();
                if (disableButton != null)
                {
                    CUIEventScript script2 = disableButton.GetComponent<CUIEventScript>();
                    if (script2 != null)
                    {
                        if (script2.ClearInputStatus())
                        {
                            Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
                        }
                        script2.enabled = true;
                    }
                    disableButton.CustomSetActive(false);
                }
            }
            return true;
        }

        public void SetEnergyDisableButton(SkillSlotType skillSlotType)
        {
            if (Singleton<CBattleSystem>.GetInstance().FightFormScript != null)
            {
                SkillButton button = this.GetButton(skillSlotType);
                if (button != null)
                {
                    if (button.m_button != null)
                    {
                        CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                        if (component != null)
                        {
                            component.enabled = false;
                        }
                    }
                    button.bDisableFlag = true;
                    CUICommonSystem.PlayAnimation(button.GetAnimationPresent(), enSkillButtonAnimationName.disable.ToString(), false);
                    GameObject disableButton = button.GetDisableButton();
                    if (disableButton != null)
                    {
                        disableButton.CustomSetActive(true);
                    }
                }
            }
        }

        public void SetlearnBtnHighLight(GameObject learnBtn, bool highLight)
        {
            Transform transform = learnBtn.transform.FindChild("highlighter");
            if (transform != null)
            {
                transform.gameObject.CustomSetActive(highLight);
            }
        }

        public void SetLimitButton(SkillSlotType skillSlotType)
        {
            CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
            if (fightFormScript != null)
            {
                if (this.m_currentSkillSlotType == skillSlotType)
                {
                    this.SkillButtonUp(fightFormScript, skillSlotType, false, new Vector2());
                }
                SkillButton button = this.GetButton(skillSlotType);
                DebugHelper.Assert(button != null);
                if (button != null)
                {
                    button.bLimitedFlag = true;
                    if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                    {
                        if (button.m_button != null)
                        {
                            CUIEventScript component = button.m_button.GetComponent<CUIEventScript>();
                            if (component != null)
                            {
                                if (component.ClearInputStatus())
                                {
                                    Singleton<CBattleSystem>.GetInstance().FightForm.HideSkillDescInfo();
                                }
                                component.enabled = false;
                            }
                        }
                        GameObject gameObject = button.GetAnimationPresent().transform.Find("disableFrame").gameObject;
                        DebugHelper.Assert(gameObject != null);
                        if (gameObject != null)
                        {
                            gameObject.CustomSetActive(true);
                        }
                        CUICommonSystem.PlayAnimation(button.GetAnimationPresent(), enSkillButtonAnimationName.disable.ToString(), false);
                    }
                }
            }
        }

        private void SetSelectTargetBtnState(bool bActive)
        {
            if (GameSettings.TheCommonAttackType == CommonAttactType.Type2)
            {
                CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
                if (fightFormScript != null)
                {
                    GameObject widget = fightFormScript.GetWidget(0x44);
                    GameObject obj3 = fightFormScript.GetWidget(0x39);
                    if ((widget != null) && (obj3 != null))
                    {
                        Color color = CUIUtility.s_Color_Full;
                        if (bActive)
                        {
                            color = CUIUtility.s_Color_DisableGray;
                        }
                        GameObject gameObject = obj3.transform.FindChild("disable").gameObject;
                        if (gameObject != null)
                        {
                            gameObject.CustomSetActive(bActive);
                        }
                        GameObject obj5 = obj3.transform.FindChild("Present").gameObject;
                        if (obj5 != null)
                        {
                            Image component = obj5.GetComponent<Image>();
                            if (component != null)
                            {
                                component.color = color;
                            }
                        }
                        gameObject = widget.transform.FindChild("disable").gameObject;
                        if (gameObject != null)
                        {
                            gameObject.CustomSetActive(bActive);
                        }
                        obj5 = widget.transform.FindChild("Present").gameObject;
                        if (obj5 != null)
                        {
                            Image image2 = obj5.GetComponent<Image>();
                            if (image2 != null)
                            {
                                image2.color = color;
                            }
                        }
                    }
                }
            }
        }

        public void SetSkillIndicatorMode(enSkillIndicatorMode indicaMode)
        {
            this.m_skillIndicatorMode = indicaMode;
        }

        private void SetSkillIndicatorSelectedTarget(int index)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                SkillSlot skillSlot = hostPlayer.Captain.handle.SkillControl.GetSkillSlot(this.m_currentSkillSlotType);
                if ((skillSlot != null) && (skillSlot.skillIndicator != null))
                {
                    if ((index < 0) || (index >= this.m_campHeroInfos.Length))
                    {
                        skillSlot.skillIndicator.SetUseSkillTarget(null);
                    }
                    else
                    {
                        PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(this.m_campHeroInfos[this.m_currentSkillJoystickSelectedIndex].m_objectID);
                        if (actor != 0)
                        {
                            skillSlot.skillIndicator.SetUseSkillTarget(actor.handle);
                        }
                        else
                        {
                            skillSlot.skillIndicator.SetUseSkillTarget(null);
                        }
                    }
                }
            }
        }

        private void SetSkillJoystickTargetHead(CUIFormScript battleFormScript, GameObject head, bool selected)
        {
            if (head != null)
            {
                head.transform.localScale = new Vector3(!selected ? 1f : 1.3f, !selected ? 1f : 1.3f, !selected ? 1f : 1.3f);
                CUIComponent component = head.GetComponent<CUIComponent>();
                if (((component != null) && (component.m_widgets != null)) && (component.m_widgets.Length >= 2))
                {
                    GameObject obj2 = component.m_widgets[1];
                    if (obj2 != null)
                    {
                        Image image = obj2.GetComponent<Image>();
                        if (image != null)
                        {
                            image.SetSprite(!selected ? "UGUI/Sprite/Battle/Battle_skillFrameBg_new.prefab" : "UGUI/Sprite/Battle/Battle_ComboCD.prefab", battleFormScript, true, false, false);
                        }
                    }
                }
            }
        }

        public void SkillButtonDown(CUIFormScript formScript, SkillSlotType skillSlotType, Vector2 downScreenPosition)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                int skillLevel = 0;
                if (((hostPlayer.Captain.handle.SkillControl != null) && (skillSlotType >= SkillSlotType.SLOT_SKILL_0)) && ((skillSlotType < SkillSlotType.SLOT_SKILL_COUNT) && (hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType] != null)))
                {
                    skillLevel = hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType].GetSkillLevel();
                }
                if (skillLevel <= 0)
                {
                    return;
                }
            }
            if (this.m_currentSkillSlotType != SkillSlotType.SLOT_SKILL_COUNT)
            {
                this.SkillButtonUp(formScript, this.m_currentSkillSlotType, false, new Vector2());
            }
            this.m_currentSkillSlotType = skillSlotType;
            this.m_currentSkillDownScreenPosition = downScreenPosition;
            this.m_currentSkillIndicatorEnabled = false;
            this.m_currentSkillIndicatorJoystickEnabled = false;
            this.m_currentSkillIndicatorInCancelArea = false;
            this.m_commonAtkSlide = false;
            GameObject animationPresent = this.GetButton(skillSlotType).GetAnimationPresent();
            if (hostPlayer != null)
            {
                if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
                {
                    this.SendUseCommonAttack(1, 0);
                    Singleton<CUIParticleSystem>.GetInstance().AddParticle(CUIParticleSystem.s_particleSkillBtnEffect_Path, 0.5f, animationPresent, formScript);
                }
                else
                {
                    if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                    {
                        CUICommonSystem.PlayAnimation(animationPresent, enSkillButtonAnimationName.pressDown.ToString(), false);
                    }
                    this.ReadyUseSkillSlot(skillSlotType);
                    this.EnableSkillCursor(formScript, ref downScreenPosition, this.IsUseSkillCursorJoystick(skillSlotType), skillSlotType, skillSlotType != SkillSlotType.SLOT_SKILL_0);
                }
            }
        }

        public void SkillButtonUp(CUIFormScript formScript)
        {
            if ((this.m_currentSkillSlotType != SkillSlotType.SLOT_SKILL_COUNT) && (formScript != null))
            {
                this.SkillButtonUp(formScript, this.m_currentSkillSlotType, false, new Vector2());
            }
        }

        public void SkillButtonUp(CUIFormScript formScript, SkillSlotType skillSlotType, bool isTriggeredActively, [Optional] Vector2 screenPosition)
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
            if (((hostPlayer != null) && (this.m_currentSkillSlotType == skillSlotType)) && (formScript != null))
            {
                if (hostPlayer.Captain != 0)
                {
                    int skillLevel = 0;
                    if (((hostPlayer.Captain.handle.SkillControl != null) && (skillSlotType >= SkillSlotType.SLOT_SKILL_0)) && ((skillSlotType < SkillSlotType.SLOT_SKILL_COUNT) && (hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType] != null)))
                    {
                        skillLevel = hostPlayer.Captain.handle.SkillControl.SkillSlotArray[(int) skillSlotType].GetSkillLevel();
                    }
                    if (skillLevel <= 0)
                    {
                        return;
                    }
                }
                SkillButton button = this.GetButton(skillSlotType);
                if (button != null)
                {
                    GameObject animationPresent = button.GetAnimationPresent();
                    if (skillSlotType == SkillSlotType.SLOT_SKILL_0)
                    {
                        if (this.m_commonAtkSlide)
                        {
                            this.CommonAtkSlide(formScript, screenPosition);
                            this.m_commonAtkSlide = false;
                        }
                        this.SendUseCommonAttack(0, 0);
                    }
                    else
                    {
                        if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                        {
                            CUICommonSystem.PlayAnimation(animationPresent, enSkillButtonAnimationName.pressUp.ToString(), false);
                        }
                        if (isTriggeredActively && !this.m_currentSkillIndicatorInCancelArea)
                        {
                            if (skillSlotType != SkillSlotType.SLOT_SKILL_0)
                            {
                                uint objID = 0;
                                if (this.m_currentSkillJoystickSelectedIndex != -1)
                                {
                                    objID = this.m_campHeroInfos[this.m_currentSkillJoystickSelectedIndex].m_objectID;
                                }
                                this.RequestUseSkillSlot(skillSlotType, this.m_currentSkillJoystickMode, objID);
                            }
                        }
                        else
                        {
                            this.CancelUseSkillSlot(skillSlotType, this.m_currentSkillJoystickMode);
                        }
                        if (this.m_currentSkillIndicatorEnabled)
                        {
                            this.DisableSkillCursor(formScript);
                        }
                    }
                    if ((this.m_currentSkillJoystickMode == enSkillJoystickMode.SelectTarget) && (this.m_currentSkillJoystickSelectedIndex >= 0))
                    {
                        this.m_currentSkillJoystickSelectedIndex = -1;
                        this.SetSkillIndicatorSelectedTarget(this.m_currentSkillJoystickSelectedIndex);
                    }
                    this.m_currentSkillSlotType = SkillSlotType.SLOT_SKILL_COUNT;
                    this.m_currentSkillDownScreenPosition = Vector2.zero;
                }
            }
        }

        private int SkillJoystickSelectTarget(CUIFormScript battleFormScript, GameObject skillJoystick, ref Vector2 screenPosition)
        {
            Vector2 point = screenPosition - this.m_currentSkillIndicatorScreenPosition;
            if (battleFormScript.ChangeScreenValueToForm(point.magnitude) <= 270f)
            {
                float radian = GetRadian(point);
                if ((battleFormScript != null) && (skillJoystick != null))
                {
                    CUIComponent component = skillJoystick.GetComponent<CUIComponent>();
                    if (((component != null) && (component.m_widgets != null)) && (component.m_widgets.Length >= this.m_campHeroInfos.Length))
                    {
                        for (int i = 0; i < this.m_campHeroInfos.Length; i++)
                        {
                            GameObject obj2 = component.m_widgets[i];
                            if (((obj2 != null) && obj2.activeSelf) && ((0 != 0) || ((radian >= this.m_campHeroInfos[i].m_headAreaFan.m_minRadian) && (radian <= this.m_campHeroInfos[i].m_headAreaFan.m_maxRadian))))
                            {
                                return i;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        public void UpdateButtonCD(SkillSlotType skillSlotType, int cd)
        {
            SkillButton button = this.GetButton(skillSlotType);
            if (cd <= 0)
            {
                this.SetEnableButton(skillSlotType);
            }
            this.UpdateButtonCDText((button == null) ? null : button.m_button, (button == null) ? null : button.m_cdText, cd);
        }

        private void UpdateButtonCDText(GameObject button, GameObject cdText, int cd)
        {
            if (cdText != null)
            {
                if (cd <= 0)
                {
                    cdText.CustomSetActive(false);
                }
                else
                {
                    cdText.CustomSetActive(true);
                    Text component = cdText.GetComponent<Text>();
                    if (component != null)
                    {
                        component.text = SimpleNumericString.GetNumeric(Mathf.CeilToInt((float) (cd / 0x3e8)) + 1);
                    }
                }
            }
            if ((button != null) && (cdText != null))
            {
                cdText.transform.position = button.transform.position;
            }
        }

        public void UpdateLogic(int delta)
        {
            for (int i = 0; i < this._skillButtons.Length; i++)
            {
                SkillButton button = this._skillButtons[i];
                if ((button != null) && (null != button.effectTimeImage))
                {
                    button.effectTimeLeft -= delta;
                    if (button.effectTimeLeft < 0)
                    {
                        button.effectTimeLeft = 0;
                    }
                    button.effectTimeImage.CustomFillAmount(((float) button.effectTimeLeft) / ((float) button.effectTimeTotal));
                    if (button.effectTimeLeft <= 0)
                    {
                        button.effectTimeTotal = 0;
                        button.effectTimeImage.gameObject.CustomSetActive(false);
                        button.effectTimeImage = null;
                    }
                }
            }
        }

        public bool CurrentSkillIndicatorResponed
        {
            get
            {
                return this.m_currentSkillIndicatorResponed;
            }
        }

        public bool CurrentSkillTipsResponed
        {
            get
            {
                return this.m_currentSkillTipsResponed;
            }
        }
    }
}

