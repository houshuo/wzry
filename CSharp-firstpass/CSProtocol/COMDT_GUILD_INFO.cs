namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_GUILD_INFO : ProtocolObject
    {
        public COMDT_GUILD_BUILDING[] astBuildingInfo;
        public COMDT_GUILD_MEMBER_INFO[] astMemberInfo = new COMDT_GUILD_MEMBER_INFO[150];
        public COMDT_SELF_RECOMMEND_INFO[] astSelfRecommendInfo;
        public static readonly uint BASEVERSION = 1;
        public byte bBuildingCnt;
        public byte bSelfRecommendCnt;
        public static readonly int CLASS_ID = 0x13f;
        public static readonly uint CURRVERSION = 0x84;
        public uint dwActive;
        public uint dwChangeNameCnt;
        public uint dwCoinPool;
        public uint dwElectionTime;
        public uint dwGroupGuildID;
        public uint dwGuildMoney;
        public uint dwMaintainTime;
        public uint dwStar;
        public static readonly uint LENGTH_szGroupOpenID = 0x80;
        public COMDT_GUILD_BRIEF_INFO stBriefInfo = ((COMDT_GUILD_BRIEF_INFO) ProtocolObjectPool.Get(COMDT_GUILD_BRIEF_INFO.CLASS_ID));
        public COMDT_GUILD_RANK_INFO stRankInfo;
        public byte[] szGroupOpenID;
        public ulong ullBuildTime;
        public static readonly uint VERSION_dwChangeNameCnt = 0x3b;
        public static readonly uint VERSION_dwGroupGuildID = 0x56;
        public static readonly uint VERSION_dwStar = 0x52;
        public static readonly uint VERSION_szGroupOpenID = 0x60;

        public COMDT_GUILD_INFO()
        {
            for (int i = 0; i < 150; i++)
            {
                this.astMemberInfo[i] = (COMDT_GUILD_MEMBER_INFO) ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_INFO.CLASS_ID);
            }
            this.astBuildingInfo = new COMDT_GUILD_BUILDING[10];
            for (int j = 0; j < 10; j++)
            {
                this.astBuildingInfo[j] = (COMDT_GUILD_BUILDING) ProtocolObjectPool.Get(COMDT_GUILD_BUILDING.CLASS_ID);
            }
            this.astSelfRecommendInfo = new COMDT_SELF_RECOMMEND_INFO[2];
            for (int k = 0; k < 2; k++)
            {
                this.astSelfRecommendInfo[k] = (COMDT_SELF_RECOMMEND_INFO) ProtocolObjectPool.Get(COMDT_SELF_RECOMMEND_INFO.CLASS_ID);
            }
            this.stRankInfo = (COMDT_GUILD_RANK_INFO) ProtocolObjectPool.Get(COMDT_GUILD_RANK_INFO.CLASS_ID);
            this.szGroupOpenID = new byte[0x80];
        }

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
            if (this.stBriefInfo != null)
            {
                this.stBriefInfo.Release();
                this.stBriefInfo = null;
            }
            this.ullBuildTime = 0L;
            this.dwGuildMoney = 0;
            this.dwActive = 0;
            this.dwCoinPool = 0;
            this.dwMaintainTime = 0;
            this.dwElectionTime = 0;
            if (this.astMemberInfo != null)
            {
                for (int i = 0; i < this.astMemberInfo.Length; i++)
                {
                    if (this.astMemberInfo[i] != null)
                    {
                        this.astMemberInfo[i].Release();
                        this.astMemberInfo[i] = null;
                    }
                }
            }
            this.bBuildingCnt = 0;
            if (this.astBuildingInfo != null)
            {
                for (int j = 0; j < this.astBuildingInfo.Length; j++)
                {
                    if (this.astBuildingInfo[j] != null)
                    {
                        this.astBuildingInfo[j].Release();
                        this.astBuildingInfo[j] = null;
                    }
                }
            }
            this.bSelfRecommendCnt = 0;
            if (this.astSelfRecommendInfo != null)
            {
                for (int k = 0; k < this.astSelfRecommendInfo.Length; k++)
                {
                    if (this.astSelfRecommendInfo[k] != null)
                    {
                        this.astSelfRecommendInfo[k].Release();
                        this.astSelfRecommendInfo[k] = null;
                    }
                }
            }
            if (this.stRankInfo != null)
            {
                this.stRankInfo.Release();
                this.stRankInfo = null;
            }
            this.dwChangeNameCnt = 0;
            this.dwStar = 0;
            this.dwGroupGuildID = 0;
        }

        public override void OnUse()
        {
            this.stBriefInfo = (COMDT_GUILD_BRIEF_INFO) ProtocolObjectPool.Get(COMDT_GUILD_BRIEF_INFO.CLASS_ID);
            if (this.astMemberInfo != null)
            {
                for (int i = 0; i < this.astMemberInfo.Length; i++)
                {
                    this.astMemberInfo[i] = (COMDT_GUILD_MEMBER_INFO) ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_INFO.CLASS_ID);
                }
            }
            if (this.astBuildingInfo != null)
            {
                for (int j = 0; j < this.astBuildingInfo.Length; j++)
                {
                    this.astBuildingInfo[j] = (COMDT_GUILD_BUILDING) ProtocolObjectPool.Get(COMDT_GUILD_BUILDING.CLASS_ID);
                }
            }
            if (this.astSelfRecommendInfo != null)
            {
                for (int k = 0; k < this.astSelfRecommendInfo.Length; k++)
                {
                    this.astSelfRecommendInfo[k] = (COMDT_SELF_RECOMMEND_INFO) ProtocolObjectPool.Get(COMDT_SELF_RECOMMEND_INFO.CLASS_ID);
                }
            }
            this.stRankInfo = (COMDT_GUILD_RANK_INFO) ProtocolObjectPool.Get(COMDT_GUILD_RANK_INFO.CLASS_ID);
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
            type = this.stBriefInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt64(this.ullBuildTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwGuildMoney);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwActive);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwCoinPool);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwMaintainTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwElectionTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (150 < this.stBriefInfo.bMemberNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astMemberInfo.Length < this.stBriefInfo.bMemberNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.stBriefInfo.bMemberNum; i++)
                {
                    type = this.astMemberInfo[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bBuildingCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.bBuildingCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astBuildingInfo.Length < this.bBuildingCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int j = 0; j < this.bBuildingCnt; j++)
                {
                    type = this.astBuildingInfo[j].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bSelfRecommendCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (2 < this.bSelfRecommendCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astSelfRecommendInfo.Length < this.bSelfRecommendCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int k = 0; k < this.bSelfRecommendCnt; k++)
                {
                    type = this.astSelfRecommendInfo[k].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stRankInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwChangeNameCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwChangeNameCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwStar <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwStar);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwGroupGuildID <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwGroupGuildID);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_szGroupOpenID > cutVer)
                {
                    return type;
                }
                int pos = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = destBuf.getUsedSize();
                int count = TdrTypeUtil.cstrlen(this.szGroupOpenID);
                if (count >= 0x80)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szGroupOpenID, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num7 = destBuf.getUsedSize() - num5;
                type = destBuf.writeUInt32((uint) num7, pos);
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
            type = this.stBriefInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt64(ref this.ullBuildTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwGuildMoney);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwActive);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCoinPool);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMaintainTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwElectionTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (150 < this.stBriefInfo.bMemberNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.stBriefInfo.bMemberNum; i++)
                {
                    type = this.astMemberInfo[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bBuildingCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.bBuildingCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int j = 0; j < this.bBuildingCnt; j++)
                {
                    type = this.astBuildingInfo[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bSelfRecommendCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (2 < this.bSelfRecommendCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int k = 0; k < this.bSelfRecommendCnt; k++)
                {
                    type = this.astSelfRecommendInfo[k].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stRankInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwChangeNameCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwChangeNameCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwChangeNameCnt = 0;
                }
                if (VERSION_dwStar <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwStar);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwStar = 0;
                }
                if (VERSION_dwGroupGuildID <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwGroupGuildID);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwGroupGuildID = 0;
                }
                if (VERSION_szGroupOpenID > cutVer)
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
                if (dest > this.szGroupOpenID.GetLength(0))
                {
                    if (dest > LENGTH_szGroupOpenID)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGroupOpenID = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGroupOpenID, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGroupOpenID[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num5 = TdrTypeUtil.cstrlen(this.szGroupOpenID) + 1;
                if (dest != num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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

