namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_RANKING_LIST_ITEM_EXTRA_DETAIL : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public sbyte chReserved;
        public static readonly int CLASS_ID = 0x1d7;
        public static readonly uint CURRVERSION = 0x6b;
        public ProtocolObject dataObject;

        public TdrError.ErrorType construct(long selector)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.construct();
            }
            this.chReserved = 0;
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
            this.chReserved = 0;
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
            type = destBuf.writeInt8(this.chReserved);
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
            if (selector <= 0x41L)
            {
                this.select_1_65(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_1_65(long selector)
        {
            long num = selector;
            if ((num >= 1L) && (num <= 0x41L))
            {
                switch (((int) (num - 1L)))
                {
                    case 0:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 6:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 7:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 8:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 9:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 10:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 11:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 15:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 0x15:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP.CLASS_ID);
                        }
                        return;

                    case 0x20:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x21:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x22:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x23:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x24:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x25:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x26:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x27:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 40:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x29:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x2a:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x2b:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x2c:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x2d:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x2e:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x2f:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x30:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x31:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 50:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 0x33:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 0x34:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 0x35:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 0x36:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 0x37:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 0x38:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 0x39:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 0x3a:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT.CLASS_ID);
                        }
                        return;

                    case 0x3b:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 60:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x3d:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x3e:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x3f:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
                        }
                        return;

                    case 0x40:
                        if (!(this.dataObject is COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO.CLASS_ID);
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
            type = srcBuf.readInt8(ref this.chReserved);
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

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stAchievement
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stContinousWin
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP stCustomEquip
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_CUSTOM_EQUIP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stDailyRankMatch
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER stGuildPower
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_POWER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stGuildRankPoint
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stGuildSeason
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHeroNum
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighCoinDay
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stHighCoinGuild
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighCoinSeason
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighCoinWin
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighCoupDay
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stHighCoupGuild
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighCoupSeason
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighDiamDay
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stHighDiamGuild
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighDiamondWin
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stHighDiamSeason
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLadderPoint
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowCoinDay
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stLowCoinGuild
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowCoinSeason
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowCoinWin
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowCoupDay
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stLowCoupGuild
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowCoupSeason
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowDiamDay
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stLowDiamGuild
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowDiamondWin
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stLowDiamSeason
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO stMasterHero
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidCoinDay
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stMidCoinGuild
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidCoinSeason
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidCoupDay
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stMidCoupGuild
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidCoupSeason
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidDiamDay
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT stMidDiamGuild
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_GUILD_RANK_POINT);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stMidDiamSeason
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stPower
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stPvpExp
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stSkinNum
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stUseCoupons
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stVipScore
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stWinGameNum
        {
            get
            {
                return (this.dataObject as COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

