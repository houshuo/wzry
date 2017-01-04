namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResDT_WealCommon : IUnpackable, tsf4g_csharp_interface
    {
        public static readonly uint BASEVERSION = 1;
        public byte bColorBar;
        public byte bEntrance;
        public static readonly uint CURRVERSION = 1;
        public uint dwSortID;
        public uint dwTimeType;
        public static readonly uint LENGTH_szBrief = 0x400;
        public static readonly uint LENGTH_szDescContent = 0x800;
        public static readonly uint LENGTH_szIcon = 0x10;
        public static readonly uint LENGTH_szName = 0x40;
        public static readonly uint LENGTH_szTips = 0x40;
        public static readonly uint LENGTH_szTitle = 0x40;
        public static readonly uint LENGTH_szWidgets = 0x10;
        public string szBrief = string.Empty;
        public byte[] szBrief_ByteArray = new byte[1];
        public string szDescContent = string.Empty;
        public byte[] szDescContent_ByteArray = new byte[1];
        public string szIcon = string.Empty;
        public byte[] szIcon_ByteArray = new byte[1];
        public string szName = string.Empty;
        public byte[] szName_ByteArray = new byte[1];
        public string szTips = string.Empty;
        public byte[] szTips_ByteArray = new byte[1];
        public string szTitle = string.Empty;
        public byte[] szTitle_ByteArray = new byte[1];
        public string szWidgets = string.Empty;
        public byte[] szWidgets_ByteArray = new byte[1];
        public ulong ullEndTime;
        public ulong ullShowTime;
        public ulong ullStartTime;

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
            int count = 0x40;
            if (this.szName_ByteArray.GetLength(0) < count)
            {
                this.szName_ByteArray = new byte[LENGTH_szName];
            }
            type = srcBuf.readCString(ref this.szName_ByteArray, count);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int num2 = 0x40;
                if (this.szTitle_ByteArray.GetLength(0) < num2)
                {
                    this.szTitle_ByteArray = new byte[LENGTH_szTitle];
                }
                type = srcBuf.readCString(ref this.szTitle_ByteArray, num2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = 0x10;
                if (this.szWidgets_ByteArray.GetLength(0) < num3)
                {
                    this.szWidgets_ByteArray = new byte[LENGTH_szWidgets];
                }
                type = srcBuf.readCString(ref this.szWidgets_ByteArray, num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = 0x400;
                if (this.szBrief_ByteArray.GetLength(0) < num4)
                {
                    this.szBrief_ByteArray = new byte[LENGTH_szBrief];
                }
                type = srcBuf.readCString(ref this.szBrief_ByteArray, num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = 0x10;
                if (this.szIcon_ByteArray.GetLength(0) < num5)
                {
                    this.szIcon_ByteArray = new byte[LENGTH_szIcon];
                }
                type = srcBuf.readCString(ref this.szIcon_ByteArray, num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num6 = 0x800;
                if (this.szDescContent_ByteArray.GetLength(0) < num6)
                {
                    this.szDescContent_ByteArray = new byte[LENGTH_szDescContent];
                }
                type = srcBuf.readCString(ref this.szDescContent_ByteArray, num6);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num7 = 0x40;
                if (this.szTips_ByteArray.GetLength(0) < num7)
                {
                    this.szTips_ByteArray = new byte[LENGTH_szTips];
                }
                type = srcBuf.readCString(ref this.szTips_ByteArray, num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bColorBar);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSortID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bEntrance);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt64(ref this.ullShowTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTimeType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt64(ref this.ullStartTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt64(ref this.ullEndTime);
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
            this.szName = StringHelper.UTF8BytesToString(ref this.szName_ByteArray);
            this.szName_ByteArray = null;
            this.szTitle = StringHelper.UTF8BytesToString(ref this.szTitle_ByteArray);
            this.szTitle_ByteArray = null;
            this.szWidgets = StringHelper.UTF8BytesToString(ref this.szWidgets_ByteArray);
            this.szWidgets_ByteArray = null;
            this.szBrief = StringHelper.UTF8BytesToString(ref this.szBrief_ByteArray);
            this.szBrief_ByteArray = null;
            this.szIcon = StringHelper.UTF8BytesToString(ref this.szIcon_ByteArray);
            this.szIcon_ByteArray = null;
            this.szDescContent = StringHelper.UTF8BytesToString(ref this.szDescContent_ByteArray);
            this.szDescContent_ByteArray = null;
            this.szTips = StringHelper.UTF8BytesToString(ref this.szTips_ByteArray);
            this.szTips_ByteArray = null;
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
                if (dest > this.szName_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szName_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szName_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szName_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szName_ByteArray) + 1;
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
                if (num3 > this.szTitle_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szTitle)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szTitle_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szTitle_ByteArray, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szTitle_ByteArray[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szTitle_ByteArray) + 1;
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
                if (num5 > this.szWidgets_ByteArray.GetLength(0))
                {
                    if (num5 > LENGTH_szWidgets)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szWidgets_ByteArray = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szWidgets_ByteArray, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szWidgets_ByteArray[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szWidgets_ByteArray) + 1;
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
                if (num7 > this.szBrief_ByteArray.GetLength(0))
                {
                    if (num7 > LENGTH_szBrief)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szBrief_ByteArray = new byte[num7];
                }
                if (1 > num7)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szBrief_ByteArray, (int) num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szBrief_ByteArray[((int) num7) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num8 = TdrTypeUtil.cstrlen(this.szBrief_ByteArray) + 1;
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
                if (num9 > this.szIcon_ByteArray.GetLength(0))
                {
                    if (num9 > LENGTH_szIcon)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szIcon_ByteArray = new byte[num9];
                }
                if (1 > num9)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szIcon_ByteArray, (int) num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szIcon_ByteArray[((int) num9) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num10 = TdrTypeUtil.cstrlen(this.szIcon_ByteArray) + 1;
                if (num9 != num10)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num11 = 0;
                type = srcBuf.readUInt32(ref num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num11 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num11 > this.szDescContent_ByteArray.GetLength(0))
                {
                    if (num11 > LENGTH_szDescContent)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szDescContent_ByteArray = new byte[num11];
                }
                if (1 > num11)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szDescContent_ByteArray, (int) num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szDescContent_ByteArray[((int) num11) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num12 = TdrTypeUtil.cstrlen(this.szDescContent_ByteArray) + 1;
                if (num11 != num12)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num13 = 0;
                type = srcBuf.readUInt32(ref num13);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num13 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num13 > this.szTips_ByteArray.GetLength(0))
                {
                    if (num13 > LENGTH_szTips)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szTips_ByteArray = new byte[num13];
                }
                if (1 > num13)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szTips_ByteArray, (int) num13);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szTips_ByteArray[((int) num13) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num14 = TdrTypeUtil.cstrlen(this.szTips_ByteArray) + 1;
                    if (num13 != num14)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt8(ref this.bColorBar);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwSortID);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bEntrance);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt64(ref this.ullShowTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwTimeType);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt64(ref this.ullStartTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt64(ref this.ullEndTime);
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

