namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_WEAL_UNION : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x1eb;
        public static readonly uint CURRVERSION = 0x7f;
        public ProtocolObject dataObject;
        public static readonly uint VERSION_stCondition = 12;
        public static readonly uint VERSION_stExchange = 0x4c;
        public static readonly uint VERSION_stPtExchange = 0x7f;

        public TdrError.ErrorType construct(long selector)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.construct();
            }
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
                        if (!(this.dataObject is COMDT_WEAL_CHECKIN_DETAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_WEAL_CHECKIN_DETAIL) ProtocolObjectPool.Get(COMDT_WEAL_CHECKIN_DETAIL.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is COMDT_WEAL_FIXEDTIME_DETAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_WEAL_FIXEDTIME_DETAIL) ProtocolObjectPool.Get(COMDT_WEAL_FIXEDTIME_DETAIL.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is COMDT_WEAL_MUTIPLE_DETAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_WEAL_MUTIPLE_DETAIL) ProtocolObjectPool.Get(COMDT_WEAL_MUTIPLE_DETAIL.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_WEAL_CONDITION_DETAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_WEAL_CONDITION_DETAIL) ProtocolObjectPool.Get(COMDT_WEAL_CONDITION_DETAIL.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is COMDT_WEAL_EXCHANGE_DETAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_WEAL_EXCHANGE_DETAIL) ProtocolObjectPool.Get(COMDT_WEAL_EXCHANGE_DETAIL.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is COMDT_WEAL_EXCHANGE_DETAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_WEAL_EXCHANGE_DETAIL) ProtocolObjectPool.Get(COMDT_WEAL_EXCHANGE_DETAIL.CLASS_ID);
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

        public COMDT_WEAL_CHECKIN_DETAIL stCheckIn
        {
            get
            {
                return (this.dataObject as COMDT_WEAL_CHECKIN_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_WEAL_CONDITION_DETAIL stCondition
        {
            get
            {
                return (this.dataObject as COMDT_WEAL_CONDITION_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_WEAL_EXCHANGE_DETAIL stExchange
        {
            get
            {
                return (this.dataObject as COMDT_WEAL_EXCHANGE_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_WEAL_FIXEDTIME_DETAIL stFixedTime
        {
            get
            {
                return (this.dataObject as COMDT_WEAL_FIXEDTIME_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_WEAL_MUTIPLE_DETAIL stMultiple
        {
            get
            {
                return (this.dataObject as COMDT_WEAL_MUTIPLE_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_WEAL_EXCHANGE_DETAIL stPtExchange
        {
            get
            {
                return (this.dataObject as COMDT_WEAL_EXCHANGE_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

