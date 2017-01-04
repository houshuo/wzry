namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_CHAPTER_COMPLETE_INFO : ProtocolObject
    {
        public COMDT_CHAPTER_DIFFICULT_INFO[] astDiffDetail;
        public COMDT_LEVEL_COMPLETE_INFO[] astLevelDetail = new COMDT_LEVEL_COMPLETE_INFO[4];
        public static readonly uint BASEVERSION = 1;
        public byte bDiffNum;
        public byte bLevelNum;
        public static readonly int CLASS_ID = 0x7a;
        public static readonly uint CURRVERSION = 1;

        public COMDT_CHAPTER_COMPLETE_INFO()
        {
            for (int i = 0; i < 4; i++)
            {
                this.astLevelDetail[i] = (COMDT_LEVEL_COMPLETE_INFO) ProtocolObjectPool.Get(COMDT_LEVEL_COMPLETE_INFO.CLASS_ID);
            }
            this.astDiffDetail = new COMDT_CHAPTER_DIFFICULT_INFO[4];
            for (int j = 0; j < 4; j++)
            {
                this.astDiffDetail[j] = (COMDT_CHAPTER_DIFFICULT_INFO) ProtocolObjectPool.Get(COMDT_CHAPTER_DIFFICULT_INFO.CLASS_ID);
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
            this.bDiffNum = 0;
            if (this.astDiffDetail != null)
            {
                for (int j = 0; j < this.astDiffDetail.Length; j++)
                {
                    if (this.astDiffDetail[j] != null)
                    {
                        this.astDiffDetail[j].Release();
                        this.astDiffDetail[j] = null;
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
                    this.astLevelDetail[i] = (COMDT_LEVEL_COMPLETE_INFO) ProtocolObjectPool.Get(COMDT_LEVEL_COMPLETE_INFO.CLASS_ID);
                }
            }
            if (this.astDiffDetail != null)
            {
                for (int j = 0; j < this.astDiffDetail.Length; j++)
                {
                    this.astDiffDetail[j] = (COMDT_CHAPTER_DIFFICULT_INFO) ProtocolObjectPool.Get(COMDT_CHAPTER_DIFFICULT_INFO.CLASS_ID);
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
                if (4 < this.bLevelNum)
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
                type = destBuf.writeUInt8(this.bDiffNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (4 < this.bDiffNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astDiffDetail.Length < this.bDiffNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int j = 0; j < this.bDiffNum; j++)
                {
                    type = this.astDiffDetail[j].pack(ref destBuf, cutVer);
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
                if (4 < this.bLevelNum)
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
                type = srcBuf.readUInt8(ref this.bDiffNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (4 < this.bDiffNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int j = 0; j < this.bDiffNum; j++)
                {
                    type = this.astDiffDetail[j].unpack(ref srcBuf, cutVer);
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

