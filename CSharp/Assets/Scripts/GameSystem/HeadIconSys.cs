namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class HeadIconSys : Singleton<HeadIconSys>
    {
        private DictionaryView<enHeadImgTab, ListView<ResHeadImage>> headImageDic = new DictionaryView<enHeadImgTab, ListView<ResHeadImage>>();
        private ListView<HeadImgInfo> m_headImgInfo = new ListView<HeadImgInfo>();
        public static readonly string s_headImgChgForm = (CUIUtility.s_IDIP_Form_Dir + "Form_HeadChangeIcon");

        public void AddHeadImgInfo(uint dwHeadImgID, uint dwGetTime)
        {
            HeadImgInfo item = new HeadImgInfo {
                dwID = dwHeadImgID,
                dwGetTime = dwGetTime,
                bNtfFlag = 1
            };
            for (int i = 0; i < this.m_headImgInfo.Count; i++)
            {
                if (this.m_headImgInfo[i].dwID == dwHeadImgID)
                {
                    this.m_headImgInfo.RemoveAt(i);
                    i--;
                }
            }
            this.m_headImgInfo.Add(item);
        }

        public void Clear()
        {
            this.headImageDic.Clear();
            this.m_headImgInfo.Clear();
        }

        public void ClearHeadImgFlag(uint headImg)
        {
            for (int i = 0; i < this.m_headImgInfo.Count; i++)
            {
                if (this.m_headImgInfo[i].dwID == headImg)
                {
                    this.m_headImgInfo[i].bNtfFlag = 0;
                }
            }
        }

        private int ComparisonByTab(ResHeadImage left, ResHeadImage right)
        {
            HeadImgInfo info = this.GetInfo(left.dwID);
            HeadImgInfo info2 = this.GetInfo(right.dwID);
            if ((info != null) && (info2 == null))
            {
                return -10000;
            }
            if ((info == null) && (info2 != null))
            {
                return 0x2710;
            }
            if ((info == null) && (info2 == null))
            {
                if (left.bHeadType < right.bHeadType)
                {
                    return -1;
                }
                if ((left.bHeadType <= right.bHeadType) && (left.dwID < right.dwID))
                {
                    return -1;
                }
                return 1;
            }
            if ((info != null) && (info2 != null))
            {
                if ((info.bNtfFlag == 1) && (info2.bNtfFlag == 0))
                {
                    return -100;
                }
                if ((info.bNtfFlag == 0) && (info2.bNtfFlag == 1))
                {
                    return 100;
                }
                if ((info.bNtfFlag == 0) && (info2.bNtfFlag == 0))
                {
                    if (left.bHeadType < right.bHeadType)
                    {
                        return -1;
                    }
                    if ((left.bHeadType <= right.bHeadType) && (left.dwID < right.dwID))
                    {
                        return -1;
                    }
                    return 1;
                }
            }
            return 0;
        }

        private int ComparisonByTime(ResHeadImage left, ResHeadImage right)
        {
            HeadImgInfo info = this.GetInfo(left.dwID);
            HeadImgInfo info2 = this.GetInfo(right.dwID);
            if ((info != null) && (info2 == null))
            {
                return -10000;
            }
            if ((info == null) && (info2 != null))
            {
                return 0x2710;
            }
            if ((info == null) && (info2 == null))
            {
                if (left.bSortWeight > right.bSortWeight)
                {
                    return -10;
                }
                if (left.bSortWeight < right.bSortWeight)
                {
                    return 10;
                }
                if (left.dwID < right.dwID)
                {
                    return -1;
                }
                return 1;
            }
            if ((info == null) || (info2 == null))
            {
                return 0;
            }
            if ((info.bNtfFlag == 1) && (info2.bNtfFlag == 0))
            {
                return -1000;
            }
            if ((info.bNtfFlag == 0) && (info2.bNtfFlag == 1))
            {
                return 0x3e8;
            }
            if (info.dwGetTime > info2.dwGetTime)
            {
                return -100;
            }
            if (info.dwGetTime < info2.dwGetTime)
            {
                return 100;
            }
            if (left.bSortWeight > right.bSortWeight)
            {
                return -10;
            }
            if (left.bSortWeight < right.bSortWeight)
            {
                return 10;
            }
            if (left.dwID < right.dwID)
            {
                return -1;
            }
            return 1;
        }

        private int ComparisonByWeight(ResHeadImage left, ResHeadImage right)
        {
            HeadImgInfo info = this.GetInfo(left.dwID);
            HeadImgInfo info2 = this.GetInfo(right.dwID);
            if ((info != null) && (info2 == null))
            {
                return -10000;
            }
            if ((info == null) && (info2 != null))
            {
                return 0x2710;
            }
            if ((info == null) && (info2 == null))
            {
                if (left.bSortWeight > right.bSortWeight)
                {
                    return -10;
                }
                if (left.bSortWeight < right.bSortWeight)
                {
                    return 10;
                }
                if (left.dwID < right.dwID)
                {
                    return -1;
                }
                return 1;
            }
            if ((info == null) || (info2 == null))
            {
                return 0;
            }
            if ((info.bNtfFlag == 1) && (info2.bNtfFlag == 0))
            {
                return -1000;
            }
            if ((info.bNtfFlag == 0) && (info2.bNtfFlag == 1))
            {
                return 0x3e8;
            }
            if (left.bSortWeight > right.bSortWeight)
            {
                return -10;
            }
            if (left.bSortWeight < right.bSortWeight)
            {
                return 10;
            }
            if (left.dwID < right.dwID)
            {
                return -1;
            }
            return 1;
        }

        public void DelHeadImgInfo(uint dwHeadImgID)
        {
            for (int i = 0; i < this.m_headImgInfo.Count; i++)
            {
                if (this.m_headImgInfo[i].dwID == dwHeadImgID)
                {
                    this.m_headImgInfo.RemoveAt(i);
                    i--;
                }
            }
        }

        private ListView<ResHeadImage> GetCurHeadImgList(enHeadImgTab curTab)
        {
            ListView<ResHeadImage> view = null;
            if (!this.headImageDic.TryGetValue(curTab, out view))
            {
                DictionaryView<uint, ResHeadImage>.Enumerator enumerator = GameDataMgr.headImageDict.GetEnumerator();
                RES_HEADIMG_SOURCE_TYPE headType = this.GetHeadType(curTab);
                view = new ListView<ResHeadImage>();
                if (GameDataMgr.headImageDict.Count == 0)
                {
                    return view;
                }
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, ResHeadImage> current = enumerator.Current;
                    ResHeadImage item = current.Value;
                    if ((headType == RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_MAX) || (headType == ((RES_HEADIMG_SOURCE_TYPE) item.bHeadType)))
                    {
                        view.Add(item);
                    }
                }
                this.headImageDic.Add(curTab, view);
            }
            return view;
        }

        private enHeadImgTab GetCurTab()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_headImgChgForm);
            if (form != null)
            {
                return (enHeadImgTab) Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBg/Panel_Menu/List").GetSelectedIndex();
            }
            return enHeadImgTab.All;
        }

        public string GetHeadIdxResName(int id)
        {
            ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(enHeadImgTab.All);
            for (int i = 0; i < curHeadImgList.Count; i++)
            {
                ResHeadImage image = curHeadImgList[i];
                if ((image != null) && (image.dwID == id))
                {
                    return Utility.UTF8Convert(image.szHeadIcon);
                }
            }
            return string.Empty;
        }

        private RES_HEADIMG_SOURCE_TYPE GetHeadType(enHeadImgTab type)
        {
            if (type != enHeadImgTab.All)
            {
                if (type == enHeadImgTab.Nobe)
                {
                    return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_NOBE;
                }
                if (type == enHeadImgTab.Activity)
                {
                    return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_ACTIVITY;
                }
                if (type == enHeadImgTab.Skin)
                {
                    return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_SKIN;
                }
                if (type == enHeadImgTab.Team)
                {
                    return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_BATTLE;
                }
            }
            return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_MAX;
        }

        public HeadImgInfo GetInfo(uint headImgId)
        {
            for (int i = 0; i < this.m_headImgInfo.Count; i++)
            {
                if (this.m_headImgInfo[i].dwID == headImgId)
                {
                    return this.m_headImgInfo[i];
                }
            }
            return null;
        }

        public ResHeadImage GetResInfo(uint headImgId)
        {
            ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(enHeadImgTab.All);
            for (int i = 0; i < curHeadImgList.Count; i++)
            {
                if (curHeadImgList[i].dwID == headImgId)
                {
                    return curHeadImgList[i];
                }
            }
            return null;
        }

        private ResHeadImage GetSelectedHeadImg()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_headImgChgForm);
            if (form != null)
            {
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBg/Panel_Menu/List");
                CUIListScript script3 = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBg/Panel_Main/List");
                int selectedIndex = componetInChild.GetSelectedIndex();
                int num2 = script3.GetSelectedIndex();
                ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList((enHeadImgTab) selectedIndex);
                if ((num2 > -1) && (num2 < curHeadImgList.Count))
                {
                    return curHeadImgList[num2];
                }
            }
            return null;
        }

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Form_Open, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Form_Open));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Form_Close, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Form_Close));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Tab_Click, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Tab_Click));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Icon_Click, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Icon_Click));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Confirm, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Confirm));
            Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Item_Enable, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Item_Enable));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.NOBE_STATE_CHANGE, new System.Action(this.OnHeadInfoRefresh));
        }

        private bool IsHeadIconInUse(uint headIconId)
        {
            return (MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId == headIconId);
        }

        private void On_HeadIcon_Change_Confirm(CUIEvent uiEvent)
        {
            ResHeadImage selectedHeadImg = this.GetSelectedHeadImg();
            if ((selectedHeadImg != null) && (MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId != selectedHeadImg.dwID))
            {
                OnHeadIconChangeReq(selectedHeadImg.dwID);
            }
        }

        private void On_HeadIcon_Change_Form_Close(CUIEvent uiEvent)
        {
        }

        private void On_HeadIcon_Change_Icon_Click(CUIEvent uiEvent)
        {
            if (Singleton<CUIManager>.instance.GetForm(s_headImgChgForm) != null)
            {
                int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
                this.OnShowDetailPanel(selectedIndex);
                ResHeadImage selectedHeadImg = this.GetSelectedHeadImg();
                if (selectedHeadImg != null)
                {
                    HeadImgInfo info = this.GetInfo(selectedHeadImg.dwID);
                    if ((info != null) && (info.bNtfFlag == 1))
                    {
                        OnHeadIconFlagClearReq(info.dwID);
                    }
                }
            }
        }

        private void On_HeadIcon_Change_Item_Enable(CUIEvent uiEvent)
        {
            enHeadImgTab curTab = this.GetCurTab();
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(curTab);
            this.OnUpdateElement(uiEvent.m_srcWidget, srcWidgetIndexInBelongedList, curTab);
        }

        private void On_HeadIcon_Change_Tab_Click(CUIEvent uiEvent)
        {
            enHeadImgTab curTab = this.GetCurTab();
            this.SortResDic(curTab);
            this.OnShowMainPanel(curTab);
        }

        private void On_HeadIcon_Form_Open(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.instance.OpenForm(s_headImgChgForm, false, false);
            if (script != null)
            {
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(script.gameObject, "pnlBg/Panel_Menu/List");
                componetInChild.SetElementAmount(5);
                for (int i = 0; i < 5; i++)
                {
                    componetInChild.GetElemenet(i).GetComponentInChildren<Text>().text = Singleton<CTextManager>.instance.GetText(string.Format("HeadImg_Tab_Txt_{0}", i + 1));
                }
                componetInChild.SelectElement(0, true);
            }
        }

        [MessageHandler(0x11ff)]
        public static void OnHeadIconAddNtf(CSPkg msg)
        {
            Singleton<HeadIconSys>.instance.AddHeadImgInfo(msg.stPkgData.stHeadImgAddNtf.dwHeadImgID, msg.stPkgData.stHeadImgAddNtf.dwGetTime);
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.HEAD_IMAGE_FLAG_CHANGE);
        }

        [MessageHandler(0x11fd)]
        public static void OnHeadIconChangeNtf(CSPkg msg)
        {
            Singleton<HeadIconSys>.instance.SetMasterHeadImg(msg.stPkgData.stHeadImgChgNtf.dwHeadImgID);
            Singleton<HeadIconSys>.instance.RefreshUseButton(false);
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.NOBE_STATE_CHANGE);
        }

        private static void OnHeadIconChangeReq(uint headImgId)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x11f9);
            msg.stPkgData.stHeadImgChgReq.dwHeadImgID = headImgId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        [MessageHandler(0x11fa)]
        public static void OnHeadIconChangeRsp(CSPkg msg)
        {
            if (msg.stPkgData.stHeadImgChgRsp.iResult == 0)
            {
                Singleton<HeadIconSys>.instance.SetMasterHeadImg(msg.stPkgData.stHeadImgChgRsp.dwHeadImgID);
                Singleton<EventRouter>.instance.BroadCastEvent(EventID.NOBE_STATE_CHANGE);
            }
        }

        [MessageHandler(0x1200)]
        public static void OnHeadIconDelNtf(CSPkg msg)
        {
            Singleton<HeadIconSys>.instance.DelHeadImgInfo(msg.stPkgData.stHeadImgDelNtf.dwHeadImgID);
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.HEAD_IMAGE_FLAG_CHANGE);
        }

        private static void OnHeadIconFlagClearReq(uint headImgId)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x11fb);
            msg.stPkgData.stHeadImgFlagClrReq.dwHeadImgID = headImgId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        [MessageHandler(0x11fc)]
        public static void OnHeadIconFlagClearRsp(CSPkg msg)
        {
            Singleton<HeadIconSys>.instance.ClearHeadImgFlag(msg.stPkgData.stHeadImgFlagClrRsp.dwHeadImgID);
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.NOBE_STATE_CHANGE);
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.HEAD_IMAGE_FLAG_CHANGE);
        }

        public void OnHeadIconSyncList(ushort count, COMDT_ACNT_HEADIMG_INFO[] astHeadImgInfo)
        {
            this.m_headImgInfo.Clear();
            for (int i = 0; i < count; i++)
            {
                HeadImgInfo item = new HeadImgInfo {
                    dwID = astHeadImgInfo[i].dwID,
                    dwGetTime = astHeadImgInfo[i].dwGetTime,
                    bNtfFlag = astHeadImgInfo[i].bNtfFlag
                };
                this.m_headImgInfo.Add(item);
            }
        }

        [MessageHandler(0x11fe)]
        public static void OnHeadIconSyncNtf(CSPkg msg)
        {
            Singleton<HeadIconSys>.instance.OnHeadIconSyncList(msg.stPkgData.stHeadImgListSync.stHeadImgList.wHeadImgCnt, msg.stPkgData.stHeadImgListSync.stHeadImgList.astHeadImgInfo);
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.HEAD_IMAGE_FLAG_CHANGE);
        }

        private void OnHeadInfoRefresh()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_headImgChgForm);
            if (form != null)
            {
                enHeadImgTab curTab = this.GetCurTab();
                int count = this.GetCurHeadImgList(curTab).Count;
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBg/Panel_Main/List");
                CUIListElementScript elemenet = null;
                for (int i = 0; i < count; i++)
                {
                    elemenet = componetInChild.GetElemenet(i);
                    if ((elemenet != null) && (elemenet.gameObject != null))
                    {
                        this.OnUpdateElement(elemenet.gameObject, i, curTab);
                    }
                }
            }
        }

        private void OnShowDetailPanel(int index)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_headImgChgForm);
            if (form != null)
            {
                enHeadImgTab curTab = this.GetCurTab();
                ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(curTab);
                if (index == -1)
                {
                    Utility.FindChild(form.gameObject, "pnlBg/Panel_Detail/Node").SetActive(false);
                }
                else if (index < curHeadImgList.Count)
                {
                    Utility.FindChild(form.gameObject, "pnlBg/Panel_Detail/Node").SetActive(true);
                    ResHeadImage image = curHeadImgList[index];
                    HeadImgInfo info = this.GetInfo(image.dwID);
                    Text componetInChild = Utility.GetComponetInChild<Text>(form.gameObject, "pnlBg/Panel_Detail/Node/DescTxt");
                    Text text2 = Utility.GetComponetInChild<Text>(form.gameObject, "pnlBg/Panel_Detail/Node/AvaildTimeTxt");
                    Image image2 = Utility.GetComponetInChild<Image>(form.gameObject, "pnlBg/Panel_Detail/Node/HeadImg");
                    Button button = Utility.GetComponetInChild<Button>(form.gameObject, "pnlBg/Panel_Detail/Node/Button");
                    Text text3 = Utility.GetComponetInChild<Text>(form.gameObject, "pnlBg/Panel_Detail/Node/SrcTxt");
                    componetInChild.text = image.szHeadDesc;
                    image2.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Nobe_Dir, image.szHeadIcon), form, true, false, false);
                    if (image.dwValidSecond == 0)
                    {
                        text2.text = Singleton<CTextManager>.instance.GetText("HeadImg_Tips_1");
                    }
                    else if (info != null)
                    {
                        DateTime time = Utility.ToUtcTime2Local(CRoleInfo.GetCurrentUTCTime() + image.dwValidSecond);
                        string[] args = new string[] { time.Year.ToString(), time.Month.ToString(), time.Day.ToString() };
                        text2.text = Singleton<CTextManager>.instance.GetText("HeadImg_Tips_2", args);
                    }
                    else
                    {
                        string[] textArray2 = new string[] { Math.Ceiling((double) (((float) image.dwValidSecond) / 86400f)).ToString() };
                        text2.text = Singleton<CTextManager>.instance.GetText("HeadImg_Tips_3", textArray2);
                    }
                    if (info != null)
                    {
                        button.gameObject.CustomSetActive(!this.IsHeadIconInUse(info.dwID));
                        text3.gameObject.SetActive(false);
                    }
                    else
                    {
                        button.gameObject.CustomSetActive(false);
                        text3.gameObject.SetActive(true);
                        text3.text = image.szHeadAccess;
                    }
                }
            }
        }

        private void OnShowMainPanel(enHeadImgTab curTab)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_headImgChgForm);
            if (form != null)
            {
                ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(curTab);
                int count = curHeadImgList.Count;
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBg/Panel_Main/List");
                componetInChild.SetElementAmount(count);
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    if (this.IsHeadIconInUse(curHeadImgList[i].dwID))
                    {
                        index = i;
                        break;
                    }
                }
                if (count > 0)
                {
                    componetInChild.SelectElement(index, true);
                }
                else
                {
                    this.OnShowDetailPanel(-1);
                }
            }
        }

        private void OnUpdateElement(GameObject element, int index, enHeadImgTab curTab)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(s_headImgChgForm);
            if (form != null)
            {
                ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(curTab);
                if ((index >= 0) && (index < curHeadImgList.Count))
                {
                    ResHeadImage image = curHeadImgList[index];
                    if (image != null)
                    {
                        HeadImgInfo info;
                        info = info = this.GetInfo(curHeadImgList[index].dwID);
                        Utility.GetComponetInChild<Image>(element, "HeadImg").SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Nobe_Dir, image.szHeadIcon), form, true, false, false);
                        if (info != null)
                        {
                            Utility.FindChild(element, "Flag").CustomSetActive(info.bNtfFlag == 1);
                            Utility.FindChild(element, "Lock").CustomSetActive(false);
                            Utility.FindChild(element, "Text").CustomSetActive(this.IsHeadIconInUse(info.dwID));
                        }
                        else
                        {
                            Utility.FindChild(element, "Flag").CustomSetActive(false);
                            Utility.FindChild(element, "Lock").CustomSetActive(true);
                            Utility.FindChild(element, "Text").CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void RefreshUseButton(bool isShowUseButton)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_headImgChgForm);
            if (form != null)
            {
                GameObject obj2 = Utility.FindChild(form.gameObject, "pnlBg/Panel_Detail/Node/Button");
                if (obj2 != null)
                {
                    obj2.CustomSetActive(isShowUseButton);
                }
            }
        }

        public void SetMasterHeadImg(uint headImg)
        {
            MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId = headImg;
        }

        private void SortResDic(enHeadImgTab tab)
        {
            ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(tab);
            if (tab == enHeadImgTab.All)
            {
                curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByTab));
            }
            else if ((tab == enHeadImgTab.Nobe) || (tab == enHeadImgTab.Team))
            {
                curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByWeight));
            }
            else if ((tab == enHeadImgTab.Activity) || (tab == enHeadImgTab.Skin))
            {
                curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByTime));
            }
        }

        public void SortResDicAll()
        {
            this.GetCurHeadImgList(enHeadImgTab.All).Sort(new Comparison<ResHeadImage>(this.ComparisonByTab));
            this.GetCurHeadImgList(enHeadImgTab.Nobe).Sort(new Comparison<ResHeadImage>(this.ComparisonByWeight));
            this.GetCurHeadImgList(enHeadImgTab.Activity).Sort(new Comparison<ResHeadImage>(this.ComparisonByTime));
            this.GetCurHeadImgList(enHeadImgTab.Skin).Sort(new Comparison<ResHeadImage>(this.ComparisonByTime));
            this.GetCurHeadImgList(enHeadImgTab.Team).Sort(new Comparison<ResHeadImage>(this.ComparisonByWeight));
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Form_Open, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Form_Open));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Form_Close, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Form_Close));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Tab_Click, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Tab_Click));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Icon_Click, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Icon_Click));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Confirm, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Confirm));
            Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Item_Enable, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Item_Enable));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.NOBE_STATE_CHANGE, new System.Action(this.OnHeadInfoRefresh));
        }

        public uint UnReadFlagNum
        {
            get
            {
                uint num = 0;
                for (int i = 0; i < this.m_headImgInfo.Count; i++)
                {
                    if (this.m_headImgInfo[i].bNtfFlag == 1)
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        private enum enHeadImgTab
        {
            All,
            Nobe,
            Activity,
            Skin,
            Team
        }

        public class HeadImgInfo
        {
            public byte bNtfFlag;
            public uint dwGetTime;
            public uint dwID;
        }
    }
}

