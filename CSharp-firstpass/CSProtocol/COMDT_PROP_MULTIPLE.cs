namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_PROP_MULTIPLE : ProtocolObject
    {
        public COMDT_PROP_MULTIPLE_INFO[] astMultipleInfo = new COMDT_PROP_MULTIPLE_INFO[4];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x1fa;
        public static readonly uint CURRVERSION = 1;
        public ushort wCnt;

        public COMDT_PROP_MULTIPLE()
        {
            for (int i = 0; i < 4; i++)
            {
                this.astMultipleInfo[i] = (COMDT_PROP_MULTIPLE_INFO) ProtocolObjectPool.Get(COMDT_PROP_MULTIPLE_INFO.CLASS_ID);
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
            this.wCnt = 0;
            if (this.astMultipleInfo != null)
            {
                for (int i = 0; i < this.astMultipleInfo.Length; i++)
                {
                    if (this.astMultipleInfo[i] != null)
                    {
                        this.astMultipleInfo[i].Release();
                        this.astMultipleInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astMultipleInfo != null)
            {
                for (int i = 0; i < this.astMultipleInfo.Length; i++)
                {
                    this.astMultipleInfo[i] = (COMDT_PROP_MULTIPLE_INFO) ProtocolObjectPool.Get(COMDT_PROP_MULTIPLE_INFO.CLASS_ID);
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
            type = destBuf.writeUInt16(this.wCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (4 < this.wCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astMultipleInfo.Length < this.wCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.wCnt; i++)
                {
                    type = this.astMultipleInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt16(ref this.wCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (4 < this.wCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.wCnt; i++)
                {
                    type = this.astMultipleInfo[i].unpack(ref srcBuf, cutVer);
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

