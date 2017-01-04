namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class CMallMysteryProduct
    {
        private uint m_BoughtCnt;
        private ResAkaliShopGoods m_Cfg;
        private uint m_ID;
        private uint m_itemID;
        private COM_ITEM_TYPE m_ItemType;

        public CMallMysteryProduct()
        {
            this.m_ID = 0;
            this.m_itemID = 0;
            this.m_ItemType = COM_ITEM_TYPE.COM_OBJTYPE_NULL;
            this.m_BoughtCnt = 0;
            this.m_Cfg = null;
        }

        public CMallMysteryProduct(ResAkaliShopGoods goodsCfg)
        {
            this.m_ID = goodsCfg.dwID;
            this.m_itemID = goodsCfg.dwGoodsID;
            this.m_ItemType = (COM_ITEM_TYPE) goodsCfg.bGoodsType;
            this.m_BoughtCnt = 0;
            this.m_Cfg = goodsCfg;
        }

        public void OpenBuy(CUIFormScript form, Transform srcTrans)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "master roleInfo is null");
            if (masterRoleInfo != null)
            {
                switch (this.m_ItemType)
                {
                    case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                    {
                        ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_itemID);
                        DebugHelper.Assert(dataByKey != null, "神秘商店配置的英雄ID有错，英雄表里不存在");
                        if (dataByKey != null)
                        {
                            if (masterRoleInfo.IsHaveHero(dataByKey.dwCfgID, false))
                            {
                                stUIEventParams par = new stUIEventParams();
                                par.openHeroFormPar.heroId = dataByKey.dwCfgID;
                                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_OpenForm, par);
                                return;
                            }
                            stPayInfoSet lowestPayInfoSetOfGood = CMallSystem.GetLowestPayInfoSetOfGood(dataByKey, CMallSystem.ResBuyTypeToPayType(this.Cfg.bMoneyType));
                            CHeroSkinBuyManager.OpenBuyHeroForm(form, dataByKey.dwCfgID, lowestPayInfoSetOfGood, enUIEventID.Mall_Mystery_On_Buy_Item);
                            break;
                        }
                        return;
                    }
                    case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                    {
                        ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(this.m_itemID);
                        DebugHelper.Assert(heroSkin != null, "神秘商店配置的皮肤ID有错，皮肤表里不存在");
                        if (heroSkin != null)
                        {
                            ResHeroCfgInfo info3 = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
                            DebugHelper.Assert(info3 != null, "神秘商店配置的皮肤ID有错，皮肤对应的英雄不存在");
                            if (info3 == null)
                            {
                                return;
                            }
                            if (masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
                            {
                                stUIEventParams params2 = new stUIEventParams();
                                params2.openHeroFormPar.heroId = heroSkin.dwHeroID;
                                params2.openHeroFormPar.skinId = heroSkin.dwSkinID;
                                params2.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_OpenForm, params2);
                                return;
                            }
                            if (masterRoleInfo.IsCanBuySkinButNotHaveHero(heroSkin.dwHeroID, heroSkin.dwSkinID))
                            {
                                stUIEventParams params3 = new stUIEventParams {
                                    heroId = heroSkin.dwHeroID
                                };
                                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format("暂未拥有英雄{0}，是否购买", StringHelper.UTF8BytesToString(ref info3.szName)), enUIEventID.Mall_Mystery_On_Buy_Hero_Not_Own, enUIEventID.None, params3, false);
                                return;
                            }
                            stPayInfoSet payInfoSet = CMallSystem.GetLowestPayInfoSetOfGood(heroSkin, CMallSystem.ResBuyTypeToPayType(this.Cfg.bMoneyType));
                            CHeroSkinBuyManager.OpenBuyHeroSkinForm(heroSkin.dwHeroID, heroSkin.dwSkinID, true, payInfoSet, enUIEventID.Mall_Mystery_On_Buy_Item);
                            break;
                        }
                        return;
                    }
                }
            }
        }

        private void UpdateItemPricePnl(CUIFormScript form, Transform pricePnlTrans, Transform tagTrans, ref stPayInfoSet payInfoSet)
        {
            GameObject obj2 = Utility.FindChild(pricePnlTrans.gameObject, "oldPricePanel");
            Text componetInChild = Utility.GetComponetInChild<Text>(pricePnlTrans.gameObject, "oldPricePanel/oldPriceText");
            Image image = Utility.GetComponetInChild<Image>(pricePnlTrans.gameObject, "newPricePanel/costImage");
            Text text2 = Utility.GetComponetInChild<Text>(pricePnlTrans.gameObject, "newPricePanel/newCostText");
            Text text3 = Utility.GetComponetInChild<Text>(tagTrans.gameObject, "Text");
            tagTrans.gameObject.CustomSetActive(false);
            obj2.CustomSetActive(false);
            for (int i = 0; i < payInfoSet.m_payInfoCount; i++)
            {
                stPayInfo info = payInfoSet.m_payInfos[i];
                if (!Singleton<CMallMysteryShop>.GetInstance().HasGotDiscount)
                {
                    tagTrans.gameObject.CustomSetActive(true);
                    text3.text = "?折";
                    text2.text = info.m_payValue.ToString();
                    image.SetSprite(CMallSystem.GetPayTypeIconPath(info.m_payType), form, true, false, false);
                }
                else if (Singleton<CMallMysteryShop>.GetInstance().HasGotDiscount && ((info.m_discountForDisplay == 0x2710) || (info.m_oriValue == info.m_payValue)))
                {
                    text2.text = info.m_payValue.ToString();
                    image.SetSprite(CMallSystem.GetPayTypeIconPath(info.m_payType), form, true, false, false);
                }
                else
                {
                    tagTrans.gameObject.CustomSetActive(true);
                    float num2 = ((float) info.m_discountForDisplay) / 1000f;
                    if (Math.Abs((float) (num2 % 1f)) < float.Epsilon)
                    {
                        text3.text = string.Format("{0}折", ((int) num2).ToString("D"));
                    }
                    else
                    {
                        text3.text = string.Format("{0}折", num2.ToString("0.0"));
                    }
                    obj2.CustomSetActive(true);
                    componetInChild.text = info.m_oriValue.ToString();
                    text2.text = info.m_payValue.ToString();
                    image.SetSprite(CMallSystem.GetPayTypeIconPath(info.m_payType), form, true, false, false);
                }
            }
        }

        public void UpdateView(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if ((srcFormScript != null) && (srcWidget != null))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(masterRoleInfo != null, "master roleInfo is null");
                if (masterRoleInfo != null)
                {
                    GameObject p = Utility.FindChild(srcWidget, "heroItem");
                    if (p == null)
                    {
                        DebugHelper.Assert(p != null, "hero item is null");
                    }
                    else
                    {
                        Text componetInChild = Utility.GetComponetInChild<Text>(p, "heroDataPanel/heroNamePanel/heroNameText");
                        GameObject obj4 = Utility.FindChild(p, "heroDataPanel/heroNamePanel/heroSkinText");
                        if (obj4 != null)
                        {
                            Text component = obj4.GetComponent<Text>();
                            GameObject obj5 = Utility.FindChild(p, "tag");
                            if (obj5 != null)
                            {
                                GameObject obj6 = Utility.FindChild(p, "profession");
                                if (obj6 != null)
                                {
                                    GameObject obj7 = Utility.FindChild(srcWidget, "imgExperienceMark");
                                    if (obj7 != null)
                                    {
                                        GameObject obj8 = Utility.FindChild(p, "skinLabelImage");
                                        if (obj8 != null)
                                        {
                                            GameObject obj9 = Utility.FindChild(p, "heroDataPanel/heroPricePanel");
                                            if (obj9 != null)
                                            {
                                                obj9.CustomSetActive(false);
                                                GameObject obj10 = Utility.FindChild(srcWidget, "ButtonGroup/BuyBtn");
                                                if (obj10 != null)
                                                {
                                                    obj10.CustomSetActive(false);
                                                    Text text3 = Utility.GetComponetInChild<Text>(obj10, "Text");
                                                    Button button = obj10.GetComponent<Button>();
                                                    if (button != null)
                                                    {
                                                        CUIEventScript script2 = obj10.GetComponent<CUIEventScript>();
                                                        if (script2 != null)
                                                        {
                                                            script2.enabled = false;
                                                            button.enabled = false;
                                                            GameObject obj11 = Utility.FindChild(srcWidget, "ButtonGroup/LinkBtn");
                                                            if (obj11 != null)
                                                            {
                                                                obj11.CustomSetActive(false);
                                                                Text text4 = Utility.GetComponetInChild<Text>(obj11, "Text");
                                                                Button button2 = obj11.GetComponent<Button>();
                                                                if (button2 != null)
                                                                {
                                                                    CUIEventScript script3 = obj11.GetComponent<CUIEventScript>();
                                                                    if (script3 != null)
                                                                    {
                                                                        script3.enabled = false;
                                                                        button2.enabled = false;
                                                                        switch (this.m_ItemType)
                                                                        {
                                                                            case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
                                                                            {
                                                                                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_itemID);
                                                                                DebugHelper.Assert(dataByKey != null, "神秘商店配置的英雄ID有错，英雄表里不存在");
                                                                                if (dataByKey != null)
                                                                                {
                                                                                    ResHeroShop shop = null;
                                                                                    GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out shop);
                                                                                    CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, p, StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), enHeroHeadType.enBust, false);
                                                                                    obj6.CustomSetActive(false);
                                                                                    obj8.CustomSetActive(false);
                                                                                    obj4.CustomSetActive(false);
                                                                                    if (componetInChild != null)
                                                                                    {
                                                                                        componetInChild.text = StringHelper.UTF8BytesToString(ref dataByKey.szName);
                                                                                    }
                                                                                    if (masterRoleInfo.IsHaveHero(dataByKey.dwCfgID, false))
                                                                                    {
                                                                                        obj10.CustomSetActive(true);
                                                                                        text3.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Hero_State_Own");
                                                                                        obj5.CustomSetActive(false);
                                                                                        obj7.CustomSetActive(false);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        obj7.CustomSetActive(masterRoleInfo.IsValidExperienceHero(dataByKey.dwCfgID));
                                                                                        stPayInfoSet lowestPayInfoSetOfGood = CMallSystem.GetLowestPayInfoSetOfGood(dataByKey, CMallSystem.ResBuyTypeToPayType(this.Cfg.bMoneyType));
                                                                                        if (lowestPayInfoSetOfGood.m_payInfoCount == 0)
                                                                                        {
                                                                                            obj11.CustomSetActive(true);
                                                                                            if (shop != null)
                                                                                            {
                                                                                                text4.text = StringHelper.UTF8BytesToString(ref shop.szObtWay);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                text4.text = null;
                                                                                            }
                                                                                            if ((shop != null) && (shop.bObtWayType > 0))
                                                                                            {
                                                                                                script3.enabled = true;
                                                                                                button2.enabled = true;
                                                                                                stUIEventParams eventParams = new stUIEventParams {
                                                                                                    tag = shop.bObtWayType
                                                                                                };
                                                                                                script3.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Jump_Form, eventParams);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                script3.enabled = false;
                                                                                                button2.enabled = false;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            obj9.CustomSetActive(true);
                                                                                            obj10.CustomSetActive(true);
                                                                                            text3.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Buy");
                                                                                            script2.enabled = true;
                                                                                            button.enabled = true;
                                                                                            this.UpdateItemPricePnl(srcFormScript, obj9.transform, obj5.transform, ref lowestPayInfoSetOfGood);
                                                                                            stUIEventParams params2 = new stUIEventParams {
                                                                                                tag = uiEvent.m_srcWidgetIndexInBelongedList
                                                                                            };
                                                                                            script2.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Mystery_On_Open_Buy_Form, params2);
                                                                                        }
                                                                                    }
                                                                                    break;
                                                                                }
                                                                                return;
                                                                            }
                                                                            case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
                                                                            {
                                                                                ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(this.m_itemID);
                                                                                DebugHelper.Assert(heroSkin != null, "神秘商店配置的皮肤ID有错，皮肤表里不存在");
                                                                                if (heroSkin != null)
                                                                                {
                                                                                    ResHeroSkinShop shop2 = null;
                                                                                    GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwSkinID, out shop2);
                                                                                    ResHeroCfgInfo info3 = GameDataMgr.heroDatabin.GetDataByKey(heroSkin.dwHeroID);
                                                                                    DebugHelper.Assert(info3 != null, "神秘商店配置的皮肤ID有错，皮肤对应的英雄不存在");
                                                                                    if (info3 == null)
                                                                                    {
                                                                                        return;
                                                                                    }
                                                                                    CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, p.gameObject, heroSkin.szSkinPicID, enHeroHeadType.enBust, false);
                                                                                    obj6.CustomSetActive(false);
                                                                                    CUICommonSystem.SetHeroSkinLabelPic(uiEvent.m_srcFormScript, obj8, heroSkin.dwHeroID, heroSkin.dwSkinID);
                                                                                    obj4.CustomSetActive(true);
                                                                                    if (componetInChild != null)
                                                                                    {
                                                                                        componetInChild.text = StringHelper.UTF8BytesToString(ref info3.szName);
                                                                                    }
                                                                                    if (component != null)
                                                                                    {
                                                                                        component.text = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
                                                                                    }
                                                                                    if (masterRoleInfo.IsHaveHeroSkin(heroSkin.dwHeroID, heroSkin.dwSkinID, false))
                                                                                    {
                                                                                        obj10.CustomSetActive(true);
                                                                                        text3.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Own");
                                                                                        obj5.CustomSetActive(false);
                                                                                        obj7.CustomSetActive(false);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        obj7.CustomSetActive(masterRoleInfo.IsValidExperienceSkin(heroSkin.dwHeroID, heroSkin.dwSkinID));
                                                                                        obj9.CustomSetActive(true);
                                                                                        stPayInfoSet payInfoSet = CMallSystem.GetLowestPayInfoSetOfGood(heroSkin, CMallSystem.ResBuyTypeToPayType(this.Cfg.bMoneyType));
                                                                                        if (payInfoSet.m_payInfoCount == 0)
                                                                                        {
                                                                                            obj11.CustomSetActive(true);
                                                                                            if (shop2 != null)
                                                                                            {
                                                                                                text4.text = StringHelper.UTF8BytesToString(ref shop2.szGetPath);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            obj11.CustomSetActive(false);
                                                                                            this.UpdateItemPricePnl(srcFormScript, obj9.transform, obj5.transform, ref payInfoSet);
                                                                                        }
                                                                                        if (masterRoleInfo.IsCanBuySkinButNotHaveHero(heroSkin.dwHeroID, heroSkin.dwSkinID))
                                                                                        {
                                                                                            obj10.CustomSetActive(true);
                                                                                            script2.enabled = true;
                                                                                            text3.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Skin_State_Buy_hero");
                                                                                            button.enabled = true;
                                                                                            stUIEventParams params3 = new stUIEventParams();
                                                                                            params3.openHeroFormPar.heroId = heroSkin.dwHeroID;
                                                                                            params3.openHeroFormPar.skinId = heroSkin.dwSkinID;
                                                                                            params3.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
                                                                                            script2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenForm, params3);
                                                                                            if (payInfoSet.m_payInfoCount > 0)
                                                                                            {
                                                                                                this.UpdateItemPricePnl(srcFormScript, obj9.transform, obj5.transform, ref payInfoSet);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            obj11.CustomSetActive(false);
                                                                                            obj10.CustomSetActive(true);
                                                                                            text3.text = Singleton<CTextManager>.GetInstance().GetText("Mall_Buy");
                                                                                            script2.enabled = true;
                                                                                            button.enabled = true;
                                                                                            stUIEventParams params4 = new stUIEventParams {
                                                                                                tag = uiEvent.m_srcWidgetIndexInBelongedList
                                                                                            };
                                                                                            script2.SetUIEvent(enUIEventType.Click, enUIEventID.Mall_Mystery_On_Open_Buy_Form, params4);
                                                                                        }
                                                                                    }
                                                                                    break;
                                                                                }
                                                                                return;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
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

        public ResAkaliShopGoods Cfg
        {
            get
            {
                return this.m_Cfg;
            }
        }

        public uint ID
        {
            get
            {
                return this.m_ID;
            }
        }

        public uint ItemID
        {
            get
            {
                return this.m_itemID;
            }
        }

        public COM_ITEM_TYPE ItemType
        {
            get
            {
                return this.m_ItemType;
            }
        }
    }
}

