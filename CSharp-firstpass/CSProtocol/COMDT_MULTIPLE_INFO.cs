namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_MULTIPLE_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0xcb;
        public static readonly uint CURRVERSION = 0x71;
        public uint dwFirstWinAdd;
        public uint dwGameVIPRatio;
        public uint dwGuildRatio;
        public uint dwIOSVisitorLoginRatio;
        public uint dwPropRatio;
        public uint dwPvpDailyRatio;
        public uint dwQQGameCenterLoginRatio;
        public uint dwQQVIPRatio;
        public uint dwWealRatio;
        public uint dwWXGameCenterLoginRatio;
        public static readonly uint VERSION_dwFirstWinAdd = 0x45;
        public static readonly uint VERSION_dwGameVIPRatio = 0x55;
        public static readonly uint VERSION_dwGuildRatio = 0x44;
        public static readonly uint VERSION_dwIOSVisitorLoginRatio = 0x71;
        public static readonly uint VERSION_dwPvpDailyRatio = 0x1c;
        public static readonly uint VERSION_dwQQGameCenterLoginRatio = 0x65;
        public static readonly uint VERSION_dwWXGameCenterLoginRatio = 0x3f;

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
            this.dwWealRatio = 0;
            this.dwQQVIPRatio = 0;
            this.dwPropRatio = 0;
            this.dwPvpDailyRatio = 0;
            this.dwWXGameCenterLoginRatio = 0;
            this.dwGuildRatio = 0;
            this.dwFirstWinAdd = 0;
            this.dwGameVIPRatio = 0;
            this.dwQQGameCenterLoginRatio = 0;
            this.dwIOSVisitorLoginRatio = 0;
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
            type = destBuf.writeUInt32(this.dwWealRatio);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwQQVIPRatio);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwPropRatio);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwPvpDailyRatio <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwPvpDailyRatio);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwWXGameCenterLoginRatio <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwWXGameCenterLoginRatio);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwGuildRatio <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwGuildRatio);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwFirstWinAdd <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwFirstWinAdd);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwGameVIPRatio <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwGameVIPRatio);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwQQGameCenterLoginRatio <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwQQGameCenterLoginRatio);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwIOSVisitorLoginRatio <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwIOSVisitorLoginRatio);
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
            type = srcBuf.readUInt32(ref this.dwWealRatio);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwQQVIPRatio);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPropRatio);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwPvpDailyRatio <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwPvpDailyRatio);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwPvpDailyRatio = 0;
                }
                if (VERSION_dwWXGameCenterLoginRatio <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwWXGameCenterLoginRatio);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwWXGameCenterLoginRatio = 0;
                }
                if (VERSION_dwGuildRatio <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwGuildRatio);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwGuildRatio = 0;
                }
                if (VERSION_dwFirstWinAdd <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwFirstWinAdd);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwFirstWinAdd = 0;
                }
                if (VERSION_dwGameVIPRatio <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwGameVIPRatio);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwGameVIPRatio = 0;
                }
                if (VERSION_dwQQGameCenterLoginRatio <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwQQGameCenterLoginRatio);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwQQGameCenterLoginRatio = 0;
                }
                if (VERSION_dwIOSVisitorLoginRatio <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwIOSVisitorLoginRatio);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwIOSVisitorLoginRatio = 0;
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

