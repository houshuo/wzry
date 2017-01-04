namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stShopItemInfo
    {
        public COM_ITEM_TYPE enItemType;
        public RES_SHOPBUY_COINTYPE enCostType;
        public string sName;
        public uint dwItemId;
        public ushort wItemCnt;
        public ushort wSaleDiscount;
        public bool isSoldOut;
        public float fPrice;
    }
}

