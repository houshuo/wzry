namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResPvpLevelReward : IUnpackable, tsf4g_csharp_interface
    {
        public ResDT_LevelReward_UnlockInfo[] astLockInfo = new ResDT_LevelReward_UnlockInfo[2];
        public ResDT_LevelReward_Info[] astRewardInfo;
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public int iLevel;

        public ResPvpLevelReward()
        {
            for (int i = 0; i < 2; i++)
            {
                this.astLockInfo[i] = new ResDT_LevelReward_UnlockInfo();
            }
            this.astRewardInfo = new ResDT_LevelReward_Info[2];
            for (int j = 0; j < 2; j++)
            {
                this.astRewardInfo[j] = new ResDT_LevelReward_Info();
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
            type = srcBuf.readInt32(ref this.iLevel);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 2; i++)
                {
                    type = this.astLockInfo[i].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int j = 0; j < 2; j++)
                {
                    type = this.astRewardInfo[j].load(ref srcBuf, cutVer);
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
            type = srcBuf.readInt32(ref this.iLevel);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 2; i++)
                {
                    type = this.astLockInfo[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int j = 0; j < 2; j++)
                {
                    type = this.astRewardInfo[j].unpack(ref srcBuf, cutVer);
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

