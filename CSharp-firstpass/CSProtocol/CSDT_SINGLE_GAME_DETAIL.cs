namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_SINGLE_GAME_DETAIL : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bReserved;
        public static readonly int CLASS_ID = 0x250;
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
            if (selector <= 8L)
            {
                this.select_0_8(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_0_8(long selector)
        {
            long num = selector;
            if ((num >= 0L) && (num <= 8L))
            {
                switch (((int) num))
                {
                    case 0:
                        if (!(this.dataObject is CSDT_SINGLE_GAME_OF_ADVENTURE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_SINGLE_GAME_OF_ADVENTURE) ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_ADVENTURE.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is CSDT_SINGLE_GAME_OF_COMBAT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_SINGLE_GAME_OF_COMBAT) ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_COMBAT.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is CSDT_SINGLE_GAME_OF_GUIDE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_SINGLE_GAME_OF_GUIDE) ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_GUIDE.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is CSDT_SINGLE_GAME_OF_ACTIVITY))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_SINGLE_GAME_OF_ACTIVITY) ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_ACTIVITY.CLASS_ID);
                        }
                        return;

                    case 7:
                        if (!(this.dataObject is CSDT_SINGLE_GAME_OF_BURNING))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_SINGLE_GAME_OF_BURNING) ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_BURNING.CLASS_ID);
                        }
                        return;

                    case 8:
                        if (!(this.dataObject is CSDT_SINGLE_GAME_OF_ARENA))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_SINGLE_GAME_OF_ARENA) ProtocolObjectPool.Get(CSDT_SINGLE_GAME_OF_ARENA.CLASS_ID);
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

        public CSDT_SINGLE_GAME_OF_ACTIVITY stGameOfActivity
        {
            get
            {
                return (this.dataObject as CSDT_SINGLE_GAME_OF_ACTIVITY);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_SINGLE_GAME_OF_ADVENTURE stGameOfAdventure
        {
            get
            {
                return (this.dataObject as CSDT_SINGLE_GAME_OF_ADVENTURE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_SINGLE_GAME_OF_ARENA stGameOfArena
        {
            get
            {
                return (this.dataObject as CSDT_SINGLE_GAME_OF_ARENA);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_SINGLE_GAME_OF_BURNING stGameOfBurning
        {
            get
            {
                return (this.dataObject as CSDT_SINGLE_GAME_OF_BURNING);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_SINGLE_GAME_OF_COMBAT stGameOfCombat
        {
            get
            {
                return (this.dataObject as CSDT_SINGLE_GAME_OF_COMBAT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_SINGLE_GAME_OF_GUIDE stGameOfGuide
        {
            get
            {
                return (this.dataObject as CSDT_SINGLE_GAME_OF_GUIDE);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

