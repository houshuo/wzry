namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CCheatSystem : Singleton<CCheatSystem>
    {
        private bool m_cheatFormHasBeenOpend;
        public bool m_enabled;
        private string m_errorLog = string.Empty;
        private OnDisable m_onDisable;
        private List<GameObject> m_triggers = new List<GameObject>();
        private static string s_cheatErrorLogFormPath = "UGUI/Form/System/Cheat/Form_CheatErrorLog.prefab";
        private static string s_cheatFormPath = "UGUI/Form/System/Cheat/Form_Cheat.prefab";
        private static string s_cheatTriggerFormPath = "UGUI/Form/System/Cheat/Form_CheatTrigger.prefab";
        private static string[] s_errorLogFlagName = new string[] { "显示ErrorLog" };
        private static bool[] s_errorLogFlags = new bool[s_errorLogFlagName.Length];
        private static string[] s_ignoreMaintainName = new string[] { "不选可以忽略维护状态" };
        private static string[] s_joystickConfigNames = new string[] { "强制跟随移动" };
        private static bool[] s_joystickConfigs = new bool[s_joystickConfigNames.Length];
        private static string s_rmsJoystickForceMoveable = "RMS_JoystickForceMoveable";
        private static string[] s_TDirServerTypeName = new string[] { "使用默认", "测试服", "中转服", "正式服", "体验版正式服", "体验版中转服", "测试专用服", "比赛测试服", "比赛正式服" };
        private static string[] s_tversionServerTypeName = new string[] { "正式服", "中转服", "测试服", "体验版正式服", "体验版中转服", "测试专用服", "比赛正式服", "比赛测试服", "跳过更新" };

        public void CloseCheatTriggerForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_cheatTriggerFormPath);
        }

        public override void Init()
        {
            s_joystickConfigs[0] = PlayerPrefs.GetInt(s_rmsJoystickForceMoveable, 0) == 1;
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_TriggerDown, new CUIEventManager.OnUIEventHandler(this.OnCheatTriggerDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_TriggerUp, new CUIEventManager.OnUIEventHandler(this.OnCheatTriggerUp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_OnIIPSServerSelectChanged, new CUIEventManager.OnUIEventHandler(this.OnCheatOnIIPSServerSelectChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_CheatFormClosed, new CUIEventManager.OnUIEventHandler(this.OnCheatFormClosed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_ClearCache, new CUIEventManager.OnUIEventHandler(this.OnClearCache));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_TDirChanged, new CUIEventManager.OnUIEventHandler(this.OnCheatOnTDirSelectChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_OnErrorLogSelectChanged, new CUIEventManager.OnUIEventHandler(this.OnCheatErrorLogSelectChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_HideErrorLogPanel, new CUIEventManager.OnUIEventHandler(this.OnCheatHideErrorLogPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_AppearErrorLogPanel, new CUIEventManager.OnUIEventHandler(this.OnCheatAppearErrorLogPanel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_ClearErrorLog, new CUIEventManager.OnUIEventHandler(this.OnCheatClearErrorLog));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_MaintainBlock, new CUIEventManager.OnUIEventHandler(this.OnCheatMaintainBlock));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Cheat_JoystickConfigChanged, new CUIEventManager.OnUIEventHandler(this.OnCheatJoystickConfigChanged));
        }

        public bool IsDisplayErrorLog()
        {
            return s_errorLogFlags[0];
        }

        public static bool IsJoystickForceMoveable()
        {
            return s_joystickConfigs[0];
        }

        private void OnCheatAppearErrorLogPanel(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            srcFormScript.GetWidget(0).CustomSetActive(true);
            srcFormScript.GetWidget(2).CustomSetActive(true);
            srcFormScript.GetWidget(3).CustomSetActive(false);
        }

        private void OnCheatClearErrorLog(CUIEvent uiEvent)
        {
            this.m_errorLog = string.Empty;
            Text component = uiEvent.m_srcFormScript.GetWidget(1).GetComponent<Text>();
            if (component != null)
            {
                component.text = string.Empty;
            }
        }

        private void OnCheatErrorLogSelectChanged(CUIEvent uiEvent)
        {
            bool[] multiSelected = ((CUIToggleListScript) uiEvent.m_srcWidgetScript).GetMultiSelected();
            for (int i = 0; i < s_errorLogFlags.Length; i++)
            {
                s_errorLogFlags[i] = multiSelected[i];
            }
        }

        private void OnCheatFormClosed(CUIEvent uiEvent)
        {
            this.m_enabled = false;
            if (this.m_onDisable != null)
            {
                this.m_onDisable();
            }
        }

        private void OnCheatHideErrorLogPanel(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            srcFormScript.GetWidget(0).CustomSetActive(false);
            srcFormScript.GetWidget(2).CustomSetActive(false);
            srcFormScript.GetWidget(3).CustomSetActive(true);
        }

        private void OnCheatJoystickConfigChanged(CUIEvent uiEvent)
        {
            bool[] multiSelected = ((CUIToggleListScript) uiEvent.m_srcWidgetScript).GetMultiSelected();
            for (int i = 0; i < multiSelected.Length; i++)
            {
                s_joystickConfigs[i] = multiSelected[i];
                if (i == 0)
                {
                    PlayerPrefs.SetInt(s_rmsJoystickForceMoveable, !s_joystickConfigs[i] ? 0 : 1);
                }
            }
            PlayerPrefs.Save();
        }

        private void OnCheatMaintainBlock(CUIEvent uiEvent)
        {
            bool[] multiSelected = ((CUIToggleListScript) uiEvent.m_srcWidgetScript).GetMultiSelected();
            if (multiSelected.Length > 0)
            {
                TdirMgr.s_maintainBlock = multiSelected[0];
            }
        }

        private void OnCheatOnIIPSServerSelectChanged(CUIEvent uiEvent)
        {
            enIIPSServerType selected = (enIIPSServerType) ((CUIToggleListScript) uiEvent.m_srcWidgetScript).GetSelected();
            if (selected >= enIIPSServerType.None)
            {
                selected = enIIPSServerType.Test;
                ((CUIToggleListScript) uiEvent.m_srcWidgetScript).SetSelected((int) selected);
            }
            CVersionUpdateSystem.SetIIPSServerType(selected);
        }

        private void OnCheatOnTDirSelectChanged(CUIEvent uiEvent)
        {
            TdirConfig.cheatServerType = (TdirServerType) ((CUIToggleListScript) uiEvent.m_srcWidgetScript).GetSelected();
        }

        private void OnCheatTriggerDown(CUIEvent uiEvent)
        {
            if (!this.m_triggers.Contains(uiEvent.m_srcWidget))
            {
                this.m_triggers.Add(uiEvent.m_srcWidget);
            }
            if (this.m_triggers.Count >= 5)
            {
                if (!this.m_cheatFormHasBeenOpend)
                {
                    this.OpenCheatForm();
                    this.m_cheatFormHasBeenOpend = true;
                }
                this.m_triggers.Clear();
            }
        }

        private void OnCheatTriggerUp(CUIEvent uiEvent)
        {
            if (this.m_triggers.Contains(uiEvent.m_srcWidget))
            {
                this.m_triggers.Remove(uiEvent.m_srcWidget);
            }
        }

        private void OnClearCache(CUIEvent uiEvent)
        {
            if (MonoSingleton<CVersionUpdateSystem>.GetInstance().ClearCachePath())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("缓存清理成功！", false, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips("缓存清理失败！", false, 1.5f, null, new object[0]);
            }
        }

        private void OpenCheatForm()
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_cheatFormPath, false, false);
            if (script != null)
            {
                this.m_enabled = true;
                GameObject widget = script.GetWidget(0);
                if (widget != null)
                {
                    CUIToggleListScript component = widget.GetComponent<CUIToggleListScript>();
                    if (component != null)
                    {
                        component.SetElementAmount(s_tversionServerTypeName.Length);
                        for (int i = 0; i < s_tversionServerTypeName.Length; i++)
                        {
                            Transform transform = component.GetElemenet(i).gameObject.transform.Find("Label");
                            if (transform != null)
                            {
                                Text text = transform.gameObject.GetComponent<Text>();
                                if (text != null)
                                {
                                    text.text = s_tversionServerTypeName[i];
                                    if (i == 8)
                                    {
                                        text.color = new Color(1f, 0f, 0f, 1f);
                                    }
                                }
                            }
                        }
                        component.SetSelected((int) CVersionUpdateSystem.GetIIPSServerType());
                    }
                }
                GameObject obj3 = script.GetWidget(1);
                if (obj3 != null)
                {
                    CUIToggleListScript script4 = obj3.GetComponent<CUIToggleListScript>();
                    if (script4 != null)
                    {
                        script4.SetElementAmount(s_TDirServerTypeName.Length);
                        for (int j = 0; j < s_TDirServerTypeName.Length; j++)
                        {
                            Transform transform2 = script4.GetElemenet(j).gameObject.transform.Find("Label");
                            if (transform2 != null)
                            {
                                Text text2 = transform2.gameObject.GetComponent<Text>();
                                if (text2 != null)
                                {
                                    text2.text = s_TDirServerTypeName[j];
                                    if (j == 0)
                                    {
                                        text2.color = new Color(1f, 0f, 0f, 1f);
                                    }
                                }
                            }
                        }
                        script4.SetSelected((int) TdirConfig.cheatServerType);
                    }
                }
                GameObject obj4 = script.GetWidget(2);
                if (obj4 != null)
                {
                    CUIToggleListScript script6 = obj4.GetComponent<CUIToggleListScript>();
                    if (script6 != null)
                    {
                        script6.SetElementAmount(s_errorLogFlagName.Length);
                        for (int k = 0; k < s_errorLogFlagName.Length; k++)
                        {
                            Transform transform3 = script6.GetElemenet(k).gameObject.transform.Find("Label");
                            if (transform3 != null)
                            {
                                Text text3 = transform3.gameObject.GetComponent<Text>();
                                if (text3 != null)
                                {
                                    text3.text = s_errorLogFlagName[k];
                                }
                            }
                        }
                        for (int m = 0; m < s_errorLogFlags.Length; m++)
                        {
                            script6.SetMultiSelected(m, s_errorLogFlags[m]);
                        }
                    }
                }
                GameObject obj5 = script.GetWidget(3);
                if (obj5 != null)
                {
                    CUIToggleListScript script8 = obj5.GetComponent<CUIToggleListScript>();
                    if (script8 != null)
                    {
                        script8.SetElementAmount(s_ignoreMaintainName.Length);
                        for (int n = 0; n < s_ignoreMaintainName.Length; n++)
                        {
                            Transform transform4 = script8.GetElemenet(n).gameObject.transform.Find("Label");
                            if (transform4 != null)
                            {
                                Text text4 = transform4.gameObject.GetComponent<Text>();
                                if (text4 != null)
                                {
                                    text4.text = s_ignoreMaintainName[n];
                                }
                            }
                        }
                        script8.SetMultiSelected(0, TdirMgr.s_maintainBlock);
                    }
                }
                GameObject obj6 = script.GetWidget(4);
                if (obj6 != null)
                {
                    CUIToggleListScript script10 = obj6.GetComponent<CUIToggleListScript>();
                    if (script10 != null)
                    {
                        script10.SetElementAmount(s_joystickConfigNames.Length);
                        for (int num6 = 0; num6 < s_joystickConfigNames.Length; num6++)
                        {
                            Transform transform5 = script10.GetElemenet(num6).gameObject.transform.Find("Label");
                            if (transform5 != null)
                            {
                                Text text5 = transform5.gameObject.GetComponent<Text>();
                                if (text5 != null)
                                {
                                    text5.text = s_joystickConfigNames[num6];
                                }
                            }
                            if (num6 == 0)
                            {
                                script10.SetMultiSelected(num6, s_joystickConfigs[num6]);
                            }
                        }
                    }
                }
            }
        }

        public void OpenCheatTriggerForm(OnDisable onDisable)
        {
            Singleton<CUIManager>.GetInstance().OpenForm(s_cheatTriggerFormPath, false, false);
            this.m_onDisable = onDisable;
        }

        public void RecordErrorLog(string errorLog)
        {
            if (this.IsDisplayErrorLog())
            {
                this.m_errorLog = this.m_errorLog + errorLog;
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_cheatErrorLogFormPath, false, false);
                if (script != null)
                {
                    Text component = script.GetWidget(1).GetComponent<Text>();
                    if (component != null)
                    {
                        component.text = this.m_errorLog;
                    }
                }
            }
        }

        public override void UnInit()
        {
        }

        public delegate void OnDisable();
    }
}

