namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResWealMultiple : IUnpackable, tsf4g_csharp_interface
    {
        public ResDT_WealMultiplePeriod[] astPeriod = new ResDT_WealMultiplePeriod[] { new ResDT_WealMultiplePeriod() };
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public uint dwID;
        public uint dwPvpLvlLimit;
        public ResDT_WealCommon stCommon = new ResDT_WealCommon();
        public ushort wPeriodNum;

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
        {
            srcBuf.disableEndian();
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = srcBuf.readUInt32(ref this.dwID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stCommon.load(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPvpLvlLimit);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wPeriodNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.astPeriod.Length < 4)
                {
                    this.astPeriod = new ResDT_WealMultiplePeriod[4];
                    for (int j = 0; j < 4; j++)
                    {
                        this.astPeriod[j] = new ResDT_WealMultiplePeriod();
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    type = this.astPeriod[i].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                this.TransferData();
            }
            return type;
        }

        public TdrError.ErrorType load(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = new TdrReadBuf(ref buffer, size);
            TdrError.ErrorType type = this.load(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            return type;
        }

        private void TransferData()
        {
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
            type = srcBuf.readUInt32(ref this.dwID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stCommon.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPvpLvlLimit);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wPeriodNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (4 < this.wPeriodNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.astPeriod = new ResDT_WealMultiplePeriod[this.wPeriodNum];
                for (int i = 0; i < this.wPeriodNum; i++)
                {
                    this.astPeriod[i] = new ResDT_WealMultiplePeriod();
                }
                for (int j = 0; j < this.wPeriodNum; j++)
                {
                    type = this.astPeriod[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                this.TransferData();
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

