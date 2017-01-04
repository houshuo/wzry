using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Runtime.InteropServices;

public class CMallItem
{
    private string m_firstName;
    private IHeroData m_heroData;
    private string m_iconPath;
    private IconType m_iconType;
    private stPayInfoSet m_payInfoSet;
    private CMallFactoryShopController.ShopProduct m_product;
    private string m_secondName;
    private ResHeroSkin m_skinData;
    private ItemType m_type;
    private CUseable m_useable;

    public CMallItem(CMallFactoryShopController.ShopProduct product, IconType iconType = 1)
    {
        this.m_type = ItemType.Item;
        this.m_iconType = iconType;
        if (product != null)
        {
            this.m_useable = CUseableManager.CreateUseable(product.Type, 0L, product.ID, (int) product.LimitCount, 0);
            this.m_product = product;
            this.m_firstName = this.m_useable.m_name;
            this.m_secondName = null;
            this.m_iconPath = this.m_useable.GetIconPath();
            RES_SHOPBUY_COINTYPE coinType = product.CoinType;
            enPayType type = CMallSystem.ResBuyTypeToPayType((int) coinType);
            uint buyPrice = this.m_useable.GetBuyPrice(coinType);
            uint num2 = product.ConvertWithRealDiscount(buyPrice);
            this.m_payInfoSet = new stPayInfoSet(1);
            this.m_payInfoSet.m_payInfoCount = 1;
            this.m_payInfoSet.m_payInfos[0].m_discountForDisplay = product.DiscountForShow;
            this.m_payInfoSet.m_payInfos[0].m_oriValue = buyPrice;
            this.m_payInfoSet.m_payInfos[0].m_payType = type;
            this.m_payInfoSet.m_payInfos[0].m_payValue = num2;
        }
        else
        {
            this.m_useable = null;
            this.m_firstName = null;
            this.m_secondName = null;
            this.m_useable = null;
            this.m_iconPath = null;
            this.m_payInfoSet = new stPayInfoSet();
        }
    }

    public CMallItem(uint heroID, IconType iconType = 0)
    {
        this.m_type = ItemType.Hero;
        this.m_iconType = iconType;
        this.m_heroData = CHeroDataFactory.CreateHeroData(heroID);
        if (this.m_heroData != null)
        {
            this.m_firstName = this.m_heroData.heroName;
            this.m_secondName = null;
            string str = CUIUtility.s_Sprite_Dynamic_Icon_Dir;
            if (iconType == IconType.Small)
            {
                this.m_useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, heroID, 1);
                if (this.m_useable != null)
                {
                    this.m_iconPath = this.m_useable.GetIconPath();
                }
                else
                {
                    this.m_iconPath = null;
                }
            }
            else if (this.m_heroData.heroCfgInfo != null)
            {
                this.m_iconPath = CUIUtility.s_Sprite_Dynamic_BustHero_Dir + this.m_heroData.heroCfgInfo.szImagePath;
            }
            else
            {
                this.m_iconPath = null;
            }
            ResHeroPromotion resPromotion = this.m_heroData.promotion();
            this.m_payInfoSet = CMallSystem.GetPayInfoSetOfGood(this.m_heroData.heroCfgInfo, resPromotion);
        }
        else
        {
            this.m_useable = null;
            this.m_firstName = null;
            this.m_secondName = null;
            this.m_iconPath = null;
            this.m_payInfoSet = new stPayInfoSet();
        }
    }

    public CMallItem(uint heroID, uint skinID, IconType iconType = 0)
    {
        this.m_type = ItemType.Skin;
        this.m_iconType = iconType;
        this.m_heroData = CHeroDataFactory.CreateHeroData(heroID);
        this.m_skinData = CSkinInfo.GetHeroSkin(heroID, skinID);
        if ((this.m_heroData != null) && (this.m_skinData != null))
        {
            this.m_firstName = this.m_heroData.heroName;
            this.m_secondName = this.m_skinData.szSkinName;
            if (iconType == IconType.Small)
            {
                this.m_useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, this.m_skinData.dwID, 1);
                if (this.m_useable != null)
                {
                    this.m_iconPath = this.m_useable.GetIconPath();
                }
                else
                {
                    this.m_iconPath = null;
                }
            }
            else
            {
                this.m_iconPath = CUIUtility.s_Sprite_Dynamic_BustHero_Dir + this.m_skinData.szSkinPicID;
            }
            ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(heroID, skinID);
            this.m_payInfoSet = CMallSystem.GetPayInfoSetOfGood(this.m_skinData, skinPromotion);
        }
        else
        {
            this.m_useable = null;
            this.m_firstName = null;
            this.m_secondName = null;
            this.m_iconPath = null;
            this.m_payInfoSet = new stPayInfoSet();
        }
    }

    public bool CanSendFriend()
    {
        ItemType type = this.m_type;
        if (type != ItemType.Hero)
        {
            if (type != ItemType.Skin)
            {
                return false;
            }
        }
        else
        {
            if (this.m_heroData == null)
            {
                return false;
            }
            return CHeroSkinBuyManager.ShouldShowBuyForFriend(false, this.m_heroData.cfgID, 0, false);
        }
        if (this.m_skinData == null)
        {
            return false;
        }
        return CHeroSkinBuyManager.ShouldShowBuyForFriend(true, this.m_skinData.dwHeroID, this.m_skinData.dwSkinID, false);
    }

    public string FirstName()
    {
        return this.m_firstName;
    }

    public IconType GetIconType()
    {
        return this.m_iconType;
    }

    public OldPriceType GetOldPriceType()
    {
        switch (this.m_payInfoSet.m_payInfoCount)
        {
            case 0:
                return OldPriceType.None;

            case 1:
                if (this.m_payInfoSet.m_payInfos[0].m_oriValue == this.m_payInfoSet.m_payInfos[0].m_payValue)
                {
                    return OldPriceType.None;
                }
                return OldPriceType.FirstOne;

            case 2:
                if ((this.m_payInfoSet.m_payInfos[0].m_oriValue == this.m_payInfoSet.m_payInfos[0].m_payValue) || (this.m_payInfoSet.m_payInfos[1].m_oriValue == this.m_payInfoSet.m_payInfos[1].m_payValue))
                {
                    if (this.m_payInfoSet.m_payInfos[0].m_oriValue != this.m_payInfoSet.m_payInfos[0].m_payValue)
                    {
                        return OldPriceType.FirstOne;
                    }
                    if (this.m_payInfoSet.m_payInfos[1].m_oriValue != this.m_payInfoSet.m_payInfos[1].m_payValue)
                    {
                        return OldPriceType.SecondOne;
                    }
                    return OldPriceType.None;
                }
                return OldPriceType.Both;
        }
        return OldPriceType.None;
    }

    public int Grade()
    {
        if ((this.m_iconType != IconType.Normal) && (this.m_useable != null))
        {
            return (this.m_useable.m_grade + 1);
        }
        return -1;
    }

    public uint HeroID()
    {
        if (this.m_heroData != null)
        {
            return this.m_heroData.cfgID;
        }
        return 0xa6;
    }

    public string Icon()
    {
        return this.m_iconPath;
    }

    public bool IsValidExperience()
    {
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
        DebugHelper.Assert(masterRoleInfo != null, "Master Role Info Is Null");
        if (masterRoleInfo == null)
        {
            return false;
        }
        ItemType type = this.m_type;
        if (type != ItemType.Hero)
        {
            if (type != ItemType.Skin)
            {
                return false;
            }
        }
        else
        {
            if (this.m_heroData == null)
            {
                return false;
            }
            return masterRoleInfo.IsValidExperienceHero(this.m_heroData.cfgID);
        }
        if (this.m_skinData == null)
        {
            return false;
        }
        return masterRoleInfo.IsValidExperienceSkin(this.m_skinData.dwHeroID, this.m_skinData.dwSkinID);
    }

    public enHeroJobType Job()
    {
        if ((this.m_heroData != null) && (this.m_type == ItemType.Hero))
        {
            return (enHeroJobType) this.m_heroData.heroCfgInfo.bMainJob;
        }
        return enHeroJobType.All;
    }

    public string ObtWay()
    {
        ItemType type = this.m_type;
        if (type != ItemType.Hero)
        {
            if (type != ItemType.Skin)
            {
                return null;
            }
        }
        else
        {
            if (this.m_heroData == null)
            {
                return null;
            }
            ResHeroShop shop = null;
            GameDataMgr.heroShopInfoDict.TryGetValue(this.m_heroData.heroCfgInfo.dwCfgID, out shop);
            return ((shop == null) ? null : shop.szObtWay);
        }
        if (this.m_skinData == null)
        {
            return null;
        }
        ResHeroSkinShop shop2 = null;
        GameDataMgr.skinShopInfoDict.TryGetValue(this.m_skinData.dwID, out shop2);
        return ((shop2 == null) ? null : shop2.szGetPath);
    }

    public byte ObtWayType()
    {
        ItemType type = this.m_type;
        if (type != ItemType.Hero)
        {
            if (type != ItemType.Skin)
            {
                return 0;
            }
        }
        else
        {
            if (this.m_heroData == null)
            {
                return 0;
            }
            ResHeroShop shop = null;
            GameDataMgr.heroShopInfoDict.TryGetValue(this.m_heroData.heroCfgInfo.dwCfgID, out shop);
            return ((shop == null) ? ((byte) 0) : shop.bObtWayType);
        }
        if (this.m_skinData == null)
        {
            return 0;
        }
        ResHeroSkinShop shop2 = null;
        GameDataMgr.skinShopInfoDict.TryGetValue(this.m_skinData.dwID, out shop2);
        return ((shop2 == null) ? ((byte) 0) : shop2.bGetPathType);
    }

    public bool Owned(bool isIncludeValidExperience = false)
    {
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
        DebugHelper.Assert(masterRoleInfo != null, "Master Role Info Is Null");
        if (masterRoleInfo == null)
        {
            return false;
        }
        ItemType type = this.m_type;
        if (type != ItemType.Hero)
        {
            if (type != ItemType.Skin)
            {
                return false;
            }
        }
        else
        {
            if (this.m_heroData == null)
            {
                return false;
            }
            return masterRoleInfo.IsHaveHero(this.m_heroData.cfgID, isIncludeValidExperience);
        }
        if (this.m_skinData == null)
        {
            return false;
        }
        return masterRoleInfo.IsHaveHeroSkin(this.m_skinData.dwHeroID, this.m_skinData.dwSkinID, isIncludeValidExperience);
    }

    public stPayInfoSet PayInfoSet()
    {
        return this.m_payInfoSet;
    }

    public PayBy PayWay()
    {
        switch (this.m_payInfoSet.m_payInfoCount)
        {
            case 0:
                return PayBy.None;

            case 1:
                return PayBy.OnlyOne;

            case 2:
                return PayBy.Both;
        }
        return PayBy.None;
    }

    public int ProductIdx()
    {
        if (this.m_product == null)
        {
            return -1;
        }
        return Singleton<CMallFactoryShopController>.GetInstance().GetProductIndex(this.m_product);
    }

    public string SecondName()
    {
        return this.m_secondName;
    }

    public uint SkinID()
    {
        if (this.m_skinData != null)
        {
            return this.m_skinData.dwSkinID;
        }
        return 0;
    }

    public uint SkinUniqID()
    {
        if (this.m_skinData != null)
        {
            return this.m_skinData.dwID;
        }
        return 0;
    }

    public bool TagInfo(ref string iconPath, ref string text, bool owned = false)
    {
        float num;
        CTextManager instance = Singleton<CTextManager>.GetInstance();
        if (owned)
        {
            iconPath = "UGUI/Sprite/Common/Product_New.prefab";
            text = instance.GetText("Mall_Hero_State_Own");
            return true;
        }
        if (this.Owned(false))
        {
            iconPath = null;
            text = null;
            return false;
        }
        ResHeroPromotion heroPromotion = null;
        ResSkinPromotion skinPromotion = null;
        switch (this.TagType(ref heroPromotion, ref skinPromotion))
        {
            case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NONE:
                iconPath = null;
                text = null;
                return false;

            case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_UNUSUAL:
            {
                int num2 = 0;
                uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
                switch (this.m_type)
                {
                    case ItemType.Hero:
                        if (heroPromotion == null)
                        {
                            iconPath = null;
                            text = null;
                            return false;
                        }
                        if (heroPromotion.dwOnTimeGen > currentUTCTime)
                        {
                            num2 = (int) (heroPromotion.dwOffTimeGen - heroPromotion.dwOnTimeGen);
                        }
                        else
                        {
                            num2 = (int) (heroPromotion.dwOffTimeGen - currentUTCTime);
                        }
                        break;

                    case ItemType.Skin:
                        if (skinPromotion == null)
                        {
                            iconPath = null;
                            text = null;
                            return false;
                        }
                        if (skinPromotion.dwOnTimeGen > currentUTCTime)
                        {
                            num2 = (int) (skinPromotion.dwOffTimeGen - skinPromotion.dwOnTimeGen);
                        }
                        else
                        {
                            num2 = (int) (skinPromotion.dwOffTimeGen - currentUTCTime);
                        }
                        break;
                }
                if (num2 > 0)
                {
                    int num4 = (int) Math.Ceiling(((double) num2) / 86400.0);
                    if (num4 > 0)
                    {
                        iconPath = "UGUI/Sprite/Common/Product_Unusual.prefab";
                        string[] args = new string[] { num4.ToString() };
                        text = Singleton<CTextManager>.GetInstance().GetText("Mall_Promotion_Tag_1", args);
                        return true;
                    }
                    iconPath = null;
                    text = null;
                    return false;
                }
                iconPath = null;
                text = null;
                return false;
            }
            case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NEW:
                iconPath = "UGUI/Sprite/Common/Product_New.prefab";
                text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_New");
                return true;

            case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_HOT:
                iconPath = "UGUI/Sprite/Common/Product_Hot.prefab";
                text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_Hot");
                return true;

            case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_DISCOUNT:
                num = 100f;
                switch (this.m_type)
                {
                    case ItemType.Hero:
                        if (heroPromotion == null)
                        {
                            iconPath = null;
                            text = null;
                            return false;
                        }
                        num = ((float) heroPromotion.dwDiscount) / 10f;
                        goto Label_00E2;

                    case ItemType.Skin:
                        if (skinPromotion == null)
                        {
                            iconPath = null;
                            text = null;
                            return false;
                        }
                        num = ((float) skinPromotion.dwDiscount) / 10f;
                        goto Label_00E2;
                }
                break;

            default:
                iconPath = null;
                text = null;
                return false;
        }
    Label_00E2:
        iconPath = "UGUI/Sprite/Common/Product_Discount.prefab";
        if (Math.Abs((float) (num % 1f)) < float.Epsilon)
        {
            text = string.Format("{0}折", ((int) num).ToString("D"));
        }
        else
        {
            text = string.Format("{0}折", num.ToString("0.0"));
        }
        return true;
    }

    public RES_LUCKYDRAW_ITEMTAG TagType(ref ResHeroPromotion heroPromotion, ref ResSkinPromotion skinPromotion)
    {
        ItemType type = this.m_type;
        if (type != ItemType.Hero)
        {
            if ((type == ItemType.Skin) && (this.m_skinData != null))
            {
                skinPromotion = CSkinInfo.GetSkinPromotion(this.m_skinData.dwID);
                if (skinPromotion == null)
                {
                    return RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NONE;
                }
                return (RES_LUCKYDRAW_ITEMTAG) skinPromotion.bTag;
            }
        }
        else if (this.m_heroData != null)
        {
            heroPromotion = this.m_heroData.promotion();
            if (heroPromotion == null)
            {
                return RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NONE;
            }
            return (RES_LUCKYDRAW_ITEMTAG) heroPromotion.bTag;
        }
        return RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NONE;
    }

    public ItemType Type()
    {
        return this.m_type;
    }

    public enum IconType
    {
        Normal,
        Small
    }

    public enum ItemType
    {
        Hero,
        Skin,
        Item
    }

    public enum OldPriceType
    {
        None,
        FirstOne,
        SecondOne,
        Both
    }

    public enum PayBy
    {
        None,
        OnlyOne,
        Both
    }
}

