using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InBattleMsgInputView
{
    public uint intime_cd = 0x1b58;
    public int intimeMaxCount = 5;
    public int intimeMSecond = 0x2710;
    public ulong intimeMSecond_StartTime;
    private int intimeSendCount;
    private StringBuilder lineBuilder = new StringBuilder();
    private GameObject m_buttonNode;
    private GameObject[] m_cacheNode = new GameObject[InBattleInputChat.CHAT_LINE_COUNT];
    private Text m_campTxt;
    private string m_inCDTxt;
    private CDButton m_inputButton;
    public InputField m_inputField;
    private GameObject m_inputText;
    private string m_inWramBattle;
    private GameObject m_lineNode;
    private bool[] m_nodeUseage = new bool[InBattleInputChat.CHAT_LINE_COUNT];

    private void CalcLineWidth(out int lineWidth, out int fontSize)
    {
        GameObject obj2 = this.m_cacheNode[0];
        if (obj2 == null)
        {
            lineWidth = 200;
            fontSize = 0x12;
        }
        else
        {
            RectTransform transform = obj2.transform as RectTransform;
            if (transform == null)
            {
                lineWidth = 200;
                fontSize = 0x12;
            }
            else
            {
                lineWidth = (int) transform.sizeDelta.x;
                Text component = obj2.transform.Find("Text").GetComponent<Text>();
                if (component == null)
                {
                    lineWidth = 200;
                    fontSize = 0x12;
                }
                else
                {
                    fontSize = component.fontSize;
                }
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < this.m_cacheNode.Length; i++)
        {
            this.m_cacheNode[i] = null;
        }
        this.m_cacheNode = null;
        this.m_nodeUseage = null;
        this.m_lineNode = null;
        this.m_buttonNode = null;
        this.m_campTxt = null;
        this.m_inputText = null;
        this.m_inputField.onEndEdit.RemoveAllListeners();
        this.m_inputField = null;
        this.m_inCDTxt = (string) (this.m_inWramBattle = null);
        if (this.m_inputButton != null)
        {
            this.m_inputButton.Clear();
        }
        this.m_inputButton = null;
        this.intimeSendCount = 0;
        this.intimeMSecond_StartTime = 0L;
    }

    public void EnableInputFiled()
    {
        if (this.m_inputField != null)
        {
            this.m_inputField.ActivateInputField();
        }
    }

    private void GetCom(int index, out Text txtCom)
    {
        DebugHelper.Assert(index < InBattleInputChat.CHAT_LINE_COUNT, string.Format("---InBattleMsgInputView.GetCom index < CHAT_LINE_COUNT, index:{0},CHAT_LINE_COUNT:{1}", index, InBattleInputChat.CHAT_LINE_COUNT));
        txtCom = null;
        if (index < InBattleInputChat.CHAT_LINE_COUNT)
        {
            GameObject obj2 = this.m_cacheNode[index];
            DebugHelper.Assert(obj2 != null, string.Format("---InBattleMsgInputView.GetCom node != null, index:{0}", index));
            if (obj2 != null)
            {
                GameObject gameObject = obj2.transform.Find("Text").gameObject;
                DebugHelper.Assert(gameObject != null, string.Format("---InBattleMsgInputView.GetCom txtObj != null, index:{0}", index));
                txtCom = gameObject.GetComponent<Text>();
            }
        }
    }

    public void Init(CUIFormScript battleFormScript)
    {
        if (battleFormScript != null)
        {
            int num2;
            int num3;
            this.m_lineNode = Utility.FindChild(battleFormScript.gameObject, "InputChat_Lines");
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(InBattleShortcut.InBattleMsgView_FORM_PATH);
            this.m_campTxt = Utility.FindChild(form.gameObject, "chatTools/node/InputChat_Buttons/Button_Camp/Text").GetComponent<Text>();
            this.m_buttonNode = Utility.FindChild(form.gameObject, "chatTools/node/InputChat_Buttons");
            this.m_inputField = Utility.FindChild(form.gameObject, "chatTools/node/InputChat_Buttons/Button_Input/InputField").GetComponent<InputField>();
            this.m_inputText = Utility.FindChild(this.m_inputField.gameObject, "Text");
            GameObject button = Utility.FindChild(form.gameObject, "chatTools/node/InputChat_Buttons/Button_Input/CDButton");
            if (button != null)
            {
                this.m_inputButton = new CDButton(button);
            }
            DebugHelper.Assert(this.m_campTxt != null, "---InBattleMsgInputView m_campTxt == null, check out...");
            DebugHelper.Assert(this.m_lineNode != null, "---InBattleMsgInputView m_lineNode == null, check out...");
            DebugHelper.Assert(this.m_buttonNode != null, "---InBattleMsgInputView m_buttonNode == null, check out...");
            DebugHelper.Assert(this.m_inputField != null, "---InBattleMsgInputView m_inputField == null, check out...");
            DebugHelper.Assert(this.m_inputText != null, "---InBattleMsgInputView m_inputText == null, check out...");
            bool bActive = GameSettings.InBattleInputChatEnable == 1;
            if (this.m_lineNode != null)
            {
                this.m_lineNode.CustomSetActive(bActive);
            }
            if (this.m_buttonNode != null)
            {
                this.m_buttonNode.CustomSetActive(bActive);
            }
            for (int i = 0; i < InBattleInputChat.CHAT_LINE_COUNT; i++)
            {
                this.m_cacheNode[i] = Utility.FindChild(battleFormScript.gameObject, string.Format("InputChat_Lines/line{0}", i));
                DebugHelper.Assert(this.m_cacheNode[i] != null, "---InBattleMsgInputView m_cacheNode == null, index:" + i);
                if (this.m_cacheNode[i] != null)
                {
                    this.m_cacheNode[i].CustomSetActive(false);
                }
            }
            if (this.m_inputField != null)
            {
                this.m_inputField.onEndEdit.AddListener(new UnityAction<string>(this.On_EndEdit));
            }
            this.CalcLineWidth(out num2, out num3);
            this.lineWidth = num2;
            this.fontSize = num3;
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("InBat_IntimeMaxCount"), out this.intimeMaxCount))
            {
                DebugHelper.Assert(false, "---InBatMsg jason 你配置的 InBat_IntimeMaxCount 好像不是整数哦， check out");
            }
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("InBat_IntimeMSecond"), out this.intimeMSecond))
            {
                DebugHelper.Assert(false, "---InBatMsg jason 你配置的 InBat_IntimeMSecond 好像不是整数哦， check out");
            }
            if (!uint.TryParse(Singleton<CTextManager>.instance.GetText("InBat_IntimeCD"), out this.intime_cd))
            {
                DebugHelper.Assert(false, "---InBatMsg jason 你配置的 InBat_IntimeCD 好像不是整数哦， check out");
            }
            this.m_inCDTxt = Singleton<CTextManager>.instance.GetText("InBatInput_InCD");
            this.m_inWramBattle = Singleton<CTextManager>.instance.GetText("InBatInput_InWarm");
        }
    }

    public bool IsAllTextLineShowed()
    {
        for (int i = 0; i < this.m_cacheNode.Length; i++)
        {
            GameObject obj2 = this.m_cacheNode[i];
            if ((obj2 != null) && !obj2.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsSameCamp(ulong uid)
    {
        Player playerByUid = Singleton<GamePlayerCenter>.instance.GetPlayerByUid(uid);
        if (playerByUid == null)
        {
            return false;
        }
        return Singleton<GamePlayerCenter>.instance.IsAtSameCamp(Singleton<GamePlayerCenter>.instance.HostPlayerId, playerByUid.PlayerId);
    }

    private void On_EndEdit(string content)
    {
        Singleton<InBattleMsgMgr>.instance.HideView();
        if (this.m_inputField != null)
        {
            this.m_inputField.text = string.Empty;
        }
        if (this.intimeMSecond_StartTime != 0)
        {
            uint num = (uint) (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.intimeMSecond_StartTime);
            if (num < this.intimeMSecond)
            {
                if ((this.intimeSendCount + 1) > this.intimeMaxCount)
                {
                    if (this.m_inputButton != null)
                    {
                        this.m_inputButton.StartCooldown(this.intime_cd, new System.Action(this.OnBlockCDEnd));
                    }
                    InBattleInputChat chat = Singleton<InBattleMsgMgr>.instance.m_InputChat;
                    if (chat != null)
                    {
                        InBattleInputChat.InBatChatEntity ent = chat.ConstructEnt(this.m_inCDTxt, InBattleInputChat.InBatChatEntity.EType.System);
                        chat.Add(ent);
                    }
                    this.intimeSendCount = 0;
                    return;
                }
            }
            else
            {
                this.intimeSendCount = 0;
            }
        }
        InBattleInputChat inputChat = Singleton<InBattleMsgMgr>.instance.m_InputChat;
        if ((inputChat != null) && !string.IsNullOrEmpty(content))
        {
            content = CUIUtility.RemoveEmoji(content);
            if (this.m_inputText != null)
            {
                this.m_inputText.CustomSetActive(false);
            }
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            if ((curLvelContext != null) && !Singleton<InBattleMsgMgr>.instance.ShouldBeThroughNet(curLvelContext))
            {
                InBattleInputChat chat3 = Singleton<InBattleMsgMgr>.instance.m_InputChat;
                if (chat3 != null)
                {
                    InBattleInputChat.InBatChatEntity entity2 = chat3.ConstructEnt(this.m_inWramBattle, InBattleInputChat.InBatChatEntity.EType.System);
                    chat3.Add(entity2);
                }
            }
            else if (!string.IsNullOrEmpty(content))
            {
                InBattleMsgNetCore.SendInBattleMsg_InputChat(content, (byte) inputChat.m_curChatCamp);
            }
            if (this.intimeSendCount == 0)
            {
                this.intimeMSecond_StartTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
            }
            this.intimeSendCount++;
        }
    }

    private void OnBlockCDEnd()
    {
        this.intimeSendCount = 0;
    }

    public void Refresh(ListView<InBattleInputChat.InBatChatEntity> chatEntityList)
    {
        if (chatEntityList != null)
        {
            int index = 0;
            for (int i = 0; (i < chatEntityList.Count) && (index < InBattleInputChat.CHAT_LINE_COUNT); i++)
            {
                InBattleInputChat.InBatChatEntity entity = chatEntityList[i];
                if (entity != null)
                {
                    entity.Shade(this.lineBuilder);
                    if (!string.IsNullOrEmpty(entity.line1) && this.ShowTextInLine(index, entity.line1, entity.camp, entity.type, entity.ullUid))
                    {
                        entity.bShow = true;
                        if (this.m_cacheNode[index] != null)
                        {
                            this.m_cacheNode[index].CustomSetActive(true);
                        }
                        if (!entity.IsInCoolDown())
                        {
                            entity.StartCooldown(0);
                        }
                        index++;
                    }
                    if (!string.IsNullOrEmpty(entity.line2) && this.ShowTextInLine(index, entity.line2, entity.camp, entity.type, entity.ullUid))
                    {
                        entity.bShow = true;
                        if (this.m_cacheNode[index] != null)
                        {
                            this.m_cacheNode[index].CustomSetActive(true);
                        }
                        if (!entity.IsInCoolDown())
                        {
                            entity.StartCooldown(0);
                        }
                        index++;
                    }
                }
            }
            while (index < InBattleInputChat.CHAT_LINE_COUNT)
            {
                if (this.m_cacheNode[index] != null)
                {
                    this.m_cacheNode[index].CustomSetActive(false);
                }
                index++;
            }
        }
    }

    public void SetChatButtonNodeCheckable(bool bEnable)
    {
        if (this.m_buttonNode != null)
        {
            GameObject gameObject = this.m_buttonNode.transform.Find("Button_Camp").gameObject;
            GameObject obj3 = this.m_buttonNode.transform.Find("Button_Input").gameObject;
            DebugHelper.Assert(gameObject != null, "---InBattleMsgInputView.SetChatButtonNodeCheckable campObj  != null");
            DebugHelper.Assert(obj3 != null, "---InBattleMsgInputView.SetChatButtonNodeCheckable inputObj != null");
            if (gameObject != null)
            {
                gameObject.GetComponent<CUIEventScript>().enabled = bEnable;
            }
            if (obj3 != null)
            {
                obj3.GetComponent<CUIEventScript>().enabled = bEnable;
            }
        }
    }

    public void SetChatButtonNodeShowable(bool bShow)
    {
        if (this.m_buttonNode != null)
        {
            this.m_buttonNode.gameObject.CustomSetActive(bShow);
        }
    }

    public void SetChatLineNodeShowable(bool bShow)
    {
        if (this.m_lineNode != null)
        {
            this.m_lineNode.gameObject.CustomSetActive(bShow);
        }
    }

    public void ShowCamp(InBattleInputChat.EChatCamp v)
    {
        if (v != InBattleInputChat.EChatCamp.None)
        {
            if (v == InBattleInputChat.EChatCamp.Alice)
            {
                this.m_campTxt.text = "我方";
            }
            else
            {
                this.m_campTxt.text = "全部";
            }
        }
    }

    private bool ShowTextInLine(int index, string content, byte camp, InBattleInputChat.InBatChatEntity.EType type, ulong uid)
    {
        if (index < InBattleInputChat.CHAT_LINE_COUNT)
        {
            Text txtCom = null;
            this.GetCom(index, out txtCom);
            if (txtCom != null)
            {
                txtCom.text = content;
                return true;
            }
        }
        return false;
    }

    public void Update()
    {
        if (this.m_inputButton != null)
        {
            this.m_inputButton.Update();
        }
    }

    public int fontSize { get; private set; }

    public int lineWidth { get; private set; }
}

