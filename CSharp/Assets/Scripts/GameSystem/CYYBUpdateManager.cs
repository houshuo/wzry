namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class CYYBUpdateManager
    {
        private AndroidJavaClass m_androidUtilityJavaClass;

        public CYYBUpdateManager(OnYYBCheckNeedUpdateInfo onYYBCheckNeedUpdateInfoHandler, OnDownloadYYBProgressChanged onDownloadYYBProgressChangedHandler, OnDownloadYYBStateChanged onDownloadYYBStateChangedHandler)
        {
            CYYBUpdateObserver observer = new GameObject(CYYBUpdateObserver.s_gameObjectName).AddComponent<CYYBUpdateObserver>();
            observer.m_onYYBCheckNeedUpdateInfoHandler = (OnYYBCheckNeedUpdateInfo) Delegate.Combine(observer.m_onYYBCheckNeedUpdateInfoHandler, onYYBCheckNeedUpdateInfoHandler);
            observer.m_onDownloadYYBProgressChangedHandler = (OnDownloadYYBProgressChanged) Delegate.Combine(observer.m_onDownloadYYBProgressChangedHandler, onDownloadYYBProgressChangedHandler);
            observer.m_onDownloadYYBStateChangedHandler = (OnDownloadYYBStateChanged) Delegate.Combine(observer.m_onDownloadYYBStateChangedHandler, onDownloadYYBStateChangedHandler);
            this.m_androidUtilityJavaClass = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
            this.m_androidUtilityJavaClass.CallStatic("AddYYBSaveUpdateListener", new object[0]);
        }

        public int CheckYYBInstalled()
        {
            return this.m_androidUtilityJavaClass.CallStatic<int>("CheckYYBInstalled", new object[0]);
        }

        public void StartYYBCheckVersionInfo()
        {
            this.m_androidUtilityJavaClass.CallStatic("StartYYBCheckVersionInfo", new object[0]);
        }

        public void StartYYBSaveUpdate()
        {
            this.m_androidUtilityJavaClass.CallStatic("StartYYBSaveUpdate", new object[0]);
        }

        public delegate void OnDownloadYYBProgressChanged(string msg);

        public delegate void OnDownloadYYBStateChanged(string msg);

        public delegate void OnYYBCheckNeedUpdateInfo(string msg);
    }
}

