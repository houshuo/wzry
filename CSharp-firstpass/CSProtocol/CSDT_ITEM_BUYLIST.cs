namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_ITEM_BUYLIST : ProtocolObject
    {
        public CSDT_ITEM_BUYOBJ[] astBuyObj = new CSDT_ITEM_BUYOBJ[400];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x2e7;
        public static readonly uint CURRVERSION = 1;
        public ushort wBuyCnt;

        public CSDT_ITEM_BUYLIST()
        {
            for (int i = 0; i < 400; i++)
            {
                this.astBuyObj[i] = (CSDT_ITEM_BUYOBJ) ProtocolObjectPool.Get(CSDT_ITEM_BUYOBJ.CLASS_ID);
            }
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
            this.wBuyCnt = 0;
            if (this.astBuyObj != null)
            {
                for (int i = 0; i < this.astBuyObj.Length; i++)
                {
                    if (this.astBuyObj[i] != null)
                    {
                        this.astBuyObj[i].Release();
                        this.astBuyObj[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astBuyObj != null)
            {
                for (int i = 0; i < this.astBuyObj.Length; i++)
                {
                    this.astBuyObj[i] = (CSDT_ITEM_BUYOBJ) ProtocolObjectPool.Get(CSDT_ITEM_BUYOBJ.CLASS_ID);
                }
            }
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
            type = destBuf.writeUInt16(this.wBuyCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (400 < this.wBuyCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astBuyObj.Length < this.wBuyCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.wBuyCnt; i++)
                {
                    type = this.astBuyObj[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt16(ref this.wBuyCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (400 < this.wBuyCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.wBuyCnt; i++)
                {
                    type = this.astBuyObj[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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

