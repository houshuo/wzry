namespace CSProtocol
{
    using System;
    using tsf4g_tdr_csharp;

    public class SNSShareDetail : IPackable, IUnpackable, tsf4g_csharp_interface
    {
        public PlayerInfo[] astPlayerInfo = new PlayerInfo[2];
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public int iCount;
        public int iShareType;

        public SNSShareDetail()
        {
            for (int i = 0; i < 2; i++)
            {
                this.astPlayerInfo[i] = new PlayerInfo();
            }
        }

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
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
            type = destBuf.writeInt32(this.iCount);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0 > this.iCount)
                {
                    return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
                }
                if (2 < this.iCount)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astPlayerInfo.Length < this.iCount)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.iCount; i++)
                {
                    type = this.astPlayerInfo[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeInt32(this.iShareType);
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
            TdrWriteBuf destBuf = new TdrWriteBuf(ref buffer, size);
            TdrError.ErrorType type = this.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();
                usedSize = destBuf.getUsedSize();
            }
            return type;
        }

        public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
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
            type = srcBuf.readInt32(ref this.iCount);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0 > this.iCount)
                {
                    return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
                }
                if (2 < this.iCount)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.astPlayerInfo = new PlayerInfo[this.iCount];
                for (int i = 0; i < this.iCount; i++)
                {
                    this.astPlayerInfo[i] = new PlayerInfo();
                }
                for (int j = 0; j < this.iCount; j++)
                {
                    type = this.astPlayerInfo[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readInt32(ref this.iShareType);
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
            TdrReadBuf srcBuf = new TdrReadBuf(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            return type;
        }
    }
}

