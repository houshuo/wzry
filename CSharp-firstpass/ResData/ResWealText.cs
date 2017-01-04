namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResWealText : IUnpackable, tsf4g_csharp_interface
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public uint dwID;
        public uint dwJumpEntrance;
        public static readonly uint LENGTH_szJumpBtnText = 20;
        public static readonly uint LENGTH_szJumpBtnToUrl = 0x80;
        public ResDT_WealCommon stCommon = new ResDT_WealCommon();
        public string szJumpBtnText = string.Empty;
        public byte[] szJumpBtnText_ByteArray = new byte[1];
        public string szJumpBtnToUrl = string.Empty;
        public byte[] szJumpBtnToUrl_ByteArray = new byte[1];

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
                type = this.stCommon.load(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int count = 20;
                if (this.szJumpBtnText_ByteArray.GetLength(0) < count)
                {
                    this.szJumpBtnText_ByteArray = new byte[LENGTH_szJumpBtnText];
                }
                type = srcBuf.readCString(ref this.szJumpBtnText_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwJumpEntrance);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = 0x80;
                if (this.szJumpBtnToUrl_ByteArray.GetLength(0) < num2)
                {
                    this.szJumpBtnToUrl_ByteArray = new byte[LENGTH_szJumpBtnToUrl];
                }
                type = srcBuf.readCString(ref this.szJumpBtnToUrl_ByteArray, num2);
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
            this.szJumpBtnText = StringHelper.UTF8BytesToString(ref this.szJumpBtnText_ByteArray);
            this.szJumpBtnText_ByteArray = null;
            this.szJumpBtnToUrl = StringHelper.UTF8BytesToString(ref this.szJumpBtnToUrl_ByteArray);
            this.szJumpBtnToUrl_ByteArray = null;
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
                type = this.stCommon.unpack(ref srcBuf, cutVer);
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
                if (dest > this.szJumpBtnText_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szJumpBtnText)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szJumpBtnText_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szJumpBtnText_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szJumpBtnText_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szJumpBtnText_ByteArray) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwJumpEntrance);
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
                if (num3 > this.szJumpBtnToUrl_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szJumpBtnToUrl)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szJumpBtnToUrl_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szJumpBtnToUrl_ByteArray, (int) num3);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szJumpBtnToUrl_ByteArray[((int) num3) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num4 = TdrTypeUtil.cstrlen(this.szJumpBtnToUrl_ByteArray) + 1;
                    if (num3 != num4)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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

