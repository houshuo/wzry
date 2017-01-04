namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_STARTSINGLEGAMERSP : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bGameType;
        public static readonly int CLASS_ID = 0x270;
        public static readonly uint CURRVERSION = 0x8a;
        public uint dwRewardNum;
        public int iErrCode;
        public int iLevelId;
        public CSDT_SINGLEGAME_DETAIL stDetail = ((CSDT_SINGLEGAME_DETAIL) ProtocolObjectPool.Get(CSDT_SINGLEGAME_DETAIL.CLASS_ID));
        public CSDT_SINGLEGAMETYPE_RSP stGameParam = ((CSDT_SINGLEGAMETYPE_RSP) ProtocolObjectPool.Get(CSDT_SINGLEGAMETYPE_RSP.CLASS_ID));

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.iErrCode = 0;
            this.dwRewardNum = 0;
            this.bGameType = 0;
            long bGameType = this.bGameType;
            type = this.stGameParam.construct(bGameType);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                this.iLevelId = 0;
                long iErrCode = this.iErrCode;
                type = this.stDetail.construct(iErrCode);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            this.iErrCode = 0;
            this.dwRewardNum = 0;
            this.bGameType = 0;
            if (this.stGameParam != null)
            {
                this.stGameParam.Release();
                this.stGameParam = null;
            }
            this.iLevelId = 0;
            if (this.stDetail != null)
            {
                this.stDetail.Release();
                this.stDetail = null;
            }
        }

        public override void OnUse()
        {
            this.stGameParam = (CSDT_SINGLEGAMETYPE_RSP) ProtocolObjectPool.Get(CSDT_SINGLEGAMETYPE_RSP.CLASS_ID);
            this.stDetail = (CSDT_SINGLEGAME_DETAIL) ProtocolObjectPool.Get(CSDT_SINGLEGAME_DETAIL.CLASS_ID);
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
            type = destBuf.writeInt32(this.iErrCode);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwRewardNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bGameType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                long bGameType = this.bGameType;
                type = this.stGameParam.pack(bGameType, ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeInt32(this.iLevelId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                long iErrCode = this.iErrCode;
                type = this.stDetail.pack(iErrCode, ref destBuf, cutVer);
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
            type = srcBuf.readInt32(ref this.iErrCode);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwRewardNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bGameType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                long bGameType = this.bGameType;
                type = this.stGameParam.unpack(bGameType, ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iLevelId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                long iErrCode = this.iErrCode;
                type = this.stDetail.unpack(iErrCode, ref srcBuf, cutVer);
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

