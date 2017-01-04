namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResSpecialFucUnlock : IUnpackable, tsf4g_csharp_interface
    {
        public static readonly uint BASEVERSION = 1;
        public byte bIsAnd;
        public byte bIsShowUnlockTip;
        public static readonly uint CURRVERSION = 1;
        public uint dwFucID;
        public static readonly uint LENGTH_szLockedTip = 0x80;
        public static readonly uint LENGTH_szUnlockTip = 0x80;
        public static readonly uint LENGTH_szUnlockTipIcon = 0x80;
        public string szLockedTip = string.Empty;
        public byte[] szLockedTip_ByteArray = new byte[1];
        public string szUnlockTip = string.Empty;
        public byte[] szUnlockTip_ByteArray = new byte[1];
        public string szUnlockTipIcon = string.Empty;
        public byte[] szUnlockTipIcon_ByteArray = new byte[1];
        public uint[] UnlockArray = new uint[4];

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
            type = srcBuf.readUInt32(ref this.dwFucID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 4; i++)
                {
                    type = srcBuf.readUInt32(ref this.UnlockArray[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                int count = 0x80;
                if (this.szLockedTip_ByteArray.GetLength(0) < count)
                {
                    this.szLockedTip_ByteArray = new byte[LENGTH_szLockedTip];
                }
                type = srcBuf.readCString(ref this.szLockedTip_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsShowUnlockTip);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = 0x80;
                if (this.szUnlockTip_ByteArray.GetLength(0) < num3)
                {
                    this.szUnlockTip_ByteArray = new byte[LENGTH_szUnlockTip];
                }
                type = srcBuf.readCString(ref this.szUnlockTip_ByteArray, num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = 0x80;
                if (this.szUnlockTipIcon_ByteArray.GetLength(0) < num4)
                {
                    this.szUnlockTipIcon_ByteArray = new byte[LENGTH_szUnlockTipIcon];
                }
                type = srcBuf.readCString(ref this.szUnlockTipIcon_ByteArray, num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsAnd);
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
            this.szLockedTip = StringHelper.UTF8BytesToString(ref this.szLockedTip_ByteArray);
            this.szLockedTip_ByteArray = null;
            this.szUnlockTip = StringHelper.UTF8BytesToString(ref this.szUnlockTip_ByteArray);
            this.szUnlockTip_ByteArray = null;
            this.szUnlockTipIcon = StringHelper.UTF8BytesToString(ref this.szUnlockTipIcon_ByteArray);
            this.szUnlockTipIcon_ByteArray = null;
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
            type = srcBuf.readUInt32(ref this.dwFucID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 4; i++)
                {
                    type = srcBuf.readUInt32(ref this.UnlockArray[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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
                if (dest > this.szLockedTip_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szLockedTip)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szLockedTip_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szLockedTip_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szLockedTip_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num3 = TdrTypeUtil.cstrlen(this.szLockedTip_ByteArray) + 1;
                if (dest != num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bIsShowUnlockTip);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num4 = 0;
                type = srcBuf.readUInt32(ref num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num4 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num4 > this.szUnlockTip_ByteArray.GetLength(0))
                {
                    if (num4 > LENGTH_szUnlockTip)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szUnlockTip_ByteArray = new byte[num4];
                }
                if (1 > num4)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szUnlockTip_ByteArray, (int) num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szUnlockTip_ByteArray[((int) num4) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num5 = TdrTypeUtil.cstrlen(this.szUnlockTip_ByteArray) + 1;
                if (num4 != num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
                if (num6 > this.szUnlockTipIcon_ByteArray.GetLength(0))
                {
                    if (num6 > LENGTH_szUnlockTipIcon)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szUnlockTipIcon_ByteArray = new byte[num6];
                }
                if (1 > num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szUnlockTipIcon_ByteArray, (int) num6);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szUnlockTipIcon_ByteArray[((int) num6) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num7 = TdrTypeUtil.cstrlen(this.szUnlockTipIcon_ByteArray) + 1;
                    if (num6 != num7)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt8(ref this.bIsAnd);
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

