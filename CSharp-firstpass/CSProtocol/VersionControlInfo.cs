namespace CSProtocol
{
    using System;
    using tsf4g_tdr_csharp;

    public class VersionControlInfo : IPackable, IUnpackable, tsf4g_csharp_interface
    {
        public byte[][] aszReviewVer = new byte[10][];
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public int iReviewVerCount;
        public static readonly uint LENGTH_aszReviewVer = 0x40;
        public static readonly uint LENGTH_szAllowedLowVer = 0x40;
        public static readonly uint LENGTH_szCurrentVer = 0x40;
        public ulong[] ReviewVerUint;
        public byte[] szAllowedLowVer = new byte[0x40];
        public byte[] szCurrentVer = new byte[0x40];
        public ulong ullAllowedLowVerUint;
        public ulong ullCurrentVerUint;

        public VersionControlInfo()
        {
            for (int i = 0; i < 10; i++)
            {
                this.aszReviewVer[i] = new byte[0x40];
            }
            this.ReviewVerUint = new ulong[10];
        }

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
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
            int pos = destBuf.getUsedSize();
            type = destBuf.reserve(4);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int num2 = destBuf.getUsedSize();
                int count = TdrTypeUtil.cstrlen(this.szAllowedLowVer);
                if (count >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szAllowedLowVer, count);
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
                int num7 = TdrTypeUtil.cstrlen(this.szCurrentVer);
                if (num7 >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szCurrentVer, num7);
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
                type = destBuf.writeUInt64(this.ullAllowedLowVerUint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt64(this.ullCurrentVerUint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iReviewVerCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0 > this.iReviewVerCount)
                {
                    return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
                }
                if (10 < this.iReviewVerCount)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.aszReviewVer.Length < this.iReviewVerCount)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.iReviewVerCount; i++)
                {
                    int num10 = destBuf.getUsedSize();
                    type = destBuf.reserve(4);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    int num11 = destBuf.getUsedSize();
                    int num12 = TdrTypeUtil.cstrlen(this.aszReviewVer[i]);
                    if (num12 >= 0x40)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    type = destBuf.writeCString(this.aszReviewVer[i], num12);
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
                if (0 > this.iReviewVerCount)
                {
                    return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
                }
                if (10 < this.iReviewVerCount)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.ReviewVerUint.Length < this.iReviewVerCount)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int j = 0; j < this.iReviewVerCount; j++)
                {
                    type = destBuf.writeUInt64(this.ReviewVerUint[j]);
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
            TdrWriteBuf destBuf = new TdrWriteBuf(ref buffer, size);
            TdrError.ErrorType type = this.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();
                usedSize = destBuf.getUsedSize();
            }
            return type;
        }

        public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
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
            uint dest = 0;
            type = srcBuf.readUInt32(ref dest);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (dest > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (dest > this.szAllowedLowVer.GetLength(0))
                {
                    if (dest > LENGTH_szAllowedLowVer)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szAllowedLowVer = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szAllowedLowVer, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szAllowedLowVer[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szAllowedLowVer) + 1;
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
                if (num3 > this.szCurrentVer.GetLength(0))
                {
                    if (num3 > LENGTH_szCurrentVer)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szCurrentVer = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szCurrentVer, (int) num3);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szCurrentVer[((int) num3) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num4 = TdrTypeUtil.cstrlen(this.szCurrentVer) + 1;
                    if (num3 != num4)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt64(ref this.ullAllowedLowVerUint);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt64(ref this.ullCurrentVerUint);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iReviewVerCount);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    if (0 > this.iReviewVerCount)
                    {
                        return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
                    }
                    if (10 < this.iReviewVerCount)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    this.aszReviewVer = new byte[this.iReviewVerCount][];
                    for (int i = 0; i < this.iReviewVerCount; i++)
                    {
                        this.aszReviewVer[i] = new byte[1];
                    }
                    for (int j = 0; j < this.iReviewVerCount; j++)
                    {
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
                        if (num7 > this.aszReviewVer[j].GetLength(0))
                        {
                            if (num7 > LENGTH_aszReviewVer)
                            {
                                return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                            }
                            this.aszReviewVer[j] = new byte[num7];
                        }
                        if (1 > num7)
                        {
                            return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                        }
                        type = srcBuf.readCString(ref this.aszReviewVer[j], (int) num7);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                        if (this.aszReviewVer[j][((int) num7) - 1] != 0)
                        {
                            return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                        }
                        int num8 = TdrTypeUtil.cstrlen(this.aszReviewVer[j]) + 1;
                        if (num7 != num8)
                        {
                            return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                        }
                    }
                    if (0 > this.iReviewVerCount)
                    {
                        return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
                    }
                    if (10 < this.iReviewVerCount)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    this.ReviewVerUint = new ulong[this.iReviewVerCount];
                    for (int k = 0; k < this.iReviewVerCount; k++)
                    {
                        type = srcBuf.readUInt64(ref this.ReviewVerUint[k]);
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
            TdrReadBuf srcBuf = new TdrReadBuf(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            return type;
        }
    }
}

