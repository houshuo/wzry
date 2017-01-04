namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_STATISTIC_DATA_DETAIL : ProtocolObject
    {
        public COMDT_STATISTIC_DATA_INFO_MULTI[] astMultiDetail;
        public COMDT_STATISTIC_DATA_INFO_SINGLE[] astSingleDetail = new COMDT_STATISTIC_DATA_INFO_SINGLE[20];
        public COMDT_WARM_BATTLE_INFO[] astWarmDetail;
        public static readonly uint BASEVERSION = 1;
        public byte bMultiNum;
        public byte bSingleNum;
        public byte bWarmNum;
        public static readonly int CLASS_ID = 0x1c4;
        public static readonly uint CURRVERSION = 140;
        public uint dwNormalMMRContinuousLoseNum;
        public uint dwNormalMMRContinuousWinNum;
        public uint dwNormalMMRLoseNum;
        public uint dwNormalMMRWinNum;
        public COMDT_STATISTIC_KEY_VALUE_DETAIL stKVDetail;
        public COMDT_WARM_BATTLE_INFO stLadderWarm;
        public static readonly uint VERSION_astWarmDetail = 0x36;
        public static readonly uint VERSION_bWarmNum = 0x36;
        public static readonly uint VERSION_dwNormalMMRContinuousLoseNum = 0x5e;
        public static readonly uint VERSION_dwNormalMMRContinuousWinNum = 0x5e;
        public static readonly uint VERSION_dwNormalMMRLoseNum = 0x5f;
        public static readonly uint VERSION_dwNormalMMRWinNum = 0x5f;
        public static readonly uint VERSION_stLadderWarm = 140;

        public COMDT_STATISTIC_DATA_DETAIL()
        {
            for (int i = 0; i < 20; i++)
            {
                this.astSingleDetail[i] = (COMDT_STATISTIC_DATA_INFO_SINGLE) ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_SINGLE.CLASS_ID);
            }
            this.astMultiDetail = new COMDT_STATISTIC_DATA_INFO_MULTI[40];
            for (int j = 0; j < 40; j++)
            {
                this.astMultiDetail[j] = (COMDT_STATISTIC_DATA_INFO_MULTI) ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_MULTI.CLASS_ID);
            }
            this.stKVDetail = (COMDT_STATISTIC_KEY_VALUE_DETAIL) ProtocolObjectPool.Get(COMDT_STATISTIC_KEY_VALUE_DETAIL.CLASS_ID);
            this.astWarmDetail = new COMDT_WARM_BATTLE_INFO[10];
            for (int k = 0; k < 10; k++)
            {
                this.astWarmDetail[k] = (COMDT_WARM_BATTLE_INFO) ProtocolObjectPool.Get(COMDT_WARM_BATTLE_INFO.CLASS_ID);
            }
            this.stLadderWarm = (COMDT_WARM_BATTLE_INFO) ProtocolObjectPool.Get(COMDT_WARM_BATTLE_INFO.CLASS_ID);
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
            this.bSingleNum = 0;
            if (this.astSingleDetail != null)
            {
                for (int i = 0; i < this.astSingleDetail.Length; i++)
                {
                    if (this.astSingleDetail[i] != null)
                    {
                        this.astSingleDetail[i].Release();
                        this.astSingleDetail[i] = null;
                    }
                }
            }
            this.bMultiNum = 0;
            if (this.astMultiDetail != null)
            {
                for (int j = 0; j < this.astMultiDetail.Length; j++)
                {
                    if (this.astMultiDetail[j] != null)
                    {
                        this.astMultiDetail[j].Release();
                        this.astMultiDetail[j] = null;
                    }
                }
            }
            if (this.stKVDetail != null)
            {
                this.stKVDetail.Release();
                this.stKVDetail = null;
            }
            this.bWarmNum = 0;
            if (this.astWarmDetail != null)
            {
                for (int k = 0; k < this.astWarmDetail.Length; k++)
                {
                    if (this.astWarmDetail[k] != null)
                    {
                        this.astWarmDetail[k].Release();
                        this.astWarmDetail[k] = null;
                    }
                }
            }
            this.dwNormalMMRContinuousWinNum = 0;
            this.dwNormalMMRContinuousLoseNum = 0;
            this.dwNormalMMRWinNum = 0;
            this.dwNormalMMRLoseNum = 0;
            if (this.stLadderWarm != null)
            {
                this.stLadderWarm.Release();
                this.stLadderWarm = null;
            }
        }

        public override void OnUse()
        {
            if (this.astSingleDetail != null)
            {
                for (int i = 0; i < this.astSingleDetail.Length; i++)
                {
                    this.astSingleDetail[i] = (COMDT_STATISTIC_DATA_INFO_SINGLE) ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_SINGLE.CLASS_ID);
                }
            }
            if (this.astMultiDetail != null)
            {
                for (int j = 0; j < this.astMultiDetail.Length; j++)
                {
                    this.astMultiDetail[j] = (COMDT_STATISTIC_DATA_INFO_MULTI) ProtocolObjectPool.Get(COMDT_STATISTIC_DATA_INFO_MULTI.CLASS_ID);
                }
            }
            this.stKVDetail = (COMDT_STATISTIC_KEY_VALUE_DETAIL) ProtocolObjectPool.Get(COMDT_STATISTIC_KEY_VALUE_DETAIL.CLASS_ID);
            if (this.astWarmDetail != null)
            {
                for (int k = 0; k < this.astWarmDetail.Length; k++)
                {
                    this.astWarmDetail[k] = (COMDT_WARM_BATTLE_INFO) ProtocolObjectPool.Get(COMDT_WARM_BATTLE_INFO.CLASS_ID);
                }
            }
            this.stLadderWarm = (COMDT_WARM_BATTLE_INFO) ProtocolObjectPool.Get(COMDT_WARM_BATTLE_INFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bSingleNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (20 < this.bSingleNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astSingleDetail.Length < this.bSingleNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bSingleNum; i++)
                {
                    type = this.astSingleDetail[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bMultiNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (40 < this.bMultiNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astMultiDetail.Length < this.bMultiNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int j = 0; j < this.bMultiNum; j++)
                {
                    type = this.astMultiDetail[j].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stKVDetail.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_bWarmNum <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bWarmNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_astWarmDetail <= cutVer)
                {
                    if (10 < this.bWarmNum)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    if (this.astWarmDetail.Length < this.bWarmNum)
                    {
                        return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                    }
                    for (int k = 0; k < this.bWarmNum; k++)
                    {
                        type = this.astWarmDetail[k].pack(ref destBuf, cutVer);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                }
                if (VERSION_dwNormalMMRContinuousWinNum <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwNormalMMRContinuousWinNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwNormalMMRContinuousLoseNum <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwNormalMMRContinuousLoseNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwNormalMMRWinNum <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwNormalMMRWinNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwNormalMMRLoseNum <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwNormalMMRLoseNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stLadderWarm <= cutVer)
                {
                    type = this.stLadderWarm.pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bSingleNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (20 < this.bSingleNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bSingleNum; i++)
                {
                    type = this.astSingleDetail[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bMultiNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (40 < this.bMultiNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int j = 0; j < this.bMultiNum; j++)
                {
                    type = this.astMultiDetail[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stKVDetail.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_bWarmNum <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bWarmNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bWarmNum = 0;
                }
                if (VERSION_astWarmDetail <= cutVer)
                {
                    if (10 < this.bWarmNum)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    for (int k = 0; k < this.bWarmNum; k++)
                    {
                        type = this.astWarmDetail[k].unpack(ref srcBuf, cutVer);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                }
                else
                {
                    if (10 < this.bWarmNum)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    for (int m = 0; m < this.bWarmNum; m++)
                    {
                        type = this.astWarmDetail[m].construct();
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                }
                if (VERSION_dwNormalMMRContinuousWinNum <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwNormalMMRContinuousWinNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwNormalMMRContinuousWinNum = 0;
                }
                if (VERSION_dwNormalMMRContinuousLoseNum <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwNormalMMRContinuousLoseNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwNormalMMRContinuousLoseNum = 0;
                }
                if (VERSION_dwNormalMMRWinNum <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwNormalMMRWinNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwNormalMMRWinNum = 0;
                }
                if (VERSION_dwNormalMMRLoseNum <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwNormalMMRLoseNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwNormalMMRLoseNum = 0;
                }
                if (VERSION_stLadderWarm <= cutVer)
                {
                    type = this.stLadderWarm.unpack(ref srcBuf, cutVer);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                type = this.stLadderWarm.construct();
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

