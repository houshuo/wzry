namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ACTIVITY_DETAIL : ProtocolObject
    {
        public COMDT_SUBACTIVITY_DETAIL[] astSubActivityDetail = new COMDT_SUBACTIVITY_DETAIL[20];
        public static readonly uint BASEVERSION = 1;
        public byte bOpenType;
        public static readonly int CLASS_ID = 0x80;
        public static readonly uint CURRVERSION = 1;
        public uint dwAccPlayedCount;
        public uint dwActivityID;
        public COMDT_ACTIVITY_OPEN_UNION stOpenData = ((COMDT_ACTIVITY_OPEN_UNION) ProtocolObjectPool.Get(COMDT_ACTIVITY_OPEN_UNION.CLASS_ID));
        public ushort wSubActivityCnt;

        public COMDT_ACTIVITY_DETAIL()
        {
            for (int i = 0; i < 20; i++)
            {
                this.astSubActivityDetail[i] = (COMDT_SUBACTIVITY_DETAIL) ProtocolObjectPool.Get(COMDT_SUBACTIVITY_DETAIL.CLASS_ID);
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
            this.dwActivityID = 0;
            this.dwAccPlayedCount = 0;
            this.bOpenType = 0;
            if (this.stOpenData != null)
            {
                this.stOpenData.Release();
                this.stOpenData = null;
            }
            this.wSubActivityCnt = 0;
            if (this.astSubActivityDetail != null)
            {
                for (int i = 0; i < this.astSubActivityDetail.Length; i++)
                {
                    if (this.astSubActivityDetail[i] != null)
                    {
                        this.astSubActivityDetail[i].Release();
                        this.astSubActivityDetail[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            this.stOpenData = (COMDT_ACTIVITY_OPEN_UNION) ProtocolObjectPool.Get(COMDT_ACTIVITY_OPEN_UNION.CLASS_ID);
            if (this.astSubActivityDetail != null)
            {
                for (int i = 0; i < this.astSubActivityDetail.Length; i++)
                {
                    this.astSubActivityDetail[i] = (COMDT_SUBACTIVITY_DETAIL) ProtocolObjectPool.Get(COMDT_SUBACTIVITY_DETAIL.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwActivityID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwAccPlayedCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bOpenType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                long bOpenType = this.bOpenType;
                type = this.stOpenData.pack(bOpenType, ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt16(this.wSubActivityCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (20 < this.wSubActivityCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astSubActivityDetail.Length < this.wSubActivityCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.wSubActivityCnt; i++)
                {
                    type = this.astSubActivityDetail[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwActivityID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwAccPlayedCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bOpenType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                long bOpenType = this.bOpenType;
                type = this.stOpenData.unpack(bOpenType, ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wSubActivityCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (20 < this.wSubActivityCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.wSubActivityCnt; i++)
                {
                    type = this.astSubActivityDetail[i].unpack(ref srcBuf, cutVer);
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

