namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_PLAYERINFO_OF_ACNT : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x6f;
        public static readonly uint CURRVERSION = 0x67;
        public uint dwHeadId;
        public uint dwPvpLevel;
        public int iHonorID;
        public int iHonorLevel;
        public int iLogicWorldID;
        public COMDT_GAME_VIP_CLIENT stGameVip = ((COMDT_GAME_VIP_CLIENT) ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID));
        public ulong ullUid;
        public static readonly uint VERSION_iHonorID = 0x67;
        public static readonly uint VERSION_iHonorLevel = 0x67;
        public static readonly uint VERSION_stGameVip = 0x2a;

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
            this.dwHeadId = 0;
            this.ullUid = 0L;
            this.iLogicWorldID = 0;
            this.dwPvpLevel = 0;
            if (this.stGameVip != null)
            {
                this.stGameVip.Release();
                this.stGameVip = null;
            }
            this.iHonorID = 0;
            this.iHonorLevel = 0;
        }

        public override void OnUse()
        {
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
            type = destBuf.writeUInt32(this.dwHeadId);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt64(this.ullUid);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iLogicWorldID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwPvpLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stGameVip <= cutVer)
                {
                    type = this.stGameVip.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_iHonorID <= cutVer)
                {
                    type = destBuf.writeInt32(this.iHonorID);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_iHonorLevel <= cutVer)
                {
                    type = destBuf.writeInt32(this.iHonorLevel);
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
            type = srcBuf.readUInt32(ref this.dwHeadId);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt64(ref this.ullUid);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iLogicWorldID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPvpLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stGameVip <= cutVer)
                {
                    type = this.stGameVip.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    type = this.stGameVip.construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_iHonorID <= cutVer)
                {
                    type = srcBuf.readInt32(ref this.iHonorID);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.iHonorID = 0;
                }
                if (VERSION_iHonorLevel <= cutVer)
                {
                    type = srcBuf.readInt32(ref this.iHonorLevel);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.iHonorLevel = 0;
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

