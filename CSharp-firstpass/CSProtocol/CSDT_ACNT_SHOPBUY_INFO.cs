namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_ACNT_SHOPBUY_INFO : ProtocolObject
    {
        public CSDT_SHOPBUY_DRAWINFO[] astShopDrawInfo = new CSDT_SHOPBUY_DRAWINFO[15];
        public static readonly uint BASEVERSION = 1;
        public byte bCurCoinDrawStep;
        public static readonly int CLASS_ID = 0x23d;
        public static readonly uint CURRVERSION = 0x47;
        public uint dwDirectBuyItemCnt;
        public uint dwOpenBoxByCouponsCnt;
        public int iGameSysTime;
        public int[] LeftShopBuyCnt;
        public COMDT_DRAWCNT_RECORD stSymbolDrawCommon;
        public COMDT_DRAWCNT_RECORD stSymbolDrawSenior;

        public CSDT_ACNT_SHOPBUY_INFO()
        {
            for (int i = 0; i < 15; i++)
            {
                this.astShopDrawInfo[i] = (CSDT_SHOPBUY_DRAWINFO) ProtocolObjectPool.Get(CSDT_SHOPBUY_DRAWINFO.CLASS_ID);
            }
            this.LeftShopBuyCnt = new int[20];
            this.stSymbolDrawCommon = (COMDT_DRAWCNT_RECORD) ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
            this.stSymbolDrawSenior = (COMDT_DRAWCNT_RECORD) ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
        }

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
            this.iGameSysTime = 0;
            if (this.astShopDrawInfo != null)
            {
                for (int i = 0; i < this.astShopDrawInfo.Length; i++)
                {
                    if (this.astShopDrawInfo[i] != null)
                    {
                        this.astShopDrawInfo[i].Release();
                        this.astShopDrawInfo[i] = null;
                    }
                }
            }
            this.bCurCoinDrawStep = 0;
            this.dwOpenBoxByCouponsCnt = 0;
            this.dwDirectBuyItemCnt = 0;
            if (this.stSymbolDrawCommon != null)
            {
                this.stSymbolDrawCommon.Release();
                this.stSymbolDrawCommon = null;
            }
            if (this.stSymbolDrawSenior != null)
            {
                this.stSymbolDrawSenior.Release();
                this.stSymbolDrawSenior = null;
            }
        }

        public override void OnUse()
        {
            if (this.astShopDrawInfo != null)
            {
                for (int i = 0; i < this.astShopDrawInfo.Length; i++)
                {
                    this.astShopDrawInfo[i] = (CSDT_SHOPBUY_DRAWINFO) ProtocolObjectPool.Get(CSDT_SHOPBUY_DRAWINFO.CLASS_ID);
                }
            }
            this.stSymbolDrawCommon = (COMDT_DRAWCNT_RECORD) ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
            this.stSymbolDrawSenior = (COMDT_DRAWCNT_RECORD) ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
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
            type = destBuf.writeInt32(this.iGameSysTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 15; i++)
                {
                    type = this.astShopDrawInfo[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int j = 0; j < 20; j++)
                {
                    type = destBuf.writeInt32(this.LeftShopBuyCnt[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bCurCoinDrawStep);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwOpenBoxByCouponsCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwDirectBuyItemCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSymbolDrawCommon.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSymbolDrawSenior.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
            type = srcBuf.readInt32(ref this.iGameSysTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 15; i++)
                {
                    type = this.astShopDrawInfo[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int j = 0; j < 20; j++)
                {
                    type = srcBuf.readInt32(ref this.LeftShopBuyCnt[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bCurCoinDrawStep);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwOpenBoxByCouponsCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwDirectBuyItemCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSymbolDrawCommon.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSymbolDrawSenior.unpack(ref srcBuf, cutVer);
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

