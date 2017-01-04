namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_GAMEACNT_DETAIL : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0xb6;
        public static readonly uint CURRVERSION = 0x79;
        public COMDT_ACNT_BANTIME stBanTime = ((COMDT_ACNT_BANTIME) ProtocolObjectPool.Get(COMDT_ACNT_BANTIME.CLASS_ID));
        public COMDT_ACNT_EXTRAINFO stBriefInfo = ((COMDT_ACNT_EXTRAINFO) ProtocolObjectPool.Get(COMDT_ACNT_EXTRAINFO.CLASS_ID));
        public COMDT_FREEHERO_INACNT stFreeHeroRcd = ((COMDT_FREEHERO_INACNT) ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID));
        public COMDT_GAME_VIP_CLIENT stGameVip = ((COMDT_GAME_VIP_CLIENT) ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID));
        public COMDT_ACNT_MATCH_BRIEF_INFO stMatchInfo = ((COMDT_ACNT_MATCH_BRIEF_INFO) ProtocolObjectPool.Get(COMDT_ACNT_MATCH_BRIEF_INFO.CLASS_ID));
        public COMDT_GAMEACNT_SYMBOLPAGE stSymbolPage = ((COMDT_GAMEACNT_SYMBOLPAGE) ProtocolObjectPool.Get(COMDT_GAMEACNT_SYMBOLPAGE.CLASS_ID));

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
            if (this.stMatchInfo != null)
            {
                this.stMatchInfo.Release();
                this.stMatchInfo = null;
            }
            if (this.stSymbolPage != null)
            {
                this.stSymbolPage.Release();
                this.stSymbolPage = null;
            }
            if (this.stFreeHeroRcd != null)
            {
                this.stFreeHeroRcd.Release();
                this.stFreeHeroRcd = null;
            }
            if (this.stBanTime != null)
            {
                this.stBanTime.Release();
                this.stBanTime = null;
            }
            if (this.stBriefInfo != null)
            {
                this.stBriefInfo.Release();
                this.stBriefInfo = null;
            }
            if (this.stGameVip != null)
            {
                this.stGameVip.Release();
                this.stGameVip = null;
            }
        }

        public override void OnUse()
        {
            this.stMatchInfo = (COMDT_ACNT_MATCH_BRIEF_INFO) ProtocolObjectPool.Get(COMDT_ACNT_MATCH_BRIEF_INFO.CLASS_ID);
            this.stSymbolPage = (COMDT_GAMEACNT_SYMBOLPAGE) ProtocolObjectPool.Get(COMDT_GAMEACNT_SYMBOLPAGE.CLASS_ID);
            this.stFreeHeroRcd = (COMDT_FREEHERO_INACNT) ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
            this.stBanTime = (COMDT_ACNT_BANTIME) ProtocolObjectPool.Get(COMDT_ACNT_BANTIME.CLASS_ID);
            this.stBriefInfo = (COMDT_ACNT_EXTRAINFO) ProtocolObjectPool.Get(COMDT_ACNT_EXTRAINFO.CLASS_ID);
            this.stGameVip = (COMDT_GAME_VIP_CLIENT) ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
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
            type = this.stMatchInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stSymbolPage.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stFreeHeroRcd.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBanTime.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBriefInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGameVip.pack(ref destBuf, cutVer);
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
            type = this.stMatchInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stSymbolPage.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stFreeHeroRcd.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBanTime.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBriefInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGameVip.unpack(ref srcBuf, cutVer);
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

