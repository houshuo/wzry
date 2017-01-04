namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResSignalInfo : IUnpackable, tsf4g_csharp_interface
    {
        public static readonly uint BASEVERSION = 1;
        public byte bCooldownTime;
        public byte bSignalID;
        public byte bSignalType;
        public byte bTime;
        public static readonly uint CURRVERSION = 1;
        public static readonly uint LENGTH_szRealEffect = 0x80;
        public static readonly uint LENGTH_szSceneEffect = 0x80;
        public static readonly uint LENGTH_szSound = 0x80;
        public static readonly uint LENGTH_szText = 0x80;
        public static readonly uint LENGTH_szUIIcon = 0x80;
        public string szRealEffect = string.Empty;
        public byte[] szRealEffect_ByteArray = new byte[1];
        public string szSceneEffect = string.Empty;
        public byte[] szSceneEffect_ByteArray = new byte[1];
        public string szSound = string.Empty;
        public byte[] szSound_ByteArray = new byte[1];
        public string szText = string.Empty;
        public byte[] szText_ByteArray = new byte[1];
        public string szUIIcon = string.Empty;
        public byte[] szUIIcon_ByteArray = new byte[1];

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
            type = srcBuf.readUInt8(ref this.bSignalID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt8(ref this.bSignalType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int count = 0x80;
                if (this.szUIIcon_ByteArray.GetLength(0) < count)
                {
                    this.szUIIcon_ByteArray = new byte[LENGTH_szUIIcon];
                }
                type = srcBuf.readCString(ref this.szUIIcon_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = 0x80;
                if (this.szText_ByteArray.GetLength(0) < num2)
                {
                    this.szText_ByteArray = new byte[LENGTH_szText];
                }
                type = srcBuf.readCString(ref this.szText_ByteArray, num2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = 0x80;
                if (this.szSceneEffect_ByteArray.GetLength(0) < num3)
                {
                    this.szSceneEffect_ByteArray = new byte[LENGTH_szSceneEffect];
                }
                type = srcBuf.readCString(ref this.szSceneEffect_ByteArray, num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = 0x80;
                if (this.szRealEffect_ByteArray.GetLength(0) < num4)
                {
                    this.szRealEffect_ByteArray = new byte[LENGTH_szRealEffect];
                }
                type = srcBuf.readCString(ref this.szRealEffect_ByteArray, num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = 0x80;
                if (this.szSound_ByteArray.GetLength(0) < num5)
                {
                    this.szSound_ByteArray = new byte[LENGTH_szSound];
                }
                type = srcBuf.readCString(ref this.szSound_ByteArray, num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCooldownTime);
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
            this.szUIIcon = StringHelper.UTF8BytesToString(ref this.szUIIcon_ByteArray);
            this.szUIIcon_ByteArray = null;
            this.szText = StringHelper.UTF8BytesToString(ref this.szText_ByteArray);
            this.szText_ByteArray = null;
            this.szSceneEffect = StringHelper.UTF8BytesToString(ref this.szSceneEffect_ByteArray);
            this.szSceneEffect_ByteArray = null;
            this.szRealEffect = StringHelper.UTF8BytesToString(ref this.szRealEffect_ByteArray);
            this.szRealEffect_ByteArray = null;
            this.szSound = StringHelper.UTF8BytesToString(ref this.szSound_ByteArray);
            this.szSound_ByteArray = null;
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
            type = srcBuf.readUInt8(ref this.bSignalID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt8(ref this.bSignalType);
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
                if (dest > this.szUIIcon_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szUIIcon)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szUIIcon_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szUIIcon_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szUIIcon_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szUIIcon_ByteArray) + 1;
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
                if (num3 > this.szText_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szText)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szText_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szText_ByteArray, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szText_ByteArray[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szText_ByteArray) + 1;
                if (num3 != num4)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
                if (num5 > this.szSceneEffect_ByteArray.GetLength(0))
                {
                    if (num5 > LENGTH_szSceneEffect)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSceneEffect_ByteArray = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSceneEffect_ByteArray, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSceneEffect_ByteArray[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szSceneEffect_ByteArray) + 1;
                if (num5 != num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
                if (num7 > this.szRealEffect_ByteArray.GetLength(0))
                {
                    if (num7 > LENGTH_szRealEffect)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szRealEffect_ByteArray = new byte[num7];
                }
                if (1 > num7)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szRealEffect_ByteArray, (int) num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szRealEffect_ByteArray[((int) num7) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num8 = TdrTypeUtil.cstrlen(this.szRealEffect_ByteArray) + 1;
                if (num7 != num8)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num9 = 0;
                type = srcBuf.readUInt32(ref num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num9 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num9 > this.szSound_ByteArray.GetLength(0))
                {
                    if (num9 > LENGTH_szSound)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSound_ByteArray = new byte[num9];
                }
                if (1 > num9)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSound_ByteArray, (int) num9);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szSound_ByteArray[((int) num9) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num10 = TdrTypeUtil.cstrlen(this.szSound_ByteArray) + 1;
                    if (num9 != num10)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt8(ref this.bTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bCooldownTime);
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

