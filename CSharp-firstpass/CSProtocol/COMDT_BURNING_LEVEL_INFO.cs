namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_BURNING_LEVEL_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bLevelNo;
        public byte bLevelStatus;
        public byte bRewardStatus;
        public static readonly int CLASS_ID = 0x131;
        public static readonly uint CURRVERSION = 0x8a;
        public uint dwAccPassCnt;
        public int iLevelID;
        public COMDT_BURNING_ENEMY_TEAM_INFO stEnemyDetail = ((COMDT_BURNING_ENEMY_TEAM_INFO) ProtocolObjectPool.Get(COMDT_BURNING_ENEMY_TEAM_INFO.CLASS_ID));
        public COMDT_BURNING_LUCKY_BUFF_INFO stLuckyBuffDetail = ((COMDT_BURNING_LUCKY_BUFF_INFO) ProtocolObjectPool.Get(COMDT_BURNING_LUCKY_BUFF_INFO.CLASS_ID));
        public COMDT_REWARD_MULTIPLE_DETAIL stMultipleDetail = ((COMDT_REWARD_MULTIPLE_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID));
        public static readonly uint VERSION_stMultipleDetail = 5;

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.bLevelNo = 0;
            this.iLevelID = 0;
            this.bLevelStatus = 0;
            this.bRewardStatus = 0;
            this.dwAccPassCnt = 0;
            type = this.stEnemyDetail.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stLuckyBuffDetail.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMultipleDetail.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
            this.bLevelNo = 0;
            this.iLevelID = 0;
            this.bLevelStatus = 0;
            this.bRewardStatus = 0;
            this.dwAccPassCnt = 0;
            if (this.stEnemyDetail != null)
            {
                this.stEnemyDetail.Release();
                this.stEnemyDetail = null;
            }
            if (this.stLuckyBuffDetail != null)
            {
                this.stLuckyBuffDetail.Release();
                this.stLuckyBuffDetail = null;
            }
            if (this.stMultipleDetail != null)
            {
                this.stMultipleDetail.Release();
                this.stMultipleDetail = null;
            }
        }

        public override void OnUse()
        {
            this.stEnemyDetail = (COMDT_BURNING_ENEMY_TEAM_INFO) ProtocolObjectPool.Get(COMDT_BURNING_ENEMY_TEAM_INFO.CLASS_ID);
            this.stLuckyBuffDetail = (COMDT_BURNING_LUCKY_BUFF_INFO) ProtocolObjectPool.Get(COMDT_BURNING_LUCKY_BUFF_INFO.CLASS_ID);
            this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bLevelNo);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeInt32(this.iLevelID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bLevelStatus);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bRewardStatus);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwAccPassCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stEnemyDetail.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stLuckyBuffDetail.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stMultipleDetail <= cutVer)
                {
                    type = this.stMultipleDetail.pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bLevelNo);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readInt32(ref this.iLevelID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bLevelStatus);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bRewardStatus);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwAccPassCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stEnemyDetail.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stLuckyBuffDetail.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stMultipleDetail <= cutVer)
                {
                    type = this.stMultipleDetail.unpack(ref srcBuf, cutVer);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                type = this.stMultipleDetail.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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

