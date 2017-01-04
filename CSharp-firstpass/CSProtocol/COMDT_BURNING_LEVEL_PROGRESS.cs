namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_BURNING_LEVEL_PROGRESS : ProtocolObject
    {
        public COMDT_BURNING_LEVEL_DETAIL[] astDiffDetail = new COMDT_BURNING_LEVEL_DETAIL[2];
        public static readonly uint BASEVERSION = 1;
        public byte bDiffNum;
        public static readonly int CLASS_ID = 0x134;
        public static readonly uint CURRVERSION = 0x8a;

        public COMDT_BURNING_LEVEL_PROGRESS()
        {
            for (int i = 0; i < 2; i++)
            {
                this.astDiffDetail[i] = (COMDT_BURNING_LEVEL_DETAIL) ProtocolObjectPool.Get(COMDT_BURNING_LEVEL_DETAIL.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.bDiffNum = 0;
            for (int i = 0; i < 2; i++)
            {
                type = this.astDiffDetail[i].construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            this.bDiffNum = 0;
            if (this.astDiffDetail != null)
            {
                for (int i = 0; i < this.astDiffDetail.Length; i++)
                {
                    if (this.astDiffDetail[i] != null)
                    {
                        this.astDiffDetail[i].Release();
                        this.astDiffDetail[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astDiffDetail != null)
            {
                for (int i = 0; i < this.astDiffDetail.Length; i++)
                {
                    this.astDiffDetail[i] = (COMDT_BURNING_LEVEL_DETAIL) ProtocolObjectPool.Get(COMDT_BURNING_LEVEL_DETAIL.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bDiffNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (2 < this.bDiffNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astDiffDetail.Length < this.bDiffNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bDiffNum; i++)
                {
                    type = this.astDiffDetail[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bDiffNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (2 < this.bDiffNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bDiffNum; i++)
                {
                    type = this.astDiffDetail[i].unpack(ref srcBuf, cutVer);
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

