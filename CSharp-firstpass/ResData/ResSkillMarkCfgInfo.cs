namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResSkillMarkCfgInfo : IUnpackable, tsf4g_csharp_interface
    {
        public byte bAgeImmeExcute;
        public static readonly uint BASEVERSION = 1;
        public byte bAutoTrigger;
        public byte bLayerEffect;
        public static readonly uint CURRVERSION = 1;
        public uint dwEffectMask;
        public int iCDTime;
        public int iCfgID;
        public int iCostLayer;
        public int iDependCfgID;
        public int iImmuneTime;
        public int iLastMaxTime;
        public int iMaxLayer;
        public int iTriggerLayer;
        public static readonly uint LENGTH_szActionName = 0x80;
        public static readonly uint LENGTH_szLayerEffectName1 = 0x80;
        public static readonly uint LENGTH_szLayerEffectName2 = 0x80;
        public static readonly uint LENGTH_szLayerEffectName3 = 0x80;
        public static readonly uint LENGTH_szLayerEffectName4 = 0x80;
        public static readonly uint LENGTH_szLayerEffectName5 = 0x80;
        public static readonly uint LENGTH_szMarkDesc = 0x80;
        public static readonly uint LENGTH_szMarkName = 0x80;
        public string szActionName = string.Empty;
        public byte[] szActionName_ByteArray = new byte[1];
        public string szLayerEffectName1 = string.Empty;
        public byte[] szLayerEffectName1_ByteArray = new byte[1];
        public string szLayerEffectName2 = string.Empty;
        public byte[] szLayerEffectName2_ByteArray = new byte[1];
        public string szLayerEffectName3 = string.Empty;
        public byte[] szLayerEffectName3_ByteArray = new byte[1];
        public string szLayerEffectName4 = string.Empty;
        public byte[] szLayerEffectName4_ByteArray = new byte[1];
        public string szLayerEffectName5 = string.Empty;
        public byte[] szLayerEffectName5_ByteArray = new byte[1];
        public string szMarkDesc = string.Empty;
        public byte[] szMarkDesc_ByteArray = new byte[1];
        public string szMarkName = string.Empty;
        public byte[] szMarkName_ByteArray = new byte[1];

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
            type = srcBuf.readInt32(ref this.iCfgID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readInt32(ref this.iDependCfgID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int count = 0x80;
                if (this.szMarkName_ByteArray.GetLength(0) < count)
                {
                    this.szMarkName_ByteArray = new byte[LENGTH_szMarkName];
                }
                type = srcBuf.readCString(ref this.szMarkName_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = 0x80;
                if (this.szMarkDesc_ByteArray.GetLength(0) < num2)
                {
                    this.szMarkDesc_ByteArray = new byte[LENGTH_szMarkDesc];
                }
                type = srcBuf.readCString(ref this.szMarkDesc_ByteArray, num2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = 0x80;
                if (this.szActionName_ByteArray.GetLength(0) < num3)
                {
                    this.szActionName_ByteArray = new byte[LENGTH_szActionName];
                }
                type = srcBuf.readCString(ref this.szActionName_ByteArray, num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bLayerEffect);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iMaxLayer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iCostLayer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iTriggerLayer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iImmuneTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iLastMaxTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iCDTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bAutoTrigger);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwEffectMask);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = 0x80;
                if (this.szLayerEffectName1_ByteArray.GetLength(0) < num4)
                {
                    this.szLayerEffectName1_ByteArray = new byte[LENGTH_szLayerEffectName1];
                }
                type = srcBuf.readCString(ref this.szLayerEffectName1_ByteArray, num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = 0x80;
                if (this.szLayerEffectName2_ByteArray.GetLength(0) < num5)
                {
                    this.szLayerEffectName2_ByteArray = new byte[LENGTH_szLayerEffectName2];
                }
                type = srcBuf.readCString(ref this.szLayerEffectName2_ByteArray, num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num6 = 0x80;
                if (this.szLayerEffectName3_ByteArray.GetLength(0) < num6)
                {
                    this.szLayerEffectName3_ByteArray = new byte[LENGTH_szLayerEffectName3];
                }
                type = srcBuf.readCString(ref this.szLayerEffectName3_ByteArray, num6);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num7 = 0x80;
                if (this.szLayerEffectName4_ByteArray.GetLength(0) < num7)
                {
                    this.szLayerEffectName4_ByteArray = new byte[LENGTH_szLayerEffectName4];
                }
                type = srcBuf.readCString(ref this.szLayerEffectName4_ByteArray, num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = 0x80;
                if (this.szLayerEffectName5_ByteArray.GetLength(0) < num8)
                {
                    this.szLayerEffectName5_ByteArray = new byte[LENGTH_szLayerEffectName5];
                }
                type = srcBuf.readCString(ref this.szLayerEffectName5_ByteArray, num8);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bAgeImmeExcute);
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
            this.szMarkName = StringHelper.UTF8BytesToString(ref this.szMarkName_ByteArray);
            this.szMarkName_ByteArray = null;
            this.szMarkDesc = StringHelper.UTF8BytesToString(ref this.szMarkDesc_ByteArray);
            this.szMarkDesc_ByteArray = null;
            this.szActionName = StringHelper.UTF8BytesToString(ref this.szActionName_ByteArray);
            this.szActionName_ByteArray = null;
            this.szLayerEffectName1 = StringHelper.UTF8BytesToString(ref this.szLayerEffectName1_ByteArray);
            this.szLayerEffectName1_ByteArray = null;
            this.szLayerEffectName2 = StringHelper.UTF8BytesToString(ref this.szLayerEffectName2_ByteArray);
            this.szLayerEffectName2_ByteArray = null;
            this.szLayerEffectName3 = StringHelper.UTF8BytesToString(ref this.szLayerEffectName3_ByteArray);
            this.szLayerEffectName3_ByteArray = null;
            this.szLayerEffectName4 = StringHelper.UTF8BytesToString(ref this.szLayerEffectName4_ByteArray);
            this.szLayerEffectName4_ByteArray = null;
            this.szLayerEffectName5 = StringHelper.UTF8BytesToString(ref this.szLayerEffectName5_ByteArray);
            this.szLayerEffectName5_ByteArray = null;
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
            type = srcBuf.readInt32(ref this.iCfgID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readInt32(ref this.iDependCfgID);
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
                if (dest > this.szMarkName_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szMarkName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szMarkName_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szMarkName_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szMarkName_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szMarkName_ByteArray) + 1;
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
                if (num3 > this.szMarkDesc_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szMarkDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szMarkDesc_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szMarkDesc_ByteArray, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szMarkDesc_ByteArray[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szMarkDesc_ByteArray) + 1;
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
                if (num5 > this.szActionName_ByteArray.GetLength(0))
                {
                    if (num5 > LENGTH_szActionName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szActionName_ByteArray = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szActionName_ByteArray, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szActionName_ByteArray[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szActionName_ByteArray) + 1;
                if (num5 != num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bLayerEffect);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iMaxLayer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iCostLayer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iTriggerLayer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iImmuneTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iLastMaxTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iCDTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bAutoTrigger);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwEffectMask);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
                if (num7 > this.szLayerEffectName1_ByteArray.GetLength(0))
                {
                    if (num7 > LENGTH_szLayerEffectName1)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szLayerEffectName1_ByteArray = new byte[num7];
                }
                if (1 > num7)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szLayerEffectName1_ByteArray, (int) num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szLayerEffectName1_ByteArray[((int) num7) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num8 = TdrTypeUtil.cstrlen(this.szLayerEffectName1_ByteArray) + 1;
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
                if (num9 > this.szLayerEffectName2_ByteArray.GetLength(0))
                {
                    if (num9 > LENGTH_szLayerEffectName2)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szLayerEffectName2_ByteArray = new byte[num9];
                }
                if (1 > num9)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szLayerEffectName2_ByteArray, (int) num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szLayerEffectName2_ByteArray[((int) num9) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num10 = TdrTypeUtil.cstrlen(this.szLayerEffectName2_ByteArray) + 1;
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
                if (num11 > this.szLayerEffectName3_ByteArray.GetLength(0))
                {
                    if (num11 > LENGTH_szLayerEffectName3)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szLayerEffectName3_ByteArray = new byte[num11];
                }
                if (1 > num11)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szLayerEffectName3_ByteArray, (int) num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szLayerEffectName3_ByteArray[((int) num11) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num12 = TdrTypeUtil.cstrlen(this.szLayerEffectName3_ByteArray) + 1;
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
                if (num13 > this.szLayerEffectName4_ByteArray.GetLength(0))
                {
                    if (num13 > LENGTH_szLayerEffectName4)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szLayerEffectName4_ByteArray = new byte[num13];
                }
                if (1 > num13)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szLayerEffectName4_ByteArray, (int) num13);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szLayerEffectName4_ByteArray[((int) num13) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num14 = TdrTypeUtil.cstrlen(this.szLayerEffectName4_ByteArray) + 1;
                if (num13 != num14)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num15 = 0;
                type = srcBuf.readUInt32(ref num15);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num15 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num15 > this.szLayerEffectName5_ByteArray.GetLength(0))
                {
                    if (num15 > LENGTH_szLayerEffectName5)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szLayerEffectName5_ByteArray = new byte[num15];
                }
                if (1 > num15)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szLayerEffectName5_ByteArray, (int) num15);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szLayerEffectName5_ByteArray[((int) num15) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num16 = TdrTypeUtil.cstrlen(this.szLayerEffectName5_ByteArray) + 1;
                    if (num15 != num16)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt8(ref this.bAgeImmeExcute);
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

