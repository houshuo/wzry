namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_BAN_PICK_LOG_DETAIL : ProtocolObject
    {
        public COMDT_BAN_PICK_LOG_INFO[] astRoundDetail = new COMDT_BAN_PICK_LOG_INFO[20];
        public static readonly uint BASEVERSION = 1;
        public byte bRoundNum;
        public static readonly int CLASS_ID = 0xad;
        public static readonly uint CURRVERSION = 1;

        public COMDT_BAN_PICK_LOG_DETAIL()
        {
            for (int i = 0; i < 20; i++)
            {
                this.astRoundDetail[i] = (COMDT_BAN_PICK_LOG_INFO) ProtocolObjectPool.Get(COMDT_BAN_PICK_LOG_INFO.CLASS_ID);
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
            this.bRoundNum = 0;
            if (this.astRoundDetail != null)
            {
                for (int i = 0; i < this.astRoundDetail.Length; i++)
                {
                    if (this.astRoundDetail[i] != null)
                    {
                        this.astRoundDetail[i].Release();
                        this.astRoundDetail[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astRoundDetail != null)
            {
                for (int i = 0; i < this.astRoundDetail.Length; i++)
                {
                    this.astRoundDetail[i] = (COMDT_BAN_PICK_LOG_INFO) ProtocolObjectPool.Get(COMDT_BAN_PICK_LOG_INFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bRoundNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (20 < this.bRoundNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astRoundDetail.Length < this.bRoundNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bRoundNum; i++)
                {
                    type = this.astRoundDetail[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bRoundNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (20 < this.bRoundNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bRoundNum; i++)
                {
                    type = this.astRoundDetail[i].unpack(ref srcBuf, cutVer);
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

