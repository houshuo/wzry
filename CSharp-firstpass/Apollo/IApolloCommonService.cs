namespace Apollo
{
    using System;

    public interface IApolloCommonService : IApolloServiceBase
    {
        event OnCrashExtMessageNotifyHandle onCrashExtMessageEvent;

        event OnFeedbackNotifyHandle onFeedbackEvent;

        event OnReceivedPushNotifyHandle onReceivedPushEvent;

        void Feedback(string body);
        string GetChannelId();
        string GetRegisterChannelId();
        bool OpenAmsCenter(string param);
        void OpenUrl(string openUrl);
        void OpenWeiXinDeeplink(string link);
        void PushInit();
    }
}

