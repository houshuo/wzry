namespace com.tencent.pandora
{
    using System;

    public class Logger
    {
        public static int DEBUG;
        public static int ERROR = 3;
        public static int FATAL = 4;
        public static int INEED_LOG_TEXT;
        public static int INFO = 1;
        public static int LOG_LEVEL;
        public static int WARN = 2;

        public static void d(string msg)
        {
            Log(DEBUG, 0, msg);
        }

        public static void e(string msg)
        {
            if (!Pandora.stopConnectAll)
            {
                if (LOG_LEVEL > ERROR)
                {
                    string[] textArray1 = new string[] { DateTime.Now.ToString(), "_", DateTime.Now.Millisecond.ToString(), "|Pdr|", msg };
                    msg = string.Concat(textArray1);
                    FileUtils.AppendFile(FileUtils.strLogFileName, msg);
                    FileUtils.LoggCommInfo(msg);
                }
                LogNetError(0x270d, msg);
            }
        }

        public static void Log(int iLevel, int iErrorCode, string strMsg)
        {
            if (!Pandora.stopConnectAll && (LOG_LEVEL <= iLevel))
            {
                if (INEED_LOG_TEXT == 1)
                {
                    LogText(iLevel, strMsg);
                }
                if ((Configer.strSendLogFlag.Equals("1") || (iLevel == ERROR)) || (iLevel == FATAL))
                {
                    LogNet(iErrorCode, strMsg);
                }
            }
        }

        public static void LogCommInfo(string strLogCommInfo)
        {
            if (!Pandora.stopConnectAll)
            {
                FileUtils.LoggCommInfo(strLogCommInfo);
            }
        }

        public static void LogNet(int iErrorCode, string strMsg)
        {
            if (!Pandora.stopConnectAll)
            {
                try
                {
                    if (strMsg.Length > 600)
                    {
                        strMsg = strMsg.Substring(0, 600);
                    }
                    NetProxcy.SendLogReport(1, 1, iErrorCode, strMsg);
                }
                catch (Exception)
                {
                    LogText(DEBUG, strMsg);
                }
            }
        }

        public static void LogNetError(int iErrorCode, string strMsg)
        {
            if (!Pandora.stopConnectAll)
            {
                LogNet(iErrorCode, strMsg);
            }
        }

        public static void LogText(int iLevel, string msg)
        {
            if (!Pandora.stopConnectAll)
            {
                string[] textArray1 = new string[] { DateTime.Now.ToString(), "_", DateTime.Now.Millisecond.ToString(), "|Pdr|", msg };
                msg = string.Concat(textArray1);
                FileUtils.AppendFile(FileUtils.strLogFileName, msg);
            }
        }
    }
}

