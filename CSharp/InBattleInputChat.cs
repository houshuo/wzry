using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using System;
using System.Runtime.InteropServices;
using System.Text;

public class InBattleInputChat
{
    public static int CHAT_LINE_COUNT = 4;
    public static int ChatEntityDisapearCDTime;
    private ListView<InBatChatEntity> m_caches = new ListView<InBatChatEntity>();
    private ListView<InBatChatEntity> m_chatEntityList = new ListView<InBatChatEntity>();
    public EChatCamp m_curChatCamp = EChatCamp.None;
    private InBattleMsgInputView m_view = new InBattleMsgInputView();
    public static uint Show_Max_Time = 0x2710;

    public InBattleInputChat()
    {
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattle_InputChat_SwitchCamp, new CUIEventManager.OnUIEventHandler(this.On_InputChat_SwitchCamp));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattle_InputChat_InputClick, new CUIEventManager.OnUIEventHandler(this.On_InputChat_InputClick));
    }

    public void Add(InBatChatEntity ent)
    {
        if (ent != null)
        {
            bool flag = false;
            int num = 0;
            int startIndex = 0;
            for (int i = 0; i < ent.rawText.Length; i++)
            {
                int characterWidth = this.GetCharacterWidth(ent.rawText[i]);
                if ((num + characterWidth) > (this.m_view.lineWidth - 10))
                {
                    flag = true;
                    ent.line1 = ent.rawText.Substring(startIndex, i);
                    ent.line2 = ent.rawText.Substring(i);
                    break;
                }
                num += characterWidth;
            }
            if (!flag)
            {
                ent.line1 = ent.rawText;
            }
            if ((this.m_view != null) && this.m_view.IsAllTextLineShowed())
            {
                this.m_chatEntityList.RemoveAt(0);
            }
            if (this.m_chatEntityList.Count == CHAT_LINE_COUNT)
            {
                this.m_chatEntityList.RemoveAt(0);
            }
            this.m_chatEntityList.Add(ent);
            if (this.m_view != null)
            {
                this.m_view.Refresh(this.m_chatEntityList);
            }
        }
    }

    public void Clear()
    {
        if (this.m_view != null)
        {
            this.m_view.Clear();
        }
        this.m_view = null;
        this.m_chatEntityList.Clear();
        this.m_caches.Clear();
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattle_InputChat_SwitchCamp, new CUIEventManager.OnUIEventHandler(this.On_InputChat_SwitchCamp));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattle_InputChat_InputClick, new CUIEventManager.OnUIEventHandler(this.On_InputChat_InputClick));
    }

    public InBatChatEntity ConstructEnt(string content, InBatChatEntity.EType type = 1)
    {
        InBatChatEntity validChatEntity = this.GetValidChatEntity();
        validChatEntity.ullUid = 0L;
        string str = string.Empty;
        if (type == InBatChatEntity.EType.System)
        {
            str = "[系统]";
        }
        validChatEntity.type = type;
        validChatEntity.rawText = string.Format("{0} {1}", str, content);
        validChatEntity.colorEnd = (byte) validChatEntity.rawText.Length;
        return validChatEntity;
    }

    public InBatChatEntity ConstructEnt(ulong ullUid, string playerName, string content, byte camp)
    {
        InBatChatEntity validChatEntity = this.GetValidChatEntity();
        string name = string.Empty;
        Player playerByUid = Singleton<GamePlayerCenter>.instance.GetPlayerByUid(ullUid);
        if ((playerByUid != null) && (playerByUid.Captain != 0))
        {
            name = playerByUid.Captain.handle.TheStaticData.TheResInfo.Name;
            validChatEntity.bAlice = playerByUid.Captain.handle.IsHostCamp();
        }
        validChatEntity.ullUid = ullUid;
        string str2 = string.Empty;
        if (camp == 0)
        {
            str2 = "[全部]";
        }
        string str3 = string.Format("{0}{1}({2}):", str2, playerName, name);
        validChatEntity.colorEnd = (byte) str3.Length;
        validChatEntity.rawText = string.Format("{0}{1}", str3, content);
        validChatEntity.camp = camp;
        validChatEntity.type = InBatChatEntity.EType.Normal;
        return validChatEntity;
    }

    private int GetCharacterWidth(char ch)
    {
        int num = ch;
        if ((num >= 0x20) && (num <= 0x7e))
        {
            return CChatParser.ascii_width[num - 0x20];
        }
        return this.m_view.fontSize;
    }

    private InBatChatEntity GetValidChatEntity()
    {
        InBatChatEntity entity = null;
        if (this.m_caches.Count > 0)
        {
            entity = this.m_caches[0];
            this.m_caches.RemoveAt(0);
            return entity;
        }
        return new InBatChatEntity();
    }

    public void Init(CUIFormScript battleFormScript)
    {
        if (battleFormScript != null)
        {
            if (this.m_view != null)
            {
                this.m_view.Init(battleFormScript);
            }
            for (int i = 0; i < (CHAT_LINE_COUNT + 2); i++)
            {
                this.m_caches.Add(new InBatChatEntity());
            }
            this.SetCurChatCamp(EChatCamp.Alice);
        }
    }

    private void On_InputChat_InputClick(CUIEvent uievent)
    {
        if (this.m_view != null)
        {
            this.m_view.EnableInputFiled();
        }
    }

    private void On_InputChat_SwitchCamp(CUIEvent uievent)
    {
        if (this.m_curChatCamp == EChatCamp.Alice)
        {
            this.SetCurChatCamp(EChatCamp.All);
        }
        else if (this.m_curChatCamp == EChatCamp.All)
        {
            this.SetCurChatCamp(EChatCamp.Alice);
        }
        else
        {
            this.SetCurChatCamp(EChatCamp.Alice);
        }
    }

    private void ReclyChatEntity(InBatChatEntity ent)
    {
        if (ent != null)
        {
            int index = -1;
            for (int i = 0; i < this.m_chatEntityList.Count; i++)
            {
                InBatChatEntity entity = this.m_chatEntityList[i];
                if ((entity != null) && (ent.ullUid == entity.ullUid))
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                this.m_caches.Add(ent);
                this.m_chatEntityList.RemoveAt(index);
                if (this.m_view != null)
                {
                    this.m_view.Refresh(this.m_chatEntityList);
                }
            }
        }
    }

    public void ServerDisableInputChat()
    {
        if (this.m_view != null)
        {
            this.m_view.SetChatButtonNodeCheckable(false);
            this.m_view.SetChatLineNodeShowable(false);
        }
    }

    private void SetCurChatCamp(EChatCamp v)
    {
        if (this.m_curChatCamp != v)
        {
            this.m_curChatCamp = v;
            if (this.m_view != null)
            {
                this.m_view.ShowCamp(this.m_curChatCamp);
            }
        }
    }

    public void SetInputChatEnable(int v)
    {
        if (this.m_view != null)
        {
            bool bShow = v == 1;
            this.m_view.SetChatLineNodeShowable(bShow);
            this.m_view.SetChatButtonNodeShowable(bShow);
        }
    }

    public void Update()
    {
        for (int i = 0; i < this.m_chatEntityList.Count; i++)
        {
            InBatChatEntity entity = this.m_chatEntityList[i];
            if ((entity != null) && entity.bShow)
            {
                entity.UpdateCooldown();
            }
        }
        if (this.m_view != null)
        {
            this.m_view.Update();
        }
    }

    public enum EChatCamp
    {
        Alice = 1,
        All = 0,
        None = -1
    }

    public class InBatChatEntity
    {
        public bool bAlice;
        private bool bLine1Colored;
        private bool bLine2Colored;
        public bool bShow;
        public byte camp;
        private static readonly string COLOR_ENEMY = "<color=#ff463c>";
        private static readonly string COLOR_OTHERS = "<color=#18c8ff>";
        private static readonly string COLOR_SYSTEM = "<color=#ffbe00>";
        private static readonly string COLOR_WHITE = "<color=#ffffff>";
        public byte colorEnd;
        public string line1;
        public string line2;
        private uint m_maxCooldownTime;
        private ulong m_startCooldownTimestamp;
        public string rawText;
        public byte startIndex;
        public EType type;
        public ulong ullUid;

        private bool _shade(StringBuilder builder, ref string content, int endIndex, string color1)
        {
            if ((builder == null) || string.IsNullOrEmpty(content))
            {
                return false;
            }
            builder.Remove(0, builder.Length);
            builder.Append((string) content);
            if (endIndex != content.Length)
            {
                builder.Insert(0, color1);
                builder.Insert(this.colorEnd + color1.Length, "</color><color=#ffffff>");
                builder.Insert(builder.Length, "</color>");
            }
            else
            {
                builder.Insert(0, color1);
                builder.Insert(builder.Length, "</color>");
            }
            content = builder.ToString();
            return true;
        }

        public void Clear()
        {
            this.colorEnd = 0;
            this.ullUid = 0L;
            this.camp = 0;
            this.type = EType.Normal;
            this.startIndex = 0;
            this.line1 = this.line2 = string.Empty;
            this.rawText = string.Empty;
            this.bShow = false;
            this.m_maxCooldownTime = 0;
            this.m_startCooldownTimestamp = 0L;
            this.bLine1Colored = this.bLine1Colored = false;
        }

        public bool IsInCoolDown()
        {
            return (this.m_maxCooldownTime > 0);
        }

        public void Shade(StringBuilder builder)
        {
            if (((builder != null) && (this.colorEnd > 0)) && !string.IsNullOrEmpty(this.line1))
            {
                if (this.colorEnd > this.line1.Length)
                {
                    this.colorEnd = (byte) this.line1.Length;
                }
                if (this.type == EType.System)
                {
                    if (!this.bLine1Colored && !string.IsNullOrEmpty(this.line1))
                    {
                        this.bLine1Colored = this._shade(builder, ref this.line1, this.line1.Length, COLOR_SYSTEM);
                    }
                    if (!this.bLine2Colored && !string.IsNullOrEmpty(this.line2))
                    {
                        this.bLine2Colored = this._shade(builder, ref this.line2, this.line2.Length, COLOR_SYSTEM);
                    }
                }
                else
                {
                    if (!this.bLine1Colored && !string.IsNullOrEmpty(this.line1))
                    {
                        this.bLine1Colored = this._shade(builder, ref this.line1, this.colorEnd, this.bAlice ? COLOR_OTHERS : COLOR_ENEMY);
                    }
                    if (!this.bLine2Colored && !string.IsNullOrEmpty(this.line2))
                    {
                        this.bLine2Colored = this._shade(builder, ref this.line2, this.line2.Length, COLOR_WHITE);
                    }
                }
            }
        }

        public void StartCooldown(uint maxCooldownTime = 0)
        {
            if (maxCooldownTime == 0)
            {
                maxCooldownTime = InBattleInputChat.Show_Max_Time;
            }
            this.m_maxCooldownTime = maxCooldownTime;
            if (maxCooldownTime > 0)
            {
                this.m_startCooldownTimestamp = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
            }
        }

        public void UpdateCooldown()
        {
            if ((this.m_startCooldownTimestamp != 0) && (this.m_maxCooldownTime != 0))
            {
                uint num = (uint) (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.m_startCooldownTimestamp);
                if (num >= this.m_maxCooldownTime)
                {
                    this.m_startCooldownTimestamp = 0L;
                    InBattleInputChat inputChat = Singleton<InBattleMsgMgr>.instance.m_InputChat;
                    if (inputChat != null)
                    {
                        this.Clear();
                        inputChat.ReclyChatEntity(this);
                    }
                }
            }
        }

        public enum EType
        {
            None = -1,
            Normal = 0,
            System = 1
        }
    }
}

