namespace Assets.Scripts.GameSystem
{
    using ApolloUpdate;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using IIPSMobile;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CVersionUpdateSystem : MonoSingleton<CVersionUpdateSystem>
    {
        private const float c_appYYBCheckVersionMaxWaitingTime = 10f;
        private const string c_homePageUrl = "http://pvp.qq.com";
        private ListView<CAnnouncementInfo> m_announcementInfos = new ListView<CAnnouncementInfo>();
        private ApolloUpdateSpeedCounter m_apolloUpdateSpeedCounter = new ApolloUpdateSpeedCounter();
        private uint m_appDownloadSize;
        private string m_appDownloadUrl;
        private int m_appEnableYYB;
        private bool m_appIsDelayToInstall;
        private bool m_appIsNoIFS;
        private uint m_appRecommendUpdateVersionMax;
        private uint m_appRecommendUpdateVersionMin;
        private string m_appSavePath;
        private bool m_appYYBCheckVersionInfoCallBackHandled;
        private float m_appYYBCheckVersionInfoStartTime;
        private CYYBUpdateManager m_appYYBUpdateManager;
        private string m_cachedResourceBuildNumber;
        private int m_cachedResourceType;
        private string m_cachedResourceVersion;
        private string m_downloadAppVersion;
        private uint m_downloadCounter;
        private string m_downloadResourceVersion;
        private uint m_downloadSpeed;
        private string m_firstExtractResourceBuildNumber;
        private string m_firstExtractResourceVersion;
        private IIPSMobile.IIPSMobileErrorCodeCheck m_iipsErrorCodeChecker = new IIPSMobile.IIPSMobileErrorCodeCheck();
        private IIPSMobileVersionMgrInterface m_IIPSVersionMgr;
        private IIPSMobileVersion m_IIPSVersionMgrFactory;
        private bool m_isAnnouncementPanelOpened;
        private bool m_isError;
        private OnVersionUpdateComplete m_onVersionUpdateComplete;
        private uint m_reportAppDownloadSize;
        private DateTime m_reportAppDownloadStartTime = new DateTime();
        private uint m_reportAppTotalDownloadSize;
        private uint m_reportResourceDownloadSize;
        private DateTime m_reportResourceDownloadStartTime = new DateTime();
        private uint m_reportResourceTotalDownloadSize;
        private bool m_resourceCheckCompatibility = true;
        private bool m_resourceDownloadNeedConfirm;
        private uint m_resourceDownloadSize;
        private bool m_useCurseResourceDownloadSize;
        private CUIFormScript m_versionUpdateFormScript;
        private enVersionUpdateState m_versionUpdateState;
        private static AndroidJavaClass s_androidUtilityJavaClass;
        private static uint s_appID = 1;
        private static enAppType s_appType = enAppType.Unknown;
        private static string s_appVersion = null;
        private static string s_cachedResourceVersionFilePath = null;
        private static string s_downloadedIFSSavePath = null;
        private static string s_firstExtractIFSName = null;
        private static string s_firstExtractIFSPath = null;
        private static string s_ifsExtractPath = null;
        private static enIIPSServerType s_IIPSServerType;
        private static string[][] s_IIPSServerUrls;
        private static enPlatform s_platform = GetPlatform();
        private static string s_resourcePackerInfoSetFullPath = null;
        private static uint[] s_serviceIDsForUpdataApp = new uint[] { 0x9e0005b, 0x9e0005a };
        private static uint[] s_serviceIDsForUpdateCompetitionApp = new uint[] { 0x9e0009f, 0x9e0009e };
        private static uint[] s_serviceIDsForUpdateCompetitionResource = new uint[] { 0x9e000a2, 0x9e000a1 };
        private static uint[] s_serviceIDsForUpdateResource = new uint[] { 0x9e00010, 0x9e0000f };
        public static string s_splashFormPathFuckLOL0 = "UGUI/Form/System/Login/Form_Splash.prefab";
        public static string s_splashFormPathFuckLOL1 = "UGUI/Form/System/Login/Form_Splash_New.prefab";
        private static string s_versionUpdateFormPath;
        private static string s_waitingFormPath;

        static CVersionUpdateSystem()
        {
            string[][] textArrayArray1 = new string[9][];
            textArrayArray1[0] = new string[] { "tcp://mtcls.qq.com:50011", "tcp://61.151.224.100:50011", "tcp://58.251.61.169:50011", "tcp://203.205.151.237:50011", "tcp://203.205.147.178:50011", "tcp://183.61.49.177:50011", "tcp://183.232.103.166:50011", "tcp://182.254.4.176:50011", "tcp://182.254.10.82:50011", "tcp://140.207.127.61:50011", "tcp://117.144.242.115:50011" };
            textArrayArray1[1] = new string[] { "tcp://middle.mtcls.qq.com:20001", "tcp://101.226.141.88:20001" };
            textArrayArray1[2] = new string[] { "tcp://testa4.mtcls.qq.com:10001", "tcp://101.227.153.83:10001" };
            textArrayArray1[3] = new string[] { "tcp://exp.mtcls.qq.com:10001", "tcp://61.151.234.47:10001", "tcp://182.254.42.103:10001", "tcp://140.207.62.111:10001", "tcp://140.207.123.164:10001", "tcp://117.144.242.28:10001", "tcp://117.135.171.74:10001", "tcp://103.7.30.91:10001", "tcp://101.227.130.79:10001" };
            textArrayArray1[4] = new string[] { "tcp://testb4.mtcls.qq.com:10001", "tcp://101.227.153.86:10001" };
            textArrayArray1[5] = new string[] { "tcp://testc.mtcls.qq.com:10001", "tcp://183.61.39.51:10001" };
            textArrayArray1[6] = new string[] { "exp.mtcls.qq.com:10011", "61.151.234.47:10011", "182.254.42.103:10011", "140.207.62.111:10011", "140.207.123.164:10011", "117.144.242.28:10011", "117.135.171.74:10011", "103.7.30.91:10011", "101.227.130.79:10011" };
            textArrayArray1[7] = new string[] { "testa4.mtcls.qq.com:10001", "101.227.153.83:10001" };
            textArrayArray1[8] = new string[] { string.Empty };
            s_IIPSServerUrls = textArrayArray1;
            s_IIPSServerType = GameVersion.IIPSServerType;
            s_versionUpdateFormPath = "UGUI/Form/System/VersionUpdate/Form_VersionUpdate.prefab";
            s_waitingFormPath = "UGUI/Form/Common/Form_SendMsgAlert.prefab";
        }

        private static void Android_ExitApp()
        {
            s_androidUtilityJavaClass.CallStatic("ExitApp", new object[0]);
        }

        private static string Android_GetApkAbsPath()
        {
            return s_androidUtilityJavaClass.CallStatic<string>("GetApkAbsPath", new object[0]);
        }

        private static int Android_GetNetworkType()
        {
            return s_androidUtilityJavaClass.CallStatic<int>("GetNetworkType", new object[0]);
        }

        private void Android_InstallAPK(string path)
        {
            if (this.m_IIPSVersionMgr != null)
            {
                this.m_IIPSVersionMgr.InstallApk(path);
            }
        }

        private static bool Android_IsFileExistInStreamingAssets(string fileName)
        {
            object[] args = new object[] { fileName };
            return s_androidUtilityJavaClass.CallStatic<bool>("IsFileExistInStreamingAssets", args);
        }

        private void CheckAppVersion()
        {
            if (((this.m_appYYBCheckVersionInfoStartTime > 0f) && !this.m_appYYBCheckVersionInfoCallBackHandled) && ((Time.realtimeSinceStartup - this.m_appYYBCheckVersionInfoStartTime) >= 10f))
            {
                this.OpenAppUpdateConfirmPanel(true, false);
                this.m_appYYBCheckVersionInfoCallBackHandled = true;
            }
        }

        [DebuggerHidden]
        private IEnumerator CheckFirstExtractResource()
        {
            return new <CheckFirstExtractResource>c__Iterator25 { <>f__this = this };
        }

        private bool CheckResourceCompatibility(string appVersion, string resourceVersion)
        {
            if (string.IsNullOrEmpty(appVersion) || string.IsNullOrEmpty(resourceVersion))
            {
                return false;
            }
            int length = appVersion.LastIndexOf(".");
            if (length >= 0)
            {
                appVersion = appVersion.Substring(0, length);
            }
            length = resourceVersion.LastIndexOf(".");
            if (length >= 0)
            {
                resourceVersion = resourceVersion.Substring(0, length);
            }
            return string.Equals(appVersion, resourceVersion);
        }

        public bool ClearCachePath()
        {
            string[] fileExtensionFilter = new string[] { ".json", ".flist", ".res", ".bytes" };
            string[] folderFilter = new string[] { CFileManager.s_ifsExtractFolder };
            return CFileManager.ClearDirectory(CFileManager.GetCachePath(), fileExtensionFilter, folderFilter);
        }

        private bool ClearDownloadedApk()
        {
            string[] fileExtensionFilter = new string[] { ".apk" };
            return CFileManager.ClearDirectory(CFileManager.GetCachePath(), fileExtensionFilter, null);
        }

        private void CloseAnnouncementPanel()
        {
            GameObject widget = this.m_versionUpdateFormScript.GetWidget(7);
            if (widget != null)
            {
                widget.CustomSetActive(false);
            }
            this.m_isAnnouncementPanelOpened = false;
        }

        private void CloseConfirmPanel()
        {
            GameObject widget = this.m_versionUpdateFormScript.GetWidget(5);
            if (widget != null)
            {
                widget.CustomSetActive(false);
            }
            GameObject obj3 = this.m_versionUpdateFormScript.GetWidget(0x11);
            if (obj3 != null)
            {
                obj3.CustomSetActive(true);
            }
        }

        private void CloseWaitingForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_waitingFormPath);
        }

        private void Complete()
        {
            this.m_versionUpdateState = enVersionUpdateState.End;
            base.StartCoroutine(this.VersionUpdateComplete());
            Singleton<BeaconHelper>.GetInstance().Event_CommonReport("Event_VerUpdateFinish");
        }

        private void CreateIIPSVersionMgr(string config)
        {
            CVersionUpdateCallback callback;
            if ((this.m_IIPSVersionMgr != null) || (this.m_IIPSVersionMgrFactory != null))
            {
                this.DisposeIIPSVersionMgr();
            }
            callback = new CVersionUpdateCallback {
                m_onGetNewVersionInfoDelegate = (CVersionUpdateCallback.OnGetNewVersionInfoDelegate) Delegate.Combine(callback.m_onGetNewVersionInfoDelegate, new CVersionUpdateCallback.OnGetNewVersionInfoDelegate(this.OnGetNewVersionInfo)),
                m_onProgressDelegate = (CVersionUpdateCallback.OnProgressDelegate) Delegate.Combine(callback.m_onProgressDelegate, new CVersionUpdateCallback.OnProgressDelegate(this.OnProgress)),
                m_onErrorDelegate = (CVersionUpdateCallback.OnErrorDelegate) Delegate.Combine(callback.m_onErrorDelegate, new CVersionUpdateCallback.OnErrorDelegate(this.OnError)),
                m_onSuccessDelegate = (CVersionUpdateCallback.OnSuccessDelegate) Delegate.Combine(callback.m_onSuccessDelegate, new CVersionUpdateCallback.OnSuccessDelegate(this.OnSuccess)),
                m_onNoticeInstallApkDelegate = (CVersionUpdateCallback.OnNoticeInstallApkDelegate) Delegate.Combine(callback.m_onNoticeInstallApkDelegate, new CVersionUpdateCallback.OnNoticeInstallApkDelegate(this.OnNoticeInstallApk)),
                m_onActionMsgDelegate = (CVersionUpdateCallback.OnActionMsgDelegate) Delegate.Combine(callback.m_onActionMsgDelegate, new CVersionUpdateCallback.OnActionMsgDelegate(this.OnActionMsg))
            };
            this.m_IIPSVersionMgrFactory = new IIPSMobileVersion();
            this.m_IIPSVersionMgr = this.m_IIPSVersionMgrFactory.CreateVersionMgr(callback, config);
        }

        private void CreateYYBUpdateManager()
        {
            if (this.m_appYYBUpdateManager == null)
            {
                this.m_appYYBUpdateManager = new CYYBUpdateManager(new Assets.Scripts.GameSystem.CYYBUpdateManager.OnYYBCheckNeedUpdateInfo(this.OnYYBCheckNeedUpdateInfo), new Assets.Scripts.GameSystem.CYYBUpdateManager.OnDownloadYYBProgressChanged(this.OnDownloadYYBProgressChanged), new Assets.Scripts.GameSystem.CYYBUpdateManager.OnDownloadYYBStateChanged(this.OnDownloadYYBStateChanged));
            }
        }

        private void DisposeIIPSVersionMgr()
        {
            if (this.m_IIPSVersionMgr != null)
            {
                this.m_IIPSVersionMgr.MgrUnitVersionManager();
                this.m_IIPSVersionMgr = null;
            }
            if (this.m_IIPSVersionMgrFactory != null)
            {
                this.m_IIPSVersionMgrFactory.DeleteVersionMgr();
                this.m_IIPSVersionMgrFactory = null;
            }
        }

        private void EnableAnnouncementElementPointer(int index, bool enabled)
        {
            if ((index >= 0) && (index < this.m_announcementInfos.Count))
            {
                GameObject widget = this.m_versionUpdateFormScript.GetWidget(4);
                if (widget != null)
                {
                    CUIContainerScript component = widget.GetComponent<CUIContainerScript>();
                    if ((component != null) && (index >= 0))
                    {
                        GameObject element = component.GetElement(this.m_announcementInfos[index].m_pointerSequence);
                        if (element != null)
                        {
                            Transform transform = element.transform.FindChild("Image_Pointer");
                            if (transform != null)
                            {
                                transform.gameObject.CustomSetActive(enabled);
                            }
                        }
                    }
                }
            }
        }

        private bool EnableYYBSaveUpdate()
        {
            return ((s_platform == enPlatform.Android) && (((this.m_appEnableYYB == 1) && (Singleton<ApolloHelper>.GetInstance().GetChannelID() == 0x7d2)) || (this.m_appEnableYYB == 2)));
        }

        private void FinishFirstExtractResource()
        {
            this.DisposeIIPSVersionMgr();
            if (s_IIPSServerType == enIIPSServerType.None)
            {
                this.m_versionUpdateState = enVersionUpdateState.FinishUpdateResource;
            }
            else
            {
                this.m_versionUpdateState = enVersionUpdateState.StartCheckResourceVersion;
            }
        }

        private void FinishUpdateApp()
        {
            this.DisposeIIPSVersionMgr();
            this.m_versionUpdateState = enVersionUpdateState.StartCheckFirstExtractResource;
        }

        private void FinishUpdateResource()
        {
            this.UpdateUIProgress(enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, 1f);
            if ((s_platform == enPlatform.Android) && this.m_appIsDelayToInstall)
            {
                if (!string.IsNullOrEmpty(this.m_appSavePath))
                {
                    this.Android_InstallAPK(this.m_appSavePath);
                }
                this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
            }
            else
            {
                this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_Complete"));
                this.m_versionUpdateState = enVersionUpdateState.Complete;
            }
            this.DisposeIIPSVersionMgr();
        }

        private string GetAndroidApkAbsPath()
        {
            return Android_GetApkAbsPath();
        }

        private enAnnouncementType GetAnnouncementType(string extension)
        {
            if (string.Equals(extension, "txt"))
            {
                return enAnnouncementType.Text;
            }
            return enAnnouncementType.Image;
        }

        private string GetApkDownloadUrl()
        {
            string filePath = CFileManager.CombinePath(CFileManager.GetCachePath(), this.m_downloadAppVersion);
            if (CFileManager.IsFileExist(filePath))
            {
                string msg = File.ReadAllText(filePath);
                string titleContentInMsg = this.GetTitleContentInMsg(msg, "\"full\"");
                return this.GetParamValueInContent(titleContentInMsg, "url");
            }
            return string.Empty;
        }

        private string GetCheckAppVersionJsonConfig(enPlatform platform)
        {
            string str = string.Empty;
            if (platform == enPlatform.IOS)
            {
                object[] args = new object[] { this.GetIIPSServerUrl(), s_appID, this.GetIIPSServiceIDForUpdateApp(s_IIPSServerType, platform), s_appVersion, this.GetIIPSServerAmount() + 2 };
                return string.Format("{{\r\n                            \"m_update_type\" : 4,\r\n                            \"log_debug\" : false,\r\n\r\n                            \"basic_version\":\r\n                            {{\r\n                                \"m_server_url_list\" : [{0}],\r\n                                \"m_app_id\" : {1},\r\n                                \"m_service_id\" : {2},\r\n                                \"m_current_version_str\" : \"{3}\",\r\n                                \"m_retry_count\" : {4},\r\n                                \"m_retry_interval_ms\" : 1000,\r\n                                \"m_connect_timeout_ms\" : 1000,\r\n                                \"m_send_timeout_ms\" : 2000,\r\n                                \"m_recv_timeout_ms\" : 3000\r\n                            }}\r\n                    }}", args);
            }
            if (platform == enPlatform.Android)
            {
                object[] objArray2 = new object[] { this.GetIIPSServerUrl(), s_appID, this.GetIIPSServiceIDForUpdateApp(s_IIPSServerType, platform), s_appVersion, s_downloadedIFSSavePath, this.GetAndroidApkAbsPath(), this.GetIIPSServerAmount() + 2 };
                str = string.Format("{{\r\n                    \"basic_update\":\r\n                    {{\r\n                        \"m_ifs_save_path\" : \"{4}\",\r\n                        \"m_nextaction\" : \"basic_diffupdata\"\r\n                    }},\r\n\r\n                    \"basic_diffupdata\":             \r\n                    {{\r\n                        \"m_diff_config_save_path\" : \"{4}\",\r\n                        \"m_diff_temp_path\" : \"{4}\",\r\n                        \"m_nMaxDownloadSpeed\" : 102400000,\r\n                        \"m_apk_abspath\" : \"{5}\"\r\n                    }},\r\n\r\n                    \"m_update_type\" : 4,\r\n                    \"log_debug\" : false,\r\n\r\n                    \"basic_version\":\r\n                    {{\r\n                        \"m_server_url_list\" : [{0}],\r\n                        \"m_app_id\" : {1},\r\n                        \"m_service_id\" : {2},\r\n                        \"m_current_version_str\" : \"{3}\",\r\n                        \"m_retry_count\" : {6},\r\n                        \"m_retry_interval_ms\" : 1000,\r\n                        \"m_connect_timeout_ms\" : 1000,\r\n                        \"m_send_timeout_ms\" : 2000,\r\n                        \"m_recv_timeout_ms\" : 3000\r\n                    }}\r\n                }}", objArray2);
            }
            return str;
        }

        private string GetCheckResourceVersionJsonConfig(enPlatform platform)
        {
            object[] args = new object[] { this.GetIIPSServerUrl(), s_appID, this.GetIIPSServiceIDForUpdateResource(s_IIPSServerType, platform), this.m_cachedResourceVersion, s_downloadedIFSSavePath, s_ifsExtractPath, this.GetIIPSServerAmount() + 2, !this.m_useCurseResourceDownloadSize ? string.Empty : "\"need_down_size\" : true," };
            return string.Format("{{\r\n                        \"basic_update\":\r\n                        {{\r\n                            \"m_ifs_save_path\" : \"{4}\",\r\n                            \"m_nextaction\" : \"basic_diffupdata\"\r\n                        }},\r\n                \r\n                        \"full_diff\":\r\n\t\t\t\t        {{ \r\n\t\t\t\t\t        \"m_ifs_save_path\":\"{4}\",\r\n\t\t\t\t\t        \"m_file_extract_path\":\"{5}\"\r\n\t\t\t\t        }},\r\n                \r\n                        \"m_update_type\" : 5,\r\n                        \"log_debug\" : false,\r\n                        {7}\r\n\r\n                        \"basic_version\":\r\n                        {{\r\n                            \"m_server_url_list\" : [{0}],\r\n                            \"m_app_id\" : {1},\r\n                            \"m_service_id\" : {2},\r\n                            \"m_current_version_str\" : \"{3}\",\r\n                            \"m_retry_count\" : {6},\r\n\t\t                    \"m_retry_interval_ms\" : 1000,\r\n\t\t                    \"m_connect_timeout_ms\" : 1000,\r\n\t\t                    \"m_send_timeout_ms\" : 2000,\r\n\t\t                    \"m_recv_timeout_ms\" : 3000\r\n                        }}\r\n                    }}\r\n                ", args);
        }

        private string GetDownloadSpeed(int speed)
        {
            return string.Format("{0}/s", this.GetSizeString(speed));
        }

        private string GetDownloadTotalSize(int size)
        {
            return this.GetSizeString(size);
        }

        private string GetErrorResult(uint errorCode)
        {
            string key = "IIPS_Error_Result_Unknown";
            switch (this.m_iipsErrorCodeChecker.CheckIIPSErrorCode((int) errorCode).m_nErrorType)
            {
                case 1:
                    key = "IIPS_Error_Result_NetworkError";
                    break;

                case 2:
                    key = "IIPS_Error_Result_NetworkTimeout";
                    break;

                case 3:
                    key = "IIPS_Error_Result_DiskFull";
                    break;

                case 4:
                    key = "IIPS_Error_Result_OtherSystemError";
                    break;

                case 5:
                    key = "IIPS_Error_Result_OtherError";
                    break;

                case 6:
                    key = "IIPS_Error_Result_NoSupportUpdate";
                    break;

                case 7:
                    key = "IIPS_Error_Result_NotSure";
                    break;

                case 8:
                    key = "IIPS_Error_Result_NoSupportUpdate";
                    break;
            }
            return Singleton<CTextManager>.GetInstance().GetText(key);
        }

        private string GetFirstExtractResourceJsonConfig(enPlatform platform)
        {
            object[] args = new object[] { this.GetIIPSServerUrl(), s_appID, this.GetIIPSServiceIDForUpdateResource(s_IIPSServerType, platform), this.m_firstExtractResourceVersion, s_downloadedIFSSavePath, s_ifsExtractPath, s_firstExtractIFSName, s_firstExtractIFSPath, this.GetIIPSServerAmount() + 2 };
            return string.Format("{{\r\n                        \"basic_update\": \r\n                        {{ \r\n                            \"m_ifs_save_path\" : \"{4}\", \r\n                            \"m_nextaction\" : \"basic_diffupdata\" \r\n                        }}, \r\n                \r\n                        \"full_diff\":          \r\n\t\t\t\t        {{ \r\n\t\t\t\t\t        \"m_ifs_save_path\":\"{4}\", \r\n\t\t\t\t\t        \"m_file_extract_path\":\"{5}\" \r\n\t\t\t\t        }}, \r\n                \r\n                        \"m_update_type\" : 5,\r\n                        \"log_debug\" : false,\r\n\r\n                        \"basic_version\":\r\n                        {{\r\n                            \"m_server_url_list\" : [{0}],\r\n                            \"m_app_id\" : {1},\r\n                            \"m_service_id\" : {2},\r\n                            \"m_current_version_str\" : \"{3}\",\r\n                            \"m_retry_count\" : {8},\r\n\t\t                    \"m_retry_interval_ms\" : 1000,\r\n\t\t                    \"m_connect_timeout_ms\" : 1000,\r\n\t\t                    \"m_send_timeout_ms\" : 2000,\r\n\t\t                    \"m_recv_timeout_ms\" : 3000\r\n                        }},\r\n\r\n                        \"first_extract\":\r\n\t\t\t\t        {{\r\n\t\t\t\t\t        \"m_ifs_extract_path\":\"{5}\",\r\n\t\t\t\t\t        \"m_ifs_res_save_path\":\"{4}\",\r\n\t\t\t\t\t        \"filelist\":[\r\n\t\t\t\t\t\t        {{\r\n\t\t\t\t\t\t\t        \"filename\":\"{6}\",\r\n\t\t\t\t\t\t\t        \"filepath\":\"{7}\"\r\n\t\t\t\t\t\t        }}\r\n\t\t\t\t\t        ]\r\n\t\t\t\t        }}\r\n                    }}\r\n                ", args);
        }

        private int GetIIPSServerAmount()
        {
            string[] strArray = s_IIPSServerUrls[(int) s_IIPSServerType];
            return strArray.Length;
        }

        public static enIIPSServerType GetIIPSServerType()
        {
            return s_IIPSServerType;
        }

        private string GetIIPSServerUrl()
        {
            string[] strArray = s_IIPSServerUrls[(int) s_IIPSServerType];
            string str = string.Empty;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (i != (strArray.Length - 1))
                {
                    str = str + string.Format("\"{0}\",", strArray[i]);
                }
                else
                {
                    str = str + string.Format("\"{0}\"", strArray[i]);
                }
            }
            return str;
        }

        private uint GetIIPSServiceIDForUpdateApp(enIIPSServerType serverType, enPlatform platForm)
        {
            switch (serverType)
            {
                case enIIPSServerType.Official:
                case enIIPSServerType.Middle:
                case enIIPSServerType.Test:
                case enIIPSServerType.ExpOfficial:
                case enIIPSServerType.ExpTest:
                case enIIPSServerType.TestForTester:
                    return s_serviceIDsForUpdataApp[(int) platForm];

                case enIIPSServerType.CompetitionOfficial:
                case enIIPSServerType.CompetitionTest:
                    return s_serviceIDsForUpdateCompetitionApp[(int) platForm];
            }
            return 0;
        }

        private uint GetIIPSServiceIDForUpdateResource(enIIPSServerType serverType, enPlatform platForm)
        {
            switch (serverType)
            {
                case enIIPSServerType.Official:
                case enIIPSServerType.Middle:
                case enIIPSServerType.Test:
                case enIIPSServerType.ExpOfficial:
                case enIIPSServerType.ExpTest:
                case enIIPSServerType.TestForTester:
                    return s_serviceIDsForUpdateResource[(int) platForm];

                case enIIPSServerType.CompetitionOfficial:
                case enIIPSServerType.CompetitionTest:
                    return s_serviceIDsForUpdateCompetitionResource[(int) platForm];
            }
            return 0;
        }

        private static string GetIIPSStreamingAssetsPath(string ifsName)
        {
            return string.Format("apk://{0}?assets/{1}", Android_GetApkAbsPath(), ifsName);
        }

        private void GetParamPair(string paramPairStr, out string param, out string value)
        {
            param = string.Empty;
            value = string.Empty;
            char[] separator = new char[] { ':' };
            string[] strArray = paramPairStr.Split(separator, 2);
            if ((strArray != null) && (strArray.Length == 2))
            {
                param = this.RemoveQuotationMark(strArray[0].Trim());
                value = this.RemoveQuotationMark(strArray[1].Trim());
            }
        }

        private string GetParamValueInContent(string titleContent, string param)
        {
            char[] separator = new char[] { ',' };
            string[] strArray = titleContent.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                string str;
                string str2;
                this.GetParamPair(strArray[i], out str, out str2);
                if (string.Equals(param, str))
                {
                    return str2;
                }
            }
            return string.Empty;
        }

        private static enPlatform GetPlatform()
        {
            return enPlatform.Android;
        }

        private string GetSizeString(int size)
        {
            if (size >= 0x100000)
            {
                float num = ((float) size) / 1048576f;
                return string.Format("{0}MB", Mathf.RoundToInt(num));
            }
            float f = ((float) size) / 1024f;
            return string.Format("{0}KB", Mathf.RoundToInt(f));
        }

        private string GetTitleContentInMsg(string msg, string title)
        {
            int index = msg.IndexOf(title);
            if (index >= 0)
            {
                int num2 = msg.IndexOf("{", index);
                int num3 = msg.IndexOf("}", index);
                if ((num2 > 0) && (num3 > 0))
                {
                    return msg.Substring(num2 + 1, ((num3 - num2) + 1) - 2);
                }
            }
            return string.Empty;
        }

        private T GetUIComponent<T>(CUIFormScript formScript, enVersionUpdateFormWidget widget) where T: MonoBehaviour
        {
            if (formScript == null)
            {
                return null;
            }
            GameObject obj2 = this.m_versionUpdateFormScript.GetWidget((int) widget);
            if (obj2 == null)
            {
                return null;
            }
            return obj2.GetComponent<T>();
        }

        private static bool I2B(int value)
        {
            return (value > 0);
        }

        protected override void Init()
        {
            s_downloadedIFSSavePath = CFileManager.GetCachePath();
            s_ifsExtractPath = CFileManager.GetIFSExtractPath();
            s_firstExtractIFSName = CFileManager.EraseExtension(CResourcePackerInfoSet.s_resourceIFSFileName) + ".png";
            s_firstExtractIFSPath = null;
            s_resourcePackerInfoSetFullPath = CFileManager.CombinePath(s_ifsExtractPath, CResourcePackerInfoSet.s_resourcePackerInfoSetFileName);
            s_appVersion = GameFramework.AppVersion;
            s_appType = enAppType.General;
            s_cachedResourceVersionFilePath = CFileManager.CombinePath(CFileManager.GetCachePath(), "Resource.bytes");
            this.m_versionUpdateState = enVersionUpdateState.None;
            this.m_cachedResourceVersion = CVersion.s_emptyResourceVersion;
            this.m_cachedResourceBuildNumber = CVersion.s_emptyBuildNumber;
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_JumpToHomePage, new CUIEventManager.OnUIEventHandler(this.OnJumpToHomePage));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_RetryCheckAppVersion, new CUIEventManager.OnUIEventHandler(this.OnRetryCheckApp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_ConfirmUpdateApp, new CUIEventManager.OnUIEventHandler(this.OnConfirmUpdateApp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_ConfirmUpdateAppNoWifi, new CUIEventManager.OnUIEventHandler(this.OnConfirmUpdateAppNoWifi));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_CancelUpdateApp, new CUIEventManager.OnUIEventHandler(this.OnCancelUpdateApp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_QuitApp, new CUIEventManager.OnUIEventHandler(this.OnQuitApp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_RetryCheckFirstExtractResource, new CUIEventManager.OnUIEventHandler(this.OnRetryCheckFirstExtractResource));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_RetryCheckResourceVersion, new CUIEventManager.OnUIEventHandler(this.OnRetryCheckResourceVersion));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_ConfirmUpdateResource, new CUIEventManager.OnUIEventHandler(this.OnConfirmUpdateResource));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_ConfirmUpdateResourceNoWifi, new CUIEventManager.OnUIEventHandler(this.OnConfirmUpdateResourceNoWifi));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_ConfirmYYBSaveUpdateApp, new CUIEventManager.OnUIEventHandler(this.OnConfirmYYBSaveUpdateApp));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_OnAnnouncementListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnAnnouncementListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_SwitchAnnouncementListElementToFront, new CUIEventManager.OnUIEventHandler(this.OnSwitchAnnouncementListElementToFront));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_SwitchAnnouncementListElementToBehind, new CUIEventManager.OnUIEventHandler(this.OnSwitchAnnouncementListElementToBehind));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.VersionUpdate_OnAnnouncementListSelectChanged, new CUIEventManager.OnUIEventHandler(this.OnAnnouncementListSelectChanged));
            s_androidUtilityJavaClass = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
            s_firstExtractIFSPath = GetIIPSStreamingAssetsPath(s_firstExtractIFSName);
            CLoginSystem.s_splashFormPath = s_splashFormPathFuckLOL1;
            if (PlayerPrefs.GetInt("SplashHack", 0) == 1)
            {
                CLoginSystem.s_splashFormPath = s_splashFormPathFuckLOL0;
            }
        }

        private void InitializeAnnouncement(string announcementUrl, string announcementExtension)
        {
            if ((this.m_announcementInfos.Count <= 0) && (!string.IsNullOrEmpty(announcementUrl) && !string.IsNullOrEmpty(announcementExtension)))
            {
                char[] separator = new char[] { '|' };
                string[] strArray = announcementExtension.Split(separator);
                for (int i = 0; i < strArray.Length; i++)
                {
                    strArray[i] = strArray[i].Trim();
                    CAnnouncementInfo item = new CAnnouncementInfo();
                    try
                    {
                        item.m_type = this.GetAnnouncementType(strArray[i]);
                        int num2 = i + 1;
                        item.m_url = announcementUrl.Replace("%ID%", num2.ToString()).Replace("%Extension%", strArray[i]);
                        item.m_pointerSequence = -1;
                    }
                    catch (Exception)
                    {
                    }
                    this.m_announcementInfos.Add(item);
                }
            }
        }

        private bool IsFileExistInStreamingAssets(string fileName)
        {
            return Android_IsFileExistInStreamingAssets(fileName);
        }

        private bool IsInFirstExtractResourceStage()
        {
            return ((this.m_versionUpdateState >= enVersionUpdateState.StartCheckFirstExtractResource) && (this.m_versionUpdateState <= enVersionUpdateState.FinishFirstExtractResouce));
        }

        private bool IsInUpdateAppStage()
        {
            return ((this.m_versionUpdateState >= enVersionUpdateState.StartCheckAppVersion) && (this.m_versionUpdateState <= enVersionUpdateState.FinishUpdateApp));
        }

        private bool IsInUpdateResourceStage()
        {
            return ((this.m_versionUpdateState >= enVersionUpdateState.StartCheckResourceVersion) && (this.m_versionUpdateState <= enVersionUpdateState.FinishUpdateResource));
        }

        private bool IsUseWifi()
        {
            return (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
        }

        public byte OnActionMsg(string msg)
        {
            string titleContentInMsg = this.GetTitleContentInMsg(msg, "@App");
            if (!string.IsNullOrEmpty(titleContentInMsg))
            {
                this.m_appDownloadUrl = this.GetParamValueInContent(titleContentInMsg, "url");
                string paramValueInContent = this.GetParamValueInContent(titleContentInMsg, "size");
                string str3 = this.GetParamValueInContent(titleContentInMsg, "minversion");
                string str4 = this.GetParamValueInContent(titleContentInMsg, "maxversion");
                string str5 = this.GetParamValueInContent(titleContentInMsg, "EnableYYB");
                string announcementUrl = this.GetParamValueInContent(titleContentInMsg, "AnnouncementUrl");
                string announcementExtension = this.GetParamValueInContent(titleContentInMsg, "AnnouncementExtension");
                string str8 = this.GetParamValueInContent(titleContentInMsg, "NoIFS");
                if (!string.IsNullOrEmpty(paramValueInContent))
                {
                    try
                    {
                        this.m_appDownloadSize = uint.Parse(paramValueInContent);
                    }
                    catch (Exception)
                    {
                        this.m_appDownloadSize = 0;
                    }
                }
                if (!string.IsNullOrEmpty(str3))
                {
                    this.m_appRecommendUpdateVersionMin = CVersion.GetVersionNumber(str3);
                }
                else
                {
                    this.m_appRecommendUpdateVersionMin = 0;
                }
                if (!string.IsNullOrEmpty(str4))
                {
                    this.m_appRecommendUpdateVersionMax = CVersion.GetVersionNumber(str4);
                }
                else
                {
                    this.m_appRecommendUpdateVersionMax = 0;
                }
                if (!string.IsNullOrEmpty(str5))
                {
                    try
                    {
                        this.m_appEnableYYB = int.Parse(str5);
                    }
                    catch (Exception)
                    {
                        this.m_appEnableYYB = 0;
                    }
                }
                else
                {
                    this.m_appEnableYYB = 0;
                }
                this.InitializeAnnouncement(announcementUrl, announcementExtension);
                if (!string.IsNullOrEmpty(str8))
                {
                    try
                    {
                        this.m_appIsNoIFS = int.Parse(str8) > 0;
                    }
                    catch (Exception)
                    {
                        this.m_appIsNoIFS = false;
                    }
                }
                else
                {
                    this.m_appIsNoIFS = false;
                }
            }
            string str9 = this.GetTitleContentInMsg(msg, "@Resource");
            if (!string.IsNullOrEmpty(str9))
            {
                string str10 = this.GetParamValueInContent(str9, "needconfirm");
                string str11 = this.GetParamValueInContent(str9, "size");
                string str12 = this.GetParamValueInContent(str9, "AnnouncementUrl");
                string str13 = this.GetParamValueInContent(str9, "AnnouncementExtension");
                string str14 = this.GetParamValueInContent(str9, "CheckCompatibility");
                this.m_resourceDownloadNeedConfirm = false;
                if (!string.IsNullOrEmpty(str10))
                {
                    this.m_resourceDownloadNeedConfirm = string.Equals(str10, "1");
                }
                this.m_resourceCheckCompatibility = true;
                if (!string.IsNullOrEmpty(str14))
                {
                    this.m_resourceCheckCompatibility = string.Equals(str14, "1");
                }
                if (!this.m_useCurseResourceDownloadSize && !string.IsNullOrEmpty(str11))
                {
                    try
                    {
                        this.m_resourceDownloadSize = uint.Parse(str11);
                    }
                    catch (Exception)
                    {
                        this.m_resourceDownloadSize = 0;
                    }
                }
                this.InitializeAnnouncement(str12, str13);
            }
            return 1;
        }

        private void OnAnnouncementListElementEnable(CUIEvent uiEvent)
        {
            CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
            if (((srcWidgetScript != null) && (this.m_announcementInfos != null)) && (this.m_announcementInfos.Count > uiEvent.m_srcWidgetIndexInBelongedList))
            {
                CAnnouncementInfo info = this.m_announcementInfos[uiEvent.m_srcWidgetIndexInBelongedList];
                GameObject widget = srcWidgetScript.GetWidget(0);
                GameObject obj3 = srcWidgetScript.GetWidget(1);
                if (info.m_type == enAnnouncementType.Text)
                {
                    if (widget != null)
                    {
                        widget.CustomSetActive(false);
                    }
                    if (obj3 != null)
                    {
                        obj3.CustomSetActive(true);
                        CUIHttpTextScript component = obj3.GetComponent<CUIHttpTextScript>();
                        if (component != null)
                        {
                            component.SetTextUrl(info.m_url, false);
                        }
                    }
                }
                else if (info.m_type == enAnnouncementType.Image)
                {
                    if (widget != null)
                    {
                        widget.CustomSetActive(true);
                        CUIHttpImageScript script3 = widget.GetComponent<CUIHttpImageScript>();
                        if (script3 != null)
                        {
                            script3.SetImageUrl(info.m_url);
                        }
                    }
                    if (obj3 != null)
                    {
                        obj3.CustomSetActive(false);
                    }
                }
            }
        }

        private void OnAnnouncementListSelectChanged(CUIEvent uiEvent)
        {
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListScript;
            if (srcWidgetScript != null)
            {
                int selectedIndex = srcWidgetScript.GetSelectedIndex();
                srcWidgetScript.MoveElementInScrollArea(selectedIndex, false);
                this.EnableAnnouncementElementPointer(srcWidgetScript.GetLastSelectedIndex(), false);
                this.EnableAnnouncementElementPointer(selectedIndex, true);
                GameObject widget = uiEvent.m_srcFormScript.GetWidget(11);
                GameObject obj3 = uiEvent.m_srcFormScript.GetWidget(12);
                if (selectedIndex == 0)
                {
                    widget.CustomSetActive(false);
                    obj3.CustomSetActive(true);
                }
                else if (selectedIndex == (this.m_announcementInfos.Count - 1))
                {
                    widget.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                }
                else
                {
                    widget.CustomSetActive(true);
                    obj3.CustomSetActive(true);
                }
            }
        }

        private void OnCancelUpdateApp(CUIEvent uiEvent)
        {
            this.CloseConfirmPanel();
            this.m_versionUpdateState = enVersionUpdateState.FinishUpdateApp;
        }

        private void OnConfirmUpdateApp(CUIEvent uiEvent)
        {
            if (this.IsUseWifi() || (s_platform == enPlatform.IOS))
            {
                this.StartDownloadApp();
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_NoWifiConfirm"), this.GetSizeString((int) this.m_appDownloadSize)), enUIEventID.VersionUpdate_ConfirmUpdateAppNoWifi, enUIEventID.None, Singleton<CTextManager>.GetInstance().GetText("Common_Confirm"), Singleton<CTextManager>.GetInstance().GetText("Common_Cancel"), false);
            }
        }

        private void OnConfirmUpdateAppNoWifi(CUIEvent uiEvent)
        {
            this.StartDownloadApp();
        }

        private void OnConfirmUpdateResource(CUIEvent uiEvent)
        {
            if (this.IsUseWifi())
            {
                this.StartDownloadResource();
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_NoWifiConfirm"), this.GetSizeString((int) this.m_resourceDownloadSize)), enUIEventID.VersionUpdate_ConfirmUpdateResourceNoWifi, enUIEventID.None, Singleton<CTextManager>.GetInstance().GetText("Common_Confirm"), Singleton<CTextManager>.GetInstance().GetText("Common_Cancel"), false);
            }
        }

        private void OnConfirmUpdateResourceNoWifi(CUIEvent uiEvent)
        {
            this.StartDownloadResource();
        }

        private void OnConfirmYYBSaveUpdateApp(CUIEvent uiEvent)
        {
            this.StartYYBSaveUpdate();
        }

        public void OnDownloadYYBProgressChanged(string msg)
        {
            int num = 1;
            int size = 1;
            char[] separator = new char[] { ',' };
            string[] strArray = msg.Split(separator);
            if (strArray.Length >= 2)
            {
                try
                {
                    num = int.Parse(strArray[0].Trim());
                    size = int.Parse(strArray[1].Trim());
                }
                catch (Exception)
                {
                    num = 1;
                    size = 1;
                }
            }
            if ((size < 0) && (num > 0))
            {
                size = num;
            }
            this.UpdateUIProgress(enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, ((float) num) / ((size != 0) ? ((float) size) : ((float) num)));
            this.UpdateUIDownloadProgressTextContent(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_DownloadYYBProgress"), this.GetDownloadTotalSize(size)));
        }

        public void OnDownloadYYBStateChanged(string msg)
        {
            int num = 0;
            int num2 = 0;
            string str = string.Empty;
            char[] separator = new char[] { ',' };
            string[] strArray = msg.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                try
                {
                    switch (i)
                    {
                        case 0:
                        {
                            num = int.Parse(strArray[i].Trim());
                            continue;
                        }
                        case 1:
                        {
                            num2 = int.Parse(strArray[i].Trim());
                            continue;
                        }
                        case 2:
                        {
                            str = strArray[i];
                            continue;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            switch (num)
            {
                case 5:
                    this.m_isError = true;
                    Singleton<BeaconHelper>.GetInstance().Event_CommonReport("Event_VerUpdateFail");
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_DownloadYYBFail"), num2.ToString()), enUIEventID.VersionUpdate_RetryCheckAppVersion, false);
                    break;

                case 4:
                    this.UpdateUIProgress(enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, 1f);
                    this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
                    break;

                case 1:
                    this.UpdateUIProgress(enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, 0f);
                    break;
            }
            this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_DownloadYYB"));
        }

        public void OnError(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, uint errorCode)
        {
            this.m_isError = true;
            Singleton<BeaconHelper>.GetInstance().Event_CommonReport("Event_VerUpdateFail");
            if (this.IsInUpdateAppStage())
            {
                enUIEventID confirmID = enUIEventID.VersionUpdate_RetryCheckAppVersion;
                IIPSMobile.IIPSMobileErrorCodeCheck.ErrorCodeInfo info = this.m_iipsErrorCodeChecker.CheckIIPSErrorCode((int) errorCode);
                if ((errorCode == 6) || (errorCode == 8))
                {
                    confirmID = enUIEventID.VersionUpdate_JumpToHomePage;
                }
                Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_AppUpdateFail"), errorCode.ToString(), this.GetErrorResult(errorCode)), confirmID, false);
                if ((s_platform == enPlatform.Android) && (this.m_versionUpdateState == enVersionUpdateState.DownloadApp))
                {
                    this.SendVersionUpdateReportEvent(this.m_reportAppDownloadStartTime, 0, CVersion.GetAppVersion(), this.m_downloadAppVersion, false, errorCode, this.m_reportAppTotalDownloadSize, this.m_reportAppDownloadSize, this.GetApkDownloadUrl());
                }
            }
            else if (this.IsInFirstExtractResourceStage())
            {
                if ((s_IIPSServerType == enIIPSServerType.None) && (errorCode == 0x9300005))
                {
                    this.m_isError = false;
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_FirstExtractFail"), errorCode.ToString(), this.GetErrorResult(errorCode)), enUIEventID.VersionUpdate_RetryCheckFirstExtractResource, false);
                }
            }
            else if (this.IsInUpdateResourceStage())
            {
                this.m_apolloUpdateSpeedCounter.StopSpeedCounter();
                this.m_downloadCounter = 0;
                this.m_downloadSpeed = 0;
                Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_ResourceUpdateFail"), errorCode.ToString(), this.GetErrorResult(errorCode)), enUIEventID.VersionUpdate_RetryCheckResourceVersion, false);
                if (this.m_versionUpdateState == enVersionUpdateState.DownloadResource)
                {
                    this.SendVersionUpdateReportEvent(this.m_reportResourceDownloadStartTime, 1, this.m_cachedResourceVersion, this.m_downloadResourceVersion, false, errorCode, this.m_reportResourceTotalDownloadSize, this.m_reportResourceDownloadSize, string.Empty);
                }
            }
        }

        public byte OnGetNewVersionInfo(IIPSMobileVersionCallBack.VERSIONINFO newVersionInfo)
        {
            if (this.IsInUpdateAppStage())
            {
                if (I2B(newVersionInfo.isAppUpdating))
                {
                    object[] args = new object[] { newVersionInfo.newAppVersion.programmeVersion.MajorVersion_Number, newVersionInfo.newAppVersion.programmeVersion.MinorVersion_Number, newVersionInfo.newAppVersion.programmeVersion.Revision_Number, newVersionInfo.newAppVersion.dataVersion.DataVersion };
                    this.m_downloadAppVersion = string.Format("{0}.{1}.{2}.{3}", args);
                    if (I2B(newVersionInfo.isForcedUpdating))
                    {
                        if (this.EnableYYBSaveUpdate())
                        {
                            this.CreateYYBUpdateManager();
                            this.StartYYBCheckVersionInfo();
                        }
                        else
                        {
                            this.OpenAppUpdateConfirmPanel(true, false);
                        }
                        this.OpenAnnouncementPanel();
                    }
                    else if ((this.m_appRecommendUpdateVersionMin > 0) && (CVersion.GetVersionNumber(s_appVersion) < this.m_appRecommendUpdateVersionMin))
                    {
                        if (this.EnableYYBSaveUpdate())
                        {
                            this.CreateYYBUpdateManager();
                            this.StartYYBCheckVersionInfo();
                        }
                        else
                        {
                            this.OpenAppUpdateConfirmPanel(true, false);
                        }
                        this.OpenAnnouncementPanel();
                    }
                    else if ((this.m_appRecommendUpdateVersionMax > 0) && (CVersion.GetVersionNumber(s_appVersion) > this.m_appRecommendUpdateVersionMax))
                    {
                        if (s_platform == enPlatform.Android)
                        {
                            this.ClearDownloadedApk();
                        }
                        this.m_versionUpdateState = enVersionUpdateState.FinishUpdateApp;
                    }
                    else
                    {
                        this.OpenAppUpdateConfirmPanel(false, false);
                        this.OpenAnnouncementPanel();
                    }
                }
                else
                {
                    if (s_platform == enPlatform.Android)
                    {
                        this.ClearDownloadedApk();
                    }
                    this.m_versionUpdateState = enVersionUpdateState.FinishUpdateApp;
                }
            }
            else if (this.IsInFirstExtractResourceStage())
            {
                this.StartFirstExtractResource();
            }
            else if (this.IsInUpdateResourceStage())
            {
                object[] objArray2 = new object[] { newVersionInfo.newAppVersion.programmeVersion.MajorVersion_Number, newVersionInfo.newAppVersion.programmeVersion.MinorVersion_Number, newVersionInfo.newAppVersion.programmeVersion.Revision_Number, newVersionInfo.newAppVersion.dataVersion.DataVersion };
                this.m_downloadResourceVersion = string.Format("{0}.{1}.{2}.{3}", objArray2);
                if (this.m_useCurseResourceDownloadSize)
                {
                    this.m_resourceDownloadSize = (uint) newVersionInfo.needDownloadSize;
                }
                if (I2B(newVersionInfo.isAppUpdating))
                {
                    if (this.m_resourceCheckCompatibility && !this.CheckResourceCompatibility(!this.m_appIsDelayToInstall ? CVersion.GetAppVersion() : this.m_downloadAppVersion, this.m_downloadResourceVersion))
                    {
                        Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("VersionIsLow"), enUIEventID.VersionUpdate_QuitApp, false);
                        return 1;
                    }
                    if (this.m_resourceDownloadNeedConfirm && !this.m_appIsDelayToInstall)
                    {
                        this.OpenResourceUpdateConfirmPanel();
                        this.OpenAnnouncementPanel();
                    }
                    else
                    {
                        this.StartDownloadResource();
                    }
                }
                else
                {
                    if ((this.m_resourceCheckCompatibility && !this.m_appIsDelayToInstall) && !this.CheckResourceCompatibility(CVersion.GetAppVersion(), this.m_cachedResourceVersion))
                    {
                        Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("VersionIsLow"), enUIEventID.VersionUpdate_QuitApp, false);
                        return 1;
                    }
                    this.m_versionUpdateState = enVersionUpdateState.FinishUpdateResource;
                }
            }
            return 1;
        }

        private void OnJumpToHomePage(CUIEvent uiEvent)
        {
            CUICommonSystem.OpenUrl("http://pvp.qq.com", false);
            this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
        }

        public byte OnNoticeInstallApk(string path)
        {
            this.UpdateUIProgress(enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, 1f);
            if (s_platform == enPlatform.Android)
            {
                this.SendVersionUpdateReportEvent(this.m_reportAppDownloadStartTime, 0, CVersion.GetAppVersion(), this.m_downloadAppVersion, true, 0, this.m_reportAppTotalDownloadSize, this.m_reportAppDownloadSize, this.GetApkDownloadUrl());
                if (this.m_appIsDelayToInstall)
                {
                    this.m_appSavePath = path;
                    this.m_versionUpdateState = enVersionUpdateState.FinishUpdateApp;
                }
                else
                {
                    this.Android_InstallAPK(path);
                    this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
                }
            }
            return 1;
        }

        public void OnProgress(IIPSMobileVersionCallBack.VERSIONSTAGE curVersionStage, ulong totalSize, ulong nowSize)
        {
            this.UpdateUIProgress(enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, (totalSize != 0) ? (((float) nowSize) / ((float) totalSize)) : 0f);
            switch (curVersionStage)
            {
                case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_ExtractData:
                case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_FirstExtract:
                case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_FullUpdate_Extract:
                case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_SourceExtract:
                    this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_ExtractResource"));
                    return;

                case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_FullUpdate:
                case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_SourceDownload:
                    this.m_reportResourceTotalDownloadSize = (uint) totalSize;
                    this.m_reportResourceDownloadSize = (uint) nowSize;
                    this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_DownloadResource"));
                    if (curVersionStage == IIPSMobileVersionCallBack.VERSIONSTAGE.VS_SourceDownload)
                    {
                        if (this.m_downloadCounter == 0)
                        {
                            this.m_apolloUpdateSpeedCounter.StartSpeedCounter();
                        }
                        this.m_apolloUpdateSpeedCounter.SetSize((uint) nowSize);
                        this.m_apolloUpdateSpeedCounter.SpeedCounter();
                        this.m_downloadSpeed = this.m_apolloUpdateSpeedCounter.GetSpeed();
                        this.m_downloadCounter++;
                    }
                    else
                    {
                        this.m_downloadSpeed = this.m_IIPSVersionMgr.MgrGetActionDownloadSpeed();
                    }
                    this.UpdateUIDownloadProgressTextContent(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_DownloadResourceProgress"), this.GetDownloadTotalSize((int) totalSize), this.GetDownloadSpeed((int) this.m_downloadSpeed), !string.IsNullOrEmpty(this.m_downloadResourceVersion) ? string.Format("(v{0})", this.m_downloadResourceVersion) : string.Empty));
                    return;

                case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_CreateApk:
                    this.m_reportAppTotalDownloadSize = (uint) totalSize;
                    this.m_reportAppDownloadSize = (uint) nowSize;
                    this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_DownloadApp"));
                    this.UpdateUIDownloadProgressTextContent(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_DownloadResourceProgress"), this.GetDownloadTotalSize((int) totalSize), this.GetDownloadSpeed((int) this.m_IIPSVersionMgr.MgrGetActionDownloadSpeed()), string.Format("(v{0})", this.m_downloadAppVersion)));
                    return;

                case IIPSMobileVersionCallBack.VERSIONSTAGE.VS_CheckApkMd5:
                    this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_PrepareInstall"));
                    this.UpdateUIDownloadProgressTextContent(string.Empty);
                    return;
            }
        }

        private void OnQuitApp(CUIEvent uiEvent)
        {
            QuitApp();
        }

        private void OnRetryCheckApp(CUIEvent uiEvent)
        {
            this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
        }

        private void OnRetryCheckFirstExtractResource(CUIEvent uiEvent)
        {
            this.m_versionUpdateState = enVersionUpdateState.StartCheckFirstExtractResource;
        }

        private void OnRetryCheckResourceVersion(CUIEvent uiEvent)
        {
            this.m_versionUpdateState = enVersionUpdateState.StartCheckResourceVersion;
        }

        public void OnSuccess()
        {
            if (!this.m_isError)
            {
                if (this.IsInUpdateAppStage())
                {
                    this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
                }
                else if (this.IsInFirstExtractResourceStage())
                {
                    this.WriteCachedResourceInfo();
                    this.m_versionUpdateState = enVersionUpdateState.FinishFirstExtractResouce;
                }
                else if (this.IsInUpdateResourceStage())
                {
                    this.m_apolloUpdateSpeedCounter.StopSpeedCounter();
                    this.m_downloadCounter = 0;
                    this.m_downloadSpeed = 0;
                    this.WriteCachedResourceInfo();
                    this.m_versionUpdateState = enVersionUpdateState.FinishUpdateResource;
                    this.SendVersionUpdateReportEvent(this.m_reportResourceDownloadStartTime, 1, this.m_cachedResourceVersion, this.m_downloadResourceVersion, true, 0, this.m_reportResourceTotalDownloadSize, this.m_reportResourceDownloadSize, string.Empty);
                }
                this.UpdateUIProgress(enVersionUpdateFormWidget.Slider_SingleProgress, enVersionUpdateFormWidget.Text_SinglePercent, 1f);
            }
        }

        private void OnSwitchAnnouncementListElementToBehind(CUIEvent uiEvent)
        {
            GameObject widget = uiEvent.m_srcFormScript.GetWidget(3);
            if (widget != null)
            {
                CUIListScript component = widget.GetComponent<CUIListScript>();
                if (component != null)
                {
                    int index = component.GetSelectedIndex() + 1;
                    if ((index >= 0) && (index < this.m_announcementInfos.Count))
                    {
                        component.SelectElement(index, true);
                    }
                }
            }
        }

        private void OnSwitchAnnouncementListElementToFront(CUIEvent uiEvent)
        {
            GameObject widget = uiEvent.m_srcFormScript.GetWidget(3);
            if (widget != null)
            {
                CUIListScript component = widget.GetComponent<CUIListScript>();
                if (component != null)
                {
                    int index = component.GetSelectedIndex() - 1;
                    if ((index >= 0) && (index < this.m_announcementInfos.Count))
                    {
                        component.SelectElement(index, true);
                    }
                }
            }
        }

        public void OnYYBCheckNeedUpdateInfo(string msg)
        {
            if (!this.m_appYYBCheckVersionInfoCallBackHandled)
            {
                this.OpenAppUpdateConfirmPanel(true, string.Equals(msg, "1"));
                this.m_appYYBCheckVersionInfoCallBackHandled = true;
            }
        }

        private void OpenAnnouncementPanel()
        {
            if (((this.m_versionUpdateFormScript != null) && (this.m_announcementInfos != null)) && ((this.m_announcementInfos.Count > 0) && !this.m_isAnnouncementPanelOpened))
            {
                GameObject widget = this.m_versionUpdateFormScript.GetWidget(7);
                if (widget != null)
                {
                    widget.CustomSetActive(true);
                }
                GameObject obj3 = this.m_versionUpdateFormScript.GetWidget(4);
                if (obj3 != null)
                {
                    CUIContainerScript component = obj3.GetComponent<CUIContainerScript>();
                    if (component != null)
                    {
                        component.RecycleAllElement();
                        for (int i = 0; i < this.m_announcementInfos.Count; i++)
                        {
                            this.m_announcementInfos[i].m_pointerSequence = component.GetElement();
                        }
                    }
                }
                GameObject obj4 = this.m_versionUpdateFormScript.GetWidget(3);
                if (obj4 != null)
                {
                    CUIListScript script2 = obj4.GetComponent<CUIListScript>();
                    if (script2 != null)
                    {
                        script2.SetElementAmount(this.m_announcementInfos.Count);
                        script2.SelectElement(0, true);
                        this.EnableAnnouncementElementPointer(0, true);
                    }
                }
                this.m_isAnnouncementPanelOpened = true;
            }
        }

        private void OpenAppUpdateConfirmPanel(bool isForcedUpdating, bool useYYBSaveUpdate)
        {
            if (this.m_versionUpdateFormScript != null)
            {
                GameObject widget = this.m_versionUpdateFormScript.GetWidget(0x11);
                if (widget != null)
                {
                    widget.CustomSetActive(false);
                }
                GameObject obj3 = this.m_versionUpdateFormScript.GetWidget(5);
                if (obj3 != null)
                {
                    obj3.CustomSetActive(true);
                }
                if (isForcedUpdating)
                {
                    this.SetUpdateNotice(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_ForceUpdateClient"));
                    this.SetConfirmPanelButton(enVersionUpdateFormWidget.Button_Cancel, true, "Quit", enUIEventID.VersionUpdate_QuitApp);
                    this.SetConfirmPanelButton(enVersionUpdateFormWidget.Button_Confirm, true, "VersionUpdate", enUIEventID.VersionUpdate_ConfirmUpdateApp);
                    this.SetConfirmPanelButton(enVersionUpdateFormWidget.Button_ConfirmYYBSaveUpdate, useYYBSaveUpdate, "VersionUpdate_YYBSaveUpdate", enUIEventID.VersionUpdate_ConfirmYYBSaveUpdateApp);
                }
                else
                {
                    this.SetUpdateNotice(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_RecommendUpdateClient"));
                    this.SetConfirmPanelButton(enVersionUpdateFormWidget.Button_Cancel, true, "Common_Cancel", enUIEventID.VersionUpdate_CancelUpdateApp);
                    this.SetConfirmPanelButton(enVersionUpdateFormWidget.Button_Confirm, true, "VersionUpdate", enUIEventID.VersionUpdate_ConfirmUpdateApp);
                    this.SetConfirmPanelButton(enVersionUpdateFormWidget.Button_ConfirmYYBSaveUpdate, false, string.Empty, enUIEventID.None);
                }
            }
        }

        private void OpenResourceUpdateConfirmPanel()
        {
            if (this.m_versionUpdateFormScript != null)
            {
                GameObject widget = this.m_versionUpdateFormScript.GetWidget(0x11);
                if (widget != null)
                {
                    widget.CustomSetActive(false);
                }
                GameObject obj3 = this.m_versionUpdateFormScript.GetWidget(5);
                if (obj3 != null)
                {
                    obj3.CustomSetActive(true);
                }
                this.SetUpdateNotice(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_ForceUpdateResource"));
                this.SetConfirmPanelButton(enVersionUpdateFormWidget.Button_Cancel, true, "Quit", enUIEventID.VersionUpdate_QuitApp);
                this.SetConfirmPanelButton(enVersionUpdateFormWidget.Button_Confirm, true, "VersionUpdate", enUIEventID.VersionUpdate_ConfirmUpdateResource);
                this.SetConfirmPanelButton(enVersionUpdateFormWidget.Button_ConfirmYYBSaveUpdate, false, string.Empty, enUIEventID.None);
            }
        }

        private void OpenWaitingForm()
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_waitingFormPath, false, false);
            if (script != null)
            {
                script.transform.Find("Panel/Panel").gameObject.CustomSetActive(false);
                script.transform.Find("Panel/Image").gameObject.CustomSetActive(true);
            }
        }

        public static void QuitApp()
        {
            SGameApplication.Quit();
            Android_ExitApp();
        }

        private void ReadCachedResourceInfo()
        {
            this.m_cachedResourceVersion = CVersion.s_emptyResourceVersion;
            this.m_cachedResourceBuildNumber = CVersion.s_emptyBuildNumber;
            this.m_cachedResourceType = 0;
            if (CFileManager.IsFileExist(s_cachedResourceVersionFilePath))
            {
                byte[] data = CFileManager.ReadFile(s_cachedResourceVersionFilePath);
                int offset = 0;
                if (((data != null) && (data.Length > 4)) && (CMemoryManager.ReadInt(data, ref offset) == data.Length))
                {
                    this.m_cachedResourceVersion = CMemoryManager.ReadString(data, ref offset);
                    this.m_cachedResourceBuildNumber = CMemoryManager.ReadString(data, ref offset);
                    this.m_cachedResourceType = CMemoryManager.ReadByte(data, ref offset);
                }
            }
        }

        private string RemoveQuotationMark(string str)
        {
            int index = str.IndexOf('"');
            int num2 = str.LastIndexOf('"');
            if (((index >= 0) && (num2 >= 0)) && (index != num2))
            {
                char[] trimChars = new char[] { '\\' };
                return str.Substring(index + 1, (num2 - index) - 1).Trim(trimChars).Trim();
            }
            return str.Trim();
        }

        public void Repair()
        {
        }

        private void SendVersionUpdateReportEvent(DateTime startTime, int versionType, string currentVersion, string updateVersion, bool isSuccessful, uint errorCode, uint totalDownloadSize, uint downloadSize, string downloadUrl)
        {
            object[] args = new object[] { startTime.ToString("yyyy-mm-dd HH:mm:ss"), versionType.ToString(), currentVersion, updateVersion, isSuccessful.ToString(), errorCode.ToString(), totalDownloadSize.ToString(), downloadSize.ToString(), downloadUrl };
            UnityEngine.Debug.Log(string.Format("Send \"Service_DownloadEvent\", startTime = {0}, versionType = {1}, currentVersion = {2}, updateVersion = {3}, isSuccessful = {4}, errorCode = {5}, totalDownloadSize = {6}, downloadSize = {7}, downloadUrl = {8}", args));
            if (isSuccessful && (totalDownloadSize != downloadSize))
            {
                downloadSize = totalDownloadSize;
            }
            try
            {
                List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("openid", string.Empty),
                    new KeyValuePair<string, string>("begintime", Utility.ToUtcSeconds(startTime).ToString()),
                    new KeyValuePair<string, string>("versionType", versionType.ToString()),
                    new KeyValuePair<string, string>("Version", updateVersion),
                    new KeyValuePair<string, string>("oldversion", currentVersion)
                };
                int num2 = !isSuccessful ? 1 : 0;
                events.Add(new KeyValuePair<string, string>("errorCode", num2.ToString()));
                events.Add(new KeyValuePair<string, string>("gameerrorcode", errorCode.ToString()));
                events.Add(new KeyValuePair<string, string>("errorinfo", string.Empty));
                TimeSpan span = (TimeSpan) (DateTime.Now - startTime);
                events.Add(new KeyValuePair<string, string>("totaltime", span.TotalMilliseconds.ToString()));
                events.Add(new KeyValuePair<string, string>("totalfilesize", totalDownloadSize.ToString()));
                events.Add(new KeyValuePair<string, string>("filesize", downloadSize.ToString()));
                events.Add(new KeyValuePair<string, string>("url", downloadUrl));
                events.Add(new KeyValuePair<string, string>("final_url", string.Empty));
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_DownloadEvent", events, true);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.Log(exception.ToString());
            }
        }

        private void SetConfirmPanelButton(enVersionUpdateFormWidget widgetIndex, bool active, string textKey, enUIEventID eventID)
        {
            if (this.m_versionUpdateFormScript != null)
            {
                GameObject widget = this.m_versionUpdateFormScript.GetWidget((int) widgetIndex);
                if (widget != null)
                {
                    widget.CustomSetActive(active);
                    if (!string.IsNullOrEmpty(textKey))
                    {
                        Transform transform = widget.transform.FindChild("Text");
                        if (transform != null)
                        {
                            Text text = transform.gameObject.GetComponent<Text>();
                            if (text != null)
                            {
                                text.text = Singleton<CTextManager>.GetInstance().GetText(textKey);
                            }
                        }
                    }
                    CUIEventScript component = widget.GetComponent<CUIEventScript>();
                    if (component != null)
                    {
                        component.SetUIEvent(enUIEventType.Click, eventID);
                    }
                }
            }
        }

        public static void SetIIPSServerType(enIIPSServerType iipsServerType)
        {
            s_IIPSServerType = iipsServerType;
        }

        public static void SetIIPSServerTypeFromFile()
        {
            TdirConfigData fileTdirAndTverData = TdirConfig.GetFileTdirAndTverData();
            if (fileTdirAndTverData != null)
            {
                s_IIPSServerType = (enIIPSServerType) fileTdirAndTverData.versionType;
            }
        }

        private void SetUpdateNotice(string noticeContent)
        {
            GameObject widget = this.m_versionUpdateFormScript.GetWidget(13);
            if (widget != null)
            {
                Text component = widget.GetComponent<Text>();
                if (component != null)
                {
                    component.text = noticeContent;
                }
            }
        }

        private void StartCheckAppVersion()
        {
            Singleton<CUIManager>.GetInstance().CloseMessageBox();
            this.CloseConfirmPanel();
            this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_CheckAppVersion"));
            this.UpdateUIDownloadProgressTextContent(string.Empty);
            this.m_downloadAppVersion = string.Empty;
            this.m_appDownloadUrl = string.Empty;
            this.m_appDownloadSize = 0;
            this.m_appRecommendUpdateVersionMin = 0;
            this.m_appRecommendUpdateVersionMax = 0;
            this.m_appIsNoIFS = false;
            this.m_appSavePath = string.Empty;
            this.m_appIsDelayToInstall = false;
            this.m_appEnableYYB = 0;
            this.m_appYYBCheckVersionInfoStartTime = 0f;
            this.m_appYYBCheckVersionInfoCallBackHandled = false;
            this.CreateIIPSVersionMgr(this.GetCheckAppVersionJsonConfig(s_platform));
            if ((this.m_IIPSVersionMgr == null) || !this.m_IIPSVersionMgr.MgrCheckAppUpdate())
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_AppUpdateFail"), 0, this.GetErrorResult(0)), enUIEventID.VersionUpdate_RetryCheckAppVersion, false);
            }
            this.m_versionUpdateState = enVersionUpdateState.CheckAppVersion;
            this.m_isError = false;
        }

        private void StartCheckFirstExtractResource()
        {
            this.m_versionUpdateState = enVersionUpdateState.CheckFirstExtractResource;
            this.m_firstExtractResourceVersion = string.Empty;
            this.m_firstExtractResourceBuildNumber = string.Empty;
            base.StartCoroutine(this.CheckFirstExtractResource());
        }

        private void StartCheckPathPermission()
        {
            if (string.IsNullOrEmpty(CFileManager.GetCachePath()))
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_PermissionFail"), enUIEventID.VersionUpdate_QuitApp, false);
                this.m_versionUpdateState = enVersionUpdateState.CheckPathPermission;
            }
            else if (s_IIPSServerType == enIIPSServerType.None)
            {
                this.ClearDownloadedApk();
                this.m_versionUpdateState = enVersionUpdateState.FinishUpdateApp;
            }
            else
            {
                this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
            }
        }

        private void StartCheckResourceVersion()
        {
            Singleton<CUIManager>.GetInstance().CloseMessageBox();
            this.CloseConfirmPanel();
            this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_CheckResourceVersion"));
            this.m_downloadResourceVersion = string.Empty;
            this.m_resourceDownloadNeedConfirm = false;
            this.m_resourceCheckCompatibility = true;
            this.m_useCurseResourceDownloadSize = false;
            this.m_resourceDownloadSize = 0;
            this.CreateIIPSVersionMgr(this.GetCheckResourceVersionJsonConfig(s_platform));
            if ((this.m_IIPSVersionMgr == null) || !this.m_IIPSVersionMgr.MgrCheckAppUpdate())
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_ResourceUpdateFail"), 0, this.GetErrorResult(0)), enUIEventID.VersionUpdate_RetryCheckResourceVersion, false);
            }
            this.m_versionUpdateState = enVersionUpdateState.CheckResourceVersion;
            this.m_isError = false;
        }

        private void StartDownloadApp()
        {
            this.CloseConfirmPanel();
            if (s_platform == enPlatform.Android)
            {
                if (this.m_IIPSVersionMgr != null)
                {
                    this.m_IIPSVersionMgr.MgrSetNextStage(true);
                }
                if (this.m_appIsNoIFS)
                {
                    this.m_appIsDelayToInstall = true;
                }
                this.m_versionUpdateState = enVersionUpdateState.DownloadApp;
                this.m_reportAppDownloadStartTime = DateTime.Now;
                this.m_reportAppTotalDownloadSize = 0;
                this.m_reportAppDownloadSize = 0;
            }
            else
            {
                CUICommonSystem.OpenUrl(this.m_appDownloadUrl, false);
                this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
            }
        }

        private void StartDownloadResource()
        {
            this.CloseConfirmPanel();
            this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_PrepareDownloadResource"));
            this.UpdateUIDownloadProgressTextContent(string.Empty);
            if (this.m_IIPSVersionMgr != null)
            {
                this.m_IIPSVersionMgr.MgrSetNextStage(true);
            }
            this.m_apolloUpdateSpeedCounter.StopSpeedCounter();
            this.m_downloadCounter = 0;
            this.m_downloadSpeed = 0;
            this.m_reportResourceDownloadStartTime = DateTime.Now;
            this.m_reportResourceTotalDownloadSize = 0;
            this.m_reportResourceDownloadSize = 0;
            this.m_versionUpdateState = enVersionUpdateState.DownloadResource;
        }

        private void StartFirstExtractResource()
        {
            if (this.m_IIPSVersionMgr != null)
            {
                this.m_IIPSVersionMgr.MgrSetNextStage(true);
            }
            this.m_versionUpdateState = enVersionUpdateState.FirstExtractResource;
        }

        public void StartVersionUpdate(OnVersionUpdateComplete onVersionUpdateComplete)
        {
            this.m_onVersionUpdateComplete = onVersionUpdateComplete;
            this.ReadCachedResourceInfo();
            Singleton<CUIManager>.GetInstance().OpenForm(CLoginSystem.s_splashFormPath, false, true);
            this.m_versionUpdateFormScript = Singleton<CUIManager>.GetInstance().OpenForm(s_versionUpdateFormPath, false, true);
            this.UpdateUIVersionTextContent(s_appVersion, this.m_cachedResourceVersion);
            this.UpdateUIStateInfoTextContent(string.Empty);
            this.UpdateUIDownloadProgressTextContent(string.Empty);
            this.m_announcementInfos.Clear();
            this.CloseAnnouncementPanel();
            this.CloseConfirmPanel();
            this.m_versionUpdateState = enVersionUpdateState.StartCheckPathPermission;
        }

        public void StartYYBCheckVersionInfo()
        {
            if (this.m_appYYBUpdateManager != null)
            {
                this.m_appYYBUpdateManager.StartYYBCheckVersionInfo();
            }
            this.m_appYYBCheckVersionInfoStartTime = Time.realtimeSinceStartup;
            this.m_appYYBCheckVersionInfoCallBackHandled = false;
        }

        public void StartYYBSaveUpdate()
        {
            this.CloseConfirmPanel();
            if (this.m_appYYBUpdateManager != null)
            {
                int num = this.m_appYYBUpdateManager.CheckYYBInstalled();
                this.m_appYYBUpdateManager.StartYYBSaveUpdate();
                if (num == 0)
                {
                    this.m_versionUpdateState = enVersionUpdateState.StartCheckAppVersion;
                }
                else
                {
                    this.m_versionUpdateState = enVersionUpdateState.DownloadYYB;
                    this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_DownloadYYB"));
                }
            }
        }

        public void Update()
        {
            switch (this.m_versionUpdateState)
            {
                case enVersionUpdateState.StartCheckPathPermission:
                    this.StartCheckPathPermission();
                    break;

                case enVersionUpdateState.StartCheckAppVersion:
                    this.StartCheckAppVersion();
                    break;

                case enVersionUpdateState.CheckAppVersion:
                    this.CheckAppVersion();
                    break;

                case enVersionUpdateState.FinishUpdateApp:
                    this.FinishUpdateApp();
                    break;

                case enVersionUpdateState.StartCheckFirstExtractResource:
                    this.StartCheckFirstExtractResource();
                    break;

                case enVersionUpdateState.FinishFirstExtractResouce:
                    this.FinishFirstExtractResource();
                    break;

                case enVersionUpdateState.StartCheckResourceVersion:
                    this.StartCheckResourceVersion();
                    break;

                case enVersionUpdateState.FinishUpdateResource:
                    this.FinishUpdateResource();
                    break;

                case enVersionUpdateState.Complete:
                    this.Complete();
                    break;
            }
            this.UpdateIIPSVersionMgr();
            this.UpdateUIProgress(enVersionUpdateFormWidget.Slider_TotalProgress, enVersionUpdateFormWidget.Text_TotalPercent, ((float) this.m_versionUpdateState) / 16f);
        }

        private void UpdateIIPSVersionMgr()
        {
            if (this.m_IIPSVersionMgr != null)
            {
                this.m_IIPSVersionMgr.MgrPoll();
            }
        }

        private void UpdateUIDownloadProgressTextContent(string content)
        {
            Text uIComponent = this.GetUIComponent<Text>(this.m_versionUpdateFormScript, enVersionUpdateFormWidget.Text_UpdateInfo);
            if (uIComponent != null)
            {
                uIComponent.text = content;
            }
        }

        private void UpdateUIProgress(enVersionUpdateFormWidget progressBarWidget, enVersionUpdateFormWidget progressPercentTextWidget, float progress)
        {
            if (progress > 1f)
            {
                progress = 1f;
            }
            Slider uIComponent = this.GetUIComponent<Slider>(this.m_versionUpdateFormScript, progressBarWidget);
            if (uIComponent != null)
            {
                uIComponent.value = progress;
            }
            Text text = this.GetUIComponent<Text>(this.m_versionUpdateFormScript, progressPercentTextWidget);
            if (text != null)
            {
                text.text = string.Format("{0}%", (int) (progress * 100f));
            }
        }

        private void UpdateUIStateInfoTextContent(string content)
        {
            Text uIComponent = this.GetUIComponent<Text>(this.m_versionUpdateFormScript, enVersionUpdateFormWidget.Text_CurrentState);
            if (uIComponent != null)
            {
                uIComponent.text = content;
            }
        }

        private void UpdateUITextContent(enVersionUpdateFormWidget textWidget, string content)
        {
            Text uIComponent = this.GetUIComponent<Text>(this.m_versionUpdateFormScript, textWidget);
            if (uIComponent != null)
            {
                uIComponent.text = content;
            }
        }

        private void UpdateUIVersionTextContent(string appVersion, string resourceVersion)
        {
            this.UpdateUITextContent(enVersionUpdateFormWidget.Text_Version, string.Format("App v{0}   Res v{1}", appVersion, resourceVersion));
        }

        [DebuggerHidden]
        private IEnumerator VersionUpdateComplete()
        {
            return new <VersionUpdateComplete>c__Iterator26 { <>f__this = this };
        }

        private void WriteCachedResourceInfo()
        {
            if (CFileManager.IsFileExist(s_cachedResourceVersionFilePath))
            {
                CFileManager.DeleteFile(s_cachedResourceVersionFilePath);
            }
            if (CFileManager.IsFileExist(s_resourcePackerInfoSetFullPath))
            {
                byte[] data = CFileManager.ReadFile(s_resourcePackerInfoSetFullPath);
                int offset = 0;
                CResourcePackerInfoSet.ReadVersionAndBuildNumber(data, ref offset, ref this.m_cachedResourceVersion, ref this.m_cachedResourceBuildNumber);
                this.m_cachedResourceType = (int) s_appType;
                data = new byte[0x400];
                offset = 0;
                int num2 = offset;
                offset += 4;
                CMemoryManager.WriteString(this.m_cachedResourceVersion, data, ref offset);
                CMemoryManager.WriteString(this.m_cachedResourceBuildNumber, data, ref offset);
                CMemoryManager.WriteByte((byte) this.m_cachedResourceType, data, ref offset);
                CMemoryManager.WriteInt(offset, data, ref num2);
                CFileManager.WriteFile(s_cachedResourceVersionFilePath, data, 0, offset);
                this.UpdateUIVersionTextContent(s_appVersion, this.m_cachedResourceVersion);
            }
        }

        [CompilerGenerated]
        private sealed class <CheckFirstExtractResource>c__Iterator25 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal CVersionUpdateSystem <>f__this;
            internal int <offset>__1;
            internal WWW <www>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this.UpdateUIStateInfoTextContent(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_CheckFirstExtractResource"));
                        if (this.<>f__this.IsFileExistInStreamingAssets(CVersionUpdateSystem.s_firstExtractIFSName) && this.<>f__this.IsFileExistInStreamingAssets(CResourcePackerInfoSet.s_resourcePackerInfoSetFileName))
                        {
                            this.<www>__0 = new WWW(CFileManager.GetStreamingAssetsPathWithHeader(CResourcePackerInfoSet.s_resourcePackerInfoSetFileName));
                            this.$current = this.<www>__0;
                            this.$PC = 1;
                            return true;
                        }
                        this.<>f__this.m_versionUpdateState = enVersionUpdateState.FinishFirstExtractResouce;
                        break;

                    case 1:
                        this.<offset>__1 = 0;
                        CResourcePackerInfoSet.ReadVersionAndBuildNumber(this.<www>__0.bytes, ref this.<offset>__1, ref this.<>f__this.m_firstExtractResourceVersion, ref this.<>f__this.m_firstExtractResourceBuildNumber);
                        if ((this.<>f__this.m_cachedResourceType != CVersionUpdateSystem.s_appType) || (((CVersionUpdateSystem.s_IIPSServerType == enIIPSServerType.None) || (CVersion.GetVersionNumber(this.<>f__this.m_cachedResourceVersion) <= CVersion.GetVersionNumber(this.<>f__this.m_firstExtractResourceVersion))) && (!string.Equals(this.<>f__this.m_cachedResourceVersion, this.<>f__this.m_firstExtractResourceVersion) || !string.Equals(this.<>f__this.m_cachedResourceBuildNumber, this.<>f__this.m_firstExtractResourceBuildNumber))))
                        {
                            this.<>f__this.ClearCachePath();
                            this.<>f__this.CreateIIPSVersionMgr(this.<>f__this.GetFirstExtractResourceJsonConfig(CVersionUpdateSystem.s_platform));
                            if ((this.<>f__this.m_IIPSVersionMgr == null) || !this.<>f__this.m_IIPSVersionMgr.MgrCheckAppUpdate())
                            {
                                Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.GetInstance().GetText("VersionUpdate_FirstExtractFail"), 0, this.<>f__this.GetErrorResult(0)), enUIEventID.VersionUpdate_RetryCheckFirstExtractResource, false);
                            }
                            this.<>f__this.m_isError = false;
                            this.$PC = -1;
                            break;
                        }
                        this.<>f__this.m_versionUpdateState = enVersionUpdateState.FinishFirstExtractResouce;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <VersionUpdateComplete>c__Iterator26 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal CVersionUpdateSystem <>f__this;
            internal string <content>__2;
            internal CBinaryObject <tAsstet>__1;
            internal CResource <textResource>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this.OpenWaitingForm();
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_01A8;

                    case 1:
                        Singleton<CResourceManager>.GetInstance().LoadResourcePackerInfoSet();
                        PlayerPrefs.SetInt("SplashHack", 1);
                        this.<textResource>__0 = Singleton<CResourceManager>.GetInstance().GetResource("Config/Splash.txt", typeof(TextAsset), enResourceType.Numeric, false, true);
                        if (this.<textResource>__0 != null)
                        {
                            this.<tAsstet>__1 = this.<textResource>__0.m_content as CBinaryObject;
                            if ((null != this.<tAsstet>__1) && (this.<tAsstet>__1.m_data != null))
                            {
                                this.<content>__2 = StringHelper.ASCIIBytesToString(this.<tAsstet>__1.m_data);
                                if ((this.<content>__2 != null) && (this.<content>__2.Trim() == "0"))
                                {
                                    PlayerPrefs.SetInt("SplashHack", 0);
                                }
                            }
                        }
                        PlayerPrefs.Save();
                        this.$current = this.<>f__this.StartCoroutine(Singleton<CResourceManager>.GetInstance().LoadResidentAssetBundles());
                        this.$PC = 2;
                        goto Label_01A8;

                    case 2:
                        this.$current = this.<>f__this.StartCoroutine(MonoSingleton<GameFramework>.GetInstance().PrepareGameSystem());
                        this.$PC = 3;
                        goto Label_01A8;

                    case 3:
                        this.<>f__this.CloseWaitingForm();
                        Singleton<CUIManager>.GetInstance().CloseForm(CVersionUpdateSystem.s_versionUpdateFormPath);
                        this.<>f__this.m_versionUpdateFormScript = null;
                        if (this.<>f__this.m_onVersionUpdateComplete != null)
                        {
                            this.<>f__this.m_onVersionUpdateComplete();
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_01A8:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public delegate void OnVersionUpdateComplete();
    }
}

