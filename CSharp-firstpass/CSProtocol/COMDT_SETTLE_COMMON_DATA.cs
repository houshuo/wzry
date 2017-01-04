namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_SETTLE_COMMON_DATA : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x97;
        public static readonly uint CURRVERSION = 1;
        public COMDT_SETTLE_GAME_GENERAL_INFO stGeneralData = ((COMDT_SETTLE_GAME_GENERAL_INFO) ProtocolObjectPool.Get(COMDT_SETTLE_GAME_GENERAL_INFO.CLASS_ID));
        public COMDT_SETTLE_HERO_DETAIL stHeroData = ((COMDT_SETTLE_HERO_DETAIL) ProtocolObjectPool.Get(COMDT_SETTLE_HERO_DETAIL.CLASS_ID));
        public COMDT_NONHERO_DETAIL stNonHeroData = ((COMDT_NONHERO_DETAIL) ProtocolObjectPool.Get(COMDT_NONHERO_DETAIL.CLASS_ID));
        public COMDT_STATISTIC_DATA stStatisticData = ((COMDT_STATISTIC_DATA) ProtocolObjectPool.Get(COMDT_STATISTIC_DATA.CLASS_ID));

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
            if (this.stStatisticData != null)
            {
                this.stStatisticData.Release();
                this.stStatisticData = null;
            }
            if (this.stHeroData != null)
            {
                this.stHeroData.Release();
                this.stHeroData = null;
            }
            if (this.stGeneralData != null)
            {
                this.stGeneralData.Release();
                this.stGeneralData = null;
            }
            if (this.stNonHeroData != null)
            {
                this.stNonHeroData.Release();
                this.stNonHeroData = null;
            }
        }

        public override void OnUse()
        {
            this.stStatisticData = (COMDT_STATISTIC_DATA) ProtocolObjectPool.Get(COMDT_STATISTIC_DATA.CLASS_ID);
            this.stHeroData = (COMDT_SETTLE_HERO_DETAIL) ProtocolObjectPool.Get(COMDT_SETTLE_HERO_DETAIL.CLASS_ID);
            this.stGeneralData = (COMDT_SETTLE_GAME_GENERAL_INFO) ProtocolObjectPool.Get(COMDT_SETTLE_GAME_GENERAL_INFO.CLASS_ID);
            this.stNonHeroData = (COMDT_NONHERO_DETAIL) ProtocolObjectPool.Get(COMDT_NONHERO_DETAIL.CLASS_ID);
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
            type = this.stStatisticData.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stHeroData.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGeneralData.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stNonHeroData.pack(ref destBuf, cutVer);
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
            type = this.stStatisticData.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stHeroData.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGeneralData.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stNonHeroData.unpack(ref srcBuf, cutVer);
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

