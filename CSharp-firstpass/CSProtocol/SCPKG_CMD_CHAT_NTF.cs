namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_CMD_CHAT_NTF : ProtocolObject
    {
        public COMDT_CHAT_MSG[] astChatMsg = new COMDT_CHAT_MSG[100];
        public static readonly uint BASEVERSION = 1;
        public byte bMsgCnt;
        public byte bNeedSetChatFreeCnt;
        public static readonly int CLASS_ID = 0x379;
        public static readonly uint CURRVERSION = 0x3e;
        public uint dwRestChatFreeCnt;
        public uint dwTimeStamp;
        public static readonly uint VERSION_bNeedSetChatFreeCnt = 0x3e;
        public static readonly uint VERSION_dwRestChatFreeCnt = 0x3e;

        public SCPKG_CMD_CHAT_NTF()
        {
            for (int i = 0; i < 100; i++)
            {
                this.astChatMsg[i] = (COMDT_CHAT_MSG) ProtocolObjectPool.Get(COMDT_CHAT_MSG.CLASS_ID);
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
            this.dwTimeStamp = 0;
            this.bNeedSetChatFreeCnt = 0;
            this.dwRestChatFreeCnt = 0;
        }

        public override void OnUse()
        {
            if (this.astChatMsg != null)
            {
                for (int i = 0; i < this.astChatMsg.Length; i++)
                {
                    this.astChatMsg[i] = (COMDT_CHAT_MSG) ProtocolObjectPool.Get(COMDT_CHAT_MSG.CLASS_ID);
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
                if (100 < this.bMsgCnt)
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
                type = destBuf.writeUInt32(this.dwTimeStamp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_bNeedSetChatFreeCnt <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bNeedSetChatFreeCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwRestChatFreeCnt <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwRestChatFreeCnt);
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
                if (100 < this.bMsgCnt)
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
                type = srcBuf.readUInt32(ref this.dwTimeStamp);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_bNeedSetChatFreeCnt <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bNeedSetChatFreeCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bNeedSetChatFreeCnt = 0;
                }
                if (VERSION_dwRestChatFreeCnt <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwRestChatFreeCnt);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwRestChatFreeCnt = 0;
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

