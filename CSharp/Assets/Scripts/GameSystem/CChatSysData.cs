namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using System;

    public class CChatSysData
    {
        public EChatChannel CurChannel = EChatChannel.None;
        public uint dwLogicWorldId;
        public CChatEntity entryEntity = new CChatEntity();
        public EChatChannel LastChannel = EChatChannel.None;
        public uint lastTimeStamp = 0;
        private int m_chatCostNum;
        private int m_chatCostType;
        public uint restChatFreeCnt;
        public ulong ullUid;

        public void Add_NewContent_Entry(string a, EChatChannel curChannel)
        {
            if ((curChannel != EChatChannel.Lobby) || !Singleton<CLoudSpeakerSys>.instance.IsSpeakerShowing())
            {
                this.CurChannel = curChannel;
                if (this.entryEntity != null)
                {
                    this.entryEntity.Clear();
                    this.entryEntity.text = a;
                    float x = CChatView.entrySizeLobby.x;
                    if (Singleton<CChatController>.instance.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Room)
                    {
                        x = CChatView.entrySizeRoom.x;
                    }
                    else if (Singleton<CChatController>.instance.model.channelMgr.ChatTab == CChatChannelMgr.EChatTab.Team)
                    {
                        x = CChatView.entrySizeTeam.x;
                    }
                    Singleton<CChatController>.instance.view.ChatParser.bProc_ChatEntry = true;
                    Singleton<CChatController>.instance.view.ChatParser.maxWidth = ((int) x) - CChatParser.chat_entry_channel_img_width;
                    Singleton<CChatController>.instance.view.ChatParser.Parse(this.entryEntity.text, CChatParser.start_x, this.entryEntity);
                }
            }
        }

        public void Add_NewContent_Entry_Speaker(string a)
        {
            this.CurChannel = EChatChannel.Speaker;
            if (this.entryEntity != null)
            {
                this.entryEntity.Clear();
                this.entryEntity.text = a;
                float x = CChatView.entrySizeLobby.x;
                Singleton<CChatController>.instance.view.ChatParser.bProc_ChatEntry = true;
                Singleton<CChatController>.instance.view.ChatParser.maxWidth = ((int) x) - CChatParser.chat_entry_channel_img_width;
                Singleton<CChatController>.instance.view.ChatParser.Parse(this.entryEntity.text, CChatParser.start_x, this.entryEntity);
            }
        }

        public void Clear()
        {
            this.ullUid = 0L;
            this.dwLogicWorldId = 0;
            this.lastTimeStamp = 0;
            this.LastChannel = EChatChannel.None;
            this.CurChannel = EChatChannel.None;
            this.restChatFreeCnt = 0;
            this.m_chatCostNum = 0;
            this.m_chatCostType = 0;
            this.entryEntity.Clear();
        }

        public void ClearEntryText()
        {
            this.CurChannel = EChatChannel.None;
            if (this.entryEntity != null)
            {
                this.entryEntity.Clear();
            }
            Singleton<CChatController>.instance.view.SetEntryChannelImage(EChatChannel.None);
            Singleton<CChatController>.instance.view.Clear_EntryForm_Node();
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_ChatEntry_Change");
        }

        public int chatCostNum
        {
            get
            {
                if (this.m_chatCostNum == 0)
                {
                    this.m_chatCostNum = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x7d).dwConfValue;
                }
                return this.m_chatCostNum;
            }
        }

        public int chatCostType
        {
            get
            {
                if (this.m_chatCostType == 0)
                {
                    this.m_chatCostType = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x7e).dwConfValue;
                }
                return this.m_chatCostType;
            }
        }
    }
}

