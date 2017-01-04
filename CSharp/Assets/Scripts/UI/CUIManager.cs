namespace Assets.Scripts.UI
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.Sound;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CUIManager : Singleton<CUIManager>
    {
        private const int c_formCameraDepth = 10;
        private const int c_formCameraMaskLayer = 5;
        private Camera m_formCamera;
        private int m_formOpenOrder;
        private ListView<CUIFormScript> m_forms;
        private int m_formSequence;
        private bool m_needSortForms;
        private bool m_needUpdateRaycasterAndHide;
        private ListView<CUIFormScript> m_pooledForms;
        private EventSystem m_uiInputEventSystem;
        private GameObject m_uiRoot;
        public OnFormSorted onFormSorted;
        private static string s_formCameraName = "Camera_Form";
        private static string s_uiSceneName = "UI_Scene";
        public static int s_uiSystemRenderFrameCounter;

        public void ClearFormPool()
        {
            for (int i = 0; i < this.m_pooledForms.Count; i++)
            {
                UnityEngine.Object.Destroy(this.m_pooledForms[i].gameObject);
            }
            this.m_pooledForms.Clear();
        }

        public void CloseAllForm(string[] exceptFormNames = null, bool closeImmediately = true, bool clearFormPool = true)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                bool flag = true;
                if (exceptFormNames != null)
                {
                    for (int j = 0; j < exceptFormNames.Length; j++)
                    {
                        if (string.Equals(this.m_forms[i].m_formPath, exceptFormNames[j]))
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    this.m_forms[i].Close();
                }
            }
            if (closeImmediately)
            {
                int index = 0;
                while (index < this.m_forms.Count)
                {
                    if (this.m_forms[index].IsNeedClose() || this.m_forms[index].IsClosed())
                    {
                        if (this.m_forms[index].IsNeedClose())
                        {
                            this.m_forms[index].TurnToClosed(true);
                        }
                        this.RecycleForm(this.m_forms[index]);
                        this.m_forms.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
                if (exceptFormNames != null)
                {
                    this.ProcessFormList(true, true);
                }
            }
            if (clearFormPool)
            {
                this.ClearFormPool();
            }
        }

        public void CloseAllFormExceptLobby(bool closeImmediately = true)
        {
            string[] exceptFormNames = new string[] { CLobbySystem.LOBBY_FORM_PATH, CLobbySystem.SYSENTRY_FORM_PATH, CChatController.ChatFormPath, CLobbySystem.RANKING_BTN_FORM_PATH };
            Singleton<CUIManager>.GetInstance().CloseAllForm(exceptFormNames, closeImmediately, true);
        }

        public void CloseForm(CUIFormScript formScript)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (this.m_forms[i] == formScript)
                {
                    this.m_forms[i].Close();
                }
            }
        }

        public void CloseForm(int formSequence)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (this.m_forms[i].GetSequence() == formSequence)
                {
                    this.m_forms[i].Close();
                }
            }
        }

        public void CloseForm(string formPath)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (string.Equals(this.m_forms[i].m_formPath, formPath))
                {
                    this.m_forms[i].Close();
                }
            }
        }

        public void CloseGroupForm(int group)
        {
            if (group != 0)
            {
                for (int i = 0; i < this.m_forms.Count; i++)
                {
                    if (this.m_forms[i].m_group == group)
                    {
                        this.m_forms[i].Close();
                    }
                }
            }
        }

        public void CloseMessageBox()
        {
            Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_MessageBox.prefab");
        }

        public void CloseSendMsgAlert()
        {
            CUIEvent uiEvent = new CUIEvent {
                m_eventID = enUIEventID.Common_SendMsgAlertClose
            };
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        public void CloseSmallMessageBox()
        {
            Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_SmallMessageBox.prefab");
        }

        public void CloseTips()
        {
            this.CloseForm("UGUI/Form/Common/Form_Tips.prefab");
        }

        private void CreateCamera()
        {
            GameObject obj2 = new GameObject(s_formCameraName);
            obj2.transform.SetParent(this.m_uiRoot.transform, true);
            obj2.transform.localPosition = Vector3.zero;
            obj2.transform.localRotation = Quaternion.identity;
            obj2.transform.localScale = Vector3.one;
            Camera camera = obj2.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 50f;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.cullingMask = 0x20;
            camera.depth = 10f;
            this.m_formCamera = camera;
        }

        private void CreateEventSystem()
        {
            this.m_uiInputEventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
            if (this.m_uiInputEventSystem == null)
            {
                GameObject obj2 = new GameObject("EventSystem");
                this.m_uiInputEventSystem = obj2.AddComponent<EventSystem>();
                obj2.AddComponent<TouchInputModule>();
            }
            this.m_uiInputEventSystem.gameObject.transform.parent = this.m_uiRoot.transform;
        }

        private GameObject CreateForm(string formPrefabPath, bool useFormPool)
        {
            GameObject gameObject = null;
            if (useFormPool)
            {
                for (int i = 0; i < this.m_pooledForms.Count; i++)
                {
                    if (string.Equals(formPrefabPath, this.m_pooledForms[i].m_formPath, StringComparison.OrdinalIgnoreCase))
                    {
                        this.m_pooledForms[i].Appear(enFormHideFlag.HideByCustom, true);
                        gameObject = this.m_pooledForms[i].gameObject;
                        this.m_pooledForms.RemoveAt(i);
                        break;
                    }
                }
            }
            if (gameObject == null)
            {
                GameObject content = (GameObject) Singleton<CResourceManager>.GetInstance().GetResource(formPrefabPath, typeof(GameObject), enResourceType.UIForm, false, false).m_content;
                if (content == null)
                {
                    return null;
                }
                gameObject = (GameObject) UnityEngine.Object.Instantiate(content);
            }
            if (gameObject != null)
            {
                CUIFormScript component = gameObject.GetComponent<CUIFormScript>();
                if (component != null)
                {
                    component.m_useFormPool = useFormPool;
                }
            }
            return gameObject;
        }

        private void CreateUIRoot()
        {
            this.m_uiRoot = new GameObject("CUIManager");
            GameObject obj2 = GameObject.Find("BootObj");
            if (obj2 != null)
            {
                this.m_uiRoot.transform.parent = obj2.transform;
            }
        }

        private void CreateUISecene()
        {
            GameObject obj2 = new GameObject(s_uiSceneName) {
                transform = { parent = this.m_uiRoot.transform }
            };
        }

        public void DisableInput()
        {
            if (this.m_uiInputEventSystem != null)
            {
                this.m_uiInputEventSystem.gameObject.CustomSetActive(false);
            }
        }

        public void EnableInput()
        {
            if (this.m_uiInputEventSystem != null)
            {
                this.m_uiInputEventSystem.gameObject.CustomSetActive(true);
            }
        }

        public EventSystem GetEventSystem()
        {
            return this.m_uiInputEventSystem;
        }

        public CUIFormScript GetForm(int formSequence)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (((this.m_forms[i].GetSequence() == formSequence) && !this.m_forms[i].IsNeedClose()) && !this.m_forms[i].IsClosed())
                {
                    return this.m_forms[i];
                }
            }
            return null;
        }

        public CUIFormScript GetForm(string formPath)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if ((this.m_forms[i].m_formPath.Equals(formPath) && !this.m_forms[i].IsNeedClose()) && !this.m_forms[i].IsClosed())
                {
                    return this.m_forms[i];
                }
            }
            return null;
        }

        private string GetFormName(string formPath)
        {
            return CFileManager.EraseExtension(CFileManager.GetFullName(formPath));
        }

        public ListView<CUIFormScript> GetForms()
        {
            return this.m_forms;
        }

        public CUIFormScript GetTopForm()
        {
            CUIFormScript script = null;
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (this.m_forms[i] != null)
                {
                    if (script == null)
                    {
                        script = this.m_forms[i];
                    }
                    else if (this.m_forms[i].GetSortingOrder() > script.GetSortingOrder())
                    {
                        script = this.m_forms[i];
                    }
                }
            }
            return script;
        }

        private CUIFormScript GetUnClosedForm(string formPath)
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                if (this.m_forms[i].m_formPath.Equals(formPath) && !this.m_forms[i].IsClosed())
                {
                    return this.m_forms[i];
                }
            }
            return null;
        }

        public bool HasForm()
        {
            return (this.m_forms.Count > 0);
        }

        public override void Init()
        {
            this.m_forms = new ListView<CUIFormScript>();
            this.m_pooledForms = new ListView<CUIFormScript>();
            this.m_formOpenOrder = 1;
            this.m_formSequence = 0;
            s_uiSystemRenderFrameCounter = 0;
            this.CreateUIRoot();
            this.CreateEventSystem();
            this.CreateCamera();
            this.CreateUISecene();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UI_OnFormPriorityChanged, new CUIEventManager.OnUIEventHandler(this.OnFormPriorityChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UI_OnFormVisibleChanged, new CUIEventManager.OnUIEventHandler(this.OnFormVisibleChanged));
        }

        public bool IsTipsFormExist()
        {
            return (this.GetForm("UGUI/Form/Common/Form_Tips.prefab") != null);
        }

        public void LateUpdate()
        {
            for (int i = 0; i < this.m_forms.Count; i++)
            {
                this.m_forms[i].CustomLateUpdate();
            }
            s_uiSystemRenderFrameCounter++;
        }

        public void LoadSoundBank()
        {
            Singleton<CSoundManager>.GetInstance().LoadBank("UI", CSoundManager.BankType.Global);
        }

        public void LoadUIScenePrefab(string sceneName, CUIFormScript formScript)
        {
            if ((formScript != null) && !formScript.IsRelatedSceneExist(sceneName))
            {
                formScript.AddRelatedScene(CUICommonSystem.GetAnimation3DOjb(sceneName), sceneName);
            }
        }

        private void OnFormPriorityChanged(CUIEvent uiEvent)
        {
            this.m_needSortForms = true;
        }

        private void OnFormVisibleChanged(CUIEvent uiEvent)
        {
            this.m_needUpdateRaycasterAndHide = true;
        }

        public void OpenAwardTip(CUseable[] items, string title = null, bool playSound = false, enUIEventID eventID = 0, bool displayAll = false, bool forceNotGoToBag = false, string formPath = "Form_Award")
        {
            if (items != null)
            {
                int b = 10;
                int amount = Mathf.Min(items.Length, b);
                CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/" + formPath, false, true);
                if (formScript != null)
                {
                    formScript.transform.FindChild("btnGroup/Button_Back").GetComponent<CUIEventScript>().m_onClickEventID = eventID;
                    if (title != null)
                    {
                        Utility.GetComponetInChild<Text>(formScript.gameObject, "bg/Title").text = title;
                    }
                    CUIListScript component = formScript.transform.FindChild("IconContainer").gameObject.GetComponent<CUIListScript>();
                    component.SetElementAmount(amount);
                    for (int i = 0; i < amount; i++)
                    {
                        if ((component.GetElemenet(i) != null) && (items[i] != null))
                        {
                            GameObject gameObject = component.GetElemenet(i).gameObject;
                            CUICommonSystem.SetItemCell(formScript, gameObject, items[i], true, displayAll);
                            gameObject.CustomSetActive(true);
                            gameObject.transform.FindChild("ItemName").GetComponent<Text>().text = items[i].m_name;
                            if (playSound)
                            {
                                COM_REWARDS_TYPE mapRewardType = items[i].MapRewardType;
                                if (mapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN)
                                {
                                    if (mapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP)
                                    {
                                        goto Label_0162;
                                    }
                                    if (mapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND)
                                    {
                                        goto Label_014C;
                                    }
                                }
                                else
                                {
                                    Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_coin", null);
                                }
                            }
                        }
                        continue;
                    Label_014C:
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_diamond", null);
                        continue;
                    Label_0162:
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_hall_add_physical_power", null);
                    }
                    CUIEventScript script3 = formScript.transform.Find("btnGroup/Button_Use").GetComponent<CUIEventScript>();
                    script3.gameObject.CustomSetActive(false);
                    if ((!forceNotGoToBag && (amount == 1)) && (items[0].m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP))
                    {
                        CItem item = items[0] as CItem;
                        if (((item.m_itemData.bType == 4) || (item.m_itemData.bType == 1)) || (item.m_itemData.bType == 11))
                        {
                            CUseable useableByBaseID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, item.m_baseID);
                            if (useableByBaseID != null)
                            {
                                script3.gameObject.CustomSetActive(true);
                                script3.m_onClickEventParams.iconUseable = useableByBaseID;
                                script3.m_onClickEventParams.tag = Mathf.Min(item.m_stackCount, useableByBaseID.m_stackCount);
                            }
                        }
                    }
                }
            }
        }

        public void OpenEditForm(string title, string editContent, enUIEventID confirmEventId = 0)
        {
            CUIFormScript script = this.OpenForm("UGUI/Form/Common/Form_Edit.prefab", false, true);
            DebugHelper.Assert(script != null, "CUIManager.OpenEditForm(): form == null!!!");
            if (script != null)
            {
                if (title != null)
                {
                    script.GetWidget(0).GetComponent<Text>().text = title;
                }
                if (editContent != null)
                {
                    script.GetWidget(1).GetComponent<Text>().text = editContent;
                }
                script.GetWidget(2).GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, confirmEventId);
            }
        }

        public CUIFormScript OpenForm(string formPath, bool useFormPool, bool useCameraRenderMode = true)
        {
            CUIFormScript unClosedForm = this.GetUnClosedForm(formPath);
            if ((unClosedForm != null) && unClosedForm.m_isSingleton)
            {
                unClosedForm.Open(this.m_formSequence, this.m_formOpenOrder, true);
                this.m_formSequence++;
                this.m_formOpenOrder++;
                this.m_needSortForms = true;
                return unClosedForm;
            }
            GameObject obj2 = this.CreateForm(formPath, useFormPool);
            if (obj2 == null)
            {
                return null;
            }
            if (!obj2.activeSelf)
            {
                obj2.CustomSetActive(true);
            }
            string formName = this.GetFormName(formPath);
            obj2.name = formName;
            if (obj2.transform.parent != this.m_uiRoot.transform)
            {
                obj2.transform.SetParent(this.m_uiRoot.transform);
            }
            unClosedForm = obj2.GetComponent<CUIFormScript>();
            if (unClosedForm != null)
            {
                unClosedForm.Open(formPath, !useCameraRenderMode ? null : this.m_formCamera, this.m_formSequence, this.m_formOpenOrder, false);
                if (unClosedForm.m_group > 0)
                {
                    this.CloseGroupForm(unClosedForm.m_group);
                }
                this.m_forms.Add(unClosedForm);
            }
            this.m_formSequence++;
            this.m_formOpenOrder++;
            this.m_needSortForms = true;
            return unClosedForm;
        }

        public void OpenInfoForm(int txtKey)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((long) txtKey);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        public void OpenInfoForm(string title = null, string info = null)
        {
            CUIFormScript script = this.OpenForm("UGUI/Form/Common/Form_Info.prefab", false, true);
            DebugHelper.Assert(script != null, "CUIManager.OpenInfoForm(): form == null!!!");
            if (script != null)
            {
                if (title != null)
                {
                    script.GetWidget(0).GetComponent<Text>().text = title;
                }
                if (info != null)
                {
                    script.GetWidget(1).GetComponent<Text>().text = info;
                }
            }
        }

        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID)
        {
            stUIEventParams par = new stUIEventParams();
            this.OpenInputBoxBase(title, inputTip, confirmID, enUIEventID.None, par, "确定", "取消");
        }

        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, stUIEventParams par)
        {
            this.OpenInputBoxBase(title, inputTip, confirmID, enUIEventID.None, par, "确定", "取消");
        }

        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par)
        {
            this.OpenInputBoxBase(title, inputTip, confirmID, cancelID, par, "确定", "取消");
        }

        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, string confirmStr, string cancelStr)
        {
            this.OpenInputBoxBase(title, inputTip, confirmID, cancelID, par, confirmStr, cancelStr);
        }

        private void OpenInputBoxBase(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, string confirmStr = "确定", string cancelStr = "取消")
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_InputBox.prefab", false, false);
            GameObject gameObject = null;
            if (script != null)
            {
                gameObject = script.gameObject;
            }
            if (gameObject != null)
            {
                GameObject obj3 = gameObject.transform.Find("Panel/btnGroup/Button_Confirm").gameObject;
                obj3.GetComponentInChildren<Text>().text = confirmStr;
                GameObject obj4 = gameObject.transform.Find("Panel/btnGroup/Button_Cancel").gameObject;
                obj4.GetComponentInChildren<Text>().text = cancelStr;
                gameObject.transform.Find("Panel/title/Text").GetComponent<Text>().text = title;
                gameObject.transform.Find("Panel/inputText/Placeholder").GetComponent<Text>().text = inputTip;
                CUIEventScript component = obj3.GetComponent<CUIEventScript>();
                CUIEventScript script3 = obj4.GetComponent<CUIEventScript>();
                component.SetUIEvent(enUIEventType.Click, confirmID, par);
                script3.SetUIEvent(enUIEventType.Click, cancelID, par);
            }
        }

        public void OpenMessageBox(string strContent, bool isContentLeftAlign = false)
        {
            stUIEventParams par = new stUIEventParams();
            this.OpenMessageBoxBase(strContent, false, enUIEventID.None, enUIEventID.None, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBox(string strContent, enUIEventID confirmID, bool isContentLeftAlign = false)
        {
            stUIEventParams par = new stUIEventParams();
            this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, string titleStr, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr, string.Empty, titleStr, 0, enUIEventID.None);
        }

        private void OpenMessageBoxBase(string strContent, bool isHaveCancelBtn, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false, string confirmStr = "", string cancelStr = "", string titleStr = "", int autoCloseTime = 0, enUIEventID timeUpID = 0)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_MessageBox.prefab", false, false);
            if (script != null)
            {
                GameObject gameObject = script.gameObject;
                if (gameObject != null)
                {
                    if (confirmStr == string.Empty)
                    {
                        confirmStr = Singleton<CTextManager>.GetInstance().GetText("Common_Confirm");
                    }
                    if (cancelStr == string.Empty)
                    {
                        cancelStr = Singleton<CTextManager>.GetInstance().GetText("Common_Cancel");
                    }
                    if (titleStr == string.Empty)
                    {
                        titleStr = Singleton<CTextManager>.GetInstance().GetText("Common_MsgBox_Title");
                    }
                    GameObject obj3 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Confirm").gameObject;
                    obj3.GetComponentInChildren<Text>().text = confirmStr;
                    GameObject obj4 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Cancel").gameObject;
                    obj4.GetComponentInChildren<Text>().text = cancelStr;
                    gameObject.transform.Find("Panel/Panel/title/Text").gameObject.GetComponentInChildren<Text>().text = titleStr;
                    Text component = gameObject.transform.Find("Panel/Panel/Text").GetComponent<Text>();
                    component.text = strContent;
                    if (!isHaveCancelBtn)
                    {
                        obj4.CustomSetActive(false);
                    }
                    else
                    {
                        obj4.CustomSetActive(true);
                    }
                    CUIEventScript script2 = obj3.GetComponent<CUIEventScript>();
                    CUIEventScript script3 = obj4.GetComponent<CUIEventScript>();
                    script2.SetUIEvent(enUIEventType.Click, confirmID, par);
                    script3.SetUIEvent(enUIEventType.Click, cancelID, par);
                    if (isContentLeftAlign)
                    {
                        component.alignment = TextAnchor.MiddleLeft;
                    }
                    if (autoCloseTime != 0)
                    {
                        Transform transform = script.transform.Find("closeTimer");
                        if (transform != null)
                        {
                            CUITimerScript script4 = transform.GetComponent<CUITimerScript>();
                            if (script4 != null)
                            {
                                script4.SetTotalTime((float) autoCloseTime);
                                script4.StartTimer();
                                script4.m_eventIDs[1] = timeUpID;
                                script4.m_eventParams[1] = par;
                            }
                        }
                    }
                    this.CloseSendMsgAlert();
                }
            }
        }

        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, new stUIEventParams(), isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, string confirmStr, string cancelStr, bool isContentLeftAlign = false)
        {
            stUIEventParams par = new stUIEventParams();
            this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, confirmStr, cancelStr, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams param, string confirmStr, string cancelStr, bool isContentLeftAlign = false)
        {
            this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, param, isContentLeftAlign, confirmStr, cancelStr, string.Empty, 0, enUIEventID.None);
        }

        public void OpenMessageBoxWithCancelAndAutoClose(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false, int autoCloseTime = 0, enUIEventID timeUpID = 0)
        {
            this.OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, autoCloseTime, timeUpID);
        }

        public void OpenSendMsgAlert(int autoCloseTime = 5, enUIEventID timeUpEventId = 0)
        {
            CUIEvent uiEvent = new CUIEvent {
                m_eventID = enUIEventID.Common_SendMsgAlertOpen
            };
            stUIEventParams @params = new stUIEventParams {
                tag = autoCloseTime,
                tag2 = (int) timeUpEventId
            };
            uiEvent.m_eventParams = @params;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        public void OpenSendMsgAlert(string txtContent, int autoCloseTime = 10, enUIEventID timeUpEventId = 0)
        {
            CUIEvent uiEvent = new CUIEvent {
                m_eventID = enUIEventID.Common_SendMsgAlertOpen
            };
            stUIEventParams @params = new stUIEventParams {
                tagStr = txtContent,
                tag = autoCloseTime,
                tag2 = (int) timeUpEventId
            };
            uiEvent.m_eventParams = @params;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        public void OpenSmallMessageBox(string strContent, bool isHaveCancelBtn, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, int autoCloseTime = 0, enUIEventID closeTimeID = 0, string confirmStr = "", string cancelStr = "", bool isContentLeftAlign = false)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_SmallMessageBox.prefab", false, false);
            if (script != null)
            {
                GameObject gameObject = script.gameObject;
                if (gameObject != null)
                {
                    if (string.IsNullOrEmpty(confirmStr))
                    {
                        confirmStr = Singleton<CTextManager>.GetInstance().GetText("Common_Confirm");
                    }
                    if (string.IsNullOrEmpty(cancelStr))
                    {
                        cancelStr = Singleton<CTextManager>.GetInstance().GetText("Common_Cancel");
                    }
                    GameObject obj3 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Confirm").gameObject;
                    obj3.GetComponentInChildren<Text>().text = confirmStr;
                    GameObject obj4 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Cancel").gameObject;
                    obj4.GetComponentInChildren<Text>().text = cancelStr;
                    Text component = gameObject.transform.Find("Panel/Panel/Text").GetComponent<Text>();
                    component.text = strContent;
                    if (!isHaveCancelBtn)
                    {
                        obj4.CustomSetActive(false);
                    }
                    else
                    {
                        obj4.CustomSetActive(true);
                    }
                    CUIEventScript script2 = obj3.GetComponent<CUIEventScript>();
                    CUIEventScript script3 = obj4.GetComponent<CUIEventScript>();
                    script2.SetUIEvent(enUIEventType.Click, confirmID, par);
                    script3.SetUIEvent(enUIEventType.Click, cancelID, par);
                    if (isContentLeftAlign)
                    {
                        component.alignment = TextAnchor.MiddleLeft;
                    }
                    if (autoCloseTime != 0)
                    {
                        Transform transform = script.transform.Find("closeTimer");
                        if (transform != null)
                        {
                            CUITimerScript script4 = transform.GetComponent<CUITimerScript>();
                            if (script4 != null)
                            {
                                if (closeTimeID > enUIEventID.None)
                                {
                                    script4.m_eventIDs[1] = closeTimeID;
                                }
                                script4.SetTotalTime((float) autoCloseTime);
                                script4.StartTimer();
                            }
                        }
                    }
                    this.CloseSendMsgAlert();
                }
            }
        }

        public void OpenTips(string strContent, bool bReadDatabin = false, float timeDuration = 1.5f, GameObject referenceGameObject = null, params object[] replaceArr)
        {
            string text = strContent;
            if (bReadDatabin)
            {
                text = Singleton<CTextManager>.GetInstance().GetText(strContent);
            }
            if (!string.IsNullOrEmpty(text))
            {
                if (replaceArr != null)
                {
                    try
                    {
                        text = string.Format(text, replaceArr);
                    }
                    catch (FormatException exception)
                    {
                        object[] inParameters = new object[] { text, exception.Message };
                        DebugHelper.Assert(false, "Format Exception for string \"{0}\", Exception:{1}", inParameters);
                    }
                }
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_Tips.prefab", false, false);
                if (script != null)
                {
                    script.gameObject.transform.Find("Panel/Text").GetComponent<Text>().text = text;
                }
                if ((script != null) && (referenceGameObject != null))
                {
                    RectTransform component = referenceGameObject.GetComponent<RectTransform>();
                    RectTransform transform2 = script.gameObject.transform.Find("Panel") as RectTransform;
                    if ((component != null) && (transform2 != null))
                    {
                        Vector3[] fourCornersArray = new Vector3[4];
                        component.GetWorldCorners(fourCornersArray);
                        float num = Math.Abs((float) (CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, fourCornersArray[2]).x - CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, fourCornersArray[0]).x));
                        float num2 = Math.Abs((float) (CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, fourCornersArray[2]).y - CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, fourCornersArray[0]).y));
                        Vector2 screenPoint = new Vector2(CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, fourCornersArray[0]).x + (num / 2f), CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, fourCornersArray[0]).y + (num2 / 2f));
                        transform2.position = CUIUtility.ScreenToWorldPoint(null, screenPoint, transform2.position.z);
                    }
                }
                if (script != null)
                {
                    CUITimerScript script2 = script.gameObject.transform.Find("Timer").GetComponent<CUITimerScript>();
                    script2.EndTimer();
                    script2.m_totalTime = timeDuration;
                    script2.StartTimer();
                }
                Singleton<CSoundManager>.instance.PostEvent("UI_Click", null);
            }
        }

        private void ProcessFormList(bool sort, bool handleInputAndHide)
        {
            if (sort)
            {
                this.m_forms.Sort();
                for (int i = 0; i < this.m_forms.Count; i++)
                {
                    this.m_forms[i].SetDisplayOrder(i + 1);
                }
                this.m_formOpenOrder = this.m_forms.Count + 1;
            }
            if (handleInputAndHide)
            {
                this.UpdateFormHided();
                this.UpdateFormRaycaster();
            }
            if (this.onFormSorted != null)
            {
                this.onFormSorted(this.m_forms);
            }
        }

        private void RecycleForm(CUIFormScript formScript)
        {
            if (formScript != null)
            {
                if (formScript.m_useFormPool)
                {
                    formScript.Hide(enFormHideFlag.HideByCustom, true);
                    this.m_pooledForms.Add(formScript);
                }
                else
                {
                    UnityEngine.Object.Destroy(formScript.gameObject);
                }
            }
        }

        public void Update()
        {
            int index = 0;
            while (index < this.m_forms.Count)
            {
                this.m_forms[index].CustomUpdate();
                if (this.m_forms[index].IsNeedClose())
                {
                    if (!this.m_forms[index].TurnToClosed(false))
                    {
                        goto Label_00CA;
                    }
                    this.RecycleForm(this.m_forms[index]);
                    this.m_forms.RemoveAt(index);
                    this.m_needSortForms = true;
                    continue;
                }
                if (this.m_forms[index].IsClosed() && !this.m_forms[index].IsInFadeOut())
                {
                    this.RecycleForm(this.m_forms[index]);
                    this.m_forms.RemoveAt(index);
                    this.m_needSortForms = true;
                    continue;
                }
            Label_00CA:
                index++;
            }
            if (this.m_needSortForms)
            {
                this.ProcessFormList(true, true);
            }
            else if (this.m_needUpdateRaycasterAndHide)
            {
                this.ProcessFormList(false, true);
            }
            this.m_needSortForms = false;
            this.m_needUpdateRaycasterAndHide = false;
        }

        private void UpdateFormHided()
        {
            bool flag = false;
            for (int i = this.m_forms.Count - 1; i >= 0; i--)
            {
                if (flag)
                {
                    this.m_forms[i].Hide(enFormHideFlag.HideByOtherForm, false);
                }
                else
                {
                    this.m_forms[i].Appear(enFormHideFlag.HideByOtherForm, false);
                }
                if ((!flag && !this.m_forms[i].IsHided()) && this.m_forms[i].m_hideUnderForms)
                {
                    flag = true;
                }
            }
        }

        private void UpdateFormRaycaster()
        {
            bool flag = true;
            for (int i = this.m_forms.Count - 1; i >= 0; i--)
            {
                if (!this.m_forms[i].m_disableInput && !this.m_forms[i].IsHided())
                {
                    GraphicRaycaster graphicRaycaster = this.m_forms[i].GetGraphicRaycaster();
                    if (graphicRaycaster != null)
                    {
                        graphicRaycaster.enabled = flag;
                    }
                    if (this.m_forms[i].m_isModal && flag)
                    {
                        flag = false;
                    }
                }
            }
        }

        public Camera FormCamera
        {
            get
            {
                return this.m_formCamera;
            }
        }

        public enum enEditFormWidgets
        {
            Title_Text,
            Input_Text,
            Confirm_Button
        }

        private enum enInfoFormWidgets
        {
            Title_Text,
            Info_Text
        }

        public delegate void OnFormSorted(ListView<CUIFormScript> inForms);
    }
}

