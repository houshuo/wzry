namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal class CReqInfoDataBuilder
    {
        public string getActionListReqJson(string md5, string jsonExtend = "")
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("open_id", CUserData.user.strOpenId);
            dictionary.Add("gid", "gid");
            dictionary.Add("device_id", CUserData.user.device_id);
            dictionary.Add("device_type", 1);
            dictionary.Add("imei", CUserData.user.imei);
            dictionary.Add("net_type", 1);
            dictionary.Add("imsi", CUserData.user.imsi);
            dictionary.Add("client_ip", CUserData.user.client_ip);
            dictionary.Add("app_id", CUserData.user.strAppid);
            dictionary.Add("g_tk", "1842395457");
            dictionary.Add("sarea", CUserData.user.strArea);
            dictionary.Add("splatid", CUserData.user.platid);
            dictionary.Add("spartition", CUserData.user.sPartition);
            dictionary.Add("sroleid", CUserData.user.strRoleId);
            dictionary.Add("access_token", CUserData.user.sAccessToken);
            dictionary.Add("acctype", CUserData.user.acctype);
            dictionary.Add("uin", string.Empty);
            dictionary.Add("skey", string.Empty);
            dictionary.Add("p_uin", string.Empty);
            dictionary.Add("p_skey", string.Empty);
            dictionary.Add("pt4_token", string.Empty);
            dictionary.Add("md5val", md5);
            dictionary.Add("triger_type", 0);
            dictionary.Add("gameappversion", CUserData.user.gameVer);
            if (!string.IsNullOrEmpty(jsonExtend))
            {
                dictionary.Add("extend", Json.Deserialize(jsonExtend));
            }
            return Json.Serialize(dictionary);
        }

        public string getGPMBuyPayReqJson(string strDjcActionId, int iCostType, string payType, string strGoodsId, int iNum)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("gpm_app_id", CUserData.user.gpm_app_id);
            dictionary.Add("gameappid", CUserData.user.strAppid);
            dictionary.Add("_plug_id", CUserData.user._plug_id);
            dictionary.Add("_output_fmt", string.Empty);
            dictionary.Add("acctype", CUserData.user.acctype);
            dictionary.Add("openid", CUserData.user.strOpenId);
            dictionary.Add("access_token", CUserData.user.sAccessToken);
            dictionary.Add("pay_token", CUserData.user.pay_token);
            if (NetProxcy.tokenkey != string.Empty)
            {
                dictionary.Add("pay_token", NetProxcy.tokenkey);
                Logger.d("user configer paytoken");
            }
            dictionary.Add("plat", CUserData.user.platid);
            dictionary.Add("areaid", CUserData.user.strArea);
            dictionary.Add("partition", CUserData.user.sPartition);
            dictionary.Add("roleid", CUserData.user.strRoleId);
            dictionary.Add("rolename", CUserData.user.strRoleName);
            dictionary.Add("propid", strGoodsId);
            dictionary.Add("buynum", iNum.ToString());
            dictionary.Add("paytype", payType);
            dictionary.Add("_test", "1");
            dictionary.Add("_jsonp", string.Empty);
            dictionary.Add("_cs", string.Empty);
            dictionary.Add("_retkey", string.Empty);
            dictionary.Add("apptype", iCostType.ToString());
            dictionary.Add("serial", "1234");
            dictionary.Add("ru", string.Empty);
            dictionary.Add("pu", string.Empty);
            dictionary.Add("hu", string.Empty);
            dictionary.Add("iGoodsId", strGoodsId);
            dictionary.Add("iActionId", strDjcActionId);
            dictionary.Add("productid", CUserData.user.productid);
            dictionary.Add("pay_zone", CUserData.user.pay_zone);
            return Json.Serialize(dictionary);
        }

        public string getInitSocketReqJson(string strMd5)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("gid", "gid");
            dictionary.Add("open_id", CUserData.user.strOpenId);
            dictionary.Add("gameappversion", CUserData.user.gameVer);
            dictionary.Add("device_id", CUserData.user.device_id);
            dictionary.Add("device_type", 1);
            dictionary.Add("imei", CUserData.user.imei);
            dictionary.Add("net_type", 1);
            dictionary.Add("imsi", CUserData.user.imsi);
            dictionary.Add("client_ip", CUserData.user.client_ip);
            dictionary.Add("app_id", CUserData.user.strAppid);
            dictionary.Add("access_token", CUserData.user.sAccessToken);
            dictionary.Add("acctype", CUserData.user.acctype);
            dictionary.Add("uin", string.Empty);
            dictionary.Add("skey", string.Empty);
            dictionary.Add("p_uin", string.Empty);
            dictionary.Add("p_skey", string.Empty);
            dictionary.Add("pt4_token", string.Empty);
            dictionary.Add("IED_LOG_INFO2", string.Empty);
            dictionary.Add("sarea", CUserData.user.strArea);
            dictionary.Add("splatid", CUserData.user.platid);
            dictionary.Add("spartition", CUserData.user.sPartition);
            Logger.d("filePath:" + FileUtils.path);
            dictionary.Add("sLogFile", FileUtils.path);
            dictionary.Add("sdkversion", Configer.strSDKVer);
            dictionary.Add("triger_type", 1);
            dictionary.Add("sroleid", CUserData.user.strRoleId);
            dictionary.Add("md5val", strMd5);
            dictionary.Add("iNotLogin", "1");
            dictionary.Add("iNotGetActList", "1");
            return Json.Serialize(dictionary);
        }

        public string getLogReportReqJson(int logLevel, int reportType, int toReturnCode, string logMsg)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("uint_log_level", logLevel);
            dictionary.Add("str_sdk_version", Configer.strSDKVer);
            dictionary.Add("str_hardware_os", CUserData.user.str_hardware_os);
            dictionary.Add("str_openid", CUserData.user.strOpenId);
            dictionary.Add("str_userip", Configer.GetIP());
            dictionary.Add("uint_report_type", reportType);
            dictionary.Add("uint_toreturncode", toReturnCode);
            dictionary.Add("str_respara", logMsg);
            dictionary.Add("uint_serialtime", DateUtils.GetCurTimestamp());
            dictionary.Add("sarea", CUserData.user.strArea);
            dictionary.Add("spartition", CUserData.user.sPartition);
            dictionary.Add("splatid", CUserData.user.platid);
            return Json.Serialize(dictionary);
        }

        public string staticReportReqJson(int iModuleId, int iChannelId, int iReportType, int iActionId, int iJumpType, string strJumpUrl, string strGoodsId, int iGoodsNum, int iGoodFee, int iMoneyType)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("uint_timestamp", DateUtils.GetCurTimestamp());
            dictionary.Add("uint_clientip", CUserData.user.client_ip);
            dictionary.Add("str_open_id", CUserData.user.strOpenId);
            dictionary.Add("str_phoneid", 1);
            dictionary.Add("uint_module", iModuleId.ToString());
            dictionary.Add("uint_channel_id", iChannelId);
            dictionary.Add("uint_typ", iReportType);
            dictionary.Add("str_appid", CUserData.user.strAppid);
            dictionary.Add("str_sdkversion", Configer.strSDKVer);
            dictionary.Add("uint_ostype", 1);
            dictionary.Add("uint_act_id", iActionId);
            dictionary.Add("uint_jump_type", iJumpType);
            dictionary.Add("str_jump_url", strJumpUrl);
            dictionary.Add("partition", CUserData.user.sPartition);
            dictionary.Add("recommend_id", 1);
            dictionary.Add("changjing_id", 1);
            dictionary.Add("goods_id", strGoodsId);
            dictionary.Add("uint_count", iGoodsNum);
            dictionary.Add("uint_fee", iGoodFee);
            dictionary.Add("currency_type", iMoneyType);
            return Json.Serialize(dictionary);
        }
    }
}

