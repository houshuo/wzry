namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_DAILY_CHECK_DATA : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x1f7;
        public static readonly uint CURRVERSION = 0x1c;
        public uint dwDailyAddCoin;
        public uint dwDailyAddExp;
        public uint dwDailyPvpCnt;
        public uint dwLastAddPvpCoinTime;
        public uint dwLastUpdateTime;
        public int iTotalAddPvpCoinOneDay;
        public static readonly uint VERSION_dwDailyAddCoin = 14;
        public static readonly uint VERSION_dwDailyAddExp = 14;
        public static readonly uint VERSION_dwDailyPvpCnt = 0x1c;
        public static readonly uint VERSION_dwLastUpdateTime = 14;

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
            this.dwLastAddPvpCoinTime = 0;
            this.iTotalAddPvpCoinOneDay = 0;
            this.dwLastUpdateTime = 0;
            this.dwDailyAddExp = 0;
            this.dwDailyAddCoin = 0;
            this.dwDailyPvpCnt = 0;
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
            type = destBuf.writeUInt32(this.dwLastAddPvpCoinTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeInt32(this.iTotalAddPvpCoinOneDay);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwLastUpdateTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwLastUpdateTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwDailyAddExp <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwDailyAddExp);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwDailyAddCoin <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwDailyAddCoin);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwDailyPvpCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwDailyPvpCnt);
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
            type = srcBuf.readUInt32(ref this.dwLastAddPvpCoinTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readInt32(ref this.iTotalAddPvpCoinOneDay);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwLastUpdateTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwLastUpdateTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwLastUpdateTime = 0;
                }
                if (VERSION_dwDailyAddExp <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwDailyAddExp);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwDailyAddExp = 0;
                }
                if (VERSION_dwDailyAddCoin <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwDailyAddCoin);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwDailyAddCoin = 0;
                }
                if (VERSION_dwDailyPvpCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwDailyPvpCnt);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwDailyPvpCnt = 0;
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

