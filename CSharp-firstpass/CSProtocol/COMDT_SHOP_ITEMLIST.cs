namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_SHOP_ITEMLIST : ProtocolObject
    {
        public COMDT_SHOP_ITEMINFO[] astShopItem = new COMDT_SHOP_ITEMINFO[12];
        public static readonly uint BASEVERSION = 1;
        public byte bItemCnt;
        public static readonly int CLASS_ID = 0x4a;
        public static readonly uint CURRVERSION = 1;

        public COMDT_SHOP_ITEMLIST()
        {
            for (int i = 0; i < 12; i++)
            {
                this.astShopItem[i] = (COMDT_SHOP_ITEMINFO) ProtocolObjectPool.Get(COMDT_SHOP_ITEMINFO.CLASS_ID);
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
            this.bItemCnt = 0;
            if (this.astShopItem != null)
            {
                for (int i = 0; i < this.astShopItem.Length; i++)
                {
                    if (this.astShopItem[i] != null)
                    {
                        this.astShopItem[i].Release();
                        this.astShopItem[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astShopItem != null)
            {
                for (int i = 0; i < this.astShopItem.Length; i++)
                {
                    this.astShopItem[i] = (COMDT_SHOP_ITEMINFO) ProtocolObjectPool.Get(COMDT_SHOP_ITEMINFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bItemCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (12 < this.bItemCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astShopItem.Length < this.bItemCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bItemCnt; i++)
                {
                    type = this.astShopItem[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bItemCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (12 < this.bItemCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bItemCnt; i++)
                {
                    type = this.astShopItem[i].unpack(ref srcBuf, cutVer);
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

