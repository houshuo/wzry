namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_CREATE_TVOIP_ROOM_NTF : ProtocolObject
    {
        public CSDT_TVOIP_IP_INFO[] astAccessIPList = new CSDT_TVOIP_IP_INFO[0x10];
        public CSDT_TVOIP_ROOM_USER_INFO[] astRoomUserList;
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x4e2;
        public static readonly uint CURRVERSION = 1;
        public uint dwRoomUserCnt;
        public ulong ullRoomID;
        public ulong ullRoomKey;
        public ushort wAccessIPCount;

        public SCPKG_CREATE_TVOIP_ROOM_NTF()
        {
            for (int i = 0; i < 0x10; i++)
            {
                this.astAccessIPList[i] = (CSDT_TVOIP_IP_INFO) ProtocolObjectPool.Get(CSDT_TVOIP_IP_INFO.CLASS_ID);
            }
            this.astRoomUserList = new CSDT_TVOIP_ROOM_USER_INFO[0x10];
            for (int j = 0; j < 0x10; j++)
            {
                this.astRoomUserList[j] = (CSDT_TVOIP_ROOM_USER_INFO) ProtocolObjectPool.Get(CSDT_TVOIP_ROOM_USER_INFO.CLASS_ID);
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
            this.ullRoomID = 0L;
            this.ullRoomKey = 0L;
            this.wAccessIPCount = 0;
            if (this.astAccessIPList != null)
            {
                for (int i = 0; i < this.astAccessIPList.Length; i++)
                {
                    if (this.astAccessIPList[i] != null)
                    {
                        this.astAccessIPList[i].Release();
                        this.astAccessIPList[i] = null;
                    }
                }
            }
            this.dwRoomUserCnt = 0;
            if (this.astRoomUserList != null)
            {
                for (int j = 0; j < this.astRoomUserList.Length; j++)
                {
                    if (this.astRoomUserList[j] != null)
                    {
                        this.astRoomUserList[j].Release();
                        this.astRoomUserList[j] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astAccessIPList != null)
            {
                for (int i = 0; i < this.astAccessIPList.Length; i++)
                {
                    this.astAccessIPList[i] = (CSDT_TVOIP_IP_INFO) ProtocolObjectPool.Get(CSDT_TVOIP_IP_INFO.CLASS_ID);
                }
            }
            if (this.astRoomUserList != null)
            {
                for (int j = 0; j < this.astRoomUserList.Length; j++)
                {
                    this.astRoomUserList[j] = (CSDT_TVOIP_ROOM_USER_INFO) ProtocolObjectPool.Get(CSDT_TVOIP_ROOM_USER_INFO.CLASS_ID);
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
            type = destBuf.writeUInt64(this.ullRoomID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt64(this.ullRoomKey);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt16(this.wAccessIPCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x10 < this.wAccessIPCount)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astAccessIPList.Length < this.wAccessIPCount)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.wAccessIPCount; i++)
                {
                    type = this.astAccessIPList[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwRoomUserCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x10 < this.dwRoomUserCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astRoomUserList.Length < this.dwRoomUserCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int j = 0; j < this.dwRoomUserCnt; j++)
                {
                    type = this.astRoomUserList[j].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt64(ref this.ullRoomID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt64(ref this.ullRoomKey);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wAccessIPCount);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x10 < this.wAccessIPCount)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.wAccessIPCount; i++)
                {
                    type = this.astAccessIPList[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwRoomUserCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x10 < this.dwRoomUserCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int j = 0; j < this.dwRoomUserCnt; j++)
                {
                    type = this.astRoomUserList[j].unpack(ref srcBuf, cutVer);
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

