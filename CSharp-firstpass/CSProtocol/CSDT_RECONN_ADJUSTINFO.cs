namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_RECONN_ADJUSTINFO : ProtocolObject
    {
        public CSDT_RECONN_CAMPPICKINFO[] astCampInfo = new CSDT_RECONN_CAMPPICKINFO[2];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x41c;
        public static readonly uint CURRVERSION = 0x8a;
        public uint dwLeftMs;
        public COMDT_DESKINFO stDeskInfo = ((COMDT_DESKINFO) ProtocolObjectPool.Get(COMDT_DESKINFO.CLASS_ID));
        public COMDT_FREEHERO stFreeHero;
        public COMDT_FREEHERO_INACNT stFreeHeroSymbol;
        public CSDT_RECONN_ADJUST_STATE_EXTRA stHeroSwapInfo;

        public CSDT_RECONN_ADJUSTINFO()
        {
            for (int i = 0; i < 2; i++)
            {
                this.astCampInfo[i] = (CSDT_RECONN_CAMPPICKINFO) ProtocolObjectPool.Get(CSDT_RECONN_CAMPPICKINFO.CLASS_ID);
            }
            this.stFreeHero = (COMDT_FREEHERO) ProtocolObjectPool.Get(COMDT_FREEHERO.CLASS_ID);
            this.stFreeHeroSymbol = (COMDT_FREEHERO_INACNT) ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
            this.stHeroSwapInfo = (CSDT_RECONN_ADJUST_STATE_EXTRA) ProtocolObjectPool.Get(CSDT_RECONN_ADJUST_STATE_EXTRA.CLASS_ID);
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = this.stDeskInfo.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                this.dwLeftMs = 0;
                for (int i = 0; i < 2; i++)
                {
                    type = this.astCampInfo[i].construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stFreeHero.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stFreeHeroSymbol.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroSwapInfo.construct();
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
            if (this.stDeskInfo != null)
            {
                this.stDeskInfo.Release();
                this.stDeskInfo = null;
            }
            this.dwLeftMs = 0;
            if (this.astCampInfo != null)
            {
                for (int i = 0; i < this.astCampInfo.Length; i++)
                {
                    if (this.astCampInfo[i] != null)
                    {
                        this.astCampInfo[i].Release();
                        this.astCampInfo[i] = null;
                    }
                }
            }
            if (this.stFreeHero != null)
            {
                this.stFreeHero.Release();
                this.stFreeHero = null;
            }
            if (this.stFreeHeroSymbol != null)
            {
                this.stFreeHeroSymbol.Release();
                this.stFreeHeroSymbol = null;
            }
            if (this.stHeroSwapInfo != null)
            {
                this.stHeroSwapInfo.Release();
                this.stHeroSwapInfo = null;
            }
        }

        public override void OnUse()
        {
            this.stDeskInfo = (COMDT_DESKINFO) ProtocolObjectPool.Get(COMDT_DESKINFO.CLASS_ID);
            if (this.astCampInfo != null)
            {
                for (int i = 0; i < this.astCampInfo.Length; i++)
                {
                    this.astCampInfo[i] = (CSDT_RECONN_CAMPPICKINFO) ProtocolObjectPool.Get(CSDT_RECONN_CAMPPICKINFO.CLASS_ID);
                }
            }
            this.stFreeHero = (COMDT_FREEHERO) ProtocolObjectPool.Get(COMDT_FREEHERO.CLASS_ID);
            this.stFreeHeroSymbol = (COMDT_FREEHERO_INACNT) ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
            this.stHeroSwapInfo = (CSDT_RECONN_ADJUST_STATE_EXTRA) ProtocolObjectPool.Get(CSDT_RECONN_ADJUST_STATE_EXTRA.CLASS_ID);
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
            type = this.stDeskInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwLeftMs);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 2; i++)
                {
                    type = this.astCampInfo[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stFreeHero.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stFreeHeroSymbol.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroSwapInfo.pack(ref destBuf, cutVer);
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
            type = this.stDeskInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwLeftMs);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 2; i++)
                {
                    type = this.astCampInfo[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stFreeHero.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stFreeHeroSymbol.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroSwapInfo.unpack(ref srcBuf, cutVer);
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

