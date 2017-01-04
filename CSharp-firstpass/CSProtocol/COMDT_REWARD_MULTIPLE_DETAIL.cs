namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_REWARD_MULTIPLE_DETAIL : ProtocolObject
    {
        public COMDT_REWARD_MULTIPLE_INFO[] astMultiple = new COMDT_REWARD_MULTIPLE_INFO[0x15];
        public static readonly uint BASEVERSION = 1;
        public byte bNum;
        public byte bZeroProfit;
        public static readonly int CLASS_ID = 0xcd;
        public static readonly uint CURRVERSION = 0x71;
        public static readonly uint VERSION_bZeroProfit = 0x2e;

        public COMDT_REWARD_MULTIPLE_DETAIL()
        {
            for (int i = 0; i < 0x15; i++)
            {
                this.astMultiple[i] = (COMDT_REWARD_MULTIPLE_INFO) ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_INFO.CLASS_ID);
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
            this.bNum = 0;
            if (this.astMultiple != null)
            {
                for (int i = 0; i < this.astMultiple.Length; i++)
                {
                    if (this.astMultiple[i] != null)
                    {
                        this.astMultiple[i].Release();
                        this.astMultiple[i] = null;
                    }
                }
            }
            this.bZeroProfit = 0;
        }

        public override void OnUse()
        {
            if (this.astMultiple != null)
            {
                for (int i = 0; i < this.astMultiple.Length; i++)
                {
                    this.astMultiple[i] = (COMDT_REWARD_MULTIPLE_INFO) ProtocolObjectPool.Get(COMDT_REWARD_MULTIPLE_INFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x15 < this.bNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astMultiple.Length < this.bNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bNum; i++)
                {
                    type = this.astMultiple[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bZeroProfit <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bZeroProfit);
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
            type = srcBuf.readUInt8(ref this.bNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x15 < this.bNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bNum; i++)
                {
                    type = this.astMultiple[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bZeroProfit <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bZeroProfit);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.bZeroProfit = 0;
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

