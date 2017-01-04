namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface IApolloReportService : IApolloServiceBase
    {
        void ApolloEnableCrashReport(bool rdm, bool mta);
        bool ApolloRepoertEvent(string eventName, List<KeyValuePair<string, string>> events, bool isReal);
        void ApolloReportInit(bool rdm = true, bool mta = true);
        [Obsolete("Obsolete since 1.1.16, Is Obsolete", true)]
        void EnableBuglyLog(bool enable);
        void EnableExceptionHandler(LogSeverity level = 6);
        [Obsolete("Obsolete since 1.1.16, use ReportException instead")]
        void HandleException(Exception e);
        void RegisterLogCallback(ApolloBuglyLogDelegate handler);
        void ReportException(Exception e);
        void ReportException(Exception e, string message);
        void ReportException(string name, string message, string stackTrace);
        void SetAutoQuitApplication(bool autoExit);
        [Obsolete("Obsolete since 1.1.16, Is Obsolete", true)]
        void SetGameObjectForCallback(string gameObject);
        void SetUserId(string userId);
    }
}

