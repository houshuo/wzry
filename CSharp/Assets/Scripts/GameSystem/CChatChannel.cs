namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CChatChannel
    {
        public int addTimeSplice_timer = -1;
        public bool bOffline;
        public uint cd_time;
        public EChatChannel ChannelType;
        private static uint clt_pendding_time = 0x7d0;
        public uint dwLogicWorldId;
        private int lastSendTime;
        public uint lastTimeStamp;
        public ListView<CChatEntity> list = new ListView<CChatEntity>();
        public static int MaxDeltaTime_Seconds = 60;
        public List<Vector2> sizeVec = new List<Vector2>();
        public ulong ullUid;
        private int unread_count;
        private int unread_time_entity_count;
        private int unreadIndex;

        public CChatChannel(EChatChannel channelType, uint cdTime = 0, ulong ullUid = 0, uint dwLogicWorldId = 0)
        {
            this.ChannelType = channelType;
            this.cd_time = 0;
            this.ullUid = ullUid;
            this.dwLogicWorldId = dwLogicWorldId;
        }

        public void Add(CChatEntity ent)
        {
            if (ent != null)
            {
                if (ent.type == EChaterType.Time)
                {
                    this.unread_time_entity_count++;
                }
                this.list.Add(ent);
                if (this.list.Count > CChatController.MaxCount)
                {
                    this.list.RemoveRange(0, this.list.Count - CChatController.MaxCount);
                }
                if (this.IsMeanbleChatEnt(ent))
                {
                    this.unread_count++;
                    ent.bHasReaded = false;
                }
            }
        }

        public void Clear()
        {
            this.list.Clear();
            this.sizeVec.Clear();
            this.ullUid = 0L;
            this.dwLogicWorldId = 0;
            for (int i = 0; i < this.list.Count; i++)
            {
                this.list[i].Clear();
            }
            this.list.Clear();
            this.unread_time_entity_count = this.unreadIndex = 0;
            this.lastTimeStamp = 0;
            this.lastSendTime = 0;
            this.unread_count = 0;
        }

        public void ClearCd()
        {
            this.lastSendTime = 0;
        }

        public int Get_Left_CDTime()
        {
            return (int) Mathf.Max(0f, (float) ((this.lastSendTime + this.cd_time) - CRoleInfo.GetCurrentUTCTime()));
        }

        public CChatEntity GetLast()
        {
            if (this.list.Count == 0)
            {
                return null;
            }
            if (this.ChannelType != EChatChannel.Friend)
            {
                return this.list[this.list.Count - 1];
            }
            return (((this.list[this.list.Count - 1].type != EChaterType.System) && (this.list[this.list.Count - 1].type != EChaterType.OfflineInfo)) ? this.list[this.list.Count - 1] : null);
        }

        public int GetUnreadCount()
        {
            return this.unread_count;
        }

        public int GetUnreadMeanbleChatEntCount(int start_index = 0)
        {
            int num = 0;
            for (int i = start_index; i < this.list.Count; i++)
            {
                CChatEntity ent = this.list[i];
                if ((ent != null) && (this.IsMeanbleChatEnt(ent) && !ent.bHasReaded))
                {
                    num++;
                }
            }
            return num;
        }

        public bool HasAnyValidChatEntity()
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                CChatEntity entity = this.list[i];
                if ((entity != null) && (((entity.type == EChaterType.Self) || (entity.type == EChaterType.Friend)) || ((entity.type == EChaterType.Strenger) || (entity.type == EChaterType.GuildMember))))
                {
                    return true;
                }
            }
            return false;
        }

        public void Init_Timer()
        {
            if (this.ChannelType == EChatChannel.Lobby)
            {
                ResAcntExpInfo dataByKey = GameDataMgr.acntExpDatabin.GetDataByKey(Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel);
                this.cd_time = dataByKey.dwChatCD;
                if (this.cd_time > 0)
                {
                    this.cd_time += clt_pendding_time;
                }
                this.cd_time /= 0x3e8;
            }
        }

        public void InitChat_InputTimer(int time)
        {
            this.lastSendTime = 0;
            if (time > 0)
            {
                this.cd_time = ((uint) time) + clt_pendding_time;
                this.cd_time /= 0x3e8;
            }
        }

        public bool IsInputValid()
        {
            return ((this.lastSendTime + this.cd_time) <= CRoleInfo.GetCurrentUTCTime());
        }

        public bool IsMeanbleChatEnt(CChatEntity ent)
        {
            return (((ent.type != EChaterType.System) && (ent.type != EChaterType.Time)) && (ent.type != EChaterType.OfflineInfo));
        }

        public void ReadAll()
        {
            this.unread_count = 0;
        }

        public void Start_InputCD()
        {
            if (this.cd_time != 0)
            {
                this.lastSendTime = CRoleInfo.GetCurrentUTCTime();
            }
        }
    }
}

