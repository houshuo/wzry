namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_CHOICEHERO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 110;
        public static readonly uint CURRVERSION = 0x8a;
        public uint[] HeroEquipList = new uint[6];
        public COMDT_HEROINFO stBaseInfo = ((COMDT_HEROINFO) ProtocolObjectPool.Get(COMDT_HEROINFO.CLASS_ID));
        public COMDT_HERO_BURNING_INFO stBurningInfo = ((COMDT_HERO_BURNING_INFO) ProtocolObjectPool.Get(COMDT_HERO_BURNING_INFO.CLASS_ID));
        public COMDT_HEROEXTRALINFO stHeroExtral = ((COMDT_HEROEXTRALINFO) ProtocolObjectPool.Get(COMDT_HEROEXTRALINFO.CLASS_ID));
        public uint[] SymbolID = new uint[30];
        public static readonly uint VERSION_HeroEquipList = 0x51;

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = this.stBaseInfo.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stBurningInfo.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroExtral.construct();
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
            if (this.stBaseInfo != null)
            {
                this.stBaseInfo.Release();
                this.stBaseInfo = null;
            }
            if (this.stBurningInfo != null)
            {
                this.stBurningInfo.Release();
                this.stBurningInfo = null;
            }
            if (this.stHeroExtral != null)
            {
                this.stHeroExtral.Release();
                this.stHeroExtral = null;
            }
        }

        public override void OnUse()
        {
            this.stBaseInfo = (COMDT_HEROINFO) ProtocolObjectPool.Get(COMDT_HEROINFO.CLASS_ID);
            this.stBurningInfo = (COMDT_HERO_BURNING_INFO) ProtocolObjectPool.Get(COMDT_HERO_BURNING_INFO.CLASS_ID);
            this.stHeroExtral = (COMDT_HEROEXTRALINFO) ProtocolObjectPool.Get(COMDT_HEROEXTRALINFO.CLASS_ID);
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
            type = this.stBaseInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 30; i++)
                {
                    type = destBuf.writeUInt32(this.SymbolID[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stBurningInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroExtral.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_HeroEquipList > cutVer)
                {
                    return type;
                }
                for (int j = 0; j < 6; j++)
                {
                    type = destBuf.writeUInt32(this.HeroEquipList[j]);
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
            type = this.stBaseInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 30; i++)
                {
                    type = srcBuf.readUInt32(ref this.SymbolID[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stBurningInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroExtral.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_HeroEquipList > cutVer)
                {
                    return type;
                }
                for (int j = 0; j < 6; j++)
                {
                    type = srcBuf.readUInt32(ref this.HeroEquipList[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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

