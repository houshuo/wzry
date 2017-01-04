namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_CMD_LIST_FRIENDREQ : ProtocolObject
    {
        public CSDT_VERIFICATION_INFO[] astVerificationList = new CSDT_VERIFICATION_INFO[220];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x341;
        public static readonly uint CURRVERSION = 0x63;
        public uint dwFriendReqNum;
        public uint dwResult;

        public SCPKG_CMD_LIST_FRIENDREQ()
        {
            for (int i = 0; i < 220; i++)
            {
                this.astVerificationList[i] = (CSDT_VERIFICATION_INFO) ProtocolObjectPool.Get(CSDT_VERIFICATION_INFO.CLASS_ID);
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
            this.dwFriendReqNum = 0;
            if (this.astVerificationList != null)
            {
                for (int i = 0; i < this.astVerificationList.Length; i++)
                {
                    if (this.astVerificationList[i] != null)
                    {
                        this.astVerificationList[i].Release();
                        this.astVerificationList[i] = null;
                    }
                }
            }
            this.dwResult = 0;
        }

        public override void OnUse()
        {
            if (this.astVerificationList != null)
            {
                for (int i = 0; i < this.astVerificationList.Length; i++)
                {
                    this.astVerificationList[i] = (CSDT_VERIFICATION_INFO) ProtocolObjectPool.Get(CSDT_VERIFICATION_INFO.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwFriendReqNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (220 < this.dwFriendReqNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astVerificationList.Length < this.dwFriendReqNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwFriendReqNum; i++)
                {
                    type = this.astVerificationList[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwResult);
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
            type = srcBuf.readUInt32(ref this.dwFriendReqNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (220 < this.dwFriendReqNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwFriendReqNum; i++)
                {
                    type = this.astVerificationList[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwResult);
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

