namespace Assets.Scripts.GameSystem
{
    using System;

    public class CMallSortHelper
    {
        public static string[] heroSortTypeNameKeys = new string[] { "Mall_Hero_Sort_Type_Default", "Mall_Hero_Sort_Type_Name", "Mall_Hero_Sort_Type_Coupons", "Mall_Hero_Sort_Type_Coin", "Mall_Hero_Sort_Type_ReleaseTime" };
        public static string[] skinSortTypeNameKeys = new string[] { "Mall_Skin_Sort_Type_Default", "Mall_Skin_Sort_Type_Name", "Mall_Skin_Sort_Type_Coupons", "Mall_Skin_Sort_Type_ReleaseTime" };

        public static HeroSortImp CreateHeroSorter()
        {
            return Singleton<HeroSortImp>.GetInstance();
        }

        public static SkinSortImp CreateSkinSorter()
        {
            return Singleton<SkinSortImp>.GetInstance();
        }

        public enum HeroSortType
        {
            Default,
            Name,
            Coupons,
            Coin,
            ReleaseTime,
            TypeCount
        }

        public enum SkinSortType
        {
            Default,
            Name,
            Coupons,
            ReleaseTime,
            TypeCount
        }
    }
}

