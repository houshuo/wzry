namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_NTF_SNS_FRIEND : ProtocolObject
    {
        public CSDT_SNS_FRIEND_INFO[] astSnsFriendList = new CSDT_SNS_FRIEND_INFO[100];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x374;
        public static readonly uint CURRVERSION = 0x63;
        public uint dwSnsFriendNum;

        public SCPKG_NTF_SNS_FRIEND()
        {
            for (int i = 0; i < 100; i++)
            {
                this.astSnsFriendList[i] = (CSDT_SNS_FRIEND_INFO) ProtocolObjectPool.Get(CSDT_SNS_FRIEND_INFO.CLASS_ID);
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
            this.dwSnsFriendNum = 0;
            if (this.astSnsFriendList != null)
            {
                for (int i = 0; i < this.astSnsFriendList.Length; i++)
                {
                    if (this.astSnsFriendList[i] != null)
                    {
                        this.astSnsFriendList[i].Release();
                        this.astSnsFriendList[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astSnsFriendList != null)
            {
                for (int i = 0; i < this.astSnsFriendList.Length; i++)
                {
                    this.astSnsFriendList[i] = (CSDT_SNS_FRIEND_INFO) ProtocolObjectPool.Get(CSDT_SNS_FRIEND_INFO.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwSnsFriendNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (100 < this.dwSnsFriendNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astSnsFriendList.Length < this.dwSnsFriendNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwSnsFriendNum; i++)
                {
                    type = this.astSnsFriendList[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwSnsFriendNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (100 < this.dwSnsFriendNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwSnsFriendNum; i++)
                {
                    type = this.astSnsFriendList[i].unpack(ref srcBuf, cutVer);
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

