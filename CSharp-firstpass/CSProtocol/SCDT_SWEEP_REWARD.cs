namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCDT_SWEEP_REWARD : ProtocolObject
    {
        public COMDT_REWARD_DETAIL[] astRewardDetail = new COMDT_REWARD_DETAIL[10];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x283;
        public static readonly uint CURRVERSION = 0x71;
        public uint dwAcntExp;
        public uint dwGold;
        public uint dwSweepCnt;
        public COMDT_REWARD_MULTIPLE_DETAIL stMultipleDetail;
        public ushort wMultipleApplyCnt;

        public SCDT_SWEEP_REWARD()
        {
            for (int i = 0; i < 10; i++)
            {
                this.astRewardDetail[i] = (COMDT_REWARD_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
            }
            this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
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
            this.dwAcntExp = 0;
            this.dwGold = 0;
            this.dwSweepCnt = 0;
            if (this.astRewardDetail != null)
            {
                for (int i = 0; i < this.astRewardDetail.Length; i++)
                {
                    if (this.astRewardDetail[i] != null)
                    {
                        this.astRewardDetail[i].Release();
                        this.astRewardDetail[i] = null;
                    }
                }
            }
            this.wMultipleApplyCnt = 0;
            if (this.stMultipleDetail != null)
            {
                this.stMultipleDetail.Release();
                this.stMultipleDetail = null;
            }
        }

        public override void OnUse()
        {
            if (this.astRewardDetail != null)
            {
                for (int i = 0; i < this.astRewardDetail.Length; i++)
                {
                    this.astRewardDetail[i] = (COMDT_REWARD_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_DETAIL.CLASS_ID);
                }
            }
            this.stMultipleDetail = (COMDT_REWARD_MULTIPLE_DETAIL) ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_DETAIL.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwAcntExp);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwGold);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwSweepCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.dwSweepCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astRewardDetail.Length < this.dwSweepCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwSweepCnt; i++)
                {
                    type = this.astRewardDetail[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt16(this.wMultipleApplyCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMultipleDetail.pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwAcntExp);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwGold);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSweepCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (10 < this.dwSweepCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwSweepCnt; i++)
                {
                    type = this.astRewardDetail[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt16(ref this.wMultipleApplyCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMultipleDetail.unpack(ref srcBuf, cutVer);
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
            TdrReadBuf srcBuf = ClassObjPool<TdrReadBuf>.Get();
            srcBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            srcBuf.Release();
            return type;
        }
    }
}

