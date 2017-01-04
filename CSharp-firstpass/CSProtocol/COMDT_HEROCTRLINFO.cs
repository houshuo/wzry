namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_HEROCTRLINFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x69;
        public static readonly uint CURRVERSION = 0x1d;
        public uint dwInitHeroID;
        public COMDT_BATTLEHERO stBattleListOfArena = ((COMDT_BATTLEHERO) ProtocolObjectPool.Get(COMDT_BATTLEHERO.CLASS_ID));
        public COMDT_FREEHERO_INACNT stFreeHeroOfArena = ((COMDT_FREEHERO_INACNT) ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID));
        public static readonly uint VERSION_stFreeHeroOfArena = 0x17;

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
            this.dwInitHeroID = 0;
            if (this.stBattleListOfArena != null)
            {
                this.stBattleListOfArena.Release();
                this.stBattleListOfArena = null;
            }
            if (this.stFreeHeroOfArena != null)
            {
                this.stFreeHeroOfArena.Release();
                this.stFreeHeroOfArena = null;
            }
        }

        public override void OnUse()
        {
            this.stBattleListOfArena = (COMDT_BATTLEHERO) ProtocolObjectPool.Get(COMDT_BATTLEHERO.CLASS_ID);
            this.stFreeHeroOfArena = (COMDT_FREEHERO_INACNT) ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwInitHeroID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stBattleListOfArena.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stFreeHeroOfArena <= cutVer)
                {
                    type = this.stFreeHeroOfArena.pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwInitHeroID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stBattleListOfArena.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stFreeHeroOfArena <= cutVer)
                {
                    type = this.stFreeHeroOfArena.unpack(ref srcBuf, cutVer);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                type = this.stFreeHeroOfArena.construct();
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

