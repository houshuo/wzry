namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class InBattleMsgMgr : Singleton<InBattleMsgMgr>
    {
        public ListView<ResInBatMsgCfg> flagConfigData = new ListLinqView<ResInBatMsgCfg>();
        private DictionaryView<uint, DictionaryView<uint, ResInBatMsgHeroActCfg>> heroActData = new DictionaryView<uint, DictionaryView<uint, ResInBatMsgHeroActCfg>>();
        public ListView<TabElement> inbatEntList = new ListView<TabElement>();
        public ListView<TabElement> lastMenuEntList = new ListView<TabElement>();
        public InBattleShortcutMenu m_customMenu = new InBattleShortcutMenu();
        private CUIFormScript m_formScript;
        public InBattleInputChat m_InputChat;
        public InBattleShortcut m_shortcutChat;
        public ListView<TabElement> menuEntList = new ListView<TabElement>();
        public DictionaryView<string, ListView<TabElement>> tabElements = new DictionaryView<string, ListView<TabElement>>();
        public List<string> title_list = new List<string>();
        public byte totalCount;

        public void BuildInBatEnt()
        {
            this.inbatEntList.Clear();
            for (int i = 0; i < this.menuEntList.Count; i++)
            {
                TabElement item = this.menuEntList[i];
                if ((item != null) && (item.cfgId != 0))
                {
                    this.inbatEntList.Add(item);
                }
            }
        }

        public void Clear()
        {
            this.UnRegInBattleEvent();
            this.inbatEntList.Clear();
            if (this.m_shortcutChat != null)
            {
                this.m_shortcutChat.Clear();
            }
            this.m_shortcutChat = null;
            if (this.m_InputChat != null)
            {
                this.m_InputChat.Clear();
            }
            this.m_InputChat = null;
            this.m_formScript = null;
        }

        public void ClearData()
        {
            this.inbatEntList.Clear();
            this.menuEntList.Clear();
            this.lastMenuEntList.Clear();
        }

        public void ClearMenuItem(int index)
        {
            if ((index >= 0) && (index < this.menuEntList.Count))
            {
                this.menuEntList[index] = null;
            }
        }

        public TabElement GeTabElement(int tabIndex, int list_index)
        {
            if ((tabIndex >= 0) && (tabIndex < Singleton<InBattleMsgMgr>.instance.title_list.Count))
            {
                ListView<TabElement> view = null;
                string key = Singleton<InBattleMsgMgr>.instance.title_list[tabIndex];
                Singleton<InBattleMsgMgr>.instance.tabElements.TryGetValue(key, out view);
                if ((view != null) && ((list_index >= 0) && (list_index < view.Count)))
                {
                    return view[list_index];
                }
            }
            return null;
        }

        public ResInBatMsgCfg GetCfgData(uint id)
        {
            return GameDataMgr.inBattleMsgDatabin.GetDataByKey(id);
        }

        private TabElement GetConfigMenuItem()
        {
            return null;
        }

        public ResInBatMsgHeroActCfg GetHeroActCfg(uint heroid, uint actID)
        {
            DictionaryView<uint, ResInBatMsgHeroActCfg> view = null;
            this.heroActData.TryGetValue(heroid, out view);
            if (view == null)
            {
                return null;
            }
            ResInBatMsgHeroActCfg cfg = null;
            view.TryGetValue(actID, out cfg);
            return cfg;
        }

        public void Handle_InBattleMsg_Ntf(COMDT_CHAT_MSG_INBATTLE obj)
        {
            if (obj != null)
            {
                ulong ullUid = obj.stFrom.ullUid;
                uint dwAcntHeroID = obj.stFrom.dwAcntHeroID;
                uint dwTextID = 0;
                if (obj.bChatType == 1)
                {
                    dwTextID = obj.stChatInfo.stSignalID.dwTextID;
                    if (this.m_shortcutChat != null)
                    {
                        this.m_shortcutChat.InnerHandle_InBat_PreConfigMsg((COM_INBATTLE_CHAT_TYPE) obj.bChatType, dwAcntHeroID, dwTextID, ullUid);
                    }
                }
                else if (obj.bChatType == 2)
                {
                    dwTextID = obj.stChatInfo.stBubbleID.dwTextID;
                    if (this.m_shortcutChat != null)
                    {
                        this.m_shortcutChat.InnerHandle_InBat_PreConfigMsg((COM_INBATTLE_CHAT_TYPE) obj.bChatType, dwAcntHeroID, dwTextID, ullUid);
                    }
                }
                else if (obj.bChatType == 3)
                {
                    string playerName = StringHelper.BytesToString(obj.stFrom.szName);
                    string content = StringHelper.BytesToString_FindFristZero(obj.stChatInfo.stContentStr.szContent);
                    byte bCampLimit = obj.stChatInfo.stContentStr.bCampLimit;
                    this.InnerHandle_InBat_InputChat(ullUid, playerName, content, bCampLimit);
                }
                else
                {
                    DebugHelper.Assert(false, string.Format("Handle_InBattleMsg_Ntf chatType:{0} beyond scope", obj.bChatType));
                }
            }
        }

        public void HideView()
        {
            if (this.m_shortcutChat != null)
            {
                this.m_shortcutChat.Show(false);
            }
        }

        public void InitView(GameObject cdButton, CUIFormScript formScript)
        {
            if (formScript != null)
            {
                this.m_formScript = formScript;
                if (this.IsEnableShortcut())
                {
                    this.m_shortcutChat = new InBattleShortcut();
                    this.m_shortcutChat.OpenForm(formScript, null, false);
                    this.m_InputChat = new InBattleInputChat();
                    this.m_InputChat.Init(formScript);
                }
                else if (cdButton != null)
                {
                    cdButton.CustomSetActive(false);
                }
            }
        }

        public void InnerHandle_InBat_InputChat(ulong ullUid, string playerName, string content, byte camp)
        {
            if (this.m_InputChat != null)
            {
                InBattleInputChat.InBatChatEntity ent = this.m_InputChat.ConstructEnt(ullUid, playerName, content, camp);
                this.m_InputChat.Add(ent);
            }
        }

        public bool IsAllChannel_CD_Valid()
        {
            return true;
        }

        public bool IsChannel_CD_Valid(int channel_id)
        {
            return true;
        }

        public bool IsEnableInputChat()
        {
            return ((Singleton<CChatController>.instance.model.bEnableInBattleInputChat && this.IsEnableShortcut()) && (GameSettings.InBattleInputChatEnable == 1));
        }

        public bool IsEnableShortcut()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if (curLvelContext == null)
            {
                return false;
            }
            return (curLvelContext.m_isWarmBattle || Singleton<LobbyLogic>.instance.inMultiGame);
        }

        public void On_InBatMenu_OpenForm(CUIEvent uievent)
        {
            if (this.m_customMenu != null)
            {
                this.m_customMenu.OpenForm();
            }
            Singleton<CMiShuSystem>.instance.SetNewFlagForMessageBtnEntry(false);
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(3, null);
            CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_BattleChatSetBtn);
        }

        public void On_InBattleMsg_OpenForm(CUIEvent uiEvent)
        {
            CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_Signal_Textmsg);
            if (this.m_shortcutChat != null)
            {
                this.m_shortcutChat.OpenForm(this.m_formScript, uiEvent, true);
            }
        }

        public void ParseCfgData()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_InBatMenu_OpenForm));
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("InBat_Bubble_CDTime"), out InBattleShortcut.InBat_Bubble_CDTime))
            {
                DebugHelper.Assert(false, "---InBatMsg 教练你配的 InBat_Bubble_CDTime 好像不是整数哦， check out");
            }
            ListView<TabElement> view = null;
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.inBattleMsgDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                view = null;
                KeyValuePair<long, object> current = enumerator.Current;
                ResInBatMsgCfg cfg = (ResInBatMsgCfg) current.Value;
                if (cfg != null)
                {
                    string szChannelTitle = cfg.szChannelTitle;
                    this.tabElements.TryGetValue(szChannelTitle, out view);
                    if (view == null)
                    {
                        view = new ListView<TabElement>();
                        this.tabElements.Add(szChannelTitle, view);
                        this.title_list.Add(szChannelTitle);
                    }
                    TabElement item = new TabElement(cfg.dwID, cfg.szContent) {
                        camp = cfg.bCampVisible
                    };
                    view.Add(item);
                }
            }
            Dictionary<long, object>.Enumerator enumerator2 = GameDataMgr.inBattleHeroActDatabin.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                KeyValuePair<long, object> pair2 = enumerator2.Current;
                ResInBatMsgHeroActCfg cfg2 = (ResInBatMsgHeroActCfg) pair2.Value;
                if (cfg2 != null)
                {
                    DictionaryView<uint, ResInBatMsgHeroActCfg> view2 = null;
                    this.heroActData.TryGetValue(cfg2.dwHeroID, out view2);
                    if (view2 == null)
                    {
                        view2 = new DictionaryView<uint, ResInBatMsgHeroActCfg>();
                        this.heroActData.Add(cfg2.dwHeroID, view2);
                    }
                    if (!view2.ContainsKey(cfg2.dwActionID))
                    {
                        view2.Add(cfg2.dwActionID, cfg2);
                    }
                }
            }
            GameDataMgr.inBattleHeroActDatabin.Unload();
            Dictionary<long, object>.Enumerator enumerator3 = GameDataMgr.inBattleDefaultDatabin.GetEnumerator();
            while (enumerator3.MoveNext())
            {
                KeyValuePair<long, object> pair3 = enumerator3.Current;
                ResShortcutDefault default2 = (ResShortcutDefault) pair3.Value;
                if (default2 != null)
                {
                    DebugHelper.Assert(GameDataMgr.inBattleMsgDatabin.GetDataByKey(default2.dwConfigID) != null, "---jason 检查下 局内交流配置表中的默认配置sheet, 配置id:" + default2.dwConfigID);
                }
            }
        }

        public void ParseServerData(COMDT_SELFDEFINE_CHATINFO chatInfo)
        {
            if (chatInfo != null)
            {
                bool flag = false;
                this.totalCount = chatInfo.bMsgCnt;
                for (int i = 0; i < chatInfo.bMsgCnt; i++)
                {
                    COMDT_SELFDEFINE_DETAIL_CHATINFO comdt_selfdefine_detail_chatinfo = chatInfo.astChatMsg[i];
                    if (comdt_selfdefine_detail_chatinfo.bChatType == 4)
                    {
                        flag = true;
                        break;
                    }
                    if ((comdt_selfdefine_detail_chatinfo.bChatType == 1) && (comdt_selfdefine_detail_chatinfo.stChatInfo.stSignalID.dwTextID > 0))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    this.UseDefaultShortcut();
                }
                else
                {
                    for (int j = 0; j < chatInfo.bMsgCnt; j++)
                    {
                        COMDT_SELFDEFINE_DETAIL_CHATINFO comdt_selfdefine_detail_chatinfo2 = chatInfo.astChatMsg[j];
                        if (comdt_selfdefine_detail_chatinfo2.bChatType == 1)
                        {
                            uint dwTextID = comdt_selfdefine_detail_chatinfo2.stChatInfo.stSignalID.dwTextID;
                            TabElement item = new TabElement(dwTextID, string.Empty) {
                                cfgId = dwTextID
                            };
                            if (dwTextID > 0)
                            {
                                ResInBatMsgCfg cfgData = this.GetCfgData(item.cfgId);
                                DebugHelper.Assert(cfgData != null, "custom shortcut ParseServerData cfgdata is null, cfgID:" + item.cfgId);
                                if (cfgData != null)
                                {
                                    item.configContent = cfgData.szContent;
                                }
                            }
                            this.menuEntList.Add(item);
                        }
                        else if ((comdt_selfdefine_detail_chatinfo2.bChatType == 1) && !string.IsNullOrEmpty(StringHelper.BytesToString(comdt_selfdefine_detail_chatinfo2.stChatInfo.stSelfDefineStr.szContent)))
                        {
                            TabElement element2 = new TabElement(string.Empty);
                            this.menuEntList.Add(element2);
                        }
                    }
                    this.SyncData(this.menuEntList, this.lastMenuEntList);
                    this.Print(this.menuEntList, "服务器解析出来的自定义数据");
                }
            }
        }

        public void Print(ListView<TabElement> list, string name)
        {
            object[] objArray1 = new object[] { "---shortCut list:", name, ",count:", list.Count, ", " };
            string message = string.Concat(objArray1);
            for (int i = 0; i < list.Count; i++)
            {
                TabElement element = list[i];
                if (element != null)
                {
                    string str2 = message;
                    object[] objArray2 = new object[] { str2, "  ,i:", i, ",id:", element.cfgId };
                    message = string.Concat(objArray2);
                }
            }
            Debug.Log(message);
        }

        public void RegInBattleEvent()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattleMsg_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_OpenForm));
        }

        public void ServerDisableInputChat()
        {
            if (this.m_InputChat != null)
            {
                this.m_InputChat.ServerDisableInputChat();
            }
        }

        public void SetMenuItem(int index, string selfDef)
        {
        }

        public void SetMenuItem(int index, uint configID)
        {
            if ((index >= 0) && (index < this.menuEntList.Count))
            {
                TabElement element = this.menuEntList[index];
                if (element != null)
                {
                    element.cfgId = configID;
                }
            }
        }

        public bool ShouldBeThroughNet(SLevelContext levelContent)
        {
            if (levelContent == null)
            {
                return false;
            }
            if (levelContent.m_isWarmBattle && levelContent.IsGameTypeComBat())
            {
                return false;
            }
            return true;
        }

        public void SyncData(ListView<TabElement> src, ListView<TabElement> dest)
        {
            if (dest != null)
            {
                dest.Clear();
            }
            for (int i = 0; i < src.Count; i++)
            {
                TabElement element = src[i];
                if (element != null)
                {
                    dest.Add(element.Clone());
                }
            }
        }

        public void UnRegInBattleEvent()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattleMsg_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_OpenForm));
        }

        public void Update()
        {
            if (this.m_shortcutChat != null)
            {
                this.m_shortcutChat.Update();
            }
            if (this.m_InputChat != null)
            {
                this.m_InputChat.Update();
            }
        }

        public void UseDefaultShortcut()
        {
            this.menuEntList.Clear();
            this.lastMenuEntList.Clear();
            for (int i = 0; i < this.totalCount; i++)
            {
                ResShortcutDefault dataByKey = GameDataMgr.inBattleDefaultDatabin.GetDataByKey((long) (i + 1));
                DebugHelper.Assert(dataByKey != null, "---shortcut id:" + (i + 1) + ", 找不到对应配置数据, jason 检查下");
                if (dataByKey != null)
                {
                    ResInBatMsgCfg cfg = GameDataMgr.inBattleMsgDatabin.GetDataByKey(dataByKey.dwConfigID);
                    DebugHelper.Assert(cfg != null, "---shortcut dwConfigID:" + dataByKey.dwConfigID + ", 找不到对应配置数据, jason 检查下");
                    if (cfg != null)
                    {
                        this.menuEntList.Add(new TabElement(cfg.dwID, cfg.szContent));
                        this.lastMenuEntList.Add(new TabElement(cfg.dwID, cfg.szContent));
                    }
                }
            }
        }

        public bool IsUseDefault { get; private set; }
    }
}

