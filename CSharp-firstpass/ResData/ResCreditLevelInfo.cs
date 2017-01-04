namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResCreditLevelInfo : IUnpackable, tsf4g_csharp_interface
    {
        public ResDT_CreditRewardShowItem[] astCreditRewardDetail = new ResDT_CreditRewardShowItem[3];
        public static readonly uint BASEVERSION = 1;
        public byte bCreditDayRewardSwitch;
        public byte bCreditLevel;
        public byte bCreditLevelResult;
        public byte bCreditWeekRewardSwitch;
        public static readonly uint CURRVERSION = 1;
        public uint dwCreditDayRewardID;
        public uint dwCreditThresholdHigh;
        public uint dwCreditThresholdLow;
        public uint dwCreditWeekRewardID;
        public uint dwID;
        public int iSettlePvpCoinTTH;
        public int iSettlePvpExpTTH;
        public static readonly uint LENGTH_szCreditDayRewardDesc = 0x100;
        public static readonly uint LENGTH_szCreditLevelTxt = 0x10;
        public static readonly uint LENGTH_szCreditWeekRewardDesc = 0x100;
        public string szCreditDayRewardDesc;
        public byte[] szCreditDayRewardDesc_ByteArray = new byte[1];
        public string szCreditLevelTxt;
        public byte[] szCreditLevelTxt_ByteArray = new byte[1];
        public string szCreditWeekRewardDesc;
        public byte[] szCreditWeekRewardDesc_ByteArray = new byte[1];

        public ResCreditLevelInfo()
        {
            for (int i = 0; i < 3; i++)
            {
                this.astCreditRewardDetail[i] = new ResDT_CreditRewardShowItem();
            }
            this.szCreditDayRewardDesc = string.Empty;
            this.szCreditWeekRewardDesc = string.Empty;
            this.szCreditLevelTxt = string.Empty;
        }

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
                type = srcBuf.readUInt32(ref this.dwCreditThresholdLow);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCreditThresholdHigh);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCreditLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCreditDayRewardSwitch);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int count = 0x100;
                if (this.szCreditDayRewardDesc_ByteArray.GetLength(0) < count)
                {
                    this.szCreditDayRewardDesc_ByteArray = new byte[LENGTH_szCreditDayRewardDesc];
                }
                type = srcBuf.readCString(ref this.szCreditDayRewardDesc_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCreditDayRewardID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCreditWeekRewardSwitch);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = 0x100;
                if (this.szCreditWeekRewardDesc_ByteArray.GetLength(0) < num2)
                {
                    this.szCreditWeekRewardDesc_ByteArray = new byte[LENGTH_szCreditWeekRewardDesc];
                }
                type = srcBuf.readCString(ref this.szCreditWeekRewardDesc_ByteArray, num2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCreditWeekRewardID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSettlePvpExpTTH);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSettlePvpCoinTTH);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCreditLevelResult);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = 0x10;
                if (this.szCreditLevelTxt_ByteArray.GetLength(0) < num3)
                {
                    this.szCreditLevelTxt_ByteArray = new byte[LENGTH_szCreditLevelTxt];
                }
                type = srcBuf.readCString(ref this.szCreditLevelTxt_ByteArray, num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 3; i++)
                {
                    type = this.astCreditRewardDetail[i].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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
            this.szCreditDayRewardDesc = StringHelper.UTF8BytesToString(ref this.szCreditDayRewardDesc_ByteArray);
            this.szCreditDayRewardDesc_ByteArray = null;
            this.szCreditWeekRewardDesc = StringHelper.UTF8BytesToString(ref this.szCreditWeekRewardDesc_ByteArray);
            this.szCreditWeekRewardDesc_ByteArray = null;
            this.szCreditLevelTxt = StringHelper.UTF8BytesToString(ref this.szCreditLevelTxt_ByteArray);
            this.szCreditLevelTxt_ByteArray = null;
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
                type = srcBuf.readUInt32(ref this.dwCreditThresholdLow);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCreditThresholdHigh);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCreditLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCreditDayRewardSwitch);
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
                if (dest > this.szCreditDayRewardDesc_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szCreditDayRewardDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szCreditDayRewardDesc_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szCreditDayRewardDesc_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szCreditDayRewardDesc_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szCreditDayRewardDesc_ByteArray) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwCreditDayRewardID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCreditWeekRewardSwitch);
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
                if (num3 > this.szCreditWeekRewardDesc_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szCreditWeekRewardDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szCreditWeekRewardDesc_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szCreditWeekRewardDesc_ByteArray, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szCreditWeekRewardDesc_ByteArray[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szCreditWeekRewardDesc_ByteArray) + 1;
                if (num3 != num4)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwCreditWeekRewardID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSettlePvpExpTTH);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSettlePvpCoinTTH);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bCreditLevelResult);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
                if (num5 > this.szCreditLevelTxt_ByteArray.GetLength(0))
                {
                    if (num5 > LENGTH_szCreditLevelTxt)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szCreditLevelTxt_ByteArray = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szCreditLevelTxt_ByteArray, (int) num5);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szCreditLevelTxt_ByteArray[((int) num5) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num6 = TdrTypeUtil.cstrlen(this.szCreditLevelTxt_ByteArray) + 1;
                    if (num5 != num6)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        type = this.astCreditRewardDetail[i].unpack(ref srcBuf, cutVer);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
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

