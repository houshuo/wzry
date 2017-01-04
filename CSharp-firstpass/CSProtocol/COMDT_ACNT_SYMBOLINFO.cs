namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ACNT_SYMBOLINFO : ProtocolObject
    {
        public COMDT_SYMBOLPAGE_EXTRA[] astPageExtra;
        public COMDT_SYMBOLPAGE_DETAIL[] astPageList = new COMDT_SYMBOLPAGE_DETAIL[12];
        public static readonly uint BASEVERSION = 1;
        public byte bBuyPageCnt;
        public byte bValidPageCnt;
        public static readonly int CLASS_ID = 0x51;
        public static readonly uint CURRVERSION = 0x6f;
        public COMDT_SYMBOLPAGE_RCMD stRecommend;
        public static readonly uint VERSION_bBuyPageCnt = 0x13;
        public static readonly uint VERSION_stRecommend = 0x6f;

        public COMDT_ACNT_SYMBOLINFO()
        {
            for (int i = 0; i < 12; i++)
            {
                this.astPageList[i] = (COMDT_SYMBOLPAGE_DETAIL) ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_DETAIL.CLASS_ID);
            }
            this.astPageExtra = new COMDT_SYMBOLPAGE_EXTRA[30];
            for (int j = 0; j < 30; j++)
            {
                this.astPageExtra[j] = (COMDT_SYMBOLPAGE_EXTRA) ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_EXTRA.CLASS_ID);
            }
            this.stRecommend = (COMDT_SYMBOLPAGE_RCMD) ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_RCMD.CLASS_ID);
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
            this.bValidPageCnt = 0;
            if (this.astPageList != null)
            {
                for (int i = 0; i < this.astPageList.Length; i++)
                {
                    if (this.astPageList[i] != null)
                    {
                        this.astPageList[i].Release();
                        this.astPageList[i] = null;
                    }
                }
            }
            this.bBuyPageCnt = 0;
            if (this.astPageExtra != null)
            {
                for (int j = 0; j < this.astPageExtra.Length; j++)
                {
                    if (this.astPageExtra[j] != null)
                    {
                        this.astPageExtra[j].Release();
                        this.astPageExtra[j] = null;
                    }
                }
            }
            if (this.stRecommend != null)
            {
                this.stRecommend.Release();
                this.stRecommend = null;
            }
        }

        public override void OnUse()
        {
            if (this.astPageList != null)
            {
                for (int i = 0; i < this.astPageList.Length; i++)
                {
                    this.astPageList[i] = (COMDT_SYMBOLPAGE_DETAIL) ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_DETAIL.CLASS_ID);
                }
            }
            if (this.astPageExtra != null)
            {
                for (int j = 0; j < this.astPageExtra.Length; j++)
                {
                    this.astPageExtra[j] = (COMDT_SYMBOLPAGE_EXTRA) ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_EXTRA.CLASS_ID);
                }
            }
            this.stRecommend = (COMDT_SYMBOLPAGE_RCMD) ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_RCMD.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bValidPageCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (12 < this.bValidPageCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astPageList.Length < this.bValidPageCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bValidPageCnt; i++)
                {
                    type = this.astPageList[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bBuyPageCnt <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bBuyPageCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int j = 0; j < 30; j++)
                {
                    type = this.astPageExtra[j].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stRecommend <= cutVer)
                {
                    type = this.stRecommend.pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bValidPageCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (12 < this.bValidPageCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bValidPageCnt; i++)
                {
                    type = this.astPageList[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bBuyPageCnt <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bBuyPageCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bBuyPageCnt = 0;
                }
                for (int j = 0; j < 30; j++)
                {
                    type = this.astPageExtra[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stRecommend <= cutVer)
                {
                    type = this.stRecommend.unpack(ref srcBuf, cutVer);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                type = this.stRecommend.construct();
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

