namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_GAMING_CSSYNCUN : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bReserve;
        public static readonly int CLASS_ID = 0x3ff;
        public static readonly uint CURRVERSION = 1;
        public ProtocolObject dataObject;

        public TdrError.ErrorType construct(long selector)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.construct();
            }
            this.bReserve = 0;
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            this.bReserve = 0;
        }

        public TdrError.ErrorType pack(long selector, ref TdrWriteBuf destBuf, uint cutVer)
        {
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.pack(ref destBuf, cutVer);
            }
            type = destBuf.writeUInt8(this.bReserve);
            if (type != TdrError.ErrorType.TDR_NO_ERROR)
            {
                return type;
            }
            return type;
        }

        public TdrError.ErrorType pack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if ((buffer.GetLength(0) == 0) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrWriteBuf destBuf = ClassObjPool<TdrWriteBuf>.Get();
            destBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.pack(selector, ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();
                usedSize = destBuf.getUsedSize();
            }
            destBuf.Release();
            return type;
        }

        public ProtocolObject select(long selector)
        {
            if (selector <= 3L)
            {
                this.select_1_3(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_1_3(long selector)
        {
            long num = selector;
            if ((num >= 1L) && (num <= 3L))
            {
                switch (((int) (num - 1L)))
                {
                    case 0:
                        if (!(this.dataObject is CSDT_CMD_USEOBJECTIVESKILL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_CMD_USEOBJECTIVESKILL) ProtocolObjectPool.Get(CSDT_CMD_USEOBJECTIVESKILL.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is CSDT_CMD_USEDIRECTIONALSKILL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_CMD_USEDIRECTIONALSKILL) ProtocolObjectPool.Get(CSDT_CMD_USEDIRECTIONALSKILL.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is CSDT_CMD_USEPOSITIONSKILL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_CMD_USEPOSITIONSKILL) ProtocolObjectPool.Get(CSDT_CMD_USEPOSITIONSKILL.CLASS_ID);
                        }
                        return;
                }
            }
            if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
        }

        public TdrError.ErrorType unpack(long selector, ref TdrReadBuf srcBuf, uint cutVer)
        {
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.unpack(ref srcBuf, cutVer);
            }
            type = srcBuf.readUInt8(ref this.bReserve);
            if (type != TdrError.ErrorType.TDR_NO_ERROR)
            {
                return type;
            }
            return type;
        }

        public TdrError.ErrorType unpack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if ((buffer.GetLength(0) == 0) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = ClassObjPool<TdrReadBuf>.Get();
            srcBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.unpack(selector, ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            srcBuf.Release();
            return type;
        }

        public CSDT_CMD_USEDIRECTIONALSKILL stDirectionSkill
        {
            get
            {
                return (this.dataObject as CSDT_CMD_USEDIRECTIONALSKILL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_CMD_USEOBJECTIVESKILL stObjectiveSkill
        {
            get
            {
                return (this.dataObject as CSDT_CMD_USEOBJECTIVESKILL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_CMD_USEPOSITIONSKILL stPositionSkill
        {
            get
            {
                return (this.dataObject as CSDT_CMD_USEPOSITIONSKILL);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

