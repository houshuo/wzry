namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_SETTLE_RESULT_DETAIL : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0xd1;
        public static readonly uint CURRVERSION = 0x71;
        public COMDT_ACNT_INFO stAcntInfo = ((COMDT_ACNT_INFO) ProtocolObjectPool.Get(COMDT_ACNT_INFO.CLASS_ID));
        public COMDT_GAME_INFO stGameInfo = ((COMDT_GAME_INFO) ProtocolObjectPool.Get(COMDT_GAME_INFO.CLASS_ID));
        public COMDT_SETTLE_HERO_RESULT_DETAIL stHeroList = ((COMDT_SETTLE_HERO_RESULT_DETAIL) ProtocolObjectPool.Get(COMDT_SETTLE_HERO_RESULT_DETAIL.CLASS_ID));
        public COMDT_REWARD_MULTIPLE_DETAIL stMultipleDetail = ((COMDT_REWARD_MULTIPLE_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID));
        public COMDT_RANK_SETTLE_INFO stRankInfo = ((COMDT_RANK_SETTLE_INFO) ProtocolObjectPool.Get(COMDT_RANK_SETTLE_INFO.CLASS_ID));
        public COMDT_REWARD_DETAIL stReward = ((COMDT_REWARD_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID));
        public COMDT_PVPSPECITEM_OUTPUT stSpecReward = ((COMDT_PVPSPECITEM_OUTPUT) ProtocolObjectPool.Get(COMDT_PVPSPECITEM_OUTPUT.CLASS_ID));

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
            if (this.stGameInfo != null)
            {
                this.stGameInfo.Release();
                this.stGameInfo = null;
            }
            if (this.stAcntInfo != null)
            {
                this.stAcntInfo.Release();
                this.stAcntInfo = null;
            }
            if (this.stRankInfo != null)
            {
                this.stRankInfo.Release();
                this.stRankInfo = null;
            }
            if (this.stHeroList != null)
            {
                this.stHeroList.Release();
                this.stHeroList = null;
            }
            if (this.stReward != null)
            {
                this.stReward.Release();
                this.stReward = null;
            }
            if (this.stMultipleDetail != null)
            {
                this.stMultipleDetail.Release();
                this.stMultipleDetail = null;
            }
            if (this.stSpecReward != null)
            {
                this.stSpecReward.Release();
                this.stSpecReward = null;
            }
        }

        public override void OnUse()
        {
            this.stGameInfo = (COMDT_GAME_INFO) ProtocolObjectPool.Get(COMDT_GAME_INFO.CLASS_ID);
            this.stAcntInfo = (COMDT_ACNT_INFO) ProtocolObjectPool.Get(COMDT_ACNT_INFO.CLASS_ID);
            this.stRankInfo = (COMDT_RANK_SETTLE_INFO) ProtocolObjectPool.Get(COMDT_RANK_SETTLE_INFO.CLASS_ID);
            this.stHeroList = (COMDT_SETTLE_HERO_RESULT_DETAIL) ProtocolObjectPool.Get(COMDT_SETTLE_HERO_RESULT_DETAIL.CLASS_ID);
            this.stReward = (COMDT_REWARD_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
            this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
            this.stSpecReward = (COMDT_PVPSPECITEM_OUTPUT) ProtocolObjectPool.Get(COMDT_PVPSPECITEM_OUTPUT.CLASS_ID);
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
            type = this.stGameInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stAcntInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stRankInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroList.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stReward.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMultipleDetail.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSpecReward.pack(ref destBuf, cutVer);
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
            type = this.stGameInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stAcntInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stRankInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroList.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stReward.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMultipleDetail.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSpecReward.unpack(ref srcBuf, cutVer);
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

