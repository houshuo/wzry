namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_HERO_COMMON_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bSymbolPageWear;
        public static readonly int CLASS_ID = 0x62;
        public static readonly uint CURRVERSION = 0x8a;
        public uint dwDeadLine;
        public uint dwExp;
        public uint dwGameLoseNum;
        public uint dwGameWinNum;
        public uint dwHeroID;
        public uint dwLastMasterGameSec;
        public uint dwMaskBits;
        public uint dwMasterTotalFightCnt;
        public uint dwMasterTotalWinCnt;
        public uint dwRankGameTotalFightCnt;
        public uint dwRankGameTotalWinCnt;
        public COMDT_HERO_PROFICIENCY stProficiency = ((COMDT_HERO_PROFICIENCY) ProtocolObjectPool.Get(COMDT_HERO_PROFICIENCY.CLASS_ID));
        public COMDT_ACNTHERO_QUALITY stQuality = ((COMDT_ACNTHERO_QUALITY) ProtocolObjectPool.Get(COMDT_ACNTHERO_QUALITY.CLASS_ID));
        public COMDT_SKILLARRAY stSkill = ((COMDT_SKILLARRAY) ProtocolObjectPool.Get(COMDT_SKILLARRAY.CLASS_ID));
        public COMDT_HERO_STATISTIC_DETAIL stStatisticDetail = ((COMDT_HERO_STATISTIC_DETAIL) ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_DETAIL.CLASS_ID));
        public COMDT_TALENTARRAY stTalent = ((COMDT_TALENTARRAY) ProtocolObjectPool.Get(COMDT_TALENTARRAY.CLASS_ID));
        public static readonly uint VERSION_dwDeadLine = 0x33;
        public static readonly uint VERSION_dwGameLoseNum = 0x16;
        public static readonly uint VERSION_dwGameWinNum = 0x16;
        public static readonly uint VERSION_dwLastMasterGameSec = 0x8a;
        public static readonly uint VERSION_dwMasterTotalFightCnt = 0x8a;
        public static readonly uint VERSION_dwMasterTotalWinCnt = 0x8a;
        public static readonly uint VERSION_dwRankGameTotalFightCnt = 0x21;
        public static readonly uint VERSION_dwRankGameTotalWinCnt = 0x21;
        public static readonly uint VERSION_stStatisticDetail = 0x88;
        public static readonly uint VERSION_stTalent = 0x1a;
        public ushort wLevel;
        public ushort wSkinID;
        public ushort wStar;

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.dwHeroID = 0;
            this.dwMaskBits = 0;
            this.wLevel = 1;
            this.wStar = 0;
            type = this.stQuality.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                this.dwExp = 0;
                type = this.stSkill.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stProficiency.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                this.bSymbolPageWear = 0;
                this.wSkinID = 0;
                this.dwGameWinNum = 0;
                this.dwGameLoseNum = 0;
                type = this.stTalent.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                this.dwRankGameTotalFightCnt = 0;
                this.dwRankGameTotalWinCnt = 0;
                this.dwDeadLine = 0;
                type = this.stStatisticDetail.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                this.dwLastMasterGameSec = 0;
                this.dwMasterTotalFightCnt = 0;
                this.dwMasterTotalWinCnt = 0;
            }
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            this.dwHeroID = 0;
            this.dwMaskBits = 0;
            this.wLevel = 0;
            this.wStar = 0;
            if (this.stQuality != null)
            {
                this.stQuality.Release();
                this.stQuality = null;
            }
            this.dwExp = 0;
            if (this.stSkill != null)
            {
                this.stSkill.Release();
                this.stSkill = null;
            }
            if (this.stProficiency != null)
            {
                this.stProficiency.Release();
                this.stProficiency = null;
            }
            this.bSymbolPageWear = 0;
            this.wSkinID = 0;
            this.dwGameWinNum = 0;
            this.dwGameLoseNum = 0;
            if (this.stTalent != null)
            {
                this.stTalent.Release();
                this.stTalent = null;
            }
            this.dwRankGameTotalFightCnt = 0;
            this.dwRankGameTotalWinCnt = 0;
            this.dwDeadLine = 0;
            if (this.stStatisticDetail != null)
            {
                this.stStatisticDetail.Release();
                this.stStatisticDetail = null;
            }
            this.dwLastMasterGameSec = 0;
            this.dwMasterTotalFightCnt = 0;
            this.dwMasterTotalWinCnt = 0;
        }

        public override void OnUse()
        {
            this.stQuality = (COMDT_ACNTHERO_QUALITY) ProtocolObjectPool.Get(COMDT_ACNTHERO_QUALITY.CLASS_ID);
            this.stSkill = (COMDT_SKILLARRAY) ProtocolObjectPool.Get(COMDT_SKILLARRAY.CLASS_ID);
            this.stProficiency = (COMDT_HERO_PROFICIENCY) ProtocolObjectPool.Get(COMDT_HERO_PROFICIENCY.CLASS_ID);
            this.stTalent = (COMDT_TALENTARRAY) ProtocolObjectPool.Get(COMDT_TALENTARRAY.CLASS_ID);
            this.stStatisticDetail = (COMDT_HERO_STATISTIC_DETAIL) ProtocolObjectPool.Get(COMDT_HERO_STATISTIC_DETAIL.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwHeroID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwMaskBits);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt16(this.wLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt16(this.wStar);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stQuality.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwExp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSkill.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stProficiency.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bSymbolPageWear);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt16(this.wSkinID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwGameWinNum <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwGameWinNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwGameLoseNum <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwGameLoseNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stTalent <= cutVer)
                {
                    type = this.stTalent.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwRankGameTotalFightCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwRankGameTotalFightCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwRankGameTotalWinCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwRankGameTotalWinCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwDeadLine <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwDeadLine);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stStatisticDetail <= cutVer)
                {
                    type = this.stStatisticDetail.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastMasterGameSec <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastMasterGameSec);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwMasterTotalFightCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwMasterTotalFightCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwMasterTotalWinCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwMasterTotalWinCnt);
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
            type = srcBuf.readUInt32(ref this.dwHeroID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwMaskBits);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wStar);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stQuality.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwExp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSkill.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stProficiency.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSymbolPageWear);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wSkinID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwGameWinNum <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwGameWinNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwGameWinNum = 0;
                }
                if (VERSION_dwGameLoseNum <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwGameLoseNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwGameLoseNum = 0;
                }
                if (VERSION_stTalent <= cutVer)
                {
                    type = this.stTalent.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    type = this.stTalent.construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwRankGameTotalFightCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwRankGameTotalFightCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwRankGameTotalFightCnt = 0;
                }
                if (VERSION_dwRankGameTotalWinCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwRankGameTotalWinCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwRankGameTotalWinCnt = 0;
                }
                if (VERSION_dwDeadLine <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwDeadLine);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwDeadLine = 0;
                }
                if (VERSION_stStatisticDetail <= cutVer)
                {
                    type = this.stStatisticDetail.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    type = this.stStatisticDetail.construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastMasterGameSec <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastMasterGameSec);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastMasterGameSec = 0;
                }
                if (VERSION_dwMasterTotalFightCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwMasterTotalFightCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwMasterTotalFightCnt = 0;
                }
                if (VERSION_dwMasterTotalWinCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwMasterTotalWinCnt);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwMasterTotalWinCnt = 0;
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

