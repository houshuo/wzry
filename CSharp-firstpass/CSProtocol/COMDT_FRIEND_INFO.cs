namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_FRIEND_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bGender;
        public byte bGuildState;
        public byte bIsOnline;
        public byte bPrivilege;
        public static readonly int CLASS_ID = 0xe0;
        public static readonly uint CURRVERSION = 0x63;
        public uint dwHeadID;
        public uint dwLastLoginTime;
        public uint dwLevel;
        public uint dwPvpLvl;
        public uint dwQQVIPMask;
        public uint dwRankClass;
        public uint dwRefuseFriendBits;
        public uint dwVipLvl;
        public uint dwVipScore;
        public static readonly uint LENGTH_szHeadUrl = 0x100;
        public static readonly uint LENGTH_szOpenId = 0x40;
        public static readonly uint LENGTH_szUserName = 0x40;
        public uint[] RankVal = new uint[0x20];
        public COMDT_GAME_VIP_CLIENT stGameVip = ((COMDT_GAME_VIP_CLIENT) ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID));
        public COMDT_ACNT_UNIQ stUin = ((COMDT_ACNT_UNIQ) ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID));
        public byte[] szHeadUrl = new byte[0x100];
        public byte[] szOpenId = new byte[0x40];
        public byte[] szUserName = new byte[0x40];
        public static readonly uint VERSION_bGender = 70;
        public static readonly uint VERSION_bPrivilege = 0x3f;
        public static readonly uint VERSION_dwQQVIPMask = 0x11;
        public static readonly uint VERSION_dwRefuseFriendBits = 0x63;
        public static readonly uint VERSION_dwVipScore = 0x22;
        public static readonly uint VERSION_RankVal = 0x20;
        public static readonly uint VERSION_stGameVip = 0x2a;
        public static readonly uint VERSION_szHeadUrl = 3;
        public static readonly uint VERSION_szOpenId = 0x2f;

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
            if (this.stUin != null)
            {
                this.stUin.Release();
                this.stUin = null;
            }
            this.dwLevel = 0;
            this.dwVipLvl = 0;
            this.bIsOnline = 0;
            this.dwLastLoginTime = 0;
            this.dwHeadID = 0;
            this.bGuildState = 0;
            this.dwPvpLvl = 0;
            this.dwRankClass = 0;
            this.dwRefuseFriendBits = 0;
            this.dwQQVIPMask = 0;
            this.dwVipScore = 0;
            if (this.stGameVip != null)
            {
                this.stGameVip.Release();
                this.stGameVip = null;
            }
            this.bPrivilege = 0;
            this.bGender = 0;
        }

        public override void OnUse()
        {
            this.stUin = (COMDT_ACNT_UNIQ) ProtocolObjectPool.Get(COMDT_ACNT_UNIQ.CLASS_ID);
            this.stGameVip = (COMDT_GAME_VIP_CLIENT) ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
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
            type = this.stUin.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int pos = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = destBuf.getUsedSize();
                int count = TdrTypeUtil.cstrlen(this.szUserName);
                if (count >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szUserName, count);
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
                type = destBuf.writeUInt32(this.dwLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwVipLvl);
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
                type = destBuf.writeUInt32(this.dwHeadID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bGuildState);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwPvpLvl);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwRankClass);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwRefuseFriendBits <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwRefuseFriendBits);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_szHeadUrl <= cutVer)
                {
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
                }
                if (VERSION_dwQQVIPMask <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwQQVIPMask);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_RankVal <= cutVer)
                {
                    for (int i = 0; i < 0x20; i++)
                    {
                        type = destBuf.writeUInt32(this.RankVal[i]);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                }
                if (VERSION_dwVipScore <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwVipScore);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stGameVip <= cutVer)
                {
                    type = this.stGameVip.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_szOpenId <= cutVer)
                {
                    int num10 = destBuf.getUsedSize();
                    type = destBuf.reserve(4);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    int num11 = destBuf.getUsedSize();
                    int num12 = TdrTypeUtil.cstrlen(this.szOpenId);
                    if (num12 >= 0x40)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    type = destBuf.writeCString(this.szOpenId, num12);
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
            type = this.stUin.unpack(ref srcBuf, cutVer);
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
                if (dest > this.szUserName.GetLength(0))
                {
                    if (dest > LENGTH_szUserName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szUserName = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szUserName, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szUserName[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szUserName) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwVipLvl);
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
                type = srcBuf.readUInt32(ref this.dwHeadID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bGuildState);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPvpLvl);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwRankClass);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwRefuseFriendBits <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwRefuseFriendBits);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwRefuseFriendBits = 0;
                }
                if (VERSION_szHeadUrl <= cutVer)
                {
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
                if (VERSION_RankVal <= cutVer)
                {
                    for (int i = 0; i < 0x20; i++)
                    {
                        type = srcBuf.readUInt32(ref this.RankVal[i]);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                }
                if (VERSION_dwVipScore <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwVipScore);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwVipScore = 0;
                }
                if (VERSION_stGameVip <= cutVer)
                {
                    type = this.stGameVip.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    type = this.stGameVip.construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_szOpenId <= cutVer)
                {
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
                    if (num6 > this.szOpenId.GetLength(0))
                    {
                        if (num6 > LENGTH_szOpenId)
                        {
                            return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                        }
                        this.szOpenId = new byte[num6];
                    }
                    if (1 > num6)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                    }
                    type = srcBuf.readCString(ref this.szOpenId, (int) num6);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    if (this.szOpenId[((int) num6) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num7 = TdrTypeUtil.cstrlen(this.szOpenId) + 1;
                    if (num6 != num7)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
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
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.bGender = 0;
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

