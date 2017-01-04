using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CHeroSelectChatView
{
    private Tab _tab;
    private GameObject backgroundObject;
    private bool bIsInChatSend_CD;
    private GameObject bottomSendVoiceBtn;
    private GameObject bottomTextBtn;
    private GameObject bottomVoiceBtn;
    private GameObject bubbleObj0;
    private GameObject bubbleObj1;
    private GameObject bubbleObj2;
    private GameObject bubbleObj3;
    private GameObject bubbleObj4;
    private Text bubbleText0;
    private Text bubbleText1;
    private Text bubbleText2;
    private Text bubbleText3;
    private Text bubbleText4;
    private int chat_cd_timer = -1;
    private GameObject chatDetailNode;
    private GameObject chatEntryNode;
    private enChatType curChatType = enChatType.Text;
    private CUIFormScript formScript;
    private InputField inputField;
    private bool m_bOpenMic;
    private bool m_bOpenSpeak;
    private Transform m_OpenMicObj;
    private Transform m_OpenMicTipObj;
    private Text m_OpenMicTipText;
    private Transform m_OpenSpeakeObj;
    private Transform m_OpenSpeakerTipObj;
    private Text m_OpenSpeakerTipText;
    private int m_Vocetimer;
    private int m_VoiceMictime;
    private string microphone_path = (CUIUtility.s_Sprite_System_HeroSelect_Dir + "HeroSelect_btn_Microphone.prefab");
    private string no_microphone_path = (CUIUtility.s_Sprite_System_HeroSelect_Dir + "HeroSelect_btn_No_Microphone.prefab");
    private string no_voiceIcon_path = (CUIUtility.s_Sprite_System_HeroSelect_Dir + "HeroSelect_btn_No_voice.prefab");
    private CUIListScript show_listScript;
    private CUIListScript tablistScript;
    private GameObject textSendBtn;
    private GameObject voiceBtnNode;
    private string voiceIcon_path = (CUIUtility.s_Sprite_System_HeroSelect_Dir + "HeroSelect_btn_voice.prefab");

    private string _get_current_info(Tab type, int index)
    {
        string str = string.Empty;
        if (type == Tab.Template)
        {
            return Singleton<CChatController>.instance.model.Get_HeroSelect_ChatTemplate(index).GetTemplateString();
        }
        if (type == Tab.All)
        {
            ListView<CChatEntity> list = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Select_Hero).list;
            if ((index >= 0) && (index < list.Count))
            {
                str = string.Format("{0}：{1}", list[index].name, list[index].text);
            }
        }
        return str;
    }

    private void _refresh_list(CUIListScript listScript, ListView<CChatEntity> data_list)
    {
        if (listScript != null)
        {
            int count = data_list.Count;
            listScript.SetElementAmount(count);
            for (int i = 0; i < count; i++)
            {
                CUIListElementScript elemenet = listScript.GetElemenet(i);
                if ((elemenet != null) && listScript.IsElementInScrollArea(i))
                {
                    elemenet.transform.Find("Text").GetComponent<Text>().text = string.Format("{0}：{1}", data_list[i].name, data_list[i].text);
                }
            }
        }
    }

    private void _refresh_list(CUIListScript listScript, List<CChatModel.HeroChatTemplateInfo> data_list)
    {
        if (listScript != null)
        {
            int count = data_list.Count;
            listScript.SetElementAmount(count);
            for (int i = 0; i < count; i++)
            {
                CUIListElementScript elemenet = listScript.GetElemenet(i);
                if ((elemenet != null) && listScript.IsElementInScrollArea(i))
                {
                    elemenet.transform.Find("Text").GetComponent<Text>().text = data_list[i].GetTemplateString();
                }
            }
        }
    }

    public void Clear()
    {
        Singleton<CChatController>.instance.model.Clear_HeroSelected();
        this.bubbleText0 = this.bubbleText1 = this.bubbleText2 = this.bubbleText3 = (Text) (this.bubbleText4 = null);
        this.bubbleObj0 = this.bubbleObj1 = this.bubbleObj2 = this.bubbleObj3 = (GameObject) (this.bubbleObj4 = null);
        this.backgroundObject = null;
        this.tablistScript = null;
        this.formScript = null;
        this.show_listScript = null;
        Singleton<CTimerManager>.instance.RemoveTimer(this.chat_cd_timer);
        this.chat_cd_timer = -1;
        this.bottomTextBtn = null;
        this.bottomVoiceBtn = null;
        this.inputField = null;
        this.bottomSendVoiceBtn = null;
        this.textSendBtn = null;
        this.voiceBtnNode = null;
        this.chatDetailNode = null;
        this.chatEntryNode = null;
        this.bIsInChatSend_CD = false;
        this.m_OpenMicObj = null;
        this.m_OpenSpeakeObj = null;
        this.m_bOpenSpeak = false;
        this.m_bOpenMic = false;
        this.m_OpenSpeakerTipObj = null;
        this.m_OpenSpeakerTipText = null;
        this.m_OpenMicTipObj = null;
        this.m_OpenMicTipText = null;
        Singleton<CTimerManager>.instance.RemoveTimer(this.m_Vocetimer);
        Singleton<CTimerManager>.instance.RemoveTimer(this.m_VoiceMictime);
        Singleton<CUIManager>.GetInstance().CloseForm(CChatController.ChatSelectHeroPath_Normal);
        Singleton<CUIManager>.GetInstance().CloseForm(CChatController.ChatSelectHeroPath_BanPick);
    }

    public void CloseForm()
    {
        this.Clear();
    }

    private GameObject getBubbleObj(int index)
    {
        if (index == 0)
        {
            return this.bubbleObj0;
        }
        if (index == 1)
        {
            return this.bubbleObj1;
        }
        if (index == 2)
        {
            return this.bubbleObj2;
        }
        if (index == 3)
        {
            return this.bubbleObj3;
        }
        if (index == 4)
        {
            return this.bubbleObj4;
        }
        return null;
    }

    public void On_Bottom_Btn_Click()
    {
        if (((this.chatEntryNode != null) && (this.chatDetailNode != null)) && (this.backgroundObject != null))
        {
            if (this.chatEntryNode.activeInHierarchy)
            {
                this.chatEntryNode.CustomSetActive(false);
                this.chatDetailNode.CustomSetActive(true);
                this.backgroundObject.CustomSetActive(true);
            }
            else
            {
                this.chatEntryNode.CustomSetActive(true);
                this.chatDetailNode.CustomSetActive(false);
                this.backgroundObject.CustomSetActive(false);
            }
        }
    }

    public void On_Chat_HeorSelectChatData_Change()
    {
        if (this.formScript != null)
        {
            this.Refresh_List(this.CurTab);
            this.Refresh_Bubble();
            this.Refresh_BottomChat();
        }
    }

    private void On_Input_Timer_End(int timer)
    {
        this.bIsInChatSend_CD = false;
    }

    public void On_List_ElementEnable(CUIEvent uievent)
    {
        int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
        string str = this._get_current_info(this.CurTab, srcWidgetIndexInBelongedList);
        if ((uievent.m_srcWidget != null) && !string.IsNullOrEmpty(str))
        {
            uievent.m_srcWidget.transform.Find("Text").GetComponent<Text>().text = str;
        }
    }

    public void On_List_Item_Click(int index)
    {
        if (this._tab == Tab.Template)
        {
            if (!this.bIsInChatSend_CD)
            {
                if (Singleton<CChatController>.instance.model.IsTemplate_IndexValid(index))
                {
                    if (CFakePvPHelper.bInFakeSelect)
                    {
                        CFakePvPHelper.FakeSendChatTemplate(index);
                    }
                    else
                    {
                        CChatNetUT.Send_SelectHero_Chat((uint) index);
                    }
                    this.Start_Input_Timer();
                    this.On_Bottom_Btn_Click();
                }
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips("Chat_Common_Tips_9", true, 1.5f, null, new object[0]);
            }
        }
    }

    public void On_Send_Text()
    {
        if (!this.bIsInChatSend_CD)
        {
            if (this.inputField != null)
            {
                string text = this.inputField.text;
                if (!string.IsNullOrEmpty(text))
                {
                    if (CFakePvPHelper.bInFakeSelect)
                    {
                        CFakePvPHelper.FakeSendChat(text);
                    }
                    else
                    {
                        CChatNetUT.Send_SelectHero_Chat(text);
                    }
                    this.Start_Input_Timer();
                    this.inputField.text = string.Empty;
                }
            }
        }
        else
        {
            Singleton<CUIManager>.instance.OpenTips("Chat_Common_Tips_9", true, 1.5f, null, new object[0]);
        }
    }

    public void On_Tab_Change(int index)
    {
        this.CurTab = (Tab) index;
    }

    public void OnChatBubbleClose(GameObject bubble)
    {
        bubble.CustomSetActive(false);
    }

    public void OnChatHeroSelectOpenMic(bool bShowTips = true)
    {
        if (this.m_OpenMicTipObj != null)
        {
            if (!this.m_bOpenSpeak)
            {
                if (this.m_OpenMicTipText != null)
                {
                    this.m_OpenMicTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_FIrstOPenSpeak;
                }
                if (bShowTips)
                {
                    this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
                }
                Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
            }
            else if (!CFakePvPHelper.bInFakeSelect && !MonoSingleton<VoiceSys>.GetInstance().GlobalVoiceSetting)
            {
                if (this.m_OpenMicTipText != null)
                {
                    this.m_OpenMicTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Server_Not_Open_Tips;
                }
                if (bShowTips)
                {
                    this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
                }
                Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
            }
            else
            {
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
                MonoSingleton<VoiceSys>.GetInstance().UseMicOnUser = this.m_bOpenMic;
                if (bShowTips)
                {
                    this.m_OpenMicTipObj.gameObject.CustomSetActive(true);
                }
                Singleton<CTimerManager>.instance.ResumeTimer(this.m_VoiceMictime);
            }
        }
    }

    public void OnChatHeroSelectOpenSpeaker(bool bAutoOpen = false)
    {
        if (this.m_OpenSpeakerTipObj != null)
        {
            if (!CFakePvPHelper.bInFakeSelect && !MonoSingleton<VoiceSys>.GetInstance().GlobalVoiceSetting)
            {
                if (!bAutoOpen)
                {
                    if (this.m_OpenSpeakerTipText != null)
                    {
                        this.m_OpenSpeakerTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Server_Not_Open_Tips;
                    }
                    this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(true);
                    Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
                }
            }
            else
            {
                if (this.m_bOpenSpeak)
                {
                    if (this.m_OpenSpeakerTipText != null)
                    {
                        this.m_OpenSpeakerTipText.text = MonoSingleton<VoiceSys>.GetInstance().m_Voice_Battle_CloseSpeaker;
                    }
                    MonoSingleton<VoiceSys>.GetInstance().ClosenSpeakers();
                    MonoSingleton<VoiceSys>.GetInstance().CloseMic();
                    this.m_bOpenMic = false;
                    if (this.m_OpenSpeakeObj != null)
                    {
                        CUIUtility.SetImageSprite(this.m_OpenSpeakeObj.GetComponent<Image>(), this.no_voiceIcon_path, null, true, false, false);
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
                    if (this.m_OpenSpeakeObj != null)
                    {
                        CUIUtility.SetImageSprite(this.m_OpenSpeakeObj.GetComponent<Image>(), this.voiceIcon_path, null, true, false, false);
                    }
                }
                this.m_bOpenSpeak = !this.m_bOpenSpeak;
                if (this.m_bOpenSpeak)
                {
                    if (!GameSettings.EnableVoice)
                    {
                        GameSettings.EnableVoice = true;
                    }
                    this.OnChatHeroSelectOpenMic(false);
                }
                else if (GameSettings.EnableVoice)
                {
                    GameSettings.EnableVoice = false;
                }
                this.m_OpenSpeakerTipObj.gameObject.CustomSetActive(true);
                Singleton<CTimerManager>.instance.ResumeTimer(this.m_Vocetimer);
            }
        }
    }

    private void OnUpdateBottomButtons()
    {
        if ((this.inputField != null) && (this.inputField.gameObject != null))
        {
            this.inputField.gameObject.CustomSetActive(this.curChatType == enChatType.Text);
        }
        if (this.bottomSendVoiceBtn != null)
        {
            this.bottomSendVoiceBtn.CustomSetActive(this.curChatType == enChatType.Voice);
        }
        if (this.textSendBtn != null)
        {
            this.textSendBtn.CustomSetActive(this.curChatType == enChatType.Text);
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

    public void OpenForm()
    {
        this.chat_cd_timer = Singleton<CTimerManager>.instance.AddTimer(0xbb8, -1, new CTimer.OnTimeUpHandler(this.On_Input_Timer_End));
        Singleton<CTimerManager>.instance.PauseTimer(this.chat_cd_timer);
        string str = string.Empty;
        if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enNormal)
        {
            str = CChatController.ChatSelectHeroPath_Normal;
        }
        else if (Singleton<CHeroSelectBaseSystem>.instance.uiType == enUIType.enBanPick)
        {
            str = CChatController.ChatSelectHeroPath_BanPick;
        }
        if (!string.IsNullOrEmpty(str))
        {
            this.formScript = Singleton<CUIManager>.GetInstance().OpenForm(str, false, true);
            this.backgroundObject = this.formScript.transform.Find("Background").gameObject;
            this.show_listScript = this.formScript.transform.Find("chatTools/node/ListView/chatList").gameObject.GetComponent<CUIListScript>();
            this.tablistScript = this.formScript.transform.Find("chatTools/node/Tab/List").gameObject.GetComponent<CUIListScript>();
            this.tablistScript.SetElementAmount(2);
            CUIListElementScript elemenet = null;
            elemenet = this.tablistScript.GetElemenet(0);
            this.ShowTemplate(elemenet, true);
            elemenet = this.tablistScript.GetElemenet(1);
            this.ShowTemplate(elemenet, false);
            this.tablistScript.m_alwaysDispatchSelectedChangeEvent = true;
            this.tablistScript.SelectElement(0, true);
            this.tablistScript.m_alwaysDispatchSelectedChangeEvent = false;
            this.formScript.transform.Find("bubble_node").gameObject.CustomSetActive(true);
            this.bubbleObj0 = this.formScript.transform.Find("bubble_node/bubble0").gameObject;
            this.bubbleText0 = this.bubbleObj0.transform.Find("text_bubble0").GetComponent<Text>();
            this.bubbleObj1 = this.formScript.transform.Find("bubble_node/bubble1").gameObject;
            this.bubbleText1 = this.bubbleObj1.transform.Find("text_bubble0").GetComponent<Text>();
            this.bubbleObj2 = this.formScript.transform.Find("bubble_node/bubble2").gameObject;
            this.bubbleText2 = this.bubbleObj2.transform.Find("text_bubble0").GetComponent<Text>();
            this.bubbleObj3 = this.formScript.transform.Find("bubble_node/bubble3").gameObject;
            this.bubbleText3 = this.bubbleObj3.transform.Find("text_bubble0").GetComponent<Text>();
            this.bubbleObj4 = this.formScript.transform.Find("bubble_node/bubble4").gameObject;
            this.bubbleText4 = this.bubbleObj4.transform.Find("text_bubble0").GetComponent<Text>();
            this.bubbleObj0.CustomSetActive(false);
            this.bubbleObj1.CustomSetActive(false);
            this.bubbleObj2.CustomSetActive(false);
            this.bubbleObj3.CustomSetActive(false);
            this.bubbleObj4.CustomSetActive(false);
            this.bottomVoiceBtn = this.formScript.transform.Find("chatTools/ChatVoiceBtn").gameObject;
            this.inputField = this.formScript.transform.Find("chatTools/InputField").GetComponent<InputField>();
            this.bottomSendVoiceBtn = this.formScript.transform.Find("chatTools/voice_Btn").gameObject;
            this.textSendBtn = this.formScript.transform.Find("chatTools/TextSendBtn").gameObject;
            this.voiceBtnNode = this.formScript.transform.Find("VoiceBtn").gameObject;
            this.chatEntryNode = this.formScript.transform.Find("entry_node").gameObject;
            this.chatDetailNode = this.formScript.transform.Find("chatTools").gameObject;
            this.m_OpenSpeakeObj = this.formScript.transform.Find("VoiceBtn/Voice_OpenSpeaker");
            this.m_OpenMicObj = this.formScript.transform.Find("VoiceBtn/Voice_OpenMic");
            this.m_OpenSpeakerTipObj = this.formScript.transform.Find("VoiceBtn/Voice_OpenSpeaker/info");
            if ((this.m_OpenSpeakerTipObj != null) && (this.m_OpenSpeakerTipObj.transform.Find("Text") != null))
            {
                this.m_OpenSpeakerTipText = this.m_OpenSpeakerTipObj.transform.Find("Text").GetComponent<Text>();
            }
            this.m_OpenMicTipObj = this.formScript.transform.Find("VoiceBtn/Voice_OpenMic/info");
            if ((this.m_OpenMicTipObj != null) && (this.m_OpenMicTipObj.transform.Find("Text") != null))
            {
                this.m_OpenMicTipText = this.m_OpenMicTipObj.transform.Find("Text").GetComponent<Text>();
            }
            this.m_Vocetimer = Singleton<CTimerManager>.instance.AddTimer(0x7d0, -1, new CTimer.OnTimeUpHandler(this.OnVoiceTimeEnd));
            Singleton<CTimerManager>.instance.PauseTimer(this.m_Vocetimer);
            Singleton<CTimerManager>.instance.ResetTimer(this.m_Vocetimer);
            this.m_VoiceMictime = Singleton<CTimerManager>.instance.AddTimer(0x7d0, -1, new CTimer.OnTimeUpHandler(this.OnVoiceMicTimeEnd));
            Singleton<CTimerManager>.instance.PauseTimer(this.m_VoiceMictime);
            Singleton<CTimerManager>.instance.ResetTimer(this.m_VoiceMictime);
            MonoSingleton<VoiceSys>.GetInstance().UseMicOnUser = false;
            if (MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
            {
                this.OnChatHeroSelectOpenSpeaker(true);
            }
            this.Show_SelectChat_MidNode(false);
            this._tab = Tab.None;
            this.CurTab = Tab.Template;
            this.curChatType = enChatType.Text;
            this.OnUpdateBottomButtons();
            this.Refresh_BottomChat();
            MonoSingleton<VoiceSys>.GetInstance().ShowVoiceBtn_HeroSelect(this.formScript);
        }
    }

    private void Refresh_BottomChat()
    {
        if (this.chatEntryNode != null)
        {
            CChatEntity last = Singleton<CChatController>.instance.model.channelMgr.GetChannel(EChatChannel.Select_Hero).GetLast();
            Text componetInChild = Utility.GetComponetInChild<Text>(this.chatEntryNode, "Text");
            if (last != null)
            {
                componetInChild.text = string.Format("{0}：{1}", last.name, last.text);
            }
            else
            {
                componetInChild.text = string.Empty;
            }
        }
    }

    public void Refresh_Bubble()
    {
        CChatEntity ent = Singleton<CChatController>.instance.model.GetLastUnread_Selected();
        if (ent != null)
        {
            this.Show_Bubble(ent);
        }
    }

    public void Refresh_List(Tab type)
    {
        if (type == Tab.Template)
        {
            this._refresh_list(this.show_listScript, Singleton<CChatController>.instance.model.GetCurGroupTemplateInfo());
        }
        else if (type == Tab.All)
        {
            CChatChannel channel = Singleton<CChatController>.instance.model.channelMgr.GetChannel(EChatChannel.Select_Hero);
            this._refresh_list(this.show_listScript, channel.list);
        }
    }

    public void Set_Show_Bottom(bool bShow)
    {
        if ((this.inputField != null) && (this.inputField.gameObject != null))
        {
            this.inputField.gameObject.CustomSetActive(bShow && (this.curChatType == enChatType.Text));
        }
        if (this.bottomSendVoiceBtn != null)
        {
            this.bottomSendVoiceBtn.CustomSetActive(bShow && (this.curChatType == enChatType.Voice));
        }
        if (this.textSendBtn != null)
        {
            this.textSendBtn.CustomSetActive(bShow && (this.curChatType == enChatType.Text));
        }
    }

    private void setBubbleText(int index, string text)
    {
        if (index == 0)
        {
            if (this.bubbleText0 != null)
            {
                this.bubbleText0.text = text;
            }
        }
        else if (index == 1)
        {
            if (this.bubbleText1 != null)
            {
                this.bubbleText1.text = text;
            }
        }
        else if (index == 2)
        {
            if (this.bubbleText2 != null)
            {
                this.bubbleText2.text = text;
            }
        }
        else if (index == 3)
        {
            if (this.bubbleText3 != null)
            {
                this.bubbleText3.text = text;
            }
        }
        else if ((index == 4) && (this.bubbleText4 != null))
        {
            this.bubbleText4.text = text;
        }
    }

    public void SetEntryNodeVoiceBtnShowable(bool bShow)
    {
        if (this.voiceBtnNode != null)
        {
            this.voiceBtnNode.CustomSetActive(bShow);
        }
        if (this.chatEntryNode != null)
        {
            this.chatEntryNode.CustomSetActive(bShow);
        }
    }

    private bool Show_Bubble(CChatEntity ent)
    {
        if (ent != null)
        {
            if (Singleton<CHeroSelectBaseSystem>.instance.roomInfo == null)
            {
                return false;
            }
            MemberInfo masterMemberInfo = Singleton<CHeroSelectBaseSystem>.instance.roomInfo.GetMasterMemberInfo();
            if (masterMemberInfo == null)
            {
                return false;
            }
            int teamPlayerIndex = Singleton<CHeroSelectBaseSystem>.instance.GetTeamPlayerIndex(ent.ullUid, masterMemberInfo.camp);
            if ((teamPlayerIndex >= 0) && (teamPlayerIndex <= 4))
            {
                this.Show_Bubble(teamPlayerIndex, ent.text);
                return true;
            }
        }
        return false;
    }

    public void Show_Bubble(int index, string content)
    {
        GameObject obj2 = this.getBubbleObj(index);
        if (obj2 != null)
        {
            CUITimerScript component = obj2.GetComponent<CUITimerScript>();
            obj2.CustomSetActive(true);
            component.ReStartTimer();
            this.setBubbleText(index, content);
        }
    }

    public void Show_SelectChat_MidNode(bool bShow)
    {
        if (((this.chatEntryNode != null) && (this.chatDetailNode != null)) && (this.backgroundObject != null))
        {
            this.chatEntryNode.CustomSetActive(!bShow);
            this.chatDetailNode.CustomSetActive(bShow);
            this.backgroundObject.CustomSetActive(bShow);
        }
    }

    private void ShowTemplate(CUIListElementScript tab_element, bool bShowTemplate)
    {
        if (tab_element != null)
        {
            GameObject gameObject = tab_element.gameObject.transform.Find("img_template").gameObject;
            if (gameObject != null)
            {
                gameObject.CustomSetActive(bShowTemplate);
            }
            GameObject obj3 = tab_element.gameObject.transform.Find("img_history").gameObject;
            if (obj3 != null)
            {
                obj3.CustomSetActive(!bShowTemplate);
            }
        }
    }

    public void Start_Input_Timer()
    {
        this.bIsInChatSend_CD = true;
        if (this.chat_cd_timer != -1)
        {
            Singleton<CTimerManager>.instance.ResetTimer(this.chat_cd_timer);
            Singleton<CTimerManager>.instance.ResumeTimer(this.chat_cd_timer);
        }
    }

    public enChatType ChatType
    {
        set
        {
            this.curChatType = value;
            this.OnUpdateBottomButtons();
        }
    }

    public Tab CurTab
    {
        get
        {
            return this._tab;
        }
        set
        {
            if (this._tab != value)
            {
                this._tab = value;
                this.Refresh_List(this.CurTab);
            }
        }
    }

    public enum enChatType
    {
        Voice,
        Text
    }

    public enum Tab
    {
        Template,
        All,
        None
    }
}

