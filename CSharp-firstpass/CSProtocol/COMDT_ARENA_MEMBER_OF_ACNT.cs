namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ARENA_MEMBER_OF_ACNT : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x15f;
        public static readonly uint CURRVERSION = 0x43;
        public uint dwForceValue;
        public uint dwLevel;
        public uint dwPVPLevel;
        public static readonly uint LENGTH_szHeadUrl = 0x100;
        public static readonly uint LENGTH_szName = 0x40;
        public COMDT_ARENA_HEROINFO stBattleHero = ((COMDT_ARENA_HEROINFO) ProtocolObjectPool.Get(COMDT_ARENA_HEROINFO.CLASS_ID));
        public COMDT_GAME_VIP_CLIENT stVip = ((COMDT_GAME_VIP_CLIENT) ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID));
        public byte[] szHeadUrl = new byte[0x100];
        public byte[] szName = new byte[0x40];
        public ulong ullUid;
        public static readonly uint VERSION_stVip = 0x43;
        public static readonly uint VERSION_szHeadUrl = 4;

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.ullUid = 0L;
            this.dwLevel = 0;
            this.dwForceValue = 0;
            this.dwPVPLevel = 0;
            type = this.stBattleHero.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stVip.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            this.ullUid = 0L;
            this.dwLevel = 0;
            this.dwForceValue = 0;
            this.dwPVPLevel = 0;
            if (this.stBattleHero != null)
            {
                this.stBattleHero.Release();
                this.stBattleHero = null;
            }
            if (this.stVip != null)
            {
                this.stVip.Release();
                this.stVip = null;
            }
        }

        public override void OnUse()
        {
            this.stBattleHero = (COMDT_ARENA_HEROINFO) ProtocolObjectPool.Get(COMDT_ARENA_HEROINFO.CLASS_ID);
            this.stVip = (COMDT_GAME_VIP_CLIENT) ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
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
                type = destBuf.writeUInt32(this.dwLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwForceValue);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
                type = destBuf.writeUInt32(this.dwPVPLevel);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    type = this.stBattleHero.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    if (VERSION_stVip <= cutVer)
                    {
                        type = this.stVip.pack(ref destBuf, cutVer);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
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
                type = srcBuf.readUInt32(ref this.dwLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwForceValue);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
                type = srcBuf.readUInt32(ref this.dwPVPLevel);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    type = this.stBattleHero.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    if (VERSION_stVip <= cutVer)
                    {
                        type = this.stVip.unpack(ref srcBuf, cutVer);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                        return type;
                    }
                    type = this.stVip.construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
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

