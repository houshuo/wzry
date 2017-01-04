namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_GET_GUILD_INFO_RSP : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bResult;
        public static readonly int CLASS_ID = 0x434;
        public static readonly uint CURRVERSION = 0x84;
        public COMDT_GUILD_INFO stGuildInfo = ((COMDT_GUILD_INFO) ProtocolObjectPool.Get(COMDT_GUILD_INFO.CLASS_ID));
        public COMDT_GUILD_REWARDPOINT_LIST stGuildPoint = ((COMDT_GUILD_REWARDPOINT_LIST) ProtocolObjectPool.Get(COMDT_GUILD_REWARDPOINT_LIST.CLASS_ID));

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
            this.bResult = 0;
            if (this.stGuildInfo != null)
            {
                this.stGuildInfo.Release();
                this.stGuildInfo = null;
            }
            if (this.stGuildPoint != null)
            {
                this.stGuildPoint.Release();
                this.stGuildPoint = null;
            }
        }

        public override void OnUse()
        {
            this.stGuildInfo = (COMDT_GUILD_INFO) ProtocolObjectPool.Get(COMDT_GUILD_INFO.CLASS_ID);
            this.stGuildPoint = (COMDT_GUILD_REWARDPOINT_LIST) ProtocolObjectPool.Get(COMDT_GUILD_REWARDPOINT_LIST.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bResult);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stGuildInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGuildPoint.pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bResult);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stGuildInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stGuildPoint.unpack(ref srcBuf, cutVer);
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

