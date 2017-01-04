namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_NTF_SNS_NICKNAME : ProtocolObject
    {
        public CSDT_SNS_NICKNAME[] astSnsNameList = new CSDT_SNS_NICKNAME[500];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x36b;
        public static readonly uint CURRVERSION = 1;
        public uint dwSnsFriendNum;

        public SCPKG_NTF_SNS_NICKNAME()
        {
            for (int i = 0; i < 500; i++)
            {
                this.astSnsNameList[i] = (CSDT_SNS_NICKNAME) ProtocolObjectPool.Get(CSDT_SNS_NICKNAME.CLASS_ID);
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
            if (this.astSnsNameList != null)
            {
                for (int i = 0; i < this.astSnsNameList.Length; i++)
                {
                    if (this.astSnsNameList[i] != null)
                    {
                        this.astSnsNameList[i].Release();
                        this.astSnsNameList[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astSnsNameList != null)
            {
                for (int i = 0; i < this.astSnsNameList.Length; i++)
                {
                    this.astSnsNameList[i] = (CSDT_SNS_NICKNAME) ProtocolObjectPool.Get(CSDT_SNS_NICKNAME.CLASS_ID);
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
                if (500 < this.dwSnsFriendNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astSnsNameList.Length < this.dwSnsFriendNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwSnsFriendNum; i++)
                {
                    type = this.astSnsNameList[i].pack(ref destBuf, cutVer);
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
                if (500 < this.dwSnsFriendNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwSnsFriendNum; i++)
                {
                    type = this.astSnsNameList[i].unpack(ref srcBuf, cutVer);
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

