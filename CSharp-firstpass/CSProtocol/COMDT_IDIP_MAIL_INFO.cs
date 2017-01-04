namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_IDIP_MAIL_INFO : ProtocolObject
    {
        public COMDT_MAILACCESS[] astAccess = new COMDT_MAILACCESS[10];
        public byte bAccessCnt;
        public static readonly uint BASEVERSION = 1;
        public byte bMailsubType;
        public byte bMailType;
        public byte bSubjectLen;
        public static readonly int CLASS_ID = 0x181;
        public static readonly uint CURRVERSION = 0x53;
        public uint dwToLogicWorld;
        public static readonly uint LENGTH_szFrom = 0x40;
        public static readonly uint LENGTH_szTo = 0x40;
        public sbyte[] szContent = new sbyte[600];
        public byte[] szFrom = new byte[0x40];
        public sbyte[] szSubject = new sbyte[0x80];
        public byte[] szTo = new byte[0x40];
        public ulong ullToUid;
        public ushort wContentLen;

        public COMDT_IDIP_MAIL_INFO()
        {
            for (int i = 0; i < 10; i++)
            {
                this.astAccess[i] = (COMDT_MAILACCESS) ProtocolObjectPool.Get(COMDT_MAILACCESS.CLASS_ID);
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
            this.bMailType = 0;
            this.bMailsubType = 0;
            this.ullToUid = 0L;
            this.dwToLogicWorld = 0;
            this.bSubjectLen = 0;
            this.wContentLen = 0;
            this.bAccessCnt = 0;
            if (this.astAccess != null)
            {
                for (int i = 0; i < this.astAccess.Length; i++)
                {
                    if (this.astAccess[i] != null)
                    {
                        this.astAccess[i].Release();
                        this.astAccess[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astAccess != null)
            {
                for (int i = 0; i < this.astAccess.Length; i++)
                {
                    this.astAccess[i] = (COMDT_MAILACCESS) ProtocolObjectPool.Get(COMDT_MAILACCESS.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bMailType);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt8(this.bMailsubType);
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
                int count = TdrTypeUtil.cstrlen(this.szFrom);
                if (count >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szFrom, count);
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
                int num7 = TdrTypeUtil.cstrlen(this.szTo);
                if (num7 >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szTo, num7);
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
                type = destBuf.writeUInt64(this.ullToUid);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwToLogicWorld);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bSubjectLen);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x80 < this.bSubjectLen)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.szSubject.Length < this.bSubjectLen)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bSubjectLen; i++)
                {
                    type = destBuf.writeInt8(this.szSubject[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt16(this.wContentLen);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (600 < this.wContentLen)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.szContent.Length < this.wContentLen)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int j = 0; j < this.wContentLen; j++)
                {
                    type = destBuf.writeInt8(this.szContent[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bAccessCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.bAccessCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astAccess.Length < this.bAccessCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int k = 0; k < this.bAccessCnt; k++)
                {
                    type = this.astAccess[k].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bMailType);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt8(ref this.bMailsubType);
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
                if (dest > this.szFrom.GetLength(0))
                {
                    if (dest > LENGTH_szFrom)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szFrom = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szFrom, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szFrom[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szFrom) + 1;
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
                if (num3 > this.szTo.GetLength(0))
                {
                    if (num3 > LENGTH_szTo)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szTo = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szTo, (int) num3);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szTo[((int) num3) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num4 = TdrTypeUtil.cstrlen(this.szTo) + 1;
                    if (num3 != num4)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt64(ref this.ullToUid);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwToLogicWorld);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bSubjectLen);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    if (0x80 < this.bSubjectLen)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    this.szSubject = new sbyte[this.bSubjectLen];
                    for (int i = 0; i < this.bSubjectLen; i++)
                    {
                        type = srcBuf.readInt8(ref this.szSubject[i]);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                    type = srcBuf.readUInt16(ref this.wContentLen);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    if (600 < this.wContentLen)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    this.szContent = new sbyte[this.wContentLen];
                    for (int j = 0; j < this.wContentLen; j++)
                    {
                        type = srcBuf.readInt8(ref this.szContent[j]);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                    type = srcBuf.readUInt8(ref this.bAccessCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    if (10 < this.bAccessCnt)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    for (int k = 0; k < this.bAccessCnt; k++)
                    {
                        type = this.astAccess[k].unpack(ref srcBuf, cutVer);
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

