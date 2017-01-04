namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_TRANSACTION_MSG_DETAIL : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bReserved;
        public static readonly int CLASS_ID = 0x191;
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
            this.bReserved = 0;
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
            this.bReserved = 0;
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
            type = destBuf.writeUInt8(this.bReserved);
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
            if (selector <= 0x13L)
            {
                this.select_1_19(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_1_19(long selector)
        {
            long num = selector;
            if ((num >= 1L) && (num <= 0x13L))
            {
                switch (((int) (num - 1L)))
                {
                    case 0:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_GETCLASSIDRSP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_GETCLASSIDRSP) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_GETCLASSIDRSP.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_GETGUILDNAMEREQ))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_GETGUILDNAMEREQ) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_GETGUILDNAMEREQ.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_GETGUILDNAMERSP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_GETGUILDNAMERSP) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_GETGUILDNAMERSP.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILREQ))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILREQ) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILREQ.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILRSP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILRSP) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILRSP.CLASS_ID);
                        }
                        return;

                    case 6:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPBANACNTREQ))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPBANACNTREQ) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPBANACNTREQ.CLASS_ID);
                        }
                        return;

                    case 7:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPBANACNTRSP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPBANACNTRSP) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPBANACNTRSP.CLASS_ID);
                        }
                        return;

                    case 8:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPBANTIMEREQ))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPBANTIMEREQ) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPBANTIMEREQ.CLASS_ID);
                        }
                        return;

                    case 9:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPBANTIMERSP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPBANTIMERSP) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPBANTIMERSP.CLASS_ID);
                        }
                        return;

                    case 10:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITREQ))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITREQ) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITREQ.CLASS_ID);
                        }
                        return;

                    case 11:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITRSP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITRSP) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITRSP.CLASS_ID);
                        }
                        return;

                    case 12:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINEREQ))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINEREQ) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINEREQ.CLASS_ID);
                        }
                        return;

                    case 13:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINERSP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINERSP) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINERSP.CLASS_ID);
                        }
                        return;

                    case 14:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ.CLASS_ID);
                        }
                        return;

                    case 15:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP.CLASS_ID);
                        }
                        return;

                    case 0x10:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ.CLASS_ID);
                        }
                        return;

                    case 0x11:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP.CLASS_ID);
                        }
                        return;

                    case 0x12:
                        if (!(this.dataObject is COMDT_TRANSACTION_MSG_DUPKICK))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_TRANSACTION_MSG_DUPKICK) ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_DUPKICK.CLASS_ID);
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
            type = srcBuf.readUInt8(ref this.bReserved);
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

        public COMDT_TRANSACTION_MSG_DUPKICK stDupKick
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_DUPKICK);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ stGetClassIdReq
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_GETCLASSIDRSP stGetClassIdRsp
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_GETCLASSIDRSP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_GETGUILDNAMEREQ stGetGuildNameReq
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_GETGUILDNAMEREQ);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_GETGUILDNAMERSP stGetGuildNameRsp
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_GETGUILDNAMERSP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIPBANACNTREQ stIdipBanAcntReq
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPBANACNTREQ);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIPBANACNTRSP stIdipBanAcntRsp
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPBANACNTRSP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIPBANTIMEREQ stIdipBanTimeReq
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPBANTIMEREQ);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIPBANTIMERSP stIdipBanTimeRsp
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPBANTIMERSP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINEREQ stIdipChgAcntOnlineInfoReq
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINEREQ);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINERSP stIdipChgAcntOnlineInfoRsp
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINERSP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ stIdipDelAcntPkgInfoReq
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP stIdipDelAcntPkgInfoRsp
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ stIdipQueryGlodRankInfoReq
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP stIdipQueryGoldRankInfoRsp
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILREQ stIdipSendMailReq
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILREQ);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILRSP stIdipSendMailRsp
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILRSP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITREQ stWorldRewardLimitReq
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITREQ);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITRSP stWorldRewardLimitRsp
        {
            get
            {
                return (this.dataObject as COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITRSP);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

