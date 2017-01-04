namespace com.tencent.pandora
{
    using AOT;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class NetProxcy
    {
        public static bool haveSOData = false;
        private IncreaceUpdate increaceUpdate = new IncreaceUpdate();
        public static NetProxcy instance = null;
        private static bool isReportLogFile = false;
        public static Queue listInfoSO = new Queue();
        private static string md5val = string.Empty;
        private static string strActionListFile = "action_json.dat";
        public static string strCloudConfigInfo = string.Empty;
        public static string tokenkey = string.Empty;

        [DllImport("pandora")]
        private static extern void AttendAct(string sData, int iLength, int iFlag);
        [DllImport("pandora", CallingConvention=CallingConvention.Cdecl)]
        public static extern void AutoDownloadCallback(int iFlag);
        public static void CloseSocket()
        {
            try
            {
                U3dCloseSocket();
            }
            catch (Exception exception)
            {
                Logger.d(exception.StackTrace);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(CallBack))]
        public static void DoCallBack(string jsonFromSO, int jsonLength, int iFlag)
        {
            if (iFlag == 1)
            {
                Logger.d("in Connect:" + jsonFromSO);
                listInfoSO.Enqueue(new infoFromSo(jsonFromSO, jsonLength, iFlag));
                PraseConnect(jsonFromSO, jsonLength, iFlag);
            }
            else if (((iFlag != 9) && (iFlag != 7)) && (iFlag != 6))
            {
                Logger.d("iFlag:" + iFlag.ToString() + ",jsonFromSO:" + jsonFromSO);
                listInfoSO.Enqueue(new infoFromSo(jsonFromSO, jsonLength, iFlag));
            }
        }

        public static void GetActionList(string jsonExtend = "")
        {
            try
            {
                Logger.d("start to get actlist");
                string msg = new CReqInfoDataBuilder().getActionListReqJson(md5val, jsonExtend);
                Logger.d(msg);
                Logger.d("strJson.Length:" + msg.Length);
                Logger.d("get act list return:" + GetActList(msg, msg.Length, 3));
            }
            catch (Exception exception)
            {
                Logger.e("error:" + exception.Message);
                Logger.e(exception.StackTrace);
            }
        }

        [DllImport("pandora")]
        private static extern int GetActList(string sData, int iLength, int iFlag);
        [DllImport("pandora", CallingConvention=CallingConvention.Cdecl)]
        public static extern int GetCallBackData(CallBack callFunConnect);
        public static NetProxcy GetInstance()
        {
            if (instance == null)
            {
                instance = new NetProxcy();
            }
            return instance;
        }

        [DllImport("pandora")]
        public static extern int GpmPay(string sData, int iLength, int iFlag);
        [DllImport("pandora")]
        private static extern void InitPushDataCallback(int iFlag);
        public void InitSocket()
        {
            string sData = new CReqInfoDataBuilder().getInitSocketReqJson(md5val);
            Logger.d("in InitSocket......：" + sData);
            InitTcpSocket(1, sData, sData.Length, 2, 5);
            InitPushDataCallback(4);
        }

        [DllImport("pandora", CallingConvention=CallingConvention.Cdecl)]
        public static extern int InitTcpSocket(int iFlag, string sData, int iLength, int iLoginFlag, int iGetListFlag);
        public void midaspayCallBack(string callback)
        {
            Logger.d("XXXXXXXXXX midaspayCallBack:" + callback);
            listInfoSO.Enqueue(new infoFromSo(callback, callback.Length, 9));
        }

        public static void PandoraLibDownloadControl(bool blCanDown)
        {
            try
            {
                PandoraLibDownloadControl(blCanDown, 10);
            }
            catch (Exception exception)
            {
                Logger.d(exception.StackTrace);
            }
        }

        [DllImport("pandora", CallingConvention=CallingConvention.Cdecl)]
        public static extern int PandoraLibDownloadControl(bool bWork, int iFlag);
        public static int PraseConnect(string strContent, int iLen, int iFlag)
        {
            try
            {
                Dictionary<string, object> dictionary = Json.Deserialize(strContent) as Dictionary<string, object>;
                if (dictionary.ContainsKey("iPdrLibRet"))
                {
                    int num = Convert.ToInt32(dictionary["iPdrLibRet"] as string);
                    int num2 = -1;
                    if (dictionary.ContainsKey("ret"))
                    {
                        num2 = Convert.ToInt32(dictionary["ret"] as string);
                    }
                    if (num == 0)
                    {
                        strCloudConfigInfo = strContent;
                        if (dictionary.ContainsKey("isDebug"))
                        {
                            string str = dictionary["isDebug"] as string;
                            if (str.Equals("1"))
                            {
                                Logger.INEED_LOG_TEXT = 1;
                            }
                            else
                            {
                                Logger.INEED_LOG_TEXT = 0;
                            }
                        }
                        if (dictionary.ContainsKey("log_level"))
                        {
                            string s = dictionary["log_level"] as string;
                            int result = 0;
                            int.TryParse(s, out result);
                            Logger.LOG_LEVEL = result;
                        }
                        if (dictionary.ContainsKey("isNetLog"))
                        {
                            string str3 = dictionary["isNetLog"] as string;
                            if (str3 == "1")
                            {
                                Configer.strSendLogFlag = "1";
                            }
                            else
                            {
                                Configer.strSendLogFlag = "0";
                            }
                        }
                        if (dictionary.ContainsKey("totalSwitch"))
                        {
                            Logger.d("ret:" + num2.ToString());
                            if (((num2 != 0) && (num2 != 1)) && (num2 != 2))
                            {
                                Logger.LogNetError(0x3f1, "cgi return exception:" + dictionary["ret"].ToString());
                            }
                            else
                            {
                                Logger.d("cgi return: " + num2.ToString());
                            }
                            if (dictionary.ContainsKey("curr_lua_dir") && (dictionary["curr_lua_dir"].ToString() != string.Empty))
                            {
                                Configer.m_CurHotUpdatePath = dictionary["curr_lua_dir"].ToString();
                                if (Configer.m_CurHotUpdatePath != string.Empty)
                                {
                                    Configer.m_CurHotUpdatePath = Configer.m_CurHotUpdatePath + "/res/";
                                }
                                Logger.d("m_CurHotUpdatePath:" + Configer.m_CurHotUpdatePath);
                            }
                            string strCurState = string.Empty;
                            string strFile = string.Empty;
                            if (dictionary.ContainsKey("filelist"))
                            {
                                strCurState = dictionary["filelist"].ToString();
                            }
                            if (dictionary.ContainsKey("filename"))
                            {
                                strFile = dictionary["filename"].ToString();
                            }
                            if (dictionary.ContainsKey("lua_newversion"))
                            {
                                int curTimestamp;
                                if (!int.TryParse(dictionary["lua_newversion"].ToString(), out curTimestamp))
                                {
                                    Logger.d("云端lua_newversion获取不成功，使用时间戳为版本号：" + dictionary["lua_newversion"].ToString());
                                    curTimestamp = DateUtils.GetCurTimestamp();
                                }
                                ResourceManager.iLuaVer = curTimestamp;
                            }
                            else if (ResourceManager.iLuaVer == 1)
                            {
                                Logger.d("use time ver");
                                ResourceManager.iLuaVer = DateUtils.GetCurTimestamp();
                            }
                            string strConfigInfo = string.Empty;
                            if (dictionary.ContainsKey("dependency"))
                            {
                                strConfigInfo = dictionary["dependency"].ToString();
                            }
                            Configer.strCtrFlagTotalSwitch = dictionary["totalSwitch"] as string;
                            if (Configer.strCtrFlagTotalSwitch.Equals("1"))
                            {
                                if ((dictionary["ret"].ToString() == "1") || (dictionary["ret"].ToString() == "2"))
                                {
                                    Logger.d("初始化下载列表");
                                    GetInstance().increaceUpdate.SetUpdateInfo(strConfigInfo, strCurState);
                                }
                                if (dictionary["ret"].ToString() == "0")
                                {
                                    Logger.d("下载成功回调:" + strFile);
                                    GetInstance().increaceUpdate.AddDownSuccFile(strFile);
                                }
                            }
                        }
                        else
                        {
                            Logger.d("totalSwitch no data");
                        }
                    }
                    else
                    {
                        Logger.d("connect iRet:" + num.ToString());
                    }
                }
                else
                {
                    Logger.d("no para iPdrLibRet");
                }
            }
            catch (Exception exception)
            {
                Logger.e("connect fail,retry:" + exception.ToString());
            }
            return 0;
        }

        public static void RefreshLuaLoadStatus()
        {
            try
            {
                GetInstance().increaceUpdate.RefreshLuaLoadStatus();
            }
            catch (Exception exception)
            {
                Logger.d(exception.StackTrace);
            }
        }

        [DllImport("pandora")]
        private static extern int SendLogin(string sData, int iLength, int iFlag);
        public static void SendLogReport(int logLevel, int reportType, int toReturnCode, string logMsg)
        {
            try
            {
                string sData = new CReqInfoDataBuilder().getLogReportReqJson(logLevel, reportType, toReturnCode, logMsg);
                SendSDKLogReport(sData, sData.Length, 7);
            }
            catch (Exception)
            {
            }
        }

        public static void SendPandoraLibCmd(int iCmdId, string sData, int iLength, int iFlag)
        {
            Logger.d("enter C# SendPandoraLibCmd");
            SendPdrLibCmd(iCmdId, sData, iLength, iFlag);
        }

        [DllImport("pandora", CallingConvention=CallingConvention.Cdecl)]
        public static extern int SendPdrLibCmd(int iCmdId, string sData, int iLength, int iFlag);
        [DllImport("pandora")]
        private static extern int SendSDKLogReport(string sData, int iLenght, int iFlag);
        public static void SetLoadFinishLuaFile(string strFileName)
        {
            try
            {
                GetInstance().increaceUpdate.SetLuaLoadSucc(strFileName);
            }
            catch (Exception exception)
            {
                Logger.d(exception.StackTrace);
            }
        }

        public static void SetReDownCallBack()
        {
            AutoDownloadCallback(9);
        }

        [DllImport("pandora")]
        private static extern int StaticReport(string sData, int iLength, int iFlag);
        public static void StaticReport(int iModuleId, int iChannelId, int iActionId, int iReportType, int iJumpType, string strJumpUrl, string strGoodsId, int iGoodsNum, int iGoodFee, int iMoneyType)
        {
            try
            {
                string sData = new CReqInfoDataBuilder().staticReportReqJson(iModuleId, iChannelId, iActionId, iReportType, iJumpType, strJumpUrl, strGoodsId, iGoodsNum, iGoodFee, iMoneyType);
                Logger.d("StaticReport:" + sData);
                Logger.d("strJson.Length:" + sData.Length);
                Logger.d("get StaticReport return:" + StaticReport(sData, sData.Length, 6));
            }
            catch (Exception exception)
            {
                Logger.d("in exeption");
                Logger.e("error2:" + exception.Message);
                Logger.e(exception.StackTrace);
            }
        }

        [DllImport("pandora")]
        public static extern int U3dCloseSocket();
        [DllImport("pandora", CallingConvention=CallingConvention.Cdecl)]
        public static extern int UploadLogFile(string sData, int iLength, int iFlag);

        public delegate void CallBack([MarshalAs(UnmanagedType.LPStr)] string a, int i, int j);

        private enum CallBackFuncType
        {
            ACTIONLIST = 3,
            ATTEND = 5,
            CAN_DOWN_CONTROL = 10,
            CONNECT = 1,
            GPM_PAY = 8,
            LOG = 7,
            LOGIN = 2,
            PUSH = 4,
            RE_DOWN = 9,
            STATIC = 6,
            UPLOAD_FILE = 11
        }

        public class infoFromSo
        {
            public int iFlagSO;
            public string jsonFromSO = string.Empty;
            public int jsonFromSOLength;

            public infoFromSo(string json, int jsonLenth, int reqFlag)
            {
                this.jsonFromSO = json;
                this.jsonFromSOLength = jsonLenth;
                this.iFlagSO = reqFlag;
            }
        }
    }
}

