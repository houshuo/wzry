namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCDT_CMD_LOGINTASKRSP : ProtocolObject
    {
        public COMDT_ACNT_CURTASK[] astCurtask = new COMDT_ACNT_CURTASK[0x55];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x23b;
        public static readonly uint CURRVERSION = 0x25;
        public uint dwCurtaskNum;

        public SCDT_CMD_LOGINTASKRSP()
        {
            for (int i = 0; i < 0x55; i++)
            {
                this.astCurtask[i] = (COMDT_ACNT_CURTASK) ProtocolObjectPool.Get(COMDT_ACNT_CURTASK.CLASS_ID);
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
            this.dwCurtaskNum = 0;
            if (this.astCurtask != null)
            {
                for (int i = 0; i < this.astCurtask.Length; i++)
                {
                    if (this.astCurtask[i] != null)
                    {
                        this.astCurtask[i].Release();
                        this.astCurtask[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astCurtask != null)
            {
                for (int i = 0; i < this.astCurtask.Length; i++)
                {
                    this.astCurtask[i] = (COMDT_ACNT_CURTASK) ProtocolObjectPool.Get(COMDT_ACNT_CURTASK.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwCurtaskNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x55 < this.dwCurtaskNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astCurtask.Length < this.dwCurtaskNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwCurtaskNum; i++)
                {
                    type = this.astCurtask[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwCurtaskNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x55 < this.dwCurtaskNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwCurtaskNum; i++)
                {
                    type = this.astCurtask[i].unpack(ref srcBuf, cutVer);
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

