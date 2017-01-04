namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_MULTGAME_BEGINLOAD : ProtocolObject
    {
        public CSDT_CAMPINFO[] astCampInfo = new CSDT_CAMPINFO[2];
        public static readonly uint BASEVERSION = 1;
        public byte bGameType;
        public byte bKFrapsLater;
        public byte bPreActFrap;
        public static readonly int CLASS_ID = 0x261;
        public static readonly uint CURRVERSION = 0x8a;
        public uint dwCltLogMask;
        public uint dwCltLogSize;
        public uint dwDeskId;
        public uint dwDeskSeq;
        public uint dwHaskChkFreq;
        public uint dwKFrapsFreqMs;
        public uint dwRandomSeed;
        public COMDT_DESKINFO stDeskInfo = ((COMDT_DESKINFO) ProtocolObjectPool.Get(COMDT_DESKINFO.CLASS_ID));

        public SCPKG_MULTGAME_BEGINLOAD()
        {
            for (int i = 0; i < 2; i++)
            {
                this.astCampInfo[i] = (CSDT_CAMPINFO) ProtocolObjectPool.Get(CSDT_CAMPINFO.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.dwDeskId = 0;
            this.dwDeskSeq = 0;
            this.bKFrapsLater = 0;
            this.dwKFrapsFreqMs = 0;
            this.bPreActFrap = 0;
            this.dwRandomSeed = 0;
            this.bGameType = 0;
            type = this.stDeskInfo.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 2; i++)
                {
                    type = this.astCampInfo[i].construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                this.dwHaskChkFreq = 0;
                this.dwCltLogMask = 0;
                this.dwCltLogSize = 0;
            }
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            this.dwDeskId = 0;
            this.dwDeskSeq = 0;
            this.bKFrapsLater = 0;
            this.dwKFrapsFreqMs = 0;
            this.bPreActFrap = 0;
            this.dwRandomSeed = 0;
            this.bGameType = 0;
            if (this.stDeskInfo != null)
            {
                this.stDeskInfo.Release();
                this.stDeskInfo = null;
            }
            if (this.astCampInfo != null)
            {
                for (int i = 0; i < this.astCampInfo.Length; i++)
                {
                    if (this.astCampInfo[i] != null)
                    {
                        this.astCampInfo[i].Release();
                        this.astCampInfo[i] = null;
                    }
                }
            }
            this.dwHaskChkFreq = 0;
            this.dwCltLogMask = 0;
            this.dwCltLogSize = 0;
        }

        public override void OnUse()
        {
            this.stDeskInfo = (COMDT_DESKINFO) ProtocolObjectPool.Get(COMDT_DESKINFO.CLASS_ID);
            if (this.astCampInfo != null)
            {
                for (int i = 0; i < this.astCampInfo.Length; i++)
                {
                    this.astCampInfo[i] = (CSDT_CAMPINFO) ProtocolObjectPool.Get(CSDT_CAMPINFO.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwDeskId);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwDeskSeq);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bKFrapsLater);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwKFrapsFreqMs);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bPreActFrap);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwRandomSeed);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bGameType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stDeskInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 2; i++)
                {
                    type = this.astCampInfo[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwHaskChkFreq);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwCltLogMask);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwCltLogSize);
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
            type = srcBuf.readUInt32(ref this.dwDeskId);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwDeskSeq);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bKFrapsLater);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwKFrapsFreqMs);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bPreActFrap);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwRandomSeed);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bGameType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stDeskInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 2; i++)
                {
                    type = this.astCampInfo[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwHaskChkFreq);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCltLogMask);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCltLogSize);
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

