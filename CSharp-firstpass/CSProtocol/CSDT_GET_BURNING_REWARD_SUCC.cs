namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_GET_BURNING_REWARD_SUCC : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bNextLevelNo;
        public static readonly int CLASS_ID = 0x428;
        public static readonly uint CURRVERSION = 0x8a;
        public COMDT_REWARD_MULTIPLE_DETAIL stMultipleDetail = ((COMDT_REWARD_MULTIPLE_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID));
        public COMDT_BURNING_ENEMY_TEAM_INFO stNextEnemyInfo = ((COMDT_BURNING_ENEMY_TEAM_INFO) ProtocolObjectPool.Get(COMDT_BURNING_ENEMY_TEAM_INFO.CLASS_ID));
        public COMDT_REWARD_DETAIL stReward = ((COMDT_REWARD_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID));

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = this.stReward.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                this.bNextLevelNo = 0;
                type = this.stNextEnemyInfo.construct();
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
            if (this.stReward != null)
            {
                this.stReward.Release();
                this.stReward = null;
            }
            this.bNextLevelNo = 0;
            if (this.stNextEnemyInfo != null)
            {
                this.stNextEnemyInfo.Release();
                this.stNextEnemyInfo = null;
            }
            if (this.stMultipleDetail != null)
            {
                this.stMultipleDetail.Release();
                this.stMultipleDetail = null;
            }
        }

        public override void OnUse()
        {
            this.stReward = (COMDT_REWARD_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
            this.stNextEnemyInfo = (COMDT_BURNING_ENEMY_TEAM_INFO) ProtocolObjectPool.Get(COMDT_BURNING_ENEMY_TEAM_INFO.CLASS_ID);
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
            type = this.stReward.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt8(this.bNextLevelNo);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stNextEnemyInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMultipleDetail.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
            type = this.stReward.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt8(ref this.bNextLevelNo);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stNextEnemyInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMultipleDetail.unpack(ref srcBuf, cutVer);
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

