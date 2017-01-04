namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_PVPDETAIL_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x3aa;
        public static readonly uint CURRVERSION = 0x34;
        public CSDT_PVPBATTLE_INFO stEntertainmentInfo = ((CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID));
        public CSDT_PVPBATTLE_INFO stFiveVsFiveInfo = ((CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID));
        public CSDT_PVPBATTLE_INFO stFourVsFourInfo = ((CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID));
        public COMDT_STATISTIC_KEY_VALUE_DETAIL stKVDetail = ((COMDT_STATISTIC_KEY_VALUE_DETAIL) ProtocolObjectPool.Get(COMDT_STATISTIC_KEY_VALUE_DETAIL.CLASS_ID));
        public CSDT_PVPBATTLE_INFO stLadderInfo = ((CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID));
        public CSDT_PVPBATTLE_INFO stOneVsOneInfo = ((CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID));
        public CSDT_PVPBATTLE_INFO stThreeVsThreeInfo = ((CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID));
        public CSDT_PVPBATTLE_INFO stTwoVsTwoInfo = ((CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID));
        public CSDT_PVPBATTLE_INFO stVsMachineInfo = ((CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID));
        public static readonly uint VERSION_stFiveVsFiveInfo = 0x34;
        public static readonly uint VERSION_stFourVsFourInfo = 0x34;

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
            if (this.stOneVsOneInfo != null)
            {
                this.stOneVsOneInfo.Release();
                this.stOneVsOneInfo = null;
            }
            if (this.stTwoVsTwoInfo != null)
            {
                this.stTwoVsTwoInfo.Release();
                this.stTwoVsTwoInfo = null;
            }
            if (this.stThreeVsThreeInfo != null)
            {
                this.stThreeVsThreeInfo.Release();
                this.stThreeVsThreeInfo = null;
            }
            if (this.stFourVsFourInfo != null)
            {
                this.stFourVsFourInfo.Release();
                this.stFourVsFourInfo = null;
            }
            if (this.stFiveVsFiveInfo != null)
            {
                this.stFiveVsFiveInfo.Release();
                this.stFiveVsFiveInfo = null;
            }
            if (this.stLadderInfo != null)
            {
                this.stLadderInfo.Release();
                this.stLadderInfo = null;
            }
            if (this.stVsMachineInfo != null)
            {
                this.stVsMachineInfo.Release();
                this.stVsMachineInfo = null;
            }
            if (this.stEntertainmentInfo != null)
            {
                this.stEntertainmentInfo.Release();
                this.stEntertainmentInfo = null;
            }
            if (this.stKVDetail != null)
            {
                this.stKVDetail.Release();
                this.stKVDetail = null;
            }
        }

        public override void OnUse()
        {
            this.stOneVsOneInfo = (CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID);
            this.stTwoVsTwoInfo = (CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID);
            this.stThreeVsThreeInfo = (CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID);
            this.stFourVsFourInfo = (CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID);
            this.stFiveVsFiveInfo = (CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID);
            this.stLadderInfo = (CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID);
            this.stVsMachineInfo = (CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID);
            this.stEntertainmentInfo = (CSDT_PVPBATTLE_INFO) ProtocolObjectPool.Get(CSDT_PVPBATTLE_INFO.CLASS_ID);
            this.stKVDetail = (COMDT_STATISTIC_KEY_VALUE_DETAIL) ProtocolObjectPool.Get(COMDT_STATISTIC_KEY_VALUE_DETAIL.CLASS_ID);
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
            type = this.stOneVsOneInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stTwoVsTwoInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stThreeVsThreeInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stFourVsFourInfo <= cutVer)
                {
                    type = this.stFourVsFourInfo.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stFiveVsFiveInfo <= cutVer)
                {
                    type = this.stFiveVsFiveInfo.pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stLadderInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stVsMachineInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stEntertainmentInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stKVDetail.pack(ref destBuf, cutVer);
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
            type = this.stOneVsOneInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stTwoVsTwoInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stThreeVsThreeInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_stFourVsFourInfo <= cutVer)
                {
                    type = this.stFourVsFourInfo.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    type = this.stFourVsFourInfo.construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stFiveVsFiveInfo <= cutVer)
                {
                    type = this.stFiveVsFiveInfo.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    type = this.stFiveVsFiveInfo.construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stLadderInfo.unpack(ref srcBuf, cutVer);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    type = this.stVsMachineInfo.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = this.stEntertainmentInfo.unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = this.stKVDetail.unpack(ref srcBuf, cutVer);
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

