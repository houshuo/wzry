namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ACNT_CURTASK : ProtocolObject
    {
        public COMDT_PREREQUISITE_DETAIL[] astPrerequisiteInfo = new COMDT_PREREQUISITE_DETAIL[2];
        public static readonly uint BASEVERSION = 1;
        public byte bPrerequisiteNum;
        public byte bTaskState;
        public static readonly int CLASS_ID = 0xdd;
        public static readonly uint CURRVERSION = 0x25;
        public uint dwBaseID;
        public uint dwBelongHeroID;
        public uint dwGetTime;
        public uint dwOnlineTime;
        public static readonly uint VERSION_dwBelongHeroID = 0x23;
        public static readonly uint VERSION_dwGetTime = 0x19;
        public static readonly uint VERSION_dwOnlineTime = 0x25;

        public COMDT_ACNT_CURTASK()
        {
            for (int i = 0; i < 2; i++)
            {
                this.astPrerequisiteInfo[i] = (COMDT_PREREQUISITE_DETAIL) ProtocolObjectPool.Get(COMDT_PREREQUISITE_DETAIL.CLASS_ID);
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
            this.dwBaseID = 0;
            this.bTaskState = 0;
            this.dwGetTime = 0;
            this.dwOnlineTime = 0;
            this.bPrerequisiteNum = 0;
            if (this.astPrerequisiteInfo != null)
            {
                for (int i = 0; i < this.astPrerequisiteInfo.Length; i++)
                {
                    if (this.astPrerequisiteInfo[i] != null)
                    {
                        this.astPrerequisiteInfo[i].Release();
                        this.astPrerequisiteInfo[i] = null;
                    }
                }
            }
            this.dwBelongHeroID = 0;
        }

        public override void OnUse()
        {
            if (this.astPrerequisiteInfo != null)
            {
                for (int i = 0; i < this.astPrerequisiteInfo.Length; i++)
                {
                    this.astPrerequisiteInfo[i] = (COMDT_PREREQUISITE_DETAIL) ProtocolObjectPool.Get(COMDT_PREREQUISITE_DETAIL.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwBaseID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt8(this.bTaskState);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwGetTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwGetTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwOnlineTime <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwOnlineTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bPrerequisiteNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (2 < this.bPrerequisiteNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astPrerequisiteInfo.Length < this.bPrerequisiteNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bPrerequisiteNum; i++)
                {
                    type = this.astPrerequisiteInfo[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwBelongHeroID <= cutVer)
                {
                    type = destBuf.writeUInt32(this.dwBelongHeroID);
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
            type = srcBuf.readUInt32(ref this.dwBaseID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt8(ref this.bTaskState);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (VERSION_dwGetTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwGetTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwGetTime = 0;
                }
                if (VERSION_dwOnlineTime <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwOnlineTime);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.dwOnlineTime = 0;
                }
                type = srcBuf.readUInt8(ref this.bPrerequisiteNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (2 < this.bPrerequisiteNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bPrerequisiteNum; i++)
                {
                    type = this.astPrerequisiteInfo[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_dwBelongHeroID <= cutVer)
                {
                    type = srcBuf.readUInt32(ref this.dwBelongHeroID);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.dwBelongHeroID = 0;
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

