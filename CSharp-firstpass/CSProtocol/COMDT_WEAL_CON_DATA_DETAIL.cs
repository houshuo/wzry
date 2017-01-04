namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_WEAL_CON_DATA_DETAIL : ProtocolObject
    {
        public COMDT_WEAL_CON_DATA_INFO[] astConData = new COMDT_WEAL_CON_DATA_INFO[10];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x1c8;
        public static readonly uint CURRVERSION = 0x18;
        public uint dwLastRefreshTime;
        public uint dwLimitReachMask;
        public uint dwReachMask;
        public uint dwRewardMask;
        public uint dwWealID;
        public static readonly uint VERSION_dwLastRefreshTime = 0x18;
        public static readonly uint VERSION_dwLimitReachMask = 0x18;
        public static readonly uint VERSION_dwReachMask = 0x18;
        public static readonly uint VERSION_dwRewardMask = 0x18;
        public ushort wConNum;

        public COMDT_WEAL_CON_DATA_DETAIL()
        {
            for (int i = 0; i < 10; i++)
            {
                this.astConData[i] = (COMDT_WEAL_CON_DATA_INFO) ProtocolObjectPool.Get(COMDT_WEAL_CON_DATA_INFO.CLASS_ID);
            }
        }

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
            this.dwWealID = 0;
            this.dwRewardMask = 0;
            this.dwReachMask = 0;
            this.dwLimitReachMask = 0;
            this.dwLastRefreshTime = 0;
            this.wConNum = 0;
            if (this.astConData != null)
            {
                for (int i = 0; i < this.astConData.Length; i++)
                {
                    if (this.astConData[i] != null)
                    {
                        this.astConData[i].Release();
                        this.astConData[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astConData != null)
            {
                for (int i = 0; i < this.astConData.Length; i++)
                {
                    this.astConData[i] = (COMDT_WEAL_CON_DATA_INFO) ProtocolObjectPool.Get(COMDT_WEAL_CON_DATA_INFO.CLASS_ID);
                }
            }
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
            type = destBuf.writeUInt32(this.dwWealID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (VERSION_dwRewardMask <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwRewardMask);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwReachMask <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwReachMask);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLimitReachMask <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLimitReachMask);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwLastRefreshTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastRefreshTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt16(this.wConNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.wConNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astConData.Length < this.wConNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.wConNum; i++)
                {
                    type = this.astConData[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwWealID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (VERSION_dwRewardMask <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwRewardMask);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwRewardMask = 0;
                }
                if (VERSION_dwReachMask <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwReachMask);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwReachMask = 0;
                }
                if (VERSION_dwLimitReachMask <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLimitReachMask);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLimitReachMask = 0;
                }
                if (VERSION_dwLastRefreshTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastRefreshTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastRefreshTime = 0;
                }
                type = srcBuf.readUInt16(ref this.wConNum);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (10 < this.wConNum)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    for (int i = 0; i < this.wConNum; i++)
                    {
                        type = this.astConData[i].unpack(ref srcBuf, cutVer);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
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

