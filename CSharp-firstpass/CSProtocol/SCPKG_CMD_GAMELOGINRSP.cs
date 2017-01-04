namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_CMD_GAMELOGINRSP : ProtocolObject
    {
        public byte bAcntGM;
        public byte bAcntNewbieType;
        public uint[] BanTime = new uint[100];
        public static readonly uint BASEVERSION = 1;
        public byte bGender;
        public byte bGetCoinNums;
        public byte bGradeOfRank;
        public byte bIsInBatInputAllowed;
        public byte bIsSucc;
        public byte bIsVisitorSvr;
        public byte bPlatChannelOpen;
        public byte bPrivilege;
        public static readonly int CLASS_ID = 0x23f;
        public static readonly uint CURRVERSION = 0x88;
        public uint dwApolloEnvFlag;
        public uint dwChgNameCnt;
        public uint dwClassOfRank;
        public uint dwCreditValue;
        public uint dwCurActionPoint;
        public uint dwExp;
        public uint dwFirstLoginTime;
        public uint dwFirstWinPvpLvl;
        public uint dwGameAcntObjID;
        public uint dwHeadId;
        public uint dwHeroPoolExp;
        public uint dwLastLoginTime;
        public uint dwLevel;
        public uint dwMaxActionPoint;
        public uint dwNextFirstWinSec;
        public uint dwPvpExp;
        public uint dwPvpLevel;
        public uint dwQQVipInfo;
        public uint dwRefuseFriendBits;
        public uint dwServerCurTimeMs;
        public uint dwServerCurTimeSec;
        public uint dwSkillPoint;
        public uint dwSPUpdTimeSec;
        public uint dwTitleId;
        public static readonly uint LENGTH_szHeadUrl = 0x100;
        public static readonly uint LENGTH_szName = 0x40;
        public static readonly uint LENGTH_szSignatureInfo = 0x80;
        public COMDT_ACNT_ACTIVITY_INFO stActivityInfo = ((COMDT_ACNT_ACTIVITY_INFO) ProtocolObjectPool.Get(COMDT_ACNT_ACTIVITY_INFO.CLASS_ID));
        public CSDT_ACNT_ARENADATA stArenaData = ((CSDT_ACNT_ARENADATA) ProtocolObjectPool.Get(CSDT_ACNT_ARENADATA.CLASS_ID));
        public COMDT_CLIENT_BITS stClientBits = ((COMDT_CLIENT_BITS) ProtocolObjectPool.Get(COMDT_CLIENT_BITS.CLASS_ID));
        public COMDT_COIN_LIST stCoinList = ((COMDT_COIN_LIST) ProtocolObjectPool.Get(COMDT_COIN_LIST.CLASS_ID));
        public COMDT_ACNT_GUILD_INFO stGuildBaseInfo = ((COMDT_ACNT_GUILD_INFO) ProtocolObjectPool.Get(COMDT_ACNT_GUILD_INFO.CLASS_ID));
        public COMDT_ACNT_GUILD_EXT_INFO stGuildExtInfo = ((COMDT_ACNT_GUILD_EXT_INFO) ProtocolObjectPool.Get(COMDT_ACNT_GUILD_EXT_INFO.CLASS_ID));
        public COMDT_ACNT_HEADIMG_LIST stHeadImage = ((COMDT_ACNT_HEADIMG_LIST) ProtocolObjectPool.Get(COMDT_ACNT_HEADIMG_LIST.CLASS_ID));
        public COMDT_INBATTLE_NEWBIE_BITS_DETAIL stInBattleNewbieBits = ((COMDT_INBATTLE_NEWBIE_BITS_DETAIL) ProtocolObjectPool.Get(COMDT_INBATTLE_NEWBIE_BITS_DETAIL.CLASS_ID));
        public COMDT_ACNT_LICENSE stLicense = ((COMDT_ACNT_LICENSE) ProtocolObjectPool.Get(COMDT_ACNT_LICENSE.CLASS_ID));
        public COMDT_LIKE_NUMS stLikeNum = ((COMDT_LIKE_NUMS) ProtocolObjectPool.Get(COMDT_LIKE_NUMS.CLASS_ID));
        public COMDT_ACNT_TASKINFO stLoginTaskInfo = ((COMDT_ACNT_TASKINFO) ProtocolObjectPool.Get(COMDT_ACNT_TASKINFO.CLASS_ID));
        public COMDT_MONTH_WEEK_CARD_INFO stMonthWeekCardInfo = ((COMDT_MONTH_WEEK_CARD_INFO) ProtocolObjectPool.Get(COMDT_MONTH_WEEK_CARD_INFO.CLASS_ID));
        public COMDT_MOST_USED_HERO_DETAIL stMostUsedHero = ((COMDT_MOST_USED_HERO_DETAIL) ProtocolObjectPool.Get(COMDT_MOST_USED_HERO_DETAIL.CLASS_ID));
        public COMDT_NEWBIE_STATUS_BITS stNewbieBits = ((COMDT_NEWBIE_STATUS_BITS) ProtocolObjectPool.Get(COMDT_NEWBIE_STATUS_BITS.CLASS_ID));
        public COMDT_NEWCLIENT_BITS stNewCltBits = ((COMDT_NEWCLIENT_BITS) ProtocolObjectPool.Get(COMDT_NEWCLIENT_BITS.CLASS_ID));
        public COMDT_HERO_PRESENT_LIMIT stPresentLimit = ((COMDT_HERO_PRESENT_LIMIT) ProtocolObjectPool.Get(COMDT_HERO_PRESENT_LIMIT.CLASS_ID));
        public COMDT_PROP_MULTIPLE stPropMultiple = ((COMDT_PROP_MULTIPLE) ProtocolObjectPool.Get(COMDT_PROP_MULTIPLE.CLASS_ID));
        public COMDT_ACNT_LEVEL_COMPLETE_DETAIL stPveProgress = ((COMDT_ACNT_LEVEL_COMPLETE_DETAIL) ProtocolObjectPool.Get(COMDT_ACNT_LEVEL_COMPLETE_DETAIL.CLASS_ID));
        public COMDT_SELFDEFINE_CHATINFO stSelfDefineChatInfo = ((COMDT_SELFDEFINE_CHATINFO) ProtocolObjectPool.Get(COMDT_SELFDEFINE_CHATINFO.CLASS_ID));
        public CSDT_ACNT_SHOPBUY_INFO stShopBuyRcd = ((CSDT_ACNT_SHOPBUY_INFO) ProtocolObjectPool.Get(CSDT_ACNT_SHOPBUY_INFO.CLASS_ID));
        public byte[] szHeadUrl = new byte[0x100];
        public byte[] szName = new byte[0x40];
        public byte[] szSignatureInfo = new byte[0x80];
        public ulong ullFuncUnlockFlag;
        public ulong ullGameAcntUid;
        public ulong ullLevelRewardFlag;
        public static readonly uint VERSION_bGender = 70;
        public static readonly uint VERSION_bGetCoinNums = 0x7d;
        public static readonly uint VERSION_bPrivilege = 0x3e;
        public static readonly uint VERSION_dwChgNameCnt = 0x35;
        public static readonly uint VERSION_dwFirstWinPvpLvl = 0x45;
        public static readonly uint VERSION_dwLastLoginTime = 0x31;
        public static readonly uint VERSION_dwNextFirstWinSec = 0x45;
        public static readonly uint VERSION_stInBattleNewbieBits = 0x41;
        public static readonly uint VERSION_stLikeNum = 0x7e;

        public override TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            this.bIsSucc = 0;
            this.dwGameAcntObjID = 0;
            this.ullGameAcntUid = 0L;
            this.dwLevel = 0;
            this.dwExp = 0;
            this.dwPvpLevel = 0;
            this.dwPvpExp = 0;
            this.dwMaxActionPoint = 0;
            this.dwCurActionPoint = 0;
            this.dwTitleId = 0;
            this.dwHeadId = 0;
            this.dwSkillPoint = 0;
            if (this.stLoginTaskInfo != null)
            {
                this.stLoginTaskInfo.Release();
                this.stLoginTaskInfo = null;
            }
            if (this.stPveProgress != null)
            {
                this.stPveProgress.Release();
                this.stPveProgress = null;
            }
            if (this.stNewbieBits != null)
            {
                this.stNewbieBits.Release();
                this.stNewbieBits = null;
            }
            if (this.stClientBits != null)
            {
                this.stClientBits.Release();
                this.stClientBits = null;
            }
            if (this.stNewCltBits != null)
            {
                this.stNewCltBits.Release();
                this.stNewCltBits = null;
            }
            if (this.stShopBuyRcd != null)
            {
                this.stShopBuyRcd.Release();
                this.stShopBuyRcd = null;
            }
            this.dwServerCurTimeSec = 0;
            this.dwServerCurTimeMs = 0;
            this.dwSPUpdTimeSec = 0;
            this.bGradeOfRank = 0;
            this.dwClassOfRank = 0;
            if (this.stActivityInfo != null)
            {
                this.stActivityInfo.Release();
                this.stActivityInfo = null;
            }
        }

        public override void OnUse()
        {
            this.stLoginTaskInfo = (COMDT_ACNT_TASKINFO) ProtocolObjectPool.Get(COMDT_ACNT_TASKINFO.CLASS_ID);
            this.stPveProgress = (COMDT_ACNT_LEVEL_COMPLETE_DETAIL) ProtocolObjectPool.Get(COMDT_ACNT_LEVEL_COMPLETE_DETAIL.CLASS_ID);
            this.stNewbieBits = (COMDT_NEWBIE_STATUS_BITS) ProtocolObjectPool.Get(COMDT_NEWBIE_STATUS_BITS.CLASS_ID);
            this.stClientBits = (COMDT_CLIENT_BITS) ProtocolObjectPool.Get(COMDT_CLIENT_BITS.CLASS_ID);
            this.stNewCltBits = (COMDT_NEWCLIENT_BITS) ProtocolObjectPool.Get(COMDT_NEWCLIENT_BITS.CLASS_ID);
            this.stShopBuyRcd = (CSDT_ACNT_SHOPBUY_INFO) ProtocolObjectPool.Get(CSDT_ACNT_SHOPBUY_INFO.CLASS_ID);
            this.stActivityInfo = (COMDT_ACNT_ACTIVITY_INFO) ProtocolObjectPool.Get(COMDT_ACNT_ACTIVITY_INFO.CLASS_ID);
        }

        public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = destBuf.writeUInt8(this.bIsSucc);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwGameAcntObjID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt64(this.ullGameAcntUid);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwExp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwPvpLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwPvpExp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwMaxActionPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwCurActionPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwTitleId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwHeadId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwSkillPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int pos = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = destBuf.getUsedSize();
                int count = TdrTypeUtil.cstrlen(this.szName);
                if (count >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szName, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = destBuf.getUsedSize() - num2;
                type = destBuf.writeUInt32((uint) num4, pos);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stLoginTaskInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stPveProgress.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stNewbieBits.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stClientBits.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stNewCltBits.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stShopBuyRcd.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwServerCurTimeSec);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwServerCurTimeMs);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwSPUpdTimeSec);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bGradeOfRank);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwClassOfRank);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stActivityInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt64(this.ullFuncUnlockFlag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGuildBaseInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGuildExtInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bAcntGM);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stArenaData.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwHeroPoolExp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stCoinList.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num6 = destBuf.getUsedSize();
                int num7 = TdrTypeUtil.cstrlen(this.szHeadUrl);
                if (num7 >= 0x100)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szHeadUrl, num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = destBuf.getUsedSize() - num6;
                type = destBuf.writeUInt32((uint) num8, num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bIsVisitorSvr);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bAcntNewbieType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 100; i++)
                {
                    type = destBuf.writeUInt32(this.BanTime[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwFirstLoginTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stPropMultiple.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwApolloEnvFlag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMostUsedHero.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwQQVipInfo);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwLastLoginTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastLoginTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwChgNameCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwChgNameCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bPrivilege <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bPrivilege);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stInBattleNewbieBits <= cutVer)
                {
                    type = this.stInBattleNewbieBits.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwNextFirstWinSec <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwNextFirstWinSec);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwFirstWinPvpLvl <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwFirstWinPvpLvl);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bGender <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bGender);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stLicense.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMonthWeekCardInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeadImage.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt64(this.ullLevelRewardFlag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwRefuseFriendBits);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwCreditValue);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stPresentLimit.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bIsInBatInputAllowed);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bPlatChannelOpen);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSelfDefineChatInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_bGetCoinNums <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bGetCoinNums);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stLikeNum <= cutVer)
                {
                    type = this.stLikeNum.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                int num10 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num11 = destBuf.getUsedSize();
                int num12 = TdrTypeUtil.cstrlen(this.szSignatureInfo);
                if (num12 >= 0x80)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szSignatureInfo, num12);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num13 = destBuf.getUsedSize() - num11;
                type = destBuf.writeUInt32((uint) num13, num10);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrWriteBuf destBuf = ClassObjPool<TdrWriteBuf>.Get();
            destBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();
                usedSize = destBuf.getUsedSize();
            }
            destBuf.Release();
            return type;
        }

        public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = srcBuf.readUInt8(ref this.bIsSucc);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwGameAcntObjID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt64(ref this.ullGameAcntUid);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwExp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPvpLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPvpExp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMaxActionPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCurActionPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTitleId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwHeadId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSkillPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint dest = 0;
                type = srcBuf.readUInt32(ref dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (dest > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (dest > this.szName.GetLength(0))
                {
                    if (dest > LENGTH_szName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szName = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szName, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szName[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szName) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = this.stLoginTaskInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stPveProgress.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stNewbieBits.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stClientBits.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stNewCltBits.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stShopBuyRcd.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwServerCurTimeSec);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwServerCurTimeMs);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSPUpdTimeSec);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bGradeOfRank);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwClassOfRank);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stActivityInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt64(ref this.ullFuncUnlockFlag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGuildBaseInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGuildExtInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bAcntGM);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stArenaData.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwHeroPoolExp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stCoinList.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num3 = 0;
                type = srcBuf.readUInt32(ref num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num3 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num3 > this.szHeadUrl.GetLength(0))
                {
                    if (num3 > LENGTH_szHeadUrl)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szHeadUrl = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szHeadUrl, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szHeadUrl[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szHeadUrl) + 1;
                if (num3 != num4)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bIsVisitorSvr);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bAcntNewbieType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 100; i++)
                {
                    type = srcBuf.readUInt32(ref this.BanTime[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwFirstLoginTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stPropMultiple.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwApolloEnvFlag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMostUsedHero.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwQQVipInfo);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwLastLoginTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastLoginTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastLoginTime = 0;
                }
                if (VERSION_dwChgNameCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwChgNameCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwChgNameCnt = 0;
                }
                if (VERSION_bPrivilege <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bPrivilege);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bPrivilege = 0;
                }
                if (VERSION_stInBattleNewbieBits <= cutVer)
                {
                    type = this.stInBattleNewbieBits.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    type = this.stInBattleNewbieBits.construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwNextFirstWinSec <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwNextFirstWinSec);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwNextFirstWinSec = 0;
                }
                if (VERSION_dwFirstWinPvpLvl <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwFirstWinPvpLvl);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwFirstWinPvpLvl = 0;
                }
                if (VERSION_bGender <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bGender);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bGender = 0;
                }
                type = this.stLicense.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMonthWeekCardInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeadImage.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt64(ref this.ullLevelRewardFlag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwRefuseFriendBits);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCreditValue);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stPresentLimit.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsInBatInputAllowed);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bPlatChannelOpen);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSelfDefineChatInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_bGetCoinNums <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bGetCoinNums);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bGetCoinNums = 0;
                }
                if (VERSION_stLikeNum <= cutVer)
                {
                    type = this.stLikeNum.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    type = this.stLikeNum.construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                uint num6 = 0;
                type = srcBuf.readUInt32(ref num6);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num6 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num6 > this.szSignatureInfo.GetLength(0))
                {
                    if (num6 > LENGTH_szSignatureInfo)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSignatureInfo = new byte[num6];
                }
                if (1 > num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSignatureInfo, (int) num6);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szSignatureInfo[((int) num6) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num7 = TdrTypeUtil.cstrlen(this.szSignatureInfo) + 1;
                    if (num6 != num7)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                }
            }
            return type;
        }

        public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = ClassObjPool<TdrReadBuf>.Get();
            srcBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            srcBuf.Release();
            return type;
        }
    }
}

