namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class ApolloReportService : ApolloObject, IApolloReportService, IApolloServiceBase
    {
        public static readonly ApolloReportService Instance = new ApolloReportService();
        private ApolloBuglyLogDelegate m_LogCallback;

        private ApolloReportService()
        {
            Console.WriteLine("ApolloReportService Create With ID:{0}", base.ObjectId);
        }

        private void apollo_bugly_log_callback(string log, string stackTrace, LogType type)
        {
            if (this.m_LogCallback != null)
            {
                this.m_LogCallback(log, stackTrace, type);
            }
        }

        public void ApolloEnableCrashReport(bool rdm, bool mta)
        {
            ApolloEnableCrashReport(base.ObjectId, rdm, mta);
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool ApolloEnableCrashReport(ulong objId, bool rdm, bool mta);
        public bool ApolloRepoertEvent(string eventName, List<KeyValuePair<string, string>> events, bool isReal)
        {
            if (eventName == null)
            {
                return false;
            }
            string payload = string.Empty;
            if (events != null)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    KeyValuePair<string, string> pair = events[i];
                    string str2 = payload;
                    string[] textArray1 = new string[] { str2, pair.Key.ToString(), ":", pair.Value.ToString(), "," };
                    payload = string.Concat(textArray1);
                }
            }
            Debug.Log("payload is :" + payload);
            ApolloReportEvent(base.ObjectId, eventName, payload, isReal);
            return true;
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool ApolloReportEvent(ulong objId, [MarshalAs(UnmanagedType.LPStr)] string eventName, [MarshalAs(UnmanagedType.LPStr)] string payload, bool isReal);
        public void ApolloReportInit(bool rdm = true, bool mta = true)
        {
            ApolloReportInit(base.ObjectId, rdm, mta);
        }

        [DllImport("MsdkAdapter", CallingConvention=CallingConvention.Cdecl)]
        private static extern bool ApolloReportInit(ulong objId, bool rdm, bool mta);
        [Obsolete("Obsolete since 1.1.16, Is Obsolete", true)]
        public void EnableBuglyLog(bool enable)
        {
        }

        public void EnableExceptionHandler(LogSeverity level)
        {
            BuglyAgent.EnableExceptionHandler();
            BuglyAgent.ConfigAutoReportLogLevel(level);
        }

        public void HandleException(Exception e)
        {
            ADebug.Log("ApolloReportService HandleException:" + e.ToString());
            this.ReportException(e);
        }

        public void RegisterLogCallback(ApolloBuglyLogDelegate callback)
        {
            ADebug.Log("Apollo RegisterExceptionHandler");
            if (callback != null)
            {
                this.m_LogCallback = callback;
                BuglyAgent.RegisterLogCallback(new BuglyAgent.LogCallbackDelegate(this.apollo_bugly_log_callback));
            }
            else
            {
                BuglyAgent.RegisterLogCallback(null);
            }
        }

        public static void RegistExceptionHandler(string text, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                AndroidJavaClass class2 = new AndroidJavaClass("com.tsf4g.apollo.report.UnityException");
                object[] args = new object[] { text + stackTrace };
                class2.CallStatic("CatchException", args);
            }
        }

        public void ReportException(Exception e)
        {
            ADebug.Log("ApolloReportService ReportException:" + e.ToString());
            BuglyAgent.ReportException(e, string.Empty);
        }

        public void ReportException(Exception e, string message)
        {
            ADebug.Log("ApolloReportService ReportException:" + e.ToString() + " message:" + message);
            BuglyAgent.ReportException(e, message);
        }

        public void ReportException(string name, string message, string stackTrace)
        {
            BuglyAgent.ReportException(name, message, stackTrace);
        }

        public void SetAutoQuitApplication(bool autoExit)
        {
            BuglyAgent.ConfigAutoQuitApplication(autoExit);
        }

        [Obsolete("Obsolete since 1.1.16, Is Obsolete", true)]
        public void SetGameObjectForCallback(string gameObject)
        {
        }

        public void SetUserId(string userId)
        {
            BuglyAgent.SetUserId(userId);
        }
    }
}

