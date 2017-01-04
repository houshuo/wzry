namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCDT_USUTASKLIST : ProtocolObject
    {
        public DT_USUTASKINFO[] astNewUsualTask = new DT_USUTASKINFO[0x55];
        public static readonly uint BASEVERSION = 1;
        public byte bNewUsualTaskCnt;
        public static readonly int CLASS_ID = 0x381;
        public static readonly uint CURRVERSION = 1;

        public SCDT_USUTASKLIST()
        {
            for (int i = 0; i < 0x55; i++)
            {
                this.astNewUsualTask[i] = (DT_USUTASKINFO) ProtocolObjectPool.Get(DT_USUTASKINFO.CLASS_ID);
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
            this.bNewUsualTaskCnt = 0;
            if (this.astNewUsualTask != null)
            {
                for (int i = 0; i < this.astNewUsualTask.Length; i++)
                {
                    if (this.astNewUsualTask[i] != null)
                    {
                        this.astNewUsualTask[i].Release();
                        this.astNewUsualTask[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astNewUsualTask != null)
            {
                for (int i = 0; i < this.astNewUsualTask.Length; i++)
                {
                    this.astNewUsualTask[i] = (DT_USUTASKINFO) ProtocolObjectPool.Get(DT_USUTASKINFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bNewUsualTaskCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x55 < this.bNewUsualTaskCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astNewUsualTask.Length < this.bNewUsualTaskCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bNewUsualTaskCnt; i++)
                {
                    type = this.astNewUsualTask[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bNewUsualTaskCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x55 < this.bNewUsualTaskCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bNewUsualTaskCnt; i++)
                {
                    type = this.astNewUsualTask[i].unpack(ref srcBuf, cutVer);
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

