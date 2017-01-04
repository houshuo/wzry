namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_MAILOPT_RESINFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x3a4;
        public static readonly uint CURRVERSION = 0x53;
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
                this.select_1_6(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_1_6(long selector)
        {
            long num = selector;
            if ((num >= 1L) && (num <= 6L))
            {
                switch (((int) (num - 1L)))
                {
                    case 0:
                        if (!(this.dataObject is CSDT_MAILOPTRES_GETMAILLIST))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_MAILOPTRES_GETMAILLIST) ProtocolObjectPool.Get(CSDT_MAILOPTRES_GETMAILLIST.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is CSDT_MAILOPTRES_SENDMAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_MAILOPTRES_SENDMAIL) ProtocolObjectPool.Get(CSDT_MAILOPTRES_SENDMAIL.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is CSDT_MAILOPTRES_READMAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_MAILOPTRES_READMAIL) ProtocolObjectPool.Get(CSDT_MAILOPTRES_READMAIL.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is CSDT_MAILOPTRES_DELMAIL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_MAILOPTRES_DELMAIL) ProtocolObjectPool.Get(CSDT_MAILOPTRES_DELMAIL.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is CSDT_MAILOPTRES_GETACCESS))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_MAILOPTRES_GETACCESS) ProtocolObjectPool.Get(CSDT_MAILOPTRES_GETACCESS.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is CSDT_MAILOPTRES_UNREADINFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_MAILOPTRES_UNREADINFO) ProtocolObjectPool.Get(CSDT_MAILOPTRES_UNREADINFO.CLASS_ID);
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

        public CSDT_MAILOPTRES_DELMAIL stDelMail
        {
            get
            {
                return (this.dataObject as CSDT_MAILOPTRES_DELMAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_MAILOPTRES_GETACCESS stGetAccess
        {
            get
            {
                return (this.dataObject as CSDT_MAILOPTRES_GETACCESS);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_MAILOPTRES_GETMAILLIST stGetMailList
        {
            get
            {
                return (this.dataObject as CSDT_MAILOPTRES_GETMAILLIST);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_MAILOPTRES_READMAIL stReadMail
        {
            get
            {
                return (this.dataObject as CSDT_MAILOPTRES_READMAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_MAILOPTRES_SENDMAIL stSendMail
        {
            get
            {
                return (this.dataObject as CSDT_MAILOPTRES_SENDMAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_MAILOPTRES_UNREADINFO stUnReadMail
        {
            get
            {
                return (this.dataObject as CSDT_MAILOPTRES_UNREADINFO);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

