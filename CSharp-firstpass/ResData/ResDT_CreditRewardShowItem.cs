namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResDT_CreditRewardShowItem : IUnpackable, tsf4g_csharp_interface
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public static readonly uint LENGTH_szCreditRewardItemDesc = 0x40;
        public static readonly uint LENGTH_szCreditRewardItemIcon = 0x20;
        public string szCreditRewardItemDesc = string.Empty;
        public byte[] szCreditRewardItemDesc_ByteArray = new byte[1];
        public string szCreditRewardItemIcon = string.Empty;
        public byte[] szCreditRewardItemIcon_ByteArray = new byte[1];

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
            int count = 0x20;
            if (this.szCreditRewardItemIcon_ByteArray.GetLength(0) < count)
            {
                this.szCreditRewardItemIcon_ByteArray = new byte[LENGTH_szCreditRewardItemIcon];
            }
            type = srcBuf.readCString(ref this.szCreditRewardItemIcon_ByteArray, count);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int num2 = 0x40;
                if (this.szCreditRewardItemDesc_ByteArray.GetLength(0) < num2)
                {
                    this.szCreditRewardItemDesc_ByteArray = new byte[LENGTH_szCreditRewardItemDesc];
                }
                type = srcBuf.readCString(ref this.szCreditRewardItemDesc_ByteArray, num2);
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
            this.szCreditRewardItemIcon = StringHelper.UTF8BytesToString(ref this.szCreditRewardItemIcon_ByteArray);
            this.szCreditRewardItemIcon_ByteArray = null;
            this.szCreditRewardItemDesc = StringHelper.UTF8BytesToString(ref this.szCreditRewardItemDesc_ByteArray);
            this.szCreditRewardItemDesc_ByteArray = null;
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
                if (dest > this.szCreditRewardItemIcon_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szCreditRewardItemIcon)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szCreditRewardItemIcon_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szCreditRewardItemIcon_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szCreditRewardItemIcon_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szCreditRewardItemIcon_ByteArray) + 1;
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
                if (num3 > this.szCreditRewardItemDesc_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szCreditRewardItemDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szCreditRewardItemDesc_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szCreditRewardItemDesc_ByteArray, (int) num3);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szCreditRewardItemDesc_ByteArray[((int) num3) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num4 = TdrTypeUtil.cstrlen(this.szCreditRewardItemDesc_ByteArray) + 1;
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

