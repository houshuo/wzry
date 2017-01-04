namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResDT_SkillFunc : IUnpackable, tsf4g_csharp_interface
    {
        public ResDT_IntParamArrayNode[] astSkillFuncGroup;
        public ResDT_IntParamArrayNode[] astSkillFuncParam = new ResDT_IntParamArrayNode[6];
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public uint dwSkillFuncFreq;
        public uint dwSkillFuncType;

        public ResDT_SkillFunc()
        {
            for (int i = 0; i < 6; i++)
            {
                this.astSkillFuncParam[i] = new ResDT_IntParamArrayNode();
            }
            this.astSkillFuncGroup = new ResDT_IntParamArrayNode[6];
            for (int j = 0; j < 6; j++)
            {
                this.astSkillFuncGroup[j] = new ResDT_IntParamArrayNode();
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
            type = srcBuf.readUInt32(ref this.dwSkillFuncType);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwSkillFuncFreq);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 6; i++)
                {
                    type = this.astSkillFuncParam[i].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int j = 0; j < 6; j++)
                {
                    type = this.astSkillFuncGroup[j].load(ref srcBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwSkillFuncType);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwSkillFuncFreq);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 6; i++)
                {
                    type = this.astSkillFuncParam[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int j = 0; j < 6; j++)
                {
                    type = this.astSkillFuncGroup[j].unpack(ref srcBuf, cutVer);
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

