namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_SELFDEFINE_CHATINFO : ProtocolObject
    {
        public COMDT_SELFDEFINE_DETAIL_CHATINFO[] astChatMsg = new COMDT_SELFDEFINE_DETAIL_CHATINFO[30];
        public static readonly uint BASEVERSION = 1;
        public byte bMsgCnt;
        public static readonly int CLASS_ID = 0x102;
        public static readonly uint CURRVERSION = 1;

        public COMDT_SELFDEFINE_CHATINFO()
        {
            for (int i = 0; i < 30; i++)
            {
                this.astChatMsg[i] = (COMDT_SELFDEFINE_DETAIL_CHATINFO) ProtocolObjectPool.Get(COMDT_SELFDEFINE_DETAIL_CHATINFO.CLASS_ID);
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
            this.bMsgCnt = 0;
            if (this.astChatMsg != null)
            {
                for (int i = 0; i < this.astChatMsg.Length; i++)
                {
                    if (this.astChatMsg[i] != null)
                    {
                        this.astChatMsg[i].Release();
                        this.astChatMsg[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astChatMsg != null)
            {
                for (int i = 0; i < this.astChatMsg.Length; i++)
                {
                    this.astChatMsg[i] = (COMDT_SELFDEFINE_DETAIL_CHATINFO) ProtocolObjectPool.Get(COMDT_SELFDEFINE_DETAIL_CHATINFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bMsgCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (30 < this.bMsgCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astChatMsg.Length < this.bMsgCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bMsgCnt; i++)
                {
                    type = this.astChatMsg[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bMsgCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (30 < this.bMsgCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bMsgCnt; i++)
                {
                    type = this.astChatMsg[i].unpack(ref srcBuf, cutVer);
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

