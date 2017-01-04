namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUseableManager
    {
        public static CUseable CreateCoinUseable(RES_SHOPBUY_COINTYPE coinType, int count)
        {
            enVirtualItemType enNull = enVirtualItemType.enNull;
            switch (coinType)
            {
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                    enNull = enVirtualItemType.enDianQuan;
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                    enNull = enVirtualItemType.enGoldCoin;
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                    enNull = enVirtualItemType.enHeart;
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                    enNull = enVirtualItemType.enArenaCoin;
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SKINCOIN:
                    enNull = enVirtualItemType.enSkinCoin;
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SYMBOLCOIN:
                    enNull = enVirtualItemType.enSymbolCoin;
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY:
                    enNull = enVirtualItemType.enDiamond;
                    break;

                default:
                    Debug.LogError("CoinType:" + coinType.ToString() + " is not supported!");
                    break;
            }
            return new CVirtualItem(enNull, count);
        }

        public static CUseable CreateExpUseable(COM_ITEM_TYPE useableType, ulong objID, uint expDays, uint baseID, int bCount = 0, int addTime = 0)
        {
            CUseable useable = null;
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
            {
                return new CExpHeroItem(0L, baseID, expDays, bCount, 0);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
            {
                useable = new CExpHeroSkin(0L, baseID, expDays, bCount, 0);
            }
            return useable;
        }

        public static CUseable CreateUsableByRandowReward(RES_RANDOM_REWARD_TYPE type, int cnt, uint baseId)
        {
            COM_REWARDS_TYPE com_rewards_type;
            RandomRewardTypeToComRewardType(type, out com_rewards_type);
            return CreateUsableByServerType(com_rewards_type, cnt, baseId);
        }

        public static CUseable CreateUsableByServerType(COM_REWARDS_TYPE type, int cnt, uint baseId)
        {
            CUseable useable = null;
            switch (type)
            {
                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN:
                    return CreateVirtualUseable(enVirtualItemType.enGoldCoin, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM:
                    return CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, 0L, baseId, cnt, 0);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_EXP:
                    return CreateVirtualUseable(enVirtualItemType.enExp, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_COUPONS:
                    return CreateVirtualUseable(enVirtualItemType.enDianQuan, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_EQUIP:
                    return CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, 0L, baseId, cnt, 0);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO:
                    return CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, baseId, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOL:
                    return CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, 0L, baseId, cnt, 0);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_BURNING_COIN:
                    return CreateVirtualUseable(enVirtualItemType.enBurningCoin, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_ARENA_COIN:
                    return CreateVirtualUseable(enVirtualItemType.enArenaCoin, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP:
                    return CreateVirtualUseable(enVirtualItemType.enHeart, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN:
                    return CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, 0L, baseId, cnt, 0);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_HONOUR:
                    return CreateVirtualUseable(enVirtualItemType.enGoldCoin, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEROPOOLEXP:
                    return CreateVirtualUseable(enVirtualItemType.enExpPool, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKINCOIN:
                    return CreateVirtualUseable(enVirtualItemType.enSkinCoin, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOLCOIN:
                    return CreateVirtualUseable(enVirtualItemType.enSymbolCoin, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_PVPEXP:
                    return useable;

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND:
                    return CreateVirtualUseable(enVirtualItemType.enDiamond, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_HUOYUEDU:
                    return CreateVirtualUseable(enVirtualItemType.enHuoyueDu, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_MATCH_POINT_PERSON:
                    return CreateVirtualUseable(enVirtualItemType.enMatchPersonalPoint, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_MATCH_POINT_GUILD:
                    return CreateVirtualUseable(enVirtualItemType.enMatchTeamPoint, cnt);

                case COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEADIMAGE:
                    return CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG, 0L, baseId, cnt, 0);
            }
            return useable;
        }

        public static CUseable CreateUsableByServerType(RES_REWARDS_TYPE type, int cnt, uint baseId)
        {
            COM_REWARDS_TYPE com_rewards_type;
            ResRewardTypeToComRewardType(type, out com_rewards_type);
            return CreateUsableByServerType(com_rewards_type, cnt, baseId);
        }

        public static ListView<CUseable> CreateUsableListByRandowReward(RES_RANDOM_REWARD_TYPE type, int cnt, uint baseId)
        {
            ListView<CUseable> view = new ListView<CUseable>();
            if (type != RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_NEST)
            {
                CUseable item = CreateUsableByRandowReward(type, cnt, baseId);
                if (item != null)
                {
                    view.Add(item);
                }
                return view;
            }
            ResRandomRewardStore dataByKey = null;
            dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(baseId);
            if (dataByKey != null)
            {
                for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
                {
                    if (dataByKey.astRewardDetail[i].bItemType == 0)
                    {
                        return view;
                    }
                    if (dataByKey.astRewardDetail[i].bItemType >= 0x11)
                    {
                        return view;
                    }
                    view.AddRange(CreateUsableListByRandowReward((RES_RANDOM_REWARD_TYPE) dataByKey.astRewardDetail[i].bItemType, (int) dataByKey.astRewardDetail[i].dwLowCnt, dataByKey.astRewardDetail[i].dwItemID));
                }
            }
            return view;
        }

        public static CUseable CreateUseable(COM_ITEM_TYPE useableType, uint baseID, int bCount = 0)
        {
            CUseable useable = null;
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
            {
                return new CItem(0L, baseID, bCount, 0);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)
            {
                return new CEquip(0L, baseID, bCount, 0);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
            {
                return new CHeroItem(0L, baseID, bCount, 0);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
            {
                return new CSymbolItem(0L, baseID, bCount, 0);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
            {
                return new CHeroSkin(0L, baseID, bCount, 0);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG)
            {
                useable = new CHeadImg(0L, baseID, 0);
            }
            return useable;
        }

        public static CUseable CreateUseable(COM_ITEM_TYPE useableType, ulong objID, uint baseID, int bCount = 0, int addTime = 0)
        {
            CUseable useable = null;
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
            {
                return new CItem(objID, baseID, bCount, addTime);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)
            {
                return new CEquip(objID, baseID, bCount, addTime);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
            {
                return new CHeroItem(objID, baseID, bCount, addTime);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
            {
                return new CSymbolItem(objID, baseID, bCount, addTime);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
            {
                return new CHeroSkin(objID, baseID, bCount, addTime);
            }
            if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG)
            {
                useable = new CHeadImg(objID, baseID, 0);
            }
            return useable;
        }

        public static CUseable CreateVirtualUseable(enVirtualItemType vType, int bCount)
        {
            return new CVirtualItem(vType, bCount);
        }

        public static CUseable GetUseableByRewardInfo(ResRandomRewardStore inRewardInfo)
        {
            if (inRewardInfo != null)
            {
                COM_REWARDS_TYPE com_rewards_type;
                RandomRewardTypeToComRewardType((RES_RANDOM_REWARD_TYPE) inRewardInfo.astRewardDetail[0].bItemType, out com_rewards_type);
                int dwLowCnt = (int) inRewardInfo.astRewardDetail[0].dwLowCnt;
                uint dwItemID = inRewardInfo.astRewardDetail[0].dwItemID;
                return CreateUsableByServerType(com_rewards_type, dwLowCnt, dwItemID);
            }
            return null;
        }

        public static ListView<CUseable> GetUseableListFromItemList(COMDT_REWARD_ITEMLIST itemList)
        {
            ListView<CUseable> view = new ListView<CUseable>();
            for (int i = 0; i < itemList.wRewardCnt; i++)
            {
                ushort wItemType = itemList.astRewardList[i].wItemType;
                ushort wItemCnt = itemList.astRewardList[i].wItemCnt;
                uint dwItemID = itemList.astRewardList[i].dwItemID;
                CUseable item = CreateUseable((COM_ITEM_TYPE) wItemType, 0L, dwItemID, wItemCnt, 0);
                if (item != null)
                {
                    switch (itemList.astRewardList[i].bFromType)
                    {
                        case 1:
                            item.ExtraFromType = itemList.astRewardList[i].bFromType;
                            item.ExtraFromData = (int) itemList.astRewardList[i].stFromInfo.stHeroInfo.dwHeroID;
                            break;

                        case 2:
                            item.ExtraFromType = itemList.astRewardList[i].bFromType;
                            item.ExtraFromData = (int) itemList.astRewardList[i].stFromInfo.stSkinInfo.dwSkinID;
                            break;
                    }
                    view.Add(item);
                }
            }
            return view;
        }

        public static ListView<CUseable> GetUseableListFromReward(COMDT_REWARD_DETAIL reward)
        {
            ListView<CUseable> view = new ListView<CUseable>();
            for (int i = 0; i < reward.bNum; i++)
            {
                CUseable useable;
                switch (reward.astRewardDetail[i].bType)
                {
                    case 0:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwCoin, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 1:
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.stItem.dwCnt, reward.astRewardDetail[i].stRewardInfo.stItem.dwItemID);
                        if (useable == null)
                        {
                            continue;
                        }
                        if (reward.astRewardDetail[i].bFromType != 1)
                        {
                            break;
                        }
                        useable.ExtraFromType = reward.astRewardDetail[i].bFromType;
                        useable.ExtraFromData = (int) reward.astRewardDetail[i].stFromInfo.stHeroInfo.dwHeroID;
                        goto Label_02AA;

                    case 2:
                    case 15:
                    {
                        continue;
                    }
                    case 3:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwCoupons, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 4:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.stEquip.dwCnt, reward.astRewardDetail[i].stRewardInfo.stEquip.dwEquipID);
                        view.Add(useable);
                        continue;
                    }
                    case 5:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.stHero.dwCnt, reward.astRewardDetail[i].stRewardInfo.stHero.dwHeroID);
                        view.Add(useable);
                        continue;
                    }
                    case 6:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.stSymbol.dwCnt, reward.astRewardDetail[i].stRewardInfo.stSymbol.dwSymbolID);
                        view.Add(useable);
                        continue;
                    }
                    case 7:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwBurningCoin, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 8:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwArenaCoin, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 9:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwAP, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 10:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.stSkin.dwCnt, reward.astRewardDetail[i].stRewardInfo.stSkin.dwSkinID);
                        view.Add(useable);
                        continue;
                    }
                    case 11:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwPvpCoin, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 12:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwHeroPoolExp, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 13:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwSkinCoin, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 14:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwSymbolCoin, 0);
                        if (useable != null)
                        {
                            view.Add(useable);
                        }
                        continue;
                    }
                    case 0x10:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwDiamond, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 0x11:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwHuoYueDu, 0);
                        if (useable != null)
                        {
                            view.Add(useable);
                        }
                        continue;
                    }
                    case 0x12:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwMatchPointPer, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 0x13:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, (int) reward.astRewardDetail[i].stRewardInfo.dwMatchPointGuild, 0);
                        view.Add(useable);
                        continue;
                    }
                    case 20:
                    {
                        useable = CreateUsableByServerType((COM_REWARDS_TYPE) reward.astRewardDetail[i].bType, 1, reward.astRewardDetail[i].stRewardInfo.stHeadImage.dwHeadImgID);
                        if (useable != null)
                        {
                            view.Add(useable);
                        }
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                if (reward.astRewardDetail[i].bFromType == 2)
                {
                    useable.ExtraFromType = reward.astRewardDetail[i].bFromType;
                    useable.ExtraFromData = (int) reward.astRewardDetail[i].stFromInfo.stSkinInfo.dwSkinID;
                }
            Label_02AA:
                view.Add(useable);
            }
            return view;
        }

        public static void RandomRewardTypeToComRewardType(RES_RANDOM_REWARD_TYPE rType, out COM_REWARDS_TYPE cType)
        {
            switch (rType)
            {
                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_ITEM:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_EQUIP:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_EQUIP;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_HERO:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_SYMBOL:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOL;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_AP:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_COIN:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_COUPONS:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_COUPONS;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_BURNINGCOIN:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_BURNING_COIN;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_ARENACOIN:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_ARENA_COIN;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_SKIN:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_SKINCOIN:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKINCOIN;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_HEROPOOLEXP:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEROPOOLEXP;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_SYMBOLCOIN:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOLCOIN;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_DIAMOND:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND;
                    return;

                case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_HEADIMG:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEADIMAGE;
                    return;
            }
            cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_MAX;
        }

        public static void ResRewardTypeToComRewardType(RES_REWARDS_TYPE rType, out COM_REWARDS_TYPE cType)
        {
            switch (rType)
            {
                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_ITEM:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_EXP:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_EXP;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_COUPONS:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_COUPONS;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_EQUIP:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_EQUIP;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_SYMBOL:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOL;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_ARENACOIN:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_ARENA_COIN;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_HONOUR:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HONOUR;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_SKINCOIN:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKINCOIN;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_HEROPOOLEXP:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEROPOOLEXP;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_SYMBOLCOIN:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOLCOIN;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_DIAMOND:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_HUOYUEDU:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HUOYUEDU;
                    return;

                case RES_REWARDS_TYPE.RES_REWARDS_TYPE_HEADIMAGE:
                    cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEADIMAGE;
                    return;
            }
            cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_MAX;
        }

        public static void ShowUseableItem(CUseable item)
        {
            if (item != null)
            {
                if ((item.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) || (item.MapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM))
                {
                    if (item.ExtraFromType == 1)
                    {
                        CUICommonSystem.ShowNewHeroOrSkin((uint) item.ExtraFromData, 0, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, (uint) item.m_stackCount, 0);
                    }
                    else if (item.ExtraFromType == 2)
                    {
                        int extraFromData = item.ExtraFromData;
                        CUICommonSystem.ShowNewHeroOrSkin(0, (uint) extraFromData, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority1, (uint) item.m_stackCount, 0);
                    }
                    else
                    {
                        CUseable[] items = new CUseable[] { item };
                        Singleton<CUIManager>.GetInstance().OpenAwardTip(items, Singleton<CTextManager>.GetInstance().GetText("gotAward"), true, enUIEventID.None, false, true, "Form_Award");
                    }
                }
                else if (item is CHeroSkin)
                {
                    CHeroSkin skin = item as CHeroSkin;
                    CUICommonSystem.ShowNewHeroOrSkin(skin.m_heroId, skin.m_skinId, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority1, 0, 0);
                }
                else if (item is CHeroItem)
                {
                    CUICommonSystem.ShowNewHeroOrSkin(item.m_baseID, 0, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, 0, 0);
                }
                else
                {
                    CUseable[] useableArray2 = new CUseable[] { item };
                    Singleton<CUIManager>.GetInstance().OpenAwardTip(useableArray2, Singleton<CTextManager>.GetInstance().GetText("gotAward"), true, enUIEventID.None, false, true, "Form_Award");
                }
            }
        }
    }
}

