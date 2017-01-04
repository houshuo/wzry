namespace com.tencent.pandora
{
    using System;
    using UnityEngine;

    public class ThirdSDK
    {
        private static ThirdSDK instance;

        public void BuyGoods(string strGoodsId, string iActionId, string payType, int iNum)
        {
            try
            {
                int iFlag = 8;
                com.tencent.pandora.Logger.d("start to buy goods:" + strGoodsId);
                int iCostType = 2;
                string sData = new CReqInfoDataBuilder().getGPMBuyPayReqJson(iActionId, iCostType, payType, strGoodsId, iNum);
                com.tencent.pandora.Logger.d("GPM PAY:" + sData);
                com.tencent.pandora.Logger.d("strJson.Length:" + sData.Length);
                com.tencent.pandora.Logger.d("get GpmPay return:" + NetProxcy.GpmPay(sData, sData.Length, iFlag));
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.e("error2:" + exception.Message);
                com.tencent.pandora.Logger.e(exception.StackTrace);
            }
        }

        public static ThirdSDK GetInstance()
        {
            if (instance == null)
            {
                instance = new ThirdSDK();
            }
            return instance;
        }

        public void midasPay(bool bTest, string offerId, string pf, string goodsTokenUrl, string accType, string payToken, string zoneId, string pfKey, string openid, string obj, string method, int iFlag)
        {
            using (AndroidJavaClass class2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject obj2 = class2.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    com.tencent.pandora.Logger.d("serali33" + obj2);
                    AndroidJavaObject @static = class2.GetStatic<AndroidJavaObject>("currentActivity");
                    com.tencent.pandora.Logger.d("serali44");
                    AndroidJavaClass class3 = new AndroidJavaClass("com.tencent.pandora.pay.PandoraPay");
                    com.tencent.pandora.Logger.d("serali55");
                    Pandora.iMidasPayFlag = iFlag;
                    object[] args = new object[] { @static, bTest, offerId, pf, goodsTokenUrl, accType, payToken, zoneId, pfKey, openid, obj, method };
                    class3.CallStatic("midasPay", args);
                    com.tencent.pandora.Logger.d("serali44");
                }
            }
        }
    }
}

