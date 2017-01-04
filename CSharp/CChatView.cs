using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CChatView
{
    private EChatChannel _tab;
    public Animator Anim;
    private bool bInited;
    public bool bRefreshNew = true;
    public bool bShow;
    public GameObject bubbleNode;
    private Image channel_friend;
    private Image channel_gulid;
    private Image channel_lobby;
    private Image channel_room;
    private Image channel_speaker;
    private Image channel_team;
    public static int ChatFaceCount = 0x4d;
    public CUIListScript ChatFaceListScript;
    public CUIFormScript chatForm;
    public static int ChatMaxLength = 20;
    public CChatParser ChatParser = new CChatParser();
    private int checkTimer = -1;
    private List<uint> curChannels;
    private GameObject deleteGameObject;
    private static float double_line_height = 76f;
    private GameObject entry_node;
    private GameObject entry_node_lobby_bg;
    private GameObject entry_node_speaker;
    private Vector3 entryPosLobby = new Vector3(0f, 63f);
    private Vector3 entryPosRoom = new Vector3(6f, 30f);
    private Vector3 entryPosSettle = new Vector3(6f, 83f);
    private Vector3 entryPosTeam = new Vector3(6f, 65f);
    public static Vector2 entrySizeLobby = new Vector2(355f, 42f);
    public static Vector2 entrySizeRoom = new Vector2(350f, 42f);
    public static Vector2 entrySizeTeam = new Vector2(350f, 42f);
    private ListView<COMDT_FRIEND_INFO> friendTablist;
    public CUIListScript FriendTabListScript;
    public CanvasGroup FriendTabListScript_cg;
    public GameObject image_template;
    public GameObject info_node_obj;
    public Text info_text;
    private InputField inputField;
    private bool lastB;
    private CUIListScript listScript;
    public CUIListScript LobbyScript;
    public CanvasGroup LobbyScript_cg;
    private GameObject loudSpeakerNode;
    private bool m_inputTextChanged;
    public static int max_bubble_num = 0x63;
    private GameObject nodeGameObject;
    private GameObject screenBtn;
    private GameObject sendBtn;
    private GameObject sendLoudSpeaker;
    private GameObject sendSpeaker;
    private static float single_line_height = 38f;
    public GameObject text_template;
    private GameObject toolBarNode;
    private static float trible_line_height = 100f;
    private Text txt_down;

    private ListView<CChatEntity> _getList()
    {
        ListView<CChatEntity> view = null;
        if (this.CurTab == EChatChannel.Lobby)
        {
            CChatChannel channel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Lobby);
            if (channel == null)
            {
                return null;
            }
            return channel.list;
        }
        if (this.CurTab == EChatChannel.Room)
        {
            CChatChannel channel2 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Room);
            if (channel2 == null)
            {
                return null;
            }
            return channel2.list;
        }
        if (this.CurTab == EChatChannel.Guild)
        {
            CChatChannel channel3 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Guild);
            if (channel3 == null)
            {
                return null;
            }
            return channel3.list;
        }
        if (this.CurTab == EChatChannel.Friend_Chat)
        {
            CChatSysData sysData = Singleton<CChatController>.instance.model.sysData;
            if (sysData == null)
            {
                return null;
            }
            CChatChannel friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(sysData.ullUid, sysData.dwLogicWorldId);
            if (friendChannel == null)
            {
                return null;
            }
            return friendChannel.list;
        }
        if (this.CurTab == EChatChannel.Team)
        {
            CChatChannel channel5 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Team);
            if (channel5 == null)
            {
                return null;
            }
            return channel5.list;
        }
        if (this.CurTab != EChatChannel.Settle)
        {
            return view;
        }
        CChatChannel channel6 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Settle);
        if (channel6 == null)
        {
            return null;
        }
        return channel6.list;
    }

    private void _refresh_friends_list(CUIListScript listScript, ListView<COMDT_FRIEND_INFO> data_list)
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
                    this.Show_FriendTabItem(elemenet.gameObject, data_list[i]);
                }
            }
        }
    }

    private void _refresh_list(CUIListScript listScript, ListView<CChatEntity> data_list, bool bShow_Last, List<Vector2> sizeVec, CChatChannel channel)
    {
        if ((listScript != null) && (channel != null))
        {
            this.calc_size(data_list, sizeVec);
            int count = data_list.Count;
            listScript.SetElementAmount(count, sizeVec);
            if (this.bRefreshNew)
            {
                listScript.MoveElementInScrollArea(count - 1, true);
            }
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                CUIListElementScript elemenet = listScript.GetElemenet(i);
                if ((elemenet != null) && listScript.IsElementInScrollArea(i))
                {
                    this.Show_ChatItem(elemenet.gameObject, data_list[i]);
                    num2 = i;
                }
            }
            if (!this.bRefreshNew)
            {
                int unreadMeanbleChatEntCount = channel.GetUnreadMeanbleChatEntCount(num2);
                if (unreadMeanbleChatEntCount > 0)
                {
                    if (this.bubbleNode != null)
                    {
                        this.bubbleNode.CustomSetActive(true);
                    }
                    Text component = this.bubbleNode.transform.Find("Text").GetComponent<Text>();
                    if (component != null)
                    {
                        if (unreadMeanbleChatEntCount > max_bubble_num)
                        {
                            component.text = string.Format("{0}+", max_bubble_num);
                        }
                        else
                        {
                            component.text = unreadMeanbleChatEntCount.ToString();
                        }
                    }
                }
                else if (this.bubbleNode != null)
                {
                    this.bubbleNode.CustomSetActive(false);
                }
            }
            else if (this.bubbleNode != null)
            {
                this.bubbleNode.CustomSetActive(false);
            }
            this.bRefreshNew = true;
        }
    }

    private void _setRedPoint(Text redText, int count)
    {
        if (redText != null)
        {
            redText.gameObject.CustomSetActive(false);
            redText.transform.parent.gameObject.CustomSetActive(false);
            if (count > 0)
            {
                redText.transform.parent.gameObject.CustomSetActive(true);
                redText.text = count.ToString();
                if ((count <= 9) && (count >= 1))
                {
                    redText.gameObject.CustomSetActive(true);
                }
            }
        }
    }

    public void BuildTabList(List<uint> list, int index = 0)
    {
        this.listScript.SetElementAmount(list.Count);
        CUIListElementScript elemenet = null;
        for (int i = 0; i < this.listScript.m_elementAmount; i++)
        {
            elemenet = this.listScript.GetElemenet(i);
            elemenet.GetComponent<CUIEventScript>().m_onClickEventParams.tag = i;
            elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText(CChatUT.GetEChatChannel_Text(list[i]));
        }
        this.listScript.SelectElement(index, true);
    }

    private void calc_size(ListView<CChatEntity> data_list, List<Vector2> sizeVec)
    {
        if ((data_list != null) && (sizeVec != null))
        {
            sizeVec.Clear();
            for (int i = 0; i < data_list.Count; i++)
            {
                CChatEntity entity = data_list[i];
                float y = (entity.numLine <= 1) ? CChatParser.element_height : (CChatParser.element_height + CChatParser.lineHeight);
                sizeVec.Add(new Vector2(CChatParser.element_width, y));
            }
        }
    }

    public void Clear_EntryForm_Node()
    {
        if ((this.txt_down != null) && (this.txt_down.gameObject != null))
        {
            GameObject gameObject = this.txt_down.gameObject;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                UnityEngine.Object.Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }
    }

    public void ClearChatForm()
    {
        this._tab = EChatChannel.None;
        this.info_node_obj = null;
        this.info_text = null;
        this.bShow = false;
        this.bRefreshNew = true;
        this.bInited = false;
        if (this.friendTablist != null)
        {
            this.friendTablist.Clear();
        }
        this.friendTablist = null;
        this.bubbleNode = null;
        this.lastB = false;
        if (this.checkTimer != -1)
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimer(this.checkTimer);
        }
        this.checkTimer = -1;
        this.listScript = null;
        this.LobbyScript = (CUIListScript) (this.FriendTabListScript = null);
        this.LobbyScript_cg = (CanvasGroup) (this.FriendTabListScript_cg = null);
        this.ChatFaceListScript = null;
        this.inputField = null;
        this.toolBarNode = null;
        this.screenBtn = null;
        this.nodeGameObject = null;
        this.deleteGameObject = null;
        this.m_inputTextChanged = false;
        this.entry_node = null;
        this.entry_node_lobby_bg = null;
        this.entry_node_speaker = null;
        this.sendSpeaker = null;
        this.sendLoudSpeaker = null;
        this.loudSpeakerNode = null;
        this.channel_friend = null;
        this.channel_gulid = null;
        this.channel_lobby = null;
        this.channel_room = null;
        this.channel_team = null;
        this.channel_speaker = null;
        this.txt_down = null;
        this.friendTablist = null;
        this.chatForm = null;
    }

    public void ClearInputText()
    {
        if (this.inputField != null)
        {
            this.inputField.text = string.Empty;
        }
    }

    public void CloseSpeakerEntryNode()
    {
    }

    private void Create_Content(GameObject pNode, string content, bool bText, float x, float y, float width, bool bSelf, bool bVip = false, bool bUse_Entry = false)
    {
        if (bText)
        {
            if ((this.text_template != null) && (pNode != null))
            {
                Text text = this.GetText("text", this.text_template, pNode);
                text.text = CUIUtility.RemoveEmoji(content);
                if (!bVip)
                {
                    if (bSelf)
                    {
                        text.color = new Color(0f, 0f, 0f);
                    }
                    else
                    {
                        text.color = CUIUtility.s_Color_White;
                    }
                }
                else if (bSelf)
                {
                    text.color = CUIUtility.s_Text_Color_Vip_Chat_Self;
                }
                else
                {
                    text.color = CUIUtility.s_Text_Color_Vip_Chat_Other;
                }
                if (bUse_Entry)
                {
                    text.color = CUIUtility.s_Color_White;
                }
                if (bUse_Entry)
                {
                    text.rectTransform.anchoredPosition = new Vector2(x, (float) -CChatParser.chat_entry_lineHeight);
                }
                else
                {
                    text.rectTransform.anchoredPosition = new Vector2(x, y);
                }
                text.rectTransform.sizeDelta = new Vector2(width, text.rectTransform.sizeDelta.y);
            }
        }
        else
        {
            int num;
            int.TryParse(content, out num);
            this.GetImage(num, this.image_template, pNode).rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    public void CreateDetailChatForm()
    {
        if (this.chatForm == null)
        {
            this.chatForm = Singleton<CUIManager>.GetInstance().OpenForm(CChatController.ChatFormPath, false, true);
            this.Init();
        }
    }

    public void Flag_Readed(EChatChannel e)
    {
        if (e == EChatChannel.Friend_Chat)
        {
            CChatSysData sysData = Singleton<CChatController>.instance.model.sysData;
            Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(sysData.ullUid, sysData.dwLogicWorldId).ReadAll();
        }
        else
        {
            Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(e).ReadAll();
        }
    }

    private Image GetImage(int index, GameObject template, GameObject pNode)
    {
        GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(template);
        obj2.name = "image";
        obj2.CustomSetActive(true);
        obj2.transform.SetParent(pNode.transform);
        Image component = obj2.GetComponent<Image>();
        component.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
        component.rectTransform.pivot = new Vector2(0f, 0f);
        component.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        if (index > 0x4d)
        {
            index = 0x4d;
        }
        if (index < 1)
        {
            index = 1;
        }
        component.SetSprite(string.Format("UGUI/Sprite/Dynamic/ChatFace/{0}", index), null, true, false, false);
        component.SetNativeSize();
        return component;
    }

    public string GetInputText()
    {
        if (this.inputField != null)
        {
            return this.inputField.text;
        }
        return null;
    }

    private Text GetText(string name, GameObject template, GameObject pNode)
    {
        GameObject obj2 = (GameObject) UnityEngine.Object.Instantiate(template);
        obj2.name = "text";
        obj2.CustomSetActive(true);
        obj2.transform.SetParent(pNode.transform);
        Text component = obj2.GetComponent<Text>();
        component.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
        component.rectTransform.pivot = new Vector2(0f, 0f);
        component.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        return component;
    }

    private int GetUnReadCount(EChatChannel channelType)
    {
        if (channelType == EChatChannel.Friend)
        {
            return Singleton<CChatController>.instance.model.channelMgr.GetFriendTotal_UnreadCount();
        }
        CChatChannel channel = Singleton<CChatController>.instance.model.channelMgr.GetChannel(channelType);
        if (channel != null)
        {
            return channel.GetUnreadCount();
        }
        return 0;
    }

    private void Hide_ChatEntry(int index)
    {
        this.HideEntryForm();
    }

    public void HideDetailChatForm()
    {
        this.bShow = false;
        if (this.chatForm != null)
        {
            this.nodeGameObject.CustomSetActive(false);
        }
        this.SetCheckTimerEnable(false);
    }

    public void HideEntry()
    {
        if (this.entry_node != null)
        {
            this.entry_node.gameObject.CustomSetActive(false);
        }
    }

    public void HideEntryForm()
    {
        this.HideEntry();
    }

    private void Init()
    {
        Singleton<CChatController>.instance.model.channelMgr.GetChannel(EChatChannel.Lobby).Init_Timer();
        this.nodeGameObject = Utility.FindChild(this.chatForm.gameObject, "node");
        this.nodeGameObject.CustomSetActive(false);
        this.Anim = this.nodeGameObject.GetComponent<Animator>();
        this.info_node_obj = this.chatForm.transform.FindChild("node/info_node").gameObject;
        this.info_text = this.chatForm.transform.FindChild("node/info_node/Text").gameObject.GetComponent<Text>();
        this.inputField = this.chatForm.transform.FindChild("node/ToolBar/InputField").gameObject.GetComponent<InputField>();
        this.inputField.onValueChange.AddListener(new UnityAction<string>(this.On_InputFiled_ValueChange));
        this.toolBarNode = this.chatForm.transform.FindChild("node/ToolBar").gameObject;
        this.sendBtn = this.chatForm.transform.FindChild("node/ToolBar/SendBtn").gameObject;
        this.bubbleNode = this.chatForm.transform.FindChild("node/bubble").gameObject;
        this.listScript = this.chatForm.transform.FindChild("node/Tab/List").gameObject.GetComponent<CUIListScript>();
        this.curChannels = Singleton<CChatController>.instance.model.channelMgr.CurActiveChannels;
        this.BuildTabList(this.curChannels, 0);
        this.entry_node = Utility.FindChild(this.chatForm.gameObject, "entry_node");
        this.entry_node.CustomSetActive(true);
        this.entry_node_speaker = Utility.FindChild(this.chatForm.gameObject, "entry_node_speaker");
        this.entry_node_speaker.CustomSetActive(false);
        this.entry_node_lobby_bg = Utility.FindChild(this.entry_node, "LobbyBg");
        this.sendSpeaker = Utility.FindChild(this.chatForm.gameObject, "node/SendSpeaker");
        this.sendLoudSpeaker = Utility.FindChild(this.chatForm.gameObject, "node/SendLoudSpeaker");
        this.loudSpeakerNode = Utility.FindChild(this.chatForm.gameObject, "node/ListView/LobbyChatList/LoudSpeakerNode");
        this.txt_down = Utility.GetComponetInChild<Text>(this.entry_node, "txt_down");
        this.txt_down.text = string.Empty;
        this.channel_friend = this.entry_node.transform.FindChild("channel_img/friend").gameObject.GetComponent<Image>();
        this.channel_gulid = this.entry_node.transform.FindChild("channel_img/gulid").gameObject.GetComponent<Image>();
        this.channel_lobby = this.entry_node.transform.FindChild("channel_img/lobby").gameObject.GetComponent<Image>();
        this.channel_room = this.entry_node.transform.FindChild("channel_img/room").gameObject.GetComponent<Image>();
        this.channel_team = this.entry_node.transform.FindChild("channel_img/team").gameObject.GetComponent<Image>();
        this.channel_speaker = this.entry_node.transform.FindChild("channel_img/speaker").gameObject.GetComponent<Image>();
        this.LobbyScript = this.chatForm.transform.FindChild("node/ListView/LobbyChatList").gameObject.GetComponent<CUIListScript>();
        this.FriendTabListScript = this.chatForm.transform.FindChild("node/ListView/FriendItemList").gameObject.GetComponent<CUIListScript>();
        this.FriendTabListScript.m_alwaysDispatchSelectedChangeEvent = true;
        this.ChatFaceListScript = this.chatForm.transform.FindChild("node/ListView/ChatFaceList").gameObject.GetComponent<CUIListScript>();
        this.screenBtn = Utility.FindChild(this.chatForm.gameObject, "node/ListView/Button");
        this.LobbyScript_cg = this.LobbyScript.GetComponent<CanvasGroup>();
        this.FriendTabListScript_cg = this.FriendTabListScript.GetComponent<CanvasGroup>();
        if (this.FriendTabListScript != null)
        {
            this.FriendTabListScript.gameObject.CustomSetActive(true);
        }
        if (this.LobbyScript != null)
        {
            this.LobbyScript.gameObject.CustomSetActive(true);
        }
        this.deleteGameObject = Utility.FindChild(this.chatForm.gameObject, "node/ToolBar/delete");
        this.deleteGameObject.CustomSetActive(false);
        this.SetInputFiledEnable(false);
        this.InitCheckTimer();
        this.text_template = Utility.FindChild(this.chatForm.gameObject, "Text_template");
        this.image_template = Utility.FindChild(this.chatForm.gameObject, "Image_template");
        this.Refresh_All_RedPoint();
        this._tab = EChatChannel.None;
        this.bInited = true;
        CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(this.chatForm.gameObject, "node/SendSpeaker");
        CUIEventScript script2 = Utility.GetComponetInChild<CUIEventScript>(this.chatForm.gameObject, "node/SendLoudSpeaker");
        componetInChild.m_onClickEventParams.commonUInt32Param1 = 0x2739;
        script2.m_onClickEventParams.commonUInt32Param1 = 0x273a;
    }

    public void InitCheckTimer()
    {
        if (this.checkTimer == -1)
        {
            this.checkTimer = Singleton<CTimerManager>.GetInstance().AddTimer(40, 0, new CTimer.OnTimeUpHandler(this.On_CheckInputField_Focus));
            UT.ResetTimer(this.checkTimer, true);
        }
    }

    public bool IsCheckHistory()
    {
        ListView<CChatEntity> view = this._getList();
        if ((view == null) || (this.LobbyScript == null))
        {
            return false;
        }
        int count = view.Count;
        if (count == 0)
        {
            return false;
        }
        return !this.LobbyScript.IsElementInScrollArea(count - 1);
    }

    public void Jump2FriendChat(COMDT_FRIEND_INFO info)
    {
        if (info != null)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Chat_EntryPanel_Click);
            CChatModel model = Singleton<CChatController>.GetInstance().model;
            model.sysData.ullUid = info.stUin.ullUid;
            model.sysData.dwLogicWorldId = info.stUin.dwLogicWorldId;
            if (model.channelMgr._getChannel(EChatChannel.Friend, info.stUin.ullUid, info.stUin.dwLogicWorldId) == null)
            {
                CChatChannel channel = model.channelMgr.CreateChannel(EChatChannel.Friend, info.stUin.ullUid, info.stUin.dwLogicWorldId);
            }
            this.curChannels = Singleton<CChatController>.instance.model.channelMgr.CurActiveChannels;
            int index = this.curChannels.IndexOf(3);
            if (index == -1)
            {
                this.curChannels.Add(3);
                index = this.curChannels.Count - 1;
            }
            this.BuildTabList(this.curChannels, index);
            this.CurTab = EChatChannel.Friend_Chat;
            this.HideEntryForm();
            this.listScript.GetSelectedElement().gameObject.transform.FindChild("Text").GetComponent<Text>().text = Utility.UTF8Convert(info.szUserName);
            Singleton<CTimerManager>.instance.AddTimer(250, 1, new CTimer.OnTimeUpHandler(this.Hide_ChatEntry));
        }
    }

    public void On_Chat_FaceList_Selected(CUIEvent uiEvent)
    {
        if (this.bInited)
        {
            int num = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex() + 1;
            this.inputField.text = string.Format("{0}%{1}", this.inputField.text, num);
        }
    }

    public void On_Chat_ScreenButton_Click()
    {
        this.SetChatFaceShow(false);
    }

    private void On_CheckInputField_Focus(int timer)
    {
        if (this.lastB != this.inputField.isFocused)
        {
            this.lastB = this.inputField.isFocused;
            this.SetInputFiledEnable(this.lastB);
        }
    }

    public void On_Friend_TabList_Selected(CUIEvent uiEvent)
    {
        if (this.bInited)
        {
            DebugHelper.Assert(uiEvent.m_srcWidgetIndexInBelongedList <= (this.friendTablist.Count - 1), "---Chat, On_Friend_TabList_Selected");
            if (uiEvent.m_srcWidgetIndexInBelongedList <= (this.friendTablist.Count - 1))
            {
                int selectedIndex = (uiEvent.m_srcWidgetScript as CUIListScript).GetSelectedIndex();
                COMDT_FRIEND_INFO comdt_friend_info = this.friendTablist[selectedIndex];
                Singleton<CChatController>.GetInstance().model.sysData.ullUid = comdt_friend_info.stUin.ullUid;
                Singleton<CChatController>.GetInstance().model.sysData.dwLogicWorldId = comdt_friend_info.stUin.dwLogicWorldId;
                COfflineChatIndex cOfflineChatIndex = Singleton<CChatController>.instance.model.GetCOfflineChatIndex(comdt_friend_info.stUin.ullUid, comdt_friend_info.stUin.dwLogicWorldId);
                if (cOfflineChatIndex != null)
                {
                    CChatNetUT.Send_Clear_Offline(cOfflineChatIndex.indexList);
                    Singleton<CChatController>.instance.model.ClearCOfflineChatIndex(cOfflineChatIndex);
                }
                GameObject gameObject = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetElemenet(selectedIndex).gameObject;
                this._setRedPoint(gameObject.transform.FindChild("head/redPoint/Text").GetComponent<Text>(), 0);
                CUICommonSystem.DelRedDot(this.listScript.GetElemenet(0).gameObject);
                this.CurTab = EChatChannel.Friend_Chat;
                this.listScript.GetSelectedElement().gameObject.transform.FindChild("Text").GetComponent<Text>().text = Utility.UTF8Convert(comdt_friend_info.szUserName);
            }
        }
    }

    public void On_FriendsList_ElementEnable(CUIEvent uievent)
    {
        int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
        COMDT_FRIEND_INFO info = null;
        ListView<COMDT_FRIEND_INFO> friendTablist = this.friendTablist;
        if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < friendTablist.Count))
        {
            info = friendTablist[srcWidgetIndexInBelongedList];
        }
        if ((info != null) && (uievent.m_srcWidget != null))
        {
            this.Show_FriendTabItem(uievent.m_srcWidget, info);
        }
    }

    private void On_InputFiled_ValueChange(string arg0)
    {
        this.m_inputTextChanged = true;
    }

    public void On_List_ElementEnable(CUIEvent uievent)
    {
        if (uievent != null)
        {
            int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
            CChatEntity ent = null;
            ListView<CChatEntity> view = this._getList();
            if (view != null)
            {
                if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < view.Count))
                {
                    ent = view[srcWidgetIndexInBelongedList];
                }
                if ((ent != null) && (uievent.m_srcWidget != null))
                {
                    this.Show_ChatItem(uievent.m_srcWidget, ent);
                }
                if ((srcWidgetIndexInBelongedList == (view.Count - 1)) && (this.bubbleNode != null))
                {
                    this.bubbleNode.CustomSetActive(false);
                }
            }
        }
    }

    public void On_Tab_Change(int index)
    {
        if (this.bInited && ((index >= 0) && (index < this.curChannels.Count)))
        {
            this.CurTab = this.curChannels[index];
        }
    }

    public void OpenSpeakerEntryNode(string constent)
    {
    }

    public void Process_Friend_Tip()
    {
        this.info_node_obj.CustomSetActive(true);
        bool flag = Singleton<CFriendContoller>.instance.model.IsAnyFriendExist(true);
        if (!Singleton<CFriendContoller>.instance.model.IsAnyFriendExist(false))
        {
            this.info_text.text = UT.GetText("Chat_NoFriend_Tip");
        }
        else if (!flag)
        {
            this.info_text.text = UT.GetText("Chat_NoOnlineFriend_Tip");
        }
        else
        {
            this.info_text.text = UT.GetText("Chat_HasFriend_Tip");
        }
    }

    public void Rebuild_ChatFace_List()
    {
        this.ChatFaceListScript.SetElementAmount(ChatFaceCount);
        for (int i = 0; i < ChatFaceCount; i++)
        {
            Image component = this.ChatFaceListScript.GetElemenet(i).GetComponent<Image>();
            UT.SetChatFace(this.ChatFaceListScript.m_belongedFormScript, component, i + 1);
        }
    }

    public void ReBuildTabText()
    {
        if (this.listScript != null)
        {
            CUIListElementScript elemenet = null;
            for (int i = 0; i < this.curChannels.Count; i++)
            {
                elemenet = this.listScript.GetElemenet(i);
                if (elemenet == null)
                {
                    return;
                }
                elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText(CChatUT.GetEChatChannel_Text(this.curChannels[i]));
            }
        }
    }

    public void Refresh_All_RedPoint()
    {
        CChatModel model = Singleton<CChatController>.instance.model;
        if (((this.curChannels != null) && (model != null)) && (this.listScript != null))
        {
            for (int i = 0; i < this.curChannels.Count; i++)
            {
                int alertNum = 0;
                uint num3 = this.curChannels[i];
                if (num3 == 3)
                {
                    alertNum = model.channelMgr.GetFriendTotal_UnreadCount();
                }
                else
                {
                    CChatChannel channel = model.channelMgr.GetChannel((EChatChannel) num3);
                    if (channel != null)
                    {
                        alertNum = channel.GetUnreadCount();
                    }
                    else
                    {
                        alertNum = 0;
                    }
                }
                CUIListElementScript elemenet = this.listScript.GetElemenet(i);
                if (elemenet != null)
                {
                    if (alertNum > 0)
                    {
                        CUICommonSystem.AddRedDot(elemenet.gameObject, enRedDotPos.enTopRight, alertNum);
                    }
                    else
                    {
                        CUICommonSystem.DelRedDot(elemenet.gameObject);
                    }
                }
            }
        }
    }

    public void Refresh_ChatEntity_List(bool bForce = true, EChatChannel tab = 9)
    {
        if (bForce || (this.CurTab == tab))
        {
            ListView<CChatEntity> list = null;
            CChatChannel friendChannel = null;
            if (this.CurTab == EChatChannel.Lobby)
            {
                friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Lobby);
                list = friendChannel.list;
                friendChannel.ReadAll();
            }
            else if (this.CurTab == EChatChannel.Room)
            {
                friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Room);
                list = friendChannel.list;
                friendChannel.ReadAll();
            }
            else if (this.CurTab == EChatChannel.Guild)
            {
                friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Guild);
                list = friendChannel.list;
                friendChannel.ReadAll();
            }
            else if (this.CurTab == EChatChannel.Friend_Chat)
            {
                CChatSysData sysData = Singleton<CChatController>.instance.model.sysData;
                friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(sysData.ullUid, sysData.dwLogicWorldId);
                list = friendChannel.list;
                this.Flag_Readed(EChatChannel.Friend_Chat);
            }
            else
            {
                if (this.CurTab == EChatChannel.Friend)
                {
                    this.friendTablist = Singleton<CFriendContoller>.GetInstance().model.GetValidChatFriendList();
                    this.friendTablist.Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.FriendDataSortForChatFriendList));
                    this._refresh_friends_list(this.FriendTabListScript, this.friendTablist);
                    return;
                }
                if (this.CurTab == EChatChannel.Team)
                {
                    friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Team);
                    list = friendChannel.list;
                    friendChannel.ReadAll();
                }
                else if (this.CurTab == EChatChannel.Settle)
                {
                    Debug.Log("---刷新 结算聊天...");
                    friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Settle);
                    list = friendChannel.list;
                    friendChannel.ReadAll();
                }
            }
            if (list != null)
            {
                this._refresh_list(this.LobbyScript, list, true, friendChannel.sizeVec, friendChannel);
            }
            this.Refresh_ChatInputView();
        }
    }

    public void Refresh_ChatInputView()
    {
        if ((this.inputField != null) && (this.sendBtn != null))
        {
            if (this.CurTab == EChatChannel.Lobby)
            {
                CChatChannel channel = Singleton<CChatController>.instance.model.channelMgr.GetChannel(EChatChannel.Lobby);
                if (Singleton<CChatController>.instance.CheckSend(EChatChannel.Lobby, string.Empty, false) == CChatController.enCheckChatResult.CdLimit)
                {
                    this.inputField.placeholder.GetComponent<Text>().text = string.Format(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_2"), channel.Get_Left_CDTime());
                }
                else if (!CSysDynamicBlock.bChatPayBlock)
                {
                    string[] args = new string[] { Singleton<CChatController>.GetInstance().model.sysData.restChatFreeCnt.ToString() };
                    this.inputField.placeholder.GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_1", args);
                    bool bActive = Singleton<CChatController>.GetInstance().model.sysData.restChatFreeCnt > 0;
                    this.sendBtn.transform.GetChild(0).gameObject.CustomSetActive(!bActive);
                    this.sendBtn.transform.GetChild(1).gameObject.CustomSetActive(bActive);
                    if (!bActive)
                    {
                        GameObject gameObject = this.sendBtn.transform.Find("CostObj").gameObject;
                        enPayType payType = CMallSystem.ResBuyTypeToPayType(Singleton<CChatController>.GetInstance().model.sysData.chatCostType);
                        if (this.chatForm != null)
                        {
                            CHeroSkinBuyManager.SetPayCostIcon(this.chatForm, gameObject.transform, payType);
                        }
                        gameObject.transform.Find("priceTxt").GetComponent<Text>().text = string.Format("x{0}", Singleton<CChatController>.GetInstance().model.sysData.chatCostNum);
                    }
                }
                else
                {
                    this.inputField.placeholder.GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_5");
                    this.sendBtn.transform.GetChild(0).gameObject.CustomSetActive(false);
                    this.sendBtn.transform.GetChild(1).gameObject.CustomSetActive(true);
                }
            }
            else if (this.CurTab == EChatChannel.Team)
            {
                this.inputField.placeholder.GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_11");
                this.sendBtn.transform.GetChild(0).gameObject.CustomSetActive(false);
                this.sendBtn.transform.GetChild(1).gameObject.CustomSetActive(true);
            }
            else
            {
                this.inputField.placeholder.GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_5");
                this.sendBtn.transform.GetChild(0).gameObject.CustomSetActive(false);
                this.sendBtn.transform.GetChild(1).gameObject.CustomSetActive(true);
            }
        }
    }

    public void Refresh_EntryForm()
    {
        CChatEntity entryEntity = Singleton<CChatController>.GetInstance().model.sysData.entryEntity;
        if ((this.txt_down != null) && (this.txt_down.gameObject != null))
        {
            GameObject gameObject = this.txt_down.gameObject;
            this.SetEntryChannelImage(Singleton<CChatController>.GetInstance().model.sysData.CurChannel);
            this.Clear_EntryForm_Node();
            bool bActive = Singleton<CChatController>.GetInstance().model.sysData.CurChannel == EChatChannel.Speaker;
            this.entry_node_lobby_bg.CustomSetActive(!bActive);
            this.entry_node_speaker.CustomSetActive(bActive);
            if (bActive)
            {
                CUIAutoScroller component = this.entry_node_speaker.GetComponent<CUIAutoScroller>();
                component.SetText(CUIUtility.RemoveEmoji(entryEntity.text));
                component.StopAutoScroll();
                component.StartAutoScroll(true);
            }
            else
            {
                for (int i = 0; i < entryEntity.TextObjList.Count; i++)
                {
                    CTextImageNode node = entryEntity.TextObjList[i];
                    if (node.bText)
                    {
                        this.Create_Content(gameObject, node.content, true, node.posX, node.posY, node.width, entryEntity.type == EChaterType.Self, false, true);
                    }
                    else
                    {
                        this.Create_Content(gameObject, node.content, false, node.posX, node.posY, node.width, entryEntity.type == EChaterType.Self, false, true);
                    }
                }
            }
        }
    }

    public void RevertNodeEntry()
    {
        if (this.entry_node != null)
        {
            this.entry_node.gameObject.CustomSetActive(true);
            RectTransform component = this.entry_node.GetComponent<RectTransform>();
            if (Singleton<CChatController>.GetInstance().model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Normal)
            {
                component.sizeDelta = entrySizeLobby;
                component.anchoredPosition = this.entryPosLobby;
            }
            else if (Singleton<CChatController>.GetInstance().model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Room)
            {
                component.sizeDelta = entrySizeRoom;
                component.anchoredPosition = this.entryPosRoom;
            }
            else if (Singleton<CChatController>.GetInstance().model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Team)
            {
                component.sizeDelta = entrySizeTeam;
                component.anchoredPosition = this.entryPosTeam;
            }
            else if (Singleton<CChatController>.GetInstance().model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Settle)
            {
                component.anchoredPosition = this.entryPosSettle;
            }
        }
    }

    public void SetChatFaceShow(bool bShow)
    {
        if (this.bInited)
        {
            this.ChatFaceListScript.gameObject.CustomSetActive(bShow);
            this.screenBtn.CustomSetActive(bShow);
            if (bShow && (this.ChatFaceListScript.GetElementAmount() < ChatFaceCount))
            {
                this.Rebuild_ChatFace_List();
            }
        }
    }

    public void SetCheckTimerEnable(bool b)
    {
        if (this.checkTimer != -1)
        {
            UT.ResetTimer(this.checkTimer, !b);
        }
    }

    public void SetEntryChannelImage(EChatChannel v)
    {
        if ((((this.channel_friend != null) && (this.channel_gulid != null)) && ((this.channel_lobby != null) && (this.channel_room != null))) && ((this.channel_team != null) && (this.channel_speaker != null)))
        {
            this.channel_friend.gameObject.CustomSetActive(false);
            this.channel_gulid.gameObject.CustomSetActive(false);
            this.channel_lobby.gameObject.CustomSetActive(false);
            this.channel_room.gameObject.CustomSetActive(false);
            this.channel_team.gameObject.CustomSetActive(false);
            this.channel_speaker.gameObject.CustomSetActive(false);
            switch (v)
            {
                case EChatChannel.Team:
                    this.channel_team.gameObject.CustomSetActive(true);
                    return;

                case EChatChannel.Room:
                    this.channel_room.gameObject.CustomSetActive(true);
                    return;

                case EChatChannel.Lobby:
                    this.channel_lobby.gameObject.CustomSetActive(true);
                    return;

                case EChatChannel.Friend:
                    this.channel_friend.gameObject.CustomSetActive(true);
                    return;

                case EChatChannel.Guild:
                    this.channel_gulid.gameObject.CustomSetActive(true);
                    return;

                case EChatChannel.Settle:
                    this.channel_room.gameObject.CustomSetActive(true);
                    return;
            }
        }
    }

    public void SetInputFiledEnable(bool bEnable)
    {
        if (bEnable)
        {
            this.deleteGameObject.CustomSetActive(false);
            this.inputField.MoveTextEnd(false);
        }
        else
        {
            this.deleteGameObject.CustomSetActive(false);
        }
    }

    public void SetShow(CanvasGroup cg, bool bShow)
    {
        if (cg != null)
        {
            if (bShow)
            {
                cg.alpha = 1f;
                cg.blocksRaycasts = true;
            }
            else
            {
                cg.alpha = 0f;
                cg.blocksRaycasts = false;
            }
        }
    }

    public void Show_ChatItem(GameObject node, CChatEntity ent)
    {
        if ((node != null) && (ent != null))
        {
            GameObject obj2 = null;
            GameObject gameObject = node.transform.Find("self").gameObject;
            GameObject obj4 = node.transform.Find("other").gameObject;
            GameObject obj5 = node.transform.Find("system").gameObject;
            GameObject obj6 = node.transform.Find("offline").gameObject;
            GameObject obj7 = node.transform.Find("time").gameObject;
            GameObject obj8 = node.transform.Find("speaker_1").gameObject;
            GameObject obj9 = node.transform.Find("speaker_2").gameObject;
            gameObject.CustomSetActive(false);
            obj4.CustomSetActive(false);
            obj5.CustomSetActive(false);
            obj6.CustomSetActive(false);
            obj7.CustomSetActive(false);
            obj8.CustomSetActive(false);
            obj9.CustomSetActive(false);
            if (ent.type == EChaterType.System)
            {
                obj2 = obj5;
            }
            else if (ent.type == EChaterType.OfflineInfo)
            {
                obj2 = obj6;
            }
            else if (ent.type == EChaterType.Self)
            {
                obj2 = gameObject;
            }
            else if (ent.type == EChaterType.Time)
            {
                obj2 = obj7;
            }
            else if (ent.type == EChaterType.Speaker)
            {
                obj2 = obj8;
            }
            else if (ent.type == EChaterType.LoudSpeaker)
            {
                obj2 = obj9;
            }
            else
            {
                obj2 = obj4;
            }
            obj2.CustomSetActive(true);
            CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(obj2, "pnlSnsHead");
            if (componetInChild != null)
            {
                componetInChild.m_onClickEventParams.commonUInt64Param1 = ent.ullUid;
                componetInChild.m_onClickEventParams.tag2 = (int) ent.iLogicWorldID;
            }
            if (ent.type == EChaterType.System)
            {
                this.ShowRawText(obj2, ent);
            }
            else if (ent.type == EChaterType.OfflineInfo)
            {
                this.ShowRawText(obj2, ent);
            }
            else if (ent.type == EChaterType.Time)
            {
                this.ShowRawText(obj2, ent);
            }
            else if (ent.type == EChaterType.Speaker)
            {
                this.ShowRich(obj2, ent);
            }
            else if (ent.type == EChaterType.LoudSpeaker)
            {
                this.ShowRich(obj2, ent);
            }
            else
            {
                this.ShowRich(obj2, ent);
            }
            ent.bHasReaded = true;
        }
    }

    public void Show_FriendTabItem(GameObject node, COMDT_FRIEND_INFO info)
    {
        node.transform.FindChild("name").GetComponent<Text>().text = UT.Bytes2String(info.szUserName);
        node.transform.FindChild("head/LevelBg/Level").GetComponent<Text>().text = info.dwPvpLvl.ToString();
        Text component = node.transform.FindChild("head/redPoint/Text").GetComponent<Text>();
        GameObject gameObject = node.transform.FindChild("head/pnlSnsHead/HttpImage").gameObject;
        string url = UT.Bytes2String(info.szHeadUrl);
        if (!CSysDynamicBlock.bSocialBlocked)
        {
            gameObject.GetComponent<CUIHttpImageScript>().SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
        }
        Image img = gameObject.GetComponent<Image>();
        if (img != null)
        {
            bool flag = Singleton<CFriendContoller>.instance.model.IsFriendOfflineOnline(info.stUin.ullUid, info.stUin.dwLogicWorldId);
            UT.SetImage(img, !flag);
        }
        if (info.stGameVip != null)
        {
            GameObject obj3 = node.transform.Find("head/pnlSnsHead/NobeIcon").gameObject;
            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(obj3.GetComponent<Image>(), (int) info.stGameVip.dwCurLevel, false);
            GameObject obj4 = node.transform.Find("head/pnlSnsHead/NobeImag").gameObject;
            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(obj4.GetComponent<Image>(), (int) info.stGameVip.dwHeadIconId);
        }
        CChatChannel friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(info.stUin.ullUid, info.stUin.dwLogicWorldId);
        this._setRedPoint(component, friendChannel.GetUnreadCount());
        CChatEntity last = friendChannel.GetLast();
        Text text2 = node.transform.FindChild("Text").GetComponent<Text>();
        if (last != null)
        {
            text2.text = last.text;
        }
        else
        {
            text2.text = Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_3");
        }
        if ((info.stGameVip != null) && (info.stGameVip.dwCurLevel > 0))
        {
            text2.color = CUIUtility.s_Text_Color_Vip_Chat_Other;
        }
    }

    public void ShowDetailChatForm()
    {
        CChatChannelMgr.EChatTab chatTab = Singleton<CChatController>.GetInstance().model.channelMgr.ChatTab;
        switch (chatTab)
        {
            case CChatChannelMgr.EChatTab.Normal:
                this.chatForm.SetPriority(enFormPriority.Priority2);
                this.sendSpeaker.CustomSetActive(true);
                this.sendLoudSpeaker.CustomSetActive(true);
                break;

            case CChatChannelMgr.EChatTab.Room:
            case CChatChannelMgr.EChatTab.Team:
                this.sendSpeaker.CustomSetActive(false);
                this.sendLoudSpeaker.CustomSetActive(false);
                break;

            case CChatChannelMgr.EChatTab.Settle:
                this.sendSpeaker.CustomSetActive(false);
                this.sendLoudSpeaker.CustomSetActive(false);
                break;
        }
        if (this.bInited && (this.chatForm != null))
        {
            this._tab = EChatChannel.None;
            this.bShow = true;
            this.nodeGameObject.CustomSetActive(true);
            this.curChannels = Singleton<CChatController>.GetInstance().model.channelMgr.CurActiveChannels;
            this.curChannels.Sort();
            if (chatTab == CChatChannelMgr.EChatTab.Normal)
            {
                this.SortChannels();
            }
            this.BuildTabList(this.curChannels, 0);
            this.CurTab = Singleton<CChatController>.instance.model.sysData.LastChannel;
            int index = 0;
            for (int i = 0; i < this.curChannels.Count; i++)
            {
                if (((EChatChannel) this.curChannels[i]) == this.CurTab)
                {
                    index = i;
                }
            }
            UT.SetListIndex(this.listScript, index);
            this.SetChatFaceShow(false);
            this.SetCheckTimerEnable(true);
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                Transform transform = this.chatForm.transform.FindChild("node/SendSpeaker");
                Transform transform2 = this.chatForm.transform.FindChild("node/SendLoudSpeaker");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(false);
                }
                if (transform2 != null)
                {
                    transform2.gameObject.CustomSetActive(false);
                }
            }
        }
    }

    public void ShowEntryForm()
    {
        if (this.chatForm != null)
        {
            switch (Singleton<CChatController>.GetInstance().model.channelMgr.ChatTab)
            {
                case CChatChannelMgr.EChatTab.Normal:
                    this.chatForm.SetPriority(enFormPriority.Priority0);
                    break;

                case CChatChannelMgr.EChatTab.Room:
                case CChatChannelMgr.EChatTab.Team:
                case CChatChannelMgr.EChatTab.Settle:
                    this.chatForm.SetPriority(enFormPriority.Priority5);
                    break;
            }
        }
        this.RevertNodeEntry();
    }

    public void ShowLoudSpeaker(bool bShow, COMDT_CHAT_MSG_HORN data = null)
    {
        if (this.loudSpeakerNode != null)
        {
            if (data != null)
            {
                string str;
                string str2;
                string str3;
                COMDT_GAME_VIP_CLIENT comdt_game_vip_client;
                CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(this.loudSpeakerNode, "pnlSnsHead/HttpImage");
                Text text = Utility.GetComponetInChild<Text>(this.loudSpeakerNode, "name");
                Text text2 = Utility.GetComponetInChild<Text>(this.loudSpeakerNode, "content");
                Image image = Utility.GetComponetInChild<Image>(this.loudSpeakerNode, "pnlSnsHead/HttpImage/NobeIcon");
                Image image2 = Utility.GetComponetInChild<Image>(this.loudSpeakerNode, "pnlSnsHead/HttpImage/NobeImag");
                Text text3 = Utility.GetComponetInChild<Text>(this.loudSpeakerNode, "pnlSnsHead/HttpImage/bg/level");
                CUIEventScript script2 = Utility.GetComponetInChild<CUIEventScript>(this.loudSpeakerNode, "pnlSnsHead");
                script2.m_onClickEventID = enUIEventID.Chat_Form_Open_Mini_Player_Info_Form;
                script2.m_onClickEventParams.commonUInt64Param1 = data.stFrom.ullUid;
                script2.m_onClickEventParams.tag2 = data.stFrom.iLogicWorldID;
                CChatUT.GetUser(EChaterType.LoudSpeaker, data.stFrom.ullUid, (uint) data.stFrom.iLogicWorldID, out str, out str2, out str3, out comdt_game_vip_client);
                text.text = str;
                text2.text = Utility.UTF8Convert(data.szContent);
                text3.text = str2;
                UT.SetHttpImage(componetInChild, str3);
                if (comdt_game_vip_client != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, (int) comdt_game_vip_client.dwCurLevel, false);
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, (int) comdt_game_vip_client.dwHeadIconId);
                }
                if (text2.preferredHeight > 60f)
                {
                    this.loudSpeakerNode.GetComponent<RectTransform>().sizeDelta = new Vector2(544f, 106f);
                }
                else
                {
                    this.loudSpeakerNode.GetComponent<RectTransform>().sizeDelta = new Vector2(544f, 86f);
                }
            }
            this.loudSpeakerNode.CustomSetActive(bShow && (data != null));
        }
    }

    private void ShowRawText(GameObject playerNode, CChatEntity ent)
    {
        playerNode.transform.Find("Text").GetComponent<Text>().text = ent.text;
    }

    private void ShowRich(GameObject playerNode, CChatEntity ent)
    {
        if ((ent.TextObjList != null) && (ent.TextObjList.Count != 0))
        {
            playerNode.transform.Find("name").gameObject.GetComponent<Text>().text = ent.name;
            GameObject gameObject = playerNode.transform.Find("pnlSnsHead/HttpImage").gameObject;
            UT.SetHttpImage(gameObject.GetComponent<CUIHttpImageScript>(), ent.head_url);
            Image component = gameObject.GetComponent<Image>();
            if (((component != null) && (ent.type == EChaterType.Friend)) && (ent.type == EChaterType.Friend))
            {
                bool flag = Singleton<CFriendContoller>.instance.model.IsFriendOfflineOnline(ent.ullUid, ent.iLogicWorldID);
                UT.SetImage(component, !flag);
            }
            if (ent.stGameVip != null)
            {
                GameObject obj3 = playerNode.transform.Find("pnlSnsHead/HttpImage/NobeIcon").gameObject;
                if (obj3 != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(obj3.GetComponent<Image>(), (int) ent.stGameVip.dwCurLevel, false);
                }
                GameObject obj4 = playerNode.transform.Find("pnlSnsHead/HttpImage/NobeImag").gameObject;
                if (obj4 != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(obj4.GetComponent<Image>(), (int) ent.stGameVip.dwHeadIconId);
                }
            }
            playerNode.transform.Find("pnlSnsHead/HttpImage/bg/level").gameObject.GetComponent<Text>().text = ent.level;
            GameObject pNode = playerNode.transform.Find("textImgNode").gameObject;
            if (pNode != null)
            {
                for (int j = 0; j < pNode.transform.childCount; j++)
                {
                    UnityEngine.Object.Destroy(pNode.transform.GetChild(j).gameObject);
                }
            }
            bool flag2 = false;
            for (int i = 0; i < ent.TextObjList.Count; i++)
            {
                CTextImageNode node = ent.TextObjList[i];
                if (node.bText)
                {
                    if (!flag2)
                    {
                        flag2 = node.posY <= -52f;
                    }
                    if (ent.stGameVip != null)
                    {
                        this.Create_Content(pNode, node.content, true, node.posX, node.posY, node.width, ent.type == EChaterType.Self, ent.stGameVip.dwCurLevel > 0, false);
                    }
                }
                else if (ent.stGameVip != null)
                {
                    this.Create_Content(pNode, node.content, false, node.posX, node.posY, node.width, ent.type == EChaterType.Self, ent.stGameVip.dwCurLevel > 0, false);
                }
            }
            RectTransform transform = playerNode.transform.Find("textImgNode").transform as RectTransform;
            if (ent.numLine == 1)
            {
                transform.sizeDelta = new Vector2(ent.final_width, single_line_height);
            }
            else if (ent.numLine == 2)
            {
                transform.sizeDelta = new Vector2(ent.final_width, double_line_height);
            }
            else
            {
                transform.sizeDelta = new Vector2(ent.final_width, trible_line_height);
            }
        }
    }

    private void SortChannels()
    {
        EChatChannel none = EChatChannel.None;
        for (int i = 0; i < this.curChannels.Count; i++)
        {
            none = this.curChannels[i];
            if ((none == EChatChannel.Friend) && (this.GetUnReadCount(none) > 0))
            {
                this.curChannels.RemoveAt(i);
                this.curChannels.Insert(0, (uint) none);
                return;
            }
            if ((none == EChatChannel.Guild) && (this.GetUnReadCount(none) > 0))
            {
                this.curChannels.RemoveAt(i);
                this.curChannels.Insert(0, (uint) none);
                return;
            }
        }
    }

    public void Update()
    {
        if (this.m_inputTextChanged && (this.inputField != null))
        {
            this.m_inputTextChanged = false;
            string text = this.inputField.text;
            if ((text != null) && (text.Length > ChatMaxLength))
            {
                this.inputField.DeactivateInputField();
                this.inputField.text = text.Substring(0, ChatMaxLength);
                Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.instance.GetText("chat_input_max"), ChatMaxLength), false);
            }
        }
    }

    public void UpView(bool bup)
    {
        if (this.chatForm != null)
        {
            if (bup)
            {
                this.chatForm.SetPriority(enFormPriority.Priority5);
            }
            else
            {
                this.chatForm.RestorePriority();
            }
        }
    }

    public EChatChannel CurTab
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
                this.SetChatFaceShow(false);
                this.SetShow(this.LobbyScript_cg, false);
                this.SetShow(this.FriendTabListScript_cg, false);
                if (this.bubbleNode != null)
                {
                    this.bubbleNode.CustomSetActive(false);
                }
                this.toolBarNode.CustomSetActive(true);
                this.info_node_obj.CustomSetActive(false);
                if (Singleton<CChatController>.instance.view != null)
                {
                    Singleton<CChatController>.instance.view.ShowLoudSpeaker(false, null);
                }
                this.Flag_Readed(this.CurTab);
                switch (this._tab)
                {
                    case EChatChannel.Team:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.Room:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.Lobby:
                        if ((Singleton<CChatController>.instance.view != null) && (Singleton<CLoudSpeakerSys>.instance.CurLoudSpeaker != null))
                        {
                            Singleton<CChatController>.instance.view.ShowLoudSpeaker(true, Singleton<CLoudSpeakerSys>.instance.CurLoudSpeaker);
                        }
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.Friend:
                        this.SetShow(this.FriendTabListScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        this.toolBarNode.CustomSetActive(false);
                        this.Process_Friend_Tip();
                        break;

                    case EChatChannel.Guild:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.Friend_Chat:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;

                    case EChatChannel.Settle:
                        this.SetShow(this.LobbyScript_cg, true);
                        this.Refresh_ChatEntity_List(true, EChatChannel.None);
                        break;
                }
                this.Refresh_All_RedPoint();
            }
        }
    }
}

