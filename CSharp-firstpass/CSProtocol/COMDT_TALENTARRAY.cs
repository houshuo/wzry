namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_TALENTARRAY : ProtocolObject
    {
        public COMDT_TALENTINFO[] astTalentInfo = new COMDT_TALENTINFO[20];
        public static readonly uint BASEVERSION = 1;
        public byte bWakeState;
        public static readonly int CLASS_ID = 0x5f;
        public static readonly uint CURRVERSION = 0x23;
        public COMDT_HEROWAKE_STEP stWakeStep;
        public static readonly uint VERSION_stWakeStep = 0x23;

        public COMDT_TALENTARRAY()
        {
            for (int i = 0; i < 20; i++)
            {
                this.astTalentInfo[i] = (COMDT_TALENTINFO) ProtocolObjectPool.Get(COMDT_TALENTINFO.CLASS_ID);
            }
            this.stWakeStep = (COMDT_HEROWAKE_STEP) ProtocolObjectPool.Get(COMDT_HEROWAKE_STEP.CLASS_ID);
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
            this.bWakeState = 0;
            if (this.astTalentInfo != null)
            {
                for (int i = 0; i < this.astTalentInfo.Length; i++)
                {
                    if (this.astTalentInfo[i] != null)
                    {
                        this.astTalentInfo[i].Release();
                        this.astTalentInfo[i] = null;
                    }
                }
            }
            if (this.stWakeStep != null)
            {
                this.stWakeStep.Release();
                this.stWakeStep = null;
            }
        }

        public override void OnUse()
        {
            if (this.astTalentInfo != null)
            {
                for (int i = 0; i < this.astTalentInfo.Length; i++)
                {
                    this.astTalentInfo[i] = (COMDT_TALENTINFO) ProtocolObjectPool.Get(COMDT_TALENTINFO.CLASS_ID);
                }
            }
            this.stWakeStep = (COMDT_HEROWAKE_STEP) ProtocolObjectPool.Get(COMDT_HEROWAKE_STEP.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bWakeState);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 20; i++)
                {
                    type = this.astTalentInfo[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stWakeStep <= cutVer)
                {
                    type = this.stWakeStep.pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bWakeState);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 20; i++)
                {
                    type = this.astTalentInfo[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_stWakeStep <= cutVer)
                {
                    type = this.stWakeStep.unpack(ref srcBuf, cutVer);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                type = this.stWakeStep.construct();
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

