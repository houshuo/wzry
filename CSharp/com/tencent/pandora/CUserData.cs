namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;

    public class CUserData
    {
        public static CUserInfoData user = new CUserInfoData();

        public static void SetPara(Dictionary<string, string> dicPara)
        {
            foreach (KeyValuePair<string, string> pair in dicPara)
            {
                Logger.d("key " + pair.Key + ": " + pair.Value);
            }
            if (dicPara.ContainsKey("sOpenId"))
            {
                user.strOpenId = dicPara["sOpenId"];
            }
            if (dicPara.ContainsKey("sServiceType"))
            {
                user.sServiceType = dicPara["sServiceType"];
            }
            if (dicPara.ContainsKey("sAcountType"))
            {
                user.acctype = dicPara["sAcountType"];
            }
            if (dicPara.ContainsKey("sArea"))
            {
                user.strArea = dicPara["sArea"];
            }
            if (dicPara.ContainsKey("sPartition"))
            {
                user.sPartition = dicPara["sPartition"];
                user.pay_zone = dicPara["sPartition"];
            }
            if (dicPara.ContainsKey("sAppId"))
            {
                user.strAppid = dicPara["sAppId"];
            }
            if (dicPara.ContainsKey("sAppId"))
            {
                user.strAppid = dicPara["sAppId"];
            }
            if (dicPara.ContainsKey("sRoleId"))
            {
                user.strRoleId = dicPara["sRoleId"];
            }
            if (dicPara.ContainsKey("sAccessToken"))
            {
                user.sAccessToken = dicPara["sAccessToken"];
            }
            if (dicPara.ContainsKey("sPayToken"))
            {
                user.pay_token = dicPara["sPayToken"];
            }
            if (dicPara.ContainsKey("sGameVer"))
            {
                user.gameVer = dicPara["sGameVer"];
            }
            if (dicPara.ContainsKey("sPlatID"))
            {
                user.platid = dicPara["sPlatID"];
            }
            if (dicPara.ContainsKey("sQQInstalled"))
            {
                user.sQQInstalled = dicPara["sQQInstalled"];
            }
            if (dicPara.ContainsKey("sWXInstalled"))
            {
                user.sWXInstalled = dicPara["sWXInstalled"];
            }
            if (dicPara.ContainsKey("sGameName"))
            {
                user.sGameName = dicPara["sGameName"];
            }
        }

        public void setUserInfo(CUserInfoData u)
        {
            user.strOpenId = u.strOpenId;
            user.sServiceType = u.sServiceType;
            user.acctype = u.acctype;
            user.platid = u.platid;
        }
    }
}

