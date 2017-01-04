namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_SHOPBUY_RECORD : ProtocolObject
    {
        public COMDT_DRAWCNT_RECORD[] astDrawRecord = new COMDT_DRAWCNT_RECORD[15];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x112;
        public static readonly uint CURRVERSION = 0x47;
        public uint dwDirectBuyCurItemCnt;
        public uint dwDirectBuyItemRefreshTime;
        public int iLimitRefreshTime;
        public int[] ShopBuyLimit;
        public COMDT_COINDRAW_INFO stCoinDrawInfo;
        public static readonly uint VERSION_dwDirectBuyCurItemCnt = 0x23;
        public static readonly uint VERSION_dwDirectBuyItemRefreshTime = 9;

        public COMDT_SHOPBUY_RECORD()
        {
            for (int i = 0; i < 15; i++)
            {
                this.astDrawRecord[i] = (COMDT_DRAWCNT_RECORD) ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
            }
            this.ShopBuyLimit = new int[20];
            this.stCoinDrawInfo = (COMDT_COINDRAW_INFO) ProtocolObjectPool.Get(COMDT_COINDRAW_INFO.CLASS_ID);
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
            if (this.astDrawRecord != null)
            {
                for (int i = 0; i < this.astDrawRecord.Length; i++)
                {
                    if (this.astDrawRecord[i] != null)
                    {
                        this.astDrawRecord[i].Release();
                        this.astDrawRecord[i] = null;
                    }
                }
            }
            this.iLimitRefreshTime = 0;
            if (this.stCoinDrawInfo != null)
            {
                this.stCoinDrawInfo.Release();
                this.stCoinDrawInfo = null;
            }
            this.dwDirectBuyItemRefreshTime = 0;
            this.dwDirectBuyCurItemCnt = 0;
        }

        public override void OnUse()
        {
            if (this.astDrawRecord != null)
            {
                for (int i = 0; i < this.astDrawRecord.Length; i++)
                {
                    this.astDrawRecord[i] = (COMDT_DRAWCNT_RECORD) ProtocolObjectPool.Get(COMDT_DRAWCNT_RECORD.CLASS_ID);
                }
            }
            this.stCoinDrawInfo = (COMDT_COINDRAW_INFO) ProtocolObjectPool.Get(COMDT_COINDRAW_INFO.CLASS_ID);
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
            for (int i = 0; i < 15; i++)
            {
                type = this.astDrawRecord[i].pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            type = destBuf.writeInt32(this.iLimitRefreshTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int j = 0; j < 20; j++)
                {
                    type = destBuf.writeInt32(this.ShopBuyLimit[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stCoinDrawInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwDirectBuyItemRefreshTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwDirectBuyItemRefreshTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwDirectBuyCurItemCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwDirectBuyCurItemCnt);
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
            for (int i = 0; i < 15; i++)
            {
                type = this.astDrawRecord[i].unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            type = srcBuf.readInt32(ref this.iLimitRefreshTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int j = 0; j < 20; j++)
                {
                    type = srcBuf.readInt32(ref this.ShopBuyLimit[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stCoinDrawInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwDirectBuyItemRefreshTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwDirectBuyItemRefreshTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwDirectBuyItemRefreshTime = 0;
                }
                if (VERSION_dwDirectBuyCurItemCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwDirectBuyCurItemCnt);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwDirectBuyCurItemCnt = 0;
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

