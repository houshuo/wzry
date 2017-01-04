namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ACNT_TRANK_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x1dc;
        public static readonly uint CURRVERSION = 0x22;
        public uint dwCurContinousWinNum;
        public uint dwCurWeekUseCouponsNum;
        public uint dwLastAchievementNo;
        public uint dwLastAllPower;
        public uint dwLastContinousWinNo;
        public uint dwLastHeroNumNo;
        public uint dwLastLadderPointNo;
        public uint dwLastPowerRankNo;
        public uint dwLastPvpExpRankNo;
        public uint dwLastSkinNumNo;
        public uint dwLastUseCouponsNo;
        public uint dwLastUseCouponsTime;
        public uint dwLastVipScoreNo;
        public uint dwLastWinGameNumNo;
        public uint dwWeekTopConWinNum;
        public uint dwWeekTopConWinTime;
        public static readonly uint VERSION_dwCurContinousWinNum = 0x1b;
        public static readonly uint VERSION_dwCurWeekUseCouponsNum = 30;
        public static readonly uint VERSION_dwLastAchievementNo = 0x1b;
        public static readonly uint VERSION_dwLastContinousWinNo = 0x1b;
        public static readonly uint VERSION_dwLastHeroNumNo = 20;
        public static readonly uint VERSION_dwLastLadderPointNo = 0x1b;
        public static readonly uint VERSION_dwLastSkinNumNo = 20;
        public static readonly uint VERSION_dwLastUseCouponsNo = 30;
        public static readonly uint VERSION_dwLastUseCouponsTime = 30;
        public static readonly uint VERSION_dwLastVipScoreNo = 0x22;
        public static readonly uint VERSION_dwLastWinGameNumNo = 0x1b;
        public static readonly uint VERSION_dwWeekTopConWinNum = 0x1b;
        public static readonly uint VERSION_dwWeekTopConWinTime = 0x1b;

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
            this.dwLastAllPower = 0;
            this.dwLastPowerRankNo = 0;
            this.dwLastPvpExpRankNo = 0;
            this.dwLastHeroNumNo = 0;
            this.dwLastSkinNumNo = 0;
            this.dwLastLadderPointNo = 0;
            this.dwLastAchievementNo = 0;
            this.dwLastWinGameNumNo = 0;
            this.dwLastContinousWinNo = 0;
            this.dwWeekTopConWinTime = 0;
            this.dwWeekTopConWinNum = 0;
            this.dwCurContinousWinNum = 0;
            this.dwLastUseCouponsNo = 0;
            this.dwLastUseCouponsTime = 0;
            this.dwCurWeekUseCouponsNum = 0;
            this.dwLastVipScoreNo = 0;
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
            type = destBuf.writeUInt32(this.dwLastAllPower);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwLastPowerRankNo);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwLastPvpExpRankNo);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwLastHeroNumNo <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastHeroNumNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastSkinNumNo <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastSkinNumNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastLadderPointNo <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastLadderPointNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastAchievementNo <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastAchievementNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastWinGameNumNo <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastWinGameNumNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastContinousWinNo <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastContinousWinNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwWeekTopConWinTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwWeekTopConWinTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwWeekTopConWinNum <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwWeekTopConWinNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwCurContinousWinNum <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwCurContinousWinNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastUseCouponsNo <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastUseCouponsNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastUseCouponsTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastUseCouponsTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwCurWeekUseCouponsNum <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwCurWeekUseCouponsNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastVipScoreNo <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastVipScoreNo);
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
            type = srcBuf.readUInt32(ref this.dwLastAllPower);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwLastPowerRankNo);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwLastPvpExpRankNo);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwLastHeroNumNo <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastHeroNumNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastHeroNumNo = 0;
                }
                if (VERSION_dwLastSkinNumNo <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastSkinNumNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastSkinNumNo = 0;
                }
                if (VERSION_dwLastLadderPointNo <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastLadderPointNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastLadderPointNo = 0;
                }
                if (VERSION_dwLastAchievementNo <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastAchievementNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastAchievementNo = 0;
                }
                if (VERSION_dwLastWinGameNumNo <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastWinGameNumNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastWinGameNumNo = 0;
                }
                if (VERSION_dwLastContinousWinNo <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastContinousWinNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastContinousWinNo = 0;
                }
                if (VERSION_dwWeekTopConWinTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwWeekTopConWinTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwWeekTopConWinTime = 0;
                }
                if (VERSION_dwWeekTopConWinNum <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwWeekTopConWinNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwWeekTopConWinNum = 0;
                }
                if (VERSION_dwCurContinousWinNum <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwCurContinousWinNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwCurContinousWinNum = 0;
                }
                if (VERSION_dwLastUseCouponsNo <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastUseCouponsNo);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastUseCouponsNo = 0;
                }
                if (VERSION_dwLastUseCouponsTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastUseCouponsTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastUseCouponsTime = 0;
                }
                if (VERSION_dwCurWeekUseCouponsNum <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwCurWeekUseCouponsNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwCurWeekUseCouponsNum = 0;
                }
                if (VERSION_dwLastVipScoreNo <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastVipScoreNo);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwLastVipScoreNo = 0;
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

