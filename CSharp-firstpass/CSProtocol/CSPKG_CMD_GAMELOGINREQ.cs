namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSPKG_CMD_GAMELOGINREQ : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bPrivilege;
        public static readonly int CLASS_ID = 0x238;
        public static readonly uint CURRVERSION = 0x3e;
        public uint dwTaccZoneID;
        public int iCltAppVersion;
        public int iCltResVersion;
        public int iLogicWorldID;
        public static readonly uint LENGTH_szCltIMEI = 0x40;
        public COMDT_CLIENT_INFO_DATA stClientInfo = ((COMDT_CLIENT_INFO_DATA) ProtocolObjectPool.Get(COMDT_CLIENT_INFO_DATA.CLASS_ID));
        public byte[] szCltIMEI = new byte[0x40];
        public ulong ullVisitorUid;
        public static readonly uint VERSION_bPrivilege = 0x3e;

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.iCltAppVersion = 0;
            this.iCltResVersion = 0;
            type = this.stClientInfo.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                this.ullVisitorUid = 0L;
                this.iLogicWorldID = 0;
                this.dwTaccZoneID = 0;
                this.bPrivilege = 0;
            }
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            this.iCltAppVersion = 0;
            this.iCltResVersion = 0;
            if (this.stClientInfo != null)
            {
                this.stClientInfo.Release();
                this.stClientInfo = null;
            }
            this.ullVisitorUid = 0L;
            this.iLogicWorldID = 0;
            this.dwTaccZoneID = 0;
            this.bPrivilege = 0;
        }

        public override void OnUse()
        {
            this.stClientInfo = (COMDT_CLIENT_INFO_DATA) ProtocolObjectPool.Get(COMDT_CLIENT_INFO_DATA.CLASS_ID);
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
            type = destBuf.writeInt32(this.iCltAppVersion);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeInt32(this.iCltResVersion);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stClientInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt64(this.ullVisitorUid);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iLogicWorldID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwTaccZoneID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int pos = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = destBuf.getUsedSize();
                int count = TdrTypeUtil.cstrlen(this.szCltIMEI);
                if (count >= 0x40)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szCltIMEI, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(0);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = destBuf.getUsedSize() - num2;
                type = destBuf.writeUInt32((uint) num4, pos);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_bPrivilege <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bPrivilege);
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
            type = srcBuf.readInt32(ref this.iCltAppVersion);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readInt32(ref this.iCltResVersion);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stClientInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt64(ref this.ullVisitorUid);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iLogicWorldID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTaccZoneID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint dest = 0;
                type = srcBuf.readUInt32(ref dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (dest > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (dest > this.szCltIMEI.GetLength(0))
                {
                    if (dest > LENGTH_szCltIMEI)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szCltIMEI = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szCltIMEI, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szCltIMEI[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szCltIMEI) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                if (VERSION_bPrivilege <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bPrivilege);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.bPrivilege = 0;
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

