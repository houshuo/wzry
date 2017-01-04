namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_WEAL_STATISTIC_DATA : ProtocolObject
    {
        public COMDT_STATISTIC_DATA_INFO_MULTI[] astMultiDetail;
        public COMDT_STATISTIC_DATA_INFO_SINGLE[] astSingleDetail = new COMDT_STATISTIC_DATA_INFO_SINGLE[20];
        public static readonly uint BASEVERSION = 1;
        public byte bMultiNum;
        public byte bSingleNum;
        public static readonly int CLASS_ID = 0x1c5;
        public static readonly uint CURRVERSION = 0x26;
        public uint dwWealID;

        public COMDT_WEAL_STATISTIC_DATA()
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
            this.dwWealID = 0;
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
            type = destBuf.writeUInt32(this.dwWealID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt8(this.bSingleNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
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
            type = srcBuf.readUInt32(ref this.dwWealID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt8(ref this.bSingleNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
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

