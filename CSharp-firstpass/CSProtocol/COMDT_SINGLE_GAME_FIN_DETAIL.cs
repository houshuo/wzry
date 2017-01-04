namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_SINGLE_GAME_FIN_DETAIL : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x9c;
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
            if (selector <= 7L)
            {
                this.select_0_7(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_0_7(long selector)
        {
            long num = selector;
            if ((num >= 0L) && (num <= 3L))
            {
                switch (((int) num))
                {
                    case 0:
                        if (!(this.dataObject is COMDT_SINGLE_GAME_PARAM_ADVENTURE))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_SINGLE_GAME_PARAM_ADVENTURE) ProtocolObjectPool.Get(COMDT_SINGLE_GAME_PARAM_ADVENTURE.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_ACTIVITY_COMMON))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_ACTIVITY_COMMON) ProtocolObjectPool.Get(COMDT_ACTIVITY_COMMON.CLASS_ID);
                        }
                        return;
                }
            }
            if (num == 7L)
            {
                if (!(this.dataObject is COMDT_BURNING_ENEMY_HERO_DETAIL))
                {
                    if (this.dataObject != null)
                    {
                        this.dataObject.Release();
                    }
                    this.dataObject = (COMDT_BURNING_ENEMY_HERO_DETAIL) ProtocolObjectPool.Get(COMDT_BURNING_ENEMY_HERO_DETAIL.CLASS_ID);
                }
            }
            else if (this.dataObject != null)
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

        public COMDT_ACTIVITY_COMMON stActivity
        {
            get
            {
                return (this.dataObject as COMDT_ACTIVITY_COMMON);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_SINGLE_GAME_PARAM_ADVENTURE stAdventure
        {
            get
            {
                return (this.dataObject as COMDT_SINGLE_GAME_PARAM_ADVENTURE);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_BURNING_ENEMY_HERO_DETAIL stBurning
        {
            get
            {
                return (this.dataObject as COMDT_BURNING_ENEMY_HERO_DETAIL);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

