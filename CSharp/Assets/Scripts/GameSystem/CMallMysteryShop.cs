namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CMallMysteryShop : Singleton<CMallMysteryShop>
    {
        private uint m_BoughtCnt;
        private uint m_Discount;
        private DateTime m_EndTime;
        private int m_EndTimerSeq;
        private bool m_HasGotDiscount;
        private bool m_Inited;
        private bool m_IsRandomDiscount;
        private uint m_LimitBuyCnt;
        private int m_NextRefreshTimerSeq;
        private DictionaryView<long, CMallMysteryProduct> m_ProductDic;
        private ListView<CMallMysteryProduct> m_Products;
        private uint m_ShopID;
        private DateTime m_StartTime;
        public static string MYSTERY_ROLL_DISCOUNT_FORM_PATH = "UGUI/Form/System/Mall/Form_NewDiscount.prefab";

        public void Clear()
        {
            this.m_ShopID = 0;
            this.m_LimitBuyCnt = 0;
            this.m_BoughtCnt = 0;
            this.m_StartTime = DateTime.MinValue;
            this.m_EndTime = DateTime.MinValue;
            this.m_Products.Clear();
            this.m_ProductDic.Clear();
            this.m_IsRandomDiscount = false;
            this.m_HasGotDiscount = false;
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_NextRefreshTimerSeq);
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_EndTimerSeq);
            this.m_Discount = 100;
            this.m_Inited = false;
        }

        public void CloseShop(int seq = -1)
        {
            this.Clear();
        }

        public void Draw(CUIFormScript form)
        {
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Roll_Discount, new CUIEventManager.OnUIEventHandler(this.OnRollDiscount));
            instance.AddUIEventListener(enUIEventID.Mall_Close_Mystery_Shop, new CUIEventManager.OnUIEventHandler(this.OnCloseMysteryShop));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Open_Buy_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyForm));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnBuyItem));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Confirm_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnConfirmuyItem));
            instance.AddUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Time_End, new CUIEventManager.OnUIEventHandler(this.OnShopTimeEnd));
            instance.AddUIEventListener(enUIEventID.Mall_Mystery_On_Buy_Hero_Not_Own, new CUIEventManager.OnUIEventHandler(this.OnBuyHeroNotOwn));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Change_Tab, new System.Action(this.OnMallTabChange));
            this.InitElements();
            this.RefreshBanner();
            this.RefreshProducts();
        }

        public string GetDiscountNumIconPath(uint discount)
        {
            if ((discount > 0) && (discount < 100))
            {
                return (CUIUtility.s_Sprite_System_Mall_Dir + string.Format("Discount_Bg_N{0}", discount / 10));
            }
            return string.Format("{0}{1}", CUIUtility.s_Sprite_System_Mall_Dir, "Discount_Bg_WenHao");
        }

        public static stPayInfoSet GetPayInfoSetOfGood(COM_ITEM_TYPE itemType, uint itemID)
        {
            CMallMysteryShop instance = Singleton<CMallMysteryShop>.GetInstance();
            stPayInfoSet set = new stPayInfoSet(2);
            long doubleKey = GameDataMgr.GetDoubleKey((uint) itemType, itemID);
            CMallMysteryProduct product = new CMallMysteryProduct();
            if (!instance.m_ProductDic.TryGetValue(doubleKey, out product))
            {
                return set;
            }
            uint index = (instance.Discount / 10) - 1;
            stPayInfo info = new stPayInfo();
            switch (product.ItemType)
            {
                case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(itemID);
                    DebugHelper.Assert(dataByKey != null, "神秘商店配置的英雄ID有错，英雄表里不存在");
                    if (dataByKey != null)
                    {
                        ResHeroShop shop2 = null;
                        GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out shop2);
                        info.m_payType = CMallSystem.ResBuyTypeToPayType(product.Cfg.bMoneyType);
                        switch (info.m_payType)
                        {
                            case enPayType.GoldCoin:
                                info.m_oriValue = (shop2 == null) ? 1 : shop2.dwBuyCoin;
                                goto Label_0207;

                            case enPayType.DianQuan:
                                info.m_oriValue = (shop2 == null) ? 1 : shop2.dwBuyCoupons;
                                goto Label_0207;

                            case enPayType.Diamond:
                            case enPayType.DiamondAndDianQuan:
                                info.m_oriValue = (shop2 == null) ? 1 : shop2.dwBuyDiamond;
                                goto Label_0207;
                        }
                        break;
                    }
                    return set;
                }
                case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                {
                    ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(itemID);
                    DebugHelper.Assert(heroSkin != null, "神秘商店配置的皮肤ID有错，皮肤表里不存在");
                    if (heroSkin != null)
                    {
                        ResHeroSkinShop shop3 = null;
                        GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwID, out shop3);
                        info.m_payType = CMallSystem.ResBuyTypeToPayType(product.Cfg.bMoneyType);
                        switch (info.m_payType)
                        {
                            case enPayType.DianQuan:
                                info.m_oriValue = (shop3 == null) ? 1 : shop3.dwBuyCoupons;
                                goto Label_0207;

                            case enPayType.Diamond:
                            case enPayType.DiamondAndDianQuan:
                                info.m_oriValue = (shop3 == null) ? 1 : shop3.dwBuyDiamond;
                                goto Label_0207;
                        }
                        break;
                    }
                    return set;
                }
            }
        Label_0207:
            if (info.m_oriValue == 0)
            {
                info.m_oriValue = (uint) product.Cfg.RouZheKou[product.Cfg.RouZheKou.Length - 1];
            }
            if ((index < 0) || (index >= product.Cfg.RouZheKou.Length))
            {
                info.m_payValue = info.m_oriValue;
                info.m_discountForDisplay = 0x2710;
            }
            else if (instance.IsRandomDiscount)
            {
                info.m_payValue = (uint) product.Cfg.RouZheKou[index];
                info.m_discountForDisplay = instance.Discount * 100;
            }
            else
            {
                info.m_payValue = (uint) product.Cfg.iConfirmZheKou;
                info.m_discountForDisplay = instance.Discount * 100;
            }
            set.m_payInfos[set.m_payInfoCount] = info;
            set.m_payInfoCount++;
            return set;
        }

        public CMallMysteryProduct GetProduct(uint itemType, uint itemID)
        {
            CMallMysteryProduct product = new CMallMysteryProduct();
            if (this.m_ProductDic.TryGetValue(GameDataMgr.GetDoubleKey(itemType, itemID), out product))
            {
                return product;
            }
            return null;
        }

        [MessageHandler(0x582)]
        public static void GetRollDiscount(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            byte bZheKou = msg.stPkgData.stAkaliShopZheKouRsp.bZheKou;
            Singleton<CMallMysteryShop>.GetInstance().Discount = (uint) (bZheKou * 10);
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(MYSTERY_ROLL_DISCOUNT_FORM_PATH, false, true);
            DebugHelper.Assert(script != null, "获得随机折扣form失败");
            if (script != null)
            {
                CUICommonSystem.PlayAnimator(Utility.FindChild(script.gameObject, "Panel_NewDiscount/Content/Discount"), string.Format("Discount_{0}", bZheKou));
            }
        }

        public int GetTimeToClose()
        {
            TimeSpan span = (TimeSpan) (this.m_EndTime - Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime()));
            if (span.TotalSeconds > 0.0)
            {
                return ((span.TotalSeconds <= 2147483647.0) ? ((int) span.TotalSeconds) : 0x7fffffff);
            }
            return 0;
        }

        public override void Init()
        {
            base.Init();
            this.m_StartTime = DateTime.MinValue;
            this.m_EndTime = DateTime.MinValue;
            this.m_IsRandomDiscount = false;
            this.m_HasGotDiscount = false;
            this.m_Discount = 100;
            this.m_Products = new ListView<CMallMysteryProduct>();
            this.m_ProductDic = new DictionaryView<long, CMallMysteryProduct>();
            this.m_NextRefreshTimerSeq = 0;
            this.m_EndTimerSeq = 0;
            this.m_Inited = false;
        }

        public void InitData()
        {
            DateTime time = new DateTime(0x7f6, 1, 0x12);
            if (!this.m_Inited)
            {
                DateTime time2 = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                DictionaryView<uint, ResAkaliShopCtrl>.Enumerator enumerator = GameDataMgr.mysteryShopCtlDict.GetEnumerator();
                DateTime time3 = time;
                bool flag = false;
                bool flag2 = false;
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, ResAkaliShopCtrl> current = enumerator.Current;
                    ResAkaliShopCtrl ctrl = current.Value;
                    DateTime time4 = Utility.StringToDateTime(Utility.UTF8Convert(ctrl.szOpenTime), DateTime.MinValue);
                    DateTime time5 = Utility.StringToDateTime(Utility.UTF8Convert(ctrl.szCloseTime), DateTime.MinValue);
                    if ((time2.CompareTo(time4) >= 0) && (time2.CompareTo(time5) <= 0))
                    {
                        this.m_ShopID = ctrl.dwShopID;
                        this.m_Products.Clear();
                        this.m_ProductDic.Clear();
                        this.m_StartTime = time4;
                        this.m_EndTime = time5;
                        this.m_LimitBuyCnt = ctrl.dwTotalBuyNumLimit;
                        this.m_BoughtCnt = 0;
                        switch (((RES_AKALISHOPZHEKOU_TYPE) ctrl.bZheKouType))
                        {
                            case RES_AKALISHOPZHEKOU_TYPE.RES_AKALISHOPZHEKOU_CONFIRM:
                                this.m_IsRandomDiscount = false;
                                this.m_Discount = (uint) ctrl.iConfirmZheKou;
                                this.m_HasGotDiscount = true;
                                break;

                            case RES_AKALISHOPZHEKOU_TYPE.RES_AKALISHOPZHEKOU_RANDOM:
                                this.m_IsRandomDiscount = true;
                                this.m_Discount = 100;
                                this.m_HasGotDiscount = false;
                                break;
                        }
                        DictionaryView<uint, ResAkaliShopGoods>.Enumerator enumerator2 = GameDataMgr.mysteryShopProductDict.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            ResHeroCfgInfo dataByKey;
                            ResHeroSkin heroSkin;
                            ResHeroCfgInfo info2;
                            long num;
                            KeyValuePair<uint, ResAkaliShopGoods> pair2 = enumerator2.Current;
                            ResAkaliShopGoods goodsCfg = pair2.Value;
                            if (goodsCfg.dwShopID != ctrl.dwShopID)
                            {
                                continue;
                            }
                            CMallMysteryProduct item = new CMallMysteryProduct(goodsCfg);
                            switch (item.ItemType)
                            {
                                case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                                {
                                    dataByKey = GameDataMgr.heroDatabin.GetDataByKey(item.ItemID);
                                    if (dataByKey != null)
                                    {
                                        break;
                                    }
                                    continue;
                                }
                                case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                                {
                                    heroSkin = CSkinInfo.GetHeroSkin(item.ItemID);
                                    if (heroSkin != null)
                                    {
                                        goto Label_0201;
                                    }
                                    continue;
                                }
                                default:
                                    goto Label_0254;
                            }
                            if (GameDataMgr.IsHeroAvailable(dataByKey.dwCfgID))
                            {
                                this.m_Products.Add(item);
                            }
                            goto Label_0266;
                        Label_0201:
                            info2 = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
                            if (info2 == null)
                            {
                                continue;
                            }
                            if (GameDataMgr.IsSkinAvailable(heroSkin.dwID) && GameDataMgr.IsHeroAvailable(info2.dwCfgID))
                            {
                                this.m_Products.Add(item);
                            }
                            goto Label_0266;
                        Label_0254:
                            this.m_Products.Add(item);
                        Label_0266:
                            num = GameDataMgr.GetDoubleKey((uint) item.ItemType, item.ItemID);
                            if (!this.m_ProductDic.ContainsKey(num))
                            {
                                this.m_ProductDic.Add(num, item);
                            }
                        }
                        if (this.m_Products.Count == 0)
                        {
                            DebugHelper.Assert(false, "神秘商店配置了开启时段，但商店里没物品可卖");
                        }
                        else
                        {
                            flag = true;
                        }
                        continue;
                    }
                    if ((time4.CompareTo(time2) > 0) && (time4.CompareTo(time3) < 0))
                    {
                        time3 = time4;
                    }
                }
                TimeSpan span = (TimeSpan) (time3 - time2);
                Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_NextRefreshTimerSeq);
                long num2 = (long) (span.TotalSeconds * 1000.0);
                if ((num2 > 0L) && (num2 < 0x7fffffffL))
                {
                    this.m_NextRefreshTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer((int) num2, 1, new CTimer.OnTimeUpHandler(this.RefreshShop));
                    flag2 = true;
                }
                TimeSpan span2 = (TimeSpan) (this.m_EndTime - time2);
                Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_EndTimerSeq);
                long num3 = ((int) span2.TotalSeconds) * 0x3e8;
                if ((num3 > 0L) && (num3 < 0x7fffffffL))
                {
                    this.m_EndTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer((int) num3, 1, new CTimer.OnTimeUpHandler(this.CloseShop));
                }
                if (flag || flag2)
                {
                    this.m_Inited = true;
                }
            }
        }

        private void InitElements()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery").CustomSetActive(true);
            }
        }

        public bool IsShopAvailable()
        {
            if (!this.m_Inited)
            {
                this.InitData();
            }
            DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            return ((time.CompareTo(this.m_StartTime) >= 0) && (time.CompareTo(this.m_EndTime) <= 0));
        }

        public void Load(CUIFormScript form)
        {
            CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/Mystery", "pnlMystery", form.GetWidget(3), form);
        }

        public bool Loaded(CUIFormScript form)
        {
            if (Utility.FindChild(form.GetWidget(3), "pnlMystery") == null)
            {
                return false;
            }
            return true;
        }

        private void OnBuyHeroNotOwn(CUIEvent uiEvent)
        {
            uint heroId = uiEvent.m_eventParams.heroId;
            DebugHelper.Assert(heroId != 0, "未拥有的英雄ID不能为0");
            if (heroId != 0)
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                DebugHelper.Assert(dataByKey != null, "神秘商店配置的英雄ID有错，英雄表里不存在");
                if (dataByKey != null)
                {
                    CMallMysteryProduct product = this.GetProduct(4, heroId);
                    if (product != null)
                    {
                        stPayInfoSet lowestPayInfoSetOfGood = CMallSystem.GetLowestPayInfoSetOfGood(dataByKey, CMallSystem.ResBuyTypeToPayType(product.Cfg.bMoneyType));
                        CHeroSkinBuyManager.OpenBuyHeroForm(uiEvent.m_srcFormScript, heroId, lowestPayInfoSetOfGood, enUIEventID.Mall_Mystery_On_Buy_Item);
                    }
                    else
                    {
                        CHeroSkinBuyManager.OpenBuyHeroForm(uiEvent.m_srcFormScript, heroId, new stPayInfoSet(), enUIEventID.None);
                    }
                }
            }
        }

        private void OnBuyItem(CUIEvent uiEvent)
        {
            enPayType tag = (enPayType) uiEvent.m_eventParams.tag;
            uint payValue = uiEvent.m_eventParams.commonUInt32Param1;
            string goodName = string.Empty;
            COM_ITEM_TYPE com_item_type = COM_ITEM_TYPE.COM_OBJTYPE_NULL;
            uint uniSkinId = 0;
            long key = 0L;
            if (uiEvent.m_eventParams.heroSkinParam.skinId != 0)
            {
                com_item_type = COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN;
                uniSkinId = CSkinInfo.GetSkinCfgId(uiEvent.m_eventParams.heroSkinParam.heroId, uiEvent.m_eventParams.heroSkinParam.skinId);
                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(uniSkinId);
                DebugHelper.Assert(heroSkin != null, string.Format("找不到皮肤{0}的配置信息", uniSkinId));
                if (heroSkin == null)
                {
                    return;
                }
                goodName = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
            }
            else if (uiEvent.m_eventParams.heroId != 0)
            {
                com_item_type = COM_ITEM_TYPE.COM_OBJTYPE_HERO;
                uniSkinId = uiEvent.m_eventParams.heroId;
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(uniSkinId);
                DebugHelper.Assert(dataByKey != null, string.Format("找不到英雄{0}的配置信息", uniSkinId));
                if (dataByKey == null)
                {
                    return;
                }
                goodName = StringHelper.UTF8BytesToString(ref dataByKey.szName);
            }
            else
            {
                DebugHelper.Assert(false, "神秘商店购买不支持该物品类型");
                return;
            }
            key = GameDataMgr.GetDoubleKey((uint) com_item_type, uniSkinId);
            CMallMysteryProduct product = new CMallMysteryProduct();
            if (!this.m_ProductDic.TryGetValue(key, out product))
            {
                DebugHelper.Assert(false, string.Format("神秘商店找不到该物品{0}/{1}", Enum.GetName(typeof(COM_ITEM_TYPE), com_item_type), uniSkinId));
            }
            else
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = enUIEventID.Mall_Mystery_On_Confirm_Buy_Item;
                uIEvent.m_eventParams.tag = (int) product.ID;
                CMallSystem.TryToPay(enPayPurpose.Buy, goodName, tag, payValue, uIEvent.m_eventID, ref uIEvent.m_eventParams, enUIEventID.None, false, true, false);
            }
        }

        private void OnCloseMysteryShop(CUIEvent uiEvent)
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("神秘商店关闭", false, 1.5f, null, new object[0]);
                this.CloseShop(-1);
                mallForm.Close();
            }
        }

        private void OnConfirmuyItem(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            if (tag < 0)
            {
                DebugHelper.Assert(false, "商品ID不能为0");
                Singleton<CUIManager>.GetInstance().OpenTips("该商品无法购买", false, 1.5f, null, new object[0]);
            }
            else if (this.BoughtCnt >= this.LimitBuyCnt)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("您的购买次数已达到神秘商店限购的次数，欢迎下次再来", false, 1.5f, null, new object[0]);
            }
            else
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x57f);
                msg.stPkgData.stAkaliShopBuyReq.dwShopID = this.ShopID;
                msg.stPkgData.stAkaliShopBuyReq.dwGoodsID = (uint) tag;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private void OnElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((this.m_Products != null) && (srcWidgetIndexInBelongedList <= this.m_Products.Count))
            {
                CMallMysteryProduct product = this.m_Products[srcWidgetIndexInBelongedList];
                if (product != null)
                {
                    product.UpdateView(uiEvent);
                }
            }
        }

        private void OnMallAppear(CUIEvent uiEvent)
        {
            if (((uiEvent.m_srcFormScript != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen) && (Singleton<CMallSystem>.GetInstance().CurTab == CMallSystem.Tab.Mystery))
            {
                this.RefreshBanner();
                this.RefreshProducts();
            }
        }

        private void OnMallTabChange()
        {
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Roll_Discount, new CUIEventManager.OnUIEventHandler(this.OnRollDiscount));
            instance.RemoveUIEventListener(enUIEventID.Mall_Close_Mystery_Shop, new CUIEventManager.OnUIEventHandler(this.OnCloseMysteryShop));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Open_Buy_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyForm));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnBuyItem));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Confirm_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnConfirmuyItem));
            instance.RemoveUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Time_End, new CUIEventManager.OnUIEventHandler(this.OnShopTimeEnd));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Buy_Hero_Not_Own, new CUIEventManager.OnUIEventHandler(this.OnBuyHeroNotOwn));
        }

        private void OnOpenBuyForm(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((this.m_Products != null) && (srcWidgetIndexInBelongedList <= this.m_Products.Count))
            {
                CMallMysteryProduct product = this.m_Products[srcWidgetIndexInBelongedList];
                if (product != null)
                {
                    product.OpenBuy(uiEvent.m_srcFormScript, uiEvent.m_srcWidget.transform);
                }
            }
        }

        private void OnRollDiscount(CUIEvent uiEvent)
        {
            if (!this.IsShopAvailable())
            {
                DebugHelper.Assert(false, "神秘商店未开启");
            }
            else if (this.HasGotDiscount)
            {
                DebugHelper.Assert(false, "随机折扣不能重复获取");
            }
            else
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x581);
                msg.stPkgData.stAkaliShopZheKouReq.dwShopID = this.ShopID;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private void OnShopTimeEnd(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenTips("神秘商店已关闭", false, 1.5f, null, new object[0]);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_CloseForm);
        }

        [MessageHandler(0x583)]
        public static void ReceiveShopBuy(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            uint dwShopID = msg.stPkgData.stAkaliShopUpdate.dwShopID;
            uint dwLeftBuyCnt = msg.stPkgData.stAkaliShopUpdate.dwLeftBuyCnt;
            uint dwGoodsID = msg.stPkgData.stAkaliShopUpdate.dwGoodsID;
            byte bGoodsType = msg.stPkgData.stAkaliShopUpdate.bGoodsType;
            CMallMysteryShop instance = Singleton<CMallMysteryShop>.GetInstance();
            if (instance.ShopID == dwShopID)
            {
                instance.BoughtCnt++;
                CMallMysteryProduct product = instance.GetProduct(bGoodsType, dwGoodsID);
                if (product != null)
                {
                    uint num5;
                    instance.RefreshBanner();
                    product.BoughtCnt = (num5 = product.BoughtCnt) + 1;
                    instance.UpdateProduct(bGoodsType, dwGoodsID, num5);
                }
            }
        }

        [MessageHandler(0x580)]
        public static void ReceiveShopBuyErr(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x580, msg.stPkgData.stAkaliShopError.iErrorCode), false, 1.5f, null, new object[0]);
        }

        private void RefreshBanner()
        {
            this.RefreshDiscount();
            this.RefreshTimer();
        }

        private void RefreshDiscount()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/PL_Content/GetDiscountBtn");
                GameObject obj3 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/PL_Content/BoughtLimit");
                GameObject obj4 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/Discount/DiscountBg");
                GameObject obj5 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/UnkownDiscount");
                if (this.m_HasGotDiscount)
                {
                    obj2.CustomSetActive(false);
                    obj5.CustomSetActive(false);
                    obj3.CustomSetActive(true);
                    obj4.CustomSetActive(true);
                    if (obj4 != null)
                    {
                        Image componetInChild = Utility.GetComponetInChild<Image>(obj4, "Num");
                        if (componetInChild != null)
                        {
                            componetInChild.SetSprite(this.GetDiscountNumIconPath(this.m_Discount), mallForm, true, false, false);
                        }
                    }
                    if (obj3 != null)
                    {
                        Text text = Utility.GetComponetInChild<Text>(obj3, "Cnt");
                        if (text != null)
                        {
                            text.text = string.Format("{0}/{1}", this.BoughtCnt, this.LimitBuyCnt);
                        }
                    }
                }
                else
                {
                    obj2.CustomSetActive(true);
                    obj5.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                }
            }
        }

        private void RefreshProducts()
        {
            <RefreshProducts>c__AnonStorey7C storeyc = new <RefreshProducts>c__AnonStorey7C {
                role = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo()
            };
            DebugHelper.Assert(storeyc.role != null, "master roleInfo is null");
            if (storeyc.role != null)
            {
                if (this.m_Products != null)
                {
                    this.m_Products.Sort(new Comparison<CMallMysteryProduct>(storeyc.<>m__6E));
                }
                CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
                if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
                {
                    CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/List");
                    if ((componetInChild != null) && (this.m_Products != null))
                    {
                        componetInChild.SetElementAmount(this.m_Products.Count);
                    }
                }
            }
        }

        public void RefreshShop(int seq)
        {
            this.m_Inited = false;
            this.InitData();
        }

        private void RefreshTimer()
        {
            CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
            if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
            {
                CUITimerScript componetInChild = Utility.GetComponetInChild<CUITimerScript>(mallForm.gameObject, "pnlBodyBg/pnlMystery/Content/Banner/TimeLeft/Timer");
                int timeToClose = this.GetTimeToClose();
                if (timeToClose > 0x15180)
                {
                    componetInChild.m_timerDisplayType = enTimerDisplayType.D_H_M_S;
                }
                else
                {
                    componetInChild.m_timerDisplayType = enTimerDisplayType.H_M_S;
                }
                componetInChild.SetTotalTime((float) timeToClose);
                componetInChild.StartTimer();
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            this.m_Products.Clear();
            this.m_Products = null;
            this.m_ProductDic.Clear();
            this.m_ProductDic = null;
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_NextRefreshTimerSeq);
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this.m_EndTimerSeq);
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Roll_Discount, new CUIEventManager.OnUIEventHandler(this.OnRollDiscount));
            instance.RemoveUIEventListener(enUIEventID.Mall_Close_Mystery_Shop, new CUIEventManager.OnUIEventHandler(this.OnCloseMysteryShop));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Open_Buy_Form, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyForm));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnBuyItem));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Confirm_Buy_Item, new CUIEventManager.OnUIEventHandler(this.OnConfirmuyItem));
            instance.RemoveUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Time_End, new CUIEventManager.OnUIEventHandler(this.OnShopTimeEnd));
            instance.RemoveUIEventListener(enUIEventID.Mall_Mystery_On_Buy_Hero_Not_Own, new CUIEventManager.OnUIEventHandler(this.OnBuyHeroNotOwn));
        }

        public void UpdateProduct(uint itemType, uint itemID, uint boughtCnt)
        {
            CMallMysteryProduct product = new CMallMysteryProduct();
            if (this.m_ProductDic.TryGetValue(GameDataMgr.GetDoubleKey(itemType, itemID), out product))
            {
                product.BoughtCnt = boughtCnt;
            }
            this.RefreshProducts();
        }

        public uint BoughtCnt
        {
            get
            {
                return this.m_BoughtCnt;
            }
            set
            {
                this.m_BoughtCnt = value;
            }
        }

        public uint Discount
        {
            get
            {
                return this.m_Discount;
            }
            set
            {
                this.m_Discount = value;
                this.m_HasGotDiscount = true;
            }
        }

        public uint EndTime
        {
            get
            {
                return Utility.ToUtcSeconds(this.m_EndTime);
            }
            set
            {
                this.m_EndTime = Utility.ToUtcTime2Local((long) value);
            }
        }

        public bool HasGotDiscount
        {
            get
            {
                return this.m_HasGotDiscount;
            }
        }

        public bool IsRandomDiscount
        {
            get
            {
                return this.m_IsRandomDiscount;
            }
        }

        public uint LimitBuyCnt
        {
            get
            {
                return this.m_LimitBuyCnt;
            }
            set
            {
                this.m_LimitBuyCnt = value;
            }
        }

        public int ProductCnt
        {
            get
            {
                return this.m_Products.Count;
            }
        }

        public uint ShopID
        {
            get
            {
                return this.m_ShopID;
            }
        }

        [CompilerGenerated]
        private sealed class <RefreshProducts>c__AnonStorey7C
        {
            internal CRoleInfo role;

            internal int <>m__6E(CMallMysteryProduct l, CMallMysteryProduct r)
            {
                if (l == null)
                {
                    return -1;
                }
                if (r == null)
                {
                    return 1;
                }
                uint id = 0;
                uint skinId = 0;
                uint itemID = 0;
                uint num4 = 0;
                int num5 = 0;
                if (l.ID < r.ID)
                {
                    num5 = -1;
                }
                else if (l.ID > r.ID)
                {
                    num5 = 1;
                }
                bool flag = false;
                bool flag2 = false;
                switch (l.ItemType)
                {
                    case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                        id = l.ItemID;
                        if (this.role.IsHaveHero(id, false))
                        {
                            flag = true;
                        }
                        break;

                    case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                        CSkinInfo.ResolveHeroSkin(l.ItemID, out id, out skinId);
                        if (this.role.IsHaveHeroSkin(id, skinId, false))
                        {
                            flag = true;
                        }
                        break;
                }
                switch (r.ItemType)
                {
                    case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                        itemID = r.ItemID;
                        if (this.role.IsHaveHero(itemID, false))
                        {
                            flag2 = true;
                        }
                        break;

                    case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                        CSkinInfo.ResolveHeroSkin(r.ItemID, out itemID, out num4);
                        if (this.role.IsHaveHeroSkin(itemID, num4, false))
                        {
                            flag2 = true;
                        }
                        break;
                }
                if (!flag || !flag2)
                {
                    if (flag)
                    {
                        return 1;
                    }
                    if (flag2)
                    {
                        return -1;
                    }
                }
                return num5;
            }
        }
    }
}

