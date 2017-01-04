namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_FRIEND_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bGameState;
        public byte bVideoState;
        public static readonly int CLASS_ID = 0x33a;
        public static readonly uint CURRVERSION = 0x74;
        public uint dwGameStartTime;
        public COMDT_FRIEND_INFO stFriendInfo = ((COMDT_FRIEND_INFO) ProtocolObjectPool.Get(COMDT_FRIEND_INFO.CLASS_ID));
        public CSDT_GAMEINFO stGameInfo = ((CSDT_GAMEINFO) ProtocolObjectPool.Get(CSDT_GAMEINFO.CLASS_ID));
        public COMDT_INTIMACY_DATA stIntimacyData = ((COMDT_INTIMACY_DATA) ProtocolObjectPool.Get(COMDT_INTIMACY_DATA.CLASS_ID));
        public ulong ullDonateAPSec;

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
            if (this.stFriendInfo != null)
            {
                this.stFriendInfo.Release();
                this.stFriendInfo = null;
            }
            this.ullDonateAPSec = 0L;
            this.bGameState = 0;
            this.dwGameStartTime = 0;
            if (this.stIntimacyData != null)
            {
                this.stIntimacyData.Release();
                this.stIntimacyData = null;
            }
            this.bVideoState = 0;
            if (this.stGameInfo != null)
            {
                this.stGameInfo.Release();
                this.stGameInfo = null;
            }
        }

        public override void OnUse()
        {
            this.stFriendInfo = (COMDT_FRIEND_INFO) ProtocolObjectPool.Get(COMDT_FRIEND_INFO.CLASS_ID);
            this.stIntimacyData = (COMDT_INTIMACY_DATA) ProtocolObjectPool.Get(COMDT_INTIMACY_DATA.CLASS_ID);
            this.stGameInfo = (CSDT_GAMEINFO) ProtocolObjectPool.Get(CSDT_GAMEINFO.CLASS_ID);
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
            type = this.stFriendInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt64(this.ullDonateAPSec);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bGameState);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwGameStartTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stIntimacyData.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bVideoState);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                long bVideoState = this.bVideoState;
                type = this.stGameInfo.pack(bVideoState, ref destBuf, cutVer);
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
            type = this.stFriendInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt64(ref this.ullDonateAPSec);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bGameState);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwGameStartTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stIntimacyData.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bVideoState);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                long bVideoState = this.bVideoState;
                type = this.stGameInfo.unpack(bVideoState, ref srcBuf, cutVer);
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

