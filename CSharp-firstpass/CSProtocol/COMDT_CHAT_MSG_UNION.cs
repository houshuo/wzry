namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_CHAT_MSG_UNION : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0xff;
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
                        if (!(this.dataObject is COMDT_CHAT_MSG_LOGIC_WORLD))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_CHAT_MSG_LOGIC_WORLD) ProtocolObjectPool.Get(COMDT_CHAT_MSG_LOGIC_WORLD.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is COMDT_CHAT_MSG_PRIVATE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_CHAT_MSG_PRIVATE) ProtocolObjectPool.Get(COMDT_CHAT_MSG_PRIVATE.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is COMDT_CHAT_MSG_ROOM))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_CHAT_MSG_ROOM) ProtocolObjectPool.Get(COMDT_CHAT_MSG_ROOM.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_CHAT_MSG_GUILD))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_CHAT_MSG_GUILD) ProtocolObjectPool.Get(COMDT_CHAT_MSG_GUILD.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is COMDT_CHAT_MSG_BATTLE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_CHAT_MSG_BATTLE) ProtocolObjectPool.Get(COMDT_CHAT_MSG_BATTLE.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is COMDT_CHAT_MSG_TEAM))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_CHAT_MSG_TEAM) ProtocolObjectPool.Get(COMDT_CHAT_MSG_TEAM.CLASS_ID);
                        }
                        return;

                    case 6:
                        if (!(this.dataObject is COMDT_CHAT_MSG_INBATTLE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_CHAT_MSG_INBATTLE) ProtocolObjectPool.Get(COMDT_CHAT_MSG_INBATTLE.CLASS_ID);
                        }
                        return;

                    case 7:
                        if (!(this.dataObject is COMDT_CHAT_MSG_SETTLE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_CHAT_MSG_SETTLE) ProtocolObjectPool.Get(COMDT_CHAT_MSG_SETTLE.CLASS_ID);
                        }
                        return;

                    case 8:
                        if (!(this.dataObject is COMDT_CHAT_MSG_HORN))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_CHAT_MSG_HORN) ProtocolObjectPool.Get(COMDT_CHAT_MSG_HORN.CLASS_ID);
                        }
                        return;

                    case 9:
                        if (!(this.dataObject is COMDT_CHAT_MSG_HORN))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_CHAT_MSG_HORN) ProtocolObjectPool.Get(COMDT_CHAT_MSG_HORN.CLASS_ID);
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

        public COMDT_CHAT_MSG_BATTLE stBattle
        {
            get
            {
                return (this.dataObject as COMDT_CHAT_MSG_BATTLE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_CHAT_MSG_HORN stBigHorn
        {
            get
            {
                return (this.dataObject as COMDT_CHAT_MSG_HORN);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_CHAT_MSG_GUILD stGuild
        {
            get
            {
                return (this.dataObject as COMDT_CHAT_MSG_GUILD);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_CHAT_MSG_INBATTLE stInBattle
        {
            get
            {
                return (this.dataObject as COMDT_CHAT_MSG_INBATTLE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_CHAT_MSG_LOGIC_WORLD stLogicWord
        {
            get
            {
                return (this.dataObject as COMDT_CHAT_MSG_LOGIC_WORLD);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_CHAT_MSG_PRIVATE stPrivate
        {
            get
            {
                return (this.dataObject as COMDT_CHAT_MSG_PRIVATE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_CHAT_MSG_ROOM stRoom
        {
            get
            {
                return (this.dataObject as COMDT_CHAT_MSG_ROOM);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_CHAT_MSG_SETTLE stSettle
        {
            get
            {
                return (this.dataObject as COMDT_CHAT_MSG_SETTLE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_CHAT_MSG_HORN stSmallHorn
        {
            get
            {
                return (this.dataObject as COMDT_CHAT_MSG_HORN);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_CHAT_MSG_TEAM stTeam
        {
            get
            {
                return (this.dataObject as COMDT_CHAT_MSG_TEAM);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

