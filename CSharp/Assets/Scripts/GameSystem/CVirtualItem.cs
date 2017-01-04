namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;

    public class CVirtualItem : CUseable
    {
        public enVirtualItemType m_virtualType;

        public CVirtualItem(enVirtualItemType bType, int bCount)
        {
            this.m_virtualType = bType;
            base.m_stackCount = bCount;
            if (bType == enVirtualItemType.enExp)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("Experience");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_EXP");
                base.m_grade = 0;
            }
            else if (bType == enVirtualItemType.enNoUsed)
            {
                base.m_name = string.Empty;
                base.m_description = string.Empty;
                base.m_grade = 2;
            }
            else if (bType == enVirtualItemType.enDianQuan)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_DianQuan");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_DianQuan");
                base.m_grade = 3;
            }
            else if (bType == enVirtualItemType.enHeart)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("Action_Point");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_AP");
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enExpPool)
            {
                base.m_name = "经验池";
                base.m_description = "经验池";
                base.m_grade = 0;
            }
            else if (bType == enVirtualItemType.enGoldCoin)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_GoldCoin");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_GoldCoin");
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enBurningCoin)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Burning_Coin");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_Burning_Coin");
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enArenaCoin)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Arena_Coin");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_Arena_Coin");
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enSkinCoin)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Skin_Coin");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_Skin_Coin");
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enGuildConstruct)
            {
                base.m_name = string.Empty;
                base.m_description = string.Empty;
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enSymbolCoin)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("Symbol_Coin_Text");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("Symbol_Coin_Desc");
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enDiamond)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("Shop_Money_Type_Diamond");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("Description_Diamond");
                base.m_grade = 3;
            }
            else if (bType == enVirtualItemType.enAchievementPoint)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("Achievement_Point");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("Achievement_Point_Desc");
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enHuoyueDu)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("HuoyueDu_Text");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("HuoyueDu_Desc");
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enMatchPersonalPoint)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("MatchPersonalPoint_Name");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("MatchPersonalPoint_Desc");
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enMatchTeamPoint)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("MatchTeamPoint_Name");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("MatchTeamPoint_Desc");
                base.m_grade = 1;
            }
            else if (bType == enVirtualItemType.enDianJuanJiFen)
            {
                base.m_name = Singleton<CTextManager>.GetInstance().GetText("DianJuanJiFenName");
                base.m_description = Singleton<CTextManager>.GetInstance().GetText("DianJuanJiFenDesc");
                base.m_grade = 3;
            }
        }

        public override string GetIconPath()
        {
            string str = CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f90;
            switch (this.m_virtualType)
            {
                case enVirtualItemType.enNoUsed:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f91);

                case enVirtualItemType.enExp:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f90);

                case enVirtualItemType.enDianQuan:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f92);

                case enVirtualItemType.enHeart:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f94);

                case enVirtualItemType.enGoldCoin:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f91);

                case enVirtualItemType.enArenaCoin:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f99);

                case enVirtualItemType.enBurningCoin:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f98);

                case enVirtualItemType.enExpPool:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f9a);

                case enVirtualItemType.enSkinCoin:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f97);

                case enVirtualItemType.enGuildConstruct:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f99);

                case enVirtualItemType.enSymbolCoin:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f9b);

                case enVirtualItemType.enDiamond:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f95);

                case enVirtualItemType.enAchievementPoint:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f9c);

                case enVirtualItemType.enHuoyueDu:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f9a);

                case enVirtualItemType.enMatchPersonalPoint:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f96);

                case enVirtualItemType.enMatchTeamPoint:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x15f96);

                case enVirtualItemType.enDianJuanJiFen:
                    return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + 0x16321);
            }
            return str;
        }

        public override COM_REWARDS_TYPE MapRewardType
        {
            get
            {
                switch (this.m_virtualType)
                {
                    case enVirtualItemType.enNoUsed:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN;

                    case enVirtualItemType.enExp:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_EXP;

                    case enVirtualItemType.enDianQuan:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_COUPONS;

                    case enVirtualItemType.enHeart:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP;

                    case enVirtualItemType.enGoldCoin:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_HONOUR;

                    case enVirtualItemType.enArenaCoin:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_ARENA_COIN;

                    case enVirtualItemType.enBurningCoin:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_BURNING_COIN;

                    case enVirtualItemType.enExpPool:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEROPOOLEXP;

                    case enVirtualItemType.enSkinCoin:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKINCOIN;

                    case enVirtualItemType.enSymbolCoin:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOLCOIN;

                    case enVirtualItemType.enDiamond:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND;

                    case enVirtualItemType.enHuoyueDu:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_HUOYUEDU;

                    case enVirtualItemType.enMatchPersonalPoint:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_MATCH_POINT_PERSON;

                    case enVirtualItemType.enMatchTeamPoint:
                        return COM_REWARDS_TYPE.COM_REWARDS_TYPE_MATCH_POINT_GUILD;
                }
                return base.MapRewardType;
            }
        }
    }
}

