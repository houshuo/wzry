namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_HERO_CUSTOM_EQUIP_DETAIL : ProtocolObject
    {
        public COMDT_HERO_CUSTOM_EQUIP_INFO[] astDetail = new COMDT_HERO_CUSTOM_EQUIP_INFO[0x3e8];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x21a;
        public static readonly uint CURRVERSION = 1;
        public uint dwUsedNum;

        public COMDT_HERO_CUSTOM_EQUIP_DETAIL()
        {
            for (int i = 0; i < 0x3e8; i++)
            {
                this.astDetail[i] = (COMDT_HERO_CUSTOM_EQUIP_INFO) ProtocolObjectPool.Get(COMDT_HERO_CUSTOM_EQUIP_INFO.CLASS_ID);
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
            this.dwUsedNum = 0;
            if (this.astDetail != null)
            {
                for (int i = 0; i < this.astDetail.Length; i++)
                {
                    if (this.astDetail[i] != null)
                    {
                        this.astDetail[i].Release();
                        this.astDetail[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astDetail != null)
            {
                for (int i = 0; i < this.astDetail.Length; i++)
                {
                    this.astDetail[i] = (COMDT_HERO_CUSTOM_EQUIP_INFO) ProtocolObjectPool.Get(COMDT_HERO_CUSTOM_EQUIP_INFO.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwUsedNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x3e8 < this.dwUsedNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astDetail.Length < this.dwUsedNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwUsedNum; i++)
                {
                    type = this.astDetail[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwUsedNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x3e8 < this.dwUsedNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwUsedNum; i++)
                {
                    type = this.astDetail[i].unpack(ref srcBuf, cutVer);
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

