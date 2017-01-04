namespace com.tencent.pandora
{
    using System;
    using System.Net;

    internal class Configer
    {
        public static int iCodeCSException = 100;
        public static bool IsUseLocalRes;
        public static string m_CurHotUpdatePath = string.Empty;
        public static string strCtrFlagTotalSwitch = string.Empty;
        public static string strIP = string.Empty;
        public static string strSDKVer = "YXZJ-Android-V0.3";
        public static string strSendLogFlag = string.Empty;

        public static string GetIP()
        {
            try
            {
                if (strIP != string.Empty)
                {
                    return strIP;
                }
                IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                if (hostAddresses.Length > 0)
                {
                    strIP = hostAddresses[0].ToString();
                    return strIP;
                }
                return string.Empty;
            }
            catch (Exception exception)
            {
                Logger.e(exception.Message);
                return "0";
            }
        }

        public enum ErrorCode
        {
            ACTION = 2,
            BROKER_SUCC = 0,
            ER_ATM_CAP = 0x3f0,
            ER_ATM_CONNECT = 0x3ef,
            ER_BROKER_CAP = 0x3ed,
            ER_BROKER_CONNECT = 0x3ea,
            ER_CLOUD_CAP = 0x3ee,
            ER_CLOUD_CONNECT = 0x3eb,
            ER_CLOUD_RETURN = 0x3f1,
            ER_CODE = 1,
            ER_DNS_ERROR = 0x3f2,
            ER_DOWNLOAD_ERROR = 0x3e9,
            ER_SHOW_PANEL = 0x3ec
        }
    }
}

