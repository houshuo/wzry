namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_SHOPBUY_REWARDLIST : ProtocolObject
    {
        public COMDT_REWARD_DETAIL[] astRewardInfo = new COMDT_REWARD_DETAIL[10];
        public static readonly uint BASEVERSION = 1;
        public byte bRewardCnt;
        public static readonly int CLASS_ID = 0x2fe;
        public static readonly uint CURRVERSION = 1;

        public CSDT_SHOPBUY_REWARDLIST()
        {
            for (int i = 0; i < 10; i++)
            {
                this.astRewardInfo[i] = (COMDT_REWARD_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
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
            this.bRewardCnt = 0;
            if (this.astRewardInfo != null)
            {
                for (int i = 0; i < this.astRewardInfo.Length; i++)
                {
                    if (this.astRewardInfo[i] != null)
                    {
                        this.astRewardInfo[i].Release();
                        this.astRewardInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astRewardInfo != null)
            {
                for (int i = 0; i < this.astRewardInfo.Length; i++)
                {
                    this.astRewardInfo[i] = (COMDT_REWARD_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bRewardCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (10 < this.bRewardCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astRewardInfo.Length < this.bRewardCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bRewardCnt; i++)
                {
                    type = this.astRewardInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bRewardCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (10 < this.bRewardCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bRewardCnt; i++)
                {
                    type = this.astRewardInfo[i].unpack(ref srcBuf, cutVer);
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
            TdrReadBuf srcBuf = ClassObjPool<TdrReadBuf>.Get();
            srcBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            srcBuf.Release();
            return type;
        }
    }
}

