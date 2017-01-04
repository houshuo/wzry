namespace Apollo
{
    using MiniJSON;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class ApolloDNSClient : ApolloObject, IApolloDNSClient
    {
        private DictionaryView<string, ListView<ProcessQueryResult>> callbackObjectDic = new DictionaryView<string, ListView<ProcessQueryResult>>();
        private bool isCacheEnable = true;
        private bool isCallbackMode = false;
        private bool isLogEnable = false;
        private string mCurrentAPN = "Default";
        private string mDNSvrDomainName = "dns.tcls.qq.com";
        private string mDNSvrPortName = "port.dns.tcls.qq.com";
        private DNSErrCode mErrCode = DNSErrCode.DNS_NO_ERROR;
        private string mErrString = "no error";
        private IApolloDNSFileSys mFileSys = null;
        private DictionaryView<string, QueryValue> mInitDomainNameMap = new DictionaryView<string, QueryValue>();
        private DNSErrCode mInitStatus = DNSErrCode.DNS_UN_INIT;
        private string mUploadeDomainName = string.Empty;
        private string mUploadeUserInfo = "N";

        private DNSErrCode ClearCache(string domainName)
        {
            if ((this.mFileSys == null) || (domainName.Length == 0))
            {
                this.DNS_LOG_ERROR("fileSys or domainName is empty");
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            this.DNS_LOG_INFO(string.Format("domainName is: [{0}]", domainName));
            if (this.mFileSys.Remove(domainName))
            {
                this.DNS_LOG_INFO("success");
                return DNSErrCode.DNS_NO_ERROR;
            }
            this.DNS_LOG_ERROR("failed");
            return DNSErrCode.DNS_HANDLE_FAILED;
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern DNSErrCode dns_GetErrorCode(ulong objID);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr dns_GetErrorString(ulong objID);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr dns_GetIPStringFromJsonValue(ulong objID, [MarshalAs(UnmanagedType.LPStr)] string domainName, [MarshalAs(UnmanagedType.LPStr)] string jsonValue);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern DNSErrCode dns_Init(ulong objID, bool enable, int cacheTime);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool dns_IsIPStringExistInJsonValue(ulong objID, [MarshalAs(UnmanagedType.LPStr)] string domainName, [MarshalAs(UnmanagedType.LPStr)] string jsonValue);
        private void DNS_LOG_ERROR(string data)
        {
            this.Log("DNSClient[C#] [ERROR] " + string.Format("[{0}] {1}", this.GetCurrentMethonName(), data));
        }

        private void DNS_LOG_INFO(string data)
        {
            this.Log("DNSClient[C#] [INFO] " + string.Format("[{0}] {1}", this.GetCurrentMethonName(), data));
        }

        private void DNS_LOG_WARN(string data)
        {
            this.Log("DNSClient[C#] [WARN] " + string.Format("[{0}] {1}", this.GetCurrentMethonName(), data));
        }

        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern void dns_Poll(ulong objID, int timeout);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern DNSErrCode dns_Query(ulong objID, [MarshalAs(UnmanagedType.LPStr)] string domainName, [MarshalAs(UnmanagedType.LPStr)] string OId, [MarshalAs(UnmanagedType.LPStr)] string SId, [MarshalAs(UnmanagedType.LPStr)] string version, [MarshalAs(UnmanagedType.LPStr)] string userData);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern DNSErrCode dns_SetCurrentAPN(ulong objID, [MarshalAs(UnmanagedType.LPStr)] string APN);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern DNSErrCode dns_SetEnableLog(ulong objID, bool enable);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool dns_SetUploadIntData(ulong objID, int type, int date);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool dns_SetUploadStringData(ulong objID, int type, [MarshalAs(UnmanagedType.LPStr)] string data);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern DNSErrCode dns_UpdataIPList(ulong objID, [MarshalAs(UnmanagedType.LPStr)] string data);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern DNSErrCode dns_UpdataPortList(ulong objID, [MarshalAs(UnmanagedType.LPStr)] string data);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern IntPtr dns_UpdateIPStringIntoJsonValue(ulong objID, [MarshalAs(UnmanagedType.LPStr)] string domainName, [MarshalAs(UnmanagedType.LPStr)] string initJsonValue, [MarshalAs(UnmanagedType.LPStr)] string IPListString, int cacheTime);
        [DllImport("apollo", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool dns_UploadStatisticData(ulong objID);
        private void DNSClientApapterLog(string strLog)
        {
            if ((strLog.Length != 0) && this.isLogEnable)
            {
                this.Log("DNSClient[Apapter] " + strLog);
            }
        }

        private void DNSClientLog(string strLog)
        {
            if ((strLog.Length != 0) && this.isLogEnable)
            {
                this.Log("DNSClient[C++] " + strLog);
            }
        }

        private DNSErrCode GetCacheData(string domainName, ref string cacheData)
        {
            if ((this.mFileSys == null) || (domainName.Length == 0))
            {
                this.DNS_LOG_ERROR("fileSys or domainName is empty");
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            if (!this.mFileSys.Exist(domainName))
            {
                this.DNS_LOG_INFO(string.Format("no such domainName[{0}]'s date in cache", domainName));
                return DNSErrCode.DNS_NO_ERROR;
            }
            if (!this.mFileSys.Read(domainName, ref cacheData))
            {
                this.DNS_LOG_ERROR("read data from cache failed");
                return DNSErrCode.DNS_HANDLE_FAILED;
            }
            if (cacheData.Length == 0)
            {
                this.DNS_LOG_ERROR("the data read from cache is empty");
                return DNSErrCode.DNS_HANDLE_FAILED;
            }
            this.DNS_LOG_INFO(string.Format("cache data is [{0}]", cacheData));
            return DNSErrCode.DNS_NO_ERROR;
        }

        private string GetCurrentMethonName()
        {
            StackTrace trace = new StackTrace(new StackFrame(2));
            return trace.GetFrame(0).GetMethod().Name;
        }

        public DNSErrCode GetErrorCode()
        {
            return this.mErrCode;
        }

        public string GetErrorString()
        {
            return this.mErrString;
        }

        private DNSErrCode GetIPListFromCacheDate(string domainName, ref List<string> dataList)
        {
            if ((this.mFileSys == null) || (domainName.Length == 0))
            {
                this.DNS_LOG_ERROR("input param error");
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            string cacheData = string.Empty;
            this.GetCacheData(domainName, ref cacheData);
            if (cacheData.Length == 0)
            {
                this.DNS_LOG_ERROR("data get from cache is empty");
                return DNSErrCode.DNS_HANDLE_FAILED;
            }
            this.DNS_LOG_INFO(string.Format("date get from cache is [{0}]", cacheData));
            string str2 = Marshal.PtrToStringAnsi(dns_GetIPStringFromJsonValue(base.ObjectId, domainName, cacheData));
            if (str2.Length == 0)
            {
                this.DNS_LOG_ERROR("get IP string from C++ interface failed");
                return DNSErrCode.DNS_HANDLE_FAILED;
            }
            char[] separator = new char[] { '|' };
            string[] strArray = str2.Split(separator);
            if (strArray.Length == 0)
            {
                this.DNS_LOG_ERROR("IP list is empty");
                return DNSErrCode.DNS_HANDLE_FAILED;
            }
            foreach (string str3 in strArray)
            {
                dataList.Add(str3);
            }
            return DNSErrCode.DNS_NO_ERROR;
        }

        public DNSErrCode Init(bool callbackMode, int cacheTime)
        {
            if (this.mInitStatus == DNSErrCode.DNS_NO_ERROR)
            {
                this.DNS_LOG_INFO("init had been finished");
                return DNSErrCode.DNS_NO_ERROR;
            }
            this.isCallbackMode = callbackMode;
            if (this.mFileSys != null)
            {
                this.ReadDNSverConfig();
            }
            this.mInitStatus = dns_Init(base.ObjectId, callbackMode, cacheTime);
            this.mErrCode = dns_GetErrorCode(base.ObjectId);
            IntPtr ptr = dns_GetErrorString(base.ObjectId);
            this.mErrString = Marshal.PtrToStringAnsi(ptr);
            return this.mInitStatus;
        }

        private bool IsIPListExistInCache(string domainName)
        {
            if ((this.mFileSys == null) || (domainName.Length == 0))
            {
                this.DNS_LOG_ERROR("fileSys or domainName is empty");
                return false;
            }
            if (this.mFileSys.Exist(domainName))
            {
                this.DNS_LOG_INFO(string.Format("domainName[{0}] exist in cache", domainName));
                string cacheData = string.Empty;
                this.GetCacheData(domainName, ref cacheData);
                if (cacheData.Length == 0)
                {
                    this.DNS_LOG_ERROR("get empty data from cache");
                    return false;
                }
                return dns_IsIPStringExistInJsonValue(base.ObjectId, domainName, cacheData);
            }
            this.DNS_LOG_INFO(string.Format("domainName[{0}] doesn't exist in cache", domainName));
            return false;
        }

        private void Log(string strLog)
        {
            if (this.isLogEnable)
            {
                Debug.Log(strLog);
            }
        }

        private DNSErrCode ParseJsonValue(ref string valueString, ref QueryValue queryValue)
        {
            if (valueString.Length == 0)
            {
                this.DNS_LOG_ERROR("input param error");
                this.mErrCode = DNSErrCode.DNS_INPUT_PARAM_ERROR;
                this.mErrString = "the input json value string is empty";
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            this.DNS_LOG_INFO(string.Format("json value is [{0}]", valueString));
            Dictionary<string, object> dictionary = Json.Deserialize(valueString) as Dictionary<string, object>;
            if ((dictionary != null) && (dictionary.Count != 0))
            {
                long num = (long) dictionary["ErrCode"];
                if (num == 0)
                {
                    List<object> list = dictionary["Results"] as List<object>;
                    foreach (object obj2 in list)
                    {
                        long num2 = 0L;
                        string str = "no error";
                        string str2 = string.Empty;
                        List<string> dataList = new List<string>();
                        Dictionary<string, object> dictionary2 = obj2 as Dictionary<string, object>;
                        str2 = (string) dictionary2["Dn"];
                        long num3 = (long) dictionary2["ErrCode"];
                        num2 = num3;
                        if (num3 == 0)
                        {
                            this.DNS_LOG_INFO(string.Format("domainName[{0}] is success", str2));
                            List<object> list3 = dictionary2["Ips"] as List<object>;
                            foreach (object obj3 in list3)
                            {
                                Dictionary<string, object> dictionary3 = obj3 as Dictionary<string, object>;
                                dataList.Add((string) dictionary3["Ip"]);
                            }
                            if (0 < this.mUploadeDomainName.Length)
                            {
                                this.mUploadeDomainName = this.mUploadeDomainName + ":" + str2 + "_2";
                            }
                            else
                            {
                                this.mUploadeDomainName = this.mUploadeDomainName + str2 + "_2";
                            }
                        }
                        else
                        {
                            this.DNS_LOG_INFO(string.Format("domainName[{0}] is error", str2));
                            str = (string) dictionary2["ErrStr"];
                            if (0 < this.mUploadeDomainName.Length)
                            {
                                this.mUploadeDomainName = this.mUploadeDomainName + ":" + str2 + "_0";
                            }
                            else
                            {
                                this.mUploadeDomainName = this.mUploadeDomainName + str2 + "_0";
                            }
                        }
                        int cacheTime = 0;
                        if ((this.mDNSvrDomainName == str2) || (this.mDNSvrPortName == str2))
                        {
                            this.DNS_LOG_INFO("current domainName is DNS server's domainName or port");
                        }
                        else
                        {
                            DnValue value2;
                            value2.errCode = num2;
                            value2.errString = str;
                            value2.domainName = str2;
                            value2.IPList = dataList;
                            queryValue.value.Add(value2);
                            cacheTime = 1;
                        }
                        this.UpdateIPListIntoCacheDate(str2, dataList, cacheTime);
                    }
                    return DNSErrCode.DNS_NO_ERROR;
                }
                this.mErrCode = (DNSErrCode) ((int) num);
                this.mErrString = (string) dictionary["ErrStr"];
                queryValue.errCode = num;
                queryValue.errString = (string) dictionary["ErrStr"];
                return DNSErrCode.DNS_NO_ERROR;
            }
            this.mErrCode = DNSErrCode.DNS_JSON_PARSE_ERROR;
            this.mErrString = "parse the json value error";
            return DNSErrCode.DNS_JSON_PARSE_ERROR;
        }

        private void ParseRecvData(DnResult result)
        {
            QueryValue value2;
            this.DNS_LOG_INFO("ParseRecvData begin");
            value2.errCode = 0L;
            value2.errString = "no error";
            value2.value = new ListView<DnValue>();
            foreach (KeyValuePair<string, QueryValue> pair in this.mInitDomainNameMap)
            {
                if (pair.Key == result.domainName)
                {
                    QueryValue value3 = pair.Value;
                    if (value3.value.Count != 0)
                    {
                        foreach (DnValue value4 in value3.value)
                        {
                            value2.value.Add(value4);
                        }
                    }
                }
            }
            if (result.errCode == 0)
            {
                this.DNS_LOG_INFO("result is success");
                if (this.ParseJsonValue(ref result.value, ref value2) == DNSErrCode.DNS_NO_ERROR)
                {
                    this.DNS_LOG_INFO("json parse success");
                }
                else
                {
                    this.DNS_LOG_ERROR("parse json date return from DNS server error");
                    this.mErrCode = DNSErrCode.DNS_JSON_PARSE_ERROR;
                    this.mErrString = "parse json date return from DNS server error";
                    if (value2.value.Count == 0)
                    {
                        value2.errCode = 0x452L;
                        value2.errString = "json parse error";
                    }
                }
            }
            else
            {
                this.DNS_LOG_ERROR("result is failed");
                this.mErrCode = dns_GetErrorCode(base.ObjectId);
                IntPtr ptr = dns_GetErrorString(base.ObjectId);
                this.mErrString = Marshal.PtrToStringAnsi(ptr);
                if (value2.value.Count == 0)
                {
                    value2.errCode = result.errCode;
                    value2.errString = result.errString;
                }
                char[] separator = new char[] { '|' };
                foreach (string str in result.domainName.Split(separator))
                {
                    DnValue value5;
                    value5.errCode = result.errCode;
                    value5.errString = result.errString;
                    value5.domainName = str;
                    value5.IPList = new List<string>();
                    value2.value.Add(value5);
                    if (0 < this.mUploadeDomainName.Length)
                    {
                        this.mUploadeDomainName = this.mUploadeDomainName + ":" + str + "_0";
                    }
                    else
                    {
                        this.mUploadeDomainName = this.mUploadeDomainName + str + "_0";
                    }
                }
            }
            foreach (KeyValuePair<string, ListView<ProcessQueryResult>> pair2 in this.callbackObjectDic)
            {
                if (pair2.Key == result.domainName)
                {
                    ListView<ProcessQueryResult> view = pair2.Value;
                    if (view.Count != 0)
                    {
                        this.DNS_LOG_INFO(string.Format("process delegate function for domainNames[{0}]", result.domainName));
                        ProcessQueryResult result2 = view[0];
                        this.UploadStatisticData();
                        result2(value2);
                        view.RemoveAt(0);
                        if (view.Count == 0)
                        {
                            this.mInitDomainNameMap.Remove(result.domainName);
                            this.callbackObjectDic.Remove(result.domainName);
                        }
                    }
                    else
                    {
                        this.DNS_LOG_INFO(string.Format("no delegate function for domainName[{0}]", result.domainName));
                        this.mErrCode = DNSErrCode.DNS_HANDLE_FAILED;
                        this.mErrString = "no delegate function for domainName";
                    }
                    return;
                }
            }
            this.DNS_LOG_INFO(string.Format("no such domainName[{0}] in dictionary", result.domainName));
            this.mErrCode = DNSErrCode.DNS_HANDLE_FAILED;
            this.mErrString = "no such domainName in dictionary";
        }

        public void Poll(int timeout)
        {
            if (this.callbackObjectDic.Count != 0)
            {
                dns_Poll(base.ObjectId, timeout);
            }
            else
            {
                this.DNS_LOG_INFO("no query request");
            }
        }

        public DNSErrCode Query(string domainName, ProcessQueryResult process, string OId, string SId, string version, string userData)
        {
            QueryValue value2;
            if (this.mInitStatus != DNSErrCode.DNS_NO_ERROR)
            {
                this.DNS_LOG_ERROR("init failed or uninit");
                this.mErrCode = this.mInitStatus;
                this.mErrString = "init failed or uninit";
                return this.mInitStatus;
            }
            if ((domainName == null) || (domainName.Length == 0))
            {
                this.DNS_LOG_ERROR("domainName is null");
                this.mErrCode = DNSErrCode.DNS_INPUT_PARAM_ERROR;
                this.mErrString = "domainName is null";
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            if (process == null)
            {
                this.DNS_LOG_ERROR("delegate function is null");
                this.mErrCode = DNSErrCode.DNS_INPUT_PARAM_ERROR;
                this.mErrString = "delegate function is null";
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            this.DNS_LOG_INFO(string.Format("domainName is [{0}]", domainName));
            this.mUploadeDomainName = string.Empty;
            if ((((OId != null) || (version != null)) || (userData != null)) && ((SId == null) || (SId.Length == 0)))
            {
                this.DNS_LOG_ERROR("OId, version or userData is not null but the SId is null");
                this.mErrCode = DNSErrCode.DNS_INPUT_PARAM_ERROR;
                this.mErrString = "OId, version or userData is not null but the SId is null";
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            if ((OId != null) && (OId.Length != 0))
            {
                this.mUploadeUserInfo = this.mUploadeUserInfo + ":" + OId;
            }
            else
            {
                this.mUploadeUserInfo = this.mUploadeUserInfo + ":N";
            }
            if ((SId != null) && (SId.Length != 0))
            {
                this.mUploadeUserInfo = this.mUploadeUserInfo + ":" + SId;
            }
            else
            {
                this.mUploadeUserInfo = this.mUploadeUserInfo + ":N";
            }
            if ((version != null) && (version.Length != 0))
            {
                this.mUploadeUserInfo = this.mUploadeUserInfo + ":" + version;
            }
            else
            {
                this.mUploadeUserInfo = this.mUploadeUserInfo + ":N";
            }
            char[] separator = new char[] { '|' };
            string[] strArray = domainName.Split(separator);
            List<string> domainNameList = new List<string>();
            foreach (string str in strArray)
            {
                if ((str.Length != 0) && !domainNameList.Contains(str))
                {
                    domainNameList.Add(str);
                }
            }
            if (domainNameList.Count == 0)
            {
                this.DNS_LOG_ERROR("all domainNames is empty");
                this.mErrCode = DNSErrCode.DNS_INPUT_PARAM_ERROR;
                this.mErrString = "all domainNames is empty";
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            if (10 < domainNameList.Count)
            {
                this.DNS_LOG_ERROR("more then ten domainNames in the list");
                this.mErrCode = DNSErrCode.DNS_TOOMANY_DOMAINNAMES;
                this.mErrString = "more then ten domainNames in the list";
                return DNSErrCode.DNS_TOOMANY_DOMAINNAMES;
            }
            this.DNS_LOG_INFO("all param is legal");
            value2.errCode = 0L;
            value2.errString = "no error";
            value2.value = new ListView<DnValue>();
            List<string> subDomainNames = new List<string>();
            if ((this.mFileSys != null) && this.isCacheEnable)
            {
                this.DNS_LOG_INFO("search domainName in cache");
                this.SearchDomainNameInCache(domainNameList, ref value2, ref subDomainNames);
            }
            else
            {
                this.DNS_LOG_INFO("search all domainNames in DNS server");
                foreach (string str2 in domainNameList)
                {
                    subDomainNames.Add(str2);
                }
            }
            if (subDomainNames.Count == 0)
            {
                this.DNS_LOG_INFO("all domainName's IP exist in cache");
                this.UploadStatisticData();
                process(value2);
                return DNSErrCode.DNS_NO_ERROR;
            }
            string str3 = string.Empty;
            for (int i = 0; i < (subDomainNames.Count - 1); i++)
            {
                str3 = str3 + subDomainNames[i] + "|";
            }
            str3 = str3 + subDomainNames[subDomainNames.Count - 1];
            this.DNS_LOG_INFO(string.Format("send to DNS server's domainNames are [{0}]", str3));
            DNSErrCode code = dns_Query(base.ObjectId, str3, OId, SId, version, userData);
            this.mErrCode = dns_GetErrorCode(base.ObjectId);
            IntPtr ptr = dns_GetErrorString(base.ObjectId);
            this.mErrString = Marshal.PtrToStringAnsi(ptr);
            if (code != DNSErrCode.DNS_NO_ERROR)
            {
                this.DNS_LOG_ERROR(string.Format("send request failed, error code is [{0}]", Convert.ToString(code)));
                foreach (string str4 in subDomainNames)
                {
                    DnValue value3;
                    value3.errCode = 0x455L;
                    value3.errString = "send request error";
                    value3.domainName = str4;
                    value3.IPList = new List<string>();
                    value2.value.Add(value3);
                    if (0 < this.mUploadeDomainName.Length)
                    {
                        this.mUploadeDomainName = this.mUploadeDomainName + ":" + str4 + "_0";
                    }
                    else
                    {
                        this.mUploadeDomainName = this.mUploadeDomainName + str4 + "_0";
                    }
                }
                if (subDomainNames.Count == domainNameList.Count)
                {
                    value2.errCode = 0x455L;
                    value2.errString = "send request error";
                }
                this.UploadStatisticData();
                process(value2);
                return code;
            }
            if (this.callbackObjectDic.ContainsKey(str3))
            {
                this.callbackObjectDic[str3].Add(process);
            }
            else
            {
                ListView<ProcessQueryResult> view = new ListView<ProcessQueryResult>();
                view.Add(process);
                this.callbackObjectDic.Add(str3, view);
            }
            if (this.isCallbackMode)
            {
                process(value2);
                value2.value.Clear();
            }
            this.mInitDomainNameMap.Add(str3, value2);
            return DNSErrCode.DNS_NO_ERROR;
        }

        private DNSErrCode ReadDNSverConfig()
        {
            if (this.IsIPListExistInCache(this.mDNSvrDomainName))
            {
                this.DNS_LOG_INFO("DNS server's IP exist in cache");
                List<string> dataList = new List<string>();
                this.GetIPListFromCacheDate(this.mDNSvrDomainName, ref dataList);
                if (dataList.Count != 0)
                {
                    string str = string.Empty;
                    for (int i = 0; i < (dataList.Count - 1); i++)
                    {
                        str = str + dataList[i] + "|";
                    }
                    str = str + dataList[dataList.Count - 1];
                    this.DNS_LOG_INFO(string.Format("send to DNS server's IP are [{0}]", str));
                    dns_UpdataIPList(base.ObjectId, str);
                }
                else
                {
                    this.DNS_LOG_ERROR("get empty DNS server's IP list from cache");
                }
            }
            else
            {
                this.DNS_LOG_INFO("DNS server's IP doesn't exist in cache");
            }
            if (this.IsIPListExistInCache(this.mDNSvrPortName))
            {
                this.DNS_LOG_INFO("DNS server's port exist in cache");
                List<string> list2 = new List<string>();
                this.GetIPListFromCacheDate(this.mDNSvrPortName, ref list2);
                if (list2.Count != 0)
                {
                    string str2 = string.Empty;
                    for (int j = 0; j < (list2.Count - 1); j++)
                    {
                        str2 = str2 + list2[j] + "|";
                    }
                    str2 = str2 + list2[list2.Count - 1];
                    this.DNS_LOG_INFO(string.Format("send to DNS server's port are [{0}]", str2));
                    dns_UpdataPortList(base.ObjectId, str2);
                }
                else
                {
                    this.DNS_LOG_ERROR("get empty DNS server's port list from cache");
                }
            }
            else
            {
                this.DNS_LOG_ERROR("DNS server's IP doesn't exist in cache");
            }
            return DNSErrCode.DNS_NO_ERROR;
        }

        private void RecvData(IntPtr ptr)
        {
            this.DNS_LOG_INFO("recv data from DNS server");
            DnResult result = (DnResult) Marshal.PtrToStructure(ptr, typeof(DnResult));
            if (result.domainName.Length != 0)
            {
                if (this.callbackObjectDic.ContainsKey(result.domainName))
                {
                    this.DNS_LOG_INFO(string.Format("recive domainName is [{0}]", result.domainName));
                    this.ParseRecvData(result);
                }
                else
                {
                    this.DNS_LOG_ERROR(string.Format("no such domainName[{0}] in callback object dictionary", result.domainName));
                    this.mErrCode = DNSErrCode.DNS_INPUT_PARAM_ERROR;
                    this.mErrString = "no domainName's delegate function in callback object dictionary";
                }
            }
            else
            {
                this.DNS_LOG_ERROR("the result domainName is empty");
                this.mErrCode = DNSErrCode.DNS_INPUT_PARAM_ERROR;
                this.mErrString = "the result domainName is empty";
            }
        }

        private DNSErrCode SearchDomainNameInCache(List<string> domainNameList, ref QueryValue queryValue, ref List<string> subDomainNames)
        {
            foreach (string str in domainNameList)
            {
                if (!this.IsIPListExistInCache(str))
                {
                    subDomainNames.Add(str);
                }
                else
                {
                    List<string> dataList = new List<string>();
                    this.GetIPListFromCacheDate(str, ref dataList);
                    if (dataList.Count == 0)
                    {
                        this.DNS_LOG_ERROR(string.Format("no IP in list from cache data for domainName[{0}]", str));
                        subDomainNames.Add(str);
                    }
                    else
                    {
                        DnValue value2;
                        value2.errCode = 0L;
                        value2.errString = "no error";
                        value2.domainName = str;
                        value2.IPList = new List<string>();
                        foreach (string str2 in dataList)
                        {
                            value2.IPList.Add(str2);
                        }
                        queryValue.value.Add(value2);
                        if (0 < this.mUploadeDomainName.Length)
                        {
                            this.mUploadeDomainName = this.mUploadeDomainName + ":" + str + "_1";
                        }
                        else
                        {
                            this.mUploadeDomainName = this.mUploadeDomainName + str + "_1";
                        }
                    }
                }
            }
            return DNSErrCode.DNS_NO_ERROR;
        }

        public DNSErrCode SetCurrentAPN(string APN)
        {
            if ((APN == null) || (APN.Length == 0))
            {
                this.DNS_LOG_ERROR("input param error");
                this.mErrCode = DNSErrCode.DNS_INPUT_PARAM_ERROR;
                this.mErrString = "the input APN string is illegal";
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            this.mCurrentAPN = APN;
            this.mUploadeUserInfo = APN;
            return dns_SetCurrentAPN(base.ObjectId, APN);
        }

        public DNSErrCode SetEnableCache(bool enable)
        {
            this.isCacheEnable = enable;
            return DNSErrCode.DNS_NO_ERROR;
        }

        public DNSErrCode SetEnableLog(bool enable)
        {
            this.isLogEnable = enable;
            return dns_SetEnableLog(base.ObjectId, enable);
        }

        public DNSErrCode SetFileSys(IApolloDNSFileSys fileSys)
        {
            if (fileSys == null)
            {
                this.DNS_LOG_ERROR("input param error");
                this.mErrCode = DNSErrCode.DNS_INPUT_PARAM_ERROR;
                this.mErrString = "the input file system pointer is empty";
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            this.mFileSys = fileSys;
            return DNSErrCode.DNS_NO_ERROR;
        }

        private DNSErrCode UpdateCache(string domainName, string cacheData)
        {
            if (((this.mFileSys == null) || (domainName.Length == 0)) || (cacheData.Length == 0))
            {
                this.DNS_LOG_ERROR("fileSys, domainName or cacheData is empty");
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            this.DNS_LOG_INFO(string.Format("domainName[{0}]'s cache data is: [{1}]", domainName, cacheData));
            if (this.mFileSys.Write(domainName, cacheData))
            {
                this.DNS_LOG_INFO("success");
                return DNSErrCode.DNS_NO_ERROR;
            }
            this.DNS_LOG_ERROR("failed");
            return DNSErrCode.DNS_HANDLE_FAILED;
        }

        private DNSErrCode UpdateIPListIntoCacheDate(string domainName, List<string> dataList, int cacheTime = 1)
        {
            if (((this.mFileSys == null) || (domainName.Length == 0)) || (dataList.Count == 0))
            {
                this.DNS_LOG_ERROR("input param error");
                return DNSErrCode.DNS_INPUT_PARAM_ERROR;
            }
            string str = string.Empty;
            int count = dataList.Count;
            for (int i = 0; i < (count - 1); i++)
            {
                str = str + dataList[i] + "|";
            }
            str = str + dataList[count - 1];
            this.DNS_LOG_INFO(string.Format("IP string is [{0}]", str));
            string cacheData = string.Empty;
            this.GetCacheData(domainName, ref cacheData);
            string str3 = Marshal.PtrToStringAnsi(dns_UpdateIPStringIntoJsonValue(base.ObjectId, domainName, cacheData, str, cacheTime));
            if (str3.Length == 0)
            {
                this.DNS_LOG_ERROR("get update json string error");
                return DNSErrCode.DNS_HANDLE_FAILED;
            }
            this.DNS_LOG_INFO(string.Format("get update json string is [{0}]", str3));
            return this.UpdateCache(domainName, str3);
        }

        private DNSErrCode UploadStatisticData()
        {
            int date = -1;
            if (this.isCacheEnable)
            {
                date = 1;
            }
            else
            {
                date = 0;
            }
            dns_SetUploadIntData(base.ObjectId, 0, (int) this.mErrCode);
            dns_SetUploadIntData(base.ObjectId, 1, date);
            dns_SetUploadStringData(base.ObjectId, 10, this.mUploadeDomainName);
            dns_SetUploadStringData(base.ObjectId, 11, this.mUploadeUserInfo);
            dns_UploadStatisticData(base.ObjectId);
            if ("Default" == this.mCurrentAPN)
            {
                this.mUploadeUserInfo = "N";
            }
            else
            {
                this.mUploadeUserInfo = this.mCurrentAPN;
            }
            return DNSErrCode.DNS_NO_ERROR;
        }
    }
}

