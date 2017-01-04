namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CLoudSpeakerSys : Singleton<CLoudSpeakerSys>
    {
        public COMDT_CHAT_MSG_HORN CurLoudSpeaker;
        public COMDT_CHAT_MSG_HORN CurSpekaer;
        public const int LOUDSPEAKER_ID = 0x273a;
        private ListView<COMDT_CHAT_MSG_HORN> loudSpeakerList = new ListView<COMDT_CHAT_MSG_HORN>();
        private static ResHornInfo loudSpeakerRes;
        private uint m_characterLimit;
        private uint m_itemID;
        private uint m_lastLoudSpeakerBeginSec;
        private uint m_lastSpeakerBeginSec;
        private int m_timerLoudSpeaker = -1;
        private int m_timerReq = -1;
        private int m_timerSpeaker = -1;
        public const int REQ_TIME_DELTA = 5;
        private static string s_characterLimitString = string.Empty;
        public static readonly string SPEAKER_FORM_PATH = "UGUI/Form/System/LoudSpeaker/Form_LoudSpeaker.prefab";
        public const int SPEAKER_ID = 0x2739;
        private ListView<COMDT_CHAT_MSG_HORN> speakerList = new ListView<COMDT_CHAT_MSG_HORN>();
        private static ResHornInfo speakerRes;

        public void AddSpeakerArray(CS_HORN_TYPE type, COMDT_CHAT_MSG_HORN[] astMsgInfo, uint len)
        {
            ListView<COMDT_CHAT_MSG_HORN> speakerList = this.GetSpeakerList(type);
            int lastSpeakerBeginSec = (int) this.GetLastSpeakerBeginSec(type);
            for (int i = 0; i < len; i++)
            {
                if (lastSpeakerBeginSec < astMsgInfo[i].dwBeginShowSec)
                {
                    speakerList.Add(astMsgInfo[i]);
                    Singleton<CChatController>.instance.model.Add_Palyer_Info(astMsgInfo[i].stFrom);
                    if (type == CS_HORN_TYPE.CS_HORNTYPE_SMALL)
                    {
                        bool flag = astMsgInfo[i].stFrom.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
                        if (Singleton<CChatController>.instance.view != null)
                        {
                            Singleton<CChatController>.instance.view.bRefreshNew = !Singleton<CChatController>.instance.view.IsCheckHistory() || flag;
                        }
                        CChatEntity chatEnt = CChatUT.Build_4_Speaker(astMsgInfo[i]);
                        Singleton<CChatController>.instance.model.channelMgr.Add_ChatEntity(chatEnt, EChatChannel.Lobby, 0L, 0);
                        this.m_lastSpeakerBeginSec = Math.Max(astMsgInfo[i].dwBeginShowSec, this.m_lastSpeakerBeginSec);
                    }
                    else
                    {
                        this.m_lastLoudSpeakerBeginSec = Math.Max(astMsgInfo[i].dwBeginShowSec, this.m_lastLoudSpeakerBeginSec);
                    }
                }
            }
            if (len > 0)
            {
                COMDT_CHAT_MSG_HORN comdt_chat_msg_horn = astMsgInfo[0];
                if (type == CS_HORN_TYPE.CS_HORNTYPE_SMALL)
                {
                    this.OnSpeakerNodeOpen();
                }
                else if (type == CS_HORN_TYPE.CS_HORNTYPE_BIGER)
                {
                    this.OnLoudSpeakerTipsOpen();
                }
            }
        }

        public void Clear()
        {
            if (this.m_timerReq != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerReq);
            }
            this.m_timerReq = -1;
            if (this.m_timerSpeaker != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerSpeaker);
            }
            this.m_timerSpeaker = -1;
            if (this.m_timerLoudSpeaker != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerLoudSpeaker);
            }
            this.m_timerLoudSpeaker = -1;
            this.m_lastSpeakerBeginSec = 0;
            this.m_lastLoudSpeakerBeginSec = 0;
            this.m_itemID = 0;
            this.speakerList.Clear();
            this.loudSpeakerList.Clear();
            this.CurLoudSpeaker = null;
            this.CurSpekaer = null;
        }

        private string GetInputText()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(SPEAKER_FORM_PATH);
            if ((form != null) && (form.gameObject != null))
            {
                return Utility.GetComponetInChild<InputField>(form.gameObject, "pnlBg/Panel_Main/InputField").text;
            }
            return string.Empty;
        }

        private uint GetLastSpeakerBeginSec(CS_HORN_TYPE type)
        {
            if (type == CS_HORN_TYPE.CS_HORNTYPE_SMALL)
            {
                return this.m_lastSpeakerBeginSec;
            }
            return this.m_lastLoudSpeakerBeginSec;
        }

        public static ResHornInfo GetRes(int speakerID)
        {
            if (speakerID == 0x2739)
            {
                if (speakerRes == null)
                {
                    speakerRes = GameDataMgr.speakerDatabin.GetDataByKey((uint) 0x2739);
                }
                return speakerRes;
            }
            if (speakerID != 0x273a)
            {
                return null;
            }
            if (loudSpeakerRes == null)
            {
                loudSpeakerRes = GameDataMgr.speakerDatabin.GetDataByKey((uint) 0x273a);
            }
            return loudSpeakerRes;
        }

        private uint GetSpeakerEndTime(CS_HORN_TYPE type)
        {
            if (type == CS_HORN_TYPE.CS_HORNTYPE_SMALL)
            {
                if (this.speakerList.Count > 0)
                {
                    return this.speakerList[0].dwBeginShowSec;
                }
                if (this.CurSpekaer != null)
                {
                    return this.CurSpekaer.dwEndShowSec;
                }
                return 0;
            }
            if (this.loudSpeakerList.Count > 0)
            {
                return this.loudSpeakerList[0].dwBeginShowSec;
            }
            if (this.CurLoudSpeaker != null)
            {
                return this.CurLoudSpeaker.dwEndShowSec;
            }
            return 0;
        }

        private ListView<COMDT_CHAT_MSG_HORN> GetSpeakerList(CS_HORN_TYPE type)
        {
            if (type == CS_HORN_TYPE.CS_HORNTYPE_SMALL)
            {
                return this.speakerList;
            }
            return this.loudSpeakerList;
        }

        private void GetSpeakerMsg(CS_HORN_TYPE type, uint beginSec)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x519);
            msg.stPkgData.stGetHornMsgReq.bHornType = (byte) type;
            msg.stPkgData.stGetHornMsgReq.dwBeginShowSec = beginSec;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnSpeakerFormOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_Form_Clsoe, new CUIEventManager.OnUIEventHandler(this.OnSpeakerFormClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_Send, new CUIEventManager.OnUIEventHandler(this.OnSpeakerSend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_Form_Update, new CUIEventManager.OnUIEventHandler(this.OnUpdateCharacterLimitText));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Speaker_OpenFactoryShop, new CUIEventManager.OnUIEventHandler(this.OnOpenFactoryShop));
            s_characterLimitString = Singleton<CTextManager>.instance.GetText("Speaker_CharacterLimit");
        }

        public bool IsLoudSpeakerShowing()
        {
            return (((this.m_timerLoudSpeaker != -1) && (this.CurLoudSpeaker != null)) && (CRoleInfo.GetCurrentUTCTime() < this.CurLoudSpeaker.dwEndShowSec));
        }

        public bool IsSpeakerShowing()
        {
            return (((this.m_timerSpeaker != -1) && (this.CurSpekaer != null)) && (CRoleInfo.GetCurrentUTCTime() < this.CurSpekaer.dwEndShowSec));
        }

        [MessageHandler(0x51a)]
        public static void OnGetSpeakerMsgRsp(CSPkg msg)
        {
            CS_HORN_TYPE bHornType = (CS_HORN_TYPE) msg.stPkgData.stGetHornMsgRsp.bHornType;
            Singleton<CLoudSpeakerSys>.instance.AddSpeakerArray(bHornType, msg.stPkgData.stGetHornMsgRsp.astMsgInfo, msg.stPkgData.stGetHornMsgRsp.wMsgCnt);
            if ((bHornType == CS_HORN_TYPE.CS_HORNTYPE_SMALL) && (msg.stPkgData.stGetHornMsgRsp.wMsgCnt > 0))
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_LobbyChatData_Change");
            }
        }

        private void OnLoudSpeakerTipsOpen()
        {
            if (!this.IsLoudSpeakerShowing())
            {
                this.CurLoudSpeaker = this.PopSpeakerList(CS_HORN_TYPE.CS_HORNTYPE_BIGER);
                if (this.CurLoudSpeaker != null)
                {
                    if (this.m_timerLoudSpeaker != -1)
                    {
                        Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerLoudSpeaker);
                        this.m_timerLoudSpeaker = -1;
                    }
                    this.ShowLoudSpeaker(this.CurLoudSpeaker);
                }
            }
        }

        private void OnOpenFactoryShop(CUIEvent uiEvent)
        {
            uiEvent.m_eventID = enUIEventID.Chat_CloseForm;
            Singleton<CUIEventManager>.instance.DispatchUIEvent(uiEvent);
            uiEvent.m_eventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
            Singleton<CUIEventManager>.instance.DispatchUIEvent(uiEvent);
        }

        private void OnSpeakerFormClose(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.instance.CloseForm(SPEAKER_FORM_PATH);
        }

        private void OnSpeakerFormOpen(CUIEvent uiEvent)
        {
            uint itemID = uiEvent.m_eventParams.commonUInt32Param1;
            this.OpenSpeakerForm(itemID);
        }

        private void OnSpeakerNodeOpen()
        {
            if (!this.IsSpeakerShowing())
            {
                this.CurSpekaer = this.PopSpeakerList(CS_HORN_TYPE.CS_HORNTYPE_SMALL);
                if (this.CurSpekaer != null)
                {
                    if (this.m_timerSpeaker != -1)
                    {
                        Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerSpeaker);
                        this.m_timerSpeaker = -1;
                    }
                    this.ShowSpeaker(this.CurSpekaer);
                }
            }
        }

        private void OnSpeakerSend(CUIEvent uiEvent)
        {
            string inputText = this.GetInputText();
            if (string.IsNullOrEmpty(inputText))
            {
                Singleton<CUIManager>.instance.OpenTips("Chat_Common_Tips_10", true, 1.5f, null, new object[0]);
            }
            else
            {
                CUseable useableByBaseID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, this.m_itemID);
                if (useableByBaseID != null)
                {
                    this.OnSpeakerSend(inputText, useableByBaseID.m_objID);
                }
            }
        }

        private void OnSpeakerSend(string content, ulong uniqueID)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x463);
            msg.stPkgData.stHornUseReq.ullUniqueID = uniqueID;
            byte[] sourceArray = Utility.BytesConvert(content);
            byte[] szContent = msg.stPkgData.stHornUseReq.szContent;
            Array.Copy(sourceArray, szContent, Math.Min(sourceArray.Length, szContent.Length));
            szContent[szContent.Length - 1] = 0;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        [MessageHandler(0x464)]
        public static void OnSpeakerSendRsp(CSPkg msg)
        {
            if (msg.stPkgData.stHornUseRsp.iResult == 0)
            {
                Singleton<CUIManager>.instance.CloseForm(SPEAKER_FORM_PATH);
            }
            else if (msg.stPkgData.stHornUseRsp.iResult == 2)
            {
                Singleton<CUIManager>.instance.OpenTips("Speaker_Use_Err_2", true, 1.5f, null, new object[0]);
            }
            else if (msg.stPkgData.stHornUseRsp.iResult == 4)
            {
                Singleton<CUIManager>.instance.OpenTips("Speaker_Use_Err_4", true, 1.5f, null, new object[0]);
            }
            else
            {
                object[] replaceArr = new object[] { msg.stPkgData.stHornUseRsp.iResult };
                Singleton<CUIManager>.instance.OpenTips("Speaker_Use_Err_1", true, 1f, null, replaceArr);
            }
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
        }

        private void OnTimerLoudSpeaker(int timerSequence)
        {
            if (CRoleInfo.GetCurrentUTCTime() >= this.GetSpeakerEndTime(CS_HORN_TYPE.CS_HORNTYPE_BIGER))
            {
                Singleton<CTimerManager>.instance.RemoveTimer(timerSequence);
                this.m_timerLoudSpeaker = -1;
                this.CurLoudSpeaker = null;
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
                if (form != null)
                {
                    CUIAutoScroller component = form.GetWidget(5).GetComponent<CUIAutoScroller>();
                    if (component != null)
                    {
                        GameObject widget = form.GetWidget(6);
                        if (widget != null)
                        {
                            component.StopAutoScroll();
                            if (Singleton<CChatController>.instance.view != null)
                            {
                                Singleton<CChatController>.instance.view.ShowLoudSpeaker(false, null);
                            }
                            this.CurLoudSpeaker = this.PopSpeakerList(CS_HORN_TYPE.CS_HORNTYPE_BIGER);
                            if (this.CurLoudSpeaker == null)
                            {
                                component.gameObject.CustomSetActive(false);
                                widget.CustomSetActive(false);
                                this.GetSpeakerMsg(CS_HORN_TYPE.CS_HORNTYPE_BIGER, this.m_lastLoudSpeakerBeginSec);
                            }
                            else
                            {
                                component.gameObject.CustomSetActive(true);
                                widget.CustomSetActive(true);
                                this.ShowLoudSpeaker(this.CurLoudSpeaker);
                            }
                        }
                    }
                }
            }
        }

        private void OnTimerReq(int timerSequence)
        {
            if (!Singleton<BattleLogic>.instance.isRuning)
            {
                if (this.speakerList.Count <= 2)
                {
                    this.GetSpeakerMsg(CS_HORN_TYPE.CS_HORNTYPE_SMALL, this.m_lastSpeakerBeginSec);
                }
                if (this.loudSpeakerList.Count <= 2)
                {
                    this.GetSpeakerMsg(CS_HORN_TYPE.CS_HORNTYPE_BIGER, this.m_lastLoudSpeakerBeginSec);
                }
            }
        }

        private void OnTimerSpeaker(int timerSequence)
        {
            if (CRoleInfo.GetCurrentUTCTime() >= this.GetSpeakerEndTime(CS_HORN_TYPE.CS_HORNTYPE_SMALL))
            {
                Singleton<CTimerManager>.instance.RemoveTimer(timerSequence);
                this.m_timerSpeaker = -1;
                this.CurSpekaer = null;
                this.CurSpekaer = this.PopSpeakerList(CS_HORN_TYPE.CS_HORNTYPE_SMALL);
                if (this.CurSpekaer != null)
                {
                    this.ShowSpeaker(this.CurSpekaer);
                }
                else
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Speaker_EntryNode_TimeUp);
                    this.GetSpeakerMsg(CS_HORN_TYPE.CS_HORNTYPE_SMALL, this.m_lastSpeakerBeginSec);
                }
            }
        }

        private void OnUpdateCharacterLimitText(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(SPEAKER_FORM_PATH);
            if ((form != null) && (form.gameObject != null))
            {
                Text componetInChild = Utility.GetComponetInChild<Text>(form.gameObject, "pnlBg/Panel_Main/CharLimitTxt");
                if (componetInChild != null)
                {
                    InputField field = Utility.GetComponetInChild<InputField>(form.gameObject, "pnlBg/Panel_Main/InputField");
                    if (field != null)
                    {
                        int num = ((int) this.m_characterLimit) - field.text.Length;
                        if (num < 0)
                        {
                            num = 0;
                        }
                        componetInChild.text = string.Format(s_characterLimitString, num);
                    }
                }
            }
        }

        public void OpenSpeakerForm(uint itemID)
        {
            if ((itemID == 0x2739) || (itemID == 0x273a))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                    if (useableContainer != null)
                    {
                        if (useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, itemID) == 0)
                        {
                            CMallFactoryShopController.ShopProduct shopProduct = null;
                            if (itemID == 0x2739)
                            {
                                shopProduct = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xd4).dwConfValue);
                            }
                            else
                            {
                                shopProduct = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xd3).dwConfValue);
                            }
                            if (shopProduct != null)
                            {
                                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                                uIEvent.m_eventID = enUIEventID.Mall_Buy_Product_Confirm;
                                uIEvent.m_eventParams.commonUInt64Param1 = shopProduct.Key;
                                uIEvent.m_eventParams.commonUInt32Param1 = 1;
                                Singleton<CMallFactoryShopController>.GetInstance().BuyShopProduct(shopProduct, 1, true, uIEvent);
                            }
                        }
                        else
                        {
                            ResHornInfo dataByKey = GameDataMgr.speakerDatabin.GetDataByKey(itemID);
                            this.m_itemID = itemID;
                            this.m_characterLimit = dataByKey.dwWordLimit;
                            if (dataByKey != null)
                            {
                                CUIFormScript script = Singleton<CUIManager>.instance.OpenForm(SPEAKER_FORM_PATH, false, false);
                                if ((script != null) && (script.gameObject != null))
                                {
                                    GameObject obj2 = Utility.FindChild(script.gameObject, "pnlBg/Title/speakerText");
                                    GameObject obj3 = Utility.FindChild(script.gameObject, "pnlBg/Title/loudSpeakerText");
                                    GameObject obj4 = Utility.FindChild(script.gameObject, "pnlBg/Model/speaker");
                                    GameObject obj5 = Utility.FindChild(script.gameObject, "pnlBg/Model/loudspeaker");
                                    InputField componetInChild = Utility.GetComponetInChild<InputField>(script.gameObject, "pnlBg/Panel_Main/InputField");
                                    Utility.GetComponetInChild<CUITimerScript>(script.gameObject, "Timer").ReStartTimer();
                                    if (itemID == 0x2739)
                                    {
                                        obj2.CustomSetActive(true);
                                        obj3.CustomSetActive(false);
                                        obj4.CustomSetActive(true);
                                        obj5.CustomSetActive(false);
                                        componetInChild.characterLimit = (int) this.m_characterLimit;
                                    }
                                    else
                                    {
                                        obj2.CustomSetActive(false);
                                        obj3.CustomSetActive(true);
                                        obj4.CustomSetActive(false);
                                        obj5.CustomSetActive(true);
                                        componetInChild.characterLimit = (int) this.m_characterLimit;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private COMDT_CHAT_MSG_HORN PopSpeakerList(CS_HORN_TYPE type)
        {
            COMDT_CHAT_MSG_HORN comdt_chat_msg_horn = null;
            ListView<COMDT_CHAT_MSG_HORN> speakerList = this.GetSpeakerList(type);
            if (speakerList.Count > 0)
            {
                comdt_chat_msg_horn = speakerList[0];
                speakerList.RemoveAt(0);
            }
            return comdt_chat_msg_horn;
        }

        public void ShowLoudSpeaker(COMDT_CHAT_MSG_HORN data)
        {
            this.m_timerLoudSpeaker = Singleton<CTimerManager>.instance.AddTimer(0x3e8, 0, new CTimer.OnTimeUpHandler(this.OnTimerLoudSpeaker));
            if (Singleton<BattleLogic>.instance.isRuning)
            {
                this.loudSpeakerList.Clear();
            }
            else
            {
                CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
                if (form != null)
                {
                    CUIAutoScroller component = form.GetWidget(5).GetComponent<CUIAutoScroller>();
                    if (component != null)
                    {
                        GameObject widget = form.GetWidget(6);
                        if (widget != null)
                        {
                            string rawText = UT.Bytes2String(data.szContent);
                            string str = CChatUT.Build_4_LoudSpeaker_EntryString(data.stFrom.ullUid, (uint) data.stFrom.iLogicWorldID, rawText);
                            component.SetText(CUIUtility.RemoveEmoji(str));
                            component.gameObject.CustomSetActive(true);
                            widget.CustomSetActive(true);
                            component.StopAutoScroll();
                            component.StartAutoScroll(true);
                            Singleton<CChatController>.instance.view.ShowLoudSpeaker(true, data);
                        }
                    }
                }
            }
        }

        public void ShowSpeaker(COMDT_CHAT_MSG_HORN data)
        {
            string a = CChatUT.Build_4_Speaker_EntryString(data.stFrom.ullUid, (uint) data.stFrom.iLogicWorldID, UT.Bytes2String(data.szContent));
            this.m_timerSpeaker = Singleton<CTimerManager>.instance.AddTimer(0x3e8, 0, new CTimer.OnTimeUpHandler(this.OnTimerSpeaker));
            if (Singleton<CChatController>.instance.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Normal)
            {
                Singleton<CChatController>.instance.model.sysData.Add_NewContent_Entry_Speaker(a);
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
            }
        }

        public void StartReqTimer()
        {
            if (this.m_timerReq == -1)
            {
                this.m_timerReq = Singleton<CTimerManager>.instance.AddTimer(0x1388, 0, new CTimer.OnTimeUpHandler(this.OnTimerReq));
                this.OnTimerReq(0);
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Speaker_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnSpeakerFormOpen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Speaker_Form_Clsoe, new CUIEventManager.OnUIEventHandler(this.OnSpeakerFormClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Speaker_Send, new CUIEventManager.OnUIEventHandler(this.OnSpeakerSend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Speaker_Form_Update, new CUIEventManager.OnUIEventHandler(this.OnUpdateCharacterLimitText));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Speaker_OpenFactoryShop, new CUIEventManager.OnUIEventHandler(this.OnOpenFactoryShop));
            base.UnInit();
        }
    }
}

