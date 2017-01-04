namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_OFFLINE_CHAT_NTF : ProtocolObject
    {
        public CSDT_OFFLINE_CHAT_INFO[] astChatInfo = new CSDT_OFFLINE_CHAT_INFO[50];
        public static readonly uint BASEVERSION = 1;
        public byte bChatCnt;
        public static readonly int CLASS_ID = 0x37d;
        public static readonly uint CURRVERSION = 1;

        public SCPKG_OFFLINE_CHAT_NTF()
        {
            for (int i = 0; i < 50; i++)
            {
                this.astChatInfo[i] = (CSDT_OFFLINE_CHAT_INFO) ProtocolObjectPool.Get(CSDT_OFFLINE_CHAT_INFO.CLASS_ID);
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
            this.bChatCnt = 0;
            if (this.astChatInfo != null)
            {
                for (int i = 0; i < this.astChatInfo.Length; i++)
                {
                    if (this.astChatInfo[i] != null)
                    {
                        this.astChatInfo[i].Release();
                        this.astChatInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astChatInfo != null)
            {
                for (int i = 0; i < this.astChatInfo.Length; i++)
                {
                    this.astChatInfo[i] = (CSDT_OFFLINE_CHAT_INFO) ProtocolObjectPool.Get(CSDT_OFFLINE_CHAT_INFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bChatCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (50 < this.bChatCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astChatInfo.Length < this.bChatCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bChatCnt; i++)
                {
                    type = this.astChatInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bChatCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (50 < this.bChatCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bChatCnt; i++)
                {
                    type = this.astChatInfo[i].unpack(ref srcBuf, cutVer);
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

