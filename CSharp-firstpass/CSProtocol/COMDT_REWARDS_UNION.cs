namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_REWARDS_UNION : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bReverse;
        public static readonly int CLASS_ID = 190;
        public static readonly uint CURRVERSION = 1;
        public ProtocolObject dataObject;
        public uint dwAP;
        public uint dwArenaCoin;
        public uint dwBurningCoin;
        public uint dwCoin;
        public uint dwCoupons;
        public uint dwDiamond;
        public uint dwHeroPoolExp;
        public uint dwHuoYueDu;
        public uint dwMatchPointGuild;
        public uint dwMatchPointPer;
        public uint dwPvpCoin;
        public uint dwSkinCoin;
        public uint dwSymbolCoin;

        public TdrError.ErrorType construct(long selector)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.construct();
            }
            if (selector == 0)
            {
                this.dwCoin = 0;
                return type;
            }
            if (selector == 3L)
            {
                this.dwCoupons = 0;
                return type;
            }
            if (selector == 7L)
            {
                this.dwBurningCoin = 0;
                return type;
            }
            if (selector == 8L)
            {
                this.dwArenaCoin = 0;
                return type;
            }
            if (selector == 9L)
            {
                this.dwAP = 0;
                return type;
            }
            if (selector == 11L)
            {
                this.dwPvpCoin = 0;
                return type;
            }
            if (selector == 12L)
            {
                this.dwHeroPoolExp = 0;
                return type;
            }
            if (selector == 13L)
            {
                this.dwSkinCoin = 0;
                return type;
            }
            if (selector == 14L)
            {
                this.dwSymbolCoin = 0;
                return type;
            }
            if (selector == 0x10L)
            {
                this.dwDiamond = 0;
                return type;
            }
            if (selector == 0x11L)
            {
                this.dwHuoYueDu = 0;
                return type;
            }
            if (selector == 0x12L)
            {
                this.dwMatchPointPer = 0;
                return type;
            }
            if (selector == 0x13L)
            {
                this.dwMatchPointGuild = 0;
                return type;
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
            this.dwCoin = 0;
            this.dwCoupons = 0;
            this.dwBurningCoin = 0;
            this.dwArenaCoin = 0;
            this.dwAP = 0;
            this.dwPvpCoin = 0;
            this.dwHeroPoolExp = 0;
            this.dwSkinCoin = 0;
            this.dwSymbolCoin = 0;
            this.dwDiamond = 0;
            this.dwHuoYueDu = 0;
            this.dwMatchPointPer = 0;
            this.dwMatchPointGuild = 0;
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
            if (selector == 0)
            {
                type = destBuf.writeUInt32(this.dwCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 3L)
            {
                type = destBuf.writeUInt32(this.dwCoupons);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 7L)
            {
                type = destBuf.writeUInt32(this.dwBurningCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 8L)
            {
                type = destBuf.writeUInt32(this.dwArenaCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 9L)
            {
                type = destBuf.writeUInt32(this.dwAP);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 11L)
            {
                type = destBuf.writeUInt32(this.dwPvpCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 12L)
            {
                type = destBuf.writeUInt32(this.dwHeroPoolExp);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 13L)
            {
                type = destBuf.writeUInt32(this.dwSkinCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 14L)
            {
                type = destBuf.writeUInt32(this.dwSymbolCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 0x10L)
            {
                type = destBuf.writeUInt32(this.dwDiamond);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 0x11L)
            {
                type = destBuf.writeUInt32(this.dwHuoYueDu);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 0x12L)
            {
                type = destBuf.writeUInt32(this.dwMatchPointPer);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 0x13L)
            {
                type = destBuf.writeUInt32(this.dwMatchPointGuild);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
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
            if (selector <= 20L)
            {
                this.select_1_20(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_1_20(long selector)
        {
            long num = selector;
            if ((num >= 1L) && (num <= 10L))
            {
                switch (((int) (num - 1L)))
                {
                    case 0:
                        if (!(this.dataObject is COMDT_REWARD_ITEM))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_REWARD_ITEM) ProtocolObjectPool.Get(COMDT_REWARD_ITEM.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is COMDT_REWARD_EXP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_REWARD_EXP) ProtocolObjectPool.Get(COMDT_REWARD_EXP.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is COMDT_REWARD_EQUIP))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_REWARD_EQUIP) ProtocolObjectPool.Get(COMDT_REWARD_EQUIP.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is COMDT_REWARD_HERO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_REWARD_HERO) ProtocolObjectPool.Get(COMDT_REWARD_HERO.CLASS_ID);
                        }
                        return;

                    case 5:
                        if (!(this.dataObject is COMDT_REWARD_SYMBOL))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_REWARD_SYMBOL) ProtocolObjectPool.Get(COMDT_REWARD_SYMBOL.CLASS_ID);
                        }
                        return;

                    case 9:
                        if (!(this.dataObject is COMDT_REWARD_SKIN))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (COMDT_REWARD_SKIN) ProtocolObjectPool.Get(COMDT_REWARD_SKIN.CLASS_ID);
                        }
                        return;
                }
            }
            if (num == 20L)
            {
                if (!(this.dataObject is COMDT_REWARD_HEADIMG))
                {
                    if (this.dataObject != null)
                    {
                        this.dataObject.Release();
                    }
                    this.dataObject = (COMDT_REWARD_HEADIMG) ProtocolObjectPool.Get(COMDT_REWARD_HEADIMG.CLASS_ID);
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
            if (selector == 0)
            {
                type = srcBuf.readUInt32(ref this.dwCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 3L)
            {
                type = srcBuf.readUInt32(ref this.dwCoupons);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 7L)
            {
                type = srcBuf.readUInt32(ref this.dwBurningCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 8L)
            {
                type = srcBuf.readUInt32(ref this.dwArenaCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 9L)
            {
                type = srcBuf.readUInt32(ref this.dwAP);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 11L)
            {
                type = srcBuf.readUInt32(ref this.dwPvpCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 12L)
            {
                type = srcBuf.readUInt32(ref this.dwHeroPoolExp);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 13L)
            {
                type = srcBuf.readUInt32(ref this.dwSkinCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 14L)
            {
                type = srcBuf.readUInt32(ref this.dwSymbolCoin);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 0x10L)
            {
                type = srcBuf.readUInt32(ref this.dwDiamond);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 0x11L)
            {
                type = srcBuf.readUInt32(ref this.dwHuoYueDu);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 0x12L)
            {
                type = srcBuf.readUInt32(ref this.dwMatchPointPer);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
            }
            if (selector == 0x13L)
            {
                type = srcBuf.readUInt32(ref this.dwMatchPointGuild);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                return type;
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

        public COMDT_REWARD_EQUIP stEquip
        {
            get
            {
                return (this.dataObject as COMDT_REWARD_EQUIP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_REWARD_EXP stExp
        {
            get
            {
                return (this.dataObject as COMDT_REWARD_EXP);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_REWARD_HEADIMG stHeadImage
        {
            get
            {
                return (this.dataObject as COMDT_REWARD_HEADIMG);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_REWARD_HERO stHero
        {
            get
            {
                return (this.dataObject as COMDT_REWARD_HERO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_REWARD_ITEM stItem
        {
            get
            {
                return (this.dataObject as COMDT_REWARD_ITEM);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_REWARD_SKIN stSkin
        {
            get
            {
                return (this.dataObject as COMDT_REWARD_SKIN);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public COMDT_REWARD_SYMBOL stSymbol
        {
            get
            {
                return (this.dataObject as COMDT_REWARD_SYMBOL);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

