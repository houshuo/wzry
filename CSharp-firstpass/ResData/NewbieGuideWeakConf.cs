namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class NewbieGuideWeakConf : IUnpackable, tsf4g_csharp_interface
    {
        public static readonly uint BASEVERSION = 1;
        public byte bNotShowArrow;
        public byte bOnlyOnce;
        public static readonly uint CURRVERSION = 1;
        public uint dwID;
        public uint dwType;
        public uint dwWeakLineID;
        public int iOffsetHighLightX;
        public int iOffsetHighLightY;
        public static readonly uint LENGTH_szRemark = 60;
        public uint[] Param = new uint[3];
        public string szRemark = string.Empty;
        public byte[] szRemark_ByteArray = new byte[1];
        public ushort wSpecialTip;

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
        {
            srcBuf.disableEndian();
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = srcBuf.readUInt32(ref this.dwID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int count = 60;
                if (this.szRemark_ByteArray.GetLength(0) < count)
                {
                    this.szRemark_ByteArray = new byte[LENGTH_szRemark];
                }
                type = srcBuf.readCString(ref this.szRemark_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwWeakLineID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 3; i++)
                {
                    type = srcBuf.readUInt32(ref this.Param[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt16(ref this.wSpecialTip);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bOnlyOnce);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bNotShowArrow);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iOffsetHighLightX);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iOffsetHighLightY);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                this.TransferData();
            }
            return type;
        }

        public TdrError.ErrorType load(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = new TdrReadBuf(ref buffer, size);
            TdrError.ErrorType type = this.load(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            return type;
        }

        private void TransferData()
        {
            this.szRemark = StringHelper.UTF8BytesToString(ref this.szRemark_ByteArray);
            this.szRemark_ByteArray = null;
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
            type = srcBuf.readUInt32(ref this.dwID);
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
                if (dest > this.szRemark_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szRemark)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szRemark_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szRemark_ByteArray, (int) dest);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szRemark_ByteArray[((int) dest) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num2 = TdrTypeUtil.cstrlen(this.szRemark_ByteArray) + 1;
                    if (dest != num2)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt32(ref this.dwType);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwWeakLineID);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        type = srcBuf.readUInt32(ref this.Param[i]);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                    type = srcBuf.readUInt16(ref this.wSpecialTip);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bOnlyOnce);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bNotShowArrow);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iOffsetHighLightX);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iOffsetHighLightY);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    this.TransferData();
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

