namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_MULTROOMMEMBER_CAMP : ProtocolObject
    {
        public COMDT_ROOMMEMBER_DT[] astMemInfo = new COMDT_ROOMMEMBER_DT[5];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x30;
        public static readonly uint CURRVERSION = 1;
        public uint dwMemNum;

        public COMDT_MULTROOMMEMBER_CAMP()
        {
            for (int i = 0; i < 5; i++)
            {
                this.astMemInfo[i] = (COMDT_ROOMMEMBER_DT) ProtocolObjectPool.Get(COMDT_ROOMMEMBER_DT.CLASS_ID);
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
            this.dwMemNum = 0;
            if (this.astMemInfo != null)
            {
                for (int i = 0; i < this.astMemInfo.Length; i++)
                {
                    if (this.astMemInfo[i] != null)
                    {
                        this.astMemInfo[i].Release();
                        this.astMemInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astMemInfo != null)
            {
                for (int i = 0; i < this.astMemInfo.Length; i++)
                {
                    this.astMemInfo[i] = (COMDT_ROOMMEMBER_DT) ProtocolObjectPool.Get(COMDT_ROOMMEMBER_DT.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwMemNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (5 < this.dwMemNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astMemInfo.Length < this.dwMemNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwMemNum; i++)
                {
                    type = this.astMemInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwMemNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (5 < this.dwMemNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwMemNum; i++)
                {
                    type = this.astMemInfo[i].unpack(ref srcBuf, cutVer);
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

