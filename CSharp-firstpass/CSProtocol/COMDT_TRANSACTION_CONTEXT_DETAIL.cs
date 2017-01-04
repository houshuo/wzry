namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_TRANSACTION_CONTEXT_DETAIL : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bReserved;
        public static readonly int CLASS_ID = 0x177;
        public static readonly uint CURRVERSION = 0x89;
        public ProtocolObject dataObject;

        public TdrError.ErrorType construct(long selector)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.construct();
            }
            this.bReserved = 0;
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            this.bReserved = 0;
        }

        public TdrError.ErrorType pack(long selector, ref TdrWriteBuf destBuf, uint cutVer)
        {
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.pack(ref destBuf, cutVer);
            }
            type = destBuf.writeUInt8(this.bReserved);
            if (type != TdrError.ErrorType.TDR_NO_ERROR)
            {
                return type;
            }
            return type;
        }

        public TdrError.ErrorType pack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if ((buffer.GetLength(0) == 0) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrWriteBuf destBuf = ClassObjPool<TdrWriteBuf>.Get();
            destBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.pack(selector, ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();
                usedSize = destBuf.getUsedSize();
            }
            destBuf.Release();
            return type;
        }

        public ProtocolObject select(long selector)
        {
            if (selector <= 0x17L)
            {
                this.select_1_23(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_1_23(long selector)
        {
            long num = selector;
            if ((num >= 1L) && (num <= 0x17L))
            {
                switch (((int) (num - 1L)))
                {
                    case 0:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_RANK))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_RANK) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_RANK.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_ONLINECHK))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_ONLINECHK) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_ONLINECHK.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_JOIN_GUILD))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_JOIN_GUILD) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_JOIN_GUILD.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_QUIT_GUILD))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_QUIT_GUILD) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_QUIT_GUILD.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GUILD_INVITE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_GUILD_INVITE) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GUILD_INVITE.CLASS_ID);
                        }
                        return;

                    case 6:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_SEARCH_GUILD))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_SEARCH_GUILD) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_SEARCH_GUILD.CLASS_ID);
                        }
                        return;

                    case 7:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_DEAL_GUILD_INVITE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_DEAL_GUILD_INVITE) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_DEAL_GUILD_INVITE.CLASS_ID);
                        }
                        return;

                    case 8:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GUILD_RECOMMEND))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_GUILD_RECOMMEND) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GUILD_RECOMMEND.CLASS_ID);
                        }
                        return;

                    case 9:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GUILD_GETNAME))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_GUILD_GETNAME) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GUILD_GETNAME.CLASS_ID);
                        }
                        return;

                    case 10:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GETARENA_TARGETDATA))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_GETARENA_TARGETDATA) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GETARENA_TARGETDATA.CLASS_ID);
                        }
                        return;

                    case 11:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_APPOINT_POSITION))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_APPOINT_POSITION) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_APPOINT_POSITION.CLASS_ID);
                        }
                        return;

                    case 12:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_FIRE_MEMBER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_FIRE_MEMBER) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_FIRE_MEMBER.CLASS_ID);
                        }
                        return;

                    case 13:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_ADD_ARENAFIGHT_HISTORY))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_ADD_ARENAFIGHT_HISTORY) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_ADD_ARENAFIGHT_HISTORY.CLASS_ID);
                        }
                        return;

                    case 14:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_DELETEBURNINGENEMY))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_DELETEBURNINGENEMY) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_DELETEBURNINGENEMY.CLASS_ID);
                        }
                        return;

                    case 15:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_TRANS_BASE_INFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_TRANS_BASE_INFO) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_TRANS_BASE_INFO.CLASS_ID);
                        }
                        return;

                    case 0x10:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_ADD_RANKCURSEASONDATA))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_ADD_RANKCURSEASONDATA) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_ADD_RANKCURSEASONDATA.CLASS_ID);
                        }
                        return;

                    case 0x11:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA.CLASS_ID);
                        }
                        return;

                    case 0x12:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_LUCKYDRAW))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_LUCKYDRAW) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_LUCKYDRAW.CLASS_ID);
                        }
                        return;

                    case 0x13:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_CHANGE_PLAYER_NAME))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_CHANGE_PLAYER_NAME) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_CHANGE_PLAYER_NAME.CLASS_ID);
                        }
                        return;

                    case 20:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_CHANGE_GUILD_NAME))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_CHANGE_GUILD_NAME) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_CHANGE_GUILD_NAME.CLASS_ID);
                        }
                        return;

                    case 0x15:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_ADD_FIGHTHISTORY))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_ADD_FIGHTHISTORY) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_ADD_FIGHTHISTORY.CLASS_ID);
                        }
                        return;

                    case 0x16:
                        if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_TRANS_REGISTER_INFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANS_CONTEXT_OF_TRANS_REGISTER_INFO) ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_TRANS_REGISTER_INFO.CLASS_ID);
                        }
                        return;
                }
            }
            if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
        }

        public TdrError.ErrorType unpack(long selector, ref TdrReadBuf srcBuf, uint cutVer)
        {
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.unpack(ref srcBuf, cutVer);
            }
            type = srcBuf.readUInt8(ref this.bReserved);
            if (type != TdrError.ErrorType.TDR_NO_ERROR)
            {
                return type;
            }
            return type;
        }

        public TdrError.ErrorType unpack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if ((buffer.GetLength(0) == 0) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = ClassObjPool<TdrReadBuf>.Get();
            srcBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.unpack(selector, ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            srcBuf.Release();
            return type;
        }

        public COMDT_TRANS_CONTEXT_OF_ADD_ARENAFIGHT_HISTORY stAddArenaFightHistory
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_ADD_ARENAFIGHT_HISTORY);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_ADD_FIGHTHISTORY stAddFightHistory
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_ADD_FIGHTHISTORY);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_ADD_RANKCURSEASONDATA stAddRankCurSeasonData
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_ADD_RANKCURSEASONDATA);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA stAddRankPastSeasonData
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_APPOINT_POSITION stAppointPosition
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_APPOINT_POSITION);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD stApproveJoinGuild
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_CHANGE_GUILD_NAME stChangeGuildName
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_CHANGE_GUILD_NAME);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_CHANGE_PLAYER_NAME stChangePlayerName
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_CHANGE_PLAYER_NAME);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_RANK stClassOfRank
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_RANK);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_DEAL_GUILD_INVITE stDealGuildInvite
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_DEAL_GUILD_INVITE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_DELETEBURNINGENEMY stDeleteBurningEnemy
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_DELETEBURNINGENEMY);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_FIRE_MEMBER stFireMember
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_FIRE_MEMBER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_GETARENA_TARGETDATA stGetArenaTargetData
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_GETARENA_TARGETDATA);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_GUILD_GETNAME stGetGuildName
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_GUILD_GETNAME);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_GUILD_INVITE stGuildInvite
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_GUILD_INVITE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_GUILD_RECOMMEND stGuildRecommend
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_GUILD_RECOMMEND);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_JOIN_GUILD stJoinGuild
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_JOIN_GUILD);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_LUCKYDRAW stLuckyDraw
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_LUCKYDRAW);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_ONLINECHK stOnLineChk
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_ONLINECHK);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_QUIT_GUILD stQuitGuild
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_QUIT_GUILD);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_SEARCH_GUILD stSearchGuild
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_SEARCH_GUILD);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_TRANS_BASE_INFO stTransAcntBaseInfo
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_TRANS_BASE_INFO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANS_CONTEXT_OF_TRANS_REGISTER_INFO stTransAcntRegisterInfo
        {
            get
            {
                return (this.dataObject as COMDT_TRANS_CONTEXT_OF_TRANS_REGISTER_INFO);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

