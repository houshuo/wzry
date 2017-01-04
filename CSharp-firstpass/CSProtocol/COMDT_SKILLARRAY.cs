namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_SKILLARRAY : ProtocolObject
    {
        public COMDT_SKILLINFO[] astSkillInfo = new COMDT_SKILLINFO[5];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x59;
        public static readonly uint CURRVERSION = 0x1d;
        public uint dwSelSkillID;
        public static readonly uint VERSION_dwSelSkillID = 0x1d;

        public COMDT_SKILLARRAY()
        {
            for (int i = 0; i < 5; i++)
            {
                this.astSkillInfo[i] = (COMDT_SKILLINFO) ProtocolObjectPool.Get(COMDT_SKILLINFO.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            for (int i = 0; i < 5; i++)
            {
                type = this.astSkillInfo[i].construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            this.dwSelSkillID = 0;
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            if (this.astSkillInfo != null)
            {
                for (int i = 0; i < this.astSkillInfo.Length; i++)
                {
                    if (this.astSkillInfo[i] != null)
                    {
                        this.astSkillInfo[i].Release();
                        this.astSkillInfo[i] = null;
                    }
                }
            }
            this.dwSelSkillID = 0;
        }

        public override void OnUse()
        {
            if (this.astSkillInfo != null)
            {
                for (int i = 0; i < this.astSkillInfo.Length; i++)
                {
                    this.astSkillInfo[i] = (COMDT_SKILLINFO) ProtocolObjectPool.Get(COMDT_SKILLINFO.CLASS_ID);
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
            for (int i = 0; i < 5; i++)
            {
                type = this.astSkillInfo[i].pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            if (VERSION_dwSelSkillID <= cutVer)
            {
                type = destBuf.writeUInt32(this.dwSelSkillID);
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
            for (int i = 0; i < 5; i++)
            {
                type = this.astSkillInfo[i].unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            if (VERSION_dwSelSkillID <= cutVer)
            {
                type = srcBuf.readUInt32(ref this.dwSelSkillID);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            this.dwSelSkillID = 0;
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

