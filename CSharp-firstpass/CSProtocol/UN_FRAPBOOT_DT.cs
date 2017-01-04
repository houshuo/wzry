namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class UN_FRAPBOOT_DT : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public sbyte chReserve;
        public static readonly int CLASS_ID = 0x40a;
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
                        if (!(this.dataObject is CSDT_FRAPBOOT_CC))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_FRAPBOOT_CC) ProtocolObjectPool.Get(CSDT_FRAPBOOT_CC.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is CSDT_FRAPBOOT_CS))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_FRAPBOOT_CS) ProtocolObjectPool.Get(CSDT_FRAPBOOT_CS.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is CSDT_FRAPBOOT_ACNTSTATE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_FRAPBOOT_ACNTSTATE) ProtocolObjectPool.Get(CSDT_FRAPBOOT_ACNTSTATE.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is CSDT_FRAPBOOT_ASSISTSTATE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_FRAPBOOT_ASSISTSTATE) ProtocolObjectPool.Get(CSDT_FRAPBOOT_ASSISTSTATE.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is CSDT_FRAPBOOT_AISTATE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_FRAPBOOT_AISTATE) ProtocolObjectPool.Get(CSDT_FRAPBOOT_AISTATE.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is CSDT_FRAPBOOT_GAMEOVERNTF))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_FRAPBOOT_GAMEOVERNTF) ProtocolObjectPool.Get(CSDT_FRAPBOOT_GAMEOVERNTF.CLASS_ID);
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

        public CSDT_FRAPBOOT_ACNTSTATE stAcntState
        {
            get
            {
                return (this.dataObject as CSDT_FRAPBOOT_ACNTSTATE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_FRAPBOOT_AISTATE stAiState
        {
            get
            {
                return (this.dataObject as CSDT_FRAPBOOT_AISTATE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_FRAPBOOT_ASSISTSTATE stAssistState
        {
            get
            {
                return (this.dataObject as CSDT_FRAPBOOT_ASSISTSTATE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_FRAPBOOT_CC stCCBoot
        {
            get
            {
                return (this.dataObject as CSDT_FRAPBOOT_CC);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_FRAPBOOT_CS stCSBoot
        {
            get
            {
                return (this.dataObject as CSDT_FRAPBOOT_CS);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_FRAPBOOT_GAMEOVERNTF stGameOverNtf
        {
            get
            {
                return (this.dataObject as CSDT_FRAPBOOT_GAMEOVERNTF);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

