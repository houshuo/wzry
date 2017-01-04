namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ITEM_UNION : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x1fd;
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
                        if (!(this.dataObject is COMDT_ITEM_PACKAGEINFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_ITEM_PACKAGEINFO) ProtocolObjectPool.Get(COMDT_ITEM_PACKAGEINFO.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is COMDT_ITEM_PACKAGEINFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_ITEM_PACKAGEINFO) ProtocolObjectPool.Get(COMDT_ITEM_PACKAGEINFO.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_ITEM_PACKAGEINFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_ITEM_PACKAGEINFO) ProtocolObjectPool.Get(COMDT_ITEM_PACKAGEINFO.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is COMDT_ITEM_WEARINFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_ITEM_WEARINFO) ProtocolObjectPool.Get(COMDT_ITEM_WEARINFO.CLASS_ID);
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

        public COMDT_ITEM_PACKAGEINFO stItemEquip
        {
            get
            {
                return (this.dataObject as COMDT_ITEM_PACKAGEINFO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_ITEM_WEARINFO stItemGear
        {
            get
            {
                return (this.dataObject as COMDT_ITEM_WEARINFO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_ITEM_PACKAGEINFO stItemProp
        {
            get
            {
                return (this.dataObject as COMDT_ITEM_PACKAGEINFO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_ITEM_PACKAGEINFO stItemSymbol
        {
            get
            {
                return (this.dataObject as COMDT_ITEM_PACKAGEINFO);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

