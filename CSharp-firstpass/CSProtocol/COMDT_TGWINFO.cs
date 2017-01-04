namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_TGWINFO : ProtocolObject
    {
        public COMDT_VIP[] astVipInfo = new COMDT_VIP[8];
        public static readonly uint BASEVERSION = 1;
        public byte bIsUDP;
        public static readonly int CLASS_ID = 0x26;
        public static readonly uint CURRVERSION = 1;
        public uint dwVipCnt;
        public static readonly uint LENGTH_szRelayUrl = 0x80;
        public byte[] szRelayUrl = new byte[0x80];
        public ushort wEchoPort;
        public ushort wVPort;

        public COMDT_TGWINFO()
        {
            for (int i = 0; i < 8; i++)
            {
                this.astVipInfo[i] = (COMDT_VIP) ProtocolObjectPool.Get(COMDT_VIP.CLASS_ID);
            }
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
            this.bIsUDP = 0;
            this.wVPort = 0;
            this.wEchoPort = 0;
            this.dwVipCnt = 0;
            if (this.astVipInfo != null)
            {
                for (int i = 0; i < this.astVipInfo.Length; i++)
                {
                    if (this.astVipInfo[i] != null)
                    {
                        this.astVipInfo[i].Release();
                        this.astVipInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astVipInfo != null)
            {
                for (int i = 0; i < this.astVipInfo.Length; i++)
                {
                    this.astVipInfo[i] = (COMDT_VIP) ProtocolObjectPool.Get(COMDT_VIP.CLASS_ID);
                }
            }
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
            type = destBuf.writeUInt8(this.bIsUDP);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt16(this.wVPort);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt16(this.wEchoPort);
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
                int count = TdrTypeUtil.cstrlen(this.szRelayUrl);
                if (count >= 0x80)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szRelayUrl, count);
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
                type = destBuf.writeUInt32(this.dwVipCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (8 < this.dwVipCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astVipInfo.Length < this.dwVipCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwVipCnt; i++)
                {
                    type = this.astVipInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bIsUDP);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt16(ref this.wVPort);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wEchoPort);
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
                if (dest > this.szRelayUrl.GetLength(0))
                {
                    if (dest > LENGTH_szRelayUrl)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szRelayUrl = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szRelayUrl, (int) dest);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szRelayUrl[((int) dest) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num2 = TdrTypeUtil.cstrlen(this.szRelayUrl) + 1;
                    if (dest != num2)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt32(ref this.dwVipCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    if (8 < this.dwVipCnt)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    for (int i = 0; i < this.dwVipCnt; i++)
                    {
                        type = this.astVipInfo[i].unpack(ref srcBuf, cutVer);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
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

