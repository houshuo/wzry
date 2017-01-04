namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_SYMBOLCHG_LIST : ProtocolObject
    {
        public CSDT_SYMBOLCHG_OBJ[] astChgInfo = new CSDT_SYMBOLCHG_OBJ[30];
        public static readonly uint BASEVERSION = 1;
        public byte bChgCnt;
        public static readonly int CLASS_ID = 0x31e;
        public static readonly uint CURRVERSION = 1;

        public CSDT_SYMBOLCHG_LIST()
        {
            for (int i = 0; i < 30; i++)
            {
                this.astChgInfo[i] = (CSDT_SYMBOLCHG_OBJ) ProtocolObjectPool.Get(CSDT_SYMBOLCHG_OBJ.CLASS_ID);
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
            this.bChgCnt = 0;
            if (this.astChgInfo != null)
            {
                for (int i = 0; i < this.astChgInfo.Length; i++)
                {
                    if (this.astChgInfo[i] != null)
                    {
                        this.astChgInfo[i].Release();
                        this.astChgInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astChgInfo != null)
            {
                for (int i = 0; i < this.astChgInfo.Length; i++)
                {
                    this.astChgInfo[i] = (CSDT_SYMBOLCHG_OBJ) ProtocolObjectPool.Get(CSDT_SYMBOLCHG_OBJ.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bChgCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (30 < this.bChgCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astChgInfo.Length < this.bChgCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bChgCnt; i++)
                {
                    type = this.astChgInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bChgCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (30 < this.bChgCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bChgCnt; i++)
                {
                    type = this.astChgInfo[i].unpack(ref srcBuf, cutVer);
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

