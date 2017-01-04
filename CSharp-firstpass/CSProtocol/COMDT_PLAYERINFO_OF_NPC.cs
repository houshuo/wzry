namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_PLAYERINFO_OF_NPC : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bReserved;
        public static readonly int CLASS_ID = 0x70;
        public static readonly uint CURRVERSION = 0x42;
        public uint dwFakeLogicWorldID;
        public uint dwFakePvpLevel;
        public ulong ullFakeUid;
        public static readonly uint VERSION_dwFakeLogicWorldID = 0x42;
        public static readonly uint VERSION_dwFakePvpLevel = 0x42;
        public static readonly uint VERSION_ullFakeUid = 0x42;

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
            this.bReserved = 0;
            this.ullFakeUid = 0L;
            this.dwFakeLogicWorldID = 0;
            this.dwFakePvpLevel = 0;
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
            type = destBuf.writeUInt8(this.bReserved);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (VERSION_ullFakeUid <= cutVer)
                {
                    type = destBuf.writeUInt64(this.ullFakeUid);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwFakeLogicWorldID <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwFakeLogicWorldID);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwFakePvpLevel <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwFakePvpLevel);
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
            type = srcBuf.readUInt8(ref this.bReserved);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (VERSION_ullFakeUid <= cutVer)
                {
                    type = srcBuf.readUInt64(ref this.ullFakeUid);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.ullFakeUid = 0L;
                }
                if (VERSION_dwFakeLogicWorldID <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwFakeLogicWorldID);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwFakeLogicWorldID = 0;
                }
                if (VERSION_dwFakePvpLevel <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwFakePvpLevel);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwFakePvpLevel = 0;
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

