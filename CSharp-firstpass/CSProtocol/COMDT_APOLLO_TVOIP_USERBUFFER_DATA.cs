namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_APOLLO_TVOIP_USERBUFFER_DATA : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bReverse;
        public static readonly int CLASS_ID = 0x1bc;
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
            if (selector <= 2L)
            {
                this.select_1_2(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_1_2(long selector)
        {
            switch (selector)
            {
                case 1L:
                    if (!(this.dataObject is COMDT_APOLLO_TVOIP_CREATE_ROOM))
                    {
                        if (this.dataObject != null)
                        {
                            this.dataObject.Release();
                        }
                        this.dataObject = (COMDT_APOLLO_TVOIP_CREATE_ROOM) ProtocolObjectPool.Get(COMDT_APOLLO_TVOIP_CREATE_ROOM.CLASS_ID);
                    }
                    break;

                case 2L:
                    if (!(this.dataObject is COMDT_APOLLO_TVOIP_JOIN_ROOM))
                    {
                        if (this.dataObject != null)
                        {
                            this.dataObject.Release();
                        }
                        this.dataObject = (COMDT_APOLLO_TVOIP_JOIN_ROOM) ProtocolObjectPool.Get(COMDT_APOLLO_TVOIP_JOIN_ROOM.CLASS_ID);
                    }
                    break;

                default:
                    if (this.dataObject != null)
                    {
                        this.dataObject.Release();
                        this.dataObject = null;
                    }
                    break;
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

        public COMDT_APOLLO_TVOIP_CREATE_ROOM stCreateRoom
        {
            get
            {
                return (this.dataObject as COMDT_APOLLO_TVOIP_CREATE_ROOM);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_APOLLO_TVOIP_JOIN_ROOM stJoinRoom
        {
            get
            {
                return (this.dataObject as COMDT_APOLLO_TVOIP_JOIN_ROOM);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

