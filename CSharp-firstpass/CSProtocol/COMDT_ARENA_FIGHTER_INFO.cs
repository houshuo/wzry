namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ARENA_FIGHTER_INFO : ProtocolObject
    {
        public COMDT_ARENA_FIGHTER_DETAIL[] astFigterDetail = new COMDT_ARENA_FIGHTER_DETAIL[100];
        public static readonly uint BASEVERSION = 1;
        public byte bFigterNum;
        public static readonly int CLASS_ID = 0x1ce;
        public static readonly uint CURRVERSION = 0x43;

        public COMDT_ARENA_FIGHTER_INFO()
        {
            for (int i = 0; i < 100; i++)
            {
                this.astFigterDetail[i] = (COMDT_ARENA_FIGHTER_DETAIL) ProtocolObjectPool.Get(COMDT_ARENA_FIGHTER_DETAIL.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.bFigterNum = 0;
            for (int i = 0; i < 100; i++)
            {
                type = this.astFigterDetail[i].construct();
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
            this.bFigterNum = 0;
            if (this.astFigterDetail != null)
            {
                for (int i = 0; i < this.astFigterDetail.Length; i++)
                {
                    if (this.astFigterDetail[i] != null)
                    {
                        this.astFigterDetail[i].Release();
                        this.astFigterDetail[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astFigterDetail != null)
            {
                for (int i = 0; i < this.astFigterDetail.Length; i++)
                {
                    this.astFigterDetail[i] = (COMDT_ARENA_FIGHTER_DETAIL) ProtocolObjectPool.Get(COMDT_ARENA_FIGHTER_DETAIL.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bFigterNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (100 < this.bFigterNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astFigterDetail.Length < this.bFigterNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bFigterNum; i++)
                {
                    type = this.astFigterDetail[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bFigterNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (100 < this.bFigterNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bFigterNum; i++)
                {
                    type = this.astFigterDetail[i].unpack(ref srcBuf, cutVer);
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

