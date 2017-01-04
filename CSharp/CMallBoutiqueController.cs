using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

[MessageHandlerClass]
public class CMallBoutiqueController : Singleton<CMallBoutiqueController>
{
    private ListView<ResBoutiqueConf> m_HotSaleListView;
    private ListView<ResBoutiqueConf> m_NewArrivalListView;

    public void Draw(CUIFormScript form)
    {
        this.InitElements();
        if (!this.RefreshData())
        {
            Singleton<CUIManager>.GetInstance().OpenTips("Initializing data failed", false, 1.5f, null, new object[0]);
            Singleton<CUIManager>.GetInstance().CloseForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
        }
        else
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Boutique_New_Arrival_Enable, new CUIEventManager.OnUIEventHandler(this.OnNewArrivalEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Boutique_Hot_Sale_Enable, new CUIEventManager.OnUIEventHandler(this.OnHotSaleEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Click, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Confirm_Buy, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductConfirmBuy));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Mall_Change_Tab, new System.Action(this.OnMallTabChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Bought_Success, new Action<CMallFactoryShopController.ShopProduct>(this.OnFactoryProductBought));
            Singleton<EventRouter>.GetInstance().AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.GetInstance().AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
            this.InitBannerCtrl();
            this.RefreshNewArrivals();
            this.RefreshHotSale();
        }
    }

    public override void Init()
    {
        base.Init();
        this.m_NewArrivalListView = new ListView<ResBoutiqueConf>();
        this.m_HotSaleListView = new ListView<ResBoutiqueConf>();
    }

    public void InitBannerCtrl()
    {
        CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
        if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
        {
            GameObject obj2 = Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlBoutique/Left/Top/StepList");
            if (obj2 == null)
            {
                DebugHelper.Assert(false, "banner img object is null");
            }
            else
            {
                BannerImageCtrl component = obj2.GetComponent<BannerImageCtrl>();
                if (component == null)
                {
                    DebugHelper.Assert(false, "banner img ctrl is null");
                }
                else
                {
                    component.InitSys();
                }
            }
        }
    }

    public void InitElements()
    {
        CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
        if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
        {
            Utility.FindChild(mallForm.gameObject, "pnlBodyBg/pnlBoutique").CustomSetActive(true);
        }
    }

    public void Load(CUIFormScript form)
    {
        CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/Boutique", "pnlBoutique", form.GetWidget(3), form);
    }

    public bool Loaded(CUIFormScript form)
    {
        if (Utility.FindChild(form.GetWidget(3), "pnlBoutique") == null)
        {
            return false;
        }
        return true;
    }

    private void OnFactoryProductBought(CMallFactoryShopController.ShopProduct shopProduct)
    {
        if (shopProduct.IsOnSale != 1)
        {
            this.RefreshData();
            this.RefreshHotSale();
        }
    }

    public void OnFactoryProductClick(CUIEvent uiEvent)
    {
        ListView<CMallFactoryShopController.ShopProduct> products = Singleton<CMallFactoryShopController>.GetInstance().GetProducts();
        if (products == null)
        {
            stUIEventParams par = new stUIEventParams {
                tag = 1
            };
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_Open_Factory_Shop_Tab, par);
        }
        else if ((uiEvent.m_eventParams.tag < 0) || (uiEvent.m_eventParams.tag >= products.Count))
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBox("internal error: out of index range.", false);
        }
        else
        {
            CMallFactoryShopController.ShopProduct shopProduct = products[uiEvent.m_eventParams.tag];
            if (shopProduct.CanBuy())
            {
                if ((shopProduct.Type == COM_ITEM_TYPE.COM_OBJTYPE_HERO) || (shopProduct.Type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN))
                {
                    CUIEvent event2 = new CUIEvent {
                        m_eventID = enUIEventID.Mall_Boutique_Factory_Product_Confirm_Buy
                    };
                    event2.m_eventParams.commonUInt64Param1 = shopProduct.Key;
                    event2.m_eventParams.commonUInt32Param1 = 1;
                    Singleton<CMallFactoryShopController>.GetInstance().BuyShopProduct(shopProduct, 1, false, event2);
                }
                else
                {
                    CUseable useableByBaseID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableByBaseID(shopProduct.Type, shopProduct.ID);
                    uint maxCount = 0;
                    if (shopProduct.LimitCount > 0)
                    {
                        maxCount = shopProduct.LimitCount - shopProduct.BoughtCount;
                    }
                    if (useableByBaseID != null)
                    {
                        uint num2 = (uint) (useableByBaseID.m_stackMax - useableByBaseID.m_stackCount);
                        if ((num2 < maxCount) || (maxCount == 0))
                        {
                            maxCount = num2;
                        }
                    }
                    CUIEvent uieventPars = new CUIEvent {
                        m_eventID = enUIEventID.Mall_Boutique_Factory_Product_Confirm_Buy
                    };
                    uieventPars.m_eventParams.commonUInt64Param1 = shopProduct.Key;
                    BuyPickDialog.Show(shopProduct.Type, shopProduct.ID, shopProduct.CoinType, shopProduct.RealDiscount, maxCount, new BuyPickDialog.OnConfirmBuyDelegate(Singleton<CMallFactoryShopController>.GetInstance().BuyShopProduct), shopProduct, null, uieventPars);
                }
            }
        }
    }

    public void OnFactoryProductConfirmBuy(CUIEvent uiEvent)
    {
        uint key = (uint) uiEvent.m_eventParams.commonUInt64Param1;
        uint count = uiEvent.m_eventParams.commonUInt32Param1;
        CMallFactoryShopController.ShopProduct shopProduct = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(key);
        if (shopProduct != null)
        {
            Singleton<CMallFactoryShopController>.GetInstance().RequestBuy(shopProduct, count);
        }
    }

    private void OnHotSaleEnable(CUIEvent uiEvent)
    {
        int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
        if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_HotSaleListView.Count))
        {
            GameObject srcWidget = uiEvent.m_srcWidget;
            if (srcWidget != null)
            {
                CMallItemWidget component = srcWidget.GetComponent<CMallItemWidget>();
                if (component != null)
                {
                    ResBoutiqueConf conf = this.m_HotSaleListView[srcWidgetIndexInBelongedList];
                    DebugHelper.Assert(conf != null, "hot sale cfg is null");
                    if (conf != null)
                    {
                        switch (conf.wItemType)
                        {
                            case 2:
                            {
                                CMallItem item2 = new CMallItem(Singleton<CMallFactoryShopController>.GetInstance().GetProduct(conf.dwItemID), CMallItem.IconType.Small);
                                Singleton<CMallSystem>.GetInstance().SetMallItem(component, item2);
                                break;
                            }
                            case 4:
                            {
                                CMallItem item = new CMallItem(conf.dwItemID, CMallItem.IconType.Small);
                                Singleton<CMallSystem>.GetInstance().SetMallItem(component, item);
                                break;
                            }
                            case 7:
                            {
                                uint heroId = 0;
                                uint skinId = 0;
                                CSkinInfo.ResolveHeroSkin(conf.dwItemID, out heroId, out skinId);
                                CMallItem item3 = new CMallItem(heroId, skinId, CMallItem.IconType.Small);
                                Singleton<CMallSystem>.GetInstance().SetMallItem(component, item3);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnMallTabChange()
    {
        if ((Singleton<CMallSystem>.GetInstance().m_MallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_New_Arrival_Enable, new CUIEventManager.OnUIEventHandler(this.OnNewArrivalEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Hot_Sale_Enable, new CUIEventManager.OnUIEventHandler(this.OnHotSaleEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Click, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Confirm_Buy, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductConfirmBuy));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Mall_Change_Tab, new System.Action(this.OnMallTabChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Bought_Success, new Action<CMallFactoryShopController.ShopProduct>(this.OnFactoryProductBought));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
        }
    }

    private void OnNewArrivalEnable(CUIEvent uiEvent)
    {
        int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
        if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_NewArrivalListView.Count))
        {
            GameObject srcWidget = uiEvent.m_srcWidget;
            if (srcWidget != null)
            {
                CMallItemWidget component = srcWidget.GetComponent<CMallItemWidget>();
                if (component != null)
                {
                    ResBoutiqueConf conf = this.m_NewArrivalListView[srcWidgetIndexInBelongedList];
                    DebugHelper.Assert(conf != null, "new arrival cfg is null");
                    if (conf != null)
                    {
                        switch (conf.wItemType)
                        {
                            case 4:
                            {
                                CMallItem item = new CMallItem(conf.dwItemID, CMallItem.IconType.Normal);
                                Singleton<CMallSystem>.GetInstance().SetMallItem(component, item);
                                break;
                            }
                            case 7:
                            {
                                uint heroId = 0;
                                uint skinId = 0;
                                CSkinInfo.ResolveHeroSkin(conf.dwItemID, out heroId, out skinId);
                                CMallItem item2 = new CMallItem(heroId, skinId, CMallItem.IconType.Normal);
                                Singleton<CMallSystem>.GetInstance().SetMallItem(component, item2);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnNtyAddHero(uint id)
    {
        this.RefreshData();
        this.RefreshHotSale();
        this.RefreshNewArrivals();
    }

    private void OnNtyAddSkin(uint heroId, uint skinId, uint addReason)
    {
        this.RefreshData();
        this.RefreshHotSale();
        this.RefreshNewArrivals();
    }

    private bool RefreshData()
    {
        this.m_HotSaleListView.Clear();
        this.m_NewArrivalListView.Clear();
        DictionaryView<uint, ResBoutiqueConf>.Enumerator enumerator = GameDataMgr.boutiqueDict.GetEnumerator();
        int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
        while (enumerator.MoveNext())
        {
            ListView<ResBoutiqueConf> newArrivalListView;
            ResHeroCfgInfo dataByKey;
            ResHeroSkin heroSkin;
            ResHeroCfgInfo info2;
            KeyValuePair<uint, ResBoutiqueConf> current = enumerator.Current;
            ResBoutiqueConf item = current.Value;
            if ((item.dwOnTimeGen <= currentUTCTime) && (item.dwOffTimeGen >= currentUTCTime))
            {
                RES_BOUTIQUE_TYPE bBoutiqueType = (RES_BOUTIQUE_TYPE) item.bBoutiqueType;
                newArrivalListView = null;
                switch (bBoutiqueType)
                {
                    case RES_BOUTIQUE_TYPE.RES_BOUTIQUE_TYPE_NEW_ARRIVAL:
                        newArrivalListView = this.m_NewArrivalListView;
                        break;

                    case RES_BOUTIQUE_TYPE.RES_BOUTIQUE_TYPE_HOT_SALE:
                        newArrivalListView = this.m_HotSaleListView;
                        break;
                }
                switch (item.wItemType)
                {
                    case 2:
                    case 5:
                    {
                        CMallFactoryShopController.ShopProduct product = Singleton<CMallFactoryShopController>.GetInstance().GetProduct(item.dwItemID);
                        if (((product != null) && (product.IsOnSale == 1)) && (newArrivalListView != null))
                        {
                            newArrivalListView.Add(item);
                        }
                        break;
                    }
                    case 4:
                        dataByKey = GameDataMgr.heroDatabin.GetDataByKey(item.dwItemID);
                        if (dataByKey != null)
                        {
                            goto Label_011F;
                        }
                        break;

                    case 7:
                        heroSkin = CSkinInfo.GetHeroSkin(item.dwItemID);
                        if (heroSkin != null)
                        {
                            goto Label_015D;
                        }
                        break;
                }
            }
            continue;
        Label_011F:
            if (GameDataMgr.IsHeroAvailable(dataByKey.dwCfgID) && (newArrivalListView != null))
            {
                newArrivalListView.Add(item);
            }
            continue;
        Label_015D:
            info2 = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
            if ((info2 != null) && ((GameDataMgr.IsSkinAvailable(heroSkin.dwID) && GameDataMgr.IsHeroAvailable(info2.dwCfgID)) && (newArrivalListView != null)))
            {
                newArrivalListView.Add(item);
            }
        }
        return true;
    }

    public void RefreshHotSale()
    {
        CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
        if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
        {
            Utility.GetComponetInChild<CUIListScript>(mallForm.gameObject, "pnlBodyBg/pnlBoutique/Right/Hot").SetElementAmount(this.m_HotSaleListView.Count);
        }
    }

    public void RefreshNewArrivals()
    {
        CUIFormScript mallForm = Singleton<CMallSystem>.GetInstance().m_MallForm;
        if ((mallForm != null) && Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen)
        {
            Utility.GetComponetInChild<CUIListScript>(mallForm.gameObject, "pnlBodyBg/pnlBoutique/Left/Bottom/NewArrivals").SetElementAmount(this.m_NewArrivalListView.Count);
        }
    }

    public override void UnInit()
    {
        base.UnInit();
        this.m_NewArrivalListView = null;
        this.m_HotSaleListView = null;
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_New_Arrival_Enable, new CUIEventManager.OnUIEventHandler(this.OnNewArrivalEnable));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Hot_Sale_Enable, new CUIEventManager.OnUIEventHandler(this.OnHotSaleEnable));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Click, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductClick));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Boutique_Factory_Product_Confirm_Buy, new CUIEventManager.OnUIEventHandler(this.OnFactoryProductConfirmBuy));
        Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Mall_Change_Tab, new System.Action(this.OnMallTabChange));
        Singleton<EventRouter>.GetInstance().RemoveEventHandler<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Bought_Success, new Action<CMallFactoryShopController.ShopProduct>(this.OnFactoryProductBought));
        Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
        Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyAddSkin));
    }
}

