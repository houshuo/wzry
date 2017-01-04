namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class CYYBUpdateObserver : MonoBehaviour
    {
        [CompilerGenerated]
        private static CYYBUpdateManager.OnYYBCheckNeedUpdateInfo <>f__am$cache4;
        [CompilerGenerated]
        private static CYYBUpdateManager.OnDownloadYYBProgressChanged <>f__am$cache5;
        [CompilerGenerated]
        private static CYYBUpdateManager.OnDownloadYYBStateChanged <>f__am$cache6;
        public CYYBUpdateManager.OnDownloadYYBProgressChanged m_onDownloadYYBProgressChangedHandler;
        public CYYBUpdateManager.OnDownloadYYBStateChanged m_onDownloadYYBStateChangedHandler;
        public CYYBUpdateManager.OnYYBCheckNeedUpdateInfo m_onYYBCheckNeedUpdateInfoHandler;
        public static string s_gameObjectName = "CYYBUpdateObserver";

        public CYYBUpdateObserver()
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new CYYBUpdateManager.OnYYBCheckNeedUpdateInfo(CYYBUpdateObserver.<m_onYYBCheckNeedUpdateInfoHandler>m__90);
            }
            this.m_onYYBCheckNeedUpdateInfoHandler = <>f__am$cache4;
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new CYYBUpdateManager.OnDownloadYYBProgressChanged(CYYBUpdateObserver.<m_onDownloadYYBProgressChangedHandler>m__91);
            }
            this.m_onDownloadYYBProgressChangedHandler = <>f__am$cache5;
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new CYYBUpdateManager.OnDownloadYYBStateChanged(CYYBUpdateObserver.<m_onDownloadYYBStateChangedHandler>m__92);
            }
            this.m_onDownloadYYBStateChangedHandler = <>f__am$cache6;
        }

        [CompilerGenerated]
        private static void <m_onDownloadYYBProgressChangedHandler>m__91(string)
        {
        }

        [CompilerGenerated]
        private static void <m_onDownloadYYBStateChangedHandler>m__92(string)
        {
        }

        [CompilerGenerated]
        private static void <m_onYYBCheckNeedUpdateInfoHandler>m__90(string)
        {
        }

        public void HandleMsgOnDownloadYYBProgressChanged(string msg)
        {
            this.m_onDownloadYYBProgressChangedHandler(msg);
        }

        public void HandleMsgOnDownloadYYBStateChanged(string msg)
        {
            this.m_onDownloadYYBStateChangedHandler(msg);
        }

        public void HandleMsgOnYYBCheckNeedUpdateInfo(string msg)
        {
            this.m_onYYBCheckNeedUpdateInfoHandler(msg);
        }
    }
}

