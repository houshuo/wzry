namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_RANK_PASTSEASON_FIGHT_RECORD : ProtocolObject
    {
        public COMDT_RANK_COMMON_USED_HERO[] astCommonUsedHeroInfo = new COMDT_RANK_COMMON_USED_HERO[5];
        public static readonly uint BASEVERSION = 1;
        public byte bGrade;
        public static readonly int CLASS_ID = 0x16f;
        public static readonly uint CURRVERSION = 0x79;
        public uint dwClassOfRank;
        public uint dwCommonUsedHeroNum;
        public uint dwMaxContinuousWinCnt;
        public uint dwSeaEndTime;
        public uint dwSeaStartTime;
        public uint dwTotalFightCnt;
        public uint dwTotalWinCnt;
        public static readonly uint VERSION_dwClassOfRank = 0x79;

        public COMDT_RANK_PASTSEASON_FIGHT_RECORD()
        {
            for (int i = 0; i < 5; i++)
            {
                this.astCommonUsedHeroInfo[i] = (COMDT_RANK_COMMON_USED_HERO) ProtocolObjectPool.Get(COMDT_RANK_COMMON_USED_HERO.CLASS_ID);
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
            this.dwSeaStartTime = 0;
            this.dwSeaEndTime = 0;
            this.bGrade = 0;
            this.dwTotalFightCnt = 0;
            this.dwTotalWinCnt = 0;
            this.dwMaxContinuousWinCnt = 0;
            this.dwCommonUsedHeroNum = 0;
            if (this.astCommonUsedHeroInfo != null)
            {
                for (int i = 0; i < this.astCommonUsedHeroInfo.Length; i++)
                {
                    if (this.astCommonUsedHeroInfo[i] != null)
                    {
                        this.astCommonUsedHeroInfo[i].Release();
                        this.astCommonUsedHeroInfo[i] = null;
                    }
                }
            }
            this.dwClassOfRank = 0;
        }

        public override void OnUse()
        {
            if (this.astCommonUsedHeroInfo != null)
            {
                for (int i = 0; i < this.astCommonUsedHeroInfo.Length; i++)
                {
                    this.astCommonUsedHeroInfo[i] = (COMDT_RANK_COMMON_USED_HERO) ProtocolObjectPool.Get(COMDT_RANK_COMMON_USED_HERO.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwSeaStartTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwSeaEndTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bGrade);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwTotalFightCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwTotalWinCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwMaxContinuousWinCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwCommonUsedHeroNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (5 < this.dwCommonUsedHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astCommonUsedHeroInfo.Length < this.dwCommonUsedHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwCommonUsedHeroNum; i++)
                {
                    type = this.astCommonUsedHeroInfo[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwClassOfRank <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwClassOfRank);
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
            type = srcBuf.readUInt32(ref this.dwSeaStartTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwSeaEndTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bGrade);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTotalFightCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTotalWinCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMaxContinuousWinCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCommonUsedHeroNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (5 < this.dwCommonUsedHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwCommonUsedHeroNum; i++)
                {
                    type = this.astCommonUsedHeroInfo[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwClassOfRank <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwClassOfRank);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwClassOfRank = 0;
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

