namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ARENA_BLOCK : ProtocolObject
    {
        public COMDT_ARENA_BLOCKDATA[] astData = new COMDT_ARENA_BLOCKDATA[200];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 460;
        public static readonly uint CURRVERSION = 0x43;
        public uint dwUsedNum;

        public COMDT_ARENA_BLOCK()
        {
            for (int i = 0; i < 200; i++)
            {
                this.astData[i] = (COMDT_ARENA_BLOCKDATA) ProtocolObjectPool.Get(COMDT_ARENA_BLOCKDATA.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.dwUsedNum = 0;
            for (int i = 0; i < 200; i++)
            {
                type = this.astData[i].construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
            this.dwUsedNum = 0;
            if (this.astData != null)
            {
                for (int i = 0; i < this.astData.Length; i++)
                {
                    if (this.astData[i] != null)
                    {
                        this.astData[i].Release();
                        this.astData[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astData != null)
            {
                for (int i = 0; i < this.astData.Length; i++)
                {
                    this.astData[i] = (COMDT_ARENA_BLOCKDATA) ProtocolObjectPool.Get(COMDT_ARENA_BLOCKDATA.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwUsedNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (200 < this.dwUsedNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astData.Length < this.dwUsedNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwUsedNum; i++)
                {
                    type = this.astData[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwUsedNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (200 < this.dwUsedNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwUsedNum; i++)
                {
                    type = this.astData[i].unpack(ref srcBuf, cutVer);
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

