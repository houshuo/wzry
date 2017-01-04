namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class BuyPickDialog
    {
        private bool _bDynamicCorrectPrice;
        private bool _bHeroSkinGift;
        private CMallFactoryShopController.ShopProduct _callContext;
        private RES_SHOPBUY_COINTYPE _coinType;
        private CUseable _coinUsb;
        private Text _costText;
        private uint _count;
        private Text _countText;
        private Text _descText;
        private uint _heroSkinGiftCost;
        private uint _maxCount;
        private OnConfirmBuyDelegate _onConfirm;
        private OnConfirmBuyCommonDelegate _onConfirmdCommon;
        private float _realDiscount;
        private GameObject _root;
        private CUIEvent _uieventPars;
        private CUseable _usb;
        public static string s_Form_Path = "UGUI/Form/Common/Form_BuyPick.prefab";
        public static string s_Gift_Form_Path = "UGUI/Form/Common/Form_BuyPick_Gift.prefab";
        private static BuyPickDialog s_theDlg;

        public BuyPickDialog(COM_ITEM_TYPE type, uint id, RES_SHOPBUY_COINTYPE coinType, float discount, uint maxCount, OnConfirmBuyDelegate onConfirm, CMallFactoryShopController.ShopProduct callContext, OnConfirmBuyCommonDelegate onConfirmCommon = null, CUIEvent uieventPars = null)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_Form_Path, false, true);
            if (null != script)
            {
                this._root = script.gameObject;
                this._usb = CUseableManager.CreateUseable(type, id, 0);
                this._bHeroSkinGift = false;
                this._bDynamicCorrectPrice = false;
                this._heroSkinGiftCost = 0;
                this._count = 1;
                this._maxCount = maxCount;
                if (this._maxCount == 0)
                {
                    this._maxCount = 0x3e7;
                }
                this._onConfirm = onConfirm;
                this._callContext = callContext;
                this._onConfirmdCommon = onConfirmCommon;
                this._uieventPars = uieventPars;
                this._coinType = coinType;
                this._realDiscount = discount;
                if (this._usb != null)
                {
                    this._countText = Utility.GetComponetInChild<Text>(this._root, "Panel/Count");
                    this._costText = Utility.GetComponetInChild<Text>(this._root, "Panel/Cost");
                    this._descText = Utility.GetComponetInChild<Text>(this._root, "Panel/Desc/Image/Text");
                    if (this._descText != null)
                    {
                        this._descText.text = this._usb.m_description;
                    }
                    Utility.GetComponetInChild<Image>(this._root, "Panel/Slot/Icon").SetSprite(CUIUtility.GetSpritePrefeb(this._usb.GetIconPath(), false, false));
                    Utility.GetComponetInChild<Text>(this._root, "Panel/Name").text = this._usb.m_name;
                    this._coinUsb = CUseableManager.CreateCoinUseable(coinType, 0);
                    if (this._coinUsb != null)
                    {
                        Utility.GetComponetInChild<Image>(this._root, "Panel/Cost/CoinType").SetSprite(CUIUtility.GetSpritePrefeb(this._coinUsb.GetIconPath(), false, false));
                        Utility.GetComponetInChild<Text>(this._root, "Panel/Price").text = CMallFactoryShopController.ShopProduct.SConvertWithRealDiscount(this._usb.GetBuyPrice(coinType), this._realDiscount).ToString();
                    }
                    Image componetInChild = Utility.GetComponetInChild<Image>(this._root, "Panel/Slot/imgExperienceMark");
                    if (componetInChild != null)
                    {
                        if ((this._usb.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsHeroExperienceCard(this._usb.m_baseID))
                        {
                            componetInChild.gameObject.CustomSetActive(true);
                            componetInChild.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false));
                        }
                        else if ((this._usb.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP) && CItem.IsSkinExperienceCard(this._usb.m_baseID))
                        {
                            componetInChild.gameObject.CustomSetActive(true);
                            componetInChild.SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false));
                        }
                        else
                        {
                            componetInChild.gameObject.CustomSetActive(false);
                        }
                    }
                }
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Add, new CUIEventManager.OnUIEventHandler(this.OnClickAdd));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Dec, new CUIEventManager.OnUIEventHandler(this.OnClickDec));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Max, new CUIEventManager.OnUIEventHandler(this.OnClickMax));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Confirm, new CUIEventManager.OnUIEventHandler(this.OnClickConfirm));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Cancel, new CUIEventManager.OnUIEventHandler(this.OnClickCancel));
                this.ValidateDynamic();
            }
        }

        public BuyPickDialog(bool isGift, COM_ITEM_TYPE type, uint id, RES_SHOPBUY_COINTYPE coinType, float discount, uint maxCount, OnConfirmBuyDelegate onConfirm, CMallFactoryShopController.ShopProduct callContext, OnConfirmBuyCommonDelegate onConfirmCommon = null, CUIEvent uieventPars = null)
        {
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(s_Gift_Form_Path, false, true);
            if (null != formScript)
            {
                this._root = formScript.gameObject;
                this._usb = CUseableManager.CreateUseable(type, id, 0);
                this._count = 1;
                this._bHeroSkinGift = false;
                this._bDynamicCorrectPrice = false;
                this._heroSkinGiftCost = 0;
                this._maxCount = maxCount;
                if (this._maxCount == 0)
                {
                    this._maxCount = 0x3e7;
                }
                this._onConfirm = onConfirm;
                this._callContext = callContext;
                this._onConfirmdCommon = onConfirmCommon;
                this._uieventPars = uieventPars;
                this._coinType = coinType;
                this._realDiscount = discount;
                if (this._usb != null)
                {
                    this._countText = Utility.GetComponetInChild<Text>(this._root, "Panel/Count");
                    this._costText = Utility.GetComponetInChild<Text>(this._root, "Panel/Cost");
                    this._descText = Utility.GetComponetInChild<Text>(this._root, "Panel/lblDesc");
                    CItem item = new CItem(0L, id, 0, 0);
                    uint key = (uint) item.m_itemData.EftParam[0];
                    ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(key);
                    ListView<CUseable> view = new ListView<CUseable>();
                    for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
                    {
                        if (dataByKey.astRewardDetail[i].bItemType != 0)
                        {
                            CUseable useable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE) dataByKey.astRewardDetail[i].bItemType, (int) dataByKey.astRewardDetail[i].dwLowCnt, dataByKey.astRewardDetail[i].dwItemID);
                            if (useable != null)
                            {
                                view.Add(useable);
                            }
                        }
                    }
                    if (this._descText != null)
                    {
                        this._descText.text = item.m_description;
                    }
                    uint num3 = 0;
                    int num4 = 0;
                    if (this._usb.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                    {
                        CItem item2 = (CItem) this._usb;
                        if (((item2 != null) && (item2.m_itemData != null)) && (item2.m_itemData.bType == 4))
                        {
                            this._bDynamicCorrectPrice = item2.m_itemData.EftParam[3] > 0f;
                        }
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        GameObject gameObject = this._root.transform.Find("Panel/itemGroup/itemCell" + j).gameObject;
                        if (j < view.Count)
                        {
                            gameObject.CustomSetActive(true);
                            CUICommonSystem.SetItemCell(formScript, gameObject, view[j], true, false);
                            Transform transform = gameObject.transform.Find("HaveItemFlag");
                            transform.gameObject.CustomSetActive(false);
                            if (view[j].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
                            {
                                this._bHeroSkinGift = true;
                                CHeroItem item3 = view[j] as CHeroItem;
                                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                if ((masterRoleInfo != null) && masterRoleInfo.IsOwnHero(item3.m_heroData.dwCfgID))
                                {
                                    num3 += CHeroInfo.GetHeroCost(item3.m_heroData.dwCfgID, coinType);
                                    num4++;
                                    transform.gameObject.CustomSetActive(true);
                                }
                            }
                            else if (view[j].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
                            {
                                this._bHeroSkinGift = true;
                                CHeroSkin skin = view[j] as CHeroSkin;
                                CRoleInfo info2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                                if ((info2 != null) && info2.IsHaveHeroSkin(skin.m_heroId, skin.m_skinId, false))
                                {
                                    num3 += CSkinInfo.GetHeroSkinCost(skin.m_heroId, skin.m_skinId, coinType);
                                    num4++;
                                    transform.gameObject.CustomSetActive(true);
                                }
                            }
                        }
                        else
                        {
                            gameObject.CustomSetActive(false);
                        }
                    }
                    this._coinUsb = CUseableManager.CreateCoinUseable(coinType, 0);
                    if (this._coinUsb != null)
                    {
                        Utility.GetComponetInChild<Image>(this._root, "Panel/Cost/CoinType").SetSprite(CUIUtility.GetSpritePrefeb(this._coinUsb.GetIconPath(), false, false));
                    }
                    Text componetInChild = Utility.GetComponetInChild<Text>(this._root, "Panel/costDescText");
                    componetInChild.text = string.Empty;
                    if (this._bHeroSkinGift && this._bDynamicCorrectPrice)
                    {
                        uint buyPrice = this._usb.GetBuyPrice(coinType);
                        Button btn = Utility.GetComponetInChild<Button>(this._root, "Panel/Button_Sale");
                        if (num4 >= view.Count)
                        {
                            CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
                            componetInChild.text = Singleton<CTextManager>.GetInstance().GetText("Gift_Can_Not_Buy_Tip");
                            this._heroSkinGiftCost = 0;
                        }
                        else
                        {
                            CUICommonSystem.SetButtonEnableWithShader(btn, true, true);
                            componetInChild.text = Singleton<CTextManager>.GetInstance().GetText("Gift_Own_Hero_Skin_Tip");
                            uint num7 = CMallFactoryShopController.ShopProduct.SConvertWithRealDiscount(buyPrice - num3, this._realDiscount);
                            if ((buyPrice >= num3) && (num7 >= (buyPrice / 10)))
                            {
                                this._heroSkinGiftCost = num7;
                            }
                            else
                            {
                                this._heroSkinGiftCost = buyPrice / 10;
                            }
                        }
                        if (this._callContext != null)
                        {
                            this._callContext.m_bChangeGiftPrice = true;
                            this._callContext.m_newGiftPrice = this._heroSkinGiftCost;
                        }
                    }
                }
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Add, new CUIEventManager.OnUIEventHandler(this.OnClickAdd));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Dec, new CUIEventManager.OnUIEventHandler(this.OnClickDec));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Max, new CUIEventManager.OnUIEventHandler(this.OnClickMax));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Confirm, new CUIEventManager.OnUIEventHandler(this.OnClickConfirm));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_Cancel, new CUIEventManager.OnUIEventHandler(this.OnClickCancel));
                this.ValidateDynamic();
            }
        }

        private void OnClickAdd(CUIEvent uiEvent)
        {
            if (this._count < this._maxCount)
            {
                this._count++;
                this.ValidateDynamic();
            }
        }

        private void OnClickCancel(CUIEvent uiEvent)
        {
            this.OnClose(false);
        }

        private void OnClickConfirm(CUIEvent uiEvent)
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
            {
                this.OnClose(false);
            }
            else
            {
                this.OnClose(true);
            }
        }

        private void OnClickDec(CUIEvent uiEvent)
        {
            if (this._count > 1)
            {
                this._count--;
                this.ValidateDynamic();
            }
        }

        private void OnClickMax(CUIEvent uiEvent)
        {
            if (this._count != this._maxCount)
            {
                this._count = this._maxCount;
                this.ValidateDynamic();
            }
        }

        private void OnClose(bool isOk)
        {
            if (isOk && (this._onConfirm != null))
            {
                this._onConfirm(this._callContext, this._count, false, this._uieventPars);
            }
            else if (isOk && (this._onConfirmdCommon != null))
            {
                this._onConfirmdCommon(this._uieventPars, this._count);
            }
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Add, new CUIEventManager.OnUIEventHandler(this.OnClickAdd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Dec, new CUIEventManager.OnUIEventHandler(this.OnClickDec));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Max, new CUIEventManager.OnUIEventHandler(this.OnClickMax));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Confirm, new CUIEventManager.OnUIEventHandler(this.OnClickConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_Cancel, new CUIEventManager.OnUIEventHandler(this.OnClickCancel));
            Singleton<CUIManager>.GetInstance().CloseForm(s_Form_Path);
            Singleton<CUIManager>.GetInstance().CloseForm(s_Gift_Form_Path);
            s_theDlg = null;
        }

        public static void Show(COM_ITEM_TYPE type, uint id, RES_SHOPBUY_COINTYPE coinType, float discount, uint maxCount, OnConfirmBuyDelegate onClose, CMallFactoryShopController.ShopProduct callContext = null, OnConfirmBuyCommonDelegate onConfirmCommon = null, CUIEvent uieventPars = null)
        {
            if (s_theDlg == null)
            {
                if (type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
                {
                    CItem item = new CItem(0L, id, 0, 0);
                    if (item.m_itemData.bIsView != 0)
                    {
                        s_theDlg = new BuyPickDialog(true, type, id, coinType, discount, maxCount, onClose, callContext, onConfirmCommon, uieventPars);
                    }
                    else
                    {
                        s_theDlg = new BuyPickDialog(type, id, coinType, discount, maxCount, onClose, callContext, onConfirmCommon, uieventPars);
                    }
                }
                else
                {
                    s_theDlg = new BuyPickDialog(type, id, coinType, discount, maxCount, onClose, callContext, onConfirmCommon, uieventPars);
                }
                if (s_theDlg._root == null)
                {
                    s_theDlg = null;
                }
            }
        }

        private void ValidateDynamic()
        {
            if (this._usb != null)
            {
                this._countText.text = this._count.ToString();
                if (this._bHeroSkinGift && this._bDynamicCorrectPrice)
                {
                    this._costText.text = (this._count * this._heroSkinGiftCost).ToString();
                }
                else
                {
                    this._costText.text = CMallFactoryShopController.ShopProduct.SConvertWithRealDiscount(this._count * this._usb.GetBuyPrice(this._coinType), this._realDiscount).ToString();
                }
            }
        }

        public delegate void OnConfirmBuyCommonDelegate(CUIEvent uievent, uint count);

        public delegate void OnConfirmBuyDelegate(CMallFactoryShopController.ShopProduct shopProduct, uint count, bool needConfirm, CUIEvent uiEvent);
    }
}

