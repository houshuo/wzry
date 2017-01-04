namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_WEAL_STATISTIC_LIST : ProtocolObject
    {
        public COMDT_WEAL_STATISTIC_DATA[] astStatisticList = new COMDT_WEAL_STATISTIC_DATA[4];
        public static readonly uint BASEVERSION = 1;
        public byte bListNum;
        public static readonly int CLASS_ID = 0x1c6;
        public static readonly uint CURRVERSION = 0x26;

        public COMDT_WEAL_STATISTIC_LIST()
        {
            for (int i = 0; i < 4; i++)
            {
                this.astStatisticList[i] = (COMDT_WEAL_STATISTIC_DATA) ProtocolObjectPool.Get(COMDT_WEAL_STATISTIC_DATA.CLASS_ID);
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
            this.bListNum = 0;
            if (this.astStatisticList != null)
            {
                for (int i = 0; i < this.astStatisticList.Length; i++)
                {
                    if (this.astStatisticList[i] != null)
                    {
                        this.astStatisticList[i].Release();
                        this.astStatisticList[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astStatisticList != null)
            {
                for (int i = 0; i < this.astStatisticList.Length; i++)
                {
                    this.astStatisticList[i] = (COMDT_WEAL_STATISTIC_DATA) ProtocolObjectPool.Get(COMDT_WEAL_STATISTIC_DATA.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bListNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (4 < this.bListNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astStatisticList.Length < this.bListNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bListNum; i++)
                {
                    type = this.astStatisticList[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bListNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (4 < this.bListNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bListNum; i++)
                {
                    type = this.astStatisticList[i].unpack(ref srcBuf, cutVer);
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

