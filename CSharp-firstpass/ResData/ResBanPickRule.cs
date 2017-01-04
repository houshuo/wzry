namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResBanPickRule : IUnpackable, tsf4g_csharp_interface
    {
        public ResDT_BPItem[] astBanProcess = new ResDT_BPItem[6];
        public ResDT_BPItem[] astPickProcess;
        public static readonly uint BASEVERSION = 1;
        public byte bBanRoundNum;
        public byte bPickRoundNum;
        public static readonly uint CURRVERSION = 1;
        public uint dwBanTimeout;
        public uint dwPickTimeout;
        public uint dwRuleID;

        public ResBanPickRule()
        {
            for (int i = 0; i < 6; i++)
            {
                this.astBanProcess[i] = new ResDT_BPItem();
            }
            this.astPickProcess = new ResDT_BPItem[10];
            for (int j = 0; j < 10; j++)
            {
                this.astPickProcess[j] = new ResDT_BPItem();
            }
        }

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
            type = srcBuf.readUInt32(ref this.dwRuleID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwBanTimeout);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBanRoundNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 6; i++)
                {
                    type = this.astBanProcess[i].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwPickTimeout);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bPickRoundNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int j = 0; j < 10; j++)
                {
                    type = this.astPickProcess[j].load(ref srcBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwRuleID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwBanTimeout);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBanRoundNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 6; i++)
                {
                    type = this.astBanProcess[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwPickTimeout);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bPickRoundNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int j = 0; j < 10; j++)
                {
                    type = this.astPickProcess[j].unpack(ref srcBuf, cutVer);
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

