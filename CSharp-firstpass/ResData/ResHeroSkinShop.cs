namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResHeroSkinShop : IUnpackable, tsf4g_csharp_interface
    {
        public static readonly uint BASEVERSION = 1;
        public byte bGetPathType;
        public byte bIsBuyCoupons;
        public byte bIsBuyDiamond;
        public byte bIsBuyItem;
        public byte bIsBuyMixPay;
        public byte bIsBuySkinCoin;
        public byte bIsLimitSkin;
        public byte bIsPresent;
        public byte bPromotionCnt;
        public byte bRankLimitGrade;
        public byte bRankLimitType;
        public byte bShowInMgr;
        public byte bShowInShop;
        public byte bSkinQuality;
        public static readonly uint CURRVERSION = 1;
        public uint dwBuyCoupons;
        public uint dwBuyDiamond;
        public uint dwBuyItemCnt;
        public uint dwBuySkinCoin;
        public uint dwChgItemCnt;
        public uint dwHeroID;
        public uint dwID;
        public uint dwOffTimeGen;
        public uint dwOnTimeGen;
        public uint dwReleaseId;
        public uint dwSkinID;
        public uint dwSortId;
        public static readonly uint LENGTH_szGetPath = 0x100;
        public static readonly uint LENGTH_szHeroName = 0x20;
        public static readonly uint LENGTH_szLimitLabelPic = 0x80;
        public static readonly uint LENGTH_szLimitQualityPic = 0x80;
        public static readonly uint LENGTH_szOffTimeStr = 0x10;
        public static readonly uint LENGTH_szOnTimeStr = 0x10;
        public static readonly uint LENGTH_szSkinDesc = 0x100;
        public static readonly uint LENGTH_szSkinName = 0x20;
        public uint[] PromotionID = new uint[5];
        public string szGetPath = string.Empty;
        public byte[] szGetPath_ByteArray = new byte[1];
        public string szHeroName = string.Empty;
        public byte[] szHeroName_ByteArray = new byte[1];
        public string szLimitLabelPic = string.Empty;
        public byte[] szLimitLabelPic_ByteArray = new byte[1];
        public string szLimitQualityPic = string.Empty;
        public byte[] szLimitQualityPic_ByteArray = new byte[1];
        public string szOffTimeStr = string.Empty;
        public byte[] szOffTimeStr_ByteArray = new byte[1];
        public string szOnTimeStr = string.Empty;
        public byte[] szOnTimeStr_ByteArray = new byte[1];
        public string szSkinDesc = string.Empty;
        public byte[] szSkinDesc_ByteArray = new byte[1];
        public string szSkinName = string.Empty;
        public byte[] szSkinName_ByteArray = new byte[1];
        public ulong ullRankLimitParam;

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
                type = srcBuf.readUInt32(ref this.dwHeroID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int count = 0x20;
                if (this.szHeroName_ByteArray.GetLength(0) < count)
                {
                    this.szHeroName_ByteArray = new byte[LENGTH_szHeroName];
                }
                type = srcBuf.readCString(ref this.szHeroName_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSkinID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = 0x20;
                if (this.szSkinName_ByteArray.GetLength(0) < num2)
                {
                    this.szSkinName_ByteArray = new byte[LENGTH_szSkinName];
                }
                type = srcBuf.readCString(ref this.szSkinName_ByteArray, num2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = 0x100;
                if (this.szSkinDesc_ByteArray.GetLength(0) < num3)
                {
                    this.szSkinDesc_ByteArray = new byte[LENGTH_szSkinDesc];
                }
                type = srcBuf.readCString(ref this.szSkinDesc_ByteArray, num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsLimitSkin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = 0x80;
                if (this.szLimitQualityPic_ByteArray.GetLength(0) < num4)
                {
                    this.szLimitQualityPic_ByteArray = new byte[LENGTH_szLimitQualityPic];
                }
                type = srcBuf.readCString(ref this.szLimitQualityPic_ByteArray, num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = 0x80;
                if (this.szLimitLabelPic_ByteArray.GetLength(0) < num5)
                {
                    this.szLimitLabelPic_ByteArray = new byte[LENGTH_szLimitLabelPic];
                }
                type = srcBuf.readCString(ref this.szLimitLabelPic_ByteArray, num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSkinQuality);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bGetPathType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num6 = 0x100;
                if (this.szGetPath_ByteArray.GetLength(0) < num6)
                {
                    this.szGetPath_ByteArray = new byte[LENGTH_szGetPath];
                }
                type = srcBuf.readCString(ref this.szGetPath_ByteArray, num6);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsBuyCoupons);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBuyCoupons);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsBuySkinCoin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBuySkinCoin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsBuyDiamond);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBuyDiamond);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsBuyMixPay);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsBuyItem);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBuyItemCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsPresent);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSortId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bPromotionCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 5; i++)
                {
                    type = srcBuf.readUInt32(ref this.PromotionID[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwChgItemCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = 0x10;
                if (this.szOnTimeStr_ByteArray.GetLength(0) < num8)
                {
                    this.szOnTimeStr_ByteArray = new byte[LENGTH_szOnTimeStr];
                }
                type = srcBuf.readCString(ref this.szOnTimeStr_ByteArray, num8);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num9 = 0x10;
                if (this.szOffTimeStr_ByteArray.GetLength(0) < num9)
                {
                    this.szOffTimeStr_ByteArray = new byte[LENGTH_szOffTimeStr];
                }
                type = srcBuf.readCString(ref this.szOffTimeStr_ByteArray, num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwOnTimeGen);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwOffTimeGen);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwReleaseId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bShowInShop);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bShowInMgr);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bRankLimitType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bRankLimitGrade);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt64(ref this.ullRankLimitParam);
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
            this.szHeroName = StringHelper.UTF8BytesToString(ref this.szHeroName_ByteArray);
            this.szHeroName_ByteArray = null;
            this.szSkinName = StringHelper.UTF8BytesToString(ref this.szSkinName_ByteArray);
            this.szSkinName_ByteArray = null;
            this.szSkinDesc = StringHelper.UTF8BytesToString(ref this.szSkinDesc_ByteArray);
            this.szSkinDesc_ByteArray = null;
            this.szLimitQualityPic = StringHelper.UTF8BytesToString(ref this.szLimitQualityPic_ByteArray);
            this.szLimitQualityPic_ByteArray = null;
            this.szLimitLabelPic = StringHelper.UTF8BytesToString(ref this.szLimitLabelPic_ByteArray);
            this.szLimitLabelPic_ByteArray = null;
            this.szGetPath = StringHelper.UTF8BytesToString(ref this.szGetPath_ByteArray);
            this.szGetPath_ByteArray = null;
            this.szOnTimeStr = StringHelper.UTF8BytesToString(ref this.szOnTimeStr_ByteArray);
            this.szOnTimeStr_ByteArray = null;
            this.szOffTimeStr = StringHelper.UTF8BytesToString(ref this.szOffTimeStr_ByteArray);
            this.szOffTimeStr_ByteArray = null;
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
                type = srcBuf.readUInt32(ref this.dwHeroID);
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
                if (dest > this.szHeroName_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szHeroName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szHeroName_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szHeroName_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szHeroName_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szHeroName_ByteArray) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwSkinID);
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
                if (num3 > this.szSkinName_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szSkinName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSkinName_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSkinName_ByteArray, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSkinName_ByteArray[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szSkinName_ByteArray) + 1;
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
                if (num5 > this.szSkinDesc_ByteArray.GetLength(0))
                {
                    if (num5 > LENGTH_szSkinDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSkinDesc_ByteArray = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSkinDesc_ByteArray, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSkinDesc_ByteArray[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szSkinDesc_ByteArray) + 1;
                if (num5 != num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bIsLimitSkin);
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
                if (num7 > this.szLimitQualityPic_ByteArray.GetLength(0))
                {
                    if (num7 > LENGTH_szLimitQualityPic)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szLimitQualityPic_ByteArray = new byte[num7];
                }
                if (1 > num7)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szLimitQualityPic_ByteArray, (int) num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szLimitQualityPic_ByteArray[((int) num7) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num8 = TdrTypeUtil.cstrlen(this.szLimitQualityPic_ByteArray) + 1;
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
                if (num9 > this.szLimitLabelPic_ByteArray.GetLength(0))
                {
                    if (num9 > LENGTH_szLimitLabelPic)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szLimitLabelPic_ByteArray = new byte[num9];
                }
                if (1 > num9)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szLimitLabelPic_ByteArray, (int) num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szLimitLabelPic_ByteArray[((int) num9) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num10 = TdrTypeUtil.cstrlen(this.szLimitLabelPic_ByteArray) + 1;
                if (num9 != num10)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bSkinQuality);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bGetPathType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
                if (num11 > this.szGetPath_ByteArray.GetLength(0))
                {
                    if (num11 > LENGTH_szGetPath)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGetPath_ByteArray = new byte[num11];
                }
                if (1 > num11)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGetPath_ByteArray, (int) num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGetPath_ByteArray[((int) num11) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num12 = TdrTypeUtil.cstrlen(this.szGetPath_ByteArray) + 1;
                if (num11 != num12)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bIsBuyCoupons);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBuyCoupons);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsBuySkinCoin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBuySkinCoin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsBuyDiamond);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBuyDiamond);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsBuyMixPay);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsBuyItem);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwBuyItemCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsPresent);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSortId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bPromotionCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 5; i++)
                {
                    type = srcBuf.readUInt32(ref this.PromotionID[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwChgItemCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num14 = 0;
                type = srcBuf.readUInt32(ref num14);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num14 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num14 > this.szOnTimeStr_ByteArray.GetLength(0))
                {
                    if (num14 > LENGTH_szOnTimeStr)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szOnTimeStr_ByteArray = new byte[num14];
                }
                if (1 > num14)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szOnTimeStr_ByteArray, (int) num14);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szOnTimeStr_ByteArray[((int) num14) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num15 = TdrTypeUtil.cstrlen(this.szOnTimeStr_ByteArray) + 1;
                if (num14 != num15)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num16 = 0;
                type = srcBuf.readUInt32(ref num16);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num16 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num16 > this.szOffTimeStr_ByteArray.GetLength(0))
                {
                    if (num16 > LENGTH_szOffTimeStr)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szOffTimeStr_ByteArray = new byte[num16];
                }
                if (1 > num16)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szOffTimeStr_ByteArray, (int) num16);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szOffTimeStr_ByteArray[((int) num16) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num17 = TdrTypeUtil.cstrlen(this.szOffTimeStr_ByteArray) + 1;
                    if (num16 != num17)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt32(ref this.dwOnTimeGen);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwOffTimeGen);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwReleaseId);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bShowInShop);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bShowInMgr);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bRankLimitType);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bRankLimitGrade);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt64(ref this.ullRankLimitParam);
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

