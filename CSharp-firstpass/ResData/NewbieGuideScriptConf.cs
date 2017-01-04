namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class NewbieGuideScriptConf : IUnpackable, tsf4g_csharp_interface
    {
        public static readonly uint BASEVERSION = 1;
        public byte bNotShowArrow;
        public byte bShowTransparency;
        public byte bStopGame;
        public static readonly uint CURRVERSION = 1;
        public uint dwID;
        public int iOffsetHighLightX;
        public int iOffsetHighLightY;
        public static readonly uint LENGTH_szSoundFileName = 60;
        public ushort[] Param = new ushort[4];
        public string szSoundFileName = string.Empty;
        public byte[] szSoundFileName_ByteArray = new byte[1];
        public ushort wFlipType;
        public ushort wMainLineID;
        public ushort wSpecialTip;
        public ushort wType;

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
                type = srcBuf.readUInt16(ref this.wMainLineID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bStopGame);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 4; i++)
                {
                    type = srcBuf.readUInt16(ref this.Param[i]);
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
                type = srcBuf.readUInt8(ref this.bNotShowArrow);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int count = 60;
                if (this.szSoundFileName_ByteArray.GetLength(0) < count)
                {
                    this.szSoundFileName_ByteArray = new byte[LENGTH_szSoundFileName];
                }
                type = srcBuf.readCString(ref this.szSoundFileName_ByteArray, count);
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
                type = srcBuf.readUInt16(ref this.wFlipType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bShowTransparency);
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
            this.szSoundFileName = StringHelper.UTF8BytesToString(ref this.szSoundFileName_ByteArray);
            this.szSoundFileName_ByteArray = null;
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
                type = srcBuf.readUInt16(ref this.wMainLineID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bStopGame);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 4; i++)
                {
                    type = srcBuf.readUInt16(ref this.Param[i]);
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
                type = srcBuf.readUInt8(ref this.bNotShowArrow);
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
                if (dest > this.szSoundFileName_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szSoundFileName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSoundFileName_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSoundFileName_ByteArray, (int) dest);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szSoundFileName_ByteArray[((int) dest) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num3 = TdrTypeUtil.cstrlen(this.szSoundFileName_ByteArray) + 1;
                    if (dest != num3)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
                    type = srcBuf.readUInt16(ref this.wFlipType);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bShowTransparency);
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

