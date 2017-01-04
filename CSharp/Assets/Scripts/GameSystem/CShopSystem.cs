namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.Sound;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CShopSystem : Singleton<CShopSystem>
    {
        public CUseable[] aSaleItems;
        public const ushort AUTO_REFRESH_WAITING_TIME = 0x5dc;
        public const byte ITEMS_PERPAGE = 6;
        private byte m_CurPage;
        public int m_currentSelectItemIdx = -1;
        public RES_SHOP_TYPE m_CurShopType;
        private Tab m_CurTab = Tab.Fix_Shop;
        public bool m_isMysteryShopOpenedOnce = false;
        public bool m_isMysteryShopRefused = false;
        private bool m_IsNormalShopItemsInited = false;
        public bool m_IsShopFormOpen = false;
        public RES_SHOP_TYPE m_LastNormalShop;
        public ushort m_saleItemsCnt;
        public CUIFormScript m_ShopForm;
        public DictionaryView<uint, stShopItemInfo[]> m_ShopItems;
        public Dictionary<uint, stShopInfo> m_Shops;
        private int m_TimerSeq;
        public int MAX_SALE_ITEM_CNT = 2;
        public static uint s_CoinShowMaxValue = 0xf1b30;
        public static uint s_CoinShowStepValue = 0x2710;
        public string sMysteryShopFormPath = "UGUI/Form/System/Shop/Form_Mystery_Shop.prefab";
        public string sShopBuyFormPath = "UGUI/Form/System/Shop/Form_Shop_Buy_Item.prefab";
        public string sShopFormPath = "UGUI/Form/System/Shop/Form_Shop.prefab";

        private int EnoughMoneyToRefresh(ref string msg)
        {
            stShopInfo info = new stShopInfo();
            if (!this.m_Shops.TryGetValue((uint) this.m_CurShopType, out info))
            {
                msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Manual_Refresh_Forbdden");
                return -7;
            }
            ResShopRefreshCost cost = this.GetCost(info.enShopType, info.dwManualRefreshCnt);
            if (cost != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                switch (cost.bCostType)
                {
                    case 2:
                        if (masterRoleInfo.DianQuan >= cost.dwCostPrice)
                        {
                            break;
                        }
                        msg = Singleton<CTextManager>.GetInstance().GetText("Common_DianQuan_Not_Enough");
                        return -1;

                    case 4:
                        if (masterRoleInfo.GoldCoin >= cost.dwCostPrice)
                        {
                            break;
                        }
                        msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_GoldCoin_Not_Enough2");
                        return -3;

                    case 5:
                        if (masterRoleInfo.BurningCoin >= cost.dwCostPrice)
                        {
                            break;
                        }
                        msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Burning_Not_Enough2");
                        return -4;

                    case 6:
                        if (masterRoleInfo.ArenaCoin >= cost.dwCostPrice)
                        {
                            break;
                        }
                        msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Arena_Not_Enough2");
                        return -5;

                    case 8:
                        if (CGuildHelper.GetPlayerGuildConstruct() >= cost.dwCostPrice)
                        {
                            break;
                        }
                        msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Guild_Not_Enough");
                        return -6;
                }
            }
            return 0;
        }

        public string GetCoinString(uint coinValue)
        {
            string str = coinValue.ToString();
            if (coinValue > s_CoinShowMaxValue)
            {
                int num = (int) (coinValue / s_CoinShowStepValue);
                str = string.Format("{0}万", num);
            }
            return str;
        }

        private ResShopRefreshCost GetCost(RES_SHOP_TYPE shopType, int refreshCnt)
        {
            <GetCost>c__AnonStorey89 storey = new <GetCost>c__AnonStorey89 {
                shopType = shopType,
                refreshCnt = refreshCnt
            };
            return GameDataMgr.shopRefreshCostDatabin.FindIf(new Func<ResShopRefreshCost, bool>(storey.<>m__8F));
        }

        private int GetManualRefreshedCnt()
        {
            stShopInfo info = new stShopInfo();
            if ((this.m_Shops != null) && this.m_Shops.TryGetValue((uint) this.m_CurShopType, out info))
            {
                return GetManualRefreshedCnt(info);
            }
            return 0;
        }

        private static int GetManualRefreshedCnt(stShopInfo shopInfo)
        {
            return (shopInfo.dwManualRefreshCnt - 1);
        }

        private static int GetManualRefreshMaxCnt(RES_SHOP_TYPE shopType)
        {
            int wRefreshFreq = 0;
            for (int i = 0; i < GameDataMgr.shopRefreshCostDatabin.count; i++)
            {
                if ((GameDataMgr.shopRefreshCostDatabin.GetDataByIndex(i).wShopType == ((ushort) shopType)) && (GameDataMgr.shopRefreshCostDatabin.GetDataByIndex(i).wRefreshFreq > wRefreshFreq))
                {
                    wRefreshFreq = GameDataMgr.shopRefreshCostDatabin.GetDataByIndex(i).wRefreshFreq;
                }
            }
            return wRefreshFreq;
        }

        private DateTime GetNextAutoRefreshTime(RES_SHOP_TYPE shopType)
        {
            DateTime time2 = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double) CRoleInfo.GetCurrentUTCTime());
            DateTime time3 = new DateTime(time2.Year, time2.Month, time2.Day, 0, 0, 0, DateTimeKind.Utc);
            DateTime time4 = new DateTime();
            DateTime time5 = new DateTime();
            int[] refreshTime = GameDataMgr.shopTypeDatabin.GetDataByKey((uint) ((ushort) shopType)).RefreshTime;
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < refreshTime.Length; i++)
            {
                if ((refreshTime[i] == 0) && (i == 0))
                {
                    return time4;
                }
                if (refreshTime[i] == 0)
                {
                    return time5.AddSeconds(115200.0);
                }
                num = refreshTime[i] / 100;
                num2 = refreshTime[i] % 100;
                time4 = time3.AddSeconds(((num * 0xe10) + (num2 * 60)) - 28800.0);
                if (time5 == DateTime.MinValue)
                {
                    time5 = time4;
                }
                if (DateTime.Compare(time2, time4) < 0)
                {
                    return time4.AddSeconds(28800.0);
                }
                if (i == (refreshTime.Length - 1))
                {
                    return time5.AddSeconds(115200.0);
                }
            }
            return time4;
        }

        private string GetShopCharacterTip(string tipKeyPrefix)
        {
            string[] strArray = new string[10];
            for (byte i = 1; i <= strArray.Length; i = (byte) (i + 1))
            {
                strArray[i - 1] = Singleton<CTextManager>.GetInstance().GetText(tipKeyPrefix + i);
            }
            int index = new System.Random().Next(0, 10);
            return strArray[index];
        }

        private void GetShopItemPrice(ref stShopItemInfo info)
        {
            uint buyPrice = 0;
            CUseable useable = CUseableManager.CreateUseable(info.enItemType, info.dwItemId, info.wItemCnt);
            if (useable != null)
            {
                buyPrice = useable.GetBuyPrice(info.enCostType);
            }
            float num2 = 0f;
            if (info.wSaleDiscount != 100)
            {
                num2 = ((info.wItemCnt * buyPrice) * info.wSaleDiscount) / 100;
            }
            else
            {
                num2 = info.wItemCnt * buyPrice;
            }
            info.fPrice = num2;
        }

        private int GetSubTabSelectedIndex()
        {
            if (this.m_ShopForm != null)
            {
                return this.m_ShopForm.GetWidget(5).GetComponent<CUIListScript>().GetSelectedIndex();
            }
            return 0;
        }

        public override void Init()
        {
            base.Init();
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.AddUIEventListener(enUIEventID.Shop_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenForm));
            instance.AddUIEventListener(enUIEventID.Shop_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnShop_CloseForm));
            instance.AddUIEventListener(enUIEventID.Shop_SelectTab, new CUIEventManager.OnUIEventHandler(this.OnShop_Tab_Change));
            instance.AddUIEventListener(enUIEventID.Shop_SelectSubTab, new CUIEventManager.OnUIEventHandler(this.OnShop_SubTab_Change));
            instance.AddUIEventListener(enUIEventID.Shop_SelectItem, new CUIEventManager.OnUIEventHandler(this.OnShop_SelectItem));
            instance.AddUIEventListener(enUIEventID.Shop_CloseItemForm, new CUIEventManager.OnUIEventHandler(this.OnShop_CloseItemBuyForm));
            instance.AddUIEventListener(enUIEventID.Shop_BuyItem, new CUIEventManager.OnUIEventHandler(this.OnShop_BuyItem));
            instance.AddUIEventListener(enUIEventID.Shop_ManualRefresh, new CUIEventManager.OnUIEventHandler(this.OnShop_ManualRefresh));
            instance.AddUIEventListener(enUIEventID.Shop_ConfirmManualRefresh, new CUIEventManager.OnUIEventHandler(this.OnShop_ConfirmManualRefresh));
            instance.AddUIEventListener(enUIEventID.Shop_SaleTipCancel, new CUIEventManager.OnUIEventHandler(this.OnShop_SaleTipCancel));
            instance.AddUIEventListener(enUIEventID.Shop_SaleTipConfirm, new CUIEventManager.OnUIEventHandler(this.OnShop_SaleTipConfirm));
            instance.AddUIEventListener(enUIEventID.Shop_AutoRefreshTimerTimeUp, new CUIEventManager.OnUIEventHandler(this.OnShop_AutoRefreshTimerTimeUp));
            instance.AddUIEventListener(enUIEventID.Shop_ReturnToShopForm, new CUIEventManager.OnUIEventHandler(this.OnShop_ReturnToShopForm));
            instance.AddUIEventListener(enUIEventID.Shop_OpenGuildShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenGuildShopForm));
            instance.AddUIEventListener(enUIEventID.Shop_GetBurningCoin, new CUIEventManager.OnUIEventHandler(this.OnShop_GetBurningCoin));
            instance.AddUIEventListener(enUIEventID.Shop_GetBurningCoinConfirm, new CUIEventManager.OnUIEventHandler(this.OnShop_GetBurningCoinConfirm));
            instance.AddUIEventListener(enUIEventID.Shop_GetArenaCoin, new CUIEventManager.OnUIEventHandler(this.OnShop_GetArenaCoin));
            instance.AddUIEventListener(enUIEventID.Shop_GetArenaCoinConfirm, new CUIEventManager.OnUIEventHandler(this.OnShop_GetArenaCoinConfirm));
            instance.AddUIEventListener(enUIEventID.Shop_Page_Up, new CUIEventManager.OnUIEventHandler(this.OnShop_PageUp));
            instance.AddUIEventListener(enUIEventID.Shop_Page_Down, new CUIEventManager.OnUIEventHandler(this.OnShop_PageDown));
            instance.AddUIEventListener(enUIEventID.Shop_OpenArenaShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenArenaShopForm));
            instance.AddUIEventListener(enUIEventID.Shop_OpenBurningShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenBurningShopForm));
            instance.AddUIEventListener(enUIEventID.Shop_BuyItem_Confirm, new CUIEventManager.OnUIEventHandler(this.OnShop_BuyItemConfirm));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Shop_Auto_Refresh_Shop_Items, new System.Action(this.OnRequestAutoRefreshShopItems));
            Singleton<EventRouter>.GetInstance().AddEventHandler<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, new Action<RES_SHOP_TYPE>(this.OnReceiveShopItems));
            Singleton<EventRouter>.instance.AddEventHandler("MasterAttributesChanged", new System.Action(this.MasterAttrChanged));
            this.m_Shops = new Dictionary<uint, stShopInfo>();
            this.m_ShopItems = new DictionaryView<uint, stShopItemInfo[]>();
            this.m_TimerSeq = 0;
        }

        private void InitSaleTip()
        {
            GameObject gameObject = this.m_ShopForm.transform.Find("pnlTipContainer").gameObject;
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            int curUseableCount = useableContainer.GetCurUseableCount();
            if (curUseableCount <= 0)
            {
                gameObject.CustomSetActive(false);
            }
            else
            {
                CUseable useableByIndex = null;
                this.aSaleItems = new CUseable[this.MAX_SALE_ITEM_CNT];
                this.m_saleItemsCnt = 0;
                for (int i = 0; i < curUseableCount; i++)
                {
                    useableByIndex = useableContainer.GetUseableByIndex(i);
                    if (this.m_saleItemsCnt == this.MAX_SALE_ITEM_CNT)
                    {
                        break;
                    }
                    switch (useableByIndex.m_type)
                    {
                        case COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP:
                        {
                            CItem item = (CItem) useableByIndex;
                            if (item.m_itemData.bIsAutoSale == 1)
                            {
                                this.aSaleItems[this.m_saleItemsCnt] = useableByIndex;
                                this.m_saleItemsCnt = (ushort) (this.m_saleItemsCnt + 1);
                            }
                            break;
                        }
                        case COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP:
                        {
                            CEquip equip = (CEquip) useableByIndex;
                            if (equip.m_equipData.bIsAutoSale == 1)
                            {
                                this.aSaleItems[this.m_saleItemsCnt] = useableByIndex;
                                this.m_saleItemsCnt = (ushort) (this.m_saleItemsCnt + 1);
                            }
                            break;
                        }
                        case COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL:
                        {
                            CSymbolItem item2 = (CSymbolItem) useableByIndex;
                            if (item2.m_SymbolData.bIsAutoSale == 1)
                            {
                                this.aSaleItems[this.m_saleItemsCnt] = useableByIndex;
                                this.m_saleItemsCnt = (ushort) (this.m_saleItemsCnt + 1);
                            }
                            break;
                        }
                    }
                }
                if (this.m_saleItemsCnt > 0)
                {
                    GameObject obj3 = gameObject.transform.Find("pnlTip/pnlItems/List").gameObject;
                    Text component = gameObject.transform.Find("pnlTip/pnlContainer/pnlPrice/txtCnt").GetComponent<Text>();
                    CUIListScript script = obj3.GetComponent<CUIListScript>();
                    script.SetElementAmount(this.m_saleItemsCnt);
                    CUIListElementScript listElementScript = null;
                    uint num3 = 0;
                    for (byte j = 0; j < this.m_saleItemsCnt; j = (byte) (j + 1))
                    {
                        stShopItemInfo info = new stShopItemInfo();
                        useableByIndex = this.aSaleItems[j];
                        switch (useableByIndex.m_type)
                        {
                            case COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP:
                            {
                                CItem item3 = (CItem) useableByIndex;
                                info.sName = item3.m_name;
                                info.dwItemId = item3.m_baseID;
                                info.wItemCnt = (ushort) item3.m_stackCount;
                                info.enItemType = COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP;
                                info.enCostType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
                                info.wSaleDiscount = 100;
                                info.isSoldOut = false;
                                break;
                            }
                            case COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP:
                            {
                                CEquip equip2 = (CEquip) useableByIndex;
                                info.sName = equip2.m_name;
                                info.dwItemId = equip2.m_baseID;
                                info.wItemCnt = (ushort) equip2.m_stackCount;
                                info.enItemType = COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP;
                                info.enCostType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
                                info.wSaleDiscount = 100;
                                info.isSoldOut = false;
                                break;
                            }
                            case COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL:
                            {
                                CSymbolItem item4 = (CSymbolItem) useableByIndex;
                                info.sName = item4.m_name;
                                info.dwItemId = item4.m_baseID;
                                info.wItemCnt = (ushort) item4.m_stackCount;
                                info.enItemType = COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL;
                                info.enCostType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
                                info.wSaleDiscount = 100;
                                info.isSoldOut = false;
                                break;
                            }
                        }
                        listElementScript = script.GetElemenet(j);
                        this.SetSaleItemInfo(listElementScript, ref info);
                        num3 += (uint) info.fPrice;
                    }
                    component.text = num3.ToString();
                    gameObject.CustomSetActive(true);
                }
                else
                {
                    gameObject.CustomSetActive(false);
                }
            }
        }

        public void InitShop()
        {
            this.m_CurPage = 0;
            if (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY)
            {
                Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Shop_Auto_Refresh_Shop_Items);
            }
            else
            {
                switch (this.CurTab)
                {
                    case Tab.Fix_Shop:
                    case Tab.Pvp_Symbol_Shop:
                    case Tab.Burning_Exp_Shop:
                    case Tab.Arena_Shop:
                    case Tab.Guild_Shop:
                        Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                        Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Shop_Auto_Refresh_Shop_Items);
                        break;
                }
            }
        }

        public void InitSubTab()
        {
            GameObject widget = this.m_ShopForm.GetWidget(5);
            string[] strArray = new string[] { Singleton<CTextManager>.GetInstance().GetText("Guild_Shop_Tab_Item"), Singleton<CTextManager>.GetInstance().GetText("Guild_Shop_Tab_HeadFrame") };
            CUIListScript component = widget.GetComponent<CUIListScript>();
            component.SetElementAmount(strArray.Length);
            for (int i = 0; i < strArray.Length; i++)
            {
                component.GetElemenet(i).transform.Find("Text").GetComponent<Text>().text = strArray[i];
            }
        }

        public void InitTab()
        {
            if ((this.m_ShopForm != null) && this.m_IsShopFormOpen)
            {
                Tab[] values = (Tab[]) Enum.GetValues(typeof(Tab));
                List<string> list = new List<string>();
                List<int> list2 = new List<int>();
                for (byte i = 0; i < values.Length; i = (byte) (i + 1))
                {
                    switch (values[i])
                    {
                        case Tab.Fix_Shop:
                            if (this.IsShopAvailable(RES_SHOP_TYPE.RES_SHOPTYPE_FIXED))
                            {
                                list.Add(Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Fixed"));
                                list2.Add(0);
                            }
                            break;

                        case Tab.Pvp_Symbol_Shop:
                            if (this.IsShopAvailable(RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL))
                            {
                                list.Add(Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Pvp_Symbol"));
                                list2.Add(1);
                            }
                            break;

                        case Tab.Burning_Exp_Shop:
                            if (this.IsShopAvailable(RES_SHOP_TYPE.RES_SHOPTYPE_BURNING))
                            {
                                list.Add(Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Burning_Exp"));
                                list2.Add(2);
                            }
                            break;

                        case Tab.Arena_Shop:
                            if (this.IsShopAvailable(RES_SHOP_TYPE.RES_SHOPTYPE_ARENA))
                            {
                                list.Add(Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Arena"));
                                list2.Add(3);
                            }
                            break;

                        case Tab.Guild_Shop:
                            if (this.IsShopAvailable(RES_SHOP_TYPE.RES_SHOPTYPE_GUILD))
                            {
                                list.Add(Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Guild"));
                                list2.Add(4);
                            }
                            break;
                    }
                }
                CUIListScript component = this.m_ShopForm.GetWidget(2).GetComponent<CUIListScript>();
                if (component != null)
                {
                    CUIListElementScript elemenet = null;
                    component.SetElementAmount(list.Count);
                    int index = 0;
                    for (int j = 0; j < component.m_elementAmount; j++)
                    {
                        elemenet = component.GetElemenet(j);
                        elemenet.gameObject.transform.Find("txtTab").GetComponent<Text>().text = list[j];
                        elemenet.gameObject.transform.Find("txtShopTypeData").GetComponent<Text>().text = list2[j].ToString();
                        if (this.CurTab == list2[j])
                        {
                            index = j;
                        }
                    }
                    component.m_alwaysDispatchSelectedChangeEvent = true;
                    component.SelectElement(index, true);
                }
            }
        }

        public void InitTopBar()
        {
            if (this.m_ShopForm != null)
            {
                GameObject widget = this.m_ShopForm.GetWidget(3);
                if (widget != null)
                {
                    widget.CustomSetActive(false);
                }
            }
        }

        public bool IsMysteryShopAvailable()
        {
            return false;
        }

        public bool IsNormalShopItemsInited()
        {
            return this.m_IsNormalShopItemsInited;
        }

        private bool IsOnlyAndOwned(ref stShopItemInfo info)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return false;
            }
            bool flag = false;
            switch (info.enItemType)
            {
                case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                    return masterRoleInfo.IsHaveHero(info.dwItemId, false);

                case COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL:
                case COM_ITEM_TYPE.COM_OBJTYPE_ITEMGEAR:
                    return flag;

                case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                    return masterRoleInfo.IsHaveHeroSkin(info.dwItemId, false);

                case COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG:
                    return (Singleton<HeadIconSys>.GetInstance().GetInfo(info.dwItemId) != null);
            }
            return flag;
        }

        private bool IsShopAvailable(RES_SHOP_TYPE shopType)
        {
            bool flag = this.IsShopOpen(shopType);
            if ((shopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD) || (shopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE))
            {
                bool flag2 = Singleton<CGuildSystem>.GetInstance().IsInNormalGuild();
                return (flag && flag2);
            }
            return flag;
        }

        private bool IsShopOpen(RES_SHOP_TYPE shopType)
        {
            ResShopType dataByKey = GameDataMgr.shopTypeDatabin.GetDataByKey((uint) ((ushort) shopType));
            if (dataByKey == null)
            {
                object[] inParameters = new object[] { shopType };
                DebugHelper.Assert(false, "CShopSystem.IsShopOpen(): resShopType is null, shopType={0}", inParameters);
                return false;
            }
            return (dataByKey.bIsOpen == 1);
        }

        private bool IsSlotLocked(int slotOffset)
        {
            return (((this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD) && (slotOffset >= CGuildHelper.GetGuildItemShopOpenSlotCount())) || ((this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE) && (slotOffset >= CGuildHelper.GetGuildHeadImageShopOpenSlotCount())));
        }

        private bool ManualRefreshLimit(ref string msg)
        {
            stShopInfo info = new stShopInfo();
            if (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY)
            {
                msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Manual_Refresh_Forbdden");
                return true;
            }
            if (!this.m_Shops.TryGetValue((uint) this.m_CurShopType, out info))
            {
                msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Manual_Refresh_Forbdden");
                return true;
            }
            if (info.dwManualRefreshLimit < info.dwManualRefreshCnt)
            {
                msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Manual_Refresh_Limit");
                return true;
            }
            if (info.bManualRefreshSent)
            {
                msg = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Manual_Refresh_Req_Sent");
                return true;
            }
            return false;
        }

        private void MasterAttrChanged()
        {
            if (this.m_IsShopFormOpen)
            {
                this.RefreshShop(this.m_CurShopType);
            }
        }

        private bool NeedAutoRefreshShop(RES_SHOP_TYPE shopType)
        {
            ResShopType dataByKey = GameDataMgr.shopTypeDatabin.GetDataByKey((uint) ((ushort) shopType));
            if (dataByKey != null)
            {
                if ((this.m_Shops.ContainsKey((uint) shopType) && this.m_ShopItems.ContainsKey((uint) shopType)) && (this.m_ShopItems[(uint) shopType].Length == 0))
                {
                    return true;
                }
                stShopInfo info = new stShopInfo();
                bool flag = false;
                flag = this.m_Shops.TryGetValue((uint) shopType, out info);
                if (info == null)
                {
                    return false;
                }
                DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                DateTime time2 = time.AddSeconds((double) CRoleInfo.GetCurrentUTCTime());
                DateTime time3 = time.AddSeconds((double) info.dwAutoRefreshTime);
                DateTime time4 = new DateTime();
                int[] refreshTime = dataByKey.RefreshTime;
                int num = 0;
                int num2 = 0;
                for (int i = 0; i < refreshTime.Length; i++)
                {
                    if (refreshTime[i] == 0)
                    {
                        return false;
                    }
                    DateTime time5 = new DateTime(time3.Year, time3.Month, time3.Day, 0, 0, 0, DateTimeKind.Utc);
                    num = refreshTime[i] / 100;
                    num2 = refreshTime[i] % 100;
                    time4 = time5.AddSeconds(((num * 0xe10) + (num2 * 60)) - 28800.0);
                    while (true)
                    {
                        if (DateTime.Compare(time4, time2) > 0)
                        {
                            break;
                        }
                        if ((DateTime.Compare(time3, time4) < 0) && (DateTime.Compare(time4, time2) < 0))
                        {
                            return true;
                        }
                        time4 = time4.AddDays(1.0);
                    }
                }
                if (flag)
                {
                    return (info.dwMaxRefreshTime == 0);
                }
            }
            return false;
        }

        private void OnReceiveShopItems(RES_SHOP_TYPE shopType)
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_TimerSeq);
            if (shopType == RES_SHOP_TYPE.RES_SHOPTYPE_FIXED)
            {
                this.m_IsNormalShopItemsInited = true;
            }
            this.RefreshShop(shopType);
        }

        private void OnRequestAutoRefreshShopItems()
        {
            if (!this.NeedAutoRefreshShop(this.m_CurShopType))
            {
                Singleton<EventRouter>.GetInstance().BroadCastEvent<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, this.m_CurShopType);
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            }
            else
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x45e);
                CSPKG_CMD_AUTOREFRESH cspkg_cmd_autorefresh = new CSPKG_CMD_AUTOREFRESH {
                    wShopType = (ushort) this.m_CurShopType
                };
                msg.stPkgData.stAutoRefresh = cspkg_cmd_autorefresh;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                this.m_TimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(0x5dc, 1, new CTimer.OnTimeUpHandler(this.UseOldShopItems));
            }
        }

        private void OnShop_AutoRefreshTimerTimeUp(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.Shop_Auto_Refresh_Shop_Items);
        }

        private void OnShop_BuyItem(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            stShopItemInfo info2 = Singleton<CShopSystem>.GetInstance().m_ShopItems[(uint) this.m_CurShopType][this.m_currentSelectItemIdx];
            switch (info2.enCostType)
            {
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                    if (masterRoleInfo.DianQuan >= info2.fPrice)
                    {
                        break;
                    }
                    CUICommonSystem.OpenDianQuanNotEnoughTip();
                    return;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                    if (masterRoleInfo.GoldCoin >= info2.fPrice)
                    {
                        break;
                    }
                    CUICommonSystem.OpenGoldCoinNotEnoughTip();
                    return;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                    if (masterRoleInfo.BurningCoin >= info2.fPrice)
                    {
                        break;
                    }
                    CUICommonSystem.OpenBurningCoinNotEnoughTip();
                    return;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                    if (masterRoleInfo.ArenaCoin >= info2.fPrice)
                    {
                        break;
                    }
                    CUICommonSystem.OpenArenaCoinNotEnoughTip();
                    return;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
                    if (CGuildHelper.GetPlayerGuildConstruct() >= info2.fPrice)
                    {
                        break;
                    }
                    CUICommonSystem.OpenGuildCoinNotEnoughTip();
                    return;
            }
            int useableStackCount = 0;
            if (info2.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
            {
                if (masterRoleInfo.IsHaveHero(info2.dwItemId, false))
                {
                    useableStackCount = 1;
                }
            }
            else if (info2.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
            {
                uint heroId = 0;
                uint skinId = 0;
                CSkinInfo.ResolveHeroSkin(info2.dwItemId, out heroId, out skinId);
                if (masterRoleInfo.IsHaveHeroSkin(heroId, skinId, false))
                {
                    useableStackCount = 1;
                }
            }
            else
            {
                useableStackCount = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableStackCount(info2.enItemType, info2.dwItemId);
            }
            CUseable useable = CUseableManager.CreateUseable(info2.enItemType, info2.dwItemId, 0);
            if (useableStackCount < useable.m_stackMax)
            {
                switch (info2.enCostType)
                {
                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_DianQuan_Pay_Tip"), info2.fPrice, info2.sName), enUIEventID.Shop_BuyItem_Confirm, enUIEventID.None, false);
                        break;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_GoldCoin_Pay_Tip"), info2.fPrice, info2.sName), enUIEventID.Shop_BuyItem_Confirm, enUIEventID.None, false);
                        break;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_Burning_Coin_Pay_Tip"), info2.fPrice, info2.sName), enUIEventID.Shop_BuyItem_Confirm, enUIEventID.None, false);
                        break;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_Arena_Coin_Pay_Tip"), info2.fPrice, info2.sName), enUIEventID.Shop_BuyItem_Confirm, enUIEventID.None, false);
                        break;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("Common_GuildCoin_Pay_Tip"), info2.fPrice, info2.sName), enUIEventID.Shop_BuyItem_Confirm, enUIEventID.None, false);
                        break;
                }
            }
            else
            {
                switch (info2.enItemType)
                {
                    case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                        Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("hasOwnHero"), false, 1.5f, null, new object[0]);
                        return;

                    case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                        Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Hero_SkinBuy_Has_Own"), false, 1.5f, null, new object[0]);
                        return;
                }
                string[] args = new string[] { useable.m_name, useable.m_stackMax.ToString() };
                Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Bag_Text_1", args), false, 1.5f, null, new object[0]);
                return;
            }
            Singleton<CUIManager>.GetInstance().CloseForm(this.sShopBuyFormPath);
        }

        private void OnShop_BuyItemConfirm(CUIEvent uiEvent)
        {
            int dwMaxRefreshTime = 0;
            stShopInfo info = new stShopInfo();
            if (this.m_Shops.TryGetValue((uint) this.m_CurShopType, out info))
            {
                dwMaxRefreshTime = info.dwMaxRefreshTime;
            }
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x465);
            CSPKG_CMD_ITEMBUY cspkg_cmd_itembuy = new CSPKG_CMD_ITEMBUY {
                wShopType = (ushort) this.m_CurShopType,
                bItemIdx = (byte) this.m_currentSelectItemIdx,
                iRefreshTime = dwMaxRefreshTime
            };
            msg.stPkgData.stItemBuyReq = cspkg_cmd_itembuy;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void OnShop_CloseForm(CUIEvent uiEvent)
        {
            if (this.m_IsShopFormOpen)
            {
                this.m_IsShopFormOpen = false;
                Singleton<CSoundManager>.GetInstance().UnLoadBank("Store_VO", CSoundManager.BankType.Lobby);
                Singleton<CUIManager>.GetInstance().CloseForm(this.sShopFormPath);
                this.m_ShopForm = null;
            }
        }

        private void OnShop_CloseItemBuyForm(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(this.sShopBuyFormPath);
        }

        private void OnShop_ConfirmManualRefresh(CUIEvent uiEvent)
        {
            stShopInfo info = new stShopInfo();
            if (this.m_Shops.TryGetValue((uint) this.m_CurShopType, out info))
            {
                info.bManualRefreshSent = true;
                this.m_Shops[(uint) this.m_CurShopType] = info;
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x45f);
                CSPKG_CMD_MANUALREFRESH cspkg_cmd_manualrefresh = new CSPKG_CMD_MANUALREFRESH {
                    wShopType = (ushort) this.m_CurShopType
                };
                msg.stPkgData.stManualRefresh = cspkg_cmd_manualrefresh;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private void OnShop_GetArenaCoin(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Shop_Arena_Shop_Go"), enUIEventID.Shop_GetArenaCoinConfirm, enUIEventID.None, false);
        }

        private void OnShop_GetArenaCoinConfirm(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_CloseForm);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_CloseItemForm);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Arena_OpenForm);
        }

        private void OnShop_GetBurningCoin(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Shop_Expedition_Shop_Go"), enUIEventID.Shop_GetBurningCoinConfirm, enUIEventID.None, false);
        }

        private void OnShop_GetBurningCoinConfirm(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_CloseForm);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_CloseItemForm);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Burn_OpenForm);
        }

        private void OnShop_ManualRefresh(CUIEvent uiEvent)
        {
            string msg = string.Empty;
            if (this.ManualRefreshLimit(ref msg))
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(msg, false);
            }
            else
            {
                int num = this.EnoughMoneyToRefresh(ref msg);
                if (num != 0)
                {
                    int num2 = num;
                    switch (num2)
                    {
                        case -2:
                            CUICommonSystem.OpenGoldCoinNotEnoughTip();
                            return;

                        case -1:
                            CUICommonSystem.OpenDianQuanNotEnoughTip();
                            return;
                    }
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(msg, false);
                }
                else
                {
                    stShopInfo info = new stShopInfo();
                    if (this.m_Shops.TryGetValue((uint) this.m_CurShopType, out info))
                    {
                        ResShopRefreshCost cost = this.GetCost(info.enShopType, info.dwManualRefreshCnt);
                        if (cost == null)
                        {
                            string[] values = new string[] { "0", Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Coin"), (info.dwManualRefreshCnt - 1).ToString() };
                            msg = CUIUtility.StringReplace(Singleton<CTextManager>.GetInstance().GetText("Shop_Manual_Refresh_Tip"), values);
                        }
                        else
                        {
                            string text = string.Empty;
                            switch (cost.bCostType)
                            {
                                case 2:
                                    text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_DianQuan");
                                    break;

                                case 4:
                                    text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_GoldCoin");
                                    break;

                                case 5:
                                    text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Burning_Coin");
                                    break;

                                case 6:
                                    text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Arena_Coin");
                                    break;

                                case 8:
                                    text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Guild_Coin");
                                    break;

                                default:
                                    text = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_GoldCoin");
                                    break;
                            }
                            string[] textArray2 = new string[] { cost.dwCostPrice.ToString(), text, (info.dwManualRefreshCnt - 1).ToString() };
                            msg = CUIUtility.StringReplace(Singleton<CTextManager>.GetInstance().GetText("Shop_Manual_Refresh_Tip"), textArray2);
                        }
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(msg, enUIEventID.Shop_ConfirmManualRefresh, enUIEventID.Shop_CancelManualRefresh, false);
                    }
                }
            }
        }

        private void OnShop_OpenArenaShopForm(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Arena_Shop;
            uiEvent.m_eventID = enUIEventID.Shop_OpenForm;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        private void OnShop_OpenBurningShopForm(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Burning_Exp_Shop;
            uiEvent.m_eventID = enUIEventID.Shop_OpenForm;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        private void OnShop_OpenForm(CUIEvent uiEvent)
        {
            if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP))
            {
                if (!this.m_IsShopFormOpen)
                {
                    this.m_ShopForm = Singleton<CUIManager>.GetInstance().OpenForm(this.sShopFormPath, false, true);
                    this.m_IsShopFormOpen = true;
                    Singleton<CSoundManager>.GetInstance().LoadBank("Store_VO", CSoundManager.BankType.Lobby);
                    this.InitTopBar();
                    this.InitTab();
                    this.InitSubTab();
                }
            }
            else
            {
                ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 12);
                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
            }
        }

        private void OnShop_OpenGuildShopForm(CUIEvent uiEvent)
        {
            this.CurTab = Tab.Guild_Shop;
            uiEvent.m_eventID = enUIEventID.Shop_OpenForm;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        private void OnShop_OpenHonorShopForm(CUIEvent uiEvent)
        {
        }

        private void OnShop_PageDown(CUIEvent uiEvent)
        {
            if (this.m_IsShopFormOpen)
            {
                CUIListScript component = this.m_ShopForm.GetWidget(1).transform.FindChild("List").GetComponent<CUIListScript>();
                this.m_CurPage = (byte) (this.m_CurPage + 1);
                if (component != null)
                {
                    component.MoveElementInScrollArea(this.m_CurPage, true);
                }
            }
        }

        private void OnShop_PageUp(CUIEvent uiEvent)
        {
            if (this.m_IsShopFormOpen)
            {
                CUIListScript component = this.m_ShopForm.GetWidget(1).transform.FindChild("List").GetComponent<CUIListScript>();
                this.m_CurPage = (byte) (this.m_CurPage - 1);
                if (component != null)
                {
                    component.MoveElementInScrollArea(this.m_CurPage, true);
                }
            }
        }

        private void OnShop_ReturnToShopForm(CUIEvent uiEvent)
        {
            this.m_IsShopFormOpen = false;
            this.m_CurShopType = this.m_LastNormalShop;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Shop_OpenForm);
        }

        public void OnShop_SaleTipCancel(CUIEvent uiEvent)
        {
            GameObject gameObject = this.m_ShopForm.transform.Find("pnlTipContainer").gameObject;
            if (gameObject != null)
            {
                gameObject.CustomSetActive(false);
            }
        }

        public void OnShop_SaleTipConfirm(CUIEvent uiEvent)
        {
            GameObject gameObject = this.m_ShopForm.transform.Find("pnlTipContainer").gameObject;
            if (gameObject != null)
            {
                gameObject.CustomSetActive(false);
            }
            ListView<CSDT_ITEM_DELINFO> inList = new ListView<CSDT_ITEM_DELINFO>();
            CUseable useable = null;
            for (int i = 0; i < this.m_saleItemsCnt; i++)
            {
                useable = this.aSaleItems[i];
                if (useable != null)
                {
                    CSDT_ITEM_DELINFO item = new CSDT_ITEM_DELINFO {
                        ullUniqueID = useable.m_objID,
                        iItemCnt = (ushort) useable.m_stackCount
                    };
                    inList.Add(item);
                }
            }
            if (inList.Count > 0)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x44d);
                CSDT_ITEM_DELLIST csdt_item_dellist = new CSDT_ITEM_DELLIST {
                    wItemCnt = (ushort) inList.Count,
                    astItemList = LinqS.ToArray<CSDT_ITEM_DELINFO>(inList)
                };
                msg.stPkgData.stItemSale.stSaleList = csdt_item_dellist;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private void OnShop_SelectItem(CUIEvent uiEvent)
        {
            this.m_currentSelectItemIdx = uiEvent.m_eventParams.tag;
            if ((this.m_currentSelectItemIdx >= 0) && (this.m_currentSelectItemIdx < Singleton<CShopSystem>.GetInstance().m_ShopItems[(uint) this.m_CurShopType].Length))
            {
                stShopItemInfo info = Singleton<CShopSystem>.GetInstance().m_ShopItems[(uint) this.m_CurShopType][this.m_currentSelectItemIdx];
                if (((!info.isSoldOut && !this.IsSlotLocked(this.m_currentSelectItemIdx)) && !this.IsOnlyAndOwned(ref info)) && (Singleton<CUIManager>.GetInstance().OpenForm(this.sShopBuyFormPath, false, true) != null))
                {
                    this.RefreshBuyPnl();
                }
            }
        }

        private void OnShop_SubTab_Change(CUIEvent uiEvent)
        {
            GameObject widget = uiEvent.m_srcFormScript.GetWidget(6);
            switch (((SubTab) this.GetSubTabSelectedIndex()))
            {
                case SubTab.Guild_Item_Shop:
                    this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_GUILD;
                    widget.CustomSetActive(true);
                    break;

                case SubTab.Guild_HeadImage_Shop:
                    this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE;
                    widget.CustomSetActive(false);
                    break;
            }
            this.CurTab = Tab.Guild_Shop;
            this.InitShop();
        }

        private void OnShop_Tab_Change(CUIEvent uiEvent)
        {
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            if (component == null)
            {
                DebugHelper.Assert(false, "CShopSystem.OnShop_Tab_Change(): lst is null!!!");
            }
            else
            {
                int num;
                Text text = component.GetSelectedElement().transform.Find("txtShopTypeData").GetComponent<Text>();
                if (!int.TryParse(text.text, out num))
                {
                    object[] inParameters = new object[] { text.text };
                    DebugHelper.Assert(false, "CShopSystem.OnShop_Tab_Change(): txtShopTypeData.text={0}", inParameters);
                }
                else
                {
                    bool flag = true;
                    uiEvent.m_srcFormScript.GetWidget(6).CustomSetActive(true);
                    GameObject widget = uiEvent.m_srcFormScript.GetWidget(5);
                    widget.CustomSetActive(false);
                    switch (((Tab) num))
                    {
                        case Tab.Fix_Shop:
                            if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP))
                            {
                                ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 12);
                                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
                                flag = false;
                            }
                            break;

                        case Tab.Pvp_Symbol_Shop:
                            if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOLSHOP))
                            {
                                ResSpecialFucUnlock unlock4 = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 14);
                                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(unlock4.szLockedTip), false, 1.5f, null, new object[0]);
                                flag = false;
                            }
                            break;

                        case Tab.Burning_Exp_Shop:
                            if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_LIUGUOYUANZHENG))
                            {
                                ResSpecialFucUnlock unlock2 = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 4);
                                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(unlock2.szLockedTip), false, 1.5f, null, new object[0]);
                                flag = false;
                            }
                            break;

                        case Tab.Arena_Shop:
                            if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA))
                            {
                                ResSpecialFucUnlock unlock = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 9);
                                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(unlock.szLockedTip), false, 1.5f, null, new object[0]);
                                flag = false;
                            }
                            break;

                        case Tab.Guild_Shop:
                            if (!Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_GUILDSHOP))
                            {
                                ResSpecialFucUnlock unlock5 = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 0x11);
                                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(unlock5.szLockedTip), false, 1.5f, null, new object[0]);
                                flag = false;
                            }
                            break;
                    }
                    if (!flag)
                    {
                        int lastSelectedIndex = component.GetLastSelectedIndex();
                        if (lastSelectedIndex != -1)
                        {
                            component.m_alwaysDispatchSelectedChangeEvent = true;
                            component.SelectElement(lastSelectedIndex, true);
                        }
                    }
                    else
                    {
                        this.CurTab = (Tab) num;
                        if (this.CurTab == Tab.Guild_Shop)
                        {
                            widget.CustomSetActive(true);
                            widget.GetComponent<CUIListScript>().SelectElement(0, true);
                        }
                        else
                        {
                            this.InitShop();
                        }
                    }
                }
            }
        }

        public bool OpenMysteryShopActiveTip()
        {
            return false;
        }

        [MessageHandler(0x466)]
        public static void ReceiveItemBuy(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CShopSystem instance = Singleton<CShopSystem>.GetInstance();
            SCPKG_CMD_ITEMBUY stItemBuyRsp = msg.stPkgData.stItemBuyRsp;
            int dwMaxRefreshTime = 0;
            stShopInfo info = new stShopInfo();
            if (instance.m_Shops.TryGetValue((uint) instance.m_CurShopType, out info))
            {
                dwMaxRefreshTime = info.dwMaxRefreshTime;
            }
            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.shoppingSuccess, new uint[0]);
            if (stItemBuyRsp.iRefreshTime == dwMaxRefreshTime)
            {
                instance.m_ShopItems[(uint) instance.m_CurShopType][stItemBuyRsp.bItemIdx].isSoldOut = true;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, instance.m_CurShopType);
                stShopItemInfo info2 = instance.m_ShopItems[(uint) instance.m_CurShopType][stItemBuyRsp.bItemIdx];
                switch (info2.enItemType)
                {
                    case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                        CUICommonSystem.ShowNewHeroOrSkin(info2.dwItemId, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority2, 0, 0);
                        break;

                    case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                    {
                        uint heroId = 0;
                        uint skinId = 0;
                        CSkinInfo.ResolveHeroSkin(info2.dwItemId, out heroId, out skinId);
                        CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority2, 0, 0);
                        break;
                    }
                }
            }
        }

        [MessageHandler(0x460)]
        public static void ReceiveShopItems(CSPkg msg)
        {
            COMDT_SHOP_DETAIL stShopInfo = msg.stPkgData.stShopDetail.stShopInfo;
            COMDT_SHOP_ITEMLIST stItemList = stShopInfo.stItemList;
            COMDT_SHOP_ITEMINFO[] astShopItem = stItemList.astShopItem;
            RES_SHOP_TYPE wShopType = (RES_SHOP_TYPE) stShopInfo.wShopType;
            CShopSystem instance = Singleton<CShopSystem>.GetInstance();
            stShopInfo info = new stShopInfo {
                enShopType = wShopType,
                dwAutoRefreshTime = stShopInfo.iAutoRefreshTime,
                dwManualRefreshTime = stShopInfo.iManualRefreshTime,
                dwApRefreshTime = stShopInfo.iAPRefreshTime,
                dwManualRefreshCnt = stShopInfo.iManualRefreshCnt + 1
            };
            ResShopType dataByKey = GameDataMgr.shopTypeDatabin.GetDataByKey((uint) ((ushort) wShopType));
            if (dataByKey != null)
            {
                info.dwManualRefreshLimit = dataByKey.iManualLimit;
            }
            else
            {
                info.dwManualRefreshLimit = 0;
            }
            info.dwMaxRefreshTime = info.dwAutoRefreshTime;
            if (info.dwMaxRefreshTime < info.dwManualRefreshTime)
            {
                info.dwMaxRefreshTime = info.dwManualRefreshTime;
            }
            else if (info.dwMaxRefreshTime < info.dwApRefreshTime)
            {
                info.dwMaxRefreshTime = info.dwApRefreshTime;
            }
            info.dwItemValidTime = stShopInfo.iItemValidTime;
            info.bManualRefreshSent = false;
            if (!instance.m_Shops.ContainsKey((uint) wShopType))
            {
                instance.m_Shops.Add((uint) wShopType, info);
            }
            else
            {
                instance.m_Shops[(uint) wShopType] = info;
            }
            if (instance.m_ShopItems.ContainsKey((uint) wShopType))
            {
                instance.m_ShopItems[(uint) wShopType] = new stShopItemInfo[stItemList.bItemCnt];
            }
            else
            {
                instance.m_ShopItems.Add((uint) wShopType, new stShopItemInfo[stItemList.bItemCnt]);
            }
            COMDT_SHOP_ITEMINFO comdt_shop_iteminfo = null;
            for (byte i = 0; i < stItemList.bItemCnt; i = (byte) (i + 1))
            {
                comdt_shop_iteminfo = astShopItem[i];
                stShopItemInfo info2 = new stShopItemInfo {
                    dwItemId = comdt_shop_iteminfo.dwItemID,
                    wItemCnt = comdt_shop_iteminfo.wItemCnt,
                    enItemType = (COM_ITEM_TYPE) comdt_shop_iteminfo.wItemType,
                    enCostType = (RES_SHOPBUY_COINTYPE) comdt_shop_iteminfo.bCostType,
                    wSaleDiscount = comdt_shop_iteminfo.wSaleDiscout
                };
                if (comdt_shop_iteminfo.bIsBuy == 1)
                {
                    info2.isSoldOut = true;
                }
                else
                {
                    info2.isSoldOut = false;
                }
                instance.m_ShopItems[(uint) wShopType][i] = (stShopItemInfo[]) info2;
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, wShopType);
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        private void RefreshBuyPnl()
        {
            int useableStackCount;
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(this.sShopBuyFormPath);
            if ((form == null) || (this.m_currentSelectItemIdx == -1))
            {
                return;
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                return;
            }
            stShopItemInfo info = Singleton<CShopSystem>.GetInstance().m_ShopItems[(uint) this.m_CurShopType][this.m_currentSelectItemIdx];
            if (info.isSoldOut)
            {
                return;
            }
            Singleton<CShopSystem>.GetInstance().GetShopItemPrice(ref info);
            CUseable itemUseable = null;
            GameObject itemCell = Utility.FindChild(form.gameObject, "Panel_Left/pnlItem/itemCell");
            GameObject obj3 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgCoin");
            GameObject obj4 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgGold");
            GameObject obj5 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgPvp");
            GameObject obj6 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgBurning");
            GameObject obj7 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgArena");
            GameObject obj8 = Utility.FindChild(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/imgGuild");
            Text componetInChild = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlItem/pnlTitle/lblName");
            Text text2 = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlItem/pnlStatus/lblCount");
            Text text3 = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlItem/pnlStatus/lblCountTitleSuffix");
            Text text4 = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/txtPrice");
            Text text5 = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlMemo/pnlDes/Text");
            Text text6 = Utility.GetComponetInChild<Text>(form.gameObject, "Panel_Left/pnlBuy/pnlPrice/txtCnt");
            itemUseable = CUseableManager.CreateUseable(info.enItemType, info.dwItemId, info.wItemCnt);
            if (itemUseable == null)
            {
                return;
            }
            switch (info.enCostType)
            {
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                    if (text4 != null)
                    {
                        if (masterRoleInfo.DianQuan >= info.fPrice)
                        {
                            text4.color = new Color(1f, 0.91f, 0.45f, 1f);
                            break;
                        }
                        text4.color = new Color(1f, 0.4f, 0.4f, 1f);
                    }
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                    if (text4 != null)
                    {
                        if (masterRoleInfo.GoldCoin >= info.fPrice)
                        {
                            text4.color = new Color(1f, 0.91f, 0.45f, 1f);
                        }
                        else
                        {
                            text4.color = new Color(1f, 0.4f, 0.4f, 1f);
                        }
                    }
                    obj5.CustomSetActive(true);
                    obj4.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj6.CustomSetActive(false);
                    obj7.CustomSetActive(false);
                    obj8.CustomSetActive(false);
                    goto Label_04C2;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                    if (text4 != null)
                    {
                        if (masterRoleInfo.BurningCoin >= info.fPrice)
                        {
                            text4.color = new Color(1f, 0.91f, 0.45f, 1f);
                        }
                        else
                        {
                            text4.color = new Color(1f, 0.4f, 0.4f, 1f);
                        }
                    }
                    obj6.CustomSetActive(true);
                    obj5.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj7.CustomSetActive(false);
                    obj8.CustomSetActive(false);
                    goto Label_04C2;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                    if (text4 != null)
                    {
                        if (masterRoleInfo.ArenaCoin >= info.fPrice)
                        {
                            text4.color = new Color(1f, 0.91f, 0.45f, 1f);
                        }
                        else
                        {
                            text4.color = new Color(1f, 0.4f, 0.4f, 1f);
                        }
                    }
                    obj7.CustomSetActive(true);
                    obj5.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj6.CustomSetActive(false);
                    obj8.CustomSetActive(false);
                    goto Label_04C2;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
                    if (text4 != null)
                    {
                        if (CGuildHelper.GetPlayerGuildConstruct() >= info.fPrice)
                        {
                            text4.color = new Color(1f, 0.91f, 0.45f, 1f);
                        }
                        else
                        {
                            text4.color = new Color(1f, 0.4f, 0.4f, 1f);
                        }
                    }
                    obj8.CustomSetActive(true);
                    obj5.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj6.CustomSetActive(false);
                    obj7.CustomSetActive(false);
                    goto Label_04C2;

                default:
                    goto Label_04C2;
            }
            obj4.CustomSetActive(true);
            obj3.CustomSetActive(false);
            obj5.CustomSetActive(false);
            obj6.CustomSetActive(false);
            obj7.CustomSetActive(false);
            obj8.CustomSetActive(false);
        Label_04C2:
            useableStackCount = 0;
            if (info.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
            {
                if (masterRoleInfo.IsHaveHero(info.dwItemId, false))
                {
                    useableStackCount = 1;
                }
            }
            else if (info.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
            {
                uint heroId = 0;
                uint skinId = 0;
                CSkinInfo.ResolveHeroSkin(info.dwItemId, out heroId, out skinId);
                if (masterRoleInfo.IsHaveHeroSkin(heroId, skinId, false))
                {
                    useableStackCount = 1;
                }
            }
            else
            {
                useableStackCount = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableStackCount(info.enItemType, info.dwItemId);
            }
            if (text2 != null)
            {
                text2.text = useableStackCount.ToString();
            }
            if (text3 != null)
            {
                text3.text = Singleton<CTextManager>.GetInstance().GetText("Shop_Item_Buy_Status_Unit");
            }
            if (componetInChild != null)
            {
                componetInChild.text = itemUseable.m_name;
            }
            if (text4 != null)
            {
                text4.text = info.fPrice.ToString();
            }
            if (text5 != null)
            {
                text5.text = itemUseable.m_description;
            }
            if (info.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
            {
                CItem item = (CItem) itemUseable;
                if (((item.m_itemData.bClass == 2) || (item.m_itemData.bClass == 3)) && (text5 != null))
                {
                    string[] values = new string[] { useableStackCount.ToString() };
                    text5.text = CUIUtility.StringReplace(text5.text, values);
                }
            }
            else if (info.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)
            {
                CEquip equip = (CEquip) itemUseable;
                if ((equip != null) && (text5 != null))
                {
                    text5.text = equip.m_description + "\n<color=#a52a2aff>" + CUICommonSystem.GetFuncEftDesc(ref equip.m_equipData.astFuncEftList, false) + "</color>";
                }
            }
            else if (info.enItemType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
            {
            }
            if (text6 != null)
            {
                text6.text = info.wItemCnt + Singleton<CTextManager>.GetInstance().GetText("Shop_Item_Buy_Status_Unit");
            }
            if (itemCell != null)
            {
                CUICommonSystem.SetItemCell(form, itemCell, itemUseable, false, false);
            }
        }

        public void RefreshInfoPanel()
        {
            switch (this.m_CurShopType)
            {
                case RES_SHOP_TYPE.RES_SHOPTYPE_FIXED:
                case RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL:
                case RES_SHOP_TYPE.RES_SHOPTYPE_BURNING:
                case RES_SHOP_TYPE.RES_SHOPTYPE_ARENA:
                case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD:
                case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE:
                {
                    GameObject widget = this.m_ShopForm.GetWidget(0);
                    GameObject gameObject = widget.transform.Find("pnlAutoRefresh/pnlRefreshTime/Text").gameObject;
                    GameObject obj4 = widget.transform.Find("pnlManualRefresh/btnRefresh").gameObject;
                    DateTime time2 = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double) CRoleInfo.GetCurrentUTCTime());
                    Text text = gameObject.GetComponent<Text>();
                    CUITimerScript script = widget.transform.Find("pnlAutoRefresh/timerAutoRefresh/timer").GetComponent<CUITimerScript>();
                    DateTime nextAutoRefreshTime = this.GetNextAutoRefreshTime(this.m_CurShopType);
                    obj4.CustomSetActive(true);
                    if (!(nextAutoRefreshTime == DateTime.MinValue))
                    {
                        TimeSpan span = (TimeSpan) (nextAutoRefreshTime - time2);
                        float time = (float) (span.TotalSeconds - 28800.0);
                        script.SetTotalTime(time);
                        script.StartTimer();
                        text.text = Singleton<CTextManager>.GetInstance().GetText("Shop_Next_Refresh_Time_Prefix") + " <color=#f5be17>" + nextAutoRefreshTime.ToString("HH:mm") + "</color>";
                        gameObject.CustomSetActive(true);
                        break;
                    }
                    gameObject.CustomSetActive(false);
                    break;
                }
                case RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY:
                {
                    GameObject obj5 = this.m_ShopForm.transform.Find("pnlShop/pnlInfo/timerMisteryAvailable").gameObject;
                    CUITimerScript script2 = obj5.transform.Find("timer").gameObject.GetComponent<CUITimerScript>();
                    int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                    stShopInfo info = new stShopInfo();
                    if (!this.m_Shops.TryGetValue(2, out info))
                    {
                        obj5.CustomSetActive(false);
                        return;
                    }
                    int num6 = info.dwItemValidTime - currentUTCTime;
                    if (num6 < 0)
                    {
                        num6 = 0;
                    }
                    script2.SetTotalTime((float) num6);
                    script2.StartTimer();
                    obj5.CustomSetActive(true);
                    return;
                }
                default:
                    return;
            }
            Text component = this.m_ShopForm.GetWidget(4).GetComponent<Text>();
            int manualRefreshMaxCnt = GetManualRefreshMaxCnt(this.m_CurShopType);
            this.ResetManualRefreshCountIfNeed();
            int manualRefreshedCnt = this.GetManualRefreshedCnt();
            string[] args = new string[] { (manualRefreshMaxCnt - manualRefreshedCnt).ToString() };
            component.text = Singleton<CTextManager>.GetInstance().GetText("Shop_Today_Left_Cnt", args);
        }

        private void RefreshReturnToShopEntry()
        {
            if (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY)
            {
                if (this.IsMysteryShopAvailable())
                {
                    this.m_ShopForm.transform.Find("pnlShop/imgReturnToShop").gameObject.CustomSetActive(true);
                }
                else
                {
                    this.m_ShopForm.transform.Find("pnlShop/imgReturnToShop").gameObject.CustomSetActive(false);
                }
            }
        }

        public void RefreshShop(RES_SHOP_TYPE shopType)
        {
            if (this.m_IsShopFormOpen)
            {
                switch (shopType)
                {
                    case RES_SHOP_TYPE.RES_SHOPTYPE_FIXED:
                        if (this.CurTab == Tab.Fix_Shop)
                        {
                            break;
                        }
                        return;

                    case RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL:
                        if (this.CurTab == Tab.Pvp_Symbol_Shop)
                        {
                            break;
                        }
                        return;

                    case RES_SHOP_TYPE.RES_SHOPTYPE_BURNING:
                        if (this.CurTab == Tab.Burning_Exp_Shop)
                        {
                            break;
                        }
                        return;

                    case RES_SHOP_TYPE.RES_SHOPTYPE_ARENA:
                        if (this.CurTab == Tab.Arena_Shop)
                        {
                            break;
                        }
                        return;

                    case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD:
                    case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE:
                        if (this.CurTab == Tab.Guild_Shop)
                        {
                            break;
                        }
                        return;
                }
                this.RefreshTopBar();
                this.RefreshShopItemsPanel();
                this.RefreshInfoPanel();
                this.RefreshReturnToShopEntry();
                this.RefreshBuyPnl();
            }
        }

        public void RefreshShopItemsPanel()
        {
            if ((this.m_ShopForm != null) && this.m_IsShopFormOpen)
            {
                GameObject widget = this.m_ShopForm.GetWidget(1);
                if (widget != null)
                {
                    CUIListScript component = widget.transform.FindChild("List").GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        CUIListElementScript listElementScript = null;
                        byte amount = 0;
                        if (this.m_ShopItems.ContainsKey((uint) this.m_CurShopType))
                        {
                            amount = Convert.ToByte(Math.Ceiling(((double) this.m_ShopItems[(uint) this.m_CurShopType].Length) / 6.0));
                        }
                        if (amount <= 1)
                        {
                            GameObject gameObject = component.transform.FindChild("Scrollbar").gameObject;
                            if (gameObject != null)
                            {
                                gameObject.CustomSetActive(false);
                            }
                        }
                        else
                        {
                            GameObject obj4 = component.transform.FindChild("Scrollbar").gameObject;
                            if (obj4 != null)
                            {
                                obj4.CustomSetActive(true);
                            }
                        }
                        component.SetElementAmount(amount);
                        for (byte i = 0; i < amount; i = (byte) (i + 1))
                        {
                            listElementScript = component.GetElemenet(i);
                            if (this.m_CurPage == i)
                            {
                                component.MoveElementInScrollArea(this.m_CurPage, true);
                            }
                            this.SetPageItems(listElementScript, i);
                        }
                        component.m_alwaysDispatchSelectedChangeEvent = false;
                    }
                }
            }
        }

        public void RefreshTopBar()
        {
            if (this.m_ShopForm != null)
            {
                GameObject widget = this.m_ShopForm.GetWidget(3);
                if (widget != null)
                {
                    Transform transform = widget.transform;
                    GameObject gameObject = transform.FindChild("Coin").gameObject;
                    GameObject obj4 = transform.FindChild("Gold").gameObject;
                    GameObject obj5 = transform.FindChild("PvpCoin").gameObject;
                    GameObject obj6 = transform.FindChild("BurningCoin").gameObject;
                    GameObject obj7 = transform.FindChild("ArenaCoin").gameObject;
                    GameObject obj8 = transform.FindChild("GuildConstruct").gameObject;
                    Text component = null;
                    switch (this.m_CurShopType)
                    {
                        case RES_SHOP_TYPE.RES_SHOPTYPE_FIXED:
                        case RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY:
                            widget.CustomSetActive(true);
                            gameObject.CustomSetActive(true);
                            component = gameObject.transform.FindChild("Text").GetComponent<Text>();
                            if (component != null)
                            {
                                component.text = this.GetCoinString(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GoldCoin);
                            }
                            obj4.CustomSetActive(true);
                            component = obj4.transform.FindChild("Text").GetComponent<Text>();
                            if (component != null)
                            {
                                component.text = this.GetCoinString(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GoldCoin);
                            }
                            obj5.CustomSetActive(true);
                            component = obj5.transform.FindChild("Text").GetComponent<Text>();
                            if (component != null)
                            {
                                component.text = this.GetCoinString(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GoldCoin);
                            }
                            obj6.CustomSetActive(false);
                            obj7.CustomSetActive(false);
                            obj8.CustomSetActive(false);
                            break;

                        case RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL:
                            widget.CustomSetActive(true);
                            gameObject.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj5.CustomSetActive(true);
                            obj6.CustomSetActive(false);
                            obj7.CustomSetActive(false);
                            obj8.CustomSetActive(false);
                            break;

                        case RES_SHOP_TYPE.RES_SHOPTYPE_BURNING:
                            widget.CustomSetActive(true);
                            gameObject.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj5.CustomSetActive(false);
                            obj6.CustomSetActive(true);
                            component = obj6.transform.FindChild("Text").GetComponent<Text>();
                            if (component != null)
                            {
                                component.text = this.GetCoinString(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().BurningCoin);
                            }
                            obj7.CustomSetActive(false);
                            obj8.CustomSetActive(false);
                            break;

                        case RES_SHOP_TYPE.RES_SHOPTYPE_ARENA:
                            widget.CustomSetActive(true);
                            gameObject.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj5.CustomSetActive(false);
                            obj6.CustomSetActive(false);
                            obj7.CustomSetActive(true);
                            component = obj7.transform.FindChild("Text").GetComponent<Text>();
                            if (component != null)
                            {
                                component.text = this.GetCoinString(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().ArenaCoin);
                            }
                            obj8.CustomSetActive(false);
                            break;

                        case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD:
                        case RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE:
                            widget.CustomSetActive(true);
                            gameObject.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj5.CustomSetActive(false);
                            obj6.CustomSetActive(false);
                            obj7.CustomSetActive(false);
                            obj8.CustomSetActive(true);
                            component = obj8.transform.FindChild("Text").GetComponent<Text>();
                            if (component != null)
                            {
                                component.text = this.GetCoinString(CGuildHelper.GetPlayerGuildConstruct());
                            }
                            break;
                    }
                }
            }
        }

        private void ResetManualRefreshCountIfNeed()
        {
            stShopInfo info;
            if (this.m_Shops.TryGetValue((uint) this.m_CurShopType, out info))
            {
                uint globalRefreshTimeSeconds = Utility.GetGlobalRefreshTimeSeconds();
                int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                if ((info.dwManualRefreshTime < globalRefreshTimeSeconds) && (globalRefreshTimeSeconds <= currentUTCTime))
                {
                    info.dwManualRefreshCnt = 1;
                }
            }
        }

        private void SetItemInfo(GameObject item, ref stShopItemInfo info, int slotOffset)
        {
            Singleton<CShopSystem>.GetInstance().GetShopItemPrice(ref info);
            Transform transform = item.transform;
            GameObject gameObject = transform.Find("itemCell").gameObject;
            GameObject obj3 = transform.Find("txtName").gameObject;
            GameObject obj4 = transform.Find("pnlPrice").gameObject;
            Transform transform2 = obj4.transform;
            GameObject obj5 = transform.Find("pnlLock").gameObject;
            GameObject obj6 = transform2.Find("imgCoin").gameObject;
            GameObject obj7 = transform2.Find("imgGold").gameObject;
            GameObject obj8 = transform2.Find("imgPvp").gameObject;
            GameObject obj9 = transform2.Find("imgBurning").gameObject;
            GameObject obj10 = transform2.Find("imgArena").gameObject;
            GameObject obj11 = transform2.Find("imgGuild").gameObject;
            GameObject obj12 = transform.Find("pnlDiscount").gameObject;
            GameObject obj13 = transform.Find("imgNormal").gameObject;
            GameObject obj14 = transform.Find("imgGray").gameObject;
            GameObject obj15 = transform.Find("imgSoldOut").gameObject;
            GameObject obj16 = transform.Find("pnlStatus").gameObject;
            Transform transform3 = obj12.transform;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            Text component = transform.Find("txtName").GetComponent<Text>();
            Text text2 = transform3.Find("txtDiscount").GetComponent<Text>();
            Text text3 = transform2.Find("txtPrice").GetComponent<Text>();
            CUseable itemUseable = null;
            switch (info.enCostType)
            {
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                    obj7.CustomSetActive(true);
                    obj6.CustomSetActive(false);
                    obj8.CustomSetActive(false);
                    obj9.CustomSetActive(false);
                    obj10.CustomSetActive(false);
                    obj11.CustomSetActive(false);
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                    obj8.CustomSetActive(true);
                    obj7.CustomSetActive(false);
                    obj6.CustomSetActive(false);
                    obj9.CustomSetActive(false);
                    obj10.CustomSetActive(false);
                    obj11.CustomSetActive(false);
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                    obj9.CustomSetActive(true);
                    obj8.CustomSetActive(false);
                    obj7.CustomSetActive(false);
                    obj6.CustomSetActive(false);
                    obj10.CustomSetActive(false);
                    obj11.CustomSetActive(false);
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                    obj10.CustomSetActive(true);
                    obj8.CustomSetActive(false);
                    obj7.CustomSetActive(false);
                    obj6.CustomSetActive(false);
                    obj9.CustomSetActive(false);
                    obj11.CustomSetActive(false);
                    break;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
                    obj11.CustomSetActive(true);
                    obj8.CustomSetActive(false);
                    obj7.CustomSetActive(false);
                    obj6.CustomSetActive(false);
                    obj9.CustomSetActive(false);
                    obj10.CustomSetActive(false);
                    break;
            }
            itemUseable = CUseableManager.CreateUseable(info.enItemType, info.dwItemId, info.wItemCnt);
            if (itemUseable != null)
            {
                bool flag = this.IsOnlyAndOwned(ref info);
                string name = itemUseable.m_name;
                component.text = name;
                info.sName = name;
                text3.text = info.fPrice.ToString("N0");
                switch (info.enCostType)
                {
                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                        if (masterRoleInfo.DianQuan >= info.fPrice)
                        {
                            text3.color = new Color(1f, 0.91f, 0.45f, 1f);
                            break;
                        }
                        text3.color = new Color(1f, 0.4f, 0.4f, 1f);
                        break;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                        if (masterRoleInfo.GoldCoin >= info.fPrice)
                        {
                            text3.color = new Color(1f, 0.91f, 0.45f, 1f);
                            break;
                        }
                        text3.color = new Color(1f, 0.4f, 0.4f, 1f);
                        break;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                        if (masterRoleInfo.BurningCoin >= info.fPrice)
                        {
                            text3.color = new Color(1f, 0.91f, 0.45f, 1f);
                            break;
                        }
                        text3.color = new Color(1f, 0.4f, 0.4f, 1f);
                        break;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                        if (masterRoleInfo.ArenaCoin >= info.fPrice)
                        {
                            text3.color = new Color(1f, 0.91f, 0.45f, 1f);
                            break;
                        }
                        text3.color = new Color(1f, 0.4f, 0.4f, 1f);
                        break;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
                        if (CGuildHelper.GetPlayerGuildConstruct() >= info.fPrice)
                        {
                            text3.color = new Color(1f, 0.91f, 0.45f, 1f);
                            break;
                        }
                        text3.color = new Color(1f, 0.4f, 0.4f, 1f);
                        break;
                }
                if (info.isSoldOut)
                {
                    text3.color = new Color(0.7176f, 0.7176f, 0.7176f, 1f);
                    obj13.CustomSetActive(false);
                    obj14.CustomSetActive(true);
                    obj15.CustomSetActive(true);
                    gameObject.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                }
                else
                {
                    obj13.CustomSetActive(true);
                    obj14.CustomSetActive(false);
                    obj15.CustomSetActive(false);
                    gameObject.CustomSetActive(true);
                    obj3.CustomSetActive(true);
                    CUICommonSystem.SetItemCell(item.GetComponent<CUIEventScript>().m_belongedFormScript, gameObject, itemUseable, false, false);
                }
                if (info.wSaleDiscount != 100)
                {
                    obj12.CustomSetActive(true);
                    text2.text = ((((float) info.wSaleDiscount) / 10f)).ToString("F1") + Singleton<CTextManager>.GetInstance().GetText("Shop_Discount");
                }
                else
                {
                    obj12.CustomSetActive(false);
                }
                if (this.IsSlotLocked(slotOffset))
                {
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    obj5.CustomSetActive(true);
                    this.SetItemLockPanel(obj5, slotOffset);
                }
                else
                {
                    obj3.CustomSetActive(true);
                    obj4.CustomSetActive(true);
                    obj5.CustomSetActive(false);
                }
                if (flag)
                {
                    obj16.CustomSetActive(true);
                    obj3.CustomSetActive(true);
                    obj4.CustomSetActive(false);
                    obj5.CustomSetActive(false);
                }
                else
                {
                    obj16.CustomSetActive(false);
                }
            }
        }

        private void SetItemLockPanel(GameObject lockPanel, int slotOffset)
        {
            Text component = lockPanel.transform.Find("txtUnlockTip").GetComponent<Text>();
            if (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD)
            {
                uint starLevelForOpenGuildItemShopSlot = CGuildHelper.GetStarLevelForOpenGuildItemShopSlot(slotOffset);
                string[] args = new string[] { starLevelForOpenGuildItemShopSlot.ToString() };
                component.text = Singleton<CTextManager>.GetInstance().GetText("Shop_Guild_Item_Shop_Unlock_Tip", args);
            }
            else if (this.m_CurShopType == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE)
            {
                string gradeNameForOpenGuildHeadImageShopSlot = CGuildHelper.GetGradeNameForOpenGuildHeadImageShopSlot(slotOffset);
                string[] textArray2 = new string[] { gradeNameForOpenGuildHeadImageShopSlot };
                component.text = Singleton<CTextManager>.GetInstance().GetText("Shop_Guild_Head_Image_Shop_Unlock_Tip", textArray2);
            }
        }

        private void SetPageItems(CUIListElementScript listElementScript, byte page)
        {
            int slotOffset = 6 * page;
            int num2 = slotOffset + 6;
            GameObject gameObject = listElementScript.gameObject;
            byte num3 = 0;
            while (slotOffset < num2)
            {
                Transform transform = gameObject.transform.FindChild("pnlItem" + num3);
                if (slotOffset >= this.m_ShopItems[(uint) this.m_CurShopType].Length)
                {
                    if (transform != null)
                    {
                        transform.gameObject.CustomSetActive(false);
                    }
                }
                else if (transform != null)
                {
                    GameObject obj3 = transform.gameObject;
                    obj3.CustomSetActive(true);
                    CUIEventScript component = obj3.GetComponent<CUIEventScript>();
                    if (component == null)
                    {
                        component = obj3.AddComponent<CUIEventScript>();
                        component.Initialize(listElementScript.m_belongedFormScript);
                    }
                    stUIEventParams eventParams = new stUIEventParams {
                        tag = slotOffset
                    };
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Shop_SelectItem, eventParams);
                    this.SetItemInfo(obj3, ref this.m_ShopItems[(uint) this.m_CurShopType][slotOffset], slotOffset);
                }
                num3 = (byte) (num3 + 1);
                slotOffset++;
            }
        }

        private void SetSaleItemInfo(CUIListElementScript listElementScript, ref stShopItemInfo info)
        {
            Transform transform = listElementScript.gameObject.transform;
            GameObject gameObject = transform.Find("itemCell").gameObject;
            Text component = transform.Find("txtName").GetComponent<Text>();
            Text text2 = transform.Find("txtCnt").GetComponent<Text>();
            CUseable itemUseable = null;
            uint dwCoinSale = 0;
            switch (info.enItemType)
            {
                case COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP:
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, info.dwItemId, info.wItemCnt);
                    if (itemUseable != null)
                    {
                        dwCoinSale = GameDataMgr.itemDatabin.GetDataByKey(info.dwItemId).dwCoinSale;
                    }
                    break;

                case COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP:
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, info.dwItemId, info.wItemCnt);
                    if (itemUseable != null)
                    {
                        dwCoinSale = GameDataMgr.equipInfoDatabin.GetDataByKey(info.dwItemId).dwCoinSale;
                    }
                    break;

                case COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL:
                    itemUseable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, info.dwItemId, info.wItemCnt);
                    if (itemUseable != null)
                    {
                        dwCoinSale = GameDataMgr.itemDatabin.GetDataByKey(info.dwItemId).dwCoinSale;
                    }
                    break;
            }
            if (itemUseable != null)
            {
                component.text = info.sName;
                text2.text = "\x00d7" + info.wItemCnt;
                uint num2 = 0;
                num2 = info.wItemCnt * dwCoinSale;
                info.fPrice = num2;
                CUICommonSystem.SetItemCell(listElementScript.m_belongedFormScript, gameObject, itemUseable, false, false);
            }
        }

        [MessageHandler(0x467)]
        public static void ShopTimeOut(CSPkg msg)
        {
            SCPKG_NTF_SHOPTIMEOUT stShopTimeOut = msg.stPkgData.stShopTimeOut;
            string text = Singleton<CTextManager>.GetInstance().GetText("Shop_Err_Shop_Time_Out");
            string str2 = string.Empty;
            RES_SHOP_TYPE wShopType = (RES_SHOP_TYPE) stShopTimeOut.wShopType;
            if (wShopType == RES_SHOP_TYPE.RES_SHOPTYPE_FIXED)
            {
                str2 = Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Fixed");
            }
            else if (wShopType == RES_SHOP_TYPE.RES_SHOPTYPE_MYSTERY)
            {
                str2 = Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Mystery");
            }
            else
            {
                str2 = Singleton<CTextManager>.GetInstance().GetText("Shop_Type_Fixed");
            }
            string[] values = new string[] { str2 };
            text = CUIUtility.StringReplace(text, values);
            Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
        }

        public override void UnInit()
        {
            base.UnInit();
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Shop_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenForm));
            instance.RemoveUIEventListener(enUIEventID.Shop_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnShop_CloseForm));
            instance.RemoveUIEventListener(enUIEventID.Shop_SelectTab, new CUIEventManager.OnUIEventHandler(this.OnShop_Tab_Change));
            instance.RemoveUIEventListener(enUIEventID.Shop_SelectItem, new CUIEventManager.OnUIEventHandler(this.OnShop_SelectItem));
            instance.RemoveUIEventListener(enUIEventID.Shop_CloseItemForm, new CUIEventManager.OnUIEventHandler(this.OnShop_CloseItemBuyForm));
            instance.RemoveUIEventListener(enUIEventID.Shop_BuyItem, new CUIEventManager.OnUIEventHandler(this.OnShop_BuyItem));
            instance.RemoveUIEventListener(enUIEventID.Shop_ManualRefresh, new CUIEventManager.OnUIEventHandler(this.OnShop_ManualRefresh));
            instance.RemoveUIEventListener(enUIEventID.Shop_ConfirmManualRefresh, new CUIEventManager.OnUIEventHandler(this.OnShop_ConfirmManualRefresh));
            instance.RemoveUIEventListener(enUIEventID.Shop_SaleTipCancel, new CUIEventManager.OnUIEventHandler(this.OnShop_SaleTipCancel));
            instance.RemoveUIEventListener(enUIEventID.Shop_SaleTipConfirm, new CUIEventManager.OnUIEventHandler(this.OnShop_SaleTipConfirm));
            instance.RemoveUIEventListener(enUIEventID.Shop_AutoRefreshTimerTimeUp, new CUIEventManager.OnUIEventHandler(this.OnShop_AutoRefreshTimerTimeUp));
            instance.RemoveUIEventListener(enUIEventID.Shop_ReturnToShopForm, new CUIEventManager.OnUIEventHandler(this.OnShop_ReturnToShopForm));
            instance.RemoveUIEventListener(enUIEventID.Shop_GetBurningCoin, new CUIEventManager.OnUIEventHandler(this.OnShop_GetBurningCoin));
            instance.RemoveUIEventListener(enUIEventID.Shop_GetArenaCoin, new CUIEventManager.OnUIEventHandler(this.OnShop_GetArenaCoin));
            instance.RemoveUIEventListener(enUIEventID.Shop_Page_Up, new CUIEventManager.OnUIEventHandler(this.OnShop_PageUp));
            instance.RemoveUIEventListener(enUIEventID.Shop_Page_Down, new CUIEventManager.OnUIEventHandler(this.OnShop_PageDown));
            instance.RemoveUIEventListener(enUIEventID.Shop_OpenHonorShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenHonorShopForm));
            instance.RemoveUIEventListener(enUIEventID.Shop_OpenArenaShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenArenaShopForm));
            instance.RemoveUIEventListener(enUIEventID.Shop_OpenBurningShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenBurningShopForm));
            instance.RemoveUIEventListener(enUIEventID.Shop_OpenGuildShop, new CUIEventManager.OnUIEventHandler(this.OnShop_OpenGuildShopForm));
            instance.RemoveUIEventListener(enUIEventID.Shop_BuyItem_Confirm, new CUIEventManager.OnUIEventHandler(this.OnShop_BuyItemConfirm));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Shop_Auto_Refresh_Shop_Items, new System.Action(this.OnRequestAutoRefreshShopItems));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, new Action<RES_SHOP_TYPE>(this.OnReceiveShopItems));
            Singleton<EventRouter>.instance.RemoveEventHandler("MasterAttributesChanged", new System.Action(this.MasterAttrChanged));
            this.m_ShopForm = null;
        }

        private void UseOldShopItems(int seq)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.GetInstance().BroadCastEvent<RES_SHOP_TYPE>(EventID.Shop_Receive_Shop_Items, this.m_CurShopType);
        }

        public Tab CurTab
        {
            get
            {
                return this.m_CurTab;
            }
            set
            {
                this.m_CurTab = value;
                switch (this.m_CurTab)
                {
                    case Tab.Fix_Shop:
                        this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_FIXED;
                        break;

                    case Tab.Pvp_Symbol_Shop:
                        this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL;
                        break;

                    case Tab.Burning_Exp_Shop:
                        this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_BURNING;
                        break;

                    case Tab.Arena_Shop:
                        this.m_CurShopType = RES_SHOP_TYPE.RES_SHOPTYPE_ARENA;
                        break;

                    case Tab.Guild_Shop:
                        this.m_CurShopType = (this.GetSubTabSelectedIndex() != 1) ? RES_SHOP_TYPE.RES_SHOPTYPE_GUILD : RES_SHOP_TYPE.RES_SHOPTYPE_GUILD_HEAD_IMAGE;
                        break;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetCost>c__AnonStorey89
        {
            internal int refreshCnt;
            internal RES_SHOP_TYPE shopType;

            internal bool <>m__8F(ResShopRefreshCost x)
            {
                return ((((RES_SHOP_TYPE) x.wShopType) == this.shopType) && (this.refreshCnt == x.wRefreshFreq));
            }
        }

        public enum enShopFormWidget
        {
            Info_Panel,
            Items_Panel,
            Tab,
            TopBar,
            Left_Refresh_Count_Text,
            Sub_Tab,
            Manual_Refresh_Panel
        }

        public enum SubTab
        {
            Guild_Item_Shop,
            Guild_HeadImage_Shop
        }

        public enum Tab
        {
            Fix_Shop,
            Pvp_Symbol_Shop,
            Burning_Exp_Shop,
            Arena_Shop,
            Guild_Shop
        }
    }
}

