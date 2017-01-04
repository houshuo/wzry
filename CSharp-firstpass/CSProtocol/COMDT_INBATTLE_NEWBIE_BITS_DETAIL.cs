namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_INBATTLE_NEWBIE_BITS_DETAIL : ProtocolObject
    {
        public COMDT_INBATTLE_NEWBIE_BITS_INFO[] astLevelDetail = new COMDT_INBATTLE_NEWBIE_BITS_INFO[10];
        public static readonly uint BASEVERSION = 1;
        public byte bLevelNum;
        public static readonly int CLASS_ID = 0x20b;
        public static readonly uint CURRVERSION = 1;

        public COMDT_INBATTLE_NEWBIE_BITS_DETAIL()
        {
            for (int i = 0; i < 10; i++)
            {
                this.astLevelDetail[i] = (COMDT_INBATTLE_NEWBIE_BITS_INFO) ProtocolObjectPool.Get(COMDT_INBATTLE_NEWBIE_BITS_INFO.CLASS_ID);
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
            this.bLevelNum = 0;
            if (this.astLevelDetail != null)
            {
                for (int i = 0; i < this.astLevelDetail.Length; i++)
                {
                    if (this.astLevelDetail[i] != null)
                    {
                        this.astLevelDetail[i].Release();
                        this.astLevelDetail[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astLevelDetail != null)
            {
                for (int i = 0; i < this.astLevelDetail.Length; i++)
                {
                    this.astLevelDetail[i] = (COMDT_INBATTLE_NEWBIE_BITS_INFO) ProtocolObjectPool.Get(COMDT_INBATTLE_NEWBIE_BITS_INFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bLevelNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (10 < this.bLevelNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astLevelDetail.Length < this.bLevelNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bLevelNum; i++)
                {
                    type = this.astLevelDetail[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bLevelNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (10 < this.bLevelNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bLevelNum; i++)
                {
                    type = this.astLevelDetail[i].unpack(ref srcBuf, cutVer);
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

