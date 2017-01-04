namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_ACNTHEROINFO_NTY : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x2c3;
        public static readonly uint CURRVERSION = 0x8a;
        public COMDT_BATTLELIST_LIST stBattleListInfo = ((COMDT_BATTLELIST_LIST) ProtocolObjectPool.Get(COMDT_BATTLELIST_LIST.CLASS_ID));
        public COMDT_HEROCTRLINFO stHeroCtrlInfo = ((COMDT_HEROCTRLINFO) ProtocolObjectPool.Get(COMDT_HEROCTRLINFO.CLASS_ID));
        public COMDT_HEROINFO_LIST stHeroInfo = ((COMDT_HEROINFO_LIST) ProtocolObjectPool.Get(COMDT_HEROINFO_LIST.CLASS_ID));
        public COMDT_HERO_LIMIT_SKIN_LIST stLimitSkinInfo = ((COMDT_HERO_LIMIT_SKIN_LIST) ProtocolObjectPool.Get(COMDT_HERO_LIMIT_SKIN_LIST.CLASS_ID));
        public COMDT_SELFDEFINE_EQUIP_INFO stSelfDefineEquipInfo = ((COMDT_SELFDEFINE_EQUIP_INFO) ProtocolObjectPool.Get(COMDT_SELFDEFINE_EQUIP_INFO.CLASS_ID));
        public COMDT_HERO_SKIN_LIST stSkinInfo = ((COMDT_HERO_SKIN_LIST) ProtocolObjectPool.Get(COMDT_HERO_SKIN_LIST.CLASS_ID));

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = this.stHeroCtrlInfo.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stSkinInfo.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBattleListInfo.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroInfo.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stLimitSkinInfo.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSelfDefineEquipInfo.construct();
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
            if (this.stHeroCtrlInfo != null)
            {
                this.stHeroCtrlInfo.Release();
                this.stHeroCtrlInfo = null;
            }
            if (this.stSkinInfo != null)
            {
                this.stSkinInfo.Release();
                this.stSkinInfo = null;
            }
            if (this.stBattleListInfo != null)
            {
                this.stBattleListInfo.Release();
                this.stBattleListInfo = null;
            }
            if (this.stHeroInfo != null)
            {
                this.stHeroInfo.Release();
                this.stHeroInfo = null;
            }
            if (this.stLimitSkinInfo != null)
            {
                this.stLimitSkinInfo.Release();
                this.stLimitSkinInfo = null;
            }
            if (this.stSelfDefineEquipInfo != null)
            {
                this.stSelfDefineEquipInfo.Release();
                this.stSelfDefineEquipInfo = null;
            }
        }

        public override void OnUse()
        {
            this.stHeroCtrlInfo = (COMDT_HEROCTRLINFO) ProtocolObjectPool.Get(COMDT_HEROCTRLINFO.CLASS_ID);
            this.stSkinInfo = (COMDT_HERO_SKIN_LIST) ProtocolObjectPool.Get(COMDT_HERO_SKIN_LIST.CLASS_ID);
            this.stBattleListInfo = (COMDT_BATTLELIST_LIST) ProtocolObjectPool.Get(COMDT_BATTLELIST_LIST.CLASS_ID);
            this.stHeroInfo = (COMDT_HEROINFO_LIST) ProtocolObjectPool.Get(COMDT_HEROINFO_LIST.CLASS_ID);
            this.stLimitSkinInfo = (COMDT_HERO_LIMIT_SKIN_LIST) ProtocolObjectPool.Get(COMDT_HERO_LIMIT_SKIN_LIST.CLASS_ID);
            this.stSelfDefineEquipInfo = (COMDT_SELFDEFINE_EQUIP_INFO) ProtocolObjectPool.Get(COMDT_SELFDEFINE_EQUIP_INFO.CLASS_ID);
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
            type = this.stHeroCtrlInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stSkinInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBattleListInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stLimitSkinInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSelfDefineEquipInfo.pack(ref destBuf, cutVer);
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
            type = this.stHeroCtrlInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stSkinInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBattleListInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stLimitSkinInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSelfDefineEquipInfo.unpack(ref srcBuf, cutVer);
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

