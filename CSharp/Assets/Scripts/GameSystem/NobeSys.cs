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
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class NobeSys : MonoSingleton<NobeSys>
    {
        private static uint _lastNobeLv;
        private GameObject m_BaseInfoOnLeftObj;
        private GameObject m_BaseInfoOnRightObj;
        private bool m_bInit;
        private bool m_bInitDiamondForm;
        private CUIFormScript m_ChangeHeadIconForm;
        private ListView<ResNobeInfo> m_ChangeHeadIconResList = new ListView<ResNobeInfo>();
        private int m_CurPage = 1;
        private int m_CurPageLevel;
        private Tab m_CurTab;
        public DelayNobeInfo m_DelayNobeInfo = new DelayNobeInfo();
        private CUIFormScript m_Form;
        private GameObject m_GrowInfoOnLeftObj;
        private GameObject m_GrowInfoOnRightObj;
        public int m_headidx = 2;
        private string m_iconPath = (CUIUtility.s_Sprite_System_ShareUI_Dir + "nobe_iconXiao.prefab");
        private bool m_IsFormOpen;
        public bool m_IsShowDelayNobeTips;
        public int m_Level = 2;
        private DelayNobeInfo m_LoseNobeInfo = new DelayNobeInfo();
        private const int m_nTotalLevel = 8;
        private Vector2[] m_NumPosBack = new Vector2[6];
        private Vector2[] m_NumPosBackDiamondForm = new Vector2[6];
        public int m_Score = 100;
        public SCPKG_GAME_VIP_NTF m_vipInfo = new SCPKG_GAME_VIP_NTF();
        private const int maxPrivilegeNum = 4;
        public string sNobeChangeHeadIconForm = (CUIUtility.s_IDIP_Form_Dir + "Form_Nobe_HeadChangeIcon.prefab");
        public string sPlayerInfoFormPath = (CUIUtility.s_IDIP_Form_Dir + "Form_Nobe.prefab");
        private uint[] tempIndex = new uint[4];
        private string[] tempName = new string[4];
        private string[] tempStr = new string[4];

        private void CloseChangeHeadIconForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(this.m_ChangeHeadIconForm);
            this.m_ChangeHeadIconForm = null;
        }

        private void CloseForm(CUIEvent uiEvent)
        {
            if (this.m_IsFormOpen)
            {
                this.m_IsFormOpen = false;
                Singleton<CUIManager>.GetInstance().CloseForm(this.m_Form);
                this.m_Form = null;
            }
        }

        private static int ComparebyShowIdx(ResNobeInfo info1, ResNobeInfo info2)
        {
            if (info1.bShowIdx > info2.bShowIdx)
            {
                return 1;
            }
            if (info1.bShowIdx == info2.bShowIdx)
            {
                return 0;
            }
            return -1;
        }

        private ListView<ResNobeInfo> GetAllHeadIcon()
        {
            ListView<ResNobeInfo> view = new ListView<ResNobeInfo>();
            for (int i = 0; i < GameDataMgr.resNobeInfoDatabin.count; i++)
            {
                ResNobeInfo dataByIndex = GameDataMgr.resNobeInfoDatabin.GetDataByIndex(i);
                if ((dataByIndex != null) && (dataByIndex.bResType == 2))
                {
                    view.Add(dataByIndex);
                }
            }
            return view;
        }

        private string GetHeadIdxResName(int idx)
        {
            return Singleton<HeadIconSys>.instance.GetHeadIdxResName(idx);
        }

        public string GetNobleNameColorString(string playerName, int playerLevel)
        {
            playerLevel = 1;
            if (playerLevel > 0)
            {
                return string.Format("<color=#ff0000ff>{0}</color>", playerName);
            }
            return playerName;
        }

        public int GetSelfNobeHeadIdx()
        {
            if (this.m_vipInfo != null)
            {
                return (int) this.m_vipInfo.stGameVipClient.dwHeadIconId;
            }
            return 0;
        }

        public int GetSelfNobeLevel()
        {
            if (this.m_vipInfo != null)
            {
                return (int) this.m_vipInfo.stGameVipClient.dwCurLevel;
            }
            return 0;
        }

        public int GetSelfNobeScore()
        {
            if (this.m_vipInfo != null)
            {
                return (int) this.m_vipInfo.stGameVipClient.dwScore;
            }
            return 0;
        }

        protected override void Init()
        {
            base.Init();
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.AddUIEventListener(enUIEventID.NOBE_OPEN_Form, new CUIEventManager.OnUIEventHandler(this.OpenForm));
            instance.AddUIEventListener(enUIEventID.NOBE_CLOSE_Form, new CUIEventManager.OnUIEventHandler(this.CloseForm));
            instance.AddUIEventListener(enUIEventID.NOBE_TAB_Change, new CUIEventManager.OnUIEventHandler(this.OnTabChange));
            instance.AddUIEventListener(enUIEventID.NOBE_PAY, new CUIEventManager.OnUIEventHandler(this.OnNobePay));
            instance.AddUIEventListener(enUIEventID.NOBE_GOTO_STROE, new CUIEventManager.OnUIEventHandler(this.OnNobeGotoStore));
            instance.AddUIEventListener(enUIEventID.NOBE_LEFT, new CUIEventManager.OnUIEventHandler(this.OnLeft));
            instance.AddUIEventListener(enUIEventID.NOBE_RIGHT, new CUIEventManager.OnUIEventHandler(this.OnRight));
            instance.AddUIEventListener(enUIEventID.NOBE_LEFT_Nobe_Level, new CUIEventManager.OnUIEventHandler(this.OnLeftLevel));
            instance.AddUIEventListener(enUIEventID.NOBE_RIGHT_Nobe_Level, new CUIEventManager.OnUIEventHandler(this.OnRightLevel));
            instance.AddUIEventListener(enUIEventID.NOBE_CHANGEHEAD_ICON_OPEN_FORM, new CUIEventManager.OnUIEventHandler(this.OpenChangeHeadIconForm));
            instance.AddUIEventListener(enUIEventID.NOBE_CHANGEHEAD_ICON_CLOSE_FORM, new CUIEventManager.OnUIEventHandler(this.CloseChangeHeadIconForm));
            instance.AddUIEventListener(enUIEventID.Mall_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnMallOpenForm));
            instance.AddUIEventListener(enUIEventID.Mall_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnMallCloseForm));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Get_Product_OK, new System.Action(this.OnMall_Get_Product_OK));
            instance.AddUIEventListener(enUIEventID.Mall_Get_AWARD_CLOSE_FORM, new CUIEventManager.OnUIEventHandler(this.OnMall_Get_AWARD_CLOSE_FORM));
            instance.AddUIEventListener(enUIEventID.Mall_Roulette_Close_Award_Form, new CUIEventManager.OnUIEventHandler(this.OnMall_Get_AWARD_CLOSE_FORM));
            this.m_IsFormOpen = false;
            this.m_CurTab = Tab.Base_Info;
            this.m_Form = null;
            this.m_ChangeHeadIconResList = this.GetAllHeadIcon();
        }

        private void InitTab()
        {
            if ((this.m_Form != null) && this.m_IsFormOpen)
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if ((widget != null) && widget.activeSelf)
                {
                    widget.CustomSetActive(false);
                }
                GameObject obj3 = this.m_Form.GetWidget(2);
                if ((obj3 != null) && obj3.activeSelf)
                {
                    obj3.CustomSetActive(false);
                }
                Tab[] values = (Tab[]) Enum.GetValues(typeof(Tab));
                string[] strArray = new string[values.Length];
                for (byte i = 0; i < values.Length; i = (byte) (i + 1))
                {
                    switch (values[i])
                    {
                        case Tab.Base_Info:
                            strArray[i] = "贵族特权";
                            break;

                        case Tab.Grow_Info:
                            strArray[i] = "成长体系";
                            break;
                    }
                }
                CUIListScript component = this.m_Form.GetWidget(0).GetComponent<CUIListScript>();
                if (component != null)
                {
                    component.SetElementAmount(strArray.Length);
                    for (int j = 0; j < component.m_elementAmount; j++)
                    {
                        component.GetElemenet(j).gameObject.transform.Find("Text").GetComponent<Text>().text = strArray[j];
                    }
                    component.m_alwaysDispatchSelectedChangeEvent = true;
                    component.SelectElement((int) this.CurTab, true);
                }
            }
        }

        [MessageHandler(0x11f8)]
        public static void OnGetNobeVipInfo(CSPkg msg)
        {
            MonoSingleton<NobeSys>.GetInstance().m_vipInfo = msg.stPkgData.stGameVipNtf;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            _lastNobeLv = masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel;
            masterRoleInfo.SetNobeInfo(msg.stPkgData.stGameVipNtf);
            if (Singleton<BattleLogic>.instance.isRuning)
            {
                MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.Type = 0;
                MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow = 0;
                MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack = msg.stPkgData.stGameVipNtf;
            }
            else
            {
                if (MonoSingleton<NobeSys>.GetInstance().m_IsShowDelayNobeTips && (msg.stPkgData.stGameVipNtf.dwNtfType > 0))
                {
                    MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.Type = 1;
                    MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow = 0;
                    MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack = msg.stPkgData.stGameVipNtf;
                }
                else
                {
                    MonoSingleton<NobeSys>.GetInstance().ShowLoginTips(MonoSingleton<NobeSys>.GetInstance().m_vipInfo);
                    MonoSingleton<NobeSys>.GetInstance().UpdateNobeLevelChange();
                }
                MonoSingleton<NobeSys>.GetInstance().ShowNobeTipsInDiamond();
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.NOBE_STATE_CHANGE);
        }

        private void OnLeft(CUIEvent uiEvent)
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                int curPage = this.m_CurPage - 1;
                this.UpdateBaseInfolevel(curPage);
                if (curPage >= 0)
                {
                    this.m_CurPage = curPage;
                    this.m_Form.GetWidget(20).GetComponent<CUIListScript>().MoveElementInScrollArea(this.m_CurPage, false);
                    this.UpdateNobeLevelText(this.m_CurPage);
                    this.UpdateNobeGiftIcon(this.m_CurPage);
                }
            }
        }

        private void OnLeftLevel(CUIEvent uiEvent)
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                int curPage = this.m_CurPageLevel - 1;
                this.UpdateGrowInfoLevel(curPage);
                if (curPage >= 0)
                {
                    this.m_CurPageLevel = curPage;
                    this.m_Form.GetWidget(4).GetComponent<CUIListScript>().MoveElementInScrollArea(this.m_CurPageLevel, false);
                }
            }
        }

        private void OnMall_Get_AWARD_CLOSE_FORM(CUIEvent uiEvent)
        {
            this.OnMall_Get_Product_OK();
        }

        private void OnMall_Get_Product_OK()
        {
            if (MonoSingleton<NobeSys>.GetInstance().m_IsShowDelayNobeTips && (MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow == 0))
            {
                MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow = 1;
                MonoSingleton<NobeSys>.GetInstance().ShowLoginTips(MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack);
                MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack.dwNtfType = 0;
            }
        }

        private void OnMallCloseForm(CUIEvent uiEvent)
        {
            MonoSingleton<NobeSys>.GetInstance().m_IsShowDelayNobeTips = false;
        }

        private void OnMallOpenForm(CUIEvent uiEvent)
        {
            MonoSingleton<NobeSys>.GetInstance().m_IsShowDelayNobeTips = true;
        }

        private void OnNobeGotoStore(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_OpenForm);
        }

        private void OnNobePay(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Pay_OpenBuyDianQuanPanel);
        }

        private void OnRight(CUIEvent uiEvent)
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                int curPage = this.m_CurPage + 1;
                this.UpdateBaseInfolevel(curPage);
                if (curPage < 8)
                {
                    this.m_CurPage = curPage;
                    this.m_Form.GetWidget(20).GetComponent<CUIListScript>().MoveElementInScrollArea(this.m_CurPage, false);
                    this.UpdateNobeLevelText(this.m_CurPage);
                    this.UpdateNobeGiftIcon(this.m_CurPage);
                }
            }
        }

        private void OnRightLevel(CUIEvent uiEvent)
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                int curPage = this.m_CurPageLevel + 1;
                this.UpdateGrowInfoLevel(curPage);
                if (curPage < 2)
                {
                    this.m_GrowInfoOnLeftObj.CustomSetActive(true);
                    this.m_CurPageLevel = curPage;
                    this.m_Form.GetWidget(4).GetComponent<CUIListScript>().MoveElementInScrollArea(this.m_CurPageLevel, false);
                }
            }
        }

        private void OnTabChange(CUIEvent uiEvent)
        {
            if (((this.m_Form != null) && this.m_IsFormOpen) && (this.m_vipInfo != null))
            {
                if (!this.m_bInit)
                {
                    GameObject widget = this.m_Form.GetWidget(0x16);
                    for (int i = 0; i < 5; i++)
                    {
                        Transform transform = widget.transform.Find("n" + i);
                        this.m_NumPosBack[i] = transform.GetComponent<RectTransform>().anchoredPosition;
                    }
                    Transform transform2 = widget.transform.Find("tipsRight");
                    this.m_NumPosBack[5] = transform2.GetComponent<RectTransform>().anchoredPosition;
                    this.m_bInit = true;
                }
                CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
                int selectedIndex = component.GetSelectedIndex();
                this.CurTab = (Tab) selectedIndex;
                if (this.m_Form != null)
                {
                    GameObject obj3 = this.m_Form.GetWidget(1);
                    GameObject obj4 = this.m_Form.GetWidget(2);
                    Text text = this.m_Form.GetWidget(3).GetComponent<Text>();
                    switch (this.m_CurTab)
                    {
                        case Tab.Base_Info:
                            if ((obj3 != null) && !obj3.activeSelf)
                            {
                                obj3.CustomSetActive(true);
                                if (obj4 != null)
                                {
                                    obj4.CustomSetActive(false);
                                }
                                text.text = component.GetElemenet(selectedIndex).gameObject.transform.Find("Text").GetComponent<Text>().text;
                                this.UpdateBaseInfo();
                            }
                            break;

                        case Tab.Grow_Info:
                            if (((obj3 != null) && (obj4 != null)) && !obj4.activeSelf)
                            {
                                obj3.CustomSetActive(false);
                                obj4.CustomSetActive(true);
                                text.text = component.GetElemenet(selectedIndex).gameObject.transform.Find("Text").GetComponent<Text>().text;
                                this.UpdateGrowInfo();
                            }
                            break;
                    }
                }
            }
        }

        private void OpenChangeHeadIconForm(CUIEvent uiEvent)
        {
            this.m_ChangeHeadIconForm = Singleton<CUIManager>.GetInstance().OpenForm(this.sNobeChangeHeadIconForm, false, true);
            int dwHeadIconId = (int) this.m_vipInfo.stGameVipClient.dwHeadIconId;
            this.UpdateCHangeHeadFormData(dwHeadIconId);
        }

        public void OpenForm(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Pay_ClosePayDianQuanForm);
            Singleton<CUIManager>.GetInstance().CloseForm(this.sPlayerInfoFormPath);
            this.m_IsFormOpen = true;
            this.m_Form = Singleton<CUIManager>.GetInstance().OpenForm(this.sPlayerInfoFormPath, false, true);
            this.CurTab = Tab.Base_Info;
            this.m_BaseInfoOnLeftObj = this.m_Form.GetWidget(0x1a);
            this.m_BaseInfoOnRightObj = this.m_Form.GetWidget(0x1b);
            this.m_GrowInfoOnLeftObj = this.m_Form.GetWidget(0x1c);
            this.m_GrowInfoOnRightObj = this.m_Form.GetWidget(0x1d);
            this.InitTab();
        }

        public void SetGameCenterVisible(GameObject obj, COM_PRIVILEGE_TYPE privilegeType, ApolloPlatform platform, bool isHideInIos = false, bool isIosBlock = false)
        {
            if (obj != null)
            {
                bool bActive = false;
                if ((Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat) && (platform == ApolloPlatform.Wechat))
                {
                    if (privilegeType == COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_WXGAMECENTER_LOGIN)
                    {
                        bActive = true;
                    }
                }
                else if ((Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ) && (platform == ApolloPlatform.QQ))
                {
                    if (privilegeType == COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_QQGAMECENTER_LOGIN)
                    {
                        bActive = true;
                    }
                }
                else if ((Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Guest) && (platform == ApolloPlatform.Guest))
                {
                    bActive = true;
                }
                if (MonoSingleton<BannerImageSys>.GetInstance().IsWaifaBlockChannel())
                {
                    bActive = false;
                }
                obj.CustomSetActive(bActive);
            }
        }

        public void SetHeadIconBk(Image image, int headIdx)
        {
            if (image != null)
            {
                if (headIdx > 0)
                {
                    string headIdxResName = this.GetHeadIdxResName(headIdx);
                    if (headIdxResName == string.Empty)
                    {
                        image.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Nobe_Dir, headIdxResName);
                        if (prefabPath != string.Empty)
                        {
                            image.gameObject.CustomSetActive(true);
                            CUIUtility.SetImageSprite(image, prefabPath, null, true, false, false);
                        }
                    }
                }
                else
                {
                    image.gameObject.CustomSetActive(false);
                }
            }
        }

        public void SetMyQQVipHead(Image image)
        {
            if (image != null)
            {
                GameObject gameObject = image.gameObject;
                if (gameObject != null)
                {
                    if ((ApolloConfig.platform != ApolloPlatform.QQ) && (ApolloConfig.platform != ApolloPlatform.WTLogin))
                    {
                        gameObject.CustomSetActive(false);
                    }
                    if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().HasVip(0x10))
                    {
                        gameObject.CustomSetActive(true);
                        CUIUtility.SetImageSprite(image, string.Format("{0}QQ_Svip_icon.prefab", "UGUI/Sprite/Common/"), null, true, false, false);
                    }
                    else if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().HasVip(1))
                    {
                        gameObject.CustomSetActive(true);
                        CUIUtility.SetImageSprite(image, string.Format("{0}QQ_vip_icon.prefab", "UGUI/Sprite/Common/"), null, true, false, false);
                    }
                    else
                    {
                        gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        private void SetNobeGift(uint curLv, CUIFormScript formScript)
        {
            int num = 1;
            for (int i = 1; i <= GameDataMgr.resNobeInfoDatabin.count; i++)
            {
                ResNobeInfo dataByIndex = GameDataMgr.resNobeInfoDatabin.GetDataByIndex(i - 1);
                if (((dataByIndex != null) && (dataByIndex.bResType == 3)) && (dataByIndex.dwNobeLevel == curLv))
                {
                    Transform transform = formScript.m_formWidgets[4].transform.FindChild(string.Format("Gift{0}", num));
                    if (transform != null)
                    {
                        transform.gameObject.CustomSetActive(true);
                        Transform transform2 = transform.FindChild(string.Format("Icon", new object[0]));
                        Transform transform3 = transform.FindChild(string.Format("Text", new object[0]));
                        Transform transform4 = transform.FindChild(string.Format("ExperienceCard", new object[0]));
                        Transform transform5 = transform.FindChild(string.Format("TextNum", new object[0]));
                        transform4.gameObject.CustomSetActive(dataByIndex.bShowIdx == 1);
                        transform5.gameObject.GetComponent<Text>().text = (dataByIndex.dwJiaoBiaoNum <= 0) ? null : dataByIndex.dwJiaoBiaoNum.ToString(CultureInfo.InvariantCulture);
                        string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_Dynamic_Icon_Dir, Utility.UTF8Convert(dataByIndex.szResIcon));
                        if ((transform2 != null) && (transform3 != null))
                        {
                            transform2.gameObject.GetComponent<Image>().SetSprite(prefabPath, formScript, true, false, false);
                            transform3.gameObject.GetComponent<Text>().text = Utility.UTF8Convert(dataByIndex.szResName);
                            num++;
                        }
                    }
                }
            }
            for (int j = num; j <= 3; j++)
            {
                Transform transform6 = formScript.m_formWidgets[4].transform.FindChild(string.Format("Gift{0}", j));
                if (transform6 != null)
                {
                    transform6.gameObject.CustomSetActive(false);
                }
            }
        }

        public void SetNobeIcon(Image image, int level, bool useDefaultBackImage = false)
        {
            if (image != null)
            {
                GameObject gameObject = null;
                if (image.gameObject.name == "NobeIcon")
                {
                    gameObject = image.gameObject;
                }
                else
                {
                    gameObject = image.transform.FindChild("NobeIcon").gameObject;
                }
                if (gameObject != null)
                {
                    if (CSysDynamicBlock.bSocialBlocked)
                    {
                        gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        Image component = gameObject.GetComponent<Image>();
                        if (level <= 0)
                        {
                            component.gameObject.CustomSetActive(false);
                        }
                        else
                        {
                            component.gameObject.CustomSetActive(true);
                            if (!useDefaultBackImage)
                            {
                                CUIUtility.SetImageSprite(component, this.m_iconPath, null, true, false, false);
                            }
                            Transform transform = component.transform.FindChild("text");
                            if (transform != null)
                            {
                                transform.GetComponent<Text>().text = level.ToString();
                            }
                        }
                    }
                }
            }
        }

        private void SetNobePrivilege(uint curLv, CUIFormScript formScript, int widgetID)
        {
            int num = 1;
            int num2 = 0;
            int num3 = 100;
            for (int i = 0; i < 4; i++)
            {
                this.tempIndex[i] = 0x3e6;
                this.tempStr[i] = null;
                this.tempName[i] = null;
            }
            for (int j = 1; j <= GameDataMgr.resNobeInfoDatabin.count; j++)
            {
                ResNobeInfo dataByIndex = GameDataMgr.resNobeInfoDatabin.GetDataByIndex(j - 1);
                if ((dataByIndex != null) && (dataByIndex.bResType == 1))
                {
                    if (dataByIndex.dwNobeLevel > curLv)
                    {
                        break;
                    }
                    if ((((j == 4) || (j == 2)) || (j == 5)) && (j > num2))
                    {
                        num2 = j;
                        num3 = Math.Min(num, num3);
                        string str = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, Utility.UTF8Convert(dataByIndex.szResIcon));
                        this.tempIndex[num3 - 1] = dataByIndex.bShowIdx;
                        this.tempStr[num3 - 1] = str;
                        this.tempName[num3 - 1] = dataByIndex.szResName;
                        if (num <= num3)
                        {
                            num++;
                        }
                    }
                    else if (((j != 4) && (j != 2)) && (j != 5))
                    {
                        string str2 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, Utility.UTF8Convert(dataByIndex.szResIcon));
                        this.tempIndex[num - 1] = dataByIndex.bShowIdx;
                        this.tempStr[num - 1] = str2;
                        this.tempName[num - 1] = dataByIndex.szResName;
                        num++;
                    }
                }
            }
            for (int k = 3; k >= 1; k--)
            {
                for (int num7 = k - 1; num7 >= 0; num7--)
                {
                    if (this.tempIndex[k] < this.tempIndex[num7])
                    {
                        string str3 = this.tempStr[num7];
                        string str4 = this.tempName[num7];
                        uint num8 = this.tempIndex[num7];
                        this.tempStr[num7] = this.tempStr[k];
                        this.tempName[num7] = this.tempName[k];
                        this.tempIndex[num7] = this.tempIndex[k];
                        this.tempStr[k] = str3;
                        this.tempName[k] = str4;
                        this.tempIndex[k] = num8;
                    }
                }
            }
            for (int m = 0; m < (num - 1); m++)
            {
                Transform transform = formScript.m_formWidgets[widgetID].transform.FindChild(string.Format("TeQuan{0}", m + 1));
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(true);
                    Transform transform2 = transform.FindChild(string.Format("Icon", new object[0]));
                    Transform transform3 = transform.FindChild(string.Format("Text", new object[0]));
                    transform2.gameObject.GetComponent<Image>().SetSprite(this.tempStr[m], formScript, true, false, false);
                    transform3.gameObject.GetComponent<Text>().text = Utility.UTF8Convert(this.tempName[m]);
                }
            }
            for (int n = num; n <= 4; n++)
            {
                Transform transform4 = formScript.m_formWidgets[widgetID].transform.FindChild(string.Format("TeQuan{0}", n));
                if (transform4 != null)
                {
                    transform4.gameObject.CustomSetActive(false);
                }
            }
        }

        public void SetOtherQQVipHead(Image image, int bit)
        {
            if (image != null)
            {
                GameObject gameObject = image.gameObject;
                if (gameObject != null)
                {
                    if ((ApolloConfig.platform != ApolloPlatform.QQ) && (ApolloConfig.platform != ApolloPlatform.WTLogin))
                    {
                        gameObject.CustomSetActive(false);
                    }
                    else if ((bit & 0x10) != 0)
                    {
                        gameObject.CustomSetActive(true);
                        CUIUtility.SetImageSprite(image, string.Format("{0}QQ_Svip_icon.prefab", "UGUI/Sprite/Common/"), null, true, false, false);
                    }
                    else if ((bit & 1) != 0)
                    {
                        gameObject.CustomSetActive(true);
                        CUIUtility.SetImageSprite(image, string.Format("{0}QQ_vip_icon.prefab", "UGUI/Sprite/Common/"), null, true, false, false);
                    }
                    else
                    {
                        gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        public void ShowDelayNobeLoseTipsInfo()
        {
            if (this.m_LoseNobeInfo.nShow == 0)
            {
                this.m_LoseNobeInfo.nShow = 1;
                this.ShowNobeLoseTips(MonoSingleton<NobeSys>.GetInstance().m_LoseNobeInfo.m_vipInfoBack);
                this.m_LoseNobeInfo.m_vipInfoBack.dwNtfType = 0;
            }
        }

        public void ShowDelayNobeTipsInfo()
        {
            if (MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow == 0)
            {
                MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow = 1;
                MonoSingleton<NobeSys>.GetInstance().ShowLoginTips(MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack);
                MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack.dwNtfType = 0;
            }
        }

        private void ShowLoginTips(SCPKG_GAME_VIP_NTF vipInfo)
        {
            if (vipInfo != null)
            {
                switch (vipInfo.dwNtfType)
                {
                    case 1:
                        this.ShowNobeLevelUpForm(vipInfo);
                        break;

                    case 2:
                        this.m_LoseNobeInfo.m_vipInfoBack = vipInfo;
                        this.m_LoseNobeInfo.nShow = 0;
                        this.m_LoseNobeInfo.Type = 2;
                        break;

                    case 3:
                    {
                        string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("VIPREGAIN"), vipInfo.stGameVipClient.dwCurLevel);
                        Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
                        break;
                    }
                    case 4:
                        this.ShowNobeLevelUpForm(vipInfo);
                        break;

                    case 5:
                        this.m_LoseNobeInfo.m_vipInfoBack = vipInfo;
                        this.m_LoseNobeInfo.nShow = 0;
                        this.m_LoseNobeInfo.Type = 5;
                        break;
                }
            }
        }

        protected void ShowNobeLevelUpForm(SCPKG_GAME_VIP_NTF vipInfo)
        {
            uint dwCurLevel = vipInfo.stGameVipClient.dwCurLevel;
            string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "IDIPNotice/Form_NobeLevelUp.prefab");
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
            if (formScript != null)
            {
                formScript.m_formWidgets[0].GetComponent<Text>().text = dwCurLevel.ToString(CultureInfo.InvariantCulture);
                formScript.m_formWidgets[1].GetComponent<Text>().text = _lastNobeLv.ToString(CultureInfo.InvariantCulture);
                formScript.m_formWidgets[2].GetComponent<Text>().text = dwCurLevel.ToString(CultureInfo.InvariantCulture);
                this.SetNobePrivilege(dwCurLevel, formScript, 3);
                this.SetNobeGift(dwCurLevel, formScript);
            }
        }

        private void ShowNobeLoseTips(SCPKG_GAME_VIP_NTF vipInfo)
        {
            int dwMaxLevel;
            string str;
            string str2;
            if (vipInfo != null)
            {
                int dwCurLevel = (int) vipInfo.stGameVipClient.dwCurLevel;
                dwMaxLevel = (int) vipInfo.dwMaxLevel;
                CS_GAMEVIP_NTF_TYPE dwNtfType = (CS_GAMEVIP_NTF_TYPE) vipInfo.dwNtfType;
                str = string.Empty;
                switch (dwNtfType)
                {
                    case CS_GAMEVIP_NTF_TYPE.CS_GAMEVIP_NTF_LOSE:
                        str = "<size=28>尊贵的召唤师：</size>                               非常遗憾，由于您上个月未消费点券，您将暂时失去贵族身份，不过不用担心哦，只要您本月消费<color=#e7b135>任意金额</color>的点券，就能立即恢复到<color=#e7b135>贵族" + dwMaxLevel + "级，1点券</color>也可以哦！";
                        goto Label_008C;

                    case CS_GAMEVIP_NTF_TYPE.CS_GAMEVIP_NTF_DEGRADE:
                    {
                        object[] objArray1 = new object[] { "<size=28>尊贵的召唤师：</size>                               非常遗憾，由于您上个月未消费点券，您的贵族等级下降为<color=#e7b135>贵族", dwCurLevel, "级</color>，不过不用担心哦，只要您本月消费<color=#e7b135>任意金额</color>的点券，就能立即恢复到<color=#e7b135>贵族", dwMaxLevel, "级，1点券</color>也可以哦！" };
                        str = string.Concat(objArray1);
                        goto Label_008C;
                    }
                }
            }
            return;
        Label_008C:
            str2 = string.Format("{0}{1}", "UGUI/Form/System/", "IDIPNotice/Form_NobeTip.prefab");
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(str2, false, true);
            if (formScript != null)
            {
                Text component = formScript.GetWidget(0).GetComponent<Text>();
                if (component != null)
                {
                    component.text = str;
                }
                this.SetNobePrivilege((uint) dwMaxLevel, formScript, 1);
                Text text2 = formScript.GetWidget(2).GetComponent<Text>();
                if (text2 != null)
                {
                    text2.text = string.Format("贵族{0}特权", dwMaxLevel);
                }
            }
        }

        public void ShowNobeTipsInDiamond()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CPaySystem.s_buyDianQuanFormPath);
            if (((form != null) && (this.m_vipInfo != null)) && (form != null))
            {
                Transform transform = form.gameObject.transform.Find("NobTips");
                if (transform != null)
                {
                    int dwCurLevel = (int) this.m_vipInfo.stGameVipClient.dwCurLevel;
                    int dwScore = (int) this.m_vipInfo.stGameVipClient.dwScore;
                    int num3 = GameDataMgr.resVipDianQuan.Count();
                    Transform transform2 = transform.Find("txtTips");
                    if (transform2 != null)
                    {
                        if (!this.m_bInitDiamondForm)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                Transform transform3 = transform2.transform.Find("n" + i);
                                this.m_NumPosBackDiamondForm[i] = transform3.GetComponent<RectTransform>().anchoredPosition;
                            }
                            Transform transform4 = transform2.transform.Find("tipsRight");
                            this.m_NumPosBackDiamondForm[5] = transform4.GetComponent<RectTransform>().anchoredPosition;
                            this.m_bInitDiamondForm = true;
                        }
                        transform2.gameObject.CustomSetActive(false);
                        Transform transform5 = transform.transform.Find("txtWins");
                        string str = string.Empty;
                        if (dwCurLevel >= num3)
                        {
                            str = "贵族等级已满";
                            if (transform5 != null)
                            {
                                transform5.gameObject.CustomSetActive(true);
                                transform5.GetComponent<Text>().text = str;
                            }
                            Transform transform6 = transform2.Find("tipsLeft/vipLevel");
                            if (transform6 != null)
                            {
                                transform6.gameObject.CustomSetActive(false);
                            }
                        }
                        else
                        {
                            int num5 = dwCurLevel + 1;
                            ResVIPCoupons dataByKey = GameDataMgr.resVipDianQuan.GetDataByKey((long) num5);
                            if (dataByKey != null)
                            {
                                int num6 = ((int) dataByKey.dwConsumeCoupons) - dwScore;
                                if (num6 > 0)
                                {
                                    str = string.Format("再消费{0}点券，升级到贵族{1}", num6, num5);
                                    if (transform5 != null)
                                    {
                                        transform5.gameObject.CustomSetActive(false);
                                    }
                                    this.UpdateImageLevelTips(transform2.gameObject, num5, num6, true);
                                }
                                else
                                {
                                    int dwMaxLevel = (int) this.m_vipInfo.dwMaxLevel;
                                    ResVIPCoupons coupons2 = GameDataMgr.resVipDianQuan.GetDataByKey((long) dwMaxLevel);
                                    str = "只要消费任意金额的点券，即可恢复到之前的最高等级";
                                    if (transform5 != null)
                                    {
                                        transform5.gameObject.CustomSetActive(true);
                                        transform5.GetComponent<Text>().text = str;
                                    }
                                }
                            }
                            Transform transform7 = transform2.Find("tipsLeft/vipLevel");
                            if (transform7 != null)
                            {
                                transform7.gameObject.CustomSetActive(true);
                                string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
                                CUIUtility.SetImageSprite(transform7.gameObject.GetComponent<Image>(), prefabPath, null, true, false, false);
                            }
                        }
                        if (CSysDynamicBlock.bVipBlock)
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void UpdateBaseInfo()
        {
            if (this.m_vipInfo != null)
            {
                Image component = this.m_Form.GetWidget(12).GetComponent<Image>();
                Text text = this.m_Form.GetWidget(13).GetComponent<Text>();
                Image image = this.m_Form.GetWidget(14).GetComponent<Image>();
                Image image3 = this.m_Form.GetWidget(15).GetComponent<Image>();
                Image image4 = this.m_Form.GetWidget(10).GetComponent<Image>();
                Image image5 = this.m_Form.GetWidget(11).GetComponent<Image>();
                Transform parent = null;
                if (image5 != null)
                {
                    parent = image5.transform.parent;
                }
                Text text2 = this.m_Form.GetWidget(0x13).GetComponent<Text>();
                int dwCurLevel = (int) this.m_vipInfo.stGameVipClient.dwCurLevel;
                int dwScore = (int) this.m_vipInfo.stGameVipClient.dwScore;
                int num3 = GameDataMgr.resVipDianQuan.Count();
                string str = string.Empty;
                if (dwCurLevel == 0)
                {
                    image4.color = new Color(image4.color.r, image4.color.g, image4.color.b, 0.5f);
                    image.gameObject.CustomSetActive(false);
                }
                else
                {
                    image4.color = new Color(image4.color.r, image4.color.g, image4.color.b, 1f);
                    image.gameObject.CustomSetActive(true);
                }
                GameObject widget = this.m_Form.GetWidget(0x16);
                widget.CustomSetActive(false);
                if (dwCurLevel >= num3)
                {
                    str = "贵族等级已满";
                    text.gameObject.CustomSetActive(true);
                    text.text = str;
                    component.CustomFillAmount(1f);
                    string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
                    CUIUtility.SetImageSprite(image, prefabPath, null, true, false, false);
                    image5.gameObject.CustomSetActive(false);
                    if (parent != null)
                    {
                        parent.gameObject.CustomSetActive(false);
                    }
                    ResVIPCoupons dataByKey = GameDataMgr.resVipDianQuan.GetDataByKey((long) dwCurLevel);
                    if (dataByKey != null)
                    {
                        text2.text = string.Format("{0}/{1}", dwScore, (int) dataByKey.dwConsumeCoupons);
                    }
                }
                else
                {
                    int num4 = dwCurLevel + 1;
                    ResVIPCoupons coupons2 = GameDataMgr.resVipDianQuan.GetDataByKey((long) num4);
                    if (coupons2 != null)
                    {
                        int num5 = ((int) coupons2.dwConsumeCoupons) - dwScore;
                        if (num5 > 0)
                        {
                            str = string.Format("再消费{0}点券，升级到贵族{1}", num5, num4);
                            text.gameObject.CustomSetActive(false);
                            this.UpdateImageLevelTips(widget, num4, num5, false);
                            component.CustomFillAmount((1f * dwScore) / ((float) coupons2.dwConsumeCoupons));
                            string str3 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
                            CUIUtility.SetImageSprite(image, str3, null, true, false, false);
                            str3 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, (dwCurLevel + 1).ToString());
                            CUIUtility.SetImageSprite(image3, str3, null, true, false, false);
                            image5.gameObject.CustomSetActive(true);
                            image4.gameObject.CustomSetActive(true);
                            if (parent != null)
                            {
                                parent.gameObject.CustomSetActive(true);
                            }
                            text2.text = string.Format("{0}/{1}", dwScore, (int) coupons2.dwConsumeCoupons);
                        }
                        else
                        {
                            int dwMaxLevel = (int) this.m_vipInfo.dwMaxLevel;
                            ResVIPCoupons coupons3 = GameDataMgr.resVipDianQuan.GetDataByKey((long) dwMaxLevel);
                            str = "只要消费任意金额的点券，即可恢复到之前的最高等级";
                            component.CustomFillAmount((1f * dwScore) / ((float) coupons3.dwConsumeCoupons));
                            text.gameObject.CustomSetActive(true);
                            text.text = str;
                            string str4 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
                            CUIUtility.SetImageSprite(image, str4, null, true, false, false);
                            str4 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwMaxLevel.ToString());
                            CUIUtility.SetImageSprite(image3, str4, null, true, false, false);
                            image5.gameObject.CustomSetActive(true);
                            image4.gameObject.CustomSetActive(true);
                            if (parent != null)
                            {
                                parent.gameObject.CustomSetActive(true);
                            }
                            text2.text = string.Format("{0}/{1}", dwScore, (int) coupons3.dwConsumeCoupons);
                        }
                    }
                }
                DictionaryView<uint, ListView<ResNobeInfo>> view = new DictionaryView<uint, ListView<ResNobeInfo>>();
                for (int i = 0; i < GameDataMgr.resNobeInfoDatabin.count; i++)
                {
                    ResNobeInfo dataByIndex = GameDataMgr.resNobeInfoDatabin.GetDataByIndex(i);
                    if ((dataByIndex != null) && (dataByIndex.bResType == 1))
                    {
                        if (!view.ContainsKey(dataByIndex.dwNobeLevel))
                        {
                            ListView<ResNobeInfo> view2 = new ListView<ResNobeInfo>();
                            view2.Add(dataByIndex);
                            view.Add(dataByIndex.dwNobeLevel, view2);
                        }
                        else
                        {
                            view[dataByIndex.dwNobeLevel].Add(dataByIndex);
                        }
                    }
                }
                DictionaryView<uint, ListView<ResNobeInfo>> view4 = new DictionaryView<uint, ListView<ResNobeInfo>>();
                for (uint j = 1; j <= 8; j++)
                {
                    ListView<ResNobeInfo> view5 = new ListView<ResNobeInfo>();
                    for (uint m = j; m >= 1; m--)
                    {
                        if (view.ContainsKey(m))
                        {
                            for (int n = 0; n < view[m].Count; n++)
                            {
                                ResNobeInfo item = view[m][n];
                                bool flag = false;
                                for (int num11 = 0; num11 < view5.Count; num11++)
                                {
                                    ResNobeInfo info3 = view5[num11];
                                    string str5 = Utility.UTF8Convert(info3.szResIcon);
                                    string str6 = Utility.UTF8Convert(item.szResIcon);
                                    if (str5 == str6)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (!flag)
                                {
                                    view5.Add(item);
                                    view5.Sort(new Comparison<ResNobeInfo>(NobeSys.ComparebyShowIdx));
                                }
                            }
                        }
                    }
                    view4.Add(j, view5);
                }
                CUIListScript script = this.m_Form.GetWidget(20).GetComponent<CUIListScript>();
                script.SetElementAmount(8);
                for (uint k = 0; k < 8; k++)
                {
                    CUIListElementScript elemenet = script.GetElemenet((int) k);
                    if (view4.ContainsKey(k + 1))
                    {
                        ListView<ResNobeInfo> view6 = view4[k + 1];
                        for (int num13 = 0; num13 < 4; num13++)
                        {
                            GameObject gameObject = null;
                            Transform transform2 = elemenet.transform.Find("itemCell" + num13);
                            if (transform2 != null)
                            {
                                gameObject = transform2.gameObject;
                            }
                            if ((num13 < view6.Count) && (gameObject != null))
                            {
                                ResNobeInfo info4 = view6[num13];
                                if (gameObject != null)
                                {
                                    gameObject.CustomSetActive(true);
                                    Image image6 = gameObject.transform.Find("imgIcon").GetComponent<Image>();
                                    Text text3 = gameObject.transform.Find("Text").GetComponent<Text>();
                                    string str7 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, Utility.UTF8Convert(info4.szResIcon));
                                    CUIUtility.SetImageSprite(image6, str7, null, true, false, false);
                                    text3.text = Utility.UTF8Convert(info4.szResName);
                                }
                            }
                            else if (gameObject != null)
                            {
                                gameObject.CustomSetActive(false);
                            }
                        }
                    }
                }
                if (dwCurLevel == 0)
                {
                    script.MoveElementInScrollArea(dwCurLevel, false);
                    this.m_CurPage = dwCurLevel;
                    this.UpdateNobeLevelText(this.m_CurPage);
                    this.UpdateNobeGiftIcon(this.m_CurPage);
                }
                else
                {
                    script.MoveElementInScrollArea(dwCurLevel - 1, false);
                    this.m_CurPage = dwCurLevel - 1;
                    this.UpdateNobeLevelText(this.m_CurPage);
                    this.UpdateNobeGiftIcon(this.m_CurPage);
                }
                this.UpdateBaseInfolevel(this.m_CurPage);
                if (script != null)
                {
                    Transform transform3 = script.transform.Find("ScrollRect");
                    if (transform3 != null)
                    {
                        ScrollRect rect = transform3.GetComponent<ScrollRect>();
                        if (rect != null)
                        {
                            rect.horizontal = false;
                            rect.vertical = false;
                        }
                    }
                }
            }
        }

        private void UpdateBaseInfolevel(int curPage)
        {
            if (curPage == 0)
            {
                this.m_BaseInfoOnLeftObj.CustomSetActive(false);
            }
            else
            {
                this.m_BaseInfoOnLeftObj.CustomSetActive(true);
            }
            if (curPage == 7)
            {
                this.m_BaseInfoOnRightObj.CustomSetActive(false);
            }
            else
            {
                this.m_BaseInfoOnRightObj.CustomSetActive(true);
            }
        }

        private void UpdateCHangeHeadFormData(int nHeadIDX)
        {
            if (this.m_ChangeHeadIconForm != null)
            {
                int dwCurLevel = (int) this.m_vipInfo.stGameVipClient.dwCurLevel;
                ListView<ResNobeInfo> changeHeadIconResList = this.m_ChangeHeadIconResList;
                int count = changeHeadIconResList.Count;
                CUIListScript component = this.m_ChangeHeadIconForm.GetWidget(0).GetComponent<CUIListScript>();
                component.SetElementAmount(count);
                for (int i = 0; i < count; i++)
                {
                    CUIListElementScript elemenet = component.GetElemenet(i);
                    ResNobeInfo info = changeHeadIconResList[i];
                    string str = Utility.UTF8Convert(info.szResIcon);
                    string str2 = Utility.UTF8Convert(info.szResName);
                    GameObject gameObject = elemenet.transform.Find("itemCell").gameObject;
                    Image image = gameObject.transform.Find("imgIcon").GetComponent<Image>();
                    string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, str);
                    CUIUtility.SetImageSprite(image, prefabPath, null, true, false, false);
                    gameObject.transform.Find("Text").GetComponent<Text>().text = str2;
                    if (info.dwNobeLevel <= dwCurLevel)
                    {
                        GameObject obj3 = gameObject.transform.Find("Btn/Btn").gameObject;
                        GameObject obj4 = gameObject.transform.Find("Btn/Text").gameObject;
                        if (nHeadIDX == info.dwID)
                        {
                            obj4.CustomSetActive(true);
                            obj4.GetComponent<Text>().text = "已使用";
                            obj3.CustomSetActive(false);
                        }
                        else
                        {
                            obj4.CustomSetActive(false);
                            obj3.CustomSetActive(true);
                        }
                        continue;
                    }
                    gameObject.transform.Find("Btn/Btn").gameObject.CustomSetActive(false);
                    GameObject obj6 = gameObject.transform.Find("Btn/Text").gameObject;
                    obj6.CustomSetActive(true);
                    string str5 = string.Empty;
                    switch (i)
                    {
                        case 0:
                            str5 = "贵族1级解锁";
                            break;

                        case 1:
                            str5 = "贵族4级解锁";
                            break;

                        case 2:
                            str5 = "贵族7级解锁";
                            break;
                    }
                    obj6.GetComponent<Text>().text = str5;
                }
            }
        }

        private void UpdateGrowInfo()
        {
            if (this.m_vipInfo != null)
            {
                Image component = this.m_Form.GetWidget(8).GetComponent<Image>();
                Text text = this.m_Form.GetWidget(9).GetComponent<Text>();
                Image image = this.m_Form.GetWidget(0x10).GetComponent<Image>();
                Image image3 = this.m_Form.GetWidget(0x11).GetComponent<Image>();
                Image image4 = this.m_Form.GetWidget(6).GetComponent<Image>();
                Image image5 = this.m_Form.GetWidget(7).GetComponent<Image>();
                Transform parent = null;
                if (image5 != null)
                {
                    parent = image5.transform.parent;
                }
                Text text2 = this.m_Form.GetWidget(0x12).GetComponent<Text>();
                int dwCurLevel = (int) this.m_vipInfo.stGameVipClient.dwCurLevel;
                int dwScore = (int) this.m_vipInfo.stGameVipClient.dwScore;
                int num3 = GameDataMgr.resVipDianQuan.Count();
                GameObject widget = this.m_Form.GetWidget(0x17);
                widget.CustomSetActive(false);
                if (dwCurLevel == 0)
                {
                    image4.color = new Color(image4.color.r, image4.color.g, image4.color.b, 0.5f);
                    image.gameObject.CustomSetActive(false);
                }
                else
                {
                    image4.color = new Color(image4.color.r, image4.color.g, image4.color.b, 1f);
                    image.gameObject.CustomSetActive(true);
                }
                string str = string.Empty;
                if (dwCurLevel >= num3)
                {
                    str = "贵族等级已满";
                    component.CustomFillAmount(1f);
                    text.gameObject.CustomSetActive(true);
                    text.text = str;
                    string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
                    CUIUtility.SetImageSprite(image, prefabPath, null, true, false, false);
                    image5.gameObject.CustomSetActive(false);
                    if (parent != null)
                    {
                        parent.gameObject.CustomSetActive(false);
                    }
                    ResVIPCoupons dataByKey = GameDataMgr.resVipDianQuan.GetDataByKey((long) dwCurLevel);
                    if (dataByKey != null)
                    {
                        text2.text = string.Format("{0}/{1}", dwScore, (int) dataByKey.dwConsumeCoupons);
                    }
                }
                else
                {
                    int num4 = dwCurLevel + 1;
                    ResVIPCoupons coupons2 = GameDataMgr.resVipDianQuan.GetDataByKey((long) num4);
                    if (coupons2 != null)
                    {
                        int num5 = ((int) coupons2.dwConsumeCoupons) - dwScore;
                        if (num5 > 0)
                        {
                            str = string.Format("再消费{0}点券，升级到贵族{1}", num5, num4);
                            text.gameObject.CustomSetActive(false);
                            this.UpdateImageLevelTips(widget, num4, num5, false);
                            component.CustomFillAmount((1f * dwScore) / ((float) coupons2.dwConsumeCoupons));
                            string str3 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
                            CUIUtility.SetImageSprite(image, str3, null, true, false, false);
                            str3 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, (dwCurLevel + 1).ToString());
                            CUIUtility.SetImageSprite(image3, str3, null, true, false, false);
                            image5.gameObject.CustomSetActive(true);
                            image4.gameObject.CustomSetActive(true);
                            if (parent != null)
                            {
                                parent.gameObject.CustomSetActive(true);
                            }
                            text2.text = string.Format("{0}/{1}", dwScore, (int) coupons2.dwConsumeCoupons);
                        }
                        else
                        {
                            int dwMaxLevel = (int) this.m_vipInfo.dwMaxLevel;
                            ResVIPCoupons coupons3 = GameDataMgr.resVipDianQuan.GetDataByKey((long) dwMaxLevel);
                            str = "只要消费任意金额的点券，即可恢复到之前的最高等级";
                            component.CustomFillAmount((1f * dwScore) / ((float) coupons3.dwConsumeCoupons));
                            text.gameObject.CustomSetActive(true);
                            text.text = str;
                            string str4 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
                            CUIUtility.SetImageSprite(image, str4, null, true, false, false);
                            str4 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwMaxLevel.ToString());
                            CUIUtility.SetImageSprite(image3, str4, null, true, false, false);
                            image5.gameObject.CustomSetActive(true);
                            image4.gameObject.CustomSetActive(true);
                            if (parent != null)
                            {
                                parent.gameObject.CustomSetActive(true);
                            }
                            text2.text = string.Format("{0}/{1}", dwScore, (int) coupons3.dwConsumeCoupons);
                        }
                    }
                }
                CUIListScript script = this.m_Form.GetWidget(4).GetComponent<CUIListScript>();
                int amount = num3 / 5;
                float num8 = (num3 * 1f) / 5f;
                if ((num8 - amount) > 0f)
                {
                    amount++;
                }
                script.SetElementAmount(amount);
                DictionaryView<int, ListView<ResVIPCoupons>> view = new DictionaryView<int, ListView<ResVIPCoupons>>();
                for (int i = 1; i <= GameDataMgr.resVipDianQuan.count; i++)
                {
                    ResVIPCoupons item = GameDataMgr.resVipDianQuan.GetDataByKey((long) i);
                    int key = (i - 1) / 5;
                    if (!view.ContainsKey(key))
                    {
                        ListView<ResVIPCoupons> view2 = new ListView<ResVIPCoupons>();
                        view2.Add(item);
                        view.Add(key, view2);
                    }
                    else
                    {
                        view[key].Add(item);
                    }
                }
                for (int j = 0; j < amount; j++)
                {
                    CUIListElementScript elemenet = script.GetElemenet(j);
                    if (view.ContainsKey(j))
                    {
                        ListView<ResVIPCoupons> view4 = view[j];
                        for (int k = 0; k < 5; k++)
                        {
                            GameObject gameObject = null;
                            Transform transform2 = elemenet.transform.Find("itemCell" + k);
                            if (transform2 != null)
                            {
                                gameObject = transform2.gameObject;
                            }
                            if ((k < view4.Count) && (gameObject != null))
                            {
                                ResVIPCoupons coupons5 = view4[k];
                                gameObject.CustomSetActive(true);
                                gameObject.transform.Find("Text").GetComponent<Text>().text = string.Format("<size=24><color=#f3CA4Dff>{0}</color></size><size=18><color=#7e88a2ff>积分</color></size>", coupons5.dwConsumeCoupons.ToString());
                                Image image6 = gameObject.transform.Find("imgIcon/Img_Bg1").GetComponent<Image>();
                                string str5 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, coupons5.dwVIPLevel);
                                CUIUtility.SetImageSprite(image6, str5, null, true, false, false);
                            }
                            else if (gameObject != null)
                            {
                                gameObject.CustomSetActive(false);
                            }
                        }
                    }
                }
                this.UpdateGrowInfoLevel(0);
                if (script != null)
                {
                    this.m_CurPageLevel = 0;
                    script.MoveElementInScrollArea(this.m_CurPageLevel, true);
                }
                if (script != null)
                {
                    Transform transform3 = script.transform.Find("ScrollRect");
                    if (transform3 != null)
                    {
                        ScrollRect rect = transform3.GetComponent<ScrollRect>();
                        if (rect != null)
                        {
                            rect.horizontal = false;
                            rect.vertical = false;
                        }
                    }
                }
            }
        }

        private void UpdateGrowInfoLevel(int curPage)
        {
            if (curPage == 0)
            {
                this.m_GrowInfoOnLeftObj.CustomSetActive(false);
            }
            else
            {
                this.m_GrowInfoOnLeftObj.CustomSetActive(true);
            }
            if (curPage == 1)
            {
                this.m_GrowInfoOnRightObj.CustomSetActive(false);
            }
            else
            {
                this.m_GrowInfoOnRightObj.CustomSetActive(true);
            }
        }

        private void UpdateImageLevelTips(GameObject parent, int level, int resPoint, bool bdiamondForm = false)
        {
            parent.CustomSetActive(true);
            Transform[] transformArray = new Transform[5];
            for (int i = 0; i < 5; i++)
            {
                transformArray[i] = parent.transform.Find("n" + i);
                transformArray[i].gameObject.CustomSetActive(false);
                if (bdiamondForm)
                {
                    transformArray[i].GetComponent<RectTransform>().anchoredPosition = this.m_NumPosBackDiamondForm[i];
                }
                else
                {
                    transformArray[i].GetComponent<RectTransform>().anchoredPosition = this.m_NumPosBack[i];
                }
            }
            int length = resPoint.ToString().Length;
            int num3 = 5 - length;
            if (num3 >= 0)
            {
                string[] strArray = new string[5];
                string str = resPoint.ToString();
                for (int j = 0; j < num3; j++)
                {
                    strArray[j] = "-1";
                }
                for (int k = num3; k < 5; k++)
                {
                    strArray[k] = str.Substring(k - num3, 1);
                }
                float num6 = 0f;
                for (int m = 0; m < 5; m++)
                {
                    if (strArray[m] == "-1")
                    {
                        num6 += Mathf.Abs((float) (transformArray[m].GetComponent<RectTransform>().anchoredPosition.x - transformArray[m + 1].GetComponent<RectTransform>().anchoredPosition.x));
                        transformArray[m].gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        transformArray[m].gameObject.CustomSetActive(true);
                        string str2 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, strArray[m].ToString());
                        CUIUtility.SetImageSprite(transformArray[m].gameObject.GetComponent<Image>(), str2, null, true, false, false);
                        transformArray[m].GetComponent<RectTransform>().anchoredPosition = new Vector2(transformArray[m].GetComponent<RectTransform>().anchoredPosition.x - num6, transformArray[m].GetComponent<RectTransform>().anchoredPosition.y);
                    }
                }
                Transform transform = parent.transform.Find("tipsRight");
                if (bdiamondForm)
                {
                    transform.GetComponent<RectTransform>().anchoredPosition = this.m_NumPosBackDiamondForm[5];
                }
                else
                {
                    transform.GetComponent<RectTransform>().anchoredPosition = this.m_NumPosBack[5];
                }
                Transform transform2 = parent.transform.Find("tipsRight/vipLevel");
                string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, level.ToString());
                CUIUtility.SetImageSprite(transform2.gameObject.GetComponent<Image>(), prefabPath, null, true, false, false);
                transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(transform.GetComponent<RectTransform>().anchoredPosition.x - num6, transform.GetComponent<RectTransform>().anchoredPosition.y);
            }
        }

        private void UpdateNobeGiftIcon(int level)
        {
            level++;
            int num = 1;
            int num2 = 3;
            if (this.m_Form != null)
            {
                GameObject widget = this.m_Form.GetWidget(1);
                for (int i = 1; i <= GameDataMgr.resNobeInfoDatabin.count; i++)
                {
                    ResNobeInfo dataByIndex = GameDataMgr.resNobeInfoDatabin.GetDataByIndex(i - 1);
                    if (((dataByIndex != null) && (dataByIndex.bResType == 3)) && (dataByIndex.dwNobeLevel == level))
                    {
                        Transform transform = widget.transform.Find("chestPanel/GiftPanel/Gift" + num);
                        if (transform != null)
                        {
                            transform.gameObject.CustomSetActive(true);
                            Transform transform2 = transform.FindChild(string.Format("Icon", new object[0]));
                            Transform transform3 = transform.FindChild(string.Format("Text", new object[0]));
                            transform.FindChild(string.Format("ExperienceCard", new object[0])).gameObject.CustomSetActive(dataByIndex.bShowIdx == 1);
                            transform.FindChild(string.Format("TextNum", new object[0])).gameObject.GetComponent<Text>().text = (dataByIndex.dwJiaoBiaoNum <= 0) ? null : dataByIndex.dwJiaoBiaoNum.ToString(CultureInfo.InvariantCulture);
                            string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_Dynamic_Icon_Dir, Utility.UTF8Convert(dataByIndex.szResIcon));
                            if ((transform2 != null) && (transform3 != null))
                            {
                                transform2.gameObject.GetComponent<Image>().SetSprite(prefabPath, this.m_Form, true, false, false);
                                transform3.gameObject.GetComponent<Text>().text = Utility.UTF8Convert(dataByIndex.szResName);
                                num++;
                            }
                        }
                    }
                }
                for (int j = num; j <= num2; j++)
                {
                    Transform transform6 = widget.transform.Find("chestPanel/GiftPanel/Gift" + j);
                    if (transform6 != null)
                    {
                        transform6.gameObject.CustomSetActive(false);
                    }
                }
            }
        }

        private void UpdateNobeLevelChange()
        {
            if (this.m_Form != null)
            {
                GameObject widget = this.m_Form.GetWidget(1);
                GameObject obj3 = this.m_Form.GetWidget(2);
                Text component = this.m_Form.GetWidget(3).GetComponent<Text>();
                switch (this.m_CurTab)
                {
                    case Tab.Base_Info:
                        if ((widget != null) && widget.activeSelf)
                        {
                            this.UpdateBaseInfo();
                        }
                        break;

                    case Tab.Grow_Info:
                        if (((widget != null) && (obj3 != null)) && obj3.activeSelf)
                        {
                            this.UpdateGrowInfo();
                        }
                        break;
                }
            }
        }

        private void UpdateNobeLevelText(int level)
        {
            level++;
            string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, level.ToString());
            Image component = this.m_Form.GetWidget(0x18).GetComponent<Image>();
            Image image = this.m_Form.GetWidget(0x19).GetComponent<Image>();
            CUIUtility.SetImageSprite(component, prefabPath, null, true, false, false);
            CUIUtility.SetImageSprite(image, prefabPath, null, true, false, false);
        }

        public Tab CurTab
        {
            get
            {
                return this.m_CurTab;
            }
            set
            {
                this.m_CurTab = value;
            }
        }

        public class DelayNobeInfo
        {
            public SCPKG_GAME_VIP_NTF m_vipInfoBack = new SCPKG_GAME_VIP_NTF();
            public int nShow = -1;
            public int Type;
        }

        public enum enPlayerFormWidget
        {
            Tab,
            Base_Info_Tab,
            Grow_Info_Tab,
            Top_Title_Text,
            GrowInfo_Reawrd_List,
            GrowInfo_Rule_Desc,
            GrowInfo_Head_Left,
            GrowInfo_Head_Right,
            GrowInfo_Head_Progress,
            GrowInfo_Head_Progress_Tips,
            BaseInfo_Head_Left,
            BaseInfo_Head_Right,
            BaseInfo_Head_Progress,
            BaseInfo_Head_Progress_Tips,
            BaseInfo_Head_Left_LevelText,
            BaseInfo_Head_Right_LevelText,
            GrowInfo_Head_Left_LevelText,
            GrowInfo_Head_Right_LevelText,
            GrowInfo_Level_Desc,
            BaseInfo_Level_Desc,
            BaseInfo_Reawrd_List,
            BaseInfo_Noble_LevelText,
            BaseInfo_Image_Level_Tips,
            GrowInfo_Image_Level_Tips,
            BaseInfo_Noble_LevelNUm1,
            BaseInfo_Noble_LevelNUm2,
            BaseInfo_OnLeftObject,
            BaseInfo_OnRightObject,
            GrowInfo_OnLeftObject,
            GrowInfo_OnRightObject
        }

        protected enum NobeLevelUpFormWidget
        {
            CurNobeLv = 0,
            CurNobeLv2 = 2,
            Gift = 4,
            LastNobeLv = 1,
            None = -1,
            TeQuanPanel = 3
        }

        private enum NobelLoseFormWidget
        {
            TextContrl,
            TeQuanPanel,
            TeQuanPanelText
        }

        public enum Tab
        {
            Base_Info,
            Grow_Info
        }
    }
}

