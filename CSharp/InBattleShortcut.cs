using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class InBattleShortcut
{
    private CUIListScript contentList;
    public static int InBat_Bubble_CDTime = 0xbb8;
    public static readonly string InBattleMsgView_FORM_PATH = "UGUI/Form/System/Chat/Form_InBattleChat.prefab";
    private CUIFormScript m_battleForm;
    public CDButton m_cdButton;
    private CUIFormScript m_CUIForm;
    private DictionaryView<ulong, BubbleTimerEntity> player_bubbleTime_map = new DictionaryView<ulong, BubbleTimerEntity>();

    private void _refresh_list(CUIListScript listScript, ListView<TabElement> data_list)
    {
        if (((listScript != null) && (data_list != null)) && (data_list.Count != 0))
        {
            int count = data_list.Count;
            listScript.SetElementAmount(count);
        }
    }

    public void Clear()
    {
        Singleton<CSoundManager>.GetInstance().UnLoadBank("System_Call", CSoundManager.BankType.Battle);
        this.UnRegInBattleEvent();
        Singleton<InBattleMsgMgr>.instance.inbatEntList.Clear();
        DictionaryView<ulong, BubbleTimerEntity>.Enumerator enumerator = this.player_bubbleTime_map.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<ulong, BubbleTimerEntity> current = enumerator.Current;
            BubbleTimerEntity entity = current.Value;
            if (entity != null)
            {
                entity.Clear();
            }
        }
        this.player_bubbleTime_map.Clear();
        this.contentList = null;
        if (this.m_cdButton != null)
        {
            this.m_cdButton.Clear();
            this.m_cdButton = null;
        }
        this.m_CUIForm = null;
        this.m_battleForm = null;
        Singleton<CUIManager>.GetInstance().CloseForm(InBattleMsgView_FORM_PATH);
    }

    public void InnerHandle_InBat_PreConfigMsg(COM_INBATTLE_CHAT_TYPE chatType, uint herocfgID, uint cfg_id, ulong ullUid)
    {
        ResInBatMsgHeroActCfg heroActCfg = Singleton<InBattleMsgMgr>.instance.GetHeroActCfg(herocfgID, cfg_id);
        ResInBatMsgCfg cfgData = Singleton<InBattleMsgMgr>.instance.GetCfgData(cfg_id);
        if (cfgData != null)
        {
            if (heroActCfg != null)
            {
                InBattleMsgUT.ShowInBattleMsg(chatType, ullUid, herocfgID, heroActCfg.szContent, heroActCfg.szSound, true);
            }
            else
            {
                InBattleMsgUT.ShowInBattleMsg(chatType, ullUid, herocfgID, cfgData.szContent, cfgData.szSound, false);
            }
            if ((chatType == COM_INBATTLE_CHAT_TYPE.COM_INBATTLE_CHATTYPE_SIGNAL) && (Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini))
            {
                ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = Singleton<GamePlayerCenter>.instance.GetPlayerByUid(ullUid).GetAllHeroes();
                for (int i = 0; i < allHeroes.Count; i++)
                {
                    PoolObjHandle<ActorRoot> handle = allHeroes[i];
                    ActorRoot root = handle.handle;
                    if ((root != null) && (root.TheActorMeta.ConfigId == herocfgID))
                    {
                        Vector2 sreenLoc = CUIUtility.WorldToScreenPoint(this.m_battleForm.GetCamera(), root.HudControl.GetSmallMapPointer_WorldPosition());
                        Singleton<CUIParticleSystem>.instance.AddParticle(cfgData.szMiniMapEffect, 2f, sreenLoc);
                        return;
                    }
                }
            }
        }
    }

    public void On_InBattleMsg_CloseForm(CUIEvent uiEvent)
    {
        this.Show(false);
    }

    public void On_InBattleMsg_ListElement_Click(CUIEvent uiEvent)
    {
        this.Show(false);
        int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
        if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < Singleton<InBattleMsgMgr>.instance.inbatEntList.Count))
        {
            TabElement element = Singleton<InBattleMsgMgr>.instance.inbatEntList[srcWidgetIndexInBelongedList];
            if (element != null)
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (hostPlayer != null)
                {
                    ResInBatMsgCfg cfgData = Singleton<InBattleMsgMgr>.instance.GetCfgData(element.cfgId);
                    DebugHelper.Assert(cfgData != null, "InbattleMsgView cfg_data == null");
                    if (cfgData != null)
                    {
                        SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                        if (curLvelContext != null)
                        {
                            if (!Singleton<InBattleMsgMgr>.instance.ShouldBeThroughNet(curLvelContext))
                            {
                                if ((element.cfgId >= 1) && (element.cfgId <= 15))
                                {
                                    CPlayerBehaviorStat.Plus((CPlayerBehaviorStat.BehaviorType) element.cfgId);
                                }
                                this.InnerHandle_InBat_PreConfigMsg((COM_INBATTLE_CHAT_TYPE) cfgData.bShowType, hostPlayer.CaptainId, element.cfgId, hostPlayer.PlayerUId);
                            }
                            else
                            {
                                if ((element.cfgId >= 1) && (element.cfgId <= 15))
                                {
                                    CPlayerBehaviorStat.Plus((CPlayerBehaviorStat.BehaviorType) element.cfgId);
                                }
                                InBattleMsgNetCore.SendInBattleMsg_PreConfig(element.cfgId, (COM_INBATTLE_CHAT_TYPE) cfgData.bShowType, hostPlayer.CaptainId);
                            }
                            if (this.m_cdButton != null)
                            {
                                ResInBatChannelCfg dataByKey = GameDataMgr.inBattleChannelDatabin.GetDataByKey((uint) cfgData.bInBatChannelID);
                                if (dataByKey != null)
                                {
                                    this.m_cdButton.StartCooldown(dataByKey.dwCdTime, null);
                                }
                                else
                                {
                                    this.m_cdButton.StartCooldown(0xfa0, null);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void On_InBattleMsg_ListElement_Enable(CUIEvent uievent)
    {
        int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
        if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < Singleton<InBattleMsgMgr>.instance.inbatEntList.Count))
        {
            TabElement element = Singleton<InBattleMsgMgr>.instance.inbatEntList[srcWidgetIndexInBelongedList];
            if (element != null)
            {
                InBattleMsgShower component = uievent.m_srcWidget.GetComponent<InBattleMsgShower>();
                if ((component != null) && (element != null))
                {
                    component.Set(element.cfgId, element.configContent);
                }
            }
        }
    }

    public void On_InBattleMsg_TabChange(int index)
    {
    }

    public void OpenForm(CUIFormScript battleForm, CUIEvent uiEvent, bool bShow = true)
    {
        if (this.m_CUIForm != null)
        {
            this.m_CUIForm.gameObject.CustomSetActive(true);
            if (this.contentList != null)
            {
                this.contentList.SelectElement(-1, false);
            }
            GameObject obj2 = Utility.FindChild(this.m_CUIForm.gameObject, "chatTools/node/InputChat_Buttons");
            if (obj2 != null)
            {
                obj2.CustomSetActive(GameSettings.InBattleInputChatEnable == 1);
            }
        }
        else
        {
            this.m_battleForm = battleForm;
            this.m_CUIForm = Singleton<CUIManager>.GetInstance().OpenForm(InBattleMsgView_FORM_PATH, true, true);
            DebugHelper.Assert(this.m_CUIForm != null, "InbattleMsgView m_CUIForm == null");
            if (this.m_CUIForm != null)
            {
                Singleton<CSoundManager>.GetInstance().LoadBank("System_Call", CSoundManager.BankType.Battle);
                this.RegInBattleEvent();
                Singleton<InBattleMsgMgr>.instance.BuildInBatEnt();
                this.contentList = this.m_CUIForm.transform.Find("chatTools/node/ListView/List").GetComponent<CUIListScript>();
                DebugHelper.Assert(this.contentList != null, "InbattleMsgView contentList == null");
                if (this.contentList != null)
                {
                    GameObject gameObject = battleForm.transform.Find("panelTopRight/SignalPanel/Button_Chat").gameObject;
                    DebugHelper.Assert(gameObject != null, "InbattleMsgView btnObj == null");
                    if (gameObject != null)
                    {
                        gameObject.CustomSetActive(true);
                    }
                    this.m_cdButton = new CDButton(gameObject);
                    GameObject obj4 = Utility.FindChild(this.m_CUIForm.gameObject, "chatTools/node/InputChat_Buttons");
                    if (obj4 != null)
                    {
                        obj4.CustomSetActive(GameSettings.InBattleInputChatEnable == 1);
                    }
                    if (!bShow)
                    {
                        this.m_CUIForm.gameObject.CustomSetActive(false);
                    }
                    this.Refresh_List();
                }
            }
        }
    }

    public void Refresh_List()
    {
        ListView<TabElement> inbatEntList = Singleton<InBattleMsgMgr>.instance.inbatEntList;
        if (inbatEntList != null)
        {
            this._refresh_list(this.contentList, inbatEntList);
        }
    }

    public void RegInBattleEvent()
    {
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattleMsg_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_CloseForm));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattleMsg_ListElement_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_ListElement_Enable));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattleMsg_ListElement_Click, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_ListElement_Click));
    }

    public void Show(bool bShow)
    {
        if (this.m_CUIForm != null)
        {
            this.m_CUIForm.gameObject.CustomSetActive(bShow);
        }
    }

    public void UnRegInBattleEvent()
    {
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattleMsg_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_CloseForm));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattleMsg_ListElement_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_ListElement_Enable));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattleMsg_ListElement_Click, new CUIEventManager.OnUIEventHandler(this.On_InBattleMsg_ListElement_Click));
    }

    public void Update()
    {
        if (this.m_cdButton != null)
        {
            this.m_cdButton.Update();
        }
    }

    public void UpdatePlayerBubbleTimer(ulong playerid, uint heroid)
    {
        BubbleTimerEntity entity = null;
        this.player_bubbleTime_map.TryGetValue(playerid, out entity);
        if (entity == null)
        {
            entity = new BubbleTimerEntity(playerid, heroid, InBat_Bubble_CDTime);
            this.player_bubbleTime_map.Add(playerid, entity);
        }
        entity.Start();
    }
}

