namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_BURNING_LEVEL_DETAIL : ProtocolObject
    {
        public COMDT_BURNING_LEVEL_INFO[] astLevelDetail = new COMDT_BURNING_LEVEL_INFO[10];
        public static readonly uint BASEVERSION = 1;
        public byte bDifficultType;
        public byte bLevelNum;
        public byte bResetNum;
        public static readonly int CLASS_ID = 0x133;
        public static readonly uint CURRVERSION = 0x8a;
        public uint dwLastPlayTime;
        public COMDT_BURNING_HERO_DETAIL stHeroDetail = ((COMDT_BURNING_HERO_DETAIL) ProtocolObjectPool.Get(COMDT_BURNING_HERO_DETAIL.CLASS_ID));

        public COMDT_BURNING_LEVEL_DETAIL()
        {
            for (int i = 0; i < 10; i++)
            {
                this.astLevelDetail[i] = (COMDT_BURNING_LEVEL_INFO) ProtocolObjectPool.Get(COMDT_BURNING_LEVEL_INFO.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.bDifficultType = 0;
            this.dwLastPlayTime = 0;
            this.bResetNum = 0;
            type = this.stHeroDetail.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                this.bLevelNum = 0;
                for (int i = 0; i < 10; i++)
                {
                    type = this.astLevelDetail[i].construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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
            this.bDifficultType = 0;
            this.dwLastPlayTime = 0;
            this.bResetNum = 0;
            if (this.stHeroDetail != null)
            {
                this.stHeroDetail.Release();
                this.stHeroDetail = null;
            }
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
            this.stHeroDetail = (COMDT_BURNING_HERO_DETAIL) ProtocolObjectPool.Get(COMDT_BURNING_HERO_DETAIL.CLASS_ID);
            if (this.astLevelDetail != null)
            {
                for (int i = 0; i < this.astLevelDetail.Length; i++)
                {
                    this.astLevelDetail[i] = (COMDT_BURNING_LEVEL_INFO) ProtocolObjectPool.Get(COMDT_BURNING_LEVEL_INFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bDifficultType);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwLastPlayTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bResetNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroDetail.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bLevelNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
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
            type = srcBuf.readUInt8(ref this.bDifficultType);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwLastPlayTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bResetNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroDetail.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bLevelNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
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

