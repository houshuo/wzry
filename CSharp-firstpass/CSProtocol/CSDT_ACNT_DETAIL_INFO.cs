namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_ACNT_DETAIL_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bGender;
        public byte bGradeOfRank;
        public byte bIsOnline;
        public byte bMaxGradeOfRank;
        public byte bPrivilege;
        public static readonly int CLASS_ID = 0x3f4;
        public static readonly uint CURRVERSION = 0x88;
        public uint dwCreditValue;
        public uint dwCurClassOfRank;
        public uint dwExp;
        public uint dwLastLoginTime;
        public uint dwLevel;
        public uint dwPower;
        public uint dwPvpExp;
        public uint dwPvpLevel;
        public uint dwQQVIPMask;
        public int iLogicWorldId;
        public static readonly uint LENGTH_szAcntName = 0x40;
        public static readonly uint LENGTH_szOpenID = 0x40;
        public static readonly uint LENGTH_szOpenUrl = 0x100;
        public static readonly uint LENGTH_szSignatureInfo = 0x80;
        public COMDT_ACNT_BANTIME stBanTime = ((COMDT_ACNT_BANTIME) ProtocolObjectPool.Get(COMDT_ACNT_BANTIME.CLASS_ID));
        public COMDT_GAME_VIP_CLIENT stGameVip = ((COMDT_GAME_VIP_CLIENT) ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID));
        public COMDT_ACNT_GUILD_INFO stGuildInfo = ((COMDT_ACNT_GUILD_INFO) ProtocolObjectPool.Get(COMDT_ACNT_GUILD_INFO.CLASS_ID));
        public COMDT_ACNT_HONORINFO stHonorInfo = ((COMDT_ACNT_HONORINFO) ProtocolObjectPool.Get(COMDT_ACNT_HONORINFO.CLASS_ID));
        public COMDT_LIKE_NUMS stLikeNum = ((COMDT_LIKE_NUMS) ProtocolObjectPool.Get(COMDT_LIKE_NUMS.CLASS_ID));
        public COMDT_MOST_USED_HERO_DETAIL stMostUsedHero = ((COMDT_MOST_USED_HERO_DETAIL) ProtocolObjectPool.Get(COMDT_MOST_USED_HERO_DETAIL.CLASS_ID));
        public COMDT_RANKDETAIL stRankInfo = ((COMDT_RANKDETAIL) ProtocolObjectPool.Get(COMDT_RANKDETAIL.CLASS_ID));
        public CSDT_PVPDETAIL_INFO stStatistic = ((CSDT_PVPDETAIL_INFO) ProtocolObjectPool.Get(CSDT_PVPDETAIL_INFO.CLASS_ID));
        public byte[] szAcntName = new byte[0x40];
        public byte[] szOpenID = new byte[0x40];
        public byte[] szOpenUrl = new byte[0x100];
        public byte[] szSignatureInfo = new byte[0x80];
        public ulong ullUid;
        public static readonly uint VERSION_bGender = 70;
        public static readonly uint VERSION_bPrivilege = 0x3e;
        public static readonly uint VERSION_dwQQVIPMask = 0x31;
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
            this.ullUid = 0L;
            this.iLogicWorldId = 0;
            this.bIsOnline = 0;
            this.dwLastLoginTime = 0;
            this.dwLevel = 0;
            this.dwExp = 0;
            this.dwPower = 0;
            this.dwPvpLevel = 0;
            this.dwPvpExp = 0;
            this.bGradeOfRank = 0;
            this.bMaxGradeOfRank = 0;
            if (this.stGameVip != null)
            {
                this.stGameVip.Release();
                this.stGameVip = null;
            }
            if (this.stStatistic != null)
            {
                this.stStatistic.Release();
                this.stStatistic = null;
            }
            if (this.stGuildInfo != null)
            {
                this.stGuildInfo.Release();
                this.stGuildInfo = null;
            }
            if (this.stBanTime != null)
            {
                this.stBanTime.Release();
                this.stBanTime = null;
            }
            if (this.stMostUsedHero != null)
            {
                this.stMostUsedHero.Release();
                this.stMostUsedHero = null;
            }
            this.dwQQVIPMask = 0;
            this.bPrivilege = 0;
            this.bGender = 0;
            if (this.stHonorInfo != null)
            {
                this.stHonorInfo.Release();
                this.stHonorInfo = null;
            }
            this.dwCreditValue = 0;
            if (this.stRankInfo != null)
            {
                this.stRankInfo.Release();
                this.stRankInfo = null;
            }
            this.dwCurClassOfRank = 0;
            if (this.stLikeNum != null)
            {
                this.stLikeNum.Release();
                this.stLikeNum = null;
            }
        }

        public override void OnUse()
        {
            this.stGameVip = (COMDT_GAME_VIP_CLIENT) ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
            this.stStatistic = (CSDT_PVPDETAIL_INFO) ProtocolObjectPool.Get(CSDT_PVPDETAIL_INFO.CLASS_ID);
            this.stGuildInfo = (COMDT_ACNT_GUILD_INFO) ProtocolObjectPool.Get(COMDT_ACNT_GUILD_INFO.CLASS_ID);
            this.stBanTime = (COMDT_ACNT_BANTIME) ProtocolObjectPool.Get(COMDT_ACNT_BANTIME.CLASS_ID);
            this.stMostUsedHero = (COMDT_MOST_USED_HERO_DETAIL) ProtocolObjectPool.Get(COMDT_MOST_USED_HERO_DETAIL.CLASS_ID);
            this.stHonorInfo = (COMDT_ACNT_HONORINFO) ProtocolObjectPool.Get(COMDT_ACNT_HONORINFO.CLASS_ID);
            this.stRankInfo = (COMDT_RANKDETAIL) ProtocolObjectPool.Get(COMDT_RANKDETAIL.CLASS_ID);
            this.stLikeNum = (COMDT_LIKE_NUMS) ProtocolObjectPool.Get(COMDT_LIKE_NUMS.CLASS_ID);
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
            type = destBuf.writeUInt64(this.ullUid);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int pos = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = destBuf.getUsedSize();
                int count = TdrTypeUtil.cstrlen(this.szAcntName);
                if (count >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szAcntName, count);
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
                int num5 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num6 = destBuf.getUsedSize();
                int num7 = TdrTypeUtil.cstrlen(this.szOpenID);
                if (num7 >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szOpenID, num7);
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
                type = destBuf.writeInt32(this.iLogicWorldId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bIsOnline);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwLastLoginTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num9 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num10 = destBuf.getUsedSize();
                int num11 = TdrTypeUtil.cstrlen(this.szOpenUrl);
                if (num11 >= 0x100)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szOpenUrl, num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num12 = destBuf.getUsedSize() - num10;
                type = destBuf.writeUInt32((uint) num12, num9);
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
                type = destBuf.writeUInt32(this.dwPower);
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
                type = destBuf.writeUInt8(this.bGradeOfRank);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bMaxGradeOfRank);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGameVip.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stStatistic.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGuildInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBanTime.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMostUsedHero.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwQQVIPMask <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwQQVIPMask);
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
                if (VERSION_bGender <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bGender);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stHonorInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwCreditValue);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stRankInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwCurClassOfRank);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stLikeNum <= cutVer)
                {
                    type = this.stLikeNum.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                int num13 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num14 = destBuf.getUsedSize();
                int num15 = TdrTypeUtil.cstrlen(this.szSignatureInfo);
                if (num15 >= 0x80)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szSignatureInfo, num15);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num16 = destBuf.getUsedSize() - num14;
                type = destBuf.writeUInt32((uint) num16, num13);
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
            type = srcBuf.readUInt64(ref this.ullUid);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
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
                if (dest > this.szAcntName.GetLength(0))
                {
                    if (dest > LENGTH_szAcntName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szAcntName = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szAcntName, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szAcntName[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szAcntName) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
                if (num3 > this.szOpenID.GetLength(0))
                {
                    if (num3 > LENGTH_szOpenID)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szOpenID = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szOpenID, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szOpenID[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szOpenID) + 1;
                if (num3 != num4)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readInt32(ref this.iLogicWorldId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsOnline);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwLastLoginTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num5 = 0;
                type = srcBuf.readUInt32(ref num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num5 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num5 > this.szOpenUrl.GetLength(0))
                {
                    if (num5 > LENGTH_szOpenUrl)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szOpenUrl = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szOpenUrl, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szOpenUrl[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szOpenUrl) + 1;
                if (num5 != num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
                type = srcBuf.readUInt32(ref this.dwPower);
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
                type = srcBuf.readUInt8(ref this.bGradeOfRank);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bMaxGradeOfRank);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGameVip.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stStatistic.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGuildInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBanTime.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMostUsedHero.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwQQVIPMask <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwQQVIPMask);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwQQVIPMask = 0;
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
                type = this.stHonorInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCreditValue);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stRankInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCurClassOfRank);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
                uint num7 = 0;
                type = srcBuf.readUInt32(ref num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num7 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num7 > this.szSignatureInfo.GetLength(0))
                {
                    if (num7 > LENGTH_szSignatureInfo)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSignatureInfo = new byte[num7];
                }
                if (1 > num7)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSignatureInfo, (int) num7);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szSignatureInfo[((int) num7) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num8 = TdrTypeUtil.cstrlen(this.szSignatureInfo) + 1;
                    if (num7 != num8)
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

