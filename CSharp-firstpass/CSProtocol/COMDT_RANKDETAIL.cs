namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_RANKDETAIL : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bGetReward;
        public byte bMaxSeasonGrade;
        public byte bState;
        public byte bSubState;
        public static readonly int CLASS_ID = 0xa2;
        public static readonly uint CURRVERSION = 0x79;
        public uint dwAddScoreOfConWinCnt;
        public uint dwContinuousLose;
        public uint dwContinuousWin;
        public uint dwLastActiveTime;
        public uint dwMaxContinuousWinCnt;
        public uint dwMaxSeasonClass;
        public uint dwScore;
        public uint dwSeasonEndTime;
        public uint dwSeasonIdx;
        public uint dwSeasonStartTime;
        public uint dwTopClassOfRank;
        public uint dwTotalFightCnt;
        public uint dwTotalWinCnt;
        public int iMMR;
        public COMDT_RANK_GRADEDOWN stGradeDown = ((COMDT_RANK_GRADEDOWN) ProtocolObjectPool.Get(COMDT_RANK_GRADEDOWN.CLASS_ID));
        public COMDT_RANK_GRADEUP stGradeUp = ((COMDT_RANK_GRADEUP) ProtocolObjectPool.Get(COMDT_RANK_GRADEUP.CLASS_ID));
        public static readonly uint VERSION_bGetReward = 0x21;
        public static readonly uint VERSION_bMaxSeasonGrade = 0x48;
        public static readonly uint VERSION_bSubState = 0x21;
        public static readonly uint VERSION_dwAddScoreOfConWinCnt = 0x67;
        public static readonly uint VERSION_dwLastActiveTime = 0x67;
        public static readonly uint VERSION_dwMaxContinuousWinCnt = 0x21;
        public static readonly uint VERSION_dwMaxSeasonClass = 0x79;
        public static readonly uint VERSION_dwSeasonEndTime = 0x21;
        public static readonly uint VERSION_dwSeasonIdx = 0x21;
        public static readonly uint VERSION_dwSeasonStartTime = 0x21;
        public static readonly uint VERSION_dwTopClassOfRank = 0x79;

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
            this.iMMR = 0;
            this.dwTotalFightCnt = 0;
            this.dwTotalWinCnt = 0;
            this.dwScore = 0;
            this.bState = 0;
            this.dwContinuousWin = 0;
            this.dwContinuousLose = 0;
            if (this.stGradeUp != null)
            {
                this.stGradeUp.Release();
                this.stGradeUp = null;
            }
            if (this.stGradeDown != null)
            {
                this.stGradeDown.Release();
                this.stGradeDown = null;
            }
            this.dwSeasonIdx = 0;
            this.dwSeasonStartTime = 0;
            this.dwSeasonEndTime = 0;
            this.dwMaxContinuousWinCnt = 0;
            this.bGetReward = 0;
            this.bSubState = 0;
            this.bMaxSeasonGrade = 0;
            this.dwLastActiveTime = 0;
            this.dwAddScoreOfConWinCnt = 0;
            this.dwMaxSeasonClass = 0;
            this.dwTopClassOfRank = 0;
        }

        public override void OnUse()
        {
            this.stGradeUp = (COMDT_RANK_GRADEUP) ProtocolObjectPool.Get(COMDT_RANK_GRADEUP.CLASS_ID);
            this.stGradeDown = (COMDT_RANK_GRADEDOWN) ProtocolObjectPool.Get(COMDT_RANK_GRADEDOWN.CLASS_ID);
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
            type = destBuf.writeInt32(this.iMMR);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
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
                type = destBuf.writeUInt32(this.dwScore);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bState);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwContinuousWin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwContinuousLose);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGradeUp.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGradeDown.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwSeasonIdx <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwSeasonIdx);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwSeasonStartTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwSeasonStartTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwSeasonEndTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwSeasonEndTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwMaxContinuousWinCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwMaxContinuousWinCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bGetReward <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bGetReward);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bSubState <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bSubState);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bMaxSeasonGrade <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bMaxSeasonGrade);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastActiveTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastActiveTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwAddScoreOfConWinCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwAddScoreOfConWinCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwMaxSeasonClass <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwMaxSeasonClass);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwTopClassOfRank <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwTopClassOfRank);
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
            type = srcBuf.readInt32(ref this.iMMR);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
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
                type = srcBuf.readUInt32(ref this.dwScore);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bState);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwContinuousWin);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwContinuousLose);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGradeUp.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGradeDown.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwSeasonIdx <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwSeasonIdx);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwSeasonIdx = 0;
                }
                if (VERSION_dwSeasonStartTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwSeasonStartTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwSeasonStartTime = 0;
                }
                if (VERSION_dwSeasonEndTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwSeasonEndTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwSeasonEndTime = 0;
                }
                if (VERSION_dwMaxContinuousWinCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwMaxContinuousWinCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwMaxContinuousWinCnt = 0;
                }
                if (VERSION_bGetReward <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bGetReward);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bGetReward = 0;
                }
                if (VERSION_bSubState <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bSubState);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bSubState = 0;
                }
                if (VERSION_bMaxSeasonGrade <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bMaxSeasonGrade);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bMaxSeasonGrade = 0;
                }
                if (VERSION_dwLastActiveTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastActiveTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastActiveTime = 0;
                }
                if (VERSION_dwAddScoreOfConWinCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwAddScoreOfConWinCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwAddScoreOfConWinCnt = 0;
                }
                if (VERSION_dwMaxSeasonClass <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwMaxSeasonClass);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwMaxSeasonClass = 0;
                }
                if (VERSION_dwTopClassOfRank <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwTopClassOfRank);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwTopClassOfRank = 0;
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

