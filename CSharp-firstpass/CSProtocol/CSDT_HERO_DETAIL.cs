namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_HERO_DETAIL : ProtocolObject
    {
        public CSDT_HERO_WEARINFO[] astGearWear = new CSDT_HERO_WEARINFO[6];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x2ba;
        public static readonly uint CURRVERSION = 0x8a;
        public COMDT_HERO_COMMON_INFO stCommonInfo = ((COMDT_HERO_COMMON_INFO) ProtocolObjectPool.Get(COMDT_HERO_COMMON_INFO.CLASS_ID));

        public CSDT_HERO_DETAIL()
        {
            for (int i = 0; i < 6; i++)
            {
                this.astGearWear[i] = (CSDT_HERO_WEARINFO) ProtocolObjectPool.Get(CSDT_HERO_WEARINFO.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = this.stCommonInfo.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 6; i++)
                {
                    type = this.astGearWear[i].construct();
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
            }
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            if (this.stCommonInfo != null)
            {
                this.stCommonInfo.Release();
                this.stCommonInfo = null;
            }
            if (this.astGearWear != null)
            {
                for (int i = 0; i < this.astGearWear.Length; i++)
                {
                    if (this.astGearWear[i] != null)
                    {
                        this.astGearWear[i].Release();
                        this.astGearWear[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            this.stCommonInfo = (COMDT_HERO_COMMON_INFO) ProtocolObjectPool.Get(COMDT_HERO_COMMON_INFO.CLASS_ID);
            if (this.astGearWear != null)
            {
                for (int i = 0; i < this.astGearWear.Length; i++)
                {
                    this.astGearWear[i] = (CSDT_HERO_WEARINFO) ProtocolObjectPool.Get(CSDT_HERO_WEARINFO.CLASS_ID);
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
            type = this.stCommonInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 6; i++)
                {
                    type = this.astGearWear[i].pack(ref destBuf, cutVer);
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
            type = this.stCommonInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                for (int i = 0; i < 6; i++)
                {
                    type = this.astGearWear[i].unpack(ref srcBuf, cutVer);
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

