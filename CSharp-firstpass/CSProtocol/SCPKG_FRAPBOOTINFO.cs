namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_FRAPBOOTINFO : ProtocolObject
    {
        public SCPKG_FRAPBOOT_SINGLE[] astSpareFrap = new SCPKG_FRAPBOOT_SINGLE[4];
        public static readonly uint BASEVERSION = 1;
        public byte bSpareNum;
        public static readonly int CLASS_ID = 0x40e;
        public static readonly uint CURRVERSION = 1;

        public SCPKG_FRAPBOOTINFO()
        {
            for (int i = 0; i < 4; i++)
            {
                this.astSpareFrap[i] = (SCPKG_FRAPBOOT_SINGLE) ProtocolObjectPool.Get(SCPKG_FRAPBOOT_SINGLE.CLASS_ID);
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
            this.bSpareNum = 0;
            if (this.astSpareFrap != null)
            {
                for (int i = 0; i < this.astSpareFrap.Length; i++)
                {
                    if (this.astSpareFrap[i] != null)
                    {
                        this.astSpareFrap[i].Release();
                        this.astSpareFrap[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astSpareFrap != null)
            {
                for (int i = 0; i < this.astSpareFrap.Length; i++)
                {
                    this.astSpareFrap[i] = (SCPKG_FRAPBOOT_SINGLE) ProtocolObjectPool.Get(SCPKG_FRAPBOOT_SINGLE.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bSpareNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (4 < this.bSpareNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astSpareFrap.Length < this.bSpareNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bSpareNum; i++)
                {
                    type = this.astSpareFrap[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bSpareNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (4 < this.bSpareNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bSpareNum; i++)
                {
                    type = this.astSpareFrap[i].unpack(ref srcBuf, cutVer);
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

