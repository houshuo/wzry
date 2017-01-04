namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ITEM_DETAIL : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bReverse;
        public static readonly int CLASS_ID = 0x47;
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
            this.bReverse = 0;
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
            this.bReverse = 0;
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
            type = destBuf.writeUInt8(this.bReverse);
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
            if (selector <= 6L)
            {
                this.select_2_6(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_2_6(long selector)
        {
            long num = selector;
            if ((num >= 2L) && (num <= 6L))
            {
                switch (((int) (num - 2L)))
                {
                    case 0:
                        if (!(this.dataObject is COMDT_PROP_DETAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_PROP_DETAIL) ProtocolObjectPool.Get(COMDT_PROP_DETAIL.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is COMDT_EQUIP_DETAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_EQUIP_DETAIL) ProtocolObjectPool.Get(COMDT_EQUIP_DETAIL.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_SYMBOL_DETAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_SYMBOL_DETAIL) ProtocolObjectPool.Get(COMDT_SYMBOL_DETAIL.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is COMDT_GEAR_DETAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_GEAR_DETAIL) ProtocolObjectPool.Get(COMDT_GEAR_DETAIL.CLASS_ID);
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
            type = srcBuf.readUInt8(ref this.bReverse);
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

        public COMDT_EQUIP_DETAIL stEquipInfo
        {
            get
            {
                return (this.dataObject as COMDT_EQUIP_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_GEAR_DETAIL stGearInfo
        {
            get
            {
                return (this.dataObject as COMDT_GEAR_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_PROP_DETAIL stPropInfo
        {
            get
            {
                return (this.dataObject as COMDT_PROP_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_SYMBOL_DETAIL stSymbolInfo
        {
            get
            {
                return (this.dataObject as COMDT_SYMBOL_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

