namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ROOMCHG_DT : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public sbyte chReserve;
        public static readonly int CLASS_ID = 0x38;
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
            this.chReserve = 0;
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
            this.chReserve = 0;
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
            type = destBuf.writeInt8(this.chReserve);
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
            if (selector <= 5L)
            {
                this.select_0_5(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_0_5(long selector)
        {
            long num = selector;
            if ((num >= 0L) && (num <= 5L))
            {
                switch (((int) num))
                {
                    case 0:
                        if (!(this.dataObject is COMDT_ROOMCHG_PLAYERADD))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_ROOMCHG_PLAYERADD) ProtocolObjectPool.Get(COMDT_ROOMCHG_PLAYERADD.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is COMDT_ROOMCHG_PLAYERLEAVE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_ROOMCHG_PLAYERLEAVE) ProtocolObjectPool.Get(COMDT_ROOMCHG_PLAYERLEAVE.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_ROOMCHG_STATECHG))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_ROOMCHG_STATECHG) ProtocolObjectPool.Get(COMDT_ROOMCHG_STATECHG.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is COMDT_ROOMCHG_MASTERCHG))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_ROOMCHG_MASTERCHG) ProtocolObjectPool.Get(COMDT_ROOMCHG_MASTERCHG.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is COMDT_ROOMCHG_CHGMEMBERPOS))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_ROOMCHG_CHGMEMBERPOS) ProtocolObjectPool.Get(COMDT_ROOMCHG_CHGMEMBERPOS.CLASS_ID);
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
            type = srcBuf.readInt8(ref this.chReserve);
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

        public COMDT_ROOMCHG_CHGMEMBERPOS stChgMemberPos
        {
            get
            {
                return (this.dataObject as COMDT_ROOMCHG_CHGMEMBERPOS);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_ROOMCHG_MASTERCHG stMasterChg
        {
            get
            {
                return (this.dataObject as COMDT_ROOMCHG_MASTERCHG);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_ROOMCHG_PLAYERADD stPlayerAdd
        {
            get
            {
                return (this.dataObject as COMDT_ROOMCHG_PLAYERADD);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_ROOMCHG_PLAYERLEAVE stPlayerLeave
        {
            get
            {
                return (this.dataObject as COMDT_ROOMCHG_PLAYERLEAVE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_ROOMCHG_STATECHG stStateChg
        {
            get
            {
                return (this.dataObject as COMDT_ROOMCHG_STATECHG);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

