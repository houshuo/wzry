namespace CSProtocol
{
    using System;
    using tsf4g_tdr_csharp;

    public class ShareStatusList : IPackable, IUnpackable, tsf4g_csharp_interface
    {
        public ShareStatus[] astShareStatus = new ShareStatus[50];
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public int iCount;

        public ShareStatusList()
        {
            for (int i = 0; i < 50; i++)
            {
                this.astShareStatus[i] = new ShareStatus();
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
                if (50 < this.iCount)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astShareStatus.Length < this.iCount)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.iCount; i++)
                {
                    type = this.astShareStatus[i].pack(ref destBuf, cutVer);
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
                if (50 < this.iCount)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.astShareStatus = new ShareStatus[this.iCount];
                for (int i = 0; i < this.iCount; i++)
                {
                    this.astShareStatus[i] = new ShareStatus();
                }
                for (int j = 0; j < this.iCount; j++)
                {
                    type = this.astShareStatus[j].unpack(ref srcBuf, cutVer);
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
            TdrReadBuf srcBuf = new TdrReadBuf(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            return type;
        }
    }
}

