namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_REWARDMATCH_DATA : ProtocolObject
    {
        public COMDT_REWARDMATCH_RECORD[] astRecord = new COMDT_REWARDMATCH_RECORD[4];
        public static readonly uint BASEVERSION = 1;
        public byte bRecordCnt;
        public static readonly int CLASS_ID = 0x22a;
        public static readonly uint CURRVERSION = 1;

        public COMDT_REWARDMATCH_DATA()
        {
            for (int i = 0; i < 4; i++)
            {
                this.astRecord[i] = (COMDT_REWARDMATCH_RECORD) ProtocolObjectPool.Get(COMDT_REWARDMATCH_RECORD.CLASS_ID);
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
            this.bRecordCnt = 0;
            if (this.astRecord != null)
            {
                for (int i = 0; i < this.astRecord.Length; i++)
                {
                    if (this.astRecord[i] != null)
                    {
                        this.astRecord[i].Release();
                        this.astRecord[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astRecord != null)
            {
                for (int i = 0; i < this.astRecord.Length; i++)
                {
                    this.astRecord[i] = (COMDT_REWARDMATCH_RECORD) ProtocolObjectPool.Get(COMDT_REWARDMATCH_RECORD.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bRecordCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (4 < this.bRecordCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astRecord.Length < this.bRecordCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bRecordCnt; i++)
                {
                    type = this.astRecord[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bRecordCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (4 < this.bRecordCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bRecordCnt; i++)
                {
                    type = this.astRecord[i].unpack(ref srcBuf, cutVer);
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

