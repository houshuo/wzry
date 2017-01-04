namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ITEM_PKGINFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x56;
        public static readonly uint CURRVERSION = 0x6f;
        public COMDT_ACNT_ITEMINFO stItemInfo = ((COMDT_ACNT_ITEMINFO) ProtocolObjectPool.Get(COMDT_ACNT_ITEMINFO.CLASS_ID));
        public COMDT_ACNT_SCRET_SAILINFO stScretSaleInfo = ((COMDT_ACNT_SCRET_SAILINFO) ProtocolObjectPool.Get(COMDT_ACNT_SCRET_SAILINFO.CLASS_ID));
        public COMDT_ACNT_SHOPINFO stShopInfo = ((COMDT_ACNT_SHOPINFO) ProtocolObjectPool.Get(COMDT_ACNT_SHOPINFO.CLASS_ID));
        public COMDT_ACNT_SPECIAL_SAILINFO stSpecialSaleInfo = ((COMDT_ACNT_SPECIAL_SAILINFO) ProtocolObjectPool.Get(COMDT_ACNT_SPECIAL_SAILINFO.CLASS_ID));
        public COMDT_ACNT_SYMBOLINFO stSymbolInfo = ((COMDT_ACNT_SYMBOLINFO) ProtocolObjectPool.Get(COMDT_ACNT_SYMBOLINFO.CLASS_ID));
        public static readonly uint VERSION_stScretSaleInfo = 0x37;

        public override TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            if (this.stItemInfo != null)
            {
                this.stItemInfo.Release();
                this.stItemInfo = null;
            }
            if (this.stShopInfo != null)
            {
                this.stShopInfo.Release();
                this.stShopInfo = null;
            }
            if (this.stSymbolInfo != null)
            {
                this.stSymbolInfo.Release();
                this.stSymbolInfo = null;
            }
            if (this.stSpecialSaleInfo != null)
            {
                this.stSpecialSaleInfo.Release();
                this.stSpecialSaleInfo = null;
            }
            if (this.stScretSaleInfo != null)
            {
                this.stScretSaleInfo.Release();
                this.stScretSaleInfo = null;
            }
        }

        public override void OnUse()
        {
            this.stItemInfo = (COMDT_ACNT_ITEMINFO) ProtocolObjectPool.Get(COMDT_ACNT_ITEMINFO.CLASS_ID);
            this.stShopInfo = (COMDT_ACNT_SHOPINFO) ProtocolObjectPool.Get(COMDT_ACNT_SHOPINFO.CLASS_ID);
            this.stSymbolInfo = (COMDT_ACNT_SYMBOLINFO) ProtocolObjectPool.Get(COMDT_ACNT_SYMBOLINFO.CLASS_ID);
            this.stSpecialSaleInfo = (COMDT_ACNT_SPECIAL_SAILINFO) ProtocolObjectPool.Get(COMDT_ACNT_SPECIAL_SAILINFO.CLASS_ID);
            this.stScretSaleInfo = (COMDT_ACNT_SCRET_SAILINFO) ProtocolObjectPool.Get(COMDT_ACNT_SCRET_SAILINFO.CLASS_ID);
        }

        public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
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
            type = this.stItemInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stShopInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSymbolInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSpecialSaleInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stScretSaleInfo <= cutVer)
                {
                    type = this.stScretSaleInfo.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
            }
            return type;
        }

        public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrWriteBuf destBuf = ClassObjPool<TdrWriteBuf>.Get();
            destBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();
                usedSize = destBuf.getUsedSize();
            }
            destBuf.Release();
            return type;
        }

        public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
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
            type = this.stItemInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stShopInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSymbolInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSpecialSaleInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stScretSaleInfo <= cutVer)
                {
                    type = this.stScretSaleInfo.unpack(ref srcBuf, cutVer);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                type = this.stScretSaleInfo.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
            TdrReadBuf srcBuf = ClassObjPool<TdrReadBuf>.Get();
            srcBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            srcBuf.Release();
            return type;
        }
    }
}

