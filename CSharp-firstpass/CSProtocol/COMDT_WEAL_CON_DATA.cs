namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_WEAL_CON_DATA : ProtocolObject
    {
        public COMDT_WEAL_CON_DATA_DETAIL[] astWealDetail = new COMDT_WEAL_CON_DATA_DETAIL[20];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x1c9;
        public static readonly uint CURRVERSION = 0x18;
        public static readonly uint VERSION_astWealDetail = 0x15;
        public static readonly uint VERSION_wWealNum = 0x15;
        public ushort wWealNum;

        public COMDT_WEAL_CON_DATA()
        {
            for (int i = 0; i < 20; i++)
            {
                this.astWealDetail[i] = (COMDT_WEAL_CON_DATA_DETAIL) ProtocolObjectPool.Get(COMDT_WEAL_CON_DATA_DETAIL.CLASS_ID);
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
            this.wWealNum = 0;
            if (this.astWealDetail != null)
            {
                for (int i = 0; i < this.astWealDetail.Length; i++)
                {
                    if (this.astWealDetail[i] != null)
                    {
                        this.astWealDetail[i].Release();
                        this.astWealDetail[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astWealDetail != null)
            {
                for (int i = 0; i < this.astWealDetail.Length; i++)
                {
                    this.astWealDetail[i] = (COMDT_WEAL_CON_DATA_DETAIL) ProtocolObjectPool.Get(COMDT_WEAL_CON_DATA_DETAIL.CLASS_ID);
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
            if (VERSION_wWealNum <= cutVer)
            {
                type = destBuf.writeUInt16(this.wWealNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            if (VERSION_astWealDetail <= cutVer)
            {
                if (20 < this.wWealNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astWealDetail.Length < this.wWealNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.wWealNum; i++)
                {
                    type = this.astWealDetail[i].pack(ref destBuf, cutVer);
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
            if (VERSION_wWealNum <= cutVer)
            {
                type = srcBuf.readUInt16(ref this.wWealNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            else
            {
                this.wWealNum = 0;
            }
            if (VERSION_astWealDetail <= cutVer)
            {
                if (20 < this.wWealNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int j = 0; j < this.wWealNum; j++)
                {
                    type = this.astWealDetail[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                return type;
            }
            if (20 < this.wWealNum)
            {
                return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
            }
            for (int i = 0; i < this.wWealNum; i++)
            {
                type = this.astWealDetail[i].construct();
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

