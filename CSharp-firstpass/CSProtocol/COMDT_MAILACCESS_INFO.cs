namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_MAILACCESS_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0xeb;
        public static readonly uint CURRVERSION = 0x53;
        public ProtocolObject dataObject;
        public static readonly uint VERSION_stExpHero = 0x39;
        public static readonly uint VERSION_stExpSkin = 0x39;
        public static readonly uint VERSION_stHeadImg = 0x53;

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
            if (selector <= 10L)
            {
                this.select_1_10(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_1_10(long selector)
        {
            long num = selector;
            if ((num >= 1L) && (num <= 10L))
            {
                switch (((int) (num - 1L)))
                {
                    case 0:
                        if (!(this.dataObject is COMDT_MAILACCESS_PROP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_MAILACCESS_PROP) ProtocolObjectPool.Get(COMDT_MAILACCESS_PROP.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is COMDT_MAILACCESS_MONEY))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_MAILACCESS_MONEY) ProtocolObjectPool.Get(COMDT_MAILACCESS_MONEY.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is COMDT_MAILACCESS_HEART))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_MAILACCESS_HEART) ProtocolObjectPool.Get(COMDT_MAILACCESS_HEART.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_MAILACCESS_RONGYU))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_MAILACCESS_RONGYU) ProtocolObjectPool.Get(COMDT_MAILACCESS_RONGYU.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is COMDT_MAILACCESS_EXP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_MAILACCESS_EXP) ProtocolObjectPool.Get(COMDT_MAILACCESS_EXP.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is COMDT_MAILACCESS_HERO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_MAILACCESS_HERO) ProtocolObjectPool.Get(COMDT_MAILACCESS_HERO.CLASS_ID);
                        }
                        return;

                    case 6:
                        if (!(this.dataObject is COMDT_MAILACCESS_PIFU))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_MAILACCESS_PIFU) ProtocolObjectPool.Get(COMDT_MAILACCESS_PIFU.CLASS_ID);
                        }
                        return;

                    case 7:
                        if (!(this.dataObject is COMDT_MAILACCESS_EXPHERO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_MAILACCESS_EXPHERO) ProtocolObjectPool.Get(COMDT_MAILACCESS_EXPHERO.CLASS_ID);
                        }
                        return;

                    case 8:
                        if (!(this.dataObject is COMDT_MAILACCESS_EXPSKIN))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_MAILACCESS_EXPSKIN) ProtocolObjectPool.Get(COMDT_MAILACCESS_EXPSKIN.CLASS_ID);
                        }
                        return;

                    case 9:
                        if (!(this.dataObject is COMDT_MAILACCESS_HEADIMG))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_MAILACCESS_HEADIMG) ProtocolObjectPool.Get(COMDT_MAILACCESS_HEADIMG.CLASS_ID);
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

        public COMDT_MAILACCESS_EXP stExp
        {
            get
            {
                return (this.dataObject as COMDT_MAILACCESS_EXP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_MAILACCESS_EXPHERO stExpHero
        {
            get
            {
                return (this.dataObject as COMDT_MAILACCESS_EXPHERO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_MAILACCESS_EXPSKIN stExpSkin
        {
            get
            {
                return (this.dataObject as COMDT_MAILACCESS_EXPSKIN);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_MAILACCESS_HEADIMG stHeadImg
        {
            get
            {
                return (this.dataObject as COMDT_MAILACCESS_HEADIMG);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_MAILACCESS_HEART stHeart
        {
            get
            {
                return (this.dataObject as COMDT_MAILACCESS_HEART);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_MAILACCESS_HERO stHero
        {
            get
            {
                return (this.dataObject as COMDT_MAILACCESS_HERO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_MAILACCESS_MONEY stMoney
        {
            get
            {
                return (this.dataObject as COMDT_MAILACCESS_MONEY);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_MAILACCESS_PIFU stPiFu
        {
            get
            {
                return (this.dataObject as COMDT_MAILACCESS_PIFU);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_MAILACCESS_PROP stProp
        {
            get
            {
                return (this.dataObject as COMDT_MAILACCESS_PROP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_MAILACCESS_RONGYU stRongYu
        {
            get
            {
                return (this.dataObject as COMDT_MAILACCESS_RONGYU);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

